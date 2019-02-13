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
    /// Location Station (sensor) for measurement LocationSensorMeasurement
    /// </summary>
    [DataContract(Namespace = Specification.Namespace)]
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
