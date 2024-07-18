using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using ReactiveUI;
using System.Runtime.InteropServices.ComTypes;

namespace EightBot.BigBang
{
    public static class IObservableExtensions
    {
        public static IObservable<(TSource Previous, TSource Current)> PairWithPrevious<TSource>(this IObservable<TSource> source)
        {
            return source.Scan(
                (default(TSource), default(TSource)),
                (acc, current) => (acc.Item2, current));
        }

        public static IObservable<TSource> CatchAndReturnDefault<TSource, TException>(this IObservable<TSource> source)
            where TException : Exception
        {
            return source.Catch<TSource, TException>(ex => Observable.Return(default(TSource)));
        }

        public static IObservable<Unit> SelectUnit<TSource>(this IObservable<TSource> source)
        {
            return source
                .Select(_ => Unit.Default);
        }

        public static IObservable<object> AsObject<TSource>(this IObservable<TSource> source)
        {
            return source
                .Select(x => x as object);
        }

        public static IObservable<Unit> SelectConcurrent<T>(this IObservable<T> source, Action<T> onNext, int concurrentSubscriptions = 1, IScheduler scheduler = null)
        {
            if (scheduler != null)
            {
                return source
                    .Select(x =>
                        Observable
                            .Defer(() =>
                                Observable
                                    .Start(() => onNext(x))
                                    .SubscribeOn(scheduler)))
                    .Merge(concurrentSubscriptions);
            }

            return source
                .Select(x =>
                    Observable.Defer(() =>
                        Observable.Start(() => onNext(x))))
                .Merge(concurrentSubscriptions);
        }

        public static IObservable<Unit> SelectConcurrent<T>(this IObservable<T> source, Action<T, CancellationToken> onNext, CancellationToken cancellationToken, int concurrentSubscriptions = 1, IScheduler scheduler = null)
        {
            if (scheduler != null)
            {
                return source
                    .Select(x =>
                        Observable
                            .Defer(() =>
                                Observable
                                    .Start(() => onNext(x, cancellationToken))
                                    .SubscribeOn(scheduler)))
                    .Merge(concurrentSubscriptions);
            }

            return source
                .Select(x =>
                    Observable.Defer(() =>
                        Observable.Start(() => onNext(x, cancellationToken))))
                .Merge(concurrentSubscriptions);
        }

        public static IObservable<TOut> SelectConcurrent<TIn, TOut>(this IObservable<TIn> source, Func<TIn, TOut> onNext, int concurrentSubscriptions = 1, IScheduler scheduler = null)
        {
            if (scheduler != null)
            {
                return source
                    .Select(xIn =>
                        Observable
                            .Defer(() =>
                                Observable
                                    .Start(() => onNext(xIn))
                                    .SubscribeOn(scheduler)))
                    .Merge(concurrentSubscriptions);
            }

            return source
                .Select(xIn =>
                    Observable.Defer(() =>
                        Observable.Start(() => onNext(xIn))))
                .Merge(concurrentSubscriptions);
        }

        public static IObservable<TOut> SelectConcurrent<TIn, TOut>(this IObservable<TIn> source, Func<TIn, CancellationToken, TOut> onNext, CancellationToken cancellationToken, int concurrentSubscriptions = 1, IScheduler scheduler = null)
        {
            if (scheduler != null)
            {
                return source
                    .Select(xIn =>
                        Observable
                            .Defer(() =>
                                Observable
                                    .Start(() => onNext(xIn, cancellationToken))
                                    .SubscribeOn(scheduler)))
                    .Merge(concurrentSubscriptions);
            }

            return source
                .Select(xIn =>
                    Observable.Defer(() =>
                        Observable.Start(() => onNext(xIn, cancellationToken))))
                .Merge(concurrentSubscriptions);
        }

