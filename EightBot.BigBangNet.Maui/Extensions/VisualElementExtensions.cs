using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using ReactiveUI;
using Microsoft.Maui;
using Splat;
using System.Collections.Generic;
using System.Reactive.Concurrency;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Linq;
using Microsoft.Maui.Controls.Compatibility;

namespace EightBot.BigBang.Maui
{
    public static class VisualElementExtensions
    {
        public static IDisposable ProfileLayout(this VisualElement visualElement)
        {
            var observables = new List<IObservable<(long Count, string Message)>>();

            ProfileLayout(ref observables, visualElement);

            var name = visualElement.GetType().Name;
            var totalLogs = 0L;

            return
                Observable
                    .Merge(observables)
                    .ObserveOn(Schedulers.ShortTermThreadPoolScheduler)
                    .Buffer(GlobalConfiguration.LogBufferDuration, Schedulers.ShortTermThreadPoolScheduler)
                    .Where(x => x?.Any() ?? false)
                    .Do(
                        logs =>
                        {
                            var logMessage = new StringBuilder();
                            foreach (var log in logs)
                            {
                                Interlocked.Increment(ref totalLogs);
                                logMessage.AppendLine(log.Message);
                            }

                            if (logMessage.Length > 0)
                            {
                                logMessage.AppendLine($"[Layout] Total Count: {Interlocked.Read(ref totalLogs)}");
                                LogHost.Default.Debug(logMessage.ToString(), name);
                            }
                        })
                    .Subscribe();
        }

        private static void ProfileLayout(ref List<IObservable<(long Count, string Message)>> observables, VisualElement visualElement, string parent = "")
        {
            var name = visualElement.GetType().Name;


            if (parent == "")
            {
                parent = name;
            }
            else
            {
                parent = $"{parent}→{name}";
            }

            if (visualElement is ContentPage cp && cp.Content != null)
            {
                ProfileLayout(ref observables, cp.Content, parent);
            }

            if (visualElement is ContentView cv && cv.Content != null)
            {
                ProfileLayout(ref observables, cv.Content, parent);
            }

            if (visualElement is Layout<View> layoutWithView)
            {
                foreach (var child in layoutWithView.Children)
                {
                    ProfileLayout(ref observables, child, parent);
                }
            }

            if (visualElement is ScrollView scrollView && scrollView.Content != null)
            {
                ProfileLayout(ref observables, scrollView.Content, parent);
            }

            var veObservable =
                Observable
                    .FromEvent<EventHandler, EventArgs>(
                        eventHandler =>
                        {
                            void Handler(object sender, EventArgs e) => eventHandler?.Invoke(e);
                            return Handler;
                        },
                        x => visualElement.MeasureInvalidated += x,
                        x => visualElement.MeasureInvalidated -= x)
                    .ObserveOn(Schedulers.ShortTermThreadPoolScheduler)
                    .Select(x => 1L)
                    .Scan((x, y) => x + y)
                    .Where(x => x > 1)
                    .Select(x => (x - 1, $"[Layout] Count: {x - 1,-20} Hierarchy:{parent}"));

            observables.Add(veObservable);
        }

        public static IObservable<T> DisableControlWhileThrottled<T>(this IObservable<T> observable, VisualElement visualElement, TimeSpan delay, IScheduler scheduler = null)
        {
            return observable
                .ThrottleFirst(
                    _ => visualElement.IsEnabled = false,
                    _ => visualElement.IsEnabled = true,
                    delay,
                    RxApp.MainThreadScheduler,
                    scheduler);
        }

        public static IObservable<T> DisableInteractionWhileThrottled<T>(this IObservable<T> observable, VisualElement visualElement, TimeSpan delay, IScheduler scheduler = null)
        {
            return observable
                .ThrottleFirst(
                    _ => visualElement.InputTransparent = true,
                    _ => visualElement.InputTransparent = false,
                    delay,
                    RxApp.MainThreadScheduler,
                    scheduler);
        }
    }
}
