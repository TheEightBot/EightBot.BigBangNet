using System;
using Xamarin.Forms;

namespace EightBot.BigBang.XamForms.Effects
{
	public static class ShadowEffect
	{
		public static readonly BindableProperty ShadowDistanceProperty =
			BindableProperty.CreateAttached("ShadowDistance", typeof(double), typeof(ShadowEffect), 2d);

		public static double GetShadowDistance(BindableObject view)
		{
			return (double)view.GetValue(ShadowDistanceProperty);
		}

		public static void SetShadowDistance(BindableObject view, double value)
		{
			view.SetValue(ShadowDistanceProperty, value);
		}

		public static readonly BindableProperty CornerRadiusProperty =
			BindableProperty.CreateAttached("CornerRadius", typeof(double), typeof(ShadowEffect), 2d);

		public static double GetCornerRadius(BindableObject view)
		{
			return (double)view.GetValue(CornerRadiusProperty);
		}

		public static void SetCornerRadius(BindableObject view, double value)
		{
			view.SetValue(CornerRadiusProperty, value);
		}
	}

	public class ShadowRoutingEffect : RoutingEffect
	{
		public ShadowRoutingEffect() : base(EffectNames.ShadowEffect) { }
	}
}
