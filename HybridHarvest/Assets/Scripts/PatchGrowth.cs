using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Mathematics;
using System;
using System.Linq;

public class PatchGrowth : MonoBehaviour
{
    [SerializeField] Button Patch;
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

    /// <summary>
    /// Organizes plants on patch.
    /// </summary>
    private void Start()
    {
        _inventory = GameObject.Find("DataKeeper").GetComponent<Inventory>();

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

        DateTime oldDate = DateTime.Parse(PlayerPrefs.GetString(Patch.name + "timeStart"));
        var timePassed = (DateTime.Now.Ticks - oldDate.Ticks) / 10000000;
        time = PlayerPrefs.GetInt(Patch.name + "time") - timePassed;
        Patch.interactable = false;

        var seedsCount = PlayerPrefs.GetInt(Patch.name + "seedsCount");
        for (var i = 0; i < seedsCount; i++)
        {
            var seed = ScriptableObject.CreateInstance<Seed>();
            seed.SetValues(PlayerPrefs.GetString(Patch.name + "grown" + i.ToString()));
            grownSeeds.Add(seed);
        }
        if (time <= 0)
            EndGrowthCycle();
    }

    /// <summary>
    /// Used as Timer.
    /// </summary>
    private void Update()
    {
        if (timerNeeded)
        {
            if (time > 0)
            {
                if (time < (double)growingSeed.GrowTime / 2)
                    plantImage.sprite = growingSeed.SproutSprite;
                time -= Time.deltaTime;
                var formatTime = TimeSpan.FromSeconds(math.round(time));
                //growthText.text = formatTime.ToString();
                if (formatTime.Hours > 9)
                    growthText.text = formatTime.Hours.ToString() + " ч.";
                else
                {
                    if (formatTime.Hours != 0)
                        growthText.text = formatTime.Hours.ToString() + " ч. " + formatTime.Minutes.ToString() + " м.";
                    else
                    {
                        if (formatTime.Minutes > 9)
                            growthText.text = formatTime.Minutes.ToString() + " м.";
                        else
                        {
                            if (formatTime.Minutes != 0)
                                growthText.text = formatTime.Minutes.ToString() + " м. " + formatTime.Seconds.ToString() + " c.";
                            else
                                growthText.text = formatTime.Seconds.ToString() + " c.";
                        }
                    }
                }
                textBGImage.enabled = true;
            }
            else
                EndGrowthCycle();
        }
    }

    /// <summary>
    /// Saves data when window closed
    /// </summary>
    void OnDestroy()
    {
        PlayerPrefs.SetInt(Patch.name + "time", (int)time);
        PlayerPrefs.SetString(Patch.name + "timeStart", DateTime.Now.ToString());
        PlayerPrefs.SetInt(Patch.name + "seedsCount", grownSeeds.Count);
        for (var i = 0; i < grownSeeds.Count; i++)
        {
            PlayerPrefs.SetString(Patch.name + "grown" + i.ToString(), grownSeeds[i].ToString());
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
        if (_inventory.Energy <= 0) return;
        _inventory.ConsumeEnergy(1);
        Patch.interactable = false;
        isOccupied = true;
        growingSeed = seed;
        PlayerPrefs.SetInt(Patch.name + "occupied", isOccupied ? 1 : 0);
        PlayerPrefs.SetString(Patch.name + "grows", seed.ToString());
        time = seed.GrowTime;
        timerNeeded = true;
    }

    /// <summary>
    /// Pocesses clicking
    /// </summary>
    public void Clicked()
    {
        if (!isOccupied)
        {
            InventoryFrame.GetComponent<Drawinventory>().GrowPlace = Patch;
            InventoryFrame.GetComponent<Drawinventory>().SetPurpose(PurposeOfDrawing.Plant);
            InventoryFrame.gameObject.SetActive(true);
        }
        //FindObjectOfType<SFXManager>().Play(SoundEffect.PlantSeed); //TODO make this shit play later
        if (grownSeeds.Count == 0 && growingSeed != null)
        {
            int plusSeeds = (int)Math.Round(UnityEngine.Random.value * (growingSeed.maxAmount - growingSeed.minAmount));
            for (var i = 0; i < growingSeed.minAmount + plusSeeds; i++)
                grownSeeds.Add(MutateSeed(growingSeed));
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

    private Seed MutateSeed(Seed oldSeed)
    {
        var changingStatsAmount = getAmountOfChangingStats(oldSeed.MutationPossibility);

        bool[] index = new bool[5];
        var t = -1;
        while(changingStatsAmount > 0)
        {
            t = (int)Math.Round((UnityEngine.Random.value * 100) % 4);
            if (!index[t])
            {
                changingStatsAmount--;
                index[t] = true;
            }
        }

        var newStats = MutateStats(oldSeed, index);
        var newSeed = ScriptableObject.CreateInstance<Seed>();
        newSeed.SetValues(oldSeed.ToString());
        newSeed.Gabitus = newStats[0];
        newSeed.Taste = newStats[1];
        newSeed.GrowTime = newStats[2];
        newSeed.minAmount = newStats[3];
        newSeed.MutationPossibility =  (MutationChance)newStats[4];
        newSeed.maxAmount = newStats[5];
        return newSeed;
    }

    private int getAmountOfChangingStats(MutationChance basicMutation)
    {
        var mutation = (int)basicMutation;
        var percentage = UnityEngine.Random.value;
        if (percentage <= 0.2)
            return 2+ mutation;
        if (percentage <= 0.4)
            return 1 + mutation;
        if (percentage <= 0.6)
            return mutation;
        if (percentage <= 0.6)
            return (mutation-1)>0 ? mutation - 1 : 0;
        if (percentage <= 0.8)
            return (mutation - 2) > 0 ? mutation - 2 : 0;
        return 0;
    }

    private int[] MutateStats(Seed oldSeed, bool[] index)
    {
        Tuple<int, int[]>[] statsData = {
        Tuple.Create(oldSeed.Gabitus, oldSeed.LevelData.Gabitus.Keys.ToArray()),
        Tuple.Create(oldSeed.Taste, oldSeed.LevelData.Taste.Keys.ToArray()),
        Tuple.Create(oldSeed.GrowTime, oldSeed.LevelData.GrowTime.Keys.ToArray()),
        Tuple.Create(oldSeed.minAmount, oldSeed.LevelData.MinAmount.Keys.ToArray()),
        Tuple.Create((int)oldSeed.MutationPossibility, oldSeed.LevelData.MutationChance.Keys.Select(x => (int)x).ToArray()),
        Tuple.Create(oldSeed.maxAmount, oldSeed.LevelData.MaxAmount.Keys.ToArray())
        };
        List<int> stats = new List<int>();
        for (var i = 0; i < statsData.Length -1; i++)
        {
            if (index[i] && Array.IndexOf(statsData[i].Item2, statsData[i].Item1) + 1 < statsData[i].Item2.Length)
            {
                stats.Add(statsData[i].Item2[Array.IndexOf(statsData[i].Item2, statsData[i].Item1) + 1]);
            }
            else
                stats.Add(statsData[i].Item1);
        }

        if (stats[3] != oldSeed.minAmount)
            stats.Add(statsData.Last().Item2[Array.IndexOf(statsData.Last().Item2, statsData.Last().Item1) + 1]);
        else
            stats.Add(statsData.Last().Item1);
        
        return stats.ToArray();
    }
}
