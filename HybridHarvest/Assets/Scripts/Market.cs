using System;
using System.Collections.Generic;
using System.Linq;
using CI.QuickSave;
using UnityEngine;
using UnityEngine.UI;

public class Market : MonoBehaviour
{
    [SerializeField] public GridLayoutGroup ScrollList;
    [SerializeField] public GameObject Listing;
        
    public static Dictionary<string, float> PriceMultipliers { get; private set; }

    private Inventory _inventory;

    public void Start()
    {
        PriceMultipliers ??= new Dictionary<string, float>();
        
        _inventory ??= GameObject.Find("DataKeeper").GetComponent<Inventory>();
        var seedsAvailable = _inventory.Elements.Select(el => el.Name).Distinct().ToList();
        
        foreach (var seedName in seedsAvailable)
        {
            if (PriceMultipliers.ContainsKey(seedName)) continue;
            PriceMultipliers.Add(seedName, 1.0f);
            
            var objName = $"{seedName}Multiplier";
            var listing = Instantiate(Listing, ScrollList.transform);
            listing.name = objName;
            listing.GetComponentInChildren<Image>().sprite = Resources.Load<Sprite>($"SeedsIcons\\{seedName}");
            listing.GetComponentInChildren<Text>().text = $"x {PriceMultipliers[seedName]}";
        }
    }
    
    /*
    public static void Save()
    {
        var writer = QuickSaveWriter.Create("UnlockedSeeds");
        PriceMultipliers["Tomato"] = 1.3f;
        writer.Write("PriceMultipliers", PriceMultipliers);
        writer.Commit();
        PriceMultipliers = new Dictionary<string, float>();
    }

    public static void Load()
    {
        var reader = QuickSaveReader.Create("UnlockedSeeds");
        PriceMultipliers = reader.Read<Dictionary<string, float>>("PriceMultipliers");
        Debug.Log(PriceMultipliers["Tomato"]);
    }*/
}