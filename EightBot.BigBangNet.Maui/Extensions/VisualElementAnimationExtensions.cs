using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace EightBot.BigBang.XamForms
{
	public static class VisualElementAnimationExtensions
	{
        /// <summary>
        /// Use this when things go wrong and you want the control to wiggle
        /// </summary>
        /// <returns>an awaitable task</returns>
        /// <param name="view">Any visual element will work out alright here</param>
        /// <param name="shakeAmount">If you don't pass a value here, a default of 10% of the control width will be used. 
        /// Otherwise, you need to provide a value greater than 1 for it to be considered outright. Anything less than 1 is treated like a percentage against the View's width</param>
		public static async Task InvalidShake(this VisualElement view, double shakeAmount = 0d) {
            var wiggleAmount = (int)(
                shakeAmount > 1d 
                    ? shakeAmount 
                    : shakeAmount > 0
						? view.Width * shakeAmount 
                        : view.Width * .1d);
			var rng = new Random(Guid.NewGuid().GetHashCode());
			await view.TranslateTo(rng.Next(wiggleAmount), 0d, 60, Easing.CubicOut);
			await view.TranslateTo(0, 0d, 60, Easing.CubicIn);
			await view.TranslateTo(-rng.Next(wiggleAmount), 0d, 60, Easing.CubicOut);
			await view.TranslateTo(0, 0d, 60, Easing.CubicIn);
			await view.TranslateTo(rng.Next(wiggleAmount), 0d, 60, Easing.CubicOut);
			await view.TranslateTo(0, 0d, 60, Easing.CubicIn);
			await view.TranslateTo(-rng.Next(wiggleAmount), 0d, 60, Easing.CubicOut);
			await view.TranslateTo(0, 0d, 60, Easing.CubicIn);
		}
	}
}
