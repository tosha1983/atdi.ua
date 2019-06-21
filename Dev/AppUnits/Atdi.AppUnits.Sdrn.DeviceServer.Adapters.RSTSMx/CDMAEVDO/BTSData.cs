using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using COMRMSI = Atdi.DataModels.Sdrn.DeviceServer.Commands.Results.MesureSystemInfo;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Adapters.RSTSMx.CDMAEVDO
{
    public class BTSData
    {       
        public uint FreqIndex;

        public uint Indicator;

        public int Channel;

        public decimal FreqDn;

        public decimal FreqUp;

        public string StandartSubband;

        public double TimeOfSlotInSec;

        public double TimeOfSlotInSecssch;

        public uint Detected;

        public bool Type;

        public int PN;

        public int SID;

        public int NID;

        public int MCC;

        public int MNC;

        public int BaseID;

        public string GCID;

        public double RSCP;

        public double PTotal;

        public double AverageInbandPower;

        public double IcIo;

        public bool FullData;


        public long LastLevelUpdete;


        public bool ThisIsMaximumSignalAtThisFrequency;


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
                Standart = "CDMA",
                BandWidth_Hz = 1250000,
                Level_dBm = RSCP,
                GCID = GCID,
                CID = null,
                MCC = MCC,
                MNC = MNC,
                Time = LastLevelUpdete,

                SysInfoBlocks = SysInfoBlocks,
                BaseId = BaseID,
                BSIC = null,
                ChannelNumber = Channel,
                UCID = null,
                CodePower = null,
                CtoI = null,
                ECI = null,
                eNodeBid = null,                
                IcIo = IcIo,
                InbandPower = null,
                ISCP = null,
                LAC = null,                
                NID = NID,
                PCI = null,
                PN = PN,
                Power = null,
                PTotal = PTotal,
                RNC = null,
                RSCP = RSCP,
                RSRP = null,
                RSRQ = null,
                SC = null,
                SID = SID,
                TAC = null,
                TypeCDMAEBDO = "",
            };
            if (Type) { StationSysInfo.TypeCDMAEBDO = "EVDO"; }
            else { StationSysInfo.TypeCDMAEBDO = "CDMA"; }
            StationSysInfo.Standart = StationSysInfo.TypeCDMAEBDO;
            if (Type == false)
            {
                if (MCC > -1) StationSysInfo.MCC = MCC;
                if (MNC > -1) StationSysInfo.MNC = MNC;
            }            
        }
    }
}
