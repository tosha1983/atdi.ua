using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XICSM.ICSControlClient.Metadata
{
    public class MobStationFrequencies
    {
        public static readonly string TableName = "MOBSTA_FREQS";

        public static class Fields
        {
            public static readonly string Id = "ID";
            public static readonly string StationId = "STA_ID";

            public static class ChannelTx
            {
                public static readonly string PlanId = "ChannelTx.PLAN_ID";
                public static readonly string Channel = "ChannelTx.CHANNEL";
                public static readonly string Freq = "ChannelTx.FREQ";
            }
        }
    }
}
