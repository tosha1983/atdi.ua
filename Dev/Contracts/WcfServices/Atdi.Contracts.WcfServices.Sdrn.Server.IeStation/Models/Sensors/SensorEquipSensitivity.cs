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
    /// Represents parameters of equipment of sensor for measurement depend from frequency.
    /// </summary>
    [DataContract(Namespace = Specification.Namespace)]
    public class SensorEquipSensitivity
    {
        /// <summary>
        /// Freq, MHz
        /// </summary>
        [DataMember]
        public Double Freq { get; set; }
        /// <summary>
        /// own noise level, dBm
        /// </summary>
        [DataMember]
        public Double? KTBF { get; set; }
        /// <summary>
        /// noise figure, dB
        /// </summary>
        [DataMember]
        public Double? NoiseF { get; set; }
        /// <summary>
        /// FreqStability, %
        /// </summary>
        [DataMember]
        public Double? FreqStability { get; set; }//%
        /// <summary>
        /// Additional Loss, dB
        /// </summary>
        [DataMember]
        public Double? AddLoss { get; set; }
    }
}
