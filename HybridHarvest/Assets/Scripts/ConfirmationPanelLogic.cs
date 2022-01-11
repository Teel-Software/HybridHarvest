using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class ConfirmationPanelLogic : MonoBehaviour
{
    [SerializeField] private GameObject parentGameObject;
    public Inventory targetInventory;
    public Drawinventory drawInventory;
    public bool HasPrice;
    public string ItemName;
    
    public GameObject ItemObject;

    private GameObject questionObject;
    private string originalQuestionText;
    
    /// <summary>
    /// Покупает семечко
    /// </summary>
    public void Buy(Seed seed)
    {
        targetInventory.AddMoney(-seed.ShopBuyPrice);
        targetInventory.AddItem(seed);

        Statistics.UpdatePurchasedSeeds(seed.Name);
        parentGameObject.SetActive(false);
    }

    /// <summary>
    /// Продаёт семечко
    /// </summary>
    public void Sell()
    {
        if (!int.TryParse(ItemObject.name, out int index)) return;
        
        var seed = targetInventory.Elements[index];

        targetInventory.AddMoney(seed.Price);
        targetInventory.ChangeReputation(seed.Gabitus);
        targetInventory.RemoveItem(index);
        drawInventory.Redraw();

        Statistics.UpdateSoldSeeds(seed.Name);
        
        parentGameObject.SetActive(false);
    }

    /// <summary>
    /// Сажает семечко на грядку
    /// </summary>
    public void Plant()
    {
        if (!int.TryParse(ItemObject.name, out int index)) return;
        
        var toPlant = targetInventory.Elements[index];
        drawInventory.GrowPlace.GetComponent<PatchGrowth>().PlantIt(toPlant);
        drawInventory.InventoryGameObject.SetActive(false);
    }

    /// <summary>
    /// Добавляет семечко на панель скрещивания
    /// </summary>
    public void Select()
    {
        if (!int.TryParse(ItemObject.name, out int index)) return;
        
        var toSelect = targetInventory.Elements[index];
        drawInventory.GrowPlace.GetComponent<LabButton>().ChosenSeed(toSelect);
        drawInventory.InventoryGameObject.SetActive(false);
    }

    /// <summary>
    /// Добавляет семечко на панель выставки
    /// </summary>
    public void SendToExhibition()
    {
        if (!int.TryParse(ItemObject.name, out int index)) return;
        
        var toSend = targetInventory.Elements[index];
        drawInventory.GrowPlace.GetComponent<ExhibitionButton>().ChooseSeed(toSend);
        drawInventory.InventoryGameObject.SetActive(false);
    }
    
    /// <summary>
    /// Заменяет семечко на другое
    /// </summary>   
    public void ChangeItem(Seed newSeed)
    {
        if (!int.TryParse(ItemObject.name, out int index)) return;
        
        if (index == targetInventory.Elements.Count)
        {
            targetInventory.Elements.Add(newSeed);
        }
        else
        {
            targetInventory.AddMoney(targetInventory.Elements[index].Price);
            targetInventory.ChangeReputation(targetInventory.Elements[index].Gabitus);
            targetInventory.Elements[index] = newSeed;
        }
        drawInventory.Redraw();
        parentGameObject.SetActive(false);
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

    /// <summary>
    /// Добавляет к основному тексту название растения
    /// <param name="seed">Класс семени растения</param>
    /// </summary>
    public void DefineItem(Seed seed)
    {
        UpdateQuestionText(seed.NameInRussian);
    }

    private void UpdateQuestionText(string itemName)
    {
        var questionText = transform.parent.Find("QuestionText").GetComponent<Text>();
        originalQuestionText ??= questionText.text;

        questionText.text = $"{originalQuestionText} {itemName.ToLower()}";
        if (!HasPrice) questionText.text += "?";
    }

    void OnEnable()
    {
        //SetPrice();
    }
    
    /// <summary>
    /// Добавляет ко второму текстовому объекту цену объекта семени
    /// </summary>
    private void SetPrice()
    {
        if (!HasPrice) return;
        var price = 0;
        //Случай покупки из магазина
        if (ItemObject is null)
        {
            var seed = (Seed)Resources.Load("Seeds\\" + ItemName);
            price = seed.ShopBuyPrice;
        }
        //Случай действия из инвентаря
        else if (int.TryParse(ItemObject.name, out int index))
        {
            price = targetInventory.Elements[index].Price;
        }

        var ptObj = transform.parent.Find("QuestionText").Find("PriceText");
        var priceText = ptObj.GetComponent<TextMeshProUGUI>();

        priceText.text = $"за {price} <sprite name=\"Money\"> ?";
    }
}
