﻿using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public static class CSVReader
{
    public static SeedStatistics ParseSeedStats(string seedName)
    {
        var outStats = new SeedStatistics();
        var seedData = Resources
            .Load<TextAsset>($@"SeedStats\{seedName}")
            .text
            .Split('\n');

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
                            var seconds = int.Parse(spl[0]) * 60 + int.Parse(spl[1]);
                            return seconds;
                        })
                        .ToArray());
                    break;
            }
        }

        return outStats;
    }

    private static Dictionary<T, int> ConvertToDict<T>(T[] mas)
    {
        var dict = new Dictionary<T, int>();
        for(var i=0; i<mas.Length; i++)
        {
            var current = mas[i];
            while(i < mas.Length && mas[i].Equals(current))
            {
                i++;
            }
            dict[current] = i;
            i--;
        }
        return dict;
    }
}
