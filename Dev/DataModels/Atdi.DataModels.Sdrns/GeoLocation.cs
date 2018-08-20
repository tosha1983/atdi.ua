using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns
{
    [DataContract(Namespace = Specification.Namespace)]
    public class GeoLocation
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

        /// <summary>
        /// Altitude above sea level, m
        /// </summary>
        [DataMember]
        public double? ASL { get; set; }

        /// <summary>
        /// Altitude above ground level, m
        /// </summary>
        [DataMember]
        public double? AGL { get; set; }
    }
}
