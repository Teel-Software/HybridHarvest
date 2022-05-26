using System;
using TMPro;
using Tools;
using UnityEngine;
using UnityEngine.UI;

namespace Exhibition
{
    public class CalendarPanel : MonoBehaviour
    {
        [SerializeField] 
        private TextMeshProUGUI timeTillNext;
        [SerializeField] 
        private HorizontalLayoutGroup dayContainer;
        public void Awake()
        {
            var dayIcons = dayContainer.GetComponentsInChildren<DayIcon>();
            var dayOfWeek = GetComponentInParent<Exhibition>().Now.DayOfWeek;
            var todayIndex = ((int)dayOfWeek + 6) % 7;
            dayIcons[todayIndex].Background.color = new Color(1, 0, 0, 0.75f);
        }

        public void Update()
        {
            var next = GetComponentInParent<Exhibition>().NextExhibition;
            var now = GetComponentInParent<Exhibition>().Now;
            timeTillNext.text = TimeFormatter.Format(next - now);
        }
    } 
}

