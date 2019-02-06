using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Adapters.SignalHound.EN
{
    public enum Rejection : uint
    {
        NoSpurReject = 0x0,
        SpurReject = 0x1,
    }
}
