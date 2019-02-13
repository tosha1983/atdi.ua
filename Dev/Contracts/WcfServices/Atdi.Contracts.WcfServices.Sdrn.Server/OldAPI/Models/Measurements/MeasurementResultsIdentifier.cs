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
    /// Represents an identifier of measurements results
    /// </summary>
    [DataContract(Namespace = Specification.Namespace)]
    public class MeasurementResultsIdentifier
    {
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public int MeasSdrResultsId;
        /// <summary>
        /// Id of Measurements Task
        /// </summary>
        [DataMember]
        public MeasTaskIdentifier MeasTaskId;
        /// <summary>
        /// Id of Measurements Sub Task
        /// </summary>
        [DataMember]
        public int SubMeasTaskId;
        /// <summary>
        /// Id of Measurements Sub Task for station/sensor
        /// </summary>
        [DataMember]
        public int SubMeasTaskStationId;
    }
}
