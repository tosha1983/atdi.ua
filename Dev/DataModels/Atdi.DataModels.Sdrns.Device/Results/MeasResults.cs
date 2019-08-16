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
    [Serializable]
    public class MeasResults
    {
        /// <summary>
        /// Identifier sensor ID
        /// </summary>
        [DataMember]
        public int? SensorId { get; set; }

        /// <summary>
        /// Results identifier (SpectrumOccupation, Level, BandwidthMeas, MonitoringStations, Spectr)
        /// </summary>
        [DataMember]
        public string ResultId { get; set; }

        ///// <summary>
        ///// Time start (SpectrumOccupation, Level, BandwidthMeas)
        ///// </summary>
        [DataMember]
        public DateTime StartTime { get; set; }

        ///// <summary>
        ///// Time stop  (SpectrumOccupation, Level, BandwidthMeas)
        ///// </summary>
        [DataMember]
        public DateTime StopTime { get; set; }

        /// <summary>
        /// Task identifier (SpectrumOccupation, Level, BandwidthMeas, MonitoringStations, Spectr)
        /// </summary>
        [DataMember]
        public string TaskId { get; set; }

        /// <summary>
        /// Time of measurement finish (for mobile measurement complexes it's a result sending time) 
        /// (SpectrumOccupation, Level, BandwidthMeas, MonitoringStations, Spectr)
        /// </summary>
        [DataMember]
        public DateTime Measured { get; set; }

        /// <summary>
        /// Result status (SpectrumOccupation, Level, BandwidthMeas, MonitoringStations, Spectr)
        /// </summary>
        [DataMember]
        public string Status { get; set; }

        ///// <summary>
        ///// Number of measurements, used for SO
        ///// (SpectrumOccupation)
        ///// </summary>
        [DataMember]
        public int ScansNumber { get; set; }

        /// <summary>
        /// Number of scans at a time. 
        /// (MonitoringStations)
        /// </summary>
        [DataMember]
        public int SwNumber { get; set; }

        ///// <summary>
        ///// Geolocation (SpectrumOccupation, Level, BandwidthMeas,Specter)
        ///// </summary>
        [DataMember]
        public GeoLocation Location { get; set; }

        ///// <summary>
        ///// Measurement type (SpectrumOccupation, Level, BandwidthMeas, MonitoringStations,Specter)
        ///// </summary>
        [DataMember]
        public MeasurementType Measurement { get; set; }

        ///// <summary>
        ///// Frequency samples 
        ///// (Level, SpectrumOccupation)
        ///// </summary>
        [DataMember]
        public FrequencySample[] FrequencySamples { get; set; }

        ///// <summary>
        ///// Frequencies
        /////(BandwidthMeas, Specter)
        ///// </summary>
        [DataMember]
        public double[] Frequencies { get; set; }

        ///// <summary>
        ///// Signal levels, dBm
        /////(BandwidthMeas, Specter)
        ///// </summary>
        [DataMember]
        public float[] Levels_dBm { get; set; }

        /// <summary>
        /// Station measurement results
        /// (MonitoringStations)
        /// </summary>
        [DataMember]
        public StationMeasResult[] StationResults { get; set; }

        ///// <summary>
        ///// Bandwidth measurement result
        ///// (BandwidthMeas)
        ///// </summary>
        [DataMember]
        public BandwidthMeasResult BandwidthResult { get; set; }

        /// <summary>
        /// Routes during measurements
        /// (MonitoringStations)
        /// </summary>
        [DataMember]
        public Route[] Routes { get; set; }

        /// <summary>
        /// Reference levels
        /// </summary>
        [DataMember]
        public ReferenceLevels RefLevels  { get; set; }

        /// <summary>
        /// Emittings
        /// </summary>
        [DataMember]
        public Emitting[] Emittings { get; set; }

        ///// <summary>
        ///// Signaling. Parameters of emissions above predefined level. 
        ///// </summary>
        //[DataMember]
        //public SignalingSample[] SignalingSamples { get; set; }

    }
}
