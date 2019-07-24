using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Platform
{
    class StatisticEntry<T> : IStatisticEntry<T>
    {
        protected T _data;

        public StatisticEntry(IStatisticEntryKey entryKey)
        {
            this.Key = entryKey;
        }

        public T Data => _data;

        public IStatisticEntryKey Key { get; }


        public object GetData()
        {
            return this._data;
        }

        public void Set(T newValue)
        {
            this._data = newValue;
        }

        public override string ToString()
        {
            return $"Key = '{Key.Name}', Data = '{this.Data}'";
        }
    }
}
