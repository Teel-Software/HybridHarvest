using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatPanelDrawer : MonoBehaviour
{
    [SerializeField] private Image PlantImage;
    [SerializeField] private TextMeshProUGUI PlantDesc;
    [SerializeField] private Text PlantName;
    [SerializeField] private TextMeshProUGUI PlantNameLatin;

    [SerializeField] private Image QualityColor;
    [SerializeField] private TextMeshProUGUI QualityText;

    public GameObject ProceedButton;

    public void DisplayStats(Seed seed)
    {
        PlantImage.sprite = seed.PlantSprite;

        PlantName.text = seed.NameInRussian;
        PlantNameLatin.text = $"(лат. {seed.NameInLatin})";

        QualityColor.sprite = Resources.Load<Sprite>("Packets\\Quality" + seed.PacketQuality);

        var qualityTxt = "";
        var qualityTxtColor = Color.black;
        switch (seed.PacketQuality)
        {
            case 0:
                qualityTxt = "Обычный";
                ColorUtility.TryParseHtmlString("#D7D7EF", out qualityTxtColor);
                break;
            case 1:
                qualityTxt = "Особый";
                ColorUtility.TryParseHtmlString("#73A9F4", out qualityTxtColor);
                break;
            case 2:
                qualityTxt = "Удивительный";
                ColorUtility.TryParseHtmlString("#C768ED", out qualityTxtColor);
                break;
            case 3:
                qualityTxt = "Мифический";
                ColorUtility.TryParseHtmlString("#FF0043", out qualityTxtColor);
                break;
            case 4:
                qualityTxt = "Легендарный";
                break;
        }
        QualityText.text = $"{qualityTxt}";
        QualityText.color = qualityTxtColor;

        var price = seed.ShopBuyPrice > 0 ? seed.ShopBuyPrice : seed.Price;

        PlantDesc.text = $"Вкус - {seed.Taste}\n\n" +
                         $"Габитус - {seed.Gabitus}\n\n" +
                         $"Время роста - {Tools.TimeFormatter.Format(seed.GrowTime)}\n\n" +
                         $"Цена - {price}<sprite name=\"Money\">\n";
    }

    private void OnEnable()
    {
        // тутор для панели
        GameObject.FindGameObjectWithTag("TutorialHandler")
            ?.GetComponent<Scenario>()
            ?.Tutorial_StatPanel();
    }
}
