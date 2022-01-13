using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopLogic : MonoBehaviour
{
    public Inventory targetInventory;
    public GameObject StatPanel;

    public List<string> unlockedSeeds { get; private set; }

    private void Awake()
    {
        targetInventory ??= GameObject.FindGameObjectWithTag("Inventory").GetComponent<Inventory>();
        // loading unlocked seeds from file at some point
        unlockedSeeds = new List<string>{ "Potato", "Tomato", "Cucumber", "Debug", };
    }

    /// <summary>
    /// Создаёт подтверждающую панель в магазине
    /// </summary>
    /// <param name="seedName">Имя растения</param>
    public void PrepareConfirmation(string seedName)
    {
        var statPanelDrawer = Instantiate(StatPanel, GameObject.Find("Shop").transform)
            .GetComponentInChildren<StatPanelDrawer>();
        var seed = (Seed)Resources.Load("Seeds\\" + seedName);
        statPanelDrawer.DisplayStats(seed);
            
        var text = statPanelDrawer.ProceedButton.GetComponentInChildren<Text>();
        var yesButton = statPanelDrawer.ProceedButton.GetComponent<Button>();
        var logicScript = statPanelDrawer.ProceedButton.GetComponent<ConfirmationPanelLogic>();
        
        text.text = "Купить";
        yesButton.onClick.AddListener(() => logicScript.Buy(seed));
        logicScript.targetInventory = targetInventory;
        logicScript.HasPrice = true;
    }

    public void CSVTest()
    {
        var stats = CSVReader.ParseSeedStats("Peas");
        Debug.Log(string.Join(" ", stats.Gabitus));
        Debug.Log(string.Join(" ", stats.Taste));
        Debug.Log(string.Join(" ", stats.MinAmount));
        Debug.Log(string.Join(" ", stats.MaxAmount));
        Debug.Log(string.Join(" ", stats.MutationChance));
        Debug.Log(string.Join(" ", stats.GrowTime));
    }
}
