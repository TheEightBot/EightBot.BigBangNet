using System;
using System.Reactive.Linq;
using EightBot.BigBang.Sample.ViewModels;
using EightBot.BigBang.XamForms.Effects;
using EightBot.BigBang.XamForms.Pages;
using ReactiveUI;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Reactive.Disposables;
using EightBot.BigBang.XamForms;

namespace EightBot.BigBang.SampleApp.UserInterface.Pages
{
	public partial class SampleAppPage : ContentPageBase<SampleViewModel>
	{
		public SampleAppPage()
		{
            this.ViewModel = new SampleViewModel();
		}

		protected override void SetupUserInterface()
		{
        	InitializeComponent();
		}

        protected override void BindControls()
        {
            this
                .OneWayBind(
                    ViewModel,
                    x => x.DecimalProperty, c => c.NumericToCurrencyTest.Text,
                    vmToViewConverterOverride: new TypeConverters.NumericToCurrencyStringConverter())
                .DisposeWith(ControlBindings);

            CompositeDisposable _theThrottler = new CompositeDisposable();

            //ThrottleFirst
            //    .Tapped(TimeSpan.FromSeconds(2))
            //    .Do(_ => System.Diagnostics.Debug.WriteLine($"Tapped"))
            //    .Subscribe()
            //    .DisposeWith(_theThrottler);


            //Observable
            //    .FromEvent<EventHandler, EventArgs>(
            //        eventHandler =>
            //        {
            //            void Handler(object sender, EventArgs e) => eventHandler?.Invoke(e);
            //            return Handler;
            //        },
            //        x => ThrottleFirst.Clicked += x,
            //        x => ThrottleFirst.Clicked -= x)
            //    .Publish(o =>
            //    {
            //        return o
            //            .Take(1)
            //            .Concat(
            //                o.IgnoreElements()
            //                    .TakeUntil(
            //                        Observable
            //                            .Return(default(T), )

            //                            .Delay(delay, scheduler)))
            //            .Repeat()
            //            .TakeUntil(o.IgnoreElements().Concat(Observable.Return(default(T), scheduler)));
            //    });

            //    .ThrottleFirst(TimeSpan.FromSeconds(2))
            //    .Do(_ => System.Diagnostics.Debug.WriteLine($"Message Received at: {DateTimeOffset.Now}"))
            //    .Subscribe()
            //    .DisposeWith(_theThrottler);

            //Observable
            //    .FromEvent<EventHandler, EventArgs>(
            //        eventHandler =>
            //        {
            //            void Handler(object sender, EventArgs e) => eventHandler?.Invoke(e);
            //            return Handler;
            //        },
            //        x => ThrottleKill.Clicked += x,
            //        x => ThrottleKill.Clicked -= x)
            //    .Do(_ => _theThrottler.Clear())
            //    .Subscribe()
            //    .DisposeWith(ControlBindings);

            Observable
                .FromEvent<EventHandler<TextChangedEventArgs>, TextChangedEventArgs>(
                    eventHandler =>
                    {
                        void Handler(object sender, TextChangedEventArgs e) => eventHandler?.Invoke(e);
                        return Handler;
                    },
                    x => NumericToCurrencyTest.TextChanged += x,
                    x => NumericToCurrencyTest.TextChanged -= x)
                .Select(x => x.NewTextValue)
                .Select(x => {
                    var toCurrency = new TypeConverters.ToCurrencyConverter();
                    object result;
                    toCurrency.TryConvert(x, typeof(decimal), null, out result);
                    return (decimal)result;
                })
                .Subscribe(x => {
                    ViewModel.DecimalProperty = x;
                })
                .DisposeWith(ControlBindings);
        }

		async void Handle_Clicked(object sender, System.EventArgs e)
		{
            Page page = null;

            if (sender == TabbarInactiveColorEffect)
                page = new TabBarInactiveColorEffectPage();
            else if (sender == ModalPage)
            {
                page = new AddressEdit(new AddressViewModel());
                await this.Navigation.PushModalAsync(new NavigationPage(page));

                return;
            }
            else if (sender == ListViewContentHooks)
                page = new ReactiveListViewContentHooks();
            else if (sender == ListViewBindWithPullToRefresh)
                page = new ReactiveListBindWithPullToRefresh();
            else if (sender == ListViewWithPinnedCellViewModel)
                page = new ReactiveListViewPinnedViewModelPage();
            else if (sender == ListViewWithUpdates)
                page = new ReactiveListViewUpdateNotifications();
            else if (sender == FileSelectionTest)
                page = new SelectFilePage();
            else if (sender == FileSelectionAndSaveTest)
                page = new SelectAndSaveFilePage();
            else if (sender == NumericConverters)
                page = new NumericConverterPage();
            else if (sender == ScrollPosition)
                page = new ScrollPositionPage();
            else if (sender == BackgroundProcessing)
                page = new BackgroundProcessorPage();
            else if (sender == PickersTest)
                page = new PickersTest();
            else if (sender == NavigationTest)
                page = new NavigationTest();

            if (page != null)
                await this.Navigation.PushAsync(page);
		}
	}
}
