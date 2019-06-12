using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using COMRMSI = Atdi.DataModels.Sdrn.DeviceServer.Commands.Results.MesureSystemInfo;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Adapters.RSTSMx
{
    public class GSMBTSData
    {
        #region 

        public uint FreqIndex;

        #region GSM_Channel
        public int ARFCN;

        public decimal FreqUp;

        public decimal FreqDn;

        public string StandartSubband;
        #endregion


        public uint IndSCHInfo;

        public uint IndFirstSCHInfo;

        public double TimeOfSlotInSec;

        public double TimeOfSlotInSecssch;

        public int BSIC;

        public int MCC;

        public int MNC;

        public int LAC;

        public int CID;


        public string GCID;

        public double Power;

        /// <summary>
        /// напряженность этого сигнала
        /// </summary>
        public double Strenght;

        /// <summary>
        /// Carrier-to-Interference 
        /// </summary>
        public double CarToInt;

        public bool FullData;

        public long LastLevelUpdete = 0;

        public long LastDetectionLevelUpdete = 0;

        #endregion
       
        public bool ThisIsMaximumSignalAtThisFrequency = false;

        public COMRMSI.SystemInformationBlock[] SysInfoBlocks;

        public COMRMSI.StationSystemInfo StationSysInfo;

        /// <summary>
        /// пишет изфу в station_sys_info 
        /// </summary>
        public void GetStationInfo()
        {
            StationSysInfo = new COMRMSI.StationSystemInfo()
            {
                Freq_Hz = FreqDn,
                Standart = "GSM",
                BandWidth_Hz = 271000,
                Level_dBm = Power,
                GCID = GCID,
                CID = CID,
                MCC = MCC,
                MNC = MNC,
                Time = LastLevelUpdete,
                
                SysInfoBlocks = SysInfoBlocks,
                BaseId = null,
                BSIC = BSIC,
                ChannelNumber = ARFCN,
                CodePower = null,
                CtoI = CarToInt,
                ECI = null,
                eNodeBid = null,
                IcIo = null,
                InbandPower = null,
                ISCP = null,
                LAC = LAC,
                NID = null,
                PCI = null,
                PN = null,
                Power = Power,
                PTotal = null,
                RNC = null,
                RSCP = null,
                RSRP = null,
                RSRQ = null,
                SC = null,
                SID = null,
                TAC = null,
                TypeCDMAEBDO = "",
                UCID = null
            };
        }
    }
    
}