        public static IObservable<Unit> SelectSequential<T>(this IObservable<T> source, Action<T, CancellationToken> onNext, CancellationToken cancellationToken, IScheduler scheduler = null)
        {
            if (scheduler != null)
            {
                return source
                    .Select(x =>
                        Observable
                            .Defer(() =>
                                Observable
                                    .Start(() => onNext(x, cancellationToken))
                                    .SubscribeOn(scheduler)))
                    .Concat();
            }

            return source
                .Select(x =>
                    Observable.Defer(() =>
                        Observable.Start(() => onNext(x, cancellationToken))))
                .Concat();
        }

        public static IObservable<TOut> SelectSequential<TIn, TOut>(this IObservable<TIn> source, Func<TIn, TOut> onNext, IScheduler scheduler = null)
        {
            if (scheduler != null)
            {
                return source
                    .Select(xIn =>
                        Observable
                            .Defer(() =>
                                Observable
                                    .Start(() => onNext(xIn))
                                    .SubscribeOn(scheduler)))
                    .Concat();
            }

            return source
                .Select(xIn =>
                    Observable.Defer(() =>
                        Observable.Start(() => onNext(xIn))))
                .Concat();
        }

        public static IObservable<TOut> SelectSequential<TIn, TOut>(this IObservable<TIn> source, Func<TIn, CancellationToken, TOut> onNext, CancellationToken cancellationToken, IScheduler scheduler = null)
        {
            if (scheduler != null)
            {
                return source
                    .Select(xIn =>
                        Observable
                            .Defer(() =>
                                Observable
                                    .Start(() => onNext(xIn, cancellationToken))
                                    .SubscribeOn(scheduler)))
                    .Concat();
            }

            return source
                .Select(xIn =>
                    Observable.Defer(() =>
                        Observable.Start(() => onNext(xIn, cancellationToken))))
                .Concat();
        }

        public static IObservable<Unit> SelectManyConcurrent<T>(this IObservable<T> source, Func<T, Task> onNext, int concurrentSubscriptions = 1, IScheduler scheduler = null)
        {
            if (scheduler != null)
            {
                return source
                    .Select(x =>
                        Observable
                            .FromAsync(() => onNext(x))
                            .SubscribeOn(scheduler))
                    .Merge(concurrentSubscriptions);
            };

            return source
                .Select(x => Observable.FromAsync(() => onNext(x)))
                .Merge(concurrentSubscriptions);
        }

        public static IObservable<Unit> SelectManyConcurrent<T>(this IObservable<T> source, Func<Task> onNext, int concurrentSubscriptions = 1, IScheduler scheduler = null)
        {
            if (scheduler != null)
            {
                return source
                    .Select(_ =>
                        Observable
                            .FromAsync(() => onNext())
                            .SubscribeOn(scheduler))
                    .Merge(concurrentSubscriptions);
            };

            return source
                .Select(_ => Observable.FromAsync(() => onNext()))
                .Merge(concurrentSubscriptions);
        }

        public static IObservable<Unit> SelectManyConcurrent<T>(this IObservable<T> source, Func<T, CancellationToken, Task> onNext, int concurrentSubscriptions = 1, IScheduler scheduler = null)
        {
            if (scheduler != null)
            {
                return source
                    .Select(x =>
                        Observable
                            .FromAsync((CancellationToken cancellationToken) => onNext(x, cancellationToken))
                            .SubscribeOn(scheduler))
                    .Merge(concurrentSubscriptions);
            };

            return source
                .Select(x => Observable.FromAsync((CancellationToken cancellationToken) => onNext(x, cancellationToken)))
                .Merge(concurrentSubscriptions);
        }

        public static IObservable<TOut> SelectManyConcurrent<TIn, TOut>(this IObservable<TIn> source, Func<TIn, Task<TOut>> onNext, int concurrentSubscriptions = 1, IScheduler scheduler = null)
        {
            if (scheduler != null)
            {
                return source
                    .Select(x =>
                        Observable
                            .FromAsync(() => onNext(x))
                            .SubscribeOn(scheduler))
                    .Merge(concurrentSubscriptions);
            };

            return source
                .Select(x => Observable.FromAsync(() => onNext(x)))
                .Merge(concurrentSubscriptions);
        }

