using System.Linq;
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
                    outStats.Gabitus = splited
                        .Skip(1)
                        .Select(gab => int.Parse(gab))
                        .ToArray();
                    break;
                case var str when str.Contains("Вкус"):
                    outStats.Taste = splited
                        .Skip(1)
                        .Select(taste => int.Parse(taste))
                        .ToArray();
                    break;
                case var str when str.Contains("Кол"):
                    outStats.MinAmount = splited
                        .Skip(1)
                        .Select(amount => int.Parse(amount.Split('-')[0]))
                        .ToArray();
                    outStats.MaxAmount = splited
                        .Skip(1)
                        .Select(amount => int.Parse(amount.Split('-')[1]))
                        .ToArray();
                    break;
                case var str when str.Contains("Мутац"):
                    outStats.MutationChance = splited
                        .Skip(1)
                        .Select(chance => char.ToLower(chance[0]) switch
                        {
                            'н' => "Low",
                            'с' => "Normal",
                            'в' => "High",
                            'о' => "Ultra",
                            _ => chance,
                        })
                        .ToArray();
                    break;
                case "Время":
                    outStats.GrowTime = splited
                        .Where(str => str.Contains(':'))
                        .Select(str =>
                        {
                            var spl = str.Split(':');
                            var seconds = int.Parse(spl[0]) * 60 + int.Parse(spl[1]);
                            return seconds;
                        })
                        .ToArray();
                    break;
            }
        }

        return outStats;
    }
}
