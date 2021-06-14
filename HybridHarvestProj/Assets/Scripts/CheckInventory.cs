using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckInventory : MonoBehaviour
{
    [SerializeField] Inventory targetInventory;
    public void AddOneMore(int item)
    {
        //var seed =(Seed) Resources.Load("Seeds\\apple");
        //seed.GrowTime = 10;
        //seed.GrowTimeGen = Gen.Mixed;
        //seed.Gabitus = 10;
        //seed.GabitusGen = Gen.Mixed;
        //seed.Taste = 10;
        //seed.TasteGen = Gen.Mixed;
        //seed.Amount = 10;
        switch (item) 
        {
            case 1:
                var seed = (Seed)Resources.Load("Seeds\\Cucumber2");
                targetInventory.ChangeMoney(-seed.Price);
                targetInventory.AddItem(seed);
                break;
            case 2:
                var seed1 = (Seed)Resources.Load("Seeds\\Tomato2");
                targetInventory.ChangeMoney(-seed1.Price);
                targetInventory.AddItem(seed1);
                break;
            case 3:
                var seed2 = (Seed)Resources.Load("Seeds\\Carrot2");
                targetInventory.ChangeMoney(-seed2.Price);
                targetInventory.AddItem(seed2);
                break;
        }
    }
}
