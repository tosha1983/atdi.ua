using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Device
{
    /// <summary>
    /// Represents the sensor location
    /// </summary>
    [DataContract(Namespace = Specification.Namespace)]
    public class SensorLocation : GeoLocation
    {
        /// <summary>
        /// Date: From
        /// </summary>
        [DataMember]
        public DateTime? From { get; set; }

        /// <summary>
        /// Date: To
        /// </summary>
        [DataMember]
        public DateTime? To { get; set; }

        /// <summary>
        /// Date created
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
