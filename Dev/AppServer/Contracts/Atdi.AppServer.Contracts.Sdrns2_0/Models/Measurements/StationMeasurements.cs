using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Atdi.AppServer.Contracts.Sdrns2_0
{
    /// <summary>
    /// Represents station of measurement
    /// </summary>
    [DataContract(Namespace = ServicesSpecification.Namespace)]
    public class StationMeasurements
    {
        /// <summary>
        /// StationId
        /// </summary>
        [DataMember]
        public SensorIdentifier StationId;
    }
}
