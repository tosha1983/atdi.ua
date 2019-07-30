using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Device
{
    /// <summary>
    /// Parameters of the station site
    /// </summary>
    [DataContract(Namespace = Specification.Namespace)]
    [Serializable]
    public class StationSite : GeoPoint
    {
        /// <summary>
        /// Address
        /// </summary>
        [DataMember]
        public string Adress { get; set; }

        /// <summary>
        /// Region
        /// </summary>
        [DataMember]
        public string Region { get; set; }
    }
}
