using System;
using Xamarin.Forms;

namespace EightBot.BigBang.XamForms.Effects
{
	public class TabBarColorEffect : RoutingEffect
	{
        public static readonly BindableProperty ActiveColorProperty =
        BindableProperty.CreateAttached("ActiveColor", typeof(Color), typeof(TabBarColorEffect), Color.White);

        public static Color GetActiveColor(BindableObject view)
        {
            return (Color)view.GetValue(ActiveColorProperty);
        }

        public static void SetActiveColor(BindableObject view, Color value)
        {
            view.SetValue(ActiveColorProperty, value);
        }

		public static readonly BindableProperty InactiveColorProperty =
		BindableProperty.CreateAttached("InactiveColor", typeof(Color), typeof(TabBarColorEffect), Color.LightGray);

		public static Color GetInactiveColor(BindableObject view)
		{
			return (Color)view.GetValue(InactiveColorProperty);
		}

		public static void SetInactiveColor(BindableObject view, Color value)
		{
			view.SetValue(InactiveColorProperty, value);
		}

		public TabBarColorEffect() : base(EffectNames.TabBarColorEffect) { }
	}
}
