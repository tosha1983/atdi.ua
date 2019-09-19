using Atdi.DataModels.Api.EventSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Server.Events.OnlineMeasurement
{
    public class OnOnlineMeasurementResponseDevice : Event
    {
        public OnOnlineMeasurementResponseDevice()
            : base("OnOnlineMeasurementResponseDevice")
        {
        }

        public OnOnlineMeasurementResponseDevice(string source)
            : base("OnOnlineMeasurementResponseDevice", source)
        {
        }

        public long OnlineMeasId { get; set; }

   
        public bool Conformed { get; set; }

     
        public string Message { get; set; }

    
        public byte[] Token { get; set; }

        public string WebSocketUrl { get; set; }
    }
}
