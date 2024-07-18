using System;
using EightBot.BigBang.Sample.ViewModels;
using EightBot.BigBang.XamForms.Pages;
using Xamarin.Forms;
using System.Reactive.Linq;
using System.Reactive.Disposables;
using EightBot.BigBang.XamForms;

namespace EightBot.BigBang.SampleApp.UserInterface.Pages
{
    public class NavigationTest : ContentPageBase<SampleViewModel>
    {
        Button _navBtn;

        Button _navModalBtn;

        public NavigationTest()
        {
            this.ViewModel = new SampleViewModel();
        }

        protected override void SetupUserInterface()
        {
            this.Content =
                new StackLayout
                {
                    Children =
                    {
                        (_navBtn = new Button { Text = "Nav Forward" }),
                        (_navModalBtn = new Button { Text = "Nav Modal" }),
                    }
                };
        }

        protected override void BindControls()
        {
            Observable
                .FromEventPattern(
                    x => _navBtn.Clicked += x,
                    x => _navBtn.Clicked -= x)
                .NavigateToPage(
                    this,
                    _ => new NavigationTest())
                .DisposeWith(ControlBindings);

            Observable
                .FromEventPattern(
                    x => _navModalBtn.Clicked += x,
                    x => _navModalBtn.Clicked -= x)
                .NavigateToPopupPage(_ => new PopupNavigationTest())
                .DisposeWith(ControlBindings);
        }

    }
}
