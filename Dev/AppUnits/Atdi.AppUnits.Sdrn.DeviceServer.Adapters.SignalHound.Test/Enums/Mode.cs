using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Adapters.SignalHound.Enums
{
    public enum Mode : uint
    {
        Sweeping = 0x0,
        RealTime = 0x1,
        Streaming = 0x4,
        AudioDemod = 0x7,
        TGSweeping = 0x8,
    }
}
