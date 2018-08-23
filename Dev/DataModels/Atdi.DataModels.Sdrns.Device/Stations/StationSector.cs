using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Device
{
    /// <summary>
    /// Station sector parameters
    /// </summary>
    [DataContract(Namespace = Specification.Namespace)]
    public class StationSector
    {
        /// <summary>
        /// Sector ID from ICSM
        /// </summary>
        [DataMember]
        public string SectorId { get; set; }

        /// <summary>
        /// Altitude above ground level, m
        /// </summary>
        [DataMember]
        public double? AGL { get; set; }

        /// <summary>
        /// EIRP, dBm
        /// </summary>
        [DataMember]
        public double? EIRP_dBm { get; set; }

        /// <summary>
        /// Azimuth, degrees
        /// </summary>
        [DataMember]
        public double? Azimuth { get; set; }
        
        /// <summary>
        /// Bandwidth, kHz
        /// </summary>
        [DataMember]
        public double? BW_kHz { get; set; }

        /// <summary>
        /// Class of emission
        /// </summary>
        [DataMember]
        public string ClassEmission { get; set; }

        /// <summary>
        /// Sector frequencies
        /// </summary>
        [DataMember]
        public SectorFrequency[] Frequencies { get; set; }

        /// <summary>
        /// Spectrum mask
        /// </summary>
        [DataMember]
        public ElementsMask[] BWMask { get; set; }
    }
}
