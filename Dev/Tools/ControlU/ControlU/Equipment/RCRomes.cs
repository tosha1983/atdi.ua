using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace ControlU.Equipment
{
    public class RCRomes : INotifyPropertyChanged
    {
        static Connection tc;
        public Thread TelnetTr;
        public delegate void DoubMod();
        public DoubMod TelnetDM;


        static private string _Server = "127.0.0.1";//"192.168.1.2";
        public string Server
        {
            get { return _Server; }
            set { _Server = value; OnPropertyChanged("Server"); }
        }
        static private int _Port = 8084;//025;
        public int Port
        {
            get { return _Port; }
            set { _Port = value; OnPropertyChanged("Port"); }
        }
        bool _DataCycle;
        public bool DataCycle
        {
            get { return _DataCycle; }
            set { _DataCycle = value; OnPropertyChanged("DataCycle"); }
        }
        bool _Run = false;
        public bool Run
        {
            get { return _Run; }
            set
            {
                _Run = value;
                if (Run)
                {
                    DataCycle = true;
                    Connect();
                }
                else if (!Run)
                {
                    Disconnect();
                }
                OnPropertyChanged("Run");
            }
        }
        private bool _IsRuning;
        public bool IsRuning
        {
            get { return _IsRuning; }
            set { _IsRuning = value; OnPropertyChanged("IsRuning"); }
        }
        string _Time;
        public string Time
        {
            get { return _Time; }
            set { _Time = value; OnPropertyChanged("Time"); }
        }
        string _Time1;
        public string Time1
        {
            get { return _Time1; }
            set { _Time1 = value; OnPropertyChanged("Time1"); }
        }
        string _Time2;
        public string Time2
        {
            get { return _Time2; }
            set { _Time2 = value; OnPropertyChanged("Time2"); }
        }
        bool LoadData = false;
        string inputstr = "";

        public ObservableCollection<TopNGSM> _GSM = new ObservableCollection<TopNGSM> { };
        public ObservableCollection<TopNGSM> GSM
        {
            get { return _GSM; }
            set { _GSM = value; OnPropertyChanged("GSM"); }
        }
        public ObservableCollection<TopNUMTS> _UMTS = new ObservableCollection<TopNUMTS> { };
        public ObservableCollection<TopNUMTS> UMTS
        {
            get { return _UMTS; }
            set { _UMTS = value; OnPropertyChanged("UMTS"); }
        }
        public ObservableCollection<TopNCDMA> _CDMA = new ObservableCollection<TopNCDMA> { };
        public ObservableCollection<TopNCDMA> CDMA
        {
            get { return _CDMA; }
            set { _CDMA = value; OnPropertyChanged("CDMA"); }
        }
        public ObservableCollection<TopNWiMax> _WiMax = new ObservableCollection<TopNWiMax> { };
        public ObservableCollection<TopNWiMax> WiMax
        {
            get { return _WiMax; }
            set { _WiMax = value; OnPropertyChanged("WiMax"); }
        }

        /// <summary>
        ///  Wizard ACD smart GSM@900,1800 UMTS@1
        ///  Register Data ACD
        ///  SignalSet Load GSM_TopN.xml 1 2, SignalSet Load UMTS_TopN.xml 2 2
        ///  
        ///  SignalSet GSM_RFMon.xml 0 1, SignalSet UMTS_RFMon.xml 1 1
        ///  Register Data SIGNALSET
        /// 
        /// </summary>
        public RCRomes()
        {
            //CDMA_Bands = new ObservableCollection<CDMA_Band>
            //{
            //    new CDMA_Band() { },
            //};
        }
        public void SetStartMeas()
        {
            try
            {
                string instr;
                if (tc.IsOpen)
                {
                    instr = tc.Query("LdW d:\\RFMon.rsxks");
                    Time1 += "LdW " + instr;
                    if (instr == "OK")
                    {
                        instr = tc.Query("SignalSet Reset");

                        instr = tc.Query("SignalSet Load GSM_TopN.xml 0 2");
                        Time1 += "\r\nSignalSet " + instr;
                        if (instr == "OK")
                        {
                            instr = tc.Query("Register Data SIGNALSET");
                            Time1 += "\r\nRegister SignalSet " + instr;
                            //tc.Write("meas");
                        }
                        instr = tc.Query("SignalSet Load UMTS_TopN.xml 1 2");
                        Time1 += "\r\nSignalSet " + instr;
                        if (instr == "OK")
                        {
                            instr = tc.Query("Register Data SIGNALSET");
                            Time1 += "\r\nRegister SignalSet " + instr;
                            //tc.Write("meas");
                        }
                        instr = tc.Query("SignalSet Load CDMA_TopN.xml 2 2");
                        Time1 += "\r\nSignalSet " + instr;
                        if (instr == "OK")
                        {
                            instr = tc.Query("Register Data SIGNALSET");
                            Time1 += "\r\nRegister SignalSet " + instr;
                            //tc.Write("meas");
                        }
                        instr = tc.Query("SignalSet Load WiMax_TopN.xml 3 2");
                        Time1 += "\r\nSignalSet " + instr;
                        if (instr == "OK")
                        {
                            instr = tc.Query("Register Data SIGNALSET");
                            Time1 += "\r\nRegister SignalSet " + instr;
                            //tc.Write("meas");
                        }
                        if (instr == "OK")
                        { tc.Write("meas"); }

                        LoadData = true;
                        TelnetDM += sameWork;
                        MainWindow.Rcvr.TelnetDM += MainWindow.Rcvr.SetMeasMon;
                        MainWindow.Rcvr.IsMeasMon = true;
                    }
                }
            }
            #region Exception
            catch (Exception exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "ANALYZER", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            #endregion
            TelnetDM -= SetStartMeas;
        }
        public void SetStopMeas()
        {
            try
            {
                MainWindow.Rcvr.IsMeasMon = false;
                MainWindow.Rcvr.TelnetDM -= MainWindow.Rcvr.SetMeasMon;
                string instr;
                if (tc.IsOpen)
                {
                    instr = tc.Query("stp");
                    Time1 += "Stop " + instr;
                }
            }
            #region Exception
            catch (Exception exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "ANALYZER", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            #endregion
            TelnetDM -= sameWork;
            TelnetDM -= SetStopMeas;
        }

        public void Connect()
        {
            TelnetDM = SetConnect;
            TelnetTr = new Thread(AllTelnetTimeWorks);
            TelnetTr.Name = "RomesRemoteControl";
            TelnetTr.IsBackground = true;
            TelnetTr.Start();
            TelnetDM += sameWork;
        }
        public void Disconnect()
        {
            //tmr.Stop();
            _DataCycle = false;
        }
        private void AllTelnetTimeWorks()
        {
            while (_DataCycle)
            {
                long beginTiks = DateTime.Now.Ticks;
                IsRuning = true;
                TelnetDM();
                Thread.Sleep(100);
                //Time = new TimeSpan(DateTime.Now.Ticks - beginTiks).ToString();
                //if (TelnetDM != null)
                //    foreach (Delegate d in TelnetDM.GetInvocationList())
                //    {
                //        Time += "\r\n" + d.Method.Name.ToString();
                //    }
            }
            TelnetDM -= sameWork;
            string instr;
            if (tc.IsOpen)
            {
                instr = tc.Query("stp");
                Time1 += "Stop " + instr;
            }
            tc.Close();
            IsRuning = false;
            TelnetTr.Abort();
        }
        public void sameWork()
        {
            try
            {
                if (LoadData && tc.m_Stream.DataAvailable)
                {
                    string t = tc.Read().TrimEnd();
                    //Time2 = t;
                    if (t.Length > 0 && t.Contains("SIGNALSET0"))
                    {
                        ObservableCollection<GSMBTSData> temp = new ObservableCollection<GSMBTSData> { };
                        #region GSM
                        string[] s = t.Replace("Data SIGNALSET0 ", "").Split(';');
                        //Time2 = s.Length + "    " + "    %" + s[0] + "\r\n";
                        if (s.Length > 0)
                        {
                            for (int i = 0; i < (int)(s.Length / 7); i++)
                            {
                                int iARFCN = 0;
                                GSM_Channel iChennel = new GSM_Channel();
                                int iMNC = 0;
                                int iMCC = 0;
                                int iLAC = 0;
                                int iCI = 0;
                                int iBSIC = 0;
                                double iPtotal = 0;

                                Int32.TryParse(s[0 + i * 7], out iARFCN);
                                iChennel = GetFreqFromGSMARFCN(iARFCN, new List<int> { (int)GSMBands.P_GSM900, (int)GSMBands.E_GSM900, (int)GSMBands.R_GSM900, (int)GSMBands.ER_GSM900, (int)GSMBands.GSM1800 });
                                Int32.TryParse(s[1 + i * 7], out iMNC);
                                Int32.TryParse(s[2 + i * 7], out iMCC);
                                Int32.TryParse(s[3 + i * 7], out iLAC);
                                Int32.TryParse(s[4 + i * 7], out iCI);
                                Int32.TryParse(s[5 + i * 7], out iBSIC);
                                double.TryParse(s[6 + i * 7].Replace(".", ","), out iPtotal);
                                //iPtotal += 106.98;
                                string iGCID = iMCC.ToString() + " " + iMNC.ToString() + " " + iLAC.ToString() + " " + iCI.ToString();
                                bool iFullData = false;
                                if (iARFCN != 0 && iMNC != 0 && iMCC != 0 && iLAC != 0 && iCI != 0 & iBSIC != 0) { iFullData = true; }
                                temp.Add(new GSMBTSData()
                                {
                                    //ARFCN = iARFCN,
                                    //Channel = iChennel,
                                    ARFCN = iChennel.ARFCN,
                                    FreqDn = iChennel.FreqDn,
                                    FreqUp= iChennel.FreqUp,
                                    StandartSubband = iChennel.StandartSubband,
                                    MNC = iMNC,//.ToString(),
                                    MCC = iMCC,//.ToString(),
                                    LAC = iLAC,//.ToString(),
                                    CID = iCI,//.ToString(),
                                    BSIC = iBSIC,
                                    Power = iPtotal,
                                    FullData = iFullData,
                                    GCID = iGCID, 
                                });
                                //App.Current.Dispatcher.BeginInvoke((Action)(() =>
                                //{
                                //    if (iFullData)
                                //    {
                                //        bool find = false;
                                //        if (MainWindow.db.MonMeas.Count > 0)
                                //        {
                                //            foreach (DB.MeasData mg in MainWindow.db.MonMeas)
                                //            {
                                //                if (mg.GCID == iGCID/* && mg.ChannelN == iARFCN*/) { mg.Power = iPtotal; find = true; }
                                //            }
                                //        }
                                //        if (!find && iPtotal > App.Sett.MeasMons_Settings.MeasMons.Where(x => x.Techonology == DB.Technologys.GSM).First().DetectionLevel)
                                //        {
                                //            MainWindow.db.MonMeas.Add(new DB.MeasData()
                                //            {
                                //                MeasGuid = Guid.NewGuid(),
                                //                Techonology = DB.Technologys.GSM.ToString(),
                                //                StandartSubband = iChennel.StandartSubband,
                                //                FreqUP = iChennel.FreqUp,
                                //                FreqDN = iChennel.FreqDn,
                                //                MeasSpan = 500000,
                                //                MarPeakBW = 200000,
                                //                NdBBWMax = 400000,
                                //                NdBBWMin = 150000,
                                //                ChannelN = iChennel.ARFCN,
                                //                GCID = iGCID,
                                //                TechSubInd = "BSIC: " + iBSIC.ToString(),
                                //                FullData = iFullData,
                                //                Power = iPtotal,
                                //                TraceCount = 0,
                                //                LevelUnitStr = MainWindow.Rcvr.LevelUnitStr,
                                //                Trace = null
                                //            });
                                //        }
                                //    }
                                //}));
                            }
                        }
                        #endregion
                        IdentificationData.GSM.BTS = temp;
                        //System.Windows.MessageBox.Show("GSM " + GSM.Count.ToString());
                    }
                    else if (t.Length > 0 && t.Contains("SIGNALSET1"))
                    {
                        ObservableCollection<UMTSBTSData> temp = new ObservableCollection<UMTSBTSData> { };
                        #region UMTS
                        string[] s = t.Replace("Data SIGNALSET1 ", "").Split(';');
                        //Time2 = s.Length + "    " + "    %" + s[0] + "\r\n";
                        if (s.Length > 0)
                        {
                            for (int i = 0; i < (int)(s.Length / 11); i++)
                            {
                                int iSC = 0;
                                int iSCID = 0;
                                int iMNC = 0;
                                int iMCC = 0;
                                int iLAC = 0;
                                int iCI = 0;
                                double iPtotal = 0;
                                int iUARFCN = 0;
                                UMTS_Channel iChennel = new UMTS_Channel();
                                double iIcIo = 0;
                                double iRSCP = 0;
                                double iISCP = 0;
                                bool iFullData = false;
                                Int32.TryParse(s[0 + i * 11], out iSC);
                                Int32.TryParse(s[1 + i * 11], out iSCID);
                                Int32.TryParse(s[2 + i * 11], out iMNC);
                                Int32.TryParse(s[3 + i * 11], out iMCC);
                                Int32.TryParse(s[4 + i * 11], out iLAC);
                                Int32.TryParse(s[5 + i * 11], out iCI);
                                double.TryParse(s[6 + i * 11].Replace(".", ","), out iPtotal);
                                //iPtotal += 106.98m;
                                Int32.TryParse(s[7 + i * 11], out iUARFCN);
                                iChennel = GetFreqFromUMTSUARFCN(iUARFCN, new List<int> { (int)UMTSBands.Band_1_2100 });
                                double.TryParse(s[8 + i * 11].Replace(".", ","), out iIcIo);
                                double.TryParse(s[9 + i * 11].Replace(".", ","), out iRSCP);
                                //iRSCP += 106.98m;
                                double.TryParse(s[10 + i * 11].Replace(".", ","), out iISCP);
                                //iISCP += 106.98m;
                                string iGCID = iMCC.ToString() + " " + iMNC.ToString() + " " + iLAC.ToString() + " " + iCI.ToString();
                                if (iSC != 0 && iMNC != 0 && iMCC != 0 && iLAC != 0 && iCI != 0 && iUARFCN != 0) { iFullData = true; }
                                temp.Add(new UMTSBTSData()
                                {
                                    SC = iSC,
                                    UCID = iSCID,
                                    MNC = iMNC,
                                    MCC = iMCC,
                                    LAC = iLAC,
                                    CID = iCI,
                                    InbandPower = iPtotal,
                                    //UARFCN = iUARFCN,
                                    UARFCN_DN = iChennel.UARFCN_DN,
                                    UARFCN_UP = iChennel.UARFCN_UP,
                                    FreqDn = iChennel.FreqDn,
                                    FreqUp = iChennel.FreqUp,
                                    StandartSubband = iChennel.StandartSubband,
                                    IcIo = iIcIo,
                                    RSCP = iRSCP,
                                    ISCP = iISCP,
                                    FullData = iFullData,
                                    GCID = iGCID
                                });
                                //App.Current.Dispatcher.BeginInvoke((Action)(() =>
                                //{
                                //    if (iFullData)
                                //    {
                                //        bool find = false;
                                //        if (MainWindow.db.MonMeas.Count > 0)
                                //        {
                                //            foreach (DB.MeasData mg in MainWindow.db.MonMeas)
                                //            {
                                //                if (mg.GCID == iGCID/* && mg.ChannelN ==iChennel.UARFCN*/) { mg.Power = iRSCP; find = true; }
                                //            }
                                //        }
                                //        if (!find && iRSCP > App.Sett.MeasMons_Settings.MeasMons.Where(x => x.Techonology == DB.Technologys.WCDMA).First().DetectionLevel)
                                //        {
                                //            MainWindow.db.MonMeas.Add(new DB.MeasData()
                                //            {
                                //                MeasGuid = Guid.NewGuid(),
                                //                Techonology = DB.Technologys.WCDMA.ToString(),
                                //                StandartSubband = iChennel.StandartSubband,
                                //                FreqUP = iChennel.FreqUp,
                                //                FreqDN = iChennel.FreqDn,
                                //                MeasSpan = 5000000,
                                //                MarPeakBW = 4800000,
                                //                NdBBWMax = 4850000,
                                //                NdBBWMin = 4000000,
                                //                ChannelN = iChennel.UARFCN_DN,
                                //                GCID = iGCID,
                                //                TechSubInd = "SC: " + iSC.ToString(),
                                //                FullData = iFullData,
                                //                Power = iRSCP,
                                //                TraceCount = 0,
                                //                LevelUnitStr = MainWindow.Rcvr.LevelUnitStr
                                //            });
                                //        }
                                //    }
                                //}));
                            }
                        }
                        #endregion
                        IdentificationData.UMTS.BTS = temp;
                    }
                    else if (t.Length > 0 && t.Contains("SIGNALSET2"))
                    {
                        ObservableCollection<TopNCDMA> temp = new ObservableCollection<TopNCDMA> { };
                        #region CDMA
                        string[] s = t.Replace("Data SIGNALSET2 ", "").Split(';');
                        Time2 = t;// s.Length + "    " + "    %" + s[0] + "\r\n";
                        if (s.Length > 0)
                        {
                            for (int i = 0; i < (int)(s.Length / 16); i++)
                            {
                                int iRank = 0;
                                int iMeasId = 0;
                                decimal iFreq = 0;
                                int iPNOffset = 0;
                                int iSID = 0;
                                int iNID = 0;
                                int iBaseID = 0;
                                decimal iT_ADD = 0;
                                decimal iT_DROP = 0;
                                decimal iT_COMP = 0;
                                decimal iT_TDROP = 0;
                                double iIcIo = 0;
                                double iRSCP = 0;
                                double iPTotal = 0;
                                string iBTSName = "";
                                decimal iBTSDist = 0;

                                CDMA_Channel iChennel = new CDMA_Channel();
                                bool iFullData = false;
                                Int32.TryParse(s[0 + i * 16], out iRank);
                                Int32.TryParse(s[1 + i * 16], out iMeasId);
                                decimal.TryParse(s[2 + i * 16].Split(' ')[0].Replace(".", ","), out iFreq); //decimal.TryParse(s[2 + i * 16], out iFreq);decimal.TryParse(s[2 + i * 16].Split(' ')[0].Replace(".", ","), out iFreq);
                                Int32.TryParse(s[3 + i * 16], out iPNOffset);
                                Int32.TryParse(s[4 + i * 16], out iSID);
                                Int32.TryParse(s[5 + i * 16], out iNID);
                                Int32.TryParse(s[6 + i * 16], out iBaseID);
                                decimal.TryParse(s[7 + i * 16].Replace(".", ","), out iT_ADD);
                                decimal.TryParse(s[8 + i * 16].Replace(".", ","), out iT_DROP);
                                decimal.TryParse(s[9 + i * 16].Replace(".", ","), out iT_COMP);
                                decimal.TryParse(s[10 + i * 16].Replace(".", ","), out iT_TDROP);
                                double.TryParse(s[11 + i * 16].Replace(".", ","), out iIcIo);
                                double.TryParse(s[12 + i * 16].Replace(".", ","), out iRSCP);
                                //iRSCP += 106.98m;
                                double.TryParse(s[13 + i * 16].Replace(".", ","), out iPTotal);
                               // iPTotal += 106.98m;
                                iBTSName = s[14 + i * 16];
                                decimal.TryParse(s[15 + i * 16].Replace(".", ","), out iBTSDist);

                                iChennel = GetChannelFromCDMAFreq(iFreq, new List<int> { (int)CDMABands.Band_0_800, (int)CDMABands.Band_5_450, (int)CDMABands.Band_14_US_PCS_1900_MHz });
                                string iGCID = iSID.ToString() + " " + iPNOffset.ToString() + " " + iFreq.ToString();
                                if (iPNOffset != 0 && iSID != 0) { iFullData = true; }
                                temp.Add(new TopNCDMA()
                                {
                                    Rank = iRank,
                                    MeasId = iMeasId,
                                    Freq = iFreq,
                                    PNOffset = iPNOffset,
                                    SID = iSID,
                                    NID = iNID,
                                    BaseID = iBaseID,
                                    T_ADD = iT_ADD,
                                    T_DROP = iT_DROP,
                                    T_COMP = iT_COMP,
                                    T_TDROP = iT_TDROP,
                                    IcIo = iIcIo,
                                    RSCP = iRSCP,
                                    PTotal = iPTotal,
                                    BTSName = iBTSName,
                                    BTSDist = iBTSDist,
                                    Channel = iChennel,
                                    FullData = iFullData,
                                    GCID = iGCID
                                });
                                App.Current.Dispatcher.BeginInvoke((Action)(() =>
                                {
                                    if (iFullData)
                                    {
                                        //bool find = false;
                                        //if (MainWindow.db_v2.MeasMon.Data.Count > 0)
                                        //{
                                        //    foreach (DB.MeasData mg in MainWindow.db_v2.MeasMon.Data)
                                        //    {
                                        //        if (mg.GCID == iGCID/* && mg.ChannelN ==iChennel.UARFCN*/) { mg.Power = iRSCP; find = true; }
                                        //    }
                                        //}
                                        //if (!find && iRSCP > App.Sett.MeasMons_Settings.MeasMons.Where(x => x.Techonology == DB.Technologys.CDMA).First().DetectionLevel)
                                        //{
                                        //    //MainWindow.db_v2.MeasMon.Data.Add(new DB.MeasData()
                                        //    //{
                                        //    //    MeasGuid = Guid.NewGuid(),
                                        //    //    Techonology = DB.Technologys.CDMA.ToString(),
                                        //    //    StandartSubband = iChennel.StandartSubband,
                                        //    //    FreqUP = iChennel.FreqUp,
                                        //    //    FreqDN = iChennel.FreqDn,
                                        //    //    MeasSpan = 2000000,
                                        //    //    MarPeakBW = 1250000,
                                        //    //    NdBBWMax = 1300000,
                                        //    //    NdBBWMin = 1100000,
                                        //    //    ChannelN = iChennel.ChannelN,
                                        //    //    GCID = iGCID,
                                        //    //    TechSubInd = iPNOffset,
                                        //    //    FullData = iFullData,
                                        //    //    Power = iRSCP,
                                        //    //    TraceCount = 0,
                                        //    //    LevelUnit = false,//MainWindow.Rcvr.LevelUnitStr
                                        //    //});
                                        //}
                                    }
                                }));
                            }
                        }
                        #endregion
                        CDMA = temp;
                    }
                    else if (t.Length > 0 && t.Contains("SIGNALSET3"))
                    {
                        ObservableCollection<TopNWiMax> temp = new ObservableCollection<TopNWiMax> { };
                        #region WiMax
                        string[] s = t.Replace("Data SIGNALSET3 ", "").Split(';');
                        if (s.Length > 0)
                        {
                            for (int i = 0; i < (int)(s.Length / 13); i++)
                            {
                                int iRank = 0;
                                int iPreambleIndex = 0;
                                decimal iSegment = 0;
                                int iCID = 0;
                                double iRSSI = 0;
                                double iCINR = 0;
                                decimal iFreq = 0;
                                decimal iBandwidth = 0;
                                int iFFTSize = 0;
                                int iCPRatio = 0;
                                int iFrameRate = 0;
                                string iBTSName = "";
                                decimal iBTSDistance = 0;
                                WiMax_Channel iChannel;
                                bool iFullData = false;

                                Int32.TryParse(s[0 + i * 13], out iRank);
                                Int32.TryParse(s[1 + i * 13], out iPreambleIndex);
                                decimal.TryParse(s[2 + i * 13].Split(' ')[0].Replace(".", ","), out iSegment); //decimal.TryParse(s[2 + i * 16], out iFreq);decimal.TryParse(s[2 + i * 16].Split(' ')[0].Replace(".", ","), out iFreq);
                                Int32.TryParse(s[3 + i * 13], out iCID);
                                double.TryParse(s[4 + i * 13].Replace(".", ","), out iRSSI);
                                //iRSSI += 106.98m;
                                double.TryParse(s[5 + i * 13].Replace(".", ","), out iCINR);
                                decimal.TryParse(s[6 + i * 13].Replace(".", ","), out iFreq);
                                decimal.TryParse(s[7 + i * 13].Replace(".", ","), out iBandwidth);
                                Int32.TryParse(s[8 + i * 13], out iFFTSize);
                                Int32.TryParse(s[9 + i * 13], out iCPRatio);
                                Int32.TryParse(s[10 + i * 13], out iFrameRate);
                                iBTSName = s[11 + i * 13];
                                decimal.TryParse(s[12 + i * 13].Replace(".", ","), out iBTSDistance);

                                iChannel = new WiMax_Channel() { ChannelN = 0, FreqDn = Math.Round(iFreq / 1000000), FreqUp = Math.Round(iFreq / 1000000), StandartSubband = "WiMax 2300 MHz" };
                                //iChennel = GetChannelFromCDMAFreq(iFreq, new List<int> { (int)CDMABands.Band_0_800, (int)CDMABands.Band_5_450, (int)CDMABands.Band_14_US_PCS_1900_MHz });
                                string iGCID = iCID.ToString() + " " + iPreambleIndex.ToString() + " " + Math.Round(iFreq / 1000000).ToString();
                                if (iCID != 0 && iPreambleIndex != 0) { iFullData = true; }
                                temp.Add(new TopNWiMax()
                                {
                                    Rank = iRank,
                                    PreambleIndex = iPreambleIndex,
                                    Segment = iSegment,
                                    CID = iCID,
                                    RSSI = iRSSI,
                                    CINR = iCINR,
                                    Freq = iFreq,
                                    Bandwidth = iBandwidth,
                                    FFTSize = iFFTSize,
                                    CPRatio = iCPRatio,
                                    FrameRate = iFrameRate,
                                    BTSName = iBTSName,
                                    BTSDistance = iBTSDistance,
                                    Channel = iChannel,
                                    FullData = iFullData,
                                    GCID = iGCID
                                });
                                App.Current.Dispatcher.BeginInvoke((Action)(() =>
                                {
                                    if (iFullData)
                                    {
                                        //bool find = false;
                                        //if (MainWindow.db_v2.MeasMon.Data.Count > 0)
                                        //{
                                        //    foreach (DB.MeasData mg in MainWindow.db_v2.MeasMon.Data)
                                        //    {
                                        //        if (mg.GCID == iGCID/* && mg.ChannelN ==iChennel.UARFCN*/) { mg.Power = iRSSI; find = true; }
                                        //    }
                                        //}
                                        //if (!find && iRSSI > App.Sett.MeasMons_Settings.MeasMons.Where(x => x.Techonology == DB.Technologys.WIMAX).First().DetectionLevel)
                                        //{
                                        //    //MainWindow.db_v2.MeasMon.Data.Add(new DB.MeasData()
                                        //    //{
                                        //    //    MeasGuid = Guid.NewGuid(),
                                        //    //    Techonology = DB.Technologys.WIMAX.ToString(),
                                        //    //    StandartSubband = iChannel.StandartSubband,
                                        //    //    FreqUP = iChannel.FreqUp,
                                        //    //    FreqDN = iChannel.FreqDn,
                                        //    //    MeasSpan = 10000000,
                                        //    //    MarPeakBW = 10000000,
                                        //    //    NdBBWMax = 10000000,
                                        //    //    NdBBWMin = 8000000,
                                        //    //    ChannelN = iChannel.ChannelN,
                                        //    //    GCID = iGCID,
                                        //    //    TechSubInd = iPreambleIndex,
                                        //    //    FullData = iFullData,
                                        //    //    Power = iRSSI,
                                        //    //    TraceCount = 0,
                                        //    //    LevelUnit = false// MainWindow.Rcvr.LevelUnitStr
                                        //    //});
                                        //}
                                    }
                                }));
                            }
                        }
                        #endregion
                        WiMax = temp;
                    }
                }
            }
            catch (Exception exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "RCRomes", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
        }
        //public void sameWork()
        //{
        //    try
        //    {
        //        if (LoadData && tc.m_Stream.DataAvailable)
        //        {
        //            string t = tc.Read().TrimEnd();
        //            //Time2 = t;
        //            if (t.Length > 0 && t.Contains("SIGNALSET0"))
        //            {
        //                ObservableCollection<TopNGSM> temp = new ObservableCollection<TopNGSM> { };
        //                #region GSM
        //                string[] s = t.Replace("Data SIGNALSET0 ", "").Split(';');
        //                //Time2 = s.Length + "    " + "    %" + s[0] + "\r\n";
        //                if (s.Length > 0)
        //                {
        //                    for (int i = 0; i < (int)(s.Length / 7); i++)
        //                    {
        //                        int iARFCN = 0;
        //                        GSM_Channel iChennel = new GSM_Channel();
        //                        int iMNC = 0;
        //                        int iMCC = 0;
        //                        int iLAC = 0;
        //                        int iCI = 0;
        //                        int iBSIC = 0;
        //                        decimal iPtotal = 0;

        //                        Int32.TryParse(s[0 + i * 7], out iARFCN);
        //                        iChennel = GetFreqFromGSMARFCN(iARFCN, new List<int> { (int)GSMBands.P_GSM900, (int)GSMBands.E_GSM900, (int)GSMBands.R_GSM900, (int)GSMBands.ER_GSM900, (int)GSMBands.GSM1800 });
        //                        Int32.TryParse(s[1 + i * 7], out iMNC);
        //                        Int32.TryParse(s[2 + i * 7], out iMCC);
        //                        Int32.TryParse(s[3 + i * 7], out iLAC);
        //                        Int32.TryParse(s[4 + i * 7], out iCI);
        //                        Int32.TryParse(s[5 + i * 7], out iBSIC);
        //                        decimal.TryParse(s[6 + i * 7].Replace(".", ","), out iPtotal);
        //                        iPtotal += 106.98m;
        //                        string iGCID = iMCC.ToString() + " " + iMNC.ToString() + " " + iLAC.ToString() + " " + iCI.ToString();
        //                        bool iFullData = false;
        //                        if (iARFCN != 0 && iMNC != 0 && iMCC != 0 && iLAC != 0 && iCI != 0 & iBSIC != 0) { iFullData = true; }
        //                        temp.Add(new TopNGSM()
        //                        {
        //                            ARFCN = iARFCN,
        //                            Channel = iChennel,
        //                            MNC = iMNC,
        //                            MCC = iMCC,
        //                            LAC = iLAC,
        //                            CI = iCI,
        //                            BSIC = iBSIC,
        //                            Ptotal = iPtotal,
        //                            FullData = iFullData,
        //                            GCID = iGCID
        //                        });
        //                        App.Current.Dispatcher.BeginInvoke((Action)(() =>
        //                        {
        //                            if (iFullData)
        //                            {
        //                                bool find = false;
        //                                if (MainWindow.db.MonMeas.Count > 0)
        //                                {
        //                                    foreach (DB.MeasData mg in MainWindow.db.MonMeas)
        //                                    {
        //                                        if (mg.GCID == iGCID/* && mg.ChannelN == iARFCN*/) { mg.Power = iPtotal; find = true; }
        //                                    }
        //                                }
        //                                if (!find && iPtotal > App.Sett.MeasMons_Settings.MeasMons.Where(x => x.Techonology == DB.Technologys.GSM).First().DetectionLevel)
        //                                {
        //                                    MainWindow.db.MonMeas.Add(new DB.MeasData()
        //                                    {
        //                                        MeasGuid = Guid.NewGuid(),
        //                                        Techonology = DB.Technologys.GSM.ToString(),
        //                                        StandartSubband = iChennel.StandartSubband,
        //                                        FreqUP = iChennel.FreqUp,
        //                                        FreqDN = iChennel.FreqDn,
        //                                        MeasSpan = 500000,
        //                                        MarPeakBW = 200000,
        //                                        NdBBWMax = 400000,
        //                                        NdBBWMin = 150000,
        //                                        ChannelN = iChennel.ARFCN,
        //                                        GCID = iGCID,
        //                                        TechSubInd = "BSIC: " + iBSIC.ToString(),
        //                                        FullData = iFullData,
        //                                        Power = iPtotal,
        //                                        TraceCount = 0,
        //                                        LevelUnitStr = MainWindow.Rcvr.LevelUnitStr,
        //                                        Trace = null
        //                                    });
        //                                }
        //                            }
        //                        }));
        //                    }
        //                }
        //                #endregion
        //                GSM = temp;
        //                //System.Windows.MessageBox.Show("GSM " + GSM.Count.ToString());
        //            }
        //            else if (t.Length > 0 && t.Contains("SIGNALSET1"))
        //            {
        //                ObservableCollection<TopNUMTS> temp = new ObservableCollection<TopNUMTS> { };
        //                #region UMTS
        //                string[] s = t.Replace("Data SIGNALSET1 ", "").Split(';');
        //                //Time2 = s.Length + "    " + "    %" + s[0] + "\r\n";
        //                if (s.Length > 0)
        //                {
        //                    for (int i = 0; i < (int)(s.Length / 11); i++)
        //                    {
        //                        int iSC = 0;
        //                        int iSCID = 0;
        //                        int iMNC = 0;
        //                        int iMCC = 0;
        //                        int iLAC = 0;
        //                        int iCI = 0;
        //                        decimal iPtotal = 0;
        //                        int iUARFCN = 0;
        //                        UMTS_Channel iChennel = new UMTS_Channel();
        //                        decimal iIcIo = 0;
        //                        decimal iRSCP = 0;
        //                        decimal iISCP = 0;
        //                        bool iFullData = false;
        //                        Int32.TryParse(s[0 + i * 11], out iSC);
        //                        Int32.TryParse(s[1 + i * 11], out iSCID);
        //                        Int32.TryParse(s[2 + i * 11], out iMNC);
        //                        Int32.TryParse(s[3 + i * 11], out iMCC);
        //                        Int32.TryParse(s[4 + i * 11], out iLAC);
        //                        Int32.TryParse(s[5 + i * 11], out iCI);
        //                        decimal.TryParse(s[6 + i * 11].Replace(".", ","), out iPtotal);
        //                        iPtotal += 106.98m;
        //                        Int32.TryParse(s[7 + i * 11], out iUARFCN);
        //                        iChennel = GetFreqFromUMTSUARFCN(iUARFCN, new List<int> { (int)UMTSBands.Band_1_2100 });
        //                        decimal.TryParse(s[8 + i * 11].Replace(".", ","), out iIcIo);
        //                        decimal.TryParse(s[9 + i * 11].Replace(".", ","), out iRSCP);
        //                        iRSCP += 106.98m;
        //                        decimal.TryParse(s[10 + i * 11].Replace(".", ","), out iISCP);
        //                        iISCP += 106.98m;
        //                        string iGCID = iMCC.ToString() + " " + iMNC.ToString() + " " + iLAC.ToString() + " " + iCI.ToString();
        //                        if (iSC != 0 && iMNC != 0 && iMCC != 0 && iLAC != 0 && iCI != 0 && iUARFCN != 0) { iFullData = true; }
        //                        temp.Add(new TopNUMTS()
        //                        {
        //                            SC = iSC,
        //                            SCID = iSCID,
        //                            MNC = iMNC,
        //                            MCC = iMCC,
        //                            LAC = iLAC,
        //                            CI = iCI,
        //                            Ptotal = iPtotal,
        //                            UARFCN = iUARFCN,
        //                            Channel = iChennel,
        //                            IcIo = iIcIo,
        //                            RSCP = iRSCP,
        //                            ISCP = iISCP,
        //                            FullData = iFullData,
        //                            GCID = iGCID
        //                        });
        //                        App.Current.Dispatcher.BeginInvoke((Action)(() =>
        //                        {
        //                            if (iFullData)
        //                            {
        //                                bool find = false;
        //                                if (MainWindow.db.MonMeas.Count > 0)
        //                                {
        //                                    foreach (DB.MeasData mg in MainWindow.db.MonMeas)
        //                                    {
        //                                        if (mg.GCID == iGCID/* && mg.ChannelN ==iChennel.UARFCN*/) { mg.Power = iRSCP; find = true; }
        //                                    }
        //                                }
        //                                if (!find && iRSCP > App.Sett.MeasMons_Settings.MeasMons.Where(x => x.Techonology == DB.Technologys.WCDMA).First().DetectionLevel)
        //                                {
        //                                    MainWindow.db.MonMeas.Add(new DB.MeasData()
        //                                    {
        //                                        MeasGuid = Guid.NewGuid(),
        //                                        Techonology = DB.Technologys.WCDMA.ToString(),
        //                                        StandartSubband = iChennel.StandartSubband,
        //                                        FreqUP = iChennel.FreqUp,
        //                                        FreqDN = iChennel.FreqDn,
        //                                        MeasSpan = 5000000,
        //                                        MarPeakBW = 4800000,
        //                                        NdBBWMax = 4850000,
        //                                        NdBBWMin = 4000000,
        //                                        ChannelN = iChennel.UARFCN,
        //                                        GCID = iGCID,
        //                                        TechSubInd = "SC: " + iSC.ToString(),
        //                                        FullData = iFullData,
        //                                        Power = iRSCP,
        //                                        TraceCount = 0,
        //                                        LevelUnitStr = MainWindow.Rcvr.LevelUnitStr
        //                                    });
        //                                }
        //                            }
        //                        }));
        //                    }
        //                }
        //                #endregion
        //                UMTS = temp;
        //            }
        //            else if (t.Length > 0 && t.Contains("SIGNALSET2"))
        //            {
        //                ObservableCollection<TopNCDMA> temp = new ObservableCollection<TopNCDMA> { };
        //                #region CDMA
        //                string[] s = t.Replace("Data SIGNALSET2 ", "").Split(';');
        //                Time2 = t;// s.Length + "    " + "    %" + s[0] + "\r\n";
        //                if (s.Length > 0)
        //                {
        //                    for (int i = 0; i < (int)(s.Length / 16); i++)
        //                    {
        //                        int iRank = 0;
        //                        int iMeasId = 0;
        //                        decimal iFreq = 0;
        //                        int iPNOffset = 0;
        //                        int iSID = 0;
        //                        int iNID = 0;
        //                        int iBaseID = 0;
        //                        decimal iT_ADD = 0;
        //                        decimal iT_DROP = 0;
        //                        decimal iT_COMP = 0;
        //                        decimal iT_TDROP = 0;
        //                        decimal iIcIo = 0;
        //                        decimal iRSCP = 0;
        //                        decimal iPTotal = 0;
        //                        string iBTSName = "";
        //                        decimal iBTSDist = 0;

        //                        CDMA_Channel iChennel = new CDMA_Channel();
        //                        bool iFullData = false;
        //                        Int32.TryParse(s[0 + i * 16], out iRank);
        //                        Int32.TryParse(s[1 + i * 16], out iMeasId);
        //                        decimal.TryParse(s[2 + i * 16].Split(' ')[0].Replace(".", ","), out iFreq); //decimal.TryParse(s[2 + i * 16], out iFreq);decimal.TryParse(s[2 + i * 16].Split(' ')[0].Replace(".", ","), out iFreq);
        //                        Int32.TryParse(s[3 + i * 16], out iPNOffset);
        //                        Int32.TryParse(s[4 + i * 16], out iSID);
        //                        Int32.TryParse(s[5 + i * 16], out iNID);
        //                        Int32.TryParse(s[6 + i * 16], out iBaseID);
        //                        decimal.TryParse(s[7 + i * 16].Replace(".", ","), out iT_ADD);
        //                        decimal.TryParse(s[8 + i * 16].Replace(".", ","), out iT_DROP);
        //                        decimal.TryParse(s[9 + i * 16].Replace(".", ","), out iT_COMP);
        //                        decimal.TryParse(s[10 + i * 16].Replace(".", ","), out iT_TDROP);
        //                        decimal.TryParse(s[11 + i * 16].Replace(".", ","), out iIcIo);
        //                        decimal.TryParse(s[12 + i * 16].Replace(".", ","), out iRSCP);
        //                        iRSCP += 106.98m;
        //                        decimal.TryParse(s[13 + i * 16].Replace(".", ","), out iPTotal);
        //                        iPTotal += 106.98m;
        //                        iBTSName = s[14 + i * 16];
        //                        decimal.TryParse(s[15 + i * 16].Replace(".", ","), out iBTSDist);

        //                        iChennel = GetChannelFromCDMAFreq(iFreq, new List<int> { (int)CDMABands.Band_0_800, (int)CDMABands.Band_5_450, (int)CDMABands.Band_14_US_PCS_1900_MHz });
        //                        string iGCID = iSID.ToString() + " " + iPNOffset.ToString() + " " + iFreq.ToString();
        //                        if (iPNOffset != 0 && iSID != 0) { iFullData = true; }
        //                        temp.Add(new TopNCDMA()
        //                        {
        //                            Rank = iRank,
        //                            MeasId = iMeasId,
        //                            Freq = iFreq,
        //                            PNOffset = iPNOffset,
        //                            SID = iSID,
        //                            NID = iNID,
        //                            BaseID = iBaseID,
        //                            T_ADD = iT_ADD,
        //                            T_DROP = iT_DROP,
        //                            T_COMP = iT_COMP,
        //                            T_TDROP = iT_TDROP,
        //                            IcIo = iIcIo,
        //                            RSCP = iRSCP,
        //                            PTotal = iPTotal,
        //                            BTSName = iBTSName,
        //                            BTSDist = iBTSDist,
        //                            Channel = iChennel,
        //                            FullData = iFullData,
        //                            GCID = iGCID
        //                        });
        //                        App.Current.Dispatcher.BeginInvoke((Action)(() =>
        //                        {
        //                            if (iFullData)
        //                            {
        //                                bool find = false;
        //                                if (MainWindow.db.MonMeas.Count > 0)
        //                                {
        //                                    foreach (DB.MeasData mg in MainWindow.db.MonMeas)
        //                                    {
        //                                        if (mg.GCID == iGCID/* && mg.ChannelN ==iChennel.UARFCN*/) { mg.Power = iRSCP; find = true; }
        //                                    }
        //                                }
        //                                if (!find && iRSCP > App.Sett.MeasMons_Settings.MeasMons.Where(x => x.Techonology == DB.Technologys.CDMA).First().DetectionLevel)
        //                                {
        //                                    MainWindow.db.MonMeas.Add(new DB.MeasData()
        //                                    {
        //                                        MeasGuid = Guid.NewGuid(),
        //                                        Techonology = DB.Technologys.CDMA.ToString(),
        //                                        StandartSubband = iChennel.StandartSubband,
        //                                        FreqUP = iChennel.FreqUp,
        //                                        FreqDN = iChennel.FreqDn,
        //                                        MeasSpan = 2000000,
        //                                        MarPeakBW = 1250000,
        //                                        NdBBWMax = 1300000,
        //                                        NdBBWMin = 1100000,
        //                                        ChannelN = iChennel.ChannelN,
        //                                        GCID = iGCID,
        //                                        TechSubInd = "PN: " + iPNOffset.ToString(),
        //                                        FullData = iFullData,
        //                                        Power = iRSCP,
        //                                        TraceCount = 0,
        //                                        LevelUnitStr = MainWindow.Rcvr.LevelUnitStr
        //                                    });
        //                                }
        //                            }
        //                        }));
        //                    }
        //                }
        //                #endregion
        //                CDMA = temp;
        //            }
        //            else if (t.Length > 0 && t.Contains("SIGNALSET3"))
        //            {
        //                ObservableCollection<TopNWiMax> temp = new ObservableCollection<TopNWiMax> { };
        //                #region WiMax
        //                string[] s = t.Replace("Data SIGNALSET3 ", "").Split(';');
        //                if (s.Length > 0)
        //                {
        //                    for (int i = 0; i < (int)(s.Length / 13); i++)
        //                    {
        //                        int iRank = 0;
        //                        int iPreambleIndex = 0;
        //                        decimal iSegment = 0;
        //                        int iCID = 0;
        //                        decimal iRSSI = 0;
        //                        decimal iCINR = 0;
        //                        decimal iFreq = 0;
        //                        decimal iBandwidth = 0;
        //                        int iFFTSize = 0;
        //                        int iCPRatio = 0;
        //                        int iFrameRate = 0;
        //                        string iBTSName = "";
        //                        decimal iBTSDistance = 0;
        //                        WiMax_Channel iChannel;
        //                        bool iFullData = false;

        //                        Int32.TryParse(s[0 + i * 13], out iRank);
        //                        Int32.TryParse(s[1 + i * 13], out iPreambleIndex);
        //                        decimal.TryParse(s[2 + i * 13].Split(' ')[0].Replace(".", ","), out iSegment); //decimal.TryParse(s[2 + i * 16], out iFreq);decimal.TryParse(s[2 + i * 16].Split(' ')[0].Replace(".", ","), out iFreq);
        //                        Int32.TryParse(s[3 + i * 13], out iCID);
        //                        decimal.TryParse(s[4 + i * 13].Replace(".", ","), out iRSSI);
        //                        iRSSI += 106.98m;
        //                        decimal.TryParse(s[5 + i * 13].Replace(".", ","), out iCINR);
        //                        decimal.TryParse(s[6 + i * 13].Replace(".", ","), out iFreq);
        //                        decimal.TryParse(s[7 + i * 13].Replace(".", ","), out iBandwidth);
        //                        Int32.TryParse(s[8 + i * 13], out iFFTSize);
        //                        Int32.TryParse(s[9 + i * 13], out iCPRatio);
        //                        Int32.TryParse(s[10 + i * 13], out iFrameRate);
        //                        iBTSName = s[11 + i * 13];
        //                        decimal.TryParse(s[12 + i * 13].Replace(".", ","), out iBTSDistance);

        //                        iChannel = new WiMax_Channel() { ChannelN = 0, FreqDn = Math.Round(iFreq / 1000000), FreqUp = Math.Round(iFreq / 1000000), StandartSubband = "WiMax 2300 MHz" };
        //                        //iChennel = GetChannelFromCDMAFreq(iFreq, new List<int> { (int)CDMABands.Band_0_800, (int)CDMABands.Band_5_450, (int)CDMABands.Band_14_US_PCS_1900_MHz });
        //                        string iGCID = iCID.ToString() + " " + iPreambleIndex.ToString() + " " + Math.Round(iFreq / 1000000).ToString();
        //                        if (iCID != 0 && iPreambleIndex != 0) { iFullData = true; }
        //                        temp.Add(new TopNWiMax()
        //                        {
        //                            Rank = iRank,
        //                            PreambleIndex = iPreambleIndex,
        //                            Segment = iSegment,
        //                            CID = iCID,
        //                            RSSI = iRSSI,
        //                            CINR = iCINR,
        //                            Freq = iFreq,
        //                            Bandwidth = iBandwidth,
        //                            FFTSize = iFFTSize,
        //                            CPRatio = iCPRatio,
        //                            FrameRate = iFrameRate,
        //                            BTSName = iBTSName,
        //                            BTSDistance = iBTSDistance,
        //                            Channel = iChannel,
        //                            FullData = iFullData,
        //                            GCID = iGCID
        //                        });
        //                        App.Current.Dispatcher.BeginInvoke((Action)(() =>
        //                        {
        //                            if (iFullData)
        //                            {
        //                                bool find = false;
        //                                if (MainWindow.db.MonMeas.Count > 0)
        //                                {
        //                                    foreach (DB.MeasData mg in MainWindow.db.MonMeas)
        //                                    {
        //                                        if (mg.GCID == iGCID/* && mg.ChannelN ==iChennel.UARFCN*/) { mg.Power = iRSSI; find = true; }
        //                                    }
        //                                }
        //                                if (!find && iRSSI > App.Sett.MeasMons_Settings.MeasMons.Where(x => x.Techonology == DB.Technologys.WIMAX).First().DetectionLevel)
        //                                {
        //                                    MainWindow.db.MonMeas.Add(new DB.MeasData()
        //                                    {
        //                                        MeasGuid = Guid.NewGuid(),
        //                                        Techonology = DB.Technologys.WIMAX.ToString(),
        //                                        StandartSubband = iChannel.StandartSubband,
        //                                        FreqUP = iChannel.FreqUp,
        //                                        FreqDN = iChannel.FreqDn,
        //                                        MeasSpan = 10000000,
        //                                        MarPeakBW = 10000000,
        //                                        NdBBWMax = 10000000,
        //                                        NdBBWMin = 8000000,
        //                                        ChannelN = iChannel.ChannelN,
        //                                        GCID = iGCID,
        //                                        TechSubInd = "PN: " + iPreambleIndex.ToString(),
        //                                        FullData = iFullData,
        //                                        Power = iRSSI,
        //                                        TraceCount = 0,
        //                                        LevelUnitStr = MainWindow.Rcvr.LevelUnitStr
        //                                    });
        //                                }
        //                            }
        //                        }));
        //                    }
        //                }
        //                #endregion
        //                WiMax = temp;
        //            }
        //        }
        //    }
        //    catch (Exception exp)
        //    {
        //        MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "RCRomes", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
        //    }
        //}
        private GSM_Channel GetFreqFromGSMARFCN(int inARFCN, List<int> Standarts)
        {
            GSM_Channel temp = new GSM_Channel();
            foreach (int i in Standarts)
            {
                if ((int)GSMBands.P_GSM900 == i && inARFCN >= 1 && inARFCN <= 124) { temp.StandartSubband = "P-GSM900"; temp.FreqUp = (decimal)(890 + 0.2 * (inARFCN)); temp.FreqDn = (decimal)(935 + 0.2 * (inARFCN)); }
                else if ((int)GSMBands.GSM850 == i && inARFCN >= 128 && inARFCN <= 251) { temp.StandartSubband = "GSM850"; temp.FreqUp = (decimal)(824.2 + 0.2 * (inARFCN - 128)); temp.FreqDn = (decimal)(869.2 + 0.2 * (inARFCN - 128)); }
                else if ((int)GSMBands.GSM450 == i && inARFCN >= 259 && inARFCN <= 293) { temp.StandartSubband = "GSM450"; temp.FreqUp = (decimal)(450.6 + 0.2 * (inARFCN - 259)); temp.FreqDn = (decimal)(460.6 + 0.2 * (inARFCN - 259)); }
                else if ((int)GSMBands.GSM480 == i && inARFCN >= 306 && inARFCN <= 340) { temp.StandartSubband = "GSM480"; temp.FreqUp = (decimal)(479 + 0.2 * (inARFCN - 306)); temp.FreqDn = (decimal)(489 + 0.2 * (inARFCN - 306)); }
                else if ((int)GSMBands.GSM750 == i && inARFCN >= 438 && inARFCN <= 511) { temp.StandartSubband = "GSM750"; temp.FreqUp = (decimal)(747.2 + 0.2 * (inARFCN - 438)); temp.FreqDn = (decimal)(777.2 + 0.2 * (inARFCN - 438)); }
                else if ((int)GSMBands.GSM1800 == i && inARFCN >= 512 && inARFCN <= 885) { temp.StandartSubband = "GSM1800"; temp.FreqUp = (decimal)(1805.2 + 0.2 * (inARFCN - 512)); temp.FreqDn = (decimal)(1805.2 + 0.2 * (inARFCN - 512)); }
                else if ((int)GSMBands.ER_GSM900 == i && inARFCN >= 940 && inARFCN <= 954) { temp.StandartSubband = "ER-GSM900"; temp.FreqUp = (decimal)(873.2 + 0.2 * (inARFCN - 940)); temp.FreqDn = (decimal)(918.2 + 0.2 * (inARFCN - 940)); }
                else if ((int)GSMBands.R_GSM900 == i && inARFCN >= 955 && inARFCN <= 974) { temp.StandartSubband = "R-GSM900"; temp.FreqUp = (decimal)(876.2 + 0.2 * (inARFCN - 955)); temp.FreqDn = (decimal)(921.2 + 0.2 * (inARFCN - 955)); }
                else if ((int)GSMBands.E_GSM900 == i && inARFCN >= 975 && inARFCN <= 1023) { temp.StandartSubband = "E-GSM900"; temp.FreqUp = (decimal)(880.2 + 0.2 * (inARFCN - 975)); temp.FreqDn = (decimal)(925.2 + 0.2 * (inARFCN - 975)); }
                else if ((int)GSMBands.GSM1900 == i && inARFCN >= 512 && inARFCN <= 810) { temp.StandartSubband = "GSM1900"; temp.FreqUp = (decimal)(1850.2 + 0.2 * (inARFCN - 512)); temp.FreqDn = (decimal)(1930.2 + 0.2 * (inARFCN - 512)); }
            }
            return temp;
        }
        private UMTS_Channel GetFreqFromUMTSUARFCN(int inUARFCN_Dn, List<int> Standarts)
        {
            UMTS_Channel temp = new UMTS_Channel();
            foreach (int i in Standarts)
            {
                if ((int)UMTSBands.Band_1_2100 == i && inUARFCN_Dn >= 10562 && inUARFCN_Dn <= 10838) { temp.StandartSubband = "Band-1 2100"; temp.FreqUp = (decimal)(1922.4 + 0.2 * (inUARFCN_Dn - 10562)); temp.FreqDn = (decimal)(2112.4 + 0.2 * (inUARFCN_Dn - 10562)); }
                else if ((int)UMTSBands.Band_2_1900 == i && inUARFCN_Dn >= 9262 && inUARFCN_Dn <= 9538) { temp.StandartSubband = "Band-2 1900"; temp.FreqUp = (decimal)(1852.4 + 0.2 * (inUARFCN_Dn - 9662)); temp.FreqDn = (decimal)(1932.4 + 0.2 * (inUARFCN_Dn - 9662)); }
                else if ((int)UMTSBands.Band_2_1900 == i && (inUARFCN_Dn == 412 || inUARFCN_Dn == 437 || inUARFCN_Dn == 462 || inUARFCN_Dn == 487 || inUARFCN_Dn == 512 || inUARFCN_Dn == 537
                    || inUARFCN_Dn == 562 || inUARFCN_Dn == 587 || inUARFCN_Dn == 612 || inUARFCN_Dn == 637 || inUARFCN_Dn == 662 || inUARFCN_Dn == 687))
                { temp.StandartSubband = "Band-2 1900"; temp.FreqUp = (decimal)1852.5 + 5 * (inUARFCN_Dn - 412); temp.FreqDn = (decimal)1932.5 + 5 * (inUARFCN_Dn - 412); }

                else if ((int)UMTSBands.Band_3_1800 == i && inUARFCN_Dn >= 1162 && inUARFCN_Dn <= 1513) { temp.StandartSubband = "Band-3 1800"; temp.FreqUp = (decimal)(1712.4 + 0.2 * (inUARFCN_Dn - 1162)); temp.FreqDn = (decimal)(1807.4 + 0.2 * (inUARFCN_Dn - 1162)); }
                else if ((int)UMTSBands.Band_4_1700 == i && inUARFCN_Dn >= 1537 && inUARFCN_Dn <= 1738) { temp.StandartSubband = "Band-4 1700"; temp.FreqUp = (decimal)(1712.4 + 0.2 * (inUARFCN_Dn - 1537)); temp.FreqDn = (decimal)(2112.4 + 0.2 * (inUARFCN_Dn - 1537)); }
                else if ((int)UMTSBands.Band_4_1700 == i && (inUARFCN_Dn == 1887 || inUARFCN_Dn == 1912 || inUARFCN_Dn == 1937 ||
                    inUARFCN_Dn == 1962 || inUARFCN_Dn == 1987 || inUARFCN_Dn == 2012 || inUARFCN_Dn == 037 || inUARFCN_Dn == 2062 || inUARFCN_Dn == 2087))
                { temp.StandartSubband = "Band-4 1700"; temp.FreqUp = (decimal)1712.5 + 5 * (inUARFCN_Dn - 1887); temp.FreqDn = (decimal)2112.5 + 5 * (inUARFCN_Dn - 1887); }

                else if ((int)UMTSBands.Band_5_850 == i && inUARFCN_Dn >= 4357 && inUARFCN_Dn <= 4458) { temp.StandartSubband = "Band-5 850"; temp.FreqUp = (decimal)(826.4 + 0.2 * (inUARFCN_Dn - 4357)); temp.FreqDn = (decimal)(871.4 + 0.2 * (inUARFCN_Dn - 4357)); }
                //else if ((int)UMTSBands.Band_5_850 == i && (inUARFCN_Dn == 1007 || inUARFCN_Dn == 1012 || inUARFCN_Dn == 1032 || inUARFCN_Dn == 1037 || inUARFCN_Dn == 1062 || inUARFCN_Dn == 1087))
                //{ temp.StandartSubband = "Band-5 850"; temp.FreqUp = 826.5 + 0.2 * (inUARFCN_Dn - 4357); temp.FreqDn = 871.4 + 0.2 * (inUARFCN_Dn - 4357); }


            }
            return temp;
        }
        private CDMA_Channel GetChannelFromCDMAFreq(decimal inFreq, List<int> Standarts)
        {
            CDMA_Channel temp = new CDMA_Channel();
            foreach (int i in Standarts)
            {
                if ((int)CDMABands.Band_0_800 == i && inFreq >= 870.03m && inFreq <= 893.97m) { temp.StandartSubband = "BC0 800 MHz"; temp.ChannelN = (int)((inFreq - 870) / 0.03m); temp.FreqUp = inFreq + 45; temp.FreqDn = inFreq; }
                else if ((int)CDMABands.Band_0_800 == i && inFreq >= 869.04m && inFreq <= 870m) { temp.StandartSubband = "BC0 800 MHz"; temp.ChannelN = (int)((inFreq - 870) / 0.03m) + 1023; temp.FreqUp = inFreq + 45; temp.FreqDn = inFreq; }
                else if ((int)CDMABands.Band_0_800 == i && inFreq >= 860.04m && inFreq <= 869.01m) { temp.StandartSubband = "BC0 800 MHz"; temp.ChannelN = (int)((inFreq - 860) / 0.03m) + 1024; temp.FreqUp = inFreq + 45; temp.FreqDn = inFreq; }
                else if ((int)CDMABands.Band_5_450 == i && inFreq >= 460 && inFreq <= 469.975m) { temp.StandartSubband = "BC5 450 MHz NMT"; temp.ChannelN = (int)((inFreq - 460) / 0.025m) + 1; temp.FreqUp = inFreq + 10; temp.FreqDn = inFreq; }
                else if ((int)CDMABands.Band_5_450 == i && inFreq >= 420 && inFreq <= 429.975m) { temp.StandartSubband = "BC5 450 MHz NMT"; temp.ChannelN = (int)((inFreq - 420) / 0.025m) + 472; temp.FreqUp = inFreq + 10; temp.FreqDn = inFreq; }
                else if ((int)CDMABands.Band_5_450 == i && inFreq >= 461.31m && inFreq <= 469.99m) { temp.StandartSubband = "BC5 450 MHz NMT"; temp.ChannelN = (int)((inFreq - 461.01m) / 0.02m) + 1024; temp.FreqUp = inFreq + 10; temp.FreqDn = inFreq; }
                else if ((int)CDMABands.Band_5_450 == i && inFreq >= 489m && inFreq <= 493.475m) { temp.StandartSubband = "BC5 450 MHz NMT"; temp.ChannelN = (int)((inFreq - 489m) / 0.025m) + 1536; temp.FreqUp = inFreq + 10; temp.FreqDn = inFreq; }
                else if ((int)CDMABands.Band_5_450 == i && inFreq >= 489m && inFreq <= 493.48m) { temp.StandartSubband = "BC5 450 MHz NMT"; temp.ChannelN = (int)((inFreq - 489m) / 0.02m) + 1792; temp.FreqUp = inFreq + 10; temp.FreqDn = inFreq; }
                else if ((int)CDMABands.Band_14_US_PCS_1900_MHz == i && inFreq >= 1930 && inFreq <= 1994.95m) { temp.StandartSubband = "BC5 1.9 GHz US PCS"; temp.ChannelN = (int)((inFreq - 1930) / 0.05m); temp.FreqUp = inFreq + 80; temp.FreqDn = inFreq; }
            }
            return temp;
        }
        /// <summary>
        /// Подключаемся к прибору        
        /// </summary>
        private void SetConnect()
        {
            try
            {
                tc = new Connection();
                tc.Open(Server, Port);
            }
            #region Exception
            catch (Exception exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "ANALYZER", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            #endregion
            TelnetDM -= SetConnect;
        }

        // Событие, которое нужно вызывать при изменении
        public event PropertyChangedEventHandler PropertyChanged;
        // Для удобства обернем событие в метод с единственным параметром - имя изменяемого свойства
        public void OnPropertyChanged(string propertyName)
        {
            // Если кто-то на него подписан, то вызывем его
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }


    public class TopNGSM : INotifyPropertyChanged
    {
        public int ARFCN { get; set; }
        
        public GSM_Channel Channel { get; set; }
        public int MNC { get; set; }
        public int MCC { get; set; }
        public int LAC { get; set; }
        public int CI { get; set; }
        public int BSIC { get; set; }
        public decimal Ptotal { get; set; }
        public bool FullData { get; set; }
        public string GCID { get; set; }
        // Событие, которое нужно вызывать при изменении
        public event PropertyChangedEventHandler PropertyChanged;
        // Для удобства обернем событие в метод с единственным параметром - имя изменяемого свойства
        public void OnPropertyChanged(string propertyName)
        {
            // Если кто-то на него подписан, то вызывем его
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    public class TopNUMTS : INotifyPropertyChanged
    {
        public int SC { get; set; }
        public int SCID { get; set; }
        public int MNC { get; set; }
        public int MCC { get; set; }
        public int LAC { get; set; }
        public int CI { get; set; }
        public decimal Ptotal { get; set; }
        public int UARFCN { get; set; }
        public UMTS_Channel Channel { get; set; }
        public decimal IcIo { get; set; }
        public decimal RSCP { get; set; }
        public decimal ISCP { get; set; }
        public bool FullData { get; set; }
        public string GCID { get; set; }
        // Событие, которое нужно вызывать при изменении
        public event PropertyChangedEventHandler PropertyChanged;
        // Для удобства обернем событие в метод с единственным параметром - имя изменяемого свойства
        public void OnPropertyChanged(string propertyName)
        {
            // Если кто-то на него подписан, то вызывем его
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    public class TopNCDMA : INotifyPropertyChanged
    {
        public int Rank { get; set; }
        public int MeasId { get; set; }
        public decimal Freq { get; set; }
        public int PNOffset { get; set; }
        public int SID { get; set; }
        public int NID { get; set; }
        public int BaseID { get; set; }
        public decimal T_ADD { get; set; }
        public decimal T_DROP { get; set; }
        public decimal T_COMP { get; set; }
        public decimal T_TDROP { get; set; }
        public double IcIo { get; set; }
        public double RSCP { get; set; }
        public double PTotal { get; set; }
        public string BTSName { get; set; }
        public decimal BTSDist { get; set; }
        public CDMA_Channel Channel { get; set; }
        public bool FullData { get; set; }
        public string GCID { get; set; }

        // Событие, которое нужно вызывать при изменении
        public event PropertyChangedEventHandler PropertyChanged;
        // Для удобства обернем событие в метод с единственным параметром - имя изменяемого свойства
        public void OnPropertyChanged(string propertyName)
        {
            // Если кто-то на него подписан, то вызывем его
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    public class TopNWiMax : INotifyPropertyChanged
    {
        public int Rank { get; set; }
        public int PreambleIndex { get; set; }
        public decimal Segment { get; set; }
        public int CID { get; set; }
        public double RSSI { get; set; }
        public double CINR { get; set; }
        public decimal Freq { get; set; }
        public decimal Bandwidth { get; set; }
        public int FFTSize { get; set; }
        public int CPRatio { get; set; }
        public int FrameRate { get; set; }
        public string BTSName { get; set; }
        public decimal BTSDistance { get; set; }
        public WiMax_Channel Channel { get; set; }
        public bool FullData { get; set; }
        public string GCID { get; set; }


        // Событие, которое нужно вызывать при изменении
        public event PropertyChangedEventHandler PropertyChanged;
        // Для удобства обернем событие в метод с единственным параметром - имя изменяемого свойства
        public void OnPropertyChanged(string propertyName)
        {
            // Если кто-то на него подписан, то вызывем его
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    


}
