using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using Atdi.DataModels.Sdrns.Server;

namespace Atdi.Contracts.WcfServices.Sdrn.Server.IeStation
{
    /// <summary>
    /// Represents parameters of patterns of antenna of sensor for measurement depend from frequency.
    /// </summary>
    [DataContract(Namespace = Specification.Namespace)]
    public class AntennaPattern
    {
        /// <summary>
        /// Frequency of pattren, MHz
        /// </summary>
        [DataMember]
        public Double Freq { get; set; }
        /// <summary>
        /// Gain of Antenna, dB
        /// </summary>
        [DataMember]
        public Double Gain { get; set; }
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
