using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Device
{
    /// <summary>
    /// Polarization of antenna
    /// </summary>
    [DataContract(Namespace = Specification.Namespace)]
    public enum AntennaPolarization
    {
        /// <summary>
        /// 
        /// </summary>
        [EnumMember]
        V,

        /// <summary>
        /// 
        /// </summary>
        [EnumMember]
        H,

        /// <summary>
        /// 
        /// </summary>
        [EnumMember]
        M
    }
}
