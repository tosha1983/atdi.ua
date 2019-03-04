using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.DeviceServer.Processing
{
    public struct ReferenceSignal
    {
        public double Frequency_MHz;
        public double Bandwidth_kHz;
        public double LevelSignal_dBm;
        public SignalMask SignalMask;
    }
}
