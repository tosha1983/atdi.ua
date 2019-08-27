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
    /// Mode of frequency
    /// </summary>
    [DataContract(Namespace = Specification.Namespace)]
    [Serializable]
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
