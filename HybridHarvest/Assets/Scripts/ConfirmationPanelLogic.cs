using UnityEngine;

public class ConfirmationPanelLogic : MonoBehaviour
{
    [SerializeField] Inventory targetInventory;
    [SerializeField] Drawinventory drawInventory;
    private string itemName;
    public GameObject itemObject;

    /// <summary>
    /// ��������� ������� ������� � ���������
    /// </summary>
    public void AddOneMore()
    {
        var seed = (Seed)Resources.Load("Seeds\\" + itemName);
        targetInventory.ChangeMoney(-seed.Price);
        targetInventory.AddItem(seed);
    }

    /// <summary>
    /// ������ �������
    /// </summary>
    public void Sell()
    {
        if (int.TryParse(itemObject.name, out int a))
        {
            targetInventory.ChangeMoney(targetInventory.Elements[a].Price);
            targetInventory.ChangeReputation(targetInventory.Elements[a].Gabitus);
            targetInventory.RemoveItem(a);
            drawInventory.Redraw();
        }
    }

    /// <summary>
    /// ������ ������� �� ������
    /// </summary>
    public void Plant()
    {
        if (int.TryParse(itemObject.name, out int a))
        {
            Seed toPlant = targetInventory.Elements[a];
            drawInventory.GrowPlace.GetComponent<PatchGrowth>().PlantIt(toPlant);
            drawInventory.CurrentInventoryParent.SetActive(false);
        }
    }

    /// <summary>
    /// ��������� �� ������ �����������
    /// </summary>
    public void Select()
    {
        if (int.TryParse(itemObject.name, out int a))
        {
            Seed toPlant = targetInventory.Elements[a];
            drawInventory.GrowPlace.GetComponent<LabButton>().ChosenSeed(toPlant);
            drawInventory.CurrentInventoryParent.SetActive(false);
        }
    }

    /// <summary>
    /// ����� ��� ��������, ������������ � ���������
    /// </summary>
    /// <param ���="newItem"></param>
    public void DefineItem(string newItem)
    {
        itemName = newItem;
    }
}
