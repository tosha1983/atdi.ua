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
        private readonly IStatistics _statistics;
        
        public DataCacheSite(IStatistics statistics)
        {
            this._caches = new ConcurrentDictionary<IDataCacheDescriptor, IDataCache>();
            this._statistics = statistics;
        }

        public IDataCache<TKey, TData> Ensure<TKey, TData>(IDataCacheDescriptor<TKey, TData> descriptor)
        {
            if (!_caches.TryGetValue(descriptor, out IDataCache cache))
            {
                cache = new DataCache<TKey, TData>(descriptor, _statistics);
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
