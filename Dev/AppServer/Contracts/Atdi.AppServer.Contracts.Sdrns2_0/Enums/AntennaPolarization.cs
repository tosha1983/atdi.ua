using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Atdi.AppServer.Contracts.Sdrns2_0
{
    /// <summary>
    /// Polarization of antenna
    /// </summary>
    [DataContract(Namespace = ServicesSpecification.Namespace)]
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
