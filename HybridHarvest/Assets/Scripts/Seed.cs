using System;
using UnityEngine;

[CreateAssetMenu(fileName = "seeds", menuName = "Seed")]
[Serializable]

public class Seed : ScriptableObject
{
    public string Name;
    public string NameInRussian;
    public string NameInLatin;

    public int Price
    {
        get
        {
            var multiplier =
                Market.PriceMultipliers.ContainsKey(Name) ?
                Market.PriceMultipliers[Name] : 1.0f;
            return (int)(Taste * multiplier);
        }
    }
    public int minAmount;
    public int maxAmount;
    public int ShopBuyPrice;
    public Sprite PlantSprite;
    public Sprite SproutSprite;
    public Sprite GrownSprite;
    //example import .\Packets\Packet0.png
    public Sprite PacketSprite;

    public Gen GabitusGen;
    public int Gabitus;

    public Gen TasteGen;
    public int Taste;

    public Gen GrowTimeGen;
    public int GrowTime;

    /// <summary>
    /// Imports seed data from string
    /// </summary>
    /// <param name="data">string with "|" separator</param>
    public void SetValues(string data)
    {
        var parameters = data.Split('|');
        Name = parameters[0];
        GrowTime = int.Parse(parameters[2]);
        GrowTimeGen = (Gen)int.Parse(parameters[3]);
        Gabitus = int.Parse(parameters[4]);
        GabitusGen = (Gen)int.Parse(parameters[5]);
        Taste = int.Parse(parameters[6]);
        TasteGen = (Gen)int.Parse(parameters[7]);
        minAmount = int.Parse(parameters[8]);
        maxAmount = int.Parse(parameters[9]);
        PlantSprite = Resources.Load<Sprite>("SeedsIcons\\" + parameters[0]);
        SproutSprite = Resources.Load<Sprite>("SeedsIcons\\" + parameters[0] + "Sprout");
        GrownSprite = Resources.Load<Sprite>("SeedsIcons\\" + parameters[0] + "Grown");

        UpdateRating();

        NameInRussian = parameters[10];
        NameInLatin = parameters[11];
    }

    public void UpdateRating()
    {
        var rating = Gabitus * 0.33 + Taste * 0.33 + GrowTime * 10;
        var packetQuality = 0;
        switch (rating)
        {
            case var i when i < 40:
                packetQuality = 0;
                break;
            case var i when i >= 40 && i < 60:
                packetQuality = 1;
                break;
            case var i when i >= 60 && i < 80:
                packetQuality = 2;
                break;
            case var i when i >= 80 && i < 95:
                packetQuality = 3;
                break;
            case var i when i >= 95:
                packetQuality = 4;
                break;
        }
        PacketSprite = Resources.Load<Sprite>("Packets\\Packet" + packetQuality);
    }

    /// <summary>
    /// Exports seed data as string
    /// </summary>
    /// <returns>string with "|" separator</returns>
    public override string ToString()
    {
        return Name + "|" + "_" + "|" +
               GrowTime + "|" + (int)GrowTimeGen + "|" +
               Gabitus + "|" + (int)GabitusGen + "|" +
               Taste + "|" + (int)TasteGen + "|" +
               minAmount + "|" + maxAmount +
               "|" + NameInRussian + "|" + NameInLatin;
    }
}

public enum Gen
{
    Recessive,
    Mixed,
    Dominant
}
