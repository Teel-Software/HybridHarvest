using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateAllSeeds : MonoBehaviour
{
    [SerializeField] GeneCrossing script;
    // Start is called before the first frame update
    public void Maketh()
    {
        var seeds = new List<Seed>();
        seeds.Add((Seed)Resources.Load("Seeds\\Cucumber"));
        seeds.Add((Seed)Resources.Load("Seeds\\Carrot"));
        seeds.Add((Seed)Resources.Load("Seeds\\Tomato"));
        seeds.Add((Seed)Resources.Load("Seeds\\Potato"));
        seeds.Add((Seed)Resources.Load("Seeds\\Pea"));

        var firstGen = getCrossed(seeds);
        var secondGen = getCrossed(firstGen);
        var thirdGen = getCrossed(secondGen);
        var forthGen = getCrossed(thirdGen);
        Debug.Log(forthGen.Count);
    }

    private List<Seed> getCrossed(List<Seed> parents)
    {
        var gen = new List<Seed>();
        for (int i = 0; i < parents.Count; i++)
        {
            for (int k = 0; k < parents.Count; k++)
            {
                if (i == k) continue;
                gen.Add(script.GetQuantumSeed(parents[i], parents[k]));
            }
        }
        return gen;
    } 
}
