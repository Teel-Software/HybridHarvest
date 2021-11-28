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
    
    private List<string> seedsAvailable;
    
    private readonly int hoursToRefresh = 24;
    public void Awake()
    {
        seedsAvailable = GetSeedsAvailable();
        Load();
    }

    public void Update()
    {
        seedsAvailable = GetSeedsAvailable();
        foreach (var seedName in seedsAvailable
            .Where(seedName => !PriceMultipliers.ContainsKey(seedName)))
            PriceMultipliers[seedName] = 1.0f;
        var elapsed = DateTime.Now.Subtract(lastDate);
        if (elapsed.TotalHours >= hoursToRefresh)
        {
            (PriceMultipliers, lastDate) = GetNewMarketValues();
            Save();
        }
    }
    
    public static List<string> GetSeedsAvailable()
    {
        var inventory = GameObject.Find("DataKeeper").GetComponent<Inventory>();
        return inventory.Elements.Select(el => el.Name).Distinct().ToList();
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
        writer.Write("Multipliers", PriceMultipliers);
        writer.Write("LastDate", lastDate);
        writer.Commit();
    }

    private void Load()
    {
        QuickSaveReader reader;
        try
        {
            reader = QuickSaveReader.Create("Market");
        }
        catch (QuickSaveException)
        {
            var writer = QuickSaveWriter.Create("Market");
            writer.Commit();
            reader = QuickSaveReader.Create("Market");
        }
        
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