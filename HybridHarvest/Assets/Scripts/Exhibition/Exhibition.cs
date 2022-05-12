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

        public int SeedCount { get; private set; }
        public void OnEnable()
        {
            Load();
            
            foreach (var btn in exhButtons)
                btn.gameObject.SetActive(false);
            foreach (var btn in exhButtons.Take(SeedCount))
                btn.gameObject.SetActive(true);
            
            var date = DateTime.Now;
            foreach (var btn in exhButtons)
            {
                btn.onClick.RemoveAllListeners();
                switch (date.DayOfWeek)
                {
                    case DayOfWeek.Saturday:
                        btn.onClick.AddListener(btn.GetComponent<ExhibitionButton>().ExhibitionClick);
                        break;
                    case DayOfWeek.Sunday:
                        btn.onClick.AddListener(btn.GetComponent<ExhibitionButton>().ResultClick);
                        break;
                    default:
                        btn.onClick.AddListener(btn.GetComponent<ExhibitionButton>().AddSeed);
                        break;
                }   
            }
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            if (!hasFocus)
                Save();
        }

        public void InitializeExhibition()
        {
            
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
            
            if (reader.TryRead<int>("SeedCount", out var count))
            {
                SeedCount = count;
            }
            else
            {
                var rand = new Random();
                SeedCount = rand.Next(1, 4);
            }
        }
    }
}