        public static IObservable<TOut> SelectManyConcurrent<TIn, TOut>(this IObservable<TIn> source, Func<TIn, CancellationToken, Task<TOut>> onNext, int concurrentSubscriptions = 1, IScheduler scheduler = null)
        {
            if (scheduler != null)
            {
                return source
                    .Select(x =>
                        Observable
                            .FromAsync((CancellationToken cancellationToken) => onNext(x, cancellationToken))
                            .SubscribeOn(scheduler))
                    .Merge(concurrentSubscriptions);
            };

            return source
                .Select(x => Observable.FromAsync((CancellationToken cancellationToken) => onNext(x, cancellationToken)))
                .Merge(concurrentSubscriptions);
        }

        public static IObservable<Unit> SelectManySequential<T>(this IObservable<T> source, Func<T, Task> onNext, IScheduler scheduler = null)
        {
            if (scheduler != null)
            {
                return source
                    .Select(x =>
                        Observable
                            .FromAsync(() => onNext(x))
                            .SubscribeOn(scheduler))
                    .Concat();
            };

            return source
                .Select(x => Observable.FromAsync(() => onNext(x)))
                .Concat();
        }

        public static IObservable<Unit> SelectManySequential<T>(this IObservable<T> source, Func<Task> onNext, IScheduler scheduler = null)
        {
            if (scheduler != null)
            {
                return source
                    .Select(_ =>
                        Observable
                            .FromAsync(() => onNext())
                            .SubscribeOn(scheduler))
                    .Concat();
            };

            return source
                .Select(_ => Observable.FromAsync(() => onNext()))
                .Concat();
        }

        public static IObservable<Unit> SelectManySequential<T>(this IObservable<T> source, Func<T, CancellationToken, Task> onNext, IScheduler scheduler = null)
        {
            if (scheduler != null)
            {
                return source
                    .Select(x =>
                        Observable
                            .FromAsync((CancellationToken cancellationToken) => onNext(x, cancellationToken))
                            .SubscribeOn(scheduler))
                    .Concat();
            };

            return source
                .Select(x => Observable.FromAsync((CancellationToken cancellationToken) => onNext(x, cancellationToken)))
                .Concat();
        }

        public static IObservable<TOut> SelectManySequential<TIn, TOut>(this IObservable<TIn> source, Func<TIn, Task<TOut>> onNext, IScheduler scheduler = null)
        {
            if (scheduler != null)
            {
                return source
                    .Select(x =>
                        Observable
                            .FromAsync(() => onNext(x))
                            .SubscribeOn(scheduler))
                    .Concat();
            };

            return source
                .Select(x => Observable.FromAsync(() => onNext(x)))
                .Concat();
        }

        public static IObservable<TOut> SelectManySequential<TIn, TOut>(this IObservable<TIn> source, Func<TIn, CancellationToken, Task<TOut>> onNext, IScheduler scheduler = null)
        {
            if (scheduler != null)
            {
                return source
                    .Select(x =>
                        Observable
                            .FromAsync((CancellationToken cancellationToken) => onNext(x, cancellationToken))
                            .SubscribeOn(scheduler))
                    .Concat();
            };

            return source
                .Select(x => Observable.FromAsync((CancellationToken cancellationToken) => onNext(x, cancellationToken)))
                .Concat();
        }

        public static IObservable<Unit> AsCompletion<T>(this IObservable<T> observable)
        {
            return Observable.Create<Unit>(observer =>
                {
                    Action onCompleted = () =>
                    {
                        observer.OnNext(Unit.Default);
                        observer.OnCompleted();
                    };
                    return observable.Subscribe(_ => { }, observer.OnError, onCompleted);
                });
        }

        public static IObservable<TRet> ContinueAfter<T, TRet>(
            this IObservable<T> observable, Func<IObservable<TRet>> selector)
        {
            return observable.AsCompletion().SelectMany(_ => selector());
        }

