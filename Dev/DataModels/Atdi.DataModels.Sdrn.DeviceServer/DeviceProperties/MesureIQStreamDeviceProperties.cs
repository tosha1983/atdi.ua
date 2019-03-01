using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.DeviceServer
{
    public class MesureIQStreamDeviceProperties:DevicePropertiesBase
    {
        public double BitRateMax_MBs; 
        public bool AvailabilityPPS;
        public StandardDeviceProperties standartDeviceProperties;
    }
}
