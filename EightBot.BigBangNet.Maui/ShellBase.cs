using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using EightBot.BigBang.ViewModel;
using ReactiveUI;
using Splat;
using Microsoft.Maui;

namespace EightBot.BigBang.Maui
{
    public abstract class ShellBase<TViewModel> : ReactiveShell<TViewModel>, IEnableLogger
        where TViewModel : class
    {
        readonly Subject<LifecycleEvent> _lifecycle = new Subject<LifecycleEvent>();
        public IObservable<Unit> Activated => _lifecycle.Where(x => x == LifecycleEvent.Activated).SelectUnit().AsObservable();
        public IObservable<Unit> Deactivated => _lifecycle.Where(x => x == LifecycleEvent.Deactivated).SelectUnit().AsObservable();
        public IObservable<Unit> IsAppearing => _lifecycle.Where(x => x == LifecycleEvent.IsAppearing).SelectUnit().AsObservable();
        public IObservable<Unit> IsDisappearing => _lifecycle.Where(x => x == LifecycleEvent.IsDisappearing).SelectUnit().AsObservable();
        public IObservable<LifecycleEvent> Lifecycle => _lifecycle.AsObservable();

        bool _controlsBound = false;

        readonly string _viewModelName;

        private readonly object _bindingLock = new object();

        private readonly StackCompositeDisposable _controlBindings = new StackCompositeDisposable();

        protected StackCompositeDisposable ControlBindings => _controlBindings;

        public bool ControlsBound => Volatile.Read(ref _controlsBound);

        public bool MaintainBindings { get; set; }

        protected ShellBase(bool delayBindingRegistrationUntilActivation = false)
        {
            IDisposable performanceLogger = null;

            Initialize();

            if (GlobalConfiguration.LogPerformanceMetrics)
            {
                _viewModelName = this.GetType().Name;

                performanceLogger =
                    new Helpers.PerformanceCapture(ts =>
                        this.Log().Debug($"{"[Setup User Interface]",-25}{_viewModelName,-25}{$"{ts.TotalMilliseconds:N1}ms",25}"));
            }

            SetupUserInterface();

            performanceLogger?.Dispose();

            if (!delayBindingRegistrationUntilActivation)
                RegisterBindings();

            if (GlobalConfiguration.GenerateUITestViewNames)
                this.GenerateViewNames(this);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            _lifecycle?.OnNext(LifecycleEvent.IsAppearing);
        }

        protected override void OnDisappearing()
        {
            _lifecycle?.OnNext(LifecycleEvent.IsDisappearing);

            base.OnDisappearing();
        }

        protected override async void OnPropertyChanged(string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            if (!(propertyName?.Equals(nameof(Window)) ?? false))
            {
                return;
            }

            if (Window is not null)
            {
                RegisterBindings();

                _lifecycle?.OnNext(LifecycleEvent.Activated);

                return;
            }

            _lifecycle?.OnNext(LifecycleEvent.Deactivated);

            UnregisterBindings();
        }

        protected virtual void Initialize() { }
        protected abstract void SetupUserInterface();
        protected abstract void BindControls();

        protected void RegisterBindings()
        {
            lock (_bindingLock)
            {
                if (_controlsBound)
                {
                    return;
                }

                Volatile.Write(ref _controlsBound, true);

                IDisposable performanceLogger = null;

                if (GlobalConfiguration.LogPerformanceMetrics)
                {
                    performanceLogger =
                        new Helpers.PerformanceCapture(ts =>
                            this.Log().Debug($"{"[Register Bindings]",-25}{_viewModelName,-25}{$"{ts.TotalMilliseconds:N1}ms",25}"));

                    this.ProfileLayout()
                        .DisposeWith(ControlBindings);
                }

                this.RegisterViewModelBindings()
                    .DisposeWith(ControlBindings);

                BindControls();

                performanceLogger?.Dispose();
            }
        }

        protected void UnregisterBindings()
        {
            lock (_bindingLock)
            {
                if (MaintainBindings || !_controlsBound)
                {
                    return;
                }

                Volatile.Write(ref _controlsBound, false);

                _controlBindings?.Clear();

                if (ViewModel is ViewModelBase vmb)
                {
                    vmb?.UnregisterBindings();
                }
            }
        }
    }
}