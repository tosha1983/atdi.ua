using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Atdi.AppServer.Contracts.Sdrns
{
    /// <summary>
    /// Represents main parameters of  sensor for measurement.
    /// </summary>
    [DataContract(Namespace = ServicesSpecification.Namespace)]
    public class ShortSensor
    {
        /// <summary>
        /// Id
        /// </summary>
        [DataMember]
        public SensorIdentifier Id;
        /// <summary>
        /// Status
        /// </summary>
        [DataMember]
        public string Status;
        /// <summary>
        /// Name
        /// </summary>
        [DataMember]
        public string Name;
        /// <summary>
        /// Administration;
        /// </summary>
        [DataMember]
        public string Administration;
        /// <summary>
        /// NetworkId;
        /// </summary>
        [DataMember]
        public string NetworkId;
        /// <summary>
        /// Bring into use date
        /// </summary>
        [DataMember]
        public DateTime? BiuseDate;
        /// <summary>
        /// End of use date
        /// </summary>
        [DataMember]
        public DateTime? EouseDate; 
        /// <summary>
        /// RxLoss, dB 
        /// </summary>
        [DataMember]
        public Double? RxLoss; //dB 
        /// <summary>
        /// DateCreated;
        /// </summary>
        [DataMember]
        public DateTime? DateCreated;
        /// <summary>
        /// CreatedBy;
        /// </summary>
        [DataMember]
        public string CreatedBy;
        /// <summary>
        /// Name of Antenna
        /// </summary>
        [DataMember]
        public string AntName;
        /// <summary>
        /// Manufacturer of Antenna 
        /// </summary>
        [DataMember]
        public string AntManufacturer;
        /// <summary>
        /// Maximum gain of Antenna, dB
        /// </summary>
        [DataMember]
        public Double? AntGainMax; 
        /// <summary>
        /// Code of Equipment
        /// </summary>
        [DataMember]
        public string EquipCode;
        /// <summary>
        /// Name of Equipment
        /// </summary>
        [DataMember]
        public string EquipName;
        /// <summary>
        /// Manufacturer of Equipment
        /// </summary>
        [DataMember]
        public string EquipManufacturer;
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
    }
}
