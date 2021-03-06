﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Device.OnlineMeasurement
{
    public enum FailureReason
    {
        NotFoundDevice,
        NotFoundConvertor,
        DeviceIsBusy,
        TimeoutExpired,
        CanceledBeforeExecution,
        CanceledExecution,
        Exception,
        ExecutionCompleted
    }
}
