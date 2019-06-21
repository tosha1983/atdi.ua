using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.DeviceServer.Commands.Results.MesureSystemInfo
{
    [Serializable]
    public class StationSystemInfo
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
        public double BandWidth_Hz; //полоса сигнала 
        /// <summary>
        /// Уровень сигнала во время Time
        /// </summary>
        public double Level_dBm;
        /// <summary>
        /// Global Cell ID
        /// </summary>
        public string GCID;
        /// <summary>
        /// Cell ID
        /// </summary>
        public int? CID;
        /// <summary>
        /// Mobile Country Code, все технологии кроме EVDO
        /// </summary>
        public int MCC;
        /// <summary>
        /// Mobile Network Code, все технологии кроме EVDO
        /// </summary>
        public int MNC;
        /// <summary>
        /// текушее время в тиках из TimeService когда обновился уровень сигнала Level_dBm
        /// </summary>
        public long Time;

        /// <summary>
        /// BaseID только CDMA/EVDO
        /// </summary>
        public int? BaseId;

        /// <summary>
        /// Только GSM
        /// </summary>
        public int? BSIC;
        /// <summary>
        /// Номер канала
        /// </summary>
        public int ChannelNumber;

        /// <summary>
        /// LTE
        /// </summary>
        public int? ECI;
        public int? eNodeBid;
        /// <summary>
        /// GSM/UMTS
        /// </summary>
        public int? LAC;
        /// <summary>
        /// CDMA/EVDO
        /// </summary>
        public int? NID;
        /// <summary>
        /// LTE
        /// </summary>
        public int? PCI;
        /// <summary>
        /// CDMA/EVDO
        /// </summary>
        public int? PN;
        /// <summary>
        /// UMTS
        /// </summary>
        public int? RNC;
        /// <summary>
        /// UMTS
        /// </summary>
        public int? SC;
        /// <summary>
        /// CDMA/EVDO
        /// </summary>
        public int? SID;
        /// <summary>
        /// LTE
        /// </summary>
        public int? TAC;
        /// <summary>
        /// UMTS
        /// </summary>
        public int? UCID;
        public double? CodePower;
        /// <summary>
        /// CtoI C/I GSM отношение сигнал/шум
        /// </summary>
        public double? CtoI;
        /// <summary>
        /// отношение сигнал/шум не тоже самое что CtoI
        /// </summary>
        public double? IcIo;
        public double? InbandPower;
        public double? ISCP;
        public double? Power;
        public double? PTotal;
        public double? RSCP;
        /// <summary>
        /// LTE
        /// </summary>
        public double? RSRP;
        /// <summary>
        /// LTE
        /// </summary>
        public double? RSRQ;
        public string TypeCDMAEBDO;

        public SystemInformationBlock[] SysInfoBlocks;
    }
}
