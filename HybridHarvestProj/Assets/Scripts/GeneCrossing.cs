using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GeneCrossing : MonoBehaviour
{
    [SerializeField] RectTransform InventoryFrame;
    [SerializeField] Button button1;
    [SerializeField] Button button2;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void ContactToInventory() 
    {
        
    }

    public Seed MixTwoParents(Seed first, Seed second) 
    {
        var newSeed = new Seed(first.ToString());
        (newSeed.Taste, newSeed.TasteGen) = CountParameter(first.Taste, first.TasteGen,
                                                           second.Taste, second.TasteGen);
        (newSeed.Gabitus, newSeed.GabitusGen) = CountParameter(first.Gabitus, first.GabitusGen,
                                                               second.Gabitus, second.GabitusGen);
        (newSeed.GrowTime, newSeed.GrowTimeGen) = CountParameter(first.GrowTime, first.GrowTimeGen,
                                                               second.GrowTime, second.GrowTimeGen);
        newSeed.Price = newSeed.Taste;

        return newSeed;
    }

    public (int, Gen) CountParameter(int value1, Gen gen1, int value2, Gen gen2) 
    {
        if (gen1 == Gen.Dominant && gen2 == Gen.Dominant ) 
        {
            return (Mathf.Min(value1, value2), gen1);
        }
        if (gen1 == Gen.Recessive && gen2 == Gen.Recessive)
        {
            return (Mathf.Max(value1, value2), gen1);
        }
        if ((gen1 == Gen.Dominant && gen2 == Gen.Recessive) || (gen1 == Gen.Recessive && gen2 == Gen.Dominant)) 
        {
            return (Mathf.Min(value1, value2), Gen.Mixed);
        }
        if ((gen1 == Gen.Recessive && gen2 == Gen.Mixed)||(gen2 == Gen.Recessive && gen1 == Gen.Mixed))
        {
            return (GetNewValueByPossibility(value1, 50, value2), (Gen)GetNewValueByPossibility((int)gen1, 50, (int)gen2));
        }
        if ((gen1 == Gen.Dominant && gen2 == Gen.Mixed) || (gen2 == Gen.Dominant && gen1 == Gen.Mixed))
        {
            return (Mathf.Min(value1, value2), (Gen)GetNewValueByPossibility((int)gen1, 50, (int)gen2));
        }
        else 
        {
            Gen newGen;
            var possibility =(int) (Random.value*100);
            if (possibility <= 25) newGen = Gen.Dominant;
            else if(possibility < 75) newGen = Gen.Mixed;
            else newGen = Gen.Recessive;
            return (GetNewValueByPossibility(Mathf.Min(value1, value2), 75, Mathf.Max(value1, value2)),
                newGen);
        }
    }

    public int GetNewValueByPossibility(int value1, int value1Chance, int value2) 
    {
        var fortune =(int)( Random.value*100);
        return (fortune < value1Chance) ? value1 : value2;
    }
}
