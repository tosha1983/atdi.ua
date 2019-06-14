using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using COMRMSI = Atdi.DataModels.Sdrn.DeviceServer.Commands.Results.MesureSystemInfo;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Adapters.RSTSMx.UMTS
{
    public class BTSData
    {
        #region 
        public uint FreqIndex;

        #region UMTS_Channel
        public int UARFCN_DN;

        public int UARFCN_UP;

        public decimal FreqUp;

        public decimal FreqDn;

        public string StandartSubband;


        #endregion

        public double TimeOfSlotInSec;

        public double TimeOfSlotInSecssch;

        public int SC;

        public int MCC;

        public int MNC;

        public int LAC;

        public int UCID;

        public int RNC;

        public int CID;

        public string GCID;

        public double ISCP;

        public double RSCP;

        /// <summary>
        /// напряженность этого сигнала
        /// </summary>
        public double Strenght;

        /// <summary>
        /// P total
        /// </summary>
        public double InbandPower;

        public double CodePower;

        public double IcIo;

        /// <summary>
        /// определяет есть ли GCID
        /// </summary>
        public bool FullData;

        public long LastLevelUpdete = 0;

        public long LastDetectionLevelUpdete;

        public bool ThisIsMaximumSignalAtThisFrequency;
        #endregion


        public COMRMSI.SystemInformationBlock[] SysInfoBlocks;

        public COMRMSI.StationSystemInfo StationSysInfo;

        public void GetStationInfo()
        {
            StationSysInfo = new COMRMSI.StationSystemInfo()
            {
                Freq_Hz = FreqDn,
                Standart = "UMTS",
                BandWidth_Hz = 4850000,
                Level_dBm = RSCP,
                GCID = GCID,
                CID = CID,
                MCC = MCC,
                MNC = MNC,
                Time = LastLevelUpdete,

                SysInfoBlocks = SysInfoBlocks,
                BaseId = null,
                BSIC = null,
                ChannelNumber = UARFCN_DN,
                CodePower = CodePower,
                CtoI = null,
                ECI = null,
                eNodeBid = null,
                IcIo = IcIo,
                InbandPower = InbandPower,
                ISCP = ISCP,
                LAC = LAC,
                NID = null,
                PCI = null,
                PN = null,
                Power = null,
                PTotal = InbandPower,
                RNC = RNC,
                RSCP = RSCP,
                RSRP = null,
                RSRQ = null,
                SC = SC,
                SID = null,
                TAC = null,
                TypeCDMAEBDO = "",
                UCID = UCID
            };
        }
    }
}
