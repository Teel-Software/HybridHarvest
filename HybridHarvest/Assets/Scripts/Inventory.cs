using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    [SerializeField] public List<Seed> Elements = new List<Seed>();
    [SerializeField] public Text MoneyInfo;
    [SerializeField] public Text ReputationInfo;

    //private const int Devider = 5;
    public Action onItemAdded;
    public int Money { get; private set; }
    public int Reputation { get; private set; }

    void Start()
    {
        CollectData();
        RedrawInfo();
    }

    public void AddItem(Seed newSeed)
    {
        Elements.Add(newSeed);
        SaveData();
        onItemAdded?.Invoke();
    }

    public void ChangeMoney(int changingAmount)
    {
        //Money += changingAmount > 0
        //    ? changingAmount/* / Devider*/
        //    : changingAmount;
        //if (Money <= -100 && changingAmount < 0
        //    || changingAmount > 0)
        //    Reputation += changingAmount / Devider;
        Money += changingAmount;
        RedrawInfo();
    }

    public void ChangeReputation(int changingAmount)
    {
        Reputation += changingAmount;
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
        for (var j = 0; j < i; j++)
        {
            var parameters = PlayerPrefs.GetString(j.ToString());
            var newSeed = ScriptableObject.CreateInstance<Seed>();
            newSeed.SetValues(parameters);
            Elements.Add(newSeed);
        }
    }
}
