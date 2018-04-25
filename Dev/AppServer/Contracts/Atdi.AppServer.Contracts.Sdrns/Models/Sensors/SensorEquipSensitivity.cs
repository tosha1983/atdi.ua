using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Atdi.AppServer.Contracts.Sdrns
{
    /// <summary>
    /// Represents parameters of equipment of sensor for measurement depend from frequency.
    /// </summary>
    [DataContract(Namespace = ServicesSpecification.Namespace)]
    public class SensorEquipSensitivity
    {
        /// <summary>
        /// Freq, MHz
        /// </summary>
        [DataMember]
        public Double Freq; 
        /// <summary>
        /// own noise level, dBm
        /// </summary>
        [DataMember]
        public Double? KTBF;
        /// <summary>
        /// noise figure, dB
        /// </summary>
        [DataMember]
        public Double? NoiseF;
        /// <summary>
        /// FreqStability, %
        /// </summary>
        [DataMember]
        public Double? FreqStability;//%
        /// <summary>
        /// Additional Loss, dB
        /// </summary>
        [DataMember]
        public Double? AddLoss; 
    }
}
