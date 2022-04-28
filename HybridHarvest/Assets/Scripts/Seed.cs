using System;
using UnityEngine;
using System.IO;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "seeds", menuName = "Seed")]
[Serializable]

public class Seed : ScriptableObject
{
    public string Name;
    public string NameInRussian;
    public string NameInLatin;
    
    public int ShopBuyPrice;
    public int Price
    {
        get
        {
            if (ShopBuyPrice > 0)
                return ShopBuyPrice;
            var multiplier =
                Market.PriceMultipliers.ContainsKey(Name) ?
                Market.PriceMultipliers[Name] : 1.0f;
            return (int)(Taste * multiplier);
        }
    }

    public Sprite PlantSprite
    {
        get
        {
            var sp = Resources.Load<Sprite>($"SeedsIcons\\{Name}");
            if (sp is null)
            {
                var req = new WWW("file://" + Path.Combine(Application.persistentDataPath, Name + ".png"));
                var myTexture2D = req.texture;
                myTexture2D.filterMode = FilterMode.Point;
                sp = Sprite.Create(
                    myTexture2D,
                    new Rect(0.0f, 0.0f, myTexture2D.width, myTexture2D.height),
                    new Vector2(0f, 0f), 1f
                );
            }
            return sp;
        }

    }
    public Sprite EarlySprite => Resources.Load<Sprite>($"SeedsIcons\\{Name}Early");
    public Sprite SproutSprite => Resources.Load<Sprite>($"SeedsIcons\\{Name}Sprout");
    public Sprite YoungSprite => Resources.Load<Sprite>($"SeedsIcons\\{Name}Young");
    public Sprite GrownSprite => Resources.Load<Sprite>($"SeedsIcons\\{Name}Grown");
    public Sprite PacketSprite => Resources.Load<Sprite>($"Packets\\Packet{PacketQuality}");

    public int PacketQuality
    {
        get
        {
            SeedStats ??= CSVReader.GetSeedStats(Name);
            var rating = 0;
            try
            {
                rating = SeedStats.Gabitus[Gabitus]
                         + SeedStats.Taste[Taste]
                         + SeedStats.MutationChance[MutationChance]
                         + SeedStats.MinAmount[minAmount]
                         + SeedStats.GrowTime[GrowTime];
            }
            catch
            {
                // Раскомментируйте строчку ниже для дебага. (Возможен спам)
                //Debug.Log($"В таблице характеристик не указано одно или несколько значений. Рейтинг семян \"{NameInRussian}\" равен нулю.");
            }
            var quality = 0;
            switch (rating)
            {
                case var i when i < 40:
                    quality = 0;
                    break;
                case var i when i >= 40 && i < 60:
                    quality = 1;
                    break;
                case var i when i >= 60 && i < 80:
                    quality = 2;
                    break;
                case var i when i >= 80 && i < 95:
                    quality = 3;
                    break;
                case var i when i >= 95:
                    quality = 4;
                    break;
            }
            return quality;
        }
    }

    public Gen GabitusGen;
    public int Gabitus;

    public Gen TasteGen;
    public int Taste;

    public Gen GrowTimeGen;
    public int GrowTime;
    
    public Gen AmountGen;
    public int minAmount;
    public int maxAmount;
    
    [FormerlySerializedAs("MutationPossibilityGen")]
    public Gen MutationChanceGen;
    [FormerlySerializedAs("MutationPossibility")]
    public MutationChance MutationChance;

    public SeedStatistics SeedStats;

    /// <summary>
    /// Imports seed data from string
    /// </summary>
    /// <param name="data">string with "|" separator</param>
    public void SetValues(string data)
    {
        var parameters = data.Split('|');
        SeedStats = CSVReader.GetSeedStats(parameters[0]);
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
        //PlantSprite = Resources.Load<Sprite>("SeedsIcons\\" + parameters[12]);
        //SproutSprite = Resources.Load<Sprite>("SeedsIcons\\" + parameters[13]);
        //GrownSprite = Resources.Load<Sprite>("SeedsIcons\\" + parameters[14]);
        MutationChance = (MutationChance)int.Parse(parameters[12]);
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
               //"|" + PlantSprite.name + "|" + SproutSprite.name + "|" + GrownSprite.name
               "|" + (int)MutationChance;
    }
    
    public static Seed Create(string nameEnglish, int taste, int gabitus, int growTime, int mutationChance, int minAmt, int maxAmt)
    {
        var seed = Resources.Load<Seed>($"Seeds\\{nameEnglish}");
        seed.SeedStats = CSVReader.GetSeedStats(nameEnglish);
        //pls help
        //seed.NameInRussian = baselineSeed.NameInRussian;
        //seed.NameInLatin = baselineSeed.NameInLatin;

        seed.Taste = taste;
        //seed.TasteGen = baselineSeed.TasteGen;

        seed.Gabitus = gabitus;
        //seed.GabitusGen = baselineSeed.TasteGen;

        seed.GrowTime = growTime;
        //seed.GrowTimeGen = baselineSeed.GrowTimeGen;

        seed.MutationChance = (MutationChance)mutationChance;
        //seed.MutationChanceGen = baselineSeed.MutationChanceGen;

        seed.minAmount = minAmt;
        seed.maxAmount = maxAmt;
        
        return seed;
    }
    
    /// <summary>
    /// Получает спрайт растения на стадии роста
    /// </summary>
    /// <param name="time">Сколько осталось до конца роста</param>
    /// <param name="growTime">Сколько всего времени нужно для роста</param>
    /// <returns>Спрайт растения</returns>
    public Sprite GetGrowthStageSprite(double time, double growTime)
    {
        if (growTime >= time && time > growTime * 2 / 3)
            return EarlySprite;
        if (growTime * 2 / 3 >= time && time > growTime / 3)
            return SproutSprite;
        if (growTime / 3 >= time && time > 0)
            return YoungSprite;
        else
            return GrownSprite;
    }

    public const int MutationToPointsMultiplier = 3;
    public double ConvertToPoints()
    {
        var points = (double)(maxAmount + minAmount) / 2 * 
            (Taste + Gabitus + MutationToPointsMultiplier * (int)MutationChance) / GrowTime;
        return points;
    }

    public static Seed GetRandSeed(double points)
    {
        throw new NotImplementedException();
    }
    
    public static void JsonTest(Seed seed)
    {
        var str = JsonUtility.ToJson(seed);
        var s = CreateInstance<Seed>();
        JsonUtility.FromJsonOverwrite(str, s);
        Debug.Log($"Initial: {seed}");
        Debug.Log($"Resulting: {s}");
        Debug.Log($"Equal: {seed.ToString() == s.ToString()}");
        Debug.Log(str);
        Debug.Log(JsonUtility.ToJson(s));
        Debug.Log($"Equal: {str == JsonUtility.ToJson(s)}");
        Debug.Log(seed.SeedStats);
        Debug.Log(s.SeedStats); //null
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
