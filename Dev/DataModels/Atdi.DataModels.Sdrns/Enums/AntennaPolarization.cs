using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns
{
    /// <summary>
    /// Polarization of antenna
    /// </summary>
    [DataContract(Namespace = Specification.Namespace)]
    public enum AntennaPolarization
    {
        /// <summary>
        /// Vertical
        /// </summary>
        [EnumMember]
        V,

        /// <summary>
        /// Horizontal
        /// </summary>
        [EnumMember]
        H,

        /// <summary>
        /// Mixed
        /// </summary>
        [EnumMember]
        M
    }
}
