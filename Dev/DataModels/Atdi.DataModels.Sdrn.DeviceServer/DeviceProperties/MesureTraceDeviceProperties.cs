using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.DeviceServer
{
    public class MesureTraceDeviceProperties: DevicePropertiesBase
    {
        public double RBWMin_Hz; 
        public double RBWMax_Hz; 
        public double SweepTimeMin_s; 
        public double SweepTimeMax_s;
        public StandardDeviceProperties StandardDeviceProperties;
    }
}
