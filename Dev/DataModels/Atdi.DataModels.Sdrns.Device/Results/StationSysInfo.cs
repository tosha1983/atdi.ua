using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Atdi.DataModels.Sdrns.Device
{
    /// <summary>
    /// System information
    /// </summary>
    [DataContract(Namespace = Specification.Namespace)]
    public class StationSysInfo
    {
        /// <summary>
        /// Blocks with the system information
        /// </summary>
        [DataMember]
        public StationSysInfoBlock[] InfoBlocks { get; set; }
        /// <summary>
        /// Geolocation, from which the information was obtained
        /// </summary>
        [DataMember]
        public GeoLocation Location { get; set; }
        /// <summary>
        /// Frequency
        /// </summary>
        [DataMember]
        public double? Freq { get; set; }
        /// <summary>
        /// Bandwidth
        /// </summary>
        [DataMember]
        public double? BandWidth { get; set; }
        /// <summary>
        /// RSCP
        /// </summary>
        [DataMember]
        public double? RSRP { get; set; }
        /// <summary>
        /// RSRQ
        /// </summary>
        [DataMember]
        public double? RSRQ { get; set; }
        /// <summary>
        /// Inband power
        /// </summary>
        [DataMember]
        public double? INBAND_POWER { get; set; }
        /// <summary>
        /// MCC
        /// </summary>
        [DataMember]
        public int? MCC { get; set; }
        /// <summary>
        /// MNC
        /// </summary>
        [DataMember]
        public int? MNC { get; set; }
        /// <summary>
        /// TAC
        /// </summary>
        [DataMember]
        public int? TAC { get; set; }
        /// <summary>
        /// eNodeBID
        /// </summary>
        [DataMember]
        public int? eNodeBId { get; set; }
        /// <summary>
        /// CID
        /// </summary>
        [DataMember]
        public int? CID { get; set; }
        /// <summary>
        /// ECI
        /// </summary>
        [DataMember]
        public int? ECI { get; set; }
        /// <summary>
        /// PCI
        /// </summary>
        [DataMember]
        public int? PCI { get; set; }
        /// <summary>
        /// BSIC
        /// </summary>
        [DataMember]
        public int? BSIC { get; set; }
        /// <summary>
        /// LAC
        /// </summary>
        [DataMember]
        public int? LAC { get; set; }
        /// <summary>
        /// Power
        /// </summary>
        [DataMember]
        public double? Power { get; set; }
        /// <summary>
        /// CTOI
        /// </summary>
        [DataMember]
        public double? CtoI { get; set; }
        /// <summary>
        /// SC
        /// </summary>
        [DataMember]
        public int? SC { get; set; }
        /// <summary>
        /// UCID
        /// </summary>
        [DataMember]
        public int? UCID { get; set; }
        /// <summary>
        /// RNC
        /// </summary>
        [DataMember]
        public int? RNC { get; set; }
        /// <summary>
        /// Total power
        /// </summary>
        [DataMember]
        public double? Ptotal { get; set; }
        /// <summary>
        /// RSCP
        /// </summary>
        [DataMember]
        public double? RSCP { get; set; }
        /// <summary>
        /// ISCP
        /// </summary>
        [DataMember]
        public double? ISCP { get; set; }
        /// <summary>
        /// Code
        /// </summary>
        [DataMember]
        public double? Code { get; set; }
        /// <summary>
        /// IcIo
        /// </summary>
        [DataMember]
        public double? IcIo { get; set; }
        /// <summary>
        /// Channel number
        /// </summary>
        [DataMember]
        public int? ChannelNumber { get; set; }
        /// <summary>
        /// CDMA EV-DO type
        /// </summary>
        [DataMember]
        public string TypeCDMAEVDO { get; set; }
        /// <summary>
        /// SID
        /// </summary>
        [DataMember]
        public int? SID { get; set; }
        /// <summary>
        /// NID
        /// </summary>
        [DataMember]
        public int? NID { get; set; }
        /// <summary>
        /// PN
        /// </summary>
        [DataMember]
        public int? PN { get; set; }
        /// <summary>
        /// Base station ID
        /// </summary>
        [DataMember]
        public int? BaseID { get; set; }
    }
}
