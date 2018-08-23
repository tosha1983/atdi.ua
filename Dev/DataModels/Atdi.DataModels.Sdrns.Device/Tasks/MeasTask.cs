using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Device
{
    /// <summary>
    /// Represents the measurement task
    /// </summary>
    [DataContract(Namespace = Specification.Namespace)]
    public class MeasTask
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
        /// Measurement type
        /// </summary>
        [DataMember]
        public MeasurementType Measurement { get; set; }

        /// <summary>
        /// Type of spectrum scan
        /// </summary>
        [DataMember]
        public SpectrumScanType SpectrumScan { get; set; }

        /// <summary>
        /// Number of scans at a time
        /// </summary>
        [DataMember]
        public int SwNumber { get; set; }

        /// <summary>
        /// Scanning start time
        /// </summary>
        [DataMember]
        public DateTime StartTime { get; set; }

        /// <summary>
        /// Measurements duration, sec
        /// </summary>
        [DataMember]
        public double Interval_sec { get; set; }

        /// <summary>
        /// Scanning stop time
        /// </summary>
        [DataMember]
        public DateTime StopTime { get; set; }

        /// <summary>
        /// Task priority
        /// </summary>
        [DataMember]
        public int Priority { get; set; }

        /// <summary>
        /// Task status
        /// </summary>
        [DataMember]
        public string Status { get; set; }

        /// <summary>
        /// Frequencies for scanning
        /// </summary>
        [DataMember]
        public MeasuredFrequencies Frequencies { get; set; }

        /// <summary>
        /// Measurement equipment parameters
        /// </summary>
        [DataMember]
        public DeviceMeasParam DeviceParam { get; set; }

        /// <summary>
        /// Spectrum occupancy measurement parameters
        /// </summary>
        [DataMember]
        public SpectrumOccupationMeasParam SOParam { get; set; }

        /// <summary>
        /// Parameters of location areas for measurement
        /// </summary>
        [DataMember]
        public LocationMeasParam[] LocationParams { get; set; }

        /// <summary>
        /// Parameter, which defines how many scans still need to be performed
        /// </summary>
        [DataMember]
        public int ScanPerTaskNumber { get; set; }

        /// <summary>
        /// Measurement types for mobile equipment
        /// </summary>
        [DataMember]
        public MeasurementType[] MobEqipmentMeasurements { get; set; }

        /// <summary>
        /// List of stations to be measured
        /// </summary>
        [DataMember]
        public MeasuredStation[] Stations { get; set; }
        /// <summary>
        /// Parameters of measurement equipment settings for measuring the stations of different standards
        /// </summary>
        [DataMember]
        public StandardScanParameter[] ScanParameters { get; set; }
    }
}
