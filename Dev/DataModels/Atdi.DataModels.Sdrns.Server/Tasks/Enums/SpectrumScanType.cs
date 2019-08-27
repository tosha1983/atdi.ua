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
    /// Type of spectrum
    /// </summary>
    [DataContract(Namespace = Specification.Namespace)]
    [Serializable]
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
