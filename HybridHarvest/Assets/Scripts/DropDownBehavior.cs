using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class DropDownBehavior : MonoBehaviour
{
    public GameObject item;
    [FormerlySerializedAs("drawinventory")] [SerializeField] Drawinventory drawInventory;
    [SerializeField] public Inventory targetInventory;
    [SerializeField] public string title;
    
    /// <summary>
    /// Если на определённой сцене не нужна опция, здесь можно поменять картинку и надпись.
    /// </summary>
    private void Start()
    {
        gameObject.GetComponent<Dropdown>().GetComponentInChildren<Text>().text = title;
        var blank = "";
        switch (SceneManager.GetActiveScene().buildIndex)
        {
            case 1:
                gameObject.GetComponent<Dropdown>().options[2].text = blank;
                gameObject.GetComponent<Dropdown>().options[3].text = blank;
                break;
            case 2:
                gameObject.GetComponent<Dropdown>().options[3].text = blank;
                break;
            case 3:
                gameObject.GetComponent<Dropdown>().options[2].text = blank;
                break;
            case 4:
                gameObject.GetComponent<Dropdown>().options[2].text = blank;
                break;
        }
    }

    /// <summary>
    /// Если опция невозможна на сцене, отключает её.
    /// </summary>
    private void Update()
    {
        gameObject.GetComponent<Dropdown>().GetComponentInChildren<Text>().text = title;
        var dropDownList = GetComponentInChildren<Canvas>();
        if (!dropDownList) return;
        var togg = dropDownList.GetComponentsInChildren<Toggle>(true);
        switch (SceneManager.GetActiveScene().buildIndex)
        {
            case 1:
                togg[3].enabled = false;
                togg[4].enabled = false;
                break;
            case 2:
                togg[4].enabled = false;
                break;
            case 3:
                togg[3].enabled = false;
                break;
            case 4:
                togg[3].enabled = false;
                break;
        }
    }

    /// <summary>
    /// Called when any option is chosen
    /// </summary>
    /// <param индекс опции="change"></param>
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
        }
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Sells seed from inventory
    /// </summary>
    /// <param семечко="item"></param>
    private void Sell(GameObject item)
    {
        int a;
        if (int.TryParse(item.name, out a))
        {
            targetInventory.ChangeMoney(targetInventory.Elements[a].Price);
            targetInventory.ChangeReputation(targetInventory.Elements[a].Gabitus);
            targetInventory.RemoveItem(a);
            drawInventory.Redraw();
        }
    }

    /// <summary>
    /// Сажает нв грядку
    /// </summary>
    /// <param семечко="item"></param>
    private void Plant(GameObject item)
    {
        int a;
        if (int.TryParse(item.name, out a))
        {
            Seed toPlant = targetInventory.Elements[a];
            drawInventory.GrowPlace.GetComponent<PatchGrowth>().PlantIt(toPlant);
            //targetInventory.RemoveItem(a);
            //drawinventory.Redraw();
            drawInventory.CurrentInventoryParent.SetActive(false);
        }
    }

    /// <summary>
    /// Добавляет на панель скрещивания
    /// </summary>
    /// <param семечко="item"></param>
    private void Select(GameObject item)
    {
        int a;
        if (int.TryParse(item.name, out a))
        {
            Seed toPlant = targetInventory.Elements[a];
            drawInventory.GrowPlace.GetComponent<LabButton>().ChosenSeed(toPlant);
            //targetInventory.RemoveItem(a);
            //drawinventory.Redraw();
            drawInventory.CurrentInventoryParent.SetActive(false);
        }
    }
}
