using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.AppServer.Contracts.Sdrns
{
    /// <summary>
    /// Characterizes the reason for the stay of the measuring device at a point
    /// </summary>
    [DataContract(Namespace = ServicesSpecification.Namespace)]
    public enum PointStayType
    {
        /// <summary>
        /// The device is in the state of motion when passing this point
        /// </summary>
        [EnumMember]
        InMove,
        /// <summary>
        /// The device stays for some time to a given point for any reason
        /// </summary>
        [EnumMember]
        Stay,
        /// <summary>
        /// The device stays for some time to a given point for measurements
        /// </summary>
        [EnumMember]
        StayForMeasurements
    }
}
