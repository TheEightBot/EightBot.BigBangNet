using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Runtime.CompilerServices;
using ReactiveUI;
using Splat;
using Microsoft.Maui;
using static EightBot.BigBang.Maui.Effects.EffectNames;

namespace EightBot.BigBang.Maui.Views
{
    public class ReactiveListView : ListView, IEnableLogger
    {
        readonly ConcurrentDictionary<Cell, IDisposable> _cellActivators = new ConcurrentDictionary<Cell, IDisposable>();

        private Action<CompositeDisposable, Cell, int> _cellActivatedAction;

        public ReactiveListView(Type cellType, ListViewCachingStrategy cachingStrategy = ListViewCachingStrategy.RecycleElement)
            : this(cachingStrategy)
        {
            ItemTemplate = new DataTemplate(cellType);
        }

        public ReactiveListView(Func<object> loadTemplate, ListViewCachingStrategy cachingStrategy = ListViewCachingStrategy.RecycleElement)
            : this(cachingStrategy)
        {
            ItemTemplate = new DataTemplate(loadTemplate);
        }


        public ReactiveListView(ListViewCachingStrategy cachingStrategy = ListViewCachingStrategy.RecycleElement)
            : base(cachingStrategy)
        {

            if (Device.RuntimePlatform == Device.UWP)
            {
                this.Effects.Add(new ListViewActivationRoutingEffect());
            }

        }

        public IObservable<T> ListViewItemTapped<T>() where T : class
        {
            return
                Observable
                    .FromEvent<EventHandler<ItemTappedEventArgs>, ItemTappedEventArgs>(
                        eventHandler =>
                        {
                            void Handler(object sender, ItemTappedEventArgs e) => eventHandler?.Invoke(e);
                            return Handler;
                        },
                        x => this.ItemTapped += x,
                        x => this.ItemTapped -= x)
                    .Select(x => x.Item)
                    .OfType<T>();
        }

        public IObservable<object> ListViewItemTapped()
        {
            return
                Observable
                    .FromEvent<EventHandler<ItemTappedEventArgs>, ItemTappedEventArgs>(
                        eventHandler =>
                        {
                            void Handler(object sender, ItemTappedEventArgs e) => eventHandler?.Invoke(e);
                            return Handler;
                        },
                        x => this.ItemTapped += x,
                        x => this.ItemTapped -= x)
                    .Select(args => args.Item);
        }

        public IObservable<T> ListViewItemSelected<T>() where T : class
        {
            return
                Observable
                    .FromEvent<EventHandler<SelectedItemChangedEventArgs>, SelectedItemChangedEventArgs>(
                        eventHandler =>
                        {
                            void Handler(object sender, SelectedItemChangedEventArgs e) => eventHandler?.Invoke(e);
                            return Handler;
                        },
                        x => this.ItemSelected += x,
                        x => this.ItemSelected -= x)
                    .Select(x => x.SelectedItem)
                    .OfType<T>();
        }

        public IObservable<object> ListViewItemSelected()
        {
            return
                Observable
                    .FromEvent<EventHandler<SelectedItemChangedEventArgs>, SelectedItemChangedEventArgs>(
                        eventHandler =>
                        {
                            void Handler(object sender, SelectedItemChangedEventArgs e) => eventHandler?.Invoke(e);
                            return Handler;
                        },
                        x => this.ItemSelected += x,
                        x => this.ItemSelected -= x)
                    .Select(args => args.SelectedItem);
        }

        public IDisposable SetCellActivationAction(Action<CompositeDisposable, Cell, int> cellActivatedAction)
        {
            _cellActivatedAction = cellActivatedAction;

            return Disposable.Create(
                () =>
                {
                    _cellActivatedAction = null;

                    foreach (var cellItem in _cellActivators)
                    {
                        cellItem.Value?.Dispose();
                    }

                    _cellActivators.Clear();
                });
        }

        protected override void SetupContent(Cell content, int index)
        {
            base.SetupContent(content, index);

            if (_cellActivatedAction != null && !_cellActivators.ContainsKey(content))
            {
                var disposable = new CompositeDisposable();
                _cellActivatedAction(disposable, content, index);
                _cellActivators.AddOrUpdate(content, disposable, (k, v) => disposable);
            }
        }

        protected override void UnhookContent(Cell content)
        {
            if (_cellActivators.ContainsKey(content) && _cellActivators.TryRemove(content, out var disposable))
            {
                disposable?.Dispose();
            }

            base.UnhookContent(content);
        }

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            if (!(propertyName?.Equals(nameof(Window)) ?? false))
            {
                return;
            }

            if (Window is null)
            {
                if (_cellActivators.Any())
                {
                    var keys = _cellActivators.Keys.ToArray();

                    foreach (var key in keys)
                    {
                        if (_cellActivators.TryRemove(key, out var disposable))
                        {
                            disposable?.Dispose();
                            disposable = null;
                        }
                    }
                }
            }
        }
    }

    public static class ReactiveListViewExtensions
    {
        public static IDisposable WhenCellActivated(this ReactiveListView reactiveList, Action<CompositeDisposable, Cell, int> whenCellActivated)
        {
            return reactiveList.SetCellActivationAction(whenCellActivated);
        }
    }
}

