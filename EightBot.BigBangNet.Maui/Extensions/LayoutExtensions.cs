using System;
using Xamarin.Forms;
using System.Linq.Expressions;

namespace EightBot.BigBang.XamForms
{
	public static class LayoutExtensions
	{
		public static void Add(this StackLayout sl, View item) {
			sl?.Children?.Add (item);
		}

		public static void Add(
			this RelativeLayout rl, View item, 
			Expression<Func<double>> x = null, Expression<Func<double>> y = null,
			Expression<Func<double>> width = null, Expression<Func<double>> height = null
		){
			rl?.Children?.Add (item, x, y, width, height);
		}

		public static void Add(
			this RelativeLayout rl, View item, 
			Constraint xConstraint = null, Constraint yConstraint = null,
			Constraint widthConstraint = null, Constraint heightConstraint = null
		){
			rl?.Children?. Add(item, xConstraint, yConstraint, widthConstraint, heightConstraint);
		}

		public static void Add(this RelativeLayout rl, View item, Expression<Func<Rectangle>> bounds){
			rl?.Children?. Add(item, bounds);
		}

		public static void Add(this AbsoluteLayout al, View item){
			al?.Children?.Add (item);
		}

		public static void Add(this AbsoluteLayout al, View item, Point position){
			al?.Children?.Add (item, position);
		}

		public static void Add(this AbsoluteLayout al, View item, Rectangle bounds, AbsoluteLayoutFlags flags = 0){
			al?.Children?.Add (item, bounds, flags);
		}
	}
}