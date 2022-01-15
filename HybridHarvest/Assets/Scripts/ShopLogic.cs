using System;
using System.Collections.Generic;
using CI.QuickSave;
using UnityEngine;
using UnityEngine.UI;

public class ShopLogic : MonoBehaviour, ISaveable
{
    [SerializeField] public InventoryDrawer inventoryDrawer;
    public Inventory targetInventory;
    public GameObject StatPanel;

    public List<string> unlockedSeeds { get; private set; }
    private void Awake()
    {
        targetInventory ??= GameObject.FindGameObjectWithTag("Inventory").GetComponent<Inventory>();
        Load();
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
            Save();
    }

    private void OnDisable()
    {
        Save();
    }

    /// <summary>
    /// Создаёт подтверждающую панель в магазине
    /// </summary>
    /// <param name="seed">Растение класса Seed</param>
    public void PrepareConfirmation(Seed seed)
    {
        var statPanelDrawer = Instantiate(StatPanel, GameObject.Find("Shop").transform)
            .GetComponentInChildren<StatPanelDrawer>();
        statPanelDrawer.DisplayStats(seed);
            
        var text = statPanelDrawer.ProceedButton.GetComponentInChildren<Text>();
        var yesButton = statPanelDrawer.ProceedButton.GetComponent<Button>();
        var logicScript = statPanelDrawer.ProceedButton.GetComponent<ConfirmationPanelLogic>();
        
        text.text = "Купить";
        logicScript.inventoryDrawer = inventoryDrawer;
        yesButton.onClick.AddListener(() => logicScript.Buy(seed));
        logicScript.targetInventory = targetInventory;
        logicScript.HasPrice = true;
    }

    public void Save()
    {
        var writer = QuickSaveWriter.Create("Shop");
        writer.Write("UnlockedSeeds", unlockedSeeds);
        writer.Commit();
    }

    public void Load()
    {
        var reader = QSReader.Create("Shop");
        if (reader.Exists("UnlockedSeeds"))
            unlockedSeeds = reader.Read<List<string>>("UnlockedSeeds");
        else
            unlockedSeeds = new List<string>{ "Potato", "Tomato", "Cucumber", "Pea", "Debug" };
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
