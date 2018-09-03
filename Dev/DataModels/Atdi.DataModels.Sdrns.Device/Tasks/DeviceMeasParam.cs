using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Device
{
    /// <summary>
    /// Measurement device parameters
    /// </summary>
    [DataContract(Namespace = Specification.Namespace)]
    public class DeviceMeasParam
    {
        /// <summary>
        /// Radio bandwidth, kHz
        /// </summary>
        [DataMember]
        public double RBW_kHz { get; set; }

        /// <summary>
        /// Video bandwidth, kHz
        /// </summary>
        [DataMember]
        public double? VBW_kHz { get; set; }

        /// <summary>
        /// Scanning bandwidth, kHz
        /// </summary>
        [DataMember]
        public double? ScanBW_kHz { get; set; }

        /// <summary>
        /// Measurement time, sec
        /// </summary>
        [DataMember]
        public double? MeasTime_sec { get; set; }

        /// <summary>
        /// Reference signal level, dBm
        /// </summary>
        [DataMember]
        public double? RefLevel_dBm { get; set; }

        /// <summary>
        /// Detection type
        /// </summary>
        [DataMember]
        public DetectingType DetectType { get; set; }

        /// <summary>
        /// Preamplification, dB
        /// </summary>
        [DataMember]
        public int? Preamplification_dB { get; set; }

        /// <summary>
        /// RF attenuation, dB
        /// </summary>
        [DataMember]
        public int? RfAttenuation_dB { get; set; }
    }
}
