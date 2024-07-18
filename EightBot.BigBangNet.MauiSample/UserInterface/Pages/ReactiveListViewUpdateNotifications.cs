using System;
using EightBot.BigBang.Sample.ViewModels;
using EightBot.BigBang.XamForms.Pages;
using EightBot.BigBang.XamForms.Views;
using ReactiveUI;
using Xamarin.Forms;
using System.Reactive.Disposables;
using EightBot.BigBang.XamForms;

namespace EightBot.BigBang.SampleApp.UserInterface.Pages
{
    public class ReactiveListViewUpdateNotifications : ContentPageBase<ReactiveListViewUpdateNotificationsViewModel>, ICanActivate
    {
        ReactiveListView _listView;

        public ReactiveListViewUpdateNotifications()
        {
            ViewModel = new ReactiveListViewUpdateNotificationsViewModel();
        }

        protected override void SetupUserInterface()
        {
            _listView = new ReactiveListView(typeof(Cells.SampleViewModelCell));

            this.Content = _listView;
        }

        protected override void BindControls()
        {
            _listView.Bind(this.WhenAnyValue(x => x.ViewModel.SampleList))
                .DisposeWith(ControlBindings);

            this.WhenAnyObservable(x => x.Activated)
                .InvokeCommand(this, x => x.ViewModel.StartMonitoring)
                .DisposeWith(ControlBindings);

            this.WhenAnyObservable(x => x.Deactivated)
                .InvokeCommand(this, x => x.ViewModel.StopMonitoring)
                .DisposeWith(ControlBindings);
        }

    }
}
