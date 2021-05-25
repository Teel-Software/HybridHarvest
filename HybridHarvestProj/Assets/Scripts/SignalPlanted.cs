using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
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
        //Debug.Log(isOccupied);
        if (isOccupied)
        {
            nowGrows = new Seed(PlayerPrefs.GetString(Patch.name + "grows"));
            //Debug.Log(nowGrows);
            time = PlayerPrefs.GetInt(Patch.name + "time");
            //Debug.Log(timerNeeded);
            //Debug.Log(time);
            Patch.GetComponentInChildren<Text>().text = "planted" + nowGrows.ToString();
            Patch.interactable = false;
        }
    }

    private void Update()
    {
        if (timerNeeded)
        {
            if (time > 0)
            {
                time -= Time.deltaTime;
                Patch.GetComponentInChildren<Text>().text = time.ToString();
            }
            else
                TickTack();
        }
    }

    void OnDestroy()
    {
        PlayerPrefs.SetInt(Patch.name +"time", (int)time);
        //Debug.Log("it saved");
    }

    public void TickTack()
    {
       // Debug.Log("TickTack");
        timerNeeded = false;
        Patch.interactable = true;
        Patch.GetComponentInChildren<Text>().text = "harvest time";
    }

    public void PlantIt(Seed seed)
    {
       // Debug.Log("PlantIt");
        Patch.GetComponentInChildren<Text>().text = "planted" +seed.ToString();
        Patch.interactable = false;
        isOccupied = true;
        nowGrows = seed;
       // Debug.Log(nowGrows);
        PlayerPrefs.SetInt(Patch.name+"occupied", isOccupied ? 1 : 0);
        PlayerPrefs.SetString(Patch.name + "grows", seed.ToString());
        time = seed.GrowTime;
        timerNeeded = true;
    }

    public void Clicked()
    {
       // Debug.Log(timerNeeded);
        if (!(time<0))
        {
            InventoryFrame.GetComponent<Drawinventory>().GrowPlace = Patch;
            InventoryFrame.gameObject.SetActive(true);
        }
        else
        {
            isOccupied = false;
            //nowGrows = null;
            time = 1;
           // Debug.Log(nowGrows);
            InventoryFrame.GetComponent<Drawinventory>().targetInventory.AddItem(nowGrows);
            InventoryFrame.GetComponent<Drawinventory>().targetInventory.AddItem(nowGrows);
            nowGrows = null;
            PlayerPrefs.SetInt(Patch.name + "occupied", isOccupied ? 1 : 0);
            Patch.GetComponentInChildren<Text>().text = "free place";
        }
    }
}
