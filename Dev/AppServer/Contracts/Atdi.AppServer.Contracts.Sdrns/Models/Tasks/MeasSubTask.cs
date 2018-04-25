using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Atdi.AppServer.Contracts.Sdrns
{
    /// <summary>
    /// Represents subtask for measurements.
    /// </summary>
    [DataContract(Namespace = ServicesSpecification.Namespace)]
    public class MeasSubTask
    {
        /// <summary>
        /// Id of MeasTask
        /// </summary>
        [DataMember]
        public MeasTaskIdentifier Id;
        /// <summary>
        /// TimeStart
        /// </summary>
        [DataMember]
        public DateTime TimeStart;
        /// <summary>
        /// TimeStop
        /// </summary>
        [DataMember]
        public DateTime TimeStop;
        /// <summary>
        /// Status
        /// </summarStatusy>
        [DataMember]
        public string Status;
        /// <summary>
        /// Interval
        /// </summary>
        [DataMember]
        public int? Interval;
        /// <summary>
        /// MeasSubTaskStations
        /// </summary>
        [DataMember]
        public MeasSubTaskStation[] MeasSubTaskStations;
    }
}
