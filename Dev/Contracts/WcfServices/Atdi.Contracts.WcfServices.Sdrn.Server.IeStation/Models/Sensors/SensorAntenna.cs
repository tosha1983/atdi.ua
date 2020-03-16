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
    /// Represents antenna of sensor for measurement. Includes administrative and technical data.
    /// </summary>
    [DataContract(Namespace = Specification.Namespace)]
    public class SensorAntenna
    {
        /// <summary>
        /// Code of Antenna
        /// </summary>
        [DataMember]
        public string Code { get; set; }
        /// <summary>
        /// SlewAng, degree
        /// </summary>
        [DataMember]
        public Double? SlewAng { get; set; }
        /// <summary>
        /// Manufacturer
        /// </summary>
        [DataMember]
        public string Manufacturer { get; set; }
        /// <summary>
        /// Name of antenna
        /// </summary>
        [DataMember]
        public string Name { get; set; }
        /// <summary>
        /// Technical ID
        /// </summary>
        [DataMember]
        public string TechId { get; set; }
        /// <summary>
        /// Directional of antenna D -  directional, ND - not directional;
        /// </summary>
        [DataMember]
        public AntennaDirectional AntDir { get; set; }
        /// <summary>
        /// Horizontal Beamwidth, degree
        /// </summary>
        [DataMember]
        public Double? HBeamwidth { get; set; }
        /// <summary>
        /// Vertical Beamwidth, degree
        /// </summary>
        [DataMember]
        public Double? VBeamwidth { get; set; }
        /// <summary>
        /// Polarization V, H, M
        /// </summary>
        [DataMember]
        public AntennaPolarization Polarization { get; set; }
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
        /// Type of Gain D - dipole, V -vertical, I - isotropic
        /// </summary>
        [DataMember]
        public string GainType { get; set; } //D - dipole, V -vertical, I - isotropic
        /// <summary>
        /// Maximum gain of antenna, dB
        /// </summary>
        [DataMember]
        public Double GainMax { get; set; }
        /// <summary>
        /// Lower Frequency, MHz
        /// </summary>
        [DataMember]
        public Double? LowerFreq { get; set; }
        /// <summary>
        /// Upper Frequency, MHz
        /// </summary>
        [DataMember]
        public Double? UpperFreq { get; set; }
        /// <summary>
        /// Additional loss, dB
        /// </summary>
        [DataMember]
        public Double AddLoss { get; set; }
        /// <summary>
        /// XPD, dB
        /// </summary>
        [DataMember]
        public Double? XPD { get; set; }
        /// <summary>
        /// Class of antenna
        /// </summary>
        [DataMember]
        public string AntClass { get; set; }
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
        public string CustData1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public Double CustNbr1 { get; set; }
        /// <summary>
        /// Patterns of antenna depends from frequency 
        /// </summary>
        [DataMember]
        public AntennaPattern[] AntennaPatterns { get; set; }
    }
}
