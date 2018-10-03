using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Atdi.AppServer.Contracts.Sdrns2_0
{
    /// <summary>
    /// Type of detecting
    /// </summary>
    [DataContract(Namespace = ServicesSpecification.Namespace)]
    public enum DetectingType
    {
        /// <summary>
        /// 
        /// </summary>
        [EnumMember]
        Avarage,
        /// <summary>
        /// 
        /// </summary>
        [EnumMember]
        Peak,
        /// <summary>
        /// 
        /// </summary>
        [EnumMember]
        MaxPeak,
        /// <summary>
        /// 
        /// </summary>
        [EnumMember]
        MinPeak
    }
}
