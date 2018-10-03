using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Atdi.AppServer.Contracts.Sdrns2_0
{
    /// <summary>
    /// Represents location of point of poligon of sensor
    /// </summary>
    [DataContract(Namespace = ServicesSpecification.Namespace)]
    public class SensorPoligonPoint
    {
        /// <summary>
        /// Longitude, DEC
        /// </summary>
        [DataMember]
        public Double? Lon;
        /// <summary>
        /// Latitude,  DEC
        /// </summary>
        [DataMember]
        public Double? Lat;
     }
}

