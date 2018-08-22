using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Atdi.DataModels.Sdrns.Device
{
    /// <summary>
    /// Системная информация
    /// </summary>
    [DataContract(Namespace = Specification.Namespace)]
    public class StationSysInfo
    {
        /// <summary>
        /// Блоки с системной информации 
        /// </summary>
        [DataMember]
        public StationSysInfoBlock[] InfoBlocks { get; set; }
        /// <summary>
        /// Место где была получена данная информация
        /// </summary>
        [DataMember]
        public GeoLocation Location { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public double? Freq { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public double? BandWidth { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public double? RSRP { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public double? RSRQ { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public double? INBAND_POWER { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public int? MCC { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public int? MNC { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public int? TAC { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public int? eNodeBId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public int? CID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public int? ECI { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public int? PCI { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public int? BSIC { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public int? LAC { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public double? Power { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public double? CtoI { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public int? SC { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public int? UCID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public int? RNC { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public double? Ptotal { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public double? RSCP { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public double? ISCP { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public double? Code { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public double? IcIo { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public int? ChannelNumber { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string TypeCDMAEVDO { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public int? SID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public int? NID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public int? PN { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public int? BaseID { get; set; }
    }
}
