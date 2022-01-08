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

    void Start()
    {
        
    }
    
    void Update()
    {
        
    }

    public void DisplayStats(Seed seed)
    {
        PlantImage.sprite = seed.PlantSprite;
        
        PlantName.text = seed.NameInRussian;
        PlantNameLatin.text = $"(лат. {seed.NameInLatin})";

        QualityColor.sprite = Resources.Load<Sprite>("Packets\\Quality" + seed.PacketQuality);
        var qualityTxt = "";
        switch (seed.PacketQuality)
        {
            case 0:
                qualityTxt = "Обычный";
                break;
            case 1:
                qualityTxt = "Особый";
                break;
            case 2:
                qualityTxt = "Удивительный";
                break;
            case 3:
                qualityTxt = "Мифический";
                break;
            case 4:
                qualityTxt = "Легендарный";
                break;
        }
        QualityText.text = $"{qualityTxt}";

        PlantDesc.text = $"Вкус - {seed.Taste}\n\n" +
                         $"Габитус - {seed.Gabitus}\n\n" +
                         $"Время роста - {seed.GrowTime} секунд\n\n" +
                         $"Цена - {seed.Price}<sprite name=\"Money\">\n";
    }
}
