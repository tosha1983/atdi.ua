using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Device
{
    /// <summary>
    /// Measurements general result
    /// </summary>
    [DataContract(Namespace = Specification.Namespace)]
    public class MeasResults
    {
        /// <summary>
        /// Instance name of SDRN Server, which puts a measurement task
        /// </summary>
        [DataMember]
        public string SdrnServer { get; set; }

        /// <summary>
        /// Sensor name
        /// </summary>
        [DataMember]
        public string SensorName { get; set; }

        /// <summary>
        /// Equipment technical ID
        /// </summary>
        [DataMember]
        public string EquipmentTechId { get; set; }

        /// <summary>
        /// Task identifier
        /// </summary>
        [DataMember]
        public string TaskId { get; set; }

        /// <summary>
        /// Time of measurement finish (for mobile measurement complexes it's a result sending time)
        /// </summary>
        [DataMember]
        public DateTime Measured { get; set; }

        /// <summary>
        /// Object status
        /// </summary>
        [DataMember]
        public string Status { get; set; }

        /// <summary>
        /// Number of measurements, used for SO
        /// </summary>
        [DataMember]
        public int ScansSONumber { get; set; }

        /// <summary>
        /// Number of scans at a time.
        /// </summary>
        [DataMember]
        public int SwNumber { get; set; }

        /// <summary>
        /// Geolocation
        /// </summary>
        [DataMember]
        public GeoLocation Location { get; set; }

        /// <summary>
        /// Measurement type 
        /// </summary>
        [DataMember]
        public MeasurementType Measurement { get; set; }

        /// <summary>
        /// Frequency samples
        /// </summary>
        [DataMember]
        public FrequencySample[] FrequencySamples { get; set; }

        /// <summary>
        /// Frequencies
        /// </summary>
        [DataMember]
        public float[] Frequencies { get; set; }

        /// <summary>
        /// Signal levels, dBm
        /// </summary>
        [DataMember]
        public float[] Levels_dBm { get; set; }

        /// <summary>
        /// Station measurement results
        /// </summary>
        [DataMember]
        public StationMeasResult[] StationResults { get; set; }

        /// <summary>
        /// Bandwidth measurement result
        /// </summary>
        [DataMember]
        public BandwidthMeasResult BandwidthResult { get; set; }
    }
}
