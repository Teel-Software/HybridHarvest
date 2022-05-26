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
        
        private DateTime? rewardDate;
        private State state;
        
        private bool isInProgress;
        private int _daySkip;
        
        public DateTime Now;
        public void Awake()
        {
            Load();
            
            Now = DateTime.Now;
        #if DEBUG
            Now = Now.AddDays(_daySkip);    
        #endif
            if (Now > NextExhibition)
            {
                InitializeExhibition();
            }
            
            // change to State.InProgress
            isInProgress = rewardDate > Now;
            
            foreach (var btn in exhButtons)
                btn.gameObject.SetActive(false);

            if (rewardDate is null)
            {
                beginButton.gameObject.SetActive(true);
                foreach (var btn in exhButtons.Take(SeedCount))
                    btn.gameObject.SetActive(true);
            }
            else
            {
                beginButton.gameObject.SetActive(false);    
            }
            
            foreach (var btn in exhButtons)
            {
                btn.onClick.RemoveAllListeners();
                var exhibBtn = btn.GetComponent<ExhibitionButton>();
                if (rewardDate is null)
                {
                    btn.onClick.AddListener(exhibBtn.AddSeed);      
                }
                else if (rewardDate > Now)
                {
                }
                else if (rewardDate < Now)
                {                    
                    exhibBtn.MakeDisabled();
                }
            }
            
            if (rewardDate < Now && state != State.Finished)
            {
                rewardPendingContainer.SetActive(true);
                rewardPendingContainer.GetComponentInChildren<Button>()
                    .onClick.RemoveAllListeners();
                rewardPendingContainer.GetComponentInChildren<Button>()
                    .onClick.AddListener(GetAward);
            }
        }

        public void Update()
        {
            Now = DateTime.Now;
        #if DEBUG
            Now = Now.AddDays(_daySkip);    
        #endif
            
            if (isInProgress && rewardDate < DateTime.Now)
            {
                Awake();
				isInProgress = false;
            }
            
            // switch by state here somewhere
            inProgressContainer.SetActive(rewardDate > DateTime.Now);
            finishedContainer.SetActive(state == State.Finished);
            
            if (rewardDate > DateTime.Now)
            {
                inProgressContainer.GetComponentsInChildren<TextMeshProUGUI>().Last()
                    .text = TimeFormatter.Format((DateTime)rewardDate - DateTime.Now);
            }
            else if (state == State.Finished)
            {
                finishedContainer.GetComponentsInChildren<TextMeshProUGUI>().Last()
                    .text = TimeFormatter.Format(NextExhibition - DateTime.Now);
            }
        }

        public void BeginExhibition()
        {
        #if DEBUG
            rewardDate = DateTime.Now.AddSeconds(7);
        #else
            rewardDate = DateTime.Now.AddMinutes(15);
        #endif
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
            rewardPendingContainer.SetActive(false);
            state = State.Finished;
        }

        public void AddDaySkip(int days)
        {
            _daySkip = Math.Max(0, _daySkip + days);
        }
        
        private void InitializeExhibition()
        {
            var rand = new Random();
            SeedCount = rand.Next(3) + 1;
            NextExhibition = DateTime.Today.AddDays(1);
            state = State.Inactive;
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
            if (!(rewardDate is null))
            {
                writer.Write("RewardDate", rewardDate);
            }
            writer.Write("State", state);
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

            rewardDate = reader.TryRead<DateTime>("RewardDate", out var _rewardDate)
                ? _rewardDate
                : (DateTime?)null;

            state = reader.TryRead<State>("State", out var _state)
                ? _state
                : State.Inactive;
        }
    }
    
    public enum State {
        Inactive,
        InProgress,
        RewardPending,
        Finished
    }
}   


