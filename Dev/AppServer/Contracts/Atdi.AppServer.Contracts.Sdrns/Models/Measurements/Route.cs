using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.AppServer.Contracts.Sdrns
{
    /// <summary>
    /// Route during measurements
    /// </summary>
    [DataContract(Namespace = ServicesSpecification.Namespace)]
    public class Route
    {
        /// <summary>
        /// Route identifier
        /// </summary>
        [DataMember]
        public string RouteId { get; set; }
        /// <summary>
        /// The points of route
        /// </summary>
        [DataMember]
        public RoutePoint[] RoutePoints { get; set; }
    }
}
