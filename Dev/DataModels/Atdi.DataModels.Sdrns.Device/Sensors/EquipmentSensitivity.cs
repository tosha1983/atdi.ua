using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Device
{
    /// <summary>
    /// Represents parameters of equipment of sensor for measurement depend from frequency.
    /// </summary>
    [DataContract(Namespace = Specification.Namespace)]

    public class EquipmentSensitivity
    {
        /// <summary>
        /// Freq, MHz
        /// </summary>
        [DataMember]
        public double Freq_MHz { get; set; }

        /// <summary>
        /// own noise level, dBm
        /// </summary>
        [DataMember]
        public double? KTBF_dBm { get; set; }

        /// <summary>
        /// noise figure, dB
        /// </summary>
        [DataMember]
        public double? NoiseF { get; set; }

        /// <summary>
        /// FreqStability, %
        /// </summary>
        [DataMember]
        public double? FreqStability { get; set; }

        /// <summary>
        /// Additional Loss, dB
        /// </summary>
        [DataMember]
        public double? AddLoss { get; set; }
    }
}
