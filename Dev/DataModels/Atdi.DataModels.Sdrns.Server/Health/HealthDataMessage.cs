using Atdi.DataModels.Api.DataBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Server.DataBus
{
    public class HealthDataMessage : MessageTypeBase
    {
        public HealthDataMessage() 
            : base("HealthDataMessage", QueueType.Common)
        {
        }
    }
}
