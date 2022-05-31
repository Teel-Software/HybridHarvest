using System;
using UnityEngine;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.Serialization;
using Random = System.Random;

[CreateAssetMenu(fileName = "seeds", menuName = "Seed")]
[Serializable]

public class Seed : ScriptableObject
{
    public string Name => string.Join("-", Parents);
    public string NameInRussian;
    public string NameInLatin;
    public string GrowSpritesName => Parents[0];

    public List<string> Parents;

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
                    new Rect(0f, 0f, myTexture2D.width, myTexture2D.height),
                    new Vector2(0f, 0f), 1f
                );
            }
            return sp;
        }

    }
    public Sprite EarlySprite => Resources.Load<Sprite>($"SeedsIcons\\{GrowSpritesName}Early");
    public Sprite SproutSprite => Resources.Load<Sprite>($"SeedsIcons\\{GrowSpritesName}Sprout");
    public Sprite YoungSprite => Resources.Load<Sprite>($"SeedsIcons\\{GrowSpritesName}Young");
    public Sprite GrownSprite => Resources.Load<Sprite>($"SeedsIcons\\{GrowSpritesName}Grown");
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
                         + SeedStats.MinAmount[MinAmount]
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
    [FormerlySerializedAs("minAmount")] 
    public int MinAmount;
    [FormerlySerializedAs("maxAmount")] 
    public int MaxAmount;
    
    [FormerlySerializedAs("MutationPossibilityGen")]
    public Gen MutationChanceGen;
    [FormerlySerializedAs("MutationPossibility")]
    public MutationChance MutationChance;

    public SeedStatistics SeedStats;
    
    /// <summary>
    /// Exports seed data as a JSON string
    /// </summary>
    /// <returns>JSON string</returns>
    public override string ToString()
    {
        return JsonUtility.ToJson(this);
    }

    public static Seed Create(string seedString)
    {
        var seed = CreateInstance<Seed>();
        try
        {
            JsonUtility.FromJsonOverwrite(seedString, seed);
        }
        catch
        {
            seed.setValues(seedString);
        }
        return seed;
    }
    
    public static Seed Create(string nameEnglish, int taste, int gabitus, int growTime, int mutationChance, int minAmt, int maxAmt)
    {
        var seed = LoadFromResources(nameEnglish);
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

        seed.MinAmount = minAmt;
        seed.MaxAmount = maxAmt;
        
        seed.ShopBuyPrice = 0;
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

    private const int MutationPointsMul = 3;
    private const int GrowTimePointDiv = 1000;
    public double ConvertToPoints()
    {
        var points = (double)(MaxAmount + MinAmount) / 2
            * (Taste + Gabitus + (int)MutationChance * MutationPointsMul) 
            - Math.Round((double)GrowTime / GrowTimePointDiv);
        return points;
    }

    public static Seed CreateRandom(string seedName, double points)
    {
        //Debug.Log($"{seedName} ; Initial points: {points}");

        var example = LoadFromResources(seedName);
        var stats = example.SeedStats;
        var random = new Random(Environment.TickCount);
        
        var randGrowTime = stats.GrowTime.ElementAt(random.Next(stats.GrowTime.Values.Count)).Key;
        points += Math.Round((double)randGrowTime / GrowTimePointDiv);
        
        var randMinAmt = stats.MinAmount.ElementAt(random.Next(stats.MinAmount.Count)).Key;
        var randMaxAmt = stats.MaxAmount.ElementAt(random.Next(stats.MaxAmount.Count)).Key;
        var avgAmt = (randMaxAmt + randMinAmt) / 2;
        points /= avgAmt;
        
        var maxRandMutation = (int)stats.MutationChance.Max(x => x.Key);
        var randMutationChance = random.Next(maxRandMutation + 1);
        points -= randMutationChance * MutationPointsMul;
        
        var randGabitus = random.Next((int)points);
        if (randGabitus < 1)
            randGabitus = 1;
        points -= randGabitus;
        
        var randTaste = (int)points;
        if (randTaste < 1)
            randTaste = 1;
        
        //Debug.Log($"GrowTime {randGrowTime}; Amounts {randMinAmt}-{randMaxAmt}; Average {avgAmt}");
        //Debug.Log($"Muta {randMutationChance}; Gabi {randGabitus}; Taste {randTaste}");
        
        var randSeed = Create(seedName, randTaste, randGabitus, randGrowTime, randMutationChance, randMinAmt, randMaxAmt);
        //Debug.Log(randSeed);
        //Debug.Log($"{seedName} ; Resulting points: {randSeed.ConvertToPoints()}");
        
        return randSeed;
    }

    public static Seed LoadFromResources(string seedName)
    {
        var seed = Resources.Load<Seed>($"Seeds\\{seedName}");
        seed.SeedStats = CSVReader.GetSeedStats(seedName);
        return seed;
    }
    
    public static void JsonTest(Seed seed)
    {
        var str = JsonUtility.ToJson(seed);
        var s = CreateInstance<Seed>();
        try
        {
            JsonUtility.FromJsonOverwrite("lol", s);
        }
        catch
        {
            JsonUtility.FromJsonOverwrite(str, s);
        }
        Debug.Log($"Initial: {seed}");
        Debug.Log($"Resulting: {s}");
        Debug.Log($"Equal: {seed.ToString() == s.ToString()}");
        Debug.Log(str);
        Debug.Log(JsonUtility.ToJson(s));
        Debug.Log($"Equal: {str == JsonUtility.ToJson(s)}");
        Debug.Log(seed.SeedStats);
        Debug.Log(s.SeedStats); //null
    }
        
    /// <summary>
    /// (Deprecated)
    /// Exports seed data as string
    /// </summary>
    /// <returns>string with "|" separator</returns>
    private string toString()
    {
        return Name + "|" + "_" + "|" +
               GrowTime + "|" + (int)GrowTimeGen + "|" +
               Gabitus + "|" + (int)GabitusGen + "|" +
               Taste + "|" + (int)TasteGen + "|" +
               MinAmount + "|" + MaxAmount +
               "|" + NameInRussian + "|" + NameInLatin +
               "|" + (int)MutationChance;
    }
        
    /// <summary>
    /// (Deprecated)
    /// Imports seed data from string
    /// </summary>
    /// <param name="data">string with "|" separator</param>
    private void setValues(string data)
    {
        var parameters = data.Split('|');
        SeedStats = CSVReader.GetSeedStats(parameters[0]);
        //Name = parameters[0];
        GrowTime = int.Parse(parameters[2]);
        GrowTimeGen = (Gen)int.Parse(parameters[3]);
        Gabitus = int.Parse(parameters[4]);
        GabitusGen = (Gen)int.Parse(parameters[5]);
        Taste = int.Parse(parameters[6]);
        TasteGen = (Gen)int.Parse(parameters[7]);
        MinAmount = int.Parse(parameters[8]);
        MaxAmount = int.Parse(parameters[9]);
        NameInRussian = parameters[10];
        NameInLatin = parameters[11];
        MutationChance = (MutationChance)int.Parse(parameters[12]);
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
