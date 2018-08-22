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
