using System;
using System.Collections.Generic;

namespace EightBot.BigBang
{
    public static class IEnumerableExtensions
    {
        public static IEnumerable<TSource> DistinctBy<TSource, TKey>
        (this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            HashSet<TKey> seenKeys = new HashSet<TKey>();
            foreach (TSource element in source)
            {
                if (seenKeys.Add(keySelector(element)))
                {
                    yield return element;
                }
            }
        }

        ///<summary>Finds the index of the first item matching an expression in an enumerable.</summary>
        ///<param name="items">The enumerable to search.</param>
        ///<param name="predicate">The expression to test the items against.</param>
        ///<returns>The index of the first matching item, or -1 if no items match.</returns>
        public static int FindIndex<T>(this IEnumerable<T> items, Func<T, bool> predicate)
        {
            if (items == null) throw new ArgumentNullException("items");
            if (predicate == null) throw new ArgumentNullException("predicate");

            int retVal = 0;
            foreach (var item in items)
            {
                if (predicate(item)) return retVal;
                retVal++;
            }
            return -1;
        }
        ///<summary>Finds the index of the first occurence of an item in an enumerable.</summary>
        ///<param name="items">The enumerable to search.</param>
        ///<param name="item">The item to find.</param>
        ///<returns>The index of the first matching item, or -1 if the item was not found.</returns>
        public static int IndexOf<T>(this IEnumerable<T> items, T item) { return items.FindIndex(i => EqualityComparer<T>.Default.Equals(item, i)); }

        public static void ForEach<T>(this IEnumerable<T> enumeration, Action<T> action)
        {
            foreach (T item in enumeration)
            {
                action(item);
            }
        }

        /// <summary>Checks whether the key matching an expression is contained in an enumerable.</summary>
        /// <param name="source">The enumerable to search.</param>
        /// <param name="key">The key to find.</param>
        /// <param name="keySelector">The expression to select the key to test against.</param>
        /// <returns><c>true</c>, if the key was contained, <c>false</c> otherwise.</returns>
        public static bool ContainsBy<T, TKey>(this IEnumerable<T> source, TKey key, Func<T, TKey> keySelector)
        {
            if (keySelector == null) throw new ArgumentNullException("keySelector");

            TKey itemKey;
            foreach (var item in source)
            {
                itemKey = keySelector(item);
                if ((key == null && itemKey == null) || (key != null && key.Equals(itemKey)))
                    return true;
            }

            return false;
        }
    }
}

