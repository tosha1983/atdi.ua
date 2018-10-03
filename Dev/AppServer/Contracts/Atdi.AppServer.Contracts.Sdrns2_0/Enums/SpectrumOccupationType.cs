using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Atdi.AppServer.Contracts.Sdrns2_0
{
    /// <summary>
    /// Type of spectrum occupation 
    /// </summary>
    [DataContract(Namespace = ServicesSpecification.Namespace)]
    public enum SpectrumOccupationType
    {
        /// <summary>
        /// 
        /// </summary>
        [EnumMember]
        FreqBandwidthOccupation,
        /// <summary>
        /// 
        /// </summary>
        [EnumMember]
        FreqChannelOccupation
    }
}
