using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Atdi.AppServer.Contracts.Sdrns2_0
{
    /// <summary>
    /// Represents frequencies for  measurements
    /// </summary>
    [DataContract(Namespace = ServicesSpecification.Namespace)]
    public class MeasFreqParam
    {
        /// <summary>
        /// Mode of frequency  S	- Single frequency; L - Frequency list; R - Frequency range
        /// </summary>
        [DataMember]
        public FrequencyMode Mode;
        /// <summary>
        /// Start freq, MHz
        /// </summary>
        [DataMember]
        public Double? RgL; 
        /// <summary>
        /// Stop freq, MHz
        /// </summary>
        [DataMember]
        public Double? RgU; 
        /// <summary>
        /// Step whith, kHz
        /// </summary>
        [DataMember]
        public Double? Step;  
        /// <summary>
        /// Array (List) of frequencies 
        /// </summary>
        [DataMember]
        public MeasFreq[] MeasFreqs;
    }
}
