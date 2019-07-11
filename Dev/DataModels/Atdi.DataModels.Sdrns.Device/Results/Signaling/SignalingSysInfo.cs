using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Device
{
    /// <summary>
    /// Presents system information for signaling
    /// </summary>
    [DataContract(Namespace = Specification.Namespace)]
    [Serializable]
    public class SignalingSysInfo
    {
        /// <summary>
        /// All Tech
        /// </summary>

        [DataMember]
        public decimal Freq_Hz { get; set; }

        /// <summary>
        /// только GSM/UMTS/CDMA/EVDO/LTE/TETRA
        /// </summary>
        [DataMember]
        public string Standard { get; set; }

        /// <summary>
        /// Полоса сигнала
        /// </summary>
        [DataMember]
        public double? BandWidth_Hz { get; set; }

        /// <summary>
        /// Уровень сигнала во время Time
        /// </summary>
        [DataMember]
        public double? Level_dBm { get; set; }

        /// <summary>
        /// Cell ID
        /// </summary>
        [DataMember]
        public int? CID { get; set; }

        /// <summary>
        /// Mobile Country Code, все технологии кроме EVDO
        /// </summary>
        [DataMember]
        public int? MCC { get; set; }

        /// <summary>
        /// Mobile Network Code, все технологии кроме EVDO
        /// </summary>
        public int? MNC { get; set; }

        /// <summary>
        /// текушее время в тиках из TimeService когда обновился уровень сигнала Level_dBm
        /// </summary>
        [DataMember]
        public WorkTime[] WorkTimes { get; set; }

        /// <summary>
        /// Только GSM
        /// </summary>
        [DataMember]
        public int? BSIC { get; set; }

        /// <summary>
        /// Номер канала
        /// </summary>
        [DataMember]
        public int? ChannelNumber { get; set; }

        /// <summary>
        /// GSM/UMTS
        /// </summary>
        [DataMember]
        public int? LAC { get; set; }

        /// <summary>
        /// UMTS
        /// </summary>
        [DataMember]
        public int? RNC { get; set; }

        /// <summary>
        /// CtoI C/I GSM отношение сигнал/шум
        /// </summary>
        [DataMember]
        public double? CtoI { get; set; }

        /// <summary>
        /// Мощность излучаемая передатчик
        /// </summary>
        [DataMember]
        public double? Power { get; set; }
    }
}   
