using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Adapters.SignalHound.Enums
{
    public enum DownSampleFactor : int
    {
        MinDecimation = 1,// 2^0
        MaxDecimation = 8192,// 2^13
    }
}
