using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using COMRMSI = Atdi.DataModels.Sdrn.DeviceServer.Commands.Results.MesureSystemInfo;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Adapters.RSTSMx.LTE
{
    public class BTSData
    {
        #region 
        public uint FreqIndex;

        public uint FreqBtsIndex;

        #region LTE_Channel
        public int EARFCN_DN;

        public int EARFCN_UP;

        public decimal FreqUp;

        public decimal FreqDn;


        public string StandartSubband;
        #endregion

        public decimal Bandwidth;

        public int MCC;

        public int MNC;

        public int TAC;

        public int CelId28;

        public int eNodeBId;

        public int CID;

        public int PCI;

        public double WB_RS_RSSI;

        public double RSRP;
               
        public double Strenght;

        public double WB_RSRP;

        public double RSRQ;

        public double WB_RSRQ;

        public string MIMO_2x2;

        public string eNodeB_Name;

        public double CIRofCP;

        public string ECI20p8;

        public string ECI28;

        public string ECGI;

        public string GCID;

        public bool FullData;

        public long LastLevelUpdete;

        public bool ThisIsMaximumSignalAtThisFrequency;

        #endregion
        public COMRMSI.SystemInformationBlock[] SysInfoBlocks;

        public COMRMSI.StationSystemInfo StationSysInfo;


        public void GetStationInfo()
        {
            StationSysInfo = new COMRMSI.StationSystemInfo()
            {
                Freq_Hz = FreqDn,
                Standart = "LTE",
                BandWidth_Hz = (double)Bandwidth,
                Level_dBm = RSRP,
                GCID = GCID,
                CID = CID,
                MCC = MCC,
                MNC = MNC,
                Time = LastLevelUpdete,
                
                SysInfoBlocks = SysInfoBlocks,
                BaseId = null,
                BSIC = null,
                ChannelNumber = EARFCN_DN,                
                CodePower = null,
                CtoI = null,
                ECI = CelId28,
                eNodeBid = eNodeBId,
                IcIo = null,
                InbandPower = null,
                ISCP = null,
                LAC = null,                
                NID = null,
                PCI = PCI,
                PN = null,
                Power = null,
                PTotal = null,
                RNC = null,
                RSCP = null,
                RSRP = RSRP,
                RSRQ = RSRQ,
                SC = null,
                SID = null,
                TAC = TAC,
                TypeCDMAEBDO = "",
                UCID = null                
            };
        }
    }
}
