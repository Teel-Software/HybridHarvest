using System.Collections;
using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class Inventory : MonoBehaviour
{
    [SerializeField] public List<Seed> Elements = new List<Seed>();
    [SerializeField] public Text Info;
    private const int Devider = 5;
    public Action onItemAdded;
    public int Money { get; private set; }
    public int Reputation { get; private set; }

    // Start is called before the first frame update
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
        Money += changingAmount > 0
            ? changingAmount / Devider
            : changingAmount;
        if (Money <= -100 && changingAmount < 0
            || changingAmount > 0)
            Reputation += changingAmount / Devider;
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
        Info.text = $"Деньги: {Money}   Репутация: {Reputation}";
    }

    public void SaveData()
    {
        var data = new List<string>();
        data.Add(Money.ToString());
        data.Add(Reputation.ToString());
        foreach (var e in Elements)
            data.Add(e.ToString());
        File.WriteAllLines("Assets\\Resources\\data.TXT", data);
    }

    private void CollectData()
    {
        var data = File.ReadAllLines("Assets\\Resources\\data.TXT");
        Money = int.Parse(data[0]);
        Reputation = int.Parse(data[1]);
        for(var i =2; i< data.Length; i++)
        {
            var parameters = data[i].Split('|');
            Elements.Add(new Seed ( parameters[0],int.Parse(parameters[1]), parameters[2]));
        }
    }
}
