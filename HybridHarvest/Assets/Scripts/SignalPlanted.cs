using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Mathematics;
using System;

public class SignalPlanted : MonoBehaviour
{
    [SerializeField] Button Patch;
    [SerializeField] RectTransform InventoryFrame;

    bool isOccupied;
    bool timerNeeded;
    Seed growingSeed;
    public double time;
    private Image plantImage;
    private Image textBGImage;
    private Text growthText;
    
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
        }
        if (!isOccupied) return;
        
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

    void OnDestroy()
    {
        PlayerPrefs.SetInt(Patch.name + "time", (int)time);
        PlayerPrefs.SetString(Patch.name + "timeStart", DateTime.Now.ToString());
    }

    public void EndGrowthCycle()
    {
        timerNeeded = false;
        Patch.interactable = true;
        plantImage.sprite = growingSeed.GrownSprite;
        textBGImage.enabled = true;
        growthText.text = "ГОТОВО";
    }

    public void PlantIt(Seed seed)
    {
        Patch.interactable = false;
        isOccupied = true;
        growingSeed = seed;
        PlayerPrefs.SetInt(Patch.name + "occupied", isOccupied ? 1 : 0);
        PlayerPrefs.SetString(Patch.name + "grows", seed.ToString());
        time = seed.GrowTime;
        timerNeeded = true;
    }

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
        for (var i = 0; i < growingSeed.Amount; i++)
        {
            var newSeed = MutateSeed(growingSeed, i);
            InventoryFrame.GetComponent<Drawinventory>().targetInventory.AddItem(newSeed);
        }
        //InventoryFrame.GetComponent<Drawinventory>().targetInventory.AddItem(nowGrows);
        growingSeed = null;
        PlayerPrefs.SetInt(Patch.name + "occupied", isOccupied ? 1 : 0);
    }

    public Seed MutateSeed(Seed oldSeed, int i)
    {
        var procentage = UnityEngine.Random.value;
        var newSeed = ScriptableObject.CreateInstance<Seed>();
        newSeed.SetValues(oldSeed.ToString());
        var plusAmount = UnityEngine.Random.value;
        if (procentage < 0.5 && newSeed.Gabitus <= 100)
            newSeed.Gabitus += (int)(plusAmount * 5 + 1);
        else if (newSeed.Taste <= 100)
        {
            newSeed.Taste += (int)(plusAmount * 5 + 1);
            newSeed.Price += (int)(plusAmount * 5 + 1);
        }
        return newSeed;
    }
}
