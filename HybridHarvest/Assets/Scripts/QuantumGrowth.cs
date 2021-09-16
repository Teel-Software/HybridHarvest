using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Unity.Mathematics;

public class QuantumGrowth : MonoBehaviour
{
    [SerializeField] Button Pot;
    [SerializeField] Button CrossingPerformer;
    [SerializeField] RectTransform InventoryFrame;
    [SerializeField] RectTransform CrossingMenue;

    bool isOccupied;
    bool timerNeeded;
    Seed growingSeed;
    public double time;
    
    private Image plantImage;
    private Image textBGImage;
    private Text growthText;
    private void Start()
    {
        var imagesInChildren = Pot.GetComponentsInChildren<Image>();
        plantImage = imagesInChildren[1];
        //textBGImage = imagesInChildren[2];
        growthText = Pot.GetComponentInChildren<Text>();
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
    }

    private void Update()
    {
        if (timerNeeded)
        {
            if (time > 0)
            {
                time -= Time.deltaTime;
                //growthText.text = math.round(time).ToString();
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
        growthText.text = "";
        plantImage.sprite = growingSeed.PlantSprite;
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

    public void ApplyLightning(Seed seed)//Эта функция должна овечать за анимацию молнии
    { 
        //Pot.interactable = false;
        isOccupied = true;
        growingSeed = seed;
        PlayerPrefs.SetInt(Pot.name + "occupied", isOccupied ? 1 : 0);
        PlayerPrefs.SetString(Pot.name + "grows", seed.ToString());
        time = 0.5;
        timerNeeded = true;
    }

    public void Clicked()
    {
        if (time > 0) return;
        if (!isOccupied)
        {
            CrossingPerformer.GetComponent<GeneCrossing>().CurrentPot = Pot;
            CrossingMenue.gameObject.SetActive(true);
        }
        plantImage.sprite = Resources.Load<Sprite>("Transparent");
        isOccupied = false;
        if (growingSeed != null)
            InventoryFrame.GetComponent<Drawinventory>().targetInventory.AddItem(growingSeed);
        growingSeed = null;
        PlayerPrefs.SetInt(Pot.name + "occupied", isOccupied ? 1 : 0);
        growthText.text = "";
    }
}
