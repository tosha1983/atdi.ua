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
    /// Represents Station for measurements.
    /// </summary>
    [DataContract(Namespace = Specification.Namespace)]
    public class MeasStation
    {
        /// <summary>
        /// StationId
        /// </summary>
        [DataMember]
        public MeasStationIdentifier StationId;
        /// <summary>
        /// StationType
        /// </summary>
        [DataMember]
        public string StationType; // Sensor
    }
}
