using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Platform
{
    class StatisticCounter : IStatisticCounter
    {
        private long _data;
        public StatisticCounter(IStatisticCounterKey entryKey)

        {
            this.Key = entryKey;
        }

        public IStatisticCounterKey Key { get; }

        public long Data => _data;

        public void Change(long newValue)
        {
            System.Threading.Interlocked.Exchange(ref _data, newValue);
        }


        public void Decrement()
        {
            System.Threading.Interlocked.Decrement(ref _data);
        }

        public void Increment()
        {
            System.Threading.Interlocked.Increment(ref _data);
        }

        public void Reset()
        {
            this.Change(default(long));
        }

        public override string ToString()
        {
            return $"Key = '{Key.Name}', Data = '{this.Data}'";
        }
    }
}
