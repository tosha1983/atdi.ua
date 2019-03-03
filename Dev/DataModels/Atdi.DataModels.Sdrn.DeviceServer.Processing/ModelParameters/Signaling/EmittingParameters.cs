using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.DeviceServer.Processing
{
    public struct EmittingParameters
    {
        public double RollOffFactor; // from 0.85 to 1.35
        public double StandardBW; // or channel BW
    }
}
