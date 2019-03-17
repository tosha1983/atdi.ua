using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Contracts.Sdrn.DeviceServer
{
    public interface ITimeService
    {
        ITimeStamp TimeStamp { get;  }
    }

    public interface ITimeStamp
    {
        long Milliseconds { get; }

        long Value { get; }

        long Ticks { get; }

        bool HitMilliseconds(long startStampMilliseconds, long timeoutMilliseconds);

        bool HitMilliseconds(long startStampMilliseconds, long timeoutMilliseconds, out long lateness);
    }
}
