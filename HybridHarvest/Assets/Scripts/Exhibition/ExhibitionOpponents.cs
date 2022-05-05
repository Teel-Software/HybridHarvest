using System;
using System.Collections.Generic;
using System.Linq;
using Exhibition;
using UnityEngine;
using Random = System.Random;

public class ExhibitionOpponents : MonoBehaviour
{
    [SerializeField] private GameObject[] cards;
    [SerializeField] private Inventory inventory;
    
    private Opponent[] opponents;
    private List<Opponent> totalOpponents;

    private int opponentCount = 1;
    private double exhibitonDifficulty;


    public void Awake()
    {
        totalOpponents = new List<Opponent>
        {
            new Opponent("Тамара", "Tamara"),
            new Opponent("Лариса", "Larisa"),
            new Opponent("Серафима Ивановна", "OldLady"),
            new Opponent("Дед Максим", "OldMan"),
        };
        
        foreach (var card in cards)
            card.SetActive(false);
        
        var level = inventory.Level;
        if (level < 10)
            opponentCount = 1;
        if (level >= 10)
            opponentCount = 2;
        if (level >= 20)
            opponentCount = 3;
        opponents = new Opponent [opponentCount];

        exhibitonDifficulty = 5;
        var seedName = "Pea";
        var example = Resources.Load<Seed>($"Seeds\\{seedName}");
        example.SeedStats = CSVReader.GetSeedStats(seedName);
        //Seed.JsonTest(example);
        var points = example.ConvertToPoints();
        points *= 2;
        Debug.Log(points);

        var stats = example.SeedStats;
        var picker = new Random(Environment.TickCount);
        
        var randGrowTime = stats.GrowTime.ElementAt(picker.Next(stats.GrowTime.Values.Count)).Key;
        
        var randIndex = picker.Next(stats.MinAmount.Count);
        var randMinAmt = stats.MinAmount.ElementAt(randIndex).Key;
        var randMaxAmt = stats.MaxAmount.ElementAt(randIndex).Key;
        var avgAmt = (randMaxAmt + randMinAmt) / 2;
        var remainingPoints = points * randGrowTime / avgAmt;
        
        var maxRandMutation = (int)stats.MutationChance.Max(x => x.Key);
        var randMutationChance = picker.Next(maxRandMutation + 1);
        remainingPoints -= randMutationChance * Seed.MutationToPointsMultiplier;
        
        var randGabitus = picker.Next((int)remainingPoints);
        if (randGabitus == 0)
            randGabitus = 1;
        remainingPoints -= randGabitus;
        
        var randTaste = (int)remainingPoints;
        if (randTaste == 0)
            randTaste = 1;
        Debug.Log($"GrowTime {randGrowTime}; Amounts {randMinAmt}-{randMaxAmt}; Average {avgAmt}");
        Debug.Log($"Muta {randMutationChance}; Gabi {randGabitus}; Taste {randTaste}");
        var randSeed = Seed.Create(seedName, randTaste, randGabitus, randGrowTime, randMutationChance, randMinAmt, randMaxAmt);
        Debug.Log(randSeed);
        Debug.Log(randSeed.ConvertToPoints());
        
        var rand = new Random(Environment.TickCount);
        var unusedIndexes = Enumerable.Range(0, totalOpponents.Count).ToList();
        for (var i = 0; i < opponentCount; i++)
        {
            cards[i].SetActive(true);
            // Random generation without repetitions
            var index = rand.Next(0, unusedIndexes.Count);
            var opp = unusedIndexes[index];
            unusedIndexes.RemoveAt(index); 
            opponents[i] = totalOpponents[opp];
            var cardClass = cards[i].GetComponent<ExhibitionCard>();
            cardClass.SetOpponent(opponents[i], gameObject.transform.parent);
        }

    }

    public void funnyTest(object variable, string varName)
    {
        Debug.Log($"{varName} : {variable}");
    }
    
    public void OnEnable()
    {            

    }

    public void ChangeCount(int inc)
    {
        opponentCount = (opponentCount + inc) % 3;
        if (opponentCount < 0)
            opponentCount = 0;
    }
}