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
        public string Code;
        /// <summary>
        /// Manufacturer
        /// </summary>
        [DataMember]
        public string Manufacturer;
        /// <summary>
        /// Name
        /// </summary>
        [DataMember]
        public string Name;
        /// <summary>
        /// Family
        /// </summary>
        [DataMember]
        public string Family;
        /// <summary>
        /// Technical ID
        /// </summary>
        [DataMember]
        public string TechId;
        /// <summary>
        /// Version
        /// </summary>
        [DataMember]
        public string Version;
        /// <summary>
        /// LowerFreq, MHz
        /// </summary>
        [DataMember]
        public Double? LowerFreq;  
        /// <summary>
        /// UpperFreq, MHz 
        /// </summary>
        [DataMember]
        public Double? UpperFreq;  
        /// <summary>
        /// RBWMin, kHz
        /// </summary>
        [DataMember]
        public Double? RBWMin;
        /// <summary>
        /// RBWMax, kHz
        /// </summary>
        [DataMember]
        public Double? RBWMax; 
        /// <summary>
        /// VBWMin, kHz
        /// </summary>
        [DataMember]
        public Double? VBWMin; 
        /// <summary>
        /// VBWMax, kHz
        /// </summary>
        [DataMember]
        public Double? VBWMax; 
        /// <summary>
        /// Mobility - true - can be mobile, false cannot be mobile
        /// </summary>
        [DataMember]
        public Boolean? Mobility; //true - can be mobile, false cannot be mobile
        /// <summary>
        /// Maximum of points for Fast Fourier Transform
        /// </summary>
        [DataMember]
        public Double? FFTPointMax; //Maximum of points for Fast Fourier Transform
        /// <summary>
        /// Reference level in dBm
        /// </summary>
        [DataMember]
        public Double? RefLeveldBm; 
        /// <summary>
        /// OperationMode
        /// </summary>
        [DataMember]
        public string OperationMode;
        /// <summary>
        /// Type
        /// </summary>
        [DataMember]
        public string Type;
        /// <summary>
        /// EquipClass
        /// </summary>
        [DataMember]
        public string EquipClass;
        /// <summary>
        /// TuningStep, Hz
        /// </summary>
        [DataMember]
        public Double? TuningStep;
        /// <summary>
        /// UseType
        /// </summary>
        [DataMember]
        public string UseType;
        /// <summary>
        /// Category
        /// </summary>
        [DataMember]
        public string Category;
        /// <summary>
        /// Remark
        /// </summary>
        [DataMember]
        public string Remark;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string CustTxt1;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public DateTime? CustData1;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public Double? CustNbr1;
        /// <summary>
        /// Sensitivities of equpment of sensor
        /// </summary>
        [DataMember]
        public SensorEquipSensitivity[] SensorEquipSensitivities;
    }
}
