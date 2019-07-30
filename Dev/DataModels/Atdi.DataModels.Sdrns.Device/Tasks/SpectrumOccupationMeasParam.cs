using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Device
{
    /// <summary>
    /// Spectrum occupancy measurement parameters
    /// </summary>
    [DataContract(Namespace = Specification.Namespace)]
    [Serializable]
    public class SpectrumOccupationMeasParam
    {
        /// <summary>
        /// Trigger level for spectrum occupancy determination, dBm
        /// </summary>
        [DataMember]
        public double LevelMinOccup_dBm { get; set; }

        /// <summary>
        /// Spectrum occupancy mode
        /// </summary>
        [DataMember]
        public SpectrumOccupationType Type { get; set; }

        /// <summary>
        /// Number of measurements to be performed within a channel
        /// </summary>
        [DataMember]
        public int MeasurmentNumber { get; set; }
    }
}
