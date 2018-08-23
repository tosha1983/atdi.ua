using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns
{
    /// <summary>
    /// Directivity of antenna
    /// </summary>
    [DataContract(Namespace = Specification.Namespace)]
    public enum AntennaDirectivity
    {
        /// <summary>
        /// Directional antenna
        /// </summary>
        [EnumMember]
        Directional,

        /// <summary>
        /// Non-directional antenna
        /// </summary>
        [EnumMember]
        NonDirectional
    }
}
