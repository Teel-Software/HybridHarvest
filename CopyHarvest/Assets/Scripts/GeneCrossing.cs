using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GeneCrossing : MonoBehaviour
{
    [SerializeField] public Button CurrentPot;
    [SerializeField] RectTransform InventoryFrame;
    [SerializeField] Button button1;
    [SerializeField] Button button2;
    [SerializeField] Sprite defaultSprite;

    public void Clicked()
    {
        var seed1 = button1.GetComponent<LabButton>().NowSelected;
        var seed2 = button2.GetComponent<LabButton>().NowSelected;
        if (seed1 == null || seed2 == null)
            return;
        var newSeed = MixTwoParents(seed1, seed2);
        CurrentPot.GetComponent<LabGrowth>().PlantIt(newSeed);
        button2.GetComponent<LabButton>().ClearButton();
        button1.GetComponent<LabButton>().ClearButton();
        button1.GetComponent<LabButton>().PlaceForResult.GetComponent<LabButton>().ClearButton();
        button1.GetComponent<LabButton>().PlaceForResult.gameObject.SetActive(false);
        //button1.GetComponent<LabButton>().NowSelected = null;
        //button2.GetComponent<LabButton>().NowSelected = null;
        //button1.GetComponent<Image>().sprite = defaultSprite; 
        //button2.GetComponent<Image>().sprite = defaultSprite;
    }

    public Seed MixTwoParents(Seed first, Seed second)
    {
        var newSeed = ScriptableObject.CreateInstance<Seed>();
        newSeed.SetValues(first.ToString());
        (newSeed.Taste, newSeed.TasteGen) =
            CountParameter(first.Taste, first.TasteGen, second.Taste, second.TasteGen);
        (newSeed.Gabitus, newSeed.GabitusGen) =
            CountParameter(first.Gabitus, first.GabitusGen, second.Gabitus, second.GabitusGen);
        (newSeed.GrowTime, newSeed.GrowTimeGen) =
            CountParameter(first.GrowTime, first.GrowTimeGen, second.GrowTime, second.GrowTimeGen);
        newSeed.Price = newSeed.Taste;

        return newSeed;
    }

    public (int, Gen) CountParameter(int value1, Gen gen1, int value2, Gen gen2)
    {
        var dominant = Mathf.Min(value1, value2);
        var recessive = Mathf.Max(value1, value2);
        if (gen1 == Gen.Dominant && gen2 == Gen.Dominant)
        {
            return (dominant, gen1);
        }
        if (gen1 == Gen.Recessive && gen2 == Gen.Recessive)
        {
            return (recessive, gen1);
        }
        if (gen1 == Gen.Dominant && gen2 == Gen.Recessive || gen1 == Gen.Recessive && gen2 == Gen.Dominant)
        {
            return (dominant, Gen.Mixed);
        }
        if (gen1 == Gen.Recessive && gen2 == Gen.Mixed || gen2 == Gen.Recessive && gen1 == Gen.Mixed)
        {
            return (GetNewValueByPossibility(value1, 50, value2),
                (Gen)GetNewValueByPossibility((int)gen1, 50, (int)gen2));
        }
        if (gen1 == Gen.Dominant && gen2 == Gen.Mixed || gen2 == Gen.Dominant && gen1 == Gen.Mixed)
        {
            return (dominant, (Gen)GetNewValueByPossibility((int)gen1, 50, (int)gen2));
        }
        Gen newGen;
        var possibility = (int)Random.value * 100;
        if (possibility <= 25) newGen = Gen.Dominant;
        else if (possibility < 75) newGen = Gen.Mixed;
        else newGen = Gen.Recessive;
        return (GetNewValueByPossibility(dominant, 75,
                recessive), newGen);
    }

    public int GetNewValueByPossibility(int value1, int value1Chance, int value2)
    {
        var fortune = (int)(Random.value * 100);
        return fortune < value1Chance ? value1 : value2;
    }
}
