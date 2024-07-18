using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using EightBot.BigBang.ViewModel;
using EightBot.BigBang.XamForms.Pages;
using ReactiveUI;
using Xamarin.Forms;

namespace EightBot.BigBang.XamForms.Views
{
	public class LoadingReactiveListView : Grid
	{
		Type _cellType;

		public ActivityIndicator LoadingIndicator { get; private set; }
		public ReactiveListView ListView { get; private set; }

        public LoadingReactiveListView(Type cellType, ListViewCachingStrategy cachingStrategy = ListViewCachingStrategy.RecycleElement)
        {
            _cellType = cellType;

            RowSpacing = 0;
            ColumnSpacing = 0;
            Margin = 0;
            Padding = 0;

            ListView = new ReactiveListView(_cellType, cachingStrategy)
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                IsPullToRefreshEnabled = true,
            };

            LoadingIndicator = new ActivityIndicator
            {
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalOptions = LayoutOptions.CenterAndExpand
            };

            this.Children.Add(ListView, 0, 0);
            this.Children.Add(LoadingIndicator, 0, 0);
        }

        public LoadingReactiveListView(ListViewCachingStrategy cachingStrategy = ListViewCachingStrategy.RecycleElement)
		{
            RowSpacing = 0;
            ColumnSpacing = 0;
            Margin = 0;
            Padding = 0;

			ListView = new ReactiveListView(cachingStrategy)
			{
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.FillAndExpand,
				IsPullToRefreshEnabled = true,
			};

			LoadingIndicator = new ActivityIndicator { 
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalOptions = LayoutOptions.CenterAndExpand
            };

			this.Children.Add(ListView, 0, 0);
			this.Children.Add(LoadingIndicator, 0, 0);
		}
	}

	public static class LoadingTableViewExtensions
	{
        public static IDisposable Bind<TViewModel>(this LoadingReactiveListView loadingTableView, IObservable<IEnumerable<TViewModel>> listItems, IObservable<bool> loadingChanged)
		{
			var disposables = new CompositeDisposable();

			loadingTableView.ListView
                .Bind(listItems)
				.DisposeWith(disposables);

			loadingChanged
				.ObserveOn(RxApp.MainThreadScheduler)
				.BindTo(loadingTableView.LoadingIndicator, c => c.IsRunning)
				.DisposeWith(disposables);

            loadingTableView.ListView.IsPullToRefreshEnabled = false;
			
			return disposables;
		}

        public static IDisposable Bind<TViewModel, TParam, TResult>(this LoadingReactiveListView loadingTableView, IObservable<IEnumerable<TViewModel>> listItems, IObservable<bool> loadingChanged, IObservable<ReactiveCommand<TParam, TResult>> reload = null)
        {
            var disposables = new CompositeDisposable();

            loadingTableView.ListView
                .Bind(listItems)
                .DisposeWith(disposables);

            loadingChanged
                .ObserveOn(RxApp.MainThreadScheduler)
                .BindTo(loadingTableView.LoadingIndicator, c => c.IsRunning)
                .DisposeWith(disposables);

            if (reload != null)
                reload
                    .IsNotNull()
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .BindTo(loadingTableView.ListView, c => c.RefreshCommand)
                    .DisposeWith(disposables);
            else
                loadingTableView.ListView.IsPullToRefreshEnabled = false;

            return disposables;
        }
    }
}

