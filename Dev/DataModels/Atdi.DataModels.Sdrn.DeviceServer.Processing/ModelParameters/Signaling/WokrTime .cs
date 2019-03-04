using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.DeviceServer.Processing
{
    public struct WokrTime
    {
        public DateTime StartEmitting;
        public DateTime StopEmitting;
        public int HitCount;
        public float PersentAvailability;
    }
}
