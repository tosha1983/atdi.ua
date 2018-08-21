using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns
{
    /// <summary>
    /// Type of spectrum
    /// </summary>
    [DataContract(Namespace = Specification.Namespace)]
    public enum SpectrumScanType
    {
        /// <summary>
        /// 
        /// </summary>
        [EnumMember]
        RealTime,

        /// <summary>
        /// 
        /// </summary>
        [EnumMember]
        Sweep
    }
}
