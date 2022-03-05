using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public static class SeedMutator
{
    public static Seed GetMutatedSeed(Seed oldSeed)
    {
        var changingStatsAmount = getAmountOfChangingStats(oldSeed.MutationPossibility);

        (bool[] index, int alreadyFull) = GetStatsFullness(oldSeed);
        var t = -1;
        while (changingStatsAmount > 0 && alreadyFull < 5)
        {
            t = (int)Math.Round((UnityEngine.Random.value * 100) % 4);
            if (!index[t])
            {
                changingStatsAmount--;
                alreadyFull++;
                index[t] = true;
            }
        }

        var newStats = MutateStats(oldSeed, index);
        var newSeed = ScriptableObject.CreateInstance<Seed>();

        try
        {
            newSeed.SetValues(oldSeed.ToString());
            newSeed.Gabitus = newStats[0];
            newSeed.Taste = newStats[1];
            newSeed.GrowTime = newStats[2];
            newSeed.minAmount = newStats[3];
            newSeed.MutationPossibility = (MutationChance)newStats[4];
            newSeed.maxAmount = newStats[5];
        }
        catch
        {
            Debug.Log($"В мутации ошибка -_- Семечко \"{oldSeed.Name}\".");
        }

        return newSeed;
    }

    private static int getAmountOfChangingStats(MutationChance basicMutation)
    {
        var mutation = (int)basicMutation;
        var percentage = UnityEngine.Random.value;
        if (percentage <= 0.2)
            return 2 + mutation;
        if (percentage <= 0.4)
            return 1 + mutation;
        if (percentage <= 0.6)
            return mutation;
        if (percentage <= 0.6)
            return (mutation - 1) > 0 ? mutation - 1 : 0;
        if (percentage <= 0.8)
            return (mutation - 2) > 0 ? mutation - 2 : 0;
        return 0;
    }

    private static int[] MutateStats(Seed oldSeed, bool[] index)
    {
        Tuple<int, int[]>[] statsData = {
        Tuple.Create(oldSeed.Gabitus, oldSeed.LevelData.Gabitus.Keys.ToArray()),
        Tuple.Create(oldSeed.Taste, oldSeed.LevelData.Taste.Keys.ToArray()),
        Tuple.Create(oldSeed.GrowTime, oldSeed.LevelData.GrowTime.Keys.ToArray()),
        Tuple.Create(oldSeed.minAmount, oldSeed.LevelData.MinAmount.Keys.ToArray()),
        Tuple.Create((int)oldSeed.MutationPossibility, oldSeed.LevelData.MutationChance.Keys.Select(x => (int)x).ToArray()),
        Tuple.Create(oldSeed.maxAmount, oldSeed.LevelData.MaxAmount.Keys.ToArray())
        };
        List<int> stats = new List<int>();
        for (var i = 0; i < statsData.Length - 1; i++)
        {
            if (index[i] && Array.IndexOf(statsData[i].Item2, statsData[i].Item1) + 1 < statsData[i].Item2.Length)
            {
                stats.Add(statsData[i].Item2[Array.IndexOf(statsData[i].Item2, statsData[i].Item1) + 1]);
            }
            else
                stats.Add(statsData[i].Item1);
        }

        try
        {
            if (stats[3] != oldSeed.minAmount)
                stats.Add(statsData.Last().Item2[Array.IndexOf(statsData.Last().Item2, statsData.Last().Item1) + 1]);
            else
                stats.Add(statsData.Last().Item1);
        }
        catch
        {
            Debug.Log($"В мутации ошибка -_- Семечко \"{oldSeed.Name}\".");
        }

        return stats.ToArray();
    }

    private static (bool[], int) GetStatsFullness(Seed oldSeed)
    {
        Tuple<int, int[]>[] statsData = {
        Tuple.Create(oldSeed.Gabitus, oldSeed.LevelData.Gabitus.Keys.ToArray()),
        Tuple.Create(oldSeed.Taste, oldSeed.LevelData.Taste.Keys.ToArray()),
        Tuple.Create(oldSeed.GrowTime, oldSeed.LevelData.GrowTime.Keys.ToArray()),
        Tuple.Create(oldSeed.minAmount, oldSeed.LevelData.MinAmount.Keys.ToArray()),
        Tuple.Create((int)oldSeed.MutationPossibility, oldSeed.LevelData.MutationChance.Keys.Select(x => (int)x).ToArray()),
        };
        bool[] index = new bool[5];
        var amount = 0;
        for (var i = 0; i < statsData.Length - 1; i++)
        {
            if (statsData[i].Item1 == statsData[i].Item2.Last())
            {
                index[i] = true;
                amount++;
            }
        }
        return (index, amount);
    }
}
