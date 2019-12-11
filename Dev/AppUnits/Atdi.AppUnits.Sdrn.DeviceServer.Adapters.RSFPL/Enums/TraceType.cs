using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Adapters.RSFPL.Enums
{
    public enum TraceType : int
    {
        ClearWrite = 0,
        Average = 1,
        MaxHold = 2,
        MinHold = 3,
        View = 4,
        Blank = 5,
    }
}
