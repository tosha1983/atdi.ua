using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns
{
    /// <summary>
    /// Type of detecting
    /// </summary>
    [DataContract(Namespace = Specification.Namespace)]
    public enum DetectingType
    {
        /// <summary>
        /// Auto value
        /// </summary>
        [EnumMember]
        Auto,
        /// <summary>
        /// Average value
        /// </summary>
        [EnumMember]
        Average,
        /// <summary>
        /// Maximum peak value
        /// </summary>
        [EnumMember]
        MaxPeak,
        /// <summary>
        /// Minimum peak value
        /// </summary>
        [EnumMember]
        MinPeak,
        /// <summary>
        /// Root mean square value
        /// </summary>
        [EnumMember]
        RMS,
        /// <summary>
        /// Peak value
        /// </summary>
        [EnumMember]
        Peak
    }
}
