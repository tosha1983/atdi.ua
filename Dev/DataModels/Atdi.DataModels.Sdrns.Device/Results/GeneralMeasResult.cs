using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Device
{
    /// <summary>
    /// Contains the results of transmitter measurements
    /// </summary>
    [DataContract(Namespace = Specification.Namespace)]
    public class GeneralMeasResult
    {
        /// <summary>
        /// Radio bandwidth, kHz
        /// </summary>
        [DataMember]
        public double? RBW_kHz { get; set; }

        /// <summary>
        /// Video bandwidth, kHz
        /// </summary>
        [DataMember]
        public double? VBW_kHz { get; set; }

        /// <summary>
        /// Central frequency according to the channeling plan, MHz
        /// </summary>
        [DataMember]
        public double? CentralFrequency_MHz { get; set; }

        /// <summary>
        /// Frequency measurement result, MHz
        /// </summary>
        [DataMember]
        public double? CentralFrequencyMeas_MHz { get; set; }

        /// <summary>
        /// Frequency offset, x10^-6
        /// </summary>
        [DataMember]
        public double? OffsetFrequency_mk { get; set; }

        /// <summary>
        /// Start spectrum frequency, MHz
        /// </summary>
        [DataMember]
        public decimal? SpectrumStartFreq_MHz { get; set; }

        /// <summary>
        /// Spectrum step, kHz
        /// </summary>
        [DataMember]
        public decimal? SpectrumSteps_kHz { get; set; }

        /// <summary>
        /// Spectrum levels, dBm
        /// </summary>
        [DataMember]
        public float[] LevelsSpectrum_dBm { get; set; }

        /// <summary>
        /// Signal mask
        /// </summary>
        [DataMember]
        public ElementsMask[] BWMask { get; set; }

        /// <summary>
        /// Bandwidth measurement result
        /// </summary>
        [DataMember]
        public BandwidthMeasResult BandwidthResult { get; set; }

        /// <summary>
        /// Frequency measurement duration, sec
        /// </summary>
        [DataMember]
        public double? MeasDuration_sec { get; set; }

        /// <summary>
        /// Measurement start time
        /// </summary>
        [DataMember]
        public DateTime? MeasStartTime { get; set; }

        /// <summary>
        /// Measurement finish time
        /// </summary>
        [DataMember]
        public DateTime? MeasFinishTime { get; set; }

        /// <summary>
        /// Station system information
        /// </summary>
        [DataMember]
        public StationSysInfo StationSysInfo { get; set; }


    }
}
