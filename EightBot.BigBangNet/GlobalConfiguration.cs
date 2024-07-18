using System;
namespace EightBot.BigBang
{
    public static class GlobalConfiguration
    {
        public static bool LogPerformanceMetrics { get; set; }

        public static TimeSpan LogBufferDuration { get; set; } = TimeSpan.FromSeconds(2);

        public static bool GenerateUITestViewNames { get; set; }
    }
}
