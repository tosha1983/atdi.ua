using Atdi.DataModels.Api.EventSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Server.Events
{
    public class ReceivedHealthDataEvent : Event
    {
        public ReceivedHealthDataEvent()
            : base("OnReceivedHealthData")
        {
        }

        public ReceivedHealthDataEvent(string source)
            : base("OnReceivedHealthData", source)
        {
        }

        public long HealthId { get; set; } 
    }
}
