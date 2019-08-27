using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using Atdi.DataModels.Sdrns.Server;

namespace Atdi.DataModels.Sdrns.Server
{
    /// <summary>
    /// Represents Sensor for measurements.
    /// </summary>
    [DataContract(Namespace = Specification.Namespace)]
    [Serializable]
    public class PrepareSendEvent
    {
        public string ActionType;
        public long SensorId;
        public long MeasTaskId;
        public string SensorName;
        public string EquipmentTechId;
        public string AggregationServerInstance;
        public MeasurementType MeasurementType;
        public string MeasTaskIds;
        public long SubTaskSensorId;
    }
}
