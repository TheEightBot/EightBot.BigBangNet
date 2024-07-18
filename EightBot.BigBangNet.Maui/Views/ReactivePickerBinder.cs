using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading;
using EightBot.BigBang.Maui.Views;
using ReactiveUI;
using Microsoft.Maui;
using System.Globalization;
using System.Collections;
using System.Collections.Specialized;

namespace EightBot.BigBang.Maui
{
    public static class PickerExtensions
    {
        public static ReactivePickerBinder<TViewModel> Bind<TViewModel>(this Picker picker, IEnumerable<TViewModel> items, Action<TViewModel> selectedItemChanged, Func<TViewModel, bool> selectItem, Func<TViewModel, string> titleSelector)
        {
            return new ReactivePickerBinder<TViewModel>(picker, items, selectedItemChanged, selectItem, titleSelector, null);
        }

        public static ReactivePickerBinder<TViewModel> Bind<TViewModel, TDontCare>(this Picker picker,
            IEnumerable<TViewModel> items,
            Action<TViewModel> selectedItemChanged,
            Func<TViewModel, bool> selectItem,
            Func<TViewModel, string> titleSelector,
            IObservable<TDontCare> signalRefresh = null)
        {

            IObservable<Unit> refresh = null;
            if (signalRefresh != null)
                refresh = signalRefresh.Select(_ => Unit.Default);

            return new ReactivePickerBinder<TViewModel>(picker, items, selectedItemChanged, selectItem, titleSelector, refresh);
        }

        public static ReactivePickerBinder<TViewModel> Bind<TViewModel>(this Picker picker, IObservable<IEnumerable<TViewModel>> items, Action<TViewModel> selectedItemChanged, Func<TViewModel, bool> selectItem, Func<TViewModel, string> titleSelector)
        {
            return new ReactivePickerBinder<TViewModel>(picker, items, selectedItemChanged, selectItem, titleSelector, null);
        }

        public static ReactivePickerBinder<TViewModel> Bind<TViewModel, TDontCare>(this Picker picker,
            IObservable<IEnumerable<TViewModel>> items,
            Action<TViewModel> selectedItemChanged,
            Func<TViewModel, bool> selectItem,
            Func<TViewModel, string> titleSelector,
            IObservable<TDontCare> signalRefresh = null)
        {

            IObservable<Unit> refresh = null;
            if (signalRefresh != null)
                refresh = signalRefresh.Select(_ => Unit.Default);

            return new ReactivePickerBinder<TViewModel>(picker, items, selectedItemChanged, selectItem, titleSelector, refresh);
        }

        public static void ResetToInitialValue(this Picker picker)
        {
            picker.SelectedItem = null;
            picker.SelectedIndex = -1;
        }
    }
}

namespace EightBot.BigBang.Maui.Views
{
    public class ReactivePickerBinder<TViewModel> : IDisposable
    {
        // Properties
        public TViewModel SelectedItem
            => _picker?.SelectedItem != null
                ? (TViewModel)_picker.SelectedItem
                : default(TViewModel);

        (bool Success, TViewModel FoundItem) GetItemAt(int index)
            => index < (_picker?.ItemsSource?.Count ?? 0)
                ? (true, (TViewModel)_picker.ItemsSource[index])
                : (false, default(TViewModel));

        private readonly Action<TViewModel> _selectedItemChanged;
        private readonly Func<TViewModel, bool> _selectItem;

        Picker _picker;

        CompositeDisposable _disposableSubscriptions;

        // Constructors

