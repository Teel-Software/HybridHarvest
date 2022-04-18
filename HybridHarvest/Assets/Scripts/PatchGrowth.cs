using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Mathematics;
using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using UnityEngine.Serialization;

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

    [SerializeField] private Image timerBGImage;
    [SerializeField] private Text growthText;

    private static GameObject lastInfoContainer;
    private static GameObject infoBlocker;
    private int infoContainerOpenedTimes;

    public Seed growingSeed;
    public double time; //осталось расти
    public List<Seed> grownSeeds = new List<Seed>();
    private bool isOccupied;
    private bool timerNeeded;

    private Inventory _inventory;
    private double _timeSpeedBooster = 1;

    /// <summary>
    /// plants chosen seed on patch
    /// </summary>
    public void PlantIt(Seed seed)
    {
        if (_inventory.Energy <= 0)
        {
            _inventory.GetComponent<NotificationCenter>()
                ?.Show("Недостаточно энергии!");
            return;
        }

        //Consumes energy for planting (so bad)
        _inventory.ConsumeEnergy(1);
        isOccupied = true;
        growingSeed = seed;
        PlayerPrefs.SetInt(Patch.name + "occupied", isOccupied ? 1 : 0);
        PlayerPrefs.SetString(Patch.name + "grows", seed.ToString());
        time = seed.GrowTime;
        timerNeeded = true;
        timerBGImage.enabled = true;
        ToggleInfo();
        SpeedUpTutorSeed(seed);

        var scenario = GameObject.FindGameObjectWithTag("TutorialHandler")?.GetComponent<Scenario>();

        // тутор для роста семечка
        if (QSReader.Create("TutorialState").Exists("Tutorial_StatPanel_Played"))
            scenario.Tutorial_WaitForGrowing();
    }

    /// <summary>
    /// Отменяет посадку растения
    /// </summary>
    public void CancelPlant()
    {
        ClearPatch();
        timerNeeded = false;
        cancelButton.SetActive(false);
        ToggleInfo();
    }

    /// <summary>
    /// Pocesses clicking
    /// </summary>
    public void Clicked()
    {
        ToggleInfo();

        if (!isOccupied)
        {
            InventoryFrame.GetComponent<InventoryDrawer>().GrowPlace = Patch;
            InventoryFrame.GetComponent<InventoryDrawer>().SetPurpose(PurposeOfDrawing.Plant);
            InventoryFrame.gameObject.SetActive(true);
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

        if (grownSeeds.Count != 0 && time <= 0)
        {
            HarvestWindow.GetComponent<HarvestProcessor>().ShowHarvestMenu(grownSeeds, Patch);
            HarvestWindow.gameObject.SetActive(true);
        }
    }

    public void ClearPatch()
    {
        plantImage.sprite = Resources.Load<Sprite>("Transparent");
        timerBGImage.enabled = false;
        growthText.text = "";
        isOccupied = false;
        if (growingSeed == null) return;

        growingSeed = null;
        PlayerPrefs.SetInt(Patch.name + "occupied", isOccupied ? 1 : 0);
    }

    private void ShowOnlyTimer()
    {
        infoContainer.SetActive(true);
        timerBGImage.enabled = true;
        cancelButton.SetActive(false);
    }

    private void ShowInfoContainer()
    {
        var canvas = GameObject.FindGameObjectWithTag("Canvas");
        infoContainer.transform.SetParent(canvas.transform, true);
        infoContainer.transform.SetAsLastSibling();

        infoContainerOpenedTimes = 1;
        infoContainer.SetActive(true);
        cancelButton.SetActive(timerNeeded);
        infoBlocker.SetActive(true);
    }

    private void ToggleInfo(bool isOn = false)
    {
        if (infoBlocker == null)
        {
            infoBlocker = Instantiate(blockerPrefab, transform.parent.parent, false);
            infoBlocker.transform.SetAsFirstSibling();
            infoBlocker.AddComponent<LayoutElement>().ignoreLayout = true;
        }

        if (isOn)
        {
            if (lastInfoContainer != infoContainer)
            {
                lastInfoContainer?.SetActive(false);
                ShowInfoContainer();
            }
            else
            {
                infoContainerOpenedTimes++;
                if (infoContainerOpenedTimes % 2 == 0)
                    ToggleInfo();
                else
                    ShowInfoContainer();
            }

            lastInfoContainer = infoContainer;

            var blockerOnClick = infoBlocker.GetComponent<Button>().onClick;
            blockerOnClick.RemoveAllListeners();
            blockerOnClick.AddListener(() => ToggleInfo());
        }
        else
        {
            infoContainer?.SetActive(false);
            lastInfoContainer?.SetActive(false);
            infoBlocker.SetActive(false);
        }
    }

    /// <summary>
    /// States that plant is ripe
    /// </summary>
    private void EndGrowthCycle()
    {
        timerNeeded = false;
        plantImage.sprite = growingSeed.GrownSprite;
        ShowOnlyTimer();
        growthText.text = "ГОТОВО";
        cancelButton.SetActive(false);
    }

    /// <summary>
    /// Ускоряет рост обучающих семян
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

    }

    /// <summary>
    /// Используется как таймер
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
            ShowOnlyTimer();
    }

    /// <summary>
    /// Сохраняет данные при закрытии приложения
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
