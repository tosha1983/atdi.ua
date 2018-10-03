using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Atdi.AppServer.Contracts.Sdrns2_0
{
    /// <summary>
    /// Represents parameters of location for measurements.
    /// </summary>
    [DataContract(Namespace = ServicesSpecification.Namespace)]
    public class MeasLocParam
    {
        /// <summary>
        /// Id
        /// </summary>
        [DataMember]
        public MeasLocParamIdentifier Id;
        /// <summary>
        /// Longitude, DEC
        /// </summary>
        [DataMember]
        public double? Lon;
        /// <summary>
        /// Latitude, DEC
        /// </summary>
        [DataMember]
        public double? Lat;
        /// <summary>
        /// Altitude above sea level, m
        /// </summary>
        [DataMember]
        public double? ASL;
        /// <summary>
        /// MaxDist, km
        /// </summary>
        [DataMember]
        public double? MaxDist;
    }
}
