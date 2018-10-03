using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Atdi.AppServer.Contracts.Sdrns2_0
{
    /// <summary>
    /// Gain type of antenna
    /// </summary>
    [DataContract(Namespace = ServicesSpecification.Namespace)]
    public enum AntennaGainType
    {
        /// <summary>
        /// 
        /// </summary>
        [EnumMember]
        Dipole,
        /// <summary>
        /// 
        /// </summary>
        [EnumMember]
        Vertical,
        /// <summary>
        /// 
        /// </summary>
        [EnumMember]
        Isotropic
    }
}
