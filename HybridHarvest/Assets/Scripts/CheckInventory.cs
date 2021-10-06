using UnityEngine;

public class CheckInventory : MonoBehaviour
{
    [SerializeField] Inventory targetInventory;
    public string item;

    public void AddOneMore()
    {
        var seed = (Seed)Resources.Load("Seeds\\" + item);
        targetInventory.ChangeMoney(-seed.Price);
        targetInventory.AddItem(seed);
    }

    public void DefineItem(string newItem)
    {
        item = newItem;
    }
}
