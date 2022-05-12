using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;
using CI.QuickSave;

public class Inventory : MonoBehaviour, ISaveable
{
    public GameObject InventoryDrawer;
    public List<Seed> Elements = new List<Seed>();
    public Text MoneyInfo;
    public Text ReputationInfo;
    public Text EnergyInfo;
    public Text EnergyRegenTime;

    public Action<Seed> onInventoryFull;
    public Action<string> onItemAdded;
    public int Money { get; private set; }
    public int Reputation { get; private set; }

    public int ReputationLimit =>
        (int) Math.Round((0.04 * Math.Pow(Level, 3) +
                          0.8 * Math.Pow(Level, 2) +
                          2 * Level) * 15);

    public int Level { get; set; }
    public int Energy { get; private set; }
    public int EnergyMax { get; private set; }

    public int MaxItemsAmount { get; private set; }

    //The time in *seconds* it take to regenerate 1 energy
    public int EnergyRegenDelay { get; private set; }
    private float energyBuffer;

    private Market market;

    public void Awake()
    {
        // Preventing null references etc
        EnergyRegenTime ??= GameObject.Find("RegenTime").GetComponent<Text>();
        ReputationInfo ??= GameObject.Find("ReputationInfo").GetComponent<Text>();
        Load();
        RedrawInfo();
    }

    private void Update()
    {
        RedrawInfo();
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
            Save();
    }

    private void RedrawInfo()
    {
        if (MoneyInfo != null) MoneyInfo.text = Money.ToString();
        if (ReputationInfo != null) ReputationInfo.text = $"Уровень {Level}";
        if (EnergyInfo != null) EnergyInfo.text = $"{Energy}/{EnergyMax}";
        DrawEnergyTime();
    }

    private void DrawEnergyTime()
    {
        if (Energy == EnergyMax)
        {
            EnergyRegenTime.text = "--:--";
            return;
        }

        energyBuffer -= Time.deltaTime;
        if (Math.Ceiling(energyBuffer) <= 0)
        {
            energyBuffer = EnergyRegenDelay;
            RegenEnergy(1);
        }

        var minutes = Math.Floor(energyBuffer / 60).ToString(CultureInfo.InvariantCulture);
        var seconds = Math.Floor(energyBuffer % 60).ToString(CultureInfo.InvariantCulture);
        EnergyRegenTime.text = $"{minutes.PadLeft(2, '0')}:{seconds.PadLeft(2, '0')}";
    }

    public void AddItem(Seed newSeed, bool withoutInvoke = false)
    {
        //newSeed.UpdateRating();
        InventoryDrawer.GetComponent<InventoryDrawer>().UpdateActions();
        InventoryDrawer.GetComponent<InventoryDrawer>().SetPurpose(PurposeOfDrawing.Change);
        if (!withoutInvoke)
            onInventoryFull?.Invoke(newSeed);
        else
        {
            Elements.Add(newSeed);
            onItemAdded?.Invoke(null);
        }
    }

    public void AddMoney(int amount)
    {
        //Money += changingAmount > 0
        //    ? changingAmount/* / Devider*/
        //    : changingAmount;
        //if (Money <= -100 && changingAmount < 0
        //    || changingAmount > 0)
        //    Reputation += changingAmount / Devider;
        Money += amount;
        Save();
    }

    public void ChangeReputation(int amount)
    {
        Reputation += amount;
        if (Reputation >= ReputationLimit)
        {
            Reputation -= ReputationLimit;
            Level++;

            // Бонусы за повышение вот здесь
            EnergyMax++;
            Money += 100;

            switch (Level)
            {
                case 2:
                    ShopLogic.UnlockSeeds("Tomato");
                    break;
            }
            // Конец бонусов за повышение уровня

            var scenario = GameObject.FindGameObjectWithTag("TutorialHandler")?.GetComponent<Scenario>();
            if (scenario == null) return;

            switch (Level)
            {
                // тутор для достижения уровня 2
                case 2 when QSReader.Create("TutorialState").Exists("Tutorial_FieldEnding_Played"):
                    scenario.Tutorial_LevelUp2();
                    break;
            }
        }
    }

    public void SetLevel(int value)
    {
        if (value < 1) return;

        if (value == 1)
        {
            Money = 100;
            Energy = 1;
            EnergyMax = 10;
        }

        while (value > Level)
            ChangeReputation(ReputationLimit);

        if (value < Level)
        {
            Level = value;
            Reputation = 0;
        }

        Save();
    }

    public void ConsumeEnergy(int amount)
    {
        Energy -= amount;
    }

    public void RegenEnergy(int amount)
    {
        if (Energy + amount > EnergyMax)
            Energy = EnergyMax;
        else
            Energy += amount;
    }

    public void RemoveItem(int index)
    {
        Elements.RemoveAt(index);
    }

    public void Save()
    {
        var writer = QuickSaveWriter.Create("PlayerInventoryData");
        writer.Write("money", Money)
            .Write("reputation", Reputation)
            .Write("reputationLevel", Level);
        writer.Commit();

        writer = QuickSaveWriter.Create("PlayerInventoryItems");
        writer.Write("amount", Elements.Count);
        for (var i = 0; i < Elements.Count; i++)
            writer.Write(i.ToString(), Elements[i].ToString());
        writer.Commit();

        writer = QuickSaveWriter.Create("EnergyData");
        writer.Write("Energy", Energy)
            .Write("EnergyMax", EnergyMax)
            .Write("EnergyBuffer", energyBuffer)
            .Write("LastDate", DateTime.Now);
        writer.Commit();
    }

    public void Load()
    {
        var reader = QSReader.Create("PlayerInventoryData");
        Money = reader.Exists("money") ? reader.Read<int>("money") : 100;
        Reputation = reader.Exists("reputation") ? reader.Read<int>("reputation") : 0;
        // Hачинается с 1 т.к. формула неадекватно реагирует на 0
        Level = reader.Exists("reputationLevel") ? reader.Read<int>("reputationLevel") : 1;

        MaxItemsAmount = 10;
        Elements = new List<Seed>();
        reader = QSReader.Create("PlayerInventoryItems");
        if (reader.Exists("amount"))
        {
            var i = reader.Read<int>("amount");
            for (var j = 0; j < i; j++)
            {
                var parameters = reader.Read<string>(j.ToString());
                var newSeed = Seed.Create(parameters);
                Elements.Add(newSeed);
            }
        }

        EnergyRegenDelay = 20; // time it take to regen (in seconds)
        reader = QSReader.Create("EnergyData");
        Energy = reader.Exists("Energy") ? reader.Read<int>("Energy") : 1;
        EnergyMax = reader.Exists("EnergyMax") ? reader.Read<int>("EnergyMax") : 10;
        energyBuffer = reader.Exists("EnergyBuffer")
            ? reader.Read<float>("EnergyBuffer")
            : EnergyRegenDelay;
        var lastDate = reader.Exists("LastDate") ? reader.Read<DateTime>("LastDate") : DateTime.Now;
        var secondsElapsed = (float) (DateTime.Now - lastDate).TotalSeconds;
        var regenerated = (int) secondsElapsed / EnergyRegenDelay;
        RegenEnergy(regenerated);
        energyBuffer -= secondsElapsed % EnergyRegenDelay;
    }
}
