using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.DeviceServer
{
    public enum CommandFailureReason
    {
        NotFoundDevice,
        NotFoundConvertor,
        DeviceIsBusy,
        TimeoutExpired,
        CanceledBeforeExecution,
        CanceledExecution,
        Exception
    }
}
