using System;
using System.Collections.Generic;
using System.Linq;
using CI.QuickSave;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

public class Market : MonoBehaviour
{
    [SerializeField] public GridLayoutGroup ScrollList;
    [SerializeField] public GameObject Listing;
        
    public static Dictionary<string, float> PriceMultipliers { get; private set; }
    private DateTime lastDate;

    private Inventory _inventory;
    
    private List<string> seedsAvailable;
    
    private readonly int hoursToRefresh = 24;
    public void Awake()
    {
        seedsAvailable = GetSeedsAvailable();
        Load();
    }

    public void OnDisable()
    {
        foreach (Transform child in ScrollList.transform)
            Destroy(child.gameObject);
    }

    public void OnEnable()
    {
        seedsAvailable = GetSeedsAvailable();

        var elapsed = DateTime.Now.Subtract(lastDate);
        if (elapsed.TotalHours >= hoursToRefresh)
            (PriceMultipliers, lastDate) = GetNewMarketValues();

        foreach (var seedName in seedsAvailable)
        {
            if (!PriceMultipliers.ContainsKey(seedName))
            {
                PriceMultipliers[seedName] = 1.0f;
            }
            var objName = $"{seedName}Multiplier";
            var listing = Instantiate(Listing, ScrollList.transform);
            listing.name = objName;
            listing.GetComponentInChildren<Image>().sprite = Resources.Load<Sprite>($"SeedsIcons\\{seedName}");
            listing.GetComponentInChildren<Text>().text = $"x {PriceMultipliers[seedName]}";
        }
        Save();
    }

    private List<string> GetSeedsAvailable()
    {
        _inventory ??= GameObject.Find("DataKeeper").GetComponent<Inventory>();
        return _inventory.Elements.Select(el => el.Name).Distinct().ToList();
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