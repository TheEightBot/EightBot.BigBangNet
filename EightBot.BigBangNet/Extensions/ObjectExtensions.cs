using System.Linq.Expressions;
using System;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.ComponentModel;
using System.Reactive.Linq;
using System.Reactive.Concurrency;
using System.Linq;

namespace EightBot.BigBang
{
	public static class ObjectExtensions
	{
		public static string GetMemberName<T>(Expression<Func<T>> memberExpression)
		{
			MemberExpression expressionBody = (MemberExpression)memberExpression.Body;
			return expressionBody.Member.Name;
		}
        
        public static T Clamp<T>(this T val, T min, T max) where T : IComparable<T>
        {
            if (val.CompareTo(min) < 0) return min;
            else if(val.CompareTo(max) > 0) return max;
            else return val;
        }


        public static IObservable<string> ObservePropertyChanged<T>(this T obj, params string[] propertyNamesToWatch) where T : INotifyPropertyChanged {
            return ObservePropertyChanged(obj, null, propertyNamesToWatch);
        }

        public static IObservable<string> ObservePropertyChanged<T>(this T obj, IScheduler scheduler, params string[] propertyNamesToWatch) where T : INotifyPropertyChanged
        {
            var obs = Observable
                .FromEvent<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                    eventHandler =>
                    {
                        void Handler(object sender, PropertyChangedEventArgs e) => eventHandler?.Invoke(e);
                        return Handler;
                    },
                    x => obj.PropertyChanged += x,
                    x => obj.PropertyChanged -= x,
                    scheduler ?? CurrentThreadScheduler.Instance)
                .Select(x => x.PropertyName);

            if(propertyNamesToWatch?.Any() ?? false){
                obs.Where(x => propertyNamesToWatch.Contains(x));  
            }

            return obs;
        }

        public static IObservable<string> ObservePropertyChanging<T>(this T obj, params string[] propertyNamesToWatch) where T : INotifyPropertyChanging
        {
            return ObservePropertyChanging(obj, null, propertyNamesToWatch);
        }

        public static IObservable<string> ObservePropertyChanging<T>(this T obj, IScheduler scheduler = null, params string[] propertyNamesToWatch) where T : INotifyPropertyChanging
        {
            var obs = Observable
                .FromEvent<PropertyChangingEventHandler, PropertyChangingEventArgs>(
                    eventHandler =>
                    {
                        void Handler(object sender, PropertyChangingEventArgs e) => eventHandler?.Invoke(e);
                        return Handler;
                    },
                    x => obj.PropertyChanging += x,
                    x => obj.PropertyChanging -= x,
                    scheduler ?? CurrentThreadScheduler.Instance)
                .Select(x => x.PropertyName);

            if (propertyNamesToWatch?.Any() ?? false)
            {
                obs.Where(x => propertyNamesToWatch.Contains(x));
            }

            return obs;
        }
	}
}

