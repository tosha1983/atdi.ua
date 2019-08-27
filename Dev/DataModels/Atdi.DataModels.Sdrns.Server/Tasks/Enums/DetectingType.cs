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
    /// Type of detecting
    /// </summary>
    [DataContract(Namespace = Specification.Namespace)]
    [Serializable]
    public enum DetectingType
    {
        /// <summary>
        /// Average value
        /// </summary>
        [EnumMember]
        Average,
        /// <summary>
        /// Auto value
        /// </summary>
        [EnumMember]
        Auto,
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
