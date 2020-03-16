using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using Atdi.DataModels.Sdrns.Server;

namespace Atdi.Contracts.WcfServices.Sdrn.Server.IeStation
{
    /// <summary>
    /// Represents location of point of poligon of sensor
    /// </summary>
    [DataContract(Namespace = Specification.Namespace)]
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

