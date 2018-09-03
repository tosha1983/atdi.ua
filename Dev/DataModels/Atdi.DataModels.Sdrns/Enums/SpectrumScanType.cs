using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns
{
    /// <summary>
    /// Type of spectrum scan
    /// </summary>
    [DataContract(Namespace = Specification.Namespace)]
    public enum SpectrumScanType
    {
        /// <summary>
        /// Real time
        /// </summary>
        [EnumMember]
        RealTime,

        /// <summary>
        /// Sweep
        /// </summary>
        [EnumMember]
        Sweep
    }
}
