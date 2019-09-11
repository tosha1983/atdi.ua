using Atdi.DataModels.Api.DataBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Server
{
    public class OnlineMeasToAggregationServer : MessageTypeBase
    {
        public OnlineMeasToAggregationServer() 
            : base("online", QueueType.Specific, "online")
        {
        }
    }
}
