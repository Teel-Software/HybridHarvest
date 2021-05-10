using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    public Action onItemAdded;
    [SerializeField] public List<Seed> Elements = new List<Seed>();
    [SerializeField] public Text Info;
    public int Money { get; private set; }
    public int Reputation { get; private set; }
    // Start is called before the first frame update
    void Start()
    {
        RedrawInfo();
    }

    public void AddItem(Seed newSeed) {
        Elements.Add(newSeed);
        onItemAdded?.Invoke();
    }

     public void ChangeMoney(int changingAmount)
    {
        //Money += changingAmount;
        if (Money < -100)
        {
            if (changingAmount < 0)
                Reputation -= 5;
            else
                Reputation += 5;
        }
        Money += changingAmount;
        RedrawInfo();
    }
     public void ChangeReputation(int changingAmount)
    {
        Reputation += changingAmount;
        RedrawInfo();
    }

    public void RemoveItem(int index)
    {
        Elements.RemoveAt(index);
    }

    void RedrawInfo()
    {
        Info.text = "Money: " + Money + " | " + "Reputation: " + Reputation;
    }
}
