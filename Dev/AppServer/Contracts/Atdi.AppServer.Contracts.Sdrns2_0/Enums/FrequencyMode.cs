using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Atdi.AppServer.Contracts.Sdrns2_0
{
    /// <summary>
    /// Mode of frequency
    /// </summary>
    [DataContract(Namespace = ServicesSpecification.Namespace)]
    public enum FrequencyMode
    {
        /// <summary>
        /// 
        /// </summary>
        [EnumMember]
        SingleFrequency,
        /// <summary>
        /// 
        /// </summary>
        [EnumMember]
        FrequencyList,
        /// <summary>
        /// 
        /// </summary>
        [EnumMember]
        FrequencyRange
    }
}