        public ReactivePickerBinder(Picker picker, IEnumerable<TViewModel> items,
            Action<TViewModel> selectedItemChanged, Func<TViewModel, bool> selectItem, Func<TViewModel, string> titleSelector,
            IObservable<Unit> signalViewUpdate = null) : base()
        {
            _picker = picker;
            _picker.ItemDisplayBinding = new Binding(".", BindingMode.OneWay, new FuncConverter(titleSelector));

            _selectedItemChanged = selectedItemChanged;
            _selectItem = selectItem;

            _disposableSubscriptions = new CompositeDisposable();

            Observable
                .FromEvent<EventHandler<FocusEventArgs>, FocusEventArgs>(
                    eventHandler =>
                    {
                        void Handler(object sender, FocusEventArgs e) => eventHandler?.Invoke(e);
                        return Handler;
                    },
                    x => _picker.Focused += x,
                    x => { if (_picker != null) _picker.Focused -= x; })
                .ObserveOn(Schedulers.ShortTermThreadPoolScheduler)
                .Where(_ => Device.RuntimePlatform == Device.iOS)
                .Where(args => args.IsFocused)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(args =>
                {
                    if (_picker != null && _picker.SelectedIndex < 0 && _picker.Items.Count > 0)
                        _picker.SelectedIndex = 0;
                })
                .DisposeWith(_disposableSubscriptions);

            _picker
                .WhenAnyValue(x => x.SelectedItem)
                .ObserveOn(Schedulers.ShortTermThreadPoolScheduler)
                .Select(item => item != null ? (TViewModel)item : default(TViewModel))
                .Skip(1)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Do(item => SelectedItemChanged(item, true))
                .Subscribe()
                .DisposeWith(_disposableSubscriptions);

            Observable
                .Return(items)
                .ObserveOn(Schedulers.ShortTermThreadPoolScheduler)
                .Select(itms => itms is IList<TViewModel> iltvm ? iltvm : itms?.ToList())
                .ObserveOn(RxApp.MainThreadScheduler)
                .Do(itms => SetItems(itms))
                .ObserveOn(Schedulers.ShortTermThreadPoolScheduler)
                .Select(
                    itms =>
                    {
                        var svu =
                            signalViewUpdate != null
                                ? signalViewUpdate
                                    .ObserveOn(Schedulers.ShortTermThreadPoolScheduler)
                                    .Select(_ => true)
                                : Observable.Return(false);

                        var inccChanges =
                            itms is INotifyCollectionChanged incc
                                ? Observable
                                    .FromEvent<NotifyCollectionChangedEventHandler, NotifyCollectionChangedEventArgs>(
                                        eventHandler =>
                                        {
                                            void Handler(object sender, NotifyCollectionChangedEventArgs e) => eventHandler?.Invoke(e);
                                            return Handler;
                                        },
                                        x => incc.CollectionChanged += x,
                                        x => incc.CollectionChanged -= x)
                                    .ObserveOn(Schedulers.ShortTermThreadPoolScheduler)
                                    .Select(_ => false)
                                : Observable.Empty<bool>();

                        return Observable.Merge(svu, inccChanges);
                    })
                .Switch()
                .Do(fromNotificationTrigger => SetSelectedItem(fromNotificationTrigger))
                .Subscribe()
                .DisposeWith(_disposableSubscriptions);
        }

