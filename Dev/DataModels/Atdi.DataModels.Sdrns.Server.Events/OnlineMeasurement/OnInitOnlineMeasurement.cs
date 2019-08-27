using Atdi.DataModels.Api.EventSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Server.Events.OnlineMeasurement
{
    public class OnInitOnlineMeasurement : Event
    {
        public OnInitOnlineMeasurement()
            : base("OnInitOnlineMeasurement")
        {
        }

        public OnInitOnlineMeasurement(string source)
            : base("OnInitOnlineMeasurement", source)
        {
        }

        public long OnlineMeasId { get; set; }
    }
}
