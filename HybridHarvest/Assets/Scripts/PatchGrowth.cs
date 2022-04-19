using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Mathematics;
using System;

public class PatchGrowth : MonoBehaviour
{
    public Button Patch;

    [SerializeField] private RectTransform InventoryFrame;
    [SerializeField] private RectTransform HarvestWindow;

    [SerializeField] private Sprite wetGround;
    [SerializeField] private Sprite dryGround;

    [SerializeField] private Image plantImage;
    [SerializeField] private Image groundImage;

    [SerializeField] private GameObject blockerPrefab;
    [SerializeField] private GameObject infoContainer;
    [SerializeField] private GameObject cancelButton;
    [SerializeField] private ConfirmationPanelLogic confirmationPanelPrefab;

    [SerializeField] private Image timerBGImage;
    [SerializeField] private Text growthText;

    public Seed growingSeed;
    public double time; //осталось расти
    public List<Seed> grownSeeds = new List<Seed>();

    private static GameObject infoBlocker;
    private static PatchGrowth lastPatchGrowth;
    private int infoContainerOpenedTimes;

    private bool isOccupied;
    private bool timerNeeded;

    private Inventory _inventory;
    private ConfirmationPanelLogic confPanel;
    private double _timeSpeedBooster = 1;

    /// <summary>
    /// Садит семечко на грядку.
    /// </summary>
    /// <param name="seed">Семечко.</param>
    public void PlantIt(Seed seed)
    {
        if (_inventory.Energy <= 0)
        {
            _inventory.GetComponent<NotificationCenter>()
                ?.Show("Недостаточно энергии!");
            return;
        }

        _inventory.ConsumeEnergy(1);
        isOccupied = true;
        timerNeeded = true;
        timerBGImage.enabled = true;
        infoContainerOpenedTimes = 0;
        growingSeed = seed;
        time = seed.GrowTime;

        ToggleInfo();
        SpeedUpTutorSeed(seed);
        SavePlanting(seed);

        var scenario = GameObject.FindGameObjectWithTag("TutorialHandler")?.GetComponent<Scenario>();

        // тутор для роста семечка
        if (QSReader.Create("TutorialState").Exists("Tutorial_StatPanel_Played"))
            scenario.Tutorial_WaitForGrowing();
    }

    /// <summary>
    /// Отменяет посадку растения.
    /// </summary>
    public void CancelPlant()
    {
        var canvas = GameObject.FindGameObjectWithTag("Canvas");
        confPanel = Instantiate(confirmationPanelPrefab, canvas.transform, false);
        confPanel.SetQuestion($"{growingSeed.NameInRussian}: отменить посадку?",
            "Потраченная энергия возвращена не будет.");
        confPanel.SetAction(() =>
        {
            ClearPatch();
            timerNeeded = false;
            ToggleInfo();
        });
    }

    /// <summary>
    /// Вызывается при клике.
    /// </summary>
    public void Clicked()
    {
        ToggleInfo();

        // открывает инвентарь
        if (!isOccupied)
        {
            InventoryFrame.GetComponent<InventoryDrawer>().GrowPlace = Patch;
            InventoryFrame.GetComponent<InventoryDrawer>().SetPurpose(PurposeOfDrawing.Plant);
            InventoryFrame.gameObject.SetActive(true);

            if (lastPatchGrowth != null)
                lastPatchGrowth.infoContainerOpenedTimes = 0;
        }
        else
        {
            if (time > 0)
                ToggleInfo(true);
        }

        //FindObjectOfType<SFXManager>().Play(SoundEffect.PlantSeed); //TODO make this shit play later
        if (grownSeeds.Count == 0 && growingSeed != null)
        {
            var plusSeeds =
                (int) Math.Round(UnityEngine.Random.value * (growingSeed.maxAmount - growingSeed.minAmount));
            for (var i = 0; i < growingSeed.minAmount + plusSeeds; i++)
                grownSeeds.Add(SeedMutator.GetMutatedSeed(growingSeed));
        }

        // открывает меню урожая
        if (grownSeeds.Count != 0 && time <= 0)
        {
            HarvestWindow.GetComponent<HarvestProcessor>().ShowHarvestMenu(grownSeeds, Patch);
            HarvestWindow.gameObject.SetActive(true);

            if (lastPatchGrowth != null)
                lastPatchGrowth.infoContainerOpenedTimes = 0;
        }
    }

    /// <summary>
    /// Приводит грядку к начальному виду.
    /// </summary>
    public void ClearPatch()
    {
        plantImage.sprite = Resources.Load<Sprite>("Transparent");
        timerBGImage.enabled = false;
        growthText.text = "";
        isOccupied = false;
        growingSeed = null;
        SaveClearing();
    }

    /// <summary>
    /// Сохраняет информацию о посадке семечка.
    /// </summary>
    /// <param name="seed">Семечко.</param>
    private void SavePlanting(Seed seed)
    {
        PlayerPrefs.SetInt(Patch.name + "occupied", 1);
        PlayerPrefs.SetString(Patch.name + "grows", seed.ToString());
    }

    /// <summary>
    /// Сохраняет информацию о том, что грядка пуста.
    /// </summary>
    private void SaveClearing()
    {
        PlayerPrefs.SetInt(Patch.name + "occupied", 0);
    }

    /// <summary>
    /// Показывает таймер.
    /// </summary>
    private void ShowTimer()
    {
        infoContainer.SetActive(true);
        timerBGImage.enabled = true;
    }

    /// <summary>
    /// Показывает информацию о росте.
    /// </summary>
    private void ShowInfoContainer()
    {
        infoContainer.transform.SetParent(transform.parent.parent, true);
        infoContainer.transform.SetAsLastSibling();
        infoContainer.SetActive(true);

        infoContainerOpenedTimes = 1;
        infoBlocker.SetActive(true);
        cancelButton.SetActive(timerNeeded);
    }

