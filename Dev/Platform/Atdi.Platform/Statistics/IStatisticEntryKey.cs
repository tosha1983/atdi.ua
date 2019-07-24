using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Platform
{
    public interface IStatisticEntryKey
    {
        string Name { get; }

        Type Type { get; }
    }

    public interface IStatisticEntryKey<T> : IStatisticEntryKey
    {

    }
}
