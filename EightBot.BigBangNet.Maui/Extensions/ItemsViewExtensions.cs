using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using EightBot.BigBang.Maui.Views;
using ReactiveUI;
using Microsoft.Maui;

namespace EightBot.BigBang.Maui
{
    public static class ItemsViewExtensions
    {
        public static IDisposable Bind<TVisual>(this ItemsView<TVisual> itemsView, IObservable<IEnumerable> listItems)
            where TVisual : BindableObject
        {
            var bindingDisposables = new CompositeDisposable();

            listItems
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(items =>
                {
                    if (itemsView == null)
                        return;

                    //MTS: I unfortunately think this still happens often enough that we need to null it out first
                    itemsView.ItemsSource = null;
                    itemsView.ItemsSource = items;
                })
                .DisposeWith(bindingDisposables);

            return Disposable.Create(() =>
            {
                if (itemsView != null)
                    itemsView.ItemsSource = null;

                bindingDisposables?.Dispose();
            });
        }

        public static IDisposable Bind(this ItemsView itemsView, IObservable<IEnumerable> listItems)
        {
            var bindingDisposables = new CompositeDisposable();

            listItems
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(items =>
                {
                    if (itemsView == null)
                        return;

                    //MTS: I unfortunately think this still happens often enough that we need to null it out first
                    itemsView.ItemsSource = null;
                    itemsView.ItemsSource = items;
                })
                .DisposeWith(bindingDisposables);

            return Disposable.Create(() =>
            {
                if (itemsView != null)
                    itemsView.ItemsSource = null;

                bindingDisposables?.Dispose();
            });
        }
    }
}
