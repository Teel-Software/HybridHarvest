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
        
        PlantDesc.text = $"Вкус - {seed.Taste}\n\n" +
                         $"Габитус - {seed.Gabitus}\n\n" +
                         $"Время роста - {seed.GrowTime} секунд\n\n" +
                         $"Цена - {seed.Price}<sprite name=\"Money\">\n";
    }
}
