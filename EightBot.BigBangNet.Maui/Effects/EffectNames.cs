using System;
using EightBot.BigBang.Maui.Effects;
using Microsoft.Maui;

namespace EightBot.BigBang.Maui.Effects
{
    public static class EffectNames
    {
        public const string
            EffectNameRoot = "EightBot",
            SelectTextOnFocusEffect = EffectNameRoot + "." + nameof(SelectTextOnFocusEffect),
            NoCellSelectionEffect = EffectNameRoot + "." + nameof(NoCellSelectionEffect),
            ListViewHideEmptyCellsEffect = EffectNameRoot + "." + nameof(ListViewHideEmptyCellsEffect),
            KeyboardReturnKeyTypeNameEffect = EffectNameRoot + "." + nameof(KeyboardReturnKeyTypeNameEffect),
            ShadowEffect = EffectNameRoot + "." + nameof(ShadowEffect),
            NoControlChromeEffect = EffectNameRoot + "." + nameof(NoControlChromeEffect),
            ShowKeyboardDoneButtonEffect = EffectNameRoot + "." + nameof(ShowKeyboardDoneButtonEffect),
            PickerFontSizeEffect = EffectNameRoot + "." + nameof(PickerFontSizeEffect),
            TabBarColorEffect = EffectNameRoot + "." + nameof(TabBarColorEffect),
            ListViewScrollPositionEffect = EffectNameRoot + "." + nameof(ListViewScrollPositionEffect),
            ListViewActivationEffect = EffectNameRoot + "." + nameof(ListViewActivationEffect);

        public class SelectTextOnFocusRoutingEffect : RoutingEffect
        {
            public SelectTextOnFocusRoutingEffect() : base(SelectTextOnFocusEffect) { }
        }

        public class NoCellSelectionStyleRoutingEffect : RoutingEffect
        {
            public NoCellSelectionStyleRoutingEffect() : base(NoCellSelectionEffect) { }
        }

        public class ListViewHideEmptyCellsRoutingEffect : RoutingEffect
        {
            public ListViewHideEmptyCellsRoutingEffect() : base(ListViewHideEmptyCellsEffect) { }
        }

        public class KeyboardReturnKeyTypeNameRoutingEffect : RoutingEffect
        {
            public KeyboardReturnKeyTypeNameRoutingEffect() : base(KeyboardReturnKeyTypeNameEffect) { }
        }

        public class NoControlChromeEffectRoutingEffect : RoutingEffect
        {
            public NoControlChromeEffectRoutingEffect() : base(NoControlChromeEffect) { }
        }

        public class ShowKeyboardDoneButtonRoutingEffect : RoutingEffect
        {
            public ShowKeyboardDoneButtonRoutingEffect() : base(ShowKeyboardDoneButtonEffect) { }
        }

        public class ListViewActivationRoutingEffect : RoutingEffect
        {
            public ListViewActivationRoutingEffect() : base(ListViewActivationEffect) { }
        }
    }
}
