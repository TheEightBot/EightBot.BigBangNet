using System;
using EightBot.BigBang.Sample.ViewModels;
using EightBot.BigBang.Maui.Pages;
using Microsoft.Maui;
using System.Reactive.Linq;
using System.Reactive.Disposables;
using EightBot.BigBang.Maui;
using EightBot.BigBang.Maui.PopUp;

namespace EightBot.BigBang.SampleApp.UserInterface.Pages
{
    public class PopupNavigationTest : PopupPageBase<SampleViewModel>
    {
        Button _navBtn;

        Button _navModalBtn;

        public PopupNavigationTest()
        {
            this.ViewModel = new SampleViewModel();
        }

        protected override void SetupUserInterface()
        {
            this.Content =
                new StackLayout
                {
                    BackgroundColor = Colors.White,
                    Children =
                    {
                        (_navBtn = new Button { Text = "Push Modal", VerticalOptions = LayoutOptions.CenterAndExpand, }),
                        (_navModalBtn = new Button { Text = "Pop Modal", VerticalOptions = LayoutOptions.CenterAndExpand, }),
                    }
                };
        }

        protected override void BindControls()
        {
            Observable
                .FromEventPattern(
                    x => _navBtn.Clicked += x,
                    x => _navBtn.Clicked -= x)
                .NavigateToPopupPage(_ => new PopupNavigationTest())
                .DisposeWith(ControlBindings);

            Observable
                .FromEventPattern(
                    x => _navModalBtn.Clicked += x,
                    x => _navModalBtn.Clicked -= x)
                .NavigatePopPopupPage()
                .DisposeWith(ControlBindings);
        }

    }
}
