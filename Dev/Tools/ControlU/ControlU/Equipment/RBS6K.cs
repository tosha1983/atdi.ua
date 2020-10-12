using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ControlU.Equipment
{
    public class RBS6K : PropertyChangedBase
    {
        public delegate void DoubMod();
        public DoubMod UdpDM;
        static UdpStreaming uc;
        public Thread UdpTr;

        public System.Timers.Timer tmr = new System.Timers.Timer(10);
        private long LastUpdateUDP;
        public string Time
        {
            get { return _Time; }
            set { _Time = value; OnPropertyChanged("Time"); }
        }
        string _Time;

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
        bool _Run;

        public bool IsRuning
        {
            get { return _IsRuning; }
            set { _IsRuning = value; OnPropertyChanged("IsRuning"); }
        }
        private bool _IsRuning;

        public bool IsRuningUDP
        {
            get { return _IsRuningUDP; }
            set { _IsRuningUDP = value; if (_IsRuningUDP) { IsRuning = true; } else { IsRuning = false; } }
        }
        private bool _IsRuningUDP;

        public bool DataCycle
        {
            get { return _DataCycle; }
            set { _DataCycle = value; OnPropertyChanged("DataCycle"); }
        }
        bool _DataCycle;

        public string MyIP
        {
            get { return _MyIP; }
            set { if (_MyIP != value) { _MyIP = value; OnPropertyChanged("_MyIP"); } }
        }
        private string _MyIP = "127.0.0.1";

        public int UDPPort
        {
            get { return _UDPPort; }
            set { _UDPPort = value; OnPropertyChanged("UDPPort"); }
        }
        private int _UDPPort = 6169;//025;

        public double DetectionLevelGSM = -100;
        public double DetectionLevelUMTS = -100;
        public double DetectionLevelLTE = -100;
        public double DetectionLevelCDMA = -100;


        public void Connect()
        {
            if (App.Sett.RsReceiver_Settings.IPAdress != "")
            {
                if (App.Sett.RsReceiver_Settings.TCPPort != 0)
                {
                    uc = new UdpStreaming();
                    uc.Open(MyIP, UDPPort);

                    UdpDM = sameWorkUDP;
                    UdpDM += ReaderStream;
                    UdpTr = new Thread(AllUdpTimeWorks);
                    UdpTr.Name = "RsReceiverUdpThread";
                    UdpTr.IsBackground = true;

                    UdpTr.Start();
                    IsRuning = true;
                }
                else
                {
                    Run = false;
                    if (App.Sett.RsReceiver_Settings.TCPPort == 0)
                    {
                        string str = ((SplashWindow)App.Current.MainWindow).m_mainWindow.FindResource("NotSetPortEquipment").ToString()
                          .Replace("*Equipment*", ((SplashWindow)App.Current.MainWindow).m_mainWindow.FindResource("EqMonitoringReceiver").ToString());
                        ((SplashWindow)App.Current.MainWindow).m_mainWindow.Message = str;
                    }
                }
            }
            else
            {
                Run = false;
                if (App.Sett.RsReceiver_Settings.IPAdress == "")
                {
                    string str = ((SplashWindow)App.Current.MainWindow).m_mainWindow.FindResource("NotSetIPAddressEquipment").ToString()
                      .Replace("*Equipment*", ((SplashWindow)App.Current.MainWindow).m_mainWindow.FindResource("EqMonitoringReceiver").ToString());
                    ((SplashWindow)App.Current.MainWindow).m_mainWindow.Message = str;
                }
            }
        }
        public void Disconnect()
        {
            tmr.Stop();
            _DataCycle = false;
        }
        private void AllUdpTimeWorks()
        {
            while (DataCycle)
            {
                LastUpdateUDP = DateTime.Now.Ticks;
                if (UdpDM != null)
                    UdpDM();
                //Time = new TimeSpan(DateTime.Now.Ticks - LastUpdateUDP).ToString();
            }
            IsRuningUDP = false;
        }
        public void sameWorkUDP()
        {
            try
            {
                Thread.Sleep(1);
            }
            catch { }
        }


        double LevelIsMaxIfMoreBy = 5;
        double LevelDifferenceToRemove = 5;

        public void ReaderStream()
        {
            Thread.Sleep(new TimeSpan(10));
            if (uc.IsOpen)
            {
                string t = uc.Read();
                if (t.Length > 0)
                {
                    #region
                    Time = MainWindow.gps.LocalTime.ToString() + " " + t;
                    string[] str = t.Split(':');
                    if (str[0].ToUpper().StartsWith("GSM") || str[0].ToUpper().StartsWith("DCS"))
                    {
                        #region
                        try
                        {
                            string[] read = str[1].Split(',');
                            int mcc = int.Parse((read[0].Split('/'))[1]);
                            int mnc = int.Parse(read[1]);
                            int lac = int.Parse(read[2]);
                            int cid = int.Parse(read[3] + read[4]);
                            int channel = int.Parse(read[5]);
                            decimal freq = 1000000 * decimal.Parse(read[6].Replace(".", ","));
                            double level = double.Parse((read[7].Split(';'))[0]);

                            bool find = false;
                            for (int i = 0; i < IdentificationData.GSM.BTS.Count; i++)
                            {
                                #region
                                if (IdentificationData.GSM.BTS[i].MCC == mcc &&
                                    IdentificationData.GSM.BTS[i].MNC == mnc &&
                                    IdentificationData.GSM.BTS[i].LAC == lac &&
                                    IdentificationData.GSM.BTS[i].CID == cid &&
                                    IdentificationData.GSM.BTS[i].FreqDn == freq)
                                {
                                    IdentificationData.GSM.BTS[i].Power = level;

                                    IdentificationData.GSM.BTS[i].LastLevelUpdete = MainWindow.gps.LocalTime;
                                    if (IdentificationData.GSM.BTS[i].Power > DetectionLevelGSM)/////////////////////////////////////////////////////////////////////
                                    { IdentificationData.GSM.BTS[i].LastDetectionLevelUpdete = MainWindow.gps.LocalTime.Ticks; }
                                    IdentificationData.GSM.BTS[i].DeleteFromMeasMon = (IdentificationData.GSM.BTS[i].Power < DetectionLevelGSM - LevelDifferenceToRemove);

                                    bool freqLevelMax = true;
                                    for (int l = 0; l < IdentificationData.GSM.BTS.Count; l++)
                                    {
                                        if (IdentificationData.GSM.BTS[l].FreqDn == IdentificationData.GSM.BTS[i].FreqDn &&
                                            IdentificationData.GSM.BTS[l].GCID != IdentificationData.GSM.BTS[i].GCID)
                                        {
                                            if (IdentificationData.GSM.BTS[l].Power + LevelIsMaxIfMoreBy < IdentificationData.GSM.BTS[i].Power)
                                                IdentificationData.GSM.BTS[l].ThisIsMaximumSignalAtThisFrequency = false;
                                            else { freqLevelMax = false; }
                                        }
                                    }
                                    IdentificationData.GSM.BTS[i].ThisIsMaximumSignalAtThisFrequency = freqLevelMax;

                                    find = true;
                                }
                                #endregion
                            }
                            if (find == false)
                            {
                                Equipment.GSM_Channel ch = MainWindow.help.GetGSMCHfromFreqDN(freq);
                                GSMBTSData dt = new GSMBTSData()
                                {
                                    MCC = mcc,
                                    MNC = mnc,
                                    LAC = lac,
                                    CID = cid,
                                    Power = level,
                                    ARFCN = ch.ARFCN,
                                    FreqDn = ch.FreqDn,
                                    FreqUp = ch.FreqUp,
                                    StandartSubband = ch.StandartSubband,
                                };
                                GUIThreadDispatcher.Instance.Invoke(() =>
                                {
                                    IdentificationData.GSM.BTS.Add(dt);
                                });
                            }
                        }
                        catch (Exception exp)
                        {
                            App.Current.Dispatcher.BeginInvoke((Action)(() =>
                            {
                                MainWindow.exp.ExceptionData = new ExData()
                                {
                                    ex = exp,
                                    ClassName = "TSMxReceiver",
                                    AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name +
                                    "\r\n" + t + "\r\n"
                                };
                            }));
                        }
                        #endregion
                    }
                    else if (str[0].ToUpper().StartsWith("WCDMA"))
                    {
                        #region
                        try
                        {
                            string[] read = str[1].Split(',');
                            int mcc = int.Parse((read[0].Split('/'))[1]);
                            int mnc = int.Parse(read[1]);
                            int lac = int.Parse(read[2]);
                            int cid = int.Parse(read[3] + read[4]);
                            int channel = int.Parse(read[5]);
                            decimal freq = 1000000 * decimal.Parse(read[6].Replace(".", ","));
                            double level = double.Parse((read[7].Split(';'))[0]);

                            bool find = false;
                            for (int i = 0; i < IdentificationData.UMTS.BTS.Count; i++)
                            {
                                #region
                                if (IdentificationData.UMTS.BTS[i].MCC == mcc &&
                                    IdentificationData.UMTS.BTS[i].MNC == mnc &&
                                    IdentificationData.UMTS.BTS[i].LAC == lac &&
                                    IdentificationData.UMTS.BTS[i].CID == cid &&
                                    IdentificationData.UMTS.BTS[i].UARFCN_DN == channel)
                                {
                                    IdentificationData.UMTS.BTS[i].RSCP = level;

                                    find = true;
                                }
                                #endregion
                            }
                            if (find == false)
                            {
                                Equipment.UMTS_Channel ch = MainWindow.help.GetUMTSCHFromChannelDn(channel);
                                UMTSBTSData dt = new UMTSBTSData()
                                {
                                    MCC = mcc,
                                    MNC = mnc,
                                    LAC = lac,
                                    CID = cid,
                                    RSCP = level,
                                    UARFCN_DN = ch.UARFCN_DN,
                                    UARFCN_UP = ch.UARFCN_UP,
                                    FreqDn = ch.FreqDn,
                                    FreqUp = ch.FreqUp,
                                    StandartSubband = ch.StandartSubband,
                                };
                                GUIThreadDispatcher.Instance.Invoke(() =>
                                {
                                    IdentificationData.UMTS.BTS.Add(dt);
                                });
                            }
                        }
                        catch (Exception exp)
                        {
                            App.Current.Dispatcher.BeginInvoke((Action)(() =>
                            {
                                MainWindow.exp.ExceptionData = new ExData()
                                {
                                    ex = exp,
                                    ClassName = "TSMxReceiver",
                                    AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name +
                                    "\r\n" + t + "\r\n"
                                };
                            }));
                        }
                        #endregion
                    }
                    else if (str[0].ToUpper().StartsWith("LTE"))
                    {
                        #region
                        try
                        {
                            string[] read = str[1].Split(',');
                            int mcc = int.Parse((read[0].Split('/'))[1]);
                            int mnc = int.Parse(read[1]);
                            int eci = int.Parse(read[2]);
                            int enodebid = int.Parse(read[3]);
                            int cid = int.Parse(read[4]);
                            int tac = int.Parse(read[5]);
                            int pci = int.Parse(read[6]);
                            decimal bw = decimal.Parse(read[7].Replace(".", ","));
                            int channel = int.Parse(read[8]);
                            decimal freq = 1000000 * decimal.Parse(read[9].Replace(".", ","));
                            double level = double.Parse((read[10].Split(';'))[0]);

                            bool find = false;
                            for (int i = 0; i < IdentificationData.LTE.BTS.Count; i++)
                            {
                                #region
                                if (IdentificationData.LTE.BTS[i].MCC == mcc &&
                                    IdentificationData.LTE.BTS[i].MNC == mnc &&
                                    IdentificationData.LTE.BTS[i].ECI28 == eci.ToString() &&
                                    IdentificationData.LTE.BTS[i].TAC == tac &&
                                    IdentificationData.LTE.BTS[i].PCI == pci &&
                                    IdentificationData.LTE.BTS[i].EARFCN_DN == channel)
                                {
                                    IdentificationData.LTE.BTS[i].RSRP = level;

                                    find = true;
                                }
                                #endregion
                            }
                            if (find == false)
                            {
                                Equipment.LTE_Channel ch = MainWindow.help.GetLTECHfromFreqDN(freq);
                                LTEBTSData dt = new LTEBTSData()
                                {
                                    MCC = mcc,
                                    MNC = mnc,
                                    ECI28 = eci.ToString(),
                                    eNodeBId = enodebid,
                                    CID = cid,
                                    TAC = tac,
                                    PCI = pci,
                                    Bandwidth = bw,
                                    RSRP = level,
                                    EARFCN_DN = ch.EARFCN_DN,
                                    EARFCN_UP = ch.EARFCN_UP,
                                    FreqDn = ch.FreqDn,
                                    FreqUp = ch.FreqUp,
                                    StandartSubband = ch.StandartSubband,
                                };
                                GUIThreadDispatcher.Instance.Invoke(() =>
                                {
                                    IdentificationData.LTE.BTS.Add(dt);
                                });
                            }
                        }
                        catch (Exception exp)
                        {
                            App.Current.Dispatcher.BeginInvoke((Action)(() =>
                            {
                                MainWindow.exp.ExceptionData = new ExData()
                                {
                                    ex = exp,
                                    ClassName = "TSMxReceiver",
                                    AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name +
                                    "\r\n" + t + "\r\n"
                                };
                            }));
                        }
                        #endregion
                    }
                    else if (str[0].ToUpper().StartsWith("CDMA800"))
                    {
                        #region
                        try
                        {
                            string[] read = str[1].Split(',');
                            int sid = int.Parse((read[0].Split('/'))[1]);
                            int baseid = int.Parse(read[1]);
                            int pn = int.Parse(read[2]);
                            int channel = int.Parse(read[3]);
                            decimal freq = 1000000 * decimal.Parse(read[4].Replace(".", ","));
                            double level = double.Parse((read[5].Split(';'))[0]);

                            bool find = false;
                            for (int i = 0; i < IdentificationData.CDMA.BTS.Count; i++)
                            {
                                #region
                                if (IdentificationData.CDMA.BTS[i].SID == sid &&
                                    IdentificationData.CDMA.BTS[i].BaseID == baseid &&
                                    IdentificationData.CDMA.BTS[i].PN == pn &&
                                    IdentificationData.CDMA.BTS[i].ChannelN == channel)
                                {
                                    IdentificationData.CDMA.BTS[i].RSCP = level;

                                    find = true;
                                }
                                #endregion
                            }
                            if (find == false)
                            {
                                Equipment.CDMA_Channel ch = MainWindow.help.GetCDMACHFromChannel(channel);
                                CDMABTSData dt = new CDMABTSData()
                                {
                                    SID = sid,
                                    BaseID = baseid,
                                    PN = pn,
                                    RSCP = level,
                                    ChannelN = ch.ChannelN,
                                    FreqDn = ch.FreqDn,
                                    FreqUp = ch.FreqUp,
                                    StandartSubband = ch.StandartSubband,
                                };
                                GUIThreadDispatcher.Instance.Invoke(() =>
                                {
                                    IdentificationData.CDMA.BTS.Add(dt);
                                });
                            }
                        }
                        catch (Exception exp)
                        {
                            App.Current.Dispatcher.BeginInvoke((Action)(() =>
                            {
                                MainWindow.exp.ExceptionData = new ExData()
                                {
                                    ex = exp,
                                    ClassName = "TSMxReceiver",
                                    AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name +
                                    "\r\n" + t + "\r\n"
                                };
                            }));
                        }
                        #endregion
                    }

                    if (str[0].ToUpper().StartsWith("CDMA"))
                    {
                        App.Current.Dispatcher.BeginInvoke((Action)(() =>
                        {
                            MainWindow.exp.ExceptionData = new ExData()
                            {
                                ex = new Exception(),
                                ClassName = "CDMA",

                            };
                        }));
                    }
                    #endregion
                }
            }
        }

        class toread
        {
            public string Text { get; set; }
            public DateTime TextTime { get; set; }

        }
    }
}
