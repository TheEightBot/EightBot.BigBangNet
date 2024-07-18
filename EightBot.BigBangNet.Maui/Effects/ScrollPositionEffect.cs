using System;
using Microsoft.Maui;

namespace EightBot.BigBang.Maui.Effects
{
    public delegate void ScrollActionEventHandler(object sender, ScrolledPositionEventArgs args);

    public class ScrolledPositionEventArgs : EventArgs
    {
        public ScrolledPositionEventArgs(Point location, int firstVisiblePosition, bool isInContact)
        {
            Location = location;
            IsInContact = isInContact;
            FirstVisiblePosition = firstVisiblePosition;
        }

        public int FirstVisiblePosition { get; set; }

        public Point Location { get; set; }

        public bool IsInContact { get; private set; }
    }

    public class ScrollPositionEffect : RoutingEffect
    {
        public event ScrollActionEventHandler ScrollAction;

        public void OnScrollAction(Element element, ScrolledPositionEventArgs args)
        {
            ScrollAction?.Invoke(element, args);
        }

        public ScrollPositionEffect() : base(EffectNames.ListViewScrollPositionEffect)
        {
        }
    }
}
