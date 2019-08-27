using Atdi.DataModels.Api.EventSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Modules.Sdrn.Server.Events
{
    public class OnRegisterAggregationServer : Event
    {
        public OnRegisterAggregationServer()
            : base("OnRegisterAggregationServer")
        {
        }

        public OnRegisterAggregationServer(string source) 
            : base("OnRegisterAggregationServer", source)
        {
        }

        public string SensorName { get; set; }
        public string EquipmentTechId { get; set; }
       
    }
}
