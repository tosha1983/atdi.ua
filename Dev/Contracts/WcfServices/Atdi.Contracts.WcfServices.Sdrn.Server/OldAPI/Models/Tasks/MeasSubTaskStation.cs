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
    /// Represents subtask of measurements for station
    /// </summary>
    [DataContract(Namespace = Specification.Namespace)]
    public class MeasSubTaskStation
    {
        /// <summary>
        /// Id
        /// </summary>
        [DataMember]
        public int Id;
        /// <summary>
        /// StationId
        /// </summary>
        [DataMember]
        public SensorIdentifier StationId;
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
