using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckInventory : MonoBehaviour
{
    [SerializeField] Inventory targetInventory;
     public void AddOneMore(int price)
    {
        var seed =(Seed) Resources.Load("Seeds\\apple");
        seed.GrowTime = 10;
        seed.GrowTimeGen = Gen.Mixed;
        seed.Gabitus = 10;
        seed.GabitusGen = Gen.Mixed;
        seed.Taste = 10;
        seed.TasteGen = Gen.Mixed;
        targetInventory.ChangeMoney(-price);
        targetInventory.AddItem(seed);
    }
}
