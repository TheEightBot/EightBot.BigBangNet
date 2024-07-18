using System;
using System.Linq.Expressions;
using System.Reactive.Linq;
using EightBot.BigBang.ViewModel;
using ReactiveUI;

namespace EightBot.BigBang
{
    public static class IViewForExtensions
    {
        public static IDisposable RegisterViewModelBindings<TViewModel>(this IViewFor<TViewModel> view)
            where TViewModel : class
        {
            return view
                .WhenAnyValue(x => x.ViewModel)
                .ObserveOn(Schedulers.ShortTermThreadPoolScheduler)
                .OfType<ViewModelBase>()
                .IsNotNull()
                .Do(vm => vm.RegisterBindings())
                .Subscribe();
        }
    }
}
