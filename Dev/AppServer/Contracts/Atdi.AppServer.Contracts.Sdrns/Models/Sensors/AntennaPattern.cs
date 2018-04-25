using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Atdi.AppServer.Contracts.Sdrns
{
    /// <summary>
    /// Represents parameters of patterns of antenna of sensor for measurement depend from frequency.
    /// </summary>
    [DataContract(Namespace = ServicesSpecification.Namespace)]
    public class AntennaPattern
    {
        /// <summary>
        /// Frequency of pattren, MHz
        /// </summary>
        [DataMember]
        public Double Freq; 
        /// <summary>
        /// Gain of Antenna, dB
        /// </summary>
        [DataMember]
        public Double Gain; 
        /// <summary>
        /// DiagA - 9X - Antenna pattern
        /// </summary>
        [DataMember]
        public string DiagA; 
        /// <summary>
        /// DiagH - 9XH - Horizontal diagram
        /// </summary>
        [DataMember]
        public string DiagH; 
        /// <summary>
        /// DiagV - 9XV - Vertical diagram
        /// </summary>
        [DataMember]
        public string DiagV; 
    }
}
