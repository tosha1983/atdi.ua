using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Adapters.SignalHound.EN
{
    public enum Port1 : uint
    {
        ACCoupled = 0x00,
        DCCoupled = 0x04,
        IntRefOut = 0x00,
        ExtRefIn = 0x08,
        OutACLoad = 0x10,
        OutLogicLow = 0x14,
        OutLogicHigh = 0x1C,
    }
}
