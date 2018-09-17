using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Device
{
    /// <summary>
    /// Represent the bearing of signal with additional data. 
    /// </summary>
    [DataContract(Namespace = Specification.Namespace)]
    public class DirectionFindingData
    {
        /// <summary>
        /// Geolocation
        /// </summary>
        [DataMember]
        public GeoLocation Location { get; set; }

        /// <summary>
        /// Direction Finding level measured, dBm
        /// </summary>
        [DataMember]
        public double? Level_dBm { get; set; }

        /// <summary>
        /// Direction Finding level measured, dBuV/m
        /// </summary>
        [DataMember]
        public double? Level_dBmkVm { get; set; }

        /// <summary>
        /// Time of the measurement result obtain
        /// </summary>
        [DataMember]
        public DateTime MeasurementTime { get; set; }

        /// <summary>
        /// Direction Finding Bandwidth, kHz
        /// </summary>
        [DataMember]
        public double? Bandwidth_kHz { get; set; }

        /// <summary>
        /// Direction Finding Quality, %
        /// </summary>
        [DataMember]
        public double? Quality { get; set; }

        /// <summary>
        /// Central frequency of emitting, MHz
        /// </summary>
        [DataMember]
        public double CentralFrequency_MHz { get; set; }

        /// <summary>
        /// Bearing to emitting, degree
        /// </summary>
        [DataMember]
        public double Bearing { get; set; }

        /// <summary>
        /// Antenna Azimut, degree
        /// </summary>
        [DataMember]
        public double? AntennaAzimut { get; set; }
    }
}