        public ReactivePickerBinder(Picker picker, IObservable<IEnumerable<TViewModel>> items,
            Action<TViewModel> selectedItemChanged, Func<TViewModel, bool> selectItem, Func<TViewModel, string> titleSelector,
            IObservable<Unit> signalViewUpdate = null) : base()
        {
            _picker = picker;

            _picker.ItemDisplayBinding = new Binding(".", BindingMode.OneWay, new FuncConverter(titleSelector));

            _selectedItemChanged = selectedItemChanged;
            _selectItem = selectItem;

            _disposableSubscriptions = new CompositeDisposable();

            Observable
                .FromEvent<EventHandler<FocusEventArgs>, FocusEventArgs>(
                    eventHandler =>
                    {
                        void Handler(object sender, FocusEventArgs e) => eventHandler?.Invoke(e);
                        return Handler;
                    },
                    x => _picker.Focused += x,
                    x => { if (_picker != null) _picker.Focused -= x; })
                .ObserveOn(Schedulers.ShortTermThreadPoolScheduler)
                .Where(_ => Device.RuntimePlatform == Device.iOS)
                .Where(args => args.IsFocused)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(args =>
                {
                    if (_picker != null && _picker.SelectedIndex < 0 && _picker.Items.Count > 0)
                        _picker.SelectedIndex = 0;
                })
                .DisposeWith(_disposableSubscriptions);

            _picker
                .WhenAnyValue(x => x.SelectedItem)
                .ObserveOn(Schedulers.ShortTermThreadPoolScheduler)
                .Select(item => item != null ? (TViewModel)item : default(TViewModel))
                .Skip(1)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Do(item => SelectedItemChanged(item, true))
                .Subscribe()
                .DisposeWith(_disposableSubscriptions);

            items
                .Select(itms => itms is IList<TViewModel> iltvm ? iltvm : itms?.ToList())
                .ObserveOn(RxApp.MainThreadScheduler)
                .Do(itms => SetItems(itms))
                .Select(
                    itms =>
                    {
                        var svu =
                            signalViewUpdate != null
                                ? signalViewUpdate
                                    .ObserveOn(Schedulers.ShortTermThreadPoolScheduler)
                                    .Select(_ => true)
                                : Observable.Return(false);

                        var inccChanges =
                            itms is INotifyCollectionChanged incc
                                ? Observable
                                    .FromEvent<NotifyCollectionChangedEventHandler, NotifyCollectionChangedEventArgs>(
                                        eventHandler =>
                                        {
                                            void Handler(object sender, NotifyCollectionChangedEventArgs e) => eventHandler?.Invoke(e);
                                            return Handler;
                                        },
                                        x => incc.CollectionChanged += x,
                                        x => incc.CollectionChanged -= x)
                                    .ObserveOn(Schedulers.ShortTermThreadPoolScheduler)
                                    .Select(_ => false)
                                : Observable.Empty<bool>();

                        return Observable.Merge(svu, inccChanges);
                    })
                .Switch()
                .Do(fromNotificationTrigger => SetSelectedItem(fromNotificationTrigger))
                .Subscribe()
                .DisposeWith(_disposableSubscriptions);
        }

        // Methods
        void SetItems(IList<TViewModel> items)
        {
            if (_picker != null)
            {
                _picker.ItemsSource = (IList)items;
            }
        }

        void SetSelectedItem(bool fromNotificationTrigger = false)
        {
            var pickerItemCount = _picker?.ItemsSource?.Count ?? 0;
            if (pickerItemCount <= 0)
            {
                return;
            }

            for (int index = 0; index < pickerItemCount; index++)
            {
                var itemAt = this.GetItemAt(index);
                if (itemAt.Success && _selectItem.Invoke(itemAt.FoundItem))
                {
                    if ((itemAt.FoundItem?.Equals(default(TViewModel)) ?? false) || !EqualityComparer<TViewModel>.Default.Equals(itemAt.FoundItem, this.SelectedItem))
                    {
                        SelectedItemChanged(itemAt.FoundItem);
                    }
                    return;
                }
            }

            if (fromNotificationTrigger)
            {
                SelectedItemChanged(default(TViewModel));
            }
        }

        void SelectedItemChanged(TViewModel item, bool fromUi = false)
        {
            _selectedItemChanged?.Invoke(item);

            if (!fromUi)
            {
                if (_picker != null && (_picker.SelectedItem == null || !EqualityComparer<TViewModel>.Default.Equals(item, this.SelectedItem)))
                {
                    Device.BeginInvokeOnMainThread(() => _picker.SelectedItem = item);
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                var subscriptions = Interlocked.Exchange(ref _disposableSubscriptions, null);
                subscriptions?.Dispose();

                _picker = null;
            }
        }

        private class FuncConverter : IValueConverter
        {
            private readonly Func<TViewModel, string> _titleSelector;
            public FuncConverter(Func<TViewModel, string> titleSelector)
            {
                _titleSelector = titleSelector;
            }

            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                return
                    value is TViewModel viewModel
                        ? _titleSelector.Invoke(viewModel)
                        : string.Empty;
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                return default(TViewModel);
            }
        }
    }
}

