using System;
using System.Drawing;
using Microsoft.Maui;
using System.Linq.Expressions;
using Microsoft.Maui.Controls.Compatibility;
using Microsoft.Maui.Layouts;
using AbsoluteLayout = Microsoft.Maui.Controls.AbsoluteLayout;
using Point = Microsoft.Maui.Graphics.Point;
using StackLayout = Microsoft.Maui.Controls.StackLayout;

namespace EightBot.BigBang.Maui
{
    public static class LayoutExtensions
    {
        public static void Add(this StackLayout sl, View item)
        {
            sl?.Children?.Add(item);
        }

        public static void Add(
            this RelativeLayout rl, View item,
            Expression<Func<double>> x = null, Expression<Func<double>> y = null,
            Expression<Func<double>> width = null, Expression<Func<double>> height = null
        )
        {
            rl?.Children?.Add(item, x, y, width, height);
        }

        public static void Add(
            this RelativeLayout rl, View item,
            Constraint xConstraint = null, Constraint yConstraint = null,
            Constraint widthConstraint = null, Constraint heightConstraint = null
        )
        {
            rl?.Children?.Add(item, xConstraint, yConstraint, widthConstraint, heightConstraint);
        }

        public static void Add(this RelativeLayout rl, View item, Expression<Func<Rect>> bounds)
        {
            rl?.Children?.Add(item, bounds);
        }

        public static void Add(this AbsoluteLayout al, View item)
        {
            al?.Children?.Add(item);
        }
    }
}