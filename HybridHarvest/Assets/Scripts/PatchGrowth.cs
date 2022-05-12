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
    [SerializeField] private GameObject optionsMenu;
    [SerializeField] private Transform farmSpotsContainer; // место для отрисовки блокера и инфо контейнера
    [SerializeField] private ConfirmationPanelLogic confirmationPanelPrefab;

    [SerializeField] private GameObject timerBG;
    [SerializeField] private Text growthText;

    public Seed growingSeed;
    public double time; //осталось расти
    public List<Seed> grownSeeds = new List<Seed>();

    private static GameObject infoBlocker;
    private static PatchGrowth lastPatchGrowth;
    private Transform startCointainerPlace;
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
        timerBG.SetActive(true);
        infoContainerOpenedTimes = 0;
        growingSeed = seed;
        time = seed.GrowTime;

        SetSeedSpeed(seed);
        SavePlanting(seed);
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
            if (time > 0)
                ToggleInfo(true);
        }

        //FindObjectOfType<SFXManager>().Play(SoundEffect.PlantSeed); //TODO make this shit play later
        if (grownSeeds.Count == 0 && growingSeed != null)
        {
            var plusSeeds =
                (int) Math.Round(UnityEngine.Random.value * (growingSeed.MaxAmount - growingSeed.MinAmount));
            for (var i = 0; i < growingSeed.MinAmount + plusSeeds; i++)
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
        timerBG.SetActive(false);
        growthText.text = "";
        isOccupied = false;
        growingSeed = null;
        grownSeeds = new List<Seed>();
        SaveClearing();
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
        // confPanel.SetNoAction(() => CloseActiveInfoContainer());
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
                adHandler.SpeedUpAction = () => SetSeedSpeed((int) (_timeSpeedBooster * coeff)));
        }
        else
        {
            GetComponent<Button>().enabled = false;
            confPanel.SetYesAction(() => SetSeedSpeed((int) (_timeSpeedBooster * coeff)));
            scenario?.Tutorial_ConfirmSpeedUp();
        }

        // confPanel.SetNoAction(() => CloseActiveInfoContainer());
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

        // тутор для нажатия на кнопку ускорения семян
        if (QSReader.Create("TutorialState").Exists("Tutorial_ChooseItemToSpeedUp_Played"))
            scenario?.Tutorial_SpeedUpItem();
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

        if (confPanel != null)
            confPanel.gameObject.SetActive(false);
    }

    /// <summary>
    /// Задаёт скорость роста семечка в зависимости от названия.
    /// </summary>
    /// <param name="seed">Семечко</param>
    private void SetSeedSpeed(Seed seed)
    {
        _timeSpeedBooster = seed.NameInRussian switch
        {
            "Обучающий картофель" => 30,
            "Обучающий помидор" => 60,
            _ => 1,
        };
    }

    /// <summary>
    /// Задаёт скорость роста семечка.
    /// </summary>
    /// <param name="coeff">Коэффициент скорости</param>
    private void SetSeedSpeed(int coeff)
    {
        if (coeff > 0)
            _timeSpeedBooster = coeff;

        Debug.Log($"Ускорение: {_timeSpeedBooster}");
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
        }
        else
        {
            isOccupied = false;
            return;
        }
        
        growingSeed = Seed.Create(PlayerPrefs.GetString(Patch.name + "grows"));
        SetSeedSpeed(growingSeed);

        var oldDate = DateTime.Parse(PlayerPrefs.GetString(Patch.name + "timeStart"));
        var timePassed = (DateTime.Now.Ticks - oldDate.Ticks) / 10000000;
        time = PlayerPrefs.GetInt(Patch.name + "time") - timePassed * _timeSpeedBooster;

        var seedsCount = PlayerPrefs.GetInt(Patch.name + "seedsCount");
        for (var i = 0; i < seedsCount; i++)
        {
            var seed = Seed.Create(PlayerPrefs.GetString(Patch.name + "grown" + i));
            grownSeeds.Add(seed);
        }

        lastPatchGrowth = null;
        infoBlocker = null;
        optionsMenu.SetActive(false);

        if (time <= 0)
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
