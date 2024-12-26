using System;

namespace BoardGameFrontend.Helpers
{
    public static class TimeFormatHelper
    {
        // Method that formats TimeSpan into the desired string format
        public static string FormatTimeSpan(TimeSpan timeSpan)
        {
            int minutes = (int)timeSpan.TotalMinutes;
            int seconds = timeSpan.Seconds;

            if (minutes > 0)
            {
                // If there are minutes, include them
                if (seconds > 0)
                {
                    return $"{minutes}m{seconds}s";
                }
                else
                {
                    return $"{minutes}m";
                }
            }
            else
            {
                // Only seconds if less than a minute
                return $"{seconds}s";
            }
        }
    }
}
