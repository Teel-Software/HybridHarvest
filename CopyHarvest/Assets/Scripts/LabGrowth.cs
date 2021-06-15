using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Unity.Mathematics;

public class LabGrowth : MonoBehaviour
{
    [SerializeField] Button Pot;
    [SerializeField] Button CrossingPerformer;
    [SerializeField] RectTransform InventoryFrame;
    [SerializeField] RectTransform CrossingMenue;

    bool isOccupied;
    bool timerNeeded;
    Seed growingSeed;
    public double time;

    private void Start()
    {
        if (PlayerPrefs.GetInt(Pot.name + "occupied") == 1)
        {
            isOccupied = true;
            timerNeeded = true;
        }
        else
        {
            isOccupied = false;
        }
        if (!isOccupied) return;

        growingSeed = ScriptableObject.CreateInstance<Seed>();
        growingSeed.SetValues(PlayerPrefs.GetString(Pot.name + "grows"));

        DateTime oldDate;
        oldDate = DateTime.Parse(PlayerPrefs.GetString(Pot.name + "timeStart"));
        var timePassed = DateTime.Now - oldDate;
        var timeSpan = new TimeSpan(timePassed.Days, timePassed.Hours, timePassed.Minutes, timePassed.Seconds);
        time = PlayerPrefs.GetInt(Pot.name + "time") - timeSpan.TotalSeconds;
        Pot.interactable = false;

        if (time <= 0)
            EndGrowthCycle();
        else
            Pot.GetComponentsInChildren<Image>()[1].sprite = growingSeed.GrownSprite;
    }

    private void Update()
    {
        if (timerNeeded)
        {
            if (time > 0)
            {
                if (time < (double)growingSeed.GrowTime / 2)
                    Pot.GetComponentsInChildren<Image>()[1].sprite = growingSeed.SproutSprite;
                time -= Time.deltaTime;
                Pot.GetComponentInChildren<Text>().text = math.round(time).ToString();
            }
            else
                EndGrowthCycle();
        }
    }

    void OnDestroy()
    {
        PlayerPrefs.SetInt(Pot.name + "time", (int)time);
        PlayerPrefs.SetString(Pot.name + "timeStart", DateTime.Now.ToString());
    }

    public void EndGrowthCycle()
    {
        timerNeeded = false;
        Pot.interactable = true;
        Pot.GetComponentInChildren<Text>().text = "";
        Pot.GetComponentsInChildren<Image>()[1].sprite = growingSeed.GrownSprite;
    }

    public void PlantIt(Seed seed)
    {
        Pot.interactable = false;
        isOccupied = true;
        growingSeed = seed;
        PlayerPrefs.SetInt(Pot.name + "occupied", isOccupied ? 1 : 0);
        PlayerPrefs.SetString(Pot.name + "grows", seed.ToString());
        time = seed.GrowTime;
        timerNeeded = true;
    }

    public void Clicked()
    {
        if (!(time < 0))
        {
            CrossingPerformer.GetComponent<GeneCrossing>().CurrentPot = Pot;
            CrossingMenue.gameObject.SetActive(true);
        }
        else
        {
            Pot.GetComponentsInChildren<Image>()[1].sprite = Resources.Load<Sprite>("Transparent");
            isOccupied = false;
            time = 1;
            InventoryFrame.GetComponent<Drawinventory>().targetInventory.AddItem(growingSeed);
            growingSeed = null;
            PlayerPrefs.SetInt(Pot.name + "occupied", isOccupied ? 1 : 0);
            Pot.GetComponentInChildren<Text>().text = "";
        }
    }
}
