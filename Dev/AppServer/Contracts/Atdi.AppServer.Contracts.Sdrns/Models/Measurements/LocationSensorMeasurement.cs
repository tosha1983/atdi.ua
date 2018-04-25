using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Atdi.AppServer.Contracts.Sdrns
{
    /// <summary>
    /// Location Station (sensor) for measurement LocationSensorMeasurement
    /// </summary>
    [DataContract(Namespace = ServicesSpecification.Namespace)]
    public class LocationSensorMeasurement
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
        /// Altitude above sea level, m
        /// </summary>
        [DataMember]
        public double? ASL;
    }
}
