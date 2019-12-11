using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Adapters.RSFPL.Enums
{
    public enum TraceDetector : int
    {
        AutoSelect = 0,
        AutoPeak = 1,
        Average = 2,
        MaxPeak = 3,
        MinPeak = 4,
        Sample = 5,
        Normal = 6,
        RMS = 7,
    }
}
