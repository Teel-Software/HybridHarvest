using System.Collections.Generic;
using System.Linq;
using CI.QuickSave;
using UnityEngine;
using UnityEngine.UI;

public class ShopLogic : MonoBehaviour, ISaveable
{
    [SerializeField] public InventoryDrawer inventoryDrawer;
    public Inventory targetInventory;
    public GameObject StatPanel;

    [SerializeField] private RectTransform shoppingPlace;
    [SerializeField] private GameObject ItemIcon;

    private static List<string> unlockedSeeds { get; set; } = new List<string>();

    /// <summary>
    /// Разблокирует новые семена.
    /// </summary>
    /// <param name="seedNames">Названия семян на английском.</param>
    public static void UnlockSeeds(params string[] seedNames)
    {
        if (unlockedSeeds.Count == 0)
            LoadStatic();

        unlockedSeeds = unlockedSeeds
            .Concat(seedNames)
            .Distinct()
            .ToList();
        SaveStatic();
    }

    public void Save()
    {
        SaveStatic();
    }

    public void Load()
    {
        LoadStatic();
    }

    public void CSVTest()
    {
        var stats = CSVReader.GetSeedStats("Peas");
        Debug.Log(string.Join(" ", stats.Gabitus));
        Debug.Log(string.Join(" ", stats.Taste));
        Debug.Log(string.Join(" ", stats.MinAmount));
        Debug.Log(string.Join(" ", stats.MaxAmount));
        Debug.Log(string.Join(" ", stats.MutationChance));
        Debug.Log(string.Join(" ", stats.GrowTime));
    }

    private static void SaveStatic()
    {
        var writer = QuickSaveWriter.Create("Shop");
        writer.Write("UnlockedSeeds", unlockedSeeds);
        writer.Commit();
    }

    public static void LoadStatic()
    {
        var reader = QSReader.Create("Shop");
        unlockedSeeds = reader.Exists("UnlockedSeeds")
            ? reader.Read<List<string>>("UnlockedSeeds")
            : new List<string> { "Cucumber" };
    }

    /// <summary>
    /// Создаёт подтверждающую панель в магазине
    /// </summary>
    /// <param name="seed">Растение класса Seed</param>
    private void PrepareConfirmation(Seed seed)
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
    }

    private void Awake()
    {
        targetInventory ??= GameObject.FindGameObjectWithTag("Inventory").GetComponent<Inventory>();
        Load();
    }

    private void OnEnable()
    {
        var shopLogic = GetComponent<ShopLogic>();
        shopLogic.Awake();
        foreach (var seedName in unlockedSeeds)
        {
            var itemIcon = Instantiate(ItemIcon, shoppingPlace.transform);
            itemIcon.name = $"Buy{seedName}";
            itemIcon.transform.localScale = new Vector3(0.9f, 0.9f);
            var itemIconDrawer = itemIcon.GetComponent<ItemIconDrawer>();
            // покупать можно нормально заданные семена
            // хотя через ресурсы тоже вроде норм
            var seed = (Seed) Resources.Load("Seeds\\" + seedName);
            itemIconDrawer.SetSeed(seed);
            itemIconDrawer.Button.onClick.AddListener(() => shopLogic.PrepareConfirmation(seed));
        }
    }

    private void OnDisable()
    {
        foreach (Transform child in shoppingPlace.transform)
            Destroy(child.gameObject);
        Save();
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus) Save();
    }
}
