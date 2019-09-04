using Atdi.DataModels.Api.EventSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Server.Events
{
    public class OnMeasTaskToMasterServerEvent : Event
    {
        public OnMeasTaskToMasterServerEvent()
            : base("OnMeasTaskToMasterServerEvent")
        {
        }

        public OnMeasTaskToMasterServerEvent(string source) 
            : base("OnMeasTaskToMasterServerEvent", source)
        {
        }

        public long MeasTaskId { get; set; }
        public long SensorId { get; set; }
        public string MeasTaskIds { get; set; }
        public string SensorName { get; set; }
        public string EquipmentTechId { get; set; }
        public string AggregationInstance { get; set; }
        public long SubTaskSensorId { get; set; }

    }
}
