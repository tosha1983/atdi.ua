using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Atdi.AppServer.Contracts.Sdrns
{
    /// <summary>
    /// Represents antenna of sensor for measurement. Includes administrative and technical data.
    /// </summary>
    [DataContract(Namespace = ServicesSpecification.Namespace)]
    public class SensorAntenna
    {
        /// <summary>
        /// Code of Antenna
        /// </summary>
        [DataMember]
        public string Code;
        /// <summary>
        /// SlewAng, degree
        /// </summary>
        [DataMember]
        public Double? SlewAng; 
        /// <summary>
        /// Manufacturer
        /// </summary>
        [DataMember]
        public string Manufacturer;
        /// <summary>
        /// Name of antenna
        /// </summary>
        [DataMember]
        public string Name;
        /// <summary>
        /// Technical ID
        /// </summary>
        [DataMember]
        public string TechId;
        /// <summary>
        /// Directional of antenna D -  directional, ND - not directional;
        /// </summary>
        [DataMember]
        public AntennaDirectional AntDir; 
        /// <summary>
        /// Horizontal Beamwidth, degree
        /// </summary>
        [DataMember]
        public Double? HBeamwidth;  
        /// <summary>
        /// Vertical Beamwidth, degree
        /// </summary>
        [DataMember]
        public Double? VBeamwidth;       
        /// <summary>
        /// Polarization V, H, M
        /// </summary>
        [DataMember]
        public AntennaPolarization Polarization;
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
        /// Type of Gain D - dipole, V -vertical, I - isotropic
        /// </summary>
        [DataMember]
        public string GainType; //D - dipole, V -vertical, I - isotropic
        /// <summary>
        /// Maximum gain of antenna, dB
        /// </summary>
        [DataMember]
        public Double GainMax; 
        /// <summary>
        /// Lower Frequency, MHz
        /// </summary>
        [DataMember]
        public Double? LowerFreq; 
        /// <summary>
        /// Upper Frequency, MHz
        /// </summary>
        [DataMember]
        public Double? UpperFreq;
        /// <summary>
        /// Additional loss, dB
        /// </summary>
        [DataMember]
        public Double AddLoss; 
        /// <summary>
        /// XPD, dB
        /// </summary>
        [DataMember]
        public Double? XPD;
        /// <summary>
        /// Class of antenna
        /// </summary>
        [DataMember]
        public string AntClass;
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
        public string CustData1;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public Double CustNbr1;
        /// <summary>
        /// Patterns of antenna depends from frequency 
        /// </summary>
        [DataMember]
        public AntennaPattern[] AntennaPatterns;
    }
}
