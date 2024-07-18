using System;
using System.Reactive.Disposables;

namespace System.Reactive.Disposables
{
	public static class DisposableMixins
	{    
		public static TDisposable DisposeWith<TDisposable> (this TDisposable observable, Lazy<CompositeDisposable> disposable) 
            where TDisposable : IDisposable
		{
		    disposable?.Value?.Add (observable);

			return observable;
		}

        public static TDisposable DisposeWith<TDisposable>(this TDisposable observable, SerialDisposable disposable) 
            where TDisposable : IDisposable
        {
            disposable.Disposable = observable;

            return observable;
        }

        public static TDisposable DisposeWith<TDisposable>(this TDisposable observable, Lazy<SerialDisposable> disposable) 
            where TDisposable : IDisposable
        {
            if(disposable != null && disposable.Value != null)
                disposable.Value.Disposable = observable;

            return observable;
        }
        
        public static TDisposable DisposeWith<TDisposable>(this TDisposable observable, StackCompositeDisposable disposable) 
            where TDisposable : IDisposable
        {
            disposable.Add(observable);

            return observable;
        }

        public static TDisposable DisposeWith<TDisposable>(this TDisposable observable, Lazy<StackCompositeDisposable> disposable) 
            where TDisposable : IDisposable
        {
            if(disposable != null && disposable.Value != null)
                disposable.Value.Add(observable);

            return observable;
        }        
        
        public static TDisposable DisposeWith<TDisposable>(this TDisposable observable, Action<IDisposable> disposable)
            where TDisposable : IDisposable
        {
            disposable(observable);

            return observable;
        }
	}
}
