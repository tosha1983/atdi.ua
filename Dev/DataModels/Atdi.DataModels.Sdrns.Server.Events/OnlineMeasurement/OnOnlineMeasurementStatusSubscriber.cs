using Atdi.DataModels.Api.EventSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Server.Events.OnlineMeasurement
{
    public class OnOnlineMeasurementStatusSubscriber : Event
    {
        public OnOnlineMeasurementStatusSubscriber()
            : base("OnOnlineMeasurementStatusSubscriber")
        {
        }

        public OnOnlineMeasurementStatusSubscriber(string source)
            : base("OnOnlineMeasurementStatusSubscriber", source)
        {
        }

        public long OnlineMeasId { get; set; }


        public byte SensorOnlineMeasurementStatus { get; set; }

        public string Note { get; set; }

    }
}
