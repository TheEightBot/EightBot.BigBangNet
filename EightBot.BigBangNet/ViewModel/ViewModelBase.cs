using System;
using System.Linq;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Threading;
using ReactiveUI;
using System.Reactive;
using System.Runtime.Serialization;
using System.Reactive.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Splat;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Reactive.Concurrency;
using ReactiveUI.Fody.Helpers;

namespace EightBot.BigBang.ViewModel
{
    public abstract class ViewModelBase : ReactiveObject, IEnableLogger
    {
        private readonly object _bindingLock = new object();

        private readonly StackCompositeDisposable _viewModelBindings = new StackCompositeDisposable();
        protected StackCompositeDisposable ViewModelBindings => _viewModelBindings;

        int _loadingCount;
        string _viewModelName = null;

        protected bool _isLoading, _bindingsRegistered, _maintainBindings;

        public bool IsLoading
        {
            get { return _isLoading; }
            set
            {

                if (
                    (_loadingCount == 0 && value == true)
                    ||
                    (_loadingCount == 1 && value == false)
                )
                    this.RaiseAndSetIfChanged(ref _isLoading, value);

                if (value == true)
                    Interlocked.Increment(ref _loadingCount);
                else
                    Interlocked.Decrement(ref _loadingCount);

                if (_loadingCount < 0)
                    Interlocked.Exchange(ref _loadingCount, 0);
            }
        }

        public bool MaintainBindings { get; set; }

        public virtual string Title { get; }

        protected ViewModelBase() : this(true)
        {
        }

        protected ViewModelBase(bool registerObservables = true)
        {
            IDisposable performanceLogger = null;

            if (GlobalConfiguration.LogPerformanceMetrics)
            {
                _viewModelName = this.GetType().Name;

                performanceLogger =
                    new Helpers.PerformanceCapture(ts =>
                        this.Log().Debug($"{"[Initialize]",-25}{_viewModelName,-25}{$"{ts.TotalMilliseconds:N1}ms",25}"));
            }

            Initialize();

            performanceLogger?.Dispose();

            if (registerObservables)
            {
                RegisterBindings();
            }
        }

        protected virtual void Initialize() { }

        protected abstract void RegisterObservables();

        public void RegisterBindings()
        {
            lock (_bindingLock)
            {
                if (_bindingsRegistered)
                {
                    return;
                }

                Volatile.Write(ref _bindingsRegistered, true);

                IDisposable performanceLogger = null;

                if (GlobalConfiguration.LogPerformanceMetrics)
                {
                    performanceLogger =
                        new Helpers.PerformanceCapture(ts =>
                            this.Log().Debug($"{"[Register Observables]",-25}{_viewModelName,-25}{$"{ts.TotalMilliseconds:N1}ms",25}"));
                }

                RegisterObservables();

                performanceLogger?.Dispose();
            }
        }

        public void UnregisterBindings()
        {
            lock (_bindingLock)
            {
                if (MaintainBindings || !_bindingsRegistered)
                {
                    return;
                }

                Volatile.Write(ref _bindingsRegistered, false);

                _viewModelBindings?.Clear();
            }
        }
    }
}
