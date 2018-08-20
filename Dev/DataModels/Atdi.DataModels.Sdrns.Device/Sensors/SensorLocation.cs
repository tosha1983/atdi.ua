using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Device
{
    /// <summary>
    /// Represents location of sensor
    /// </summary>
    [DataContract(Namespace = Specification.Namespace)]
    public class SensorLocation : GeoLocation
    {
        /// <summary>
        /// DataFrom
        /// </summary>
        [DataMember]
        public DateTime? From { get; set; }

        /// <summary>
        /// DataTo
        /// </summary>
        [DataMember]
        public DateTime? To { get; set; }

        /// <summary>
        /// DataCreated
        /// </summary>
        [DataMember]
        public DateTime? Created { get; set; }

        /// <summary>
        /// Status
        /// </summary>
        [DataMember]
        public string Status { get; set; }

    }
}
