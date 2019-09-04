using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Device.OnlineMeasurement
{
    [Serializable]
    public class DeviceServerParametersDataLevel : DeviceServerParametersData
    {
        public double[] Freq_Hz { get; set; }
        public double RBW_kHz { get; set; }
        public bool isChanged_RBW_kHz { get; set; }
        public int Att_dB { get; set; }
        public bool isChanged_Att_dB { get; set; }
        public int PreAmp_dB { get; set; }
        public bool isChanged_PreAmp_dB { get; set; }
        public int RefLevel_dBm { get; set; }
        public bool isChanged_RefLevel_dBm { get; set; }
    }
}
