using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LabButton : MonoBehaviour
{
    [SerializeField] Button SelectButton;
    [SerializeField] public Button PlaceForResult;
    [SerializeField] Button SecondButton;
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
        var seedInfo = seed.NameInRussian + "\nВкус: " + seed.Taste.ToString() + "\nГабитус: " + seed.Gabitus.ToString() + "\nВремя \nроста: " + seed.GrowTime.ToString();
        SelectButton.GetComponentInChildren<Text>().text = seedInfo;
        if (SecondButton == null) return;
        var seed1 = SecondButton.GetComponent<LabButton>().NowSelected;
        if (seed1 == null || NowSelected == null) return;
        DrawResult(seed1);
    }

    public void ChosenSeed(Seed seed, int[] chance)
    {
        NowSelected = seed;
        SelectButton.GetComponent<Image>().sprite = seed.PlantSprite;
        var seedInfo = seed.NameInRussian +
            "\nВкус: " + seed.Taste.ToString() +" "+chance[0].ToString()+"%"+
            "\nГабитус: " + seed.Gabitus.ToString() + " " + chance[1].ToString() + "%" +
            "\nВремя \nроста: " + seed.GrowTime.ToString()+" " + chance[2].ToString() + "%";
        SelectButton.GetComponentInChildren<Text>().text = seedInfo;
        if (SecondButton == null) return;
        var seed1 = SecondButton.GetComponent<LabButton>().NowSelected;
        if (seed1 == null || NowSelected == null) return;
        DrawResult(seed1);
    }

    private void DrawResult(Seed seed1)
    {
        var newseed = PlaceForResult.GetComponent<GeneCrossing>().MixTwoParents(seed1, NowSelected);
        var chance = PlaceForResult.GetComponent<GeneCrossing>().Chances;
        PlaceForResult.GetComponent<LabButton>().ChosenSeed(newseed, chance);
        PlaceForResult.gameObject.SetActive(true);
    }

    public void ClearButton()
    {
        gameObject.GetComponent<LabButton>().NowSelected = null;
        gameObject.GetComponent<Image>().sprite = defaultSprite;
        SelectButton.GetComponentInChildren<Text>().text = "";
    }
}
