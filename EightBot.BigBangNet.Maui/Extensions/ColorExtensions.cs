using System;
using Xamarin.Forms;

namespace EightBot.BigBang.XamForms
{
	public static class ColorExtensions
	{
		public static Color Lerp(this Color color, Color to, double amount)
		{
			return Color.FromRgb(
				color.R.Lerp(to.R, amount), 
				color.G.Lerp(to.G, amount), 
				color.B.Lerp(to.B, amount)
			);
		}

		public static double Lerp( this double start, double end, double amount)
		{
			double difference = end - start;
			double adjusted = difference * amount;
			return start + adjusted;
		}

        public static Color WithAlpha(this Color color, double opacity)
        {
            return Color.FromRgba(color.R, color.G, color.B, opacity);
        }

        public static string HexString(this Color color)
        {
            var alpha = (int)(color.A * 255);
            var red = (int)(color.R * 255);
            var green = (int)(color.G * 255);
            var blue = (int)(color.B * 255);
            var hex = $"#{alpha:X2}{red:X2}{green:X2}{blue:X2}";

            return hex;
        }
	}
}

