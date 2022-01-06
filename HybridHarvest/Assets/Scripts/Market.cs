using System;
using System.Collections.Generic;
using System.Linq;
using CI.QuickSave;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

public class Market : MonoBehaviour
{
    public static Dictionary<string, float> PriceMultipliers { get; private set; }
    private DateTime lastDate;

    private List<string> seedsAvailable
    {
        get
        { 
            var inventory = GameObject.Find("DataKeeper").GetComponent<Inventory>();
            return inventory.Elements.Select(el => el.Name).Distinct().ToList(); 
        }
    }
    
    private readonly int hoursToRefresh = 24;
    public void Awake()
    {
        Load();
    }

    public void Update()
    {
        foreach (var seedName in seedsAvailable)
            if (!PriceMultipliers.ContainsKey(seedName))
                PriceMultipliers[seedName] = 1.0f;
        
        var elapsed = DateTime.Now - lastDate;
        if (elapsed.TotalHours >= hoursToRefresh)
        {
            (PriceMultipliers, lastDate) = GetNewMarketValues();
            Save();
        }
    }

    private (Dictionary<string,float> , DateTime) GetNewMarketValues()
    {
        var rand = new Random(lastDate.DayOfYear * DateTime.Now.Second);
        var newDict = new Dictionary<string, float>();
        
        foreach (var plant in PriceMultipliers.Keys)
        {
            var newMul = rand.NextDouble() + 0.5;
            newDict[plant] = (float)Math.Round(newMul, 1);
        }

        return (newDict, DateTime.Today);
    }
    
    private void Save()
    {
        var writer = QuickSaveWriter.Create("Market");
        writer.Write("Multipliers", PriceMultipliers)
            .Write("LastDate", lastDate);
        writer.Commit();
    }

    private void Load()
    {
        var reader = QSReader.Create("Market");
        
        if (reader.Exists("Multipliers"))
            PriceMultipliers = reader.Read<Dictionary<string, float>>("Multipliers");
        else
        {
            PriceMultipliers ??= new Dictionary<string, float>();
            foreach (var seedName in seedsAvailable)
                if (!PriceMultipliers.ContainsKey(seedName))
                    PriceMultipliers.Add(seedName, 1.0f);
        }
        
        lastDate = reader.Exists("LastDate") ? reader.Read<DateTime>("LastDate") : DateTime.Today;
    }
}