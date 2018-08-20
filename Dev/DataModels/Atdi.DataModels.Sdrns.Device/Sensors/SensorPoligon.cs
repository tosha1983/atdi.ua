using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Device
{
    /// <summary>
    /// Represents location of point of poligon of sensor
    /// </summary>
    [DataContract(Namespace = Specification.Namespace)]
    public class SensorPoligon
    {
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public GeoPoint[] Points { get; set; }


    }
}
