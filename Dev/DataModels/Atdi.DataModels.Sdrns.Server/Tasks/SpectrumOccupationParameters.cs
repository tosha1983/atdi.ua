using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using Atdi.DataModels.Sdrns.Server;

namespace Atdi.DataModels.Sdrns.Server
{
    /// <summary>
    /// Represents parameter for measurements useful for SDR and Spectrum Occupation
    /// </summary>
    [DataContract(Namespace = Specification.Namespace)]
    [Serializable]
    public class SpectrumOccupationParameters
    {
        /// <summary>
        /// TypeSpectrumOccupation FBO - freq bandwidth occupation , FCO - freq channel occupation
        /// </summary>
        [DataMember]
        public SpectrumOccupationType TypeSpectrumOccupation;
        /// <summary>
        /// Level of minimum occupation, dBm
        /// </summary>
        [DataMember]
        public double? LevelMinOccup;
        /// <summary>
        /// Number of steps for measurements in channel 
        /// </summary>
        [DataMember]
        public int? NChenal; 
    }
}
