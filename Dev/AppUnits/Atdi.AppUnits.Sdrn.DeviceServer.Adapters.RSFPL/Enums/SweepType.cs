using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Adapters.RSFPL.Enums
{
    public enum SweepType : int
    {
        Auto = 0,
        Sweep = 1,
        FFT = 2,
        Fast = 3,
        Performance = 4,
        NoFFT = 5
    }
}
