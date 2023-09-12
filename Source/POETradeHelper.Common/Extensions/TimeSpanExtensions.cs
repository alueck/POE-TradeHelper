using System;

using POETradeHelper.Common.Properties;

namespace POETradeHelper.Common.Extensions
{
    public static class TimeSpanExtensions
    {
        public static string ToHumanReadableString(this TimeSpan timeSpan)
        {
            double time;
            string unit;
            if (IsRoundedAtLeastOneDay(timeSpan))
            {
                time = timeSpan.TotalDays;
                unit = Resources.Days;
            }
            else if (IsRoundedAtLeastOneHour(timeSpan))
            {
                time = timeSpan.TotalHours;
                unit = Resources.Hours;
            }
            else if (IsRoundedAtLeastOneMinute(timeSpan))
            {
                time = timeSpan.TotalMinutes;
                unit = Resources.Minutes;
            }
            else
            {
                time = timeSpan.TotalSeconds;
                unit = Resources.Seconds;
            }

            return $"{time,2:N0}{unit}";
        }

        private static bool IsRoundedAtLeastOneDay(TimeSpan timeSpan) =>
            timeSpan.TotalDays >= 1 || Math.Round(timeSpan.TotalHours) == 24;

        private static bool IsRoundedAtLeastOneHour(TimeSpan timeSpan) =>
            timeSpan.TotalHours >= 1 || Math.Round(timeSpan.TotalMinutes) == 60;

        private static bool IsRoundedAtLeastOneMinute(TimeSpan timeSpan) =>
            timeSpan.TotalMinutes >= 1 || Math.Round(timeSpan.TotalSeconds) == 60;
    }
}