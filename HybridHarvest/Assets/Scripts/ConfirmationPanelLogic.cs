using System;
using UnityEngine;
using UnityEngine.UI;

public class ConfirmationPanelLogic : MonoBehaviour
{
    [SerializeField] Inventory targetInventory;
    [SerializeField] Drawinventory drawInventory;
    
    public string ItemSpriteName;
    public GameObject ItemObject;

    private GameObject questionObject;
    private string originalQuestionText;

    public void Awake()
    {
        //questionObject = 
    }

    /// <summary>
    /// Добавляет текущий элемент в инвентарь
    /// </summary>
    public void AddOneMore()
    {
        var seed = (Seed)Resources.Load("Seeds\\" + ItemSpriteName);
        UpdateQuestionText(seed.NameInRussian);
        targetInventory.ChangeMoney(-seed.Price);
        targetInventory.AddItem(seed);
    }

    /// <summary>
    /// Продаёт семечко
    /// </summary>
    public void Sell()
    {
        if (int.TryParse(ItemObject.name, out int index))
        {
            targetInventory.ChangeMoney(targetInventory.Elements[index].Price);
            targetInventory.ChangeReputation(targetInventory.Elements[index].Gabitus);
            targetInventory.RemoveItem(index);
            drawInventory.Redraw();
        }
    }

    /// <summary>
    /// Сажает семечко на грядку
    /// </summary>
    public void Plant()
    {
        if (int.TryParse(ItemObject.name, out int index))
        {
            Seed toPlant = targetInventory.Elements[index];
            drawInventory.GrowPlace.GetComponent<PatchGrowth>().PlantIt(toPlant);
            drawInventory.CurrentInventoryParent.SetActive(false);
        }
    }

    /// <summary>
    /// Добавляет на панель скрещивания
    /// </summary>
    public void Select()
    {
        if (int.TryParse(ItemObject.name, out int index))
        {
            Seed toSelect = targetInventory.Elements[index];
            drawInventory.GrowPlace.GetComponent<LabButton>().ChosenSeed(toSelect);
            drawInventory.CurrentInventoryParent.SetActive(false);
        }
    }

    /// <summary>
    /// Задаёт имя обрабатываемого элемента
    /// </summary>
    /// <param название спрайта="itemSpriteName"></param>
    public void DefineItem(string itemSpriteName)
    {
        ItemSpriteName = itemSpriteName;
        var seed = (Seed)Resources.Load("Seeds\\" + itemSpriteName);
        UpdateQuestionText(seed.NameInRussian);
    }

    private void UpdateQuestionText(string itemName)
    {
        var questionText = transform.parent.Find("QuestionText").GetComponent<Text>();
        originalQuestionText ??= questionText.text;
        questionText.text = $"{originalQuestionText} {itemName.ToLower()}?";
    }

    public void AddPrice()
    {
        if (!int.TryParse(ItemObject.name, out int index)) return;

        var questionObj = transform.parent.Find("QuestionText").gameObject;
        var questionText = questionObj.GetComponent<Text>();
        var priceText = questionObj.transform.Find("PriceText").gameObject.GetComponent<Text>();

        questionText.text = questionText.text.Remove(questionText.text.Length - 1);
        priceText.text = $"за {targetInventory.Elements[index].Price} Пр.?";
    }
}
