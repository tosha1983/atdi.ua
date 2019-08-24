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
    /// Directional of antenna
    /// </summary>
    [DataContract(Namespace = Specification.Namespace)]
    [Serializable]
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
