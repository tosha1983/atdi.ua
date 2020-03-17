using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrns.Server;

namespace Atdi.WcfServices.Sdrn.Server.IeStation
{
    public struct EmittingDataToSort
    {
        public long? Id;
        public double StartFrequency_MHz;
        public double StopFrequency_MHz;
        public double CurrentPower_dBm;
        public int worktimeHitsCount;
    }
}
