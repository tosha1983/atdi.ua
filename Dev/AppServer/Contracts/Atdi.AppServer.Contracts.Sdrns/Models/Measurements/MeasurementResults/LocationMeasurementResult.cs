using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Atdi.AppServer.Contracts.Sdrns
{
    /// <summary>
    /// Result of measurement the Locations of emission 
    /// </summary>
    [DataContract(Namespace = ServicesSpecification.Namespace)]
    public class LocationMeasurementResult : MeasurementResult
    {
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
        /// Radius, km
        /// </summary>
        [DataMember]
        public double? Radius;//km
        /// <summary>
        /// Plan Lon, DEC
        /// </summary>
        [DataMember]
        public double? PLon;//DEC
        /// <summary>
        /// Plan Lat, DEC
        /// </summary>
        [DataMember]
        public double? PLat;//DEC
        /// <summary>
        /// Plan Diff, km
        /// </summary>
        [DataMember]
        public double? PDiff;//km
    }
}
