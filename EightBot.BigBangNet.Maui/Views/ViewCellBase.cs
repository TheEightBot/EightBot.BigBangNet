﻿using System;
using ReactiveUI;
using Xamarin.Forms;
using System.Reactive.Disposables;
using EightBot.BigBang.XamForms.Values;
using EightBot.BigBang.XamForms.Interfaces;
using EightBot.BigBang.ViewModel;
using EightBot.BigBang.XamForms;
using ReactiveUI.XamForms;
using Splat;
using System.Reactive.Subjects;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;

namespace EightBot.BigBang.XamForms.Views
{
	public abstract class ViewCellBase<TViewModel> : ReactiveViewCell<TViewModel>, IEnableLogger
        where TViewModel : class
	{
        readonly Subject<LifecycleEvent> _lifecycle = new Subject<LifecycleEvent>();
        public IObservable<Unit> Activated => _lifecycle.Where(x => x == LifecycleEvent.Activated).SelectUnit().AsObservable();
        public IObservable<Unit> Deactivated => _lifecycle.Where(x => x == LifecycleEvent.Deactivated).SelectUnit().AsObservable();
        public IObservable<LifecycleEvent> Lifecycle => _lifecycle.AsObservable();

        bool _controlsBound;

        readonly string _viewModelName;

        private readonly object _bindingLock = new object();

        private readonly StackCompositeDisposable _controlBindings = new StackCompositeDisposable();

        protected StackCompositeDisposable ControlBindings => _controlBindings;

        public bool ControlsBound => Volatile.Read(ref _controlsBound);

        public bool MaintainBindings { get; set; }

        public ViewCellBase(bool delayBindingRegistrationUntilActivation = false)
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

        protected override void OnPropertyChanged(string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            if (
                propertyName.Equals(Values.XamarinFormsHiddenProperties.RendererProperty, StringComparison.OrdinalIgnoreCase) ||
                propertyName.Equals(Values.XamarinFormsHiddenProperties.RealCellProperty, StringComparison.OrdinalIgnoreCase))
            {
                var rendererResolver = Service.RendererResolver;

                if (rendererResolver.HasCellRenderer(this))
                {
                    RegisterBindings();

                    _lifecycle?.OnNext(LifecycleEvent.Activated);
                }
                else
                {
                    _lifecycle?.OnNext(LifecycleEvent.Deactivated);

                    UnregisterBindings();
                }
            }
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

                    this.View?.ProfileLayout()
                        ?.DisposeWith(ControlBindings);
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

    public abstract class ViewCellBase<TViewModel, TDataModel> : ViewCell, IViewFor<TViewModel>, IEnableLogger
        where TViewModel : class, new()
        where TDataModel : class
    {
        readonly Subject<LifecycleEvent> _lifecycle = new Subject<LifecycleEvent>();
        public IObservable<Unit> Activated => _lifecycle.Where(x => x == LifecycleEvent.Activated).SelectUnit().AsObservable();
        public IObservable<Unit> Deactivated => _lifecycle.Where(x => x == LifecycleEvent.Deactivated).SelectUnit().AsObservable();
        public IObservable<LifecycleEvent> Lifecycle => _lifecycle.AsObservable();

        bool _controlsBound;

        readonly string _viewModelName;

        private readonly object _bindingLock = new object();

        private readonly StackCompositeDisposable _controlBindings = new StackCompositeDisposable();

        protected StackCompositeDisposable ControlBindings => _controlBindings;

        public bool ControlsBound => Volatile.Read(ref _controlsBound);

        public bool MaintainBindings { get; set; }

        public ViewCellBase(bool delayBindingRegistrationUntilActivation = false)
        {
            this.ViewModel = new TViewModel();

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

        protected override void OnPropertyChanged(string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            if (propertyName.Equals(Values.XamarinFormsHiddenProperties.RendererProperty, StringComparison.OrdinalIgnoreCase) ||
                propertyName.Equals(Values.XamarinFormsHiddenProperties.RealCellProperty, StringComparison.OrdinalIgnoreCase))
            {
                var rendererResolver = Service.RendererResolver;

                if (rendererResolver.HasCellRenderer(this))
                {
                    RegisterBindings();

                    _lifecycle?.OnNext(LifecycleEvent.Activated);
                }
                else
                {
                    _lifecycle?.OnNext(LifecycleEvent.Deactivated);

                    UnregisterBindings();
                }
            }
        }

        protected virtual void Initialize() { }
        protected abstract void SetupUserInterface();
        protected abstract void BindControls();
        protected abstract void MapDataModelToViewModel(TViewModel viewModel, TDataModel dataModel);

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

                    this.View?.ProfileLayout()
                        ?.DisposeWith(ControlBindings);
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

        /// <summary>
        /// The ViewModel to display
        /// </summary>
        public TViewModel ViewModel
        {
            get { return (TViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }
        public static readonly BindableProperty ViewModelProperty =
            BindableProperty.Create(nameof(ViewModel), typeof(TViewModel), typeof(ViewCellBase<TViewModel, TDataModel>), default(TViewModel), BindingMode.OneWay);

        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = (TViewModel)value; }
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            if (this.BindingContext is TDataModel dataModel)
            {
                MapDataModelToViewModel(this.ViewModel, dataModel);
            }
        }
    }
}
