using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Atdi.AppServer.Contracts.Sdrns
{
    /// <summary>
    /// Type of Task of measurement
    /// </summary>
    [DataContract(Namespace = ServicesSpecification.Namespace)]
    public enum MeasTaskType
    {
        /// <summary>
        /// 
        /// </summary>
        [EnumMember]
        FixedFrequencyMode,
        /// <summary>
        /// 
        /// </summary>
        [EnumMember]
        Scan,
        /// <summary>
        /// 
        /// </summary>
        [EnumMember]
        Digiscan,
        /// <summary>
        /// 
        /// </summary>
        [EnumMember]
        FrequencyListScan,
        /// <summary>
        /// 
        /// </summary>
        [EnumMember]
        TransmitterListScan,
        /// <summary>
        /// 
        /// </summary>
        [EnumMember]
        IntermodulationAnalysis,
        /// <summary>
        /// 
        /// </summary>
        [EnumMember]
        Sweep
    }
}
