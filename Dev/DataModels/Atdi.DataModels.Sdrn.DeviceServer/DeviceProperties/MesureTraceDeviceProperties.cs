using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.DeviceServer
{
    public class MesureTraceDeviceProperties
    {
        public double RBWMin_Hz; 
        public double RBWMax_Hz; 
        public decimal FreqMin_Hz; // mandatory 
        public decimal FreqMax_Hz; // mandatory 
        public double SweepTimeMin_s; //-1 = auto
        public double SweepTimeMax_s; //-1 = auto
        public int AttMin_dB; //-1 = auto, 
        public int AttMax_dB; //-1 = auto, 
        public int PreAmpMin_dB; //-1 = auto, 
        public int PreAmpMax_dB; //-1 = auto, 
        public int RefLevelMin_dBm; // -1 = auto
        public int RefLevelMax_dBm; // -1 = auto
    }
}
