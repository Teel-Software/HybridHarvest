using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Unity.Mathematics;

public class SignalPlanted : MonoBehaviour
{
    [SerializeField] Button Patch;
    [SerializeField] RectTransform InventoryFrame;

    bool isOccupied;
    bool timerNeeded;
    Seed growingSeed;
    public double time;

    private void Start()
    {
        if (PlayerPrefs.GetInt(Patch.name + "occupied") == 1)
        {
            isOccupied = true;
            timerNeeded = true;
        }
        else
        {
            isOccupied = false;
        }
        if (!isOccupied) return;
        
        growingSeed = new Seed(PlayerPrefs.GetString(Patch.name + "grows"));
        DateTime oldDate;
        oldDate = DateTime.Parse(PlayerPrefs.GetString(Patch.name + "timeStart"));
        var timePassed = DateTime.Now - oldDate;
        var timeSpan = new TimeSpan(timePassed.Days, timePassed.Hours, timePassed.Minutes, timePassed.Seconds);
        time = PlayerPrefs.GetInt(Patch.name + "time") - timeSpan.TotalSeconds;
        Patch.interactable = false;

        if (time <= 0)
            EndGrowthCycle();
        else
            Patch.GetComponentsInChildren<Image>()[1].sprite = growingSeed.GrownSprite;
    }

    private void Update()
    {
        if (timerNeeded)
        {
            if (time > 0)
            {
                if (time < (double)growingSeed.GrowTime / 2)
                    Patch.GetComponentsInChildren<Image>()[1].sprite = growingSeed.SproutSprite;
                time -= Time.deltaTime;
                Patch.GetComponentInChildren<Text>().text = math.round(time).ToString();
            }
            else
                EndGrowthCycle();
        }
    }

    void OnDestroy()
    {
        PlayerPrefs.SetInt(Patch.name +"time", (int)time);
        PlayerPrefs.SetString(Patch.name + "timeStart", DateTime.Now.ToString());
    }

    public void EndGrowthCycle()
    {
        timerNeeded = false;
        Patch.interactable = true;
        Patch.GetComponentInChildren<Text>().text = "";
        Patch.GetComponentsInChildren<Image>()[1].sprite = growingSeed.GrownSprite;
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
        if (!(time<0))
        {
            InventoryFrame.GetComponent<Drawinventory>().GrowPlace = Patch;
            InventoryFrame.gameObject.SetActive(true);
        }
        else
        {
            Patch.GetComponentsInChildren<Image>()[1].sprite = Resources.Load<Sprite>("Transparent");
            isOccupied = false;
            time = 1;
            for (var i = 0; i < growingSeed.Amount; i++)
            {
                var newSeed = MutateSeed(growingSeed, i);
                InventoryFrame.GetComponent<Drawinventory>().targetInventory.AddItem(newSeed);
            }
            //InventoryFrame.GetComponent<Drawinventory>().targetInventory.AddItem(nowGrows);
            growingSeed = null;
            PlayerPrefs.SetInt(Patch.name + "occupied", isOccupied ? 1 : 0);
            Patch.GetComponentInChildren<Text>().text = "";
        }
    }

    public Seed MutateSeed(Seed oldSeed, int i) 
    {
        var procentage =  UnityEngine.Random.value;
        var newSeed = new Seed(oldSeed.ToString());
        var plusAmount = UnityEngine.Random.value;
        if (procentage < 0.5 && newSeed.Gabitus<=100)
            newSeed.Gabitus += (int)(plusAmount * 5+1);
        else
        {
            if (newSeed.Taste <= 100)
            {
                newSeed.Taste += (int)(plusAmount * 5 + 1);
                newSeed.Price += (int)(plusAmount * 5 + 1);
            }
        }
        return newSeed;
    }
}
