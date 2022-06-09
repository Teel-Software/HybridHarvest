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

        public Opponent[] Opponents { get; private set; }
        public List<Seed> PlayerSeeds { get; private set; }
        public int[] WeeklyPlacements { get; private set; }
        
        private int _debugOppCount = 0;
        private int _daySkip = 0;

        public DateTime Now { get; private set; }
        public int DayIndex => ((int)Now.DayOfWeek + 6) % 7;

        public void Awake()
        {
            Load();
            SetDebugTime();

            if (Now > NextExhibition)
            {
                InitializeExhibition();
                if (DayIndex == 0)
                {
                    WeeklyPlacements = new int[7];
                }
            }

            foreach (var btn in exhButtons)
            {
                btn.gameObject.SetActive(false);
            }

            var isActive = State == ExhibitionState.Inactive;
            beginButton.gameObject.SetActive(isActive);
            foreach (var btn in exhButtons.Take(SeedCount))
            {
                btn.gameObject.SetActive(isActive);
            }
        }

        private void SetDebugTime()
        {
            Now = DateTime.Now;
#if DEBUG
            Now = Now.AddDays(_daySkip);
#endif
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
                        .text = TimeFormatter.Format((DateTime) RewardDate - Now);
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

        private Opponent[] GenerateOpponents()
        {
            // TODO figure out how to make this better
            var possibleOpponents = new List<Opponent>
            {
                new Opponent("Тамара", "Tamara"),
                new Opponent("Лариса", "Larisa"),
                new Opponent("Серафима Ивановна", "OldLady"),
                new Opponent("Дед Максим", "OldMan"),
                new Opponent("Алиса", "Alisa"),
            };

            var level = FindObjectOfType<Inventory>().Level;
            var opponentCount = 0;
            var countRand = new Random();
            var randRoll = countRand.Next(101);
            if (level < 5)
            {
                opponentCount = 1;
            }
            else if (level < 10)
            {
                opponentCount = randRoll <= 30 ? 1 : 2;
            }
            else
            {
                if (randRoll <= 15)
                    opponentCount = 1;
                else if (randRoll <= 30)
                    opponentCount = 2;
                else
                    opponentCount = 3;
            }

            if (_debugOppCount > 1)
                opponentCount = _debugOppCount;

            var opponents = new Opponent[opponentCount];

            var seedNames = Resources.LoadAll<Seed>("Seeds")
                .Select(x => x.Name)
                .Where(x => x != "Debug")
                .ToList();

            var exhibitonDifficulty = 1;
            var example = Seed.LoadFromResources("Cucumber");
            var points = example.ToPoints() * exhibitonDifficulty;

            var rand = new Random(Environment.TickCount);
            var seedCount = GetComponentInParent<Exhibition>().SeedCount;
            var unusedIndexes = Enumerable.Range(0, possibleOpponents.Count).ToList();
            for (var i = 0; i < opponentCount; i++)
            {
                // Random generation without repetitions
                var index = rand.Next(0, unusedIndexes.Count);
                var baseOpponent = possibleOpponents[unusedIndexes[index]];
                unusedIndexes.RemoveAt(index);

                var seeds = new List<Seed>();
                for (var j = 0; j < seedCount; j++)
                {
                    var seedName = seedNames[rand.Next(seedNames.Count)];
                    var seed = Seed.CreateRandom(seedName, points);
                    seeds.Add(seed);
                }

                opponents[i] = new Opponent(baseOpponent.Name, baseOpponent.SpriteName, seeds);
            }

            return opponents;
        }

        public void StartExhibition()
        {
#if DEBUG
            RewardDate = Now.AddSeconds(3);
#else
            RewardDate = Now.AddMinutes(15);
#endif
            State = ExhibitionState.InProgress;
            Save();
            Awake();
        }

        private const int MAXTier = 4;

        private void GetAward()
        {
            var points = Opponents
                .Select(opp => opp.Seeds)
                .Select(seeds => seeds.Select(s => s.ToPoints()).Sum())
                .ToList();
            points.Sort();
            var i = 0;
            foreach (var p in points)
            {
                Debug.Log($"Очки противника #{++i}: {p}");
            }

            var playerPoints = PlayerSeeds.Select(s => s.ToPoints()).Sum();
            Debug.Log($"Очки игрока: {playerPoints}");
            var index = points.BinarySearch(playerPoints);
            var place = index < 0 // index is negative if element wasn't found
                ? points.Count + index + 2
                : points.Count - index;
            //place = points.Count - ~index - 1;
            Debug.Log($"{place}-е Место!");

            var awardTier = MAXTier + 1 - place;
            var awards = new List<Award>
            {
                new Award(AwardType.Money, amount: 25 * awardTier),
                new Award(AwardType.Reputation, amount: 25 * awardTier)
            };

            rewardPendingContainer.GetComponent<AwardsCenter>().Show(awards, 
                $"Вы заняли {place} место!", lastAction: () => 
                {
                    if (place != 1) return;

                    var scenario = GameObject.FindGameObjectWithTag("TutorialHandler")?.GetComponent<Scenario>();
                    if (scenario is null) return;
                    
                    // диалог для победы на выставке
                    scenario.ExhibitionWin();
                });

            WeeklyPlacements[DayIndex] = place;
            
            State = ExhibitionState.Finished;
        }

        public void AddDaySkip(int days)
        {
            _daySkip = Math.Max(0, _daySkip + days);
        }

        public void ChangeOppCount(int inc)
        {
            _debugOppCount = Math.Min(_debugOppCount + inc, 3);
            _debugOppCount = Math.Max(_debugOppCount, 1);
        }

        private void InitializeExhibition()
        {
            // TODO remove random maybe?
            var rand = new Random();
            SeedCount = rand.Next(3) + 1;
            NextExhibition = Now.Date.AddDays(1);
            RewardDate = null;
            State = ExhibitionState.Inactive;
            Opponents = GenerateOpponents();
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            if (!hasFocus)
                Save();
        }

        private void OnDestroy()
        {
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

            if (!(RewardDate is null))
            {
                writer.Write("RewardDate", RewardDate);
            }

            writer.Write("Seeds", exhSeeds)
                .Write("SeedCount", SeedCount)
                .Write("NextExhibition", NextExhibition)
                .Write("State", State)
                .Write("Opponents", Opponents)
                .Write("WeeklyPlacements", WeeklyPlacements);

            writer.Commit();
        }

        public void Load()
        {
            PlayerSeeds = new List<Seed>();

            var reader = QSReader.Create("ExhibitionData");
            if (reader.TryRead("Seeds", out List<string> exhSeeds))
            {
                for (var i = 0; i < exhSeeds.Count; i++)
                {
                    if (exhSeeds[i] == "")
                        continue;

                    var seed = Seed.Create(exhSeeds[i]);
                    exhButtons[i].GetComponent<ExhibitionButton>().SetSeed(seed);

                    PlayerSeeds.Add(seed);
                }
            }

            var rand = new Random();
            SeedCount = reader.TryRead<int>("SeedCount", out var count)
                ? count
                : rand.Next(3) + 1;

            NextExhibition = reader.TryRead<DateTime>("NextExhibition", out var next)
                ? next
                : Now.Date.AddDays(1);

            RewardDate = reader.TryRead<DateTime>("RewardDate", out var rewardDate)
                ? rewardDate
                : (DateTime?) null;

            State = reader.TryRead<ExhibitionState>("State", out var state)
                ? state
                : ExhibitionState.Inactive;

            Opponents = reader.TryRead<Opponent[]>("Opponents", out var opponents)
                ? opponents
                : GenerateOpponents();

            WeeklyPlacements = reader.TryRead<int[]>("WeeklyPlacements", out var placements)
                ? placements
                : new int[7];
        }
    }

    public enum ExhibitionState
    {
        Inactive,
        InProgress,
        RewardPending,
        Finished
    }
}
