using System;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using ReactiveUI;
using Microsoft.Maui;

namespace EightBot.BigBang.Maui
{
    public static class NavigationObservableExtensions
    {
        private static long _navigatingCount;

        public static TimeSpan DefaultMultiTapThrottleDuration { get; set; }
            = TimeSpan.FromMilliseconds(17 * 12)/*this is roughly 12 UI ticks at 60fps. Slightly more than 200ms*/;

        public static bool Navigating
        {
            get
            {
                return Interlocked.Read(ref _navigatingCount) > 0;
            }
            set
            {
                if (value == true)
                {
                    Interlocked.Increment(ref _navigatingCount);
                }
                else
                {
                    Interlocked.Decrement(ref _navigatingCount);
                }
            }
        }

        public static IDisposable NavigateToPage<TParameter, TPage>(this IObservable<TParameter> observable, VisualElement element,
            Func<TParameter, TPage> pageCreator,
            Action<TPage, TParameter> preNavigation = null,
            Action<TPage, TParameter> postNavigation = null,
            bool animated = true,
            bool allowMultiple = false,
            IScheduler pageCreationScheduler = null,
            TimeSpan? multiTapThrottleDuration = null)
            where TPage : Page
        {
            return observable
                .ThrottleFirst(multiTapThrottleDuration ?? DefaultMultiTapThrottleDuration, Schedulers.ShortTermThreadPoolScheduler)
                .Where(_ => allowMultiple || !Navigating)
                .Do(_ => Navigating = true)
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
                            if (x.Page != default(TPage))
                            {
                                preNavigation?.Invoke(x.Page, x.Parameter);
                                await Task.WhenAll(
                                    x.AppearingTask,
                                    element.Navigation.PushAsync(x.Page, animated));
                                postNavigation?.Invoke(x.Page, x.Parameter);
                            }
                        }
                        finally
                        {
                            Navigating = false;
                        }

                        return Unit.Default;
                    })
                .Subscribe();
        }

        public static IDisposable NavigatePopTo<TParameter, TPage>(this IObservable<TParameter> observable, Page page,
            Action<TParameter> preNavigation = null,
            Action<TParameter> postNavigation = null,
            bool animated = true,
            bool allowMultiple = false,
            TimeSpan? multiTapThrottleDuration = null)
            where TPage : Page
        {
            return observable
                .ThrottleFirst(multiTapThrottleDuration ?? DefaultMultiTapThrottleDuration, Schedulers.ShortTermThreadPoolScheduler)
                .Where(_ => allowMultiple || !Navigating)
                .Do(_ => Navigating = true)
                .ObserveOn(RxApp.MainThreadScheduler)
                .SelectMany(
                    async parameter =>
                    {
                        try
                        {
                            preNavigation?.Invoke(parameter);
                            await page.PopTo<TPage>(animated);
                            postNavigation?.Invoke(parameter);
                        }
                        finally
                        {
                            Navigating = false;
                        }

                        return Unit.Default;
                    })
                .Subscribe();
        }

        public static IDisposable NavigatePopPage<TParameter>(this IObservable<TParameter> observable, VisualElement element,
            Action<TParameter> preNavigation = null,
            Action<TParameter> postNavigation = null,
            bool animated = true,
            bool allowMultiple = false,
            TimeSpan? multiTapThrottleDuration = null)
        {
            return observable
                .ThrottleFirst(multiTapThrottleDuration ?? DefaultMultiTapThrottleDuration, Schedulers.ShortTermThreadPoolScheduler)
                .Where(_ => allowMultiple || !Navigating)
                .Do(_ => Navigating = true)
                .ObserveOn(RxApp.MainThreadScheduler)
                .SelectMany(
                    async parameter =>
                    {
                        try
                        {
                            preNavigation?.Invoke(parameter);
                            await element.Navigation.PopAsync(animated);
                            postNavigation?.Invoke(parameter);
                        }
                        finally
                        {
                            Navigating = false;
                        }

                        return Unit.Default;
                    })
                .Subscribe();
        }

        public static IDisposable NavigatePopToRoot<TParameter>(this IObservable<TParameter> observable, VisualElement element,
            Action<TParameter> preNavigation = null,
            Action<TParameter> postNavigation = null,
            bool animated = true,
            bool allowMultiple = false,
            TimeSpan? multiTapThrottleDuration = null)
        {
            return observable
                .ThrottleFirst(multiTapThrottleDuration ?? DefaultMultiTapThrottleDuration, Schedulers.ShortTermThreadPoolScheduler)
                .Where(_ => allowMultiple || !Navigating)
                .Do(_ => Navigating = true)
                .ObserveOn(RxApp.MainThreadScheduler)
                .SelectMany(
                    async parameter =>
                    {
                        try
                        {
                            preNavigation?.Invoke(parameter);
                            await element.Navigation.PopToRootAsync(animated);
                            postNavigation?.Invoke(parameter);
                        }
                        finally
                        {
                            Navigating = false;
                        }

                        return Unit.Default;
                    })
                .Subscribe();
        }

        public static IDisposable NavigateToModalPage<TParameter, TPage>(this IObservable<TParameter> observable, VisualElement element,
            Func<TParameter, TPage> pageCreator,
            Action<TPage, TParameter> preNavigation = null,
            Action<TPage, TParameter> postNavigation = null,
            bool animated = true,
            bool allowMultiple = false,
            IScheduler pageCreationScheduler = null,
            TimeSpan? multiTapThrottleDuration = null)
            where TPage : Page
        {
            return observable
                .ThrottleFirst(multiTapThrottleDuration ?? DefaultMultiTapThrottleDuration, Schedulers.ShortTermThreadPoolScheduler)
                .Where(_ => allowMultiple || !Navigating)
                .Do(_ => Navigating = true)
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
                            if (x.Page != default(TPage))
                            {
                                preNavigation?.Invoke(x.Page, x.Parameter);
                                await Task.WhenAll(
                                    x.AppearingTask,
                                    element.Navigation.PushModalAsync(x.Page, animated));
                                postNavigation?.Invoke(x.Page, x.Parameter);
                            }
                        }
                        finally
                        {
                            Navigating = false;
                        }

                        return Unit.Default;
                    })
                .Subscribe();
        }

        public static IDisposable NavigatePopModalPage<TParameter>(this IObservable<TParameter> observable, VisualElement element,
            Action<TParameter> preNavigation = null,
            Action<TParameter> postNavigation = null,
            bool animated = true,
            bool allowMultiple = false,
            TimeSpan? multiTapThrottleDuration = null)
        {
            return observable
                .ThrottleFirst(multiTapThrottleDuration ?? DefaultMultiTapThrottleDuration, Schedulers.ShortTermThreadPoolScheduler)
                .Where(_ => allowMultiple || !Navigating)
                .Do(_ => Navigating = true)
                .ObserveOn(RxApp.MainThreadScheduler)
                .SelectMany(
                    async parameter =>
                    {
                        try
                        {
                            preNavigation?.Invoke(parameter);
                            await element.Navigation.PopModalAsync(animated);
                            postNavigation?.Invoke(parameter);
                        }
                        finally
                        {
                            Navigating = false;
                        }

                        return Unit.Default;
                    })
                .Subscribe();
        }
    }
}
