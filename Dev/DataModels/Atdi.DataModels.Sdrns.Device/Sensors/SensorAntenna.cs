using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Device
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
        public double? SlewAng { get; set; }

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
        public AntennaDirectional Direction { get; set; }

        /// <summary>
        /// Horizontal Beamwidth, degree
        /// </summary>
        [DataMember]
        public double? HBeamwidth { get; set; }

        /// <summary>
        /// Vertical Beamwidth, degree
        /// </summary>
        [DataMember]
        public double? VBeamwidth { get; set; }

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
        public string GainType { get; set; }

        /// <summary>
        /// Maximum gain of antenna, dB
        /// </summary>
        [DataMember]
        public double GainMax { get; set; }

        /// <summary>
        /// Lower Frequency, MHz
        /// </summary>
        [DataMember]
        public double? LowerFreq { get; set; }

        /// <summary>
        /// Upper Frequency, MHz
        /// </summary>
        [DataMember]
        public double? UpperFreq { get; set; }

        /// <summary>
        /// Additional loss, dB
        /// </summary>
        [DataMember]
        public double AddLoss { get; set; }

        /// <summary>
        /// XPD, dB
        /// </summary>
        [DataMember]
        public double? XPD { get; set; }

        /// <summary>
        /// Class of antenna
        /// </summary>
        [DataMember]
        public string Class { get; set; }

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
        public double CustNbr1 { get; set; }

        /// <summary>
        /// Patterns of antenna depends from frequency 
        /// </summary>
        [DataMember]
        public AntennaPattern[] Patterns { get; set; }
    }
}
