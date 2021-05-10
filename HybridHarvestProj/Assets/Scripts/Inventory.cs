using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    [SerializeField] public List<Seed> Elements = new List<Seed>();
    [SerializeField] public Text Info;
    private const int Devider = 5;
    public Action onItemAdded;
    public int Money { get; private set; }
    public int Reputation { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        RedrawInfo();
    }

    public void AddItem(Seed newSeed)
    {
        Elements.Add(newSeed);
        onItemAdded?.Invoke();
    }

    public void ChangeMoney(int changingAmount)
    {
        Money += changingAmount > 0
            ? changingAmount / Devider
            : changingAmount;
        if (Money <= -100 && changingAmount < 0
            || changingAmount > 0)
            Reputation += changingAmount / Devider;
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
        Info.text = $"Деньги: {Money}   Репутация: {Reputation}";
    }
}
