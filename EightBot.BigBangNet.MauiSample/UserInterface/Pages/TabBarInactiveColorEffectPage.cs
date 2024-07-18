using System;
using EightBot.BigBang.XamForms.Effects;
using Xamarin.Forms;

namespace EightBot.BigBang.SampleApp.UserInterface.Pages
{
	public class TabBarInactiveColorEffectPage : TabbedPage
	{
		public TabBarInactiveColorEffectPage()
		{
            var rngesus = new Random(Guid.NewGuid().GetHashCode());

			var effect = new TabBarColorEffect();
			var changeColorButton = new Button
			{
				Text = "Change Title"
			};

            TabBarColorEffect.SetActiveColor(this, Color.FromRgba(rngesus.Next(0, 255), rngesus.Next(0, 255), rngesus.Next(0, 255), rngesus.Next(0, 255)));
            TabBarColorEffect.SetInactiveColor(this, Color.FromRgba(rngesus.Next(0, 255), rngesus.Next(0, 255), rngesus.Next(0, 255), rngesus.Next(0, 255)));

            changeColorButton.Command = new Command(() => {
                TabBarColorEffect.SetActiveColor(this, Color.FromRgba(rngesus.Next(0, 255), rngesus.Next(0, 255), rngesus.Next(0, 255), rngesus.Next(0, 255)));
				TabBarColorEffect.SetInactiveColor(this, Color.FromRgba(rngesus.Next(0, 255), rngesus.Next(0, 255), rngesus.Next(0, 255), rngesus.Next(0, 255)));
			});

			this.Effects.Add(effect);
			this.Children.Add(new ContentPage { Title = "Page 1", Icon="galaxy.png", Content = changeColorButton });
            this.Children.Add(new ContentPage { Title = "Page 2", Icon="galaxy.png" });
		}
	}
}

