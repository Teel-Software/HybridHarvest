using System;
using System.Collections.Generic;
using System.Linq;
using CI.QuickSave;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

public class Market : MonoBehaviour, ISaveable
{
    public static Dictionary<string, float> PriceMultipliers { get; private set; }
    private DateTime _lastRefreshDate;

    private static List<string> SeedTypesInInventory
    {
        get
        { 
            var inventory = GameObject.FindGameObjectWithTag("Inventory").GetComponent<Inventory>();
            return inventory.Elements.Select(el => el.Name).Distinct().ToList(); 
        }
    }

    private const int HoursToRefresh = 24;

    public void Awake()
    {
        Load();
    }

    public void Update()
    {
        foreach (var seedName in SeedTypesInInventory)
            if (!PriceMultipliers.ContainsKey(seedName))
                PriceMultipliers[seedName] = 1.0f;
        
        var elapsed = DateTime.Now - _lastRefreshDate;
        if (elapsed.TotalHours >= HoursToRefresh)
        {
            PriceMultipliers = GetNewMultipliers();
            _lastRefreshDate = DateTime.Today;
        }
    }

    /// <summary>
    /// Используется вместо OnDestroy из-за особенностей мобильных платформ
    /// </summary>
    private void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus) 
            Save();
    }

    private Dictionary<string,float> GetNewMultipliers()
    {
        var rand = new Random(_lastRefreshDate.DayOfYear * DateTime.Now.Second);
        var newDict = new Dictionary<string, float>();
        
        foreach (var plant in PriceMultipliers.Keys)
        {
            var newMul = rand.NextDouble() + 0.5;
            newDict[plant] = (float)Math.Round(newMul, 1);
        }

        return newDict;
    }
    
    public void Save()
    {
        var writer = QuickSaveWriter.Create("Market");
        writer.Write("Multipliers", PriceMultipliers)
            .Write("LastDate", _lastRefreshDate);
        writer.Commit();
    }

    public void Load()
    {
        var reader = QSReader.Create("Market");
        
        if (reader.Exists("Multipliers"))
            PriceMultipliers = reader.Read<Dictionary<string, float>>("Multipliers");
        else
        {
            PriceMultipliers ??= new Dictionary<string, float>();
            foreach (var seedName in SeedTypesInInventory)
                if (!PriceMultipliers.ContainsKey(seedName))
                    PriceMultipliers.Add(seedName, 1.0f);
        }
        
        _lastRefreshDate = reader.Exists("LastDate") ? reader.Read<DateTime>("LastDate") : DateTime.Today;
    }
}