using System;
using Microsoft.Maui;

namespace EightBot.BigBang.Maui
{
    public static class ColorExtensions
    {
        public static Color Lerp(this Color color, Color to, double amount)
        {
            return Color.FromRgb(
                color.Red.Lerp(to.Red, amount),
                color.Green.Lerp(to.Green, amount),
                color.Blue.Lerp(to.Blue, amount)
            );
        }

        public static double Lerp(this double start, double end, double amount)
        {
            double difference = end - start;
            double adjusted = difference * amount;
            return start + adjusted;
        }

        public static float Lerp(this float start, float end, double amount)
        {
            float difference = end - start;
            float adjusted = difference * (float)amount;
            return start + adjusted;
        }

        public static Color WithAlpha(this Color color, double opacity)
        {
            return Color.FromRgba(color.Red, color.Green, color.Blue, opacity);
        }

        public static string HexString(this Color color)
        {
            var alpha = (int)(color.Alpha * 255);
            var red = (int)(color.Red * 255);
            var green = (int)(color.Green * 255);
            var blue = (int)(color.Blue * 255);
            var hex = $"#{alpha:X2}{red:X2}{green:X2}{blue:X2}";

            return hex;
        }
    }
}

