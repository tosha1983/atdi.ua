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

        private readonly IStatisticCounterKey _getShotsCounterKey;
        private readonly IStatisticCounterKey _getHitsCounterKey;
        private readonly IStatisticCounterKey _getMissesCounterKey;

        private readonly IStatisticCounterKey _setShotsCounterKey;
        private readonly IStatisticCounterKey _setHitsCounterKey;
        private readonly IStatisticCounterKey _setMissesCounterKey;

        private readonly IStatisticCounter _getShotsCounter;
        private readonly IStatisticCounter _getHitsCounter;
        private readonly IStatisticCounter _getMissesCounter;

        private readonly IStatisticCounter _setShotsCounter;
        private readonly IStatisticCounter _setHitsCounter;
        private readonly IStatisticCounter _setMissesCounter;

        private long _getShots = 0;
        private long _getHits = 0;
        private long _getMisses = 0;

        private long _setShots = 0;
        private long _setHits = 0;
        private long _setMisses = 0;

        private Stopwatch _timer;

        public DataCache(IDataCacheDescriptor<TKey, TData> descriptor, IStatistics statistics)
        {
            this._descriptor = descriptor;
            this._cache = new ConcurrentDictionary<TKey, DataCacheEntry<TData>>();
            this._timer = Stopwatch.StartNew();

            this._getShotsCounterKey = STS.DefineCounterKey($"DataCache.{descriptor.Name}.Getting.Shots");
            this._getHitsCounterKey = STS.DefineCounterKey($"DataCache.{descriptor.Name}.Getting.Hits");
            this._getMissesCounterKey = STS.DefineCounterKey($"DataCache.{descriptor.Name}.Getting.Misses");

            this._setShotsCounterKey = STS.DefineCounterKey($"DataCache.{descriptor.Name}.Setting.Shots");
            this._setHitsCounterKey = STS.DefineCounterKey($"DataCache.{descriptor.Name}.Setting.Hits");
            this._setMissesCounterKey = STS.DefineCounterKey($"DataCache.{descriptor.Name}.Setting.Misses");

            if (statistics != null)
            {
                this._getShotsCounter = statistics.Counter(this._getShotsCounterKey);
                this._getHitsCounter = statistics.Counter(this._getHitsCounterKey);
                this._getMissesCounter = statistics.Counter(this._getMissesCounterKey);

                this._setShotsCounter = statistics.Counter(this._setShotsCounterKey);
                this._setHitsCounter = statistics.Counter(this._setHitsCounterKey);
                this._setMissesCounter = statistics.Counter(this._setMissesCounterKey);
            }
            
        }

        public IDataCacheDescriptor Descriptor => _descriptor;

        public void Remove(TKey key)
        {
            _cache.TryRemove(key, out DataCacheEntry<TData> entry);
        }

        public void Set(TKey key, TData data)
        {
            Interlocked.Increment(ref this._setShots);
            _setShotsCounter?.Increment();

            var entry = new DataCacheEntry<TData>
            {
                Data = data,
                ElapsedMilliseconds = _timer.ElapsedMilliseconds
            };

            if (_cache.TryAdd(key, entry))
            {
                Interlocked.Increment(ref this._setHits);
                _setHitsCounter?.Increment();
            }
            else
            {
                Interlocked.Increment(ref this._setMisses);
                _setMissesCounter?.Increment();
            }
        }

        public override string ToString()
        {
            return $"Name = '{_descriptor.Name}' GetShots = {_getShots} GetHits = {_getHits} GetMisses = {_getMisses} SetShots = {_setShots} SetHits = {_setHits}, SetMisses = '{_setMisses}'";
        }

        public bool TryGet(TKey key, out TData data)
        {
            Interlocked.Increment(ref this._getShots);
            _getShotsCounter?.Increment();

            if (_cache.TryGetValue( key, out DataCacheEntry<TData> entry))
            {
                data = entry.Data;
                Interlocked.Exchange(ref entry.ElapsedMilliseconds, _timer.ElapsedMilliseconds);
                Interlocked.Increment(ref entry.Hits);
                Interlocked.Increment(ref this._getHits);
                _getHitsCounter?.Increment();
                return true;
            }

            data = default(TData);
            Interlocked.Increment(ref this._getMisses);
            _getMissesCounter?.Increment();
            return false;
        }

        public bool TrySet(TKey key, ref TData data)
        {
            Interlocked.Increment(ref this._setShots);
            _setShotsCounter?.Increment();

            var entry = new DataCacheEntry<TData>
            {
                Data = data,
                ElapsedMilliseconds = _timer.ElapsedMilliseconds
            };

            if (_cache.TryAdd(key, entry))
            {
                Interlocked.Increment(ref this._setHits);
                _setHitsCounter?.Increment();
                return true;
            }

            Interlocked.Increment(ref this._setMisses);
            _setMissesCounter?.Increment();

            if (_cache.TryGetValue(key, out entry))
            {
                data = entry.Data;
                return false;
            }

            throw new InvalidOperationException("Can not put the data to the cache store");
        }
    }
}
