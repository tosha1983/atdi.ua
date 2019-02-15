using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.DeviceServer.Commands.Parameters
{
    public class MesureTraceParameter
    {
        public double RBW_Hz; // -1 = auto; 
        public double VBW_Hz; // -1 = auto; 
        public decimal FreqStart_Hz; // mandatory 
        public decimal FreqStop_Hz;  // mandatory 
        public double SweepTime_s; //-1 = auto
        public int Att_dB; //-1 = auto, 
        public int PreAmp_dB; //-1 = auto, 
        public int RefLevel_dBm; // -1 = auto
        public DetectorType DetectorType; //
        public TraceType TraceType; // 
        public int TraceCount; // количество трейсов  // mandatory 
        public int TracePoint; // -1 = auto; 
        public LevelUnit LevelUnit;
    }
}
