using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns
{
    /// <summary>
    /// Directional of antenna
    /// </summary>
    [DataContract(Namespace = Specification.Namespace)]
    public enum AntennaDirectional
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
