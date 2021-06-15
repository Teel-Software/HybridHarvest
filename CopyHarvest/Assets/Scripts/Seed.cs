using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "seeds", menuName = "Seed")]
[System.Serializable]
public class Seed : ScriptableObject
{
    public string Name;
    public string NameInRussian;
    public string NameInLatin;
    public int Price;
    public int Amount;

    public Sprite PlantSprite;
    public Sprite SproutSprite;
    public Sprite GrownSprite;

    public Gen GabitusGen;
    public int Gabitus;

    public Gen TasteGen;
    public int Taste;

    public Gen GrowTimeGen;
    [SerializeField] public int GrowTime;

    public void SetValues(string data)
    {
        var parameters = data.Split('|');
        Name = parameters[0];
        Price = int.Parse(parameters[1]);
        GrowTime = int.Parse(parameters[2]);
        GrowTimeGen = (Gen)int.Parse(parameters[3]);
        Gabitus = int.Parse(parameters[4]);
        GabitusGen = (Gen)int.Parse(parameters[5]);
        Taste = int.Parse(parameters[6]);
        TasteGen = (Gen)int.Parse(parameters[7]);
        Amount = int.Parse(parameters[8]);
        PlantSprite = Resources.Load<Sprite>("SeedsIcons\\" + parameters[0]);
        SproutSprite = Resources.Load<Sprite>("SeedsIcons\\" + parameters[0] + "Sprout");
        GrownSprite = Resources.Load<Sprite>("SeedsIcons\\" + parameters[0] + "Grown");
        NameInRussian = parameters[9];
        NameInLatin = parameters[10];
    }

    public override string ToString()
    {
        return Name + "|" + Price + "|" +
               GrowTime + "|" + (int)GrowTimeGen + "|" +
               Gabitus + "|" + (int)GabitusGen + "|" +
               Taste + "|" + (int)TasteGen + "|" +
               Amount + "|" + NameInRussian + "|" + NameInLatin;
    }
}

public enum Gen
{
    Recessive,
    Mixed,
    Dominant
}
