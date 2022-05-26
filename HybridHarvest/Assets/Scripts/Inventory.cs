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
    public Action<List<string>> onItemAdded;
    public int Money { get; private set; }
    public int Reputation { get; private set; }

    public int ReputationLimit =>
        (int) Math.Round((0.04 * Math.Pow(Level, 3) +
                          0.8 * Math.Pow(Level, 2) +
                          2 * Level) * 15);

    public int Level { get; set; }
    public int Energy { get; private set; }
    public int MaxItemsAmount { get; private set; }

    private int EnergyMax { get; set; }

    //The time in *seconds* it take to regenerate 1 energy
    private int EnergyRegenDelay { get; set; }

    private float energyBuffer;
    private Market market;

    private readonly Dictionary<string, int> _defaultAmount = new Dictionary<string, int>()
    {
        { "Money", 100 },
        { "Reputation", 0 },
        { "Level", 1 }, // Hачинается с 1 т.к. формула неадекватно реагирует на 0
        { "MaxItemsAmount", 2 },
        { "Energy", 1 },
        { "EnergyMax", 5 },
    };

    /// <summary>
    /// Добавляет семечко в инвентарь.
    /// </summary>
    /// <param name="newSeed">Семечко.</param>
    /// <param name="withoutFullnessCheck">Указать True, если требуется добавить предмет без проверки вместимости инвентаря.</param>
    public void AddItem(Seed newSeed, bool withoutFullnessCheck = false)
    {
        //newSeed.UpdateRating();
        InventoryDrawer.GetComponent<InventoryDrawer>().UpdateActions();
        InventoryDrawer.GetComponent<InventoryDrawer>().SetPurpose(PurposeOfDrawing.Change);
        if (!withoutFullnessCheck)
            onInventoryFull?.Invoke(newSeed);
        else
        {
            Elements.Add(newSeed);
            onItemAdded?.Invoke(null);
        }

        Save();
    }

    /// <summary>
    /// Изменяет деньги.
    /// </summary>
    /// <param name="amount">Количество.</param>
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

    /// <summary>
    /// Изменяет рупутацию.
    /// </summary>
    /// <param name="amount">Количество.</param>
    public void ChangeReputation(int amount)
    {
        Reputation += amount;
        if (Reputation >= ReputationLimit)
        {
            Reputation -= ReputationLimit;
            Level++;

            // Бонусы за повышение вот здесь
            Money += 100;
            ApplyLevelBonuses();
            // Конец бонусов за повышение уровня

            var scenario = GameObject.FindGameObjectWithTag("TutorialHandler")?.GetComponent<Scenario>();
            if (scenario == null) return;

            switch (Level)
            {
                // тутор для достижения уровня 2
                case 2:
                    scenario.LevelUp2();
                    break;
            }

            Save();
        }
    }

    /// <summary>
    /// Задаёт уровень.
    /// </summary>
    /// <param name="value">Значение уровня.</param>
    public void SetLevel(int value)
    {
        if (value < 1) return;

        if (value == 1)
        {
            Money = _defaultAmount["Money"];
            Energy = _defaultAmount["Energy"];
            EnergyMax = _defaultAmount["EnergyMax"];
            MaxItemsAmount = _defaultAmount["MaxItemsAmount"];
            ShopLogic.ResetSeeds();
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

    /// <summary>
    /// Поглощает энергию.
    /// </summary>
    /// <param name="amount">Количество.</param>
    public void ConsumeEnergy(int amount)
    {
        Energy -= amount;
    }

    /// <summary>
    /// Накапливает энергию.
    /// </summary>
    /// <param name="amount">Количество.</param>
    public void RegenEnergy(int amount)
    {
        if (Energy + amount > EnergyMax)
            Energy = EnergyMax;
        else
            Energy += amount;
    }

    /// <summary>
    /// Удаляет предмет из инвентаря.
    /// </summary>
    /// <param name="index">Индекс предмета.</param>
    public void RemoveItem(int index)
    {
        Elements.RemoveAt(index);
    }

    /// <summary>
    /// Сохраняет данные.
    /// </summary>
    public void Save()
    {
        var writer = QuickSaveWriter.Create("PlayerInventoryData");
        writer.Write("Money", Money)
            .Write("Reputation", Reputation)
            .Write("Level", Level)
            .Write("MaxItemsAmount", MaxItemsAmount);
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

    /// <summary>
    /// Загружает данные из памяти.
    /// </summary>
    public void Load()
    {
        var reader = QSReader.Create("PlayerInventoryData");
        Money = LoadData(reader, "Money");
        Reputation = LoadData(reader, "Reputation");
        Level = LoadData(reader, "Level");
        MaxItemsAmount = LoadData(reader, "MaxItemsAmount");

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
        Energy = LoadData(reader, "Energy");
        EnergyMax = LoadData(reader, "EnergyMax");
        energyBuffer = reader.Exists("EnergyBuffer")
            ? reader.Read<float>("EnergyBuffer")
            : EnergyRegenDelay;

        var lastDate = reader.Exists("LastDate") ? reader.Read<DateTime>("LastDate") : DateTime.Now;
        var secondsElapsed = (float) (DateTime.Now - lastDate).TotalSeconds;
        var regenerated = (int) secondsElapsed / EnergyRegenDelay;

        RegenEnergy(regenerated);
        energyBuffer -= secondsElapsed % EnergyRegenDelay;
    }

    /// <summary>
    /// Загружает заданную характеристику из памяти.
    /// </summary>
    /// <param name="reader">Отсюда ведётся чтение данных.</param>
    /// <param name="key">Название характеристики.</param>
    /// <returns></returns>
    private int LoadData(QuickSaveReader reader, string key) =>
        reader.Exists(key) ? reader.Read<int>(key) : _defaultAmount[key];

    /// <summary>
    /// Выдаёт бонусы за повышение уровня.
    /// </summary>
    private void ApplyLevelBonuses()
    {
        switch (Level)
        {
            case 2:
                MaxItemsAmount++;
                ShopLogic.UnlockSeeds("Tomato");
                break;
            case 3:
                EnergyMax++;
                break;
            case 4:
                MaxItemsAmount++;
                ShopLogic.UnlockSeeds("Pea");
                break;
            case 5:
                EnergyMax++;
                break;
            case 6:
                MaxItemsAmount++;
                ShopLogic.UnlockSeeds("Potato");
                break;
            case 7:
                EnergyMax++;
                break;
            case 8:
                MaxItemsAmount++;
                ShopLogic.UnlockSeeds("Carrot");
                break;
            case 9:
                EnergyMax++;
                break;
            case 10:
                EnergyMax++;
                break;
            case 11:
                break;
            case 12:
                break;
            case 13:
                EnergyMax++;
                break;
            case 14:
                MaxItemsAmount++;
                break;
            case 15:
                EnergyMax++;
                break;
            case 20:
                ShopLogic.UnlockSeeds("Debug");
                break;
        }
    }

    /// <summary>
    /// Перерисовывает информаионную панель.
    /// </summary>
    private void RedrawInfo()
    {
        if (MoneyInfo != null) MoneyInfo.text = Money.ToString();
        if (ReputationInfo != null) ReputationInfo.text = $"Уровень {Level}";
        if (EnergyInfo != null) EnergyInfo.text = $"{Energy}/{EnergyMax}";
        DrawEnergyTime();
    }

    /// <summary>
    /// Отрисовывает время до регенерации энергии.
    /// </summary>
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
}
