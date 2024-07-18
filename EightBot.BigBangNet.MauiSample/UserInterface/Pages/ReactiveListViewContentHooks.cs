using System;
using EightBot.BigBang.Sample.ViewModels;
using EightBot.BigBang.Maui.Pages;
using EightBot.BigBang.Maui.Views;
using ReactiveUI;
using Microsoft.Maui;
using System.Reactive.Disposables;
using EightBot.BigBang.Maui;

namespace EightBot.BigBang.SampleApp.UserInterface.Pages
{
    public class ReactiveListViewContentHooks : ContentPageBase<ReactiveListViewContentHooksViewModel>
    {
        ReactiveListView _listView;

        public ReactiveListViewContentHooks()
        {
            ViewModel = new ReactiveListViewContentHooksViewModel();
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

