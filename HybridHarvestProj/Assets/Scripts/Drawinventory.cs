using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class Drawinventory : MonoBehaviour
{
    [SerializeField]public Inventory targetInventory;
    [SerializeField] RectTransform Place;
    //[SerializeField] Dropdown Choice;
    public Button GrowPlace { get; set; }
    List<GameObject> alreadyDrawn = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        targetInventory.onItemAdded += Redraw;
        Redraw();
    }

    // Update is called once per frame
    void Redraw()
    {
        for(var i=0; i < alreadyDrawn.Count; i++)
        {
            Destroy(alreadyDrawn[i]);
        }
        alreadyDrawn.Clear();
        for(var i = 0; i < targetInventory.Elements.Count; i++)
        {
            var item = targetInventory.Elements[i];
            //Debug.Log(item.MyImage);
            var icon = new GameObject(i.ToString(), typeof(Button));
            //icon.AddComponent<BoxCollider2D>();
            //icon.AddComponent<Rigidbody2D>();
            //icon.GetComponent<Rigidbody2D>().gravityScale = 0;
            icon.AddComponent<Image>().sprite = item.MyImage;
            icon.transform.localScale = new Vector2(0.01f, 0.01f);
            
            icon.GetComponent<Button>().onClick.AddListener(PointerDown);
            icon.transform.SetParent(Place);
            alreadyDrawn.Add(icon);
            
        }
    }

    //public void OnPointerDown(PointerEventData eventData)
    //{
    //    Debug.Log("ok1");
    //    var item = (GameObject)eventData.pointerPress;
    //    if (item == null)
    //    {
    //        Debug.Log("not ok");
    //        return;
    //    }
    //    //Debug.Log(item.name);
    //    print("ok2");
    //    int a;
    //    if (int.TryParse(item.name, out a))
    //    {
    //        targetInventory.RemoveItem(int.Parse(item.name));
    //        print("ok2.5");
    //        //targetInventory.RemoveItem(0);
    //        print("ok3");
    //        Redraw();
    //        print("Grando finale");
    //    }
    //    else
    //        Debug.Log(item.name);
    //}

    public void PointerDown()
    {
        var item = EventSystem.current.currentSelectedGameObject;
        if (item == null)
        {
            return;
        }
        var scene = SceneManager.GetActiveScene().buildIndex;
        switch (scene)
        {
            case 1:
                Sell(item);
                break;
            case 2:
                Plant(item);
                break;
        }
        //Dropdown choice = Instantiate(Dropdown, new Vector2(0,0));
        //var choice = new GameObject("dropdown", typeof(Dropdown));
        //choice.gameObject.transform.position = new Vector3(item.transform.position.x, item.transform.position.y,10);
        //choice.transform.SetParent(item.transform);
        // choice.gameObject.SetActive(true);
        //choice.transform.SetParent(Place);
        //choice.transform.localScale = new Vector3(1f, 1f, 1f);
        //choice.GetComponent<Dropdown>().AddOptions(new List<Dropdown.OptionData>() {
        //  new Dropdown.OptionData("�������"),
        // new Dropdown.OptionData("��������")});
        // choice.AddComponent<CanvasRenderer>();
        // choice.gameObject.layer = 5;
        //var i = new GameObject("fff", typeof(Button));
        //i.gameObject.transform.position = new Vector3(item.transform.position.x, item.transform.position.y, 10);
        //choice.AddComponent<Text>();
        //choice.GetComponent<Text>().text = "xnj rfr";
        //choice.gameObject.SetActive(true);
        //choice.AddComponent<Image>().sprite = item.GetComponent<Image>().sprite;
        //choice.GetComponent<Dropdown>().enabled = true;
        //Instantiate(choice);
        //Instantiate(Choice, item.transform.position, Quaternion.identity);
        // Choice.Show();
        //Choice.gameObject.SetActive(true);
        //Choice.gameObject.transform.position = item.gameObject.transform.position;
        //Choice.GetComponent<Dropdown>().onValueChanged.AddListener((option)=> {
        //    switch (option)
        //    {
        //        case 0:
        //            Sell(item);
        //            break;
        //    }
        //    //print("net");
        //    //Destroy(Choice);
        //    //Choice.gameObject.GetComponent<Dropdown>().enabled = false;
        //    Choice.gameObject.SetActive(false);
        //});
        //print("da");
        //Sell(item);
    }

    //private void Se(int option)
    //{
    //    switch (option)
    //    {
    //        case 0:
    //            Sell();
    //            break;
    //    }
    //}

    private void Sell(GameObject item)
    {
        int a;
        if (int.TryParse(item.name, out a))
        {
            a = int.Parse(item.name);
            targetInventory.ChangeMoney(targetInventory.Elements[a].Price);
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
            //targetInventory.ChangeMoney(targetInventory.Elements[a].Price);
            //targetInventory.RemoveItem(int.Parse(item.name));
            //print("planted");
            Seed toPlant = targetInventory.Elements[int.Parse(item.name)];
            //print(toPlant.ToString());
            //print(GrowPlace.)
            GrowPlace.GetComponent<SignalPlanted>().PlantIt(toPlant);
            targetInventory.RemoveItem(int.Parse(item.name));
            Redraw();
            Place.gameObject.SetActive(false);
            //targetInventory.RemoveItem(int.Parse(item.name));
        }
    }
}
