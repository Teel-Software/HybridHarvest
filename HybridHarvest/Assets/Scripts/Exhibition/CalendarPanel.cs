using System;
using TMPro;
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
            var todayIndex = ((int)DateTime.Today.DayOfWeek + 6) % 7;
            dayIcons[todayIndex].Background.color = new Color(1, 0, 0, 0.75f);
        }

        public void Update()
        {
            var now = DateTime.Now;
            var today = DateTime.Today;
#if DEBUG
            var i = 0;
            now = now.AddDays(i);
            today = today.AddDays(i);
#endif
            var next = today.AddDays(1);
            var timeSpan = next - now;
            if (timeSpan.TotalDays < 1)
            {
                timeTillNext.text = $"{timeSpan.Hours} ч. {timeSpan.Minutes} мин.";
            }
            else
            {
                timeTillNext.text = $"{timeSpan.Days} дн. {timeSpan.Hours} ч.";
            }
        }
    } 
}

