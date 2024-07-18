using System;
using System.Reactive.Linq;
using EightBot.BigBang.Sample.ViewModels;
using EightBot.BigBang.XamForms.Effects;
using EightBot.BigBang.XamForms.Pages;
using EightBot.BigBang.XamForms.Views;
using ReactiveUI;
using Xamarin.Forms;
using System.Reactive.Disposables;
using EightBot.BigBang.XamForms;

namespace EightBot.BigBang.SampleApp.UserInterface.Pages
{
    public class ScrollPositionPage : ContentPageBase<ReactiveListBindWithPullToRefreshViewModel>
    {
        ReactiveListView _listView;
        Grid _mainLayout;
        Label _position;
        ScrollPositionEffect _scrollEffect;

        public ScrollPositionPage()
        {
            ViewModel = new ReactiveListBindWithPullToRefreshViewModel();
        }

        protected override void SetupUserInterface()
        {
            _mainLayout = new Grid()
            {
                ColumnDefinitions = new ColumnDefinitionCollection()
                {
                    new ColumnDefinition() { Width = GridLength.Star }
                },
                RowDefinitions = new RowDefinitionCollection()
                {
                    new RowDefinition() { Height = 35 },
                    new RowDefinition() { Height = GridLength.Star },
                }
            };

            _position = new Label
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center,
                Text = "Scroll Position : (0)"
            };

            _listView = new ReactiveListView(typeof(Cells.SampleViewModelCell));

            _scrollEffect = new ScrollPositionEffect();
            _listView.Effects.Add(_scrollEffect);

            _mainLayout.Children.Add(_position, 0, 0);
            _mainLayout.Children.Add(_listView, 0, 1);

            this.Content = _mainLayout;
        }

        protected override void BindControls()
        {

            Observable
                .FromEvent<ScrollActionEventHandler, ScrolledPositionEventArgs>(
                    eventHandler =>
                    {
                        void Handler(object sender, ScrolledPositionEventArgs e) => eventHandler?.Invoke(e);
                        return Handler;
                    },
                    x => _scrollEffect.ScrollAction += x,
                    x => _scrollEffect.ScrollAction -= x)
                .Select(args =>
                {
                    System.Diagnostics.Debug.WriteLine($"y: {args.Location.Y}");
                    return $"Scroll Position : ({args.Location.Y})";
                })
                .ObserveOn(RxApp.MainThreadScheduler)
                .BindTo(_position, c => c.Text)
                .DisposeWith(ControlBindings);

            _listView
                .Bind(this.WhenAnyValue(x => x.ViewModel.SampleList),
                    this.WhenAnyObservable(x => x.ViewModel.RefreshCommand.IsExecuting),
                    this.WhenAnyValue(x => x.ViewModel.RefreshCommand))
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

