using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ExhibitionButton : MonoBehaviour
{
    [SerializeField] private GameObject Inventory;
    [SerializeField] private DialogPanelLogic DialogPanel;
    
    [SerializeField] private TextMeshProUGUI statText;
    [SerializeField] private Image plantIcon;
    [SerializeField] private Sprite plusIcon;
    
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

    public void ExhibitionClick()
    {
        gameObject.GetComponent<NotificationCenter>().Show("Выставка в самом разгаре\nПриходите завтра");
    }

    public void ResultClick()
    {
        if (NowSelected == null)
        {
            gameObject.GetComponent<NotificationCenter>().Show("Нет награды");
        }
        else
        {
            var awards = new List<Award>
            {
                new Award(AwardType.Money, money: 100),
                new Award(AwardType.Reputation, reputation: 100)
            };
            gameObject.GetComponent<AwardsCenter>().Show(awards);
            gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("InvFrameAdd");
            NowSelected = null;
        }

    }

    public void SetSeed(Seed seed)
    {
        NowSelected = seed;
        statText.text = Tools.SeedStatFormatter.FormatSmall(seed);
        plantIcon.sprite = seed.PlantSprite;
    }
}
