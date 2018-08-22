using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Device
{
    /// <summary>
    /// 
    /// </summary>
    [DataContract(Namespace = Specification.Namespace)]
    public class StationSite : GeoPoint
    {
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string Adress { get; set; }

        /// <summary>
        /// Район (часть области)
        /// </summary>
        [DataMember]
        public string Region { get; set; }
    }
}
