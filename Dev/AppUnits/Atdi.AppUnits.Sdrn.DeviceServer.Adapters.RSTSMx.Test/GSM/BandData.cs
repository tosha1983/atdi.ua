using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrn.DeviceServer.Commands.Parameters.MesureSystemInfo;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Adapters.RSTSMx.GSM
{
    public class BandData
    {
        public GSMBands Band;
        public int ARFCNStart;
        public int ARFCNStop;
        public decimal FreqUpStart;
        public decimal FreqUpStop;
        public decimal FreqDnStart;
        public decimal FreqDnStop;
        public List<BandFreq> FreqData;
    }
}
