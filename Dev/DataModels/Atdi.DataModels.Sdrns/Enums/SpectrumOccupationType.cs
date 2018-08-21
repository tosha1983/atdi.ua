using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns
{
    /// <summary>
    /// Type of spectrum occupation 
    /// </summary>
    [DataContract(Namespace = Specification.Namespace)]
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
