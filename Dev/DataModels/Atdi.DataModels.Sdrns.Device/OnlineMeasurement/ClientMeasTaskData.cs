using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Device.OnlineMeasurement
{
    [Serializable]
    public class ClientMeasTaskData : ClientData
    {
        public OnlineMeasType OnlineMeasType { get; set; }
        public double FreqStart_MHz { get; set; }
        public double FreqStop_MHz { get; set; }
        public double RBW_kHz { get; set; }
        public double SweepTime_s { get; set; }
        public int Att_dB { get; set; }
        public int PreAmp_dB { get; set; }
        public int RefLevel_dBm { get; set; }
        public DetectorType DetectorType { get; set; }
        public TraceType TraceType { get; set; }
        public int TraceCount { get; set; }
    }
}
