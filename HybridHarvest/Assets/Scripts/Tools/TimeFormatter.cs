using System;

namespace Tools
{
    public static class TimeFormatter
    {
        public static string Format(int seconds)
        {
            return Format(TimeSpan.FromSeconds(seconds));
        }
        
        public static string Format(TimeSpan timeSpan)
        {
            string result;
            if (timeSpan.Hours > 9)
            {
                result = timeSpan.Hours + " ч.";
            }
            else
            {
                if (timeSpan.Hours != 0)
                    result = timeSpan.Hours + " ч. " + timeSpan.Minutes + " м.";
                else
                {
                    if (timeSpan.Minutes > 9)
                        result = timeSpan.Minutes + " м.";
                    else
                    {
                        if (timeSpan.Minutes != 0)
                            result = timeSpan.Minutes + " м. " + timeSpan.Seconds + " c.";
                        else
                            result = timeSpan.Seconds + " c.";
                    }
                }
            }
            return result;
        }

        public static string FormatTotableView(int seconds)
        {
            var timeSpan = TimeSpan.FromSeconds(seconds);
            return timeSpan.ToString(@"hh\:mm\:ss");
        }
    }
}