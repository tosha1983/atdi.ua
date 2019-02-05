using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Device
{
    /// <summary>
    /// Contains the results of signal level measurements on the defined frequency
    /// </summary>
    [DataContract(Namespace = Specification.Namespace)]
    public class FrequencySample
    {
        /// <summary>
        /// Identifier
        /// </summary>
        [DataMember]
        public int Id { get; set; }

        /// <summary>
        /// Frequency, MHz
        /// </summary>
        [DataMember]
        public float Freq_MHz { get; set; }

        /// <summary>
        /// Signal level, dBm
        /// </summary>
        [DataMember]
        public float Level_dBm { get; set; }

        /// <summary>
        /// Signal level, dBuV/m
        /// </summary>
        [DataMember]
        public float Level_dBmkVm { get; set; }

        /// <summary>
        /// Minimal signal level, dBm
        /// </summary>
        [DataMember]
        public float LevelMin_dBm { get; set; }

        /// <summary>
        /// Maximal signal level, dBm
        /// </summary>
        [DataMember]
        public float LevelMax_dBm { get; set; }

        /// <summary>
        /// Spectrum occupancy percentage, %
        /// </summary>
        [DataMember]
        public float Occupation_Pt { get; set; }
    }
}
