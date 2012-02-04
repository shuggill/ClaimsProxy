using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TestApp
{
    public interface IInMemoryCache
    {
        T Get<T>(string cacheId, Func<T> getItemCallback) where T : class;

        void Remove(string key);
    }
}