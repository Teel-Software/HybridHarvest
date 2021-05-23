using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Timers;

public class SignalPlanted : MonoBehaviour
{
    //[SerializeField] Drawinventory maiObj;
    [SerializeField] Button tyt;
    [SerializeField] RectTransform Place;
    bool isOccupied;
    bool timePassed;
    Seed nowGrows;
    Timer timer = new Timer();

    private void Start()
    {
        if (PlayerPrefs.GetInt(tyt.name + "occupied") == 1)
            isOccupied = true;
        else
            isOccupied = false;
        if (isOccupied)
        {
            print("zanyato");
            nowGrows = new Seed(PlayerPrefs.GetString(tyt.name + "grows"));
            tyt.GetComponentInChildren<Text>().text = "planted" + nowGrows.ToString();
            //timer = new Timer();
            timer.Interval = nowGrows.GrowTime*100;
            timer.Elapsed += TickTack;
            timer.Start();
            // tyt.gameObject.SetActive( false);
            tyt.interactable = false;
            //tyt.gameObject.SetActive(true);
            
            //Debug.Log("timer ok");
        }
    }

    //private void Update()
   // {
        //tyt.GetComponentInChildren<Text>().text =
   // }

    public void TickTack(object source, ElapsedEventArgs e)
    {
        timer.Stop();
       // Debug.Log("timer stop");
        //timer.Stop();
        timePassed = true;
       // Debug.Log(timePassed);
        //Debug.Log(tyt == null);
        tyt.interactable = true;
        tyt.GetComponentInChildren<Text>().text = "harvest";
        //Debug.Log(tyt.gameObject == null);
        //Debug.Log(tyt.IsActive());
        // tyt.gameObject.SetActive(true);
        //Debug.Log("help");
        // tyt.GetComponent<Button>().enabled = true;
        // Debug.Log("button ok");
        // print("tip vse");
        //timer.Stop();
    }

    public void PlantIt(Seed seed)
    {
       // print(tyt == null);
        //print(tyt.gameObject.GetComponent<Text>() == null);
        //print(seed.ToString());
        tyt.GetComponentInChildren<Text>().text = "planted" +seed.ToString();
        isOccupied = true;
        nowGrows = seed;
        PlayerPrefs.SetInt(tyt.name+"occupied", isOccupied ? 1 : 0);
        PlayerPrefs.SetString(tyt.name + "grows", seed.ToString());

        timer.Interval = nowGrows.GrowTime * 100;
        timer.Elapsed += TickTack;
        timer.Start();
        tyt.interactable = false;
    }

    public void Clicked()
    {
        Debug.Log(timePassed);
        if (!timePassed)
        {
            Place.GetComponent<Drawinventory>().GrowPlace = tyt;
            Place.gameObject.SetActive(true);
        }
        else
        {
            isOccupied = false;
            nowGrows = null;
            timePassed = false;
            PlayerPrefs.SetInt(tyt.name + "occupied", isOccupied ? 1 : 0);
        }
       // maiObj.GrowPlace = tyt;

    }
}
