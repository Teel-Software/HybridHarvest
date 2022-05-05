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
        opponents = new Opponent[opponentCount];

        exhibitonDifficulty = 5;
        var seedNames = Resources.LoadAll<Seed>("Seeds")
            .Select(x => x.Name)
            .Where(x => x != "Debug")
            .ToList();
        var example = Seed.LoadFromResources("Cucumber");
        var points = example.ConvertToPoints();

        var rand = new Random(Environment.TickCount);
        var seedCount = rand.Next(3) + 1;
        var unusedIndexes = Enumerable.Range(0, totalOpponents.Count).ToList();
        for (var i = 0; i < opponentCount; i++)
        {
            cards[i].SetActive(true);
            // Random generation without repetitions
            var index = rand.Next(0, unusedIndexes.Count);
            var baseOpponent = totalOpponents[unusedIndexes[index]];
            unusedIndexes.RemoveAt(index); 

            var seeds = new List<Seed>();
            for (var j = 0; j < seedCount; j++)
            {
                var seedName = seedNames[rand.Next(seedNames.Count)];
                var seed = Seed.CreateRandom(seedName, points);
                seeds.Add(seed);
            }

            opponents[i] = new Opponent(baseOpponent.Name, baseOpponent.SpriteName, seeds);
            
            var exhibitionCard = cards[i].GetComponent<ExhibitionCard>();
            exhibitionCard.SetOpponent(opponents[i], gameObject.transform.parent);
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