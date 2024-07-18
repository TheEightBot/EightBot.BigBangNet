using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using ReactiveUI;

namespace EightBot.BigBang
{
    public static class InteractionExtensions
    {
        public static IDisposable Bind<TInput, TOutput>(this IObservable<Interaction<TInput, TOutput>> interactionObservable, Action<InteractionContext<TInput, TOutput>> handler)
        {
            var interactionDisposable = new SerialDisposable();

            return
                interactionObservable
                    .Where(x => x != null)
                    .Do(x => interactionDisposable.Disposable = x.RegisterHandler(handler))
                    .Finally(() => interactionDisposable?.Dispose())
                    .Subscribe();
        }

        public static IDisposable Bind<TInput, TOutput>(this IObservable<Interaction<TInput, TOutput>> interactionObservable, Func<InteractionContext<TInput, TOutput>, Task> handler)
        {
            var interactionDisposable = new SerialDisposable();

            return
                interactionObservable
                    .Where(x => x != null)
                    .Do(x => interactionDisposable.Disposable = x.RegisterHandler(handler))
                    .Finally(() => interactionDisposable?.Dispose())
                    .Subscribe();
        }

        public static IDisposable Bind<TInput, TOutput, TDontCare>(this IObservable<Interaction<TInput, TOutput>> interactionObservable, Func<InteractionContext<TInput, TOutput>, IObservable<TDontCare>> handler)
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
}
