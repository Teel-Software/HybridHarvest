using UnityEngine;
using UnityEngine.UI;
using System;
using Unity.Mathematics;

public class LabGrowth : MonoBehaviour
{
    [SerializeField] public Button Pot;
    [SerializeField] Button CrossingPerformer;
    [SerializeField] RectTransform InventoryFrame;
    [SerializeField] RectTransform CrossingMenue;
    [SerializeField] GameObject MiniGamePanel;

    public Seed growingSeed;

    bool isOccupied;
    bool timerNeeded;
    public double time;
    private double _totalTime;

    private Image plantImage;
    private Image textBGImage;
    private Text growthText;
    private double _timeSpeedBooster = 1;

    private void Start()
    {
        var imagesInChildren = Pot.GetComponentsInChildren<Image>();
        plantImage = imagesInChildren[1];
        textBGImage = imagesInChildren[2];
        growthText = Pot.GetComponentInChildren<Text>();
        if (PlayerPrefs.GetInt(Pot.name + "occupied") == 1)
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
        growingSeed.SetValues(PlayerPrefs.GetString(Pot.name + "grows"));
        SpeedUpTutorSeed(growingSeed);
        
        var oldDate = DateTime.Parse(PlayerPrefs.GetString(Pot.name + "timeStart"));
        var timePassed = DateTime.Now - oldDate;
        var timeSpan = new TimeSpan(timePassed.Days, timePassed.Hours, timePassed.Minutes, timePassed.Seconds);
        time = PlayerPrefs.GetInt(Pot.name + "time") - timeSpan.TotalSeconds * _timeSpeedBooster;
        _totalTime = PlayerPrefs.GetFloat(Pot.name + "TotalTime");
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
                plantImage.sprite = growingSeed.GetGrowthStageSprite(time, _totalTime);
                time -= Time.deltaTime * _timeSpeedBooster;
                var formatTime = TimeSpan.FromSeconds(math.round(time));
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

    private void OnDestroy()
    {
        PlayerPrefs.SetInt(Pot.name + "time", (int)time);
        PlayerPrefs.SetString(Pot.name + "timeStart", DateTime.Now.ToString());
    }

    private void EndGrowthCycle()
    {
        timerNeeded = false;
        Pot.interactable = true;
        textBGImage.enabled = true;
        growthText.text = "ГОТОВО";
        plantImage.sprite = growingSeed.GrownSprite;
    }

    public void PlantIt(Seed seed, int totalTime)
    {
        Pot.interactable = false;
        isOccupied = true;
        growingSeed = seed;
        PlayerPrefs.SetInt(Pot.name + "occupied", isOccupied ? 1 : 0);
        PlayerPrefs.SetString(Pot.name + "grows", seed.ToString());
        time = totalTime;
        _totalTime = totalTime;
        PlayerPrefs.SetFloat(Pot.name + "TotalTime", (float)_totalTime);
        timerNeeded = true;
        SpeedUpTutorSeed(seed);
    }

    public void Clicked()
    {
        if (!isOccupied)
        {
            CrossingPerformer.GetComponent<GeneCrossing>().CurrentPot = Pot;
            CrossingMenue.gameObject.SetActive(true);
            return;
        }

        MiniGamePanel.GetComponent<CreateMiniGame>().ResultPlace = Pot;
        MiniGamePanel.GetComponent<CreateMiniGame>().RestartGame();
        MiniGamePanel.SetActive(true);
        InventoryFrame.GetComponent<InventoryDrawer>().SuccessfulAddition += () =>
        {
            isOccupied = false;
            plantImage.sprite = Resources.Load<Sprite>("Transparent");
            textBGImage.enabled = false;
            growingSeed = null;
            PlayerPrefs.SetInt(Pot.name + "occupied", isOccupied ? 1 : 0);
            Pot.GetComponentInChildren<Text>().text = "";
        };
    }

    /// <summary>
    /// Ускоряет рост обучающих семян
    /// </summary>
    /// <param name="seed">Семечко</param>
    private void SpeedUpTutorSeed(Seed seed)
    {
        if (seed.NameInRussian == "Обучающий помидор")
            _timeSpeedBooster = 120;
    }
}
