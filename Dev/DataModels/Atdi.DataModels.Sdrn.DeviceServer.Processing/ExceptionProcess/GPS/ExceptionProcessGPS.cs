﻿using Atdi.DataModels.Sdrns.Device;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrn.DeviceServer.Commands.Parameters;

namespace Atdi.DataModels.Sdrn.DeviceServer.Processing
{
    public class ExceptionProcessGPS
    {
        public readonly CommandFailureReason _failureReason;
        public readonly Exception _ex;

        public ExceptionProcessGPS()
        {

        }
        public ExceptionProcessGPS(CommandFailureReason failureReason, Exception e)
        {
            this._failureReason = failureReason;
            this._ex = e;
        }
    }
}
