using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using ReactiveUI;
using Splat;
using Microsoft.Maui;

namespace EightBot.BigBang.Maui.Views
{
    public class GroupedReactiveListView : ListView, IEnableLogger
    {
        readonly ConcurrentDictionary<Cell, IDisposable> _cellActivators = new ConcurrentDictionary<Cell, IDisposable>();

        private Action<CompositeDisposable, Cell, int> _cellActivatedAction;

        public GroupedReactiveListView(Type cellType, Type groupingType, ListViewCachingStrategy cachingStrategy = ListViewCachingStrategy.RecycleElement)
            : this(cachingStrategy)
        {
            ItemTemplate = new DataTemplate(cellType);
            GroupHeaderTemplate = new DataTemplate(groupingType);
        }

        public GroupedReactiveListView(Type cellType, Binding groupDisplayBinding, ListViewCachingStrategy cachingStrategy = ListViewCachingStrategy.RecycleElement)
            : this(cachingStrategy)
        {
            ItemTemplate = new DataTemplate(cellType);
            GroupDisplayBinding = groupDisplayBinding;
        }

        public GroupedReactiveListView(ListViewCachingStrategy cachingStrategy = ListViewCachingStrategy.RecycleElement)
            : base(cachingStrategy)
        {
            IsGroupingEnabled = true;
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
                    .Select(args => args.Item as T);
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
                    .Select(args => args.SelectedItem as T);
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
    }

    public static class GroupedReactiveListViewExtensions
    {
        public static IDisposable WhenCellActivated(this GroupedReactiveListView reactiveList, Action<CompositeDisposable, Cell, int> whenCellActivated)
        {
            return reactiveList.SetCellActivationAction(whenCellActivated);
        }
    }
}

