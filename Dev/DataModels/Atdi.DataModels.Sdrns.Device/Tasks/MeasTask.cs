using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Device
{
    /// <summary>
    /// 
    /// </summary>
    [DataContract(Namespace = Specification.Namespace)]
    public class MeasTask
    {
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public int Id { get; set; }

        /// <summary>
        /// The instance name of the SDRN Server that puts the task on measurement
        /// </summary>
        [DataMember]
        public string SdrnServer { get; set; }

        /// <summary>
        /// SO - spectrum occupation; LV - Level; FO - Offset; FR - Frequency; FM - Freq. Modulation; AM - Ampl. Modulation; BW	- Bandwidth Meas; BE - Bearing; SA - Sub Audio Tone; PR	- Program; PI - PI Code  (Hex Code identifying radio program); SI - Sound ID; LO	- Location;
        /// </summary>
        [DataMember]
        public MeasurementType Measurement { get; set; }

        /// <summary>
        /// Type of spectrums scan RT -  Real Time; SW - sweep
        /// </summary>
        [DataMember]
        public SpectrumScanType SpectrumScan { get; set; }

        /// <summary>
        /// Number of scans at a time. 
        /// </summary>
        [DataMember]
        public int SwNumber { get; set; }

        /// <summary>
        /// Дата начала сканирования 
        /// </summary>
        [DataMember]
        public DateTime StartTime { get; set; }

        /// <summary>
        /// длительность измерений
        /// </summary>
        [DataMember]
        public double Interval { get; set; }

        /// <summary>
        /// Дата конца сканирования
        /// </summary>
        [DataMember]
        public DateTime StopTime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public int Priority { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string Status { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public MeasuredFrequencies Frequencies { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public DeviceMeasParam DeviceParam { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public SpectrumOccupationMeasParam SOParam { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public LocationMeasParam[] LocationParams { get; set; }

        /// <summary>
        /// параметр который определяет какое количество сканирований надо еще произвести 
        /// </summary>
        [DataMember]
        public int ScanPerTaskNumber { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public MeasurementType[] MobEqipmentMeasurements { get; set; }

        /// <summary>
        /// список станций для проведения измерения
        /// </summary>
        [DataMember]
        public MeasuredStation[] Stations { get; set; }
    }
}
