using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Adapters.SignalHound.Enums
{
    public enum Scale : uint
    {
        LogScale = 0x0,
        LinScale = 0x1,
        LogFullScale = 0x2,
        LinFullScale = 0x3,
    }
}
