using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class Drawinventory : MonoBehaviour
{
    [SerializeField] public Inventory targetInventory;
    [SerializeField] RectTransform Place;
    [SerializeField] public GameObject CurrentInventoryParent;
    [SerializeField] GameObject ConfirmationPanel;

    public Button GrowPlace { get; set; }

    private string originalQuestionText;
    readonly List<GameObject> alreadyDrawn = new List<GameObject>();
    private bool changeItem = false;
    private Seed changingSeed;

    private void Start()
    {
        UpdateActions();
        Redraw();
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
    }

    /// <summary>
    /// Called when user clicks on item
    /// </summary>
    public void ClickedOnItem()
    {
        var item = EventSystem.current.currentSelectedGameObject;
        if (item == null) return;

        if (changeItem)
        {
            // var it = Instantiate(ConfirmationPanel, GameObject.Find("Canvas").transform);
            //it.transform.Find("QuestionText").GetComponent<Text>().text = "Заменить?";
            if (int.TryParse(item.name, out int index))
            {
                targetInventory.ChangeMoney(targetInventory.Elements[index].Price);
                targetInventory.ChangeReputation(targetInventory.Elements[index].Gabitus);
                targetInventory.Elements[index] = changingSeed;
                Redraw();
                changeItem = false;
                gameObject.SetActive(false);
                targetInventory.SaveAllData();
            }
            return;
        }

        PrepareConfirmation(item);
    }

    /// <summary>
    /// Вызывает панель подтверждения
    /// </summary>
    /// <param семечко="item"></param>
    private void PrepareConfirmation(GameObject item)
    {
        var logic = ConfirmationPanel.GetComponentInChildren<ConfirmationPanelLogic>();
        logic.ItemObject = item;
        if (int.TryParse(item.name, out int index))
            logic.DefineItem(targetInventory.Elements[index].Name);

        ConfirmationPanel.SetActive(true);
    }

    /// <summary>
    /// Поднятие флага замены пакета
    /// </summary>
    /// <param name="newSeed"></param>
    private void ChangeExistingItem(Seed newSeed)
    {
        gameObject.SetActive(true);
        gameObject.transform.Find("ChangeSeedPanel").gameObject.SetActive(true);
        changeItem = true;
        changingSeed = newSeed;
    }
}
