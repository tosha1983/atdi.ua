using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.DeviceServer
{
    [Flags]
    public enum TaskExecutionOption
    {
        Default = 0,
        Synchronously = 1,
        RunDelayed = 2
    }
}