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

        PlantDesc.text = Tools.SeedStatFormatter.FormatLarge(seed);
    }

    private void OnEnable()
    {
        var scenario = GameObject.FindGameObjectWithTag("TutorialHandler")?.GetComponent<Scenario>();

        // тутор для скрещивания семян 2
        if (QSReader.Create("TutorialState").Exists("Tutorial_ChooseItemToCrossSecond_Played"))
            scenario?.Tutorial_ApplyItemToCrossSecond();

        // тутор для скрещивания семян 1
        else if (QSReader.Create("TutorialState").Exists("Tutorial_ChooseItemToCrossFirst_Played"))
            scenario?.Tutorial_ApplyItemToCrossFirst();

        // тутор для продажи пакета семян
        else if (QSReader.Create("TutorialState").Exists("Tutorial_ChooseItemToSell_Played"))
            scenario?.Tutorial_SellItem();

        // тутор для замены пакета семян
        else if (QSReader.Create("TutorialState").Exists("Tutorial_ChooseItemToReplace_Played"))
            scenario?.Tutorial_ReplaceItem();

        // тутор для панели статистики
        else if (QSReader.Create("TutorialState").Exists("Tutorial_ChooseItemToPlant_Played"))
            scenario?.Tutorial_PlantItem();

        // тутор для покупки пакета семян
        else if (QSReader.Create("TutorialState").Exists("Tutorial_Shop_Played"))
            scenario?.Tutorial_BuyItem();
    }
}
