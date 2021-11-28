using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MarketMenu : MonoBehaviour
{ 
    [SerializeField] public GridLayoutGroup ScrollList;
    [SerializeField] public GameObject Listing;
    public void Awake()
    {
        
    }

    public void OnEnable()
    {
        foreach (var seedName in Market.PriceMultipliers.Keys)
        {
            var objName = $"{seedName}Multiplier";
            var listing = Instantiate(Listing, ScrollList.transform);
            listing.name = objName;
            listing.GetComponentInChildren<Image>().sprite = Resources.Load<Sprite>($"SeedsIcons\\{seedName}");
            listing.GetComponentInChildren<Text>().text = $"x {Market.PriceMultipliers[seedName]}";
        }
    }

    public void OnDisable()
    {
        foreach (Transform child in ScrollList.transform)
            Destroy(child.gameObject); 
    }
}
