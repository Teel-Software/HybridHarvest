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
    [SerializeField] public Text MoneyInfo;
    [SerializeField] public Text ReputationInfo;
    private const int Devider = 5;
    public Action onItemAdded;
    public int Money { get; private set; }
    public int Reputation { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        //Info.text = "started";
        //SaveData();
       // Info.text = "data saved";
        CollectData();
        //Info.text = "data collected";
        RedrawInfo();
        //Info.text = "redrawn";
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
        if (MoneyInfo != null) MoneyInfo.text = Money.ToString();
        if (ReputationInfo != null) ReputationInfo.text = Reputation.ToString();
    }

    public void SaveData()
    {
        //var data = new List<string>();
        //data.Add(Money.ToString());
        //data.Add(Reputation.ToString());
        //foreach (var e in Elements)
        //    data.Add(e.ToString());
        //File.WriteAllLines("Assets\\Resources\\data.TXT", data);
        PlayerPrefs.SetInt("mony", Money);
        PlayerPrefs.SetInt("repa", Reputation);
        PlayerPrefs.SetInt("amo", Elements.Count);
        for (var i=0;i<Elements.Count; i++)
        {
            PlayerPrefs.SetString(i.ToString(),Elements[i].ToString());
            //print(Elements[i].ToString());
        }
    }

    private void CollectData()
    {
        //var data = File.ReadAllLines("Assets\\Resources\\data.TXT");
        //var data = Resources.Load<TextAsset>("data").text.Split('\n');
        //Money = int.Parse(data[0]);
        //Reputation = int.Parse(data[1]);
        //Debug.Log(data[2]);
        //for (var i =2; i< data.Length; i++)
        //{
        //    if (data[i] == "")
        //        return;
        //    var parameters = data[i].Split('|');
        //    Debug.Log(parameters[2].Length);
        //    // Debug.Log(parameters[2][4]);
        //    parameters[2] = parameters[2].Substring(0, parameters[2].Length - 1);
        //    Elements.Add(new Seed ( parameters[0],int.Parse(parameters[1]), parameters[2]));
        //}
        Money = PlayerPrefs.GetInt("mony");
        Reputation = PlayerPrefs.GetInt("repa");
        var i = PlayerPrefs.GetInt("amo");
        for (var j =0; j < i; j++)
        {
            var parameters = PlayerPrefs.GetString(j.ToString());
            //print(parameters);
            Elements.Add(new Seed(parameters));
        }
    }
}
