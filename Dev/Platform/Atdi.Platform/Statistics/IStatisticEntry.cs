using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Platform
{
    public interface IStatisticEntry
    {
        IStatisticEntryKey Key { get; }

        object GetData();
    }

    public interface IStatisticEntry<T> : IStatisticEntry
    {
        T Data { get; }

        void Set(T newValue);
    }
}
