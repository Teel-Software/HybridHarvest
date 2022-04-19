using System;

namespace Tools
{
    public static class TimeFormatter
    {
        public static string Format(int seconds) =>
            Format(TimeSpan.FromSeconds(seconds));

        public static string Format(TimeSpan timeSpan)
        {
            return timeSpan.Hours > 0
                ? timeSpan.Hours + " ч. " + timeSpan.Minutes + " м."
                : timeSpan.Minutes > 0
                    ? timeSpan.Minutes + " м. " + timeSpan.Seconds + " c."
                    : timeSpan.Seconds + " c.";
        }

        public static string FormatToTableView(int seconds)
        {
            var timeSpan = TimeSpan.FromSeconds(seconds);
            return timeSpan.ToString(@"hh\:mm\:ss");
        }
    }
}
