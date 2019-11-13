using Atdi.DataModels.Api.EventSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Server.Events
{
    public class OnDeviceCommandResultAggregationServer : Event
    {
        public OnDeviceCommandResultAggregationServer()
            : base("OnDeviceCommandResultAggregationServer")
        {
        }

        public OnDeviceCommandResultAggregationServer(string source) 
            : base("OnDeviceCommandResultAggregationServer", source)
        {
        }
    }
}
