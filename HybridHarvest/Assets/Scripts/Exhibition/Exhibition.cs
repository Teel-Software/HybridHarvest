using System;
using System.Collections.Generic;
using System.Linq;
using CI.QuickSave;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

namespace Exhibition
{
    public class Exhibition : MonoBehaviour, ISaveable
    {
        [SerializeField] 
        private Button[] exhButtons;
        [SerializeField] 
        private Image testImage;
        [SerializeField] 
        private Button beginButton;
        
        public int SeedCount { get; private set; }
        public DateTime NextExhibition { get; private set; }
        
        private DateTime? rewardDate;
        private bool isInProgress = false;
        private int daySkip;
        public void OnEnable()
        {
            Load();
            var now = DateTime.Now;
        #if DEBUG
            now = now.AddDays(daySkip);    
        #endif
            if (now > NextExhibition)
            {
                InitializeExhibition();
            }
            isInProgress = rewardDate > now;
            
            foreach (var btn in exhButtons)
                btn.gameObject.SetActive(false);
            
            if (rewardDate is null || rewardDate < now)
                foreach (var btn in exhButtons.Take(SeedCount))
                    btn.gameObject.SetActive(true);
            
            foreach (var btn in exhButtons)
            {
                btn.onClick.RemoveAllListeners();
                var exhibBtn = btn.GetComponent<ExhibitionButton>();
                if (rewardDate is null)
                {
                    beginButton.gameObject.SetActive(true);
                    
                    btn.onClick.AddListener(exhibBtn.AddSeed);      
                }
                else if (rewardDate > now)
                {
                    beginButton.gameObject.SetActive(false);
                    
                    btn.onClick.AddListener(exhibBtn.DisabledClick);
                }
                else if (rewardDate < now)
                {                    
                    beginButton.gameObject.SetActive(false);
                    
                    btn.onClick.AddListener(exhibBtn.GetResult);
                    if (exhibBtn.NowSelected is null)
                        exhibBtn.MakeDisabled();
                }
            }
        }

        public void Update()
        {
            if (isInProgress && rewardDate < DateTime.Now)
            {
                OnEnable();
                isInProgress = false;
            }
        }

        public void BeginExhibition()
        {
        #if DEBUG
            rewardDate = DateTime.Now.AddSeconds(3);
        #else
            RewardDate = DateTime.Now.AddMinutes(15);
        #endif
            OnEnable();
        }
        
        public void AddDaySkip(int days)
        {
            daySkip += days;
        }
        
        private void InitializeExhibition()
        {
            var rand = new Random();
            SeedCount = rand.Next(3) + 1;
            NextExhibition = NextExhibition.AddDays(1);
            rewardDate = null;
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            if (!hasFocus)
                Save();
        }
        
        public void Save()
        {
            var writer = QuickSaveWriter.Create("ExhibitionData");
            var exhSeeds = new List<string>();
            foreach (var btn in exhButtons)
            {
                exhSeeds.Add(btn.GetComponent<ExhibitionButton>().NowSelected is null
                    ? ""
                    : btn.GetComponent<ExhibitionButton>().NowSelected.ToString());
            }
            writer.Write("Seeds", exhSeeds);
            writer.Write("SeedCount", SeedCount);
            writer.Write("NextExhibition", NextExhibition);
            writer.Commit();
        }

        public void Load()
        {
            var reader = QSReader.Create("ExhibitionData");
            if (reader.TryRead("Seeds", out List<string> exhSeeds))
            {
                for (var i = 0; i < exhSeeds.Count; i++)
                {
                    if (exhSeeds[i] == "")
                        continue;
                    var seed = Seed.Create(exhSeeds[i]);
                    exhButtons[i].GetComponent<ExhibitionButton>().SetSeed(seed);   
                }
            }
            
            var rand = new Random();
            SeedCount = reader.TryRead<int>("SeedCount", out var count) 
                ? count 
                : rand.Next(3) + 1;

            NextExhibition = reader.TryRead<DateTime>("NextExhibition", out var next)
                ? next
                : DateTime.Today.AddDays(1);
        }
    }
}
