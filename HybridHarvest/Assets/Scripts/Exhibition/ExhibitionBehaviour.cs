using System;
using UnityEngine;
using UnityEngine.UI;

public class ExhibitionBehaviour : MonoBehaviour
{
    [SerializeField] public Button[] exhButtons;
    private DateTime date;

    void OnEnable()
    {
        date = DateTime.Now;
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
