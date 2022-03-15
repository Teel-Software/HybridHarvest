using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HarvestProcessor : MonoBehaviour
{
    [SerializeField] GameObject VegItem;
    [SerializeField] GameObject Inventory;
    [SerializeField] RectTransform Place;
    private List<Seed> seeds;
    private List<GameObject> seedPlaces = new List<GameObject>();
    private Button Patch;

    public void ShowHarvestMenu(List<Seed> ParentSeed, Button patch)
    {
        Patch = patch;
        seeds = ParentSeed;
        for (var i = 0; i < seeds.Count; i++)
        {
            var seed = seeds[i];

            var item = Instantiate(VegItem, Place);
            seedPlaces.Add(item);

            var plusButton = item.GetComponentInChildren<Button>();
            //button.GetComponentInChildren<Text>().text = "Может сохранить?";
            plusButton.onClick.AddListener(() =>
            {
                Inventory.GetComponent<InventoryDrawer>().SuccessfulAddition = () =>
                {
                    seeds.Remove(seed);
                    seedPlaces.Remove(item);
                    Destroy(item);
                    if (seedPlaces.Count == 0) ClearSpace();
                    Statistics.UpdateGrowedSeeds(seed.Name);
                };
                Inventory.GetComponent<InventoryDrawer>().targetInventory.AddItem(seed);
            });

            var sendToQuestBtn = item.transform.Find("SendToQuest").GetComponent<Button>();
            sendToQuestBtn.onClick.AddListener(() =>
            {
                var taskController = GetComponent<TaskController>();
                var renderPlace = taskController
                    .OpenQuestsPreview(sendToQuestBtn.GetComponentInChildren<Text>(), seed)
                    .transform;

                for (var i = 0; i < renderPlace.childCount; i++)
                {
                    var task = renderPlace.GetChild(i).GetComponent<Task>();
                    task.AddQuestItem = () =>
                    {
                        seeds.Remove(seed);
                        seedPlaces.Remove(item);
                        Destroy(item);
                        if (seedPlaces.Count == 0) ClearSpace();
                        Statistics.UpdateGrowedSeeds(seed.Name);

                        taskController.questsPreviewPanel.SetActive(false);

                        // Place.GetChild(0).Find("SendToQuest").GetComponent<Button>().onClick.Invoke();
                    };
                }

                // taskController.RenderTaskPreviews();
            });

            var label = item.transform.Find("Text");
            label.GetComponent<Text>().text =
                $"{seeds[i].NameInRussian} (англ. {seeds[i].Name}, лат. {seeds[i].NameInLatin})\n" +
                $"Вкус: {seeds[i].Taste}\n" +
                $"Габитус: {seeds[i].Gabitus}\n" +
                $"Время роста: {seeds[i].GrowTime}\n" +
                $"Кол-во плодов: {seeds[i].minAmount} - {seeds[i].maxAmount}\n" +
                $"Шанс мутации: {seeds[i].MutationPossibility}\n";

            var img = item.transform.Find("Image");
            img.GetComponent<Image>().sprite = seeds[i].PlantSprite;

        }
    }

    private void Sell(Seed seed)
    {
        var inventory = Inventory.GetComponent<InventoryDrawer>().targetInventory;
        inventory.AddMoney(seed.Price);
        inventory.ChangeReputation(seed.Gabitus);
        Statistics.UpdateSoldSeeds(seed.Name);
        Statistics.UpdateGrowedSeeds(seed.Name);
        inventory.Save();
    }

    public void SellAll()
    {
        for (var i = 0; i < seeds.Count; i++)
        {
            Sell(seeds[i]);
            Destroy(seedPlaces[i]);
        }

        seeds.RemoveAll(x => x);
        seedPlaces.RemoveAll(x => x);
        Patch.GetComponent<PatchGrowth>().ClearPatch();
        gameObject.SetActive(false);
        Save();

        // тутор для выхода из поля на другую сцену
        GameObject.FindGameObjectWithTag("TutorialHandler")
            ?.GetComponent<Scenario>()
            ?.Tutorial_FieldEnding();
    }

    public void ClearSpace()
    {
        if (seeds.Count == 0)
            Patch.GetComponent<PatchGrowth>().ClearPatch();
        for (var i = 0; i < seeds.Count; i++)
        {
            Destroy(seedPlaces[i]);
        }

        seedPlaces.RemoveAll(x => x);
        gameObject.SetActive(false);
        Save();
    }

    private void Save()
    {
        if (seeds.Count == 0) return;

        PlayerPrefs.SetInt(Patch.name + "seedsCount", seeds.Count);
        for (var i = 0; i < seeds.Count; i++)
            PlayerPrefs.SetString(Patch.name + "seedElement" + i.ToString(), seeds[i].ToString());
    }

    private void OnEnable()
    {
        GetComponent<TaskController>()
            ?.questsPreviewPanel
            ?.SetActive(false);

        var scenario = GameObject.FindGameObjectWithTag("TutorialHandler")?.GetComponent<Scenario>();

        // тутор для окна сбора урожая
        if (QSReader.Create("TutorialState").Exists("Tutorial_WaitForGrowing_Played"))
            scenario.Tutorial_HarvestPlace();
    }
}
