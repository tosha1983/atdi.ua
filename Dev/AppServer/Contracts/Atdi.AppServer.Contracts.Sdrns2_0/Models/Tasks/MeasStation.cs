using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Atdi.AppServer.Contracts.Sdrns2_0
{
    /// <summary>
    /// Represents Station for measurements.
    /// </summary>
    [DataContract(Namespace = ServicesSpecification.Namespace)]
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
