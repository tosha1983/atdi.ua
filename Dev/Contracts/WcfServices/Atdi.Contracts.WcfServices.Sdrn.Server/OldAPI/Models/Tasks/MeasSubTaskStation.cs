using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using Atdi.DataModels.Sdrns.Server;

namespace Atdi.Contracts.WcfServices.Sdrn.Server
{
    /// <summary>
    /// Represents subtask of measurements for sensor
    /// </summary>
    [DataContract(Namespace = Specification.Namespace)]
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
        public SensorIdentifier SensorId;
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
    }
}
