using System;
using System.Collections.Generic;
using System.Linq;
using CI.QuickSave;
using UnityEngine;
using UnityEngine.UI;

public enum EnhancementType
{
    FarmSpot,
    Pot,
    InventorySpace
}

public class Enhancement
{
    public readonly string Name;
    public readonly EnhancementType Type;
    public readonly int Price;

    public Enhancement(string name, EnhancementType type, int price)
    {
        Name = name;
        Type = type;
        Price = price;
    }
}

public class EnhancementLogic : MonoBehaviour
{
    [SerializeField] private RectTransform placeForRender;
    [SerializeField] private GameObject buyButtonPrefab;
    [SerializeField] private GameObject placeholder;
    [SerializeField] private ConfirmationPanelLogic confirmationPanelPrefab;

    [SerializeField] private Sprite farmSpotSprite;
    [SerializeField] private Sprite potSprite;
    [SerializeField] private Sprite inventorySpaceSprite;

    private static List<Enhancement> availableEnhs { get; set; } = new List<Enhancement>();

    /// <summary>
    /// Разблокирует улучшения для покупки.
    /// </summary>
    public static void UnlockEnhancements(params Enhancement[] enhancements)
    {
        if (availableEnhs.Count == 0)
            Load();

        availableEnhs = availableEnhs
            .Concat(enhancements)
            .GroupBy(enh => enh.Name)
            .Select(g => g.First())
            .ToList();

        Save();
    }

    /// <summary>
    /// Блокирует все открытые улучшения.
    /// </summary>
    public static void ResetEnhancements()
    {
        var reader = QSReader.Create("PurchasedEnhancements");
        var writer = QuickSaveWriter.Create("PurchasedEnhancements");

        foreach (var key in reader.GetAllKeys())
            writer.Delete(key);

        writer.Commit();

        availableEnhs = new List<Enhancement>();
        Save();
    }

    private static void Save()
    {
        var writer = QuickSaveWriter.Create("Shop");
        writer.Write("AvailableEnhancements", availableEnhs);
        writer.Commit();
    }

    private static void Load()
    {
        var reader = QSReader.Create("Shop");
        availableEnhs = reader.Exists("AvailableEnhancements")
            ? reader.Read<List<Enhancement>>("AvailableEnhancements")
            : new List<Enhancement>();
    }

    /// <summary>
    /// Покупает улучшение.
    /// </summary>
    /// <param name="enh">Улучшение.</param>
    private static void BuyEnhancement(Enhancement enh)
    {
        var inventory = GameObject.FindGameObjectWithTag("Inventory").GetComponent<Inventory>();
        inventory.ChangeMoney(-enh.Price);

        if (enh.Type == EnhancementType.InventorySpace)
            inventory.MaxItemsAmount++;

        var writer = QuickSaveWriter.Create("PurchasedEnhancements");
        writer.Write(enh.Name, enh);
        writer.Commit();
    }

    /// <summary>
    /// Возвращает спрайт по типу улучшения.
    /// </summary>
    /// <param name="type">Тип улучшения.</param>
    private Sprite GetSpriteByType(EnhancementType type)
    {
        return type switch
        {
            EnhancementType.FarmSpot => farmSpotSprite,
            EnhancementType.Pot => potSprite,
            EnhancementType.InventorySpace => inventorySpaceSprite,
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, "Укажите корректный тип улучшения!")
        };
    }

    /// <summary>
    /// Отрисовывает доступные улучшения.
    /// </summary>
    private void Render()
    {
        ClearGameData.ClearChildren(placeForRender.gameObject);
        Load();

        foreach (var enh in availableEnhs)
        {
            var obj = Instantiate(buyButtonPrefab, placeForRender.transform, false);
            obj.GetComponent<Image>().sprite = GetSpriteByType(enh.Type);
            obj.name = $"Buy {enh.Name}";
            obj.GetComponent<Button>().onClick.AddListener(() =>
            {
                var canvas = GameObject.FindGameObjectWithTag("Canvas");
                var confPanel = Instantiate(confirmationPanelPrefab, canvas.transform, false);
                var title = enh.Type switch
                {
                    EnhancementType.Pot => "дополнительный горшок в лаборатории",
                    EnhancementType.FarmSpot => "дополнительную грядку на поле",
                    EnhancementType.InventorySpace => "дополнительное место в инвентаре",
                    _ => "улучшение"
                };

                confPanel.SetQuestion($"Купить {title}?", $"Стоимость: {enh.Price} <sprite=0>");
                confPanel.SetYesAction(() =>
                {
                    BuyEnhancement(enh);
                    availableEnhs = availableEnhs
                        .Where(e => e.Name != enh.Name)
                        .ToList();
                    Save();

                    Destroy(obj);
                });
            });
        }
    }

    private void OnEnable()
    {
        Render();
    }

    private void Update()
    {
        placeholder.SetActive(availableEnhs.Count == 0);
    }
}
