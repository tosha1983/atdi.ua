using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Platform.Caching
{
    public class DataCacheSite : IDataCacheSite
    {
        private readonly ConcurrentDictionary<IDataCacheDescriptor, IDataCache> _caches;

        public DataCacheSite()
        {
            this._caches = new ConcurrentDictionary<IDataCacheDescriptor, IDataCache>();
        }

        public IDataCache<TKey, TData> Ensure<TKey, TData>(IDataCacheDescriptor<TKey, TData> descriptor)
        {
            if (!_caches.TryGetValue(descriptor, out IDataCache cache))
            {
                cache = new DataCache<TKey, TData>(descriptor);
                if (!_caches.TryAdd(descriptor, cache))
                {
                    if (!_caches.TryGetValue(descriptor, out cache))
                    {
                        throw new InvalidOperationException("Unable to add cache object data to storage");
                    }
                }
            }

            return (IDataCache<TKey, TData>)cache;
        }
    }
}
