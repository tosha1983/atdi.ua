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
    /// Polarization of antenna
    /// </summary>
    [DataContract(Namespace =Specification.Namespace)]
    [Serializable]
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
