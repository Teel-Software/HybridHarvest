using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExhibitionButton : MonoBehaviour
{
    [SerializeField] GameObject Inventory;

    public void Click()
    {
        Inventory.GetComponent<Drawinventory>().GrowPlace = gameObject.GetComponent<Button>();
        Inventory.gameObject.SetActive(true);
    }
}
