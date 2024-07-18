using System;
using System.Reactive.Linq;
using EightBot.BigBang.Sample.Models;
using EightBot.BigBang.Sample.ViewModels;
using EightBot.BigBang.XamForms.Pages;
using EightBot.BigBang.XamForms.Views;
using ReactiveUI;
using Xamarin.Forms;
using System.Reactive.Disposables;
using EightBot.BigBang.XamForms;

namespace EightBot.BigBang.SampleApp.UserInterface.Pages
{
    public class ReactiveListViewPinnedViewModelPage : ContentPageBase<ReactiveListViewPinnedViewModel>
    {
        ReactiveListView _listView;

        public ReactiveListViewPinnedViewModelPage()
        {
            ViewModel = new ReactiveListViewPinnedViewModel();
        }

        protected override void SetupUserInterface()
        {
            _listView = new ReactiveListView(typeof(Cells.SamplePinnedViewModelCell));

            this.Content = _listView;
        }

        protected override void BindControls()
        {
            _listView.Bind(this.WhenAnyValue(x => x.ViewModel.SampleList))
                .DisposeWith(ControlBindings);

            _listView
                .ListViewItemTapped<SampleModel>()
                 .ObserveOn(RxApp.MainThreadScheduler)
                 .Subscribe(_ => System.Diagnostics.Debug.WriteLine($"Tapped: {_}"))
                 .DisposeWith(ControlBindings);

            _listView
                .WhenCellActivated(
                    (disposable, cell, index) =>
                    {
                        if (cell is Cells.SampleViewModelCell svmc)
                        {
                            svmc.WhenAnyObservable(x => x.ViewModel.UnitCommand)
                                .InvokeCommand(this, x => x.ViewModel.SelectionUnitSample)
                                .DisposeWith(disposable);
                        }
                    })
                .DisposeWith(ControlBindings);
        }

    }
}

