using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Device
{
    public struct ReferenceSignal
    {
        public double Frequency_MHz;
        public double Bandwidth_kHz;
        public double LevelSignal_dBm;
        public SignalMask SignalMask;
        public int IcsmId;
        public string IcsmTable;
    }
}
