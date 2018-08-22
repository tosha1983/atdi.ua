using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Device
{
    /// <summary>
    /// Represents parameters of location for measurements.
    /// </summary>
    [DataContract(Namespace = Specification.Namespace)]
    public class LocationMeasParam : GeoLocation
    {
        /// <summary>
        /// Id
        /// </summary>
        [DataMember]
        public int Id { get; set; }

        /// <summary>
        /// MaxDist, km
        /// </summary>
        [DataMember]
        public double? MaxDist { get; set; }
    }
}
