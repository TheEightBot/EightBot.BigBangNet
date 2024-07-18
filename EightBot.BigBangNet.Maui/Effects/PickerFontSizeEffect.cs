using System;
using Microsoft.Maui;

namespace EightBot.BigBang.Maui.Effects
{
    public static class PickerFontSizeEffect
    {
        public static readonly BindableProperty FontSizeProperty =
            BindableProperty.CreateAttached("FontSize", typeof(double), typeof(PickerFontSizeEffect), 17d);

        public static double GetFontSize(BindableObject view)
        {
            return (double)view.GetValue(FontSizeProperty);
        }

        public static void SetFontSize(BindableObject view, double value)
        {
            view.SetValue(FontSizeProperty, value);
        }
    }

    public class PickerFontSizeRoutingEffect : RoutingEffect
    {
        public PickerFontSizeRoutingEffect() : base(EffectNames.PickerFontSizeEffect) { }
    }
}
