using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns
{
    /// <summary>
    /// Mode of frequency
    /// </summary>
    [DataContract(Namespace = Specification.Namespace)]
    public enum FrequencyMode
    {
        /// <summary>
        /// Single frequency mode
        /// </summary>
        [EnumMember]
        SingleFrequency,
        /// <summary>
        /// Frequency list mode
        /// </summary>
        [EnumMember]
        FrequencyList,
        /// <summary>
        /// Frequency range mode
        /// </summary>
        [EnumMember]
        FrequencyRange
    }
}
