using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Device
{
    /// <summary>
    /// Represents the antenna pattetrn parameters depending on frequency
    /// </summary>
    [DataContract(Namespace = Specification.Namespace)]
    public class AntennaPattern
    {
        /// <summary>
        /// Frequency, MHz
        /// </summary>
        [DataMember]
        public double Freq_MHz { get; set; }

        /// <summary>
        /// Antenna gain, dB
        /// </summary>
        [DataMember]
        public double Gain { get; set; }

        /// <summary>
        /// DiagA - 9X - Antenna pattern
        /// </summary>
        [DataMember]
        public string DiagA { get; set; }

        /// <summary>
        /// DiagH - 9XH - Horizontal diagram
        /// </summary>
        [DataMember]
        public string DiagH { get; set; }

        /// <summary>
        /// DiagV - 9XV - Vertical diagram
        /// </summary>
        [DataMember]
        public string DiagV { get; set; }
    }
}
