using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns
{
    /// <summary>
    /// Type of measurement
    /// </summary>
    [DataContract(Namespace = Specification.Namespace)]
    public enum MeasurementType
    {
        /// <summary>
        /// Spectrum occupation
        /// </summary>
        [EnumMember]
        SpectrumOccupation,
        /// <summary>
        /// Signal level
        /// </summary>
        [EnumMember]
        Level,
        /// <summary>
        /// Frequency offset
        /// </summary>
        [EnumMember]
        Offset,
        /// <summary>
        /// Frequency
        /// </summary>
        [EnumMember]
        Frequency,
        /// <summary>
        /// Frequency modulation
        /// </summary>
        [EnumMember]
        FreqModulation,
        /// <summary>
        /// Amplitude modulation
        /// </summary>
        [EnumMember]
        AmplModulation,
        /// <summary>
        /// Bandwidth
        /// </summary>
        [EnumMember]
        BandwidthMeas,
        /// <summary>
        /// Bearing
        /// </summary>
        [EnumMember]
        Bearing,
        /// <summary>
        /// Audio sub-tone
        /// </summary>
        [EnumMember]
        SubAudioTone,
        /// <summary>
        /// Program
        /// </summary>
        [EnumMember]
        Program,
        /// <summary>
        /// PI code
        /// </summary>
        [EnumMember]
        PICode,
        /// <summary>
        /// Sound ID
        /// </summary>
        [EnumMember]
        SoundID,
        /// <summary>
        /// Location
        /// </summary>
        [EnumMember]
        Location,
        /// <summary>
        /// Monitoring stations
        /// </summary>
        [EnumMember]
        MonitoringStations

    }
}
