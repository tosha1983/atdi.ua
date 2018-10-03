using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Atdi.AppServer.Contracts.Sdrns2_0
{
    /// <summary>
    /// Type of spectrum
    /// </summary>
    [DataContract(Namespace = ServicesSpecification.Namespace)]
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
