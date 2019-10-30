using Atdi.DataModels.Api.DataBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Server
{
    public class SendDeviceCommandFromAggregationToMasterServer : MessageTypeBase
    {
        public SendDeviceCommandFromAggregationToMasterServer() 
            : base("device_command_result", QueueType.Private, "device_command_result")
        {
        }
    }
}
