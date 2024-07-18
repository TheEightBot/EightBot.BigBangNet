using System;

namespace EightBot.BigBang.Interfaces
{
    public interface IDeviceInfo
    {
        string OperatingSystemVersion { get; }

        string ApplicationVersion { get; }

        string ApplicationBuildNumber { get; }

        string FormattedApplicationVersionAndBuildNumber { get; }

        string UniqueAppIdentifier { get; }

        string AppDataDirectory { get; }

        string CacheDirectory { get; }

        double PixelDensity { get; }
    }
}

