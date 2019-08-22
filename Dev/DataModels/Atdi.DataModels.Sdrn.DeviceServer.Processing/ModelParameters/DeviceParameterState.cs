using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.DeviceServer.Processing
{
    [Serializable]
    public class DeviceParameterState
    {
        public int RefLevel_dBm;//Возвращает в любом случаее текущий установленный
        public int Att_dB;//Возвращает в любом случаее текущий установленный
        public int PreAmp_dB;//Возвращает в любом случаее текущий установленный
        public double RBW_Hz;//Возвращает в любом случаее текущий установленный
        public double VBW_Hz;//Возвращает в любом случаее текущий установленный
        public bool ADCOverflow = false;  
    }
}
