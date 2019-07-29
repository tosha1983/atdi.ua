using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;

namespace Atdi.Platform.Caching
{
    class DataCache<TKey, TData> : IDataCache<TKey, TData>
    {
        private readonly IDataCacheDescriptor<TKey, TData> _descriptor;
        private readonly ConcurrentDictionary<TKey, DataCacheEntry<TData>> _cache;

        private long _getShots = 0;
        private long _getHits = 0;
        private long _getMisses = 0;

        private long _setShots = 0;
        private long _setHits = 0;
        private long _setMisses = 0;

        private Stopwatch _timer;

        public DataCache(IDataCacheDescriptor<TKey, TData> descriptor)
        {
            this._descriptor = descriptor;
            this._cache = new ConcurrentDictionary<TKey, DataCacheEntry<TData>>();
            this._timer = Stopwatch.StartNew();
        }

        public IDataCacheDescriptor Descriptor => _descriptor;

        public void Remove(TKey key)
        {
            _cache.TryRemove(key, out DataCacheEntry<TData> entry);
        }

        public void Set(TKey key, TData data)
        {
            Interlocked.Increment(ref this._setShots);

            var entry = new DataCacheEntry<TData>
            {
                Data = data,
                ElapsedMilliseconds = _timer.ElapsedMilliseconds
            };

            if (_cache.TryAdd(key, entry))
            {
                Interlocked.Increment(ref this._setHits);
            }
            else
            {
                Interlocked.Increment(ref this._setMisses);
            }
        }

        public override string ToString()
        {
            return $"Name = '{_descriptor.Name}' GetShots = {_getShots} GetHits = {_getHits} GetMisses = {_getMisses} SetShots = {_setShots} SetHits = {_setHits}, SetMisses = '{_setMisses}'";
        }

        public bool TryGet(TKey key, out TData data)
        {
            Interlocked.Increment(ref this._getShots);

            if (_cache.TryGetValue( key, out DataCacheEntry<TData> entry))
            {
                data = entry.Data;
                Interlocked.Exchange(ref entry.ElapsedMilliseconds, _timer.ElapsedMilliseconds);
                Interlocked.Increment(ref entry.Hits);
                Interlocked.Increment(ref this._getHits);
                return true;
            }

            data = default(TData);
            Interlocked.Increment(ref this._getMisses);
            return false;
        }

        public bool TrySet(TKey key, ref TData data)
        {
            Interlocked.Increment(ref this._setShots);

            var entry = new DataCacheEntry<TData>
            {
                Data = data,
                ElapsedMilliseconds = _timer.ElapsedMilliseconds
            };

            if (_cache.TryAdd(key, entry))
            {
                Interlocked.Increment(ref this._setHits);
                return true;
            }

            Interlocked.Increment(ref this._setMisses);

            if (_cache.TryGetValue(key, out entry))
            {
                data = entry.Data;
                return false;
            }

            throw new InvalidOperationException("Can not put the data to the cache store");
        }
    }
}
