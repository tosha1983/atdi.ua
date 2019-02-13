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
