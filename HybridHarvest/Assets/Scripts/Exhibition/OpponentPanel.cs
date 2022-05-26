using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

namespace Exhibition
{
    public class OpponentPanel : MonoBehaviour
    {
        [SerializeField] private GameObject[] cards;
        [SerializeField] private Inventory inventory;
    
        private Opponent[] opponents;
        private List<Opponent> totalOpponents;

        private int opponentCount;
        private int debugOpponentCount;
        private double exhibitonDifficulty;

        public void Awake()
        {
            totalOpponents = new List<Opponent>
            {
                new Opponent("Тамара", "Tamara"),
                new Opponent("Лариса", "Larisa"),
                new Opponent("Серафима Ивановна", "OldLady"),
                new Opponent("Дед Максим", "OldMan"),
                new Opponent("Алиса", "Alisa"),
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
            
            if (debugOpponentCount > 1)
                opponentCount = debugOpponentCount;
            
            opponents = new Opponent[opponentCount];
            
            var seedNames = Resources.LoadAll<Seed>("Seeds")
                .Select(x => x.Name)
                .Where(x => x != "Debug")
                .ToList();
            
            exhibitonDifficulty = 1;
            var example = Seed.LoadFromResources("Cucumber");
            var points = example.ConvertToPoints() * exhibitonDifficulty;

            var rand = new Random(Environment.TickCount);
            var seedCount = GetComponentInParent<Exhibition>().SeedCount;
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
            
                var exhibitionCard = cards[i].GetComponent<OpponentCard>();
                exhibitionCard.SetOpponent(opponents[i], gameObject.transform.parent);
            }
        }
    
        public void OnEnable()
        {
            gameObject.SetActive(GetComponentInParent<Exhibition>().State == ExhibitionState.Inactive);
        }
        
        public void ChangeCount(int inc)
        {
            debugOpponentCount = Math.Min(debugOpponentCount + inc, 3);
            debugOpponentCount = Math.Max(debugOpponentCount, 1);
        }
    }
}