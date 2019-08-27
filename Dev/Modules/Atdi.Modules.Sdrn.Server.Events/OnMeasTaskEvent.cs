using Atdi.DataModels.Api.EventSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Modules.Sdrn.Server.Events
{
    public class OnMeasTaskEvent : Event
    {
        public OnMeasTaskEvent()
            : base("OnNewMeasTask")
        {
        }

        public OnMeasTaskEvent(string source) 
            : base("OnNewMeasTask", source)
        {
        }

        public long MeasTaskId { get; set; }
        public long SensorId { get; set; }
        public string MeasTaskIds { get; set; }
        public string SensorName { get; set; }
        public string EquipmentTechId { get; set; }
        public long SubTaskSensorId { get; set; }

    }
}
