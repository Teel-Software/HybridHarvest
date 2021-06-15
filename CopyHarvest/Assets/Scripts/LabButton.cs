using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LabButton : MonoBehaviour
{
    [SerializeField] Button SelectButton;
    [SerializeField] RectTransform InventoryFrame;
    [SerializeField] Sprite defaultSprite;
    public Seed NowSelected;
    public void Clicked()
    {
        InventoryFrame.GetComponent<Drawinventory>().GrowPlace = SelectButton;
        InventoryFrame.gameObject.SetActive(true);
    }

    public void ChosenSeed(Seed seed)
    {
        NowSelected = seed;
        SelectButton.GetComponent<Image>().sprite = seed.PlantSprite;
        var seedInfo = seed.NameInRussian + "\n����: " + seed.Taste.ToString() + "\n�������: " + seed.Gabitus.ToString() + "\n����� \n�����: " + seed.GrowTime.ToString();
        SelectButton.GetComponentInChildren<Text>().text = seedInfo;
    }

    public void ClearButton() 
    {
        gameObject.GetComponent<LabButton>().NowSelected = null;
        gameObject.GetComponent<Image>().sprite = defaultSprite;
        SelectButton.GetComponentInChildren<Text>().text = "";
    }
}
