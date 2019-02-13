using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using Atdi.DataModels.Sdrns.Server;

namespace Atdi.Contracts.WcfServices.Sdrn.Server
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
