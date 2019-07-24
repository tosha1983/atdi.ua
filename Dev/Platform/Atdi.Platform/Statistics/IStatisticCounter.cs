using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Platform
{
    public interface IStatisticCounter
    {
        IStatisticCounterKey Key { get; }

        long Data { get; }

        void Reset();

        void Increment();

        void Decrement();

        void Change(long newValue);
    }
}
