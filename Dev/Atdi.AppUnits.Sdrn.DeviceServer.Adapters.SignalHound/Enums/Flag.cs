using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Adapters.SignalHound.EN
{
    public enum Flag : uint
    {
        StreamIQ = 0x0,
        StreamIF = 0x1,
        DirectRF = 0x2,
        TimeStamp = 0x10,
    }
}
