using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Device.OnlineMeasurement
{
    [Serializable]
    public class DeviceServerParametersDataLevel : DeviceServerData
    {
        public double MinFreq_MHz { get; set; }
        public double MaxFreq_MHz { get; set; }
        public double RBW_kHz { get; set; }
        public double SweepTime_s { get; set; }
        public int RfAttenuation_dB { get; set; }
        public int Preamplification_dB { get; set; }
        public int RefLevel_dBm { get; set; }
        public DetectorType DetectorType { get; set; }
        public TraceType TraceType { get; set; }
        public int TraceCount { get; set; }
    }
}
