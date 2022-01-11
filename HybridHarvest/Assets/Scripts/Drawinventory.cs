using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System;

public class Drawinventory : MonoBehaviour
{
    [SerializeField] public Inventory targetInventory;
    // Место отрисовки
    [SerializeField] RectTransform Place;
    // Объект к которому привязан скрипт
    public GameObject InventoryGameObject;
    [SerializeField] GameObject ConfirmationPanel;
    [SerializeField] GameObject StatPanel;
    
    [SerializeField] Text FreeSpaceCounter;

    public Button GrowPlace { get; set; }
    public Action SuccessfulAddition;
    private string originalQuestionText; //?
    public PurposeOfDrawing Purpose;
    readonly List<GameObject> alreadyDrawn = new List<GameObject>();
    private bool changeItem = false;
    private Seed changingSeed;

    private void OnEnable()
    {
        UpdateActions();
        Redraw();
    }

    private void OnDisable()
    {
        if (changeItem) changeItem = false;
        gameObject.transform.Find("ChangeSeedPanel").gameObject.SetActive(false);
    }
    /// <summary>
    /// Reassigns all actions
    /// </summary>
    public void UpdateActions()
    {
        targetInventory.onItemAdded += Redraw;
        targetInventory.onInventoryFull += ChangeExistingItem;
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
    /// Filters inventory by name in russian
    /// </summary>
    public void FilterByName(string nameInRussian)
    {
        for (var i = 0; i < alreadyDrawn.Count; i++)
        {
            Destroy(alreadyDrawn[i]);
        }
        alreadyDrawn.Clear();

        for (var i = 0; i < targetInventory.Elements.Count; i++)
        {
            var item = targetInventory.Elements[i];

            if (item.NameInRussian != nameInRussian)
                continue;

            var icon = new GameObject(i.ToString(), typeof(Button));
            icon.AddComponent<Image>().sprite = item.PacketSprite;

            var plantIcon = new GameObject("Plant" + i);
            plantIcon.AddComponent<Image>().sprite = item.PlantSprite;
            plantIcon.transform.position = new Vector2(0, -35);
            plantIcon.transform.localScale = new Vector3(1.3f, 1.3f, 1.3f);
            plantIcon.transform.SetParent(icon.transform);

            icon.transform.localScale = new Vector2(0.01f, 0.01f);
            icon.GetComponent<Button>().onClick.AddListener(ClickedOnItem);
            icon.GetComponent<Button>().targetGraphic = icon.GetComponent<Image>();
            icon.transform.SetParent(Place);
            alreadyDrawn.Add(icon);
        }
    }

    /// <summary>
    /// Draws items in inventory
    /// </summary>
    public void Redraw()
    {
        for (var i = 0; i < alreadyDrawn.Count; i++)
        {
            Destroy(alreadyDrawn[i]);
        }
        alreadyDrawn.Clear();

        for (var i = 0; i < targetInventory.Elements.Count; i++)
        {
            var item = targetInventory.Elements[i];
            var icon = new GameObject(i.ToString(), typeof(Button));
            icon.AddComponent<Image>().sprite = item.PacketSprite;

            var plantIcon = new GameObject("Plant" + i);
            plantIcon.AddComponent<Image>().sprite = item.PlantSprite;
            plantIcon.transform.position = new Vector2(0, -35);
            plantIcon.transform.localScale = new Vector3(1.3f, 1.3f, 1.3f);
            plantIcon.transform.SetParent(icon.transform);

            icon.transform.localScale = new Vector2(0.01f, 0.01f);
            icon.GetComponent<Button>().onClick.AddListener(ClickedOnItem);
            icon.GetComponent<Button>().targetGraphic = icon.GetComponent<Image>();
            icon.transform.SetParent(Place);
            alreadyDrawn.Add(icon);
        }
        if (changeItem && targetInventory.Elements.Count < targetInventory.MaxItemsAmount)
        {
            var img = Resources.Load<Sprite>("seedsplus");
            var icon = new GameObject(targetInventory.Elements.Count.ToString(), typeof(Button));
            icon.transform.localScale = new Vector2(0.01f, 0.01f);
            icon.AddComponent<Image>().sprite = img;
            icon.transform.SetParent(Place);
            icon.GetComponent<Button>().onClick.AddListener(ClickedOnItem);
            alreadyDrawn.Add(icon);
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

        /*if (changeItem)
        {
            if (int.TryParse(item.name, out int index))
            {
                if (index == targetInventory.Elements.Count)
                {
                    targetInventory.Elements.Add(changingSeed);
                }
                else
                {
                    targetInventory.ChangeMoney(targetInventory.Elements[index].Price);
                    targetInventory.ChangeReputation(targetInventory.Elements[index].Gabitus);
                    targetInventory.Elements[index] = changingSeed;
                }
                changeItem = false;
                gameObject.SetActive(false);
                targetInventory.SaveAllData();
                SuccessfulAddition?.Invoke();
            }
            return;
        }*/

        PrepareConfirmation(item);
    }

    /// <summary>
    /// Вызывает панель подтверждения
    /// </summary>
    /// <param семечко="item"></param>
    private void PrepareConfirmation(GameObject item)
    {
        if (!int.TryParse(item.name, out var index)) return;
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
        logicScript.drawInventory = this;

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
                yesButton.onClick.AddListener(()=> { 
                    logicScript.ChangeItem(changingSeed);
                    changeItem = false;
                    gameObject.SetActive(false);
                    targetInventory.SaveAllData();
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
