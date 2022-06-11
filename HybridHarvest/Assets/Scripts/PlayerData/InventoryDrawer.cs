using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;
using System;

public enum PurposeOfDrawing
{
    Sell,
    Change,
    Plant,
    AddToLab,
    AddToExhibition,
    AddToQuant
}

public class InventoryDrawer : MonoBehaviour
{
    [SerializeField] public Inventory targetInventory;

    // Место отрисовки
    [SerializeField] private RectTransform scrollViewContent;

    // Префабы
    [SerializeField] private GameObject StatPanel;
    [SerializeField] private GameObject ItemIcon;

    [SerializeField] private Text FreeSpaceCounter;

    public Button GrowPlace { get; set; }
    public Action SuccessfulAddition;
    public PurposeOfDrawing Purpose;
    private readonly List<GameObject> alreadyDrawn = new List<GameObject>();
    private bool changeItem;
    private Seed changingSeed;

    private void OnEnable()
    {
        UpdateActions();
        Redraw();

        var scenario = GameObject.FindGameObjectWithTag("TutorialHandler")?.GetComponent<Scenario>();
        if (scenario is null)
            return;

        // тутор для выбора пакета семян для замены
        if (QSReader.Create("TutorialState").Exists("Tutorial_HarvestPlace_Played"))
            scenario.Tutorial_ChooseItemToReplace();

        // тутор для выбора пакета семян для посадки
        else if (QSReader.Create("TutorialState").Exists("Tutorial_GoToField_Played"))
            scenario.Tutorial_ChooseItemToPlant();

        // тутор для добавления пакета семян
        else if (QSReader.Create("TutorialState").Exists("Tutorial_BuyItem_Played"))
            scenario.Tutorial_AddItem();
    }

    private void OnDisable()
    {
        targetInventory.Save();
        if (changeItem) changeItem = false;
        gameObject.transform.Find("ChangeSeedPanel").gameObject.SetActive(false);
    }

    public void ToggleGameObject(bool enableIt)
    {
        gameObject.SetActive(enableIt);
    }

    /// <summary>
    /// Reassigns all actions
    /// </summary>
    public void UpdateActions()
    {
        targetInventory.onItemAdded = Redraw;
        targetInventory.onInventoryFull = ChangeExistingItem;
    }

    /// <summary>
    /// Устанавливает цель вызова инвентаря по числу
    /// </summary>
    /// <param name="purpose"></param>
    public void SetPurpose(int purpose)
    {
        Purpose = (PurposeOfDrawing)purpose;
    }

    /// <summary>
    /// Устанавливает цель вызова инвентаря по enum
    /// </summary>
    /// <param name="purpose"></param>
    public void SetPurpose(PurposeOfDrawing purpose)
    {
        Purpose = purpose;
    }

    /// <summary>
    /// Отрисовывает инвентарь
    /// </summary>
    /// <param name="filterParents">Показывает только предметы с этими родителями</param>
    public void Redraw(List<string> filterParents = null)
    {
        for (var i = 0; i < alreadyDrawn.Count; i++)
            Destroy(alreadyDrawn[i]);

        alreadyDrawn.Clear();

        var exhibition = FindObjectOfType<Exhibition.Exhibition>();
        for (var i = 0; i < targetInventory.Elements.Count; i++)
        {
            var seed = targetInventory.Elements[i];

            // фильтрует семена по родителям для лаборатории
            if (Purpose == PurposeOfDrawing.AddToLab
                && filterParents != null
                && !seed.Parents.SequenceEqual(filterParents))
                continue;

            // фильтрует семена по родителям для кванта
            if (Purpose == PurposeOfDrawing.AddToQuant
                && filterParents != null
                && seed.Parents.Concat(filterParents).Distinct().Count() > 5)
                continue;

            if (Purpose == PurposeOfDrawing.AddToExhibition
                && exhibition.PlayerSeeds.Contains(seed))
            {
                continue;
            }

            var itemIcon = Instantiate(ItemIcon, scrollViewContent);
            itemIcon.name = i.ToString();
            var itemIconDrawer = itemIcon.GetComponent<ItemIconDrawer>();
            itemIconDrawer.SetSeed(seed);
            itemIconDrawer.Button.onClick.AddListener(ClickedOnItem);
            itemIcon.tag = seed.NameInRussian switch
            {
                "Обучающий картофель" => "TutorialPotato",
                "Обучающий помидор" => "TutorialTomato",
                _ => "Seed"
            };
            alreadyDrawn.Add(itemIcon);
        }

        if (changeItem && targetInventory.Elements.Count < targetInventory.MaxItemsAmount)
        {
            var itemIcon = Instantiate(ItemIcon, scrollViewContent);
            itemIcon.name = targetInventory.Elements.Count.ToString();
            var itemIconDrawer = itemIcon.GetComponent<ItemIconDrawer>();
            itemIconDrawer.SetPlus();
            itemIconDrawer.Button.onClick.AddListener(ClickedOnItem);
            itemIcon.tag = "InventoryPlusBtn";
            alreadyDrawn.Add(itemIcon);

            itemIconDrawer.Button.onClick.AddListener(() =>
            {
                var scenario = GameObject.FindGameObjectWithTag("TutorialHandler")?.GetComponent<Scenario>();
                if (scenario is null) return;

                // тутор для выхода из лаборатории
                if (QSReader.Create("TutorialState").Exists("Tutorial_ReplaceOrAddItem_Played"))
                    scenario.Tutorial_LabEnding();

                // тутор для выхода из магазина
                else if (QSReader.Create("TutorialState").Exists("Tutorial_AddItem_Played"))
                    scenario.Tutorial_ShopExit();
            });
        }
    }

