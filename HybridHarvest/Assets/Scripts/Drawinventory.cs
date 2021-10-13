using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class Drawinventory : MonoBehaviour
{
    [SerializeField] public Inventory targetInventory;
    [SerializeField] RectTransform Place;
    [SerializeField] public GameObject CurrentInventoryParent;

    [SerializeField] Dropdown Choice;
    public Button GrowPlace { get; set; }
    List<GameObject> alreadyDrawn = new List<GameObject>();

    void Start()
    {
        targetInventory.onItemAdded += Redraw;
        Redraw();
    }

    /// <summary>
    /// updates pictures of items
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
            icon.AddComponent<Image>().sprite = item.PlantSprite;
            icon.transform.localScale = new Vector2(0.01f, 0.01f);

            icon.GetComponent<Button>().onClick.AddListener(PointerDown);
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
        {
            return;
        }
        DropDownRevealer(item);
        /*var scene = SceneManager.GetActiveScene().buildIndex;
        switch (scene)
        {
            case 1:
                DropDownRevealer(item);
                break;
            case 2:
                Plant(item);
                break;
            case 3:
                Select(item);
                break;
            case 4:
                Select(item);
                break;
        }*/
    }

    /// <summary>
    /// Creates confirmation message. Actually useless
    /// </summary>
    /// <param name="item"></param>
    private void DropDownMaker(GameObject item)
    {
        var c = new GameObject();
        c.gameObject.name = "dropPLS";
        c.gameObject.AddComponent<Dropdown>();
        c.gameObject.GetComponent<Dropdown>().AddOptions(new List<Dropdown.OptionData>());
        Debug.Log(c.gameObject.transform.position.z);
        c.transform.SetParent(Place);
        Debug.Log(c.gameObject.transform.position.z);
        c.gameObject.GetComponent<RectTransform>().position = new Vector3(item.transform.position.x, item.transform.position.y, 10);
        c.transform.position = new Vector3(item.transform.position.x, item.transform.position.y, 10);
        Debug.Log(c.gameObject.transform.position.z);
        c.transform.localScale = new Vector3(1, 1, 1);
        c.AddComponent<Image>();
        c.SetActive(true);
        c.gameObject.GetComponent<Dropdown>().enabled = true;
        Debug.Log(c.transform.position.z);
        Debug.Log(c.GetComponent<RectTransform>().position.y);
    }

    /// <summary>
    /// Creates confirmation message
    /// </summary>
    /// <param name="item"></param>
    private void DropDownRevealer(GameObject item)
    {
        Choice.value = 0;
        Choice.RefreshShownValue();
        Choice.gameObject.SetActive(true);
        Choice.GetComponent<DropDownBehavior>().item = item;
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
            Redraw();
        }
    }

    private void Plant(GameObject item)
    {
        int a;
        if (int.TryParse(item.name, out a))
        {
            a = int.Parse(item.name);
            Seed toPlant = targetInventory.Elements[int.Parse(item.name)];
            GrowPlace.GetComponent<PatchGrowth>().PlantIt(toPlant);
            targetInventory.RemoveItem(int.Parse(item.name));
            Redraw();
            CurrentInventoryParent.SetActive(false);
        }
    }

    private void Select(GameObject item)
    {
        int a;
        if (int.TryParse(item.name, out a))
        {
            //a = int.Parse(item.name);
            Seed toPlant = targetInventory.Elements[a];
            GrowPlace.GetComponent<LabButton>().ChosenSeed(toPlant);
            targetInventory.RemoveItem(a);
            Redraw();
            CurrentInventoryParent.SetActive(false);
        }
    }
}
