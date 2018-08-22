using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Device
{
    /// <summary>
    /// Represents frequencies for  measurements
    /// </summary>
    [DataContract(Namespace = Specification.Namespace)]
    public class MeasuredFrequencies
    {
        /// <summary>
        /// Mode of frequency  S	- Single frequency; L - Frequency list; R - Frequency range
        /// </summary>
        [DataMember]
        public FrequencyMode Mode { get; set; }

        /// <summary>
        /// Start freq, MHz
        /// </summary>
        [DataMember]
        public double? RgL { get; set; }

        /// <summary>
        /// Stop freq, MHz
        /// </summary>
        [DataMember]
        public double? RgU { get; set; }

        /// <summary>
        /// Step whith, kHz
        /// </summary>
        [DataMember]
        public double? Step { get; set; }

        /// <summary>
        /// Array (List) of the values of the frequencies 
        /// </summary>
        [DataMember]
        public double[] Values { get; set; }
    }
}
