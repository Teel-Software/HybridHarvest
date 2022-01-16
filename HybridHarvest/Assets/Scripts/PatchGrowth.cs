using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Mathematics;
using System;
using System.Linq;

public class PatchGrowth : MonoBehaviour
{
    public Button Patch;
    [SerializeField] RectTransform InventoryFrame;
    [SerializeField] RectTransform HarvestWindow;

    bool isOccupied;
    bool timerNeeded;
    public Seed growingSeed;
    public double time; //осталось расти
    public List<Seed> grownSeeds = new List<Seed>();
    private Image plantImage;
    private Image textBGImage;
    private Text growthText;
    private Inventory _inventory;
    private double _timeSpeedBooster = 1;

    /// <summary>
    /// Organizes plants on patch.
    /// </summary>
    private void Start()
    {
        _inventory ??= GameObject.Find("DataKeeper").GetComponent<Inventory>();

        var imagesInChildren = Patch.GetComponentsInChildren<Image>();
        plantImage = imagesInChildren[1];
        textBGImage = imagesInChildren[2];
        growthText = Patch.GetComponentInChildren<Text>();

        if (PlayerPrefs.GetInt(Patch.name + "occupied") == 1)
        {
            isOccupied = true;
            timerNeeded = true;
        }
        else
        {
            isOccupied = false;
            textBGImage.enabled = false;
            return;
        }

        growingSeed = ScriptableObject.CreateInstance<Seed>();
        growingSeed.SetValues(PlayerPrefs.GetString(Patch.name + "grows"));
        SpeedUpTutorSeed(growingSeed);

        var oldDate = DateTime.Parse(PlayerPrefs.GetString(Patch.name + "timeStart"));
        var timePassed = (DateTime.Now.Ticks - oldDate.Ticks) / 10000000;
        time = PlayerPrefs.GetInt(Patch.name + "time") - timePassed * _timeSpeedBooster;
        Patch.interactable = false;

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
    /// Used as a Timer
    /// </summary>
    private void Update()
    {
        if (timerNeeded)
        {
            if (time > 0)
            {
                time -= Time.deltaTime * _timeSpeedBooster;
                var timeSpan = TimeSpan.FromSeconds(math.round(time));
                growthText.text = Tools.TimeFormatter.Format(timeSpan);
                plantImage.sprite = growingSeed.GetGrowthStageSprite(time, growingSeed.GrowTime);
            }
            else
                EndGrowthCycle();
            textBGImage.enabled = true;
        }
    }

    /// <summary>
    /// Saves data when window closed
    /// </summary>
    private void OnDestroy()
    {
        PlayerPrefs.SetInt(Patch.name + "time", (int)time);
        PlayerPrefs.SetString(Patch.name + "timeStart", DateTime.Now.ToString());
        PlayerPrefs.SetInt(Patch.name + "seedsCount", grownSeeds.Count);
        for (var i = 0; i < grownSeeds.Count; i++)
        {
            PlayerPrefs.SetString(Patch.name + "grown" + i, grownSeeds[i].ToString());
        }
    }

    /// <summary>
    /// States that plant is ripe
    /// </summary>
    public void EndGrowthCycle()
    {
        timerNeeded = false;
        Patch.interactable = true;
        plantImage.sprite = growingSeed.GrownSprite;
        textBGImage.enabled = true;
        growthText.text = "ГОТОВО";
    }

    /// <summary>
    /// plants chosen seed on patch
    /// </summary>
    /// <param name="seed"></param>
    public void PlantIt(Seed seed)
    {
        //Consumes energy for planting (so bad)
        if (_inventory.Energy <= 0)
        {
            _inventory.GetComponent<NotificationCenter>()
                ?.Show("Недостаточно энергии!");
            return;
        }

        _inventory.ConsumeEnergy(1);
        Patch.interactable = false;
        isOccupied = true;
        growingSeed = seed;
        PlayerPrefs.SetInt(Patch.name + "occupied", isOccupied ? 1 : 0);
        PlayerPrefs.SetString(Patch.name + "grows", seed.ToString());
        time = seed.GrowTime;
        timerNeeded = true;
        SpeedUpTutorSeed(seed);

        var scenario = GameObject.FindGameObjectWithTag("TutorialHandler")?.GetComponent<Scenario>();

        // тутор для роста семечка
        if (QSReader.Create("TutorialState").Exists("Tutorial_StatPanel_Played"))
            scenario.Tutorial_WaitForGrowing();
    }

    /// <summary>
    /// Pocesses clicking
    /// </summary>
    public void Clicked()
    {
        if (!isOccupied)
        {
            InventoryFrame.GetComponent<InventoryDrawer>().GrowPlace = Patch;
            InventoryFrame.GetComponent<InventoryDrawer>().SetPurpose(PurposeOfDrawing.Plant);
            InventoryFrame.gameObject.SetActive(true);
        }
        //FindObjectOfType<SFXManager>().Play(SoundEffect.PlantSeed); //TODO make this shit play later
        if (grownSeeds.Count == 0 && growingSeed != null)
        {
            int plusSeeds = (int)Math.Round(UnityEngine.Random.value * (growingSeed.maxAmount - growingSeed.minAmount));
            for (var i = 0; i < growingSeed.minAmount + plusSeeds; i++)
                grownSeeds.Add(SeedMutator.GetMutatedSeed(growingSeed));
        }
        if (grownSeeds.Count != 0)
        {
            HarvestWindow.GetComponent<HarvestProcessor>().ShowHarvestMenu(grownSeeds, Patch);
            HarvestWindow.gameObject.SetActive(true);
        }
    }

    public void ClearPatch()
    {
        plantImage.sprite = Resources.Load<Sprite>("Transparent");
        textBGImage.enabled = false;
        growthText.text = "";
        isOccupied = false;
        if (growingSeed == null) return;

        growingSeed = null;
        PlayerPrefs.SetInt(Patch.name + "occupied", isOccupied ? 1 : 0);
    }

    /// <summary>
    /// Ускоряет рост обучающих семян
    /// </summary>
    /// <param name="seed">Семечко</param>
    private void SpeedUpTutorSeed(Seed seed)
    {
        if (seed.NameInRussian == "Обучающий картофель")
            _timeSpeedBooster = 30;
    }
}
