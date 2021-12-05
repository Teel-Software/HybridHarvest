using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExhibitionButton : MonoBehaviour
{
    [SerializeField] GameObject Inventory;
    public Seed NowSelected;

    public void Click()
    {
        Inventory.GetComponent<Drawinventory>().GrowPlace = gameObject.GetComponent<Button>();
        Inventory.GetComponent<Drawinventory>().SetPurpose(PurposeOfDrawing.AddToExhibition);
        Inventory.gameObject.SetActive(true);
    }

    public void ChooseSeed(Seed seed)
    {
        NowSelected = seed;
        gameObject.GetComponent<Image>().sprite = seed.PlantSprite;
    }
}
