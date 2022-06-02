using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Mathematics;
using System;
using CI.QuickSave;

public class PatchDetails
{
    public string GrowingSeed { get; set; }
    public List<string> GrownSeeds { get; set; } = new List<string>();
    public double SecondsRemaining { get; set; }
    public DateTime LastCheckedTime { get; set; }
    public bool IsOccupied { get; set; }
    public bool TimerNeeded { get; set; }
    public double TimeSpeedBooster { get; set; } = 1;
}

public class PatchGrowth : MonoBehaviour
{
    [SerializeField] private Button Patch;
    [SerializeField] private RectTransform InventoryFrame;
    [SerializeField] private RectTransform HarvestWindow;

    [SerializeField] private Sprite wetGround;
    [SerializeField] private Sprite dryGround;

    [SerializeField] private Image plantImage;
    [SerializeField] private Image groundImage;

    [SerializeField] private GameObject blockerPrefab;
    [SerializeField] private GameObject infoContainer;
    [SerializeField] private GameObject optionsMenu;
    [SerializeField] private Transform farmSpotsContainer; // место для отрисовки блокера и инфо контейнера
    [SerializeField] private ConfirmationPanelLogic confirmationPanelPrefab;

    [SerializeField] private GameObject timerBG;
    [SerializeField] private Text growthText;

    public bool StopForTutorial;

    private PatchDetails details = new PatchDetails();

    private Seed growingSeed;
    private List<Seed> grownSeeds = new List<Seed>();
    private double secondsRemaining;
    private DateTime lastCheckedTime;
    private bool isOccupied;
    private bool timerNeeded;
    private double timeSpeedBooster = 1;

    private static GameObject infoBlocker;
    private static PatchGrowth lastPatchGrowth;
    private Transform startCointainerPlace;
    private int infoContainerOpenedTimes;

    private Inventory _inventory;
    private ConfirmationPanelLogic confPanel;

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
        timerBG.SetActive(true);
        infoContainerOpenedTimes = 0;
        growingSeed = seed;
        secondsRemaining = seed.GrowTime;
        lastCheckedTime = DateTime.Now;
        timeSpeedBooster = StopForTutorial ? 0 : 1;

