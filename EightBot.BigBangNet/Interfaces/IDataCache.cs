using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EightBot.BigBang.Interfaces
{
    public interface IDataCache
    {
        Task StoreItemsToCache<T>(IEnumerable<T> items, string cacheKey = null, string groupKey = null);

        Task StoreItemsToCache<T>(IEnumerable<T> items, Func<T, string> cacheKey = null, string groupKey = null);

        Task StoreItemToCache<T>(T item, string cacheKey = null, string groupKey = null);

        Task StoreItemToCache<T>(T item, Func<T, string> cacheKey = null, string groupKey = null);

        Task<IEnumerable<T>> GetItemsFromCache<T>(string cacheKey = null, string groupKey = null);

        Task<T> GetItemFromCache<T>(string cacheKey = null, string groupKey = null);

        Task<bool> RemoveItemFromCache<T>(string cacheKey = null, string groupKey = null);

        Task ClearCache(string groupKey = null);
    }
}
