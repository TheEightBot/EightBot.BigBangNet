using System;
using System.Linq;
using Microsoft.Maui;

namespace EightBot.BigBang.Maui.AttachedProperties
{
    public class ListViewHideEmptyCellsProperty
    {
        public const string HideEmptyCellsPropertyName = "HideEmptyCells";

        public static bool GetHideEmptyCells(BindableObject view)
        {
            return (bool)view.GetValue(HideEmptyCellsProperty);
        }

        public static void SetHideEmptyCells(BindableObject view, object value)
        {
            view.SetValue(HideEmptyCellsProperty, value);
        }

        public static BindableProperty HideEmptyCellsProperty =
            BindableProperty.CreateAttached(HideEmptyCellsPropertyName, typeof(bool),
                typeof(ListView), false, defaultBindingMode: BindingMode.Default, propertyChanged: OnEventNameChanged);

        static void OnEventNameChanged(BindableObject bindable, object oldValue, object newValue)
        {

            var listView = bindable as ListView;
            if (listView == null)
            {
                return;
            }

            var enable = (bool)newValue;

            var existingHideEmptyCellsEffect = listView.Effects.FirstOrDefault(x => x.ResolveId == Effects.EffectNames.ListViewHideEmptyCellsEffect);

            if (enable && existingHideEmptyCellsEffect == null)
            {
                var hideEmptyCellsEffect = Effect.Resolve(Effects.EffectNames.ListViewHideEmptyCellsEffect);
                listView.Effects.Add(hideEmptyCellsEffect);
            }
            else if (existingHideEmptyCellsEffect != null)
                listView.Effects.Remove(existingHideEmptyCellsEffect);
        }
    }
}
