using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using Atdi.DataModels.Sdrns.Server;

namespace Atdi.Contracts.WcfServices.Sdrn.Server.IeStation
{
    /// <summary>
    /// Represents equipment of sensor for measurement. Includes administrative and technical data.
    /// </summary>
    [DataContract(Namespace = Specification.Namespace)]
    public class SensorEquip
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
        public Double? LowerFreq { get; set; }
        /// <summary>
        /// UpperFreq, MHz 
        /// </summary>
        [DataMember]
        public Double? UpperFreq { get; set; }
        /// <summary>
        /// RBWMin, kHz
        /// </summary>
        [DataMember]
        public Double? RBWMin { get; set; }
        /// <summary>
        /// RBWMax, kHz
        /// </summary>
        [DataMember]
        public Double? RBWMax { get; set; }
        /// <summary>
        /// VBWMin, kHz
        /// </summary>
        [DataMember]
        public Double? VBWMin { get; set; }
        /// <summary>
        /// VBWMax, kHz
        /// </summary>
        [DataMember]
        public Double? VBWMax { get; set; }
        /// <summary>
        /// Mobility - true - can be mobile, false cannot be mobile
        /// </summary>
        [DataMember]
        public Boolean? Mobility { get; set; } //true - can be mobile, false cannot be mobile
        /// <summary>
        /// Maximum of points for Fast Fourier Transform
        /// </summary>
        [DataMember]
        public Double? FFTPointMax { get; set; } //Maximum of points for Fast Fourier Transform
        /// <summary>
        /// Reference level in dBm
        /// </summary>
        [DataMember]
        public Double? RefLeveldBm { get; set; }
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
        public string EquipClass { get; set; }
        /// <summary>
        /// TuningStep, Hz
        /// </summary>
        [DataMember]
        public Double? TuningStep { get; set; }
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
        public Double? CustNbr1 { get; set; }
        /// <summary>
        /// Sensitivities of equpment of sensor
        /// </summary>
        [DataMember]
        public SensorEquipSensitivity[] SensorEquipSensitivities { get; set; }
    }
}
