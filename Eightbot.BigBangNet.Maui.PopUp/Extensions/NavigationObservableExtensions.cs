using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using ReactiveUI;
using Rg.Plugins.Popup.Extensions;
using Rg.Plugins.Popup.Pages;
using Xamarin.Forms;
using System.Reactive.Concurrency;
using System.Threading;
using System.Reactive;

namespace EightBot.BigBang.XamForms
{
	public static class PopupNavigationObservableExtensions
	{

		public static IDisposable NavigateToPopupPage<TParameter, TPage>(this IObservable<TParameter> observable,
			Func<TParameter, TPage> pageCreator,
			Action<TPage, TParameter> preNavigation = null,
			Action<TPage, TParameter> postNavigation = null,
			bool animated = true,
			bool allowMultiple = false,
            IScheduler pageCreationScheduler = null,
            TimeSpan? multiTapThrottleDuration = null)
			where TPage : PopupPage
		{
            return observable
                .ThrottleFirst(multiTapThrottleDuration ?? XamForms.NavigationObservableExtensions.DefaultMultiTapThrottleDuration, Schedulers.ShortTermThreadPoolScheduler)
                .Where(_ => allowMultiple || !XamForms.NavigationObservableExtensions.Navigating)
                .Do(_ => XamForms.NavigationObservableExtensions.Navigating = true)
                .ObserveOn(pageCreationScheduler ?? Schedulers.ShortTermThreadPoolScheduler)
                .Select(
                    x =>
                    {
                        try
                        {
                            var page = pageCreator.Invoke(x);
                            return (Page: page, Parameter: x, AppearingTask: page.AppearingAsync());
                        }
                        catch
                        {
                            return (Page: default(TPage), Parameter: x, AppearingTask: Task.CompletedTask);
                        }
                    })
                .ObserveOn(RxApp.MainThreadScheduler)
                .SelectMany(
                    async x =>
                    {
                        try
                        {
                            preNavigation?.Invoke(x.Page, x.Parameter);

                            var nav = Rg.Plugins.Popup.Services.PopupNavigation.Instance;
                            await Task.WhenAll(
                                x.AppearingTask, 
                                nav.PushAsync(x.Page, animated));

                            postNavigation?.Invoke(x.Page, x.Parameter);
                        }
                        finally
                        {
                            XamForms.NavigationObservableExtensions.Navigating = false;
                        }

                        return Unit.Default;
                    })
                .Subscribe();        
		}

		public static IDisposable NavigatePopPopupPage<TParameter>(this IObservable<TParameter> observable,
			Action<TParameter> preNavigation = null,
			Action<TParameter> postNavigation = null,
			bool animated = true,
            bool allowMultiple = false,
            TimeSpan? multiTapThrottleDuration = null)
		{
            return observable
                .ThrottleFirst(multiTapThrottleDuration ?? XamForms.NavigationObservableExtensions.DefaultMultiTapThrottleDuration, Schedulers.ShortTermThreadPoolScheduler)
                .Where(_ => allowMultiple || !XamForms.NavigationObservableExtensions.Navigating)
                .Do(_ => XamForms.NavigationObservableExtensions.Navigating = true)
                .ObserveOn(RxApp.MainThreadScheduler)
                .SelectMany(
                    async parameter =>
                    {
                        try
                        {
                            preNavigation?.Invoke(parameter);
                            var nav = Rg.Plugins.Popup.Services.PopupNavigation.Instance;
                            await nav.PopAsync(animated);
                            postNavigation?.Invoke(parameter);

                        }
                        finally
                        {
                            XamForms.NavigationObservableExtensions.Navigating = false;
                        }

                        return Unit.Default;
                    })
                .Subscribe();
		}

        public static IDisposable NavigatePopAllPopupPage<TParameter>(this IObservable<TParameter> observable,
            Action<TParameter> preNavigation = null,
            Action<TParameter> postNavigation = null,
            bool animated = true,
            bool allowMultiple = false,
            TimeSpan? multiTapThrottleDuration = null)
        {
            return observable
                .ThrottleFirst(multiTapThrottleDuration ?? XamForms.NavigationObservableExtensions.DefaultMultiTapThrottleDuration, Schedulers.ShortTermThreadPoolScheduler)
                .Where(_ => allowMultiple || !XamForms.NavigationObservableExtensions.Navigating)
                .Do(_ => XamForms.NavigationObservableExtensions.Navigating = true)
                .ObserveOn(RxApp.MainThreadScheduler)
                .SelectMany(
                    async parameter =>
                    {
                        try
                        {
                            preNavigation?.Invoke(parameter);
                            var nav = Rg.Plugins.Popup.Services.PopupNavigation.Instance;
                            await nav.PopAllAsync(animated);
                            postNavigation?.Invoke(parameter);
                        }
                        finally
                        {
                            XamForms.NavigationObservableExtensions.Navigating = false;
                        }

                        return Unit.Default;
                    })
                .Subscribe();
        }

        public static IDisposable NavigateRemovePopupPage<TParameter, TPage>(this IObservable<TParameter> observable,
            TPage page,
            Action<TPage, TParameter> preNavigation = null,
            Action<TPage, TParameter> postNavigation = null,
            bool animated = true,
            bool allowMultiple = false,
            TimeSpan? multiTapThrottleDuration = null)
            where TPage : PopupPage
        {
            return observable
                .ThrottleFirst(multiTapThrottleDuration ?? XamForms.NavigationObservableExtensions.DefaultMultiTapThrottleDuration, Schedulers.ShortTermThreadPoolScheduler)
                .Where(_ => allowMultiple || !XamForms.NavigationObservableExtensions.Navigating)
                .Do(_ => XamForms.NavigationObservableExtensions.Navigating = true)
                .ObserveOn(RxApp.MainThreadScheduler)
                .SelectMany(
                    async parameter =>
                    {
                        try
                        {
                            preNavigation?.Invoke(page, parameter);
                            var nav = Rg.Plugins.Popup.Services.PopupNavigation.Instance;
                            await nav.RemovePageAsync(page, animated);
                            postNavigation?.Invoke(page, parameter);
                        }
                        finally
                        {
                            XamForms.NavigationObservableExtensions.Navigating = false;
                        }

                        return Unit.Default;
                    })
                .Subscribe();
        }
	}
}
