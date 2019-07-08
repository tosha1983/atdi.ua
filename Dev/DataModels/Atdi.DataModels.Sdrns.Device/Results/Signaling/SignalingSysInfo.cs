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
        public decimal Freq_Hz;

        /// <summary>
        /// только GSM/UMTS/CDMA/EVDO/LTE/TETRA
        /// </summary>
        public string Standart;

        /// <summary>
        /// Полоса сигнала
        /// </summary>
        public double? BandWidth_Hz;

        /// <summary>
        /// Уровень сигнала во время Time
        /// </summary>
        public double? Level_dBm;

        /// <summary>
        /// Cell ID
        /// </summary>
        public int? CID;

        /// <summary>
        /// Mobile Country Code, все технологии кроме EVDO
        /// </summary>
        public int? MCC;

        /// <summary>
        /// Mobile Network Code, все технологии кроме EVDO
        /// </summary>
        public int? MNC;

        /// <summary>
        /// текушее время в тиках из TimeService когда обновился уровень сигнала Level_dBm
        /// </summary>
        public WorkTime[] WorkTimes;

        /// <summary>
        /// Только GSM
        /// </summary>
        public int? BSIC;

        /// <summary>
        /// Номер канала
        /// </summary>
        public int? ChannelNumber;

        /// <summary>
        /// GSM/UMTS
        /// </summary>
        public int? LAC;

        /// <summary>
        /// UMTS
        /// </summary>
        public int? RNC;

        /// <summary>
        /// CtoI C/I GSM отношение сигнал/шум
        /// </summary>
        public double? CtoI;

        /// <summary>
        /// Мощность излучаемая передатчик
        /// </summary>
        public double? Power;
    }
}   
