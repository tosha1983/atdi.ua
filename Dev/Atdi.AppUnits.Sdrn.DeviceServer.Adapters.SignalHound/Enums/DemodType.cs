using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Adapters.SignalHound.EN
{
    public enum DemodType : int
    {
        AM = 0x0,
        FM = 0x1,
        USB = 0x2,
        LSB = 0x3,
        CW = 0x4,
    }
}
