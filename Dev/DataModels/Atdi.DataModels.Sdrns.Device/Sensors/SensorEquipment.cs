using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Device
{
    /// <summary>
    /// Represents the equipment of sensor for measurement. Includes administrative and technical data
    /// </summary>
    [DataContract(Namespace = Specification.Namespace)]
    [Serializable]
    public class SensorEquipment
    {
        /// <summary>
        /// Code of equipment
        /// </summary>
        [DataMember]
        public string Code { get; set; }

        /// <summary>
        /// Equipment manufacturer
        /// </summary>
        [DataMember]
        public string Manufacturer { get; set; }

        /// <summary>
        /// Name of equipment
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// Equipment family
        /// </summary>
        [DataMember]
        public string Family { get; set; }

        /// <summary>
        /// Technical ID
        /// </summary>
        [DataMember]
        public string TechId { get; set; }

        /// <summary>
        /// Version
        /// </summary>
        [DataMember]
        public string Version { get; set; }

        /// <summary>
        /// Lower frequency, MHz
        /// </summary>
        [DataMember]
        public double? LowerFreq_MHz { get; set; }

        /// <summary>
        /// Upper frequency, MHz 
        /// </summary>
        [DataMember]
        public double? UpperFreq_MHz { get; set; }

        /// <summary>
        /// Minimal radio bandwidth, kHz
        /// </summary>
        [DataMember]
        public double? RBWMin_kHz { get; set; }

        /// <summary>
        /// Maximal radio bandwidth, kHz
        /// </summary>
        [DataMember]
        public double? RBWMax_kHz { get; set; }

        /// <summary>
        /// Minimal video bandwidth, kHz
        /// </summary>
        [DataMember]
        public double? VBWMin_kHz { get; set; }

        /// <summary>
        /// Maximal video bandwidth, kHz
        /// </summary>
        [DataMember]
        public double? VBWMax_kHz { get; set; }

        /// <summary>
        /// Mobility (True - can be mobile, False  - cannot be mobile)
        /// </summary>
        [DataMember]
        public bool? Mobility { get; set; }

        /// <summary>
        /// Maximum number of points for Fast Fourier Transform
        /// </summary>
        [DataMember]
        public double? FFTPointMax { get; set; }

        /// <summary>
        /// Maximum reference level, dBm
        /// </summary>
        [DataMember]
        public double? MaxRefLevel_dBm { get; set; }

        /// <summary>
        /// Operation mode
        /// </summary>
        [DataMember]
        public string OperationMode { get; set; }

        /// <summary>
        /// Type of equipment
        /// </summary>
        [DataMember]
        public string Type { get; set; }

        /// <summary>
        /// Equipment class
        /// </summary>
        [DataMember]
        public string Class { get; set; }

        /// <summary>
        /// Tuning step, Hz
        /// </summary>
        [DataMember]
        public double? TuningStep_Hz { get; set; }

        /// <summary>
        /// Use type
        /// </summary>
        [DataMember]
        public string UseType { get; set; }

        /// <summary>
        /// Category of equipment
        /// </summary>
        [DataMember]
        public string Category { get; set; }

        /// <summary>
        /// Remarks
        /// </summary>
        [DataMember]
        public string Remark { get; set; }

        /// <summary>
        /// Custom data, text
        /// </summary>
        [DataMember]
        public string CustTxt1 { get; set; }

        /// <summary>
        /// Custom data, datetime
        /// </summary>
        [DataMember]
        public DateTime? CustDate1 { get; set; }

        /// <summary>
        /// Custom data, number (double)
        /// </summary>
        [DataMember]
        public double? CustNbr1 { get; set; }

        /// <summary>
        /// Equipment sensitivities
        /// </summary>
        [DataMember]
        public EquipmentSensitivity[] Sensitivities { get; set; }

        /// <summary>
        /// List with Command that device 
        /// </summary>
        [DataMember]
        public CommandType[] AvailableDeviceCommand;

    }
    public enum CommandType
    {
        MesureGpsLocation,
        MesureTrace,
        MesureSysInfo,
        MesureIQStream,
        MesureDF,
        MesureRealTime,
        MesureSignalParameters,
        MesureAudio,
        MesureZeroSpan
    }
}
