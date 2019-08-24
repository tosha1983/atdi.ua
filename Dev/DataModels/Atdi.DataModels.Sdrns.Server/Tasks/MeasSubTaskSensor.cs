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
    /// Represents subtask of measurements for sensor
    /// </summary>
    [DataContract(Namespace = Specification.Namespace)]
    [Serializable]
    public class MeasSubTaskSensor
    {
        /// <summary>
        /// Id
        /// </summary>
        [DataMember]
        public long Id;
        /// <summary>
        /// SensorId
        /// </summary>
        [DataMember]
        public long SensorId;
        /// <summary>
        /// Status
        /// </summary>
        [DataMember]
        public string Status;
        /// <summary>
        /// Count
        /// </summary>
        [DataMember]
        public int? Count;
        /// <summary>
        /// TimeNextTask
        /// </summary>
        [DataMember]
        public DateTime? TimeNextTask;
        /// <summary>
        /// Master SubTaskSensor Id
        /// </summary>
        [DataMember]
        public long? MasterId;
    }
}
