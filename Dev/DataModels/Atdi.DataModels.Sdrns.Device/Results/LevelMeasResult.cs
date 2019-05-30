using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Device
{
    /// <summary>
    /// Contains the signal level measurement result, obtained in one point from some transmitter
    /// </summary>
    [DataContract(Namespace = Specification.Namespace)]
    [Serializable]
    public class LevelMeasResult
    {
        /// <summary>
        /// Geolocation
        /// </summary>
        [DataMember]
        public GeoLocation Location { get; set; }

        /// <summary>
        /// Signal level measured within the channel band, dBm
        /// </summary>
        [DataMember]
        public double? Level_dBm { get; set; }

        /// <summary>
        /// Signal level measured within the channel band, dBuV/m
        /// </summary>
        [DataMember]
        public double? Level_dBmkVm { get; set; }

        /// <summary>
        /// Time of the measurement result obtain
        /// </summary>
        [DataMember]
        public DateTime MeasurementTime { get; set; }

        /// <summary>
        /// Difference with GPS time, ns
        /// </summary>
        [DataMember]
        public double? DifferenceTimeStamp_ns { get; set; }
    }
}
