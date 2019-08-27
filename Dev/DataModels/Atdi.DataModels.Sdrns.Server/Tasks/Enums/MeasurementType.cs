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
    /// Type of measurement
    /// </summary>
    [DataContract(Namespace = Specification.Namespace)]
    [Serializable]
    public enum MeasurementType
    {
        /// <summary>
        /// 
        /// </summary>
        [EnumMember]
        SpectrumOccupation,
        /// <summary>
        /// 
        /// </summary>
        [EnumMember]
        Level,
        /// <summary>
        /// 
        /// </summary>
        [EnumMember]
        Offset,
        /// <summary>
        /// 
        /// </summary>
        [EnumMember]
        Frequency,
        /// <summary>
        /// 
        /// </summary>
        [EnumMember]
        FreqModulation,
        /// <summary>
        /// 
        /// </summary>
        [EnumMember]
        AmplModulation,
        /// <summary>
        /// 
        /// </summary>
        [EnumMember]
        BandwidthMeas,
        /// <summary>
        /// 
        /// </summary>
        [EnumMember]
        Bearing,
        /// <summary>
        /// 
        /// </summary>
        [EnumMember]
        SubAudioTone,
        /// <summary>
        /// 
        /// </summary>
        [EnumMember]
        Program,
        /// <summary>
        /// 
        /// </summary>
        [EnumMember]
        PICode,
        /// <summary>
        /// 
        /// </summary>
        [EnumMember]
        SoundID,
        /// <summary>
        /// 
        /// </summary>
        [EnumMember]
        Location,
        /// <summary>
        /// 
        /// </summary>
        [EnumMember]
        MonitoringStations,
        /// <summary>
        /// 
        /// </summary>
        [EnumMember]
        Signaling
    }
}