        /// <summary>
        /// Observes changes on a specific scheduler and ignores messages that come in while the the scheduler is executing
        /// </summary>
        /// <returns>The latest on.</returns>
        /// <param name="source">Source Observable</param>
        /// <param name="scheduler">Scheduler.</param>
        /// <typeparam name="T"></typeparam>
        public static IObservable<T> ObserveLatestOn<T>(this IObservable<T> source, IScheduler scheduler)
        {
            return Observable.Create<T>(observer =>
            {
                var gate = new object();
                bool active = false;
                var cancelable = new SerialDisposable();
                var disposable = source.Materialize().Subscribe(thisNotification =>
                {
                    bool wasNotAlreadyActive;
                    Notification<T> outsideNotification;
                    lock (gate)
                    {
                        wasNotAlreadyActive = !active;
                        active = true;
                        outsideNotification = thisNotification;
                    }

                    if (wasNotAlreadyActive)
                    {
                        cancelable.Disposable = scheduler.Schedule(self =>
                        {
                            Notification<T> localNotification;
                            lock (gate)
                            {
                                localNotification = outsideNotification;
                                outsideNotification = null;
                            }
                            localNotification.Accept(observer);
                            bool hasPendingNotification;
                            lock (gate)
                            {
                                hasPendingNotification = active = (outsideNotification != null);
                            }
                            if (hasPendingNotification)
                            {
                                self();
                            }
                        });
                    }
                });
                return new CompositeDisposable(disposable, cancelable);
            });
        }

        public static IObservable<TSource> IsNotNull<TSource>(this IObservable<TSource> source)
        {
            return source.Where(obj => !EqualityComparer<TSource>.Default.Equals(obj, default(TSource)));
        }

        public static IObservable<TSource> IsNull<TSource>(this IObservable<TSource> source)
            where TSource : class
        {
            return source.Where(obj => obj == null);
        }

        public static IObservable<TSource> IsDefault<TSource>(this IObservable<TSource> source)
        {
            return source.Where(obj => EqualityComparer<TSource>.Default.Equals(obj, default(TSource)));
        }

        public static IObservable<TSource> IsNotDefault<TSource>(this IObservable<TSource> source)
        {
            return source.Where(obj => !EqualityComparer<TSource>.Default.Equals(obj, default(TSource)));
        }

        public static IObservable<T?> WhereHasValue<T>(this IObservable<T?> source)
            where T : struct
        {
            return source.Where(x => x.HasValue);
        }

        public static IObservable<T?> WhereHasValueAndIsNot<T>(this IObservable<T?> source, T comparison)
            where T : struct
        {
            return source.Where(x => x.HasValue && !EqualityComparer<T>.Default.Equals(x.Value, comparison));
        }

        public static IObservable<T?> WhereHasValueAndIsNotDefault<T>(this IObservable<T?> source)
            where T : struct
        {
            return source.Where(x => x.HasValue && !EqualityComparer<T>.Default.Equals(x.Value, default(T)));
        }

        public static IObservable<T?> WhereHasValueAndIs<T>(this IObservable<T?> source, T comparison)
            where T : struct
        {
            return source.Where(x => x.HasValue && EqualityComparer<T>.Default.Equals(x.Value, comparison));
        }

        public static IObservable<string> IsNotNullOrEmpty(this IObservable<string> source)
        {
            return source.Where(str => !string.IsNullOrEmpty(str));
        }

        public static IObservable<T> GetValueOrDefault<T>(this IObservable<T?> source, T defaultValue = default(T))
            where T : struct
        {
            return source.Select(x => x ?? defaultValue);
        }

        public static IObservable<bool> WhereIsTrue(this IObservable<bool> source)
        {
            return source.Where(result => result);
        }

        public static IObservable<bool> WhereIsFalse(this IObservable<bool> source)
        {
            return source.Where(result => !result);
        }