        CreateGrownSeeds();
        Save();
    }

    /// <summary>
    /// Вызывается при клике.
    /// </summary>
    public void Clicked()
    {
        CloseActiveInfoContainer(false);

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
            if (secondsRemaining > 0)
                ToggleInfo(true);
        }

        // открывает меню урожая
        if (secondsRemaining <= 0)
        {
            if (grownSeeds.Count == 0)
            {
                ClearPatch();
                return;
            }
            
            HarvestWindow.GetComponent<HarvestProcessor>()
                .ShowHarvestMenu(grownSeeds, Patch.GetComponent<PatchGrowth>());
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
        timerBG.SetActive(false);
        growthText.text = "";
        isOccupied = false;
        growingSeed = null;
        grownSeeds = new List<Seed>();
        Save();
    }

    /// <summary>
    /// Закрывает активную информацию о росте.
    /// </summary>
    public void CloseActiveInfoContainer(bool resetOpenedTimes = true)
    {
        HideInfoContainer();

        Destroy(infoBlocker);
        infoBlocker = null;

        if (lastPatchGrowth == null) return;

        if (resetOpenedTimes)
            lastPatchGrowth.infoContainerOpenedTimes = 0;

        lastPatchGrowth.infoContainer.SetActive(false);
        lastPatchGrowth.optionsMenu.SetActive(false);
        lastPatchGrowth.infoContainer.transform.SetParent(lastPatchGrowth.startCointainerPlace, true);
    }

    /// <summary>
    /// Отменяет посадку растения.
    /// </summary>
    public void CancelPlant()
    {
        var canvas = GameObject.FindGameObjectWithTag("Canvas");
        confPanel = Instantiate(confirmationPanelPrefab, canvas.transform, false);
        confPanel.SetQuestion($"Выкопать {growingSeed.NameInRussian}?",
            "Потраченная энергия возвращена не будет.");

        confPanel.SetYesAction(() =>
        {
            ClearPatch();
            timerNeeded = false;
            ToggleInfo();
        });
    }

    /// <summary>
    /// Ускоряет рост семечка.
    /// </summary>
    public void SpeedUpSeed()
    {
        var coeff = 2;
        var playTutorial = false;

        var scenario = GameObject.FindGameObjectWithTag("TutorialHandler")?.GetComponent<Scenario>();

        // тутор для подтверждения ускорения
        if (QSReader.Create("TutorialState").Exists("Tutorial_SpeedUpItem_Played")
            && !QSReader.Create("TutorialState").Exists("Tutorial_ConfirmSpeedUp_Played", "TutorialSkipped"))
        {
            coeff = 60;
            playTutorial = true;
        }

        var ending = "";
        if (coeff % 10 > 1
            && coeff % 10 < 5
            && (coeff < 12 || coeff > 14))
            ending = "а";

        var canvas = GameObject.FindGameObjectWithTag("Canvas");
        confPanel = Instantiate(confirmationPanelPrefab, canvas.transform, false);
        confPanel.SetQuestion($"Ускорить {growingSeed.NameInRussian} в {coeff} раз{ending}?",
            "Для ускорения нужно будет посмотреть рекламу.");

        if (!playTutorial)
        {
            var adHandler = optionsMenu.gameObject.GetComponent<AdHandler>();
            adHandler.ShowAdButton = confPanel.YesButton;
            adHandler.ShowAdButton.interactable = false;
            adHandler.Init();
            confPanel.SetYesAction(() =>
                adHandler.SpeedUpAction = () => SetSeedSpeed((int) (timeSpeedBooster * coeff)));
        }
        else
        {
            GetComponent<Button>().enabled = false;
            confPanel.SetYesAction(() =>
            {
                StopForTutorial = false;
                timeSpeedBooster = 1;
                SetSeedSpeed(coeff);
            });

            scenario?.Tutorial_ConfirmSpeedUp();
        }
    }

    /// <summary>
    /// DEBUG Пропускает время роста.
    /// </summary>
    public void SkipSeedTime()
    {
        secondsRemaining = 0;
    }

    /// <summary>
    /// Сохраняет информацию о семечке.
    /// </summary>
    public void Save()
    {
        details.IsOccupied = isOccupied;
        details.TimerNeeded = timerNeeded;
        details.GrowingSeed = growingSeed != null ? growingSeed.ToString() : null;
        details.SecondsRemaining = secondsRemaining;
        details.LastCheckedTime = lastCheckedTime;
        details.TimeSpeedBooster = timeSpeedBooster;

        details.GrownSeeds = new List<string>();
        foreach (var seed in grownSeeds)
            details.GrownSeeds.Add(seed.ToString());

        var writer = QuickSaveWriter.Create("Field");
        writer.Write(Patch.name, details);
        writer.Commit();
    }

    /// <summary>
    /// Загружает информацию о семечке.
    /// </summary>
    private void Load()
    {
        var reader = QSReader.Create("Field");
        if (reader.Exists(Patch.name))
            details = reader.Read<PatchDetails>(Patch.name);

        isOccupied = details.IsOccupied;
        if (!isOccupied) return;

        timerNeeded = details.TimerNeeded;
        growingSeed = Seed.Create(details.GrowingSeed);
        timeSpeedBooster = details.TimeSpeedBooster;
        
        if (timeSpeedBooster == 0)
            timeSpeedBooster = 1;
        
        var timePassed = (DateTime.Now.Ticks - details.LastCheckedTime.Ticks) / 10000000;
        secondsRemaining = details.SecondsRemaining - timePassed * timeSpeedBooster;

        foreach (var seed in details.GrownSeeds)
            grownSeeds.Add(Seed.Create(seed));
    }

    /// <summary>
    /// Создаёт выращенные семена.
    /// </summary>
    private void CreateGrownSeeds()
    {
        FindObjectOfType<SFXManager>()?.Play(SoundEffect.PlantSeed);
        var plusSeeds =
            (int) Math.Round(UnityEngine.Random.value * (growingSeed.MaxAmount - growingSeed.MinAmount));
        for (var i = 0; i < growingSeed.MinAmount + plusSeeds; i++)
            grownSeeds.Add(SeedMutator.GetMutatedSeed(growingSeed));
    }

    /// <summary>
    /// Показывает таймер.
    /// </summary>
    private void ShowTimer()
    {
        infoContainer.SetActive(true);
        timerBG.SetActive(true);
    }

    /// <summary>
    /// Показывает информацию о росте.
    /// </summary>
    private void ShowInfoContainer()
    {
        if (infoBlocker == null)
            SpawnBlocker();

        if (startCointainerPlace == null)
            startCointainerPlace = transform;

        infoContainer.transform.SetParent(farmSpotsContainer, true);
        infoContainer.transform.SetAsLastSibling();
        infoContainer.SetActive(true);

        infoContainerOpenedTimes = 1;
        optionsMenu.SetActive(timerNeeded);

        var scenario = GameObject.FindGameObjectWithTag("TutorialHandler")?.GetComponent<Scenario>();
        if (scenario == null) return;

        // тутор для нажатия на кнопку ускорения семян
        if (QSReader.Create("TutorialState").Exists("Tutorial_ChooseItemToSpeedUp_Played"))
            scenario.Tutorial_SpeedUpItem();
    }

    /// <summary>
    /// Скрывает информацию о росте.
    /// </summary>
    private void HideInfoContainer()
    {
        if (startCointainerPlace != null)
            infoContainer.transform.SetParent(startCointainerPlace, true);

        infoContainer.SetActive(false);
        optionsMenu.SetActive(false);

        if (lastPatchGrowth != this) return;

        Destroy(infoBlocker);
        infoBlocker = null;
    }

    /// <summary>
    /// Создаёт инфо блокер.
    /// </summary>
    private void SpawnBlocker()
    {
        infoBlocker = Instantiate(blockerPrefab, farmSpotsContainer, false);
        infoBlocker.transform.SetAsFirstSibling();
        infoBlocker.AddComponent<LayoutElement>().ignoreLayout = true;
    }

    /// <summary>
    /// Переназначает обработчик кликов на блокер.
    /// </summary>
    private void SetBlockerListener()
    {
        var blockerOnClick = infoBlocker.GetComponent<Button>().onClick;
        blockerOnClick.RemoveAllListeners();
        blockerOnClick.AddListener(() => CloseActiveInfoContainer());
    }

    /// <summary>
    /// Тоггл для окна информации.
    /// </summary>
    /// <param name="isOn">Флаг, отвечающий за активацию окна.</param>
    private void ToggleInfo(bool isOn = false)
    {
        if (isOn)
        {
            if (lastPatchGrowth != this)
            {
                if (lastPatchGrowth != null)
                {
                    lastPatchGrowth.infoContainer.SetActive(false);
                    lastPatchGrowth.optionsMenu.SetActive(false);
                }

                ShowInfoContainer();
            }
            else
            {
                if (++infoContainerOpenedTimes % 2 == 0)
                    HideInfoContainer();
                else
                    ShowInfoContainer();
            }

            lastPatchGrowth = this;
            if (infoBlocker != null)
                SetBlockerListener();
        }
        else
            HideInfoContainer();
    }

    /// <summary>
    /// Завершает рост растения.
    /// </summary>
    private void EndGrowthCycle()
    {
        GetComponent<Button>().enabled = true;
        ToggleInfo();
        timerNeeded = false;
        plantImage.sprite = growingSeed.GrownSprite;
        ShowTimer();
        growthText.text = "ГОТОВО";
        optionsMenu.SetActive(false);
        timeSpeedBooster = 1;
        Save();

        if (confPanel != null)
            confPanel.gameObject.SetActive(false);
    }

    /// <summary>
    /// Задаёт скорость роста семечка.
    /// </summary>
    /// <param name="coeff">Коэффициент скорости</param>
    private void SetSeedSpeed(double coeff)
    {
        if (coeff > 0)
            timeSpeedBooster = coeff;

        Save();
        Debug.Log($"{growingSeed.NameInRussian}. Ускорение: {timeSpeedBooster}.");
    }

    private void Start()
    {
        _inventory ??= GameObject.Find("DataKeeper").GetComponent<Inventory>();
        Load();

        lastPatchGrowth = null;
        infoBlocker = null;
        optionsMenu.SetActive(false);

        if (isOccupied && secondsRemaining <= 0)
            EndGrowthCycle();
    }

    /// <summary>
    /// Используется как таймер.
    /// </summary>
    private void Update()
    {
        if (timerNeeded)
        {
            groundImage.sprite = wetGround;

            if (secondsRemaining > 0)
            {
                secondsRemaining -= Time.deltaTime * timeSpeedBooster;
                var timeSpan = TimeSpan.FromSeconds(math.round(secondsRemaining));
                growthText.text = Tools.TimeFormatter.Format(timeSpan);
                plantImage.sprite = growingSeed.GetGrowthStageSprite(secondsRemaining, growingSeed.GrowTime);
                lastCheckedTime = DateTime.Now;
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

        if (isOccupied && secondsRemaining <= 60)
            ShowTimer();
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
            Save();
    }
}
