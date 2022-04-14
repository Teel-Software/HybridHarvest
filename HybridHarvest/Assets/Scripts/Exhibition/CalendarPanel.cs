using System;
using TMPro;
using UnityEngine;

namespace Exhibition
{
    public class CalendarPanel : MonoBehaviour
    {
        [SerializeField] 
        private TextMeshProUGUI timeTillNext;

        public void Update()
        {
            var now = DateTime.Now;
            var today = DateTime.Today;
            var i = 0;
            if (i != 0)
            {
                now = now.AddDays(i);
                today = today.AddDays(i);
            }
            var days = DayOfWeek.Saturday - today.DayOfWeek;
            var next = today.AddDays(days);
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

