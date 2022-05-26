using System;
using System.Collections.Generic;
using System.Linq;
using CI.QuickSave;
using TMPro;
using Tools;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

namespace Exhibition
{
    public class Exhibition : MonoBehaviour, ISaveable
    {
        [SerializeField] private GameObject inProgressContainer;
        [SerializeField] private GameObject rewardPendingContainer;
        [SerializeField] private GameObject finishedContainer;
        
        [SerializeField] private Image testImage;
        
        [SerializeField] private Button[] exhButtons;      
        [SerializeField] private Button beginButton;
        public int SeedCount { get; private set; }
        public DateTime NextExhibition { get; private set; }
        public DateTime? RewardDate { get; private set; }
        public ExhibitionState State { get; private set; }
        
        private int _daySkip = 0;
        
        public DateTime Now { get; private set; }
        public void Awake()
        {
            Load();
            SetDebugTime();
            
            if (Now > NextExhibition)
            {
                InitializeExhibition();
            }

            var isActive = State == ExhibitionState.Inactive;
            beginButton.gameObject.SetActive(isActive);
            foreach (var btn in exhButtons)
            {
                btn.gameObject.SetActive(isActive);
            }
        }

        public void Update()
        {
            SetDebugTime();
            
            if (State == ExhibitionState.InProgress && RewardDate < Now)
            {
                State = ExhibitionState.RewardPending;
                Save();
                Awake();
            }
            
            inProgressContainer.SetActive(State == ExhibitionState.InProgress);
            rewardPendingContainer.SetActive(State == ExhibitionState.RewardPending);
            finishedContainer.SetActive(State == ExhibitionState.Finished);

            switch (State)
            {
                case ExhibitionState.InProgress:
                    inProgressContainer.GetComponentsInChildren<TextMeshProUGUI>().Last()
                        .text = TimeFormatter.Format((DateTime)RewardDate - Now);
                    break;
                
                case ExhibitionState.RewardPending:
                    rewardPendingContainer.GetComponentInChildren<Button>()
                        .onClick.RemoveAllListeners();
                    rewardPendingContainer.GetComponentInChildren<Button>()
                        .onClick.AddListener(GetAward);
                    break;
                
                case ExhibitionState.Finished:
                    finishedContainer.GetComponentsInChildren<TextMeshProUGUI>().Last()
                        .text = TimeFormatter.Format(NextExhibition - Now);
                    break;
            }
        }

        public void BeginExhibition()
        {
        #if DEBUG
            RewardDate = Now.AddSeconds(7);
        #else
            RewardDate = Now.AddMinutes(15);
        #endif
            State = ExhibitionState.InProgress;
            Save();
            Awake();
        }

        private void GetAward()
        {
            // TODO remove random
            var rand = new Random();
            var awardTier = rand.Next(4) + 1;
            var awards = new List<Award>
            {
                new Award(AwardType.Money, amount: 25 * awardTier),
                new Award(AwardType.Reputation, amount: 25 * awardTier)
            };
            rewardPendingContainer.GetComponent<AwardsCenter>().Show(awards, 
                $"Вы заняли {5 - awardTier} место!");
            State = ExhibitionState.Finished;
        }

        public void AddDaySkip(int days)
        {
            _daySkip = Math.Max(0, _daySkip + days);
        }

        private void SetDebugTime()
        {
            Now = DateTime.Now;
#if DEBUG
            Now = Now.AddDays(_daySkip);    
#endif
        }
        
        private void InitializeExhibition()
        {
            // TODO remove random maybe?
            var rand = new Random();
            SeedCount = rand.Next(3) + 1;
            NextExhibition = Now.Date.AddDays(1);
            RewardDate = null;
            State = ExhibitionState.Inactive;
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
            if (!(RewardDate is null))
            {
                writer.Write("RewardDate", RewardDate);
            }
            writer.Write("State", State);
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
                : Now.Date.AddDays(1);

            RewardDate = reader.TryRead<DateTime>("RewardDate", out var _rewardDate)
                ? _rewardDate
                : (DateTime?)null;

            State = reader.TryRead<ExhibitionState>("State", out var _state)
                ? _state
                : ExhibitionState.Inactive;
        }
    }
    
    public enum ExhibitionState {
        Inactive,
        InProgress,
        RewardPending,
        Finished
    }
}   


