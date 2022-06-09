using System;
using TMPro;
using Tools;
using UnityEngine;
using UnityEngine.UI;

namespace Exhibition
{
    public class CalendarPanel : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI timeTillNext;
        [SerializeField] private HorizontalLayoutGroup dayContainer;
        [SerializeField] private Button startBossButton;
        
        public void Awake()
        {
            var dayIcons = dayContainer.GetComponentsInChildren<DayIcon>();
            var todayIndex = GetComponentInParent<Exhibition>().DayIndex;
            dayIcons[todayIndex].Background.color = new Color(1, 0, 0, 0.75f);
            startBossButton.gameObject.SetActive(todayIndex > 4);
        }

        public void OnEnable()
        {
            var dayIcons = dayContainer.GetComponentsInChildren<DayIcon>();
            var placements = GetComponentInParent<Exhibition>().WeeklyPlacements;
            for (var i = 0; i < dayIcons.Length; i++)
            {
                dayIcons[i].SetPlace(placements[i]);
            }
        }

        public void Update()
        {
            var next = GetComponentInParent<Exhibition>().NextExhibition;
            var now = GetComponentInParent<Exhibition>().Now;
            timeTillNext.text = TimeFormatter.Format(next - now);
        }
    } 
}

