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
    /// Polarization of antenna
    /// </summary>
    [DataContract(Namespace =Specification.Namespace)]
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
