using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Drawinventory : MonoBehaviour
{
    [SerializeField] Inventory targetInventory;
    [SerializeField] RectTransform Place;
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
        //Debug.Log("ok1");
        var item = EventSystem.current.currentSelectedGameObject;
        if (item == null)
        {
            //Debug.Log("not ok");
            return;
        }
        //print("ok2");
        int a;
        if (int.TryParse(item.name, out a))
        {
            a = int.Parse(item.name);
            targetInventory.ChangeMoney(targetInventory.Elements[a].Price);
            targetInventory.RemoveItem(int.Parse(item.name));
           // print("ok2.5");
            //targetInventory.RemoveItem(0);
          //  print("ok3");
            Redraw();
           // print("Grando finale");
        }
        //else
        //    Debug.Log(item.name);
    }
}
