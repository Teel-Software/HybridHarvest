using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CI.QuickSave;

public class ExhibitionButton : MonoBehaviour
{
    [SerializeField] GameObject Inventory;
    [SerializeField] DialogPanelLogic DialogPanel;
    public Seed NowSelected;

    public void DefaultClick()
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
            var awards = new List<Award>();
            awards.Add(new Award(AwardType.Money, money: 100));
            awards.Add(new Award(AwardType.Reputation, reputation: 100));
            gameObject.GetComponent<AwardsCenter>().Show(awards);

            gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("InvFrameAdd");
            NowSelected = null;
        }

    }

    public void ChooseSeed(Seed seed)
    {
        NowSelected = seed;
        gameObject.GetComponent<Image>().sprite = seed.PlantSprite;
    }

    private void OnDisable()
    {
        SaveData();
    }

    private void OnEnable()
    {
        gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("InvFrameAdd");
        NowSelected = null;
        CollectData();
    }

    private void SaveData()
    {
        var writer = QuickSaveWriter.Create("ExhibitionData");
        if (NowSelected != null)
            writer.Write("ExhSeed", NowSelected.ToString());
        else
            writer.Write("ExhSeed", "no");
        writer.Commit();
    }

    private void CollectData()
    {
        var reader = QSReader.Create("ExhibitionData");
        if (reader.Exists("ExhSeed"))
        {
            var parameters = reader.Read<string>("ExhSeed");
            if (parameters == "no")
                return;
            var newSeed = ScriptableObject.CreateInstance<Seed>();
            newSeed.SetValues(parameters);
            ChooseSeed(newSeed);
        }
    }
}
