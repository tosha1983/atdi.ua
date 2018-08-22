using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Device
{
    /// <summary>
    /// Параметры приемника для проведения измерений
    /// </summary>
    [DataContract(Namespace = Specification.Namespace)]
    public class DeviceMeasParam
    {
        /// <summary>
        /// kHz
        /// </summary>
        [DataMember]
        public double RBW_kHz { get; set; }

        /// <summary>
        /// kHz
        /// </summary>
        [DataMember]
        public double? VBW_kHz { get; set; }

        /// <summary>
        /// kHz
        /// </summary>
        [DataMember]
        public double? ScanBW_kHz { get; set; }

        /// <summary>
        /// sec
        /// </summary>
        [DataMember]
        public double? MeasTime_sec { get; set; }

        /// <summary>
        /// dBm
        /// </summary>
        [DataMember]
        public double? RefLevel_dBm { get; set; }

        /// <summary>
        /// Average -> bb_api.BB_AVERAGE;  othre -> bb_api.BB_MIN_AND_MAX
        /// </summary>
        [DataMember]
        public DetectingType DetectType { get; set; }

        /// <summary>
        /// 0, 10, 20, ..., dB
        /// </summary>
        [DataMember]
        public int? Preamplification_dB { get; set; }

        /// <summary>
        /// 0, 10, 20, 30, ...,  dB 
        /// </summary>
        [DataMember]
        public int? RfAttenuation_dB { get; set; }
    }
}
