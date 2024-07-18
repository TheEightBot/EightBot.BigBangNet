using System;
using System.Collections.Specialized;
using System.Reactive.Linq;
using System.ComponentModel;

namespace EightBot.BigBang
{
    public static class ChangeNotificationExtensions
    {
        public static IObservable<NotifyCollectionChangedEventArgs> ObserveCollectionChanged<T>(this T collection)
            where T : class, INotifyCollectionChanged
        {
            if (collection == null)
                return Observable.Empty<NotifyCollectionChangedEventArgs>();

            return Observable
                .FromEvent<NotifyCollectionChangedEventHandler, NotifyCollectionChangedEventArgs>(
                    eventHandler =>
                    {
                        void Handler(object sender, NotifyCollectionChangedEventArgs e) => eventHandler?.Invoke(e);
                        return Handler;
                    },
                    x => collection.CollectionChanged += x,
                    x => collection.CollectionChanged -= x);
        }
        
        public static IObservable<PropertyChangedEventArgs> ObservePropertyChanged<T>(this T obj)
            where T : class, INotifyPropertyChanged
        {
            if (obj == null)
                return Observable.Empty<PropertyChangedEventArgs>();

            return Observable
                .FromEvent<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                    eventHandler =>
                    {
                        void Handler(object sender, PropertyChangedEventArgs e) => eventHandler?.Invoke(e);
                        return Handler;
                    },
                    x => obj.PropertyChanged += x,
                    x => obj.PropertyChanged -= x);
        }
        
        public static IObservable<PropertyChangingEventArgs> ObservePropertyChanging<T>(this T obj)
            where T : class, INotifyPropertyChanging
        {
            if (obj == null)
                return Observable.Empty<PropertyChangingEventArgs>();

            return Observable
                .FromEvent<PropertyChangingEventHandler, PropertyChangingEventArgs>(
                    eventHandler =>
                    {
                        void Handler(object sender, PropertyChangingEventArgs e) => eventHandler?.Invoke(e);
                        return Handler;
                    },
                    x => obj.PropertyChanging += x,
                    x => obj.PropertyChanging -= x);
        }
    }
}
