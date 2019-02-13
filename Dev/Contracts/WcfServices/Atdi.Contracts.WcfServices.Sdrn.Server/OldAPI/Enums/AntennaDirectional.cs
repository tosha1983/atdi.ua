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
    /// Directional of antenna
    /// </summary>
    [DataContract(Namespace = Specification.Namespace)]
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
