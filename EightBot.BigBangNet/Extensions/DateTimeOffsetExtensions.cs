using System;

namespace EightBot.BigBang
{
    public static class DateTimeOffsetExtensions
    {
        public static DateTimeOffset RoundToNearestHour(this DateTimeOffset input)
        {
            var dt = new DateTimeOffset(new DateTime(input.Year, input.Month, input.Day, input.Hour, 0, 0, input.DateTime.Kind), input.Offset);

            return input.Minute > 29
                ? dt.AddHours(1)
                : dt;
        }

        public static DateTimeOffset RoundUp(this DateTimeOffset dt, TimeSpan d)
        {
            var delta = (d.Ticks - (dt.Ticks % d.Ticks)) % d.Ticks;
            return new DateTimeOffset(new DateTime(dt.Ticks + delta, dt.DateTime.Kind), dt.Offset);
        }

        public static DateTimeOffset RoundDown(this DateTimeOffset dt, TimeSpan d)
        {
            var delta = dt.Ticks % d.Ticks;
            return new DateTimeOffset(new DateTime(dt.Ticks - delta, dt.DateTime.Kind), dt.Offset);
        }

        public static DateTimeOffset RoundToNearestQuarter(this DateTimeOffset dt, TimeSpan d)
        {
            var delta = dt.Ticks % d.Ticks;
            bool roundUp = delta > d.Ticks / 2;

            return roundUp ? dt.RoundUp(d) : dt.RoundDown(d);
        }

        public static DateTimeOffset Next(this DateTimeOffset from, DayOfWeek dayOfWeek)
        {
            int start = (int)from.DayOfWeek;
            int target = (int)dayOfWeek;
            if (target <= start)
                target += 7;
            return from.AddDays(target - start);
        }

        public static long ToUnixTimestamp(System.DateTimeOffset dt)
        {
            var unixRef = new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero);
            return (dt.Ticks - unixRef.Ticks) / 10000000;
        }

        public static DateTimeOffset FromUnixTimestamp(long timestamp)
        {
            var unixRef = new DateTimeOffset(1970, 1, 1, 0, 0, 0, 0, TimeSpan.Zero);
            return unixRef.AddSeconds(timestamp);
        }
    }
}