    /// <summary>
    /// Скрывает информацию о росте.
    /// </summary>
    private void HideInfoContainer()
    {
        infoContainer.SetActive(false);
        cancelButton.SetActive(false);
        infoBlocker?.SetActive(false);
        lastPatchGrowth?.cancelButton.SetActive(false);
        lastPatchGrowth?.infoContainer.SetActive(false);
    }

    /// <summary>
    /// Создаёт инфо блокер.
    /// </summary>
    private void SpawnBlocker()
    {
        infoBlocker = Instantiate(blockerPrefab, transform.parent.parent, false);
        infoBlocker.transform.SetAsFirstSibling();
        infoBlocker.AddComponent<LayoutElement>().ignoreLayout = true;
    }

    /// <summary>
    /// Переназначает обработчик кликов на блокер.
    /// </summary>
    private void ResetBlockerListener()
    {
        var blockerOnClick = infoBlocker.GetComponent<Button>().onClick;
        blockerOnClick.RemoveAllListeners();
        blockerOnClick.AddListener(() =>
        {
            ToggleInfo();

            if (lastPatchGrowth != null)
                lastPatchGrowth.infoContainerOpenedTimes = 0;
        });
    }

    /// <summary>
    /// Тоггл для окна информации.
    /// </summary>
    /// <param name="isOn">Флаг, отвечающий за активацию окна.</param>
    private void ToggleInfo(bool isOn = false)
    {
        if (infoBlocker == null)
            SpawnBlocker();

        if (isOn)
        {
            if (lastPatchGrowth != this)
            {
                if (lastPatchGrowth != null)
                {
                    lastPatchGrowth.infoContainer.SetActive(false);
                    lastPatchGrowth.cancelButton.SetActive(false);
                }

                ShowInfoContainer();
            }
            else
            {
                if (++infoContainerOpenedTimes % 2 == 0)
                    ToggleInfo();
                else
                    ShowInfoContainer();
            }

            lastPatchGrowth = this;
            ResetBlockerListener();
        }
        else
            HideInfoContainer();
    }

    /// <summary>
    /// Завершает рост растения.
    /// </summary>
    private void EndGrowthCycle()
    {
        timerNeeded = false;
        plantImage.sprite = growingSeed.GrownSprite;
        ShowTimer();
        growthText.text = "ГОТОВО";
        cancelButton.SetActive(false);

        if (confPanel != null)
            confPanel.gameObject.SetActive(false);
    }

    /// <summary>
    /// Ускоряет рост обучающих семян.
    /// </summary>
    /// <param name="seed">Семечко</param>
    private void SpeedUpTutorSeed(Seed seed)
    {
        _timeSpeedBooster = seed.NameInRussian switch
        {
            "Обучающий картофель" => 30,
            "Обучающий помидор" => 60,
            _ => 1,
        };
    }

    /// <summary>
    /// Organizes plants on patch.
    /// </summary>
    private void Start()
    {
        _inventory ??= GameObject.Find("DataKeeper").GetComponent<Inventory>();

        if (PlayerPrefs.GetInt(Patch.name + "occupied") == 1)
        {
            isOccupied = true;
            timerNeeded = true;
            cancelButton.SetActive(true);
        }
        else
        {
            isOccupied = false;
            timerBGImage.enabled = false;
            return;
        }

        growingSeed = ScriptableObject.CreateInstance<Seed>();
        growingSeed.SetValues(PlayerPrefs.GetString(Patch.name + "grows"));
        SpeedUpTutorSeed(growingSeed);

        var oldDate = DateTime.Parse(PlayerPrefs.GetString(Patch.name + "timeStart"));
        var timePassed = (DateTime.Now.Ticks - oldDate.Ticks) / 10000000;
        time = PlayerPrefs.GetInt(Patch.name + "time") - timePassed * _timeSpeedBooster;

        var seedsCount = PlayerPrefs.GetInt(Patch.name + "seedsCount");
        for (var i = 0; i < seedsCount; i++)
        {
            var seed = ScriptableObject.CreateInstance<Seed>();
            seed.SetValues(PlayerPrefs.GetString(Patch.name + "grown" + i));
            grownSeeds.Add(seed);
        }

        if (time <= 0)
            EndGrowthCycle();

        lastPatchGrowth = null;
        infoBlocker = null;
        cancelButton.SetActive(false);
    }

    /// <summary>
    /// Используется как таймер.
    /// </summary>
    private void Update()
    {
        if (timerNeeded)
        {
            groundImage.sprite = wetGround;

            if (time > 0)
            {
                time -= Time.deltaTime * _timeSpeedBooster;
                var timeSpan = TimeSpan.FromSeconds(math.round(time));
                growthText.text = Tools.TimeFormatter.Format(timeSpan);
                plantImage.sprite = growingSeed.GetGrowthStageSprite(time, growingSeed.GrowTime);
            }
            else
            {
                EndGrowthCycle();
            }
        }
        else
        {
            groundImage.sprite = dryGround;
        }

        if (isOccupied && time <= 60)
            ShowTimer();
    }

    /// <summary>
    /// Сохраняет данные при закрытии приложения.
    /// </summary>
    private void OnDestroy()
    {
        PlayerPrefs.SetInt(Patch.name + "time", (int) time);
        PlayerPrefs.SetString(Patch.name + "timeStart", DateTime.Now.ToString());
        PlayerPrefs.SetInt(Patch.name + "seedsCount", grownSeeds.Count);

        for (var i = 0; i < grownSeeds.Count; i++)
            PlayerPrefs.SetString(Patch.name + "grown" + i, grownSeeds[i].ToString());
    }
}
