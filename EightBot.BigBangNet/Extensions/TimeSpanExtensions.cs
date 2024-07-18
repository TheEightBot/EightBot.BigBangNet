using System;
using System.Globalization;

namespace EightBot.BigBang
{
    public enum DateInterval
    {
        Days,
        Hours,
        Minutes,
        Seconds,
        Milliseconds
    }

    public static class TimeSpanExtensions
    {
        public static string ToAmPmFormatString(this TimeSpan ts)
        {
            var dateTime = DateTime.Now.Date.Add(ts);
            return dateTime.ToString("hh:mm") + dateTime.ToString("tt").ToLowerInvariant();
        }

        public static TimeSpan RoundTo(this TimeSpan timeSpan, DateInterval interval, int n)
        {
            switch (interval)
            {
                case DateInterval.Days:
                    return TimeSpan.FromDays(n * Math.Ceiling(timeSpan.TotalDays / n));
                case DateInterval.Hours:
                    return TimeSpan.FromHours(n * Math.Ceiling(timeSpan.TotalHours / n));
                case DateInterval.Minutes:
                    return TimeSpan.FromMinutes(n * Math.Ceiling(timeSpan.TotalMinutes / n));
                case DateInterval.Seconds:
                    return TimeSpan.FromSeconds(n * Math.Ceiling(timeSpan.TotalSeconds / n));
                case DateInterval.Milliseconds:
                    return TimeSpan.FromMilliseconds(n * Math.Ceiling(timeSpan.TotalMilliseconds / n));
                default:
                    return timeSpan;
            }
        }
    }
}

