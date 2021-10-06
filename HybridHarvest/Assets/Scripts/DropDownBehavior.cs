using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DropDownBehavior : MonoBehaviour
{
    public GameObject item;
    [SerializeField] Drawinventory drawinventory;
    [SerializeField] public Inventory targetInventory;
    public void DropdownValueChanged(Dropdown change)
    {
        switch (change.value)
        {
            case 0:
                
                break;
            case 1:
                Sell(item);
                break;
                /* case 2:
                     Plant(item);
                     break;
                 case 3:
                     Select(item);
                     break;
                 case 4:
                     Select(item);
                     break;*/
        }
        gameObject.SetActive(false);
    }

    private void Sell(GameObject item)
    {
        int a;
        if (int.TryParse(item.name, out a))
        {
            a = int.Parse(item.name);
            targetInventory.ChangeMoney(targetInventory.Elements[a].Price);
            targetInventory.ChangeReputation(targetInventory.Elements[a].Gabitus);
            targetInventory.RemoveItem(int.Parse(item.name));
            drawinventory.Redraw();
        }
    }

    /*private void Plant(GameObject item)
    {
        int a;
        if (int.TryParse(item.name, out a))
        {
            a = int.Parse(item.name);
            Seed toPlant = targetInventory.Elements[int.Parse(item.name)];
            GrowPlace.GetComponent<SignalPlanted>().PlantIt(toPlant);
            targetInventory.RemoveItem(int.Parse(item.name));
            drawinventory.Redraw();
            CurrentInventoryParent.SetActive(false);
        }
    }*/

    /*private void Select(GameObject item)
    {
        int a;
        if (int.TryParse(item.name, out a))
        {
            a = int.Parse(item.name);
            Seed toPlant = targetInventory.Elements[a];
            GrowPlace.GetComponent<LabButton>().ChosenSeed(toPlant);
            targetInventory.RemoveItem(a);
            drawinventory.Redraw();
            CurrentInventoryParent.SetActive(false);
        }
    }*/
}
