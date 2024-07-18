using System;

namespace EightBot.BigBang
{
    public static class DateTimeExtensions
    {
        public static DateTime RoundToNearestHour(this DateTime input)
        {
            var dt = new DateTime(input.Year, input.Month, input.Day, input.Hour, 0, 0, input.Kind);

            return input.Minute > 29
                ? dt.AddHours(1)
                : dt;
        }

        public static DateTime RoundUpToNearestHour(this DateTime input)
        {
            var dt = new DateTime(input.Year, input.Month, input.Day, input.Hour, 0, 0, input.Kind);

            return dt.AddHours(1);
        }

        public static DateTime RoundDownToNearestHour(this DateTime input)
        {
            var dt = new DateTime(input.Year, input.Month, input.Day, input.Hour, 0, 0, input.Kind);

            return dt;
        }

        public static DateTime RoundDownToNearestMinute(this DateTime input)
        {
            var dt = new DateTime(input.Year, input.Month, input.Day, input.Hour, input.Minute, 0, input.Kind);

            return dt;
        }

        public static DateTime RoundUp(this DateTime dt, TimeSpan d)
        {
            var modTicks = dt.Ticks % d.Ticks;
            var delta = modTicks != 0 ? d.Ticks - modTicks : 0;
            return new DateTime(dt.Ticks + delta, dt.Kind);
        }

        public static DateTime RoundDown(this DateTime dt, TimeSpan d)
        {
            var delta = dt.Ticks % d.Ticks;
            return new DateTime(dt.Ticks - delta, dt.Kind);
        }

        public static DateTime RoundToNearest(this DateTime dt, TimeSpan d)
        {
            var delta = dt.Ticks % d.Ticks;
            bool roundUp = delta > d.Ticks / 2;
            var offset = roundUp ? d.Ticks : 0;

            return new DateTime(dt.Ticks + offset - delta, dt.Kind);
        }

        public static DateTime RoundToNearestQuarter(this DateTime dt, TimeSpan d)
        {
            var delta = dt.Ticks % d.Ticks;
            bool roundUp = delta > d.Ticks / 2;

            return roundUp ? dt.RoundUp(d) : dt.RoundDown(d);
        }

        public static DateTime Next(this DateTime from, DayOfWeek dayOfWeek)
        {
            int start = (int)from.DayOfWeek;
            int target = (int)dayOfWeek;
            if (target <= start)
                target += 7;
            return from.AddDays(target - start);
        }

        public static long ToUnixTimestamp(this System.DateTime dt)
        {
            var unixRef = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return (dt.Ticks - unixRef.Ticks) / 10000000;
        }

        public static DateTime FromUnixTimestamp(long timestamp)
        {
            var unixRef = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return unixRef.AddSeconds(timestamp);
        }
    }
}

