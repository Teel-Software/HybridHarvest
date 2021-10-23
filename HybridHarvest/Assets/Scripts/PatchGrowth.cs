using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Mathematics;
using System;

public class PatchGrowth : MonoBehaviour
{
    [SerializeField] Button Patch;
    [SerializeField] RectTransform InventoryFrame;
    [SerializeField] RectTransform VegItem;

    bool isOccupied;
    bool timerNeeded;
    Seed growingSeed;
    public double time;
    private Image plantImage;
    private Image textBGImage;
    private Text growthText;
    
    /// <summary>
    /// Organizes plants on patch.
    /// </summary>
    private void Start()
    {
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

        DateTime oldDate;
        oldDate = DateTime.Parse(PlayerPrefs.GetString(Patch.name + "timeStart"));
        var timePassed = DateTime.Now - oldDate;
        var timeSpan = new TimeSpan(timePassed.Days, timePassed.Hours, timePassed.Minutes, timePassed.Seconds);
        time = PlayerPrefs.GetInt(Patch.name + "time") - timeSpan.TotalSeconds;
        Patch.interactable = false;

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
                growthText.text = math.round(time).ToString();
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
        GameObject.Find("DataKeeper").GetComponent<Inventory>().ConsumeEnergy(1);
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
        if (time > 0) return;
        if (!isOccupied)
        {
            InventoryFrame.GetComponent<Drawinventory>().GrowPlace = Patch;
            InventoryFrame.gameObject.SetActive(true);
        }
        //FindObjectOfType<SFXManager>().Play(SoundEffect.PlantSeed); //TODO make this shit play later
        plantImage.sprite = Resources.Load<Sprite>("Transparent");
        textBGImage.enabled = false;
        growthText.text = "";
        isOccupied = false;
        if (growingSeed == null) return;
        /*for (var i = 0; i < growingSeed.Amount; i++)
        {
            var newSeed = MutateSeed(growingSeed);
            InventoryFrame.GetComponent<Drawinventory>().targetInventory.AddItem(newSeed);
        }*/
        //var p = GameObject.Find("HarvestPlace");
        VegItem.GetComponent<HarvestProcessor>().ParentSeed = growingSeed;
        VegItem.GetComponent<HarvestProcessor>().InventoryFrame = InventoryFrame;
        VegItem.gameObject.SetActive(true);
        //VegItem.GetComponent<HarvestProcessor>().ParentSeed = growingSeed;

        growingSeed = null;
        PlayerPrefs.SetInt(Patch.name + "occupied", isOccupied ? 1 : 0);
    }

    /// <summary>
    /// mutation mechanic
    /// </summary>
    /// <param name="oldSeed"></param>
    /// <returns></returns>
    public Seed MutateSeed(Seed oldSeed)
    {
        var procentage = UnityEngine.Random.value;
        var newSeed = ScriptableObject.CreateInstance<Seed>();
        newSeed.SetValues(oldSeed.ToString());
        var plusAmount = UnityEngine.Random.value;
        if (procentage < 0.5 && newSeed.Gabitus <= 100)
        {
            newSeed.Gabitus += (int)(plusAmount * 5 + 1);
            newSeed.Price += (int)(plusAmount * 5 + 1);
        }
        else if (newSeed.Taste <= 100)
        {
            newSeed.Taste += (int)(plusAmount * 5 + 1);
            //newSeed.Price += (int)(plusAmount * 5 + 1);
        }
        return newSeed;
    }
}
