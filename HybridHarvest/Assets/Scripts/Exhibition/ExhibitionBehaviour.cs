using System;
using System.Collections.Generic;
using CI.QuickSave;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

public class ExhibitionBehaviour : MonoBehaviour, ISaveable
{
    [SerializeField] private Button[] exhButtons;
    [SerializeField] private Image testImage;
        
    public void Start()
    {
        
    }   

    public void OnEnable()
    {
        Load();
        var epic = false;
        if (epic)
        {
            foreach (var btn in exhButtons)
                btn.gameObject.SetActive(true);
            var rand = new Random();
            var count = rand.Next(0, 3);
            for (var i = 1; i <= count; i++)
                exhButtons[exhButtons.Length - i].gameObject.SetActive(false);
        }

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
                    btn.onClick.AddListener(btn.GetComponent<ExhibitionButton>().DefaultClick);
                    break;
            }   
        }
    }

    private void OnDisable()
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
        writer.Write("Seeds", exhSeeds);
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
    }
}
