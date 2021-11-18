using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class ConfirmationPanelLogic : MonoBehaviour
{
    [SerializeField] Inventory targetInventory;
    [SerializeField] Drawinventory drawInventory;
    [SerializeField] public bool HasPrice = false;
    
    [FormerlySerializedAs("ItemSpriteName")] 
    [SerializeField] public string ItemName;
    
    private GameObject itemObject;
    public GameObject ItemObject
    {
        set => itemObject = value;
    }
    
    private GameObject questionObject;
    private string originalQuestionText;

    /// <summary>
    /// Добавляет надпись о цене, если требуется
    /// </summary>
    public void Awake()
    {
        SetPrice();
    }
    
    /// <summary>
    /// Покупает семечко
    /// </summary>
    public void AddOneMore()
    {
        var seed = (Seed)Resources.Load("Seeds\\" + ItemName);
        UpdateQuestionText(seed.NameInRussian);
        targetInventory.ChangeMoney(-seed.Price);
        targetInventory.AddItem(seed);
    }

    /// <summary>
    /// Продаёт семечко
    /// </summary>
    public void Sell()
    {
        if (int.TryParse(itemObject.name, out int index))
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
        if (int.TryParse(itemObject.name, out int index))
        {
            Seed toPlant = targetInventory.Elements[index];
            drawInventory.GrowPlace.GetComponent<PatchGrowth>().PlantIt(toPlant);
            drawInventory.CurrentInventoryParent.SetActive(false);
        }
    }

    /// <summary>
    /// Добавляет семечко на панель скрещивания
    /// </summary>
    public void Select()
    {
        if (int.TryParse(itemObject.name, out int index))
        {
            Seed toSelect = targetInventory.Elements[index];
            drawInventory.GrowPlace.GetComponent<LabButton>().ChosenSeed(toSelect);
            drawInventory.CurrentInventoryParent.SetActive(false);
        }
    }
    
    /// <summary>
    /// Добавляет к основному тексту название растения
    /// <param name="itemName">Имя растения на английском</param>
    /// </summary>
    public void DefineItem(string itemName)
    {
        ItemName = itemName;
        var seed = (Seed)Resources.Load("Seeds\\" + itemName);
        UpdateQuestionText(seed.NameInRussian);
    }

    private void UpdateQuestionText(string itemName)
    {
        var questionText = transform.parent.Find("QuestionText").GetComponent<Text>();
        originalQuestionText ??= questionText.text;
        
        questionText.text = $"{originalQuestionText} {itemName.ToLower()}";
        if (!HasPrice) questionText.text += "?";
    }
    
    /// <summary>
    /// Добавляет ко второму текстовому объекту цену объекта семени
    /// </summary>
    private void SetPrice()
    {
        if (!HasPrice) return;
        var price = 0;
        //Случай покупки из магазина
        if (itemObject is null) 
        {
            var seed = (Seed)Resources.Load("Seeds\\" + ItemName);
            price = seed.Price;
        }
        //Случай действия из инвентаря
        else if (int.TryParse(itemObject.name, out int index))
        {
            price = targetInventory.Elements[index].Price;
        }
        
        var ptObj = transform.parent.Find("QuestionText").Find("PriceText");
        var priceText = ptObj.GetComponent<TextMeshProUGUI>();
        
        priceText.text = $"за {price} <sprite name=\"Money\"> ?";
    }
}
