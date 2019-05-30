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

        long TimeCorrection { get; set; }

        /// <summary>
        /// возвращается текущее системное время плюс поправка
        /// </summary>
        /// <returns></returns>
        DateTime GetGnssTime();

        /// <summary>
        /// возвращается текущее системное время по UTC плюс поправка
        /// </summary>
        /// <returns></returns>
        DateTime GetGnssUtcTime();



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