        public static IObservable<T> WhereIs<T>(this IObservable<T> source, T comparison)
        {
            return source.Where(result => EqualityComparer<T>.Default.Equals(result, comparison));
        }

        public static IObservable<T> WhereIsNot<T>(this IObservable<T> source, T comparison)
        {
            return source.Where(result => !EqualityComparer<T>.Default.Equals(result, comparison));
        }

        public static IObservable<bool> ValueIsFalse(this IObservable<bool> source)
        {
            return source.Select(result => !result);
        }

        public static IObservable<bool> ValueIsTrue(this IObservable<bool> source)
        {
            return source.Select(result => result);
        }

        public static IObservable<T> TakeOne<T>(this IObservable<T> source)
        {
            return source.Take(1);
        }

        public static IObservable<T> SkipOne<T>(this IObservable<T> source)
        {
            return source.Skip(1);
        }

        public static IObservable<PropertyChangedEventArgs> PropertyChanged<T>(this IObservable<T> inpc)
            where T : class, INotifyPropertyChanged
        {
            return inpc
                .Select(
                    data =>
                    {
                        if (data == null)
                        {
                            return Observable.Empty<PropertyChangedEventArgs>();
                        }

                        return
                            Observable
                                .FromEvent<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                                    eventHandler =>
                                    {
                                        void Handler(object sender, PropertyChangedEventArgs e) => eventHandler?.Invoke(e);
                                        return Handler;
                                    },
                                    x => data.PropertyChanged += x,
                                    x => { if (data != null) data.PropertyChanged -= x; });
                    })
                .Switch();
        }

        public static IObservable<T> ThrottleFirst<T>(this IObservable<T> source, TimeSpan delay, IScheduler scheduler = null)
        {
            scheduler = scheduler ?? Scheduler.Default;

            return source
                .Publish(o =>
                {
                    return o
                        .Take(1, scheduler)
                        .Concat(o.IgnoreElements().TakeUntil(Observable.Return(default(T), scheduler).Delay(delay, scheduler)))
                        .Repeat()
                        .TakeUntil(o.IgnoreElements().Concat(Observable.Return(default(T), scheduler)));
                });
        }

        public static IObservable<T> ThrottleFirst<T>(this IObservable<T> source, Action<T> beforeThrottle, Action<T> afterThrottle, TimeSpan delay, IScheduler beforeAndAfterThrottleScheduler = null, IScheduler scheduler = null)
        {
            scheduler = scheduler ?? Scheduler.Default;
            beforeAndAfterThrottleScheduler = beforeAndAfterThrottleScheduler ?? Scheduler.Default;

            return source
                .Publish(o =>
                {
                    return o
                        .Take(1, scheduler)
                        .Concat(
                            o.IgnoreElements()
                                .TakeUntil(
                                    Observable.Return(default(T), scheduler)
                                        .ObserveOn(beforeAndAfterThrottleScheduler)
                                        .Do(beforeThrottle)
                                        .ObserveOn(scheduler)
                                        .Delay(delay, scheduler)
                                        .ObserveOn(beforeAndAfterThrottleScheduler)
                                        .Do(afterThrottle)
                                        .ObserveOn(scheduler)))
                        .Repeat()
                        .TakeUntil(o.IgnoreElements().Concat(Observable.Return(default(T), scheduler)));
                });
        }

