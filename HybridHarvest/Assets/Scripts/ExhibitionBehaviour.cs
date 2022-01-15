using System;
using UnityEngine;
using UnityEngine.UI;

public class ExhibitionBehaviour : MonoBehaviour
{
    [SerializeField] public Button exhButton;
    DateTime date;

    void OnEnable()
    {
        date = DateTime.Now;
        exhButton.onClick.RemoveAllListeners();

        switch (date.DayOfWeek)
        {
            case DayOfWeek.Saturday:
                exhButton.onClick.AddListener(exhButton.GetComponent<ExhibitionButton>().ExhibitionClick);
                break;
            case DayOfWeek.Sunday:
                exhButton.onClick.AddListener(exhButton.GetComponent<ExhibitionButton>().ResultClick);
                break;
            default:
                exhButton.onClick.AddListener(exhButton.GetComponent<ExhibitionButton>().DefaultClick);
                break;
        }
    }
}
