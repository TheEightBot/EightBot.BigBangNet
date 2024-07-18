using System;
using System.Linq.Expressions;
using System.Reactive.Linq;
using EightBot.BigBang.ViewModel;
using ReactiveUI;

namespace EightBot.BigBang
{
	public static class IViewForExtensions
	{

		public static IObservable<ValidationInformation> MonitorValidationInformationFor<TViewModel, TProperty>(this IViewFor<TViewModel> view, Expression<Func<TViewModel, TProperty>> property) 
            where TViewModel: ValidationViewModelBase<TViewModel> 
        {
			return view
				.WhenAnyValue(x => x.ViewModel)
				.SelectMany(x => x.MonitorValidationInformationFor(property));
		}
    }
}