        /// <summary>
        /// Applies a conflation algorithm to an observable stream. 
        /// Anytime the stream OnNext twice below minimumUpdatePeriod, the second update gets delayed to respect the minimumUpdatePeriod
        /// If more than 2 update happen, only the last update is pushed
        /// Updates are pushed and rescheduled using the provided scheduler
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">stream</param>
        /// <param name="minimumUpdatePeriod">minimum delay between 2 updates</param>
        /// <param name="scheduler">to be used to publish updates and schedule delayed updates</param>
        /// <returns></returns>
        public static IObservable<T> Conflate<T>(this IObservable<T> source, TimeSpan minimumUpdatePeriod, IScheduler scheduler)
        {
            return Observable.Create<T>(observer =>
            {
                // indicate when the last update was published
                var lastUpdateTime = DateTimeOffset.MinValue;
                // indicate if an update is currently scheduled
                var updateScheduled = new MultipleAssignmentDisposable();
                // indicate if completion has been requested (we can't complete immediately if an update is in flight)
                var completionRequested = false;
                var gate = new object();

                var subscription = source
                        .ObserveOn(scheduler)
                        .Subscribe(
                            x =>
                            {
                                var currentUpdateTime = scheduler.Now;

                                bool scheduleRequired;
                                lock (gate)
                                {
                                    scheduleRequired = currentUpdateTime - lastUpdateTime < minimumUpdatePeriod;
                                    if (scheduleRequired && updateScheduled.Disposable != null)
                                    {
                                        updateScheduled.Disposable.Dispose();
                                        updateScheduled.Disposable = null;
                                    }
                                }

                                if (scheduleRequired)
                                {
                                    updateScheduled.Disposable = scheduler.Schedule(lastUpdateTime + minimumUpdatePeriod, () =>
                                    {
                                        observer.OnNext(x);

                                        lock (gate)
                                        {
                                            lastUpdateTime = scheduler.Now;
                                            updateScheduled.Disposable = null;
                                            if (completionRequested)
                                            {
                                                observer.OnCompleted();
                                            }
                                        }
                                    });
                                }
                                else
                                {
                                    observer.OnNext(x);
                                    lock (gate)
                                    {
                                        lastUpdateTime = scheduler.Now;
                                    }
                                }
                            },
                            observer.OnError,
                            () =>
                            {
                                // if we have scheduled an update we need to complete once the update has been published
                                if (updateScheduled.Disposable != null)
                                {
                                    lock (gate)
                                    {
                                        completionRequested = true;
                                    }
                                }
                                else
                                {
                                    observer.OnCompleted();
                                }
                            });

                return subscription;
            });
        }

        /// <summary>
        /// Injects heartbeats in a stream when the source stream becomes quiet:
        ///  - upon subscription if the source does not OnNext any update a heartbeat will be pushed after heartbeatPeriod, periodilcally until source receives an update
        ///  - when an update is received it is immediatly pushed. After this update, if source does not OnNext after heartbeatPeriod, heartbeats will be pushed
        /// </summary>
        /// <typeparam name="T">update type</typeparam>
        /// <param name="source">source stream</param>
        /// <param name="heartbeatPeriod"></param>
        /// <param name="scheduler"></param>
        /// <returns></returns>
        public static IObservable<IHeartbeat<T>> Heartbeat<T>(this IObservable<T> source, TimeSpan heartbeatPeriod, IScheduler scheduler)
        {
            return Observable.Create<IHeartbeat<T>>(observer =>
            {
                var heartbeatTimerSubscription = new MultipleAssignmentDisposable();
                var gate = new object();

                Action scheduleHeartbeats = () =>
                {
                    var disposable = Observable
                                .Timer(heartbeatPeriod, heartbeatPeriod, scheduler)
                                .Subscribe(
                                    _ => observer.OnNext(new Heartbeat<T>()));

                    lock (gate)
                    {
                        heartbeatTimerSubscription.Disposable = disposable;
                    }
                };

                var sourceSubscription = source.Subscribe(
                    x =>
                    {
                        lock (gate)
                        {
                            // cancel any scheduled heartbeat
                            heartbeatTimerSubscription.Disposable.Dispose();
                        }

                        observer.OnNext(new Heartbeat<T>(x));

                        scheduleHeartbeats();
                    },
                    observer.OnError,
                    observer.OnCompleted);

                scheduleHeartbeats();

                return new CompositeDisposable { sourceSubscription, heartbeatTimerSubscription };
            });
        }

