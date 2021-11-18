using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    [SerializeField] public GameObject InventoryDrawer;
    [SerializeField] public List<Seed> Elements = new List<Seed>();
    [SerializeField] public Text MoneyInfo;
    [SerializeField] public Text ReputationInfo;
    [SerializeField] public Text EnergyInfo;
    [FormerlySerializedAs("EnergyRegenInfo")] 
    [SerializeField] public Text EnergyRegenTime;

    //private const int Devider = 5;
    public Action onItemAdded;
    public Action<Seed> onInventoryFull;
    public int Money { get; private set; }
    public int Reputation { get; private set; }
    public int ReputationLimit { get; private set; }
    public int ReputationLevel { get; private set; }
    public int Energy { get; private set; }
    public int EnergyMax { get; private set; }
    public int MaxItemsAmount { get; private set; }
    //The time in *seconds* it take to regenerate 1 energy
    public int EnergyRegenDelay { get; private set; }
    private float energyTimeBuffer;

    public void Start()
    {
        // Preventing null references etc
        EnergyRegenTime ??= GameObject.Find("RegenTime").GetComponent<Text>();
        ReputationInfo ??= GameObject.Find("ReputationInfo").GetComponent<Text>();
        
        ReputationLevel = 1;
        ReputationLimit = 500;

        MaxItemsAmount = 15;

        EnergyRegenDelay = 20;
        energyTimeBuffer = EnergyRegenDelay;

        CollectData();
        CollectEnergy();

        RedrawInfo();
    }

    private void Update()
    {
        if (Energy == EnergyMax)
        {
            EnergyRegenTime.text = "--:--";
            return;
        }

        energyTimeBuffer -= Time.deltaTime;
        if (Math.Ceiling(energyTimeBuffer) <= 0)
        {
            energyTimeBuffer = EnergyRegenDelay;
            RegenEnergy(1);
        }

        SaveEnergy();
        var minutes = Math.Floor(energyTimeBuffer / 60).ToString(CultureInfo.InvariantCulture);
        var seconds = Math.Floor(energyTimeBuffer % 60).ToString(CultureInfo.InvariantCulture);
        EnergyRegenTime.text = $"{minutes.PadLeft(2, '0')}:{seconds.PadLeft(2, '0')}";
        RedrawInfo();
    }

    public void AddItem(Seed newSeed)
    {
        newSeed.UpdateRating();
        InventoryDrawer.GetComponent<Drawinventory>().UpdateActions();
        if (Elements.Count >= MaxItemsAmount)
        {
            onInventoryFull?.Invoke(newSeed);
            return;
        }

        Elements.Add(newSeed);
        SaveData();
        onItemAdded?.Invoke();
    }

    public void ChangeMoney(int amount)
    {
        //Money += changingAmount > 0
        //    ? changingAmount/* / Devider*/
        //    : changingAmount;
        //if (Money <= -100 && changingAmount < 0
        //    || changingAmount > 0)
        //    Reputation += changingAmount / Devider;
        Money += amount;
        RedrawInfo();
    }

    public void ChangeReputation(int amount)
    {
        Reputation += amount;
        if (Reputation > ReputationLimit)
        {
            Reputation -= ReputationLimit;
            ReputationLevel++;
            ReputationLimit += 100;

            //Бонусы вот здесь
            EnergyMax++;
            Money += 100;
        }
        RedrawInfo();
    }

    public void ConsumeEnergy(int amount)
    {
        Energy -= amount;
        SaveEnergy();
        RedrawInfo();
    }

    public void RegenEnergy(int amount)
    {
        if (Energy + amount > EnergyMax)
            Energy = EnergyMax;
        else
            Energy += amount;
        SaveEnergy();
        RedrawInfo();
    }

    public void RemoveItem(int index)
    {
        Elements.RemoveAt(index);
        SaveData();
    }

    void RedrawInfo()
    {
        if (MoneyInfo != null) MoneyInfo.text = Money.ToString();
        if (ReputationInfo != null) ReputationInfo.text = $"Уровень {ReputationLevel}";
        if (EnergyInfo != null) EnergyInfo.text = $"{Energy}/{EnergyMax}";
    }

    public void SaveAllData()
    {
        SaveData();
        SaveEnergy();
    }

    private void SaveData()
    {
        PlayerPrefs.SetInt("money", Money);

        PlayerPrefs.SetInt("reputation", Reputation);
        PlayerPrefs.SetInt("reputationLimit", ReputationLimit);
        PlayerPrefs.SetInt("reputationLevel", ReputationLevel);

        PlayerPrefs.SetInt("energyMax", EnergyMax);

        PlayerPrefs.SetInt("amount", Elements.Count);

        for (var i = 0; i < Elements.Count; i++)
            PlayerPrefs.SetString(i.ToString(), Elements[i].ToString());

        PlayerPrefs.Save();
    }

    private void SaveEnergy()
    {
        PlayerPrefs.SetInt("energy", Energy);
        PlayerPrefs.SetString("energytime", DateTime.Now.ToString());
        PlayerPrefs.SetFloat("energytimebuffer", energyTimeBuffer);

        PlayerPrefs.Save();
    }

    private void CollectData()
    {
        Money = PlayerPrefs.GetInt("money");

        Reputation = PlayerPrefs.GetInt("reputation");
        ReputationLimit = PlayerPrefs.GetInt("reputationLimit");
        ReputationLevel = PlayerPrefs.GetInt("reputationLevel");

        Elements = new List<Seed>();

        EnergyMax = PlayerPrefs.GetInt("energyMax");

        var i = PlayerPrefs.GetInt("amount");
        for (var j = 0; j < i; j++)
        {
            var parameters = PlayerPrefs.GetString(j.ToString());
            var newSeed = ScriptableObject.CreateInstance<Seed>();
            newSeed.SetValues(parameters);
            Elements.Add(newSeed);
        }
    }

    private void CollectEnergy()
    {
        Energy = PlayerPrefs.GetInt("energy");
        energyTimeBuffer = PlayerPrefs.GetFloat("energytimebuffer");
        var oldDate = DateTime.Parse(PlayerPrefs.GetString("energytime"));
        //10 million ticks in a second
        var secondsElapsed = (float)(DateTime.Now.Ticks - oldDate.Ticks) / 10000000;
        var regenerated = (int)secondsElapsed / EnergyRegenDelay;
        RegenEnergy(regenerated);
        energyTimeBuffer -= secondsElapsed % EnergyRegenDelay;
    }
}
