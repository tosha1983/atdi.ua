using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.CalcServer.Internal.Iterations
{
    [Serializable]
    public struct PercentTimeForGainCalcData
    {
        public ReceivedPowerCalcResult[] StationData;
        public long SensorId;
        public double SensorAntennaHeight_m;   
    }
}
