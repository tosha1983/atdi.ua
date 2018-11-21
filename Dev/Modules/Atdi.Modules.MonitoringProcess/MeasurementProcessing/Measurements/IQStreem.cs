using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.SDR.Server.MeasurementProcessing.Measurement
{
    public class IQStreem
    {
        public float[] iqSemples;
        public int[] trigger;
        public IQStreem(ISDR SDR)
        {
            SDR.GetIQStream(ref iqSemples, ref trigger);
        }
    }
}
