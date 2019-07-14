using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Adapters.RSTSMx.CDMAEVDO
{
    public class Channel
    {
        public int ChannelN;
        /// <summary>
        /// True = EVDO, False = CDMA
        /// </summary>
        public bool EVDOvsCDMA;
        public decimal FreqUp;
        public decimal FreqDn;
        public string StandartSubband;
    }
}
