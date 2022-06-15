using UnityEngine;
using UnityEngine.UI;
using System;
using CI.QuickSave;
using UnityEngine.Serialization;

public class QuantumGrowth : MonoBehaviour
{
    [SerializeField] public Button Pot;
    [FormerlySerializedAs("CrossingPerformer")] 
        [SerializeField] Button CrossingButton;

    //[SerializeField] RectTransform InventoryFrame;
    [SerializeField] GameObject InventoryFrame;

    [FormerlySerializedAs("CrossingMenue")] 
        [SerializeField] RectTransform CrossingMenu;
    
    [SerializeField] GameObject MiniGamePanel;
    [SerializeField] GameObject CooldownSign;
    
    public Seed growingSeed;

    bool isOccupied;
    bool growTimerNeeded;
    public double time;

    private Image plantImage;
    private Image textBGImage;
    private Text growthText;

    private DateTime cooldownEnd;
    private void Start()
    {
        Load();
        CheckCooldownState();
        var imagesInChildren = Pot.GetComponentsInChildren<Image>();
        plantImage = imagesInChildren[1];
        //textBGImage = imagesInChildren[2];
        growthText = Pot.GetComponentInChildren<Text>();
        
        isOccupied = PlayerPrefs.GetInt(Pot.name + "occupied") == 1;
        if (!isOccupied)
        {
            Pot.interactable = true;
            return;
        }
        
        growingSeed = Seed.Create(PlayerPrefs.GetString(Pot.name + "grows"));
        plantImage.sprite = growingSeed.PlantSprite;
        
        var oldDate = DateTime.Parse(PlayerPrefs.GetString(Pot.name + "timeStart"));
        var timePassed = DateTime.Now - oldDate;
        var timeSpan = new TimeSpan(timePassed.Days, timePassed.Hours, timePassed.Minutes, timePassed.Seconds);
        time = PlayerPrefs.GetInt(Pot.name + "time") - timeSpan.TotalSeconds;
        
        growTimerNeeded = time > 0;
        if (growTimerNeeded)
            Pot.interactable = false;
    }

    private void Update()
    {
        CheckCooldownState();

        if (growTimerNeeded)
        {
            if (time > 0)
                time -= Time.deltaTime;
            else
                EndGrowthCycle();    
        }

    }

    private void CheckCooldownState()
    {
        var timeRemaining = cooldownEnd - DateTime.Now;
        var isOver = timeRemaining.TotalSeconds <= 0;
        
        CooldownSign.SetActive(!isOver);
        CrossingButton.interactable = isOver;
        
        if (CooldownSign.activeSelf)
            UpdateSign(timeRemaining);
    }

    void OnDestroy()
    {
        PlayerPrefs.SetInt(Pot.name + "time", (int)time);
        PlayerPrefs.SetString(Pot.name + "timeStart", DateTime.Now.ToString());
    }

    private void Load()
    {
        var reader = QSReader.Create("Quantum");
        cooldownEnd = reader.Exists("CooldownEnd") ? 
                    reader.Read<DateTime>("CooldownEnd") : 
                    DateTime.Now;
    }

    private void Save()
    {
        var writer = QuickSaveWriter.Create("Quantum");
        writer.Write("CooldownEnd", cooldownEnd);
        writer.Commit();
    }
    
    public void EndGrowthCycle()
    {
        growTimerNeeded = false;
        Pot.interactable = true;
        growthText.text = "";
        plantImage.sprite = growingSeed.PlantSprite;

        //CrossingButton.interactable = false;
        
        // 24 hour delay
        cooldownEnd = DateTime.Now.AddHours(24);
        Save();

        Clicked();
        //CooldownSign.SetActive(true);
        //UpdateSign(cooldownEnd.TimeOfDay);
    }

    private void UpdateSign(TimeSpan time)
    {
        var signText = CooldownSign.GetComponentInChildren<Text>();
        signText.text = $"Приходи через {time:hh\\:mm\\:ss}";
    }

    public void PlantIt(Seed seed)
    {
        Pot.interactable = false;
        isOccupied = true;
        growingSeed = seed;
        PlayerPrefs.SetInt(Pot.name + "occupied", isOccupied ? 1 : 0);
        PlayerPrefs.SetString(Pot.name + "grows", seed.ToString());
        time = seed.GrowTime;
        growTimerNeeded = true;
    }

    public void ApplyLightning(Seed seed) //Эта функция должна отвечать за анимацию молнии
    {
        //Pot.interactable = false;
        isOccupied = true;
        growingSeed = seed;
        PlayerPrefs.SetInt(Pot.name + "occupied", isOccupied ? 1 : 0);
        PlayerPrefs.SetString(Pot.name + "grows", seed.ToString());
        time = 0.5;
        growTimerNeeded = true;
    }

    public void Clicked()
    {
        if (time > 0) return;
        if (!isOccupied)
        {
            CrossingButton.GetComponent<GeneCrossing>().CurrentPot = Pot;
            CrossingMenu.gameObject.SetActive(true);
        }

        //plantImage.sprite = Resources.Load<Sprite>("Transparent");
        //isOccupied = false;

        if (growingSeed != null)
        {
            MiniGamePanel.GetComponent<CreateMiniGame>().ResultPlace = Pot;
            MiniGamePanel.GetComponent<CreateMiniGame>().RestartQuantumGame();
            MiniGamePanel.SetActive(true);
            InventoryFrame.GetComponent<InventoryDrawer>().SuccessfulAddition += () =>
            {
                isOccupied = false;
                plantImage.sprite = Resources.Load<Sprite>("Transparent");
                //textBGImage.enabled = false;
                growingSeed = null;
                PlayerPrefs.SetInt(Pot.name + "occupied", isOccupied ? 1 : 0);
                growthText.text = "";
            };
            //InventoryFrame.GetComponent<InventoryDrawer>().UpdateActions();
            //InventoryFrame.GetComponent<InventoryDrawer>().targetInventory.AddItem(growingSeed);

            //Statistics.UpdateCrossedSeeds(growingSeed.Name);
        }

        //growingSeed = null;
        //PlayerPrefs.SetInt(Pot.name + "occupied", isOccupied ? 1 : 0);
        //growthText.text = "";
    }
}
