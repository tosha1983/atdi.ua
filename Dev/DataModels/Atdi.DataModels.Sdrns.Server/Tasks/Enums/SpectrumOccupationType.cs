using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using Atdi.DataModels.Sdrns.Server;

namespace Atdi.DataModels.Sdrns.Server
{
    /// <summary>
    /// Type of spectrum occupation 
    /// </summary>
    [DataContract(Namespace = Specification.Namespace)]
    [Serializable]
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
