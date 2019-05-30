using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Device
{
    /// <summary>
    /// Contains the result of signal bandwidth measurement
    /// </summary>
    [DataContract(Namespace = Specification.Namespace)]
    [Serializable]
    public class BandwidthMeasResult
    {
        /// <summary>
        /// Start point of frequency range to be measured
        /// </summary>
        [DataMember]
        public int? T1 { get; set; }

        /// <summary>
        /// End point of frequency range to be measured
        /// </summary>
        [DataMember]
        public int? T2 { get; set; }
        /// <summary>
        /// Index of signal spectrum with maximum level
        /// </summary>
        [DataMember]
        public int? MarkerIndex { get; set; }
        /// <summary>
        /// Bandwidth, kHz
        /// </summary>
        [DataMember]
        public double? Bandwidth_kHz { get; set; }
        /// <summary>
        /// Validity of performed measurement according to ITU 443
        /// </summary>
        [DataMember]
        public bool? СorrectnessEstimations { get; set; }
        /// <summary>
        /// Number of scans for BW determination
        /// </summary>
        [DataMember]
        public int TraceCount { get; set; }
    }
}
