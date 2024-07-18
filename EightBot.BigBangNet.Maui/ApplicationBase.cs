using System;
using Splat;
using Microsoft.Maui;
using System.Reactive.Linq;
using ReactiveUI;
using System.Linq;
using EightBot.BigBang.Interfaces;
using System.Reactive.Subjects;
using System.Reactive;
using System.Threading.Tasks;
using EightBot.BigBang.ViewModel;

namespace EightBot.BigBang.Maui
{
    public abstract class ApplicationBase : Application, IEnableLogger
    {
        private readonly bool _logNavigation;

        readonly Subject<ApplicationLifecycleEvent> _lifecycle = new Subject<ApplicationLifecycleEvent>();
        public IObservable<Unit> IsStarting => _lifecycle.Where(x => x == ApplicationLifecycleEvent.IsStarting).SelectUnit().AsObservable();
        public IObservable<Unit> IsResuming => _lifecycle.Where(x => x == ApplicationLifecycleEvent.IsResuming).SelectUnit().AsObservable();
        public IObservable<Unit> IsSleeping => _lifecycle.Where(x => x == ApplicationLifecycleEvent.IsSleeping).SelectUnit().AsObservable();
        public IObservable<ApplicationLifecycleEvent> Lifecycle => _lifecycle.AsObservable();

        new public static ApplicationBase Current
            => Application.Current as ApplicationBase;

        public ApplicationBase(bool logNavigation = false) : this(null, logNavigation)
        {
        }

        // Handle when your app starts
        public ApplicationBase(Type appType, bool logNavigation = false)
        {
            _logNavigation = logNavigation;
            SetupServices();
            PreCache(appType ?? this.GetType());
        }

        protected override void OnStart()
        {
            _lifecycle.OnNext(ApplicationLifecycleEvent.IsStarting);

            if (_logNavigation)
            {
                AddNavigationListeners();
            }
        }

        protected override void OnResume()
        {
            _lifecycle.OnNext(ApplicationLifecycleEvent.IsResuming);

            base.OnResume();
        }

        protected override void OnSleep()
        {
            _lifecycle.OnNext(ApplicationLifecycleEvent.IsSleeping);

            base.OnSleep();
        }

        protected abstract void SetupServices();

        private void AddNavigationListeners()
        {
            Observable
                .Merge(
                    Observable
                        .FromEvent<EventHandler<Page>, Page>(
                            eventHandler =>
                            {
                                void Handler(object sender, Page e) => eventHandler?.Invoke(e);
                                return Handler;
                            },
                            x => this.PageAppearing += x,
                            x => this.PageAppearing -= x)
                        .ObserveOn(Schedulers.ShortTermThreadPoolScheduler)
                        .Select(x => x.GetType().Name),
                    Observable
                        .FromEvent<EventHandler<ModalPushedEventArgs>, ModalPushedEventArgs>(
                            eventHandler =>
                            {
                                void Handler(object sender, ModalPushedEventArgs e) => eventHandler?.Invoke(e);
                                return Handler;
                            },
                            x => this.ModalPushed += x,
                            x => this.ModalPushed -= x)
                        .ObserveOn(Schedulers.ShortTermThreadPoolScheduler)
                        .Select(x => x.Modal.GetType().Name))
                .ObserveOn(Schedulers.ShortTermThreadPoolScheduler)
                .Do(name =>
                {
                    var analytics =
                        Locator.Current.GetServices<IAnalytics>()
                        ?? Enumerable.Empty<IAnalytics>();

                    foreach (var analyticsProvider in analytics)
                        analyticsProvider.LogViewDisplayed(name);
                })
                .Subscribe();

            Observable
                .Merge(
                    Observable
                        .FromEvent<EventHandler<Page>, Page>(
                            eventHandler =>
                            {
                                void Handler(object sender, Page e) => eventHandler?.Invoke(e);
                                return Handler;
                            },
                            x => this.PageDisappearing += x,
                            x => this.PageDisappearing -= x)
                        .ObserveOn(Schedulers.ShortTermThreadPoolScheduler)
                        .Select(x => x.GetType().Name),
                    Observable
                        .FromEvent<EventHandler<ModalPoppedEventArgs>, ModalPoppedEventArgs>(
                            eventHandler =>
                            {
                                void Handler(object sender, ModalPoppedEventArgs e) => eventHandler?.Invoke(e);
                                return Handler;
                            },
                            x => this.ModalPopped += x,
                            x => this.ModalPopped -= x)
                        .ObserveOn(Schedulers.ShortTermThreadPoolScheduler)
                        .Select(x => x.Modal.GetType().Name))
                .ObserveOn(Schedulers.ShortTermThreadPoolScheduler)
                .Do(name =>
                {
                    var analytics =
                        Locator.Current.GetServices<IAnalytics>()
                        ?? Enumerable.Empty<IAnalytics>();

                    foreach (var analyticsProvider in analytics)
                        analyticsProvider.LogViewHidden(name);
                })
                .Subscribe();
        }

        private Task PreCache(Type appType)
        {
            return Task.Run(
                () =>
                {
                    var sw = System.Diagnostics.Stopwatch.StartNew();
                    var sb = new System.Text.StringBuilder();
                    var totalTime = 0L;

                    sb.AppendLine();
                    sb.AppendLine("------------------------------------------------------------------------------------------");
                    sb.AppendLine(" Precaching - Start");
                    sb.AppendLine("------------------------------------------------------------------------------------------");

                    var viewModelType = typeof(ViewModelBase);
                    var iViewForType = typeof(IViewFor);
                    var precacheAttribute = typeof(PreCacheAttribute);

                    var ass = appType.Assembly;

                    var assTypes =
                        ass
                            .GetTypes()
                            .Where(
                                ti =>
                                    Attribute.IsDefined(ti, precacheAttribute) &&
                                    ti.IsClass && !ti.IsAbstract &&
                                    ti.GetConstructor(Type.EmptyTypes) != null && !ti.ContainsGenericParameters)
                            .ToList();


                    sb.AppendLine($" Assembly Loading\t-\t{sw.ElapsedMilliseconds:N1}ms");

                    sb.AppendLine();
                    sb.AppendLine("-------------------------");
                    sb.AppendLine(" Precaching Views and View Models");
                    sb.AppendLine();

                    foreach (var ti in assTypes)
                    {
                        if (ti.IsSubclassOf(viewModelType) || iViewForType.IsAssignableFrom(ti))
                        {
                            try
                            {
                                sw.Restart();
                                ass.CreateInstance(ti.FullName);
                                var elapsed = sw.ElapsedMilliseconds;
                                totalTime += elapsed;
                                sb.AppendLine($" {ti.Name,-50}\t-\t{elapsed:N1}ms");
                            }
                            catch (Exception ex)
                            {
                                sb.AppendLine($" {ti.Name,-50}\t-\tException: {ex}ms");
                                sb.AppendLine();
                                sb.AppendLine($"{ex}");
                                sb.AppendLine();
                            }
                        }
                    }

                    sw.Stop();
                    sb.AppendLine();
                    sb.AppendLine($" {"Total View and View Model Loading":-50} \t-\t{totalTime:N1}ms");
                    sb.AppendLine("-------------------------");


                    sb.AppendLine();
                    sb.AppendLine("------------------------------------------------------------------------------------------");
                    sb.AppendLine(" Precaching - End");
                    sb.AppendLine("------------------------------------------------------------------------------------------");

                    this.Log().Debug(sb.ToString());
                })
            .ContinueWith(
                result =>
                {
                    if (result.IsFaulted)
                    {
                        this.Log().Error(result.Exception);
                    }
                });
        }
    }
}


