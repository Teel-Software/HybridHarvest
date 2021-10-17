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
    public Sprite BagSprite;

    public Gen GabitusGen;
    public int Gabitus;

    public Gen TasteGen;
    public int Taste;

    public Gen GrowTimeGen;
    [SerializeField] public int GrowTime;

    /// <summary>
    /// Заполняет пустой элемент класса Seed
    /// </summary>
    /// <param name="data"></param>
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
        var rating = Gabitus * 0.33 + Taste * 0.33 + GrowTime * 0.33;
        int bagNum = 0;
        switch (rating)
        {
            case var i when i < 40:
                bagNum = 0;
                break;
            case var i when i >= 40 && i<60:
                bagNum = 1;
                break;
            case var i when i >= 60 && i<80:
                bagNum = 2;
                break;
            case var i when i >= 80 && i<95:
                bagNum = 3;
                break;
            case var i when i >= 95:
                bagNum = 4;
                break;
        }
        BagSprite = Resources.Load<Sprite>("SeedsIcons\\Packet" + bagNum.ToString());
        Debug.Log(BagSprite);
        NameInRussian = parameters[9];
        NameInLatin = parameters[10];
    }

    /// <summary>
    /// превращает элемент класса seed в строку
    /// </summary>
    /// <returns></returns>
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
