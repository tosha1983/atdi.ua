using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Platform
{
    public class Statistics : IStatistics
    {
        private readonly ConcurrentDictionary<IStatisticEntryKey, IStatisticEntry> _entries;
        private readonly ConcurrentDictionary<IStatisticCounterKey, IStatisticCounter> _counters;

        

        public Statistics()
        {
            this._entries = new ConcurrentDictionary<IStatisticEntryKey, IStatisticEntry>();
            this._counters = new ConcurrentDictionary<IStatisticCounterKey, IStatisticCounter>();
        }

        public IStatisticCounter Counter(IStatisticCounterKey entryKey)
        {
            if (!_counters.TryGetValue(entryKey, out IStatisticCounter value))
            {
                value = new StatisticCounter(entryKey);

                if (!_counters.TryAdd(entryKey, value))
                {
                    if (!_counters.TryGetValue(entryKey, out value))
                    {
                        throw new InvalidOperationException($"Can not create a statistic counter value");
                    }
                }
            }
            return value;
        }

        public IStatisticCounter[] GetCounters()
        {
            return _counters.Values.ToArray();
        }

        public IStatisticEntry[] GetEntries()
        {
            return _entries.Values.ToArray();
        }

        public IStatisticEntry<T> Entry<T>(IStatisticEntryKey<T> key)
        {
            if (!_entries.TryGetValue(key, out IStatisticEntry value))
            {
                value = new StatisticEntry<T>(key);
                if (!_entries.TryAdd(key, value))
                {
                    if (!_entries.TryGetValue(key, out value))
                    {
                        throw new InvalidOperationException($"Can not create a statistic value");
                    }
                }
            }
            return (IStatisticEntry<T>)value;
        }

        public void Set<T>(IStatisticEntryKey<T> entryKey, T newValue)
        {
            this.Entry(entryKey).Set(newValue);
        }

        public void Reset(IStatisticCounterKey entryKey)
        {
            this.Counter(entryKey).Reset();
        }

        public void Increment(IStatisticCounterKey entryKey)
        {
            this.Counter(entryKey).Increment();
        }

        public void Decrement(IStatisticCounterKey entryKey)
        {
            this.Counter(entryKey).Decrement();
        }

        public void Change(IStatisticCounterKey entryKey, long newValue)
        {
            this.Counter(entryKey).Change(newValue);
        }

        public IStatisticEntryKey[] GetEntryKeys()
        {
            return _entries.Keys.ToArray();
        }

        public IStatisticCounterKey[] GetCounterKeys()
        {
            return _counters.Keys.ToArray();
        }
    }
}
