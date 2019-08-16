using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Device
{
    /// <summary>
    /// Represents the antenna of sensor for measurement. Includes administrative and technical data
    /// </summary>
    [DataContract(Namespace = Specification.Namespace)]
    [Serializable]
    public class SensorAntenna
    {
        /// <summary>
        /// Code of antenna
        /// </summary>
        [DataMember]
        public string Code { get; set; }

        /// <summary>
        /// Slew angle, degrees
        /// </summary>
        [DataMember]
        public double? SlewAng { get; set; }

        /// <summary>
        /// Manufacturer of antenna
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
        /// Directivity of antenna (D -  directional, ND - non-directional)
        /// </summary>
        [DataMember]
        public AntennaDirectivity Direction { get; set; }

        /// <summary>
        /// Horizontal beamwidth, degrees
        /// </summary>
        [DataMember]
        public double? HBeamwidth { get; set; }

        /// <summary>
        /// Vertical beamwidth, degrees
        /// </summary>
        [DataMember]
        public double? VBeamwidth { get; set; }

        /// <summary>
        /// Polarization (V - vertical, H - horizontal, M - mixed)
        /// </summary>
        [DataMember]
        public AntennaPolarization Polarization { get; set; }

        /// <summary>
        /// Use type
        /// </summary>
        [DataMember]
        public string UseType { get; set; }

        /// <summary>
        /// Category
        /// </summary>
        [DataMember]
        public string Category { get; set; }

        /// <summary>
        /// Type of gain (D - dipole, V - vertical, I - isotropic)
        /// </summary>
        [DataMember]
        public string GainType { get; set; }

        /// <summary>
        /// Maximum gain of antenna, dB
        /// </summary>
        [DataMember]
        public double GainMax { get; set; }

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
        /// Additional loss, dB
        /// </summary>
        [DataMember]
        public double AddLoss { get; set; }

        /// <summary>
        /// Cross-polarization discrimination, dB
        /// </summary>
        [DataMember]
        public double? XPD { get; set; }

        /// <summary>
        /// Class of antenna
        /// </summary>
        [DataMember]
        public string Class { get; set; }

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
        public double CustNbr1 { get; set; }

        /// <summary>
        /// Antenna patterns depending on frequency 
        /// </summary>
        [DataMember]
        public AntennaPattern[] Patterns { get; set; }
    }
}
