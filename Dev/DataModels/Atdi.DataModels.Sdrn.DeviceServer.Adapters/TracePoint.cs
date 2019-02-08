using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.DeviceServer.Adapters
{
    public class TracePoint
    {
        public TracePoint()
        {
        }
        public decimal Freq { get; set; }
        public double Level { get; set; }
    }
}
