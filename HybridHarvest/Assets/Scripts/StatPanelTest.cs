using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class StatPanelTest : MonoBehaviour
{
    [SerializeField] public GameObject StatPanel;
    
    public void LetsGo()
    {
        var inv = GameObject.Find("DataKeeper").GetComponent<Inventory>();
        var rand = new Random();
        var num = rand.Next(0, inv.Elements.Count);
        
        var seedItem = inv.Elements[num];
        var stat = Instantiate(StatPanel, GameObject.Find("Canvas").transform);
        var statDrawer = stat.GetComponentInChildren<StatPanelDrawer>();
        statDrawer.PlantImage.sprite = seedItem.PlantSprite;
    }
}
