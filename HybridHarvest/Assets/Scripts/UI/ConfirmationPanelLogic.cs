using System;
using UnityEngine;
using UnityEngine.UI;

public class ConfirmationPanelLogic : MonoBehaviour
{
    [SerializeField] public Button YesButton;
    [SerializeField] private GameObject parentGameObject;
    [SerializeField] private Text questionHeader;
    [SerializeField] private Text questionDescription;

    public Inventory targetInventory;
    public InventoryDrawer inventoryDrawer;
    public GameObject ItemObject;

    private Action yesAction = () => { };
    private Action noAction = () => { };

    private void Awake()
    {
        targetInventory ??= GameObject.FindGameObjectWithTag("Inventory").GetComponent<Inventory>();
    }

    /// <summary>
    /// Покупает семечко.
    /// </summary>
    public void Buy(Seed seed)
    {
        inventoryDrawer.SuccessfulAddition = () => targetInventory.AddMoney(-seed.ShopBuyPrice);
        targetInventory.AddItem(seed);
        Statistics.UpdatePurchasedSeeds(seed.Name);

        parentGameObject.SetActive(false);
    }

    /// <summary>
    /// Продаёт семечко.
    /// </summary>
    public void Sell()
    {
        if (!int.TryParse(ItemObject.name, out var index)) return;

        var seed = targetInventory.Elements[index];

        targetInventory.AddMoney(seed.Price);
        targetInventory.ChangeReputation(seed.Gabitus);
        targetInventory.RemoveItem(index);
        inventoryDrawer.Redraw();

        Statistics.UpdateSoldSeeds(seed.Name);

        parentGameObject.SetActive(false);
    }

    /// <summary>
    /// Сажает семечко на грядку.
    /// </summary>
    public void Plant()
    {
        if (!int.TryParse(ItemObject.name, out var index)) return;

        var toPlant = targetInventory.Elements[index];
        inventoryDrawer.GrowPlace.GetComponent<PatchGrowth>().PlantIt(toPlant);
        inventoryDrawer.ToggleGameObject(false);
    }

    /// <summary>
    /// Добавляет семечко на панель скрещивания.
    /// </summary>
    public void Select()
    {
        if (!int.TryParse(ItemObject.name, out var index)) return;

        var toSelect = targetInventory.Elements[index];
        inventoryDrawer.GrowPlace.GetComponent<LabButton>().ChosenSeed(toSelect);
        inventoryDrawer.ToggleGameObject(false);
    }

    /// <summary>
    /// Добавляет семечко на панель выставки.
    /// </summary>
    public void SendToExhibition()
    {
        if (!int.TryParse(ItemObject.name, out var index)) return;

        var toSend = targetInventory.Elements[index];
        inventoryDrawer.GrowPlace.GetComponent<ExhibitionButton>().SetSeed(toSend);
        inventoryDrawer.ToggleGameObject(false);
    }

    /// <summary>
    /// Заменяет семечко на другое.
    /// </summary>   
    public void ChangeItem(Seed newSeed)
    {
        if (!int.TryParse(ItemObject.name, out var index)) return;

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

        inventoryDrawer.Redraw();
        parentGameObject.SetActive(false);
    }

    /// <summary>
    /// Назначает вопрос.
    /// </summary>
    /// <param name="question">Сам вопрос.</param>
    /// <param name="description">Описание вопроса.</param>
    public void SetQuestion(string question, string description)
    {
        questionHeader.text = question;
        questionDescription.text = description;
    }

    /// <summary>
    /// Назначает действие, которое будет выполенно при положительном ответе.
    /// </summary>
    /// <param name="action">Действие.</param>
    public void SetYesAction(Action action)
    {
        yesAction = action;
    }

    /// <summary>
    /// Назначает действие, которое будет выполенно при отрицательном ответе.
    /// </summary>
    /// <param name="action">Действие.</param>
    public void SetNoAction(Action action)
    {
        noAction = action;
    }

    /// <summary>
    /// Выполняет заданное ранее положительное действие.
    /// </summary>
    public void ExecuteYesAction()
    {
        yesAction.Invoke();
        gameObject.SetActive(false);
        
        var scenario = GameObject.FindGameObjectWithTag("TutorialHandler")?.GetComponent<Scenario>();

        // тутор для роста семечка
        if (QSReader.Create("TutorialState").Exists("Tutorial_ConfirmSpeedUp_Played"))
            scenario.Tutorial_WaitForGrowing();
    }

    /// <summary>
    /// Выполняет заданное ранее отрицательное действие.
    /// </summary>
    public void ExecuteNoAction()
    {
        noAction.Invoke();
        gameObject.SetActive(false);
    }
}
