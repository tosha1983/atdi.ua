using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrns.Server;

namespace Atdi.WcfServices.Sdrn.Server.IeStation
{
    public struct StationDataToSort
    {
        public long? RefSpectrumId;
        public long? DataRefSpectrumId;
        public string GlobalSID;
        public double Freq_MHz;
        public double Level_dBm;
    }
}
