using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;

namespace TestApp
{
    public class InMemoryCache : IInMemoryCache
    {
        #region IInMemoryCache Members

        public T Get<T>(string cacheId, Func<T> getItemCallback) where T : class
        {
            var item = HttpRuntime.Cache.Get(cacheId) as T;
            if (item == null)
            {
                item = getItemCallback();
                HttpContext.Current.Cache.Insert(cacheId, item, null,
                                                 DateTime.UtcNow.AddMinutes(60.0),
                                                 System.Web.Caching.Cache.NoSlidingExpiration,
                                                 CacheItemPriority.Normal, null);
            }
            return item;
        }

        public void Remove(string key)
        {
            HttpRuntime.Cache.Remove(key);
        }

        #endregion
    }
}