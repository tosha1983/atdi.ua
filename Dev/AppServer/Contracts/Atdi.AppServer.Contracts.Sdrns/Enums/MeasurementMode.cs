using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Atdi.AppServer.Contracts.Sdrns
{
    /// <summary>
    /// Mode of measurement
    /// </summary>
    [DataContract(Namespace = ServicesSpecification.Namespace)]
    public enum MeasurementMode
    {
        /// <summary>
        /// 
        /// </summary>
        [EnumMember]
        Normal,
        /// <summary>
        /// 
        /// </summary>
        [EnumMember]
        Cont,
        /// <summary>
        /// 
        /// </summary>
        [EnumMember]
        Gate
    }
}
