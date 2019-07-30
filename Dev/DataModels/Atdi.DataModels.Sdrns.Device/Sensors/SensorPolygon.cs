using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Device
{
    /// <summary>
    /// Represents the sensor polygon
    /// </summary>
    [DataContract(Namespace = Specification.Namespace)]
    [Serializable]
    public class SensorPolygon
    {
        /// <summary>
        /// Points coordinates
        /// </summary>
        [DataMember]
        public GeoPoint[] Points { get; set; }


    }
}
