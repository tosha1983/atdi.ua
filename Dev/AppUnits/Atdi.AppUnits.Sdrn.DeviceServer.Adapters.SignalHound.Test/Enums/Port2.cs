using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Adapters.SignalHound.Enums
{
    public enum Port2 : uint
    {
        OutLogicLow = 0x00,
        OutLogicHigh = 0x20,
        InTriggerRisingEdge = 0x40,
        InTriggerFallingEdge = 0x60,
    }
}
