using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HarvestProcessor : MonoBehaviour
{
    [SerializeField] private GameObject VegItem;
    [SerializeField] GameObject Inventory;
    [SerializeField] private RectTransform Place;
    [SerializeField] private Text chosenSeedsCounter;
    [SerializeField] private GameObject choseAllCheckmark;

    private List<Seed> seeds;
    private readonly List<GameObject> seedPlaces = new List<GameObject>();
    private List<Seed> chosenSeeds;
    private List<GameObject> chosenSeedPlaces;
    private Button Patch;

    /// <summary>
    /// Отрисовывает окно урожая
    /// </summary>
    /// <param name="ParentSeed">Семечко - родитель</param>
    /// <param name="patch">Грядка</param>
    public void ShowHarvestMenu(List<Seed> ParentSeed, Button patch)
    {
        Patch = patch;
        seeds = ParentSeed;
        chosenSeedsCounter.text = $"0/{seeds.Count}";

        foreach (var seed in seeds)
        {
            var item = Instantiate(VegItem, Place);
            seedPlaces.Add(item);

            var plusButton = item.GetComponentInChildren<Button>();
            plusButton.onClick.AddListener(() =>
            {
                var inventoryDrawer = Inventory.GetComponent<InventoryDrawer>();
                inventoryDrawer.SuccessfulAddition = () =>
                {
                    DeleteUsedSeed(seed, item);
                    UpdateChosenSeeds();
                };
                inventoryDrawer.targetInventory.AddItem(seed);
            });

            item.GetComponentInChildren<Toggle>().onValueChanged.AddListener(UpdateChosenSeeds);

            var label = item.transform.Find("Text");
            label.GetComponent<Text>().text =
                $"{seed.NameInRussian} (англ. {seed.Name}, лат. {seed.NameInLatin})\n" +
                $"Вкус: {seed.Taste}\n" +
                $"Габитус: {seed.Gabitus}\n" +
                $"Время роста: {seed.GrowTime}\n" +
                $"Кол-во плодов: {seed.minAmount} - {seed.maxAmount}\n" +
                $"Шанс мутации: {seed.MutationChance}\n";

            var img = item
                .transform.Find("Background")
                .transform.Find("SeedImage");
            img.GetComponent<Image>().sprite = seed.PlantSprite;
        }
    }

    /// <summary>
    /// Выбирает все плоды, либо отменяет их выбор
    /// </summary>
    public void ChoseAll()
    {
        var chosenCount = chosenSeeds.Count;

        foreach (var place in seedPlaces)
            place.GetComponentInChildren<Toggle>().isOn = chosenCount != seedPlaces.Count;

        choseAllCheckmark.SetActive(chosenCount != seedPlaces.Count);
    }

    /// <summary>
    /// Продаёт выбранные плоды
    /// </summary>
    public void SellChosen()
    {
        var removedSeeds = new List<Seed>();
        var removedSeedPlaces = new List<GameObject>();

        for (var i = 0; i < seeds.Count; i++)
        {
            if (!seedPlaces[i].GetComponentInChildren<Toggle>().isOn) continue;

            Sell(seeds[i]);
            removedSeeds.Add(seeds[i]);

            Destroy(seedPlaces[i]);
            removedSeedPlaces.Add(seedPlaces[i]);
        }

        foreach (var seed in removedSeeds)
            seeds.Remove(seed);

        foreach (var place in removedSeedPlaces)
            seedPlaces.Remove(place);

        if (seeds.Count == 0)
        {
            Patch.GetComponent<PatchGrowth>().ClearPatch();
            gameObject.SetActive(false);
            Save();
        }

        UpdateChosenSeeds();

        // тутор для выхода из поля на другую сцену
        GameObject.FindGameObjectWithTag("TutorialHandler")
            ?.GetComponent<Scenario>()
            ?.Tutorial_FieldEnding();
    }

    /// <summary>
    /// Приводит окно урожая к начальному состоянию
    /// </summary>
    public void ClearSpace()
    {
        if (seeds.Count == 0)
            Patch.GetComponent<PatchGrowth>().ClearPatch();
        for (var i = 0; i < seeds.Count; i++)
            Destroy(seedPlaces[i]);

        seedPlaces.RemoveAll(x => x);
        gameObject.SetActive(false);
        Save();
    }

    /// <summary>
    /// Отрисовывает превью задач
    /// </summary>
    private void RenderTaskPreviews()
    {
        var taskController = GetComponent<TaskController>();
        var renderPlace = taskController
            .RenderQuestsPreview(seeds[0].Name, chosenSeeds.Count)
            .transform;

        for (var i = 0; i < renderPlace.childCount; i++)
        {
            var task = renderPlace.GetChild(i).GetComponent<Task>();
            task.AddQuestItems = () =>
            {
                task.Details.ProgressAmount += chosenSeeds.Count;
                for (var j = 0; j < chosenSeeds.Count; j++)
                    DeleteUsedSeed(chosenSeeds[j], chosenSeedPlaces[j]);
                task.Save();

                UpdateChosenSeeds();
            };
        }
    }

    /// <summary>
    /// Обновляет внешний вид окна урожая в зависимости от количества выбранных плодов
    /// </summary>
    /// <param name="isOn">Флаг активации (нужен для корректной работы компонента Toggle)</param>
    private void UpdateChosenSeeds(bool isOn = false)
    {
        if (!gameObject.activeSelf) return;

        chosenSeeds = new List<Seed>();
        chosenSeedPlaces = new List<GameObject>();

        for (var i = 0; i < seeds.Count; i++)
        {
            if (!seedPlaces[i].GetComponentInChildren<Toggle>().isOn) continue;

            chosenSeeds.Add(seeds[i]);
            chosenSeedPlaces.Add(seedPlaces[i]);
        }

        GameObject.FindGameObjectWithTag("SellChosenBtn")
            .GetComponent<Button>()
            .interactable = chosenSeeds.Count > 0;

        chosenSeedsCounter.text = $"{chosenSeeds.Count}/{seeds.Count}";
        choseAllCheckmark.SetActive(chosenSeeds.Count == seedPlaces.Count);
        
        RenderTaskPreviews();
    }

    /// <summary>
    /// Удаляет выросшее семечко
    /// </summary>
    /// <param name="seed">Семечко</param>
    /// <param name="seedObj">Объект, в котором отрисовывается семечко</param>
    private void DeleteUsedSeed(Seed seed, GameObject seedObj)
    {
        seeds.Remove(seed);
        Destroy(seedObj);
        seedPlaces.Remove(seedObj);
        if (seedPlaces.Count == 0) ClearSpace();
        Statistics.UpdateGrowedSeeds(seed.Name);
    }

    /// <summary>
    /// Продаёт семечка
    /// </summary>
    /// <param name="seed">Семечко</param>
    private void Sell(Seed seed)
    {
        var inventory = Inventory.GetComponent<InventoryDrawer>().targetInventory;
        inventory.AddMoney(seed.Price);
        inventory.ChangeReputation(seed.Gabitus);
        Statistics.UpdateSoldSeeds(seed.Name);
        Statistics.UpdateGrowedSeeds(seed.Name);
        inventory.Save();
    }

    /// <summary>
    /// Сохраняет растущие семена
    /// </summary>
    private void Save()
    {
        if (seeds.Count == 0) return;

        PlayerPrefs.SetInt(Patch.name + "seedsCount", seeds.Count);
        for (var i = 0; i < seeds.Count; i++)
            PlayerPrefs.SetString(Patch.name + "seedElement" + i, seeds[i].ToString());
    }

    /// <summary>
    /// Деактивирует панель с превью заданий
    /// </summary>
    private void OnEnable()
    {
        UpdateChosenSeeds();

        var scenario = GameObject.FindGameObjectWithTag("TutorialHandler")?.GetComponent<Scenario>();

        // тутор для окна сбора урожая
        if (QSReader.Create("TutorialState").Exists("Tutorial_WaitForGrowing_Played"))
            scenario.Tutorial_HarvestPlace();
    }
}
