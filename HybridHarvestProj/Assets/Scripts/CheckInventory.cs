using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckInventory : MonoBehaviour
{
    [SerializeField] Inventory targetInventory;
     public void AddOneMore(int price)
    {
        var seed =(Seed) Resources.Load("Seeds\\apple");
        //targetInventory.AddItem(seed);
        targetInventory.ChangeMoney(-price);
        targetInventory.AddItem(seed);
        //targetInventory.SaveData();
    }
}
