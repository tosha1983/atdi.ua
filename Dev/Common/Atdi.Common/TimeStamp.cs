using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Common
{
    public static class TimeStamp
    {
        private static readonly double TickFrequency;

        static TimeStamp()
        {
            if (Stopwatch.IsHighResolution)
            {
                TickFrequency = 10000000.0;
                TickFrequency /= (double)Stopwatch.Frequency;
            }
            else
            {
                TickFrequency = 1.0;
            }
        }
        /// <summary>
        /// Gets the time stamp in milliseconds
        /// </summary>
        /// <returns></returns>
        public static long Milliseconds
        {
            get
            {
                return TimeStamp.Ticks / 10000L;
            }
        }


        public static long Value
        {
            get
            {
                return Stopwatch.GetTimestamp();
            }
        }

        public static bool HitTimeout(long startStampMilliseconds, long timeoutMilliseconds)
        {
            var pasedTime = TimeStamp.Milliseconds - startStampMilliseconds;
            var delta = timeoutMilliseconds - pasedTime;
            return delta > 0 ;
        }


        public static long Ticks
        {
            get
            {
                var rawElapsedTicks = Stopwatch.GetTimestamp();
                if (Stopwatch.IsHighResolution)
                {
                    var result = (double)rawElapsedTicks;
                    result *= TickFrequency;
                    return (long)result;
                }
                return rawElapsedTicks;
            }
            
        }

        
    }
}
