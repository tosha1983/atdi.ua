using Atdi.DataModels.Api.EventSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Server.Events.OnlineMeasurement
{

    public class OnInitExternalOnlineMeasurement : Event
    {
        public OnInitExternalOnlineMeasurement()
            : base("OnInitExternalOnlineMeasurement")
        {
        }

        public OnInitExternalOnlineMeasurement(string source)
            : base("OnInitExternalOnlineMeasurement", source)
        {
        }

        public long OnlineMeasId { get; set; }

        public string SensorName { get; set; }

        public string SensorTechId { get; set; }

        public string AggregationServerInstance { get; set; }
      
        public byte[] ServerToken;

        public TimeSpan Period { get; set; }
    }
}
