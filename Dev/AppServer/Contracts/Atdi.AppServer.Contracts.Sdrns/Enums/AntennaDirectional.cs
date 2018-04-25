using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Atdi.AppServer.Contracts.Sdrns
{
    /// <summary>
    /// Directional of antenna
    /// </summary>
    [DataContract(Namespace = ServicesSpecification.Namespace)]
    public enum  AntennaDirectional
    {
        /// <summary>
        /// 
        /// </summary>
        [EnumMember]
        Directional,
        /// <summary>
        /// 
        /// </summary>
        [EnumMember]
        NotDirectional
    }
}
