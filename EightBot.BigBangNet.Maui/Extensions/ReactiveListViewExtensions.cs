﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using EightBot.BigBang.XamForms.Views;
using ReactiveUI;
using Xamarin.Forms;

namespace EightBot.BigBang.XamForms
{
    public static class ReactiveListViewExtensions
    {
        public static IDisposable Bind(this ReactiveListView listView, IObservable<IEnumerable> listItems, IObservable<bool> loadingChanged)
        {
            var bindingDisposables = new CompositeDisposable();

            listView
                .Bind(listItems)
                .DisposeWith(bindingDisposables);

            loadingChanged
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(executing => listView.IsRefreshing = executing)
                .DisposeWith(bindingDisposables);

            return bindingDisposables;
        }

        public static IDisposable Bind<TParam, TResult>(this ReactiveListView listView, IObservable<IEnumerable> listItems, IObservable<bool> loadingChanged, IObservable<ReactiveCommand<TParam, TResult>> reload = null)
        {
            var bindingDisposables = new CompositeDisposable();
            var commandDisposable = new SerialDisposable();

            listView
                .Bind(listItems)
                .DisposeWith(bindingDisposables);

            loadingChanged
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(executing => listView.IsRefreshing = executing)
                .DisposeWith(bindingDisposables);

            reload
                .DistinctUntilChanged()
                .IsNotNull()
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(command =>
                {
                    listView.IsPullToRefreshEnabled = true;

                    listView.WhenAnyValue(x => x.IsRefreshing)
                        .DistinctUntilChanged()
                        .WhereIsTrue()
                        .InvokeCommand(command)
                        .DisposeWith(commandDisposable);
                })
                .DisposeWith(bindingDisposables);

            return Disposable.Create(() =>
            {
                bindingDisposables?.Dispose();
                commandDisposable?.Dispose();
            });
        }
    }
}
