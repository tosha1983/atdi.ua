﻿using Atdi.Contracts.Sdrn.DeviceServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Atdi.AppUnits.Sdrn.DeviceServer
{
    class TimeService : ITimeService
    {
        private readonly ITimeStamp _timeStamp;
        private long _timeCorrection;

        public TimeService()
        {
            this._timeStamp = new TimeStamp();
        }

        public ITimeStamp TimeStamp => this._timeStamp;

        public long TimeCorrection
        {
            get
            {
                return Interlocked.Read(ref this._timeCorrection);
            }
            set
            {
                Interlocked.Exchange(ref this._timeCorrection, value);
            }
        }

        public DateTime GetGnssTime()
        {
            ///TODO: Необходимо дописать код учитывающий поправку времени относительно GPS
            return DateTime.Now;
        }

        public DateTime GetGnssUtcTime()
        {
            ///TODO: Необходимо дописать код учитывающий поправку времени относительно GPS
            return DateTime.UtcNow;
        }
    }
}