    /// <summary>
    /// Called when user clicks on item
    /// </summary>
    private void ClickedOnItem()
    {
        var item = EventSystem.current.currentSelectedGameObject;
        if (item is null)
            return;

        PrepareConfirmation(item);
    }

    /// <summary>
    /// Вызывает панель подтверждения.
    /// </summary>
    /// <param name="item">Семечко.</param>
    private void PrepareConfirmation(GameObject item)
    {
        if (!int.TryParse(item.name, out var index)) return;

        if (Purpose == PurposeOfDrawing.Change && index == targetInventory.Elements.Count) // get rekt part 1
        {
            targetInventory.Elements.Add(changingSeed);
            Redraw();

            changeItem = false;
            gameObject.SetActive(false);
            targetInventory.Save();
            SuccessfulAddition?.Invoke();
            return;
        }

        var statPanelDrawer = Instantiate(StatPanel, GameObject.Find("Inventory").transform)
            .GetComponentInChildren<StatPanelDrawer>();
        statPanelDrawer.DisplayStats(targetInventory.Elements[index]);

        var text = statPanelDrawer.ProceedButton.GetComponentInChildren<Text>();
        var yesButton = statPanelDrawer.ProceedButton.GetComponent<Button>();
        var logicScript = statPanelDrawer.ProceedButton.GetComponent<ConfirmationPanelLogic>();

        logicScript.targetInventory = targetInventory;
        logicScript.inventoryDrawer = this;

        switch (Purpose)
        {
            case PurposeOfDrawing.Sell: // через кнопку инвентаря в боковом меню
                text.text = "Продать";
                yesButton.onClick.AddListener(logicScript.Sell);
                break;
            case PurposeOfDrawing.Change: // вызывается из кода инвентаря
                text.text = "Заменить";
                yesButton.onClick.AddListener(() =>
                {
                    logicScript.ChangeItem(changingSeed);
                    changeItem = false;
                    gameObject.SetActive(false);
                    targetInventory.Save();
                    SuccessfulAddition?.Invoke();
                });
                break;
            case PurposeOfDrawing.Plant: // через код на грядке
                text.text = "Посадить";
                yesButton.onClick.AddListener(logicScript.Plant);
                break;
            case PurposeOfDrawing.AddToLab: // через код labButton
                text.text = "Выбрать";
                yesButton.onClick.AddListener(logicScript.Select);
                break;
            case PurposeOfDrawing.AddToExhibition: // через код на кнопке выставки
                text.text = "Отправить";
                yesButton.onClick.AddListener(logicScript.SendToExhibition);
                break;
            case PurposeOfDrawing.AddToQuant: // через код labButton
                text.text = "Выбрать";
                yesButton.onClick.AddListener(logicScript.Select);
                break;
        }

        yesButton.onClick.AddListener(() =>
        {
            var scenario = GameObject.FindGameObjectWithTag("TutorialHandler")?.GetComponent<Scenario>();
            if (scenario is null)
                return;

            // тутор для выхода из лаборатории
            if (QSReader.Create("TutorialState").Exists("Tutorial_ReplaceOrAddItem_Played"))
                scenario.Tutorial_LabEnding();

            // тутор для окончания скрещивания
            else if (QSReader.Create("TutorialState").Exists("Tutorial_ApplyItemToCrossSecond_Played"))
                scenario.Tutorial_ApplyCrossing();

            // тутор для активации кнопки скрещивания 2
            else if (QSReader.Create("TutorialState").Exists("Tutorial_ApplyItemToCrossFirst_Played"))
                scenario.Tutorial_HybridPanelSecond();

            // тутор для захода на биржу
            else if (QSReader.Create("TutorialState").Exists("Tutorial_SellItem_Played"))
                scenario.Tutorial_GoToMarket();

            // тутор для выбора урожая
            else if (QSReader.Create("TutorialState").Exists("Tutorial_ReplaceItem_Played"))
                scenario.Tutorial_HarvestPlaceChoseAll();

            // тутор для ускорения роста
            else if (QSReader.Create("TutorialState").Exists("Tutorial_PlantItem_Played"))
                scenario.Tutorial_ChooseItemToSpeedUp();
        });
        logicScript.ItemObject = item;

        //if (int.TryParse(item.name, out var index) && targetInventory.Elements.Count > index)
        //    logicScript.DefineItem(targetInventory.Elements[index]);

        //panelObj.SetActive(true);
    }

    /// <summary>
    /// Поднятие флага замены пакета
    /// </summary>
    /// <param name="newSeed"></param>
    private void ChangeExistingItem(Seed newSeed)
    {
        changeItem = true;
        gameObject.SetActive(true);
        //gameObject.transform.Find("ChangeSeedPanel").gameObject.SetActive(true);
        changingSeed = newSeed;
    }

    private void Update()
    {
        FreeSpaceCounter.text = $"{targetInventory.Elements.Count}/{targetInventory.MaxItemsAmount}";
    }
}