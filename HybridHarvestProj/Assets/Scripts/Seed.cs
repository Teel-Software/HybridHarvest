using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "seeds", menuName = "Seed")]
[System.Serializable]
public class Seed : ScriptableObject
{
    public string Name;
    public int Price;
    public Sprite MyImage;
    public int Amount;
    
    public Gen GabitusGen;
    public int Gabitus;
    
    public Gen TasteGen;
    public int Taste;
    
    public Gen GrowTimeGen;
    [SerializeField] public int GrowTime;

    public Seed(string name, int price, string image)
    {
        Name = name;
        Price = price;
        MyImage =  Resources.Load<Sprite>("SeedsIcons\\"+image);
    }
    public Seed(string data)
    {
        var parameters = data.Split('|');
        Name = parameters[0];
            Price = int.Parse(parameters[1]);
        MyImage = Resources.Load<Sprite>("SeedsIcons\\" + parameters[2]);
        GrowTime = int.Parse(parameters[3]);
        GrowTimeGen = (Gen)int.Parse(parameters[4]);
        Gabitus = int.Parse(parameters[5]);
        GabitusGen = (Gen)int.Parse(parameters[6]);
        Taste = int.Parse(parameters[7]);
        TasteGen = (Gen)int.Parse(parameters[8]);
        Amount = int.Parse(parameters[9]);
    }

    public override string ToString()
    {
        return Name + "|" + Price + "|" + MyImage.name + "|" +
               GrowTime+ "|" + (int)GrowTimeGen + "|" +
               Gabitus + "|" + (int)GabitusGen + "|" +
               Taste + "|" + (int)TasteGen + "|" +
               Amount;
    }
}

public enum Gen
{
    Recessive,
    Mixed,
    Dominant
}
