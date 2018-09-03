using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Device
{
    /// <summary>
    /// Represents parameters of sensor measurement equipment depending on frequency
    /// </summary>
    [DataContract(Namespace = Specification.Namespace)]

    public class EquipmentSensitivity
    {
        /// <summary>
        /// Frequency, MHz
        /// </summary>
        [DataMember]
        public double Freq_MHz { get; set; }

        /// <summary>
        /// Own noise level, dBm
        /// </summary>
        [DataMember]
        public double? KTBF_dBm { get; set; }

        /// <summary>
        /// Noise figure, dB
        /// </summary>
        [DataMember]
        public double? NoiseF { get; set; }

        /// <summary>
        /// Frequency stability, %
        /// </summary>
        [DataMember]
        public double? FreqStability { get; set; }

        /// <summary>
        /// Additional loss, dB
        /// </summary>
        [DataMember]
        public double? AddLoss { get; set; }
    }
}
