using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.DeviceServer.Processing
{
    [Serializable]
    public class BWResult 
    {
        public int T1;
        public int T2; 
        public int MarkerIndex;
        public double Bandwidth_kHz;
        public bool СorrectnessEstimations;
        public int TraceCount;
        public double[] Freq_Hz;
        public float[] Levels_dBm;
        public DateTime TimeMeas;
    }
}
