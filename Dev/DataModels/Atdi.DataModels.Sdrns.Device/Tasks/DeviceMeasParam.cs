using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Device
{
    /// <summary>
    ///
    /// </summary>
    [DataContract(Namespace = Specification.Namespace)]
    public class DeviceMeasParam
    {
        /// <summary>
        /// kHz
        /// </summary>
        [DataMember]
        public double RBW { get; set; }

        /// <summary>
        /// kHz
        /// </summary>
        [DataMember]
        public double VBW { get; set; }

        /// <summary>
        /// sec
        /// </summary>
        [DataMember]
        public double MeasTime { get; set; }

        /// <summary>
        /// dBm
        /// </summary>
        [DataMember]
        public double RefLeveldBm { get; set; }

        /// <summary>
        /// Average -> bb_api.BB_AVERAGE;  othre -> bb_api.BB_MIN_AND_MAX
        /// </summary>
        [DataMember]
        public DetectingType DetectTypeSDR { get; set; }

        /// <summary>
        /// 0, 10, 20, ..., dB
        /// </summary>
        [DataMember]
        public int PreamplificationSDR { get; set; }

        /// <summary>
        /// 0, 10, 20, 30, ...,  dB 
        /// </summary>
        [DataMember]
        public int RfAttenuationSDR { get; set; }
    }
}
