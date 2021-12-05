using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MarketMenu : MonoBehaviour
{ 
    [SerializeField] public GridLayoutGroup ScrollList;
    [SerializeField] public GameObject Listing;

    public void OnEnable()
    {
        foreach (var seedName in Market.PriceMultipliers.Keys)
        {
            var objName = $"{seedName}Multiplier";
            var listing = Instantiate(Listing, ScrollList.transform);
            listing.name = objName;
            
            listing.GetComponentsInChildren<Image>()[1].sprite = Resources.Load<Sprite>($"SeedsIcons\\{seedName}");
            
            var txtComponent = listing.GetComponentInChildren<Text>();
            var multiplier = Market.PriceMultipliers[seedName];
            txtComponent.text = $"x {multiplier}";
            txtComponent.color = 
                multiplier == 1.0f ? 
                    new Color(0,0,0) : 
                multiplier > 1.0f ? 
                    new Color(0f, 0.76f, 0.02f) : 
                new Color(0.75f, 0.16f, 0.13f);
        }
    }

    public void OnDisable()
    {
        foreach (Transform child in ScrollList.transform)
            Destroy(child.gameObject); 
    }
}
