using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.DeviceServer
{
    public class DevicePropertiesBase
    {
        public decimal FreqMin_Hz; 
        public decimal FreqMax_Hz;  
        public int AttMin_dB;  
        public int AttMax_dB; 
        public int PreAmpMin_dB;  
        public int PreAmpMax_dB; 
        public int RefLevelMin_dBm; 
        public int RefLevelMax_dBm; 
    }
}
