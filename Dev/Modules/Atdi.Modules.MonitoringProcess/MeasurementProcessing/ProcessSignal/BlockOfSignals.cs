using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.SDR.Server.MeasurementProcessing.SingleHound.ProcessSignal
{
    class BlockOfSignal
    {
        public float[] IQStream;
        public int StartIndexIQ;
        public Double Durationmks; //длительность блока в микросекундах
    }
}
