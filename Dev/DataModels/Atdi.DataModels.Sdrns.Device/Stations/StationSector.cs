using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Device
{
    /// <summary>
    /// Параметры сетора станции
    /// </summary>
    [DataContract(Namespace = Specification.Namespace)]
    public class StationSector
    {
        /// <summary>
        /// В ICSM
        /// </summary>
        [DataMember]
        public string SectorId { get; set; }

        /// <summary>
        /// м, высота над уровнем земли
        /// </summary>
        [DataMember]
        public double? AGL { get; set; }

        /// <summary>
        /// дБм
        /// </summary>
        [DataMember]
        public double? EIRP_dBm { get; set; }

        /// <summary>
        /// град
        /// </summary>
        [DataMember]
        public double? Azimut { get; set; }
        
        /// <summary>
        /// kHz
        /// </summary>
        [DataMember]
        public double? BW_kHz { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string ClassEmission { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public SectorFrequency[] Frequencies { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public ElementsMask[] BWMask { get; set; }
    }
}
