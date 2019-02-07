using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using RohdeSchwarz.ViCom;
using System.ComponentModel;
using RohdeSchwarz.ViCom.Net;
using RohdeSchwarz.ViCom.Net.GSM;
using RohdeSchwarz.ViCom.Net.LTE;
using RohdeSchwarz.ViCom.Net.CDMA;
using RohdeSchwarz.ViCom.Net.WCDMA;
using RohdeSchwarz.ViCom.Net.RFPOWERSCAN;
using RohdeSchwarz.ViCom.Net.GPS;
using RohdeSchwarz.ViCom.Net.ACD;
using RohdeSchwarz.ViCom.Net.CWScan;
using RohdeSchwarz.ViCom.Net.WIMAX;
using System.Diagnostics;

namespace ControlU.Equipment
{
    public class TSMxReceiver : INotifyPropertyChanged
    {

        string ViComBinPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\bin\"; //@"c:\RuS\RS-ViCom-Pro-16.25.0.743\bin\";//@"c:\RuS\RS-ViCom-16.5.0.0\bin\";
        LocalMeasurement LM = new LocalMeasurement();
        string AppPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        public delegate void DoubMod();
        public static DoubMod DM;
        public Thread Tr;
        public System.Timers.Timer tmr = new System.Timers.Timer(100);
        public System.Timers.Timer tmrUpdate = new System.Timers.Timer(10);
        public System.Timers.Timer tmrUpdateTrace = new System.Timers.Timer(2);

        public DB.localatdi_meas_device device_ident
        {
            get { return _device_ident; }
            set { _device_ident = value; }
        }
        private DB.localatdi_meas_device _device_ident = new DB.localatdi_meas_device() { };

        public DB.localatdi_meas_device device_meas
        {
            get { return _device_meas; }
            set { _device_meas = value; }
        }
        private DB.localatdi_meas_device _device_meas = new DB.localatdi_meas_device() { };

        #region Glob Bands Lists
        public static List<GSM_Band> GSM_BandFreqs = new List<GSM_Band>() { };
        //public static List<WCDMA_Band> WCDMA_BandFreqs = new List<GSM_Band>() { };
        private void SetGSMBandData()
        {
            int startarfcn = 0;
            int stoparfcn = 0;
            #region P_GSM900
            startarfcn = 1;
            stoparfcn = 124;
            List<GSM_BandFreqs> fd1 = new List<Equipment.GSM_BandFreqs>() { };
            for (int i = startarfcn; i <= stoparfcn; i++)
            {
                fd1.Add(new GSM_BandFreqs() { ARFCN = i, FreqDn = 935.2m + 0.2m * (i - startarfcn), FreqUp = 890.2m + 0.2m * (i - startarfcn) });
            }
            GSM_Band t1 = new GSM_Band()
            {
                Band = GSMBands.P_GSM900,
                FreqData = fd1,
                ARFCNStart = fd1[0].ARFCN,
                ARFCNStop = fd1[fd1.Count - 1].ARFCN,
                FreqDnStart = fd1[0].FreqDn * 1000000,
                FreqDnStop = fd1[fd1.Count - 1].FreqDn * 1000000,
                FreqUpStart = fd1[0].FreqUp * 1000000,
                FreqUpStop = fd1[fd1.Count - 1].FreqUp * 1000000,
            };
            GSM_BandFreqs.Add(t1);
            #endregion
            #region E_GSM900
            startarfcn = 975;
            stoparfcn = 1023;
            List<GSM_BandFreqs> fd2 = new List<Equipment.GSM_BandFreqs>() { };
            for (int i = startarfcn; i <= stoparfcn; i++)
            {
                fd2.Add(new GSM_BandFreqs() { ARFCN = i, FreqDn = 925.2m + 0.2m * (i - startarfcn), FreqUp = 880.2m + 0.2m * (i - startarfcn) });
            }
            startarfcn = 0;
            stoparfcn = 124;
            for (int i = startarfcn; i <= stoparfcn; i++)
            {
                fd2.Add(new GSM_BandFreqs() { ARFCN = i, FreqDn = 935 + 0.2m * (i - startarfcn), FreqUp = 890 + 0.2m * (i - startarfcn) });
            }
            GSM_Band t2 = new GSM_Band()
            {
                Band = GSMBands.E_GSM900,
                FreqData = fd2,
                ARFCNStart = fd2[0].ARFCN,
                ARFCNStop = fd2[fd2.Count - 1].ARFCN,
                FreqDnStart = fd2[0].FreqDn * 1000000,
                FreqDnStop = fd2[fd2.Count - 1].FreqDn * 1000000,
                FreqUpStart = fd2[0].FreqUp * 1000000,
                FreqUpStop = fd2[fd2.Count - 1].FreqUp * 1000000,
            };
            GSM_BandFreqs.Add(t2);
            #endregion
            #region GSM1800
            startarfcn = 512;
            stoparfcn = 885;
            List<GSM_BandFreqs> fd3 = new List<Equipment.GSM_BandFreqs>() { };
            for (int i = startarfcn; i <= stoparfcn; i++)
            {
                fd3.Add(new GSM_BandFreqs() { ARFCN = i, FreqDn = 1805.2m + 0.2m * (i - startarfcn), FreqUp = 1710.2m + 0.2m * (i - startarfcn) });
            }

            GSM_Band t3 = new GSM_Band()
            {
                Band = GSMBands.GSM1800,
                FreqData = fd3,
                ARFCNStart = fd3[0].ARFCN,
                ARFCNStop = fd3[fd3.Count - 1].ARFCN,
                FreqDnStart = fd3[0].FreqDn * 1000000,
                FreqDnStop = fd3[fd3.Count - 1].FreqDn * 1000000,
                FreqUpStart = fd3[0].FreqUp * 1000000,
                FreqUpStop = fd3[fd3.Count - 1].FreqUp * 1000000,
            };
            GSM_BandFreqs.Add(t3);
            #endregion
            #region GSM850
            startarfcn = 128;
            stoparfcn = 251;
            List<GSM_BandFreqs> fd4 = new List<Equipment.GSM_BandFreqs>() { };
            for (int i = startarfcn; i <= stoparfcn; i++)
            {
                fd4.Add(new GSM_BandFreqs() { ARFCN = i, FreqDn = 869.2m + 0.2m * (i - startarfcn), FreqUp = 824.2m + 0.2m * (i - startarfcn) });
            }
            GSM_Band t4 = new GSM_Band()
            {
                Band = GSMBands.GSM850,
                FreqData = fd4,
                ARFCNStart = fd4[0].ARFCN,
                ARFCNStop = fd4[fd4.Count - 1].ARFCN,
                FreqDnStart = fd4[0].FreqDn * 1000000,
                FreqDnStop = fd4[fd4.Count - 1].FreqDn * 1000000,
                FreqUpStart = fd4[0].FreqUp * 1000000,
                FreqUpStop = fd4[fd4.Count - 1].FreqUp * 1000000,
            };
            GSM_BandFreqs.Add(t4);
            #endregion
            #region GSM1900
            startarfcn = 512;
            stoparfcn = 810;
            List<GSM_BandFreqs> fd5 = new List<Equipment.GSM_BandFreqs>() { };
            for (int i = startarfcn; i <= stoparfcn; i++)
            {
                fd5.Add(new GSM_BandFreqs() { ARFCN = i, FreqDn = 1930.2m + 0.2m * (i - startarfcn), FreqUp = 1850.2m + 0.2m * (i - startarfcn) });
            }
            GSM_Band t5 = new GSM_Band()
            {
                Band = GSMBands.GSM1900,
                FreqData = fd5,
                ARFCNStart = fd5[0].ARFCN,
                ARFCNStop = fd5[fd5.Count - 1].ARFCN,
                FreqDnStart = fd5[0].FreqDn * 1000000,
                FreqDnStop = fd5[fd5.Count - 1].FreqDn * 1000000,
                FreqUpStart = fd5[0].FreqUp * 1000000,
                FreqUpStop = fd5[fd5.Count - 1].FreqUp * 1000000,
            };
            GSM_BandFreqs.Add(t5);
            #endregion
            #region R_GSM900
            startarfcn = 955;
            stoparfcn = 1023;
            List<GSM_BandFreqs> fd6 = new List<Equipment.GSM_BandFreqs>() { };
            for (int i = startarfcn; i <= stoparfcn; i++)
            {
                fd6.Add(new GSM_BandFreqs() { ARFCN = i, FreqDn = 921.2m + 0.2m * (i - startarfcn), FreqUp = 876.2m + 0.2m * (i - startarfcn) });
            }
            startarfcn = 0;
            stoparfcn = 124;
            for (int i = startarfcn; i <= stoparfcn; i++)
            {
                fd6.Add(new GSM_BandFreqs() { ARFCN = i, FreqDn = 935 + 0.2m * (i - startarfcn), FreqUp = 890 + 0.2m * (i - startarfcn) });
            }
            GSM_Band t6 = new GSM_Band()
            {
                Band = GSMBands.R_GSM900,
                FreqData = fd6,
                ARFCNStart = fd6[0].ARFCN,
                ARFCNStop = fd6[fd6.Count - 1].ARFCN,
                FreqDnStart = fd6[0].FreqDn * 1000000,
                FreqDnStop = fd6[fd6.Count - 1].FreqDn * 1000000,
                FreqUpStart = fd6[0].FreqUp * 1000000,
                FreqUpStop = fd6[fd6.Count - 1].FreqUp * 1000000,
            };
            GSM_BandFreqs.Add(t6);
            #endregion
            #region ER_GSM900
            startarfcn = 940;
            stoparfcn = 1023;
            List<GSM_BandFreqs> fd7 = new List<Equipment.GSM_BandFreqs>() { };
            for (int i = startarfcn; i <= stoparfcn; i++)
            {
                fd7.Add(new GSM_BandFreqs() { ARFCN = i, FreqDn = 918.2m + 0.2m * (i - startarfcn), FreqUp = 873.2m + 0.2m * (i - startarfcn) });
            }
            startarfcn = 0;
            stoparfcn = 124;
            for (int i = startarfcn; i <= stoparfcn; i++)
            {
                fd7.Add(new GSM_BandFreqs() { ARFCN = i, FreqDn = 935 + 0.2m * (i - startarfcn), FreqUp = 890 + 0.2m * (i - startarfcn) });
            }
            GSM_Band t7 = new GSM_Band()
            {
                Band = GSMBands.ER_GSM900,
                FreqData = fd7,
                ARFCNStart = fd7[0].ARFCN,
                ARFCNStop = fd7[fd7.Count - 1].ARFCN,
                FreqDnStart = fd7[0].FreqDn * 1000000,
                FreqDnStop = fd7[fd7.Count - 1].FreqDn * 1000000,
                FreqUpStart = fd7[0].FreqUp * 1000000,
                FreqUpStop = fd7[fd7.Count - 1].FreqUp * 1000000,
            };
            GSM_BandFreqs.Add(t7);
            #endregion
            #region GSM750
            startarfcn = 438;
            stoparfcn = 511;
            List<GSM_BandFreqs> fd8 = new List<Equipment.GSM_BandFreqs>() { };
            for (int i = startarfcn; i <= stoparfcn; i++)
            {
                fd8.Add(new GSM_BandFreqs() { ARFCN = i, FreqDn = 747.2m + 0.2m * (i - startarfcn), FreqUp = 777.2m + 0.2m * (i - startarfcn) });
            }
            GSM_Band t8 = new GSM_Band()
            {
                Band = GSMBands.GSM750,
                FreqData = fd8,
                ARFCNStart = fd8[0].ARFCN,
                ARFCNStop = fd8[fd8.Count - 1].ARFCN,
                FreqDnStart = fd8[0].FreqDn * 1000000,
                FreqDnStop = fd8[fd8.Count - 1].FreqDn * 1000000,
                FreqUpStart = fd8[0].FreqUp * 1000000,
                FreqUpStop = fd8[fd8.Count - 1].FreqUp * 1000000,
            };
            GSM_BandFreqs.Add(t8);
            #endregion
            #region GSM450
            startarfcn = 259;
            stoparfcn = 293;
            List<GSM_BandFreqs> fd9 = new List<Equipment.GSM_BandFreqs>() { };
            for (int i = startarfcn; i <= stoparfcn; i++)
            {
                fd9.Add(new GSM_BandFreqs() { ARFCN = i, FreqDn = 460.6m + 0.2m * (i - startarfcn), FreqUp = 450.6m + 0.2m * (i - startarfcn) });
            }
            GSM_Band t9 = new GSM_Band()
            {
                Band = GSMBands.GSM450,
                FreqData = fd9,
                ARFCNStart = fd9[0].ARFCN,
                ARFCNStop = fd9[fd9.Count - 1].ARFCN,
                FreqDnStart = fd9[0].FreqDn * 1000000,
                FreqDnStop = fd9[fd9.Count - 1].FreqDn * 1000000,
                FreqUpStart = fd9[0].FreqUp * 1000000,
                FreqUpStop = fd9[fd9.Count - 1].FreqUp * 1000000,
            };
            GSM_BandFreqs.Add(t9);
            #endregion
            #region GSM480
            startarfcn = 306;
            stoparfcn = 340;
            List<GSM_BandFreqs> fd10 = new List<Equipment.GSM_BandFreqs>() { };
            for (int i = startarfcn; i <= stoparfcn; i++)
            {
                fd10.Add(new GSM_BandFreqs() { ARFCN = i, FreqDn = 489m + 0.2m * (i - startarfcn), FreqUp = 479m + 0.2m * (i - startarfcn) });
            }
            GSM_Band t10 = new GSM_Band()
            {
                Band = GSMBands.GSM480,
                FreqData = fd10,
                ARFCNStart = fd10[0].ARFCN,
                ARFCNStop = fd10[fd10.Count - 1].ARFCN,
                FreqDnStart = fd10[0].FreqDn * 1000000,
                FreqDnStop = fd10[fd10.Count - 1].FreqDn * 1000000,
                FreqUpStart = fd10[0].FreqUp * 1000000,
                FreqUpStop = fd10[fd10.Count - 1].FreqUp * 1000000,
            };
            GSM_BandFreqs.Add(t10);
            #endregion
        }
        #endregion


        string _Time;
        public string Time
        {
            get { return _Time; }
            set { _Time = value; OnPropertyChanged("Time"); }
        }
        private ObservableCollection<AllTSMxUnqData> AllUniqueData = new ObservableCollection<AllTSMxUnqData>
        {
        };
        #region  RFPowerScan
        private static tracepoint[] _TracefromDev;
        public static tracepoint[] TracefromDev
        {
            get { return _TracefromDev; }
            set { _TracefromDev = value; }
        }
        private tracepoint[] _Trace;
        public tracepoint[] Trace
        {
            get { return _Trace; }
            set { _Trace = value; OnPropertyChanged("Trace"); }
        }
        public bool FreqSet
        {
            get { return _FreqSet; }
            set { _FreqSet = value; OnPropertyChanged("FreqSet"); }
        }
        private static bool _FreqSet = false;

        public bool NewTrace
        {
            get { return _NewTrace; }
            set { _NewTrace = value; }
        }
        private static bool _NewTrace = false;

        private static decimal _FreqStep = 6250;
        public decimal FreqStep
        {
            get { return _FreqStep; }
            set { _FreqStep = value; }
        }
        private static int _TracePoints = 1601;
        public int TracePoints
        {
            get { return _TracePoints; }
            set { _TracePoints = value; FreqStep = (_FreqStop - _FreqStart) / TracePoints; }
        }

        //bool _FreqTrue = false;
        //public bool FreqTrue
        //{
        //    get { return _FreqTrue; }
        //    set { _FreqTrue = value; OnPropertyChanged("FreqTrue"); }
        //}
        public decimal FreqCentr
        {
            get { return _FreqCentr; }
            set
            {
                _FreqCentr = value;
                _FreqStart = _FreqCentr - _FreqSpan / 2; OnPropertyChanged("FreqStart");
                _FreqStop = _FreqCentr + _FreqSpan / 2; OnPropertyChanged("FreqStop");
                OnPropertyChanged("FreqCentr");
            }
        }
        private static decimal _FreqCentr = 950000000;//2142400000;

        public decimal FreqSpan
        {
            get { return _FreqSpan; }
            set
            {
                _FreqSpan = value;
                _FreqStart = _FreqCentr - _FreqSpan / 2; OnPropertyChanged("FreqStart");
                _FreqStop = _FreqCentr + _FreqSpan / 2; OnPropertyChanged("FreqStop");
                OnPropertyChanged("FreqSpan");
            }
        }
        private static decimal _FreqSpan = 10000000;

        public decimal FreqStart
        {
            get { return _FreqStart; }
            set
            {
                _FreqStart = value;
                _FreqCentr = (_FreqStart + _FreqStop) / 2; OnPropertyChanged("FreqCentr");
                _FreqSpan = _FreqStop - _FreqStart; OnPropertyChanged("FreqSpan");
                OnPropertyChanged("FreqStart");
            }
        }
        private static decimal _FreqStart = 945000000;//2139900000;//1800000000;//2600000000;//2490000000;//

        public decimal FreqStop
        {
            get { return _FreqStop; }
            set
            {
                _FreqStop = value;
                _FreqCentr = (_FreqStart + _FreqStop) / 2; OnPropertyChanged("FreqCentr");
                _FreqSpan = _FreqStop - _FreqStart; OnPropertyChanged("FreqSpan");
                OnPropertyChanged("FreqStop");
            }
        }
        private static decimal _FreqStop = 955000000;//2144900000;//1900000000;//2700000000;//2510000000;//
        #region level
        private double _RefLevel = -40;
        public double RefLevel
        {
            get { return _RefLevel; }
            set { _RefLevel = value; RefLevelStr = String.Concat(_RefLevel, " ", "dBm");/*µV");*/ OnPropertyChanged("RefLevel"); }
        }
        private string _RefLevelStr = "-60 dBm";
        public string RefLevelStr
        {
            get { return _RefLevelStr; }
            set { _RefLevelStr = value; OnPropertyChanged("RefLevelStr"); }
        }
        private double _LowestLevel = -140;
        public double LowestLevel
        {
            get { return _LowestLevel; }
            set { _LowestLevel = value; OnPropertyChanged("LowestLevel"); }
        }
        private double _Range = 100;
        public double Range
        {
            get { return _Range; }
            set
            {
                _Range = value;
                LowestLevel = _RefLevel - _Range;
                OnPropertyChanged("Range");
            }
        }
        private string _LevelUnitStr = "dBm";
        public string LevelUnitStr
        {
            get { return _LevelUnitStr; }
            set { _LevelUnitStr = value; OnPropertyChanged("LevelUnitStr"); }
        }
        #endregion
        #endregion
        #region TechOnThisScaner

        /// <summary>
        /// 0: не подключались и незнаем 1: подключались и есть 2: подключались и нет
        /// </summary>
        public int Option_GNSS
        {
            get { return App.Sett.TSMxReceiver_Settings.Option.GNSS; }
            set { App.Sett.TSMxReceiver_Settings.Option.GNSS = value; OnPropertyChanged("Option_GNSS"); }
        }

        /// <summary>
        /// 0: не подключались и незнаем 1: подключались и есть 2: подключались и нет
        /// </summary>
        public int Option_GSM
        {
            get { return App.Sett.TSMxReceiver_Settings.Option.GSM; }
            set { App.Sett.TSMxReceiver_Settings.Option.GSM = value; OnPropertyChanged("Option_GSM"); }
        }

        /// <summary>
        /// 0: не подключались и незнаем 1: подключались и есть 2: подключались и нет
        /// </summary>
        public int Option_UMTS
        {
            get { return App.Sett.TSMxReceiver_Settings.Option.UMTS; }
            set { App.Sett.TSMxReceiver_Settings.Option.UMTS = value; OnPropertyChanged("Option_UMTS"); }
        }

        /// <summary>
        /// 0: не подключались и незнаем 1: подключались и есть 2: подключались и нет
        /// </summary>
        public int Option_LTE
        {
            get { return App.Sett.TSMxReceiver_Settings.Option.LTE; }
            set { App.Sett.TSMxReceiver_Settings.Option.LTE = value; OnPropertyChanged("Option_LTE"); }
        }

        /// <summary>
        /// 0: не подключались и незнаем 1: подключались и есть 2: подключались и нет
        /// </summary>
        public int Option_CDMA
        {
            get { return App.Sett.TSMxReceiver_Settings.Option.CDMA; }
            set { App.Sett.TSMxReceiver_Settings.Option.CDMA = value; OnPropertyChanged("Option_CDMA"); }
        }

        /// <summary>
        /// 0: не подключались и незнаем 1: подключались и есть 2: подключались и нет
        /// </summary>
        public int Option_RFPS
        {
            get { return App.Sett.TSMxReceiver_Settings.Option.RFPS; }
            set { App.Sett.TSMxReceiver_Settings.Option.RFPS = value; OnPropertyChanged("Option_RFPS"); }
        }

        /// <summary>
        /// 0: не подключались и незнаем 1: подключались и есть 2: подключались и нет
        /// </summary>
        public int Option_ACD
        {
            get { return App.Sett.TSMxReceiver_Settings.Option.ACD; }
            set { App.Sett.TSMxReceiver_Settings.Option.ACD = value; OnPropertyChanged("Option_ACD"); }
        }
        #endregion TechOnThisScaner
        #region runs
        bool _Run;
        public bool Run
        {
            get { return _Run; }
            set
            {
                _Run = value;
                if (Run)
                {
                    GetData = true;
                    SetNewStateDevice();// ConnectToDevice();
                }
                else if (!Run)
                {
                    SetNewStateDevice(); // DisconnectFromDevice();
                }
                OnPropertyChanged("Run");
            }
        }

        private static bool IsRuningT;
        public bool IsRuning
        {
            get { return _IsRuning; }
            set { _IsRuning = value; OnPropertyChanged("IsRuning"); }
        }
        private bool _IsRuning;

        public bool GetData
        {
            get { return _GetData; }
            set { _GetData = value; /*OnPropertyChanged("OutText"); */}
        }
        public bool _GetData;// = "";

        public bool GPSIsRuning
        {
            get { return _GPSIsRuning; }
            set { _GPSIsRuning = value; OnPropertyChanged("GPSIsRuning"); }
        }
        private static bool _GPSIsRuning;

        public bool RFPSIsRuning
        {
            get { return _RFPSIsRuning; }
            set { _RFPSIsRuning = value; OnPropertyChanged("RFPSIsRuning"); }
        }
        private static bool _RFPSIsRuning;

        public bool GSMIsRuning
        {
            get { return IdentificationData.GSM.IsRuning; }
            set { IdentificationData.GSM.IsRuning = value; OnPropertyChanged("GSMIsRuning"); }
        }

        public bool UMTSIsRuning
        {
            get { return IdentificationData.UMTS.IsRuning; }
            set { IdentificationData.UMTS.IsRuning = value; OnPropertyChanged("UMTSIsRuning"); }
        }

        public bool LTEIsRuning
        {
            get { return IdentificationData.LTE.IsRuning; }
            set { IdentificationData.LTE.IsRuning = value; OnPropertyChanged("LTEIsRuning"); }
        }

        public bool CDMAIsRuning
        {
            get { return IdentificationData.CDMA.IsRuning; }
            set { IdentificationData.CDMA.IsRuning = value; OnPropertyChanged("CDMAIsRuning"); }
        }

        public bool ACDIsRuning
        {
            get { return IdentificationData.ACD.IsRuning; }
            set { IdentificationData.ACD.IsRuning = value; OnPropertyChanged("ACDIsRuning"); }
        }
        //private static bool _ACDIsRuning;
        #endregion
        private static long _LastUpdate;// = "";
        public long LastUpdate
        {
            get { return _LastUpdate; }
            set { _LastUpdate = value; /*OnPropertyChanged("OutText"); */}
        }
        private static int _ticks = 0;// = "";
        public int ticks
        {
            get { return _ticks; }
            set { _ticks = value; OnPropertyChanged("ticks"); }
        }

        private AllTSMxUnqData _UniqueData = new AllTSMxUnqData { };
        public AllTSMxUnqData UniqueData
        {
            get { return _UniqueData; }
            set { _UniqueData = value; OnPropertyChanged("UniqueData"); }
        }
        public ObservableCollection<ACD_Data> ACDData
        {
            get { return _ACDData; }
            set { _ACDData = value; OnPropertyChanged("ACDData"); }
        }
        private ObservableCollection<ACD_Data> _ACDData = new ObservableCollection<ACD_Data> { };
        #region Meas

        public bool AnyMeas
        {
            get { return _AnyMeas; }
            set { _AnyMeas = value; OnPropertyChanged("AnyMeas"); }
        }
        private bool _AnyMeas = false;

        /// <summary>
        /// Количевство измерений на частоте
        /// </summary>
        public int MeasTraceCountOnFreq
        {
            get { return _MeasTraceCountOnFreq; }
            set { _MeasTraceCountOnFreq = value; OnPropertyChanged("MeasTraceCountOnFreq"); }
        }
        private int _MeasTraceCountOnFreq = 10;

        public DB.MeasData MeasMonItem
        {
            get { return _MeasMonItem; }
            set { _MeasMonItem = value; OnPropertyChanged("MeasMonItem"); }
        }
        private DB.MeasData _MeasMonItem = new DB.MeasData() { };



        public bool GSMFilter
        {
            get { return _GSMFilter; }
            set { _GSMFilter = value; OnPropertyChanged("GSMFilter"); }
        }
        private bool _GSMFilter = false;

        public bool IsMeasMon
        {
            get { return _IsMeasMon; }
            set
            {
                _IsMeasMon = value;
                if (_IsMeasMon)
                {
                    if (MainWindow.db_v2.MeasMon.Mode == 0) DM += SetMeasMonChannel;
                    if (MainWindow.db_v2.MeasMon.Mode == 1) { /*GetMeasMonBands(); */SetMeasMonBands(); DM += SetMeasMonBand; }
                }
                else
                {
                    if (MainWindow.db_v2.MeasMon.Mode == 0) DM -= SetMeasMonChannel;
                    if (MainWindow.db_v2.MeasMon.Mode == 1) DM -= SetMeasMonBand;
                }
                AnyMeas = _IsMeasMon;
                OnPropertyChanged("IsMeasMon");
            }
        }
        private bool _IsMeasMon = false;
        public long MeasMonTimeMeas
        {
            get { return _MeasMonTimeMeas; }
            set { _MeasMonTimeMeas = value; OnPropertyChanged("MeasMonTimeMeas"); }
        }
        private long _MeasMonTimeMeas = 0;



        #endregion



        //public string TsmxIpAddess = "192.168.2.2";
        private ObservableCollection<gsmunifreq> GSMUniFreqStartStop = new ObservableCollection<gsmunifreq>() { };
        private static ObservableCollection<decimal> GSMUniFreq = new ObservableCollection<decimal>() { };
        private static ObservableCollection<decimal> UMTSUniFreq = new ObservableCollection<decimal>() { };
        private static ObservableCollection<decimal> LTEUniFreq = new ObservableCollection<decimal>() { };
        private static ObservableCollection<Settings.CDMAFreqs_Set> CDMAUniFreq = new ObservableCollection<Settings.CDMAFreqs_Set>() { };
        public class gsmunifreq
        {
            public decimal Start { get; set; }
            public decimal Stop { get; set; }
        }
        public static ObservableCollection<GSM_Channel> GSMChannelsToScan { get; set; }

        public static ObservableCollection<CDMAFPichData> CDMAFPichfromDev { get; set; }

        public string _OutText = "";
        public string OutText
        {
            get { return _OutText; }
            set { _OutText = value; OnPropertyChanged("OutText"); }
        }
        public int RFBufer
        {
            get { return _RFBufer; }
            set { _RFBufer = value; OnPropertyChanged("RFBufer"); }
        }
        private int _RFBufer = 0;
        //int _RFPowerScanResults = 0;
        //public int RFPowerScanResults
        //{
        //    get { return _RFPowerScanResults; }
        //    set { _RFPowerScanResults = value; OnPropertyChanged("RFPowerScanResults"); }
        //}
        #region instr data
        private string _InstrModel = "";
        public string InstrModel
        {
            get { return _InstrModel; }
            set { _InstrModel = value; OnPropertyChanged("InstrModel"); }
        }

        public string InstrSerialNumber
        {
            get { return App.Sett.TSMxReceiver_Settings.SerialNumber; }
            set { App.Sett.TSMxReceiver_Settings.SerialNumber = value; OnPropertyChanged("InstrSerialNumber"); }
        }
        private static string SerialNumberTemp = "";

        public static DeviceType DeviceType
        {
            get
            {
                if (App.Sett.TSMxReceiver_Settings.DeviceType == 0)
                { return RohdeSchwarz.ViCom.Net.DeviceType.Tsmw; }
                else if (App.Sett.TSMxReceiver_Settings.DeviceType == 1)
                { return RohdeSchwarz.ViCom.Net.DeviceType.Tsme; }
                else if (App.Sett.TSMxReceiver_Settings.DeviceType == 2)
                { return RohdeSchwarz.ViCom.Net.DeviceType.Tsme6; }
                else return RohdeSchwarz.ViCom.Net.DeviceType.Tsme;
            }
            private set { }
        }
        #endregion

        UserLowLevelErrorMessageHandler.LowLevelErrorHandlerRegistry LowLevelErrorHandlerRegistry;
        LowLevelErrorHandlerImplementation MyLowLevelErrorHandlerImplementation;
        MessageTracer rMessageTracer = new MessageTracer();
        CViComError error;
        CReceiverListener receiverListener;
        SConnectedReceiverTable myReceivers;
        public static List<SConnectedReceiverTable.SReceiver.SDeviceOption> option { get; set; }

        RohdeSchwarz.ViCom.Net.CViComBasicInterface BasicInterface;


        CViComLoader<CViComGpsInterface> gpsLoader;
        CViComGpsInterface GpsInterface;
        CViComBasicInterface GpsBasicInterface;
        CViComGpsInterfaceDataProcessor GPSListener;

        CViComLoader<CViComGsmInterface> gsmLoader;
        static CViComGsmInterface gsmInterface;
        CViComBasicInterface gsmBasicInterface;
        CViComGsmInterfaceDataProcessor GSMListener;

        static CViComLoader<CViComWcdmaInterface> wcdmaLoader;
        static CViComWcdmaInterface wcdmaInterface;
        static CViComBasicInterface wcdmaBasicInterface;
        static CViComWcdmaInterfaceDataProcessor WCDMAListener;

        static CViComLoader<CViComCdmaInterface> cdmaLoader;
        static CViComCdmaInterface cdmaInterface;
        static CViComBasicInterface cdmaBasicInterface;
        static CViComCdmaInterfaceDataProcessor CDMAListener;

        static CViComLoader<CViComLteInterface> lteLoader;
        static CViComLteInterface lteInterface;
        static CViComBasicInterface lteBasicInterface;
        static CViComLteInterfaceDataProcessor LteListener;

        CViComLoader<CViComRFPowerScanInterface> RFPowerScanLoader;
        static CViComRFPowerScanInterface RFPowerScanInterface;
        CViComBasicInterface RFPowerScanBasicInterface;
        CViComRFPowerScanInterfaceDataProcessor RFPowerScanListener;

        CViComLoader<CViComAcdInterface> acdLoader;
        CViComAcdInterface acdInterface;
        CViComBasicInterface acdBasicInterface;
        CViComAcdInterfaceDataProcessor ACDListener;

        CViComLoader<CViComCWScanInterface> cwScanLoader;

        static SSweepSettings rSSweepSettings = new SSweepSettings();

        public static double DetectionLevelGSM = -100;
        public static double DetectionLevelUMTS = -100;
        public static double DetectionLevelLTE = -100;
        public static double DetectionLevelCDMA = -100;

        public TSMxReceiver()
        {
            Trace = new tracepoint[TracePoints];
            decimal step = FreqSpan / (TracePoints - 1);
            for (int i = 0; i < TracePoints; i++)
            {
                tracepoint p = new tracepoint()
                {
                    freq = FreqStart + step * i,
                    level = -100
                };
                Trace[i] = p;
            }
            TracefromDev = new tracepoint[TracePoints];
            for (int i = 0; i < TracePoints; i++)
            {
                tracepoint p = new tracepoint()
                {
                    freq = FreqStart + step * i,
                    level = -100
                };
                TracefromDev[i] = p;
            }
            //===========================================================================================================================================================
            // Add the bin-directory of this installation to the PATH variable, so the ViCom dlls can be found
            // ver = RS-ViCom-16.5.0.0 
            //set bin path in ViComBinPath
            //===========================================================================================================================================================
            string newPath = Environment.GetEnvironmentVariable("PATH") + @";" + ViComBinPath + ";";
            Environment.SetEnvironmentVariable("PATH", newPath);

            option = new List<SConnectedReceiverTable.SReceiver.SDeviceOption>() { };

            //GSMChannelsToScan = new ObservableCollection<GSM_Channel>() { };
            SetGSMBandData();



            FreqCentr = 2150000000;
            FreqSpan = 100000000;

            CDMAFPichfromDev = new ObservableCollection<CDMAFPichData>() { };
            //App.Sett.TSMxReceiver_Settings.PropertyChanged += DeviceType_PropertyChanged;
            App.Sett.TSMxReceiver_Settings.GSM.PropertyChanged += TechIsEnabled_PropertyChanged;
            App.Sett.TSMxReceiver_Settings.UMTS.PropertyChanged += TechIsEnabled_PropertyChanged;
            App.Sett.TSMxReceiver_Settings.LTE.PropertyChanged += TechIsEnabled_PropertyChanged;
            App.Sett.TSMxReceiver_Settings.CDMA.PropertyChanged += TechIsEnabled_PropertyChanged;
            App.Sett.TSMxReceiver_Settings.ACD.PropertyChanged += TechIsEnabled_PropertyChanged;

        }
        private void GetTechSettings()
        {
            DetectionLevelGSM = App.Sett.MeasMons_Settings.GSM.DetectionLevel;
            DetectionLevelUMTS = App.Sett.MeasMons_Settings.UMTS.DetectionLevel;
            DetectionLevelLTE = App.Sett.MeasMons_Settings.LTE.DetectionLevel;
            DetectionLevelCDMA = App.Sett.MeasMons_Settings.CDMA.DetectionLevel;
            if (MainWindow.db_v2.MeasMon.FromTask == true)
            {
                #region инфа из ATDI 
                foreach (DB.localatdi_task_with_tech restech in MainWindow.db_v2.AtdiTask.data_from_tech)
                {
                    if (restech.tech.Contains("GSM") && restech.scan_parameters.Length == 1)
                    {
                        if (restech.scan_parameters[0].detection_level_dbm > -1000)
                            DetectionLevelGSM = restech.scan_parameters[0].detection_level_dbm;
                    }
                    if (restech.tech.Contains("UMTS") && restech.scan_parameters.Length == 1)
                    {
                        if (restech.scan_parameters[0].detection_level_dbm > -1000)
                            DetectionLevelUMTS = restech.scan_parameters[0].detection_level_dbm;
                    }
                    if (restech.tech.Contains("LTE") && restech.scan_parameters.Length > 0)
                    {
                        if (restech.scan_parameters[0].detection_level_dbm > -1000)
                            DetectionLevelLTE = restech.scan_parameters[0].detection_level_dbm;
                    }
                    if (restech.tech.Contains("CDMA") && restech.scan_parameters.Length == 1)
                    {
                        if (restech.scan_parameters[0].detection_level_dbm > -1000)
                            DetectionLevelCDMA = restech.scan_parameters[0].detection_level_dbm;
                    }
                }
                #endregion
            }

        }
        private void TechIsEnabled_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "TechIsEnabled")
            {
                if (Run == true && sender.ToString().Contains("GSM") && App.Sett.TSMxReceiver_Settings.GSM.TechIsEnabled == true && (gsmLoader == null || gsmLoader.Connected == false))
                {
                    IdentificationData.GSM.BTS.Clear();
                    DM += SetNewStateGSM;
                }
                if (Run == true && sender.ToString().Contains("GSM") && App.Sett.TSMxReceiver_Settings.GSM.TechIsEnabled == false && (gsmLoader != null || gsmLoader.Connected == true))
                {
                    for (int i = 0; i < MainWindow.db_v2.MeasMon.Data.Count; i++)
                    {
                        if (MainWindow.db_v2.MeasMon.Data[i].Techonology == "GSM")
                        {
                            MainWindow.db_v2.MeasMon.Data.Remove(MainWindow.db_v2.MeasMon.Data[i]);
                            i--;
                        }
                    }
                    DM += SetNewStateGSM;
                }

                if (Run == true && sender.ToString().Contains("UMTS") && App.Sett.TSMxReceiver_Settings.UMTS.TechIsEnabled == true && (wcdmaLoader == null || wcdmaLoader.Connected == false))
                {
                    IdentificationData.UMTS.BTS.Clear();
                    DM += SetNewStateUMTS;
                }
                if (Run == true && sender.ToString().Contains("UMTS") && App.Sett.TSMxReceiver_Settings.UMTS.TechIsEnabled == false && (wcdmaLoader != null || wcdmaLoader.Connected == true))
                {
                    for (int i = 0; i < MainWindow.db_v2.MeasMon.Data.Count; i++)
                    {
                        if (MainWindow.db_v2.MeasMon.Data[i].Techonology == "UMTS")
                        {
                            MainWindow.db_v2.MeasMon.Data.Remove(MainWindow.db_v2.MeasMon.Data[i]);
                            i--;
                        }
                    }
                    DM += SetNewStateUMTS;
                }

                if (Run == true && sender.ToString().Contains("LTE") && App.Sett.TSMxReceiver_Settings.LTE.TechIsEnabled == true && (lteLoader == null || lteLoader.Connected == false))
                {
                    IdentificationData.LTE.BTS.Clear();

                    DM += SetNewStateLTE;
                }
                if (Run == true && sender.ToString().Contains("LTE") && App.Sett.TSMxReceiver_Settings.LTE.TechIsEnabled == false && (lteLoader != null || lteLoader.Connected == true))
                {
                    for (int i = 0; i < MainWindow.db_v2.MeasMon.Data.Count; i++)
                    {
                        if (MainWindow.db_v2.MeasMon.Data[i].Techonology == "LTE")
                        {
                            MainWindow.db_v2.MeasMon.Data.Remove(MainWindow.db_v2.MeasMon.Data[i]);
                            i--;
                        }
                    }
                    DM += SetNewStateLTE;
                }


                if (Run == true && sender.ToString().Contains("CDMA") && App.Sett.TSMxReceiver_Settings.CDMA.TechIsEnabled == true && (cdmaLoader == null || cdmaLoader.Connected == false))
                {
                    IdentificationData.CDMA.BTS.Clear();
                    DM += SetNewStateCDMA;
                }
                if (Run == true && sender.ToString().Contains("CDMA") && App.Sett.TSMxReceiver_Settings.CDMA.TechIsEnabled == false && (cdmaLoader != null || cdmaLoader.Connected == true))
                {
                    for (int i = 0; i < MainWindow.db_v2.MeasMon.Data.Count; i++)
                    {
                        if (MainWindow.db_v2.MeasMon.Data[i].Techonology == "CDMA" || MainWindow.db_v2.MeasMon.Data[i].Techonology == "EVDO")
                        {
                            MainWindow.db_v2.MeasMon.Data.Remove(MainWindow.db_v2.MeasMon.Data[i]);
                            i--;
                        }
                    }
                    DM += SetNewStateCDMA;
                }
                if (Run == true && sender.ToString().Contains("ACD") && App.Sett.TSMxReceiver_Settings.ACD.TechIsEnabled == true && (acdLoader == null || acdLoader.Connected == false))
                {
                    IdentificationData.ACD.ACDData.Clear();
                    DM += SetNewStateACD;
                }
                if (Run == true && sender.ToString().Contains("ACD") && App.Sett.TSMxReceiver_Settings.ACD.TechIsEnabled == false && (acdLoader != null || acdLoader.Connected == true))
                {
                    DM += SetNewStateACD;
                }
            }
        }


        /// <summary>
        /// возвращает какой вход для чего используется
        /// True = Identification, False = Spectrum Analyzer
        /// </summary>
        /// <param name="type">Тип данных True = Identification, False = Spectrum Analyzer</param>
        /// <param name="rf">номер обработчика, используется только для rf power scan</param>
        /// <param name="tech">текстом технология GSM/UMTS/LTE/CDMA</param>
        /// <returns></returns>
        private static uint GetDeviceRFInput(bool type, uint rf, string tech)
        {
            uint rfin = 0;
            if (DeviceType == RohdeSchwarz.ViCom.Net.DeviceType.Tsmw)
            {
                if (type == true)
                {
                    if (App.Sett.TSMxReceiver_Settings.TSMWRFInput == 1) rfin = 1;
                    else if (App.Sett.TSMxReceiver_Settings.TSMWRFInput == 2) rfin = 2;
                    else if (App.Sett.TSMxReceiver_Settings.TSMWRFInput == 3) rfin = rf;
                    else if (App.Sett.TSMxReceiver_Settings.TSMWRFInput == 4)
                    {
                        if (tech == "GSM") rfin = (uint)App.Sett.TSMxReceiver_Settings.GSM.TSMWRfInput;
                        else if (tech == "UMTS") rfin = (uint)App.Sett.TSMxReceiver_Settings.UMTS.TSMWRfInput;
                        else if (tech == "LTE") rfin = (uint)App.Sett.TSMxReceiver_Settings.LTE.TSMWRfInput;
                        else if (tech == "CDMA") rfin = (uint)App.Sett.TSMxReceiver_Settings.CDMA.TSMWRfInput;
                    }
                }
                else if (type == false)
                {
                    if (App.Sett.TSMxReceiver_Settings.TSMWRFInput == 1) rfin = 2;
                    else if (App.Sett.TSMxReceiver_Settings.TSMWRFInput == 2) rfin = 1;
                    else if (App.Sett.TSMxReceiver_Settings.TSMWRFInput == 3) rfin = rf;
                }
            }
            else if (DeviceType == RohdeSchwarz.ViCom.Net.DeviceType.Tsme)
            {
                rfin = 1;
            }
            else if (DeviceType == RohdeSchwarz.ViCom.Net.DeviceType.Tsme6)
            {
                rfin = 1;
            }

            if (DeviceType == RohdeSchwarz.ViCom.Net.DeviceType.Unknown) ((SplashWindow)App.Current.MainWindow).m_mainWindow.Message = "Выберете модель приемника R&S TSMx";
            if (App.Sett.TSMxReceiver_Settings.TSMWRFInput == 0) ((SplashWindow)App.Current.MainWindow).m_mainWindow.Message = "Выберете тип подлючения антенных входов R&S TSMW";
            return rfin;
        }




        #region Connect/Disconnect
        private void SetNewStateDevice()
        {
            if (App.Sett.TSMxReceiver_Settings.IPAdress != "")
            {
                DoubMod Connect = ConnectToDevice;
                DoubMod Disconnect = DisconnectFromDevice;
                bool findConnect = false;
                bool findDisconnect = false;
                if (DM != null && Connect != null && Disconnect != null)
                {
                    foreach (Delegate d in DM.GetInvocationList())
                    {
                        if (d.Method.Name == Connect.GetInvocationList()[0].Method.Name) findConnect = true;
                        if (d.Method.Name == Disconnect.GetInvocationList()[0].Method.Name) findDisconnect = true;
                    }
                }
                bool AnyTechRunning = false;
                if (GSMIsRuning) AnyTechRunning = true;
                if (UMTSIsRuning) AnyTechRunning = true;
                if (LTEIsRuning) AnyTechRunning = true;
                if (CDMAIsRuning) AnyTechRunning = true;
                if (ACDIsRuning) AnyTechRunning = true;
                if (RFPSIsRuning) AnyTechRunning = true;
                if (Run != AnyTechRunning)
                {
                    if (Run == true)
                    {
                        if (findConnect == false && findDisconnect == false)
                        {
                            MainWindow.db_v2.MeasMon.Data.Clear();
                            DM = sameWork;
                            Tr = new Thread(AllTimeWorks);
                            Tr.Name = "RsTSMxThread";
                            Tr.IsBackground = true;
                            LowLevelErrorHandlerRegistry = new UserLowLevelErrorMessageHandler.LowLevelErrorHandlerRegistry();
                            MyLowLevelErrorHandlerImplementation = new LowLevelErrorHandlerImplementation();
                            LowLevelErrorHandlerRegistry.Register(MyLowLevelErrorHandlerImplementation);
                            ViComMessageTracerRegistry.Register(rMessageTracer);
                            Tr.Start();
                            Connect();
                        }
                    }
                    else
                    {
                        if (findConnect == false && findDisconnect == false)
                        {
                            Disconnect();// DM += (DoubMod)Disconnect.GetInvocationList()[0];
                            LowLevelErrorHandlerRegistry.Unregister(MyLowLevelErrorHandlerImplementation);
                        }
                    }
                }
                else { /*DM -= SetNewStateDevice; */}
            }
            else
            {
                Run = false;
                string str = ((SplashWindow)App.Current.MainWindow).m_mainWindow.FindResource("NotSetIPAddressEquipment").ToString()
                    .Replace("*Equipment*", ((SplashWindow)App.Current.MainWindow).m_mainWindow.FindResource("EqNetworkAnalyzer").ToString());
                ((SplashWindow)App.Current.MainWindow).m_mainWindow.Message = str;
            }
        }
        private void ConnectToDevice()
        {
            try
            {
                receiverListener = new CReceiverListener();

                DM += DetectScanner;
                if (IsMeasMon) DM += SetMeasMonChannel;

                // создаем таймер
                tmr.AutoReset = true;
                tmr.Enabled = true;
                tmr.Elapsed += WatchDog;
                tmr.Start();
                tmrUpdate.AutoReset = true;
                tmrUpdate.Enabled = true;
                tmrUpdate.Elapsed += UpdateData;
                tmrUpdate.Start();
            }
            catch { }
        }
        private void DisconnectFromDevice()
        {
            if (GPSIsRuning) DM += GpsDisconnect;
            if (App.Sett.TSMxReceiver_Settings.GSM.TechIsEnabled == true && GSMIsRuning)
            { DM += GsmDisconnect; }
            if (App.Sett.TSMxReceiver_Settings.UMTS.TechIsEnabled == true && UMTSIsRuning)
            { DM += UmtsDisconnect; }
            if (App.Sett.TSMxReceiver_Settings.LTE.TechIsEnabled == true && LTEIsRuning)
            { DM += LTEDisconnect; }
            if (App.Sett.TSMxReceiver_Settings.CDMA.TechIsEnabled == true && CDMAIsRuning)
            { DM += CDMADisconnect; }
            if (App.Sett.TSMxReceiver_Settings.ACD.TechIsEnabled == true && ACDIsRuning)
            { DM += ACDDisconnect; }
            DM -= sameWork;
            DM -= SetMeasMonChannel;
            if (App.Sett.TSMxReceiver_Settings.UseRFPowerScan == true && RFPSIsRuning)
            {
                DM += RfPowerScanDisconnect;//////////////////////////////////////////////////////////////////
            }
            DM += SetDisconnect;
        }
        private void SetDisconnect()
        {
            bool methods = false;
            foreach (Delegate d in DM.GetInvocationList())
            {
                if (methods == false)
                { methods = ((DoubMod)d).Method.Name != "SetDisconnect"; }
            }
            if (methods == false)
            {
                DM -= SetDisconnect;

                tmr.Stop();
                tmrUpdate.Stop();
                //tmrUpdateTrace.Stop();
                _GetData = false;
            }
        }
        private void AllTimeWorks()
        {
            while (GetData)
            {
                //long beginTiks = DateTime.Now.Ticks;
                //foreach (Delegate d in TelnetDM.GetInvocationList())
                //{
                //    Temp += ((DoubMod)d).Method.Name + "\r\n";
                //}
                DM();
                IsRuning = IsRuningT;
                //GUIThreadDispatcher.Instance.Invoke(() =>
                //{
                //    IsRuning = IsRuningT;
                //});

                //LastUpdate = DateTime.Now.Ticks;
            }
            //System.Windows.MessageBox.Show("Disconnected");
            DM -= sameWork;
            IsRuning = false;
            Tr.Abort();
        }

        #endregion

        public void sameWork()
        {
            Thread.Sleep(1);
            try
            {
                if (RFPowerScanLoader != null && RFPowerScanLoader.Connected && RFPowerScanLoader.GetBasicInterface().IsMeasurementStarted() && RFPowerScanLoader.GetBasicInterface().GetResultCounters().dwCountOfBufferedResults > 0)
                {
                    #region собираем трейс
                    RohdeSchwarz.ViCom.Net.RFPOWERSCAN.SMeasResult pData = RFPowerScanLoader.GetInterface().GetResult(SDefs.dwDefaultTimeOutInMs);

                    IsRuningT = true;
                    LastUpdate = DateTime.Now.Ticks;
                    if (_FreqSet == true && pData.pSpectrumResult.dwCount > 0)
                    {
                        if (pData.pSpectrumResult != null && pData.pSpectrumResult.dwCount > 0)
                        {
                            TracePoints = (int)pData.pSpectrumResult.dwCount;
                            tracepoint[] tr = new tracepoint[pData.pSpectrumResult.dwCount];
                            //MainWindow.tsmx.RFBufer = (int)MainWindow.tsmx.RFPowerScanLoader.GetInterface().GetSettings().ResultBufferDepth.dwValue;

                            //FreqSpan = (decimal)MainWindow.tsmx.RFPowerScanLoader.GetInterface().GetSettings().SweepSettings.dStopFrequencyInHz - (decimal)MainWindow.tsmx.RFPowerScanLoader.GetInterface().GetSettings().SweepSettings.dStartFrequencyInHz;
                            //FreqCentr = ((decimal)MainWindow.tsmx.RFPowerScanLoader.GetInterface().GetSettings().SweepSettings.dStopFrequencyInHz + (decimal)MainWindow.tsmx.RFPowerScanLoader.GetInterface().GetSettings().SweepSettings.dStartFrequencyInHz) / 2;
                            FreqStart = (decimal)MainWindow.tsmx.RFPowerScanLoader.GetInterface().GetSettings().SweepSettings.dStartFrequencyInHz;
                            FreqStop = (decimal)MainWindow.tsmx.RFPowerScanLoader.GetInterface().GetSettings().SweepSettings.dStopFrequencyInHz;
                            int line = (-1 + TracePoints);
                            decimal l_FreqStep = FreqSpan / (line);


                            if (Trace[0].freq != FreqStart || Trace[Trace.Length - 1].freq != FreqStop || Trace.Length != pData.pSpectrumResult.dwCount)
                            {
                                for (int i = 0; i < TracePoints; i++)
                                {
                                    tr[i] = new tracepoint()
                                    {
                                        freq = FreqStart + l_FreqStep * i,
                                        level = pData.pSpectrumResult.pfSpectrumValuesInDBm[i]
                                    };
                                    //tr[i] = p;
                                }
                                Trace = tr; //TracefromDev = tr;
                            }
                            else
                            {
                                for (int i = 0; i < TracePoints; i++)
                                {
                                    Trace[i].level = pData.pSpectrumResult.pfSpectrumValuesInDBm[i];
                                }
                            }
                            _NewTrace = true;
                            //MainWindow.tsmx.RFPowerScanResults++;
                        }
                    }
                    #endregion
                    if (NewTrace == true && IsMeasMon == true)
                    {
                        if (MainWindow.db_v2.MeasMon.Mode == 0)
                        {
                            if (GSMBandMeas == false)
                            {
                                if (FreqSet == true && MeasMonItem != null && MeasMonItem.AllTraceCountToMeas > MeasMonItem.AllTraceCount) // MeasTraceCount > -1)
                                {
                                    #region
                                    //совпадает ли частота и полоса
                                    bool t1 = MeasMonItem.FreqDN == FreqCentr;
                                    bool t2 = MeasMonItem.SpecData.FreqSpan == FreqSpan;
                                    bool t3 = MeasMonItem.ThisIsMaximumSignalAtThisFrequency;
                                    if (t1 && t2 && t3)
                                    {
                                        #region
                                        int dfc = 0;
                                        int ufc = 0;
                                        int cf = 0;
                                        double dfl = 0;
                                        double ufl = 0;
                                        double cfl = 0;
                                        //если GSM то меряем подходит ли спектр или нет
                                        if (MeasMonItem.Techonology == "GSM")
                                        {
                                            dfc = LM.FindMarkerIndOnTrace(Trace, MeasMonItem.FreqDN - MeasMonItem.BWData.BWMeasMax / 2);
                                            ufc = LM.FindMarkerIndOnTrace(Trace, MeasMonItem.FreqDN + MeasMonItem.BWData.BWMeasMax / 2);
                                            cf = LM.FindMarkerIndOnTrace(Trace, MeasMonItem.FreqDN);
                                            dfl = LM.AverageLevelNearPointTrue(Trace, dfc, 10);
                                            ufl = LM.AverageLevelNearPointTrue(Trace, ufc, 10);
                                            cfl = LM.AverageLevelNearPointTrue(Trace, cf, 10);
                                        }

                                        if (MeasMonItem.Techonology != "GSM" || (cfl > dfl + MeasMonItem.BWData.NdBLevel + 5 && cfl > ufl + MeasMonItem.BWData.NdBLevel + 5))
                                        {
                                            bool changeTrace = false;
                                            #region несовпадает имеющийся трейс из сохранений с текущим по частоте и точкам то затираем на новый
                                            if (MeasMonItem.SpecData.Trace == null ||
                                                MeasMonItem.SpecData.Trace[0] == null ||
                                                MeasMonItem.SpecData.TracePoints != Trace.Length ||
                                                MeasMonItem.SpecData.Trace[0].freq != Trace[0].freq ||
                                                MeasMonItem.SpecData.Trace[MeasMonItem.SpecData.Trace.Length - 1].freq != Trace[Trace.Length - 1].freq)//(tt1 || tt2 || tt3 || tt4 || tt5)
                                            {
                                                MeasMonItem.SpecData.Trace = new tracepoint[Trace.Length];
                                                for (int i = 0; i < Trace.Length; i++)
                                                {
                                                    MeasMonItem.SpecData.Trace[i] = new tracepoint() { freq = Trace[i].freq, level = Trace[i].level };
                                                }
                                                MeasMonItem.SpecData.FreqSpan = FreqSpan;
                                                MeasMonItem.SpecData.MeasStart = MainWindow.gps.LocalTime;
                                                MeasMonItem.SpecData.MeasStop = MainWindow.gps.LocalTime;

                                                MeasMonItem.SpecData.LastMeasLatitude = (double)MainWindow.gps.LatitudeDecimal;
                                                MeasMonItem.SpecData.LastMeasLongitude = (double)MainWindow.gps.LongitudeDecimal;
                                                MeasMonItem.SpecData.LastMeasAltitude = (double)MainWindow.gps.Altitude;
                                                MeasMonItem.SpecData.TraceCount = 1;
                                                MeasMonItem.Resets++;
                                                //changeTrace = true;
                                            }
                                            #endregion

                                            #region накапливаем трейс и если есть изменения то changeTrace = true  
                                            else
                                            //if (MeasMonItem.SpecData.Trace[0].freq == Trace[0].freq &&
                                            //    MeasMonItem.SpecData.Trace[MeasMonItem.SpecData.Trace.Length - 1].freq == Trace[Trace.Length - 1].freq)
                                            {
                                                // если чето в накоплении этот трейс поменяет
                                                for (int i = 0; i < Trace.Length; i++)
                                                {
                                                    if (Trace[i].level >= MeasMonItem.SpecData.Trace[i].level)
                                                    { MeasMonItem.SpecData.Trace[i].level = Trace[i].level; changeTrace = true; }
                                                }
                                                if (changeTrace)
                                                {
                                                    MeasMonItem.SpecData.MeasStop = MainWindow.gps.LocalTime;
                                                    MeasMonItem.SpecData.LastMeasLatitude = (double)MainWindow.gps.LatitudeDecimal;
                                                    MeasMonItem.SpecData.LastMeasLongitude = (double)MainWindow.gps.LongitudeDecimal;
                                                    MeasMonItem.SpecData.LastMeasAltitude = (double)MainWindow.gps.Altitude;
                                                }
                                                MeasMonItem.SpecData.TraceCount++;
                                            }
                                            #endregion

                                            #region если uhf то свой велосипед 
                                            //if (MeasMonItem.Techonology == "UHF")
                                            //{
                                            //    for (int i = 0; i < MeasMonItem.Trace.Length; i++)
                                            //    {
                                            //        if (MeasMonItem.Trace[i].Freq > MeasMonItem.FreqDN - MeasMonItem.TraceStep / 2 && MeasMonItem.Trace[i].Freq < MeasMonItem.FreqDN + MeasMonItem.TraceStep / 2)
                                            //        {
                                            //            MeasMonItem.Power = MeasMonItem.Trace[i].Level;
                                            //            for (int y = 0; y < MainWindow.IdfData.UHFBTS.Count; y++)
                                            //            {
                                            //                if (MainWindow.IdfData.UHFBTS[y].PlanFreq_ID == MeasMonItem.PlanFreq_ID && MainWindow.IdfData.UHFBTS[y].Plan_ID == MeasMonItem.PLAN_ID && MainWindow.IdfData.UHFBTS[y].FreqDn == MeasMonItem.FreqDN)
                                            //                { MainWindow.IdfData.UHFBTS[y].Power = MeasMonItem.Power; }
                                            //            }
                                            //        }
                                            //    }
                                            //}
                                            #endregion

                                            #region есть изменения на спектре то ищем пик уровня измерения NdB и меряем заново
                                            if (changeTrace)//&& MeasMonItem.AllTraceCountToMeas - 1 == MeasMonItem.AllTraceCount)
                                            {
                                                int ind = -1;
                                                double tl = double.MinValue;
                                                decimal minf = (MeasMonItem.FreqDN - (MeasMonItem.BWData.BWMarPeak / 2));
                                                decimal maxf = (MeasMonItem.FreqDN + (MeasMonItem.BWData.BWMarPeak / 2));
                                                for (int i = 0; i < MeasMonItem.SpecData.Trace.Length; i++)
                                                {
                                                    if (MeasMonItem.SpecData.Trace[i].freq > minf &&
                                                        MeasMonItem.SpecData.Trace[i].freq < maxf &&
                                                        MeasMonItem.SpecData.Trace[i].level > tl)
                                                    { tl = MeasMonItem.SpecData.Trace[i].level; ind = i; }
                                                }
                                                MeasMonItem.BWData.NdBResult[0] = ind;
                                                int[] mar = new int[3];
                                                if (MeasMonItem.Techonology == "GSM")
                                                {
                                                    ///////////////////////////////////////////////////
                                                    mar = LM.GetMeasNDB(MeasMonItem.SpecData.Trace, MeasMonItem.BWData.NdBResult[0], MeasMonItem.BWData.NdBLevel, MeasMonItem.SpecData.FreqCentr, MeasMonItem.BWData.BWMeasMax, MeasMonItem.BWData.BWMeasMin);//////////////////////
                                                }
                                                else
                                                {
                                                    mar = LM.GetMeasNDB(MeasMonItem.SpecData.Trace, MeasMonItem.BWData.NdBResult[0], MeasMonItem.BWData.NdBLevel, MeasMonItem.SpecData.FreqCentr, MeasMonItem.BWData.BWMeasMax, MeasMonItem.BWData.BWMeasMin);
                                                }
                                                if (mar != null && mar[1] > -1 && mar[2] > -1)
                                                {
                                                    MeasMonItem.BWData.BWMeasured = MeasMonItem.SpecData.Trace[mar[2]].freq - MeasMonItem.SpecData.Trace[mar[1]].freq;
                                                    MeasMonItem.BWData.NdBResult = mar;
                                                    //MeasMonItem.BWData.NdBResult[2] = mar[1];
                                                    MeasMonItem.DeltaFreqMeasured = Math.Round(((Math.Abs(((MeasMonItem.SpecData.Trace[mar[1]].freq + MeasMonItem.SpecData.Trace[mar[2]].freq) / 2) - MeasMonItem.SpecData.FreqCentr)) / (MeasMonItem.SpecData.FreqCentr)) * 1000000, 3);
                                                    //if (Math.Abs(MeasMonItem.Trace[MeasMonItem.MarkerInd].level - MeasMonItem.Trace[mar[0]].level - MeasMonItem.NdBLevel) < 2 && Math.Abs(MeasMonItem.Trace[MeasMonItem.MarkerInd].level - MeasMonItem.Trace[mar[1]].level - MeasMonItem.NdBLevel) < 2) MeasMonItem.Measured = true;
                                                    MeasMonItem.NewDataToSave = true;
                                                    MeasMonItem.NewSpecDataToSave = true;
                                                }
                                                else
                                                {
                                                    MeasMonItem.BWData.NdBResult[1] = -1;
                                                    MeasMonItem.BWData.NdBResult[2] = -1;
                                                    //MeasMonItem.Measured = false;
                                                    MeasMonItem.NewDataToSave = true;
                                                    MeasMonItem.NewSpecDataToSave = true;
                                                }
                                                if (MeasMonItem.IdentificationData is GSMBTSData)
                                                { MeasMonItem.station_sys_info = ((GSMBTSData)MeasMonItem.IdentificationData).GetStationInfo(); }
                                                else if (MeasMonItem.IdentificationData is LTEBTSData)
                                                { MeasMonItem.station_sys_info = ((LTEBTSData)MeasMonItem.IdentificationData).GetStationInfo(); }
                                                else if (MeasMonItem.IdentificationData is UMTSBTSData)
                                                { MeasMonItem.station_sys_info = ((UMTSBTSData)MeasMonItem.IdentificationData).GetStationInfo(); }
                                                else if (MeasMonItem.IdentificationData is CDMABTSData)
                                                { MeasMonItem.station_sys_info = ((CDMABTSData)MeasMonItem.IdentificationData).GetStationInfo(); }

                                                if (MeasMonItem.SpecData.MeasStart == DateTime.MinValue) MeasMonItem.SpecData.MeasStart = MainWindow.gps.LocalTime;
                                            }
                                            #endregion
                                        }
                                        //Даже если текуший спектр нельзя использовать то добавляем счетчик
                                        if (MeasMonItem.AllTraceCount < MeasMonItem.AllTraceCountToMeas)// &&
                                                                                                        //MeasMonItem.SpecData.Trace[0].freq == Trace[0].freq &&
                                                                                                        //MeasMonItem.SpecData.Trace[MeasMonItem.SpecData.Trace.Length - 1].freq == Trace[Trace.Length - 1].freq)
                                        { MeasMonItem.AllTraceCount++; }
                                        #endregion
                                    }
                                    else if (!t1 || !t2 || !t3)
                                    {
                                        if (MeasMonItem.AllTraceCount < MeasMonItem.AllTraceCountToMeas &&
                                            MeasMonItem.SpecData.Trace[0].freq == Trace[0].freq &&
                                            MeasMonItem.SpecData.Trace[MeasMonItem.SpecData.Trace.Length - 1].freq == Trace[Trace.Length - 1].freq)
                                        {
                                            MeasMonItem.AllTraceCount++;
                                        }
                                    }
                                    if (MeasMonItem.AllTraceCountToMeas == MeasMonItem.AllTraceCount)
                                    {
                                        MeasMonItem.SpecData.MeasDuration += new TimeSpan(DateTime.Now.Ticks - MeasMonTimeMeas).TotalSeconds;
                                        MeasMonItem.SpecData.MeasDuration = Math.Round(MeasMonItem.SpecData.MeasDuration, 4);
                                    }
                                    #endregion
                                }
                            }
                            else
                            {
                                #region 
                                if (GSMBandMeasSelected.CountAll > GSMBandMeasSelected.Count)//if (GSMBandMeasSelected.Start == FreqStart && GSMBandMeasSelected.Stop == FreqStop)
                                {
                                    long Time = DateTime.Now.Ticks;
                                    for (int i = 0; i < GSMBandMeasSelected.Trace.Length; i++)
                                    {
                                        GSMBandMeasSelected.Trace[i].level = Math.Round(LM.MeasChannelPower(Trace, GSMBandMeasSelected.Trace[i].freq, 200000.0m), 2);
                                    }
                                    GSMBandMeasSelected.Time = new TimeSpan(DateTime.Now.Ticks - Time);
                                    GSMBandMeasSelected.MeasTime = MainWindow.gps.LocalTime;
                                    GSMBandMeasSelected.latitude = (double)MainWindow.gps.LatitudeDecimal;
                                    GSMBandMeasSelected.longitude = (double)MainWindow.gps.LongitudeDecimal;
                                    GSMBandMeasSelected.altitude = (double)MainWindow.gps.Altitude;
                                    GSMBandMeasSelected.saved = false;
                                    GSMBandMeasSelected.device_meas = device_meas;
                                    GSMBandMeasSelected.device_ident = device_meas;
                                    GSMBandMeasSelected.Count++;
                                }
                                if (GSMBandMeasSelected.id == 0 && GSMBandMeasSelected.Count >= GSMBandMeasSelected.CountAll)
                                {
                                    GSMBandMeas = false; GSMBandMeasTicks = MainWindow.gps.LocalTime.Ticks;
                                }
                                #endregion
                            }
                        }
                        else if (MainWindow.db_v2.MeasMon.Mode == 1)
                        {
                            #region
                            if (FreqSet == true && MeasMonBandItem != null && MeasMonBandItem.AllTraceCountToMeas > MeasMonBandItem.AllTraceCount)
                            {
                                for (int i = 0; i < MainWindow.db_v2.MeasMon.Data.Count(); i++)
                                {
                                    DB.MeasData mmd = MainWindow.db_v2.MeasMon.Data[i];
                                    //первично попадает в этот диапазон
                                    if (mmd.ThisIsMaximumSignalAtThisFrequency &&
                                        mmd.FreqDN > MeasMonBandItem.Start && mmd.FreqDN < MeasMonBandItem.Stop)
                                    {
                                        int trc = 0;
                                        int trstart = 0, trstop = 0;
                                        for (int tr = 0; tr < Trace.Length; tr++)
                                        {
                                            if (Trace[tr].freq >= mmd.SpecData.FreqCentr - mmd.SpecData.FreqSpan / 2 && Trace[tr].freq <= mmd.SpecData.FreqCentr + mmd.SpecData.FreqSpan / 2)
                                            {
                                                if (trstart == 0) trstart = tr;
                                                trstop = tr;
                                                trc++;
                                            }
                                        }
                                        tracepoint[] mmtrace = new tracepoint[trc];
                                        Array.Copy(Trace, trstart, mmtrace, 0, trc);
                                        if (mmtrace.Length > MainWindow.db_v2.MeasMon.Data[i].SpecData.Trace.Length)
                                        {

                                            try
                                            {
                                                mmtrace = LM.ChangeFreqGrid_v2(
                                                    ref mmtrace,
                                                    MainWindow.db_v2.MeasMon.Data[i].SpecData.FreqStart,
                                                    mmtrace[2].freq - mmtrace[1].freq,
                                                    MainWindow.db_v2.MeasMon.Data[i].SpecData.FreqStart,
                                                    MainWindow.db_v2.MeasMon.Data[i].SpecData.FreqSpan / (MainWindow.db_v2.MeasMon.Data[i].SpecData.Trace.Length - 1), MainWindow.db_v2.MeasMon.Data[i].SpecData.Trace.Length, -158);

                                            }
                                            catch { }
                                        }

                                        //////////////////////////
                                        int dfc = 0;
                                        int ufc = 0;
                                        int cf = 0;
                                        double dfl = 0;
                                        double ufl = 0;
                                        double cfl = 0;
                                        //если GSM то меряем подходит ли спектр или нет
                                        if (mmd.Techonology == "GSM")
                                        {
                                            dfc = LM.FindMarkerIndOnTrace(mmtrace, mmd.SpecData.FreqCentr - mmd.BWData.BWMeasMax / 2);
                                            ufc = LM.FindMarkerIndOnTrace(mmtrace, mmd.SpecData.FreqCentr + mmd.BWData.BWMeasMax / 2);
                                            cf = LM.FindMarkerIndOnTrace(mmtrace, mmd.SpecData.FreqCentr);
                                            dfl = LM.AverageLevelNearPoint(mmtrace, dfc, 10);
                                            ufl = LM.AverageLevelNearPoint(mmtrace, ufc, 10);
                                            cfl = LM.AverageLevelNearPoint(mmtrace, cf, 10);
                                        }

                                        #region несовпадает имеющийся трейс из сохранений с текущим по частоте и точкам то затираем на новый
                                        if (mmd.SpecData.Trace == null ||
                                            mmd.SpecData.Trace[0] == null ||
                                            mmd.SpecData.Trace.Length != mmtrace.Length ||
                                            mmd.SpecData.Trace[0].freq != mmtrace[0].freq ||
                                            mmd.SpecData.Trace[mmd.SpecData.Trace.Length - 1].freq != mmtrace[mmtrace.Length - 1].freq)//(tt1 || tt2 || tt3 || tt4 || tt5)
                                        {
                                            mmd.SpecData.Trace = new tracepoint[mmtrace.Length];
                                            for (int tr = 0; tr < mmtrace.Length; tr++)
                                            {
                                                tracepoint p = new tracepoint() { freq = mmtrace[tr].freq, level = -1000 /*mmtrace[tr].level*/ };
                                                mmd.SpecData.Trace[tr] = p;
                                            }
                                            mmd.SpecData.MeasStart = MainWindow.gps.LocalTime;
                                            mmd.SpecData.MeasStop = MainWindow.gps.LocalTime;
                                            mmd.SpecData.LastMeasLatitude = (double)MainWindow.gps.LatitudeDecimal;
                                            mmd.SpecData.LastMeasLongitude = (double)MainWindow.gps.LongitudeDecimal;
                                            mmd.SpecData.LastMeasAltitude = (double)MainWindow.gps.Altitude;
                                            mmd.SpecData.TraceCount++;
                                            mmd.Resets++;
                                        }
                                        #endregion
                                        if (mmd.Techonology != "GSM" || (cfl > dfl + mmd.BWData.NdBLevel + 5 && cfl > ufl + mmd.BWData.NdBLevel + 5))
                                        {
                                            bool changeTrace = false;
                                            #region накапливаем трейс и если есть изменения то changeTrace = true                                   
                                            if (mmd.SpecData.Trace[0].freq == mmtrace[0].freq && mmd.SpecData.Trace[mmd.SpecData.Trace.Length - 1].freq == mmtrace[mmtrace.Length - 1].freq)
                                            {
                                                // если чето в накоплении этот трейс поменяет
                                                for (int tr = 0; tr < mmtrace.Length; tr++)
                                                {
                                                    if (mmtrace[tr].level >= mmd.SpecData.Trace[tr].level)
                                                    { mmd.SpecData.Trace[tr].level = mmtrace[tr].level; changeTrace = true; }
                                                }
                                                if (changeTrace)
                                                {
                                                    mmd.SpecData.MeasStop = MainWindow.gps.LocalTime;
                                                    mmd.SpecData.LastMeasLatitude = (double)MainWindow.gps.LatitudeDecimal;
                                                    mmd.SpecData.LastMeasLongitude = (double)MainWindow.gps.LongitudeDecimal;
                                                    mmd.SpecData.LastMeasAltitude = (double)MainWindow.gps.Altitude;
                                                }
                                                mmd.SpecData.TraceCount++;
                                            }
                                            #endregion

                                            #region есть изменения на спектре то ищем пик уровня измерения NdB и меряем заново
                                            if (changeTrace)//&& mmd.AllTraceCountToMeas - 1 == mmd.AllTraceCount)
                                            {
                                                int ind = -1;
                                                double tl = double.MinValue;
                                                decimal minf = (mmd.SpecData.FreqCentr - (mmd.BWData.BWMarPeak / 2));
                                                decimal maxf = (mmd.SpecData.FreqCentr + (mmd.BWData.BWMarPeak / 2));
                                                for (int j = 0; j < mmd.SpecData.Trace.Length; j++)
                                                {
                                                    if (mmd.SpecData.Trace[j].freq > minf && mmd.SpecData.Trace[j].freq < maxf && mmd.SpecData.Trace[j].level > tl)
                                                    { tl = mmd.SpecData.Trace[j].level; ind = j; }
                                                    //if (mmd.Techonology == "UHF" && mmd.Trace[i].Freq > mmd.FreqDN  - mmd.TraceStep / 2 && mmd.Trace[i].Freq < mmd.FreqDN  + mmd.TraceStep / 2)
                                                    //{
                                                    //    mmd.Power = mmd.Trace[i].Level;
                                                    //    for (int y = 0; y < MainWindow.IdfData.UHFBTS.Count; y++)
                                                    //    {
                                                    //        if (MainWindow.IdfData.UHFBTS[y].PlanFreq_ID == mmd.PlanFreq_ID && MainWindow.IdfData.UHFBTS[y].Plan_ID == mmd.PLAN_ID && MainWindow.IdfData.UHFBTS[y].FreqDn == mmd.FreqDN)
                                                    //        { MainWindow.IdfData.UHFBTS[y].Power = mmd.Power; }
                                                    //    }
                                                    //}
                                                }
                                                mmd.BWData.NdBResult[0] = ind;
                                                int[] mar = new int[3];
                                                if (mmd.Techonology == "GSM")
                                                {
                                                    ///////////////////////////////////////////////////
                                                    mar = LM.GetMeasNDB(mmd.SpecData.Trace, mmd.BWData.NdBResult[0], mmd.BWData.NdBLevel, mmd.SpecData.FreqCentr, mmd.BWData.BWMeasMax, mmd.BWData.BWMeasMin);//////////////////////
                                                }
                                                else
                                                {
                                                    mar = LM.GetMeasNDB(mmd.SpecData.Trace, mmd.BWData.NdBResult[0], mmd.BWData.NdBLevel, mmd.SpecData.FreqCentr, mmd.BWData.BWMeasMax, mmd.BWData.BWMeasMin);
                                                }
                                                if (mar != null && mar[1] > -1 && mar[2] > -1)
                                                {
                                                    mmd.BWData.BWMeasured = (decimal)(mmd.SpecData.Trace[mar[2]].freq - mmd.SpecData.Trace[mar[1]].freq);
                                                    mmd.BWData.NdBResult = mar;
                                                    //mmd.BWData.NdBResult[2] = mar[1];
                                                    mmd.DeltaFreqMeasured = Math.Round(((Math.Abs(((mmd.SpecData.Trace[mar[1]].freq + mmd.SpecData.Trace[mar[2]].freq) / 2) - mmd.SpecData.FreqCentr)) / (mmd.SpecData.FreqCentr)) * 1000000, 3);
                                                    //if (Math.Abs(mmd.Trace[mmd.MarkerInd].level - mmd.Trace[mar[1]].level - mmd.NdBLevel) < 2 && Math.Abs(mmd.Trace[mmd.MarkerInd].level - mmd.Trace[mar[2]].level - mmd.NdBLevel) < 2) mmd.Measured = true;
                                                    mmd.NewDataToSave = true;
                                                }
                                                else
                                                {
                                                    mmd.BWData.NdBResult[1] = -1;
                                                    mmd.BWData.NdBResult[2] = -1;
                                                    //mmd.Measured = false;
                                                    mmd.NewDataToSave = true;
                                                }
                                                if (mmd.IdentificationData is GSMBTSData)
                                                { mmd.station_sys_info = ((GSMBTSData)mmd.IdentificationData).GetStationInfo(); }
                                                else if (mmd.IdentificationData is LTEBTSData)
                                                { mmd.station_sys_info = ((LTEBTSData)mmd.IdentificationData).GetStationInfo(); }
                                                else if (mmd.IdentificationData is UMTSBTSData)
                                                { mmd.station_sys_info = ((UMTSBTSData)mmd.IdentificationData).GetStationInfo(); }
                                                else if (mmd.IdentificationData is CDMABTSData)
                                                { mmd.station_sys_info = ((CDMABTSData)mmd.IdentificationData).GetStationInfo(); }

                                                if (mmd.SpecData.MeasStart == DateTime.MinValue) mmd.SpecData.MeasStart = MainWindow.gps.LocalTime;
                                            }
                                            #endregion
                                        }


                                        //mmd.Trace = mmtrace;
                                        //mmd.TraceCount = trc;
                                        mmd.AllTraceCount++;

                                        mmd.AllTraceCountToMeas++;
                                    }
                                }
                                MeasMonBandItem.AllTraceCount++;
                            }
                            #endregion
                        }
                    }
                    NewTrace = false;
                }
            }
            #region Exception
            catch (RohdeSchwarz.ViCom.Net.CViComError error)
            {
                App.Current.Dispatcher.BeginInvoke((Action)(() =>
                {
                    MainWindow.exp.ExceptionData = new ExData() { ex = new Exception("RfPowerScan Code:\"" + error.ErrorCode + "\" string:\"" + error.ErrorString + "\""), ClassName = "TSMxReceiver", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
                }));
            }
            catch (Exception exp)
            {
                App.Current.Dispatcher.BeginInvoke((Action)(() =>
                {
                    MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "TSMxReceiver", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
                }));
            }
            #endregion
        }
        private void GetMeasMonBands()
        {
            MainWindow.db_v2.MeasMon.Bands.Clear();
            decimal Start = GSMUniFreq.Min();
            decimal Stopt = GSMUniFreq.Max();
            decimal temp = Start;
            for (int i = 1; i < GSMUniFreq.Count; i++)
            {
                if (GSMUniFreq[i] - temp == 200000)
                {
                    temp = GSMUniFreq[i];
                }
                else
                {
                    DB.MeasMonBand b = new DB.MeasMonBand() { };
                    b.Start = Start - 250000;
                    b.Stop = temp + 250000;

                    MainWindow.db_v2.MeasMon.Bands.Add(b);
                    Start = GSMUniFreq[i];
                    temp = GSMUniFreq[i];
                    //i++;
                }
                if (i == GSMUniFreq.Count - 1)
                {
                    DB.MeasMonBand b = new DB.MeasMonBand() { };
                    b.Start = Start - 250000;
                    b.Stop = temp + 250000;

                    MainWindow.db_v2.MeasMon.Bands.Add(b);
                }
            }
            Start = UMTSUniFreq.Min();
            Stopt = UMTSUniFreq.Max();
            temp = Start;
            for (int i = 1; i < UMTSUniFreq.Count; i++)
            {
                if (UMTSUniFreq[i] - temp == 200000)
                {
                    temp = UMTSUniFreq[i];
                }
                else
                {
                    DB.MeasMonBand b = new DB.MeasMonBand() { };
                    b.Start = Start - 250000;
                    b.Stop = temp + 250000;

                    MainWindow.db_v2.MeasMon.Bands.Add(b);
                    Start = UMTSUniFreq[i];
                    temp = UMTSUniFreq[i];
                    //i++;
                }
                if (i == UMTSUniFreq.Count - 1)
                {
                    DB.MeasMonBand b = new DB.MeasMonBand() { };
                    b.Start = Start - 250000;
                    b.Stop = temp + 250000;

                    MainWindow.db_v2.MeasMon.Bands.Add(b);
                }
            }

            if (true) { }
        }
        private void WatchDog(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (Run == true && IsRuning == true && new TimeSpan(DateTime.Now.Ticks - LastUpdate) > new TimeSpan(0, 0, 0, 0, 500))
            {
                IsRuningT = false;
                IsRuning = IsRuningT;
            }
            //else IsRuning = true;
        }
        /// <summary>
        /// ///////////////////////////////////////////////////////////////////////////
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UpdateData(object sender, System.Timers.ElapsedEventArgs e)
        {
            bool methods = false;
            foreach (Delegate d in DM.GetInvocationList())
            {
                if (methods == false)
                { methods = ((DoubMod)d).Method.Name == "UpdateDataInThread"; }
            }
            if (methods == false)
            {
                DM += UpdateDataInThread;
            }
        }

        int TimeAddAfterLevelUpdate = 50;
        private void UpdateDataInThread()
        {
            if (IsRuning && Run == true)
            {
                //try
                //{ }
                //catch (Exception exp)
                //{
                //    MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "TSMxReceiver", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
                //}
                #region GSM                
                if (GSMIsRuning == true && IdentificationData.GSM.BTS.Count() > 0)
                {
                    Settings.MeasMonTech_Set set = App.Sett.MeasMons_Settings.GSM;
                    double _NdbLevel = set.Data[0].NdBLevel;
                    decimal _NdBBWMin = set.Data[0].NdBBWMin;
                    decimal _NdBBWMax = set.Data[0].NdBBWMax;
                    decimal _BWLimit = set.Data[0].BWLimit;
                    decimal _MarPeakBW = set.Data[0].MarPeakBW;

                    decimal _MeasBW = set.Data[0].MeasBW;
                    int _TracePoints = set.Data[0].TracePoints;
                    decimal _RBW = set.Data[0].RBW;
                    decimal _VBW = set.Data[0].VBW;
                    decimal _OBWP = set.Data[0].OBWPercent;
                    decimal _DeltaFreqLimit = set.Data[0].DeltaFreqLimit;

                    for (int i = 0; i < IdentificationData.GSM.BTS.Count; i++)
                    {
                        if (IdentificationData.GSM.BTS[i].FullData)
                        {
                            #region ищем по ATDI
                            if (IdentificationData.GSM.BTS[i].ATDI_Identifier_Find == 0)
                            {
                                if (MainWindow.db_v2.MeasMon.FromTask == true)
                                {
                                    List<DB.localatdi_station> StationsTemp = new List<DB.localatdi_station>() { };
                                    foreach (DB.localatdi_task_with_tech tasktech in MainWindow.db_v2.AtdiTask.data_from_tech)
                                    {
                                        if (tasktech.tech.ToUpper().Contains("GSM"))
                                        {
                                            foreach (DB.localatdi_station tasktechitem in tasktech.TaskItems)
                                            {
                                                bool findcid = false;
                                                #region from db
                                                if (tasktechitem.Callsign_db_S0 == IdentificationData.GSM.BTS[i].MCC &&
                                                    tasktechitem.Callsign_db_S1 == IdentificationData.GSM.BTS[i].MNC &&
                                                    tasktechitem.Callsign_db_S3 == IdentificationData.GSM.BTS[i].CIDToDB)// &&
                                                                                                                         //(tasktechitem.Callsign_db_S2 == 0 || tasktechitem.Callsign_db_S2 == IdentificationData.GSM.BTS[i].LAC))
                                                {
                                                    findcid = true;
                                                    StationsTemp.Add(tasktechitem);
                                                    //IdentificationData.GSM.BTS[i].ATDI_Identifier_Find = 2;
                                                    //IdentificationData.GSM.BTS[i].ATDI_id_station = tasktechitem.id;
                                                    //IdentificationData.GSM.BTS[i].ATDI_id_permission = tasktechitem.license.icsm_id;
                                                    //IdentificationData.GSM.BTS[i].ATDI_GCID = tasktechitem.callsign_db;
                                                    //tasktechitem.IsIdentified = true;
                                                    //foreach (DB.localatdi_station_sector sectoritem in tasktechitem.sectors)
                                                    //{
                                                    //    IdentificationData.GSM.BTS[i].ATDI_id_sector = sectoritem.sector_id;
                                                    //    foreach (DB.localatdi_sector_frequency freq in sectoritem.frequencies)
                                                    //    {
                                                    //        if (IdentificationData.GSM.BTS[i].FreqDn == freq.frequency)
                                                    //        {
                                                    //            IdentificationData.GSM.BTS[i].ATDI_FreqCheck_Find = 2;
                                                    //            IdentificationData.GSM.BTS[i].ATDI_id_frequency = freq.id;
                                                    //            IdentificationData.GSM.BTS[i].ATDI_FrequencyPermission = freq.frequency;
                                                    //        }
                                                    //    }
                                                    //}
                                                }
                                                #endregion
                                                #region from radio
                                                if (!findcid)
                                                    if (tasktechitem.Callsign_radio_S0 == IdentificationData.GSM.BTS[i].MCC &&
                                                        tasktechitem.Callsign_radio_S1 == IdentificationData.GSM.BTS[i].MNC &&
                                                        tasktechitem.Callsign_radio_S2 == IdentificationData.GSM.BTS[i].LAC &&
                                                        tasktechitem.Callsign_radio_S3 == IdentificationData.GSM.BTS[i].CID)
                                                    {
                                                        StationsTemp.Add(tasktechitem);
                                                        //IdentificationData.GSM.BTS[i].ATDI_Identifier_Find = 2;
                                                        //IdentificationData.GSM.BTS[i].ATDI_id_station = tasktechitem.id;
                                                        //IdentificationData.GSM.BTS[i].ATDI_id_permission = tasktechitem.license.icsm_id;
                                                        //IdentificationData.GSM.BTS[i].ATDI_GCID = tasktechitem.callsign_db;
                                                        //tasktechitem.IsIdentified = true;
                                                        //foreach (DB.localatdi_station_sector sectoritem in tasktechitem.sectors)
                                                        //{
                                                        //    IdentificationData.GSM.BTS[i].ATDI_id_sector = sectoritem.sector_id;
                                                        //    foreach (DB.localatdi_sector_frequency freq in sectoritem.frequencies)
                                                        //    {
                                                        //        if (IdentificationData.GSM.BTS[i].FreqDn == freq.frequency)
                                                        //        {
                                                        //            IdentificationData.GSM.BTS[i].ATDI_FreqCheck_Find = 2;
                                                        //            IdentificationData.GSM.BTS[i].ATDI_id_frequency = freq.id;
                                                        //            IdentificationData.GSM.BTS[i].ATDI_FrequencyPermission = freq.frequency;
                                                        //        }
                                                        //    }
                                                        //}
                                                    }
                                                #endregion
                                            }
                                        }
                                    }
                                    IdentificationData.GSM.BTS[i].ATDI_id_task = MainWindow.db_v2.AtdiTask.task_id;
                                    #region Select station
                                    int index = 0;
                                    double indexdist = double.MaxValue;
                                    if (StationsTemp.Count() > 1)
                                    {
                                        for (int st = 0; st < StationsTemp.Count; st++)
                                        {
                                            double dist = 0, ang = 0;
                                            MainWindow.help.calcDistance(
                                                StationsTemp[st].site.location.latitude,
                                                StationsTemp[st].site.location.longitude,
                                                (double)MainWindow.gps.LatitudeDecimal,
                                                (double)MainWindow.gps.LongitudeDecimal,
                                                out dist, out ang);
                                            if (dist < indexdist)
                                            {
                                                index = st;
                                                indexdist = dist;
                                            }
                                        }
                                        //IdentificationData.GSM.BTS[i].GCID += " " + StationsTemp.Count().ToString();
                                    }
                                    if (StationsTemp.Count() > 0)
                                    {
                                        IdentificationData.GSM.BTS[i].ATDI_Identifier_Find = 2;
                                        IdentificationData.GSM.BTS[i].ATDI_id_station = StationsTemp[index].id;
                                        IdentificationData.GSM.BTS[i].ATDI_id_permission = StationsTemp[index].license.icsm_id;
                                        IdentificationData.GSM.BTS[i].ATDI_GCID = StationsTemp[index].callsign_db + " " + StationsTemp.Count().ToString();
                                        StationsTemp[index].IsIdentified = true;

                                        if (IdentificationData.GSM.BTS[i].SectorIDFromIdent - 1 > -1)
                                        {
                                            if (StationsTemp[index].sectors.Count() >= IdentificationData.GSM.BTS[i].SectorIDFromIdent - 1)
                                            {
                                                for (int sec = 0; sec < StationsTemp[index].sectors.Count; sec++)
                                                {
                                                    if (sec == IdentificationData.GSM.BTS[i].SectorIDFromIdent - 1)
                                                    {
                                                        IdentificationData.GSM.BTS[i].SectorID = sec + 1;
                                                        IdentificationData.GSM.BTS[i].ATDI_id_sector = StationsTemp[index].sectors[sec].sector_id;
                                                        foreach (DB.localatdi_sector_frequency freq in StationsTemp[index].sectors[sec].frequencies)
                                                        {
                                                            if (IdentificationData.GSM.BTS[i].FreqDn == freq.frequency)
                                                            {
                                                                IdentificationData.GSM.BTS[i].ATDI_FreqCheck_Find = 2;
                                                                IdentificationData.GSM.BTS[i].ATDI_id_frequency = freq.id;
                                                                IdentificationData.GSM.BTS[i].ATDI_FrequencyPermission = freq.frequency;
                                                            }
                                                        }

                                                    }
                                                }
                                            }
                                            else if (StationsTemp[index].sectors.Count() <= IdentificationData.GSM.BTS[i].SectorIDFromIdent - 1)
                                            {

                                            }
                                        }

                                    }
                                    //bool findcid2 = false;
                                    //#region from db
                                    //if (StationsTemp[index].Callsign_db_S0 == IdentificationData.GSM.BTS[i].MCC &&
                                    //    StationsTemp[index].Callsign_db_S1 == IdentificationData.GSM.BTS[i].MNC &&
                                    //    StationsTemp[index].Callsign_db_S3 == IdentificationData.GSM.BTS[i].CIDToDB)// &&
                                    //                                                                             //(tasktechitem.Callsign_db_S2 == 0 || tasktechitem.Callsign_db_S2 == IdentificationData.GSM.BTS[i].LAC))
                                    //{
                                    //    findcid2 = true;
                                    //    IdentificationData.GSM.BTS[i].ATDI_Identifier_Find = 2;
                                    //    IdentificationData.GSM.BTS[i].ATDI_id_station = StationsTemp[index].id;
                                    //    IdentificationData.GSM.BTS[i].ATDI_id_permission = StationsTemp[index].license.icsm_id;
                                    //    IdentificationData.GSM.BTS[i].ATDI_GCID = StationsTemp[index].callsign_db;
                                    //    StationsTemp[index].IsIdentified = true;
                                    //    foreach (DB.localatdi_station_sector sectoritem in StationsTemp[index].sectors)
                                    //    {
                                    //        IdentificationData.GSM.BTS[i].ATDI_id_sector = sectoritem.sector_id;
                                    //        foreach (DB.localatdi_sector_frequency freq in sectoritem.frequencies)
                                    //        {
                                    //            if (IdentificationData.GSM.BTS[i].FreqDn == freq.frequency)
                                    //            {
                                    //                IdentificationData.GSM.BTS[i].ATDI_FreqCheck_Find = 2;
                                    //                IdentificationData.GSM.BTS[i].ATDI_id_frequency = freq.id;
                                    //                IdentificationData.GSM.BTS[i].ATDI_FrequencyPermission = freq.frequency;
                                    //            }
                                    //        }
                                    //    }
                                    //}
                                    //#endregion
                                    //#region from radio
                                    //if (!findcid2)
                                    //    if (StationsTemp[index].Callsign_radio_S0 == IdentificationData.GSM.BTS[i].MCC &&
                                    //        StationsTemp[index].Callsign_radio_S1 == IdentificationData.GSM.BTS[i].MNC &&
                                    //        StationsTemp[index].Callsign_radio_S2 == IdentificationData.GSM.BTS[i].LAC &&
                                    //        StationsTemp[index].Callsign_radio_S3 == IdentificationData.GSM.BTS[i].CID)
                                    //    {
                                    //        IdentificationData.GSM.BTS[i].ATDI_Identifier_Find = 2;
                                    //        IdentificationData.GSM.BTS[i].ATDI_id_station = StationsTemp[index].id;
                                    //        IdentificationData.GSM.BTS[i].ATDI_id_permission = StationsTemp[index].license.icsm_id;
                                    //        IdentificationData.GSM.BTS[i].ATDI_GCID = StationsTemp[index].callsign_db;
                                    //        StationsTemp[index].IsIdentified = true;
                                    //        foreach (DB.localatdi_station_sector sectoritem in StationsTemp[index].sectors)
                                    //        {
                                    //            IdentificationData.GSM.BTS[i].ATDI_id_sector = sectoritem.sector_id;
                                    //            foreach (DB.localatdi_sector_frequency freq in sectoritem.frequencies)
                                    //            {
                                    //                if (IdentificationData.GSM.BTS[i].FreqDn == freq.frequency)
                                    //                {
                                    //                    IdentificationData.GSM.BTS[i].ATDI_FreqCheck_Find = 2;
                                    //                    IdentificationData.GSM.BTS[i].ATDI_id_frequency = freq.id;
                                    //                    IdentificationData.GSM.BTS[i].ATDI_FrequencyPermission = freq.frequency;
                                    //                }
                                    //            }
                                    //        }
                                    //    }
                                    //#endregion
                                    #endregion
                                }
                                else IdentificationData.GSM.BTS[i].ATDI_id_task = "";

                                if (IdentificationData.GSM.BTS[i].ATDI_Identifier_Find == 0)
                                {
                                    IdentificationData.GSM.BTS[i].ATDI_Identifier_Find = 1;
                                    IdentificationData.GSM.BTS[i].ATDI_id_station = IdentificationData.GSM.BTS[i].CIDToDB.ToString();
                                    IdentificationData.GSM.BTS[i].ATDI_id_permission = 0;
                                    IdentificationData.GSM.BTS[i].ATDI_id_sector = "";
                                }
                                if (IdentificationData.GSM.BTS[i].ATDI_FreqCheck_Find == 0)
                                {
                                    IdentificationData.GSM.BTS[i].ATDI_FreqCheck_Find = 1;
                                    IdentificationData.GSM.BTS[i].ATDI_id_frequency = 0;
                                }
                            }
                            #endregion
                            #region в измерения
                            if (IdentificationData.GSM.BTS[i].ATDI_Identifier_Find != 0 && IdentificationData.GSM.BTS[i].ATDI_FreqCheck_Find != 0)
                            {
                                #region найдено ли в измерениях уже
                                bool FindInMeasData = false;
                                if (MainWindow.db_v2.MeasMon.Data.Count > 0)
                                {
                                    for (int mm = 0; mm < MainWindow.db_v2.MeasMon.Data.Count; mm++)//foreach (DB.MeasData mg in MainWindow.db_v2.MonMeas)
                                    {
                                        //DB.MeasData mg = MainWindow.db_v2.MonMeas[mm];
                                        if (MainWindow.db_v2.MeasMon.Data[mm].Techonology == "GSM" && MainWindow.db_v2.MeasMon.Data[mm].TechSubInd == IdentificationData.GSM.BTS[i].BSIC &&
                                            MainWindow.db_v2.MeasMon.Data[mm].FreqDN == IdentificationData.GSM.BTS[i].FreqDn && MainWindow.db_v2.MeasMon.Data[mm].GCID == IdentificationData.GSM.BTS[i].GCID)
                                        {
                                            MainWindow.db_v2.MeasMon.Data[mm].Power = IdentificationData.GSM.BTS[i].Power;
                                            if (IdentificationData.GSM.BTS[i].level_results.Count > 0 &&
                                                IdentificationData.GSM.BTS[i].level_results[IdentificationData.GSM.BTS[i].level_results.Count - 1].saved_in_result == false)
                                                MainWindow.db_v2.MeasMon.Data[mm].LR_NewDataToSave = true;

                                            MainWindow.db_v2.MeasMon.Data[mm].ThisIsMaximumSignalAtThisFrequency = IdentificationData.GSM.BTS[i].ThisIsMaximumSignalAtThisFrequency;
                                            //MainWindow.db_v2.MeasMon.Data[mm].ATDI_Id_Task = IdentificationData.GSM.BTS[i].ATDI_id_task;
                                            //MainWindow.db_v2.MeasMon.Data[mm].ATDI_Identifier_Find = IdentificationData.GSM.BTS[i].ATDI_Identifier_Find;
                                            //MainWindow.db_v2.MeasMon.Data[mm].ATDI_FreqCheck_Find = IdentificationData.GSM.BTS[i].ATDI_FreqCheck_Find;
                                            //MainWindow.db_v2.MeasMon.Data[mm].ATDI_Id_Station = IdentificationData.GSM.BTS[i].ATDI_id_station;
                                            //MainWindow.db_v2.MeasMon.Data[mm].ATDI_Id_Permission = IdentificationData.GSM.BTS[i].ATDI_id_permission;
                                            //MainWindow.db_v2.MeasMon.Data[mm].ATDI_Id_Sector = IdentificationData.GSM.BTS[i].ATDI_id_sector;
                                            //MainWindow.db_v2.MeasMon.Data[mm].ATDI_Id_Frequency = IdentificationData.GSM.BTS[i].ATDI_id_frequency;
                                            //MainWindow.db_v2.MeasMon.Data[mm].ATDI_FrequencyPermission = IdentificationData.GSM.BTS[i].ATDI_FrequencyPermission;
                                            //MainWindow.db_v2.MeasMon.Data[mm].ATDI_GCID = IdentificationData.GSM.BTS[i].ATDI_GCID;
                                            MainWindow.db_v2.MeasMon.Data[mm].station_sys_info.information_blocks = IdentificationData.GSM.BTS[i].station_sys_info.information_blocks;
                                            MainWindow.db_v2.MeasMon.Data[mm].DeleteFromMeasMon = IdentificationData.GSM.BTS[i].DeleteFromMeasMon;

                                            FindInMeasData = true;
                                            if (IdentificationData.GSM.BTS[i].Power > DetectionLevelGSM)
                                                MainWindow.db_v2.MeasMon.Data[mm].LastSeenSignal = IdentificationData.GSM.BTS[i].LastLevelUpdete;//заменить на время из GPS с учетом часового пояса
                                            MainWindow.db_v2.MeasMon.Data[mm].LastDetectionLevelUpdete = IdentificationData.GSM.BTS[i].LastDetectionLevelUpdete;
                                        }
                                    }
                                }
                                #endregion
                                #region добавляем новое
                                if (FindInMeasData == false &&
                                    IdentificationData.GSM.BTS[i].ThisIsMaximumSignalAtThisFrequency == true &&
                                    IdentificationData.GSM.BTS[i].Power > DetectionLevelGSM &&
                                    new TimeSpan(MainWindow.gps.LocalTime.Ticks - IdentificationData.GSM.BTS[i].LastDetectionLevelUpdete) < new TimeSpan(0, 0, 0, 0, TimeAddAfterLevelUpdate))
                                {
                                    bool FindInResulData = false;//ищем в результатах
                                    int _LevelUnit = 0;
                                    bool _MeasCorrectness = false;
                                    decimal _DeltaFreqMeasured = 0;
                                    decimal _ChannelStrenght = -1000;
                                    #region Spec
                                    spectrum_data sd = new spectrum_data()
                                    {
                                        FreqCentr = IdentificationData.GSM.BTS[i].FreqDn,
                                        FreqSpan = _MeasBW,
                                        FreqStart = IdentificationData.GSM.BTS[i].FreqDn - _MeasBW / 2,
                                        FreqStop = IdentificationData.GSM.BTS[i].FreqDn + _MeasBW / 2,
                                        LastMeasAltitude = 0,
                                        LastMeasLatitude = 0,
                                        LastMeasLongitude = 0,
                                        MeasDuration = 0,
                                        MeasStart = DateTime.MinValue,
                                        MeasStop = DateTime.MinValue,
                                        PreAmp = 0,
                                        RBW = _RBW,
                                        VBW = _VBW,
                                        TraceCount = 0,
                                        ATT = 0,
                                        RefLevel = -30,
                                        TracePoints = _TracePoints
                                    };
                                    #endregion Spec
                                    #region BW
                                    bandwidth_data bd = new bandwidth_data()
                                    {
                                        BWLimit = _BWLimit,
                                        BWMarPeak = _MarPeakBW,
                                        BWMeasMax = _NdBBWMax,
                                        BWMeasMin = _NdBBWMin,
                                        BWMeasured = -1,
                                        NdBLevel = _NdbLevel,
                                        NdBResult = new int[3] { -1, -1, -1 },
                                        OBWPercent = _OBWP,
                                        OBWResult = new int[3] { -1, -1, -1 },
                                        BWIdentification = _NdBBWMin,
                                    };
                                    #endregion BW
                                    #region CP
                                    channelpower_data[] cd = new channelpower_data[]
                                    {
                                        new channelpower_data()
                                        {
                                            FreqCentr = IdentificationData.GSM.BTS[i].FreqDn,
                                            ChannelPowerBW = bd.BWMeasured,
                                            ChannelPowerResult = -1000
                                        }
                                    };
                                    #endregion CP
                                    #region из настроек таска
                                    if (MainWindow.db_v2.MeasMon.FromTask == true)
                                    {
                                        #region инфа из ATDI 
                                        foreach (DB.localatdi_task_with_tech restech in MainWindow.db_v2.AtdiTask.data_from_tech)
                                        {
                                            if (restech.tech.Contains("GSM") && restech.scan_parameters.Length > 0)
                                            {
                                                _DeltaFreqLimit = restech.scan_parameters[0].max_frequency_relative_offset_mk;
                                                bd.BWLimit = restech.scan_parameters[0].max_permission_bw;
                                                bd.NdBLevel = (double)restech.scan_parameters[0].xdb_level_db;
                                                sd.FreqSpan = restech.scan_parameters[0].meas_span;
                                                sd.FreqStart = IdentificationData.GSM.BTS[i].FreqDn - sd.FreqSpan / 2;
                                                sd.FreqStop = IdentificationData.GSM.BTS[i].FreqDn + sd.FreqSpan / 2;
                                                sd.PreAmp = restech.scan_parameters[0].preamplification_db;
                                                sd.RBW = restech.scan_parameters[0].rbw;
                                                sd.VBW = restech.scan_parameters[0].vbw;
                                                sd.RefLevel = (double)restech.scan_parameters[0].ref_level_dbm;
                                                sd.ATT = (int)restech.scan_parameters[0].rf_attenuation_db;

                                                //foreach (DB.localatdi_standard_scan_parameter param in restech.scan_parameters)
                                                //{

                                                //}
                                            }
                                        }
                                        #endregion
                                    }
                                    #endregion
                                    sd.Trace = new tracepoint[sd.TracePoints];
                                    decimal tracestep = sd.FreqSpan / (sd.TracePoints - 1);
                                    //decimal fstart = IdentificationData.GSM.BTS[i].FreqDn - _MeasBW / 2;
                                    for (int y = 0; y < sd.TracePoints; y++)
                                    {
                                        sd.Trace[y] = new tracepoint() { freq = sd.FreqStart + tracestep * y, level = -1000 };
                                    }

                                    DB.localatdi_station_sys_info station_sys_info = new DB.localatdi_station_sys_info() { };

                                    bool FindInTaskResults = false;
                                    if (MainWindow.db_v2.MeasMon.FromTask == true)
                                    {
                                        #region инфа из результатов ATDI 
                                        foreach (DB.localatdi_task_with_tech restech in MainWindow.db_v2.AtdiTask.data_from_tech)
                                        {
                                            if (restech.tech.Contains("GSM") || restech.tech.ToLower().Contains("unknown"))
                                            {
                                                foreach (DB.localatdi_result_item resi in restech.ResultItems)
                                                {
                                                    if (resi.id_station == IdentificationData.GSM.BTS[i].ATDI_id_station &&
                                                    resi.id_sector == IdentificationData.GSM.BTS[i].ATDI_id_sector &&
                                                    resi.spec_data.FreqCentr == IdentificationData.GSM.BTS[i].FreqDn &&
                                                    resi.station_identifier_from_radio_tech_sub_ind == IdentificationData.GSM.BTS[i].BSIC &&
                                                    resi.station_identifier_from_radio == IdentificationData.GSM.BTS[i].GCID)
                                                    {
                                                        if (sd.MeasStart < resi.spec_data.MeasStart || sd.MeasStart == DateTime.MinValue)
                                                        {
                                                            FindInTaskResults = true;
                                                            FindInResulData = true;
                                                            sd = resi.spec_data;
                                                            bd = resi.bw_data;
                                                            cd = resi.cp_data;
                                                            station_sys_info = resi.station_sys_info;
                                                            _ChannelStrenght = resi.meas_strength;
                                                            _MeasCorrectness = resi.meas_correctness;
                                                            if (resi.freq_centr_perm != 0 && bd.NdBResult[1] > -1 && bd.NdBResult[2] > -1)
                                                            { _DeltaFreqMeasured = Math.Round(((Math.Abs(((sd.Trace[bd.NdBResult[1]].freq + sd.Trace[bd.NdBResult[2]].freq) / 2) - (resi.freq_centr_perm))) / (resi.freq_centr_perm / 1000000)), 3); }
                                                            else if (resi.freq_centr_perm == 0 && bd.NdBResult[1] > -1 && bd.NdBResult[2] > -1)
                                                            { _DeltaFreqMeasured = Math.Round(((Math.Abs(((sd.Trace[bd.NdBResult[1]].freq + sd.Trace[bd.NdBResult[2]].freq) / 2) - (sd.FreqCentr))) / (sd.FreqCentr / 1000000)), 3); }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        #endregion
                                    }
                                    if (MainWindow.db_v2.MeasMon.FromTask == false)
                                    {
                                        #region инфа из результатов неизвестных
                                        foreach (DB.localatdi_unknown_result_with_tech restech in MainWindow.db_v2.AtdiUnknownResult.data_from_tech)
                                        {
                                            if (restech.tech.Contains("GSM"))
                                            {
                                                foreach (DB.localatdi_result_item resi in restech.ResultItems)
                                                {
                                                    if (resi.spec_data.FreqCentr == IdentificationData.GSM.BTS[i].FreqDn &&
                                                        resi.station_identifier_from_radio_s0 == IdentificationData.GSM.BTS[i].MCC &&
                                                        resi.station_identifier_from_radio_s1 == IdentificationData.GSM.BTS[i].MNC &&
                                                        resi.station_identifier_from_radio_s2 == IdentificationData.GSM.BTS[i].LAC &&
                                                        resi.station_identifier_from_radio_s3 == IdentificationData.GSM.BTS[i].CID &&
                                                        resi.station_identifier_from_radio_tech_sub_ind == IdentificationData.GSM.BTS[i].BSIC)
                                                    {
                                                        if (sd.MeasStart < resi.spec_data.MeasStart || sd.MeasStart == DateTime.MinValue)
                                                        {
                                                            FindInTaskResults = true;
                                                            FindInResulData = true;
                                                            sd = resi.spec_data;
                                                            bd = resi.bw_data;
                                                            cd = resi.cp_data;
                                                            station_sys_info = resi.station_sys_info;
                                                            _ChannelStrenght = resi.meas_strength;
                                                            _MeasCorrectness = resi.meas_correctness;
                                                            if (resi.freq_centr_perm != 0 && bd.NdBResult[1] > -1 && bd.NdBResult[2] > -1)
                                                            { _DeltaFreqMeasured = Math.Round(((Math.Abs(((sd.Trace[bd.NdBResult[1]].freq + sd.Trace[bd.NdBResult[2]].freq) / 2) - (resi.freq_centr_perm))) / (resi.freq_centr_perm / 1000000)), 3); }
                                                            else if (resi.freq_centr_perm == 0 && bd.NdBResult[1] > -1 && bd.NdBResult[2] > -1)
                                                            { _DeltaFreqMeasured = Math.Round(((Math.Abs(((sd.Trace[bd.NdBResult[1]].freq + sd.Trace[bd.NdBResult[2]].freq) / 2) - (sd.FreqCentr))) / (sd.FreqCentr / 1000000)), 3); }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        #endregion
                                    }

                                    DB.MeasData result = new DB.MeasData()
                                    {
                                        #region
                                        Techonology = "GSM",
                                        ATDI_Id_Task = IdentificationData.GSM.BTS[i].ATDI_id_task,
                                        ATDI_Id_Station = IdentificationData.GSM.BTS[i].ATDI_id_station,
                                        ATDI_Id_Permission = IdentificationData.GSM.BTS[i].ATDI_id_permission,
                                        ATDI_Id_Sector = IdentificationData.GSM.BTS[i].ATDI_id_sector,
                                        ATDI_Id_Frequency = IdentificationData.GSM.BTS[i].ATDI_id_frequency,
                                        ATDI_FrequencyPermission = IdentificationData.GSM.BTS[i].ATDI_FrequencyPermission,
                                        ATDI_Identifier_Find = IdentificationData.GSM.BTS[i].ATDI_Identifier_Find,
                                        ATDI_FreqCheck_Find = IdentificationData.GSM.BTS[i].ATDI_FreqCheck_Find,
                                        ATDI_GCID = IdentificationData.GSM.BTS[i].ATDI_GCID,

                                        SpecData = sd,
                                        NewSpecDataToSave = false,
                                        AllTraceCount = sd.TraceCount,
                                        AllTraceCountToMeas = sd.TraceCount,
                                        station_sys_info = station_sys_info,
                                        BWData = bd,

                                        FreqUP = IdentificationData.GSM.BTS[i].FreqUp,
                                        FreqDN = IdentificationData.GSM.BTS[i].FreqDn,
                                        ChannelN = IdentificationData.GSM.BTS[i].ARFCN,
                                        StandartSubband = IdentificationData.GSM.BTS[i].StandartSubband,
                                        GCID = IdentificationData.GSM.BTS[i].GCID,
                                        TechSubInd = IdentificationData.GSM.BTS[i].BSIC,
                                        FullData = IdentificationData.GSM.BTS[i].FullData,
                                        Power = IdentificationData.GSM.BTS[i].Power,
                                        LevelResults = IdentificationData.GSM.BTS[i].level_results,//lmc,
                                        LR_NewDataToSave = IdentificationData.GSM.BTS[i].LR_NewDataToSave,
                                        LastSeenSignal = IdentificationData.GSM.BTS[i].LastLevelUpdete,
                                        LastDetectionLevelUpdete = IdentificationData.GSM.BTS[i].LastDetectionLevelUpdete,
                                        IdentificationData = IdentificationData.GSM.BTS[i],

                                        DeltaFreqLimit = _DeltaFreqLimit,
                                        DeltaFreqMeasured = _DeltaFreqMeasured,
                                        DeltaFreqConclusion = 0,
                                        ChannelStrenght = _ChannelStrenght,
                                        CPData = cd,
                                        difference_time_stamp_ns = 0,

                                        MeasCorrectness = _MeasCorrectness,
                                        MeasMask = new DB.localatdi_elements_mask[] { },
                                        NewDataToSave = false,
                                        Resets = 0,
                                        ThisToMeas = false,
                                        LevelUnit = _LevelUnit,

                                        ThisIsMaximumSignalAtThisFrequency = IdentificationData.GSM.BTS[i].ThisIsMaximumSignalAtThisFrequency,
                                        device_ident = device_ident,
                                        device_meas = new DB.localatdi_meas_device() { },
                                        #endregion
                                    };
                                    if (App.Sett.MeasMons_Settings.SpectrumMeasDeviece == App.Sett.Equipments_Settings.RuSTSMx.ID)
                                        result.device_meas = device_meas;

                                    GUIThreadDispatcher.Instance.Invoke(() =>
                                    {
                                        MainWindow.db_v2.MeasMon.Data.Add(result);
                                    });
                                }
                                #endregion
                            }
                            #endregion
                        }
                    }
                    #region пилим количевство всякого по базам и т.д.
                    int BTSCountWithGCID = 0;

                    int ATDI_BTSCountWithGCID = 0;
                    int ATDI_BTSCountNDP = 0;
                    int ATDI_BTSCountNPE = 0;
                    for (int i = 0; i < IdentificationData.GSM.BTS.Count; i++)
                    {
                        if (IdentificationData.GSM.BTS[i].FullData == true && IdentificationData.GSM.BTS[i].ATDI_Identifier_Find == 1) ATDI_BTSCountNDP++;
                        if (IdentificationData.GSM.BTS[i].FullData == true && IdentificationData.GSM.BTS[i].ATDI_Identifier_Find == 2 && IdentificationData.GSM.BTS[i].ATDI_FreqCheck_Find == 1) ATDI_BTSCountNPE++;
                        if (IdentificationData.GSM.BTS[i].FullData == true && IdentificationData.GSM.BTS[i].ATDI_Identifier_Find == 2) ATDI_BTSCountWithGCID++;

                        //всего с идентификатором
                        if (IdentificationData.GSM.BTS[i].FullData == true) BTSCountWithGCID++;
                    }
                    GUIThreadDispatcher.Instance.Invoke(() =>
                    {
                        IdentificationData.GSM.ATDI_BTSCountWithGCID = ATDI_BTSCountWithGCID;
                        IdentificationData.GSM.ATDI_BTSCountNDP = ATDI_BTSCountNDP;
                        IdentificationData.GSM.ATDI_BTSCountNPE = ATDI_BTSCountNPE;

                        IdentificationData.GSM.BTSCount = IdentificationData.GSM.BTS.Count;
                        IdentificationData.GSM.BTSCountWithGCID = BTSCountWithGCID;
                    });

                    #endregion
                }

                #endregion
                #region UMTS
                if (UMTSIsRuning == true && IdentificationData.UMTS.BTS.Count() > 0)
                {
                    Settings.MeasMonTech_Set set = App.Sett.MeasMons_Settings.UMTS;
                    double _NdbLevel = set.Data[0].NdBLevel;
                    decimal _NdBBWMin = set.Data[0].NdBBWMin;
                    decimal _NdBBWMax = set.Data[0].NdBBWMax;
                    decimal _BWLimit = set.Data[0].BWLimit;
                    decimal _MarPeakBW = set.Data[0].MarPeakBW;

                    decimal _MeasBW = set.Data[0].MeasBW;
                    int _TracePoints = set.Data[0].TracePoints;
                    decimal _RBW = set.Data[0].RBW;
                    decimal _VBW = set.Data[0].VBW;
                    decimal _OBWP = set.Data[0].OBWPercent;
                    decimal _DeltaFreqLimit = set.Data[0].DeltaFreqLimit;

                    for (int i = 0; i < IdentificationData.UMTS.BTS.Count; i++)
                    {
                        if (IdentificationData.UMTS.BTS[i].FullData)
                        {
                            #region ищем по ATDI
                            if (IdentificationData.UMTS.BTS[i].ATDI_Identifier_Find == 0)
                            {
                                if (MainWindow.db_v2.MeasMon.FromTask == true)
                                {
                                    foreach (DB.localatdi_task_with_tech tasktech in MainWindow.db_v2.AtdiTask.data_from_tech)
                                    {
                                        if (tasktech.tech.ToUpper().Contains("UMTS"))
                                        {
                                            foreach (DB.localatdi_station tasktechitem in tasktech.TaskItems)
                                            {
                                                bool findcid = false;
                                                #region from db
                                                if (tasktechitem.Callsign_db_S0 == IdentificationData.UMTS.BTS[i].MCC &&
                                                    tasktechitem.Callsign_db_S1 == IdentificationData.UMTS.BTS[i].MNC &&
                                                    tasktechitem.Callsign_db_S3 == IdentificationData.UMTS.BTS[i].CIDToDB)// && (tasktechitem.Callsign_LAC == 0 || tasktechitem.Callsign_LAC == IdentificationData.UMTS.BTS[i].LAC))
                                                {
                                                    findcid = true;
                                                    IdentificationData.UMTS.BTS[i].ATDI_Identifier_Find = 2;
                                                    IdentificationData.UMTS.BTS[i].ATDI_id_station = tasktechitem.id;
                                                    IdentificationData.UMTS.BTS[i].ATDI_id_permission = tasktechitem.license.icsm_id;
                                                    IdentificationData.UMTS.BTS[i].ATDI_GCID = tasktechitem.callsign_db;
                                                    tasktechitem.IsIdentified = true;
                                                    foreach (DB.localatdi_station_sector sectoritem in tasktechitem.sectors)
                                                    {
                                                        IdentificationData.UMTS.BTS[i].ATDI_id_sector = sectoritem.sector_id;
                                                        foreach (DB.localatdi_sector_frequency freq in sectoritem.frequencies)
                                                        {
                                                            if (IdentificationData.UMTS.BTS[i].FreqDn == freq.frequency)
                                                            {
                                                                IdentificationData.UMTS.BTS[i].ATDI_FreqCheck_Find = 2;
                                                                IdentificationData.UMTS.BTS[i].ATDI_id_frequency = freq.id;
                                                                IdentificationData.UMTS.BTS[i].ATDI_FrequencyPermission = freq.frequency;
                                                            }
                                                        }
                                                    }
                                                }
                                                #endregion
                                                #region from radio
                                                if (!findcid)
                                                    if (tasktechitem.Callsign_radio_S0 == IdentificationData.UMTS.BTS[i].MCC &&
                                                        tasktechitem.Callsign_radio_S1 == IdentificationData.UMTS.BTS[i].MNC &&
                                                        tasktechitem.Callsign_radio_S2 == IdentificationData.UMTS.BTS[i].LAC &&
                                                        tasktechitem.Callsign_radio_S3 == IdentificationData.UMTS.BTS[i].CID)
                                                    {
                                                        findcid = true;
                                                        IdentificationData.UMTS.BTS[i].ATDI_Identifier_Find = 2;
                                                        IdentificationData.UMTS.BTS[i].ATDI_id_station = tasktechitem.id;
                                                        IdentificationData.UMTS.BTS[i].ATDI_id_permission = tasktechitem.license.icsm_id;
                                                        IdentificationData.UMTS.BTS[i].ATDI_GCID = tasktechitem.callsign_db;
                                                        tasktechitem.IsIdentified = true;
                                                        foreach (DB.localatdi_station_sector sectoritem in tasktechitem.sectors)
                                                        {
                                                            IdentificationData.UMTS.BTS[i].ATDI_id_sector = sectoritem.sector_id;
                                                            foreach (DB.localatdi_sector_frequency freq in sectoritem.frequencies)
                                                            {
                                                                if (IdentificationData.UMTS.BTS[i].FreqDn == freq.frequency)
                                                                {
                                                                    IdentificationData.UMTS.BTS[i].ATDI_FreqCheck_Find = 2;
                                                                    IdentificationData.UMTS.BTS[i].ATDI_id_frequency = freq.id;
                                                                    IdentificationData.UMTS.BTS[i].ATDI_FrequencyPermission = freq.frequency;
                                                                }
                                                            }
                                                        }
                                                    }
                                                #endregion
                                                //if (task.meas_task_id > IdentificationData.UMTS.BTS[i].ATDI_MeasTask_id)

                                            }
                                        }
                                    }
                                    IdentificationData.UMTS.BTS[i].ATDI_id_task = MainWindow.db_v2.AtdiTask.task_id;
                                }
                                else IdentificationData.UMTS.BTS[i].ATDI_id_task = MainWindow.db_v2.AtdiTask.task_id;

                                if (IdentificationData.UMTS.BTS[i].ATDI_Identifier_Find == 0)
                                {
                                    IdentificationData.UMTS.BTS[i].ATDI_Identifier_Find = 1;
                                    IdentificationData.UMTS.BTS[i].ATDI_id_station = IdentificationData.UMTS.BTS[i].CIDToDB.ToString();
                                    IdentificationData.UMTS.BTS[i].ATDI_id_permission = 0;
                                    IdentificationData.UMTS.BTS[i].ATDI_id_sector = "";

                                }
                                if (IdentificationData.UMTS.BTS[i].ATDI_FreqCheck_Find == 0)
                                {
                                    IdentificationData.UMTS.BTS[i].ATDI_FreqCheck_Find = 1;
                                    IdentificationData.UMTS.BTS[i].ATDI_id_frequency = 0;
                                }
                            }
                            #endregion
                            #region в измерения
                            if (IdentificationData.UMTS.BTS[i].ATDI_Identifier_Find != 0 && IdentificationData.UMTS.BTS[i].ATDI_FreqCheck_Find != 0)
                            {
                                #region найдено ли в измерениях уже
                                bool FindInMeasData = false;
                                if (MainWindow.db_v2.MeasMon.Data.Count > 0)
                                {
                                    for (int mm = 0; mm < MainWindow.db_v2.MeasMon.Data.Count; mm++)//foreach (DB.MeasData mg in MainWindow.db_v2.MonMeas)
                                    {
                                        DB.MeasData mg = MainWindow.db_v2.MeasMon.Data[mm];
                                        if (mg.Techonology == "UMTS" &&
                                            mg.TechSubInd == IdentificationData.UMTS.BTS[i].SC &&
                                            mg.FreqDN == IdentificationData.UMTS.BTS[i].FreqDn &&
                                            mg.GCID == IdentificationData.UMTS.BTS[i].GCID)
                                        {
                                            mg.Power = IdentificationData.UMTS.BTS[i].RSCP;
                                            if (IdentificationData.UMTS.BTS[i].level_results.Count > 0 &&
                                                IdentificationData.UMTS.BTS[i].level_results[IdentificationData.UMTS.BTS[i].level_results.Count - 1].saved_in_result == false)
                                                MainWindow.db_v2.MeasMon.Data[mm].LR_NewDataToSave = true;

                                            mg.ThisIsMaximumSignalAtThisFrequency = IdentificationData.UMTS.BTS[i].ThisIsMaximumSignalAtThisFrequency;
                                            //mg.ATDI_Id_Task = IdentificationData.UMTS.BTS[i].ATDI_id_task;
                                            //mg.ATDI_Identifier_Find = IdentificationData.UMTS.BTS[i].ATDI_Identifier_Find;
                                            //mg.ATDI_FreqCheck_Find = IdentificationData.UMTS.BTS[i].ATDI_FreqCheck_Find;
                                            //mg.ATDI_Id_Station = IdentificationData.UMTS.BTS[i].ATDI_id_station;
                                            //mg.ATDI_Id_Permission = IdentificationData.UMTS.BTS[i].ATDI_id_permission;
                                            //mg.ATDI_Id_Sector = IdentificationData.UMTS.BTS[i].ATDI_id_sector;
                                            //mg.ATDI_Id_Frequency = IdentificationData.UMTS.BTS[i].ATDI_id_frequency;
                                            //mg.ATDI_GCID = IdentificationData.UMTS.BTS[i].ATDI_GCID;
                                            //mg.ATDI_FrequencyPermission = IdentificationData.UMTS.BTS[i].ATDI_FrequencyPermission;
                                            mg.station_sys_info.information_blocks = IdentificationData.UMTS.BTS[i].station_sys_info.information_blocks;
                                            mg.DeleteFromMeasMon = IdentificationData.UMTS.BTS[i].DeleteFromMeasMon;

                                            FindInMeasData = true;
                                            if (IdentificationData.UMTS.BTS[i].RSCP > DetectionLevelUMTS)
                                                mg.LastSeenSignal = IdentificationData.UMTS.BTS[i].LastLevelUpdete;//заменить на время из GPS с учетом часового пояса
                                            mg.LastDetectionLevelUpdete = IdentificationData.UMTS.BTS[i].LastDetectionLevelUpdete;

                                        }
                                    }
                                }
                                #endregion
                                #region добавляем новое
                                if (FindInMeasData == false &&
                                    IdentificationData.UMTS.BTS[i].ThisIsMaximumSignalAtThisFrequency == true &&
                                    IdentificationData.UMTS.BTS[i].RSCP > DetectionLevelUMTS &&
                                    new TimeSpan(MainWindow.gps.LocalTime.Ticks - IdentificationData.UMTS.BTS[i].LastDetectionLevelUpdete) < new TimeSpan(0, 0, 0, 0, TimeAddAfterLevelUpdate))
                                {
                                    bool FindInResulData = false;
                                    int _LevelUnit = 0;
                                    bool _MeasCorrectness = false;
                                    decimal _DeltaFreqMeasured = 0;
                                    decimal _ChannelStrenght = -1000;
                                    #region Spec
                                    spectrum_data sd = new spectrum_data()
                                    {
                                        FreqCentr = IdentificationData.UMTS.BTS[i].FreqDn,
                                        FreqSpan = _MeasBW,
                                        FreqStart = IdentificationData.UMTS.BTS[i].FreqDn - _MeasBW / 2,
                                        FreqStop = IdentificationData.UMTS.BTS[i].FreqDn + _MeasBW / 2,
                                        LastMeasAltitude = 0,
                                        LastMeasLatitude = 0,
                                        LastMeasLongitude = 0,
                                        MeasDuration = 0,
                                        MeasStart = DateTime.MinValue,
                                        MeasStop = DateTime.MinValue,
                                        PreAmp = 0,
                                        RBW = _RBW,
                                        VBW = _VBW,
                                        TraceCount = 0,
                                        ATT = 0,
                                        RefLevel = -30,
                                        TracePoints = _TracePoints
                                    };
                                    #endregion Spec
                                    #region BW
                                    bandwidth_data bd = new bandwidth_data()
                                    {
                                        BWLimit = _BWLimit,
                                        BWMarPeak = _MarPeakBW,
                                        BWMeasMax = _NdBBWMax,
                                        BWMeasMin = _NdBBWMin,
                                        BWMeasured = -1,
                                        NdBLevel = _NdbLevel,
                                        NdBResult = new int[3] { -1, -1, -1 },
                                        OBWPercent = _OBWP,
                                        OBWResult = new int[3] { -1, -1, -1 },
                                        BWIdentification = _NdBBWMin,
                                    };
                                    #endregion BW
                                    #region CP
                                    channelpower_data[] cd = new channelpower_data[]
                                    {
                                    new channelpower_data()
                                    {
                                        FreqCentr = IdentificationData.UMTS.BTS[i].FreqDn,
                                        ChannelPowerBW = bd.BWMeasured,
                                        ChannelPowerResult = -1000
                                    }
                                    };
                                    #endregion CP
                                    #region из настроек таска
                                    if (MainWindow.db_v2.MeasMon.FromTask == true)
                                    {
                                        #region инфа из ATDI 
                                        foreach (DB.localatdi_task_with_tech restech in MainWindow.db_v2.AtdiTask.data_from_tech)
                                        {
                                            if (restech.tech.Contains("UMTS") && restech.scan_parameters.Length > 0)
                                            {
                                                _DeltaFreqLimit = restech.scan_parameters[0].max_frequency_relative_offset_mk;
                                                bd.BWLimit = restech.scan_parameters[0].max_permission_bw;
                                                bd.NdBLevel = (double)restech.scan_parameters[0].xdb_level_db;
                                                sd.FreqSpan = restech.scan_parameters[0].meas_span;
                                                sd.FreqStart = sd.FreqCentr - sd.FreqSpan / 2;
                                                sd.FreqStop = sd.FreqCentr + sd.FreqSpan / 2;
                                                sd.PreAmp = restech.scan_parameters[0].preamplification_db;
                                                sd.RBW = restech.scan_parameters[0].rbw;
                                                sd.VBW = restech.scan_parameters[0].vbw;
                                                sd.RefLevel = (double)restech.scan_parameters[0].ref_level_dbm;
                                                sd.ATT = (int)restech.scan_parameters[0].rf_attenuation_db;

                                                //foreach (DB.localatdi_standard_scan_parameter param in restech.scan_parameters)
                                                //{

                                                //}
                                            }
                                        }
                                        #endregion
                                    }
                                    #endregion
                                    sd.Trace = new tracepoint[sd.TracePoints];
                                    decimal tracestep = sd.FreqSpan / (sd.TracePoints - 1);
                                    for (int y = 0; y < sd.TracePoints; y++)
                                    {
                                        sd.Trace[y] = new tracepoint() { freq = sd.FreqStart + tracestep * y, level = -1000 };
                                    }

                                    DB.localatdi_station_sys_info station_sys_info = new DB.localatdi_station_sys_info() { };

                                    bool FindInTaskResults = false;

                                    if (MainWindow.db_v2.MeasMon.FromTask == true)
                                    {
                                        #region инфа из результатов ATDI
                                        foreach (DB.localatdi_task_with_tech restech in MainWindow.db_v2.AtdiTask.data_from_tech)
                                        {
                                            if (restech.tech.Contains("UMTS") || restech.tech.ToLower().Contains("unknown"))
                                            {
                                                foreach (DB.localatdi_result_item resi in restech.ResultItems)
                                                {
                                                    if (resi.id_station == IdentificationData.UMTS.BTS[i].ATDI_id_station &&
                                                    resi.id_sector == IdentificationData.UMTS.BTS[i].ATDI_id_sector &&
                                                    resi.spec_data.FreqCentr == IdentificationData.UMTS.BTS[i].FreqDn &&
                                                    resi.station_identifier_from_radio_tech_sub_ind == IdentificationData.UMTS.BTS[i].SC &&
                                                    resi.station_identifier_from_radio == IdentificationData.UMTS.BTS[i].GCID)
                                                    {
                                                        if (sd.MeasStart < resi.spec_data.MeasStart || sd.MeasStart == DateTime.MinValue)
                                                        {
                                                            FindInTaskResults = true;
                                                            FindInResulData = true;
                                                            sd = resi.spec_data;
                                                            bd = resi.bw_data;
                                                            cd = resi.cp_data;
                                                            station_sys_info = resi.station_sys_info;
                                                            _ChannelStrenght = resi.meas_strength;
                                                            _MeasCorrectness = resi.meas_correctness;

                                                            if (resi.freq_centr_perm != 0 && bd.NdBResult[1] > -1 && bd.NdBResult[2] > -1)
                                                            { _DeltaFreqMeasured = Math.Round(((Math.Abs(((sd.Trace[bd.NdBResult[1]].freq + sd.Trace[bd.NdBResult[2]].freq) / 2) - (resi.freq_centr_perm))) / (resi.freq_centr_perm / 1000000)), 3); }
                                                            else if (resi.freq_centr_perm == 0 && bd.NdBResult[1] > -1 && bd.NdBResult[2] > -1)
                                                            { _DeltaFreqMeasured = Math.Round(((Math.Abs(((sd.Trace[bd.NdBResult[1]].freq + sd.Trace[bd.NdBResult[2]].freq) / 2) - (sd.FreqCentr))) / (sd.FreqCentr / 1000000)), 3); }
                                                        }
                                                    }
                                                }
                                            }
                                        }

                                        #endregion
                                    }
                                    if (MainWindow.db_v2.MeasMon.FromTask == false)
                                    {
                                        #region инфа из результатов неизвестных
                                        foreach (DB.localatdi_unknown_result_with_tech restech in MainWindow.db_v2.AtdiUnknownResult.data_from_tech)
                                        {
                                            if (restech.tech.Contains("UMTS"))
                                            {
                                                foreach (DB.localatdi_result_item resi in restech.ResultItems)
                                                {
                                                    if (resi.spec_data.FreqCentr == IdentificationData.UMTS.BTS[i].FreqDn &&
                                                        resi.station_identifier_from_radio_s0 == IdentificationData.UMTS.BTS[i].MCC &&
                                                        resi.station_identifier_from_radio_s1 == IdentificationData.UMTS.BTS[i].MNC &&
                                                        resi.station_identifier_from_radio_s2 == IdentificationData.UMTS.BTS[i].LAC &&
                                                        resi.station_identifier_from_radio_s3 == IdentificationData.UMTS.BTS[i].CID &&
                                                        resi.station_identifier_from_radio_tech_sub_ind == IdentificationData.UMTS.BTS[i].SC)
                                                    {
                                                        if (sd.MeasStart < resi.spec_data.MeasStart || sd.MeasStart == DateTime.MinValue)
                                                        {
                                                            FindInTaskResults = true;
                                                            FindInResulData = true;
                                                            sd = resi.spec_data;
                                                            bd = resi.bw_data;
                                                            cd = resi.cp_data;
                                                            station_sys_info = resi.station_sys_info;
                                                            _ChannelStrenght = resi.meas_strength;
                                                            _MeasCorrectness = resi.meas_correctness;
                                                            if (resi.freq_centr_perm != 0 && bd.NdBResult[1] > -1 && bd.NdBResult[2] > -1)
                                                            { _DeltaFreqMeasured = Math.Round(((Math.Abs(((sd.Trace[bd.NdBResult[1]].freq + sd.Trace[bd.NdBResult[2]].freq) / 2) - (resi.freq_centr_perm))) / (resi.freq_centr_perm / 1000000)), 3); }
                                                            else if (resi.freq_centr_perm == 0 && bd.NdBResult[1] > -1 && bd.NdBResult[2] > -1)
                                                            { _DeltaFreqMeasured = Math.Round(((Math.Abs(((sd.Trace[bd.NdBResult[1]].freq + sd.Trace[bd.NdBResult[2]].freq) / 2) - (sd.FreqCentr))) / (sd.FreqCentr / 1000000)), 3); }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        #endregion
                                    }
                                    DB.MeasData result = new DB.MeasData()
                                    {
                                        #region
                                        Techonology = "UMTS",
                                        ATDI_Id_Task = IdentificationData.UMTS.BTS[i].ATDI_id_task,
                                        ATDI_Id_Station = IdentificationData.UMTS.BTS[i].ATDI_id_station,
                                        ATDI_Id_Permission = IdentificationData.UMTS.BTS[i].ATDI_id_permission,
                                        ATDI_Id_Sector = IdentificationData.UMTS.BTS[i].ATDI_id_sector,
                                        ATDI_Id_Frequency = IdentificationData.UMTS.BTS[i].ATDI_id_frequency,//Id_Frequency,
                                        ATDI_FrequencyPermission = IdentificationData.UMTS.BTS[i].ATDI_FrequencyPermission,
                                        ATDI_Identifier_Find = IdentificationData.UMTS.BTS[i].ATDI_Identifier_Find,
                                        ATDI_FreqCheck_Find = IdentificationData.UMTS.BTS[i].ATDI_FreqCheck_Find,
                                        ATDI_GCID = IdentificationData.UMTS.BTS[i].ATDI_GCID,

                                        SpecData = sd,
                                        NewSpecDataToSave = false,
                                        AllTraceCount = sd.TraceCount,
                                        AllTraceCountToMeas = sd.TraceCount,
                                        station_sys_info = station_sys_info,
                                        BWData = bd,

                                        FreqUP = IdentificationData.UMTS.BTS[i].FreqUp,
                                        FreqDN = IdentificationData.UMTS.BTS[i].FreqDn,
                                        ChannelN = IdentificationData.UMTS.BTS[i].UARFCN_DN,
                                        StandartSubband = IdentificationData.UMTS.BTS[i].StandartSubband,
                                        GCID = IdentificationData.UMTS.BTS[i].GCID,
                                        TechSubInd = IdentificationData.UMTS.BTS[i].SC,
                                        FullData = IdentificationData.UMTS.BTS[i].FullData,
                                        Power = IdentificationData.UMTS.BTS[i].RSCP,
                                        LevelResults = IdentificationData.UMTS.BTS[i].level_results,//lmc,
                                        LR_NewDataToSave = IdentificationData.UMTS.BTS[i].LR_NewDataToSave,
                                        LastSeenSignal = IdentificationData.UMTS.BTS[i].LastLevelUpdete,
                                        LastDetectionLevelUpdete = IdentificationData.UMTS.BTS[i].LastDetectionLevelUpdete,
                                        ThisIsMaximumSignalAtThisFrequency = IdentificationData.UMTS.BTS[i].ThisIsMaximumSignalAtThisFrequency,

                                        IdentificationData = IdentificationData.UMTS.BTS[i],

                                        DeltaFreqLimit = _DeltaFreqLimit,
                                        DeltaFreqMeasured = _DeltaFreqMeasured,
                                        DeltaFreqConclusion = 0,
                                        ChannelStrenght = _ChannelStrenght,
                                        CPData = cd,
                                        difference_time_stamp_ns = 0,


                                        MeasCorrectness = _MeasCorrectness,
                                        MeasMask = new DB.localatdi_elements_mask[] { },
                                        NewDataToSave = false,
                                        Resets = 0,
                                        ThisToMeas = false,
                                        LevelUnit = _LevelUnit,


                                        device_ident = device_ident,
                                        device_meas = new DB.localatdi_meas_device() { },
                                        #endregion
                                    };
                                    if (App.Sett.MeasMons_Settings.SpectrumMeasDeviece == App.Sett.Equipments_Settings.RuSTSMx.ID)
                                        result.device_meas = device_meas;

                                    App.Current.Dispatcher.Invoke((Action)(() =>
                                    {
                                        MainWindow.db_v2.MeasMon.Data.Add(result);
                                    }));
                                }
                                #endregion
                            }
                            #endregion
                        }
                    }
                    #region пилим количевство всякого по базам и т.д.
                    int BTSCountWithGCID = 0;

                    int ATDI_BTSCountWithGCID = 0;
                    int ATDI_BTSCountNDP = 0;
                    int ATDI_BTSCountNPE = 0;
                    for (int i = 0; i < IdentificationData.UMTS.BTS.Count; i++)
                    //foreach (UMTSBTSData ubd in IdentificationData.UMTS.BTS)
                    {
                        if (IdentificationData.UMTS.BTS[i].FullData == true && IdentificationData.UMTS.BTS[i].ATDI_Identifier_Find == 1) ATDI_BTSCountNDP++;
                        if (IdentificationData.UMTS.BTS[i].FullData == true && IdentificationData.UMTS.BTS[i].ATDI_Identifier_Find == 2 && IdentificationData.UMTS.BTS[i].ATDI_FreqCheck_Find == 1) ATDI_BTSCountNPE++;
                        if (IdentificationData.UMTS.BTS[i].FullData == true && IdentificationData.UMTS.BTS[i].ATDI_Identifier_Find == 2) ATDI_BTSCountWithGCID++;

                        //всего с идентификатором
                        if (IdentificationData.UMTS.BTS[i].FullData == true) BTSCountWithGCID++;
                    }
                    IdentificationData.UMTS.ATDI_BTSCountWithGCID = ATDI_BTSCountWithGCID;
                    IdentificationData.UMTS.ATDI_BTSCountNDP = ATDI_BTSCountNDP;
                    IdentificationData.UMTS.ATDI_BTSCountNPE = ATDI_BTSCountNPE;

                    IdentificationData.UMTS.BTSCount = IdentificationData.UMTS.BTS.Count;
                    IdentificationData.UMTS.BTSCountWithGCID = BTSCountWithGCID;
                    #endregion
                }
                #endregion

                #region LTE
                if (LTEIsRuning == true && IdentificationData.LTE.BTS.Count() > 0)
                {
                    Settings.MeasMonTech_Set set = App.Sett.MeasMons_Settings.LTE;
                    for (int i = 0; i < IdentificationData.LTE.BTS.Count; i++)
                    {
                        if (IdentificationData.LTE.BTS[i].FullData)
                        {
                            #region ищем по ATDI
                            if (IdentificationData.LTE.BTS[i].ATDI_Identifier_Find == 0)
                            {
                                if (MainWindow.db_v2.MeasMon.FromTask == true)
                                {
                                    foreach (DB.localatdi_task_with_tech tasktech in MainWindow.db_v2.AtdiTask.data_from_tech)
                                    {
                                        if (tasktech.tech.ToUpper().Contains("LTE"))
                                        {
                                            foreach (DB.localatdi_station tasktechitem in tasktech.TaskItems)
                                            {
                                                bool findcid = false;
                                                #region from db
                                                if (tasktechitem.Callsign_db_S0 == IdentificationData.LTE.BTS[i].MCC &&
                                                    tasktechitem.Callsign_db_S1 == IdentificationData.LTE.BTS[i].MNC &&
                                                    tasktechitem.Callsign_db_S3 == IdentificationData.LTE.BTS[i].CIDToDB)// &&
                                                                                                                         //(tasktechitem.Callsign_db_S2 == 0 || tasktechitem.Callsign_db_S2 == IdentificationData.LTE.BTS[i].LAC))
                                                {
                                                    findcid = true;
                                                    IdentificationData.LTE.BTS[i].ATDI_Identifier_Find = 2;
                                                    IdentificationData.LTE.BTS[i].ATDI_id_station = tasktechitem.id;
                                                    IdentificationData.LTE.BTS[i].ATDI_id_permission = tasktechitem.license.icsm_id;
                                                    IdentificationData.LTE.BTS[i].ATDI_GCID = tasktechitem.callsign_db;
                                                    tasktechitem.IsIdentified = true;
                                                    foreach (DB.localatdi_station_sector sectoritem in tasktechitem.sectors)
                                                    {
                                                        IdentificationData.LTE.BTS[i].ATDI_id_sector = sectoritem.sector_id;
                                                        IdentificationData.LTE.BTS[i].ATDI_Bandwidth = sectoritem.bw * 1000;
                                                        //кастыль
                                                        //if (sectoritem.bw != 0)
                                                        //    IdentificationData.LTE.BTS[i].ATDI_Bandwidth = sectoritem.bw * 1000;
                                                        //else { IdentificationData.LTE.BTS[i].ATDI_Bandwidth = 5000000; }

                                                        //ищем точное совпадение частоты
                                                        //bool freqfind = false;
                                                        foreach (DB.localatdi_sector_frequency freq in sectoritem.frequencies)
                                                        {
                                                            if (IdentificationData.LTE.BTS[i].FreqDn == freq.frequency)
                                                            {
                                                                IdentificationData.LTE.BTS[i].ATDI_FreqCheck_Find = 2;
                                                                IdentificationData.LTE.BTS[i].ATDI_id_frequency = freq.id;
                                                                IdentificationData.LTE.BTS[i].ATDI_FrequencyPermission = freq.frequency;
                                                                //freqfind = true;
                                                            }
                                                        }
                                                        ////ищем ближайшую снизу частоту
                                                        #region
                                                        //if (freqfind == false)
                                                        //{
                                                        //    List<decimal> ofset = new List<decimal>() { };
                                                        //    foreach (DB.atdi_frequency_for_sector freq in sectoritem.sector_frequencies)
                                                        //    {
                                                        //        ofset.Add(Math.Abs(IdentificationData.LTE.BTS[i].Channel.FreqDn - freq.frequency));
                                                        //    }
                                                        //    int ind = -1;
                                                        //    decimal ofsettemp = decimal.MaxValue;
                                                        //    for (int f = 0; f < ofset.Count; f++)
                                                        //    {
                                                        //        if (ofsettemp == -1 || ofset[f] < ofsettemp  )
                                                        //        { ofsettemp = ofset[f]; ind = f; }
                                                        //    }
                                                        //    IdentificationData.LTE.BTS[i].ATDI_FreqCheck_Find = 2;
                                                        //    IdentificationData.LTE.BTS[i].ATDI_Frequency_id = sectoritem.sector_frequencies[ind].id;
                                                        //    IdentificationData.LTE.BTS[i].ATDI_FrequencyPermission = sectoritem.sector_frequencies[ind].frequency;
                                                        //    if (IdentificationData.LTE.BTS[i].Channel.FreqDn == 2687500000)
                                                        //    {

                                                        //    }
                                                        //}
                                                        #endregion
                                                    }
                                                }
                                                #endregion
                                                #region from redio
                                                if (!findcid)
                                                    if (tasktechitem.Callsign_radio_S0 == IdentificationData.LTE.BTS[i].MCC &&
                                                        tasktechitem.Callsign_radio_S1 == IdentificationData.LTE.BTS[i].MNC &&
                                                        tasktechitem.Callsign_radio_S2 == IdentificationData.LTE.BTS[i].eNodeBId &&
                                                        tasktechitem.Callsign_radio_S3 == IdentificationData.LTE.BTS[i].CID)// && (tasktechitem.Callsign_LAC == 0 || tasktechitem.Callsign_LAC == IdentificationData.UMTS.BTS[i].LAC))
                                                    {
                                                        findcid = true;
                                                        IdentificationData.LTE.BTS[i].ATDI_Identifier_Find = 2;
                                                        IdentificationData.LTE.BTS[i].ATDI_id_station = tasktechitem.id;
                                                        IdentificationData.LTE.BTS[i].ATDI_id_permission = tasktechitem.license.icsm_id;
                                                        IdentificationData.LTE.BTS[i].ATDI_GCID = tasktechitem.callsign_db;
                                                        tasktechitem.IsIdentified = true;
                                                        foreach (DB.localatdi_station_sector sectoritem in tasktechitem.sectors)
                                                        {
                                                            IdentificationData.LTE.BTS[i].ATDI_id_sector = sectoritem.sector_id;
                                                            IdentificationData.LTE.BTS[i].ATDI_Bandwidth = sectoritem.bw * 1000;
                                                            //кастыль
                                                            //if (sectoritem.bw != 0)
                                                            //    IdentificationData.LTE.BTS[i].ATDI_Bandwidth = sectoritem.bw * 1000;
                                                            //else { IdentificationData.LTE.BTS[i].ATDI_Bandwidth = 5000000; }

                                                            //ищем точное совпадение частоты
                                                            //bool freqfind = false;
                                                            foreach (DB.localatdi_sector_frequency freq in sectoritem.frequencies)
                                                            {
                                                                if (IdentificationData.LTE.BTS[i].FreqDn == freq.frequency)
                                                                {
                                                                    IdentificationData.LTE.BTS[i].ATDI_FreqCheck_Find = 2;
                                                                    IdentificationData.LTE.BTS[i].ATDI_id_frequency = freq.id;
                                                                    IdentificationData.LTE.BTS[i].ATDI_FrequencyPermission = freq.frequency;
                                                                    //freqfind = true;
                                                                }
                                                            }
                                                            ////ищем ближайшую снизу частоту
                                                            #region
                                                            //if (freqfind == false)
                                                            //{
                                                            //    List<decimal> ofset = new List<decimal>() { };
                                                            //    foreach (DB.atdi_frequency_for_sector freq in sectoritem.sector_frequencies)
                                                            //    {
                                                            //        ofset.Add(Math.Abs(IdentificationData.LTE.BTS[i].Channel.FreqDn - freq.frequency));
                                                            //    }
                                                            //    int ind = -1;
                                                            //    decimal ofsettemp = decimal.MaxValue;
                                                            //    for (int f = 0; f < ofset.Count; f++)
                                                            //    {
                                                            //        if (ofsettemp == -1 || ofset[f] < ofsettemp  )
                                                            //        { ofsettemp = ofset[f]; ind = f; }
                                                            //    }
                                                            //    IdentificationData.LTE.BTS[i].ATDI_FreqCheck_Find = 2;
                                                            //    IdentificationData.LTE.BTS[i].ATDI_Frequency_id = sectoritem.sector_frequencies[ind].id;
                                                            //    IdentificationData.LTE.BTS[i].ATDI_FrequencyPermission = sectoritem.sector_frequencies[ind].frequency;
                                                            //    if (IdentificationData.LTE.BTS[i].Channel.FreqDn == 2687500000)
                                                            //    {

                                                            //    }
                                                            //}
                                                            #endregion
                                                        }
                                                    }
                                                #endregion
                                                //if (task.meas_task_id > IdentificationData.LTE.BTS[i].ATDI_MeasTask_id)

                                            }
                                        }
                                    }
                                    IdentificationData.LTE.BTS[i].ATDI_id_task = MainWindow.db_v2.AtdiTask.task_id;
                                }
                                else IdentificationData.LTE.BTS[i].ATDI_id_task = "";
                                if (IdentificationData.LTE.BTS[i].ATDI_Identifier_Find == 0)
                                {
                                    IdentificationData.LTE.BTS[i].ATDI_Identifier_Find = 1;
                                    IdentificationData.LTE.BTS[i].ATDI_id_station = IdentificationData.LTE.BTS[i].CIDToDB.ToString();
                                    IdentificationData.LTE.BTS[i].ATDI_id_permission = 0;
                                    IdentificationData.LTE.BTS[i].ATDI_id_sector = "";
                                }
                                if (IdentificationData.LTE.BTS[i].ATDI_FreqCheck_Find == 0)
                                {
                                    IdentificationData.LTE.BTS[i].ATDI_FreqCheck_Find = 1;
                                    IdentificationData.LTE.BTS[i].ATDI_id_frequency = 0;
                                }
                            }
                            #endregion
                            #region в измерения
                            #region найдено ли в измерениях уже
                            bool FindInMeasData = false;
                            if (MainWindow.db_v2.MeasMon.Data.Count > 0)
                            {
                                for (int mm = 0; mm < MainWindow.db_v2.MeasMon.Data.Count; mm++)//foreach (DB.MeasData mg in MainWindow.db_v2.MonMeas)
                                {
                                    DB.MeasData mg = MainWindow.db_v2.MeasMon.Data[mm];
                                    if (mg.Techonology == "LTE" && mg.TechSubInd == IdentificationData.LTE.BTS[i].PCI &&
                                        mg.FreqDN == IdentificationData.LTE.BTS[i].FreqDn && mg.GCID == IdentificationData.LTE.BTS[i].GCID)
                                    {
                                        mg.Power = IdentificationData.LTE.BTS[i].RSRP;
                                        if (IdentificationData.LTE.BTS[i].level_results.Count > 0 &&
                                            IdentificationData.LTE.BTS[i].level_results[IdentificationData.LTE.BTS[i].level_results.Count - 1].saved_in_result == false)
                                            MainWindow.db_v2.MeasMon.Data[mm].LR_NewDataToSave = true;

                                        mg.ThisIsMaximumSignalAtThisFrequency = IdentificationData.LTE.BTS[i].ThisIsMaximumSignalAtThisFrequency;
                                        //mg.ATDI_Id_Task = IdentificationData.LTE.BTS[i].ATDI_id_task;
                                        //mg.ATDI_Identifier_Find = IdentificationData.LTE.BTS[i].ATDI_Identifier_Find;
                                        //mg.ATDI_FreqCheck_Find = IdentificationData.LTE.BTS[i].ATDI_FreqCheck_Find;
                                        //mg.ATDI_Id_Station = IdentificationData.LTE.BTS[i].ATDI_id_station;
                                        //mg.ATDI_Id_Permission = IdentificationData.LTE.BTS[i].ATDI_id_permission;
                                        //mg.ATDI_Id_Sector = IdentificationData.LTE.BTS[i].ATDI_id_sector;
                                        //mg.ATDI_Id_Frequency = IdentificationData.LTE.BTS[i].ATDI_id_frequency;
                                        //mg.ATDI_GCID = IdentificationData.LTE.BTS[i].ATDI_GCID;
                                        //mg.ATDI_FrequencyPermission = IdentificationData.LTE.BTS[i].ATDI_FrequencyPermission;
                                        mg.station_sys_info.information_blocks = IdentificationData.LTE.BTS[i].station_sys_info.information_blocks;
                                        mg.DeleteFromMeasMon = IdentificationData.LTE.BTS[i].DeleteFromMeasMon;

                                        FindInMeasData = true;
                                        if (IdentificationData.LTE.BTS[i].RSRP > DetectionLevelLTE)
                                            mg.LastSeenSignal = IdentificationData.LTE.BTS[i].LastLevelUpdete;//заменить на время из GPS с учетом часового пояса
                                        mg.LastDetectionLevelUpdete = IdentificationData.LTE.BTS[i].LastDetectionLevelUpdete;
                                    }
                                }
                            }
                            #endregion
                            #region добавляем новое
                            #region
                            if (FindInMeasData == false &&
                                IdentificationData.LTE.BTS[i].ThisIsMaximumSignalAtThisFrequency == true &&
                                IdentificationData.LTE.BTS[i].RSRP > DetectionLevelLTE &&
                                new TimeSpan(MainWindow.gps.LocalTime.Ticks - IdentificationData.LTE.BTS[i].LastDetectionLevelUpdete) < new TimeSpan(0, 0, 0, 0, TimeAddAfterLevelUpdate))
                            {

                                int index = 0;
                                decimal offset = decimal.MaxValue;
                                for (int bw = 0; bw < set.Data.Length; bw++)
                                {
                                    if (Math.Abs(IdentificationData.LTE.BTS[i].Bandwidth - set.Data[bw].BW) < offset)
                                    {
                                        offset = Math.Abs(IdentificationData.LTE.BTS[i].Bandwidth - set.Data[bw].BW);
                                        index = bw;
                                    }
                                }
                                double _NdbLevel = set.Data[index].NdBLevel;
                                decimal _NdBBWMin = set.Data[index].NdBBWMin;
                                decimal _NdBBWMax = set.Data[index].NdBBWMax;
                                decimal _BWLimit = set.Data[index].BWLimit;
                                decimal _MarPeakBW = set.Data[index].MarPeakBW;

                                decimal _MeasBW = set.Data[index].MeasBW;
                                int _TracePoints = set.Data[index].TracePoints;
                                decimal _RBW = set.Data[index].RBW;
                                decimal _VBW = set.Data[index].VBW;
                                decimal _OBWP = set.Data[index].OBWPercent;
                                decimal _DeltaFreqLimit = set.Data[index].DeltaFreqLimit;


                                bool FindInResulData = false;
                                int _LevelUnit = 0;
                                bool _MeasCorrectness = false;
                                decimal _DeltaFreqMeasured = 0;
                                decimal _ChannelStrenght = -1000;
                                #region Spec
                                spectrum_data sd = new spectrum_data()
                                {
                                    FreqCentr = IdentificationData.LTE.BTS[i].FreqDn,
                                    FreqSpan = _MeasBW,
                                    FreqStart = IdentificationData.LTE.BTS[i].FreqDn - _MeasBW / 2,
                                    FreqStop = IdentificationData.LTE.BTS[i].FreqDn + _MeasBW / 2,
                                    LastMeasAltitude = 0,
                                    LastMeasLatitude = 0,
                                    LastMeasLongitude = 0,
                                    MeasDuration = 0,
                                    MeasStart = DateTime.MinValue,
                                    MeasStop = DateTime.MinValue,
                                    PreAmp = 0,
                                    RBW = _RBW,
                                    VBW = _VBW,
                                    TraceCount = 0,
                                    ATT = 0,
                                    RefLevel = -30,
                                    TracePoints = _TracePoints
                                };
                                #endregion Spec
                                #region BW
                                bandwidth_data bd = new bandwidth_data()
                                {
                                    BWLimit = _BWLimit,
                                    BWMarPeak = _MarPeakBW,
                                    BWMeasMax = _NdBBWMax,
                                    BWMeasMin = _NdBBWMin,
                                    BWMeasured = -1,
                                    NdBLevel = _NdbLevel,
                                    NdBResult = new int[3] { -1, -1, -1 },
                                    OBWPercent = _OBWP,
                                    OBWResult = new int[3] { -1, -1, -1 },
                                    BWIdentification = IdentificationData.LTE.BTS[i].Bandwidth,
                                };
                                #endregion BW
                                #region CP
                                channelpower_data[] cd = new channelpower_data[]
                                {
                                    new channelpower_data()
                                    {
                                        FreqCentr = IdentificationData.LTE.BTS[i].FreqDn,
                                        ChannelPowerBW = bd.BWMeasured,
                                        ChannelPowerResult = -1000
                                    }
                                };
                                #endregion CP
                                #region из настроек таска
                                if (MainWindow.db_v2.MeasMon.FromTask == true)
                                {
                                    #region инфа из ATDI 
                                    foreach (DB.localatdi_task_with_tech restech in MainWindow.db_v2.AtdiTask.data_from_tech)
                                    {
                                        if (restech.tech.Contains("LTE") && restech.scan_parameters.Length > 0)
                                        {
                                            foreach (DB.localatdi_standard_scan_parameter param in restech.scan_parameters)
                                            {
                                                _DeltaFreqLimit = restech.scan_parameters[0].max_frequency_relative_offset_mk;
                                                bd.BWLimit = restech.scan_parameters[0].max_permission_bw;
                                                bd.NdBLevel = (double)restech.scan_parameters[0].xdb_level_db;
                                                sd.FreqSpan = restech.scan_parameters[0].meas_span;
                                                sd.FreqStart = IdentificationData.GSM.BTS[i].FreqDn - sd.FreqSpan / 2;
                                                sd.FreqStop = IdentificationData.GSM.BTS[i].FreqDn + sd.FreqSpan / 2;
                                                sd.PreAmp = restech.scan_parameters[0].preamplification_db;
                                                sd.RBW = restech.scan_parameters[0].rbw;
                                                sd.VBW = restech.scan_parameters[0].vbw;
                                                sd.RefLevel = (double)restech.scan_parameters[0].ref_level_dbm;
                                                sd.ATT = (int)restech.scan_parameters[0].rf_attenuation_db;
                                            }
                                        }
                                    }
                                    #endregion
                                }
                                #endregion
                                sd.Trace = new tracepoint[sd.TracePoints];
                                decimal tracestep = sd.FreqSpan / (sd.TracePoints - 1);
                                //decimal fstart = IdentificationData.GSM.BTS[i].FreqDn - _MeasBW / 2;
                                for (int y = 0; y < sd.TracePoints; y++)
                                {
                                    sd.Trace[y] = new tracepoint() { freq = sd.FreqStart + tracestep * y, level = -1000 };
                                }

                                DB.localatdi_station_sys_info station_sys_info = new DB.localatdi_station_sys_info() { };
                                bool FindInTaskResults = false;

                                if (MainWindow.db_v2.MeasMon.FromTask == true)
                                {
                                    #region инфа из результатов ATDI
                                    foreach (DB.localatdi_task_with_tech restech in MainWindow.db_v2.AtdiTask.data_from_tech)
                                    {
                                        if (restech.tech.Contains("LTE") || restech.tech.ToLower().Contains("unknown"))
                                        {
                                            foreach (DB.localatdi_result_item resi in restech.ResultItems)
                                            {
                                                if (resi.id_station == IdentificationData.LTE.BTS[i].ATDI_id_station &&
                                                resi.id_sector == IdentificationData.LTE.BTS[i].ATDI_id_sector &&
                                                resi.spec_data.FreqCentr == IdentificationData.LTE.BTS[i].FreqDn &&
                                                resi.station_identifier_from_radio_tech_sub_ind == IdentificationData.LTE.BTS[i].PCI &&
                                                resi.station_identifier_from_radio == IdentificationData.LTE.BTS[i].GCID)
                                                {
                                                    if (sd.MeasStart < resi.spec_data.MeasStart || sd.MeasStart == DateTime.MinValue)
                                                    {
                                                        FindInTaskResults = true;
                                                        FindInResulData = true;
                                                        sd = resi.spec_data;
                                                        bd = resi.bw_data;
                                                        cd = resi.cp_data;
                                                        station_sys_info = resi.station_sys_info;
                                                        _ChannelStrenght = resi.meas_strength;
                                                        _MeasCorrectness = resi.meas_correctness;
                                                        if (resi.freq_centr_perm != 0 && bd.NdBResult[1] > -1 && bd.NdBResult[2] > -1)
                                                        { _DeltaFreqMeasured = Math.Round(((Math.Abs(((sd.Trace[bd.NdBResult[1]].freq + sd.Trace[bd.NdBResult[2]].freq) / 2) - (resi.freq_centr_perm))) / (resi.freq_centr_perm / 1000000)), 3); }
                                                        else if (resi.freq_centr_perm == 0 && bd.NdBResult[1] > -1 && bd.NdBResult[2] > -1)
                                                        { _DeltaFreqMeasured = Math.Round(((Math.Abs(((sd.Trace[bd.NdBResult[1]].freq + sd.Trace[bd.NdBResult[2]].freq) / 2) - (sd.FreqCentr))) / (sd.FreqCentr / 1000000)), 3); }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    #endregion
                                }
                                if (MainWindow.db_v2.MeasMon.FromTask == false)
                                {
                                    #region инфа из результатов неизвестных
                                    foreach (DB.localatdi_unknown_result_with_tech restech in MainWindow.db_v2.AtdiUnknownResult.data_from_tech)
                                    {
                                        if (restech.tech.Contains("LTE"))
                                        {
                                            foreach (DB.localatdi_result_item resi in restech.ResultItems)
                                            {
                                                if (resi.spec_data.FreqCentr == IdentificationData.LTE.BTS[i].FreqDn &&
                                                    resi.station_identifier_from_radio_s0 == IdentificationData.LTE.BTS[i].MCC &&
                                                    resi.station_identifier_from_radio_s1 == IdentificationData.LTE.BTS[i].MNC &&
                                                    resi.station_identifier_from_radio_s2 == IdentificationData.LTE.BTS[i].eNodeBId &&
                                                    resi.station_identifier_from_radio_s3 == IdentificationData.LTE.BTS[i].CID &&
                                                    resi.station_identifier_from_radio_tech_sub_ind == IdentificationData.LTE.BTS[i].PCI)
                                                {
                                                    if (sd.MeasStart < resi.spec_data.MeasStart || sd.MeasStart == DateTime.MinValue)
                                                    {
                                                        FindInTaskResults = true;
                                                        FindInResulData = true;
                                                        sd = resi.spec_data;
                                                        bd = resi.bw_data;
                                                        cd = resi.cp_data;
                                                        station_sys_info = resi.station_sys_info;
                                                        _ChannelStrenght = resi.meas_strength;
                                                        _MeasCorrectness = resi.meas_correctness;
                                                        if (resi.freq_centr_perm != 0 && bd.NdBResult[1] > -1 && bd.NdBResult[2] > -1)
                                                        { _DeltaFreqMeasured = Math.Round(((Math.Abs(((sd.Trace[bd.NdBResult[1]].freq + sd.Trace[bd.NdBResult[2]].freq) / 2) - (resi.freq_centr_perm))) / (resi.freq_centr_perm / 1000000)), 3); }
                                                        else if (resi.freq_centr_perm == 0 && bd.NdBResult[1] > -1 && bd.NdBResult[2] > -1)
                                                        { _DeltaFreqMeasured = Math.Round(((Math.Abs(((sd.Trace[bd.NdBResult[1]].freq + sd.Trace[bd.NdBResult[2]].freq) / 2) - (sd.FreqCentr))) / (sd.FreqCentr / 1000000)), 3); }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    #endregion
                                }
                                DB.MeasData result = new DB.MeasData()
                                {
                                    #region
                                    Techonology = "LTE",
                                    ATDI_Id_Task = IdentificationData.LTE.BTS[i].ATDI_id_task,
                                    ATDI_Id_Station = IdentificationData.LTE.BTS[i].ATDI_id_station,
                                    ATDI_Id_Permission = IdentificationData.LTE.BTS[i].ATDI_id_permission,
                                    ATDI_Id_Sector = IdentificationData.LTE.BTS[i].ATDI_id_sector,
                                    ATDI_Id_Frequency = IdentificationData.LTE.BTS[i].ATDI_id_frequency,
                                    ATDI_FrequencyPermission = IdentificationData.LTE.BTS[i].ATDI_FrequencyPermission,
                                    ATDI_Identifier_Find = IdentificationData.LTE.BTS[i].ATDI_Identifier_Find,
                                    ATDI_FreqCheck_Find = IdentificationData.LTE.BTS[i].ATDI_FreqCheck_Find,
                                    ATDI_GCID = IdentificationData.LTE.BTS[i].ATDI_GCID,

                                    SpecData = sd,
                                    AllTraceCount = sd.TraceCount,
                                    AllTraceCountToMeas = sd.TraceCount,
                                    station_sys_info = station_sys_info,
                                    BWData = bd,

                                    FreqUP = IdentificationData.LTE.BTS[i].FreqUp,
                                    FreqDN = IdentificationData.LTE.BTS[i].FreqDn,
                                    ChannelN = IdentificationData.LTE.BTS[i].EARFCN_DN,
                                    StandartSubband = IdentificationData.LTE.BTS[i].StandartSubband,
                                    GCID = IdentificationData.LTE.BTS[i].GCID,
                                    TechSubInd = IdentificationData.LTE.BTS[i].PCI,
                                    FullData = IdentificationData.LTE.BTS[i].FullData,
                                    Power = IdentificationData.LTE.BTS[i].RSRP,
                                    LevelResults = IdentificationData.LTE.BTS[i].level_results,
                                    LR_NewDataToSave = IdentificationData.LTE.BTS[i].LR_NewDataToSave,
                                    LastSeenSignal = IdentificationData.LTE.BTS[i].LastLevelUpdete,
                                    LastDetectionLevelUpdete = IdentificationData.LTE.BTS[i].LastDetectionLevelUpdete,
                                    ThisIsMaximumSignalAtThisFrequency = IdentificationData.LTE.BTS[i].ThisIsMaximumSignalAtThisFrequency,
                                    IdentificationData = IdentificationData.LTE.BTS[i],

                                    DeltaFreqLimit = _DeltaFreqLimit,
                                    DeltaFreqMeasured = _DeltaFreqMeasured,
                                    DeltaFreqConclusion = 0,
                                    ChannelStrenght = _ChannelStrenght,
                                    CPData = cd,
                                    difference_time_stamp_ns = 0,

                                    MeasCorrectness = _MeasCorrectness,
                                    MeasMask = new DB.localatdi_elements_mask[] { },
                                    NewDataToSave = false,
                                    Resets = 0,
                                    ThisToMeas = false,
                                    LevelUnit = _LevelUnit,


                                    device_ident = device_ident,
                                    device_meas = new DB.localatdi_meas_device() { },
                                    #endregion
                                };
                                if (App.Sett.MeasMons_Settings.SpectrumMeasDeviece == App.Sett.Equipments_Settings.RuSTSMx.ID)
                                    result.device_meas = device_meas;

                                App.Current.Dispatcher.BeginInvoke((Action)(() =>
                                {
                                    MainWindow.db_v2.MeasMon.Data.Add(result);
                                }));
                            }
                            #endregion
                            #endregion
                            #endregion
                        }
                    }
                    #region пилим количевство всякого по базам и т.д.
                    int BTSCountWithGCID = 0;

                    int ATDI_BTSCountWithGCID = 0;
                    int ATDI_BTSCountNDP = 0;
                    int ATDI_BTSCountNPE = 0;
                    for (int i = 0; i < IdentificationData.LTE.BTS.Count; i++)
                    //foreach (LTEBTSData ubd in IdentificationData.LTE.BTS)
                    {
                        if (IdentificationData.LTE.BTS[i].FullData == true && IdentificationData.LTE.BTS[i].ATDI_Identifier_Find == 1) ATDI_BTSCountNDP++;
                        if (IdentificationData.LTE.BTS[i].FullData == true && IdentificationData.LTE.BTS[i].ATDI_Identifier_Find == 2 && IdentificationData.LTE.BTS[i].ATDI_FreqCheck_Find == 1) ATDI_BTSCountNPE++;
                        if (IdentificationData.LTE.BTS[i].FullData == true && IdentificationData.LTE.BTS[i].ATDI_Identifier_Find == 2 && IdentificationData.LTE.BTS[i].ATDI_FreqCheck_Find == 2) ATDI_BTSCountWithGCID++;

                        //всего с идентификатором
                        if (IdentificationData.LTE.BTS[i].FullData == true) BTSCountWithGCID++;
                    }

                    IdentificationData.LTE.ATDI_BTSCountWithGCID = ATDI_BTSCountWithGCID;
                    IdentificationData.LTE.ATDI_BTSCountNDP = ATDI_BTSCountNDP;
                    IdentificationData.LTE.ATDI_BTSCountNPE = ATDI_BTSCountNPE;

                    IdentificationData.LTE.BTSCount = IdentificationData.LTE.BTS.Count;
                    IdentificationData.LTE.BTSCountWithGCID = BTSCountWithGCID;
                    #endregion
                }
                #endregion

                #region CDMA допилить идентификацию
                if (CDMAIsRuning == true && IdentificationData.CDMA.BTS.Count() > 0)
                {
                    Settings.MeasMonTech_Set set = App.Sett.MeasMons_Settings.CDMA;
                    double _NdbLevel = set.Data[0].NdBLevel;
                    decimal _NdBBWMin = set.Data[0].NdBBWMin;
                    decimal _NdBBWMax = set.Data[0].NdBBWMax;
                    decimal _BWLimit = set.Data[0].BWLimit;
                    decimal _MarPeakBW = set.Data[0].MarPeakBW;

                    decimal _MeasBW = set.Data[0].MeasBW;
                    int _TracePoints = set.Data[0].TracePoints;
                    decimal _RBW = set.Data[0].RBW;
                    decimal _VBW = set.Data[0].VBW;
                    decimal _OBWP = set.Data[0].OBWPercent;
                    decimal _DeltaFreqLimit = set.Data[0].DeltaFreqLimit;

                    for (int i = 0; i < IdentificationData.CDMA.BTS.Count; i++)
                    {
                        if (IdentificationData.CDMA.BTS[i].FullData)
                        {
                            #region ищем по ATDI
                            if (IdentificationData.CDMA.BTS[i].ATDI_Identifier_Find == 0)
                            {
                                if (MainWindow.db_v2.MeasMon.FromTask == true)
                                {
                                    if (IdentificationData.CDMA.BTS[i].ATDI_IDFrom != "COOR")
                                    {
                                        #region
                                        foreach (DB.localatdi_task_with_tech tasktech in MainWindow.db_v2.AtdiTask.data_from_tech)
                                        {
                                            if (tasktech.tech.ToUpper().Contains("CDMA"))
                                            {
                                                foreach (DB.localatdi_station tasktechitem in tasktech.TaskItems)
                                                {
                                                    for (int c = 0; c < App.Sett.MeasMons_Settings.OPSOSIdentifications.Count; c++)
                                                    {
                                                        bool findcid = false;
                                                        #region from db
                                                        if (App.Sett.MeasMons_Settings.OPSOSIdentifications[c].Techonology == "CDMA" &&
                                                            App.Sett.MeasMons_Settings.OPSOSIdentifications[c].MNC_Radio_From == "SID" &&
                                                            App.Sett.MeasMons_Settings.OPSOSIdentifications[c].MNC_Radio == IdentificationData.CDMA.BTS[i].SID &&
                                                            tasktechitem.Callsign_db_S3 == IdentificationData.CDMA.BTS[i].CIDToDB)
                                                        {
                                                            findcid = true;
                                                            IdentificationData.CDMA.BTS[i].ATDI_Identifier_Find = 2;
                                                            IdentificationData.CDMA.BTS[i].ATDI_id_station = tasktechitem.id;
                                                            IdentificationData.CDMA.BTS[i].ATDI_id_permission = tasktechitem.license.icsm_id;
                                                            IdentificationData.CDMA.BTS[i].ATDI_GCID = tasktechitem.callsign_db;
                                                            tasktechitem.IsIdentified = true;
                                                            foreach (DB.localatdi_station_sector sectoritem in tasktechitem.sectors)
                                                            {
                                                                IdentificationData.CDMA.BTS[i].ATDI_id_sector = sectoritem.sector_id;
                                                                foreach (DB.localatdi_sector_frequency freq in sectoritem.frequencies)
                                                                {
                                                                    if (IdentificationData.CDMA.BTS[i].FreqDn == freq.frequency)
                                                                    {
                                                                        IdentificationData.CDMA.BTS[i].ATDI_FreqCheck_Find = 2;
                                                                        IdentificationData.CDMA.BTS[i].ATDI_id_frequency = freq.id;
                                                                        IdentificationData.CDMA.BTS[i].ATDI_FrequencyPermission = freq.frequency;
                                                                    }
                                                                }
                                                            }
                                                        }
                                                        #endregion
                                                    }

                                                    //if (task.meas_task_id > IdentificationData.CDMA.BTS[i].ATDI_MeasTask_id)
                                                    IdentificationData.CDMA.BTS[i].ATDI_id_task = MainWindow.db_v2.AtdiTask.task_id;
                                                }
                                            }
                                        }
                                        #endregion
                                    }
                                    else
                                    {
                                        double dist = double.MaxValue, ang = 0;
                                        int freqid = 0;
                                        string sectorid = "", stationid = "";
                                        int[] freqids = new int[] { };
                                        #region 
                                        foreach (DB.localatdi_task_with_tech tasktech in MainWindow.db_v2.AtdiTask.data_from_tech)
                                        {
                                            if (tasktech.tech.ToUpper().Contains("CDMA"))
                                            {
                                                int stind = 0;
                                                for (int st = 0; st < tasktech.TaskItems.Count; st++)
                                                //foreach (DB.atdi_station_data_for_measurements tasktechitem in tasktech.TaskItems)
                                                {
                                                    double measdist = double.MaxValue, measang = 0;
                                                    MainWindow.help.calcDistance
                                                        (
                                                        (double)MainWindow.gps.LatitudeDecimal,
                                                        (double)MainWindow.gps.LongitudeDecimal,
                                                        (double)tasktech.TaskItems[st].site.location.latitude,
                                                        (double)tasktech.TaskItems[st].site.location.longitude, out measdist, out measang);

                                                    if (measdist < dist)
                                                    {
                                                        stind = st;
                                                        dist = measdist;
                                                        stationid = tasktech.TaskItems[st].id;
                                                    }
                                                    //tasktechitem.IsIdentified = true;
                                                }
                                                tasktech.TaskItems[stind].IsIdentified = true;
                                                IdentificationData.CDMA.BTS[i].ATDI_id_station = tasktech.TaskItems[stind].id;
                                                IdentificationData.CDMA.BTS[i].ATDI_id_permission = tasktech.TaskItems[stind].license.icsm_id;
                                                IdentificationData.CDMA.BTS[i].ATDI_GCID = tasktech.TaskItems[stind].callsign_db;
                                                foreach (DB.localatdi_station_sector sectoritem in tasktech.TaskItems[stind].sectors)
                                                {
                                                    IdentificationData.CDMA.BTS[i].ATDI_id_sector = sectoritem.sector_id;
                                                    foreach (DB.localatdi_sector_frequency freq in sectoritem.frequencies)
                                                    {
                                                        if (IdentificationData.CDMA.BTS[i].FreqDn == freq.frequency)
                                                        {
                                                            IdentificationData.CDMA.BTS[i].ATDI_FreqCheck_Find = 2;
                                                            IdentificationData.CDMA.BTS[i].ATDI_id_frequency = freq.id;
                                                            IdentificationData.CDMA.BTS[i].ATDI_FrequencyPermission = freq.frequency;
                                                        }
                                                    }
                                                }
                                                IdentificationData.CDMA.BTS[i].ATDI_Identifier_Find = 2;
                                                IdentificationData.CDMA.BTS[i].ATDI_id_station = stationid;
                                                IdentificationData.CDMA.BTS[i].ATDI_id_sector = sectorid;
                                                IdentificationData.CDMA.BTS[i].ATDI_id_task = MainWindow.db_v2.AtdiTask.task_id;
                                            }
                                        }
                                        #endregion

                                    }
                                }
                                else IdentificationData.CDMA.BTS[i].ATDI_id_task = MainWindow.db_v2.AtdiTask.task_id;
                                if (IdentificationData.CDMA.BTS[i].ATDI_Identifier_Find == 0)
                                {
                                    IdentificationData.CDMA.BTS[i].ATDI_Identifier_Find = 1;
                                    IdentificationData.CDMA.BTS[i].ATDI_id_station = IdentificationData.CDMA.BTS[i].CIDToDB.ToString();
                                    IdentificationData.CDMA.BTS[i].ATDI_id_permission = 0;
                                    IdentificationData.CDMA.BTS[i].ATDI_id_sector = "";
                                }
                                if (IdentificationData.CDMA.BTS[i].ATDI_FreqCheck_Find == 0)
                                {
                                    IdentificationData.CDMA.BTS[i].ATDI_FreqCheck_Find = 1;
                                    IdentificationData.CDMA.BTS[i].ATDI_id_frequency = 0;
                                }
                            }
                            #endregion
                            #region в измерения
                            #region найдено ли в измерениях уже
                            bool FindInMeasData = false;
                            if (MainWindow.db_v2.MeasMon.Data.Count > 0)
                            {
                                for (int mm = 0; mm < MainWindow.db_v2.MeasMon.Data.Count; mm++)//foreach (DB.MeasData mg in MainWindow.db_v2.MonMeas)
                                {
                                    DB.MeasData mg = MainWindow.db_v2.MeasMon.Data[mm];
                                    if (mg.Techonology == "CDMA" &&
                                        mg.TechSubInd == IdentificationData.CDMA.BTS[i].PN &&
                                        mg.FreqDN == IdentificationData.CDMA.BTS[i].FreqDn &&
                                        mg.GCID == IdentificationData.CDMA.BTS[i].GCID)
                                    {
                                        mg.Power = IdentificationData.CDMA.BTS[i].RSCP;
                                        if (IdentificationData.CDMA.BTS[i].level_results.Count > 0 &&
                                            IdentificationData.CDMA.BTS[i].level_results[IdentificationData.CDMA.BTS[i].level_results.Count - 1].saved_in_result == false)
                                            MainWindow.db_v2.MeasMon.Data[mm].LR_NewDataToSave = true;

                                        mg.ThisIsMaximumSignalAtThisFrequency = IdentificationData.CDMA.BTS[i].ThisIsMaximumSignalAtThisFrequency;
                                        //mg.ATDI_Id_Task = IdentificationData.CDMA.BTS[i].ATDI_id_task;
                                        //mg.ATDI_Identifier_Find = IdentificationData.CDMA.BTS[i].ATDI_Identifier_Find;
                                        //mg.ATDI_FreqCheck_Find = IdentificationData.CDMA.BTS[i].ATDI_FreqCheck_Find;
                                        //mg.ATDI_Id_Station = IdentificationData.CDMA.BTS[i].ATDI_id_station;
                                        //mg.ATDI_Id_Permission = IdentificationData.CDMA.BTS[i].ATDI_id_permission;
                                        //mg.ATDI_Id_Sector = IdentificationData.CDMA.BTS[i].ATDI_id_sector;
                                        //mg.ATDI_Id_Frequency = IdentificationData.CDMA.BTS[i].ATDI_id_frequency;
                                        //mg.ATDI_GCID = IdentificationData.CDMA.BTS[i].ATDI_GCID;
                                        //mg.ATDI_FrequencyPermission = IdentificationData.CDMA.BTS[i].ATDI_FrequencyPermission;
                                        mg.station_sys_info.information_blocks = IdentificationData.CDMA.BTS[i].station_sys_info.information_blocks;
                                        mg.DeleteFromMeasMon = IdentificationData.CDMA.BTS[i].DeleteFromMeasMon;

                                        FindInMeasData = true;
                                        if (IdentificationData.CDMA.BTS[i].RSCP > DetectionLevelCDMA)
                                            mg.LastSeenSignal = IdentificationData.CDMA.BTS[i].LastLevelUpdete;//заменить на время из GPS с учетом часового пояса
                                        mg.LastDetectionLevelUpdete = IdentificationData.CDMA.BTS[i].LastDetectionLevelUpdete;
                                    }
                                }
                            }
                            #endregion
                            #region добавляем новое
                            if (FindInMeasData == false &&
                                IdentificationData.CDMA.BTS[i].ThisIsMaximumSignalAtThisFrequency == true &&
                                IdentificationData.CDMA.BTS[i].RSCP > DetectionLevelCDMA &&
                                new TimeSpan(MainWindow.gps.LocalTime.Ticks - IdentificationData.CDMA.BTS[i].LastDetectionLevelUpdete) < new TimeSpan(0, 0, 0, 0, TimeAddAfterLevelUpdate))
                            {
                                bool FindInResulData = false;//ищем в результатах
                                int _LevelUnit = 0;
                                bool _MeasCorrectness = false;
                                decimal _DeltaFreqMeasured = 0;
                                decimal _ChannelStrenght = -1000;
                                #region Spec
                                spectrum_data sd = new spectrum_data()
                                {
                                    FreqCentr = IdentificationData.CDMA.BTS[i].FreqDn,
                                    FreqSpan = _MeasBW,
                                    FreqStart = IdentificationData.CDMA.BTS[i].FreqDn - _MeasBW / 2,
                                    FreqStop = IdentificationData.CDMA.BTS[i].FreqDn + _MeasBW / 2,
                                    LastMeasAltitude = 0,
                                    LastMeasLatitude = 0,
                                    LastMeasLongitude = 0,
                                    MeasDuration = 0,
                                    MeasStart = DateTime.MinValue,
                                    MeasStop = DateTime.MinValue,
                                    PreAmp = 0,
                                    RBW = _RBW,
                                    VBW = _VBW,
                                    TraceCount = 0,
                                    ATT = 0,
                                    RefLevel = -30,
                                    TracePoints = _TracePoints
                                };
                                #endregion Spec
                                #region BW
                                bandwidth_data bd = new bandwidth_data()
                                {
                                    BWLimit = _BWLimit,
                                    BWMarPeak = _MarPeakBW,
                                    BWMeasMax = _NdBBWMax,
                                    BWMeasMin = _NdBBWMin,
                                    BWMeasured = -1,
                                    NdBLevel = _NdbLevel,
                                    NdBResult = new int[3] { -1, -1, -1 },
                                    OBWPercent = _OBWP,
                                    OBWResult = new int[3] { -1, -1, -1 },
                                    BWIdentification = _NdBBWMin,
                                };
                                #endregion BW
                                #region CP
                                channelpower_data[] cd = new channelpower_data[]
                                {
                                    new channelpower_data()
                                    {
                                        FreqCentr = IdentificationData.CDMA.BTS[i].FreqDn,
                                        ChannelPowerBW = bd.BWMeasured,
                                        ChannelPowerResult = -1000
                                    }
                                };
                                #endregion CP
                                #region из настроек таска
                                if (MainWindow.db_v2.MeasMon.FromTask == true)
                                {
                                    #region инфа из ATDI 
                                    foreach (DB.localatdi_task_with_tech restech in MainWindow.db_v2.AtdiTask.data_from_tech)
                                    {
                                        if (restech.tech.Contains("CDMA") && restech.scan_parameters.Length > 0)
                                        {
                                            _DeltaFreqLimit = restech.scan_parameters[0].max_frequency_relative_offset_mk;
                                            bd.BWLimit = restech.scan_parameters[0].max_permission_bw;
                                            bd.NdBLevel = (double)restech.scan_parameters[0].xdb_level_db;
                                            sd.FreqSpan = restech.scan_parameters[0].meas_span;
                                            sd.FreqStart = sd.FreqCentr - sd.FreqSpan / 2;
                                            sd.FreqStop = sd.FreqCentr + sd.FreqSpan / 2;
                                            sd.PreAmp = restech.scan_parameters[0].preamplification_db;
                                            sd.RBW = restech.scan_parameters[0].rbw;
                                            sd.VBW = restech.scan_parameters[0].vbw;
                                            sd.RefLevel = (double)restech.scan_parameters[0].ref_level_dbm;
                                            sd.ATT = (int)restech.scan_parameters[0].rf_attenuation_db;

                                            //foreach (DB.localatdi_standard_scan_parameter param in restech.scan_parameters)
                                            //{

                                            //}
                                        }
                                    }
                                    #endregion
                                }
                                #endregion
                                sd.Trace = new tracepoint[sd.TracePoints];
                                decimal tracestep = sd.FreqSpan / (sd.TracePoints - 1);
                                for (int y = 0; y < sd.TracePoints; y++)
                                {
                                    sd.Trace[y] = new tracepoint() { freq = sd.FreqStart + tracestep * y, level = -1000 };
                                }

                                DB.localatdi_station_sys_info station_sys_info = new DB.localatdi_station_sys_info() { };
                                bool FindInTaskResults = false;

                                if (MainWindow.db_v2.MeasMon.FromTask == true)
                                {
                                    #region инфа из результатов ATDI
                                    foreach (DB.localatdi_task_with_tech restech in MainWindow.db_v2.AtdiTask.data_from_tech)
                                    {
                                        if (restech.tech.Contains("CDMA") || restech.tech.ToLower().Contains("unknown"))
                                        {
                                            foreach (DB.localatdi_result_item resi in restech.ResultItems)
                                            {
                                                if (resi.id_station == IdentificationData.CDMA.BTS[i].ATDI_id_station &&
                                                resi.id_sector == IdentificationData.CDMA.BTS[i].ATDI_id_sector &&
                                                resi.spec_data.FreqCentr == IdentificationData.CDMA.BTS[i].FreqDn &&
                                                resi.station_identifier_from_radio_tech_sub_ind == IdentificationData.CDMA.BTS[i].PN &&
                                                resi.station_identifier_from_radio == IdentificationData.CDMA.BTS[i].GCID)
                                                {
                                                    if (sd.MeasStart < resi.spec_data.MeasStart || sd.MeasStart == DateTime.MinValue)
                                                    {
                                                        FindInTaskResults = true;
                                                        FindInResulData = true;
                                                        sd = resi.spec_data;
                                                        bd = resi.bw_data;
                                                        cd = resi.cp_data;
                                                        station_sys_info = resi.station_sys_info;
                                                        _ChannelStrenght = resi.meas_strength;
                                                        _MeasCorrectness = resi.meas_correctness;

                                                        if (resi.freq_centr_perm != 0 && bd.NdBResult[1] > -1 && bd.NdBResult[2] > -1)
                                                        { _DeltaFreqMeasured = Math.Round(((Math.Abs(((sd.Trace[bd.NdBResult[1]].freq + sd.Trace[bd.NdBResult[2]].freq) / 2) - (resi.freq_centr_perm))) / (resi.freq_centr_perm / 1000000)), 3); }
                                                        else if (resi.freq_centr_perm == 0 && bd.NdBResult[1] > -1 && bd.NdBResult[2] > -1)
                                                        { _DeltaFreqMeasured = Math.Round(((Math.Abs(((sd.Trace[bd.NdBResult[1]].freq + sd.Trace[bd.NdBResult[2]].freq) / 2) - (sd.FreqCentr))) / (sd.FreqCentr / 1000000)), 3); }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    #endregion
                                }
                                if (MainWindow.db_v2.MeasMon.FromTask == false)
                                {
                                    #region инфа из результатов неизвестных
                                    foreach (DB.localatdi_unknown_result_with_tech restech in MainWindow.db_v2.AtdiUnknownResult.data_from_tech)
                                    {
                                        if (restech.tech.Contains("CDMA"))
                                        {
                                            foreach (DB.localatdi_result_item resi in restech.ResultItems)
                                            {
                                                if (resi.spec_data.FreqCentr == IdentificationData.CDMA.BTS[i].FreqDn &&
                                                    resi.station_identifier_from_radio_tech_sub_ind == IdentificationData.CDMA.BTS[i].PN &&
                                                    resi.station_identifier_from_radio == IdentificationData.CDMA.BTS[i].GCID)
                                                {
                                                    if (sd.MeasStart < resi.spec_data.MeasStart || sd.MeasStart == DateTime.MinValue)
                                                    {
                                                        FindInTaskResults = true;
                                                        FindInResulData = true;
                                                        sd = resi.spec_data;
                                                        bd = resi.bw_data;
                                                        cd = resi.cp_data;
                                                        station_sys_info = resi.station_sys_info;
                                                        _ChannelStrenght = resi.meas_strength;
                                                        _MeasCorrectness = resi.meas_correctness;
                                                        if (resi.freq_centr_perm != 0 && bd.NdBResult[1] > -1 && bd.NdBResult[2] > -1)
                                                        { _DeltaFreqMeasured = Math.Round(((Math.Abs(((sd.Trace[bd.NdBResult[1]].freq + sd.Trace[bd.NdBResult[2]].freq) / 2) - (resi.freq_centr_perm))) / (resi.freq_centr_perm / 1000000)), 3); }
                                                        else if (resi.freq_centr_perm == 0 && bd.NdBResult[1] > -1 && bd.NdBResult[2] > -1)
                                                        { _DeltaFreqMeasured = Math.Round(((Math.Abs(((sd.Trace[bd.NdBResult[1]].freq + sd.Trace[bd.NdBResult[2]].freq) / 2) - (sd.FreqCentr))) / (sd.FreqCentr / 1000000)), 3); }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    #endregion
                                }
                                DB.MeasData result = new DB.MeasData()
                                {
                                    #region
                                    Techonology = "CDMA",
                                    ATDI_Id_Task = IdentificationData.CDMA.BTS[i].ATDI_id_task,
                                    ATDI_Id_Station = IdentificationData.CDMA.BTS[i].ATDI_id_station,
                                    ATDI_Id_Permission = IdentificationData.CDMA.BTS[i].ATDI_id_permission,
                                    ATDI_Id_Sector = IdentificationData.CDMA.BTS[i].ATDI_id_sector,
                                    ATDI_Id_Frequency = IdentificationData.CDMA.BTS[i].ATDI_id_frequency,
                                    ATDI_FrequencyPermission = IdentificationData.CDMA.BTS[i].ATDI_FrequencyPermission,
                                    ATDI_Identifier_Find = IdentificationData.CDMA.BTS[i].ATDI_Identifier_Find,
                                    ATDI_FreqCheck_Find = IdentificationData.CDMA.BTS[i].ATDI_FreqCheck_Find,
                                    ATDI_GCID = IdentificationData.CDMA.BTS[i].ATDI_GCID,

                                    SpecData = sd,
                                    NewSpecDataToSave = false,
                                    AllTraceCount = sd.TraceCount,
                                    AllTraceCountToMeas = sd.TraceCount,
                                    station_sys_info = station_sys_info,
                                    BWData = bd,

                                    FreqUP = IdentificationData.CDMA.BTS[i].FreqUp,
                                    FreqDN = IdentificationData.CDMA.BTS[i].FreqDn,
                                    ChannelN = IdentificationData.CDMA.BTS[i].ChannelN,
                                    StandartSubband = IdentificationData.CDMA.BTS[i].StandartSubband,
                                    GCID = IdentificationData.CDMA.BTS[i].GCID,
                                    TechSubInd = IdentificationData.CDMA.BTS[i].PN,
                                    FullData = IdentificationData.CDMA.BTS[i].FullData,
                                    Power = IdentificationData.CDMA.BTS[i].RSCP,
                                    LevelResults = IdentificationData.CDMA.BTS[i].level_results,
                                    LR_NewDataToSave = IdentificationData.CDMA.BTS[i].LR_NewDataToSave,
                                    LastSeenSignal = IdentificationData.CDMA.BTS[i].LastLevelUpdete,
                                    LastDetectionLevelUpdete = IdentificationData.CDMA.BTS[i].LastDetectionLevelUpdete,
                                    ThisIsMaximumSignalAtThisFrequency = IdentificationData.CDMA.BTS[i].ThisIsMaximumSignalAtThisFrequency,

                                    IdentificationData = IdentificationData.CDMA.BTS[i],

                                    DeltaFreqLimit = _DeltaFreqLimit,
                                    DeltaFreqMeasured = _DeltaFreqMeasured,
                                    DeltaFreqConclusion = 0,
                                    ChannelStrenght = _ChannelStrenght,
                                    CPData = cd,
                                    difference_time_stamp_ns = 0,

                                    MeasCorrectness = _MeasCorrectness,
                                    MeasMask = new DB.localatdi_elements_mask[] { },
                                    NewDataToSave = false,
                                    Resets = 0,
                                    ThisToMeas = false,
                                    LevelUnit = _LevelUnit,


                                    device_ident = device_ident,
                                    device_meas = new DB.localatdi_meas_device() { },
                                    #endregion
                                };
                                if (App.Sett.MeasMons_Settings.SpectrumMeasDeviece == App.Sett.Equipments_Settings.RuSTSMx.ID)
                                    result.device_meas = device_meas;
                                App.Current.Dispatcher.BeginInvoke((Action)(() =>
                                {
                                    MainWindow.db_v2.MeasMon.Data.Add(result);
                                }));
                            }
                            #endregion
                            #endregion
                        }
                    }
                    #region пилим количевство всякого по базам и т.д.
                    int BTSCountWithGCID = 0;
                    int ATDI_BTSCountWithGCID = 0;
                    int ATDI_BTSCountNDP = 0;
                    int ATDI_BTSCountNPE = 0;
                    for (int i = 0; i < IdentificationData.CDMA.BTS.Count; i++)
                    //foreach (CDMABTSData ubd in IdentificationData.CDMA.BTS)
                    {
                        if (IdentificationData.CDMA.BTS[i].FullData == true && IdentificationData.CDMA.BTS[i].ATDI_Identifier_Find == 1) ATDI_BTSCountNDP++;
                        if (IdentificationData.CDMA.BTS[i].FullData == true && IdentificationData.CDMA.BTS[i].ATDI_Identifier_Find == 2 && IdentificationData.CDMA.BTS[i].ATDI_FreqCheck_Find == 1) ATDI_BTSCountNPE++;
                        if (IdentificationData.CDMA.BTS[i].FullData == true && IdentificationData.CDMA.BTS[i].ATDI_Identifier_Find == 2) ATDI_BTSCountWithGCID++;

                        //всего с идентификатором
                        if (IdentificationData.CDMA.BTS[i].FullData == true) BTSCountWithGCID++;
                    }
                    IdentificationData.CDMA.ATDI_BTSCountWithGCID = ATDI_BTSCountWithGCID;
                    IdentificationData.CDMA.ATDI_BTSCountNDP = ATDI_BTSCountNDP;
                    IdentificationData.CDMA.ATDI_BTSCountNPE = ATDI_BTSCountNPE;

                    IdentificationData.CDMA.BTSCount = IdentificationData.CDMA.BTS.Count;
                    IdentificationData.CDMA.BTSCountWithGCID = BTSCountWithGCID;
                    #endregion                    
                }
                #endregion

                //удаляем старые записи которые давно невидели по уровню детектирования
                for (int m = 0; m < MainWindow.db_v2.MeasMon.Data.Count; m++)//foreach (DB.MeasData mg in MainWindow.db_v2.MonMeas)
                {
                    if (MainWindow.db_v2.MeasMon.Data[m].ThisToMeas == false &&
                        MainWindow.db_v2.MeasMon.Data[m].NewDataToSave == false &&
                        MainWindow.db_v2.MeasMon.Data[m].DeleteFromMeasMon == true &&
                        (MainWindow.db_v2.MeasMon.Data[m].LR_NewDataToSave == false || MainWindow.db_v2.MonMeasSaveAll == false) &&
                        new TimeSpan(MainWindow.gps.LocalTime.Ticks - MainWindow.db_v2.MeasMon.Data[m].LastDetectionLevelUpdete) > new TimeSpan(0, 0, 10))
                    {
                        GUIThreadDispatcher.Instance.Invoke(() =>
                        {
                            MainWindow.db_v2.MeasMon.Data.RemoveAt(m);
                            m--;
                        });
                    }
                }
            }
            DM -= UpdateDataInThread;
        }









        #region Classes
        class CReceiverListener : RohdeSchwarz.ViCom.Net.CViComReceiverListener
        {
            public CReceiverListener()
            { }

            //* CViComReceiverListener interface implementation ************************************

            public override void OnConnectProgress(float progressInPct, String message)
            {
                if (message.Length > 0 && message.ToLower().Contains("connected"))
                {
                    int StartSn = 0;
                    int StopSn = 0;
                    StartSn = message.ToLower().IndexOf("s.n. ") + 5;
                    StopSn = message.ToLower().IndexOf(":", StartSn);
                    if (StartSn > 0 && StopSn > 0 && StartSn < StopSn)
                    {
                        string sn = message.Substring(StartSn, StopSn - StartSn);
                        if (SerialNumberTemp != sn) SerialNumberTemp = sn;
                        //Debug.WriteLine("connected " + sn);
                    }
                }
            }
            public override void OnWarning(Warning.Type warning, String message)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = new Exception(message), ClassName = "TSMxReceiver", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name + "\r\n" + warning };
                //Console.WriteLine("OnWarning: " + message);
            }
            public override void OnError(Error.Type error, String message)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = new Exception(message), ClassName = "TSMxReceiver", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name + "\r\n" + error };
                //Console.WriteLine("OnError: " + message);
            }
        };

        class MessageTracer : ViComMessageTracer
        {
            public void OnMessage(string text)
            {
                //MainWindow.exp.ExceptionData = new ExData() { ex = new Exception(String.Format("T {0} {1}", DateTime.Now, text)), ClassName = "TSMxReceiver", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name + "\r\n" };
                MainWindow.tsmx.OutText += String.Format("T {0} {1}", DateTime.Now, text) + "\r\n";//System.Console.WriteLine(text);
            }
        }

        class LowLevelErrorHandlerImplementation : UserLowLevelErrorMessageHandler.LowLevelErrorHandler
        {
            public void OnLowLevelError(string text, string module, uint nType)
            {
                App.Current.Dispatcher.BeginInvoke((Action)(() =>
                {
                    //MainWindow.tsmx.Errors += "\r\n" + String.Format("LowLevelError: Module '{0}' reported error: {1}, nType={2}", module, text, nType);
                }));
                //Console.WriteLine("LowLevelError: Module '{0}' reported error: {1}, nType={2}", module, text, nType);
            }
        }
        #endregion


        #region Methods  
        public void SetToDelegate(DoubMod m)
        {
            if (DM != null && m != null)
            {
                bool find = false;
                foreach (Delegate d in DM.GetInvocationList())
                {
                    if (d.Method.Name == m.GetInvocationList()[0].Method.Name) find = true;
                }
                if (find == false) DM += (DoubMod)m.GetInvocationList()[0];
            }
        }

        private void DetectScanner()
        {
            try
            {
                // Load
                gpsLoader = new CViComLoader<CViComGpsInterface>(DeviceType);
                gsmLoader = new CViComLoader<CViComGsmInterface>(DeviceType);
                wcdmaLoader = new CViComLoader<CViComWcdmaInterface>(DeviceType);
                lteLoader = new CViComLoader<CViComLteInterface>(DeviceType);
                acdLoader = new CViComLoader<CViComAcdInterface>(DeviceType);
                cdmaLoader = new CViComLoader<CViComCdmaInterface>(DeviceType);
                cwScanLoader = new CViComLoader<CViComCWScanInterface>(DeviceType);
                RFPowerScanLoader = new CViComLoader<CViComRFPowerScanInterface>(DeviceType);

                GetTechSettings();

                //string SN = "";
                bool firstStart = false;
                try
                {
                    GPSIsRuning = gpsLoader.Connect(App.Sett.TSMxReceiver_Settings.IPAdress, out error, receiverListener);
                    if (GPSIsRuning)
                    {
                        Option_GNSS = 1;
                    }
                    else Option_GNSS = 2;
                    GpsInterface = gpsLoader.GetInterface();
                    GpsBasicInterface = gpsLoader.GetBasicInterface();
                    Thread.Sleep(500);
                    myReceivers = GpsBasicInterface.GetConnectedReceivers(SDefs.dwDefaultTimeOutInMs);
                    if (myReceivers.Receivers != null & myReceivers.Receivers.Length > 0 && myReceivers.Receivers[0] != null)
                    { option = myReceivers.Receivers[0].ListOfDeviceOptions; }
                    //foreach (SConnectedReceiverTable.SReceiver rc in myReceivers.Receivers)
                    //{
                    //    //SN = rc.dwSerialNumber.ToString();
                    //    //Errors += "\r\n" + "Count Receivers " + myReceivers.dwCountOfReceivers.ToString();
                    //    //Errors += "\r\n" + "SerialNumber " + rc.dwSerialNumber;
                    //    option = rc.ListOfDeviceOptions;
                    //    //foreach (SConnectedReceiverTable.SReceiver.SDeviceOption op in option)
                    //    //{
                    //    //    Errors += "\r\n" + "OptionType " + op.pcOptionType + "   " + op.wOptionIndex;
                    //    //}
                    //}
                    //gpsLoader.Disconnect();

                    device_ident.manufacture = "Rohde&Schwarz";
                    device_ident.model = DeviceType.ToString();
                    device_ident.sn = SerialNumberTemp;
                    device_meas.manufacture = "Rohde&Schwarz";
                    device_meas.model = DeviceType.ToString();
                    device_meas.sn = SerialNumberTemp;
                    if (SerialNumberTemp != InstrSerialNumber)
                    {
                        Option_GSM = 0;
                        Option_UMTS = 0;
                        Option_LTE = 0;
                        Option_ACD = 0;
                        Option_CDMA = 0;
                        Option_RFPS = 0;
                        InstrSerialNumber = SerialNumberTemp;
                        firstStart = true;
                    }
                    bool K0 = false;
                    foreach (SConnectedReceiverTable.SReceiver.SDeviceOption op in option)
                    {
                        if (op.boActiveOption && op.pcOptionType.Contains("-K0"))// временная на жире
                        {
                            K0 = true;//все хорошо и активируем все
                            Option_GSM = 1;
                            Option_UMTS = 1;
                            Option_LTE = 1;
                            Option_ACD = 1;
                            Option_CDMA = 1;
                            Option_RFPS = 1;
                        }
                    }

                    if (!K0)//нету демо опции на жире проверим все, как оказалось не все K0 это на жире...
                    {
                        Option_GSM = 2;
                        Option_UMTS = 2;
                        Option_LTE = 2;
                        Option_ACD = 2;
                        Option_CDMA = 2;
                        Option_RFPS = 2;
                        if (DeviceType == DeviceType.Tsme)
                        {
                            #region
                            foreach (SConnectedReceiverTable.SReceiver.SDeviceOption op in option)//проверим что есть
                            {
                                if (op.boActiveOption && op.pcOptionType.Contains("-K21"))//UMTS
                                {
                                    Option_UMTS = 1;
                                }
                                else if (op.boActiveOption && op.pcOptionType.Contains("-K22"))//CDMA
                                {
                                    Option_CDMA = 1;
                                }
                                else if (op.boActiveOption && op.pcOptionType.Contains("-K23"))//GSM
                                {
                                    Option_GSM = 1;
                                }
                                else if (op.boActiveOption && op.pcOptionType.Contains("-K24"))//EVDO
                                {
                                    Option_CDMA = 1;
                                }
                                else if (op.boActiveOption && op.pcOptionType.Contains("-K27"))//RF Power Scan
                                {
                                    Option_RFPS = 1;
                                }
                                else if (op.boActiveOption && op.pcOptionType.Contains("-K29"))//LTE
                                {
                                    Option_LTE = 1;
                                }
                                else if (op.boActiveOption && op.pcOptionType.Contains("-K40"))//ACD
                                {
                                    Option_ACD = 1;
                                }
                            }
                            #endregion
                        }
                        else if (DeviceType == DeviceType.Tsmw)
                        {
                            #region
                            foreach (SConnectedReceiverTable.SReceiver.SDeviceOption op in option)//проверим что есть
                            {
                                if (op.boActiveOption && (op.pcOptionType.Contains("-K21") || op.pcOptionType.Contains("-K121") || op.pcOptionType.Contains("-K221")))//GSM UMTS
                                {
                                    Option_UMTS = 1; Option_GSM = 1;
                                }
                                else if (op.boActiveOption && (op.pcOptionType.Contains("-K22") || op.pcOptionType.Contains("-K122") || op.pcOptionType.Contains("-K222")))//CDMA EVDO
                                {
                                    Option_CDMA = 1;
                                }
                                else if (op.boActiveOption && (op.pcOptionType.Contains("-K27") || op.pcOptionType.Contains("-K127")))//RF Power Scan
                                {
                                    Option_RFPS = 1;
                                }
                                else if (op.boActiveOption && (op.pcOptionType.Contains("-K29") || op.pcOptionType.Contains("-K129")))//LTE
                                {
                                    Option_LTE = 1;
                                }
                                else if (op.boActiveOption && op.pcOptionType.Contains("-K40"))//ACD
                                {
                                    Option_ACD = 1;
                                }
                            }
                            #endregion
                        }
                    }

                }
                catch (RohdeSchwarz.ViCom.Net.CViComError error)
                {
                    MainWindow.exp.ExceptionData = new ExData() { ex = new Exception("ErrorCode:" + error.ErrorCode + " ErrorString:" + error.ErrorString), ClassName = "TSMxReceiver", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
                }
                catch (Exception exp)
                {
                    MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "TSMxReceiver", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
                }
                if (firstStart == true)
                {
                    App.Sett.SaveTSMxReceiver();
                }
                #region
                if (Option_GNSS == 1)
                {
                    DM += GpsConnect;
                }
                if (Option_GSM == 1)
                {
                    App.Current.Dispatcher.Invoke((Action)(() =>
                    {
                        IdentificationData.GSM.BTS.Clear();
                    }));
                    if (App.Sett.TSMxReceiver_Settings.GSM.TechIsEnabled == true)
                    { DM += SetNewStateGSM; }
                }
                if (Option_UMTS == 1)
                {
                    App.Current.Dispatcher.Invoke((Action)(() =>
                    {
                        IdentificationData.UMTS.BTS.Clear();
                    }));
                    if (App.Sett.TSMxReceiver_Settings.UMTS.TechIsEnabled == true)
                    { DM += SetNewStateUMTS; ; }
                }
                if (Option_LTE == 1)
                {
                    App.Current.Dispatcher.Invoke((Action)(() =>
                    {
                        IdentificationData.LTE.BTS.Clear();
                    }));
                    if (App.Sett.TSMxReceiver_Settings.LTE.TechIsEnabled == true)
                    { DM += SetNewStateLTE; }
                }
                if (Option_ACD == 1)
                {
                    App.Current.Dispatcher.Invoke((Action)(() =>
                    {
                        IdentificationData.ACD.ACDData.Clear();
                    }));
                    if (App.Sett.TSMxReceiver_Settings.ACD.TechIsEnabled == true)
                    { DM += SetNewStateACD; }
                }
                if (Option_CDMA == 1)
                {
                    App.Current.Dispatcher.Invoke((Action)(() =>
                    {
                        IdentificationData.CDMA.BTS.Clear();
                    }));
                    if (App.Sett.TSMxReceiver_Settings.CDMA.TechIsEnabled == true)
                    { DM += SetNewStateCDMA; }
                }
                if (Option_RFPS == 1)
                {
                    if (App.Sett.TSMxReceiver_Settings.UseRFPowerScan == true)
                    {
                        DM += SetNewStateRfPowerScan;///////////////////////////////////////////////////////////////////////////////
                    }
                }
                //if (cwScanLoaded)
                //{
                //    cwScanLoader.Disconnect();
                //}
                #endregion
            }
            #region Exception
            catch (RohdeSchwarz.ViCom.Net.CViComError error)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = new Exception("ErrorCode:" + error.ErrorCode + " ErrorString:" + error.ErrorString), ClassName = "TSMxReceiver", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            catch (Exception exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "TSMxReceiver", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            #endregion
            finally { DM -= DetectScanner; }
        }

        #region GPS
        private void GpsConnect()
        {
            try
            {
                //ViComMessageTracerRegistry.Register(new MessageTracer());
                //gpsLoader = new CViComLoader<CViComGpsInterface>(DeviceType);
                //GPSIsRuning = gpsLoader.Connect(App.Sett.TSMxReceiver_Settings.IPAdress, out error, receiverListener); //GPSIsRuning


                //GpsInterface = gpsLoader.GetInterface();
                //GpsBasicInterface = gpsLoader.GetBasicInterface();

                var resbuf = new SResultBufferDepth();
                resbuf.dwValue = SResultBufferDepth.dwMax;
                GpsBasicInterface.SetResultBufferDepth(resbuf);

                var settings = new SGPSDeviceSettings();
                settings.enGnssMode = GnssMode.Type.GPS;// settings.enSatNavSystem = SatNavSystem.Type.GPS; //settings.enGnssMode = GnssMode.Type.GPS;// 
                settings.enGPSMessageFormat = GPSMessageFormat.Type.VICOM_GPS_FORMAT_NMEA;//.VICOM_GPS_FORMAT_UBLOX;
                settings.enResetMode = ResetMode.Type.NONE;
                settings.deadReckoningSettings.enState = SDeadReckoningSettings.State.Type.DISABLED;

                GpsInterface.SetGPSDeviceSettings(settings);

                GPSListener = new MyGpsDataProcessor();

                GpsInterface.RegisterResultDataListener(GPSListener);

                GpsBasicInterface.StartMeasurement();

            }
            #region Exception
            catch (RohdeSchwarz.ViCom.Net.CViComError error)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = new Exception("ErrorCode:" + error.ErrorCode + " ErrorString:" + error.ErrorString), ClassName = "TSMxReceiver", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            catch (Exception exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "TSMxReceiver", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            #endregion
            finally { DM -= GpsConnect; }
        }
        private void GpsDisconnect()
        {
            try
            {
                if (gpsLoader != null)
                {
                    if (gpsLoader.GetBasicInterface().IsMeasurementStarted())
                    {
                        gpsLoader.GetBasicInterface().StopMeasurement();
                        gpsLoader.GetBasicInterface().HasMeasurementStopped(SDefs.dwDefaultTimeOutInMs);
                    }
                    if (gpsLoader.GetBasicInterface() != null && gpsLoader.GetBasicInterface().HasMeasurementStopped(SDefs.dwDefaultTimeOutInMs))
                    {
                        GpsInterface.UnregisterResultDataListener(GPSListener);
                        GPSIsRuning = !gpsLoader.Disconnect();
                        gpsLoader.Dispose();
                        DM -= GpsDisconnect;
                    }
                }
            }
            #region Exception
            catch (RohdeSchwarz.ViCom.Net.CViComError error)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = new Exception("ErrorCode:" + error.ErrorCode + " ErrorString:" + error.ErrorString), ClassName = "TSMxReceiver", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
                DM -= GpsDisconnect;
            }
            catch (Exception exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "TSMxReceiver", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
                DM -= GpsDisconnect;
            }
            #endregion
            #region old
            //try
            //{
            //    if (gpsLoader != null && gpsLoader.GetBasicInterface().IsMeasurementStarted())
            //    {
            //        gpsLoader.GetBasicInterface().StopMeasurement();
            //        if (gpsLoader.GetBasicInterface().HasMeasurementStopped(SDefs.dwDefaultTimeOutInMs))
            //        {
            //            GpsInterface.UnregisterResultDataListener(GPSListener);

            //            GPSIsRuning = !gpsLoader.Disconnect();
            //            gpsLoader.Dispose();
            //            DM -= GpsDisconnect;
            //        }
            //    }
            //    else if (gpsLoader.GetBasicInterface() != null && gpsLoader.GetBasicInterface().HasMeasurementStopped(SDefs.dwDefaultTimeOutInMs))
            //    {
            //        GpsInterface.UnregisterResultDataListener(GPSListener);
            //        GPSIsRuning = !gpsLoader.Disconnect();
            //        gpsLoader.Dispose();
            //        DM -= GpsDisconnect;
            //    }
            //}
            //#region Exception
            //catch (RohdeSchwarz.ViCom.Net.CViComError error)
            //{
            //    MainWindow.exp.ExceptionData = new ExData() { ex = new Exception("ErrorCode:" + error.ErrorCode + " ErrorString:" + error.ErrorString), ClassName = "TSMxReceiver", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            //}
            //catch (Exception exp)
            //{
            //    MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "TSMxReceiver", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            //}
            //#endregion
            #endregion
        }
        #endregion GPS        

        #region GSM
        private void SetNewStateGSM()
        {
            DoubMod Connect = GsmConnect;
            DoubMod Disconnect = GsmDisconnect;
            bool findConnect = false;
            bool findDisconnect = false;
            if (DM != null && Connect != null && Disconnect != null)
            {
                foreach (Delegate d in DM.GetInvocationList())
                {
                    if (d.Method.Name == Connect.GetInvocationList()[0].Method.Name) findConnect = true;
                    if (d.Method.Name == Disconnect.GetInvocationList()[0].Method.Name) findDisconnect = true;
                }
            }
            if (App.Sett.TSMxReceiver_Settings.GSM.TechIsEnabled != GSMIsRuning)
            {
                if (App.Sett.TSMxReceiver_Settings.GSM.TechIsEnabled == true)
                {
                    if (findConnect == false && findDisconnect == false) DM += (DoubMod)Connect.GetInvocationList()[0];
                }
                else
                {
                    if (findConnect == false && findDisconnect == false) DM += (DoubMod)Disconnect.GetInvocationList()[0];
                }
            }
            else { DM -= SetNewStateGSM; }
        }
        private void GsmConnect()
        {
            try
            {
                gsmLoader = new CViComLoader<RohdeSchwarz.ViCom.Net.GSM.CViComGsmInterface>(DeviceType);
                GetUnifreqsGSM();
                bool Runing = gsmLoader.Connect(App.Sett.TSMxReceiver_Settings.IPAdress, out error, receiverListener);
                if (Runing)
                {
                    //if (error != null) { Errors += "\r\n" + "error " + error.ErrorCode + error.ErrorString; }
                    gsmInterface = gsmLoader.GetInterface();
                    gsmBasicInterface = gsmLoader.GetBasicInterface();

                    var buf = new SResultBufferDepth();
                    buf.dwValue = 1024;
                    gsmBasicInterface.SetResultBufferDepth(buf);

                    //RohdeSchwarz.ViCom.Net.SRange<uint> rateLimit = (RohdeSchwarz.ViCom.Net.SRange<uint>)gsmInterface.GetMeasRateLimits();

                    var channelSettings = new RohdeSchwarz.ViCom.Net.GSM.SChannelSettings();
                    channelSettings.dwFrontEndSelectionMask = GetDeviceRFInput(true, 1, "GSM"); //вроде как канал приемника 1/2
                    channelSettings.dwMeasRatePer1000Sec = 250000;// 50000;// ((RohdeSchwarz.ViCom.Net.SRange<uint>)gsmInterface.GetMeasRateLimits()).minimum; //rateLimit.maximum; //вроде как скорость сканирования
                    channelSettings.dwCount = (uint)GSMUniFreq.Count();
                    channelSettings.pTableOfFrequencySetting = new RohdeSchwarz.ViCom.Net.GSM.SFrequencySetting[channelSettings.dwCount];


                    for (int i = 0; i < GSMUniFreq.Count; i++)
                    {
                        channelSettings.pTableOfFrequencySetting[i] = new RohdeSchwarz.ViCom.Net.GSM.SFrequencySetting();
                        channelSettings.pTableOfFrequencySetting[i].dCenterFrequencyInHz = (double)GSMUniFreq[i];
                    }

                    gsmInterface.SetFrequencyTable(channelSettings);

                    SMeasurementDetails det = new SMeasurementDetails();
                    det.ChannelPowerSpec = new SChannelPowerSpec();
                    det.ChannelPowerSpec.wCountOfResultsPerGSMTimeSlot = 16;
                    det.ChannelPowerSpec.wRMSLengthIn40ns = 900;
                    //det.SpectrumSpec = new SSpectrumSpec();
                    //det.SpectrumSpec.eFreqDetector = SSpectrumSpec.FreqDetector.Type.PEAK;
                    //det.SpectrumSpec.eTimeDetector = SSpectrumSpec.TimeDetector.Type.PEAK;
                    //det.SpectrumSpec.wCollectionTimeIn100us = 50;
                    //det.SpectrumSpec.wCountOfPowerValuesPerChannel = 1;

                    det.pTableOfChannelMeasSpec = new SChannelMeasSpec[channelSettings.pTableOfFrequencySetting.Count()];
                    det.dwCount = (uint)channelSettings.pTableOfFrequencySetting.Count();

                    for (int i = 0; i < channelSettings.pTableOfFrequencySetting.Count(); i++)
                    {
                        det.pTableOfChannelMeasSpec[i] = new SChannelMeasSpec();
                        det.pTableOfChannelMeasSpec[i].dwFrequencyIndex = (uint)i;
                        det.pTableOfChannelMeasSpec[i].bMEAS_CARRIER_TO_INTERFERENCE = true;
                        det.pTableOfChannelMeasSpec[i].bMEAS_CHANNEL_POWER = true;
                        det.pTableOfChannelMeasSpec[i].bMEAS_DB_POWER = false;
                        det.pTableOfChannelMeasSpec[i].bMEAS_DB_REMOVAL = false;
                        det.pTableOfChannelMeasSpec[i].bMEAS_REPORT_FAILED_TRIALS = false;
                        det.pTableOfChannelMeasSpec[i].bMEAS_SCH = true;
                        det.pTableOfChannelMeasSpec[i].bMEAS_SPECTRUM = false;
                        det.pTableOfChannelMeasSpec[i].bMEAS_TSC = true;
                    }
                    gsmInterface.SetMeasurementDetails(det);


                    List<RohdeSchwarz.ViCom.Net.GSM.Pdu.Type> siblist = new List<RohdeSchwarz.ViCom.Net.GSM.Pdu.Type>() { };
                    for (int i = 0; i < App.Sett.TSMxReceiver_Settings.GSM.SITypes.Count; i++)
                    { if (App.Sett.TSMxReceiver_Settings.GSM.SITypes[i].Use) { siblist.Add((RohdeSchwarz.ViCom.Net.GSM.Pdu.Type)Enum.Parse(typeof(RohdeSchwarz.ViCom.Net.GSM.Pdu.Type), App.Sett.TSMxReceiver_Settings.GSM.SITypes[i].SiType)); } }

                    RohdeSchwarz.ViCom.Net.GSM.SDemodulationSettings demod = new RohdeSchwarz.ViCom.Net.GSM.SDemodulationSettings();
                    uint dwRequests = (uint)(siblist.Count * GSMUniFreq.Count());
                    demod.dwFrontEndSelectionMask = GetDeviceRFInput(true, 1, "GSM");
                    demod.lTotalPowerOffsetInDB10 = 100;

                    RohdeSchwarz.ViCom.Net.GSM.SDemodRequests MeasurementRequests = new RohdeSchwarz.ViCom.Net.GSM.SDemodRequests();
                    MeasurementRequests.dwCountOfRequests = dwRequests;
                    MeasurementRequests.pDemodRequests = new RohdeSchwarz.ViCom.Net.GSM.SDemodRequests.SDemodRequest[dwRequests];

                    for (int i = 0; i < GSMUniFreq.Count(); i++)
                    {
                        int dwRequestStartIndex = i * siblist.Count;
                        for (int idx = 0; idx < siblist.Count; ++idx)
                        {
                            int iR = dwRequestStartIndex + idx;
                            MeasurementRequests.pDemodRequests[iR] = new RohdeSchwarz.ViCom.Net.GSM.SDemodRequests.SDemodRequest();
                            MeasurementRequests.pDemodRequests[iR].dwChannelIndex = (uint)i;
                            MeasurementRequests.pDemodRequests[iR].ePDU = siblist[idx];
                            MeasurementRequests.pDemodRequests[iR].eDemodulationMode = RohdeSchwarz.ViCom.Net.GSM.DemodMode.Type.ONCE;
                            //MeasurementRequests.pDemodRequests[iR].wRepetitionDelayIn100ms = 0;//1000;
                        }
                    }
                    demod.sStartMeasurementRequests = MeasurementRequests;
                    gsmInterface.SetDemodulationSettings(demod);


                    GSMListener = new MyGsmDataProcessor();

                    gsmInterface.RegisterResultDataListener(GSMListener);

                    gsmBasicInterface.StartMeasurement();
                    GSMIsRuning = Runing;
                }
            }
            #region Exception
            catch (RohdeSchwarz.ViCom.Net.CViComError error)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = new Exception("ErrorCode:" + error.ErrorCode + " ErrorString:" + error.ErrorString), ClassName = "TSMxReceiver", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            catch (Exception exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "TSMxReceiver", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            #endregion
            finally { DM -= GsmConnect; }
        }
        private void GsmDisconnect()
        {
            try
            {
                if (gsmLoader != null)
                {
                    if (gsmLoader.GetBasicInterface().IsMeasurementStarted())
                    {
                        gsmLoader.GetBasicInterface().StopMeasurement();
                        gsmLoader.GetBasicInterface().HasMeasurementStopped(SDefs.dwDefaultTimeOutInMs);
                    }
                    if (gsmLoader.GetBasicInterface() != null && gsmLoader.GetBasicInterface().HasMeasurementStopped(SDefs.dwDefaultTimeOutInMs))
                    {
                        gsmInterface.UnregisterResultDataListener(GSMListener);
                        GSMIsRuning = !gsmLoader.Disconnect();
                        gsmLoader.Dispose();
                        DM -= GsmDisconnect;
                    }
                }
            }
            #region Exception
            catch (RohdeSchwarz.ViCom.Net.CViComError error)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = error, ClassName = "TSMxReceiver", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            catch (Exception exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "TSMxReceiver", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            #endregion
            #region old
            //try
            //{
            //    if (gsmLoader != null && gsmLoader.GetBasicInterface().IsMeasurementStarted())
            //    {
            //        gsmLoader.GetBasicInterface().StopMeasurement();
            //        if (gsmLoader.GetBasicInterface().HasMeasurementStopped(SDefs.dwDefaultTimeOutInMs))
            //        {
            //            gsmInterface.UnregisterResultDataListener(GSMListener);
            //            GSMIsRuning = !gsmLoader.Disconnect();
            //            gsmLoader.Dispose();
            //            DM -= GsmDisconnect;
            //        }
            //    }
            //    else if (gsmLoader.GetBasicInterface() != null && gsmLoader.GetBasicInterface().HasMeasurementStopped(SDefs.dwDefaultTimeOutInMs))
            //    {
            //        gsmInterface.UnregisterResultDataListener(GSMListener);
            //        GSMIsRuning = !gsmLoader.Disconnect();
            //        gsmLoader.Dispose();
            //        DM -= GsmDisconnect;
            //    }
            //}
            //#region Exception
            //catch (RohdeSchwarz.ViCom.Net.CViComError error)
            //{
            //    MainWindow.exp.ExceptionData = new ExData() { ex = error, ClassName = "TSMxReceiver", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            //}
            //catch (Exception exp)
            //{
            //    MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "TSMxReceiver", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            //}
            //#endregion
            #endregion
        }
        private void GetUnifreqsGSM()
        {
            GUIThreadDispatcher.Instance.Invoke(() =>
            {
                MainWindow.db_v2.GSMBandMeass.Clear();
            });

            #region GSM 
            List<decimal> freq1 = new List<decimal>() { };
            int ind = 0;
            foreach (Settings.GSMBand_Set t in App.Sett.TSMxReceiver_Settings.GSM.Bands)
            {
                if (t.Use == true)
                {
                    GSMBandMeas gbm = new GSMBandMeas() { };
                    gbm.id = ind;
                    ind++;
                    gbm.Start = t.FreqStart - 250000;
                    gbm.Stop = t.FreqStop + 250000;
                    gbm.Step = 10000;// 625;
                    gbm.TracePoints = (int)((gbm.Stop - gbm.Start) / gbm.Step) + 1;

                    GUIThreadDispatcher.Instance.Invoke(() =>
                    {
                        MainWindow.db_v2.GSMBandMeass.Add(gbm);
                    });

                    for (decimal i = t.FreqStart; i <= t.FreqStop; i += 200000)
                    { freq1.Add(i); }
                }
            }
            System.Collections.Generic.HashSet<decimal> hs1 = new System.Collections.Generic.HashSet<decimal>();
            foreach (decimal al in freq1)
            {
                hs1.Add(al);
            }
            GSMUniFreq.Clear();
            GSMUniFreq = new ObservableCollection<decimal>(hs1.OrderBy(i => i));
            foreach (GSMBandMeas b in MainWindow.db_v2.GSMBandMeass)
            {
                List<tracepoint> points = new List<tracepoint>() { };
                foreach (decimal d in GSMUniFreq)
                {
                    if (d > b.Start && d < b.Stop)
                    {
                        tracepoint tp = new tracepoint() { freq = d, level = -1000 };
                        points.Add(tp);
                    }
                }
                GUIThreadDispatcher.Instance.Invoke(() =>
                {
                    b.Trace = points.ToArray();
                });
            }
            #endregion
        }
        #endregion GSM

        #region UMTS
        private void SetNewStateUMTS()
        {
            DoubMod Connect = UmtsConnect;
            DoubMod Disconnect = UmtsDisconnect;
            bool findConnect = false;
            bool findDisconnect = false;
            if (DM != null && Connect != null && Disconnect != null)
            {
                foreach (Delegate d in DM.GetInvocationList())
                {
                    if (d.Method.Name == Connect.GetInvocationList()[0].Method.Name) findConnect = true;
                    if (d.Method.Name == Disconnect.GetInvocationList()[0].Method.Name) findDisconnect = true;
                }
            }
            if (App.Sett.TSMxReceiver_Settings.UMTS.TechIsEnabled != UMTSIsRuning)
            {
                if (App.Sett.TSMxReceiver_Settings.UMTS.TechIsEnabled == true)
                {
                    if (findConnect == false && findDisconnect == false) DM += (DoubMod)Connect.GetInvocationList()[0];
                }
                else
                {
                    if (findConnect == false && findDisconnect == false) DM += (DoubMod)Disconnect.GetInvocationList()[0];
                }
            }
            else { DM -= SetNewStateUMTS; }
        }
        private void UmtsConnect()
        {
            try
            {
                wcdmaLoader = new CViComLoader<RohdeSchwarz.ViCom.Net.WCDMA.CViComWcdmaInterface>(DeviceType);
                GetUnifreqsUMTS();
                ViComMessageTracerRegistry.Register(new MessageTracer());
                bool Runing = wcdmaLoader.Connect(App.Sett.TSMxReceiver_Settings.IPAdress, out error, receiverListener);

                wcdmaInterface = wcdmaLoader.GetInterface();

                wcdmaBasicInterface = wcdmaLoader.GetBasicInterface();

                var channelConfig = new RohdeSchwarz.ViCom.Net.WCDMA.SChannelSettings();
                channelConfig.dwFrontEndSelectionMask = GetDeviceRFInput(true, 1, "UMTS");
                channelConfig.eMeasurementMode = RohdeSchwarz.ViCom.Net.WCDMA.MeasurementMode.Type.HIGH_DYNAMIC;
                channelConfig.dwMeasRatePer1000Sec = 40000;// 5000;//((RohdeSchwarz.ViCom.Net.SRange<uint>)wcdmaInterface.GetMeasRateLimits()).defaultValue;//10000;//
                uint freqs = (uint)UMTSUniFreq.Count(); //UMTSUniFreq
                channelConfig.pTableOfFrequencySetting = new RohdeSchwarz.ViCom.Net.WCDMA.SFrequencySetting[freqs];
                channelConfig.dwCount = (uint)channelConfig.pTableOfFrequencySetting.Length;
                for (int i = 0; i < freqs; i++)
                {
                    channelConfig.pTableOfFrequencySetting[i] = new RohdeSchwarz.ViCom.Net.WCDMA.SFrequencySetting();
                    channelConfig.pTableOfFrequencySetting[i].dCenterFrequencyInHz = (double)(UMTSUniFreq[i]);
                }
                wcdmaInterface.SetFrequencyTable(channelConfig);


                List<RohdeSchwarz.ViCom.Net.WCDMA.Pdu.Type> siblist = new List<RohdeSchwarz.ViCom.Net.WCDMA.Pdu.Type>() { };
                for (int i = 0; i < App.Sett.TSMxReceiver_Settings.UMTS.SIBTypes.Count; i++)
                { if (App.Sett.TSMxReceiver_Settings.UMTS.SIBTypes[i].Use) { siblist.Add((RohdeSchwarz.ViCom.Net.WCDMA.Pdu.Type)Enum.Parse(typeof(RohdeSchwarz.ViCom.Net.WCDMA.Pdu.Type), App.Sett.TSMxReceiver_Settings.UMTS.SIBTypes[i].SibType)); } }


                RohdeSchwarz.ViCom.Net.WCDMA.SDemodulationSettings demod = new RohdeSchwarz.ViCom.Net.WCDMA.SDemodulationSettings();
                demod.dwFrontEndSelectionMask = GetDeviceRFInput(true, 1, "UMTS");
                demod.lEcToIoThresholdInDB100 = -1500;// -1500;
                demod.dwMaxNodeBHoldTimeInSec = RohdeSchwarz.ViCom.Net.WCDMA.SDemodulationSettings.dwMinMaxNodeBHoldTimeInSec;

                RohdeSchwarz.ViCom.Net.WCDMA.SDemodRequests MeasurementRequests = new RohdeSchwarz.ViCom.Net.WCDMA.SDemodRequests();


                uint dwRequests = (uint)siblist.Count * freqs;
                MeasurementRequests.dwCountOfRequests = dwRequests;

                MeasurementRequests.pDemodRequests = new RohdeSchwarz.ViCom.Net.WCDMA.SDemodRequests.SDemodRequest[dwRequests];

                for (int i = 0; i < freqs; i++)
                {
                    int dwRequestStartIndex = i * siblist.Count;
                    for (int idx = 0; idx < siblist.Count; ++idx)
                    {
                        int iR = dwRequestStartIndex + idx;
                        MeasurementRequests.pDemodRequests[iR] = new RohdeSchwarz.ViCom.Net.WCDMA.SDemodRequests.SDemodRequest();
                        MeasurementRequests.pDemodRequests[iR].dwChannelIndex = (uint)i;
                        MeasurementRequests.pDemodRequests[iR].ePDU = siblist[idx];
                        MeasurementRequests.pDemodRequests[iR].eDemodulationMode = RohdeSchwarz.ViCom.Net.WCDMA.DemodMode.Type.ONCE;
                        MeasurementRequests.pDemodRequests[iR].wRepetitionDelayIn100ms = 0;//1000;
                    }
                }
                demod.sStartMeasurementRequests = MeasurementRequests;
                wcdmaInterface.SetDemodulationSettings(demod);
                WCDMAListener = new MyWcdmaDataProcessor();
                wcdmaInterface.RegisterResultDataListener(WCDMAListener);
                wcdmaBasicInterface.StartMeasurement();
                UMTSIsRuning = Runing;
            }
            #region Exception
            catch (RohdeSchwarz.ViCom.Net.CViComError error)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = error, ClassName = "TSMxReceiver", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            catch (Exception exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "TSMxReceiver", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            #endregion
            finally { DM -= UmtsConnect; }
        }
        private void UmtsDisconnect()
        {
            try
            {
                if (wcdmaLoader != null)
                {
                    if (wcdmaLoader.GetBasicInterface().IsMeasurementStarted())
                    {
                        wcdmaLoader.GetBasicInterface().StopMeasurement();
                        wcdmaLoader.GetBasicInterface().HasMeasurementStopped(SDefs.dwDefaultTimeOutInMs);
                    }
                    if (wcdmaLoader.GetBasicInterface() != null && wcdmaLoader.GetBasicInterface().HasMeasurementStopped(SDefs.dwDefaultTimeOutInMs))
                    {
                        wcdmaInterface.UnregisterResultDataListener(WCDMAListener);
                        UMTSIsRuning = !wcdmaLoader.Disconnect();
                        wcdmaLoader.Dispose();
                        DM -= UmtsDisconnect;
                    }
                }
            }
            #region Exception
            catch (RohdeSchwarz.ViCom.Net.CViComError error)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = error, ClassName = "TSMxReceiver", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            catch (Exception exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "TSMxReceiver", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            #endregion
            #region old
            //try
            //{
            //    if (wcdmaLoader != null && wcdmaLoader.GetBasicInterface().IsMeasurementStarted())
            //    {
            //        //wcdmaBasicInterface.StopMeasurement();
            //        //wcdmaBasicInterface.HasMeasurementStopped(SDefs.dwDefaultTimeOutInMs);

            //        //wcdmaInterface.UnregisterResultDataListener(WCDMAListener);

            //        //UMTSIsRuning = !wcdmaLoader.Disconnect();
            //        wcdmaLoader.GetBasicInterface().StopMeasurement();
            //        if (wcdmaLoader.GetBasicInterface().HasMeasurementStopped(SDefs.dwDefaultTimeOutInMs))
            //        {
            //            wcdmaInterface.UnregisterResultDataListener(WCDMAListener);
            //            UMTSIsRuning = !wcdmaLoader.Disconnect();
            //            wcdmaLoader.Dispose();
            //            DM -= UmtsDisconnect;
            //        }
            //    }
            //    else if (wcdmaLoader.GetBasicInterface() != null && wcdmaLoader.GetBasicInterface().HasMeasurementStopped(SDefs.dwDefaultTimeOutInMs))
            //    {
            //        wcdmaInterface.UnregisterResultDataListener(WCDMAListener);
            //        UMTSIsRuning = !wcdmaLoader.Disconnect();
            //        wcdmaLoader.Dispose();
            //        DM -= UmtsDisconnect;
            //    }
            //}
            //#region Exception
            //catch (RohdeSchwarz.ViCom.Net.CViComError error)
            //{
            //    MainWindow.exp.ExceptionData = new ExData() { ex = error, ClassName = "TSMxReceiver", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            //}
            //catch (Exception exp)
            //{
            //    MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "TSMxReceiver", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            //}
            //#endregion
            #endregion
        }
        private static void UMTSSetFreqFromACD()
        {
            try
            {
                if (wcdmaLoader != null && wcdmaLoader.GetBasicInterface().IsMeasurementStarted())
                {
                    wcdmaLoader.GetBasicInterface().StopMeasurement();
                }
                if (wcdmaLoader.GetBasicInterface().HasMeasurementStopped(SDefs.dwDefaultTimeOutInMs))
                {
                    var channelConfig = wcdmaInterface.GetSettings().ChannelSettings;
                    channelConfig.dwFrontEndSelectionMask = GetDeviceRFInput(true, 1, "UMTS");
                    channelConfig.eMeasurementMode = RohdeSchwarz.ViCom.Net.WCDMA.MeasurementMode.Type.HIGH_DYNAMIC;
                    channelConfig.dwMeasRatePer1000Sec = 40000;// 5000;//((RohdeSchwarz.ViCom.Net.SRange<uint>)wcdmaInterface.GetMeasRateLimits()).defaultValue;//10000;//

                    #region Freqs 
                    List<decimal> freq = new List<decimal>() { };
                    foreach (decimal t in UMTSUniFreq)
                    {
                        freq.Add(t);
                    }
                    for (int i = 0; i < IdentificationData.ACD.ACDData.Count(); i++)
                    {
                        if (IdentificationData.ACD.ACDData[i].Tech == 2 && IdentificationData.ACD.ACDData[i].Established == false)
                        {
                            freq.Add(IdentificationData.ACD.ACDData[i].Freq);
                            IdentificationData.ACD.ACDData[i].Established = true;
                        }
                    }
                    System.Collections.Generic.HashSet<decimal> hs2 = new System.Collections.Generic.HashSet<decimal>();
                    foreach (decimal al in freq)
                    {
                        hs2.Add(al);
                    }
                    UMTSUniFreq.Clear();
                    UMTSUniFreq = new ObservableCollection<decimal>(hs2);
                    #endregion

                    uint freqs = (uint)UMTSUniFreq.Count(); //UMTSUniFreq
                    channelConfig.pTableOfFrequencySetting = new RohdeSchwarz.ViCom.Net.WCDMA.SFrequencySetting[freqs];
                    channelConfig.dwCount = (uint)channelConfig.pTableOfFrequencySetting.Length;
                    for (int i = 0; i < freqs; i++)
                    {
                        channelConfig.pTableOfFrequencySetting[i] = new RohdeSchwarz.ViCom.Net.WCDMA.SFrequencySetting();
                        channelConfig.pTableOfFrequencySetting[i].dCenterFrequencyInHz = (double)(UMTSUniFreq[i]);
                    }
                    wcdmaInterface.SetFrequencyTable(channelConfig);


                    List<RohdeSchwarz.ViCom.Net.WCDMA.Pdu.Type> siblist = new List<RohdeSchwarz.ViCom.Net.WCDMA.Pdu.Type>() { };
                    for (int i = 0; i < App.Sett.TSMxReceiver_Settings.UMTS.SIBTypes.Count; i++)
                    { if (App.Sett.TSMxReceiver_Settings.UMTS.SIBTypes[i].Use) { siblist.Add((RohdeSchwarz.ViCom.Net.WCDMA.Pdu.Type)Enum.Parse(typeof(RohdeSchwarz.ViCom.Net.WCDMA.Pdu.Type), App.Sett.TSMxReceiver_Settings.UMTS.SIBTypes[i].SibType)); } }


                    RohdeSchwarz.ViCom.Net.WCDMA.SDemodulationSettings demod = wcdmaInterface.GetSettings().DemodulationSettings;
                    demod.dwFrontEndSelectionMask = GetDeviceRFInput(true, 1, "UMTS");
                    demod.lEcToIoThresholdInDB100 = -1500;
                    demod.dwMaxNodeBHoldTimeInSec = RohdeSchwarz.ViCom.Net.WCDMA.SDemodulationSettings.dwMinMaxNodeBHoldTimeInSec;

                    RohdeSchwarz.ViCom.Net.WCDMA.SDemodRequests MeasurementRequests = new RohdeSchwarz.ViCom.Net.WCDMA.SDemodRequests();
                    //demod.sStartMeasurementRequests = new RohdeSchwarz.ViCom.Net.WCDMA.SDemodRequests();

                    uint dwRequests = (uint)siblist.Count * freqs;
                    MeasurementRequests.dwCountOfRequests = dwRequests;

                    MeasurementRequests.pDemodRequests = new RohdeSchwarz.ViCom.Net.WCDMA.SDemodRequests.SDemodRequest[dwRequests];
                    //for (int i = 0; i < dwRequests; i++)
                    //{

                    //}
                    for (int i = 0; i < freqs; i++)
                    {
                        int dwRequestStartIndex = i * siblist.Count;
                        for (int idx = 0; idx < siblist.Count; ++idx)
                        {
                            int iR = dwRequestStartIndex + idx;
                            MeasurementRequests.pDemodRequests[iR] = new RohdeSchwarz.ViCom.Net.WCDMA.SDemodRequests.SDemodRequest();
                            MeasurementRequests.pDemodRequests[iR].dwChannelIndex = (uint)i;
                            MeasurementRequests.pDemodRequests[iR].ePDU = siblist[idx];
                            MeasurementRequests.pDemodRequests[iR].eDemodulationMode = RohdeSchwarz.ViCom.Net.WCDMA.DemodMode.Type.ONCE;
                            MeasurementRequests.pDemodRequests[iR].wRepetitionDelayIn100ms = 0;//1000;
                        }
                    }
                    demod.sStartMeasurementRequests = MeasurementRequests;
                    wcdmaInterface.SetDemodulationSettings(demod);

                    wcdmaBasicInterface.StartMeasurement();
                    DM -= UMTSSetFreqFromACD;
                }





            }
            #region Exception
            catch (RohdeSchwarz.ViCom.Net.CViComError error)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = error, ClassName = "TSMxReceiver", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            catch (Exception exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "TSMxReceiver", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            #endregion

        }
        private void GetUnifreqsUMTS()
        {
            #region UMTS 
            List<decimal> freq2 = new List<decimal>() { };
            foreach (Settings.UMTSFreqs_Set t in App.Sett.TSMxReceiver_Settings.UMTS.Freqs)
            {
                if (t.Use == true)
                { freq2.Add(t.FreqDn); }
            }
            System.Collections.Generic.HashSet<decimal> hs2 = new System.Collections.Generic.HashSet<decimal>();
            foreach (decimal al in freq2)
            {
                hs2.Add(al);
            }
            UMTSUniFreq.Clear();
            UMTSUniFreq = new ObservableCollection<decimal>(hs2.OrderBy(i => i));
            #endregion
        }
        #endregion

        #region LTE
        private void SetNewStateLTE()
        {
            DoubMod Connect = LTEConnect;
            DoubMod Disconnect = LTEDisconnect;
            bool findConnect = false;
            bool findDisconnect = false;
            if (DM != null && Connect != null && Disconnect != null)
            {
                foreach (Delegate d in DM.GetInvocationList())
                {
                    if (d.Method.Name == Connect.GetInvocationList()[0].Method.Name) findConnect = true;
                    if (d.Method.Name == Disconnect.GetInvocationList()[0].Method.Name) findDisconnect = true;
                }
            }
            if (App.Sett.TSMxReceiver_Settings.LTE.TechIsEnabled != LTEIsRuning)
            {
                if (App.Sett.TSMxReceiver_Settings.LTE.TechIsEnabled == true)
                {
                    if (findConnect == false && findDisconnect == false) DM += (DoubMod)Connect.GetInvocationList()[0];
                }
                else
                {
                    if (findConnect == false && findDisconnect == false) DM += (DoubMod)Disconnect.GetInvocationList()[0];
                }
            }
            else { DM -= SetNewStateLTE; }
        }
        private void LTEConnect()
        {
            try
            {
                lteLoader = new CViComLoader<RohdeSchwarz.ViCom.Net.LTE.CViComLteInterface>(DeviceType);
                LTEIsRuning = lteLoader.Connect(App.Sett.TSMxReceiver_Settings.IPAdress, out error, receiverListener);
                GetUnifreqsLTE();
                lteInterface = lteLoader.GetInterface();
                lteBasicInterface = lteLoader.GetBasicInterface();

                var resultBufferDepth = new SResultBufferDepth();
                resultBufferDepth.dwValue = 1024;
                lteBasicInterface.SetResultBufferDepth(resultBufferDepth);

                var channelSettings = new RohdeSchwarz.ViCom.Net.LTE.SChannelSettings();

                // Setup general DLAA operation
                //int[] dwarObservationInterval = new[] { 20, 60 }; // { 20, 60, 300, 600, 900 };                   
                //channelSettings.dlaaSettings.dwObservationCount = (uint)dwarObservationInterval.Length;
                //channelSettings.dlaaSettings.pTableOfObservationSettings
                //   = new RohdeSchwarz.ViCom.Net.LTE.SChannelSettings.SDlaaSettings.SObservationSettings[channelSettings.dlaaSettings.dwObservationCount];
                //for (uint dwI = 0; dwI < channelSettings.dlaaSettings.dwObservationCount; dwI++)
                //{
                //   channelSettings.dlaaSettings.pTableOfObservationSettings[dwI] = new RohdeSchwarz.ViCom.Net.LTE.SChannelSettings.SDlaaSettings.SObservationSettings();
                //   channelSettings.dlaaSettings.pTableOfObservationSettings[dwI].dwObservationIntervalInS = (uint)dwarObservationInterval[dwI];
                //}

                //
                // Setup TDD interference analysis
                //
                channelSettings.bTddInterferenceKpiThresholdInPct = 0;

                //
                // Setup Frequency table interference analysis
                //

                // Front End Selection Mask
                //uint dwFE_Mask = (uint)RohdeSchwarz.ViCom.Net.SRFPort.Type.RF_1;
                uint freqs = (uint)LTEUniFreq.Count(); //UMTSUniFreq
                channelSettings.pTableOfFrequencySetting = new RohdeSchwarz.ViCom.Net.LTE.SFrequencySetting[freqs];
                channelSettings.dwCount = freqs;

                // Measurement mask for NB Reference Signals
                int iNarrowbandRefSignalMeasMode = (RohdeSchwarz.ViCom.Net.LTE.SFrequencySetting.wNARROWBAND_RSRP_RSRQ |
                                                     RohdeSchwarz.ViCom.Net.LTE.SFrequencySetting.wCENTER_RSCINR_1x1080KHZ |
                                                     RohdeSchwarz.ViCom.Net.LTE.SFrequencySetting.w1MHZ_FILTER_NOISE_FOR_15RB);
                for (int i = 0; i < freqs; i++)
                {
                    #region freq set
                    channelSettings.pTableOfFrequencySetting[i] = new RohdeSchwarz.ViCom.Net.LTE.SFrequencySetting();
                    channelSettings.pTableOfFrequencySetting[i].dwFrontEndSelectionMask = GetDeviceRFInput(true, 1, "LTE");
                    channelSettings.pTableOfFrequencySetting[i].dCenterFrequencyInHz = (double)LTEUniFreq[i];
                    channelSettings.pTableOfFrequencySetting[i].dwSymbolsPerSlotMask = RohdeSchwarz.ViCom.Net.LTE.SFrequencySetting.dwDefaultSymbolsPerSlot;
                    channelSettings.pTableOfFrequencySetting[i].enFrameStructureType = FrameStructureType.Type.FDD;
                    channelSettings.pTableOfFrequencySetting[i].bUpDownLinkMask = RohdeSchwarz.ViCom.Net.LTE.SFrequencySetting.bAllUpDownLinkConfigurations;
                    channelSettings.pTableOfFrequencySetting[i].wSpecialSubframe1ConfigurationMask = RohdeSchwarz.ViCom.Net.LTE.SFrequencySetting.wAllSpecialSubFrameConfigurations;
                    channelSettings.pTableOfFrequencySetting[i].wSpecialSubframe6ConfigurationMask = RohdeSchwarz.ViCom.Net.LTE.SFrequencySetting.wAllSpecialSubFrameConfigurations;
                    channelSettings.pTableOfFrequencySetting[i].dwAvgBlockCountPer1000Sec = 2000;// 300;//((RohdeSchwarz.ViCom.Net.SRange<uint>)lteInterface.GetMeasRateLimits()).minimum;////////////////////////;
                    channelSettings.pTableOfFrequencySetting[i].enSSyncToPSyncRatioType = RohdeSchwarz.ViCom.Net.LTE.SFrequencySetting.SSyncToPSyncRatioType.Type.RatioRange;
                    channelSettings.pTableOfFrequencySetting[i].SSyncToPSyncRatio = new RohdeSchwarz.ViCom.Net.LTE.SFrequencySetting.SSyncToPSyncRatioSettings();
                    channelSettings.pTableOfFrequencySetting[i].SSyncToPSyncRatio.RatioRange = new RohdeSchwarz.ViCom.Net.LTE.SFrequencySetting.SSyncToPSyncRatioSettings._StructRatioRange();
                    channelSettings.pTableOfFrequencySetting[i].SSyncToPSyncRatio.RatioRange.fLowerRatioInDB = -5;
                    channelSettings.pTableOfFrequencySetting[i].SSyncToPSyncRatio.RatioRange.fUpperRatioInDB = 0;
                    channelSettings.pTableOfFrequencySetting[i].wNarrowbandRefSignalMeasMode = (ushort)iNarrowbandRefSignalMeasMode;
                    channelSettings.pTableOfFrequencySetting[i].enBandwidthCtrlMode = RohdeSchwarz.ViCom.Net.LTE.SFrequencySetting.BandwidthCtrlMode.Type.BW_FROM_MIB_ONCE_EACH_CELL;
                    //channelSettings.pTableOfFrequencySetting[i].wNumberOfResourceBlocks = 50;
                    channelSettings.pTableOfFrequencySetting[i].WidebandRsCinrSettings = new RohdeSchwarz.ViCom.Net.LTE.SFrequencySetting.SWidebandRsCinrSettings();
                    channelSettings.pTableOfFrequencySetting[i].WidebandRsCinrSettings.wWidebandRsCinrMeasMode = RohdeSchwarz.ViCom.Net.LTE.SFrequencySetting.SWidebandRsCinrSettings.wWIDEBAND_RS_CINR;
                    channelSettings.pTableOfFrequencySetting[i].WidebandRsCinrSettings.dwAvgBlockCountPer1000Sec = 1000;// 300;//((RohdeSchwarz.ViCom.Net.SRange<uint>)lteInterface.GetWbMeasRateLimits()).minimum;////////////////////////;
                    channelSettings.pTableOfFrequencySetting[i].WidebandRsCinrSettings.wNumberOfRBsInSubband = RohdeSchwarz.ViCom.Net.LTE.SFrequencySetting.SWidebandRsCinrSettings.wMinRBsInSubband;
                    channelSettings.pTableOfFrequencySetting[i].WidebandRsCinrSettings.dwFrontEndSelectionMask = GetDeviceRFInput(true, 1, "LTE");
                    channelSettings.pTableOfFrequencySetting[i].WidebandRsCinrSettings.bForceNoGap = true;
                    channelSettings.pTableOfFrequencySetting[i].WidebandRsCinrSettings.bMaxCountOfeNodeBs = 6;
                    channelSettings.pTableOfFrequencySetting[i].WidebandRsCinrSettings.bTransmitAntennaSelectionMask = 15;
                    channelSettings.pTableOfFrequencySetting[i].WidebandRsCinrSettings.sMinCenterRsrpInDBm100 = -13000;
                    channelSettings.pTableOfFrequencySetting[i].WidebandRsCinrSettings.wMaxRsrpDiffToBestCellInDB100 = 500;
                    channelSettings.pTableOfFrequencySetting[i].enMbmsConfigCtrlMode = RohdeSchwarz.ViCom.Net.LTE.SFrequencySetting.MbmsConfigCtrlMode.Type.MBMS_NOT_PRESENT;
                    channelSettings.pTableOfFrequencySetting[i].MimoSettings = new RohdeSchwarz.ViCom.Net.LTE.SFrequencySetting.SMimoSettings();
                    channelSettings.pTableOfFrequencySetting[i].MimoSettings.wMimoMeasMode = 0;
                    channelSettings.pTableOfFrequencySetting[i].MimoSettings.dwMimoResultMaskFor2x2 = 7;
                    channelSettings.pTableOfFrequencySetting[i].MimoSettings.dwMimoResultMaskFor2x4 = 0;
                    channelSettings.pTableOfFrequencySetting[i].MimoSettings.bMaxCountOfeNodeBs = 5;
                    channelSettings.pTableOfFrequencySetting[i].MimoSettings.sCinrThresholdForRankInDB100 = 0;
                    channelSettings.pTableOfFrequencySetting[i].MimoSettings.sMinCenterRsrpInDBm100 = -13000;
                    channelSettings.pTableOfFrequencySetting[i].MimoSettings.sMinRsCinrInDB100 = -1000;
                    channelSettings.pTableOfFrequencySetting[i].MimoSettings.wMaxRsrpDiffToBestCellInDB100 = 500;
                    channelSettings.pTableOfFrequencySetting[i].MimoSettings.wTimeResolutionInMs = 10;
                    // Setup Throughput estimation           
                    channelSettings.pTableOfFrequencySetting[i].MimoSettings.bEnableThroughputEstimation = false;
                    channelSettings.pTableOfFrequencySetting[i].RssiSettings = new RohdeSchwarz.ViCom.Net.LTE.SFrequencySetting.SRssiSettings();
                    channelSettings.pTableOfFrequencySetting[i].RssiSettings.dwFrontEndSelectionMask = GetDeviceRFInput(true, 1, "LTE");
                    channelSettings.pTableOfFrequencySetting[i].RssiSettings.wRssiMeasMode = 0;
                    channelSettings.pTableOfFrequencySetting[i].MbmsSettings.wMbmsMeasMode = 0;
                    #endregion
                }
                channelSettings.dwCount = freqs;

                channelSettings.RsCinrChannelModel = new RohdeSchwarz.ViCom.Net.LTE.SChannelSettings.SRsCinrChannelModel();
                channelSettings.RsCinrChannelModel.dwDelaySpreadInNs = RohdeSchwarz.ViCom.Net.LTE.SChannelSettings.SRsCinrChannelModel.dwMinDelaySpreadInNs;
                channelSettings.RsCinrChannelModel.dwSpeedInKmPerHour = RohdeSchwarz.ViCom.Net.LTE.SChannelSettings.SRsCinrChannelModel.dwMinSpeedInKmh;

                channelSettings.MbmsRsCinrChannelModel.dwDelaySpreadInNs = RohdeSchwarz.ViCom.Net.LTE.SChannelSettings.SMbmsRsCinrChannelModel.dwMinDelaySpreadInNs;
                channelSettings.MbmsRsCinrChannelModel.dwSpeedInKmPerHour = RohdeSchwarz.ViCom.Net.LTE.SChannelSettings.SMbmsRsCinrChannelModel.dwMinSpeedInKmh;

                List<RohdeSchwarz.ViCom.Net.LTE.Pdu.Type> siblist = new List<RohdeSchwarz.ViCom.Net.LTE.Pdu.Type>() { };
                for (int i = 0; i < App.Sett.TSMxReceiver_Settings.LTE.SIBTypes.Count; i++)
                { if (App.Sett.TSMxReceiver_Settings.LTE.SIBTypes[i].Use) { siblist.Add((RohdeSchwarz.ViCom.Net.LTE.Pdu.Type)Enum.Parse(typeof(RohdeSchwarz.ViCom.Net.LTE.Pdu.Type), App.Sett.TSMxReceiver_Settings.LTE.SIBTypes[i].SibType)); } }

                // Activate BCH demodulation
                uint dwRequests = (uint)siblist.Count * channelSettings.dwCount;
                RohdeSchwarz.ViCom.Net.LTE.SDemodulationSettings bchSettings = new RohdeSchwarz.ViCom.Net.LTE.SDemodulationSettings();
                bchSettings.sSINRThresholdDB100 = 100;// ((RohdeSchwarz.ViCom.Net.SRange<short>)lteInterface.GetDemodThresholdLimits()).minimum;
                bchSettings.dwFrontEndSelectionMask = GetDeviceRFInput(true, 1, "LTE"); //dwFE_Mask;

                bchSettings.sStartMeasurementRequests.dwCountOfRequests = dwRequests;
                bchSettings.sStartMeasurementRequests.pDemodRequests = new RohdeSchwarz.ViCom.Net.LTE.SDemodRequests.SDemodRequest[dwRequests];
                for (int i = 0; i < dwRequests; i++)
                {
                    bchSettings.sStartMeasurementRequests.pDemodRequests[i] = new RohdeSchwarz.ViCom.Net.LTE.SDemodRequests.SDemodRequest();
                }
                for (int i = 0; i < freqs; i++)
                {
                    int dwRequestStartIndex = i * siblist.Count;
                    for (int idx = 0; idx < siblist.Count; ++idx)
                    {
                        int iR = dwRequestStartIndex + idx;
                        bchSettings.sStartMeasurementRequests.pDemodRequests[iR].dwChannelIndex = (uint)i;
                        bchSettings.sStartMeasurementRequests.pDemodRequests[iR].ePDU = siblist[idx];
                        bchSettings.sStartMeasurementRequests.pDemodRequests[iR].eDemodulationMode = RohdeSchwarz.ViCom.Net.LTE.DemodMode.Type.ONCE;
                        bchSettings.sStartMeasurementRequests.pDemodRequests[iR].wRepetitionTimeOutInMs = 0;// 100;
                                                                                                            //bchSettings.sStartMeasurementRequests.pDemodRequests[iR].dwBtsId = 0;
                    }
                }

                lteInterface.SetFrequencyTable(channelSettings);

                lteInterface.SetDemodulationSettings(bchSettings);

                LteListener = new MyLteDataProcessor();

                lteInterface.RegisterResultDataListener(LteListener);

                lteBasicInterface.StartMeasurement();
            }
            catch (RohdeSchwarz.ViCom.Net.CViComError error)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = new Exception("ErrorCode:" + error.ErrorCode + " ErrorString:" + error.ErrorString), ClassName = "TSMxReceiver", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            catch (Exception exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "TSMxReceiver", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            finally { DM -= LTEConnect; }
        }
        private void LTEDisconnect()
        {
            try
            {
                if (lteLoader != null)
                {
                    if (lteLoader.GetBasicInterface().IsMeasurementStarted())
                    {
                        lteLoader.GetBasicInterface().StopMeasurement();
                        lteLoader.GetBasicInterface().HasMeasurementStopped(SDefs.dwDefaultTimeOutInMs);
                    }
                    if (lteLoader.GetBasicInterface() != null && lteLoader.GetBasicInterface().HasMeasurementStopped(SDefs.dwDefaultTimeOutInMs))
                    {
                        lteInterface.UnregisterResultDataListener(LteListener);
                        LTEIsRuning = !lteLoader.Disconnect();
                        lteLoader.Dispose();
                        DM -= LTEDisconnect;
                    }
                }
            }
            #region Exception
            catch (RohdeSchwarz.ViCom.Net.CViComError error)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = error, ClassName = "TSMxReceiver", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            catch (Exception exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "TSMxReceiver", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            #endregion
            #region old
            //try
            //{
            //    if (lteLoader != null && lteLoader.GetBasicInterface().IsMeasurementStarted())
            //    {
            //        lteLoader.GetBasicInterface().StopMeasurement();
            //        if (lteLoader.GetBasicInterface().HasMeasurementStopped(SDefs.dwDefaultTimeOutInMs))
            //        {
            //            lteInterface.UnregisterResultDataListener(LteListener);
            //            LTEIsRuning = !lteLoader.Disconnect();
            //            lteLoader.Dispose();
            //            DM -= LTEDisconnect;
            //        }
            //    }
            //    else if (lteLoader.GetBasicInterface() != null && lteLoader.GetBasicInterface().HasMeasurementStopped(SDefs.dwDefaultTimeOutInMs))
            //    {
            //        lteInterface.UnregisterResultDataListener(LteListener);
            //        LTEIsRuning = !lteLoader.Disconnect();
            //        lteLoader.Dispose();
            //        DM -= LTEDisconnect;
            //    }
            //}
            //#region Exception
            //catch (RohdeSchwarz.ViCom.Net.CViComError error)
            //{
            //    MainWindow.exp.ExceptionData = new ExData() { ex = error, ClassName = "TSMxReceiver", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            //}
            //catch (Exception exp)
            //{
            //    MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "TSMxReceiver", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            //}
            //#endregion
            #endregion
        }
        private static void LTESetFreqFromACD()
        {
            try
            {
                //IdentificationData.ACD.ACDData
                if (lteLoader != null && lteLoader.GetBasicInterface().IsMeasurementStarted())
                {
                    lteLoader.GetBasicInterface().StopMeasurement();
                }
                if (lteLoader.GetBasicInterface().HasMeasurementStopped(SDefs.dwDefaultTimeOutInMs))
                {
                    var ChannelSettings = lteInterface.GetSettings().ChannelSettings;
                    ChannelSettings.bTddInterferenceKpiThresholdInPct = 0;

                    #region Freqs 
                    List<decimal> freq = new List<decimal>() { };
                    foreach (decimal t in LTEUniFreq)
                    {
                        freq.Add(t);
                    }
                    for (int i = 0; i < IdentificationData.ACD.ACDData.Count(); i++)
                    {
                        if (IdentificationData.ACD.ACDData[i].Tech == 5 && IdentificationData.ACD.ACDData[i].Established == false)
                        {
                            freq.Add(IdentificationData.ACD.ACDData[i].Freq);
                            IdentificationData.ACD.ACDData[i].Established = true;
                        }
                    }
                    System.Collections.Generic.HashSet<decimal> hs2 = new System.Collections.Generic.HashSet<decimal>();
                    foreach (decimal al in freq)
                    {
                        hs2.Add(al);
                    }
                    LTEUniFreq.Clear();
                    LTEUniFreq = new ObservableCollection<decimal>(hs2);
                    #endregion

                    uint freqs = (uint)LTEUniFreq.Count(); //UMTSUniFreq
                    ChannelSettings.pTableOfFrequencySetting = new RohdeSchwarz.ViCom.Net.LTE.SFrequencySetting[freqs];
                    ChannelSettings.dwCount = (uint)ChannelSettings.pTableOfFrequencySetting.Length;
                    int iNarrowbandRefSignalMeasMode = (RohdeSchwarz.ViCom.Net.LTE.SFrequencySetting.wNARROWBAND_RSRP_RSRQ |
                                                         RohdeSchwarz.ViCom.Net.LTE.SFrequencySetting.wCENTER_RSCINR_1x1080KHZ |
                                                         RohdeSchwarz.ViCom.Net.LTE.SFrequencySetting.w1MHZ_FILTER_NOISE_FOR_15RB);

                    for (int i = 0; i < freqs; i++)
                    {
                        #region freq set
                        ChannelSettings.pTableOfFrequencySetting[i] = new RohdeSchwarz.ViCom.Net.LTE.SFrequencySetting();
                        ChannelSettings.pTableOfFrequencySetting[i].dwFrontEndSelectionMask = GetDeviceRFInput(true, 1, "LTE");
                        ChannelSettings.pTableOfFrequencySetting[i].dCenterFrequencyInHz = (double)LTEUniFreq[i];
                        ChannelSettings.pTableOfFrequencySetting[i].dwSymbolsPerSlotMask = RohdeSchwarz.ViCom.Net.LTE.SFrequencySetting.dwDefaultSymbolsPerSlot;
                        ChannelSettings.pTableOfFrequencySetting[i].enFrameStructureType = FrameStructureType.Type.FDD;
                        ChannelSettings.pTableOfFrequencySetting[i].bUpDownLinkMask = RohdeSchwarz.ViCom.Net.LTE.SFrequencySetting.bAllUpDownLinkConfigurations;
                        ChannelSettings.pTableOfFrequencySetting[i].wSpecialSubframe1ConfigurationMask = RohdeSchwarz.ViCom.Net.LTE.SFrequencySetting.wAllSpecialSubFrameConfigurations;
                        ChannelSettings.pTableOfFrequencySetting[i].wSpecialSubframe6ConfigurationMask = RohdeSchwarz.ViCom.Net.LTE.SFrequencySetting.wAllSpecialSubFrameConfigurations;
                        ChannelSettings.pTableOfFrequencySetting[i].dwAvgBlockCountPer1000Sec = 2000;// 300;//((RohdeSchwarz.ViCom.Net.SRange<uint>)lteInterface.GetMeasRateLimits()).minimum;////////////////////////;
                        ChannelSettings.pTableOfFrequencySetting[i].enSSyncToPSyncRatioType = RohdeSchwarz.ViCom.Net.LTE.SFrequencySetting.SSyncToPSyncRatioType.Type.RatioRange;
                        ChannelSettings.pTableOfFrequencySetting[i].SSyncToPSyncRatio = new RohdeSchwarz.ViCom.Net.LTE.SFrequencySetting.SSyncToPSyncRatioSettings();
                        ChannelSettings.pTableOfFrequencySetting[i].SSyncToPSyncRatio.RatioRange = new RohdeSchwarz.ViCom.Net.LTE.SFrequencySetting.SSyncToPSyncRatioSettings._StructRatioRange();
                        ChannelSettings.pTableOfFrequencySetting[i].SSyncToPSyncRatio.RatioRange.fLowerRatioInDB = -5;
                        ChannelSettings.pTableOfFrequencySetting[i].SSyncToPSyncRatio.RatioRange.fUpperRatioInDB = 0;
                        ChannelSettings.pTableOfFrequencySetting[i].wNarrowbandRefSignalMeasMode = (ushort)iNarrowbandRefSignalMeasMode;
                        ChannelSettings.pTableOfFrequencySetting[i].enBandwidthCtrlMode = RohdeSchwarz.ViCom.Net.LTE.SFrequencySetting.BandwidthCtrlMode.Type.BW_FROM_MIB_ONCE_EACH_CELL;
                        //ChannelSettings.pTableOfFrequencySetting[i].wNumberOfResourceBlocks = 50;
                        ChannelSettings.pTableOfFrequencySetting[i].WidebandRsCinrSettings = new RohdeSchwarz.ViCom.Net.LTE.SFrequencySetting.SWidebandRsCinrSettings();
                        ChannelSettings.pTableOfFrequencySetting[i].WidebandRsCinrSettings.wWidebandRsCinrMeasMode = RohdeSchwarz.ViCom.Net.LTE.SFrequencySetting.SWidebandRsCinrSettings.wWIDEBAND_RS_CINR;
                        ChannelSettings.pTableOfFrequencySetting[i].WidebandRsCinrSettings.dwAvgBlockCountPer1000Sec = 1000;// 300;//((RohdeSchwarz.ViCom.Net.SRange<uint>)lteInterface.GetWbMeasRateLimits()).minimum;////////////////////////;
                        ChannelSettings.pTableOfFrequencySetting[i].WidebandRsCinrSettings.wNumberOfRBsInSubband = RohdeSchwarz.ViCom.Net.LTE.SFrequencySetting.SWidebandRsCinrSettings.wMinRBsInSubband;
                        ChannelSettings.pTableOfFrequencySetting[i].WidebandRsCinrSettings.dwFrontEndSelectionMask = GetDeviceRFInput(true, 1, "LTE");
                        ChannelSettings.pTableOfFrequencySetting[i].WidebandRsCinrSettings.bForceNoGap = true;
                        ChannelSettings.pTableOfFrequencySetting[i].WidebandRsCinrSettings.bMaxCountOfeNodeBs = 6;
                        ChannelSettings.pTableOfFrequencySetting[i].WidebandRsCinrSettings.bTransmitAntennaSelectionMask = 15;
                        ChannelSettings.pTableOfFrequencySetting[i].WidebandRsCinrSettings.sMinCenterRsrpInDBm100 = -13000;
                        ChannelSettings.pTableOfFrequencySetting[i].WidebandRsCinrSettings.wMaxRsrpDiffToBestCellInDB100 = 500;
                        ChannelSettings.pTableOfFrequencySetting[i].enMbmsConfigCtrlMode = RohdeSchwarz.ViCom.Net.LTE.SFrequencySetting.MbmsConfigCtrlMode.Type.MBMS_NOT_PRESENT;
                        ChannelSettings.pTableOfFrequencySetting[i].MimoSettings = new RohdeSchwarz.ViCom.Net.LTE.SFrequencySetting.SMimoSettings();
                        ChannelSettings.pTableOfFrequencySetting[i].MimoSettings.wMimoMeasMode = 0;
                        ChannelSettings.pTableOfFrequencySetting[i].MimoSettings.dwMimoResultMaskFor2x2 = 7;
                        ChannelSettings.pTableOfFrequencySetting[i].MimoSettings.dwMimoResultMaskFor2x4 = 0;
                        ChannelSettings.pTableOfFrequencySetting[i].MimoSettings.bMaxCountOfeNodeBs = 5;
                        ChannelSettings.pTableOfFrequencySetting[i].MimoSettings.sCinrThresholdForRankInDB100 = 0;
                        ChannelSettings.pTableOfFrequencySetting[i].MimoSettings.sMinCenterRsrpInDBm100 = -13000;
                        ChannelSettings.pTableOfFrequencySetting[i].MimoSettings.sMinRsCinrInDB100 = -1000;
                        ChannelSettings.pTableOfFrequencySetting[i].MimoSettings.wMaxRsrpDiffToBestCellInDB100 = 500;
                        ChannelSettings.pTableOfFrequencySetting[i].MimoSettings.wTimeResolutionInMs = 10;
                        // Setup Throughput estimation           
                        ChannelSettings.pTableOfFrequencySetting[i].MimoSettings.bEnableThroughputEstimation = false;
                        ChannelSettings.pTableOfFrequencySetting[i].RssiSettings = new RohdeSchwarz.ViCom.Net.LTE.SFrequencySetting.SRssiSettings();
                        ChannelSettings.pTableOfFrequencySetting[i].RssiSettings.dwFrontEndSelectionMask = GetDeviceRFInput(true, 1, "LTE");
                        ChannelSettings.pTableOfFrequencySetting[i].RssiSettings.wRssiMeasMode = 0;
                        ChannelSettings.pTableOfFrequencySetting[i].MbmsSettings.wMbmsMeasMode = 0;
                        #endregion
                    }
                    ChannelSettings.dwCount = (uint)ChannelSettings.pTableOfFrequencySetting.Length;
                    ChannelSettings.RsCinrChannelModel = new RohdeSchwarz.ViCom.Net.LTE.SChannelSettings.SRsCinrChannelModel();
                    ChannelSettings.RsCinrChannelModel.dwDelaySpreadInNs = RohdeSchwarz.ViCom.Net.LTE.SChannelSettings.SRsCinrChannelModel.dwMinDelaySpreadInNs;
                    ChannelSettings.RsCinrChannelModel.dwSpeedInKmPerHour = RohdeSchwarz.ViCom.Net.LTE.SChannelSettings.SRsCinrChannelModel.dwMinSpeedInKmh;

                    ChannelSettings.MbmsRsCinrChannelModel.dwDelaySpreadInNs = RohdeSchwarz.ViCom.Net.LTE.SChannelSettings.SMbmsRsCinrChannelModel.dwMinDelaySpreadInNs;
                    ChannelSettings.MbmsRsCinrChannelModel.dwSpeedInKmPerHour = RohdeSchwarz.ViCom.Net.LTE.SChannelSettings.SMbmsRsCinrChannelModel.dwMinSpeedInKmh;

                    List<RohdeSchwarz.ViCom.Net.LTE.Pdu.Type> siblist = new List<RohdeSchwarz.ViCom.Net.LTE.Pdu.Type>() { };
                    for (int i = 0; i < App.Sett.TSMxReceiver_Settings.LTE.SIBTypes.Count; i++)
                    { if (App.Sett.TSMxReceiver_Settings.LTE.SIBTypes[i].Use) { siblist.Add((RohdeSchwarz.ViCom.Net.LTE.Pdu.Type)Enum.Parse(typeof(RohdeSchwarz.ViCom.Net.LTE.Pdu.Type), App.Sett.TSMxReceiver_Settings.LTE.SIBTypes[i].SibType)); } }

                    // Activate BCH demodulation
                    uint dwRequests = (uint)siblist.Count * ChannelSettings.dwCount;
                    RohdeSchwarz.ViCom.Net.LTE.SDemodulationSettings bchSettings = new RohdeSchwarz.ViCom.Net.LTE.SDemodulationSettings();
                    bchSettings.sSINRThresholdDB100 = 100;// ((RohdeSchwarz.ViCom.Net.SRange<short>)lteInterface.GetDemodThresholdLimits()).minimum;
                    bchSettings.dwFrontEndSelectionMask = GetDeviceRFInput(true, 1, "LTE"); //dwFE_Mask;

                    bchSettings.sStartMeasurementRequests.dwCountOfRequests = dwRequests;
                    bchSettings.sStartMeasurementRequests.pDemodRequests = new RohdeSchwarz.ViCom.Net.LTE.SDemodRequests.SDemodRequest[dwRequests];
                    for (int i = 0; i < dwRequests; i++)
                    {
                        bchSettings.sStartMeasurementRequests.pDemodRequests[i] = new RohdeSchwarz.ViCom.Net.LTE.SDemodRequests.SDemodRequest();
                    }
                    for (int i = 0; i < freqs; i++)
                    {
                        int dwRequestStartIndex = i * siblist.Count;
                        for (int idx = 0; idx < siblist.Count; ++idx)
                        {
                            int iR = dwRequestStartIndex + idx;
                            bchSettings.sStartMeasurementRequests.pDemodRequests[iR].dwChannelIndex = (uint)i;
                            bchSettings.sStartMeasurementRequests.pDemodRequests[iR].ePDU = siblist[idx];
                            bchSettings.sStartMeasurementRequests.pDemodRequests[iR].eDemodulationMode = RohdeSchwarz.ViCom.Net.LTE.DemodMode.Type.ONCE;
                            bchSettings.sStartMeasurementRequests.pDemodRequests[iR].wRepetitionTimeOutInMs = 0;// 100;
                                                                                                                //bchSettings.sStartMeasurementRequests.pDemodRequests[iR].dwBtsId = 0;
                        }
                    }

                    lteInterface.SetFrequencyTable(ChannelSettings);
                    lteInterface.SetDemodulationSettings(bchSettings);

                    lteBasicInterface.StartMeasurement();
                    DM -= LTESetFreqFromACD;
                }





            }
            #region Exception
            catch (RohdeSchwarz.ViCom.Net.CViComError error)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = error, ClassName = "TSMxReceiver", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            catch (Exception exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "TSMxReceiver", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            #endregion

        }
        private void GetUnifreqsLTE()
        {
            #region LTE 
            List<decimal> freq3 = new List<decimal>() { };
            foreach (Settings.LTEFreqs_Set t in App.Sett.TSMxReceiver_Settings.LTE.Freqs)
            {
                if (t.Use == true)
                { freq3.Add(t.FreqDn); }
            }
            System.Collections.Generic.HashSet<decimal> hs3 = new System.Collections.Generic.HashSet<decimal>();
            foreach (decimal al in freq3)
            {
                hs3.Add(al);
            }
            LTEUniFreq.Clear();
            LTEUniFreq = new ObservableCollection<decimal>(hs3.OrderBy(i => i));
            #endregion
        }
        #endregion LTE

        #region CDMA
        private void SetNewStateCDMA()
        {
            DoubMod Connect = CDMAConnect;
            DoubMod Disconnect = CDMADisconnect;
            bool findConnect = false;
            bool findDisconnect = false;
            if (DM != null && Connect != null && Disconnect != null)
            {
                foreach (Delegate d in DM.GetInvocationList())
                {
                    if (d.Method.Name == Connect.GetInvocationList()[0].Method.Name) findConnect = true;
                    if (d.Method.Name == Disconnect.GetInvocationList()[0].Method.Name) findDisconnect = true;
                }
            }
            if (App.Sett.TSMxReceiver_Settings.CDMA.TechIsEnabled != CDMAIsRuning)
            {
                if (App.Sett.TSMxReceiver_Settings.CDMA.TechIsEnabled == true)
                {
                    if (findConnect == false && findDisconnect == false) DM += (DoubMod)Connect.GetInvocationList()[0];
                }
                else
                {
                    if (findConnect == false && findDisconnect == false) DM += (DoubMod)Disconnect.GetInvocationList()[0];
                }
            }
            else { DM -= SetNewStateCDMA; }
        }
        private static SEvdoControlSettings GetEvdoSettings()
        {
            var evdoSetting = new SEvdoControlSettings();
            evdoSetting.bStopCdma2000AfterSync = false;
            evdoSetting.dwFullSyncRatePer1000Sec = 10000;
            evdoSetting.dwShortSyncRatePer1000Sec = 10000;
            evdoSetting.dwMeasRatePer1000Sec = 10000;
            evdoSetting.dwShortSyncRangeInChips = 160;


            return evdoSetting;
        }
        private void CDMAConnect()
        {
            try
            {
                cdmaLoader = new CViComLoader<CViComCdmaInterface>(DeviceType);
                bool Runing = cdmaLoader.Connect(App.Sett.TSMxReceiver_Settings.IPAdress, out error, receiverListener);
                GetUnifreqsCDMA();
                cdmaInterface = cdmaLoader.GetInterface();
                cdmaBasicInterface = cdmaLoader.GetBasicInterface();

                var resbuf = new SResultBufferDepth();
                resbuf.dwValue = 1024;
                cdmaBasicInterface.SetResultBufferDepth(resbuf);

                //RohdeSchwarz.ViCom.Net.SRange<uint> rateLimit = (RohdeSchwarz.ViCom.Net.SRange<uint>)cdmaInterface.GetMeasRateLimits();

                //CDMAUniFreq
                uint freqs = (uint)CDMAUniFreq.Count();
                var settings = new RohdeSchwarz.ViCom.Net.CDMA.SChannelSettings();
                settings.dwFrontEndSelectionMask = GetDeviceRFInput(true, 1, "CDMA");
                settings.dwCount = freqs;
                settings.dwMeasRatePer1000Sec = 1000;//1000;//((RohdeSchwarz.ViCom.Net.SRange<uint>)cdmaInterface.GetMeasRateLimits()).minimum; ;// rateLimit.defaultValue;

                settings.pTableOfFrequencySetting = new RohdeSchwarz.ViCom.Net.CDMA.SFrequencySetting[freqs];
                int cdmaFreqsCount = 0;
                int evdoFreqsCount = 0;
                for (int i = 0; i < freqs; i++)
                {
                    settings.pTableOfFrequencySetting[i] = new RohdeSchwarz.ViCom.Net.CDMA.SFrequencySetting();
                    settings.pTableOfFrequencySetting[i].bIsEvdoFrequency = CDMAUniFreq[i].EVDOvsCDMA;
                    settings.pTableOfFrequencySetting[i].dCenterFrequencyInHz = (double)(CDMAUniFreq[i].FreqDn);
                    settings.pTableOfFrequencySetting[i].bTableOfPNOffsetArbitraryLimitation = new bool[512];
                    if (CDMAUniFreq[i].EVDOvsCDMA == false)
                    {
                        for (int ii = 0; ii < settings.pTableOfFrequencySetting[i].bTableOfPNOffsetArbitraryLimitation.Length; ii++)
                        {
                            settings.pTableOfFrequencySetting[i].bTableOfPNOffsetArbitraryLimitation[ii] = true;
                        }
                        cdmaFreqsCount++;
                    }
                    else { evdoFreqsCount++; }
                }

                cdmaInterface.SetFrequencyTable(settings);

                cdmaInterface.SetEvdoSettings(GetEvdoSettings());

                CDMAListener = new MyCdmaDataProcessor();

                cdmaInterface.RegisterResultDataListener(CDMAListener);
                #region
                //===============

                List<RohdeSchwarz.ViCom.Net.CDMA.Pdu.Type> CDMAsiblist = new List<RohdeSchwarz.ViCom.Net.CDMA.Pdu.Type>() { };
                List<RohdeSchwarz.ViCom.Net.CDMA.Pdu.Type> EVDOsiblist = new List<RohdeSchwarz.ViCom.Net.CDMA.Pdu.Type>() { };
                for (int i = 0; i < App.Sett.TSMxReceiver_Settings.CDMA.SITypes.Count; i++)//foreach (RohdeSchwarz.ViCom.Net.CDMA.Pdu.Type pdu in Enum.GetValues(typeof(RohdeSchwarz.ViCom.Net.CDMA.Pdu.Type)))
                {
                    if (!App.Sett.TSMxReceiver_Settings.CDMA.SITypes[i].ToString().Contains("UNKNOWN") && !App.Sett.TSMxReceiver_Settings.CDMA.SITypes[i].ToString().Contains("NONE"))
                    {
                        if (App.Sett.TSMxReceiver_Settings.CDMA.SITypes[i].Use && App.Sett.TSMxReceiver_Settings.CDMA.SITypes[i].SiType.StartsWith("EVDO")) { EVDOsiblist.Add((RohdeSchwarz.ViCom.Net.CDMA.Pdu.Type)Enum.Parse(typeof(RohdeSchwarz.ViCom.Net.CDMA.Pdu.Type), App.Sett.TSMxReceiver_Settings.CDMA.SITypes[i].SiType)); }
                        else if (App.Sett.TSMxReceiver_Settings.CDMA.SITypes[i].Use && !App.Sett.TSMxReceiver_Settings.CDMA.SITypes[i].SiType.StartsWith("EVDO")) { CDMAsiblist.Add((RohdeSchwarz.ViCom.Net.CDMA.Pdu.Type)Enum.Parse(typeof(RohdeSchwarz.ViCom.Net.CDMA.Pdu.Type), App.Sett.TSMxReceiver_Settings.CDMA.SITypes[i].SiType)); }
                    }
                }
                RohdeSchwarz.ViCom.Net.CDMA.SDemodulationSettings demod = new RohdeSchwarz.ViCom.Net.CDMA.SDemodulationSettings();

                demod.dwFrontEndSelectionMask = GetDeviceRFInput(true, 1, "CDMA");

                uint dwRequests = (uint)(CDMAsiblist.Count * cdmaFreqsCount + EVDOsiblist.Count * evdoFreqsCount);

                RohdeSchwarz.ViCom.Net.CDMA.SDemodRequests MeasurementRequests = new RohdeSchwarz.ViCom.Net.CDMA.SDemodRequests();

                //MeasurementRequests.dwCountOfRequests = dwRequests;
                MeasurementRequests.pDemodRequests = new RohdeSchwarz.ViCom.Net.CDMA.SDemodRequests.SDemodRequest[dwRequests];

                int idwRequestIndex = 0;
                for (int i = 0; i < settings.pTableOfFrequencySetting.Count(); i++)
                {
                    if (settings.pTableOfFrequencySetting[i].bIsEvdoFrequency == false)//cdma
                    {
                        for (int idx = 0; idx < CDMAsiblist.Count; idx++)
                        {
                            MeasurementRequests.pDemodRequests[idwRequestIndex] = new RohdeSchwarz.ViCom.Net.CDMA.SDemodRequests.SDemodRequest();
                            MeasurementRequests.pDemodRequests[idwRequestIndex].dwChannelIndex = (uint)i;
                            MeasurementRequests.pDemodRequests[idwRequestIndex].eDemodulationMode = RohdeSchwarz.ViCom.Net.CDMA.DemodMode.Type.ONCE;
                            MeasurementRequests.pDemodRequests[idwRequestIndex].PduSpec.ePDU = CDMAsiblist[idx];
                            MeasurementRequests.pDemodRequests[idwRequestIndex].PduSpec.eChannelType = ChannelType.Type.UNKNOWN;
                            MeasurementRequests.pDemodRequests[idwRequestIndex].PduSpec.eEvdoProtocol = EvdoProtocol.Type.UNKNOWN;
                            MeasurementRequests.pDemodRequests[idwRequestIndex].wRepetitionDelayIn100ms = 0;
                            idwRequestIndex++;
                        }
                    }
                    else if (settings.pTableOfFrequencySetting[i].bIsEvdoFrequency == true)//evdo
                    {
                        for (int idx = 0; idx < EVDOsiblist.Count; idx++)
                        {
                            MeasurementRequests.pDemodRequests[idwRequestIndex] = new RohdeSchwarz.ViCom.Net.CDMA.SDemodRequests.SDemodRequest();
                            MeasurementRequests.pDemodRequests[idwRequestIndex].dwChannelIndex = (uint)i;
                            MeasurementRequests.pDemodRequests[idwRequestIndex].eDemodulationMode = RohdeSchwarz.ViCom.Net.CDMA.DemodMode.Type.ONCE;
                            MeasurementRequests.pDemodRequests[idwRequestIndex].PduSpec.ePDU = EVDOsiblist[idx];
                            MeasurementRequests.pDemodRequests[idwRequestIndex].PduSpec.eEvdoProtocol = EvdoProtocol.Type.UNKNOWN;
                            MeasurementRequests.pDemodRequests[idwRequestIndex].PduSpec.eChannelType = ChannelType.Type.UNKNOWN;
                            MeasurementRequests.pDemodRequests[idwRequestIndex].wRepetitionDelayIn100ms = 0;
                            idwRequestIndex++;
                        }
                    }
                }
                MeasurementRequests.dwCountOfRequests = (uint)idwRequestIndex;
                demod.sStartMeasurementRequests = MeasurementRequests;
                demod.lEcToIoThresholdInDB100_for_EVDO = -1000;
                demod.lEcToIoThresholdInDB100_for_CDMA = -1000;

                SPPSSettings pps = new SPPSSettings() { };
                pps.iDelayOfPPSFallingEdgeIn100ns = SPPSSettings.iInvalidPPSDelayIn100ns;//10000000;

                cdmaInterface.SetPPSSettings(pps);
                cdmaInterface.SetSyncChannelDemodulationMode(SyncChannelDemodulationMode.Type.ALL);


                cdmaInterface.SetDemodulationSettings(demod);
                SMaxVelocity mv = new SMaxVelocity();
                mv.dMaxVelocityInKmPerHour = 120;
                cdmaInterface.SetMaxVelocity(mv);
                //=======
                #endregion
                cdmaBasicInterface.StartMeasurement();
                CDMAIsRuning = Runing;
            }
            #region Exception
            catch (RohdeSchwarz.ViCom.Net.CViComError error)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = error, ClassName = "TSMxReceiver", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            catch (Exception exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "TSMxReceiver", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            #endregion
            finally { DM -= CDMAConnect; }
        }
        private void CDMADisconnect()
        {
            try
            {
                if (cdmaLoader != null)
                {
                    if (cdmaLoader.GetBasicInterface().IsMeasurementStarted())
                    {
                        cdmaLoader.GetBasicInterface().StopMeasurement();
                        cdmaLoader.GetBasicInterface().HasMeasurementStopped(SDefs.dwDefaultTimeOutInMs);
                    }
                    if (cdmaLoader.GetBasicInterface() != null && cdmaLoader.GetBasicInterface().HasMeasurementStopped(SDefs.dwDefaultTimeOutInMs))
                    {
                        cdmaInterface.UnregisterResultDataListener(CDMAListener);
                        CDMAIsRuning = !cdmaLoader.Disconnect();
                        cdmaLoader.Dispose();
                        DM -= CDMADisconnect;
                    }
                }
            }
            #region Exception
            catch (RohdeSchwarz.ViCom.Net.CViComError error)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = error, ClassName = "TSMxReceiver", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            catch (Exception exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "TSMxReceiver", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            #endregion
            #region old
            //try
            //{
            //    if (cdmaLoader != null && cdmaLoader.GetBasicInterface().IsMeasurementStarted())
            //    {
            //        cdmaLoader.GetBasicInterface().StopMeasurement();
            //        if (cdmaLoader.GetBasicInterface().HasMeasurementStopped(SDefs.dwDefaultTimeOutInMs))
            //        {
            //            cdmaInterface.UnregisterResultDataListener(CDMAListener);
            //            CDMAIsRuning = !cdmaLoader.Disconnect();
            //            cdmaLoader.Dispose();
            //            DM -= CDMADisconnect;
            //        }
            //    }
            //    else if (cdmaLoader.GetBasicInterface() != null && cdmaLoader.GetBasicInterface().HasMeasurementStopped(SDefs.dwDefaultTimeOutInMs))
            //    {
            //        cdmaInterface.UnregisterResultDataListener(CDMAListener);
            //        CDMAIsRuning = !cdmaLoader.Disconnect();
            //        cdmaLoader.Dispose();
            //        DM -= CDMADisconnect;
            //    }
            //}
            //#region Exception
            //catch (RohdeSchwarz.ViCom.Net.CViComError error)
            //{
            //    MainWindow.exp.ExceptionData = new ExData() { ex = error, ClassName = "TSMxReceiver", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            //}
            //catch (Exception exp)
            //{
            //    MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "TSMxReceiver", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            //}
            //#endregion
            #endregion
        }
        private static void CDMASetFreqFromACD()
        {
            try
            {
                //IdentificationData.ACD.ACDData
                if (cdmaLoader != null && cdmaLoader.GetBasicInterface().IsMeasurementStarted())
                {
                    cdmaLoader.GetBasicInterface().StopMeasurement();
                }
                if (cdmaLoader.GetBasicInterface().HasMeasurementStopped(SDefs.dwDefaultTimeOutInMs))
                {
                    var ChannelSettings = cdmaInterface.GetSettings().ChannelSettings;

                    #region Freqs 
                    List<Settings.CDMAFreqs_Set> freq = new List<Settings.CDMAFreqs_Set>() { };
                    foreach (Settings.CDMAFreqs_Set t in CDMAUniFreq)
                    {
                        freq.Add(t);
                    }
                    for (int i = 0; i < IdentificationData.ACD.ACDData.Count(); i++)
                    {
                        if (IdentificationData.ACD.ACDData[i].Tech == 3 && IdentificationData.ACD.ACDData[i].Established == false)
                        {
                            bool find = false;
                            foreach (Settings.CDMAFreqs_Set t in CDMAUniFreq)
                            {
                                if (t.FreqDn == IdentificationData.ACD.ACDData[i].Freq && t.EVDOvsCDMA == false) find = true;
                            }
                            if (find == false)
                            {
                                Settings.CDMAFreqs_Set fs = new Settings.CDMAFreqs_Set()
                                {
                                    EVDOvsCDMA = false,
                                    FreqDn = IdentificationData.ACD.ACDData[i].Freq,
                                    Use = true,
                                };
                                freq.Add(fs);
                            }
                            IdentificationData.ACD.ACDData[i].Established = true;
                        }
                        if (IdentificationData.ACD.ACDData[i].Tech == 4 && IdentificationData.ACD.ACDData[i].Established == false)
                        {
                            bool find = false;
                            foreach (Settings.CDMAFreqs_Set t in CDMAUniFreq)
                            {
                                if (t.FreqDn == IdentificationData.ACD.ACDData[i].Freq && t.EVDOvsCDMA == true) find = true;
                            }
                            if (find == false)
                            {
                                Settings.CDMAFreqs_Set fs = new Settings.CDMAFreqs_Set()
                                {
                                    EVDOvsCDMA = true,
                                    FreqDn = IdentificationData.ACD.ACDData[i].Freq,
                                    Use = true,
                                };
                                freq.Add(fs);
                            }
                            IdentificationData.ACD.ACDData[i].Established = true;
                        }
                    }
                    System.Collections.Generic.HashSet<Settings.CDMAFreqs_Set> hs = new System.Collections.Generic.HashSet<Settings.CDMAFreqs_Set>();
                    foreach (Settings.CDMAFreqs_Set al in freq)
                    {
                        hs.Add(al);
                    }
                    CDMAUniFreq.Clear();
                    CDMAUniFreq = new ObservableCollection<Settings.CDMAFreqs_Set>(hs);
                    #endregion

                    uint freqs = (uint)CDMAUniFreq.Count(); //UMTSUniFreq

                    ChannelSettings.dwFrontEndSelectionMask = GetDeviceRFInput(true, 1, "CDMA");
                    ChannelSettings.dwCount = freqs;
                    ChannelSettings.dwMeasRatePer1000Sec = 1000;

                    ChannelSettings.pTableOfFrequencySetting = new RohdeSchwarz.ViCom.Net.CDMA.SFrequencySetting[freqs];
                    int cdmaFreqsCount = 0;
                    int evdoFreqsCount = 0;
                    for (int i = 0; i < freqs; i++)
                    {
                        ChannelSettings.pTableOfFrequencySetting[i] = new RohdeSchwarz.ViCom.Net.CDMA.SFrequencySetting();
                        ChannelSettings.pTableOfFrequencySetting[i].bIsEvdoFrequency = CDMAUniFreq[i].EVDOvsCDMA;
                        ChannelSettings.pTableOfFrequencySetting[i].dCenterFrequencyInHz = (double)(CDMAUniFreq[i].FreqDn);
                        ChannelSettings.pTableOfFrequencySetting[i].bTableOfPNOffsetArbitraryLimitation = new bool[512];
                        if (CDMAUniFreq[i].EVDOvsCDMA == false)
                        {
                            for (int ii = 0; ii < ChannelSettings.pTableOfFrequencySetting[i].bTableOfPNOffsetArbitraryLimitation.Length; ii++)
                            {
                                ChannelSettings.pTableOfFrequencySetting[i].bTableOfPNOffsetArbitraryLimitation[ii] = true;
                            }
                            cdmaFreqsCount++;
                        }
                        else { evdoFreqsCount++; }
                    }
                    cdmaInterface.SetFrequencyTable(ChannelSettings);
                    cdmaInterface.SetEvdoSettings(GetEvdoSettings());
                    #region
                    List<RohdeSchwarz.ViCom.Net.CDMA.Pdu.Type> CDMAsiblist = new List<RohdeSchwarz.ViCom.Net.CDMA.Pdu.Type>() { };
                    List<RohdeSchwarz.ViCom.Net.CDMA.Pdu.Type> EVDOsiblist = new List<RohdeSchwarz.ViCom.Net.CDMA.Pdu.Type>() { };
                    for (int i = 0; i < App.Sett.TSMxReceiver_Settings.CDMA.SITypes.Count; i++)//foreach (RohdeSchwarz.ViCom.Net.CDMA.Pdu.Type pdu in Enum.GetValues(typeof(RohdeSchwarz.ViCom.Net.CDMA.Pdu.Type)))
                    {
                        if (!App.Sett.TSMxReceiver_Settings.CDMA.SITypes[i].ToString().Contains("UNKNOWN") && !App.Sett.TSMxReceiver_Settings.CDMA.SITypes[i].ToString().Contains("NONE"))
                        {
                            if (App.Sett.TSMxReceiver_Settings.CDMA.SITypes[i].Use && App.Sett.TSMxReceiver_Settings.CDMA.SITypes[i].SiType.StartsWith("EVDO")) { EVDOsiblist.Add((RohdeSchwarz.ViCom.Net.CDMA.Pdu.Type)Enum.Parse(typeof(RohdeSchwarz.ViCom.Net.CDMA.Pdu.Type), App.Sett.TSMxReceiver_Settings.CDMA.SITypes[i].SiType)); }
                            else if (App.Sett.TSMxReceiver_Settings.CDMA.SITypes[i].Use && !App.Sett.TSMxReceiver_Settings.CDMA.SITypes[i].SiType.StartsWith("EVDO")) { CDMAsiblist.Add((RohdeSchwarz.ViCom.Net.CDMA.Pdu.Type)Enum.Parse(typeof(RohdeSchwarz.ViCom.Net.CDMA.Pdu.Type), App.Sett.TSMxReceiver_Settings.CDMA.SITypes[i].SiType)); }
                        }
                    }
                    RohdeSchwarz.ViCom.Net.CDMA.SDemodulationSettings demod = new RohdeSchwarz.ViCom.Net.CDMA.SDemodulationSettings();

                    demod.dwFrontEndSelectionMask = GetDeviceRFInput(true, 1, "CDMA");

                    uint dwRequests = (uint)(CDMAsiblist.Count * cdmaFreqsCount + EVDOsiblist.Count * evdoFreqsCount);

                    RohdeSchwarz.ViCom.Net.CDMA.SDemodRequests MeasurementRequests = new RohdeSchwarz.ViCom.Net.CDMA.SDemodRequests();

                    //MeasurementRequests.dwCountOfRequests = dwRequests;
                    MeasurementRequests.pDemodRequests = new RohdeSchwarz.ViCom.Net.CDMA.SDemodRequests.SDemodRequest[dwRequests];

                    int idwRequestIndex = 0;
                    for (int i = 0; i < ChannelSettings.pTableOfFrequencySetting.Count(); i++)
                    {
                        if (ChannelSettings.pTableOfFrequencySetting[i].bIsEvdoFrequency == false)//cdma
                        {
                            for (int idx = 0; idx < CDMAsiblist.Count; idx++)
                            {
                                MeasurementRequests.pDemodRequests[idwRequestIndex] = new RohdeSchwarz.ViCom.Net.CDMA.SDemodRequests.SDemodRequest();
                                MeasurementRequests.pDemodRequests[idwRequestIndex].dwChannelIndex = (uint)i;
                                MeasurementRequests.pDemodRequests[idwRequestIndex].eDemodulationMode = RohdeSchwarz.ViCom.Net.CDMA.DemodMode.Type.ONCE;
                                MeasurementRequests.pDemodRequests[idwRequestIndex].PduSpec.ePDU = CDMAsiblist[idx];
                                MeasurementRequests.pDemodRequests[idwRequestIndex].PduSpec.eChannelType = ChannelType.Type.UNKNOWN;
                                MeasurementRequests.pDemodRequests[idwRequestIndex].PduSpec.eEvdoProtocol = EvdoProtocol.Type.UNKNOWN;
                                MeasurementRequests.pDemodRequests[idwRequestIndex].wRepetitionDelayIn100ms = 0;
                                idwRequestIndex++;
                            }
                        }
                        else if (ChannelSettings.pTableOfFrequencySetting[i].bIsEvdoFrequency == true)//evdo
                        {
                            for (int idx = 0; idx < EVDOsiblist.Count; idx++)
                            {
                                MeasurementRequests.pDemodRequests[idwRequestIndex] = new RohdeSchwarz.ViCom.Net.CDMA.SDemodRequests.SDemodRequest();
                                MeasurementRequests.pDemodRequests[idwRequestIndex].dwChannelIndex = (uint)i;
                                MeasurementRequests.pDemodRequests[idwRequestIndex].eDemodulationMode = RohdeSchwarz.ViCom.Net.CDMA.DemodMode.Type.ONCE;
                                MeasurementRequests.pDemodRequests[idwRequestIndex].PduSpec.ePDU = EVDOsiblist[idx];
                                MeasurementRequests.pDemodRequests[idwRequestIndex].PduSpec.eEvdoProtocol = EvdoProtocol.Type.UNKNOWN;
                                MeasurementRequests.pDemodRequests[idwRequestIndex].PduSpec.eChannelType = ChannelType.Type.UNKNOWN;
                                MeasurementRequests.pDemodRequests[idwRequestIndex].wRepetitionDelayIn100ms = 0;
                                idwRequestIndex++;
                            }
                        }
                    }
                    MeasurementRequests.dwCountOfRequests = (uint)idwRequestIndex;
                    demod.sStartMeasurementRequests = MeasurementRequests;
                    demod.lEcToIoThresholdInDB100_for_EVDO = -1000;
                    demod.lEcToIoThresholdInDB100_for_CDMA = -1000;

                    SPPSSettings pps = new SPPSSettings() { };
                    pps.iDelayOfPPSFallingEdgeIn100ns = SPPSSettings.iInvalidPPSDelayIn100ns;//10000000;

                    cdmaInterface.SetPPSSettings(pps);
                    cdmaInterface.SetSyncChannelDemodulationMode(SyncChannelDemodulationMode.Type.ALL);


                    cdmaInterface.SetDemodulationSettings(demod);
                    SMaxVelocity mv = new SMaxVelocity();
                    mv.dMaxVelocityInKmPerHour = 120;
                    cdmaInterface.SetMaxVelocity(mv);
                    //=======
                    #endregion
                    cdmaBasicInterface.StartMeasurement();






                    DM -= CDMASetFreqFromACD;
                }





            }
            #region Exception
            catch (RohdeSchwarz.ViCom.Net.CViComError error)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = error, ClassName = "TSMxReceiver", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            catch (Exception exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "TSMxReceiver", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            #endregion

        }
        private void GetUnifreqsCDMA()
        {
            #region CDMA 
            List<Settings.CDMAFreqs_Set> freq = new List<Settings.CDMAFreqs_Set>() { };
            foreach (Settings.CDMAFreqs_Set t in App.Sett.TSMxReceiver_Settings.CDMA.Freqs)
            {
                if (t.Use == true)
                { freq.Add(t); }
            }
            System.Collections.Generic.HashSet<Settings.CDMAFreqs_Set> hs = new System.Collections.Generic.HashSet<Settings.CDMAFreqs_Set>();
            foreach (Settings.CDMAFreqs_Set al in freq)
            {
                hs.Add(al);
            }
            CDMAUniFreq.Clear();
            CDMAUniFreq = new ObservableCollection<Settings.CDMAFreqs_Set>(hs.OrderBy(i => i.FreqDn));
            #endregion
        }
        #endregion CDMA

        #region RfPowerScan
        private void SetNewStateRfPowerScan()
        {
            DoubMod Connect = RfPowerScanConnect;
            DoubMod Disconnect = RfPowerScanDisconnect;
            bool findConnect = false;
            bool findDisconnect = false;
            if (DM != null && Connect != null && Disconnect != null)
            {
                foreach (Delegate d in DM.GetInvocationList())
                {
                    if (d.Method.Name == Connect.GetInvocationList()[0].Method.Name) findConnect = true;
                    if (d.Method.Name == Disconnect.GetInvocationList()[0].Method.Name) findDisconnect = true;
                }
            }
            if (App.Sett.TSMxReceiver_Settings.UseRFPowerScan != RFPSIsRuning)
            {
                if (App.Sett.TSMxReceiver_Settings.UseRFPowerScan == true)
                {
                    if (findConnect == false && findDisconnect == false) DM += (DoubMod)Connect.GetInvocationList()[0];
                }
                else
                {
                    if (findConnect == false && findDisconnect == false) DM += (DoubMod)Disconnect.GetInvocationList()[0];
                }
            }
            else { DM -= SetNewStateRfPowerScan; }
        }
        private CReceiverListener receiverListener2 = new CReceiverListener();
        private void RfPowerScanConnect()
        {
            try
            {
                RFPowerScanLoader = new CViComLoader<CViComRFPowerScanInterface>(DeviceType);
                ViComMessageTracerRegistry.Register(new MessageTracer());
                //RFPowerScanLoader.Connect("192.168.2.2", receiverListener2);
                bool Runing = RFPowerScanLoader.Connect(App.Sett.TSMxReceiver_Settings.IPAdress, out error, receiverListener);

                RFPowerScanInterface = RFPowerScanLoader.GetInterface();
                RFPowerScanBasicInterface = RFPowerScanLoader.GetBasicInterface();

                //RFPowerScanListener = new MyRfPowerScanDataProcessor();

                //RFPowerScanInterface.RegisterResultDataListener(RFPowerScanListener);

                //for (int i = 0; i < 2; i++)
                //{
                FreqCentr = (FreqStart + FreqStop) / 2;
                FreqSpan = FreqStop - FreqStart;
                rSSweepSettings = new RohdeSchwarz.ViCom.Net.RFPOWERSCAN.SSweepSettings();

                rSSweepSettings.dwFrontEndSelectionMask = GetDeviceRFInput(false, 1, "RF"); // tsme = 1, tsmw = 1or2
                rSSweepSettings.bRequestRawData = 0;//хз лучше не трогать
                rSSweepSettings.dStartFrequencyInHz = (double)FreqStart;// 1800.0e6;
                rSSweepSettings.dStopFrequencyInHz = (double)FreqStop;// 2100.0e6;
                rSSweepSettings.sSpectrumSettings.bPreamplifier = 1; //только с tsmw
                rSSweepSettings.sSpectrumSettings.bAutoAttenuation = 1; //только с tsmw
                rSSweepSettings.sSpectrumSettings.bAttenuationInDb = 0;//только с tsmw
                rSSweepSettings.sSpectrumSettings.bAutoBandwidth = 0;
                rSSweepSettings.sSpectrumSettings.dwBandwidthInHz = (uint)20e6; //
                rSSweepSettings.sSpectrumSettings.eFFTSize = SSpectrumSettings.FFTSize.Type.RFPOWERSCAN_FFTSIZE_8192;
                rSSweepSettings.sSpectrumSettings.fMaxDeviceMeasRateInHz = 50.0F;
                rSSweepSettings.sSpectrumSettings.fMaxReportingRateInHz = 50.0F;
                rSSweepSettings.sSpectrumSettings.fThresholdInDbm = -160.0F;
                rSSweepSettings.sFrequencyDetector.dwCountOfLines = (uint)TracePoints;
                rSSweepSettings.sFrequencyDetector.eDetectorType = SFrequencyDetector.FrequencyDetectorType.Type.RFPOWERSCAN_FREQDET_TYPE_PEAK;
                rSSweepSettings.sTimeDetector.eDetectorIntervalType = STimeDetector.TimeDetectorIntervalType.Type.RFPOWERSCAN_TIMEDET_INTERVAL_TIMERANGE;
                rSSweepSettings.sTimeDetector.eDetectorType = STimeDetector.TimeDetectorType.Type.RFPOWERSCAN_TIMEDET_TYPE_PEAK;
                rSSweepSettings.sTimeDetector.dwTimeParameterInMs = 10;
                rSSweepSettings.sChannelFilterSequence.dwCountOfSubsequences = 0;
                rSSweepSettings.sChannelFilterSequence.pSubsequences = null;
                rSSweepSettings.sChannelFilterSequence.sChannelFilterDefinition.pfMagnitudeOfTransferFunction = null;
                rSSweepSettings.sMarker.bUseMarker = 0;
                rSSweepSettings.sMeasurementTime.dwMeasTimeInNs = 1;
                rSSweepSettings.sMeasurementTime.eDetectorType = SMeasurementTime.DetectorType.Type.RFPOWERSCAN_DETECTOR_TYPE_MAXPEAK;

                RFPowerScanInterface.SetSweepSettings(rSSweepSettings);

                //Console.WriteLine("Starting RFPowerScan measurement {0}", i);

                RFPowerScanBasicInterface.StartMeasurement();
                RFPSIsRuning = Runing;
                _FreqSet = true;
            }
            #region Exception
            catch (RohdeSchwarz.ViCom.Net.CViComError error)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = error, ClassName = "TSMxReceiver", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            catch (Exception exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "TSMxReceiver", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            #endregion
            finally { DM -= RfPowerScanConnect; }
        }
        private void RfPowerScanDisconnect()
        {
            try
            {
                if (RFPowerScanLoader != null)
                {
                    if (RFPowerScanLoader.GetBasicInterface().IsMeasurementStarted())
                    {
                        RFPowerScanLoader.GetBasicInterface().StopMeasurement();
                        RFPowerScanLoader.GetBasicInterface().HasMeasurementStopped(SDefs.dwDefaultTimeOutInMs);
                    }
                    if (RFPowerScanLoader.GetBasicInterface() != null && RFPowerScanLoader.GetBasicInterface().HasMeasurementStopped(SDefs.dwDefaultTimeOutInMs))
                    {
                        //RFPowerScanInterface.UnregisterResultDataListener(RFPowerScanListener);
                        RFPSIsRuning = !RFPowerScanLoader.Disconnect();
                        RFPowerScanLoader.Dispose();
                        DM -= RfPowerScanDisconnect;
                    }
                }
            }
            #region Exception
            catch (RohdeSchwarz.ViCom.Net.CViComError error)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = error, ClassName = "TSMxReceiver", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            catch (Exception exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "TSMxReceiver", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            #endregion
            #region old
            //try
            //{
            //    if (RFPowerScanLoader != null && RFPowerScanLoader.GetBasicInterface().IsMeasurementStarted())
            //    {
            //        //RFPowerScanBasicInterface.StopMeasurement();
            //        //RFPowerScanBasicInterface.HasMeasurementStopped(SDefs.dwDefaultTimeOutInMs);

            //        ////RFPowerScanInterface.UnregisterResultDataListener(RFPowerScanListener);

            //        //RFPowerScanLoader.Disconnect();
            //        RFPowerScanLoader.GetBasicInterface().StopMeasurement();
            //        if (RFPowerScanLoader.GetBasicInterface().HasMeasurementStopped(SDefs.dwDefaultTimeOutInMs))
            //        {
            //            //RFPowerScanInterface.UnregisterResultDataListener(RFPowerScanListener);
            //            RFPSIsRuning = !RFPowerScanLoader.Disconnect();
            //            RFPowerScanLoader.Dispose();
            //            DM -= RfPowerScanDisconnect;
            //        }
            //    }
            //    else if (RFPowerScanLoader.GetBasicInterface() != null && RFPowerScanLoader.GetBasicInterface().HasMeasurementStopped(SDefs.dwDefaultTimeOutInMs))
            //    {
            //        //RFPowerScanInterface.UnregisterResultDataListener(RFPowerScanListener);
            //        RFPSIsRuning = !RFPowerScanLoader.Disconnect();
            //        RFPowerScanLoader.Dispose();
            //        DM -= RfPowerScanDisconnect;
            //    }

            //}
            //#region Exception
            //catch (RohdeSchwarz.ViCom.Net.CViComError error)
            //{
            //    MainWindow.exp.ExceptionData = new ExData() { ex = error, ClassName = "TSMxReceiver", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            //}
            //catch (Exception exp)
            //{
            //    MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "TSMxReceiver", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            //}
            //#endregion
            #endregion
        }
        private void RfPowerScanSetFreq()
        {
            try
            {
                long beginTiks = DateTime.Now.Ticks;
                if (RFPowerScanBasicInterface != null && RFPowerScanBasicInterface.IsMeasurementStarted())
                {
                    RFPowerScanBasicInterface.StopMeasurement();
                    RFPowerScanBasicInterface.HasMeasurementStopped(SDefs.dwDefaultTimeOutInMs);

                    //RFPowerScanInterface.UnregisterResultDataListener(RFPowerScanListener);

                    //RFPowerScanLoader.Disconnect();


                    RFPowerScanInterface.SetSweepSettings(RfPowerScanSetSweepSettings());

                    RFPowerScanBasicInterface.StartMeasurement();
                }
                Time = new TimeSpan(DateTime.Now.Ticks - beginTiks).ToString();
            }
            #region Exception
            catch (RohdeSchwarz.ViCom.Net.CViComError error)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = error, ClassName = "TSMxReceiver", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            catch (Exception exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "TSMxReceiver", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            #endregion
            DM -= RfPowerScanSetFreq;
        }
        private SSweepSettings RfPowerScanSetSweepSettings()
        {
            SSweepSettings sss = new RohdeSchwarz.ViCom.Net.RFPOWERSCAN.SSweepSettings();
            FreqStart = 880000000; FreqStop = 970000000;
            try
            {
                sss.dwFrontEndSelectionMask = GetDeviceRFInput(false, 1, "RF"); // tsme = 1, tsmw = 1or2
                sss.bRequestRawData = 0;//хз
                sss.dStartFrequencyInHz = (double)FreqStart;
                sss.dStopFrequencyInHz = (double)FreqStop;
                sss.sSpectrumSettings.bAutoAttenuation = 0; //только с tsmw
                sss.sSpectrumSettings.bAttenuationInDb = 0;//только с tsmw
                sss.sSpectrumSettings.bPreamplifier = 1;
                sss.sSpectrumSettings.bAutoBandwidth = 1;
                sss.sSpectrumSettings.dwBandwidthInHz = (uint)20e6; //
                sss.sSpectrumSettings.eFFTSize = RohdeSchwarz.ViCom.Net.RFPOWERSCAN.SSpectrumSettings.FFTSize.Type.RFPOWERSCAN_FFTSIZE_1024;
                sss.sSpectrumSettings.fMaxDeviceMeasRateInHz = 200.0F;
                sss.sSpectrumSettings.fMaxReportingRateInHz = 200.0F;
                sss.sSpectrumSettings.fThresholdInDbm = -160.0F;
                sss.sFrequencyDetector.dwCountOfLines = (uint)TracePoints;
                sss.sFrequencyDetector.eDetectorType = RohdeSchwarz.ViCom.Net.RFPOWERSCAN.SFrequencyDetector.FrequencyDetectorType.Type.RFPOWERSCAN_FREQDET_TYPE_RMS;
                sss.sTimeDetector.eDetectorIntervalType = RohdeSchwarz.ViCom.Net.RFPOWERSCAN.STimeDetector.TimeDetectorIntervalType.Type.RFPOWERSCAN_TIMEDET_INTERVAL_TIMERANGE;
                sss.sTimeDetector.eDetectorType = RohdeSchwarz.ViCom.Net.RFPOWERSCAN.STimeDetector.TimeDetectorType.Type.RFPOWERSCAN_TIMEDET_TYPE_RMS;
                sss.sTimeDetector.dwTimeParameterInMs = 100;
                sss.sChannelFilterSequence.dwCountOfSubsequences = 0;
                sss.sChannelFilterSequence.pSubsequences = null;
                sss.sChannelFilterSequence.sChannelFilterDefinition.pfMagnitudeOfTransferFunction = null;
                sss.sMarker.bUseMarker = 0;
                sss.sMeasurementTime.dwMeasTimeInNs = 1;
                sss.sMeasurementTime.eDetectorType = RohdeSchwarz.ViCom.Net.RFPOWERSCAN.SMeasurementTime.DetectorType.Type.RFPOWERSCAN_DETECTOR_TYPE_RMS;
                //if (RFPowerScanBasicInterface != null && RFPowerScanBasicInterface.IsMeasurementStarted())
                //{

                //}
            }
            #region Exception
            catch (RohdeSchwarz.ViCom.Net.CViComError error)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = error, ClassName = "TSMxReceiver", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            catch (Exception exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "TSMxReceiver", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            #endregion
            return sss;
        }

        private SSweepSettings RfPowerScanSetSweepSettings1(decimal _FreqCentr, decimal _FreqSpan, int _TracePoints)
        {
            SSweepSettings sss = new RohdeSchwarz.ViCom.Net.RFPOWERSCAN.SSweepSettings();
            FreqStart = _FreqCentr - _FreqSpan / 2; FreqStop = _FreqCentr + _FreqSpan / 2;
            TracePoints = _TracePoints;
            FreqStep = _FreqSpan / (TracePoints - 1);

            this.FreqCentr = _FreqCentr;
            this.FreqSpan = _FreqSpan;
            uint sp = 0;
            byte autobw = 0;
            if (FreqSpan > 20000000) { sp = 20000000; autobw = 1; }
            else if (FreqSpan >= 1000000 && FreqSpan <= 20000000) { sp = (uint)FreqSpan; autobw = 0; }
            else if (FreqSpan <= 1000000) { sp = 1000000; autobw = 0; }
            try
            {
                sss.dwFrontEndSelectionMask = GetDeviceRFInput(false, 1, "RF"); // tsme = 1, tsmw = 1or2
                sss.bRequestRawData = 0;//хз
                sss.dStartFrequencyInHz = (double)FreqStart;
                sss.dStopFrequencyInHz = (double)FreqStop;
                sss.sSpectrumSettings.bAutoAttenuation = 1; //только с tsmw
                sss.sSpectrumSettings.bAttenuationInDb = 0;//только с tsmw
                sss.sSpectrumSettings.bPreamplifier = 1;
                sss.sSpectrumSettings.bAutoBandwidth = autobw;
                sss.sSpectrumSettings.dwBandwidthInHz = sp; //
                sss.sSpectrumSettings.eFFTSize = RohdeSchwarz.ViCom.Net.RFPOWERSCAN.SSpectrumSettings.FFTSize.Type.RFPOWERSCAN_FFTSIZE_8192;
                sss.sSpectrumSettings.fMaxDeviceMeasRateInHz = 100.0F;
                sss.sSpectrumSettings.fMaxReportingRateInHz = 100.0F;
                sss.sSpectrumSettings.fThresholdInDbm = -160.0F;
                sss.sFrequencyDetector.dwCountOfLines = (uint)TracePoints;
                sss.sFrequencyDetector.eDetectorType = RohdeSchwarz.ViCom.Net.RFPOWERSCAN.SFrequencyDetector.FrequencyDetectorType.Type.RFPOWERSCAN_FREQDET_TYPE_RMS;
                sss.sTimeDetector.eDetectorIntervalType = RohdeSchwarz.ViCom.Net.RFPOWERSCAN.STimeDetector.TimeDetectorIntervalType.Type.RFPOWERSCAN_TIMEDET_INTERVAL_TIMERANGE;
                sss.sTimeDetector.eDetectorType = RohdeSchwarz.ViCom.Net.RFPOWERSCAN.STimeDetector.TimeDetectorType.Type.RFPOWERSCAN_TIMEDET_TYPE_RMS;
                sss.sTimeDetector.dwTimeParameterInMs = 100;
                sss.sChannelFilterSequence.dwCountOfSubsequences = 0;
                sss.sChannelFilterSequence.pSubsequences = null;
                sss.sChannelFilterSequence.sChannelFilterDefinition.pfMagnitudeOfTransferFunction = null;
                sss.sMarker.bUseMarker = 0;
                sss.sMeasurementTime.dwMeasTimeInNs = 1;
                sss.sMeasurementTime.eDetectorType = RohdeSchwarz.ViCom.Net.RFPOWERSCAN.SMeasurementTime.DetectorType.Type.RFPOWERSCAN_DETECTOR_TYPE_RMS;
            }
            #region Exception
            catch (RohdeSchwarz.ViCom.Net.CViComError error)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = error, ClassName = "TSMxReceiver", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            catch (Exception exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "TSMxReceiver", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            #endregion
            return sss;
        }
        private SSweepSettings RfPowerScanSetSweepSettingsBand(decimal _FreqStart, decimal _FreqStop, int _TracePoints)
        {
            SSweepSettings sss = new RohdeSchwarz.ViCom.Net.RFPOWERSCAN.SSweepSettings();
            FreqStart = _FreqStart;
            FreqStop = _FreqStop;
            TracePoints = _TracePoints;
            FreqStep = FreqSpan / (TracePoints - 1);
            uint sp = 0;
            byte autobw = 0;
            if (FreqSpan > 20000000) { sp = 20000000; autobw = 1; }
            else if (FreqSpan >= 1000000 && FreqSpan <= 20000000) { sp = (uint)FreqSpan; autobw = 0; }
            else if (FreqSpan <= 1000000) { sp = 1000000; autobw = 0; }
            try
            {
                sss.dwFrontEndSelectionMask = GetDeviceRFInput(false, 1, "RF"); // tsme = 1, tsmw = 1or2
                sss.bRequestRawData = 0;//хз
                sss.dStartFrequencyInHz = (double)FreqStart;
                sss.dStopFrequencyInHz = (double)FreqStop;
                sss.sSpectrumSettings.bAutoAttenuation = 1; //только с tsmw
                sss.sSpectrumSettings.bAttenuationInDb = 0;//только с tsmw
                sss.sSpectrumSettings.bPreamplifier = 1;
                sss.sSpectrumSettings.bAutoBandwidth = autobw;
                sss.sSpectrumSettings.dwBandwidthInHz = sp; //
                sss.sSpectrumSettings.eFFTSize = RohdeSchwarz.ViCom.Net.RFPOWERSCAN.SSpectrumSettings.FFTSize.Type.RFPOWERSCAN_FFTSIZE_8192;
                sss.sSpectrumSettings.fMaxDeviceMeasRateInHz = 500.0F;
                sss.sSpectrumSettings.fMaxReportingRateInHz = 500.0F;
                sss.sSpectrumSettings.fThresholdInDbm = -160.0F;
                sss.sFrequencyDetector.dwCountOfLines = (uint)TracePoints;
                sss.sFrequencyDetector.eDetectorType = RohdeSchwarz.ViCom.Net.RFPOWERSCAN.SFrequencyDetector.FrequencyDetectorType.Type.RFPOWERSCAN_FREQDET_TYPE_RMS;
                sss.sTimeDetector.eDetectorIntervalType = RohdeSchwarz.ViCom.Net.RFPOWERSCAN.STimeDetector.TimeDetectorIntervalType.Type.RFPOWERSCAN_TIMEDET_INTERVAL_TIMERANGE;
                sss.sTimeDetector.eDetectorType = RohdeSchwarz.ViCom.Net.RFPOWERSCAN.STimeDetector.TimeDetectorType.Type.RFPOWERSCAN_TIMEDET_TYPE_RMS;
                sss.sTimeDetector.dwTimeParameterInMs = 100;
                sss.sChannelFilterSequence.dwCountOfSubsequences = 0;
                sss.sChannelFilterSequence.pSubsequences = null;
                sss.sChannelFilterSequence.sChannelFilterDefinition.pfMagnitudeOfTransferFunction = null;
                sss.sMarker.bUseMarker = 0;
                sss.sMeasurementTime.dwMeasTimeInNs = 1;
                sss.sMeasurementTime.eDetectorType = RohdeSchwarz.ViCom.Net.RFPOWERSCAN.SMeasurementTime.DetectorType.Type.RFPOWERSCAN_DETECTOR_TYPE_RMS;
            }
            #region Exception
            catch (RohdeSchwarz.ViCom.Net.CViComError error)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = error, ClassName = "TSMxReceiver", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            catch (Exception exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "TSMxReceiver", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            #endregion
            return sss;
        }
        #endregion RfPowerScan

        #region ACD
        private void SetNewStateACD()
        {
            DoubMod Connect = ACDConnect;
            DoubMod Disconnect = ACDDisconnect;
            bool findConnect = false;
            bool findDisconnect = false;
            if (DM != null && Connect != null && Disconnect != null)
            {
                foreach (Delegate d in DM.GetInvocationList())
                {
                    if (d.Method.Name == Connect.GetInvocationList()[0].Method.Name) findConnect = true;
                    if (d.Method.Name == Disconnect.GetInvocationList()[0].Method.Name) findDisconnect = true;
                }
            }
            if (App.Sett.TSMxReceiver_Settings.ACD.TechIsEnabled != ACDIsRuning)
            {
                if (App.Sett.TSMxReceiver_Settings.ACD.TechIsEnabled == true)
                {
                    if (findConnect == false && findDisconnect == false) DM += (DoubMod)Connect.GetInvocationList()[0];
                }
                else
                {
                    if (findConnect == false && findDisconnect == false) DM += (DoubMod)Disconnect.GetInvocationList()[0];
                }
            }
            else { DM -= SetNewStateACD; }
        }
        private void ACDConnect()
        {
            try
            {
                acdLoader = new CViComLoader<CViComAcdInterface>(DeviceType);
                bool Runing = acdLoader.Connect(App.Sett.TSMxReceiver_Settings.IPAdress, out error, receiverListener);

                acdInterface = acdLoader.GetInterface();
                acdBasicInterface = acdLoader.GetBasicInterface();

                var resbuf = new SResultBufferDepth();
                resbuf.dwValue = 1024;
                acdBasicInterface.SetResultBufferDepth(resbuf);

                var settings = new RohdeSchwarz.ViCom.Net.ACD.SAcdSettings();

                settings.enMeasurementMode = (RohdeSchwarz.ViCom.Net.ACD.SAcdSettings.MeasurementMode.Type)App.Sett.TSMxReceiver_Settings.ACD.MeasurementMode;
                settings.enSensitivity = (RohdeSchwarz.ViCom.Net.ACD.SAcdSettings.Sensitivity.Type)App.Sett.TSMxReceiver_Settings.ACD.Sensitivity;
                settings.dwNumberOfTrialsPerChannel = 1;

                uint minbw = uint.MaxValue;


                bool[] techstate = new bool[] { false, false, false, false };//umts, cdma, evdo, lte
                for (int i = 0; i < App.Sett.TSMxReceiver_Settings.ACD.Data.Count(); i++)
                {
                    if (App.Sett.TSMxReceiver_Settings.ACD.Data[i].Use)
                    {
                        if (App.Sett.TSMxReceiver_Settings.ACD.Data[i].Tech == 2) { minbw = Math.Min(minbw, 5000000); techstate[0] = true; }
                        else if (App.Sett.TSMxReceiver_Settings.ACD.Data[i].Tech == 3) { minbw = Math.Min(minbw, 1250000); techstate[1] = true; }
                        else if (App.Sett.TSMxReceiver_Settings.ACD.Data[i].Tech == 4) { minbw = Math.Min(minbw, 1250000); techstate[2] = true; }
                        else if (App.Sett.TSMxReceiver_Settings.ACD.Data[i].Tech == 5) { minbw = Math.Min(minbw, 1400000); techstate[3] = true; }
                    }
                }
                settings.dwMinimumDetectedBwInHz = minbw;
                uint count = 0;

                for (int i = 0; i < techstate.Length; i++)
                { if (techstate[i]) count++; }

                settings.dwCount = count;
                settings.paTechnologies = new RohdeSchwarz.ViCom.Net.ACD.STechnologySettings[count];

                int index = 0, index2 = 0;
                ulong one = 1;
                for (int j = 0; j < App.Sett.TSMxReceiver_Settings.ACD.Data.Count(); j++)
                {
                    if (App.Sett.TSMxReceiver_Settings.ACD.Data[j].Use)
                    {
                        if (settings.paTechnologies[index2] == null)
                        {
                            settings.paTechnologies[index] = new RohdeSchwarz.ViCom.Net.ACD.STechnologySettings();
                            settings.paTechnologies[index].enTechnology = (RohdeSchwarz.ViCom.Net.Technology.Type)App.Sett.TSMxReceiver_Settings.ACD.Data[j].Tech;
                            index2 = index;
                            settings.paTechnologies[index2].u64BandIdMask1 |= (one << App.Sett.TSMxReceiver_Settings.ACD.Data[j].Band);
                            index++;
                        }
                        else
                        {
                            if (settings.paTechnologies[index2].enTechnology == (RohdeSchwarz.ViCom.Net.Technology.Type)App.Sett.TSMxReceiver_Settings.ACD.Data[j].Tech)
                            {
                                settings.paTechnologies[index2].u64BandIdMask1 |= (one << App.Sett.TSMxReceiver_Settings.ACD.Data[j].Band);
                            }
                            else
                            {
                                settings.paTechnologies[index] = new RohdeSchwarz.ViCom.Net.ACD.STechnologySettings();
                                settings.paTechnologies[index].enTechnology = (RohdeSchwarz.ViCom.Net.Technology.Type)App.Sett.TSMxReceiver_Settings.ACD.Data[j].Tech;
                                index2 = index;
                                settings.paTechnologies[index2].u64BandIdMask1 |= (one << App.Sett.TSMxReceiver_Settings.ACD.Data[j].Band);
                                index++;
                            }
                        }

                    }
                }
                acdInterface.SetAcdSettings(settings);

                ACDListener = new MyAcdDataProcessor();
                acdInterface.RegisterResultDataListener(ACDListener);
                acdBasicInterface.StartMeasurement();
                ACDIsRuning = Runing;
            }
            #region Exception
            catch (RohdeSchwarz.ViCom.Net.CViComError error)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = new Exception("ErrorCode:" + error.ErrorCode + " ErrorString:" + error.ErrorString), ClassName = "TSMxReceiver", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            catch (Exception exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "TSMxReceiver", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            #endregion
            finally { DM -= ACDConnect; }
        }
        private void ACDDisconnect()
        {
            try
            {
                if (acdLoader != null)
                {
                    if (acdLoader.GetBasicInterface().IsMeasurementStarted())
                    {
                        acdLoader.GetBasicInterface().StopMeasurement();
                        acdLoader.GetBasicInterface().HasMeasurementStopped(SDefs.dwDefaultTimeOutInMs);
                    }
                    if (acdLoader.GetBasicInterface() != null && acdLoader.GetBasicInterface().HasMeasurementStopped(SDefs.dwDefaultTimeOutInMs))
                    {
                        acdInterface.UnregisterResultDataListener(ACDListener);
                        ACDIsRuning = !acdLoader.Disconnect();
                        acdLoader.Dispose();
                        DM -= ACDDisconnect;
                    }
                }
            }
            #region Exception
            catch (RohdeSchwarz.ViCom.Net.CViComError error)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = new Exception("ErrorCode:" + error.ErrorCode + " ErrorString:" + error.ErrorString), ClassName = "TSMxReceiver", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            catch (Exception exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "TSMxReceiver", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            #endregion
            #region old
            //try
            //{
            //    if (acdLoader != null && acdLoader.GetBasicInterface().IsMeasurementStarted())
            //    {
            //        acdLoader.GetBasicInterface().StopMeasurement();
            //        if (acdLoader.GetBasicInterface().HasMeasurementStopped(SDefs.dwDefaultTimeOutInMs))
            //        {
            //            acdInterface.UnregisterResultDataListener(ACDListener);

            //            ACDIsRuning = !acdLoader.Disconnect();
            //            acdLoader.Dispose();
            //            DM -= ACDDisconnect;
            //        }
            //    }
            //    else if (acdLoader.GetBasicInterface() != null && acdLoader.GetBasicInterface().HasMeasurementStopped(SDefs.dwDefaultTimeOutInMs))
            //    {
            //        acdInterface.UnregisterResultDataListener(ACDListener);
            //        ACDIsRuning = !acdLoader.Disconnect();
            //        acdLoader.Dispose();
            //        DM -= ACDDisconnect;
            //    }
            //}
            //#region Exception
            //catch (RohdeSchwarz.ViCom.Net.CViComError error)
            //{
            //    MainWindow.exp.ExceptionData = new ExData() { ex = new Exception("ErrorCode:" + error.ErrorCode + " ErrorString:" + error.ErrorString), ClassName = "TSMxReceiver", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            //}
            //catch (Exception exp)
            //{
            //    MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "TSMxReceiver", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            //}
            //#endregion
            #endregion
        }
        #endregion ACD

        #region MeasMon
        long GSMBandMeasTicks = 0;
        bool GSMBandMeas = false;
        GSMBandMeas GSMBandMeasSelected = new Equipment.GSMBandMeas() { };
        private void SetMeasMonChannel()
        {
            #region
            if (Run && IsRuning == true && MainWindow.gps.GNSSIsValid && MainWindow.db_v2.MeasMon.Data.Count > 0)//)//
            {
                //((MainWindow)App.Current.MainWindow).Message = DateTime.Now.ToString() + "";
                try
                {
                    if (MeasMonItem != null && MeasMonItem.AllTraceCountToMeas == MeasMonItem.AllTraceCount/* || MeasMonItem.Trace[0].Freq == Trace[0].Freq || MeasMonItem.Trace[MeasMonItem.Trace.Length - 1].Freq == Trace[Trace.Length - 1].Freq)*/) //if (MeasTraceCount < 0 && MainWindow.db.MonMeas.Count > 0)
                    {
                        double dist = 0, ang = 0;
                        if (GSMBandMeasSelected.latitude > 0 && GSMBandMeasSelected.longitude > 0)
                        {
                            MainWindow.help.calcDistance(
                                  (double)GSMBandMeasSelected.latitude,
                                  (double)GSMBandMeasSelected.longitude,
                                  (double)MainWindow.gps.LatitudeDecimal,
                                  (double)MainWindow.gps.LongitudeDecimal,
                                  out dist, out ang);
                        }
                        if ((decimal)dist > MainWindow.db_v2.Atdi_LevelResults_DistanceStep ||
                            new TimeSpan(MainWindow.gps.LocalTime.Ticks - GSMBandMeasTicks) > MainWindow.db_v2.Atdi_LevelsMeasurementsCar_TimeStep)
                        {
                            if (GSMBandMeas == false) GSMBandMeas = true;
                        }
                        if (GSMBandMeas)
                        {
                            #region
                            int Ind = int.MaxValue, ii = -1;
                            if (GSMBandMeasSelected.Count >= GSMBandMeasSelected.CountAll)
                            {

                                for (int i = MainWindow.db_v2.GSMBandMeass.Count - 1; i > -1; i--)
                                {
                                    if (MainWindow.db_v2.GSMBandMeass[i].CountAll < Ind)
                                    {
                                        Ind = MainWindow.db_v2.GSMBandMeass[i].CountAll;
                                        ii = i;
                                    }
                                }
                                if (ii > -1)
                                {
                                    #region
                                    GSMBandMeasSelected.select = false;
                                    GSMBandMeasSelected = MainWindow.db_v2.GSMBandMeass[ii];
                                    GSMBandMeasSelected.select = true;
                                    GSMBandMeasSelected.CountAll = GSMBandMeasSelected.CountAll + 1;// + MeasTraceCountOnFreq;
                                    long beginTiks = DateTime.Now.Ticks;
                                    if (RFPowerScanBasicInterface != null && RFPowerScanBasicInterface.IsMeasurementStarted())
                                    {
                                        RFPowerScanBasicInterface.StopMeasurement();
                                        RFPowerScanBasicInterface.HasMeasurementStopped(SDefs.dwDefaultTimeOutInMs);
                                        rSSweepSettings = RfPowerScanSetSweepSettingsBand(GSMBandMeasSelected.Start, GSMBandMeasSelected.Stop, GSMBandMeasSelected.TracePoints);
                                        RFPowerScanInterface.SetSweepSettings(rSSweepSettings);
                                        RFPowerScanBasicInterface.StartMeasurement();

                                        FreqSet = true;
                                        ticks++;
                                    }
                                    #endregion

                                }
                            }

                            #endregion }
                        }
                        else
                        {
                            #region
                            #region
                            int Ind = int.MaxValue, ii = -1;
                            for (int i = MainWindow.db_v2.MeasMon.Data.Count - 1; i > -1; i--)
                            {
                                //MainWindow.db_v2.MeasMon.Data[i].ThisToMeas = false;
                                if (MainWindow.db_v2.MeasMon.Data[i].ThisIsMaximumSignalAtThisFrequency == true &&
                                    MainWindow.db_v2.MeasMon.Data[i].AllTraceCountToMeas < Ind)
                                {
                                    if (MainWindow.db_v2.MeasMon.Data[i].Techonology == "GSM" && MainWindow.db_v2.MeasMon.Data[i].Power > DetectionLevelGSM)
                                    {
                                        for (int j = 0; j < IdentificationData.GSM.BTS.Count; j++)
                                        {
                                            //тут проверяем купатыся чы некупатыся
                                            if (IdentificationData.GSM.BTS[j].BSIC == MainWindow.db_v2.MeasMon.Data[i].TechSubInd && IdentificationData.GSM.BTS[j].GCID == MainWindow.db_v2.MeasMon.Data[i].GCID)
                                            {
                                                Ind = MainWindow.db_v2.MeasMon.Data[i].AllTraceCount;
                                                ii = i;
                                            }
                                        }
                                    }
                                    else if (MainWindow.db_v2.MeasMon.Data[i].Techonology == "UMTS" && MainWindow.db_v2.MeasMon.Data[i].Power > DetectionLevelUMTS)
                                    {
                                        for (int j = 0; j < IdentificationData.UMTS.BTS.Count; j++)
                                        {
                                            if (IdentificationData.UMTS.BTS[j].SC == MainWindow.db_v2.MeasMon.Data[i].TechSubInd && IdentificationData.UMTS.BTS[j].GCID == MainWindow.db_v2.MeasMon.Data[i].GCID)
                                            {
                                                Ind = MainWindow.db_v2.MeasMon.Data[i].AllTraceCount;
                                                ii = i;
                                            }
                                        }
                                    }
                                    else if (MainWindow.db_v2.MeasMon.Data[i].Techonology == "LTE" && MainWindow.db_v2.MeasMon.Data[i].Power > DetectionLevelLTE)
                                    {
                                        for (int j = 0; j < IdentificationData.LTE.BTS.Count; j++)
                                        {
                                            if (IdentificationData.LTE.BTS[j].PCI == MainWindow.db_v2.MeasMon.Data[i].TechSubInd && IdentificationData.LTE.BTS[j].GCID == MainWindow.db_v2.MeasMon.Data[i].GCID)
                                            {
                                                Ind = MainWindow.db_v2.MeasMon.Data[i].AllTraceCount;
                                                ii = i;
                                            }
                                        }
                                    }
                                    else if (MainWindow.db_v2.MeasMon.Data[i].Techonology == "CDMA" && MainWindow.db_v2.MeasMon.Data[i].Power > DetectionLevelCDMA)
                                    {
                                        for (int j = 0; j < IdentificationData.CDMA.BTS.Count; j++)
                                        {
                                            if (IdentificationData.CDMA.BTS[j].GCID == MainWindow.db_v2.MeasMon.Data[i].GCID)
                                            {
                                                Ind = MainWindow.db_v2.MeasMon.Data[i].AllTraceCount;
                                                ii = i;
                                            }
                                        }
                                    }
                                    //else if (MainWindow.db_v2.MonMeas[i].Techonology == "WIMAX")
                                    //{
                                    //    foreach (TopNWiMax twimax in MainWindow.IdfData.WiMax)
                                    //    {
                                    //        if (twimax.GCID == MainWindow.db_v2.MonMeas[i].GCID && twimax.RSSI > lev)
                                    //        {
                                    //            Ind = MainWindow.db_v2.MonMeas[i].AllTraceCount;
                                    //            ii = i;
                                    //        }
                                    //    }
                                    //}
                                    else if (MainWindow.db_v2.MeasMon.Data[i].Techonology == "WRLS")
                                    {
                                        //foreach (WRLSBTSData twifi in IdentificationData.WRLSBTS)
                                        //{
                                        //    if (twifi.GCID == MainWindow.db_v2.MonMeas[i].GCID)
                                        //    {
                                        //        Ind = MainWindow.db_v2.MonMeas[i].AllTraceCount;
                                        //        ii = i;
                                        //    }
                                        //}
                                    }
                                }
                            }
                            #endregion
                            FreqSet = false;
                            if (ii > -1)
                            {
                                MeasMonItem.ThisToMeas = false;
                                MainWindow.db_v2.MeasMon.Data[ii].ThisToMeas = true;
                                DB.MeasData temp = MainWindow.db_v2.MeasMon.Data[ii];

                                //string tec = MainWindow.db_v2.MeasMon.Data[ii].Techonology;
                                //&& MainWindow.db.MonMeas[Ind].Power > MainWindow.db.MonMeas.Where(x => x.FreqDN == (MainWindow.db.MonMeas[Ind].FreqDN +200000)).First().Power + 35 && MainWindow.db.MonMeas[Ind].Power > MainWindow.db.MonMeas.Where(x => x.FreqDN == (MainWindow.db.MonMeas[Ind].FreqDN - 200000)).First().Power + 35
                                if (temp.Techonology == "GSM")
                                {
                                    #region
                                    if (IdentificationData.GSM.BTS != null && IdentificationData.GSM.BTS.Count > 0)
                                    {
                                        MeasMonItem = temp;
                                        MeasMonItem.AllTraceCountToMeas = ((MeasMonItem.AllTraceCount + MeasTraceCountOnFreq) / 10) * 10;
                                        //смена частоты
                                        long beginTiks = DateTime.Now.Ticks;
                                        if (FreqCentr != MeasMonItem.SpecData.FreqCentr || FreqSpan != MeasMonItem.SpecData.FreqSpan)
                                        {
                                            if (RFPowerScanBasicInterface != null && RFPowerScanBasicInterface.IsMeasurementStarted())
                                            {
                                                RFPowerScanBasicInterface.StopMeasurement();
                                                RFPowerScanBasicInterface.HasMeasurementStopped(SDefs.dwDefaultTimeOutInMs);

                                                rSSweepSettings = RfPowerScanSetSweepSettings1(MeasMonItem.SpecData.FreqCentr, MeasMonItem.SpecData.FreqSpan, MeasMonItem.SpecData.TracePoints);
                                                RFPowerScanInterface.SetSweepSettings(rSSweepSettings);
                                                RFPowerScanBasicInterface.StartMeasurement();
                                            }
                                        }
                                        FreqSet = true;
                                        MeasMonTimeMeas = DateTime.Now.Ticks;
                                        ticks++;
                                        //}
                                    }
                                    #endregion
                                }
                                else if (temp.Techonology == "UMTS")
                                {
                                    #region
                                    if (IdentificationData.UMTS.BTS != null && IdentificationData.UMTS.BTS.Count > 0)
                                    {
                                        MeasMonItem = temp;
                                        MeasMonItem.AllTraceCountToMeas = ((MeasMonItem.AllTraceCount + MeasTraceCountOnFreq) / 10) * 10;
                                        long beginTiks = DateTime.Now.Ticks;
                                        if (FreqCentr != MeasMonItem.FreqDN || FreqSpan != MeasMonItem.SpecData.FreqSpan)
                                        {
                                            if (RFPowerScanBasicInterface != null && RFPowerScanBasicInterface.IsMeasurementStarted())
                                            {
                                                RFPowerScanBasicInterface.StopMeasurement();
                                                RFPowerScanBasicInterface.HasMeasurementStopped(SDefs.dwDefaultTimeOutInMs);

                                                rSSweepSettings = RfPowerScanSetSweepSettings1(MeasMonItem.FreqDN, MeasMonItem.SpecData.FreqSpan, MeasMonItem.SpecData.Trace.Length);
                                                RFPowerScanInterface.SetSweepSettings(rSSweepSettings);
                                                RFPowerScanBasicInterface.StartMeasurement();
                                            }
                                        }
                                        FreqSet = true;
                                        MeasMonTimeMeas = DateTime.Now.Ticks;
                                        ticks++;
                                    }
                                    #endregion
                                }
                                else if (temp.Techonology == "LTE")
                                {
                                    #region
                                    if (IdentificationData.LTE.BTS != null && IdentificationData.LTE.BTS.Count > 0)
                                    {
                                        MeasMonItem = temp;
                                        MeasMonItem.AllTraceCountToMeas = ((MeasMonItem.AllTraceCount + MeasTraceCountOnFreq) / 10) * 10;
                                        long beginTiks = DateTime.Now.Ticks;
                                        if (FreqCentr != MeasMonItem.FreqDN || FreqSpan != MeasMonItem.SpecData.FreqSpan)
                                        {
                                            if (RFPowerScanBasicInterface != null && RFPowerScanBasicInterface.IsMeasurementStarted())
                                            {
                                                RFPowerScanBasicInterface.StopMeasurement();
                                                RFPowerScanBasicInterface.HasMeasurementStopped(SDefs.dwDefaultTimeOutInMs);

                                                rSSweepSettings = RfPowerScanSetSweepSettings1(MeasMonItem.FreqDN, MeasMonItem.SpecData.FreqSpan, MeasMonItem.SpecData.Trace.Length);
                                                RFPowerScanInterface.SetSweepSettings(rSSweepSettings);
                                                RFPowerScanBasicInterface.StartMeasurement();
                                            }
                                        }
                                        FreqSet = true;
                                        MeasMonTimeMeas = DateTime.Now.Ticks;
                                        ticks++;
                                    }
                                    #endregion
                                }
                                else if (temp.Techonology == "CDMA")
                                {
                                    #region
                                    if (IdentificationData.CDMA.BTS != null && IdentificationData.CDMA.BTS.Count > 0)
                                    {
                                        MeasMonItem = temp;
                                        MeasMonItem.AllTraceCountToMeas = ((MeasMonItem.AllTraceCount + MeasTraceCountOnFreq) / 10) * 10;
                                        long beginTiks = DateTime.Now.Ticks;
                                        if (FreqCentr != MeasMonItem.FreqDN || FreqSpan != MeasMonItem.SpecData.FreqSpan)
                                        {
                                            if (RFPowerScanBasicInterface != null && RFPowerScanBasicInterface.IsMeasurementStarted())
                                            {
                                                RFPowerScanBasicInterface.StopMeasurement();
                                                RFPowerScanBasicInterface.HasMeasurementStopped(SDefs.dwDefaultTimeOutInMs);

                                                rSSweepSettings = RfPowerScanSetSweepSettings1(MeasMonItem.FreqDN, MeasMonItem.SpecData.FreqSpan, MeasMonItem.SpecData.Trace.Length);
                                                RFPowerScanInterface.SetSweepSettings(rSSweepSettings);
                                                RFPowerScanBasicInterface.StartMeasurement();
                                            }
                                        }
                                        FreqSet = true;
                                        MeasMonTimeMeas = DateTime.Now.Ticks;
                                        ticks++;
                                    }
                                    #endregion
                                }
                                //else if (tec == "WIMAX")
                                //{
                                //    #region
                                //    if (MainWindow.RCR.WiMax != null && MainWindow.RCR.WiMax.Count > 0)
                                //    {
                                //        ObservableCollection<TopNWiMax> wimax = MainWindow.RCR.WiMax;
                                //        int wimaxind = -1;
                                //        if (wimax != null)
                                //            for (int i = 0; i < wimax.Count; i++)
                                //            {
                                //                if (wimax[i].GCID == temp.GCID && wimax[i].Channel.FreqDn == temp.FreqDN) { wimaxind = i; }
                                //            }
                                //        if (wimaxind > -1 && wimax[wimaxind].RSSI > App.Sett.MeasMons_Settings.MeasMons.Where(x => x.Techonology == DB.Technologys.WIMAX).First().DetectionLevel)
                                //        {
                                //            MeasMonItem = temp;
                                //            MeasMonItem.AllTraceCountToMeas = MeasMonItem.AllTraceCount + MeasTraceCountOnFreq;
                                //            ////MeasTraceCount = MeasMonItem.AllTraceCount + MeasTraceCountOnFreq;
                                //            //SetSettingsToMeasMon(MeasMonItem.FreqDN * 1000000, MeasMonItem.MeasSpan);
                                //            ////MeasTraceCountOnFreq = 10;
                                //        }
                                //    }
                                //    #endregion
                                //}
                                else if (temp.Techonology == "WRLS")
                                {
                                    #region
                                    //if (IdentificationData.WRLSBTS != null && IdentificationData.WRLSBTS.Count > 0)
                                    //{
                                    //    ObservableCollection<WRLSBTSData> wrls = IdentificationData.WRLSBTS;
                                    //    int wrlsind = -1;
                                    //    if (wrls != null)
                                    //        for (int i = 0; i < wrls.Count; i++)
                                    //        {
                                    //            if (wrls[i].GCID == temp.GCID/* && wrls[i].FreqCentr == temp.FreqDN*/) { wrlsind = i; }
                                    //        }
                                    //    if (wrlsind > -1 && wrls[wrlsind].Level > App.Sett.MeasMons_Settings.MeasMons.Where(x => x.Techonology == DB.Technologys.WRLS).First().DetectionLevel)
                                    //    {
                                    //        MeasMonItem = temp;
                                    //        MeasMonItem.AllTraceCountToMeas = MeasMonItem.AllTraceCount + MeasTraceCountOnFreq;
                                    //        long beginTiks = DateTime.Now.Ticks;
                                    //        if (RFPowerScanBasicInterface != null && RFPowerScanBasicInterface.IsMeasurementStarted())
                                    //        {

                                    //            RFPowerScanBasicInterface.StopMeasurement();

                                    //            RFPowerScanBasicInterface.HasMeasurementStopped(SDefs.dwDefaultTimeOutInMs);
                                    //            //RFPowerScanInterface.UnregisterResultDataListener(RFPowerScanListener);

                                    //            //Thread.Sleep(50);
                                    //            //RFPowerScanInterface.RegisterResultDataListener(RFPowerScanListener);

                                    //            rSSweepSettings = RfPowerScanSetSweepSettings1(MeasMonItem.FreqDN, MeasMonItem.MeasSpan);
                                    //            RFPowerScanInterface.SetSweepSettings(rSSweepSettings);
                                    //            RFPowerScanBasicInterface.StartMeasurement();
                                    //            //RFPowerScanListener = new MyRfPowerScanDataProcessor();//



                                    //            //RFPowerScanBasicInterface.StartMeasurement();
                                    //            FreqSet = true;
                                    //            MeasMonTimeMeas = DateTime.Now.Ticks;
                                    //            Time = new TimeSpan(DateTime.Now.Ticks - beginTiks).ToString();

                                    //            ticks++;
                                    //        }
                                    //        ////MeasTraceCountOnFreq = 10;
                                    //        ////MeasTraceCount = MeasMonItem.AllTraceCount + MeasTraceCountOnFreq;
                                    //        //SetSettingsToMeasMon(MeasMonItem.FreqDN * 1000000, MeasMonItem.MeasSpan);

                                    //    }
                                    //}
                                    #endregion
                                }
                                Thread.Sleep(10);
                            }
                            #endregion
                        }
                    }
                }
                #region Exception
                catch (Exception exp)
                {
                    MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "TSMxReceiver", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
                }
                #endregion
            }
            //}));
            #endregion
        }
        private void SetMeasMonBands()
        {
            if (MainWindow.db_v2.MeasMon.Bands.Count == 0) { }
            else { MainWindow.db_v2.MeasMon.Bands.Clear(); }
            for (int i = 0; i < App.Sett.MeasMons_Settings.MeasMonBands.Count(); i++)
            {
                if (App.Sett.MeasMons_Settings.MeasMonBands[i].Use)
                {
                    DB.MeasMonBand mmb = new DB.MeasMonBand() { };
                    mmb.Start = App.Sett.MeasMons_Settings.MeasMonBands[i].Start;
                    mmb.Stop = App.Sett.MeasMons_Settings.MeasMonBands[i].Stop;
                    mmb.Step = App.Sett.MeasMons_Settings.MeasMonBands[i].Step;
                    mmb.TracePoints = (int)((mmb.Stop - mmb.Start) / mmb.Step) + 1;
                    MainWindow.db_v2.MeasMon.Bands.Add(mmb);
                }
            }
        }

        private void SetMeasMonBand()
        {
            #region
            if (Run && IsRuning == true && MainWindow.gps.GNSSIsValid && MainWindow.db_v2.MeasMon.Data.Count > 0)//)//
            {

                try
                {
                    if (MeasMonBandItem != null && MeasMonBandItem.AllTraceCountToMeas == MeasMonBandItem.AllTraceCount)
                    {
                        int Ind = int.MaxValue, ii = -1;
                        for (int i = MainWindow.db_v2.MeasMon.Bands.Count - 1; i > -1; i--)
                        {
                            if (MainWindow.db_v2.MeasMon.Bands[i].AllTraceCountToMeas < Ind)
                            {
                                Ind = MainWindow.db_v2.MeasMon.Bands[i].AllTraceCount;
                                ii = i;
                            }
                        }
                        //настраиваем частоты бенда по ii
                        MeasMonBandItem = MainWindow.db_v2.MeasMon.Bands[ii];
                        #region
                        MeasMonBandItem.AllTraceCountToMeas = MeasMonBandItem.AllTraceCount + MeasTraceCountOnFreq;
                        //смена частоты
                        long beginTiks = DateTime.Now.Ticks;
                        if (RFPowerScanBasicInterface != null && RFPowerScanBasicInterface.IsMeasurementStarted())
                        {
                            RFPowerScanBasicInterface.StopMeasurement();

                            RFPowerScanBasicInterface.HasMeasurementStopped(SDefs.dwDefaultTimeOutInMs);

                            rSSweepSettings = RfPowerScanSetSweepSettingsBand(MeasMonBandItem.Start, MeasMonBandItem.Stop, MeasMonBandItem.TracePoints);
                            RFPowerScanInterface.SetSweepSettings(rSSweepSettings);
                            RFPowerScanBasicInterface.StartMeasurement();

                            FreqSet = true;
                            MeasMonTimeMeas = DateTime.Now.Ticks;
                            Time = new TimeSpan(DateTime.Now.Ticks - beginTiks).ToString();
                            ticks++;
                        }

                        ////MeasTraceCount = MeasMonItem.AllTraceCount + MeasTraceCountOnFreq;
                        //SetSettingsToMeasMon(MeasMonItem.FreqDN * 1000000, MeasMonItem.MeasSpan);
                        ////MeasTraceCountOnFreq = 10;

                        #endregion



                    }
                }
                #region Exception
                catch (Exception exp)
                {
                    MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "TSMxReceiver", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
                }
                #endregion
            }
            //}));
            #endregion
        }

        #endregion
        #endregion


        #region DataProcessors
        #region ParseIDtoDB
        /// <summary>
        /// Возвращает идентификатор как в БД и номер сектора
        /// </summary>
        private static int[] ParseIDtoDB(GSMBTSData data)
        {
            int[] res = new int[] { -1, -1 };
            int s0 = -1, s1 = -1, s2 = -1, s3 = -1;
            Settings.OPSOSIdentification_Set op = new Settings.OPSOSIdentification_Set();
            #region
            s0 = data.MCC;
            s1 = data.MNC;
            s2 = data.LAC;
            for (int i = 0; i < App.Sett.MeasMons_Settings.OPSOSIdentifications.Count(); i++)
            {
                if (App.Sett.MeasMons_Settings.OPSOSIdentifications[i].MCC == s0 &&
                   App.Sett.MeasMons_Settings.OPSOSIdentifications[i].MNC == s1 &&
                   App.Sett.MeasMons_Settings.OPSOSIdentifications[i].Techonology == "GSM")
                {
                    op = App.Sett.MeasMons_Settings.OPSOSIdentifications[i];
                }
            }
            if (op.ID.Length > 0 && op.IDFromIndex == 0) s3 = data.CID;
            #endregion

            res = ParseID(s0, s1, s2, s3, op);
            return res;
        }
        private static int[] ParseIDtoDB(UMTSBTSData data)
        {
            int[] res = new int[] { -1, -1 };
            int s0 = -1, s1 = -1, s2 = -1, s3 = -1;
            Settings.OPSOSIdentification_Set op = new Settings.OPSOSIdentification_Set();
            #region
            s0 = data.MCC;
            s1 = data.MNC;
            s2 = data.LAC;
            for (int i = 0; i < App.Sett.MeasMons_Settings.OPSOSIdentifications.Count(); i++)
            {
                if (App.Sett.MeasMons_Settings.OPSOSIdentifications[i].MCC == s0 &&
                   App.Sett.MeasMons_Settings.OPSOSIdentifications[i].MNC == s1 &&
                   App.Sett.MeasMons_Settings.OPSOSIdentifications[i].Techonology == "UMTS")
                {
                    op = App.Sett.MeasMons_Settings.OPSOSIdentifications[i];
                }
            }
            if (op.ID.Length > 0)
            {
                if (op.IDFromIndex == 0) s3 = data.CID;
                else if (op.IDFromIndex == 1) s3 = data.RNC;
                else if (op.IDFromIndex == 2) s3 = data.UCID;
            }
            #endregion
            res = ParseID(s0, s1, s2, s3, op);
            return res;
        }
        private static int[] ParseIDtoDB(LTEBTSData data)
        {
            int[] res = new int[] { -1, -1 };
            int s0 = -1, s1 = -1, s2 = -1, s3 = -1;
            Settings.OPSOSIdentification_Set op = new Settings.OPSOSIdentification_Set();
            #region
            s0 = data.MCC;
            s1 = data.MNC;
            for (int i = 0; i < App.Sett.MeasMons_Settings.OPSOSIdentifications.Count(); i++)
            {
                if (App.Sett.MeasMons_Settings.OPSOSIdentifications[i].MCC == s0 &&
                   App.Sett.MeasMons_Settings.OPSOSIdentifications[i].MNC == s1 &&
                   App.Sett.MeasMons_Settings.OPSOSIdentifications[i].Techonology == "LTE")
                {
                    op = App.Sett.MeasMons_Settings.OPSOSIdentifications[i];
                }
            }
            if (op.ID.Length > 0)
            {
                if (op.AreaFromIndex == 0) s2 = data.TAC;
                else if (op.AreaFromIndex == 1) s2 = data.PCI;
                if (op.IDFromIndex == 0) s3 = data.eNodeBId;
                else if (op.IDFromIndex == 1) s3 = Convert.ToInt32(data.ECI28);
                else if (op.IDFromIndex == 2) s3 = data.CID;
                else if (op.IDFromIndex == 3) s3 = data.TAC;
                else if (op.IDFromIndex == 4) s3 = data.PCI;
            }
            #endregion

            res = ParseID(s0, s1, s2, s3, op);
            return res;
        }
        private static int[] ParseIDtoDB(CDMABTSData data)
        {
            int[] res = new int[] { -1, -1 };
            int s0 = -1, s1 = -1, s2 = -1, s3 = -1;
            Settings.OPSOSIdentification_Set op = new Settings.OPSOSIdentification_Set();
            #region
            //if (data is Equipment.LTEBTSData)
            //{
            //    Equipment.LTEBTSData dt = (Equipment.LTEBTSData)data;
            //    s0 = dt.MCC;
            //    s1 = dt.MNC;
            //    for (int i = 0; i < App.Sett.MeasMons_Settings.OPSOSIdentifications.Count(); i++)
            //    {
            //        if (App.Sett.MeasMons_Settings.OPSOSIdentifications[i].MCC == s0 &&
            //           App.Sett.MeasMons_Settings.OPSOSIdentifications[i].MNC == s1 &&
            //           App.Sett.MeasMons_Settings.OPSOSIdentifications[i].Techonology == "LTE")
            //        {
            //            op = App.Sett.MeasMons_Settings.OPSOSIdentifications[i];
            //        }
            //    }
            //    if (op.ID.Length > 0)
            //    {
            //        if (op.AreaFromIndex == 0) s2 = dt.TAC;
            //        else if (op.AreaFromIndex == 1) s2 = dt.PCI;
            //        if (op.IDFromIndex == 0) s3 = dt.eNodeBId;
            //        else if (op.IDFromIndex == 1) s3 = Convert.ToInt32(dt.ECI28);
            //        else if (op.IDFromIndex == 2) s3 = dt.CID;
            //        else if (op.IDFromIndex == 3) s3 = dt.TAC;
            //        else if (op.IDFromIndex == 4) s3 = dt.PCI;
            //    }
            //}
            #endregion


            return res;
        }
        private static int[] ParseID(int s0, int s1, int s2, int s3, Settings.OPSOSIdentification_Set opset)
        {
            int[] res = new int[] { -1, -1 };

            string idlength = "";
            for (int j = 0; j < opset.ID.Length; j++)
                idlength += "0";
            string str = string.Format("{0:" + idlength + "}", s3);
            string cid = str;
            int[] CIDarr = opset.ID;
            if (CIDarr.Length > 0)
            {
                cid = "";
                for (int s = 0; s < CIDarr.Length; s++)
                {
                    if (CIDarr[s] > -1) cid += str.Substring(CIDarr[s], 1);
                }
                if (cid.Length > 0) res[0] = Convert.ToInt32(cid);
            }

            string secstr = str;
            int sec = -1;
            int[] SECarr = opset.Sector;
            if (SECarr.Length > 0)
            {
                secstr = "";
                for (int s = 0; s < SECarr.Length; s++)
                {
                    if (SECarr[s] > -1) secstr += str.Substring(SECarr[s], 1);
                }
                if (secstr.Length > 0) sec = Convert.ToInt32(secstr);
                if (sec > -1)
                {
                    for (int sc = 0; sc < opset.SectorComparisons.Count(); sc++)
                    {
                        if (opset.SectorComparisons[sc].Radio == sec) res[1] = opset.SectorComparisons[sc].Real;
                    }
                }
            }
            return res;
        }
        #endregion ParseIDtoDB
        #region SIB parsers
        public static decimal[] ParseMIBLTE(string mib)
        {
            decimal[] outarray = new decimal[1] { 0 };
            string BW = "";
            int bw = 0;
            string[] stringSeparators = new string[] { "\r\n" };
            string[] sa = mib.Split(stringSeparators, StringSplitOptions.None);

            int bwstart = 0, bwstop = 0;
            decimal[] bwa = new decimal[] { 1.4m, 5, 10, 15, 20 };
            for (int i = 0; i < sa.Length; i++)
            {
                if (sa[i].Contains("dl_Bandwidth")) { bwstart = sa[i].IndexOf("(") + 1; bwstop = sa[i].IndexOf(")"); bw = Convert.ToInt32(sa[i].Substring(bwstart, bwstop - bwstart), 16); BW = sa[i].Substring(bwstart, bwstop - bwstart); }
            }
            outarray[0] = bwa[bw - 1] * 1000000;
            return outarray;
        }
        public static int[] ParseSIB1LTE(string sib)
        {
            int[] outarray = new int[6] { 0, 0, 0, 0, 0, 0 };
            string MCC = "", MNC = "";
            int mcc = 0, mnc = 0, tac = 0, celid28 = 0, enodbid = 0, celid = 0;
            string[] stringSeparators = new string[] { "\r\n" };
            string[] sa = sib.Split(stringSeparators, StringSplitOptions.None);
            int mccpos = 0, mcclenght = 0;
            int mncpos = 0, mnclenght = 0;
            int tacstart = 0, tacstop = 0;
            int celidstart = 0, celidstop = 0;
            for (int i = 0; i < sa.Length; i++)
            {
                if (sa[i].Contains("mcc")) { mccpos = i; mcclenght = int.Parse(sa[i].Replace("mcc", "").Replace(" ", "").TrimEnd()); }
                if (i > mccpos && i <= mccpos + mcclenght/*sa[i].Contains("MCC_MNC_Digit")*/) { MCC += sa[i].Replace("MCC_MNC_Digit", "").Replace(" ", "").TrimEnd(); }
                if (sa[i].Contains("mnc")) { mncpos = i; mnclenght = int.Parse(sa[i].Replace("mnc", "").Replace(" ", "").TrimEnd()); }
                if (i > mncpos && i <= mncpos + mnclenght/*sa[i].Contains("MCC_MNC_Digit")*/) { MNC += sa[i].Replace("MCC_MNC_Digit", "").Replace(" ", "").TrimEnd(); }
                if (sa[i].Contains("trackingAreaCode")) { tacstart = sa[i].IndexOf("(") + 1; tacstop = sa[i].IndexOf(")"); tac = Convert.ToInt32(sa[i].Substring(tacstart, tacstop - tacstart), 16); }
                if (sa[i].Contains("cellIdentity"))
                {
                    celidstart = sa[i].IndexOf("(0x") + 3; celidstop = sa[i].IndexOf(")");
                    celid28 = Convert.ToInt32(sa[i].Substring(celidstart, celidstop - celidstart), 16);
                    enodbid = Convert.ToInt32(sa[i].Substring(celidstart, celidstop - celidstart - 2), 16);
                    celid = Convert.ToInt16(sa[i].Substring(celidstop - 2, 2), 16);
                }
            }
            int.TryParse(MCC, out mcc);
            int.TryParse(MNC, out mnc);
            outarray[0] = mcc;
            outarray[1] = mnc;
            outarray[2] = tac;
            outarray[3] = celid28;
            outarray[4] = enodbid;
            outarray[5] = celid;
            return outarray;
        }
        /// <summary>
        /// парсим системную инфу
        /// </summary>
        /// <param name="si"></param>
        /// <returns>[SID, BaseID,]</returns>
        public static int[] ParseEVDO_SECTOR_PARAMETERS(string si)
        {
            int[] outarray = new int[2] { 0, 0 };

            int BaseID = 0;
            int SID = 0;
            string[] stringSeparators = new string[] { "\r\n" };
            string[] sa = si.Split(stringSeparators, StringSplitOptions.None);
            for (int i = 0; i < sa.Length; i++)
            {
                if (sa[i].Contains("SectorID"))
                {
                    int baseidstart = sa[i].IndexOf("(") + 1;
                    int baseidstop = sa[i].IndexOf(")");
                    string baseid = sa[i].Substring(baseidstart, baseidstop - baseidstart);
                    SID = (Convert.ToInt32(baseid.Substring(5, 4), 16)) / 4;
                    BaseID = Convert.ToInt32(baseid.Substring(baseid.Length - 5, 5), 16);
                }
            }
            outarray[0] = SID;
            outarray[1] = BaseID;
            return outarray;
        }
        /// <summary>
        /// парсим системную инфу
        /// </summary>
        /// <param name="si"></param>
        /// <returns>[BaseID,]</returns>
        public static decimal[] ParseCDMA_SYS_PARAMS(string si)
        {
            decimal[] outarray = new decimal[1] { 0 };

            int BaseID = 0;
            string[] stringSeparators = new string[] { "\r\n" };
            string[] sa = si.Split(stringSeparators, StringSplitOptions.None);
            int baseidstart = 0;
            for (int i = 0; i < sa.Length; i++)
            {
                if (sa[i].Contains("BASE_ID"))
                {
                    baseidstart = sa[i].IndexOf(":") + 1;
                    BaseID = Convert.ToInt32(sa[i].Substring(baseidstart));
                }
            }
            outarray[0] = BaseID;
            return outarray;
        }/// <summary>
         /// парсим системную инфу
         /// </summary>
         /// <param name="si"></param>
         /// <returns>[MCC,MNC]</returns>
        public static decimal[] ParseCDMA_EXT_SYS_PARAMS(string si)
        {
            decimal[] outarray = new decimal[2] { 0, 0 };

            int MCC = 0, MNC = 0;
            string[] stringSeparators = new string[] { "\r\n" };
            string[] sa = si.Split(stringSeparators, StringSplitOptions.None);
            for (int i = 0; i < sa.Length; i++)
            {
                if (sa[i].Contains("MCC"))
                {
                    int MCCstart = sa[i].IndexOf(":") + 1;
                    MCC = Convert.ToInt32(sa[i].Substring(MCCstart));
                }
                if (sa[i].Contains("IMSI_11_12"))
                {
                    int MNCstart = sa[i].IndexOf(":") + 1;
                    int MNCstop = sa[i].IndexOf("(");
                    MNC = Convert.ToInt32(sa[i].Substring(MNCstart, MNCstop - MNCstart));
                }
            }
            outarray[0] = MCC;
            outarray[1] = MNC;
            return outarray;
        }
        #endregion
        private class MyGpsDataProcessor : RohdeSchwarz.ViCom.Net.GPS.CViComGpsInterfaceDataProcessor
        {
            public override void RegisterScannerId(ushort dwScannerDataId)
            {
                //MainWindow.exp.ExceptionData = new ExData() { ex = new Exception("ID"), ClassName = "TSMxReceiver", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name + "\r\n" + dwScannerDataId };
            }

            public override void RemoveScannerId(ushort dwScannerDataId)
            {
                //MainWindow.exp.ExceptionData = new ExData() { ex = new Exception("ID"), ClassName = "TSMxReceiver", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name + "\r\n" + dwScannerDataId };
            }

            public override void OnScannerDataMeasured(RohdeSchwarz.ViCom.Net.GPS.SMeasResult pData)
            {
                try
                {
                    IsRuningT = true;
                    _LastUpdate = DateTime.Now.Ticks;
                    foreach (SGPSMessage sgpsm in pData.ListOfMessages)
                    {
                        if (sgpsm.enMessageFormat == GPSMessageFormat.Type.VICOM_GPS_FORMAT_NMEA)
                        {
                            MainWindow.gps.ReadNMEAData(System.Text.UnicodeEncoding.UTF8.GetString(sgpsm.pbMessageText));
                            MainWindow.gps.GNSSAntennaState = pData.sReceiverInfo.enAntennaState.ToString();
                        }
                    }
                    //MainWindow.gps.GPSIsValid 

                    //Console.WriteLine("New GPS result: {0}s {1}° {2}° \r\n{3}",
                    //    pData.sPosition.dTime,
                    //    pData.sPosition.dLatitude,
                    //    pData.sPosition.dLongitude,
                    //    tt
                    //    );

                    //foreach (SGPSMessage sgpsm in pData.ListOfMessages)
                    //Console.WriteLine("New GPS result: {0}", System.Text.UnicodeEncoding.UTF8.GetString(sgpsm.pbMessageText));
                }
                #region Exception
                catch (RohdeSchwarz.ViCom.Net.CViComError error)
                {
                    MainWindow.exp.ExceptionData = new ExData() { ex = error, ClassName = "TSMxReceiver", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
                }
                catch (Exception exp)
                {
                    MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "TSMxReceiver", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
                }
                #endregion
            }
        }

        static double LevelIsMaxIfMoreBy = 5;
        static double LevelDifferenceToRemove = 5;
        private class MyGsmDataProcessor : RohdeSchwarz.ViCom.Net.GSM.CViComGsmInterfaceDataProcessor
        {
            public override void RegisterScannerId(ushort dwScannerDataId)
            {
                //MainWindow.exp.ExceptionData = new ExData() { ex = new Exception("ID"), ClassName = "TSMxReceiver", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name + "\r\n" + dwScannerDataId };
            }
            public override void RemoveScannerId(ushort dwScannerDataId)
            {
                //MainWindow.exp.ExceptionData = new ExData() { ex = new Exception("ID"), ClassName = "TSMxReceiver", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name + "\r\n" + dwScannerDataId };
            }
            public override void OnScannerDataMeasured(RohdeSchwarz.ViCom.Net.GSM.SMeasResult pData)
            {
                IsRuningT = true;
                _LastUpdate = DateTime.Now.Ticks;
                if (IdentificationData.GSM.IsRuning == true)
                {
                    //App.Current.Dispatcher.BeginInvoke((Action)(() =>
                    //{
                    try
                    {
                        //BSIC
                        if (pData.ListSCHInfoResults.Count > 0)
                        {
                            foreach (var ssch in pData.ListSCHInfoResults)
                            {
                                #region
                                bool find = false;
                                for (int i = 0; i < IdentificationData.GSM.BTS.Count; i++)
                                {
                                    if (IdentificationData.GSM.BTS[i].FreqIndex == ssch.dwFrequencyIndex && IdentificationData.GSM.BTS[i].BSIC == Convert.ToInt32(Convert.ToString(ssch.wBSIC, 8)))
                                    {
                                        IdentificationData.GSM.BTS[i].IndSCHInfo = ssch.dwIndicatorOfSCHInfo;
                                        IdentificationData.GSM.BTS[i].IndFirstSCHInfo = ssch.dwIndicatorOfFirstSCHInfo;
                                        find = true;
                                    }
                                    #region
                                    else if (IdentificationData.GSM.BTS[i].FreqIndex == ssch.dwFrequencyIndex && IdentificationData.GSM.BTS[i].BSIC == -1)
                                    {
                                        IdentificationData.GSM.BTS[i].TimeOfSlotInSec = ssch.dTimeOfSlotInSec;
                                        IdentificationData.GSM.BTS[i].BSIC = Convert.ToInt32(Convert.ToString(ssch.wBSIC, 8));
                                        IdentificationData.GSM.BTS[i].IndSCHInfo = ssch.dwIndicatorOfSCHInfo;
                                        IdentificationData.GSM.BTS[i].IndFirstSCHInfo = ssch.dwIndicatorOfFirstSCHInfo;
                                        find = true;
                                    }
                                    #endregion
                                }
                                if (find == false)
                                {
                                    Equipment.GSM_Channel t = MainWindow.help.GetGSMCHfromFreqDN(GSMUniFreq[(int)ssch.dwFrequencyIndex]);
                                    GSMBTSData dt = new GSMBTSData()
                                    {
                                        BSIC = Convert.ToInt32(Convert.ToString(ssch.wBSIC, 8)),
                                        IndSCHInfo = ssch.dwIndicatorOfSCHInfo,
                                        IndFirstSCHInfo = ssch.dwIndicatorOfFirstSCHInfo,
                                        FreqIndex = ssch.dwFrequencyIndex,
                                        ARFCN = t.ARFCN,
                                        FreqDn = t.FreqDn,
                                        FreqUp = t.FreqUp,
                                        StandartSubband = t.StandartSubband,
                                        //Channel = t
                                        //TimeOfSlotInSec = ssch.dTimeOfSlotInSec
                                    };
                                    GUIThreadDispatcher.Instance.Invoke(() =>
                                    {
                                        IdentificationData.GSM.BTS.Add(dt);
                                    });

                                }

                                #endregion
                            }
                        }
                        //POWER
                        if (pData.ListPowerResults.Count > 0)
                        {
                            foreach (var pr in pData.ListPowerResults)
                            {
                                #region
                                if (pr.dwIndicatorOfSCHInfo != 4294967295)// && pr.eMeasMode == RohdeSchwarz.ViCom.Net.GSM.SMeasResult.SPowerResult.etMeasMode.MEASMODE_DEMOD)// && pr.eMeasType == RohdeSchwarz.ViCom.Net.GSM.SMeasResult.SPowerResult.etMeasType.MEASTYPE_POWCH)
                                {
                                    bool find = false;
                                    for (int i = 0; i < IdentificationData.GSM.BTS.Count; i++)
                                    {

                                        if (IdentificationData.GSM.BTS[i].FreqIndex == pr.dwFrequencyIndex && pr.dwIndicatorOfSCHInfo == IdentificationData.GSM.BTS[i].IndSCHInfo)
                                        {
                                            IdentificationData.GSM.BTS[i].Power = pr.sPowerInDBm100 * 0.01;
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
                                            App.Current.Dispatcher.Invoke((Action)(() =>
                                            {
                                                IdentificationData.GSM.BTS[i].ThisIsMaximumSignalAtThisFrequency = freqLevelMax;
                                            }));
                                            if (pr.psCarrierToInterferenceInDB100 != null)
                                                IdentificationData.GSM.BTS[i].CarToInt = (double)pr.psCarrierToInterferenceInDB100 * 0.01;
                                            IdentificationData.GSM.BTS[i].TimeOfSlotInSec = pr.dTimeOfSlotInSec - pData.u64DeviceTimeInNs / 1000000000;

                                            find = true;
                                            #region LevelMeasurementsCar
                                            if (MainWindow.gps.Altitude != 0 && MainWindow.gps.LatitudeDecimal != 0 && MainWindow.gps.LongitudeDecimal != 0)
                                            {
                                                double dist = 0, ang = 0;
                                                if (IdentificationData.GSM.BTS[i].level_results.Count > 0)
                                                {
                                                    MainWindow.help.calcDistance(
                                                          (double)IdentificationData.GSM.BTS[i].level_results[IdentificationData.GSM.BTS[i].level_results.Count() - 1].location.latitude,
                                                          (double)IdentificationData.GSM.BTS[i].level_results[IdentificationData.GSM.BTS[i].level_results.Count() - 1].location.longitude,
                                                          (double)MainWindow.gps.LatitudeDecimal,
                                                          (double)MainWindow.gps.LongitudeDecimal,
                                                          out dist, out ang);
                                                }
                                                if ((IdentificationData.GSM.BTS[i].level_results.Count == 0 ||
                                                    (decimal)dist > MainWindow.db_v2.Atdi_LevelResults_DistanceStep &&
                                                    IdentificationData.GSM.BTS[i].level_results[IdentificationData.GSM.BTS[i].level_results.Count - 1].level_dbm != IdentificationData.GSM.BTS[i].Power) ||
                                                    new TimeSpan(MainWindow.gps.LocalTime.Ticks - IdentificationData.GSM.BTS[i].level_results[IdentificationData.GSM.BTS[i].level_results.Count() - 1].measurement_time.Ticks) > MainWindow.db_v2.Atdi_LevelsMeasurementsCar_TimeStep)
                                                {
                                                    #region
                                                    DB.localatdi_level_meas_result l = new DB.localatdi_level_meas_result()
                                                    {
                                                        difference_time_stamp_ns = (decimal)Equipment.IdentificationData.GSM.BTS[i].TimeOfSlotInSec,
                                                        level_dbm = Equipment.IdentificationData.GSM.BTS[i].Power,
                                                        //level_dbmkvm = 0,
                                                        measurement_time = Equipment.IdentificationData.GSM.BTS[i].LastLevelUpdete,
                                                        saved_in_db = false,
                                                        saved_in_result = false,
                                                        location = new DB.localatdi_geo_location()
                                                        {
                                                            //agl = 0,
                                                            asl = MainWindow.gps.Altitude,
                                                            latitude = (double)MainWindow.gps.LatitudeDecimal,
                                                            longitude = (double)MainWindow.gps.LongitudeDecimal,
                                                        }
                                                    };
                                                    IdentificationData.GSM.BTS[i].LR_NewDataToSave = true;
                                                    GUIThreadDispatcher.Instance.Invoke(() =>
                                                    {
                                                        IdentificationData.GSM.BTS[i].level_results.Add(l);
                                                    });

                                                    #endregion
                                                }
                                            }
                                            #endregion
                                        }
                                    }
                                    if (find == false)
                                    {
                                        Equipment.GSM_Channel t = MainWindow.help.GetGSMCHfromFreqDN(GSMUniFreq[(int)pr.dwFrequencyIndex]);
                                        GSMBTSData dt = new GSMBTSData()
                                        {
                                            FreqIndex = pr.dwFrequencyIndex,
                                            ARFCN = t.ARFCN,
                                            FreqDn = t.FreqDn,
                                            FreqUp = t.FreqUp,
                                            StandartSubband = t.StandartSubband,
                                            //Channel = t,
                                            Power = ((double)pr.sPowerInDBm100) * 0.01,
                                            BSIC = -1,
                                            IndSCHInfo = pr.dwIndicatorOfSCHInfo,
                                            TimeOfSlotInSec = pr.dTimeOfSlotInSec - pData.u64DeviceTimeInNs / 1000000000,
                                            LastLevelUpdete = MainWindow.gps.LocalTime
                                        };
                                        GUIThreadDispatcher.Instance.Invoke(() =>
                                        {
                                            IdentificationData.GSM.BTS.Add(dt);
                                        });
                                    }
                                }
                                #endregion
                            }
                        }

                        //MCC MNC LAC CID
                        if (pData.ListCellIdentResults.Count > 0)
                        {
                            for (int i = 0; i < pData.ListCellIdentResults.Count; i++)
                            {
                                #region
                                bool find = false;
                                for (int ii = 0; ii < IdentificationData.GSM.BTS.Count; ii++)
                                {
                                    if (IdentificationData.GSM.BTS[ii].FreqIndex == pData.ListCellIdentResults[i].dwFrequencyIndex && IdentificationData.GSM.BTS[ii].BSIC == Convert.ToInt32(Convert.ToString(pData.ListSCHInfoResults[i].wBSIC, 8)) /*&& GSMBTSfromDev[ii].GCID == ""*/)
                                    {
                                        if (IdentificationData.GSM.BTS[ii].MCC != Convert.ToInt32(pData.ListCellIdentResults[i].wMCC.ToString("X")) ||
                                            IdentificationData.GSM.BTS[ii].MNC != Convert.ToInt32(pData.ListCellIdentResults[i].wMNC.ToString("X")) ||
                                            IdentificationData.GSM.BTS[ii].LAC != pData.ListCellIdentResults[i].wLAC ||
                                            IdentificationData.GSM.BTS[ii].CID != pData.ListCellIdentResults[i].wCI)
                                        {
                                            IdentificationData.GSM.BTS[ii].MNC = Convert.ToInt32(pData.ListCellIdentResults[i].wMNC.ToString("X"));
                                            IdentificationData.GSM.BTS[ii].MCC = Convert.ToInt32(pData.ListCellIdentResults[i].wMCC.ToString("X"));
                                            IdentificationData.GSM.BTS[ii].LAC = pData.ListCellIdentResults[i].wLAC;// string.Format("{0:00000}", pData.ListCellIdentResults[i].wLAC);//.ToString();
                                            IdentificationData.GSM.BTS[ii].CID = pData.ListCellIdentResults[i].wCI;//string.Format("{0:00000}", pData.ListCellIdentResults[i].wCI);//.ToString();

                                            bool FullData = (IdentificationData.GSM.BTS[ii].MNC.ToString() != "" && IdentificationData.GSM.BTS[ii].MCC.ToString() != "" && IdentificationData.GSM.BTS[ii].LAC.ToString() != "" && IdentificationData.GSM.BTS[ii].CID.ToString() != "");
                                            if (FullData == true && IdentificationData.GSM.BTS[ii].CID > -1)
                                            {
                                                IdentificationData.GSM.BTS[ii].GCID = pData.ListCellIdentResults[i].wMCC.ToString("X") + " " +
                                                   pData.ListCellIdentResults[i].wMNC.ToString("X") + " " +
                                                   string.Format("{0:00000}", pData.ListCellIdentResults[i].wLAC) + " " +//.ToString() + " " +
                                                   string.Format("{0:00000}", pData.ListCellIdentResults[i].wCI);//.ToString();
                                                #region
                                                //int[] CIDarr = new int[] { };
                                                //int[] SECarr = new int[] { };
                                                //int[] CARarr = new int[] { };
                                                //for (int c = 0; c < App.Sett.MeasMons_Settings.OPSOSIdentifications.Count; c++)
                                                //{
                                                //    if (App.Sett.MeasMons_Settings.OPSOSIdentifications[c].MCC == IdentificationData.GSM.BTS[ii].MCC && App.Sett.MeasMons_Settings.OPSOSIdentifications[c].MNC == IdentificationData.GSM.BTS[ii].MNC && App.Sett.MeasMons_Settings.OPSOSIdentifications[c].Techonology == "GSM")
                                                //    {
                                                //        CIDarr = App.Sett.MeasMons_Settings.OPSOSIdentifications[c].ID;
                                                //        SECarr = App.Sett.MeasMons_Settings.OPSOSIdentifications[c].Sector;
                                                //        CARarr = App.Sett.MeasMons_Settings.OPSOSIdentifications[c].Carrier;
                                                //    }
                                                //}
                                                ////int[] arr = App.Sett.MeasMons_Settings.OPSOSIdentifications.Where(x => (x.MCC == GSMBTSfromDev[ii].MCC && x.MNC == GSMBTSfromDev[ii].MNC && x.Techonology == "GSM")).First().CID;
                                                //string str = string.Format("{0:00000}", IdentificationData.GSM.BTS[ii].CID);
                                                //string cid = str;
                                                //if (CIDarr.Length > 0)
                                                //{
                                                //    cid = "";
                                                //    for (int s = 0; s < CIDarr.Length; s++)
                                                //    {
                                                //        cid += str.Substring(CIDarr[s], 1);
                                                //    }
                                                //}
                                                //IdentificationData.GSM.BTS[ii].CIDToDB = Convert.ToInt32(cid);/////// по нему идентификация с бд
                                                #endregion
                                                int[] id = ParseIDtoDB(IdentificationData.GSM.BTS[ii]);
                                                if (id[0] > -1) IdentificationData.GSM.BTS[ii].CIDToDB = id[0];
                                                if (id[1] > -1) IdentificationData.GSM.BTS[ii].SectorIDFromIdent = id[1];
                                                IdentificationData.GSM.BTS[ii].Detected++;
                                                IdentificationData.GSM.BTS[ii].FullData = FullData;
                                            }
                                        }
                                        find = true;
                                    }
                                }
                                if (find == false)
                                {
                                    Equipment.GSM_Channel t = MainWindow.help.GetGSMCHfromFreqDN(GSMUniFreq[(int)pData.ListCellIdentResults[i].dwFrequencyIndex]);
                                    GSMBTSData dt = new GSMBTSData()
                                    {
                                        MCC = Convert.ToInt32(pData.ListCellIdentResults[i].wMCC.ToString("X")),
                                        MNC = Convert.ToInt32(pData.ListCellIdentResults[i].wMNC.ToString("X")),
                                        LAC = pData.ListCellIdentResults[i].wLAC,//string.Format("{0:00000}", pData.ListCellIdentResults[i].wLAC),// pData.ListCellIdentResults[i].wLAC.ToString(),
                                        CID = pData.ListCellIdentResults[i].wCI,//string.Format("{0:00000}", pData.ListCellIdentResults[i].wCI),//.ToString(),

                                        FullData = (pData.ListCellIdentResults[i].wMNC.ToString("X") != "" &&
                                        pData.ListCellIdentResults[i].wMCC.ToString("X") != "" &&
                                        pData.ListCellIdentResults[i].wLAC.ToString() != "" &&
                                        pData.ListCellIdentResults[i].wCI.ToString() != ""),
                                        GCID = pData.ListCellIdentResults[i].wMCC.ToString("X") + " " +
                                        pData.ListCellIdentResults[i].wMNC.ToString("X") + " " +
                                        string.Format("{0:00000}", pData.ListCellIdentResults[i].wLAC) + " " + // .ToString()+ " " + 
                                        string.Format("{0:00000}", pData.ListCellIdentResults[i].wCI),//.ToString(),
                                                                                                      //TimeOfSlotInSecssch = pData.ListCellIdentResults[i].dwIndicator,3
                                        IndSCHInfo = pData.ListCellIdentResults[i].dwIndicator,
                                        FreqIndex = pData.ListCellIdentResults[i].dwFrequencyIndex,
                                        ARFCN = t.ARFCN,
                                        FreqDn = t.FreqDn,
                                        FreqUp = t.FreqUp,
                                        StandartSubband = t.StandartSubband,
                                        //Channel = t,
                                        Detected = 1
                                    };
                                    if (dt.FullData == true && dt.CID != 0)
                                    {
                                        int[] id = ParseIDtoDB(dt);
                                        if (id[0] > -1) dt.CIDToDB = id[0]; //Convert.ToInt32(si);/////// по нему идентификация с бд                                        
                                        if (id[1] > -1) dt.SectorIDFromIdent = id[1];
                                    }
                                    GUIThreadDispatcher.Instance.Invoke(() =>
                                    {
                                        IdentificationData.GSM.BTS.Add(dt);
                                    });
                                }
                                #endregion
                            }
                        }
                        #region Sib
                        if (pData.pDemodResult != null && pData.pDemodResult.pbBitStream.Length > 0)
                        {
                            try
                            {
                                RohdeSchwarz.ViCom.Net.GSM.SL3DecoderRequest dec = new RohdeSchwarz.ViCom.Net.GSM.SL3DecoderRequest()
                                {
                                    dwBitCount = pData.pDemodResult.dwBitCount,
                                    pbBitStream = pData.pDemodResult.pbBitStream
                                };
                                RohdeSchwarz.ViCom.Net.GSM.SL3DecoderResult dr = new RohdeSchwarz.ViCom.Net.GSM.SL3DecoderResult();
                                dr = gsmInterface.RetrieveTextForPDU(dec, SDefs.dwDefaultTimeOutInMs);
                                if (pData.pDemodResult.pbBitStream.Length > 0)
                                {
                                    for (int i = 0; i < IdentificationData.GSM.BTS.Count; i++)
                                    {
                                        if (pData.pDemodResult.dwFrequencyIndex == IdentificationData.GSM.BTS[i].FreqIndex && pData.pDemodResult.dwIndicatorOfSCHInfo == IdentificationData.GSM.BTS[i].IndFirstSCHInfo)
                                        {
                                            #region save sib data
                                            ObservableCollection<DB.local3GPPSystemInformationBlock> ibs = new ObservableCollection<DB.local3GPPSystemInformationBlock>(IdentificationData.GSM.BTS[i].station_sys_info.information_blocks);
                                            bool fib = false;
                                            for (int ib = 0; ib < ibs.Count(); ib++)
                                            {
                                                if (ibs[ib].type == pData.pDemodResult.ePDU.ToString())
                                                {
                                                    fib = true;
                                                    ibs[ib].datastring = dr.pcPduText;
                                                    ibs[ib].saved = DateTime.Now;
                                                }
                                            }
                                            if (fib == false)
                                            {
                                                DB.local3GPPSystemInformationBlock sib = new DB.local3GPPSystemInformationBlock()
                                                {
                                                    datastring = dr.pcPduText,
                                                    saved = DateTime.Now,
                                                    type = pData.pDemodResult.ePDU.ToString()
                                                };
                                                GUIThreadDispatcher.Instance.Invoke(() =>
                                                {
                                                    ibs.Add(sib);
                                                    IdentificationData.GSM.BTS[i].station_sys_info.information_blocks = ibs.ToArray();
                                                });
                                            }
                                            #endregion
                                        }
                                    }
                                }
                            }
                            catch { }
                        }
                        #endregion
                    }
                    #region Exception
                    catch (RohdeSchwarz.ViCom.Net.CViComError error)
                    {
                        App.Current.Dispatcher.BeginInvoke((Action)(() =>
                        {
                            MainWindow.exp.ExceptionData = new ExData() { ex = error, ClassName = "TSMxReceiver", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
                        }));
                    }
                    catch (Exception exp)
                    {
                        App.Current.Dispatcher.BeginInvoke((Action)(() =>
                        {
                            MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "TSMxReceiver", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
                        }));
                    }
                    #endregion
                }
            }
        }

        private class MyWcdmaDataProcessor : RohdeSchwarz.ViCom.Net.WCDMA.CViComWcdmaInterfaceDataProcessor
        {
            public override void RegisterScannerId(ushort dwScannerDataId)
            {
                //Console.WriteLine("WcdmaDataProcessor registered at Scanner ID {0}", dwScannerDataId.ToString());
            }
            public override void RemoveScannerId(ushort dwScannerDataId)
            {
                //Console.WriteLine("WcdmaDataProcessor unregistered from Scanner ID {0}", dwScannerDataId.ToString());
            }
            public override void OnScannerDataMeasured(RohdeSchwarz.ViCom.Net.WCDMA.SMeasResult pData)
            {
                IsRuningT = true;
                _LastUpdate = DateTime.Now.Ticks;
                if (IdentificationData.UMTS.IsRuning == true)
                {
                    try
                    {
                        if (pData != null && pData.ListOfCPichCirs != null && pData.ListOfCPichCirs.Count > 0)
                        {
                            #region
                            foreach (var cpichResult in pData.ListOfCPichCirs)
                            {
                                bool find = false;
                                if (IdentificationData.UMTS.BTS.Count > 0)
                                {
                                    for (int i = 0; i < IdentificationData.UMTS.BTS.Count; i++)
                                    {
                                        if (IdentificationData.UMTS.BTS[i].FreqIndex == pData.dwChannelIndex && IdentificationData.UMTS.BTS[i].SC == cpichResult.ExtendedSC.wSC / 16 /*&& WCDMABTSfromDev[i].UCID == cpichResult.pBchCellIdentification.dwCI.ToString()*/)
                                        {
                                            IdentificationData.UMTS.BTS[i].RSCP = cpichResult.sRSCPInDBm100 * 0.01;
                                            IdentificationData.UMTS.BTS[i].LastLevelUpdete = MainWindow.gps.LocalTime;
                                            if (IdentificationData.UMTS.BTS[i].RSCP > DetectionLevelUMTS)/////////////////////////////////////////////////////////////////////
                                            { IdentificationData.UMTS.BTS[i].LastDetectionLevelUpdete = MainWindow.gps.LocalTime.Ticks; }
                                            IdentificationData.UMTS.BTS[i].DeleteFromMeasMon = (IdentificationData.UMTS.BTS[i].RSCP < DetectionLevelUMTS - LevelDifferenceToRemove);

                                            IdentificationData.UMTS.BTS[i].ISCP = cpichResult.sISCPInDBm100 * 0.01;
                                            IdentificationData.UMTS.BTS[i].InbandPower = cpichResult.sInbandPowerInDBm100 * 0.01;

                                            if (cpichResult.psCodePowerInDBm100 != null) IdentificationData.UMTS.BTS[i].CodePower = (double)cpichResult.psCodePowerInDBm100 * 0.01;
                                            IdentificationData.UMTS.BTS[i].IcIo = Math.Round(cpichResult.sRSCPInDBm100 * 0.01 - cpichResult.sInbandPowerInDBm100 * 0.01, 2);
                                            ////////////////

                                            #region LevelMeasurementsCar
                                            if (MainWindow.gps.Altitude != 0 && MainWindow.gps.LatitudeDecimal != 0 && MainWindow.gps.LongitudeDecimal != 0)
                                            {
                                                double dist = 0, ang = 0;
                                                if (IdentificationData.UMTS.BTS[i].level_results.Count > 0)
                                                {
                                                    MainWindow.help.calcDistance(
                                                          (double)IdentificationData.UMTS.BTS[i].level_results[IdentificationData.UMTS.BTS[i].level_results.Count - 1].location.latitude,
                                                          (double)IdentificationData.UMTS.BTS[i].level_results[IdentificationData.UMTS.BTS[i].level_results.Count - 1].location.longitude,
                                                          (double)MainWindow.gps.LatitudeDecimal,
                                                          (double)MainWindow.gps.LongitudeDecimal,
                                                          out dist, out ang);
                                                }
                                                if ((IdentificationData.UMTS.BTS[i].level_results.Count == 0 ||
                                                    (decimal)dist > MainWindow.db_v2.Atdi_LevelResults_DistanceStep &&
                                                    IdentificationData.UMTS.BTS[i].level_results[IdentificationData.UMTS.BTS[i].level_results.Count - 1].level_dbm != IdentificationData.UMTS.BTS[i].RSCP) ||
                                                    new TimeSpan(MainWindow.gps.LocalTime.Ticks - IdentificationData.UMTS.BTS[i].level_results[IdentificationData.UMTS.BTS[i].level_results.Count() - 1].measurement_time.Ticks) > MainWindow.db_v2.Atdi_LevelsMeasurementsCar_TimeStep)
                                                {
                                                    #region
                                                    DB.localatdi_level_meas_result l = new DB.localatdi_level_meas_result()
                                                    {
                                                        difference_time_stamp_ns = (decimal)Equipment.IdentificationData.UMTS.BTS[i].TimeOfSlotInSec,
                                                        level_dbm = Equipment.IdentificationData.UMTS.BTS[i].RSCP,
                                                        //level_dbmkvm = 0,
                                                        measurement_time = Equipment.IdentificationData.UMTS.BTS[i].LastLevelUpdete,
                                                        saved_in_db = false,
                                                        saved_in_result = false,
                                                        location = new DB.localatdi_geo_location()
                                                        {
                                                            //agl = 0,
                                                            asl = MainWindow.gps.Altitude,
                                                            latitude = (double)MainWindow.gps.LatitudeDecimal,
                                                            longitude = (double)MainWindow.gps.LongitudeDecimal,
                                                        }
                                                    };
                                                    IdentificationData.UMTS.BTS[i].LR_NewDataToSave = true;
                                                    GUIThreadDispatcher.Instance.Invoke(() =>
                                                    {
                                                        IdentificationData.UMTS.BTS[i].level_results.Add(l);
                                                    });
                                                    #endregion
                                                }
                                            }
                                            #endregion

                                            if (cpichResult.pBchCellIdentification != null && cpichResult.pBchCellIdentification.wLAC != 65535)
                                            {
                                                if (IdentificationData.UMTS.BTS[i].MNC != Convert.ToInt32(cpichResult.pBchCellIdentification.wMNC.ToString("X")) ||
                                                    IdentificationData.UMTS.BTS[i].MCC != Convert.ToInt32(cpichResult.pBchCellIdentification.wMCC.ToString("X")) ||
                                                    IdentificationData.UMTS.BTS[i].LAC != cpichResult.pBchCellIdentification.wLAC ||
                                                    IdentificationData.UMTS.BTS[i].UCID != (int)cpichResult.pBchCellIdentification.dwCI
                                                    )
                                                {
                                                    IdentificationData.UMTS.BTS[i].MNC = Convert.ToInt32(cpichResult.pBchCellIdentification.wMNC.ToString("X"));//.ToString();
                                                    IdentificationData.UMTS.BTS[i].MCC = Convert.ToInt32(cpichResult.pBchCellIdentification.wMCC.ToString("X"));//.ToString();
                                                    IdentificationData.UMTS.BTS[i].LAC = cpichResult.pBchCellIdentification.wLAC;//.ToString();
                                                    IdentificationData.UMTS.BTS[i].UCID = (int)cpichResult.pBchCellIdentification.dwCI;//.ToString();
                                                    IdentificationData.UMTS.BTS[i].RNC = (int)(IdentificationData.UMTS.BTS[i].UCID / 65536);//.ToString();
                                                    IdentificationData.UMTS.BTS[i].CID = (int)(IdentificationData.UMTS.BTS[i].UCID % 65536);//.ToString();

                                                    bool fulldata = (IdentificationData.UMTS.BTS[i].MNC != -1 && IdentificationData.UMTS.BTS[i].MCC != -1 && IdentificationData.UMTS.BTS[i].LAC != -1 && IdentificationData.UMTS.BTS[i].CID != -1);
                                                    if (fulldata)
                                                    {
                                                        IdentificationData.UMTS.BTS[i].GCID = IdentificationData.UMTS.BTS[i].MCC.ToString() + " " + IdentificationData.UMTS.BTS[i].MNC.ToString() + " " +
                                                          string.Format("{0:00000}", IdentificationData.UMTS.BTS[i].LAC) + " " + string.Format("{0:00000}", IdentificationData.UMTS.BTS[i].CID);
                                                        int[] id = ParseIDtoDB(IdentificationData.UMTS.BTS[i]);
                                                        if (id[0] > -1) IdentificationData.UMTS.BTS[i].CIDToDB = id[0];
                                                        if (id[1] > -1) IdentificationData.UMTS.BTS[i].SectorIDToDB = id[1];
                                                    }
                                                    #region                                                    
                                                    #endregion
                                                    IdentificationData.UMTS.BTS[i].FullData = fulldata;
                                                }
                                            }
                                            bool freqLevelMax = true;
                                            for (int l = 0; l < IdentificationData.UMTS.BTS.Count; l++)
                                            {
                                                if (IdentificationData.UMTS.BTS[l].FreqDn == IdentificationData.UMTS.BTS[i].FreqDn &&
                                                    IdentificationData.UMTS.BTS[l].GCID != IdentificationData.UMTS.BTS[i].GCID)
                                                {
                                                    if (IdentificationData.UMTS.BTS[l].RSCP + LevelIsMaxIfMoreBy < IdentificationData.UMTS.BTS[i].RSCP)
                                                        IdentificationData.UMTS.BTS[l].ThisIsMaximumSignalAtThisFrequency = false;
                                                    else { freqLevelMax = false; }
                                                }
                                            }
                                            IdentificationData.UMTS.BTS[i].ThisIsMaximumSignalAtThisFrequency = freqLevelMax;

                                            find = true;
                                        }
                                    }
                                }
                                #region добавляем новое
                                if (find == false)
                                {
                                    try
                                    {
                                        if (cpichResult.ExtendedSC != null)
                                        {
                                            uint FreqIndex = pData.dwChannelIndex;
                                            Equipment.UMTS_Channel t = MainWindow.help.GetUMTSCHFromFreqDN(UMTSUniFreq[(int)pData.dwChannelIndex]);
                                            t.FreqDn = t.FreqDn;
                                            t.FreqUp = t.FreqUp;
                                            int SC = cpichResult.ExtendedSC.wSC / 16;
                                            int MCC = -1;
                                            int MNC = -1;
                                            int LAC = -1;
                                            int UCID = -1;
                                            int RNC = -1;
                                            int CID = -1;
                                            if (cpichResult.pBchCellIdentification != null)
                                            {
                                                MCC = Convert.ToInt32(cpichResult.pBchCellIdentification.wMCC.ToString("X"));
                                                MNC = Convert.ToInt32(cpichResult.pBchCellIdentification.wMNC.ToString("X"));
                                                LAC = cpichResult.pBchCellIdentification.wLAC;//.ToString();
                                                if (LAC == 65535) LAC = -1;
                                                UCID = (int)cpichResult.pBchCellIdentification.dwCI;
                                                RNC = (int)(UCID / 65536);
                                                CID = (int)(UCID % 65536);//.ToString();
                                            }
                                            bool FullData = (MCC != -1 && MNC != -1 && LAC != -1 && CID != -1);
                                            string GCID = "";
                                            if (FullData) { GCID = MCC.ToString() + " " + MNC.ToString() + " " + string.Format("{0:00000}", LAC) + " " + string.Format("{0:00000}", CID); }

                                            double ISCP = cpichResult.sISCPInDBm100 * 0.01;
                                            double RSCP = cpichResult.sRSCPInDBm100 * 0.01;
                                            double InbandPower = cpichResult.sInbandPowerInDBm100 * 0.01;
                                            double CodePower = -1000;
                                            if (cpichResult.psCodePowerInDBm100 != null) CodePower = (double)cpichResult.psCodePowerInDBm100 * 0.01;
                                            double IcIo = Math.Round(cpichResult.sRSCPInDBm100 * 0.01 - cpichResult.sInbandPowerInDBm100 * 0.01, 2);
                                            DateTime LastLevelUpdete = MainWindow.gps.LocalTime;
                                            UMTSBTSData dt = new UMTSBTSData()
                                            {
                                                FreqIndex = FreqIndex,
                                                //Channel = t,
                                                UARFCN_DN = t.UARFCN_DN,
                                                UARFCN_UP = t.UARFCN_UP,
                                                FreqDn = t.FreqDn,
                                                FreqUp = t.FreqUp,
                                                StandartSubband = t.StandartSubband,
                                                SC = SC,
                                                MCC = MCC,
                                                MNC = MNC,
                                                LAC = LAC,
                                                FullData = FullData,
                                                RNC = RNC,
                                                CID = CID,
                                                UCID = UCID,
                                                GCID = GCID,
                                                ISCP = ISCP,
                                                RSCP = RSCP,
                                                InbandPower = InbandPower,
                                                CodePower = CodePower,
                                                IcIo = IcIo,
                                                LastLevelUpdete = LastLevelUpdete
                                            };
                                            if (dt.FullData == true)
                                            {
                                                int[] id = ParseIDtoDB(dt);
                                                if (id[0] > -1) dt.CIDToDB = id[0];
                                                if (id[1] > -1) dt.SectorIDToDB = id[1];
                                            }
                                            GUIThreadDispatcher.Instance.Invoke(() =>
                                            {
                                                IdentificationData.UMTS.BTS.Add(dt);
                                            });
                                        }
                                    }
                                    catch (Exception exp)
                                    {
                                        MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "TSMxReceiver", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
                                    }
                                }
                                #endregion
                            }
                            #endregion
                        }
                        #region Sib
                        if (pData.pDemodResult != null && pData.pDemodResult.pbBitStream.Length > 0)
                        {
                            try
                            {
                                RohdeSchwarz.ViCom.Net.WCDMA.SL3DecoderRequest dec = new RohdeSchwarz.ViCom.Net.WCDMA.SL3DecoderRequest()
                                {
                                    dwBitCount = pData.pDemodResult.dwBitCount,
                                    ePDU = pData.pDemodResult.ePDU,
                                    pbBitStream = pData.pDemodResult.pbBitStream
                                };
                                RohdeSchwarz.ViCom.Net.WCDMA.SL3DecoderResult dr = new RohdeSchwarz.ViCom.Net.WCDMA.SL3DecoderResult();
                                dr = wcdmaInterface.RetrieveTextForPDU(dec, SDefs.dwDefaultTimeOutInMs);
                                if (pData.pDemodResult.pbBitStream.Length > 0)
                                {
                                    for (int i = 0; i < IdentificationData.UMTS.BTS.Count; i++)
                                    {
                                        if (pData.dwChannelIndex == IdentificationData.UMTS.BTS[i].FreqIndex && pData.pDemodResult.ExtendedSC.wSC / 16 == IdentificationData.UMTS.BTS[i].SC)
                                        {
                                            #region save sib data
                                            bool fib = false;
                                            ObservableCollection<DB.local3GPPSystemInformationBlock> ibs = new ObservableCollection<DB.local3GPPSystemInformationBlock>(IdentificationData.UMTS.BTS[i].station_sys_info.information_blocks);
                                            for (int ib = 0; ib < ibs.Count(); ib++)
                                            {
                                                if (ibs[ib].type == pData.pDemodResult.ePDU.ToString())
                                                {
                                                    fib = true;
                                                    ibs[ib].datastring = dr.pcPduText;
                                                    ibs[ib].saved = DateTime.Now;
                                                }
                                            }
                                            if (fib == false)
                                            {
                                                ibs.Add(
                                                    new DB.local3GPPSystemInformationBlock()
                                                    {
                                                        datastring = dr.pcPduText,
                                                        saved = DateTime.Now,
                                                        type = pData.pDemodResult.ePDU.ToString()
                                                    });
                                                IdentificationData.UMTS.BTS[i].station_sys_info.information_blocks = ibs.ToArray();
                                            }
                                            #endregion
                                        }
                                    }
                                }
                            }
                            catch { }
                        }
                        #endregion
                    }
                    #region Exception
                    catch (RohdeSchwarz.ViCom.Net.CViComError error)
                    {
                        App.Current.Dispatcher.BeginInvoke((Action)(() =>
                        {
                            MainWindow.exp.ExceptionData = new ExData() { ex = error, ClassName = "TSMxReceiver", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
                        }));
                    }
                    catch (Exception exp)
                    {
                        App.Current.Dispatcher.BeginInvoke((Action)(() =>
                        {
                            MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "TSMxReceiver", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
                        }));
                    }
                    #endregion
                }
            }
        }

        private class MyLteDataProcessor : RohdeSchwarz.ViCom.Net.LTE.CViComLteInterfaceDataProcessor
        {
            public override void RegisterScannerId(ushort dwScannerDataId)
            {
                //Console.WriteLine("LteDataProcessor registered at Scanner ID {0} ", dwScannerDataId.ToString());
            }

            public override void RemoveScannerId(ushort dwScannerDataId)
            {
                //Console.WriteLine("LteDataProcessor unregistered from Scanner ID {0}", dwScannerDataId.ToString());
            }

            public override void OnScannerDataMeasured(RohdeSchwarz.ViCom.Net.LTE.SMeasResult pData)
            {
                if (IdentificationData.LTE.IsRuning == true)
                {
                    IsRuningT = true;
                    _LastUpdate = DateTime.Now.Ticks;
                    if (pData.ListOfSignals != null)
                    {
                        foreach (var signalres in pData.ListOfSignals)
                        {
                            bool find = false;
                            for (int i = 0; i < IdentificationData.LTE.BTS.Count; i++)
                            {
                                if (pData.dwChannelIndex == IdentificationData.LTE.BTS[i].FreqIndex && signalres.dwScannerBtsIdent == IdentificationData.LTE.BTS[i].FreqBtsIndex)
                                {
                                    find = true;
                                    if (signalres.sRefSignal.sPBCHbasedRSRPinDBm100 != short.MaxValue)
                                    {
                                        IdentificationData.LTE.BTS[i].RSRP = signalres.sRefSignal.sPBCHbasedRSRPinDBm100 * 0.01;
                                        IdentificationData.LTE.BTS[i].LastLevelUpdete = MainWindow.gps.LocalTime;
                                        if (IdentificationData.LTE.BTS[i].RSRP > DetectionLevelLTE)/////////////////////////////////////////////////////////////////////
                                        { IdentificationData.LTE.BTS[i].LastDetectionLevelUpdete = MainWindow.gps.LocalTime.Ticks; }
                                        IdentificationData.LTE.BTS[i].DeleteFromMeasMon = (IdentificationData.LTE.BTS[i].RSRP < DetectionLevelLTE - LevelDifferenceToRemove);
                                        bool freqLevelMax = true;
                                        for (int l = 0; l < IdentificationData.LTE.BTS.Count; l++)
                                        {
                                            if (IdentificationData.LTE.BTS[l].FreqDn == IdentificationData.LTE.BTS[i].FreqDn &&
                                                IdentificationData.LTE.BTS[l].GCID != IdentificationData.LTE.BTS[i].GCID)
                                            {
                                                if (IdentificationData.LTE.BTS[l].RSRP + LevelIsMaxIfMoreBy < IdentificationData.LTE.BTS[i].RSRP)
                                                    IdentificationData.LTE.BTS[l].ThisIsMaximumSignalAtThisFrequency = false;
                                                else { freqLevelMax = false; }
                                            }

                                        }
                                        IdentificationData.LTE.BTS[i].ThisIsMaximumSignalAtThisFrequency = freqLevelMax;
                                        #region LevelMeasurementsCar
                                        if (MainWindow.gps.Altitude != 0 && MainWindow.gps.LatitudeDecimal != 0 && MainWindow.gps.LongitudeDecimal != 0)
                                        {
                                            double dist = 0, ang = 0;
                                            if (IdentificationData.LTE.BTS[i].level_results.Count() > 0)
                                            {
                                                MainWindow.help.calcDistance(
                                                      (double)IdentificationData.LTE.BTS[i].level_results[IdentificationData.LTE.BTS[i].level_results.Count() - 1].location.latitude,
                                                      (double)IdentificationData.LTE.BTS[i].level_results[IdentificationData.LTE.BTS[i].level_results.Count() - 1].location.longitude,
                                                      (double)MainWindow.gps.LatitudeDecimal,
                                                      (double)MainWindow.gps.LongitudeDecimal,
                                                      out dist, out ang);
                                            }
                                            if ((IdentificationData.LTE.BTS[i].level_results.Count == 0 ||
                                                (decimal)dist > MainWindow.db_v2.Atdi_LevelResults_DistanceStep &&
                                                IdentificationData.LTE.BTS[i].level_results[IdentificationData.LTE.BTS[i].level_results.Count - 1].level_dbm != IdentificationData.LTE.BTS[i].RSRP) ||
                                                new TimeSpan(MainWindow.gps.LocalTime.Ticks - IdentificationData.LTE.BTS[i].level_results[IdentificationData.LTE.BTS[i].level_results.Count() - 1].measurement_time.Ticks) > MainWindow.db_v2.Atdi_LevelsMeasurementsCar_TimeStep)
                                            {
                                                #region
                                                DB.localatdi_level_meas_result l = new DB.localatdi_level_meas_result()
                                                {
                                                    difference_time_stamp_ns = 0,
                                                    level_dbm = Equipment.IdentificationData.LTE.BTS[i].RSRP,
                                                    //level_dbmkvm = 0,
                                                    measurement_time = Equipment.IdentificationData.LTE.BTS[i].LastLevelUpdete,
                                                    saved_in_db = false,
                                                    saved_in_result = false,
                                                    location = new DB.localatdi_geo_location()
                                                    {
                                                        //agl = 0,
                                                        asl = MainWindow.gps.Altitude,
                                                        latitude = (double)MainWindow.gps.LatitudeDecimal,
                                                        longitude = (double)MainWindow.gps.LongitudeDecimal,
                                                    }
                                                };
                                                IdentificationData.LTE.BTS[i].LR_NewDataToSave = true;
                                                App.Current.Dispatcher.Invoke((Action)(() =>
                                                {
                                                    IdentificationData.LTE.BTS[i].level_results.Add(l);
                                                }));
                                                #endregion
                                            }
                                        }
                                        #endregion
                                    }
                                    if (signalres.sRefSignal.sPBCHbasedRSRQinDB100 != short.MaxValue)
                                    { IdentificationData.LTE.BTS[i].RSRQ = signalres.sRefSignal.sPBCHbasedRSRQinDB100 * 0.01; }
                                    if (signalres.pCir != null)
                                    {
                                        if (signalres.pCir.pPowerDelayProfile.fAggregatePowerInDBm != float.MaxValue)
                                            IdentificationData.LTE.BTS[i].InbandPower = signalres.pCir.pPowerDelayProfile.fAggregatePowerInDBm;
                                    }
                                }
                            }
                            if (find == false)
                            {
                                Equipment.LTE_Channel t = MainWindow.help.GetLTECHfromFreqDN(LTEUniFreq[(int)pData.dwChannelIndex]);
                                LTEBTSData ltebts = new LTEBTSData()
                                {
                                    FreqIndex = pData.dwChannelIndex,
                                    FreqBtsIndex = signalres.dwScannerBtsIdent,
                                    //Channel = t,
                                    EARFCN_DN = t.EARFCN_DN,
                                    EARFCN_UP = t.EARFCN_UP,
                                    FreqDn = t.FreqDn,
                                    FreqUp = t.FreqUp,
                                    StandartSubband = t.StandartSubband,
                                    PCI = signalres.wPhysicalCellId,
                                };
                                if (signalres.sRefSignal != null)
                                {
                                    if (signalres.sRefSignal.sPBCHbasedRSRPinDBm100 != short.MaxValue)
                                    {
                                        ltebts.RSRP = signalres.sRefSignal.sPBCHbasedRSRPinDBm100 * 0.01;
                                        ltebts.LastLevelUpdete = MainWindow.gps.LocalTime;
                                    }
                                    if (signalres.sRefSignal.sPBCHbasedRSRQinDB100 != short.MaxValue)
                                        ltebts.RSRQ = signalres.sRefSignal.sPBCHbasedRSRQinDB100 * 0.01;
                                }
                                if (signalres.pCir != null)
                                {
                                    if (signalres.pCir.pPowerDelayProfile.fAggregatePowerInDBm != float.MaxValue)
                                        ltebts.InbandPower = signalres.pCir.pPowerDelayProfile.fAggregatePowerInDBm;
                                }
                                App.Current.Dispatcher.Invoke((Action)(() =>
                                {
                                    IdentificationData.LTE.BTS.Add(ltebts);
                                }));

                            }
                        }
                    }
                    if (pData.ListOfWidebandRsCinrResults != null)
                    {
                        foreach (var signalres in pData.ListOfWidebandRsCinrResults)
                        {
                            for (int i = 0; i < IdentificationData.LTE.BTS.Count; i++)
                            {
                                if (pData.dwChannelIndex == IdentificationData.LTE.BTS[i].FreqIndex && signalres.dwScannerBtsIdent == IdentificationData.LTE.BTS[i].FreqBtsIndex)
                                {
                                    if (signalres.sRSRPinDBm100 != short.MaxValue)
                                        IdentificationData.LTE.BTS[i].WB_RSRP = signalres.sRSRPinDBm100 * 0.01;
                                    if (signalres.sRSRQinDB100 != short.MaxValue)
                                        IdentificationData.LTE.BTS[i].WB_RSRQ = signalres.sRSRQinDB100 * 0.01;
                                    if (signalres.sRsRssiInDBm100 != short.MaxValue)
                                        IdentificationData.LTE.BTS[i].WB_RS_RSSI = signalres.sRsRssiInDBm100 * 0.01;
                                }
                            }
                        }
                    }
                    #region Sib
                    if (pData.pDemodResult != null && pData.pDemodResult.pbBitStream.Length > 0)
                    {
                        try
                        {
                            RohdeSchwarz.ViCom.Net.LTE.SL3DecoderRequest dec = new RohdeSchwarz.ViCom.Net.LTE.SL3DecoderRequest()
                            {
                                dwBitCount = pData.pDemodResult.dwBitCount,
                                ePDU = pData.pDemodResult.ePDU,
                                pbBitStream = pData.pDemodResult.pbBitStream
                            };
                            RohdeSchwarz.ViCom.Net.LTE.SL3DecoderResult dr = new RohdeSchwarz.ViCom.Net.LTE.SL3DecoderResult();
                            dr = lteInterface.RetrieveTextForPDU(dec, SDefs.dwDefaultTimeOutInMs);
                            if (pData.pDemodResult.ePDU == RohdeSchwarz.ViCom.Net.LTE.Pdu.Type.MIB)
                            {
                                if (pData.pDemodResult.pbBitStream.Length > 0)
                                {
                                    decimal[] data = ParseMIBLTE(dr.pcPduText);
                                    for (int i = 0; i < IdentificationData.LTE.BTS.Count; i++)
                                    {
                                        if (pData.dwChannelIndex == IdentificationData.LTE.BTS[i].FreqIndex && pData.pDemodResult.dwBtsId == IdentificationData.LTE.BTS[i].FreqBtsIndex && pData.pDemodResult.wPhysicalCellId == IdentificationData.LTE.BTS[i].PCI)
                                        {
                                            IdentificationData.LTE.BTS[i].Bandwidth = data[0];
                                            #region save sib data
                                            ObservableCollection<DB.local3GPPSystemInformationBlock> ibs = new ObservableCollection<DB.local3GPPSystemInformationBlock>(IdentificationData.LTE.BTS[i].station_sys_info.information_blocks);
                                            bool fib = false;
                                            for (int ib = 0; ib < ibs.Count(); ib++)
                                            {
                                                if (ibs[ib].type == pData.pDemodResult.ePDU.ToString())
                                                {
                                                    fib = true;
                                                    ibs[ib].datastring = dr.pcPduText;
                                                    ibs[ib].saved = DateTime.Now;
                                                }
                                            }
                                            if (fib == false)
                                            {
                                                DB.local3GPPSystemInformationBlock sib = new DB.local3GPPSystemInformationBlock()
                                                {
                                                    datastring = dr.pcPduText,
                                                    saved = DateTime.Now,
                                                    type = pData.pDemodResult.ePDU.ToString()
                                                };
                                                App.Current.Dispatcher.Invoke((Action)(() =>
                                                {
                                                    ibs.Add(sib);
                                                    IdentificationData.LTE.BTS[i].station_sys_info.information_blocks = ibs.ToArray();
                                                }));

                                            }
                                            #endregion
                                        }
                                    }
                                }
                            }
                            else if (pData.pDemodResult.ePDU == RohdeSchwarz.ViCom.Net.LTE.Pdu.Type.SIB1)
                            {
                                if (pData.pDemodResult.pbBitStream.Length > 0)
                                {
                                    int[] data = ParseSIB1LTE(dr.pcPduText);
                                    for (int i = 0; i < IdentificationData.LTE.BTS.Count; i++)
                                    {
                                        if (pData.dwChannelIndex == IdentificationData.LTE.BTS[i].FreqIndex &&
                                            pData.pDemodResult.dwBtsId == IdentificationData.LTE.BTS[i].FreqBtsIndex &&
                                            pData.pDemodResult.wPhysicalCellId == IdentificationData.LTE.BTS[i].PCI)
                                        {
                                            IdentificationData.LTE.BTS[i].MCC = data[0];
                                            IdentificationData.LTE.BTS[i].MNC = data[1];
                                            IdentificationData.LTE.BTS[i].TAC = data[2];
                                            IdentificationData.LTE.BTS[i].CelId28 = data[3];
                                            IdentificationData.LTE.BTS[i].eNodeBId = data[4];
                                            IdentificationData.LTE.BTS[i].CID = data[5];
                                            #region save sib data
                                            ObservableCollection<DB.local3GPPSystemInformationBlock> ibs = new ObservableCollection<DB.local3GPPSystemInformationBlock>(IdentificationData.LTE.BTS[i].station_sys_info.information_blocks);
                                            bool fib = false;
                                            for (int ib = 0; ib < ibs.Count(); ib++)
                                            {
                                                if (ibs[ib].type == pData.pDemodResult.ePDU.ToString())
                                                {
                                                    fib = true;
                                                    ibs[ib].datastring = dr.pcPduText;
                                                    ibs[ib].saved = DateTime.Now;
                                                }
                                            }
                                            if (fib == false)
                                            {
                                                DB.local3GPPSystemInformationBlock sib = new DB.local3GPPSystemInformationBlock()
                                                {
                                                    datastring = dr.pcPduText,
                                                    saved = DateTime.Now,
                                                    type = pData.pDemodResult.ePDU.ToString()
                                                };
                                                App.Current.Dispatcher.Invoke((Action)(() =>
                                                {
                                                    ibs.Add(sib);
                                                    IdentificationData.LTE.BTS[i].station_sys_info.information_blocks = ibs.ToArray();
                                                }));
                                            }
                                            #endregion
                                            bool FullData = (IdentificationData.LTE.BTS[i].MCC != -1 && IdentificationData.LTE.BTS[i].MNC != -1 && IdentificationData.LTE.BTS[i].eNodeBId != -1 && IdentificationData.LTE.BTS[i].CID != -1);
                                            string GCID = "";
                                            if (FullData)
                                            {
                                                GCID = IdentificationData.LTE.BTS[i].MCC.ToString() + " " + IdentificationData.LTE.BTS[i].MNC.ToString() + " " +
                                                  string.Format("{0:000000}", IdentificationData.LTE.BTS[i].eNodeBId) + " " + string.Format("{0:000}", IdentificationData.LTE.BTS[i].CID);
                                                IdentificationData.LTE.BTS[i].GCID = GCID;
                                                int[] id = ParseIDtoDB(IdentificationData.LTE.BTS[i]);
                                                if (id[0] > -1) IdentificationData.LTE.BTS[i].CIDToDB = id[0];
                                            }
                                            IdentificationData.LTE.BTS[i].FullData = FullData;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (pData.pDemodResult.pbBitStream.Length > 0)
                                {
                                    int[] data = ParseSIB1LTE(dr.pcPduText);
                                    for (int i = 0; i < IdentificationData.LTE.BTS.Count; i++)
                                    {
                                        if (pData.dwChannelIndex == IdentificationData.LTE.BTS[i].FreqIndex && pData.pDemodResult.dwBtsId == IdentificationData.LTE.BTS[i].FreqBtsIndex && pData.pDemodResult.wPhysicalCellId == IdentificationData.LTE.BTS[i].PCI)
                                        {
                                            #region save sib data
                                            ObservableCollection<DB.local3GPPSystemInformationBlock> ibs = new ObservableCollection<DB.local3GPPSystemInformationBlock>(IdentificationData.LTE.BTS[i].station_sys_info.information_blocks);
                                            bool fib = false;
                                            for (int ib = 0; ib < ibs.Count; ib++)
                                            {
                                                if (ibs[ib].type == pData.pDemodResult.ePDU.ToString())
                                                {
                                                    fib = true;
                                                    ibs[ib].datastring = dr.pcPduText;
                                                    ibs[ib].saved = DateTime.Now;
                                                }
                                            }
                                            if (fib == false)
                                            {
                                                DB.local3GPPSystemInformationBlock sib = new DB.local3GPPSystemInformationBlock()
                                                {
                                                    datastring = dr.pcPduText,
                                                    saved = DateTime.Now,
                                                    type = pData.pDemodResult.ePDU.ToString()
                                                };
                                                App.Current.Dispatcher.Invoke((Action)(() =>
                                                {
                                                    ibs.Add(sib);
                                                    IdentificationData.LTE.BTS[i].station_sys_info.information_blocks = ibs.ToArray();
                                                }));
                                            }
                                            #endregion
                                        }
                                    }
                                }
                            }
                        }
                        catch { }
                    }
                    #endregion
                    //}
                    //IdentificationData.LTE.OnPropertyChanged("BTS");
                }
                //}));
                #region

                //
                // DLAA Results
                //
                //if ( null != pData.pDlaaResult )
                //{
                //   Console.WriteLine(" DLAA Result ");
                //   Console.WriteLine(" BTS (eNodeB) Count       : {0} ", pData.pDlaaResult.ListOfBtsResult.Count);
                //   foreach (Net.LTE.SMeasResult.SDlaaResult.SBtsResult bts in pData.pDlaaResult.ListOfBtsResult)
                //   {
                //      int iRNtiCount = (int)bts.dwRNTICount;
                //      if( 0 < iRNtiCount )
                //      {
                //         Console.WriteLine(" BTS (eNodeB) RNTI Count  : {0} ", iRNtiCount );
                //      }
                //      for (uint dwObs = 0; dwObs < bts.dwObservationCount; dwObs++)
                //      {
                //         iRNtiCount = (int)bts.pDlaaStatisticResults[dwObs].dwRntiStatisticsCount;
                //         if (0 < iRNtiCount)
                //         {
                //            Console.WriteLine(" BTS Observation Time in s   : {0} ", bts.pDlaaStatisticResults[dwObs].dwObservationTimeInS);
                //            Console.WriteLine(" BTS Observation RNTI Count  : {0} ", iRNtiCount);
                //         }
                //      }
                //   }
                //}

                //foreach (var wbres in pData.ListOfWidebandRsCinrResults)
                //{
                //    Console.WriteLine(" SWidebandRsCinrResult");
                //    Console.WriteLine("\tPhysicalCellId: {0}", wbres.wPhysicalCellId);
                //    Console.WriteLine("\tRSRPinDBm100: {0}", wbres.sRSRPinDBm100);
                //    Console.WriteLine("\tRSRQinDB100: {0}", wbres.sRSRQinDB100);
                //    Console.WriteLine("\tWidebandRsCinrValues.Length: {0}", wbres.pWidebandRsCinrValues.Length);

                //    if (wbres.pWidebandRsCinrValues.Length > 0)
                //    {
                //        Console.WriteLine("\tWidebandRsCinrValues.sRsCinrInDB100: {0}",
                //            wbres.pWidebandRsCinrValues[wbres.pWidebandRsCinrValues.Length - 1].sRsCinrInDB100);
                //    }
                //}
                //foreach (var rssires in pData.ListOfRssiAndSpectrumResults)
                //{
                //    if (rssires.psRssiInDBm100 != null)
                //    {
                //        Console.WriteLine(" SRssiAndSpectrumResult");
                //        var rssiInDBm100 = (short)rssires.psRssiInDBm100;
                //        Console.WriteLine("\tRssiInDBm100: {0}", rssiInDBm100);
                //    }
                //}
                //foreach (var signalres in pData.ListOfSignals)
                //{
                //    Console.WriteLine(" SSignals");

                //    Single tempSSyncToPSyncRatioInDB = -99;

                //    if (signalres.pfSSyncToPSyncRatioInDB != null)
                //    {
                //        tempSSyncToPSyncRatioInDB = (Single)signalres.pfSSyncToPSyncRatioInDB;
                //        Console.WriteLine("\tSSyncToPSyncRatioInDB: {0}", tempSSyncToPSyncRatioInDB);
                //    }

                //    if (signalres.ListOfPowerValues.Count > 0)
                //    {
                //        short sschrp =
                //            Convert.ToInt16(signalres.ListOfPowerValues[signalres.ListOfPowerValues.Count - 1].fPowerInDBm - 17.9);
                //        short cinrInDB = Convert.ToInt16(signalres.ListOfPowerValues[signalres.ListOfPowerValues.Count - 1].fCinrInDB);
                //        Console.WriteLine("\tsschrp: {0}", sschrp);
                //        Console.WriteLine("\tCinrInDB: {0}", cinrInDB);

                //        short pschrp = Convert.ToInt16(sschrp - tempSSyncToPSyncRatioInDB);
                //        Console.WriteLine("\tpschrp: {0}", pschrp);
                //    }
                //}
                #endregion
            }
        }

        private class MyCdmaDataProcessor : RohdeSchwarz.ViCom.Net.CDMA.CViComCdmaInterfaceDataProcessor
        {
            public override void RegisterScannerId(ushort dwScannerDataId)
            {
                //Console.WriteLine("CdmaDataProcessor registered at Scanner ID {0}", dwScannerDataId.ToString());
            }
            public override void RemoveScannerId(ushort dwScannerDataId)
            {
                //Console.WriteLine("CdmaDataProcessor unregistered from Scanner ID {0}", dwScannerDataId.ToString());
            }
            public override void OnScannerDataMeasured(RohdeSchwarz.ViCom.Net.CDMA.SMeasResult pData)
            {
                if (IdentificationData.CDMA.IsRuning == true)
                {
                    try
                    {
                        //App.Current.Dispatcher.BeginInvoke((Action)(() =>
                        //{

                        _LastUpdate = DateTime.Now.Ticks;
                        Settings.CDMAFreqs_Set t = CDMAUniFreq[(int)pData.dwChannelIndex];
                        Equipment.CDMA_Channel tt = MainWindow.help.GetCDMACHfromFreqDN(CDMAUniFreq[(int)pData.dwChannelIndex].FreqDn);
                        #region 
                        if (pData.ListOfFPichCirs != null)
                        {
                            foreach (var fpichResult in pData.ListOfFPichCirs)
                            {
                                if (fpichResult.ExtendedPNOffset.wPNOffset != 65535)
                                {
                                    bool find = false;
                                    for (int i = 0; i < IdentificationData.CDMA.BTS.Count; i++)
                                    {
                                        if (IdentificationData.CDMA.BTS[i].FreqIndex == pData.dwChannelIndex && IdentificationData.CDMA.BTS[i].Indicator == fpichResult.ExtendedPNOffset.wIndicator && IdentificationData.CDMA.BTS[i].PN == fpichResult.ExtendedPNOffset.wPNOffset)
                                        {
                                            find = true;
                                            IdentificationData.CDMA.BTS[i].PTotal = fpichResult.sInbandPowerInDBm100 * 0.01;
                                            IdentificationData.CDMA.BTS[i].RSCP = fpichResult.sRSCPInDBm100 * 0.01;
                                            IdentificationData.CDMA.BTS[i].IcIo = 0 - Math.Round(fpichResult.sInbandPowerInDBm100 * 0.01 - fpichResult.sRSCPInDBm100 * 0.01, 2);
                                            IdentificationData.CDMA.BTS[i].LastLevelUpdete = MainWindow.gps.LocalTime;
                                            if (IdentificationData.CDMA.BTS[i].RSCP > DetectionLevelCDMA)/////////////////////////////////////////////////////////////////////
                                            { IdentificationData.CDMA.BTS[i].LastDetectionLevelUpdete = MainWindow.gps.LocalTime.Ticks; }
                                            IdentificationData.CDMA.BTS[i].DeleteFromMeasMon = (IdentificationData.CDMA.BTS[i].RSCP < DetectionLevelCDMA - LevelDifferenceToRemove);
                                            bool freqLevelMax = true;
                                            for (int l = 0; l < IdentificationData.CDMA.BTS.Count; l++)
                                            {
                                                if (IdentificationData.CDMA.BTS[l].FreqDn == IdentificationData.CDMA.BTS[i].FreqDn &&
                                                    IdentificationData.CDMA.BTS[l].GCID != IdentificationData.CDMA.BTS[i].GCID)
                                                {
                                                    if (IdentificationData.CDMA.BTS[l].RSCP + LevelIsMaxIfMoreBy < IdentificationData.CDMA.BTS[i].RSCP)
                                                        IdentificationData.CDMA.BTS[l].ThisIsMaximumSignalAtThisFrequency = false;
                                                    else { freqLevelMax = false; }
                                                }

                                            }
                                            App.Current.Dispatcher.Invoke((Action)(() =>
                                            {
                                                IdentificationData.CDMA.BTS[i].ThisIsMaximumSignalAtThisFrequency = freqLevelMax;
                                            }));
                                            ///IdentificationData.CDMA.BTS[i].FullData = true;
                                            //IdentificationData.CDMA.BTS[i].PN = fpichResult.ExtendedPNOffset.wPNOffset;
                                            #region LevelMeasurementsCar
                                            if (MainWindow.gps.Altitude != 0 && MainWindow.gps.LatitudeDecimal != 0 && MainWindow.gps.LongitudeDecimal != 0)
                                            {
                                                double dist = 0, ang = 0;
                                                if (IdentificationData.CDMA.BTS[i].level_results.Count() > 0)
                                                {
                                                    MainWindow.help.calcDistance(
                                                          (double)IdentificationData.CDMA.BTS[i].level_results[IdentificationData.CDMA.BTS[i].level_results.Count() - 1].location.latitude,
                                                          (double)IdentificationData.CDMA.BTS[i].level_results[IdentificationData.CDMA.BTS[i].level_results.Count() - 1].location.longitude,
                                                          (double)MainWindow.gps.LatitudeDecimal,
                                                          (double)MainWindow.gps.LongitudeDecimal,
                                                          out dist, out ang);
                                                }
                                                if ((IdentificationData.CDMA.BTS[i].level_results.Count == 0 ||
                                                    (decimal)dist > MainWindow.db_v2.Atdi_LevelResults_DistanceStep &&
                                                    IdentificationData.CDMA.BTS[i].level_results[IdentificationData.CDMA.BTS[i].level_results.Count - 1].level_dbm != IdentificationData.CDMA.BTS[i].RSCP) ||
                                                    new TimeSpan(MainWindow.gps.LocalTime.Ticks - IdentificationData.CDMA.BTS[i].level_results[IdentificationData.CDMA.BTS[i].level_results.Count() - 1].measurement_time.Ticks) > MainWindow.db_v2.Atdi_LevelsMeasurementsCar_TimeStep)
                                                {
                                                    #region
                                                    DB.localatdi_level_meas_result l = new DB.localatdi_level_meas_result()
                                                    {
                                                        difference_time_stamp_ns = 0,
                                                        level_dbm = Equipment.IdentificationData.CDMA.BTS[i].RSCP,
                                                        //level_dbmkvm = 0,
                                                        measurement_time = Equipment.IdentificationData.CDMA.BTS[i].LastLevelUpdete,
                                                        saved_in_db = false,
                                                        saved_in_result = false,
                                                        location = new DB.localatdi_geo_location()
                                                        {
                                                            //agl = 0,
                                                            asl = MainWindow.gps.Altitude,
                                                            latitude = (double)MainWindow.gps.LatitudeDecimal,
                                                            longitude = (double)MainWindow.gps.LongitudeDecimal,
                                                        }
                                                    };

                                                    IdentificationData.CDMA.BTS[i].LR_NewDataToSave = true;
                                                    App.Current.Dispatcher.Invoke((Action)(() =>
                                                    {
                                                        IdentificationData.CDMA.BTS[i].level_results.Add(l);
                                                    }));

                                                    #endregion
                                                }
                                            }
                                            #endregion
                                        }
                                    }
                                    if (find == false)
                                    {
                                        CDMABTSData dt = new CDMABTSData()
                                        {
                                            FreqIndex = pData.dwChannelIndex,
                                            Indicator = fpichResult.ExtendedPNOffset.wIndicator,
                                            Type = t.EVDOvsCDMA,
                                            //Channel = tt,
                                            ChannelN = tt.ChannelN,
                                            FreqDn = tt.FreqDn,
                                            FreqUp = tt.FreqUp,
                                            StandartSubband = tt.StandartSubband,
                                            PN = fpichResult.ExtendedPNOffset.wPNOffset,
                                            PTotal = fpichResult.sInbandPowerInDBm100 * 0.01,
                                            RSCP = fpichResult.sRSCPInDBm100 * 0.01,
                                            IcIo = 0 - Math.Round(fpichResult.sInbandPowerInDBm100 * 0.01 - fpichResult.sRSCPInDBm100 * 0.01, 2),
                                            LastLevelUpdete = MainWindow.gps.LocalTime
                                        };
                                        App.Current.Dispatcher.Invoke((Action)(() =>
                                        {
                                            IdentificationData.CDMA.BTS.Add(dt);
                                        }));
                                    }
                                }
                            }
                        }
                        #endregion

                        //int[] CIDarr = new int[] { };
                        //string CIDFrom = "";
                        if (pData.pSyncChannelDemodulationResult != null)
                        {
                            bool find = false;
                            for (int i = 0; i < IdentificationData.CDMA.BTS.Count; i++)
                            {
                                if (IdentificationData.CDMA.BTS[i].FreqIndex == pData.dwChannelIndex &&
                                    IdentificationData.CDMA.BTS[i].Indicator == pData.pDemodResult.wFirstBtsId &&
                                    IdentificationData.CDMA.BTS[i].PN == pData.pSyncChannelDemodulationResult.wPILOT_PN)
                                {
                                    find = true;
                                    if (IdentificationData.CDMA.BTS[i].SID != pData.pSyncChannelDemodulationResult.wSID ||
                                        IdentificationData.CDMA.BTS[i].NID != pData.pSyncChannelDemodulationResult.wNID)
                                    {
                                        IdentificationData.CDMA.BTS[i].SID = pData.pSyncChannelDemodulationResult.wSID;
                                        IdentificationData.CDMA.BTS[i].NID = pData.pSyncChannelDemodulationResult.wNID;
                                    }
                                    if (IdentificationData.CDMA.BTS[i].FullData == true)
                                    {
                                        int[] id = ParseIDtoDB(IdentificationData.CDMA.BTS[i]);
                                        if (id[0] > -1) IdentificationData.CDMA.BTS[i].CIDToDB = id[0];
                                    }
                                }
                            }
                            if (find == false)
                            {
                                CDMABTSData dt = new CDMABTSData()
                                {
                                    FreqIndex = pData.dwChannelIndex,
                                    Indicator = pData.pDemodResult.wBtsId,
                                    ChannelN = tt.ChannelN,
                                    FreqDn = tt.FreqDn,
                                    FreqUp = tt.FreqUp,
                                    StandartSubband = tt.StandartSubband,
                                    PN = pData.pSyncChannelDemodulationResult.wPILOT_PN,
                                    Type = t.EVDOvsCDMA,
                                    SID = pData.pSyncChannelDemodulationResult.wSID,
                                    NID = pData.pSyncChannelDemodulationResult.wNID,
                                };
                                if (dt.FullData == true)
                                {
                                    int[] id = ParseIDtoDB(dt);
                                    if (id[0] > -1) dt.CIDToDB = id[0];
                                }
                                App.Current.Dispatcher.Invoke((Action)(() =>
                                {
                                    IdentificationData.CDMA.BTS.Add(dt);
                                }));
                            }
                        }
                        #region Sib
                        if (pData.pDemodResult != null)
                        {
                            try
                            {
                                RohdeSchwarz.ViCom.Net.CDMA.SL3DecoderRequest dec = new RohdeSchwarz.ViCom.Net.CDMA.SL3DecoderRequest()
                                {
                                    dwBitCount = pData.pDemodResult.dwBitCount,
                                    PduSpec = pData.pDemodResult.PduSpec,
                                    pbBitStream = pData.pDemodResult.pbBitStream
                                };
                                RohdeSchwarz.ViCom.Net.CDMA.SL3DecoderResult dr = new RohdeSchwarz.ViCom.Net.CDMA.SL3DecoderResult();
                                dr = cdmaInterface.RetrieveTextForPDU(dec, SDefs.dwDefaultTimeOutInMs);

                                for (int i = 0; i < IdentificationData.CDMA.BTS.Count; i++)
                                {
                                    if (pData.dwChannelIndex == IdentificationData.CDMA.BTS[i].FreqIndex && pData.pDemodResult.wFirstBtsId == IdentificationData.CDMA.BTS[i].Indicator)
                                    {
                                        #region save sib data
                                        ObservableCollection<DB.local3GPPSystemInformationBlock> ibs = new ObservableCollection<DB.local3GPPSystemInformationBlock>(IdentificationData.CDMA.BTS[i].station_sys_info.information_blocks);
                                        bool fib = false;
                                        for (int ib = 0; ib < ibs.Count(); ib++)
                                        {
                                            if (ibs[ib].type == pData.pDemodResult.PduSpec.ePDU.ToString())
                                            {
                                                fib = true;
                                                ibs[ib].datastring = dr.pcPduText;
                                                ibs[ib].saved = DateTime.Now;
                                            }
                                        }
                                        if (fib == false)
                                        {
                                            DB.local3GPPSystemInformationBlock sib = new DB.local3GPPSystemInformationBlock()
                                            {
                                                datastring = dr.pcPduText,
                                                saved = DateTime.Now,
                                                type = pData.pDemodResult.PduSpec.ePDU.ToString()
                                            };
                                            App.Current.Dispatcher.Invoke((Action)(() =>
                                            {
                                                ibs.Add(sib);
                                                IdentificationData.CDMA.BTS[i].station_sys_info.information_blocks = ibs.ToArray();
                                            }));
                                        }
                                        #endregion save sib data
                                        #region parse
                                        if (dr.ePDU == RohdeSchwarz.ViCom.Net.CDMA.Pdu.Type.EVDO_SECTOR_PARAMETERS)
                                        {
                                            int[] data = ParseEVDO_SECTOR_PARAMETERS(dr.pcPduText);
                                            IdentificationData.CDMA.BTS[i].SID = data[0];
                                            IdentificationData.CDMA.BTS[i].NID = 0;
                                            IdentificationData.CDMA.BTS[i].BaseID = data[1];

                                            bool FullData = IdentificationData.CDMA.BTS[i].BaseID != -1 && IdentificationData.CDMA.BTS[i].SID != -1 && IdentificationData.CDMA.BTS[i].NID != -1;
                                            if (FullData)
                                            {
                                                string GCID = IdentificationData.CDMA.BTS[i].NID.ToString() + " " + IdentificationData.CDMA.BTS[i].SID.ToString() + " " +
                                                    string.Format("{0:00000}", IdentificationData.CDMA.BTS[i].PN) + " " + string.Format("{0:000000}", IdentificationData.CDMA.BTS[i].BaseID);
                                                IdentificationData.CDMA.BTS[i].GCID = GCID;
                                                int[] id = ParseIDtoDB(IdentificationData.CDMA.BTS[i]);
                                                if (id[0] > -1) IdentificationData.CDMA.BTS[i].CIDToDB = id[0];
                                            }
                                            IdentificationData.CDMA.BTS[i].FullData = FullData;
                                        }
                                        if (dr.ePDU == RohdeSchwarz.ViCom.Net.CDMA.Pdu.Type.SYS_PARAMS)
                                        {
                                            decimal[] data = ParseCDMA_SYS_PARAMS(dr.pcPduText);
                                            IdentificationData.CDMA.BTS[i].BaseID = (int)data[0];
                                            if (IdentificationData.CDMA.BTS[i].BaseID != -1 && IdentificationData.CDMA.BTS[i].SID != -1 && IdentificationData.CDMA.BTS[i].NID != -1)
                                            {
                                                IdentificationData.CDMA.BTS[i].GCID = IdentificationData.CDMA.BTS[i].NID.ToString() + " " + IdentificationData.CDMA.BTS[i].SID.ToString() + " " +
                                                      string.Format("{0:00000}", IdentificationData.CDMA.BTS[i].PN) + " " + string.Format("{0:000000}", IdentificationData.CDMA.BTS[i].BaseID);
                                                IdentificationData.CDMA.BTS[i].FullData = true;
                                            }

                                        }
                                        if (dr.ePDU == RohdeSchwarz.ViCom.Net.CDMA.Pdu.Type.EXT_SYS_PARAMS)
                                        {
                                            decimal[] data = ParseCDMA_EXT_SYS_PARAMS(dr.pcPduText);
                                            IdentificationData.CDMA.BTS[i].MCC = (int)data[0];
                                            IdentificationData.CDMA.BTS[i].MNC = (int)data[1];
                                        }

                                        if (IdentificationData.CDMA.BTS[i].FullData == true)
                                        {
                                            int[] id = ParseIDtoDB(IdentificationData.CDMA.BTS[i]);
                                            if (id[0] > -1) IdentificationData.CDMA.BTS[i].CIDToDB = id[0];
                                        }
                                        #endregion
                                    }
                                }
                            }
                            catch { }
                        }
                        #endregion
                    }
                    #region Exception
                    catch (RohdeSchwarz.ViCom.Net.CViComError error)
                    {
                        App.Current.Dispatcher.Invoke((Action)(() =>
                        {
                            MainWindow.exp.ExceptionData = new ExData() { ex = error, ClassName = "TSMxReceiver", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
                        }));
                    }
                    catch (Exception exp)
                    {
                        App.Current.Dispatcher.Invoke((Action)(() =>
                        {
                            MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "TSMxReceiver", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
                        }));
                    }
                    #endregion
                }
            }

        }

        private class MyRfPowerScanDataProcessor : RohdeSchwarz.ViCom.Net.RFPOWERSCAN.CViComRFPowerScanInterfaceDataProcessor
        {
            public override void RegisterScannerId(ushort dwScannerDataId)
            {
                //MainWindow.exp.ExceptionData = new ExData() { ex = new Exception("ID"), ClassName = "TSMxReceiver", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name + "\r\n" + dwScannerDataId };
            }

            public override void RemoveScannerId(ushort dwScannerDataId)
            {
                //MainWindow.exp.ExceptionData = new ExData() { ex = new Exception("ID"), ClassName = "TSMxReceiver", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name + "\r\n" + dwScannerDataId };
            }

            public override void OnScannerDataMeasured(RohdeSchwarz.ViCom.Net.RFPOWERSCAN.SMeasResult pData)
            {
                //try
                //{
                //    IsRuningT = true;
                //    _LastUpdate = DateTime.Now.Ticks;
                //    if (_FreqSet == true)
                //    {
                //        //Console.WriteLine("New spectrum result: bOverflow = {0}", pData.bOverflow);

                //        TracePoint[] tr = new TracePoint[pData.pSpectrumResult.dwCount];
                //        _FreqSpan = (decimal)rSSweepSettings.dStopFrequencyInHz - (decimal)rSSweepSettings.dStartFrequencyInHz;
                //        _FreqCentr = ((decimal)rSSweepSettings.dStopFrequencyInHz + (decimal)rSSweepSettings.dStartFrequencyInHz) / 2;
                //        _FreqStep = _FreqSpan / pData.pSpectrumResult.dwCount;
                //        _FreqStart = (decimal)rSSweepSettings.dStartFrequencyInHz;
                //        _FreqStop = (decimal)rSSweepSettings.dStopFrequencyInHz;
                //        //_FreqSpan = (decimal)RFPowerScanInterface.GetSettings().SweepSettings.dStopFrequencyInHz - (decimal)RFPowerScanInterface.GetSettings().SweepSettings.dStartFrequencyInHz;
                //        //_FreqCentr = ((decimal)RFPowerScanInterface.GetSettings().SweepSettings.dStopFrequencyInHz + (decimal)RFPowerScanInterface.GetSettings().SweepSettings.dStartFrequencyInHz) / 2;
                //        //_FreqStep = _FreqSpan / (decimal)RFPowerScanInterface.GetSettings().SweepSettings.sFrequencyDetector.dwCountOfLines;
                //        //_FreqStart = (decimal)RFPowerScanInterface.GetSettings().SweepSettings.dStartFrequencyInHz;
                //        //_FreqStop = (decimal)RFPowerScanInterface.GetSettings().SweepSettings.dStopFrequencyInHz;


                //        for (int i = 0; i < pData.pSpectrumResult.dwCount; i++)
                //        {
                //            TracePoint p = new TracePoint()
                //            {
                //                Freq = _FreqStart + _FreqStep * i,
                //                Level = (decimal)pData.pSpectrumResult.pfSpectrumValuesInDBm[i]
                //            };
                //            tr[i] = p;

                //        }
                //        TracefromDev = tr;
                //        _NewTrace = true;
                //    }
                //}
                //#region Exception
                //catch (RohdeSchwarz.ViCom.Net.CViComError error)
                //{
                //    MainWindow.exp.ExceptionData = new ExData() { ex = error, ClassName = "TSMxReceiver", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
                //}
                //catch (Exception exp)
                //{
                //    MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "TSMxReceiver", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
                //}
                //#endregion
            }

        }

        private class MyAcdDataProcessor : RohdeSchwarz.ViCom.Net.ACD.CViComAcdInterfaceDataProcessor
        {
            public override void RegisterScannerId(ushort dwScannerDataId)
            {
                //Debug.WriteLine("AcdDataProcessor registered at Scanner ID {0}", dwScannerDataId.ToString());
            }

            public override void RemoveScannerId(ushort dwScannerDataId)
            {
                //Debug.WriteLine("AcdDataProcessor unregistered from Scanner ID {0}", dwScannerDataId.ToString());
            }

            public override void OnScannerDataMeasured(RohdeSchwarz.ViCom.Net.ACD.SMeasResult pData)
            {
                if (IdentificationData.ACD.IsRuning == true)
                {
                    foreach (var channel in pData.ListOfChannels)
                    {
                        if (channel.enState == RohdeSchwarz.ViCom.Net.ACD.SMeasResult.SChannel.State.Type.DETECTED && channel.enDirection == RohdeSchwarz.ViCom.Net.ACD.SMeasResult.SChannel.Direction.Type.DOWNLINK)
                        {
                            bool find = false;
                            for (int i = 0; i < IdentificationData.ACD.ACDData.Count(); i++)
                            {
                                if (channel.u64FrequencyInHz == IdentificationData.ACD.ACDData[i].Freq && (int)channel.penTechnology == IdentificationData.ACD.ACDData[i].Tech)
                                {
                                    find = true;
                                    if (channel.pdwBandwidthInHz != null) IdentificationData.ACD.ACDData[i].BW = (decimal)channel.pdwBandwidthInHz;
                                    if (channel.pfRssiInDBm != null) IdentificationData.ACD.ACDData[i].RSSI = (decimal)channel.pfRssiInDBm;
                                    if (channel.pNetworkOperator != null)
                                    {
                                        IdentificationData.ACD.ACDData[i].MCC = channel.pNetworkOperator.wMCC;
                                        IdentificationData.ACD.ACDData[i].MNC = channel.pNetworkOperator.wMNC;
                                    }
                                    if (channel.pwBandId != null) IdentificationData.ACD.ACDData[i].Band = (int)channel.pwBandId;
                                }
                            }
                            if (find == false)
                            {
                                ACD_Data d = new ACD_Data()
                                {
                                    Freq = channel.u64FrequencyInHz,
                                    Tech = (int)channel.penTechnology,
                                    TechStr = channel.penTechnology.ToString(),
                                    Established = false,
                                };
                                if (d.TechStr == "WCDMA") d.TechStr = "UMTS";
                                if (channel.pdwBandwidthInHz != null) d.BW = (decimal)channel.pdwBandwidthInHz;
                                if (channel.pfRssiInDBm != null) d.RSSI = (decimal)channel.pfRssiInDBm;
                                if (channel.pNetworkOperator != null)
                                {
                                    d.MCC = channel.pNetworkOperator.wMCC;
                                    d.MNC = channel.pNetworkOperator.wMNC;
                                }
                                if (channel.pwBandId != null) d.Band = (int)channel.pwBandId;


                                if (d.Tech == 2)
                                {
                                    for (int i = 0; i < UMTSUniFreq.Count(); i++)
                                    {
                                        if (UMTSUniFreq[i] == d.Freq) d.Established = true;
                                    }
                                }
                                else if (d.Tech == 3)
                                {
                                    for (int i = 0; i < CDMAUniFreq.Count(); i++)
                                    {
                                        if (CDMAUniFreq[i].FreqDn == d.Freq && CDMAUniFreq[i].EVDOvsCDMA == false) d.Established = true;
                                    }
                                }
                                else if (d.Tech == 4)
                                {
                                    for (int i = 0; i < CDMAUniFreq.Count(); i++)
                                    {
                                        if (CDMAUniFreq[i].FreqDn == d.Freq && CDMAUniFreq[i].EVDOvsCDMA == true) d.Established = true;
                                    }
                                }
                                else if (d.Tech == 5)
                                {
                                    for (int i = 0; i < LTEUniFreq.Count(); i++)
                                    {
                                        if (LTEUniFreq[i] == d.Freq) d.Established = true;
                                    }
                                }
                                App.Current.Dispatcher.Invoke((Action)(() =>
                                {
                                    IdentificationData.ACD.ACDData.Add(d);
                                }));
                            }
                            //Debug.WriteLine("\tChannel: {0} Hz, State: {1}, Technology: {2}", channel.u64FrequencyInHz, channel.enState, channel.penTechnology);
                        }
                    }
                    bool umts = false;
                    bool CDMAEVDO = false;
                    bool lte = false;

                    for (int i = 0; i < IdentificationData.ACD.ACDData.Count(); i++)
                    {
                        if (IdentificationData.ACD.ACDData[i].Established == false)
                        {
                            if (IdentificationData.ACD.ACDData[i].Tech == 2) { umts = true; }
                            if (IdentificationData.ACD.ACDData[i].Tech == 3) { CDMAEVDO = true; }
                            if (IdentificationData.ACD.ACDData[i].Tech == 4) { CDMAEVDO = true; }
                            if (IdentificationData.ACD.ACDData[i].Tech == 5) { lte = true; }
                        }
                    }
                    if (umts && IdentificationData.UMTS.IsRuning) DM += UMTSSetFreqFromACD;
                    if (CDMAEVDO && IdentificationData.CDMA.IsRuning) DM += CDMASetFreqFromACD;
                    if (lte && IdentificationData.LTE.IsRuning) DM += LTESetFreqFromACD;
                }
            }
        }
        #endregion

        public DB.MeasMonBand MeasMonBandItem
        {
            get { return _MeasMonBandItem; }
            set { _MeasMonBandItem = value; OnPropertyChanged("MeasMonBandItem"); }
        }
        public DB.MeasMonBand _MeasMonBandItem = new DB.MeasMonBand() { };

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
    #region 
    public class SpanBW : INotifyPropertyChanged
    {


        public decimal Span
        {
            get { return _Span; }
            set { _Span = value; OnPropertyChanged("Span"); }
        }
        private decimal _Span = 0;


        public decimal StepRBW
        {
            get { return _StepRBW; }
            set { _StepRBW = value; OnPropertyChanged("StepRBW"); }
        }
        private decimal _StepRBW = 0;

        public event PropertyChangedEventHandler PropertyChanged; // Событие, которое нужно вызывать при изменении

        // Для удобства обернем событие в метод с единственным параметром - имя изменяемого свойства
        public void OnPropertyChanged(string propertyName)
        {
            // Если кто-то на него подписан, то вызывем его
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

    }
    #endregion

    public class GSMSIType
    {
        #region 
        private RohdeSchwarz.ViCom.Net.GSM.Pdu.Type _SiType;
        public RohdeSchwarz.ViCom.Net.GSM.Pdu.Type SiType
        {
            get { return _SiType; }
            set { _SiType = value; }
        }
        private bool _Use;
        public bool Use
        {
            get { return _Use; }
            set { _Use = value; }
        }
        #endregion
    }
    public class UMTSSIBType
    {
        #region 
        private RohdeSchwarz.ViCom.Net.WCDMA.Pdu.Type _SibType;
        public RohdeSchwarz.ViCom.Net.WCDMA.Pdu.Type SibType
        {
            get { return _SibType; }
            set { _SibType = value; }
        }
        private bool _Use;
        public bool Use
        {
            get { return _Use; }
            set { _Use = value; }
        }
        #endregion
    }

    public partial class AllTSMxUnqData : INotifyPropertyChanged
    {
        public string InstrModel { get; set; }
        public List<TSMxOption> InstrOption { get; set; }
        public List<TSMxOption> DefaultInstrOption { get; set; }
        public List<TSMxOption> LoadedInstrOption { get; set; }
        public Int64 MinFreq { get; set; }
        public Int64 MaxFreq { get; set; }
        private bool _PreAmp = false;
        /// <summary>
        /// True = есть False = нету
        /// </summary>
        public bool PreAmp
        {
            get { return _PreAmp; }
            set { _PreAmp = value; OnPropertyChanged("PreAmp"); }
        }
        private bool _Att = false;
        /// <summary>
        /// True = есть False = нету
        /// </summary>
        public bool Att
        {
            get { return _Att; }
            set { _Att = value; OnPropertyChanged("Att"); }
        }
        public int AttMax { get; set; }
        public int AttStep { get; set; }

        public List<TSMxFFTSize> FFTSize
        {
            get
            {
                List<TSMxFFTSize> _FFTSize = new List<TSMxFFTSize>() { };
                foreach (string t in Enum.GetNames(typeof(SSpectrumSettings.FFTSize.Type)))
                {
                    _FFTSize.Add(new TSMxFFTSize() { Parameter = t, UI = t.Split('_')[2] });
                }
                return _FFTSize;
            }
            set { }
        }
        //public List<TrDetector> TraceDetector
        //{
        //    get
        //    {
        //        List<TrDetector> _TraceDetector = new List<TrDetector>() { };
        //        foreach (string t in Enum.GetNames(typeof(SFrequencyDetector.FrequencyDetectorType.Type)))
        //        {
        //            _TraceDetector.Add(new TrDetector() { Parameter = t, UI = t.Split('_')[3] });
        //        }
        //        return _TraceDetector;
        //    }
        //    set { }
        //}
        //public List<TrType> TraceType
        //{
        //    get
        //    {
        //        List<TrType> _TraceType = new List<TrType>() { };
        //        foreach (string t in Enum.GetNames(typeof(STimeDetector.TimeDetectorType.Type)))
        //        {
        //            TraceType.Add(new TrType() { Parameter = t, UI = t.Split('_')[3] });
        //        }
        //        return TraceType;
        //    }
        //    set { }
        //}
        public List<SpanBW> SpanBWs
        {
            get { return _SpanBWs; }
            set { _SpanBWs = value; OnPropertyChanged("SpanBWs"); }
        }
        private List<SpanBW> _SpanBWs = new List<SpanBW>() { };
        private int _NumberOfTrace = 0;
        public int NumberOfTrace
        {
            get { return _NumberOfTrace; }
            set { _NumberOfTrace = value; OnPropertyChanged("NumberOfTrace"); }
        }

        private bool _SweepPointFix = false;
        public bool SweepPointFix
        {
            get { return _SweepPointFix; }
            set { _SweepPointFix = value; OnPropertyChanged("SweepPointFix"); }
        }
        public int[] SweepPointArr { get; set; }
        public int DefaultSweepPoint { get; set; }


        public event PropertyChangedEventHandler PropertyChanged; // Событие, которое нужно вызывать при изменении

        // Для удобства обернем событие в метод с единственным параметром - имя изменяемого свойства
        public void OnPropertyChanged(string propertyName)
        {
            // Если кто-то на него подписан, то вызывем его
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    public partial class TSMxOption
    {
        private string _Type = "";
        public string Type
        {
            get { return _Type; }
            set { _Type = value; }
        }
        private string _Globaltype = "";
        public string Globaltype
        {
            get { return _Globaltype; }
            set { _Globaltype = value; }
        }
        private string _Designation = "";
        public string Designation
        {
            get { return _Designation; }
            set { _Designation = value; }
        }
    }
    public partial class TSMxFFTSize
    {
        public string UI { get; set; }
        public string Parameter { get; set; }
    }


    //public partial class TrDetector
    //{
    //    public string UI { get; set; }
    //    public string Parameter { get; set; }
    //}
    //public partial class SwpType
    //{
    //    public string UI { get; set; }
    //    public string Parameter { get; set; }
    //}
}
