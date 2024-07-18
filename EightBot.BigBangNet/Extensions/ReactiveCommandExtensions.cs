using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using ReactiveUI;

namespace EightBot.BigBang
{
    public static class ReactiveCommandExtensions
    {
        public static IObservable<TTarget> CatchAndReturnDefault<TSource, TTarget, TException>(this ReactiveCommand<TSource, TTarget> source)
            where TException : Exception
        {
            return source.Catch<TTarget, TException>(ex => Observable.Return(default(TTarget)));
        }

        public static Task<TTarget> ExecuteIfCan<TSource, TTarget>(this ReactiveCommand<TSource, TTarget> source)
        {
            return source.ExecuteIfCan(default(TSource));
        }

        public static async Task<TTarget> ExecuteIfCan<TSource, TTarget>(this ReactiveCommand<TSource, TTarget> source, TSource commandParameter)
        {
            if (source == null)
                return default(TTarget);

            if ((source as ICommand).CanExecute(commandParameter))
                await source.Execute(commandParameter);

            return default(TTarget);
        }

        public static IObservable<bool> IsNotExecuting<TSource, TTarget>(this ReactiveCommand<TSource, TTarget> command)
        {
            return command.IsExecuting.Select(x => !x);
        }
    }
}

