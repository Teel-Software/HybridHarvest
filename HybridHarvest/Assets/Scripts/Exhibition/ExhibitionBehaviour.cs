using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

public class ExhibitionBehaviour : MonoBehaviour
{
    [SerializeField] public Button[] exhButtons;

    void OnEnable()
    {
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
}
