using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ExhibitionButton : MonoBehaviour
{
    [SerializeField] private GameObject Inventory;
    [SerializeField] private DialogPanelLogic DialogPanel;
    [SerializeField] private Button button;
    
    public Image plantIcon; 
    [SerializeField] 
    private TextMeshProUGUI statText;
    [SerializeField] 
    private Sprite plusIcon;
    
    public Seed NowSelected;

    public void AddSeed()
    {
        Inventory.GetComponent<InventoryDrawer>().GrowPlace = gameObject.GetComponent<Button>();
        Inventory.GetComponent<InventoryDrawer>().SetPurpose(PurposeOfDrawing.AddToExhibition);
        Inventory.gameObject.SetActive(true);
        
        if (NowSelected == null)
        {
            gameObject.GetComponent<NotificationCenter>().Show("Выберите культуру для показа");
        }
        else
        {
            gameObject.GetComponent<NotificationCenter>().Show("На что вы хотите заменить культуру?");
        }
    }

    public void DisabledClick()
    {
        gameObject.GetComponent<NotificationCenter>()
            .Show("Выставка в самом разгаре\nПриходите завтра");
    }

    public void GetResult()
    {
        if (NowSelected is null)
        {
            gameObject.GetComponent<NotificationCenter>()
                .Show("Нет награды");
        }
        else
        {
            var awards = new List<Award>
            {
                new Award(AwardType.Money, amount: 100),
                new Award(AwardType.Reputation, amount: 100)
            };
            gameObject.GetComponent<AwardsCenter>().Show(awards);
            NowSelected = null;
            MakeDisabled();
        }

    }

    public void SetSeed(Seed seed)
    {
        NowSelected = seed;
        statText.text = Tools.SeedStatFormatter.FormatSmall(seed);
        plantIcon.sprite = seed.PlantSprite;
    }

    public void MakeDisabled()
    {
        button.onClick.RemoveAllListeners();
        plantIcon.sprite = Resources.Load<Sprite>("Transparent");
        statText.text = "";
        NowSelected = null;
    }
}
