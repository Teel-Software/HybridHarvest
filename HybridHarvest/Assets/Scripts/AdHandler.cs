using System;
using UnityEngine;

public class AdHandler : MonoBehaviour
{
    private Inventory _inventory;

    private void Start()
    {
        _inventory = GameObject.Find("DataKeeper").GetComponent<Inventory>();
    }

    public void UpdateEnergy()
    {
        _inventory.RegenEnergy(1);
    }

    public void DebugResetEnergy()
    {
        _inventory.ConsumeEnergy(_inventory.Energy);
    }
}