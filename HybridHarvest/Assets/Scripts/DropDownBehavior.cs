using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DropDownBehavior : MonoBehaviour
{
    public GameObject item;
    [SerializeField] Drawinventory drawinventory;
    [SerializeField] public Inventory targetInventory;

    /// <summary>
    /// Called when any option is chosen
    /// </summary>
    /// <param name="change"></param>
    public void DropdownValueChanged(Dropdown change)
    {
        switch (change.value)
        {
            case 0:

                break;
            case 1:
                Sell(item);
                break;
            case 2:
                Plant(item);
                break;
            case 3:
                Select(item);
                break;
                /*case 4:
                    Select(item);
                    break;*/
        }
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Sells seed from inventory
    /// </summary>
    /// <param name="item"></param>
    private void Sell(GameObject item)
    {
        int a;
        if (int.TryParse(item.name, out a))
        {
            targetInventory.ChangeMoney(targetInventory.Elements[a].Price);
            targetInventory.ChangeReputation(targetInventory.Elements[a].Gabitus);
            targetInventory.RemoveItem(a);
            drawinventory.Redraw();
        }
    }

    private void Plant(GameObject item)
    {
        int a;
        if (int.TryParse(item.name, out a))
        {
            Seed toPlant = targetInventory.Elements[a];
            drawinventory.GrowPlace.GetComponent<PatchGrowth>().PlantIt(toPlant);
            targetInventory.RemoveItem(a);
            drawinventory.Redraw();
            drawinventory.CurrentInventoryParent.SetActive(false);
        }
    }

    private void Select(GameObject item)
    {
        int a;
        if (int.TryParse(item.name, out a))
        {
            Seed toPlant = targetInventory.Elements[a];
            drawinventory.GrowPlace.GetComponent<LabButton>().ChosenSeed(toPlant);
            targetInventory.RemoveItem(a);
            drawinventory.Redraw();
            drawinventory.CurrentInventoryParent.SetActive(false);
        }
    }
}
