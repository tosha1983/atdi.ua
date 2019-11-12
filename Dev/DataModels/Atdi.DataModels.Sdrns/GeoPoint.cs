using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns
{
    /// <summary>
    /// Represents location of point
    /// </summary>
    [Serializable]
    [DataContract(Namespace = Specification.Namespace)]
    public class GeoPoint
    {
        /// <summary>
        /// Longitude, DEC
        /// </summary>
        [DataMember]
        public double? Lon { get; set; }

        /// <summary>
        /// Latitude,  DEC
        /// </summary>
        [DataMember]
        public double? Lat { get; set; }
    }
}
