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
    private List<Opponent> possibleOpponents;

    private int opponentCount = 1;
    public void Awake()
    {
        possibleOpponents = new List<Opponent>
        {
            new Opponent("Тамара", "Tamara"),
            new Opponent("Лариса", "Larisa"),
            new Opponent("Серафима Ивановна", "OldLady"),
            new Opponent("Дед Максим", "OldMan"),
        };
        
        foreach (var card in cards)
            card.SetActive(false);
        
        var level = inventory.Reputation;
        //Level 10- => 1 opponent, 10+ => 2, 20+ => 3
        //opponentCount = level / 10 + 1;
        var count = opponentCount;
        opponents = new Opponent [count];
        
        var rand = new Random(DateTime.Now.Millisecond);
        var possible = Enumerable.Range(0, possibleOpponents.Count).ToList();
        for (var i = 0; i < count; i++)
        {
            cards[i].SetActive(true);
            // Random generation without repetitions
            var index = rand.Next(0, possible.Count);
            var opp = possible[index];
            possible.RemoveAt(index);
            opponents[i] = possibleOpponents[opp];
            var cardClass = cards[i].GetComponent<ExhibitionCard>();
            cardClass.SetOpponent(opponents[i], gameObject.transform.parent);
        }

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