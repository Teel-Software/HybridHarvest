using UnityEngine;
using UnityEngine.UI;

public class InitEnergy : MonoBehaviour
{
    [SerializeField] public GameObject EnergyWindowPrefab;
    private GameObject currentMenu;

    /// <summary>
    /// Спавнит префаб энергии, либо активирует существующий.
    /// </summary>
    public void OpenEnergyMenu()
    {
        if (currentMenu == null)
        {
            var canvas = GameObject.FindGameObjectWithTag("Canvas");
            currentMenu = Instantiate(EnergyWindowPrefab, canvas.transform, false);

            var inventory = GameObject.FindGameObjectWithTag("Inventory").GetComponent<Inventory>();
            inventory.EnergyRegenTime = GameObject.Find("RegenTime")?.GetComponent<Text>();
        }
        else
        {
            currentMenu.SetActive(true);
        }
    }

    public void DebugResetEnergy()
    {
        var inventory = GameObject.FindGameObjectWithTag("Inventory").GetComponent<Inventory>();
        inventory.ConsumeEnergy(inventory.Energy);
    }

    public void RegenEnergy()
    {
        var inventory = GameObject.FindGameObjectWithTag("Inventory").GetComponent<Inventory>();
        inventory.RegenEnergy(1);
    }
}
