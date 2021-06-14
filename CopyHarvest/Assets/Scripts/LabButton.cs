using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LabButton : MonoBehaviour
{
    [SerializeField] Button Patch;
    [SerializeField] RectTransform InventoryFrame;
    public Seed NowSelected;
    public void Clicked()
    {
        InventoryFrame.GetComponent<Drawinventory>().GrowPlace = Patch;
        InventoryFrame.gameObject.SetActive(true);
    }

    public void ChosenSeed(Seed seed)
    {
        NowSelected = seed;
        Patch.GetComponent<Image>().sprite = seed.PlantSprite;
    }
}
