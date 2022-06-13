using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public static class CSVReader
{
    /// <summary>
    /// Returns SeedStatistics from file seedName.csv
    /// </summary>
    public static SeedStatistics GetSeedStats(string seedName)
    {
        var outStats = new SeedStatistics();
        var seedData = new string[] { };
        try
        {
            seedData = Resources
            .Load<TextAsset>($@"SeedStats\{seedName}")
            .text
            .Split('\n');
        }
        catch
        {
            try {                 //Вариант для скрещенных
                using (var reader = new StreamReader(Path.Combine(Application.persistentDataPath, seedName + ".csv")))
                {
                    seedData = reader.ReadToEnd().Split('\n');
                }
            }
            catch { return null; }
        }

        foreach (var line in seedData)
        {
            var splited = line
                .Split(',')
                .ToArray();

            switch (splited[0])
            {
                case var str when str.Contains("Габитус"):
                    outStats.Gabitus = ConvertToDict<int>(splited
                        .Skip(1)
                        .Select(gab => int.Parse(gab))
                        .ToArray());
                    break;
                case var str when str.Contains("Вкус"):
                    outStats.Taste = ConvertToDict<int>(splited
                        .Skip(1)
                        .Select(taste => int.Parse(taste))
                        .ToArray());
                    break;
                case var str when str.Contains("Кол"):
                    outStats.MinAmount = ConvertToDict<int>(splited
                        .Skip(1)
                        .Select(amount => int.Parse(amount.Split('-')[0]))
                        .ToArray());
                    outStats.MaxAmount = ConvertToDict<int>(splited
                        .Skip(1)
                        .Select(amount => int.Parse(amount.Split('-')[1]))
                        .ToArray());
                    break;
                case var str when str.Contains("Мутац"):
                    outStats.MutationChance = ConvertToDict<MutationChance>(splited
                        .Skip(1)
                        .Select(chance => char.ToLower(chance[0]) switch
                        {
                            'н' => MutationChance.Low,
                            'с' => MutationChance.Normal,
                            'в' => MutationChance.High,
                            'о' => MutationChance.Ultra,
                            //_ => chance,
                            _ => MutationChance.Low,
                        })
                        .ToArray());
                    break;
                case "Время":
                    outStats.GrowTime = ConvertToDict<int>(splited
                        .Where(str => str.Contains(':'))
                        .Select(str =>
                        {
                            var spl = str.Split(':');
                            int seconds;
                            if(spl.Length>2)
                             seconds = int.Parse(spl[0]) * 3600 + int.Parse(spl[1])*60 + int.Parse(spl[2]);
                            else
                                seconds = int.Parse(spl[0]) * 60 + int.Parse(spl[1]);
                            return seconds;
                        })
                        .ToArray());
                    break;
            }
        }

        return outStats;
    }

    /// <summary>
    /// Gets data from file seedName.csv
    /// </summary>
    /// <returns>All rows stored in dictionary</returns>
    public static Dictionary<string, int[]> GetRawData(string seedName)
    {
        var data = new Dictionary<string, int[]>();
        var seedData = new string[] { };

        try
        {
            seedData = Resources
            .Load<TextAsset>($@"SeedStats\{seedName}")
            .text
            .Split('\n');
        }
        catch
        {
            try
            {                 //Вариант для скрещенных
                using (StreamReader reader = new StreamReader(Path.Combine(Application.persistentDataPath, seedName + ".csv")))
                {
                    seedData = reader.ReadToEnd().Split('\n');
                }
            }
            catch { return null; }
        }

        foreach (var line in seedData)
        {
            var splited = line
                .Split(',')
                .ToArray();

            switch (splited[0])
            {
                case var str when str.Contains("Кол"):
                    data[splited[0]+"min"] = splited
                        .Skip(1)
                        .Select(amount => int.Parse(amount.Split('-')[0]))
                        .ToArray();
                    data[splited[0]] = splited
                        .Skip(1)
                        .Select(amount => int.Parse(amount.Split('-')[1]))
                        .ToArray();
                    break;
                case var str when str.Contains("Мутац"):
                    data[splited[0]] = splited
                        .Skip(1)
                        .Select(chance => char.ToLower(chance[0]) switch
                        {
                            'н' => (int)MutationChance.Low,
                            'с' => (int)MutationChance.Normal,
                            'в' => (int)MutationChance.High,
                            'о' => (int)MutationChance.Ultra,
                            _ => (int)MutationChance.Low,
                        })
                        .ToArray();
                    break;
                case "Время":
                    data[splited[0]] = splited
                        .Where(str => str.Contains(':'))
                        .Select(str =>
                        {
                            var spl = str.Split(':');
                            int seconds;
                            if (spl.Length > 2)
                                seconds = int.Parse(spl[0]) * 3600 + int.Parse(spl[1]) * 60 + int.Parse(spl[2]); //hh:mm:ss
                            else
                                seconds = int.Parse(spl[0]) * 60 + int.Parse(spl[1]);
                            return seconds;
                        })
                        .ToArray();
                    break;
                default:
                    data[splited[0]] = splited
                        .Skip(1)
                        .Select(gab => int.Parse(gab))
                        .ToArray();
                    break;
            }
        }

        return data;
    }

    public static string[] GetAmountDataString(string seedName)
    {
        var seedData = new string[] { };

        try
        {
            seedData = Resources
            .Load<TextAsset>($@"SeedStats\{seedName}")
            .text
            .Split('\n');
        }
        catch
        {
            try
            {                 //Вариант для скрещенных
                using (StreamReader reader = new StreamReader(Path.Combine(Application.persistentDataPath, seedName + ".csv")))
                {
                    seedData = reader.ReadToEnd().Split('\n');
                }
            }
            catch { return null; }
        }

        foreach (var line in seedData)
        {
            var splited = line
                .Split(',')
                .ToArray();

            if (splited[0].Contains("Кол"))
                return splited.Skip(1).Distinct().ToArray();
        }

        Debug.LogError("А где колличество? CSVReader 217");
        return null;
    }

    /// <summary>
    /// Converts array to dictionary, compressing equal elements
    /// </summary>
    /// <returns>dictionary with key = element, value = number of occurances</returns>
    private static Dictionary<T, int> ConvertToDict<T>(T[] mas)
    {
        var dict = new Dictionary<T, int>();
        for (var i = 0; i < mas.Length; i++)
        {
            var current = mas[i];
            while (i < mas.Length && mas[i].Equals(current))
            {
                i++;
            }
            dict[current] = i;
            i--;
        }
        return dict;
    }
}