        /// <summary>
        /// Detects when a stream becomes inactive for some period of time
        /// </summary>
        /// <typeparam name="T">update type</typeparam>
        /// <param name="source">source stream</param>
        /// <param name="stalenessPeriod">if source steam does not OnNext any update during this period, it is declared staled</param>
        /// <param name="scheduler"></param>
        /// <returns></returns>
        public static IObservable<IStale<T>> DetectStale<T>(this IObservable<T> source, TimeSpan stalenessPeriod, IScheduler scheduler)
        {
            return Observable.Create<IStale<T>>(observer =>
            {
                var timerSubscription = new SerialDisposable();
                var observerLock = new object();

                Action scheduleStale = () =>
                {
                    timerSubscription.Disposable = Observable
                            .Timer(stalenessPeriod, scheduler)
                            .Subscribe(_ =>
                            {
                                lock (observerLock)
                                {
                                    observer.OnNext(new Stale<T>());
                                }
                            });
                };

                var sourceSubscription = source.Subscribe(
                    x =>
                    {
                        // cancel any scheduled stale update
                        var disposable = timerSubscription.Disposable;
                        if (disposable != null)
                            disposable.Dispose();

                        lock (observerLock)
                        {
                            observer.OnNext(new Stale<T>(x));
                        }

                        scheduleStale();
                    },
                    observer.OnError,
                    observer.OnCompleted);

                scheduleStale();

                return new CompositeDisposable { sourceSubscription, timerSubscription };
            });
        }

        public static IDisposable BindInteraction<TInput, TOutput>(this IObservable<Interaction<TInput, TOutput>> interactionObservable, Action<IInteractionContext<TInput, TOutput>> handler)
        {
            var interactionDisposable = new SerialDisposable();

            return
                interactionObservable
                    .Where(x => x != null)
                    .Do(x => interactionDisposable.Disposable = x.RegisterHandler(handler))
                    .Finally(() => interactionDisposable?.Dispose())
                    .Subscribe();
        }

        public static IDisposable BindInteraction<TInput, TOutput>(this IObservable<Interaction<TInput, TOutput>> interactionObservable, Func<IInteractionContext<TInput, TOutput>, Task> handler)
        {
            var interactionDisposable = new SerialDisposable();

            return
                interactionObservable
                    .Where(x => x != null)
                    .Do(x => interactionDisposable.Disposable = x.RegisterHandler(handler))
                    .Finally(() => interactionDisposable?.Dispose())
                    .Subscribe();
        }

        public static IDisposable BindInteraction<TInput, TOutput, TDontCare>(this IObservable<Interaction<TInput, TOutput>> interactionObservable, Func<IInteractionContext<TInput, TOutput>, IObservable<TDontCare>> handler)
        {
            var interactionDisposable = new SerialDisposable();

            return
                interactionObservable
                    .Where(x => x != null)
                    .Do(x => interactionDisposable.Disposable = x.RegisterHandler(handler))
                    .Finally(() => interactionDisposable?.Dispose())
                    .Subscribe();
        }
    }

    public interface IHeartbeat<out T>
    {
        bool IsHeartbeat { get; }
        T Update { get; }
    }

    class Heartbeat<T> : IHeartbeat<T>
    {
        public bool IsHeartbeat { get; private set; }
        public T Update { get; private set; }

        public Heartbeat() : this(true, default(T))
        {
        }

        public Heartbeat(T update) : this(false, update)
        {
        }

        private Heartbeat(bool isHeartbeat, T update)
        {
            IsHeartbeat = isHeartbeat;
            Update = update;
        }
    }

    public interface IStale<out T>
    {
        bool IsStale { get; }
        T Update { get; }
    }

    class Stale<T> : IStale<T>
    {
        private readonly T _update;

        public Stale() : this(true, default(T))
        {
        }

        public Stale(T update) : this(false, update)
        {
        }

        private Stale(bool isStale, T update)
        {
            IsStale = isStale;
            _update = update;
        }

        public bool IsStale { get; private set; }

        public T Update
        {
            get
            {
                if (IsStale)
                    throw new InvalidOperationException("Stale instance has no update.");
                return _update;
            }
        }
    }
}

