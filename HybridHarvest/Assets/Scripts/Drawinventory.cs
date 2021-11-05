using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;

public class Drawinventory : MonoBehaviour
{
    [SerializeField] public Inventory targetInventory;
    [SerializeField] RectTransform Place;
    [SerializeField] public GameObject CurrentInventoryParent;
    [SerializeField] GameObject ConfirmationPanel;

    public Button GrowPlace { get; set; }

    private string originalQuestionText;
    readonly List<GameObject> alreadyDrawn = new List<GameObject>();

    void Start()
    {
        targetInventory.onItemAdded += Redraw;
        Redraw();
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
            icon.GetComponent<Button>().onClick.AddListener(PointerDown);
            icon.GetComponent<Button>().targetGraphic = icon.GetComponent<Image>();
            icon.transform.SetParent(Place);
            alreadyDrawn.Add(icon);
        }
    }

    /// <summary>
    /// Called when user clicks
    /// </summary>
    public void PointerDown()
    {
        var item = EventSystem.current.currentSelectedGameObject;
        if (item == null)
            return;
        //DropDownRevealer(item);
        PrepareConfirmationPanel(item);
    }

    /// <summary>
    /// Вызывает панель подтверждения
    /// </summary>
    /// <param семечко="item"></param>
    private void PrepareConfirmationPanel(GameObject item)
    {
        // добавляю в текст подтверждения название объекта
        var questionText = ConfirmationPanel.transform.Find("QuestionText").GetComponent<Text>();
        originalQuestionText ??= questionText.text;

        if (int.TryParse(item.name, out int index))
            questionText.text = $"{originalQuestionText} {targetInventory.Elements[index].NameInRussian.ToLower()}?";

        ConfirmationPanel.GetComponentInChildren<ConfirmationPanelLogic>().itemObject = item;
        ConfirmationPanel.SetActive(true);
    }

    ///// <summary>
    ///// Creates confirmation message
    ///// </summary>
    ///// <param name="item"></param>
    //private void DropDownRevealer(GameObject item)
    //{
    //    Choice.value = 0;
    //    Choice.RefreshShownValue();
    //    Choice.gameObject.SetActive(true);
    //    Choice.GetComponent<DropDownBehavior>().item = item;
    //}
}
