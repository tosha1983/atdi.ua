using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Device
{
    /// <summary>
    /// Represents equipment of sensor for measurement. Includes administrative and technical data.
    /// </summary>
    [DataContract(Namespace = Specification.Namespace)]

    public class SensorEquipment
    {
        /// <summary>
        /// code of equipment
        /// </summary>
        [DataMember]
        public string Code { get; set; }

        /// <summary>
        /// Manufacturer
        /// </summary>
        [DataMember]
        public string Manufacturer { get; set; }

        /// <summary>
        /// Name
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// Family
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
        /// LowerFreq, MHz
        /// </summary>
        [DataMember]
        public double? LowerFreq_MHz { get; set; }

        /// <summary>
        /// UpperFreq, MHz 
        /// </summary>
        [DataMember]
        public double? UpperFreq_MHz { get; set; }

        /// <summary>
        /// RBWMin, kHz
        /// </summary>
        [DataMember]
        public double? RBWMin_kHz { get; set; }

        /// <summary>
        /// RBWMax, kHz
        /// </summary>
        [DataMember]
        public double? RBWMax_kHz { get; set; }

        /// <summary>
        /// VBWMin, kHz
        /// </summary>
        [DataMember]
        public double? VBWMin_kHz { get; set; }

        /// <summary>
        /// VBWMax, kHz
        /// </summary>
        [DataMember]
        public double? VBWMax_kHz { get; set; }

        /// <summary>
        /// Mobility - true - can be mobile, false cannot be mobile
        /// </summary>
        [DataMember]
        public bool? Mobility { get; set; }

        /// <summary>
        /// Maximum of points for Fast Fourier Transform
        /// </summary>
        [DataMember]
        public double? FFTPointMax { get; set; }

        /// <summary>
        /// Reference level in dBm
        /// </summary>
        [DataMember]
        public double? RefLevel_dBm { get; set; }

        /// <summary>
        /// OperationMode
        /// </summary>
        [DataMember]
        public string OperationMode { get; set; }

        /// <summary>
        /// Type
        /// </summary>
        [DataMember]
        public string Type { get; set; }

        /// <summary>
        /// EquipClass
        /// </summary>
        [DataMember]
        public string Class { get; set; }

        /// <summary>
        /// TuningStep, Hz
        /// </summary>
        [DataMember]
        public double? TuningStep_Hz { get; set; }

        /// <summary>
        /// UseType
        /// </summary>
        [DataMember]
        public string UseType { get; set; }

        /// <summary>
        /// Category
        /// </summary>
        [DataMember]
        public string Category { get; set; }

        /// <summary>
        /// Remark
        /// </summary>
        [DataMember]
        public string Remark { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string CustTxt1 { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public DateTime? CustData1 { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public double? CustNbr1 { get; set; }

        /// <summary>
        /// Sensitivities of equpment of sensor
        /// </summary>
        [DataMember]
        public EquipmentSensitivity[] Sensitivities { get; set; }
    }
}
