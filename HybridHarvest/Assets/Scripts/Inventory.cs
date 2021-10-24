using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    [SerializeField] public List<Seed> Elements = new List<Seed>();
    [SerializeField] public Text MoneyInfo;
    [SerializeField] public Text ReputationInfo;
    [SerializeField] public Text EnergyInfo;
    [SerializeField] public Text EnergyRegenInfo;
    //private const int Devider = 5;
    public Action onItemAdded;
    public int Money { get; private set; }
    public int Reputation { get; private set; }
    public int Energy { get; private set; }
    public int EnergyMax { get; private set; }
    //The time in *seconds* it take to regenerate 1 energy
    public int EnergyRegenDelay { get; private set; }
    private float energyTimeBuffer;
    void Start()
    {
        ReputationInfo = GameObject.Find("ReputationInfo").GetComponent<Text>();
        EnergyMax = 10;
        EnergyRegenDelay = 20;
        energyTimeBuffer = EnergyRegenDelay;
        CollectData();
        RedrawInfo();
    }

    private void Update()
    {
        if (Energy == EnergyMax) return;
        
        energyTimeBuffer -= Time.deltaTime;
        if (Math.Ceiling(energyTimeBuffer) == 0)
        {
            energyTimeBuffer = EnergyRegenDelay;
            RegenEnergy(1);
        }
        PlayerPrefs.SetFloat("energytimebuffer", energyTimeBuffer);
        var minutes = Math.Floor(energyTimeBuffer / 60).ToString(CultureInfo.InvariantCulture);
        var seconds = Math.Floor(energyTimeBuffer % 60).ToString(CultureInfo.InvariantCulture);
        EnergyRegenInfo.text = $"{minutes.PadLeft(2, '0')}:{seconds.PadLeft(2, '0')}";
    }

    public void AddItem(Seed newSeed)
    {
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
        RedrawInfo();
    }

    public void ConsumeEnergy(int amount)
    {
        Energy -= amount;
        PlayerPrefs.SetInt("energy", Energy);
        RedrawInfo();
    }
    
    public void RegenEnergy(int amount)
    {
        if (Energy == EnergyMax) return;
        Energy += amount;
        PlayerPrefs.SetInt("energy", Energy);
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
        //else MoneyInfo.text = "0";
        if (ReputationInfo != null) ReputationInfo.text = Reputation.ToString();
        //else ReputationInfo.text = "0";
        if (EnergyInfo != null) EnergyInfo.text = $"{Energy} / {EnergyMax}";
        
    }

    public void SaveData()
    {
        PlayerPrefs.SetInt("money", Money);
        PlayerPrefs.SetInt("reputation", Reputation);
        PlayerPrefs.SetInt("amount", Elements.Count);

        for (var i = 0; i < Elements.Count; i++)
        {
            PlayerPrefs.SetString(i.ToString(), Elements[i].ToString());
        }
    }

    private void CollectData()
    {
        Money = PlayerPrefs.GetInt("money");
        Reputation = PlayerPrefs.GetInt("reputation");
        var i = PlayerPrefs.GetInt("amount");
        Energy = PlayerPrefs.GetInt("energy");
        energyTimeBuffer = PlayerPrefs.GetFloat("energytimebuffer");
        for (var j = 0; j < i; j++)
        {
            var parameters = PlayerPrefs.GetString(j.ToString());
            var newSeed = ScriptableObject.CreateInstance<Seed>();
            newSeed.SetValues(parameters);
            Elements.Add(newSeed);
        }
    }
}
