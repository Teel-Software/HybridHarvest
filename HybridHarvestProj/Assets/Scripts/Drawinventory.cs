using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Drawinventory : MonoBehaviour
{
    [SerializeField] Inventory targetInventory;
    [SerializeField] RectTransform Place;
    // Start is called before the first frame update
    void Start()
    {
        Redraw();
    }

    // Update is called once per frame
    void Redraw()
    {
        for(var i = 0; i < targetInventory.Elements.Count; i++)
        {
            var item = targetInventory.Elements[i];
            var icon = new GameObject("Icon");
            icon.AddComponent<Image>().sprite = item.image;
            icon.transform.localScale = new Vector2(0.01f, 0.01f);
            icon.transform.SetParent(Place);
        }
    }
}
