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
    public int PacketQuality { get; private set; }

    public Gen GabitusGen;
    public int Gabitus;

    public Gen TasteGen;
    public int Taste;

    public Gen GrowTimeGen;
    public int GrowTime;

    public MutationChance MutationPossibility;

    public SeedStatistics LevelData;

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
        NameInRussian = parameters[10];
        NameInLatin = parameters[11];
        PlantSprite = Resources.Load<Sprite>("SeedsIcons\\" + parameters[12]);
        SproutSprite = Resources.Load<Sprite>("SeedsIcons\\" + parameters[13]);
        GrownSprite = Resources.Load<Sprite>("SeedsIcons\\" + parameters[14]);

        LevelData = CSVReader.ParseSeedStats(parameters[0]);

        MutationPossibility = (MutationChance)int.Parse(parameters[15]);
        //Debug.Log(LevelData.ToString());

        UpdateRating();
    }

    public void UpdateRating()
    {
        LevelData ??= CSVReader.ParseSeedStats(Name);
        var rating = 0;

        try
        {
            rating = LevelData.Gabitus[Gabitus]
                         + LevelData.Taste[Taste]
                         + LevelData.MutationChance[MutationPossibility]
                         + LevelData.MinAmount[minAmount]
                         + LevelData.GrowTime[GrowTime];
        }
        catch
        {
            // Раскомментируйте строчку ниже для дебага. (Возможен спам)

            //Debug.Log($"В таблице характеристик не указано одно или несколько значений. Рейтинг семян \"{NameInRussian}\" равен нулю.");
        }

        switch (rating)
        {
            case var i when i < 40:
                PacketQuality = 0;
                break;
            case var i when i >= 40 && i < 60:
                PacketQuality = 1;
                break;
            case var i when i >= 60 && i < 80:
                PacketQuality = 2;
                break;
            case var i when i >= 80 && i < 95:
                PacketQuality = 3;
                break;
            case var i when i >= 95:
                PacketQuality = 4;
                break;
        }
        PacketSprite = Resources.Load<Sprite>("Packets\\Packet" + PacketQuality);
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
               "|" + NameInRussian + "|" + NameInLatin +
               "|" + PlantSprite.name + "|" + SproutSprite.name + "|" + GrownSprite.name
               + "|" + (int)MutationPossibility;
    }
}

public enum Gen
{
    Recessive,
    Mixed,
    Dominant
}

public enum MutationChance
{
    Low,
    Normal,
    High,
    Ultra
}
