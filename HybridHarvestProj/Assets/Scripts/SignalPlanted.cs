using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Unity.Mathematics;
using UnityEngine.EventSystems;

public class SignalPlanted : MonoBehaviour
{
    [SerializeField] Button Patch;
    [SerializeField] RectTransform InventoryFrame;
    bool isOccupied;
    bool timerNeeded;
    Seed nowGrows;
    public double time;

    private void Start()
    {
        if (PlayerPrefs.GetInt(Patch.name + "occupied") == 1)
        {
            isOccupied = true;
            timerNeeded = true;
        }
        else
            isOccupied = false;
        if (isOccupied)
        {
            nowGrows = new Seed(PlayerPrefs.GetString(Patch.name + "grows"));
            DateTime oldDate;
            oldDate = DateTime.Parse(PlayerPrefs.GetString(Patch.name + "timeStart"));
            var timePassed = DateTime.Now - oldDate;
            var timeSpan = new TimeSpan(timePassed.Days, timePassed.Hours, timePassed.Minutes, timePassed.Seconds);
            time = PlayerPrefs.GetInt(Patch.name + "time") - timeSpan.TotalSeconds;
            Patch.interactable = false;
            
            if (time <= 0)
                TickTack();
        }
    }

    private void Update()
    {
        if (timerNeeded)
        {
            if (time > 0)
            {
                time -= Time.deltaTime;
                Patch.GetComponentInChildren<Text>().text = math.round(time).ToString();
            }
            else
                TickTack();
        }
    }

    void OnDestroy()
    {
        PlayerPrefs.SetInt(Patch.name +"time", (int)time);
        PlayerPrefs.SetString(Patch.name + "timeStart", DateTime.Now.ToString());
    }

    public void TickTack()
    {
        timerNeeded = false;
        Patch.interactable = true;
        Patch.GetComponentInChildren<Text>().text = "harvest time";
    }

    public void PlantIt(Seed seed)
    {
        Patch.GetComponentInChildren<Text>().text = "planted" + seed;
        Patch.interactable = false;
        isOccupied = true;
        nowGrows = seed;
        PlayerPrefs.SetInt(Patch.name+"occupied", isOccupied ? 1 : 0);
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
            isOccupied = false;
            time = 1;
            for (var i = 0; i < nowGrows.Amount; i++)
            {
                var newSeed = MutateSeed(nowGrows, i);
                InventoryFrame.GetComponent<Drawinventory>().targetInventory.AddItem(newSeed);
            }
            //InventoryFrame.GetComponent<Drawinventory>().targetInventory.AddItem(nowGrows);
            nowGrows = null;
            PlayerPrefs.SetInt(Patch.name + "occupied", isOccupied ? 1 : 0);
            Patch.GetComponentInChildren<Text>().text = "free place";
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
