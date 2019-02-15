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
    /// Represents station of measurement
    /// </summary>
    [DataContract(Namespace =Specification.Namespace)]
    public class StationMeasurements
    {
        /// <summary>
        /// StationId
        /// </summary>
        [DataMember]
        public SensorIdentifier StationId;
    }
}
