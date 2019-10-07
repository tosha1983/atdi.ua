using Atdi.DataModels.Sdrns.Device.OnlineMeasurement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XICSM.ICSControlClient
{
    public class OnlineMeasurementParameters
    {
        public double? SweepTime_s { get; set; }
        public int? Att_dB { get; set; }
        public int? PreAmp_dB { get; set; }
        public int? RefLevel_dBm { get; set; }
        public double? FreqStart_MHz { get; set; }
        public double? FreqStop_MHz { get; set; }
        public DetectorType DetectorType { get; set; }
    }
}
