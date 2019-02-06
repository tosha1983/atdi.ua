using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Adapters.SignalHound.EN
{
    public enum Unit : uint
    {
        Log = 0x0,
        Voltage = 0x1,
        Power = 0x2,
        Sample = 0x3,
    }
}
