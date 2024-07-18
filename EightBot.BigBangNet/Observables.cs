using System;
using System.Reactive;
using System.Reactive.Linq;

namespace EightBot.BigBang
{
	public static class Observables
	{
		public static readonly IObservable<Unit> UnitDefault = Observable.Return(Unit.Default);

		public static readonly IObservable<bool> True = Observable.Return(true);

		public static readonly IObservable<bool> False = Observable.Return(false);
	}
}

