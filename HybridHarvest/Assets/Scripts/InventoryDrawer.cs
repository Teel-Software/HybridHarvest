using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using UnityEngine.Serialization;

public class InventoryDrawer : MonoBehaviour
{
    [SerializeField] public Inventory targetInventory;
    // Место отрисовки
    [FormerlySerializedAs("Place")]
    [SerializeField] private RectTransform scrollViewContent;
    // Префабы
    [SerializeField] private GameObject ConfirmationPanel;
    [SerializeField] private GameObject StatPanel;
    [SerializeField] private GameObject ItemIcon;

    [SerializeField] Text FreeSpaceCounter;

    public Button GrowPlace { get; set; }
    public Action SuccessfulAddition;
    public PurposeOfDrawing Purpose;
    readonly List<GameObject> alreadyDrawn = new List<GameObject>();
    private bool changeItem = false;
    private Seed changingSeed;

    private void OnEnable()
    {
        UpdateActions();
        Redraw();

        var scenario = GameObject.FindGameObjectWithTag("TutorialHandler")?.GetComponent<Scenario>();

        // тутор для добавления пакета семян
        if (QSReader.Create("TutorialState").Exists("Tutorial_BuyItem_Played"))
            scenario?.Tutorial_AddItem();

        // тутор для выбора пакета семян для замены
        else if (QSReader.Create("TutorialState").Exists("Tutorial_HarvestPlace_Played"))
            scenario?.Tutorial_ChooseItemToReplace();

        // тутор для инвентаря
        else if (QSReader.Create("TutorialState").Exists("Tutorial_BeginningChoice_Played"))
            scenario?.Tutorial_Inventory();
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
    /// <param name="filter_RussianName">Показывает только предметы с этим русским названием</param>
    public void Redraw(string filter_RussianName = null)
    {
        for (var i = 0; i < alreadyDrawn.Count; i++)
        {
            Destroy(alreadyDrawn[i]);
        }
        alreadyDrawn.Clear();

        for (var i = 0; i < targetInventory.Elements.Count; i++)
        {
            var seed = targetInventory.Elements[i];

            // фильтрует семена по русскому названию
            if (filter_RussianName != null
                && seed.NameInRussian != filter_RussianName)
                continue;
            /*
            var itemGameObj = new GameObject(i.ToString(), typeof(Button));
            itemGameObj.AddComponent<Image>().sprite = item.PacketSprite;

            var plantIcon = new GameObject("Plant" + i);
            plantIcon.AddComponent<Image>().sprite = item.PlantSprite;
            plantIcon.transform.position = new Vector2(0, -35);
            plantIcon.transform.localScale = new Vector3(1.3f, 1.3f, 1.3f);
            plantIcon.transform.SetParent(itemGameObj.transform);

            itemGameObj.transform.localScale = new Vector2(0.01f, 0.01f);
            itemGameObj.GetComponent<Button>().onClick.AddListener(ClickedOnItem);
            itemGameObj.GetComponent<Button>().targetGraphic = itemGameObj.GetComponent<Image>();
            itemGameObj.transform.SetParent(Place);
            */
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

                // тутор для выхода из лаборатории
                if (QSReader.Create("TutorialState").Exists("Tutorial_ReplaceOrAddItem_Played"))
                    scenario?.Tutorial_LabEnding();

                // тутор для выхода из магазина
                else if (QSReader.Create("TutorialState").Exists("Tutorial_AddItem_Played"))
                    scenario?.Tutorial_ShopExit();
            });
        }
        FreeSpaceCounter.text = $"{targetInventory.Elements.Count}/{targetInventory.MaxItemsAmount}";
    }

    /// <summary>
    /// Called when user clicks on item
    /// </summary>
    public void ClickedOnItem()
    {
        var item = EventSystem.current.currentSelectedGameObject;
        if (item == null) return;
        PrepareConfirmation(item);
    }

    /// <summary>
    /// Вызывает панель подтверждения
    /// </summary>
    /// <param семечко="item"></param>
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

        Text text;
        Button yesButton;
        ConfirmationPanelLogic logicScript;

        var needsStats = targetInventory.Elements.Count > index;
        if (needsStats)
        {
            var statPanelDrawer = Instantiate(StatPanel, GameObject.Find("Inventory").transform)
                .GetComponentInChildren<StatPanelDrawer>();
            statPanelDrawer.DisplayStats(targetInventory.Elements[index]);

            text = statPanelDrawer.ProceedButton.GetComponentInChildren<Text>();
            yesButton = statPanelDrawer.ProceedButton.GetComponent<Button>();
            logicScript = statPanelDrawer.ProceedButton.GetComponent<ConfirmationPanelLogic>();
        }
        else
        {
            var confPanelDrawer = Instantiate(ConfirmationPanel, GameObject.Find("Inventory").transform)
                .GetComponentInChildren<ConfirmationPanelDrawer>();
            text = confPanelDrawer.QuestionText;
            yesButton = confPanelDrawer.YesButton.GetComponent<Button>();
            logicScript = confPanelDrawer.YesButton.GetComponentInChildren<ConfirmationPanelLogic>();
        }

        logicScript.targetInventory = targetInventory;
        logicScript.inventoryDrawer = this;

        switch (Purpose)
        {
            case PurposeOfDrawing.Sell: // через кнопку инвентаря в боковом меню
                text.text = "Продать";
                yesButton.onClick.AddListener(logicScript.Sell);
                logicScript.HasPrice = true;
                break;
            case PurposeOfDrawing.Change: // вызывается из кода инвентаря
                if (index == targetInventory.Elements.Count)
                {
                    // в этот if не заходит из-за условия в самом начале
                    text.text = "Добавить";
                    if (!needsStats)
                    {
                        text.text += $" ?";
                    }
                }
                else
                {
                    text.text = "Заменить";
                }
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
            case PurposeOfDrawing.AddToLab: // через код на кнопке лаборатории
                text.text = "Выбрать";
                yesButton.onClick.AddListener(logicScript.Select);
                break;
            case PurposeOfDrawing.AddToExhibition: // через код на кнопке выставки
                text.text = "Отправить";
                yesButton.onClick.AddListener(logicScript.SendToExhibition);
                break;
        }

        yesButton.onClick.AddListener(() =>
        {
            var scenario = GameObject.FindGameObjectWithTag("TutorialHandler")?.GetComponent<Scenario>();

            // тутор для выхода из лаборатории
            if (QSReader.Create("TutorialState").Exists("Tutorial_ReplaceOrAddItem_Played"))
                scenario?.Tutorial_LabEnding();

            // тутор для окончания скрещивания
            else if (QSReader.Create("TutorialState").Exists("Tutorial_ApplyItemToCrossSecond_Played"))
                scenario?.Tutorial_ApplyCrossing();

            // тутор для активации кнопки скрещивания 2
            else if (QSReader.Create("TutorialState").Exists("Tutorial_ApplyItemToCrossFirst_Played"))
                scenario?.Tutorial_HybridPanelSecond();

            // тутор для захода на биржу
            else if (QSReader.Create("TutorialState").Exists("Tutorial_SellItem_Played"))
                scenario?.Tutorial_GoToMarket();

            // тутор для продажи урожая
            else if (QSReader.Create("TutorialState").Exists("Tutorial_ReplaceItem_Played"))
                scenario?.Tutorial_HarvestPlaceSellAll();

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
}

public enum PurposeOfDrawing
{
    Sell,
    Change,
    Plant,
    AddToLab,
    AddToExhibition
}
