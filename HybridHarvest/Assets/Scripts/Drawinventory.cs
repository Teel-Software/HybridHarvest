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

            //var colBlock = new ColorBlock
            //{
            //    highlightedColor = Color.red,
            //    fadeDuration = 0.1f
            //};
            //icon.GetComponent<Button>().colors = colBlock;
            //icon.GetComponent<Button>().targetGraphic = icon.GetComponent<Image>();

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
    /// �������� ������ �������������
    /// </summary>
    /// <param �������="item"></param>
    private void PrepareConfirmationPanel(GameObject item)
    {
        // �������� � ����� ������������� �������� �������
        var questionText = ConfirmationPanel.transform.Find("QuestionText").GetComponent<Text>();
        originalQuestionText ??= questionText.text;

        if (int.TryParse(item.name, out int index))
            questionText.text = $"{originalQuestionText} {targetInventory.Elements[index].NameInRussian.ToLower()}?";

        ConfirmationPanel.GetComponentInChildren<ConfirmationPanelLogic>().itemObject = item;
        ConfirmationPanel.SetActive(true);
    }

    ///// <summary>
    ///// Creates confirmation message. Actually useless
    ///// </summary>
    ///// <param name="item"></param>
    //private void DropDownMaker(GameObject item)
    //{
    //    var c = new GameObject();
    //    c.gameObject.name = "dropPLS";
    //    c.gameObject.AddComponent<Dropdown>();
    //    c.gameObject.GetComponent<Dropdown>().AddOptions(new List<Dropdown.OptionData>());
    //    Debug.Log(c.gameObject.transform.position.z);
    //    c.transform.SetParent(Place);
    //    Debug.Log(c.gameObject.transform.position.z);
    //    c.gameObject.GetComponent<RectTransform>().position = new Vector3(item.transform.position.x, item.transform.position.y, 10);
    //    c.transform.position = new Vector3(item.transform.position.x, item.transform.position.y, 10);
    //    Debug.Log(c.gameObject.transform.position.z);
    //    c.transform.localScale = new Vector3(1, 1, 1);
    //    c.AddComponent<Image>();
    //    c.SetActive(true);
    //    c.gameObject.GetComponent<Dropdown>().enabled = true;
    //    Debug.Log(c.transform.position.z);
    //    Debug.Log(c.GetComponent<RectTransform>().position.y);
    //}

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
