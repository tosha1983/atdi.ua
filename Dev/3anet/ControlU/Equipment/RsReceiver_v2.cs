using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net.NetworkInformation;

namespace ControlU.Equipment
{
    public class RsReceiver_v2 : PropertyChangedBase
    {
        Helpers.Helper help = new Helpers.Helper();
        LocalMeasurement LM = new LocalMeasurement();
        static GPSNMEA gp = MainWindow.gps;
        public TelnetConnection tc;
        static UdpStreaming uc;

        public System.Timers.Timer tmr = new System.Timers.Timer(10);
        private long LastUpdateTCP;
        private long LastUpdateUDP;

        public delegate void DoubMod();
        public DoubMod TelnetDM;
        public DoubMod UdpDM;
        public DoubMod TCPDM;
        public Thread TelnetTr;
        public Thread UdpTr;
        string AppPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        public DB.localatdi_meas_device device_meas
        {
            get { return _device_meas; }
            set { _device_meas = value; }
        }
        private DB.localatdi_meas_device _device_meas = new DB.localatdi_meas_device() { };

        public string ScreenName
        {
            get { return _ScreenName; }
            set { _ScreenName = value; OnPropertyChanged("ScreenName"); }
        }
        string _ScreenName = "";
        #region Global
        private mode _Mode = new mode { Mode = "FFM", MeasAppl = "RX", FreqMode = "CW" };
        public mode Mode
        {
            get { return _Mode; }
            set
            {
                LastMode = _Mode;
                _Mode = value;
                if (_Mode.Mode != "PSCAN") { PScanRun = false; }
                TelnetDM += SetStreamingMode;
                OnPropertyChanged("Mode");
            }
        }
        private mode _LastMode;
        public mode LastMode
        {
            get { return _LastMode; }
            set { _LastMode = value; OnPropertyChanged("LastMode"); }
        }

        #region Demode
        public int DemodBWInd
        {
            get { return _DemodBWInd; }
            set
            {
                if (value < 0) _DemodBWInd = UniqueData.DemodBW.Length - 1;
                else if (value > UniqueData.DemodBW.Length - 1) _DemodBWInd = 0;
                else _DemodBWInd = value;
                DemodBW = UniqueData.DemodBW[_DemodBWInd];
                OnPropertyChanged("DemodBWInd");
            }
        }
        private int _DemodBWInd = 0;

        public decimal DemodBW
        {
            get { return _DemodBW; }
            set { _DemodBW = value; OnPropertyChanged("DemodBW"); }
        }
        private decimal _DemodBW = 0;

        public int DemodInd
        {
            get { return _DemodInd; }
            set
            {
                if (value < 0) _DemodInd = UniqueData.Demod.Length - 1;
                else if (value > UniqueData.Demod.Length - 1) _DemodInd = 0;
                else _DemodInd = value;
                Demod = UniqueData.Demod[_DemodInd];
                OnPropertyChanged("DemodInd");
            }
        }
        private int _DemodInd = 0;

        public string Demod
        {
            get { return _Demod; }
            set { _Demod = value; OnPropertyChanged("Demod"); }
        }
        private string _Demod = "";
        #endregion Demod

        #region Detector
        public int DetectorInd
        {
            get { return _DetectorInd; }
            set
            {
                if (value < 0) _DetectorInd = UniqueData.Detectors.Count - 1;
                else if (value > UniqueData.Detectors.Count - 1) _DetectorInd = 0;
                else _DetectorInd = value;
                Detector = UniqueData.Detectors[_DetectorInd];
                OnPropertyChanged("DetectorInd");
            }
        }
        private int _DetectorInd = 0;

        private ParamWithUI _Detector = new ParamWithUI() { UI = "AVG", Parameter = "AVG" };
        public ParamWithUI Detector
        {
            get { return _Detector; }
            set { _Detector = value; OnPropertyChanged("Detector"); }
        }
        #endregion Detector

        public bool AFCState
        {
            get { return _AFCState; }
            set { _AFCState = value; OnPropertyChanged("AFCState"); }
        }
        private bool _AFCState = false;

        #region ATT
        public bool ATTFixState
        {
            get { return _ATTFixState; }
            set { _ATTFixState = value; OnPropertyChanged("ATTFixState"); }
        }
        private bool _ATTFixState = false;

        public int ATT
        {
            get { return _ATT; }
            set
            {
                if (value < 0) { _ATT = UniqueData.AttMax; }
                else if (value > UniqueData.AttMax) { _ATT = 0; }
                else _ATT = value;
                //if (_ATTAuto) { ATTStr = "AUTO ( " + help.helpLevel(_ATT, "dB") + " )"; }
                //else { ATTStr = help.helpLevel(_ATT, "dB"); }
                OnPropertyChanged("ATT");
            }
        }
        private int _ATT = 0;



        public bool ATTAutoState
        {
            get { return _ATTAutoState; }
            set { _ATTAutoState = value; OnPropertyChanged("ATTAutoState"); }
        }
        private bool _ATTAutoState;
        #endregion

        #region MGC
        public int MGC
        {
            get { return _MGC; }
            set
            {
                _MGC = value;
                if (_MGC < -30) { _MGC = -30; }
                else if (_MGC > 110) { _MGC = 110; }
                OnPropertyChanged("MGC");
            }
        }
        private int _MGC = 0;

        public bool MGCAutoState
        {
            get { return _MGCAutoState; }
            set { _MGCAutoState = value; OnPropertyChanged("MGCAutoState"); }
        }
        private bool _MGCAutoState = false;
        #endregion MGC

        #region SQU
        public int SQU
        {
            get { return _SQU; }
            set
            {
                _SQU = value;
                if (_SQU < -30) { _SQU = -30; }
                else if (_SQU > 130) { _SQU = 130; }
                OnPropertyChanged("SQU");
            }
        }
        public bool SQUState
        {
            get { return _SQUState; }
            set { _SQUState = value; SQU = SQU; OnPropertyChanged("SQUState"); }
        }
        #endregion SQU
        private AllRCVUnqData _UniqueData = new AllRCVUnqData { };
        public AllRCVUnqData UniqueData
        {
            get { return _UniqueData; }
            set { _UniqueData = value; OnPropertyChanged("UniqueData"); }
        }
        private ObservableCollection<AllRCVUnqData> AllUniqueData = new ObservableCollection<AllRCVUnqData>
        {
            #region EM100 
            new AllRCVUnqData
            {
                InstrManufacrure = 1, InstrModel = "EM100", InstrOption = new List<RCVOption>()
                {
                    #region 
                    new RCVOption() { Type = "PS", Designation = "Panorama Scan", Globaltype = "Panorama Scan"},
                    new RCVOption() { Type = "IR", Designation = "Internal Recording", Globaltype = "Internal Recording"},
                    new RCVOption() { Type = "RC", Designation = "Remote Control", Globaltype = "Remote Control"},
                    new RCVOption() { Type = "ET", Designation = "External Triggered Measurement", Globaltype = "External Triggered Measurement"},
                    new RCVOption() { Type = "FS", Designation = "Fieldstrength Measurement", Globaltype = "Fieldstrength Measurement"},
                    new RCVOption() { Type = "FP", Designation = "Frequency Processing SHF", Globaltype = "Frequency Processing SHF"},
                    new RCVOption() { Type = "GP", Designation = "GPS Compass", Globaltype = "GPS Compass"},
                    new RCVOption() { Type = "FE", Designation = "Frequency Extension", Globaltype = "Frequency Extension"},
                    new RCVOption() { Type = "DF", Designation = "Direction Finder", Globaltype = "Direction Finder"},
                    #endregion
                },
                LoadedInstrOption = new List<RCVOption>() { },
                Modes = new ObservableCollection<mode>
                {
                    new mode {Mode = "FFM", MeasAppl = "RX", FreqMode = "CW"},
                    new mode {Mode = "DF", MeasAppl = "DF", FreqMode = "DF"},
                    new mode {Mode = "PSCAN", MeasAppl = "RX", FreqMode = "PSC"},
                },
                DemodFreq = false,
                DemodBW = new decimal[] { 150, 300, 600, 1500, 2400, 6000, 9000, 12000, 15000, 30000, 50000, 120000, 150000, 250000, 300000, 500000 },
                Demod =  new string[] { "FM", "AM", "PULSE", "CW", "USB", "LSB", "IQ", "ISB", "PM" },
                FFMStepBW = new decimal[] { 0.625m, 1.25m, 3.125m, 6.25m, 12.5m, 31.25m, 62.5m, 125, 312.5m, 625, 1250, 3125, 6250},
                DFBW = new decimal[] { 50, 100, 150, 300, 600, 1000, 1500, 2100, 2400, 2700, 3100, 4000, 4800, 6000, 8333, 9000, 12000, 15000, 25000, 30000, 50000, 75000, 120000, 150000, 250000, 300000, 500000, 800000, 1000000, 1250000, 1500000, 2000000 },
                PSCANStepBW = new decimal[] { 125, 250, 500, 625, 1250, 2500, 3125, 6250, 12500m, 25000m, 50000, 100000},
                FFMSpanBW = new decimal[] { 1000, 2000, 5000, 10000, 20000, 50000, 100000, 200000, 500000, 1000000, 2000000, 5000000, 10000000},
                FFTModes = new ObservableCollection<ParamWithUI>
                {
                    new ParamWithUI() {UI = "MINIMUM", Parameter = "MIN"},
                    new ParamWithUI() {UI = "MAXIMUM", Parameter = "MAX"},
                    new ParamWithUI() {UI = "AVERAGE", Parameter = "SCAL"},
                    new ParamWithUI() {UI = "CLEAR WRITE", Parameter = "OFF"},
                },
                SelectivityChangeable = false,
                SelectivityModes = new ObservableCollection<ParamWithUI>()
                {
                    new ParamWithUI() {UI = "AUTO", Parameter = "AUTO"},
                    new ParamWithUI() {UI = "NORMAL", Parameter = "NORM"},
                    new ParamWithUI() {UI = "NARROW", Parameter = "NARR"},
                    new ParamWithUI() {UI = "SHRP", Parameter = "SHAR"},
                },
                Detectors = new ObservableCollection<ParamWithUI>()
                {
                    new ParamWithUI() {UI = "AVG", Parameter = "AVG"},
                    new ParamWithUI() {UI = "FAST", Parameter = "FAST"},
                    new ParamWithUI() {UI = "PEAK", Parameter = "PEAK"},
                    new ParamWithUI() {UI = "RMS", Parameter = "RMS"},
                },
                RFModeChangeable = false,
                RFModes= new ObservableCollection<RFMode>(){ },
                ATTFix = true,
                AttMax = 10,
                AttStep = 10,

                RefLevelMAX = 110,
                RefLevelMIN = -29,
                RangeLevelMAX = 140,
                RangeLevelMIN = 10,
                RangeLevelStep = 10
            },
            #endregion
            #region PR100
            new AllRCVUnqData
            {
                InstrManufacrure = 1, InstrModel = "PR100", InstrOption = new List<RCVOption>()
                {
                    #region 
                    new RCVOption() { Type = "PS", Designation = "Panorama Scan", Globaltype = "Panorama Scan"},
                    new RCVOption() { Type = "IR", Designation = "Internal Recording", Globaltype = "Internal Recording"},
                    new RCVOption() { Type = "RC", Designation = "Remote Control", Globaltype = "Remote Control"},
                    new RCVOption() { Type = "ET", Designation = "External Triggered Measurement", Globaltype = "External Triggered Measurement"},
                    new RCVOption() { Type = "FS", Designation = "Fieldstrength Measurement", Globaltype = "Fieldstrength Measurement"},
                    new RCVOption() { Type = "FP", Designation = "Frequency Processing SHF", Globaltype = "Frequency Processing SHF"},
                    new RCVOption() { Type = "GP", Designation = "GPS Compass", Globaltype = "GPS Compass"},
                    new RCVOption() { Type = "FE", Designation = "Frequency Extension", Globaltype = "Frequency Extension"},
                    new RCVOption() { Type = "DF", Designation = "Direction Finder", Globaltype = "Direction Finder"},
                    #endregion
                },
                LoadedInstrOption = new List<RCVOption>() { },
                Modes = new ObservableCollection<mode>
                {
                    new mode {Mode = "FFM", MeasAppl = "RX", FreqMode = "CW"},
                    new mode {Mode = "DF", MeasAppl = "DF", FreqMode = "DF"},
                    new mode {Mode = "PSCAN", MeasAppl = "RX", FreqMode = "PSC"},
                },
                DemodFreq = false,
                DemodBW = new decimal[] { 150, 300, 600, 1500, 2400, 6000, 9000, 12000, 15000, 30000, 50000, 120000, 150000, 250000, 300000, 500000 },
                Demod =  new string[] { "FM", "AM", "PULSE", "CW", "USB", "LSB", "IQ", "ISB", "PM" },
                FFMStepBW = new decimal[] { 0.625m, 1.25m, 3.125m, 6.25m, 12.5m, 31.25m, 62.5m, 125, 312.5m, 625, 1250, 3125, 6250},
                DFBW = new decimal[] { 50, 100, 150, 300, 600, 1000, 1500, 2100, 2400, 2700, 3100, 4000, 4800, 6000, 8333, 9000, 12000, 15000, 25000, 30000, 50000, 75000, 120000, 150000, 250000, 300000, 500000, 800000, 1000000, 1250000, 1500000, 2000000 },
                PSCANStepBW = new decimal[] { 125, 250, 500, 625, 1250, 2500, 3125, 6250, 12500m, 25000m, 50000, 100000},
                FFMSpanBW = new decimal[] { 1000, 2000, 5000, 10000, 20000, 50000, 100000, 200000, 500000, 1000000, 2000000, 5000000, 10000000},
                FFTModes = new ObservableCollection<ParamWithUI>
                {
                    new ParamWithUI() {UI = "MINIMUM", Parameter = "MIN"},
                    new ParamWithUI() {UI = "MAXIMUM", Parameter = "MAX"},
                    new ParamWithUI() {UI = "AVERAGE", Parameter = "SCAL"},
                    new ParamWithUI() {UI = "CLEAR WRITE", Parameter = "OFF"},
                },
                SelectivityChangeable = false,
                SelectivityModes = new ObservableCollection<ParamWithUI>()
                {
                    new ParamWithUI() {UI = "AUTO", Parameter = "AUTO"},
                    new ParamWithUI() {UI = "NORMAL", Parameter = "NORM"},
                    new ParamWithUI() {UI = "NARROW", Parameter = "NARR"},
                    new ParamWithUI() {UI = "SHRP", Parameter = "SHAR"},
                },
                Detectors = new ObservableCollection<ParamWithUI>()
                {
                    new ParamWithUI() {UI = "AVG", Parameter = "AVG"},
                    new ParamWithUI() {UI = "FAST", Parameter = "FAST"},
                    new ParamWithUI() {UI = "PEAK", Parameter = "PEAK"},
                    new ParamWithUI() {UI = "RMS", Parameter = "RMS"},
                },
                RFModeChangeable = false,
                RFModes= new ObservableCollection<RFMode>(){ },
                ATTFix = true,
                AttMax = 10,
                AttStep = 10,

                RefLevelMAX = 110,
                RefLevelMIN = -29,
                RangeLevelMAX = 140,
                RangeLevelMIN = 10,
                RangeLevelStep = 10
            },
            #endregion
            #region ESMD
            new AllRCVUnqData
            {
                InstrManufacrure = 1, InstrModel = "ESMD", InstrOption = new List<RCVOption>()
                {
                    #region 
                    new RCVOption() { Type = "PS", Designation = "Panorama Scan", Globaltype = "Panorama Scan"},
                    new RCVOption() { Type = "IR", Designation = "Internal Recording", Globaltype = "Internal Recording"},
                    new RCVOption() { Type = "RC", Designation = "Remote Control", Globaltype = "Remote Control"},
                    new RCVOption() { Type = "ET", Designation = "External Triggered Measurement", Globaltype = "External Triggered Measurement"},
                    new RCVOption() { Type = "FS", Designation = "Fieldstrength Measurement", Globaltype = "Fieldstrength Measurement"},
                    new RCVOption() { Type = "FP", Designation = "Frequency Processing SHF", Globaltype = "Frequency Processing SHF"},
                    new RCVOption() { Type = "GP", Designation = "GPS Compass", Globaltype = "GPS Compass"},
                    new RCVOption() { Type = "FE", Designation = "Frequency Extension", Globaltype = "Frequency Extension"},
                    new RCVOption() { Type = "DF", Designation = "Direction Finder", Globaltype = "Direction Finder"},
                    #endregion
                },
                LoadedInstrOption = new List<RCVOption>() { },
                Modes = new ObservableCollection<mode>
                {
                    new mode {Mode = "FFM", MeasAppl = "RX", FreqMode = "CW"},
                    new mode {Mode = "DF", MeasAppl = "DF", FreqMode = "CW"},
                    new mode {Mode = "PSCAN", MeasAppl = "RX", FreqMode = "PSC"},
                },
                DemodFreq = true,
                DemodBW = new decimal[] { 100, 150, 300, 600, 1000, 1500, 2100, 2400, 2700, 3100, 4000, 4800, 6000, 8300, 9000, 12000, 15000, 25000, 30000, 50000, 75000, 120000, 150000, 250000, 300000, 500000, 800000, 1000000, 1250000, 1500000, 2000000, 5000000, 8000000, 10000000, 12500000, 15000000, 20000000 },
                Demod =  new string[] { "AM", "FM", "PULSE", "PM", "IQ", "ISB", "TV" },
                FFMStepBW = new decimal[] { 0.625m, 1.25m, 2.5m, 3.125m, 6.25m, 12.5m, 25, 31.25m, 50, 62.5m, 100, 125, 200, 250, 312.5m, 500, 625, 1000, 1250, 2000, 2500, 3125, 5000, 6250, 8333, 10000, 12500, 20000, 25000, 50000, 100000, 200000, 500000, 1000000, 2000000 },
                DFBW = new decimal[] { 50, 100, 150, 300, 600, 1000, 1500, 2100, 2400, 2700, 3100, 4000, 4800, 6000, 8333, 9000, 12000, 15000, 25000, 30000, 50000, 75000, 120000, 150000, 250000, 300000, 500000, 800000, 1000000, 1250000, 1500000, 2000000 },
                PSCANStepBW = new decimal[] { 0.625m, 1.25m, 2.5m, 3.125m, 6.25m, 12.5m, 25, 31.25m, 50, 62.5m, 100, 125, 200, 250, 312.5m, 500, 625, 1000, 1250, 2000, 2500, 3125, 5000, 6250, 8333, 10000, 12500, 20000, 25000, 50000, 100000, 200000, 500000, 1000000, 2000000 },
                FFMSpanBW = new decimal[] { 1000, 2000, 5000, 10000, 20000, 50000, 100000, 200000, 500000, 1000000, 2000000, 5000000, 10000000, 20000000, 40000000, 80000000 },
                FFTModes = new ObservableCollection<ParamWithUI>
                {
                    new ParamWithUI() {UI = "MINIMUM", Parameter = "MIN"},
                    new ParamWithUI() {UI = "MAXIMUM", Parameter = "MAX"},
                    new ParamWithUI() {UI = "AVERAGE", Parameter = "SCAL"},
                    new ParamWithUI() {UI = "CLEAR WRITE", Parameter = "OFF"},
                },
                SelectivityChangeable = true,
                SelectivityModes = new ObservableCollection<ParamWithUI>()
                {
                    new ParamWithUI() {UI = "AUTO", Parameter = "AUTO"},
                    new ParamWithUI() {UI = "NORMAL", Parameter = "NORM"},
                    new ParamWithUI() {UI = "NARROW", Parameter = "NARR"},
                    new ParamWithUI() {UI = "SHRP", Parameter = "SHAR"},
                },
                Detectors = new ObservableCollection<ParamWithUI>()
                {
                    new ParamWithUI() {UI = "AVG", Parameter = "PAV"},
                    new ParamWithUI() {UI = "PEAK", Parameter = "POS"},
                    new ParamWithUI() {UI = "FAST", Parameter = "FAST"},
                    new ParamWithUI() {UI = "RMS", Parameter = "RMS"},
                },
                RFModeChangeable = true,
                RFModes= new ObservableCollection<RFMode>()
                {
                    new RFMode() {UI = "NORMAL", Parameter = "NORM"},
                    new RFMode() {UI = "LOW NOISE", Parameter = "LOWN"},
                    new RFMode() {UI = "LOW DISTORTION", Parameter = "LOWD"},
                },
                ATTFix = false,
                AttMax = 70,
                AttStep = 1,

                RefLevelMAX = 130,
                RefLevelMIN = -29,
                RangeLevelMAX = 200,
                RangeLevelMIN = 10,
                RangeLevelStep = 1
            },
            #endregion
            new AllRCVUnqData {InstrManufacrure = 1, InstrModel = "DDF2",
                #region
                InstrOption = new List<RCVOption>()
                {
                    #region 
                    new RCVOption() { Type = "PS", Designation = "Panorama Scan", Globaltype = "Panorama Scan"},
                    new RCVOption() { Type = "IR", Designation = "Internal Recording", Globaltype = "Internal Recording"},
                    new RCVOption() { Type = "RC", Designation = "Remote Control", Globaltype = "Remote Control"},
                    new RCVOption() { Type = "ET", Designation = "External Triggered Measurement", Globaltype = "External Triggered Measurement"},
                    new RCVOption() { Type = "FS", Designation = "Fieldstrength Measurement", Globaltype = "Fieldstrength Measurement"},
                    new RCVOption() { Type = "FP", Designation = "Frequency Processing SHF", Globaltype = "Frequency Processing SHF"},
                    new RCVOption() { Type = "GP", Designation = "GPS Compass", Globaltype = "GPS Compass"},
                    new RCVOption() { Type = "FE", Designation = "Frequency Extension", Globaltype = "Frequency Extension"},
                    new RCVOption() { Type = "DF", Designation = "Direction Finder", Globaltype = "Direction Finder"},
                    #endregion
                },
                LoadedInstrOption = new List<RCVOption>() { },
                Modes = new ObservableCollection<mode>
                {
                    new mode {Mode = "FFM", MeasAppl = "RX", FreqMode = "CW"},
                    new mode {Mode = "DF", MeasAppl = "DF", FreqMode = "CW"},
                    new mode {Mode = "PSCAN", MeasAppl = "RX", FreqMode = "PSC"},
                },
                DemodFreq = true,
                DemodBW = new decimal[] { 100, 150, 300, 600, 1000, 1500, 2100, 2400, 2700, 3100, 4000, 4800, 6000, 8300, 9000, 12000, 15000, 25000, 30000, 50000, 75000, 120000, 150000, 250000, 300000, 500000, 800000, 1000000, 1250000, 1500000, 2000000, 5000000, 8000000, 10000000, 12500000, 15000000, 20000000 },
                Demod =  new string[] { "AM", "FM", "PULSE", "PM", "IQ", "ISB", "TV" },
                FFMStepBW = new decimal[] { 0.625m, 1.25m, 2.5m, 3.125m, 6.25m, 12.5m, 25, 31.25m, 50, 62.5m, 100, 125, 200, 250, 312.5m, 500, 625, 1000, 1250, 2000, 2500, 3125, 5000, 6250, 8333, 10000, 12500, 20000, 25000, 50000, 100000, 200000, 500000, 1000000, 2000000 },
                DFBW = new decimal[] { 50, 100, 150, 300, 600, 1000, 1500, 2100, 2400, 2700, 3100, 4000, 4800, 6000, 8333, 9000, 12000, 15000, 25000, 30000, 50000, 75000, 120000, 150000, 250000, 300000, 500000, 800000, 1000000, 1250000, 1500000, 2000000 },
                PSCANStepBW = new decimal[] { 0.625m, 1.25m, 2.5m, 3.125m, 6.25m, 12.5m, 25, 31.25m, 50, 62.5m, 100, 125, 200, 250, 312.5m, 500, 625, 1000, 1250, 2000, 2500, 3125, 5000, 6250, 8333, 10000, 12500, 20000, 25000, 50000, 100000, 200000, 500000, 1000000, 2000000 },
                FFMSpanBW = new decimal[] { 1000, 2000, 5000, 10000, 20000, 50000, 100000, 200000, 500000, 1000000, 2000000, 5000000, 10000000, 20000000, 40000000, 80000000 },
                FFTModes = new ObservableCollection<ParamWithUI>
                {
                    new ParamWithUI() {UI = "MINIMUM", Parameter = "MIN"},
                    new ParamWithUI() {UI = "MAXIMUM", Parameter = "MAX"},
                    new ParamWithUI() {UI = "AVERAGE", Parameter = "SCAL"},
                    new ParamWithUI() {UI = "CLEAR WRITE", Parameter = "OFF"},
                },
                SelectivityChangeable = true,
                SelectivityModes = new ObservableCollection<ParamWithUI>()
                {
                    new ParamWithUI() {UI = "AUTO", Parameter = "AUTO"},
                    new ParamWithUI() {UI = "NORMAL", Parameter = "NORM"},
                    new ParamWithUI() {UI = "NARROW", Parameter = "NARR"},
                    new ParamWithUI() {UI = "SHRP", Parameter = "SHAR"},
                },
                Detectors = new ObservableCollection<ParamWithUI>()
                {
                    new ParamWithUI() {UI = "AVG", Parameter = "PAV"},
                    new ParamWithUI() {UI = "PEAK", Parameter = "POS"},
                    new ParamWithUI() {UI = "FAST", Parameter = "FAST"},
                    new ParamWithUI() {UI = "RMS", Parameter = "RMS"},
                },
                RFModeChangeable = true,
                RFModes= new ObservableCollection<RFMode>()
                {
                    new RFMode() {UI = "NORMAL", Parameter = "NORM"},
                    new RFMode() {UI = "LOW NOISE", Parameter = "LOWN"},
                    new RFMode() {UI = "LOW DISTORTION", Parameter = "LOWD"},
                },
                ATTFix = false,
                AttMax = 70,
                AttStep = 1,

                RefLevelMAX = 130,
                RefLevelMIN = -29,
                RangeLevelMAX = 200,
                RangeLevelMIN = 10,
                RangeLevelStep = 1
                #endregion
            },
            new AllRCVUnqData {InstrManufacrure = 1, InstrModel = "EB500",
                #region
                InstrOption = new List<RCVOption>()
                {
                    #region 
                    new RCVOption() { Type = "PS", Designation = "Panorama Scan", Globaltype = "Panorama Scan"},
                    new RCVOption() { Type = "IR", Designation = "Internal Recording", Globaltype = "Internal Recording"},
                    new RCVOption() { Type = "RC", Designation = "Remote Control", Globaltype = "Remote Control"},
                    new RCVOption() { Type = "ET", Designation = "External Triggered Measurement", Globaltype = "External Triggered Measurement"},
                    new RCVOption() { Type = "FS", Designation = "Fieldstrength Measurement", Globaltype = "Fieldstrength Measurement"},
                    new RCVOption() { Type = "FP", Designation = "Frequency Processing SHF", Globaltype = "Frequency Processing SHF"},
                    new RCVOption() { Type = "GP", Designation = "GPS Compass", Globaltype = "GPS Compass"},
                    new RCVOption() { Type = "FE", Designation = "Frequency Extension", Globaltype = "Frequency Extension"},
                    new RCVOption() { Type = "DF", Designation = "Direction Finder", Globaltype = "Direction Finder"},
                    #endregion
                },
                LoadedInstrOption = new List<RCVOption>() { },
                Modes = new ObservableCollection<mode>
                {
                    new mode {Mode = "FFM", MeasAppl = "RX", FreqMode = "CW"},
                    new mode {Mode = "DF", MeasAppl = "DF", FreqMode = "CW"},
                    new mode {Mode = "PSCAN", MeasAppl = "RX", FreqMode = "PSC"},
                },
                DemodFreq = false,
                DemodBW = new decimal[] { 100, 150, 300, 600, 1000, 1500, 2100, 2400, 2700, 3100, 4000, 4800, 6000, 8300, 9000, 12000, 15000, 25000, 30000, 50000, 75000, 120000, 150000, 250000, 300000, 500000, 800000, 1000000, 1250000, 1500000, 2000000},
                Demod =  new string[] { "AM", "FM", "PULSE", "PM", "IQ", "ISB", "TV" },
                FFMStepBW = new decimal[] { 0.625m, 1.25m, 2.5m, 3.125m, 6.25m, 12.5m, 25, 31.25m, 50, 62.5m, 100, 125, 200, 250, 312.5m, 500, 625, 1000, 1250, 2000, 2500, 3125, 5000, 6250, 8333, 10000, 12500, 20000, 25000, 50000, 100000, 200000, 500000, 1000000, 2000000 },
                DFBW = new decimal[] { 50, 100, 150, 300, 600, 1000, 1500, 2100, 2400, 2700, 3100, 4000, 4800, 6000, 8333, 9000, 12000, 15000, 25000, 30000, 50000, 75000, 120000, 150000, 250000, 300000, 500000, 800000, 1000000, 1250000, 1500000, 2000000 },
                PSCANStepBW = new decimal[] { 0.625m, 1.25m, 2.5m, 3.125m, 6.25m, 12.5m, 25, 31.25m, 50, 62.5m, 100, 125, 200, 250, 312.5m, 500, 625, 1000, 1250, 2000, 2500, 3125, 5000, 6250, 8333, 10000, 12500, 20000, 25000, 50000, 100000, 200000, 500000, 1000000, 2000000 },
                FFMSpanBW = new decimal[] { 1000, 2000, 5000, 10000, 20000, 50000, 100000, 200000, 500000, 1000000, 2000000, 5000000, 10000000, 20000000 },
                FFTModes = new ObservableCollection<ParamWithUI>
                {
                    new ParamWithUI() {UI = "MINIMUM", Parameter = "MIN"},
                    new ParamWithUI() {UI = "MAXIMUM", Parameter = "MAX"},
                    new ParamWithUI() {UI = "AVERAGE", Parameter = "SCAL"},
                    new ParamWithUI() {UI = "CLEAR WRITE", Parameter = "OFF"},
                },
                SelectivityChangeable = true,
                SelectivityModes = new ObservableCollection<ParamWithUI>()
                {
                    new ParamWithUI() {UI = "AUTO", Parameter = "AUTO"},
                    new ParamWithUI() {UI = "NORMAL", Parameter = "NORM"},
                    new ParamWithUI() {UI = "NARROW", Parameter = "NARR"},
                    new ParamWithUI() {UI = "SHRP", Parameter = "SHAR"},
                },
                Detectors = new ObservableCollection<ParamWithUI>()
                {
                    new ParamWithUI() {UI = "AVG", Parameter = "PAV"},
                    new ParamWithUI() {UI = "PEAK", Parameter = "POS"},
                    new ParamWithUI() {UI = "FAST", Parameter = "FAST"},
                    new ParamWithUI() {UI = "RMS", Parameter = "RMS"},
                },
                RFModeChangeable = true,
                RFModes= new ObservableCollection<RFMode>()
                {
                    new RFMode() {UI = "NORMAL", Parameter = "NORM"},
                    new RFMode() {UI = "LOW NOISE", Parameter = "LOWN"},
                    new RFMode() {UI = "LOW DISTORTION", Parameter = "LOWD"},
                },
                ATTFix = false,
                AttMax = 70,
                AttStep = 1,

                DFSQUMAX = 130,
                DFSQUMIN = -50,
                DFMeasTimeMAX = 10000,
                DFMeasTimeMIN = 0.1m,

                RefLevelMAX = 130,
                RefLevelMIN = -29,
                RangeLevelMAX = 200,
                RangeLevelMIN = 10,
                RangeLevelStep = 1
                #endregion
            }
        };
        public bool MeasMode
        {
            get { return _MeasMode; }
            set { _MeasMode = value; OnPropertyChanged("MeasMode"); }
        }
        private bool _MeasMode = false;

        private ParamWithUI _PScanFFTMode = new ParamWithUI { UI = "CLEAR WRITE", Parameter = "OFF" };
        public ParamWithUI PScanFFTMode
        {
            get { return _PScanFFTMode; }
            set { _PScanFFTMode = value; OnPropertyChanged("PScanFFTMode"); }
        }

        private ParamWithUI _SelectivityMode = new ParamWithUI() { UI = "NORMAL", Parameter = "NORM" };
        public ParamWithUI SelectivityMode
        {
            get { return _SelectivityMode; }
            set { _SelectivityMode = value; OnPropertyChanged("SelectivityMode"); }
        }

        #region Trace
        public tracepoint[] Trace1
        {
            get { return _Trace1; }
            set { _Trace1 = value; OnPropertyChanged("Trace1"); }
        }
        private tracepoint[] _Trace1;
        public tracepoint[] Trace2
        {
            get { return _Trace2; }
            set { _Trace2 = value; OnPropertyChanged("Trace2"); }
        }
        private tracepoint[] _Trace2;

        public tracepoint[] Trace3
        {
            get { return _Trace3; }
            set { _Trace3 = value; OnPropertyChanged("Trace3"); }
        }
        private tracepoint[] _Trace3;

        public tracepoint[] TraceTemp
        {
            get { return _TraceTemp; }
            set { _TraceTemp = value; OnPropertyChanged("TraceTemp"); }
        }
        private tracepoint[] _TraceTemp;

        public ObservableCollection<ParamWithUI> TraceTypes
        {
            get { return _TraceTypes; }
            set { _TraceTypes = value; OnPropertyChanged("TraceTypes"); }
        }
        private ObservableCollection<ParamWithUI> _TraceTypes = new ObservableCollection<ParamWithUI>
        {
            new AllTraceTypes().TraceTypes[0],
            new AllTraceTypes().TraceTypes[1],
            new AllTraceTypes().TraceTypes[2],
            new AllTraceTypes().TraceTypes[3],
            new AllTraceTypes().TraceTypes[4],
            new AllTraceTypes().TraceTypes[5],
            new AllTraceTypes().TraceTypes[6],
        };
        #region trace 1
        public int Trace1TypeIndex
        {
            get { return _Trace1TypeIndex; }
            set
            {
                if (value > 6) _Trace1TypeIndex = 6;
                else if (value < 0) _Trace1TypeIndex = 0;
                else _Trace1TypeIndex = value;
                Trace1Type = TraceTypes[_Trace1TypeIndex];
                OnPropertyChanged("Trace1TypeIndex");
            }
        }
        private int _Trace1TypeIndex = 0;
        public ParamWithUI Trace1Type
        {
            get { return _Trace1Type; }
            set { _Trace1Type = value; OnPropertyChanged("Trace1Type"); }
        }
        private ParamWithUI _Trace1Type = new ParamWithUI() { Parameter = "0", UI = "Clear Write" };

        public bool Trace1Reset = false;
        #region Average Tracking
        public AverageList Trace1AveragedList
        {
            get { return _Trace1AveragedList; }
            set { _Trace1AveragedList = value; OnPropertyChanged("Trace1AveragedList"); }
        }
        public AverageList _Trace1AveragedList = new AverageList();
        public TrackingList Trace1TrackedList
        {
            get { return _Trace1TrackedList; }
            set { _Trace1TrackedList = value; OnPropertyChanged("Trace1TrackedList"); }
        }
        public TrackingList _Trace1TrackedList = new TrackingList();
        #endregion
        #endregion trace 1

        #region trace 2
        public int Trace2TypeIndex
        {
            get { return _Trace2TypeIndex; }
            set
            {
                if (value > 6) _Trace2TypeIndex = 6;
                else if (value < 0) _Trace2TypeIndex = 0;
                else _Trace2TypeIndex = value;
                Trace2Type = TraceTypes[_Trace2TypeIndex];
                OnPropertyChanged("Trace2TypeIndex");
            }
        }
        private int _Trace2TypeIndex = 6;
        public ParamWithUI Trace2Type
        {
            get { return _Trace2Type; }
            set { _Trace2Type = value; OnPropertyChanged("Trace2Type"); }
        }
        private ParamWithUI _Trace2Type = new ParamWithUI() { Parameter = "6", UI = "Blank" };

        public bool Trace2Reset = false;
        #region Average Tracking
        public AverageList Trace2AveragedList
        {
            get { return _Trace2AveragedList; }
            set { _Trace2AveragedList = value; OnPropertyChanged("Trace2AveragedList"); }
        }
        public AverageList _Trace2AveragedList = new AverageList();
        public TrackingList Trace2TrackedList
        {
            get { return _Trace2TrackedList; }
            set { _Trace2TrackedList = value; OnPropertyChanged("Trace2TrackedList"); }
        }
        public TrackingList _Trace2TrackedList = new TrackingList();
        #endregion
        #endregion trace 2

        #region trace 3
        public int Trace3TypeIndex
        {
            get { return _Trace3TypeIndex; }
            set
            {
                if (value > 6) _Trace3TypeIndex = 6;
                else if (value < 0) _Trace3TypeIndex = 0;
                else _Trace3TypeIndex = value;
                Trace3Type = TraceTypes[_Trace3TypeIndex];
                OnPropertyChanged("Trace3TypeIndex");
            }
        }
        private int _Trace3TypeIndex = 6;
        public ParamWithUI Trace3Type
        {
            get { return _Trace3Type; }
            set { _Trace3Type = value; OnPropertyChanged("Trace3Type"); }
        }
        private ParamWithUI _Trace3Type = new ParamWithUI() { Parameter = "6", UI = "Blank" };

        public bool Trace3Reset = false;
        #region Average Tracking
        public AverageList Trace3AveragedList
        {
            get { return _Trace3AveragedList; }
            set { _Trace3AveragedList = value; OnPropertyChanged("Trace3AveragedList"); }
        }
        public AverageList _Trace3AveragedList = new AverageList();
        public TrackingList Trace3TrackedList
        {
            get { return _Trace3TrackedList; }
            set { _Trace3TrackedList = value; OnPropertyChanged("Trace3TrackedList"); }
        }
        public TrackingList _Trace3TrackedList = new TrackingList();
        #endregion
        #endregion trace 3
        #endregion Trace

        #region MeasTime
        public decimal MeasTime
        {
            get { return _MeasTime; }
            set { if (value >= 0.0005M && MeasTime <= 900) _MeasTime = value; OnPropertyChanged("MeasTime"); }
        }
        private decimal _MeasTime = 0.0005M;
        public bool MeasTimeAuto
        {
            get { return _MeasTimeAuto; }
            set { _MeasTimeAuto = value; OnPropertyChanged("MeasTimeAuto"); }
        }
        private bool _MeasTimeAuto;
        #endregion MeasTime

        #region Level Out
        public decimal RefLevel
        {
            get { return _RefLevel; }
            set { _RefLevel = value; OnPropertyChanged("RefLevel"); }
        }
        private decimal _RefLevel = 80;

        public decimal Range
        {
            get { return _Range; }
            set { _Range = value; OnPropertyChanged("Range"); }
        }
        private decimal _Range = 110;

        public decimal LowestLevel
        {
            get { return _LowestLevel; }
            set { _LowestLevel = value; OnPropertyChanged("LowestLevel"); }
        }
        private decimal _LowestLevel = -30;
        #endregion


        #region Freq Out
        /// <summary>
        /// true = CentrSpan
        /// false = StartStop
        /// </summary>
        public bool Freq_CentrSpan_StartStop
        {
            get { return _Freq_CentrSpan_StartStop; }
            set { _Freq_CentrSpan_StartStop = value; OnPropertyChanged("Freq_CentrSpan_StartStop"); }
        }
        private bool _Freq_CentrSpan_StartStop;

        public decimal FreqCentr
        {
            get { return _FreqCentr; }
            set
            {
                _FreqCentr = value;
                _FreqStart = _FreqCentr - _FreqSpan / 2; OnPropertyChanged("FreqStart");
                _FreqStop = _FreqCentr + _FreqSpan / 2; OnPropertyChanged("FreqStop");
                Freq_CentrSpan_StartStop = true;
                OnPropertyChanged("FreqCentr");
            }
        }
        private decimal _FreqCentr = 950000000;

        public decimal FreqSpan
        {
            get { return _FreqSpan; }
            set
            {
                _FreqSpan = value;
                _FreqStart = _FreqCentr - _FreqSpan / 2; OnPropertyChanged("FreqStart");
                _FreqStop = _FreqCentr + _FreqSpan / 2; OnPropertyChanged("FreqStop");
                Freq_CentrSpan_StartStop = true;
                OnPropertyChanged("FreqSpan");
            }
        }
        private decimal _FreqSpan = 5000000;

        public decimal FreqStart
        {
            get { return _FreqStart; }
            set
            {
                _FreqStart = value;
                _FreqCentr = (_FreqStart + _FreqStop) / 2; OnPropertyChanged("FreqCentr");
                _FreqSpan = _FreqStop - _FreqStart; OnPropertyChanged("FreqSpan");
                Freq_CentrSpan_StartStop = false;
                OnPropertyChanged("FreqStart");
            }
        }
        private decimal _FreqStart = 947500000;

        public decimal FreqStop
        {
            get { return _FreqStop; }
            set
            {
                _FreqStop = value;
                _FreqCentr = (_FreqStart + _FreqStop) / 2; OnPropertyChanged("FreqCentr");
                _FreqSpan = _FreqStop - _FreqStart; OnPropertyChanged("FreqSpan");
                Freq_CentrSpan_StartStop = false;
                OnPropertyChanged("FreqStop");
            }
        }
        private decimal _FreqStop = 952500000;

        public decimal MinFreqReceiver
        {
            get { return _MinFreqReceiver; }
            set { if (_MinFreqReceiver != value) { _MinFreqReceiver = value; OnPropertyChanged("MinFreqReceiver"); } }
        }
        private decimal _MinFreqReceiver = 10;

        public decimal MaxFreqReceiver
        {
            get { return _MaxFreqReceiver; }
            set { if (_MaxFreqReceiver != value) { _MaxFreqReceiver = value; OnPropertyChanged("MaxFreqReceiver"); } }
        }
        private decimal _MaxFreqReceiver = 30000000000;
        #endregion Freq Out

        #region FFM
        #region FreqCentr
        public decimal FFMFreqCentr
        {
            get { return _FFMFreqCentr; }
            set { _FFMFreqCentr = value; OnPropertyChanged("FFMFreqCentr"); }
        }
        private decimal _FFMFreqCentr = 100000000;
        public decimal FFMFreqCentrToSet
        {
            get { return _FFMFreqCentrToSet; }
            set
            {
                if (FFMFreqCentrToSet < MinFreqReceiver) _FFMFreqCentrToSet = MinFreqReceiver;
                if (FFMFreqCentrToSet > MaxFreqReceiver) _FFMFreqCentrToSet = MaxFreqReceiver;
                _FFMFreqCentrToSet = value;
            }
        }
        private decimal _FFMFreqCentrToSet = 100000000;
        #endregion FreqCentr

        #region FreqSpan
        public int FFMFreqSpanInd
        {
            get { return _FFMFreqSpanInd; }
            set { if (value > -1 && value < UniqueData.FFMSpanBW.Length) _FFMFreqSpanInd = value; FFMFreqSpan = UniqueData.FFMSpanBW[_FFMFreqSpanInd]; OnPropertyChanged("FFMSpanInd"); }
        }
        private int _FFMFreqSpanInd = 1;

        public int FFMFreqSpanIndToSet
        {
            get { return _FFMFreqSpanIndToSet; }
            set { if (value > -1 && value < UniqueData.FFMSpanBW.Length) _FFMFreqSpanIndToSet = value; }
        }
        private int _FFMFreqSpanIndToSet = 1;

        public decimal FFMFreqSpan
        {
            get { return _FFMFreqSpan; }
            set { _FFMFreqSpan = value; FreqSpan = FFMFreqSpan; OnPropertyChanged("FFMFreqSpan"); }
        }
        private decimal _FFMFreqSpan = 10000000;
        #endregion FreqSpan

        #region FFMStep
        public int FFMStepInd
        {
            get { return _FFMStepInd; }
            set
            {
                if (value > -1 && value < UniqueData.FFMStepBW.Length)
                {
                    _FFMStepInd = value;
                    FFMStep = UniqueData.FFMStepBW[FFMStepInd];
                }
                OnPropertyChanged("FFMStepInd");
            }
        }
        private int _FFMStepInd = 1;

        public int FFMStepIndToSet
        {
            get { return _FFMStepIndToSet; }
            set { if (value > -1 && value < UniqueData.FFMStepBW.Length) _FFMStepIndToSet = value; }
        }
        private int _FFMStepIndToSet = 1;

        public decimal FFMStep
        {
            get { return _FFMStep; }
            set { _FFMStep = value; OnPropertyChanged("FFMStep"); }
        }
        private decimal _FFMStep = 0;

        public bool FFMStepAuto
        {
            get { return _FFMStepAuto; }
            set { _FFMStepAuto = value; OnPropertyChanged("FFMStepAuto"); }
        }
        private bool _FFMStepAuto;
        #endregion FFMStep

        public ParamWithUI FFMFFTMode
        {
            get { return _FFMFFTMode; }
            set { _FFMFFTMode = value; OnPropertyChanged("FFMFFTMode"); }
        }
        private ParamWithUI _FFMFFTMode = new ParamWithUI { UI = "CLEAR WRITE", Parameter = "OFF" };

        #region Level

        public decimal FFMStrengthLevel
        {
            get { return _FFMStrengthLevel; }
            set { _FFMStrengthLevel = value; OnPropertyChanged("FFMStrengthLevel"); }
        }
        private decimal _FFMStrengthLevel = -1;

        public decimal FFMRefLevel
        {
            get { return _FFMRefLevel; }
            set
            {
                if (value > UniqueData.RefLevelMAX) _FFMRefLevel = UniqueData.RefLevelMAX;
                else if (value < UniqueData.RefLevelMIN) _FFMRefLevel = UniqueData.RefLevelMIN;
                else _FFMRefLevel = value;
                _FFMLowestLevel = _FFMRefLevel - FFMRangeLevel; OnPropertyChanged("FFMLowestLevel");
                SetLevelFromMode();
                OnPropertyChanged("FFMRefLevel");
            }
        }
        private decimal _FFMRefLevel = 80;

        public decimal FFMRangeLevel
        {
            get { return _FFMRangeLevel; }
            set
            {
                if (value > UniqueData.RangeLevelMAX) { FFMRangeLevel = UniqueData.RangeLevelMAX; }
                else if (value < UniqueData.RangeLevelMIN) { FFMRangeLevel = UniqueData.RangeLevelMIN; }
                else _FFMRangeLevel = value;
                _FFMLowestLevel = _FFMRefLevel - FFMRangeLevel; OnPropertyChanged("FFMLowestLevel");
                SetLevelFromMode();
                OnPropertyChanged("FFMRangeLevel");
            }
        }
        private decimal _FFMRangeLevel = 110;

        public decimal FFMLowestLevel
        {
            get { return _FFMLowestLevel; }
            set { _FFMLowestLevel = value; OnPropertyChanged("FFMLowestLevel"); }
        }
        private decimal _FFMLowestLevel = -30;
        #endregion
        #endregion FFM

        #region PSCAN
        #region Freq
        /// <summary>
        /// true = CentrSpan
        /// false = StartStop
        /// </summary>
        public bool PScanFreq_CentrSpan_StartStop
        {
            get { return _PScanFreq_CentrSpan_StartStop; }
            set { _PScanFreq_CentrSpan_StartStop = value; OnPropertyChanged("PScanFreq_CentrSpan_StartStop"); }
        }
        private bool _PScanFreq_CentrSpan_StartStop;

        public decimal PScanFreqCentr
        {
            get { return _PScanFreqCentr; }
            set
            {
                if (value < MinFreqReceiver) _PScanFreqCentr = MinFreqReceiver;
                else if (value > MaxFreqReceiver) _PScanFreqCentr = MaxFreqReceiver;
                else _PScanFreqCentr = value;
                _PScanFreqStart = _PScanFreqCentr - _PScanFreqSpan / 2; OnPropertyChanged("PScanFreqStart");
                _PScanFreqStop = _PScanFreqCentr + _PScanFreqSpan / 2; OnPropertyChanged("PScanFreqStop");
                PScanFreq_CentrSpan_StartStop = true;
                OnPropertyChanged("PScanFreqCentr");
            }
        }
        private decimal _PScanFreqCentr = 950000000;

        public decimal PScanFreqSpan
        {
            get { return _PScanFreqSpan; }
            set
            {
                _PScanFreqSpan = value;
                if (PScanFreqCentr - value / 2 < MinFreqReceiver) PScanFreqCentr = (PScanFreqCentr - MinFreqReceiver) * 2;
                else if (PScanFreqCentr + value / 2 > MaxFreqReceiver) PScanFreqCentr = (MaxFreqReceiver - PScanFreqCentr) * 2;

                _PScanFreqStart = _PScanFreqCentr - _PScanFreqSpan / 2; OnPropertyChanged("PScanFreqStart");
                _PScanFreqStop = _PScanFreqCentr + _PScanFreqSpan / 2; OnPropertyChanged("PScanFreqStop");
                PScanFreq_CentrSpan_StartStop = true;
                OnPropertyChanged("PScanFreqSpan");
            }
        }
        private decimal _PScanFreqSpan = 5000000;

        public decimal PScanFreqStart
        {
            get { return _PScanFreqStart; }
            set
            {
                if (value < MinFreqReceiver) _PScanFreqStart = MinFreqReceiver;
                else if (value > MaxFreqReceiver) _PScanFreqStart = MaxFreqReceiver;
                else _PScanFreqStart = value;
                _PScanFreqCentr = (_PScanFreqStart + _PScanFreqStop) / 2; OnPropertyChanged("PScanFreqCentr");
                _PScanFreqSpan = _PScanFreqStop - _PScanFreqStart; OnPropertyChanged("PScanFreqSpan");
                PScanFreq_CentrSpan_StartStop = false;
                OnPropertyChanged("PScanFreqStart");
            }
        }
        private decimal _PScanFreqStart = 947500000;

        public decimal PScanFreqStop
        {
            get { return _PScanFreqStop; }
            set
            {
                if (value < MinFreqReceiver) _PScanFreqStop = MinFreqReceiver;
                else if (value > MaxFreqReceiver) _PScanFreqStop = MaxFreqReceiver;
                else _PScanFreqStop = value;
                _PScanFreqCentr = (_PScanFreqStart + _PScanFreqStop) / 2; OnPropertyChanged("PScanFreqCentr");
                _PScanFreqSpan = _PScanFreqStop - _PScanFreqStart; OnPropertyChanged("PScanFreqSpan");
                PScanFreq_CentrSpan_StartStop = false;
                OnPropertyChanged("PScanFreqStop");
            }
        }
        private decimal _PScanFreqStop = 952500000;
        #endregion Freq

        private bool _PScanRun = false;
        public bool PScanRun
        {
            get { return _PScanRun; }
            set
            {
                _PScanRun = value;
                if (_PScanRun)
                {
                    SetToDelegate(SetPScanRun);
                }
                else if (!_PScanRun)
                {
                    SetToDelegate(SetPScanStop);
                }
                OnPropertyChanged("PScanRun");
            }
        }

        public int PScanStepInd
        {
            get { return _PScanStepInd; }
            set
            {
                if (value < 0) _PScanStepInd = 0;
                else if (value > UniqueData.PSCANStepBW.Length - 1) _PScanStepInd = UniqueData.PSCANStepBW.Length - 1;
                else _PScanStepInd = value;
                PScanStep = UniqueData.PSCANStepBW[_PScanStepInd];
                OnPropertyChanged("PScanStepInd");
            }
        }
        private int _PScanStepInd;

        public decimal PScanStep
        {
            get { return _PScanStep; }
            set { _PScanStep = value; OnPropertyChanged("PScanStep"); }
        }
        private decimal _PScanStep = 0;

        #region Level 
        public decimal PScanRefLevel
        {
            get { return _PScanRefLevel; }
            set
            {
                if (value > UniqueData.RefLevelMAX) _PScanRefLevel = UniqueData.RefLevelMAX;
                else if (value < UniqueData.RefLevelMIN) _PScanRefLevel = UniqueData.RefLevelMIN;
                else _PScanRefLevel = value;
                _PScanLowestLevel = _PScanRefLevel - PScanRangeLevel; OnPropertyChanged("PScanLowestLevel");
                SetLevelFromMode();
                OnPropertyChanged("PScanRefLevel");
            }
        }
        private decimal _PScanRefLevel = 80;

        public decimal PScanRangeLevel
        {
            get { return _PScanRangeLevel; }
            set
            {
                if (value > UniqueData.RangeLevelMAX) { PScanRangeLevel = UniqueData.RangeLevelMAX; }
                else if (value < UniqueData.RangeLevelMIN) { PScanRangeLevel = UniqueData.RangeLevelMIN; }
                else _PScanRangeLevel = value;
                _PScanLowestLevel = _PScanRefLevel - PScanRangeLevel; OnPropertyChanged("PScanLowestLevel");
                SetLevelFromMode();
                OnPropertyChanged("PScanRangeLevel");
            }
        }
        private decimal _PScanRangeLevel = 110;

        public decimal PScanLowestLevel
        {
            get { return _PScanLowestLevel; }
            set { _PScanLowestLevel = value; OnPropertyChanged("PScanLowestLevel"); }
        }
        private decimal _PScanLowestLevel = -30;
        #endregion
        //private decimal _PScanRefLevel = 80;
        //public decimal PScanRefLevel
        //{
        //    get { return _PScanRefLevel; }
        //    set { _PScanRefLevel = value; SetLevelFromMode(); OnPropertyChanged("PScanRefLevel"); }
        //}

        //private decimal _PScanRangeLevel = 110;
        //public decimal PScanRangeLevel
        //{
        //    get { return _PScanRangeLevel; }
        //    set { _PScanRangeLevel = value; SetLevelFromMode(); OnPropertyChanged("PScanRangeLevel"); }
        //}

        #endregion PSCAN
        #endregion Global






        #region
        bool _Run;
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
        private bool _IsRuningTCP;
        public bool IsRuningTCP
        {
            get { return _IsRuningTCP; }
            set { _IsRuningTCP = value; if (_IsRuningTCP && _IsRuningUDP) { IsRuning = true; } else { IsRuning = false; } }
        }
        private bool _IsRuningUDP;
        public bool IsRuningUDP
        {
            get { return _IsRuningUDP; }
            set { _IsRuningUDP = value; if (_IsRuningTCP && _IsRuningUDP) { IsRuning = true; } else { IsRuning = false; } }
        }
        private bool _IsRuning;
        public bool IsRuning
        {
            get { return _IsRuning; }
            set { _IsRuning = value; OnPropertyChanged("IsRuning"); }
        }
        private int _Ind = 20;
        public int Ind
        {
            get { return _Ind; }
            set { _Ind = value; OnPropertyChanged("Ind"); }
        }
        private string _Temp = "";
        public string Temp
        {
            get { return _Temp; }
            set { if (_Temp != value) { _Temp = value; OnPropertyChanged("Temp"); } }
        }
        private string _Temp1 = "";
        public string Temp1
        {
            get { return _Temp1; }
            set { _Temp1 = value; OnPropertyChanged("Temp1"); }
        }
        private string _MyIP = "";
        public string MyIP
        {
            get { return _MyIP; }
            set { if (_MyIP != value) { _MyIP = value; OnPropertyChanged("_MyIP"); } }
        }
        private int _UDPPort = 23023;//025;
        public int UDPPort
        {
            get { return _UDPPort; }
            set { _UDPPort = value; OnPropertyChanged("UDPPort"); }
        }

        private bool _AnyMeas;
        public bool AnyMeas
        {
            get { return _AnyMeas; }
            set { _AnyMeas = value; OnPropertyChanged("AnyMeas"); }
        }












        //private int _SelectivityInd;
        //private string[] Selectivity = { "AUTO", "NORM", "NARR", "SHAR" };
        //private string _SelectivityStr = "";
        //private int _RfModeInd;
        //private string[] RfMode = { "NORM", "LOWN", "LOWD" };
        //private string _RfModeStr = "";
        //private decimal[] __DemodBW = { 100, 150, 300, 600, 1000, 1500, 2100, 2400, 2700, 3100, 4000, 4800, 6000, 8300, 9000, 12000, 15000, 25000, 30000, 50000, 75000, 120000, 150000, 250000, 300000, 500000, 800000, 1000000, 1250000, 1500000, 2000000, 5000000, 8000000, 10000000, 12500000, 15000000, 20000000 };


        //private string[] Demod = { "AM", "FM", "PULSE", "PM", "IQ", "ISB", "TV" };
        private string _DemodStr = "";




        private int _SQU = 0;
        private string _SQUStr = "";
        private bool _SQUState;
        private int _VUHFInd = 0;
        private string[] VUHF = { "#14(@0)", "#14(@1)", "#14(@2)" };
        private string _VUHFStr = "";
        private bool _AutoAntenna = false;
        private string _AutoAntennaStr;




        public double[] TraceFreq = new double[100];
        #region get/set
        private string _ErrorStr = "";
        public string ErrorStr
        {
            get { return _ErrorStr; }
            set { _ErrorStr = value; OnPropertyChanged("ErrorStr"); }
        }


        private int _InstrManufacrure = 0;
        public int InstrManufacrure
        {
            get { return _InstrManufacrure; }
            set { _InstrManufacrure = value; OnPropertyChanged("InstrManufacrure"); }
        }
        private string _InstrModel = "";
        public string InstrModel
        {
            get { return _InstrModel; }
            set { _InstrModel = value; OnPropertyChanged("InstrModel"); }
        }
        private string _InstrSerialNumber = "";
        public string InstrSerialNumber
        {
            get { return _InstrSerialNumber; }
            set { _InstrSerialNumber = value; OnPropertyChanged("InstrSerialNumber"); }
        }




        #region Level
        /// <summary>
        /// 0 = dBµV            1 = dBm
        /// </summary>
        public int LevelUnitInd
        {
            get { return _LevelUnitInd; }
            set { _LevelUnitInd = value; if (_LevelUnitInd >= levelUnits.Length) { _LevelUnitInd = 0; } LevelUnitStr = levelUnits[_LevelUnitInd]; OnPropertyChanged("LevelUnitInd"); }
        }
        private int _LevelUnitInd = 0;
        /// <summary>
        /// 0 = "dBµV", 1 = "dBm"
        /// </summary>
        private string[] levelUnits = { "dBµV", "dBm" };

        private string _LevelUnitStr = "dBµV";
        /// <summary>
        /// 0 = "dBµV", 1 = "dBm"
        /// </summary>
        public string LevelUnitStr
        {
            get { return _LevelUnitStr; }
            set { _LevelUnitStr = value; OnPropertyChanged("LevelUnitStr"); }
        }

        //========================


        //========================
        private decimal _DFRefLevel = 80;
        public decimal DFRefLevel
        {
            get { return _DFRefLevel; }
            set { _DFRefLevel = value; DFRefLevelStr = String.Concat(_DFRefLevel, " ", LevelUnitStr); SetLevelFromMode(); OnPropertyChanged("DFRefLevel"); }
        }
        private string _DFRefLevelStr = "80 dBµV";
        public string DFRefLevelStr
        {
            get { return _DFRefLevelStr; }
            set { _DFRefLevelStr = value; OnPropertyChanged("DFRefLevelStr"); }
        }
        private decimal _DFRangeLevel = 110;
        public decimal DFRangeLevel
        {
            get { return _DFRangeLevel; }
            set { _DFRangeLevel = value; DFRangeLevelStr = String.Concat(value, " ", LevelUnitStr); SetLevelFromMode(); OnPropertyChanged("DFRangeLevel"); }
        }
        private string _DFRangeLevelStr = "110 dBµV";
        public string DFRangeLevelStr
        {
            get { return _DFRangeLevelStr; }
            set { _DFRangeLevelStr = value; OnPropertyChanged("DFRangeLevelStr"); }
        }
        //========================

        #endregion
        #region freq
        //private decimal _MinFreqReceiver = 10;
        //public decimal MinFreqReceiver
        //{
        //    get { return _MinFreqReceiver; }
        //    set { _MinFreqReceiver = value; OnPropertyChanged("MinFreqReceiver"); }
        //}
        //private decimal _MaxFreqReceiver = 30000000000;
        //public decimal MaxFreqReceiver
        //{
        //    get { return _MaxFreqReceiver; }
        //    set { _MaxFreqReceiver = value; OnPropertyChanged("MaxFreqReceiver"); }
        //}

        private decimal _FFMDemodFreq = 100000000;
        public decimal FFMDemodFreq
        {
            get { return _FFMDemodFreq; }
            set { _FFMDemodFreq = value; FFMDemodFreqStr = help.helpFreq(value); OnPropertyChanged("FFMDemodFreq"); }
        }
        private string _FFMDemodFreqStr = "100 MHz";
        public string FFMDemodFreqStr
        {
            get { return _FFMDemodFreqStr; }
            set { _FFMDemodFreqStr = value; OnPropertyChanged("FFMDemodFreqStr"); }
        }

        //private bool _PScanFreqSSC = true;
        ///// <summary>
        ///// Centr - true, Start/Stop - false
        ///// </summary>
        //public bool PScanFreqSSC
        //{
        //    get { return _PScanFreqSSC; }
        //    set { _PScanFreqSSC = value; OnPropertyChanged("PScanFreqSSC"); }
        //}
        //private decimal _PScanFreqStart = 70000000;
        //public decimal PScanFreqStart
        //{
        //    get { return _PScanFreqStart; }
        //    set
        //    {
        //        _PScanFreqStart = value;
        //        PScanFreqSSC = false;
        //        _PScanFreqCentr = (_PScanFreqStop + _PScanFreqStart) / 2;
        //        _PScanFreqSpan = _PScanFreqStop - _PScanFreqStart;
        //        PScanFreqStartStr = help.helpFreq(value);
        //        PScanFreqCentrStr = help.helpFreq(_PScanFreqCentr);
        //        PScanFreqSpanStr = help.helpFreq(_PScanFreqSpan);
        //        OnPropertyChanged("PScanFreqStart");
        //        OnPropertyChanged("PScanFreqCentr");
        //        OnPropertyChanged("PScanFreqSpan");
        //    }
        //}
        //private string _PScanFreqStartStr = "70 MHz";
        //public string PScanFreqStartStr
        //{
        //    get { return _PScanFreqStartStr; }
        //    set { _PScanFreqStartStr = value; OnPropertyChanged("PScanFreqStartStr"); }
        //}
        //private decimal _PScanFreqStop = 140000000;
        //public decimal PScanFreqStop
        //{
        //    get { return _PScanFreqStop; }
        //    set
        //    {
        //        _PScanFreqStop = value;
        //        PScanFreqSSC = false;
        //        _PScanFreqCentr = (_PScanFreqStop + _PScanFreqStart) / 2;
        //        _PScanFreqSpan = _PScanFreqStop - _PScanFreqStart;
        //        PScanFreqStopStr = help.helpFreq(value);
        //        PScanFreqCentrStr = help.helpFreq(_PScanFreqCentr);
        //        PScanFreqSpanStr = help.helpFreq(_PScanFreqSpan);
        //        OnPropertyChanged("PScanFreqStop");
        //        OnPropertyChanged("PScanFreqCentr");
        //        OnPropertyChanged("PScanFreqSpan");
        //    }
        //}
        //private string _PScanFreqStopStr = "140 MHz";
        //public string PScanFreqStopStr
        //{
        //    get { return _PScanFreqStopStr; }
        //    set { _PScanFreqStopStr = value; OnPropertyChanged("PScanFreqStopStr"); }
        //}

        //private decimal _PScanFreqCentr = 100000000;
        //public decimal PScanFreqCentr
        //{
        //    get { return _PScanFreqCentr; }
        //    set
        //    {
        //        _PScanFreqCentr = value;
        //        PScanFreqSSC = true;
        //        _PScanFreqStart = _PScanFreqCentr - _PScanFreqSpan / 2;
        //        _PScanFreqStop = _PScanFreqCentr + _PScanFreqSpan / 2;
        //        PScanFreqCentrStr = help.helpFreq(value);
        //        PScanFreqStartStr = help.helpFreq(_PScanFreqStart);
        //        PScanFreqStopStr = help.helpFreq(_PScanFreqStop);
        //        OnPropertyChanged("PScanFreqCentr");
        //        OnPropertyChanged("PScanFreqStart");
        //        OnPropertyChanged("PScanFreqStop");
        //    }
        //}
        //private string _PScanFreqCentrStr = "100 MHz";
        //public string PScanFreqCentrStr
        //{
        //    get { return _PScanFreqCentrStr; }
        //    set { _PScanFreqCentrStr = value; OnPropertyChanged("PScanFreqCentrStr"); }
        //}
        //private decimal _PScanFreqSpan = 100000000;
        //public decimal PScanFreqSpan
        //{
        //    get { return _PScanFreqSpan; }
        //    set
        //    {
        //        _PScanFreqSpan = value;
        //        PScanFreqSSC = true;
        //        _PScanFreqStart = _PScanFreqCentr - _PScanFreqSpan / 2;
        //        _PScanFreqStop = _PScanFreqCentr + _PScanFreqSpan / 2;
        //        PScanFreqSpanStr = help.helpFreq(value);
        //        PScanFreqStartStr = help.helpFreq(_PScanFreqStart);
        //        PScanFreqStopStr = help.helpFreq(_PScanFreqStop);
        //        OnPropertyChanged("PScanFreqSpan");
        //    }
        //}
        //private string _PScanFreqSpanStr = "100 MHz";
        //public string PScanFreqSpanStr
        //{
        //    get { return _PScanFreqSpanStr; }
        //    set { _PScanFreqSpanStr = value; OnPropertyChanged("PScanFreqSpanStr"); }
        //}



        //private decimal _FreqCentr = 95000000;
        //public decimal FreqCentr
        //{
        //    get { return _FreqCentr; }
        //    set { _FreqCentr = value; FreqCentrStr = help.helpFreq(value); OnPropertyChanged("FreqCentr"); }
        //}
        //private string _FreqCentrStr = "100 MHz";
        //public string FreqCentrStr
        //{
        //    get { return _FreqCentrStr; }
        //    set { _FreqCentrStr = value; OnPropertyChanged("FreqCentrStr"); }
        //}
        //private string _FreqSpanStr = "10 MHz";
        //public string FreqSpantStr
        //{
        //    get { return _FreqSpanStr; }
        //    set { _FreqSpanStr = value; OnPropertyChanged("FreqSpanStr"); }
        //}
        //private decimal _FreqStart = 95000000;
        //public decimal FreqStart
        //{
        //    get { return _FreqStart; }
        //    set { _FreqStart = value; FreqStartStr = help.helpFreq(value); OnPropertyChanged("FreqStart"); }
        //}
        //private string _FreqStartStr = "95 MHz";
        //public string FreqStartStr
        //{
        //    get { return _FreqStartStr; }
        //    set { _FreqStartStr = value; OnPropertyChanged("FreqStartStr"); }
        //}
        //private decimal _FreqStop = 105000000;
        //public decimal FreqStop
        //{
        //    get { return _FreqStop; }
        //    set { _FreqStop = value; FreqStopStr = help.helpFreq(value); OnPropertyChanged("FreqStop"); }
        //}
        //private string _FreqStopStr = "105 MHz";
        //public string FreqStopStr
        //{
        //    get { return _FreqStopStr; }
        //    set { _FreqStopStr = value; OnPropertyChanged("FreqStopStr"); }
        //}
        #endregion
        #region BW / Span


        //public decimal[] FFMSpanBW = { 1000, 2000, 5000, 10000, 20000, 50000, 100000, 200000, 500000, 1000000, 2000000, 5000000, 10000000, 20000000, 40000000, 80000000 };


        //private decimal _FreqSpan = 10000000;
        //public decimal FreqSpan
        //{
        //    get { return _FreqSpan; }
        //    set { _FreqSpan = value; OnPropertyChanged("FreqSpan"); }
        //}



        //public decimal[] StepBW = { 0.625m, 1.25m, 2.5m, 3.125m, 6.25m, 12.5m, 25, 31.25m, 50, 62.5m, 100, 125, 200, 250, 312.5m, 500, 625, 1000, 1250, 2000, 2500, 3125, 5000, 6250, 8333, 10000, 12500, 20000, 25000, 50000, 100000, 200000, 500000, 1000000, 2000000 };




        #endregion





        private RFMode _RFMode = new RFMode() { UI = "NORMAL", Parameter = "NORM" };
        public RFMode RFMode
        {
            get { return _RFMode; }
            set { _RFMode = value; OnPropertyChanged("RFMode"); }
        }
        //public int RfModeInd
        //{
        //    get { return _RfModeInd; }
        //    set
        //    {
        //        _RfModeInd = value;
        //        if (_RfModeInd == 0) { RfModeStr = "NORMAL"; }
        //        else if (_RfModeInd == 1) { RfModeStr = "LOW NOISE"; }
        //        else if (_RfModeInd == 2) { RfModeStr = "LOW DISTORTION"; }
        //        OnPropertyChanged("RfModeInd");
        //    }
        //}
        //public string RfModeStr
        //{
        //    get { return _RfModeStr; }
        //    set { if (_RfModeStr != value) { _RfModeStr = value; OnPropertyChanged("RfModeStr"); } }
        //}
        //private string[] Detector = { "PAV", "POS", "FAST", "RMS" };
        //private int _DetectorInd = 0;
        //public int DetectorInd
        //{
        //    get { return _DetectorInd; }
        //    set
        //    {
        //        _DetectorInd = value;
        //        if (_DetectorInd < 0) { _DetectorInd = 3; }
        //        else if (_DetectorInd > 3) { _DetectorInd = 0; }
        //        if (_DetectorInd == 0) { DetectorStr = "AVG"; }
        //        else if (_DetectorInd == 1) { DetectorStr = "PEAK"; }
        //        else if (_DetectorInd == 2) { DetectorStr = "FAST"; }
        //        else if (_DetectorInd == 3) { DetectorStr = "RMS"; }
        //        OnPropertyChanged("DetectorInd");
        //    }
        //}
        //private string _DetectorStr = "";
        //public string DetectorStr
        //{
        //    get { return _DetectorStr; }
        //    set { _DetectorStr = value; SetTraceLegend(); OnPropertyChanged("DetectorStr"); }
        //}
        //public int SelectivityInd
        //{
        //    get { return _SelectivityInd; }
        //    set
        //    {
        //        _SelectivityInd = value;
        //        if (_SelectivityInd == 0) { SelectivityStr = "AUTO"; }
        //        else if (_SelectivityInd == 1) { SelectivityStr = "NORMAL"; }
        //        else if (_SelectivityInd == 2) { SelectivityStr = "NARROW"; }
        //        else if (_SelectivityInd == 3) { SelectivityStr = "SHRP"; }
        //        OnPropertyChanged("SelectivityInd");
        //    }
        //}
        //public string SelectivityStr
        //{
        //    get { return _SelectivityStr; }
        //    set { if (_SelectivityStr != value) { _SelectivityStr = value; OnPropertyChanged("SelectivityStr"); } }
        //}
        //private string[] FFTMode = { "MIN", "MAX", "SCAL", "OFF" };
        //private int _FFMFFTModeInd = 0;
        //public int FFMFFTModeInd
        //{
        //    get { return _FFMFFTModeInd; }
        //    set
        //    {
        //        _FFMFFTModeInd = value;
        //        //if (_FFMFFTModeInd == 0) { FFMFFTModeStr = "MINIMUM"; }
        //        //else if (_FFMFFTModeInd == 1) { FFMFFTModeStr = "MAXIMUM"; }
        //        //else if (_FFMFFTModeInd == 2) { FFMFFTModeStr = "AVERAGE"; }
        //        //else if (_FFMFFTModeInd == 3) { FFMFFTModeStr = "CLEAR WRITE"; }
        //        OnPropertyChanged("FFMFFTModeInd");
        //    }
        //}
        //private string _FFMFFTModeStr = "";
        //public string FFMFFTModeStr
        //{
        //    get { return _FFMFFTModeStr; }
        //    set { _FFMFFTModeStr = value; OnPropertyChanged("FFMFFTModeStr"); }
        //}

        //private int _PScanFFTModeInd = 0;
        //public int PScanFFTModeInd
        //{
        //    get { return _PScanFFTModeInd; }
        //    set
        //    {
        //        _PScanFFTModeInd = value;
        //        if (_PScanFFTModeInd == 0) { PScanFFTModeStr = "MINIMUM"; }
        //        else if (_PScanFFTModeInd == 1) { PScanFFTModeStr = "MAXIMUM"; }
        //        else if (_PScanFFTModeInd == 2) { PScanFFTModeStr = "AVERAGE"; }
        //        else if (_PScanFFTModeInd == 3) { PScanFFTModeStr = "CLEAR WRITE"; }
        //        OnPropertyChanged("PScanFFTModeInd");
        //    }
        //}
        //private string _PScanFFTModeStr = "";
        //public string PScanFFTModeStr
        //{
        //    get { return _PScanFFTModeStr; }
        //    set { _PScanFFTModeStr = value; OnPropertyChanged("PScanFFTModeStr"); }
        //}




















        public int VUHFInd
        {
            get { return _VUHFInd; }
            set
            {
                _VUHFInd = value;
                if (_VUHFInd < 1) { _VUHFInd = 3; }
                else if (_VUHFInd > 3) { _VUHFInd = 1; }
                VUHFStr = "VUHF " + VUHF[_VUHFInd].ToString();
                //if (AutoAntenna) { SQUStr = help.helpLevel(_SQU, levelUnitStr); }
                //else { VUHFStr = "OFF"; }
                OnPropertyChanged("VUHFInd");
            }
        }
        public string VUHFStr
        {
            get { return _VUHFStr; }
            set { _VUHFStr = value; OnPropertyChanged("VUHFStr"); }
        }
        public bool AutoAntenna
        {
            get { return _AutoAntenna; }
            set
            {
                _AutoAntenna = value;
                if (_AutoAntenna == true) { AutoAntennaStr = "ON"; }
                else if (_AutoAntenna == false) { AutoAntennaStr = "OFF"; }
                OnPropertyChanged("_AutoAntenna");
            }
        }
        public string AutoAntennaStr
        {
            get { return _AutoAntennaStr; }
            set { _AutoAntennaStr = value; OnPropertyChanged("AutoAntennaStr"); }
        }

        public double[] TraceLevel
        {
            get { return _TraceLevel; }
            set { _TraceLevel = value; OnPropertyChanged("TraceLevel"); }
        }
        private double[] _TraceLevel = new double[100];






        #region TraceAverage
        //private bool _TraceAverageState = false;
        //public bool TraceAverageState
        //{
        //    get { return _TraceAverageState; }
        //    set
        //    {
        //        _TraceAverageState = value;
        //        SetTraceLegend();
        //        if (_TraceAverageState == true)
        //        {
        //            _TempNumberOfSweeps = 0;
        //            NumberOfSweeps = 0;
        //            NOSEquallyAC = false;
        //            TracesToAverage.Clear();
        //        }
        //        //TracesToAverage = new List<TracePoint[]> { };
        //        //for (int i = 0; i < _AveragingCount; i++)
        //        //{ TracesToAverage.Add(new TracePoint[TracePoints]); }
        //        OnPropertyChanged("TraceAverageState");
        //    }
        //}
        //private tracepoint[] _TraceAverage;
        //public tracepoint[] TraceAverage
        //{
        //    get { return _TraceAverage; }
        //    set { _TraceAverage = value; OnPropertyChanged("TraceAverage"); }
        //}
        //private List<tracepoint[]> _TracesToAverage = new List<tracepoint[]> { };
        //public List<tracepoint[]> TracesToAverage
        //{
        //    get { return _TracesToAverage; }
        //    set { _TracesToAverage = value; }
        //}

        //private int _AveragingCount = 100;
        //public int AveragingCount
        //{
        //    get { return _AveragingCount; }
        //    set
        //    {
        //        _AveragingCount = value;
        //        _TempNumberOfSweeps = 0;
        //        _NumberOfSweeps = 0;

        //        SetTraceLegend();
        //        OnPropertyChanged("AveragingCount");
        //    }
        //}

        //private int _TempNumberOfSweeps = 0;
        //private int _NumberOfSweeps = 0;
        ///// <summary>
        ///// текущее значение
        ///// </summary>
        //public int NumberOfSweeps
        //{
        //    get { return _NumberOfSweeps; }
        //    set
        //    {
        //        _NumberOfSweeps = value;
        //        if (NumberOfSweeps == AveragingCount) { NOSEquallyAC = true; }
        //        else { NOSEquallyAC = false; }
        //        SetTraceLegend();
        //        OnPropertyChanged("NumberOfSweeps");
        //    }
        //}
        //private bool _NOSEquallyAC = false;
        //public bool NOSEquallyAC
        //{
        //    get { return _NOSEquallyAC; }
        //    set { _NOSEquallyAC = value; OnPropertyChanged("NOSEquallyAC"); }
        //}
        #endregion
        string _TraceLegend = "";
        public string TraceLegend
        {
            get { return _TraceLegend; }
            set { _TraceLegend = value; OnPropertyChanged("TraceLegend"); }
        }
        string _TraceMaxHoldLegend = "";
        public string TraceMaxHoldLegend
        {
            get { return _TraceMaxHoldLegend; }
            set { _TraceMaxHoldLegend = value; OnPropertyChanged("TraceMaxHoldLegend"); }
        }
        string _TraceAveragingLegend = "";
        public string TraceAveragingLegend
        {
            get { return _TraceAveragingLegend; }
            set { _TraceAveragingLegend = value; OnPropertyChanged("TraceAveragingLegend"); }
        }

        //private void SetTraceLegend()
        //{
        //    if (Mode.Mode == "FFM")
        //    {
        //        TraceLegend = "T(1) " + FFMFFTMode.UI + " (" + Detector.UI + ") ";
        //        if (TraceMaxHoldState) { TraceMaxHoldLegend = "T(2) MaxHold (" + Detector.UI + ") "; } else TraceMaxHoldLegend = "";
        //        if (TraceAverageState) { TraceAveragingLegend = "T(3) Averaging (" + Detector.UI + ") " + NumberOfSweeps.ToString() + "/" + AveragingCount.ToString() + " "; } else TraceAveragingLegend = "";
        //    }
        //    if (Mode.Mode == "PSCAN")
        //    {
        //        TraceLegend = "T(1) " + PScanFFTMode.UI + " (" + Detector.UI + ") ";
        //        if (TraceMaxHoldState) { TraceMaxHoldLegend = "T(2) MaxHold (" + Detector.UI + ") "; } else TraceMaxHoldLegend = "";
        //        if (TraceAverageState) { TraceAveragingLegend = "T(3) Averaging (" + Detector.UI + ") " + NumberOfSweeps.ToString() + "/" + AveragingCount.ToString() + " "; } else TraceAveragingLegend = "";
        //    }
        //}
        //public ObservableRangeCollection<Marker> _Markers = new ObservableRangeCollection<Marker>
        //{
        //    new Marker {State = false, Name = 1, NameInLegend = "M1", BitmapTexture = MarkerTexture("M1"), MarkerType = "M", OnTrace = 1 },
        //    new Marker {State = false, Name = 2, NameInLegend = "M2", BitmapTexture = MarkerTexture("M2"), MarkerType = "M", OnTrace = 1 },
        //    new Marker {State = false, Name = 3, NameInLegend = "M3", BitmapTexture = MarkerTexture("M3"), MarkerType = "M", OnTrace = 1 },
        //    new Marker {State = false, Name = 4, NameInLegend = "M4", BitmapTexture = MarkerTexture("M4"), MarkerType = "M", OnTrace = 1 },
        //    new Marker {State = false, Name = 5, NameInLegend = "M5", BitmapTexture = MarkerTexture("M5"), MarkerType = "M", OnTrace = 1 },
        //    new Marker {State = false, Name = 6, NameInLegend = "M6", BitmapTexture = MarkerTexture("M6"), MarkerType = "M", OnTrace = 1 }
        //};
        //public ObservableRangeCollection<Marker> Markers
        //{
        //    get { return _Markers; }
        //    set { _Markers = value; OnPropertyChanged("Markers"); }
        //}
        //public ObservableRangeCollection<TMarker> _TMarkers = new ObservableRangeCollection<TMarker>
        //{
        //    new TMarker() { NameInLegend = "T1", BitmapTexture = MarkerTexture("T1"), RefMar = "M1", OnTrace = 1, Funk = "" },
        //    new TMarker() { NameInLegend = "T2", BitmapTexture = MarkerTexture("T2"), RefMar = "M1", OnTrace = 1, Funk = "" }
        //};
        //public ObservableRangeCollection<TMarker> TMarkers
        //{
        //    get { return _TMarkers; }
        //    set { _TMarkers = value; OnPropertyChanged("TMarkers"); }
        //}
        //private int _SelectedMarker = -1;
        //public int SelectedMarker
        //{
        //    get { return _SelectedMarker; }
        //    set { _SelectedMarker = value; OnPropertyChanged("SelectedMarker"); }
        //}
        public ObservableCollection<Marker> Markers
        {
            get { return _Markers; }
            set { _Markers = value; OnPropertyChanged("Markers"); }
        }
        private ObservableCollection<Marker> _Markers = new ObservableCollection<Marker>
        {
            new Marker() { Index = 1, MarkerType = 0, IndexOnTrace = -1, TraceNumber = new ParamWithUI() { Parameter = "0", UI = "Trace 1"}, Freq = 99000000, LevelUnit = "dBµV" },
            new Marker() { Index = 2, MarkerType = 0, IndexOnTrace = -1, TraceNumber = new ParamWithUI() { Parameter = "0", UI = "Trace 1"}, Freq = 99000000, LevelUnit = "dBµV" },
            new Marker() { Index = 3, MarkerType = 0, IndexOnTrace = -1, TraceNumber = new ParamWithUI() { Parameter = "0", UI = "Trace 1"}, Freq = 99000000, LevelUnit = "dBµV" },
            new Marker() { Index = 4, MarkerType = 0, IndexOnTrace = -1, TraceNumber = new ParamWithUI() { Parameter = "0", UI = "Trace 1"}, Freq = 99000000, LevelUnit = "dBµV" },
            new Marker() { Index = 5, MarkerType = 0, IndexOnTrace = -1, TraceNumber = new ParamWithUI() { Parameter = "0", UI = "Trace 1"}, Freq = 99000000, LevelUnit = "dBµV" },
            new Marker() { Index = 6, MarkerType = 0, IndexOnTrace = -1, TraceNumber = new ParamWithUI() { Parameter = "0", UI = "Trace 1"}, Freq = 99000000, LevelUnit = "dBµV" },
        };
        private int _TracePoints = 0;
        public int TracePoints
        {
            get { return _TracePoints; }
            set { _TracePoints = value; OnPropertyChanged("TracePoints"); }
        }
        public bool NdBState
        {
            get { return _NdBState; }
            set { _NdBState = value; OnPropertyChanged("NdBState"); }
        }
        private bool _NdBState = false;
        public decimal NdBLevel
        {
            get { return _NdBLevel; }
            set
            {
                if (value < 0.1m) _NdBLevel = 0.1m;
                else if (value > 200) _NdBLevel = 200;
                else _NdBLevel = value;
                OnPropertyChanged("NdBLevel");
            }
        }
        private decimal _NdBLevel = 10;
        public decimal NdBResult
        {
            get { return _NdBResult; }
            set { _NdBResult = value; OnPropertyChanged("NdBResult"); }
        }
        private decimal _NdBResult = 0;
        #region ChannelPower
        public bool ChannelPowerState
        {
            get { return _ChannelPowerState; }
            set { _ChannelPowerState = value; OnPropertyChanged("ChannelPowerState"); }
        }
        private bool _ChannelPowerState = false;

        public decimal ChannelPowerBW
        {
            get { return _ChannelPowerBW; }
            set { _ChannelPowerBW = value; OnPropertyChanged("ChannelPowerBW"); }
        }
        private decimal _ChannelPowerBW = 100000;
        public double ChannelPowerResult
        {
            get { return _ChannelPowerResult; }
            set { _ChannelPowerResult = value; OnPropertyChanged("ChannelPowerResult"); }
        }
        private double _ChannelPowerResult = 0;

        #endregion
        #endregion
        #region Meas
        //private int ThisMeasCount = -1;
        //private int _MeasTraceCount = -1;
        //public int MeasTraceCount
        //{
        //    get { return _MeasTraceCount; }
        //    set { _MeasTraceCount = value; OnPropertyChanged("MeasTraceCount"); }
        //}
        int _MeasSpec = 0;
        public int MeasSpec
        {
            get { return _MeasSpec; }
            set { _MeasSpec = value; OnPropertyChanged("MeasSpec"); }
        }
        double _MeasSpecOldLow = 0;
        public double MeasSpecOldLow
        {
            get { return _MeasSpecOldLow; }
            set { _MeasSpecOldLow = value; OnPropertyChanged("MeasSpecOldLow"); }
        }
        double _MeasSpecOldHi = 0;
        public double MeasSpecOldHi
        {
            get { return _MeasSpecOldHi; }
            set { _MeasSpecOldHi = value; OnPropertyChanged("MeasSpecOldHi"); }
        }

        private int _MeasTraceCountOnFreq = 10;
        /// <summary>
        /// Количевство измерений на частоте
        /// </summary>
        public int MeasTraceCountOnFreq
        {
            get { return _MeasTraceCountOnFreq; }
            set { _MeasTraceCountOnFreq = value; OnPropertyChanged("MeasTraceCountOnFreq"); }
        }
        private DB.MeasData _MeasMonItem = new DB.MeasData() { };
        public DB.MeasData MeasMonItem
        {
            get { return _MeasMonItem; }
            set { _MeasMonItem = value; OnPropertyChanged("MeasMonItem"); }
        }
        private bool _GSMFilter = false;
        public bool GSMFilter
        {
            get { return _GSMFilter; }
            set { _GSMFilter = value; OnPropertyChanged("GSMFilter"); }
        }
        private bool _IsMeasMon = false;
        public bool IsMeasMon
        {
            get { return _IsMeasMon; }
            set
            {
                _IsMeasMon = value;
                if (_IsMeasMon) { TelnetDM += SetStartMeasMonSettings; TelnetDM += SetMeasMon; MainWindow.gps.PropertyChanged += MMSCoor_PropertyChanged; }
                else { TelnetDM -= SetMeasMon; MainWindow.gps.PropertyChanged -= MMSCoor_PropertyChanged; }
                AnyMeas = _IsMeasMon;
                OnPropertyChanged("IsMeasMon");
            }
        }
        private long _MeasMonTimeMeas = 0;
        public long MeasMonTimeMeas
        {
            get { return _MeasMonTimeMeas; }
            set { _MeasMonTimeMeas = value; OnPropertyChanged("MeasMonTimeMeas"); }
        }
        #endregion



        #region DF
        #region Draw Bearing
        private decimal _DFAzimuth = 0;
        public decimal DFAzimuth
        {
            get { return _DFAzimuth; }
            set
            {
                _DFAzimuth = value;
                if (BearingDrawMode == false) { NorthDrawAzimuth = 0; CompassDrawAzimuth = CompassHeading; BearingDrawAzimuth = _DFAzimuth; }
                else if (BearingDrawMode == true) { NorthDrawAzimuth = 0 - CompassHeading; CompassDrawAzimuth = 0; BearingDrawAzimuth = 0 - CompassHeading + _DFAzimuth; }
                OnPropertyChanged("DFAzimuth");
            }
        }
        private decimal _CompassHeading = 0;
        public decimal CompassHeading
        {
            get { return _CompassHeading; }
            set
            {
                _CompassHeading = value;
                if (BearingDrawMode == false) { NorthDrawAzimuth = 0; CompassDrawAzimuth = _CompassHeading; BearingDrawAzimuth = DFAzimuth; }
                else if (BearingDrawMode == true) { NorthDrawAzimuth = 0 - _CompassHeading; CompassDrawAzimuth = 0; BearingDrawAzimuth = 0 - _CompassHeading + DFAzimuth; }
                OnPropertyChanged("CompassHeading");
            }
        }
        private int _CompassHeadingType = 0;
        public int CompassHeadingType
        {
            get { return _CompassHeadingType; }
            set { _CompassHeadingType = value; OnPropertyChanged("CompassHeadingType"); }
        }
        private bool _BearingDrawMode;
        public bool BearingDrawMode
        {
            get { return _BearingDrawMode; }
            set
            {
                _BearingDrawMode = value;
                if (BearingDrawMode == false) { NorthDrawAzimuth = 0; CompassDrawAzimuth = CompassHeading; BearingDrawAzimuth = DFAzimuth; }
                else if (BearingDrawMode == true) { NorthDrawAzimuth = 0 - CompassHeading; CompassDrawAzimuth = 0; BearingDrawAzimuth = 0 - CompassHeading + DFAzimuth; }
                OnPropertyChanged("BearingDrawMode");
            }
        }
        public decimal NorthDrawAzimuth
        {
            get { return _NorthDrawAzimuth; }
            set { _NorthDrawAzimuth = value; OnPropertyChanged("NorthDrawAzimuth"); }
        }
        private decimal _NorthDrawAzimuth = 0;

        public decimal CompassDrawAzimuth
        {
            get { return _CompassDrawAzimuth; }
            set { _CompassDrawAzimuth = value; OnPropertyChanged("CompassDrawAzimuth"); }
        }
        private decimal _CompassDrawAzimuth = 0;

        public decimal BearingDrawAzimuth
        {
            get { return _BearingDrawAzimuth; }
            set { _BearingDrawAzimuth = value; OnPropertyChanged("BearingDrawAzimuth"); }
        }
        private decimal _BearingDrawAzimuth = 0;
        #endregion
        private int _DFSQUMode;
        public int DFSQUMode
        {
            get { return _DFSQUMode; }
            set { if (_DFSQUMode != value) { _DFSQUMode = value; OnPropertyChanged("DFSQUMode"); } }
        }

        private decimal _DFLevel = -1;
        public decimal DFLevel
        {
            get { return _DFLevel; }
            set { _DFLevel = value; OnPropertyChanged("DFLevel"); }
        }
        private decimal _DFLevelStrength = -1;
        public decimal DFLevelStrength
        {
            get { return _DFLevelStrength; }
            set { _DFLevelStrength = value; OnPropertyChanged("DFLevelStrength"); }
        }
        private int _DFSquelchValueSlider = -1;
        public bool DFSquelchValueSliderState = false;
        public int DFSquelchValueSlider
        {
            get { return _DFSquelchValueSlider; }
            set { _DFSquelchValueSlider = value; OnPropertyChanged("DFSquelchValueSlider"); }
        }
        private decimal _DFSquelchValue = -1;
        public decimal DFSquelchValue
        {
            get { return _DFSquelchValue; }
            set { _DFSquelchValue = value; OnPropertyChanged("DFSquelchValue"); }
        }
        private decimal _DFBandwidth = -1;
        public decimal DFBandwidth
        {
            get { return _DFBandwidth; }
            set { _DFBandwidth = value; OnPropertyChanged("DFBandwidth"); }
        }
        private int _DFBandwidthInd = 1;
        public int DFBandwidthInd
        {
            get { return _DFBandwidthInd; }
            set
            {
                if (value > -1)
                {
                    _DFBandwidthInd = value;
                    DFBandwidth = UniqueData.DFBW[_DFBandwidthInd];
                }
                OnPropertyChanged("DFBandwidthInd");
            }
        }
        private bool _DFBandwidthAuto = false;
        public bool DFBandwidthAuto
        {
            get { return _DFBandwidthAuto; }
            set { _DFBandwidthAuto = value; OnPropertyChanged("DFBandwidthAuto"); }
        }
        private decimal _StepWidth = -1;
        public decimal StepWidth
        {
            get { return _StepWidth; }
            set { _StepWidth = value; OnPropertyChanged("StepWidth"); }
        }
        private decimal _DFMeasureTime = -1;
        public decimal DFMeasureTime
        {
            get { return _DFMeasureTime; }
            set { _DFMeasureTime = value; OnPropertyChanged("DFMeasureTime"); }
        }
        private int _DFOption = -1;
        public int DFOption
        {
            get { return _DFOption; }
            set { _DFOption = value; OnPropertyChanged("DFOption"); }
        }

        private decimal _DFAntennaFactor = -1;
        public decimal DFAntennaFactor
        {
            get { return _DFAntennaFactor; }
            set { _DFAntennaFactor = value; OnPropertyChanged("DFAntennaFactor"); }
        }
        private decimal _DemodFreqChannel = -1;
        public decimal DemodFreqChannel
        {
            get { return _DemodFreqChannel; }
            set { _DemodFreqChannel = value; OnPropertyChanged("DemodFreqChannel"); }
        }
        private decimal _DemodFreq = -1;
        public decimal DemodFreq
        {
            get { return _DemodFreq; }
            set { _DemodFreq = value; OnPropertyChanged("DemodFreq"); }
        }

        private decimal _DFQuality = -1;
        public decimal DFQuality
        {
            get { return _DFQuality; }
            set { _DFQuality = value; OnPropertyChanged("DFQuality"); }
        }
        private int _DFQualitySQU = 0;
        public int DFQualitySQU
        {
            get { return _DFQualitySQU; }
            set { _DFQualitySQU = value; OnPropertyChanged("DFQualitySQU"); }
        }
        //decimal DFBandwidth = (decimal)BitConverter.ToUInt32(t, 48);
        //decimal StepWidth = (decimal)BitConverter.ToUInt32(t, 52);
        //decimal DFMeasureTime = ((decimal)BitConverter.ToUInt32(t, 56)) / 1000000;
        //int DFOption = (int)BitConverter.ToUInt32(t, 60);
        //decimal CompassHeading = ((decimal)BitConverter.ToUInt16(t, 64)) / 10;
        //int CompassHeadingType = (int)BitConverter.ToUInt16(t, 66);
        //decimal AntennaFactor = ((decimal)BitConverter.ToUInt32(t, 68)) / 10;
        //decimal DemodFreqChannel = (decimal)BitConverter.ToUInt32(t, 72);
        //byte[] bDemodFreq = new byte[8]; Array.Copy(t, 76, bDemodFreq, 0, 4); Array.Copy(t, 80, bDemodFreq, 4, 4);
        //                    decimal DemodFreq = (decimal)BitConverter.ToUInt64(bDemodFreq, 0);
        //UInt64 TimeStamp = BitConverter.ToUInt64(t, 84);
        //DateTime dt = new DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);
        //dt = dt.AddTicks((long)TimeStamp / 100);
        //                    //MainWindow.gps.UTCTime = dt;
        //                    //MainWindow.gps.LocalTime = dt.ToLocalTime();
        //decimal Azimuth = ((decimal)BitConverter.ToUInt16(t, traceDataFrom + traceDataLength * 2 + (traceDataLength / 2) * 2)) / 10;
        //decimal DFQuality
        #endregion
        //=======================================================================================
        
        public bool DataCycle
        {
            get { return _DataCycle; }
            set { _DataCycle = value; OnPropertyChanged("DataCycle"); }
        }
        bool _DataCycle;
        string _Time;
        public string Time
        {
            get { return _Time; }
            set { _Time = value; OnPropertyChanged("Time"); }
        }
        #endregion

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public RsReceiver_v2()
        {
            UniqueData = AllUniqueData[0];
            Trace1 = new tracepoint[1601];
            Trace2 = new tracepoint[1601];
            Trace3 = new tracepoint[1601];
            TraceTemp = new tracepoint[1601];
            for (int i = 0; i < 1601; i++)
            {
                Trace1[i] = new tracepoint()
                {
                    freq = 99000000 - 5000000 + 6250 * i,
                    level = -1000
                };
                Trace2[i] = new tracepoint()
                {
                    freq = 99000000 - 5000000 + 6250 * i,
                    level = -1000
                };
                Trace3[i] = new tracepoint()
                {
                    freq = 99000000 - 5000000 + 6250 * i,
                    level = -1000
                };
                TraceTemp[i] = new tracepoint()
                {
                    freq = 99000000 - 5000000 + 6250 * i,
                    level = -1000
                };
            }


        }
        public void SetToDelegate(DoubMod m)
        {
            if (TelnetDM != null && m != null)
            {
                bool find = false;
                foreach (Delegate d in TelnetDM.GetInvocationList())
                {
                    if (d.Method.Name == m.GetInvocationList()[0].Method.Name) find = true;
                }
                if (find == false) TelnetDM += (DoubMod)m.GetInvocationList()[0];
            }
        }
        public void Connect()
        {
            if (App.Sett.RsReceiver_Settings.IPAdress != "")
            {
                if (App.Sett.RsReceiver_Settings.TCPPort != 0)
                {
                    TelnetDM = sameWorkTelnet;
                    TelnetTr = new Thread(AllTelnetTimeWorks);
                    TelnetTr.Name = "RsReceiverTelnetThread";
                    TelnetTr.IsBackground = true;
                    TelnetTr.Start();
                    TelnetDM += SetConnect;
                    // создаем таймер
                    tmr.AutoReset = true;
                    tmr.Enabled = true;
                    tmr.Elapsed += WatchDog;
                    tmr.Start();
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
        private void WatchDog(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (Mode.Mode == "FFM" || Mode.Mode == "PSCAN")
            {
                if (IsRuning && Run == true && tc.IsOpen == true && new TimeSpan(DateTime.Now.Ticks - LastUpdateUDP) > new TimeSpan(0, 0, 0, 0, (int)(MeasTime * 1000 + 300)))
                {
                    IsRuningUDP = false;
                }
            }
            if (Mode.Mode == "DF")
            {
                if (IsRuning && Run == true && tc.IsOpen == true && new TimeSpan(DateTime.Now.Ticks - LastUpdateUDP) > new TimeSpan(0, 0, 0, 0, (int)(DFMeasureTime * 1000 + 300)))
                {
                    IsRuningUDP = false;
                }
            }
            //Temp1 = IsRuningTCP.ToString() + "  " + IsRuningUDP.ToString();
            ////Temp = DateTime.Now.ToString() + "   " + (MeasTime * 10000000 + 1000).ToString() + "   " + (DateTime.Now.Ticks - LastUpdateUDP).ToString() + "\r\n" +
            ////       TelnetTr.IsAlive.ToString() + "   " + UdpTr.IsAlive.ToString() + "\r\n" +
            ////       TelnetTr.ThreadState.ToString() + "  " + UdpTr.ThreadState.ToString() + "\r\n" +
            ////       tc.IsOpen.ToString() + "   " + uc.IsOpen.ToString();
        }
        private void AllTelnetTimeWorks()
        {
            while (DataCycle)
            {
                //Temp = "";
                long beginTiks = DateTime.Now.Ticks;
                //foreach (Delegate d in TelnetDM.GetInvocationList())
                //{
                //    Temp += ((DoubMod)d).Method.Name + "\r\n";
                //}
                TelnetDM();
                IsRuningTCP = true;
                LastUpdateTCP = DateTime.Now.Ticks;
            }
            TelnetDM -= sameWorkTelnet;
            UdpDM -= ReaderStream;
            tc.WriteLine("TRAC:UDP:DEL \"" + MyIP + "\", " + UDPPort.ToString());
            Thread.Sleep(100);
            tc.Close();
            uc.Close();
            IsRuningTCP = false;
            UdpTr.Abort();
            TelnetTr.Abort();
        }
        private void AllUdpTimeWorks()
        {
            while (DataCycle)
            {
                //Temp = "";
                LastUpdateUDP = DateTime.Now.Ticks;
                //foreach (Delegate d in UdpDM.GetInvocationList())
                //{
                //    Temp += ((DoubMod)d).Method.Name + "\r\n";
                //}
                if (UdpDM != null)
                    UdpDM();
                //Thread.Sleep(1);
                //Time = new TimeSpan(DateTime.Now.Ticks - LastUpdateUDP).ToString();
            }
            IsRuningUDP = false;
        }

        public void Disconnect()
        {
            tmr.Stop();
            _DataCycle = false;
        }
        public void sameWorkTelnet()
        {
            try
            {

                //if (ATTAuto == true)
                //{
                //    ATT = Int32.Parse(tc.Query("INP:ATT?").Replace('.', ','));
                //}
                //if (AFCState == true)
                //{
                //    DemodFreq = decimal.Parse(tc.Query("FREQ:DEM?"));
                //}
                Thread.Sleep(1);
            }
            catch { }
        }
        public void sameWorkUDP()
        {
            try
            {
                Thread.Sleep(1);
            }
            catch { }
        }
        /// <summary>
        /// Подключаемся к прибору        
        /// </summary>
        public void SetConnect()
        {
            try
            {
                tc = new TelnetConnection();

                tc.Open(App.Sett.RsReceiver_Settings.IPAdress, App.Sett.RsReceiver_Settings.TCPPort);
                if (tc.IsOpen)
                {
                    #region
                    string[] temp = tc.Query("*IDN?").Trim('"').Replace(" ", "").ToUpper().Split(',');
                    if (temp[0].ToLower() == "rohde&schwarz") InstrManufacrure = 1;
                    else if (temp[0] == "Keysight Technologies") InstrManufacrure = 2;
                    else if (temp[0] == "Anritsu") InstrManufacrure = 3;
                    InstrModel = temp[1];
                    InstrSerialNumber = temp[2];
                    if (InstrManufacrure == 1)
                        device_meas.manufacture = "Rohde&Schwarz";
                    else if (InstrManufacrure == 2)
                        device_meas.manufacture = "Keysight Technologies";
                    else if (InstrManufacrure == 3)
                        device_meas.manufacture = "Anritsu";
                    device_meas.model = InstrModel;
                    device_meas.sn = InstrSerialNumber;
                    string[] op = tc.Query("*OPT?").TrimEnd().Replace(" ", "").ToUpper().Split(',');
                    foreach (AllRCVUnqData a in AllUniqueData)
                    {
                        if (a.InstrManufacrure == InstrManufacrure)
                        {
                            if (InstrModel.ToLower().Contains(a.InstrModel.ToLower()))
                            {
                                UniqueData = a;
                                List<RCVOption> Loaded = new List<RCVOption>() { };
                                if (op.Length > 0)
                                {
                                    foreach (string s in op)
                                    {
                                        foreach (RCVOption io in a.InstrOption)
                                        {
                                            if (io.Type.ToLower() == s.ToLower())
                                                Loaded.Add(io);
                                        }
                                    }
                                }
                                UniqueData.LoadedInstrOption = Loaded;
                            }
                        }
                    }

                    MaxFreqReceiver = decimal.Parse(tc.Query("SENS:FREQ? MAX").Replace('.', ','));
                    MinFreqReceiver = decimal.Parse(tc.Query("SENS:FREQ? MIN").Replace('.', ','));
                    if (UniqueData.InstrModel == "EM100" || UniqueData.InstrModel == "PR100")
                    {
                        if (App.Sett.RsReceiver_Settings.IntegratedGPS && UniqueData.LoadedInstrOption.Exists(item => item.Type == "GP"))
                        {
                            tc.WriteLine("system:gpscompass:source gps, " + App.Sett.RsReceiver_Settings.GPS.ToLower()); //aux1");
                            tc.WriteLine("system:gpscompass:source compass, " + App.Sett.RsReceiver_Settings.COM.ToLower()); //aux1");
                            string aux1 = "SYSTem:GPSCompass:AUXiliary:ACCessory 1, ";
                            if (App.Sett.RsReceiver_Settings.Auxiliary1 == "None") { aux1 += "none"; }
                            else if (App.Sett.RsReceiver_Settings.Auxiliary1 == "Antenna") { aux1 += "ANTenna"; }
                            else if (App.Sett.RsReceiver_Settings.Auxiliary1 == "HA-240 GPS Mouse") { aux1 += "MOUSe"; }
                            else if (App.Sett.RsReceiver_Settings.Auxiliary1 == "NMEA GPS Mouse") { aux1 += "NGPS"; }
                            else if (App.Sett.RsReceiver_Settings.Auxiliary1 == "NMEA Compass") { aux1 += "NCOMpass"; }
                            else if (App.Sett.RsReceiver_Settings.Auxiliary1 == "Triggerable Antenna") { aux1 += "HE300"; }
                            tc.WriteLine(aux1);
                            tc.WriteLine("system:gpscompass:aux:configuration 1, 4800, 8, none, 1");

                            string aux2 = "SYSTem:GPSCompass:AUXiliary:ACCessory 2, ";
                            if (App.Sett.RsReceiver_Settings.Auxiliary2 == "None") { aux2 += "none"; }
                            else if (App.Sett.RsReceiver_Settings.Auxiliary2 == "HA-240 GPS Mouse") { aux2 += "MOUSe"; }
                            else if (App.Sett.RsReceiver_Settings.Auxiliary2 == "NMEA GPS Mouse") { aux2 += "NGPS"; }
                            else if (App.Sett.RsReceiver_Settings.Auxiliary2 == "NMEA Compass") { aux2 += "NCOMpass"; }
                            else if (App.Sett.RsReceiver_Settings.Auxiliary2 == "HL300") { aux2 += "HL300"; }
                            tc.WriteLine(aux2);
                            tc.WriteLine("system:gpscompass:aux:configuration 2, 4800, 8, none, 1");
                            tc.WriteLine("system:gpscompass on");

                            string DFAntennaReference = "SYSTem:ANTenna:DFREFerence ";
                            if (App.Sett.RsReceiver_Settings.DFAntennaReference == "North") { DFAntennaReference += "NORThonly"; }
                            else if (App.Sett.RsReceiver_Settings.DFAntennaReference == "GPS") { DFAntennaReference += "GPSDir"; }
                            else if (App.Sett.RsReceiver_Settings.DFAntennaReference == "Compass") { DFAntennaReference += "COMPhead"; }
                            tc.WriteLine(DFAntennaReference);
                        }
                    }
                    //tc.WriteLine("system:gpscompass:source gps, aux1;");
                    //tc.WriteLine("system:gpscompass:source compass, aux1;");
                    //tc.WriteLine("SYSTem:GPSCompass:AUXiliary:ACCessory 1, ANTenna;");
                    //tc.WriteLine("system:gpscompass:aux:configuration 1, 4800, 8, none, 1;");
                    //tc.WriteLine("system:gpscompass on;");
                    //tc.WriteLine("SYSTem:GPSCompass:STATe ON;");

                    foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
                    {
                        if (ni.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
                        {
                            //Console.WriteLine(ni.Name);
                            foreach (UnicastIPAddressInformation ipinf in ni.GetIPProperties().UnicastAddresses)
                            {
                                if (ipinf.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                                {
                                    if (ipinf.Address.ToString().Substring(0, ipinf.Address.ToString().LastIndexOf('.')) == App.Sett.RsReceiver_Settings.IPAdress.Substring(0, App.Sett.RsReceiver_Settings.IPAdress.LastIndexOf('.')))
                                    {
                                        MyIP = ipinf.Address.ToString();
                                    }
                                }
                            }
                        }
                    }
                    //MyIP = "192.168.2.4";// "172.17.75.2";//ip.ToString();
                    //System.Windows.MessageBox.Show(MyIP);
                    tc.WriteLine("TRAC:UDP:DEL \"" + MyIP + "\", " + UDPPort.ToString());
                    //tc.WriteLine("TRAC:TCP:DEL \"" + MyIP + "\", " + UDPPort.ToString());
                    //tc.WriteLine("SENS:FUNC:OFF \"VOLT:AC\", \"FREQ:OFFS\", \"FSTR\", \"AM\", \"AM:POS\", \"AM:NEG\", \"FM\", \"FM:POS\", \"FM:NEG\", \"PM\", \"BAND\"");                        
                    //Temp1 = tc.Query("SYST:GPS:DATA?");
                    tc.WriteLine("FORM:DATA PACK;:FORM:MEM PACK");//("FORM:DATA ASC;:FORM:MEM ASC");//
                                                                  //tc.WriteLine("FORM:BORD SWAP");//("FORM:DATA ASC;:FORM:MEM ASC");//
                                                                  //foreach (Delegate d in UdpDM.GetInvocationList())
                                                                  //{
                                                                  //    UdpDM -= (DoubMod)d;
                                                                  //}
                                                                  //uc = new UdpStreaming(MyIP, UDPPort, true);

                    //System.Windows.MessageBox.Show(UniqueData.InstrModel);
                    #endregion

                }
            }
            #region Exception
            catch (Exception exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "RsReceiver", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            #endregion
            Thread.Sleep(100);
            GetSettingsOnConnecting();

            Thread.Sleep(100);


            TelnetDM -= SetConnect;

            uc = new UdpStreaming();
            uc.Open(MyIP, UDPPort);

            UdpDM = sameWorkUDP;
            UdpDM += ReaderStream;
            UdpTr = new Thread(AllUdpTimeWorks);
            UdpTr.Name = "RsReceiverUdpThread";
            UdpTr.IsBackground = true;

            UdpTr.Start();
            SetStreamingMode();
            IsRuning = true;
        }
        public void GetSettingsOnConnecting()
        {
            if (tc.IsOpen)
            {
                string receive = "";
                try
                {
                    if (UniqueData.InstrModel == "EM100" || UniqueData.InstrModel == "PR100")
                    {
                        #region
                        Thread.Sleep(10);
                        string send = ":DISPlay:IFPan:LEVel:REFerence?" +
                            ";:DISPlay:IFPan:LEVel:RANGe?" +
                            ";:DISPl:PSCAN:LEV:REF?" +
                            ";:DISPlay:PSCAN:LEVel:RANGe?" +
                            ";:FREQ?" +
                            ";:FREQ:SPAN?" +
                            ";:CALC:IFP:STEP?" +
                            ";:CALC:IFP:STEP:AUTO?" +
                            ";:CALC:IFP:AVER:TYPE?" +
                            ";:CALC:PSC:AVER:TYPE?" +
                            ";:MEAS:MODE?" +
                            ";:CALC:IFP:SEL?" +
                            ";:MEAS:TIME?" +
                            ";:FREQ:PSC:STAR?" +
                            ";:FREQ:PSC:STOP?" +
                            ";:FREQ:PSC:CENT?" +
                            ";:FREQ:PSC:SPAN?" +
                            ";:PSC:STEP?" +
                            ";:BAND?" +
                            ";:DEM?" +
                            ";:DET?" +
                            ";:FREQ:AFC?" +
                            ";:GCON?" +
                            ";:GCON:MODE?" +
                            ";:OUTP:SQU:THR?" +
                            ";:OUTP:SQU?" +
                            ";:INPut:ATTenuation:STATe?";
                        tc.WriteLine(send);
                        Thread.Sleep(1500);
                        receive = tc.Read().Replace('.', ',').TrimEnd();

                        string[] data = receive.Split(';');
                        //MessageBox.Show(receive + "\r\n" + data.Length.ToString());
                        FFMRefLevel = decimal.Parse(data[0]);
                        FFMRangeLevel = decimal.Parse(data[1]);
                        PScanRefLevel = decimal.Parse(data[2]);
                        PScanRangeLevel = decimal.Parse(data[3]);
                        FFMFreqCentr = decimal.Parse(data[4]);
                        FFMFreqSpanInd = System.Array.IndexOf(UniqueData.FFMSpanBW, decimal.Parse(data[5]));
                        FFMStepInd = System.Array.IndexOf(UniqueData.FFMStepBW, decimal.Parse(data[6]));
                        if (data[7] == "1") { FFMStepAuto = true; }
                        else if (data[7] == "0") { FFMStepAuto = false; }
                        for (int i = 0; i < UniqueData.FFTModes.Count; i++)
                        {
                            if (data[8].Contains(UniqueData.FFTModes[i].Parameter)) { FFMFFTMode = UniqueData.FFTModes[i]; }///запилить проверку после изменения
                            if (data[9].Contains(UniqueData.FFTModes[i].Parameter)) { PScanFFTMode = UniqueData.FFTModes[i]; }///запилить проверку после изменения
                        }
                        if (data[10].Contains("CONT")) { MeasMode = true; }
                        else if (data[10].Contains("PER")) { MeasMode = false; }
                        for (int i = 0; i < UniqueData.SelectivityModes.Count; i++)
                        {
                            if (data[11].Contains(UniqueData.SelectivityModes[i].Parameter)) { SelectivityMode = UniqueData.SelectivityModes[i]; }// есть ли она вообще в EM100/PR100
                        }
                        if (data[12].Contains("DEF")) { MeasTime = 0.0005m; MeasTimeAuto = true; }
                        else { MeasTime = decimal.Parse(data[12]); }

                        PScanFreqStart = decimal.Parse(data[13]);
                        PScanFreqStop = decimal.Parse(data[14]);
                        PScanFreqCentr = decimal.Parse(data[15]);
                        PScanFreqSpan = decimal.Parse(data[16]);
                        PScanStepInd = System.Array.IndexOf(UniqueData.PSCANStepBW, decimal.Parse(data[17]));
                        DemodBW = Decimal.Parse(data[18]);
                        for (int i = 0; i < UniqueData.DemodBW.Length; i++)
                        {
                            if (DemodBW == UniqueData.DemodBW[i]) { DemodBWInd = i; }
                        }
                        for (int i = 0; i < UniqueData.Demod.Length; i++)
                        {
                            if (data[19].ToUpper() == UniqueData.Demod[i]) { DemodInd = i; }
                        }
                        for (int i = 0; i < UniqueData.Detectors.Count; i++)
                        {
                            if (data[20].ToUpper() == UniqueData.Detectors[i].Parameter) { DetectorInd = i; }//запилить для каждой железки
                        }
                        if (data[21].Contains("1")) { AFCState = true; }
                        else if (data[21].Contains("0")) { AFCState = false; }
                        MGC = Int32.Parse(data[22]);
                        if (data[23].Contains("AUTO")) { MGCAutoState = true; }
                        else if (data[23].Contains("FIX")) { MGCAutoState = false; }
                        SQU = Int32.Parse(data[24]);
                        if (data[25].Contains("1")) { SQUState = true; }
                        else if (data[25].Contains("0")) { SQUState = false; }
                        if (data[26].Contains("1")) { ATTFixState = true; }
                        else if (data[26].Contains("0")) { ATTFixState = false; }




                        //for (int i = 0; i < RfMode.Length; i++)
                        //{
                        //    if (data[11].Contains(RfMode[i])) { RfModeInd = i; }// есть ли она вообще в EM100/PR100
                        //}
                        //ATT = Int32.Parse(tc.Query("INP:ATT?").Replace('.', ',')); // запилить для каждой железки
                        //string t = tc.Query("INP:ATT:AUTO?");
                        //if (data[21].Contains("1")) { ATTAuto = true; }
                        //else if (data[21].Contains("0")) { ATTAuto = false; }



                        FreqStart = FFMFreqCentr - FFMFreqSpan / 2;
                        FreqStop = FFMFreqCentr + FFMFreqSpan / 2;
                        #endregion
                    }
                    if (UniqueData.InstrModel.Contains("DDF2") || UniqueData.InstrModel.Contains("EB5") || UniqueData.InstrModel == "ESMD")
                    {
                        #region
                        Thread.Sleep(10);
                        string send = ":CALCulate:PIFPan:RLEVel?;" + //++
                            ":CALCulate:PIFPan:LRANge?;" + //++
                                                           //":DISPl:PSCAN:LEV:REF?;" +
                                                           //":DISPlay:PSCAN:LEVel:RANGe?;" +
                            ":FREQ?;" +
                            ":FREQ:SPAN?;" +
                            ":CALC:IFP:STEP?;" +
                            ":CALC:IFP:STEP:AUTO?;" +
                            ":CALC:IFP:AVER:TYPE?;" +
                            ":CALC:PSC:AVER:TYPE?;" +
                            ":MEAS:MODE?;" +
                            ":CALC:IFP:SEL?;" +
                            ":MEAS:TIME?;" +
                            ":MEASure:DFINder:TIME?;" +
                            ":FREQ:PSC:STAR?;" +
                            ":FREQ:PSC:STOP?;" +
                            ":FREQ:PSC:CENT?;" +
                            ":FREQ:PSC:SPAN?;" +
                            ":PSC:STEP?;" +
                            ":BAND?;" +
                            ":DEM?;" +
                            ":DET?;" +
                            ":FREQ:AFC?;" +
                            ":GCON?;" +
                            ":GCON:MODE?;" +
                            ":OUTP:SQU:THR?;" +
                            ":OUTP:SQU?;" +
                            ":MEASure:DF:THReshold?;" +
                            ":BAND:DF:RES:AUTO?;" +
                            ":INP:ATT:AUTO?;" +
                            ":INP:ATT?;";
                        tc.WriteLine(send);
                        Thread.Sleep(1500);
                        receive = tc.Read().Replace('.', ',').TrimEnd();

                        string[] data = receive.Split(';');
                        int i = 0;
                        //MessageBox.Show(receive + "\r\n" + data.Length.ToString());
                        FFMRefLevel = decimal.Parse(data[i++]);
                        FFMRangeLevel = decimal.Parse(data[i++]);
                        //PScanRefLevel = decimal.Parse(data[i++]);
                        //PScanRangeLevel = decimal.Parse(data[i++]);
                        FFMFreqCentr = decimal.Parse(data[i++]);
                        FFMFreqSpanInd = System.Array.IndexOf(UniqueData.FFMSpanBW, decimal.Parse(data[i++]));
                        FFMStepInd = System.Array.IndexOf(UniqueData.FFMStepBW, decimal.Parse(data[i++]));
                        if (data[i] == "1") { FFMStepAuto = true; }
                        else if (data[i] == "0") { FFMStepAuto = false; }
                        i++;
                        for (int j = 0; j < UniqueData.FFTModes.Count; j++)
                        {
                            if (data[i].Contains(UniqueData.FFTModes[j].Parameter)) { FFMFFTMode = UniqueData.FFTModes[j]; }///запилить проверку после изменения
                            if (data[i].Contains(UniqueData.FFTModes[j].Parameter)) { PScanFFTMode = UniqueData.FFTModes[j]; }///запилить проверку после изменения
                        }
                        i++;
                        if (data[i].Contains("CONT")) { MeasMode = true; }
                        else if (data[i].Contains("PER")) { MeasMode = false; }
                        i++;
                        for (int j = 0; j < UniqueData.SelectivityModes.Count; j++)
                        {
                            if (data[i].Contains(UniqueData.SelectivityModes[j].Parameter)) { SelectivityMode = UniqueData.SelectivityModes[j]; }// есть ли она вообще в EM100/PR100
                        }
                        i++;
                        string ms = data[i++];
                        if (ms.Contains("DEF")) { MeasTime = 0.0005m; MeasTimeAuto = true; }
                        else { MeasTime = decimal.Parse(ms); }
                        DFMeasureTime = decimal.Parse(data[i++]);
                        PScanFreqStart = decimal.Parse(data[i++]);
                        PScanFreqStop = decimal.Parse(data[i++]);
                        PScanFreqCentr = decimal.Parse(data[i++]);
                        PScanFreqSpan = decimal.Parse(data[i++]);
                        PScanStepInd = System.Array.IndexOf(UniqueData.PSCANStepBW, decimal.Parse(data[i++]));
                        DemodBW = Decimal.Parse(data[i++]);
                        for (int j = 0; j < UniqueData.DemodBW.Length; j++)
                        {
                            if (DemodBW == UniqueData.DemodBW[j]) { DemodBWInd = j; }
                        }
                        for (int j = 0; j < UniqueData.Demod.Length; j++)
                        {
                            if (data[i].ToUpper() == UniqueData.Demod[j]) { DemodInd = j; }
                        }
                        i++;
                        for (int j = 0; j < UniqueData.Detectors.Count; j++)
                        {
                            if (data[i].ToUpper() == UniqueData.Detectors[j].Parameter) { DetectorInd = j; }//запилить для каждой железки
                        }
                        i++;
                        if (data[i].Contains("1")) { AFCState = true; }
                        else if (data[i].Contains("0")) { AFCState = false; }
                        i++;
                        MGC = Int32.Parse(data[i++]);
                        if (data[i].Contains("AUTO")) { MGCAutoState = true; }
                        else if (data[i].Contains("FIX")) { MGCAutoState = false; }
                        i++;
                        SQU = Int32.Parse(data[i++]);
                        if (data[i].Contains("1")) { SQUState = true; }
                        else if (data[i].Contains("0")) { SQUState = false; }
                        i++;
                        DFSquelchValue = decimal.Parse(data[i++]);
                        DFSquelchValueSlider = (int)DFSquelchValue;

                        if (data[i].Contains("1") || data[i].Contains("ON")) { DFBandwidthAuto = true; }
                        else if (data[i].Contains("0") || data[i].Contains("OFF")) { DFBandwidthAuto = false; }
                        i++;

                        if (data[i].Contains("1")) { ATTAutoState = true; }
                        else if (data[i].Contains("0")) { ATTAutoState = false; }
                        i++;
                        ATT = Int32.Parse(tc.Query(data[i]).Replace('.', ','));
                        i++;
                        //for (int i = 0; i < RfMode.Length; i++)
                        //{
                        //    if (data[11].Contains(RfMode[i])) { RfModeInd = i; }// есть ли она вообще в EM100/PR100
                        //}
                        //ATT = Int32.Parse(tc.Query("INP:ATT?").Replace('.', ',')); // запилить для каждой железки
                        //string t = tc.Query("INP:ATT:AUTO?");
                        //if (data[21].Contains("1")) { ATTAuto = true; }
                        //else if (data[21].Contains("0")) { ATTAuto = false; }



                        FreqStart = FFMFreqCentr - FFMFreqSpan / 2;
                        FreqStop = FFMFreqCentr + FFMFreqSpan / 2;
                        #endregion
                    }
                }
                #region Exception
                catch (Exception exp)
                {
                    MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "RsReceiver", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name + "\r\n" + receive };
                }
                #endregion
            }
            TelnetDM -= GetSettingsOnConnecting;
        }
        public void SetLevelFromMode()
        {
            if (Mode.Mode == "FFM") { RefLevel = FFMRefLevel; Range = FFMRangeLevel; LowestLevel = FFMLowestLevel; }//FFM
            else if (Mode.Mode == "DF") { RefLevel = FFMRefLevel; Range = FFMRangeLevel; /*RefLevel = DFRefLevel; Range = DFRangeLevel; */}//DF
            else if (Mode.Mode == "PSCAN") { RefLevel = PScanRefLevel; Range = PScanRangeLevel; LowestLevel = PScanLowestLevel; }//PScan
        }
        public void SetStreamingMode()
        {
            if (tc.IsOpen)
            {
                try
                {
                    tc.WriteLine("MEAS:APPL " + Mode.MeasAppl);
                    tc.WriteLine("SENSe:FREQ:MODE " + Mode.FreqMode);
                }
                #region Exception
                catch (Exception exp)
                {
                    MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "RsReceiver", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
                }
                #endregion
                #region
                try
                {
                    if (LastMode.Mode == "FFM")
                    {
                        string tagoff = "TRAC:UDP:TAG:OFF \"" + MyIP + "\", " + UDPPort.ToString() + ", IFPan";
                        if (App.Sett.RsReceiver_Settings.IntegratedGPS && UniqueData.LoadedInstrOption.Exists(item => item.Type.ToLower() == "gp"))
                        {
                            tagoff = "TRAC:UDP:TAG:OFF \"" + MyIP + "\", " + UDPPort.ToString() + ", IFPan, gpsc";
                        }
                        tc.WriteLine(tagoff);
                        tc.WriteLine("TRAC:UDP:FLAG:OFF \"" + MyIP + "\", " + UDPPort.ToString() + ", \"VOLT:AC\", \"FSTR\", \"swap\", \"opt\"");//\"FSTR\", 
                        tc.WriteLine("SENS:FUNC:OFF \"VOLT:AC\"");
                    }
                    else if (LastMode.Mode == "DF")
                    {
                        string tagoff = "TRAC:UDP:TAG:OFF \"" + MyIP + "\", " + UDPPort.ToString() + ", DFPan";
                        if (App.Sett.RsReceiver_Settings.IntegratedGPS && UniqueData.LoadedInstrOption.Exists(item => item.Type.ToLower() == "gp"))
                        {
                            tagoff = "TRAC:UDP:TAG:OFF \"" + MyIP + "\", " + UDPPort.ToString() + ", DFPan, gpsc";
                        }
                        tc.WriteLine(tagoff);
                        if (UniqueData.InstrModel == "EM100" || UniqueData.InstrModel == "PR100")
                        {
                            tc.WriteLine("TRAC:UDP:FLAG:OFF \"" + MyIP + "\", " + UDPPort.ToString() + ", \"DFLevel\", \"AZImuth\", \"DFQuality\", \"opt\", \"swap\"");
                            tc.WriteLine("SENS:FUNC:OFF \"VOLT:AC\"");
                        }
                        else if (UniqueData.InstrModel.Contains("DDF2") || UniqueData.InstrModel.Contains("EB5") || UniqueData.InstrModel == "ESMD")
                        {
                            tc.WriteLine("TRAC:UDP:FLAG:OFF \"" + MyIP + "\", " + UDPPort.ToString() + ", 'DFLevel', 'AZImuth', 'DFQuality', 'swap', 'opt'");
                            tc.WriteLine("SENS:FUNC:OFF \"VOLT:AC\"");
                        }
                        //////tc.WriteLine("TRAC:UDP:TAG:OFF \"" + MyIP + "\", " + UDPPort.ToString() + ", \"DFLevel\", \"AZImuth\", \"DFQuality\", 'swap', 'opt'");
                        //////tc.WriteLine("SENS:FUNC:OFF \"VOLT:AC\"");
                        //tc.WriteLine("TRAC:UDP:TAG:OFF \"" + MyIP + "\", " + UDPPort.ToString() + ", DFPAN");
                        //tc.WriteLine("TRAC:UDP:FLAG:OFF \"" + MyIP + "\", " + UDPPort.ToString() + ", \"DFL\"");
                        //tc.WriteLine("SENS:FUNC \"DFL\"");
                        //tc.WriteLine("TRAC:UDP:FLAG:OFF \"" + MyIP + "\", " + UDPPort.ToString() + ", \"AZIM\"");
                        //tc.WriteLine("SENS:FUNC \"AZIM\"");
                        //tc.WriteLine("TRAC:UDP:FLAG:OFF \"" + MyIP + "\", " + UDPPort.ToString() + ", \"DFFS\"");
                        //tc.WriteLine("SENS:FUNC \"DFFS\"");
                    }
                    else if (LastMode.Mode == "PSCAN")
                    {
                        string tagoff = "TRAC:UDP:TAG:OFF \"" + MyIP + "\", " + UDPPort.ToString() + ", PSC";
                        if (App.Sett.RsReceiver_Settings.IntegratedGPS && UniqueData.LoadedInstrOption.Exists(item => item.Type.ToLower() == "gp"))
                        {
                            tagoff = "TRAC:UDP:TAG:OFF \"" + MyIP + "\", " + UDPPort.ToString() + ", PSC, gpsc";
                        }
                        tc.WriteLine(tagoff);
                        if (UniqueData.InstrModel == "EM100" || UniqueData.InstrModel == "PR100")
                        { tc.WriteLine("TRAC:UDP:FLAG:OFF \"" + MyIP + "\", " + UDPPort.ToString() + ", 'freq:low:rx', 'freq:high:rx', 'volt:ac', 'swap', 'opt'"); }
                        else if (UniqueData.InstrModel.Contains("DDF2") || UniqueData.InstrModel.Contains("EB5") || UniqueData.InstrModel == "ESMD")
                        { tc.WriteLine("TRAC:UDP:FLAG:OFF \"" + MyIP + "\", " + UDPPort.ToString() + ", 'freq:rx', 'freq:high:rx', 'volt:ac', 'swap', 'opt'"); }

                        //tc.WriteLine("SENS:FUNC:OFF \"VOLT:AC\" \"FSTRength\"");
                        tc.WriteLine("SENS:FUNC:OFF \"VOLT:AC\"");
                    }
                    if (Mode.Mode == "FFM")
                    {
                        string tagoff = "TRAC:UDP:TAG:ON \"" + MyIP + "\", " + UDPPort.ToString() + ", IFPan";
                        if (App.Sett.RsReceiver_Settings.IntegratedGPS && UniqueData.LoadedInstrOption.Exists(item => item.Type.ToLower() == "gp"))
                        {
                            tagoff = "TRAC:UDP:TAG:ON \"" + MyIP + "\", " + UDPPort.ToString() + ", IFPan, gpsc";
                        }
                        tc.WriteLine(tagoff);
                        //System.Windows.MessageBox.Show(tagoff);
                        tc.WriteLine("TRAC:UDP:FLAG:ON \"" + MyIP + "\", " + UDPPort.ToString() + ", \"VOLT:AC\", \"FSTR\", \"swap\", \"opt\"");//\"FSTR\", 
                        tc.WriteLine("SENS:FUNC:ON \"VOLT:AC\"");
                    }
                    else if (Mode.Mode == "DF")
                    {
                        string tagoff = "TRAC:UDP:TAG:ON \"" + MyIP + "\", " + UDPPort.ToString() + ", DFPan";
                        if (App.Sett.RsReceiver_Settings.IntegratedGPS && UniqueData.LoadedInstrOption.Exists(item => item.Type.ToLower() == "gp"))
                        {
                            tagoff = "TRAC:UDP:TAG:ON \"" + MyIP + "\", " + UDPPort.ToString() + ", DFPan, gpsc";
                        }
                        tc.WriteLine(tagoff);
                        if (UniqueData.InstrModel == "EM100" || UniqueData.InstrModel == "PR100")
                        {
                            tc.WriteLine("TRAC:UDP:FLAG:ON \"" + MyIP + "\", " + UDPPort.ToString() + ", \"AZImuth\"");
                            tc.WriteLine("TRAC:UDP:FLAG:ON \"" + MyIP + "\", " + UDPPort.ToString() + ", \"DFQ\"");
                            tc.WriteLine("TRAC:UDP:FLAG:ON \"" + MyIP + "\", " + UDPPort.ToString() + ", \"DFL\"");
                            tc.WriteLine("TRAC:UDP:FLAG:ON \"" + MyIP + "\", " + UDPPort.ToString() + ", \"opt\"");
                            tc.WriteLine("TRAC:UDP:FLAG:ON \"" + MyIP + "\", " + UDPPort.ToString() + ", \"swap\"");
                            //tc.WriteLine("TRAC:UDP:FLAG:ON \"" + MyIP + "\", " + UDPPort.ToString() + ", \"DFFS\"");
                            tc.WriteLine("SENS:FUNC \"DFL\"");
                            tc.WriteLine("SENS:FUNC \"AZIM\"");
                            tc.WriteLine("SENS:FUNC \"DFQ\"");
                            //tc.WriteLine("SENS:FUNC \"DFFS\"");
                            //tc.WriteLine("TRAC:UDP:FLAG:ON \"" + MyIP + "\", " + UDPPort.ToString() + ", \"OPT\", \"DFLevel\", \"AZImuth\", \"DFQuality\", \"SWAP\"");
                            tc.WriteLine("SENS:FUNC:ON \"VOLT:AC\"");
                        }
                        else if (UniqueData.InstrModel.Contains("DDF2") || UniqueData.InstrModel.Contains("EB5") || UniqueData.InstrModel == "ESMD")
                        {
                            tc.WriteLine("TRAC:UDP:FLAG:ON \"" + MyIP + "\", " + UDPPort.ToString() + ", 'DFLevel', 'AZImuth', 'DFQuality', 'swap', 'opt'");
                            tc.WriteLine("SENS:FUNC:ON \"VOLT:AC\"");
                        }

                        //tc.WriteLine("SENS:FUNC:ON \"VOLT:AC\" \"FSTRength\"");
                        //tc.WriteLine("TRAC:UDP:TAG:ON \"" + MyIP + "\", " + UDPPort.ToString() + ", DFPAN");
                        //tc.WriteLine("TRAC:UDP:FLAG:ON \"" + MyIP + "\", " + UDPPort.ToString() + ", \"DFL\"");
                        //tc.WriteLine("SENS:FUNC \"DFL\"");
                        //tc.WriteLine("TRAC:UDP:FLAG:ON \"" + MyIP + "\", " + UDPPort.ToString() + ", \"AZIM\"");
                        //tc.WriteLine("SENS:FUNC \"AZIM\"");
                        //tc.WriteLine("TRAC:UDP:FLAG:ON \"" + MyIP + "\", " + UDPPort.ToString() + ", \"DFFS\"");
                        //tc.WriteLine("SENS:FUNC \"DFFS\"");
                    }
                    else if (Mode.Mode == "PSCAN")
                    {
                        string tagoff = "TRAC:UDP:TAG:ON \"" + MyIP + "\", " + UDPPort.ToString() + ", PSC";
                        if (App.Sett.RsReceiver_Settings.IntegratedGPS && UniqueData.LoadedInstrOption.Exists(item => item.Type.ToLower() == "gp"))
                        {
                            tagoff = "TRAC:UDP:TAG:ON \"" + MyIP + "\", " + UDPPort.ToString() + ", PSC, gpsc";
                        }
                        tc.WriteLine(tagoff);
                        if (UniqueData.InstrModel == "EM100" || UniqueData.InstrModel == "PR100")
                        { tc.WriteLine("TRAC:UDP:FLAG:ON \"" + MyIP + "\", " + UDPPort.ToString() + ", 'freq:low:rx', 'freq:high:rx', 'volt:ac', 'swap', 'opt'"); }
                        else if (UniqueData.InstrModel.Contains("DDF2") || UniqueData.InstrModel.Contains("EB5") || UniqueData.InstrModel == "ESMD")
                        { tc.WriteLine("TRAC:UDP:FLAG:ON \"" + MyIP + "\", " + UDPPort.ToString() + ", 'freq:rx', 'freq:high:rx', 'volt:ac', 'swap', 'opt'"); }
                        tc.WriteLine("SENS:FUNC:ON \"VOLT:AC\"");
                    }
                }
                #region Exception
                catch (Exception exp)
                {
                    MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "RsReceiver", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
                }
                #endregion
                #endregion
            }
            TelnetDM -= SetStreamingMode;
        }
        int tempind = 0;

        public bool FinishPscan = false;

        public void ReaderStream()
        {
            //Thread.Sleep(1);
            Thread.Sleep(new TimeSpan(10));
            if (uc.IsOpen)
            {
                //try
                //{
                Byte[] t = uc.ByteRead();
                if (t.Length > 0)
                {

                    UInt16 streammode = (UInt16)((t[16] << 8) | (t[17]));

                    UInt16 HeaderLength = 28;
                    UInt16 optHeaderLength = (UInt16)((t[22] << 8) | (t[23]));
                    UInt16 traceDataFrom = (UInt16)(optHeaderLength + HeaderLength);
                    UInt16 traceDataLength = (UInt16)((t[20] << 8) | (t[21]));
                    FinishPscan = false;

                    //Temp = streammode.ToString();
                    if (streammode == 501)//FFM IFPan
                    {
                        #region
                        byte[] bfreq = new byte[8]; Array.Copy(t, 28, bfreq, 0, 4); Array.Copy(t, 44, bfreq, 4, 4);
                        UInt64 freqCentr = BitConverter.ToUInt64(bfreq, 0);
                        UInt32 freqSpan = BitConverter.ToUInt32(t, 32);
                        decimal freqstart = freqCentr - (freqSpan / 2);
                        decimal freqstop = freqCentr + (freqSpan / 2);
                        decimal step = freqSpan / ((Int16)((t[20] << 8) | (t[21])) - 1);
                        for (int i = 0; i < UniqueData.FFMStepBW.Length; i++)
                        {
                            if (step == (int)UniqueData.FFMStepBW[i])
                            { step = UniqueData.FFMStepBW[i]; }
                        }
                        if (freqstart != TraceTemp[0].freq || TraceTemp.Length != traceDataLength || freqstop != TraceTemp[TraceTemp.Length - 1].freq || step != (TraceTemp[1].freq - TraceTemp[0].freq))
                        {
                            #region reset
                            TraceTemp = new tracepoint[traceDataLength];
                            if (LevelUnitInd == 0)
                            {
                                for (int i = 0; i < TraceTemp.Length; i++)
                                {
                                    tracepoint p = new tracepoint()
                                    {
                                        freq = freqstart + step * i,
                                        level = (double)((Int16)((t[i * 2 + traceDataFrom + 1] << 8) | (t[i * 2 + traceDataFrom]))) / 10
                                    };
                                    TraceTemp[i] = p;
                                }
                            }
                            else if (LevelUnitInd == 1)
                            {
                                for (int i = 0; i < traceDataLength; i++)
                                {
                                    tracepoint p = new tracepoint()
                                    {
                                        freq = freqstart + step * i,
                                        level = ((double)((Int16)((t[i * 2 + traceDataFrom + 1] << 8) | (t[i * 2 + traceDataFrom]))) / 10) - 107
                                    };
                                    TraceTemp[i] = p;
                                }
                            }
                            #endregion
                        }
                        else
                        {
                            #region without reset
                            if (LevelUnitInd == 0)
                            {

                                for (int i = 0; i < TraceTemp.Length; i++)
                                {
                                    TraceTemp[i].level = (double)((Int16)((t[i * 2 + traceDataFrom + 1] << 8) | (t[i * 2 + traceDataFrom]))) / 10;
                                }
                            }
                            else if (LevelUnitInd == 1)
                            {
                                for (int i = 0; i < TraceTemp.Length; i++)
                                {
                                    TraceTemp[i].level = ((double)((Int16)((t[i * 2 + traceDataFrom + 1] << 8) | (t[i * 2 + traceDataFrom]))) / 10) - 107;
                                }
                            }
                            #endregion
                        }
                        //TraceTemp = new tracepoint[traceDataLength];

                        //FFMStrengthLevel = Math.Round(LM.MeasChannelPower(temp, freqCentr, DemodBW) + 107, 2); //(decimal)((Int16)((t[91] << 8) | (t[90])));
                        //Trace1 = temp;
                        if (FFMFreqCentr != freqCentr || FreqCentr != freqCentr) { FFMFreqCentr = freqCentr; FreqCentr = freqCentr; }
                        if (FFMFreqSpan != freqSpan || FreqSpan != freqSpan)
                        {
                            if (System.Array.IndexOf(UniqueData.FFMSpanBW, freqSpan) > 0)
                            { FFMFreqSpanInd = System.Array.IndexOf(UniqueData.FFMSpanBW, freqSpan); FreqSpan = freqSpan; }
                        }
                        if (UniqueData.FFMStepBW[FFMStepInd] != step) { FFMStepInd = System.Array.IndexOf(UniqueData.FFMStepBW, step); }
                        SetTraceData(TraceTemp, step, true);
                        IsRuningUDP = true;
                        //Temp = System.Text.Encoding.UTF8.GetString(t);
                        #endregion
                    }
                    else if (streammode == 1201)//PSCan
                    {
                        #region                        
                        decimal freqStep = BitConverter.ToUInt32(t, 36);
                        for (int i = 0; i < UniqueData.PSCANStepBW.Length; i++)
                        {
                            if (freqStep == (int)UniqueData.PSCANStepBW[i])
                            { freqStep = UniqueData.PSCANStepBW[i]; PScanStepInd = i; }
                        }
                        byte[] bstart = new byte[8]; Array.Copy(t, 28, bstart, 0, 4); Array.Copy(t, 40, bstart, 4, 4);
                        byte[] bstop = new byte[8]; Array.Copy(t, 32, bstop, 0, 4); Array.Copy(t, 44, bstop, 4, 4);
                        UInt64 start = BitConverter.ToUInt64(bstart, 0);
                        UInt64 stop = BitConverter.ToUInt64(bstop, 0);
                        Int16 shift = (Int16)((t[traceDataLength * 2 + traceDataFrom - 1] << 8) | (t[traceDataLength * 2 + traceDataFrom - 2]));
                        int ind = traceDataLength;
                        if (shift == 2000)
                        {
                            FinishPscan = true;
                            ind--;
                        }
                        int points = (int)(((stop - start) / freqStep) + 1);

                        #region set level data on this band                       
                        #region reset trace freq
                        if (start != TraceTemp[0].freq || TraceTemp.Length != points || stop != TraceTemp[TraceTemp.Length - 1].freq || freqStep != (TraceTemp[1].freq - TraceTemp[0].freq))
                        {
                            TraceTemp = new tracepoint[points];
                            for (int i = 0; i < points; i++)
                            {
                                tracepoint p = new tracepoint()
                                {
                                    freq = start + (UInt64)freqStep * (UInt64)i,
                                    level = -1000
                                };
                                TraceTemp[i] = p;
                            }
                        }
                        #endregion 
                        int itd = traceDataFrom + traceDataLength * 2;
                        int itu = traceDataFrom + traceDataLength * 2 + traceDataLength * 4;
                        byte[] bfreq = new byte[8];
                        Array.Copy(t, itd, bfreq, 0, 4);
                        Array.Copy(t, itu, bfreq, 4, 4);
                        decimal freq = BitConverter.ToUInt64(bfreq, 0);
                        int freqindex = 0;
                        for (int j = 0; j < TraceTemp.Length; j++)
                        {
                            if (TraceTemp[j].freq == freq) freqindex = j;
                        }
                        for (int i = 0; i < ind; i++)
                        {
                            if (freqindex < TraceTemp.Length)
                            {
                                if (LevelUnitInd == 0)
                                {
                                    TraceTemp[freqindex].level = (double)((Int16)((t[i * 2 + traceDataFrom + 1] << 8) | (t[i * 2 + traceDataFrom]))) / 10;
                                }
                                else if (LevelUnitInd == 1)
                                {
                                    TraceTemp[freqindex].level = (double)((Int16)((t[i * 2 + traceDataFrom + 1] << 8) | (t[i * 2 + traceDataFrom]))) / 10 - 107;
                                }
                                freqindex++;
                            }
                        }
                        #endregion
                        if (PScanFreq_CentrSpan_StartStop == false)
                        {
                            if (start != PScanFreqStart || FreqStart != start) { PScanFreqStart = start; FreqStart = start; }
                            if (stop != PScanFreqStop || FreqStop != stop) { PScanFreqStop = stop; ; FreqStop = stop; }
                        }
                        else if (PScanFreq_CentrSpan_StartStop == true)
                        {
                            if ((stop + start) / 2 != PScanFreqCentr || (stop + start) / 2 != FreqCentr) { PScanFreqCentr = (stop + start) / 2; FreqCentr = (stop + start) / 2; }
                            if (stop - start != PScanFreqSpan || stop - start != FreqSpan) { PScanFreqSpan = stop - start; FreqSpan = stop - start; }
                        }

                        SetTraceData(TraceTemp, freqStep, FinishPscan);// true); // temp[1].freq - temp[0].freq);
                        IsRuningUDP = true;
                        #endregion
                    }
                    else if (streammode == 1401)//DFPan
                    {
                        #region
                        byte[] bfreq = new byte[8]; Array.Copy(t, 28, bfreq, 0, 4); Array.Copy(t, 32, bfreq, 4, 4);
                        decimal freqCentr = (decimal)BitConverter.ToUInt64(bfreq, 0);
                        decimal freqSpan = (decimal)BitConverter.ToUInt32(t, 36);

                        if (freqCentr > 0 && freqSpan > 0)
                        {
                            decimal freqstart = freqCentr - freqSpan / 2;
                            decimal freqstop = freqCentr + freqSpan / 2;
                            decimal step = freqSpan / ((Int16)((t[20] << 8) | (t[21])) - 1);
                            for (int i = 0; i < UniqueData.FFMStepBW.Length; i++)
                            {
                                if (step == (int)UniqueData.FFMStepBW[i])
                                { step = UniqueData.FFMStepBW[i]; }
                            }

                            if (InstrModel == "EM100" || InstrModel == "PR100")
                            {
                                DFSQUMode = (int)BitConverter.ToUInt32(t, 40);//++
                                DFSquelchValue = (decimal)BitConverter.ToInt32(t, 44);//++
                                if (!DFSquelchValueSliderState) { DFSquelchValueSlider = (int)DFSquelchValue; }
                                DFBandwidth = (decimal)BitConverter.ToUInt32(t, 48);//++
                                DFBandwidthInd = System.Array.IndexOf(UniqueData.DFBW, DFBandwidth);
                                StepWidth = (decimal)BitConverter.ToUInt32(t, 52);
                                DFMeasureTime = ((decimal)BitConverter.ToUInt32(t, 56)) / 1000;
                                DFOption = (int)BitConverter.ToUInt32(t, 60);
                                CompassHeading = ((decimal)BitConverter.ToUInt16(t, 64)) / 10;
                                CompassHeadingType = (int)BitConverter.ToUInt16(t, 66);
                                DFAntennaFactor = ((decimal)BitConverter.ToUInt32(t, 68)) / 10;
                                DemodFreqChannel = (decimal)BitConverter.ToUInt32(t, 72);
                                byte[] bDemodFreq = new byte[8]; Array.Copy(t, 76, bDemodFreq, 0, 4); Array.Copy(t, 80, bDemodFreq, 4, 4);
                                DemodFreq = (decimal)BitConverter.ToUInt64(bDemodFreq, 0);
                                UInt64 TimeStamp = BitConverter.ToUInt64(t, 84);
                                DateTime dt = new DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);
                                dt = dt.AddTicks((long)TimeStamp / 100);

                                DFLevel = ((decimal)BitConverter.ToInt16(t, Ind)) / 10;
                                //MainWindow.gps.UTCTime = dt;
                                //MainWindow.gps.LocalTime = dt.ToLocalTime();


                                ////decimal dfq = ((decimal)BitConverter.ToInt16(t, 136)) / 10;
                                ////if (dfq < 0 || dfq > 100) DFQuality = 0;

                                ////else if (dfq >= 0 && dfq <= 100) DFQuality = dfq;
                                ////if (dfq > DFQualitySQU)
                                ////{
                                ////    decimal az = ((decimal)BitConverter.ToInt16(t, 134)) / 10;
                                ////    if (az >= 0 && az <= 360) { DFAzimuth = az; }
                                ////}
                                //decimal az = ((decimal)BitConverter.ToInt16(t, 134)) / 10;
                                //if (az >= 0 && az <= 360) { DFAzimuth = az; }
                                DFQuality = ((decimal)((Int16)((t[1 + 136] << 8) | (t[0 + 136])))) / 10;// ((decimal)BitConverter.ToInt16(t, 136)) / 10;
                                DFAzimuth = ((decimal)BitConverter.ToInt16(t, 134)) / 10;
                                DFLevelStrength = DFLevel + DFAntennaFactor;
                            }
                            else if (UniqueData.InstrModel.Contains("DDF2") || UniqueData.InstrModel.Contains("EB5") || UniqueData.InstrModel == "ESMD")
                            {
                                #region
                                DFSQUMode = (int)BitConverter.ToUInt32(t, 40);//++
                                DFSquelchValue = (decimal)BitConverter.ToInt32(t, 44);//++
                                if (!DFSquelchValueSliderState) { DFSquelchValueSlider = (int)DFSquelchValue; }
                                DFBandwidth = (decimal)BitConverter.ToUInt32(t, 48);//++
                                DFBandwidthInd = System.Array.IndexOf(UniqueData.DFBW, DFBandwidth);
                                StepWidth = (decimal)BitConverter.ToUInt32(t, 52);
                                DFMeasureTime = ((decimal)BitConverter.ToUInt32(t, 56)) / 1000;
                                DFOption = (int)BitConverter.ToUInt32(t, 60);
                                CompassHeading = ((decimal)BitConverter.ToUInt16(t, 64)) / 10;
                                CompassHeadingType = (int)BitConverter.ToUInt16(t, 66);
                                DFAntennaFactor = ((decimal)BitConverter.ToUInt32(t, 68)) / 10;
                                DemodFreqChannel = (decimal)BitConverter.ToUInt32(t, 72);
                                byte[] bDemodFreq = new byte[8]; Array.Copy(t, 76, bDemodFreq, 0, 4); Array.Copy(t, 80, bDemodFreq, 4, 4);
                                DemodFreq = (decimal)BitConverter.ToUInt64(bDemodFreq, 0);
                                UInt64 TimeStamp = BitConverter.ToUInt64(t, 84);
                                DateTime dt = new DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);
                                dt = dt.AddTicks((long)TimeStamp / 100);
                                DFLevel = ((decimal)BitConverter.ToInt16(t, 132)) / 10;
                                //MainWindow.gps.UTCTime = dt;
                                //MainWindow.gps.LocalTime = dt.ToLocalTime();

                                //Azimuth = ((decimal)BitConverter.ToUInt16(t, traceDataFrom + traceDataLength * 2 + (traceDataLength / 2) * 2)) / 10;
                                //decimal dfq = ((decimal)BitConverter.ToUInt16(t, traceDataFrom + (traceDataLength / 2) * 10 + 4)) / 10;
                                decimal dfq = ((decimal)BitConverter.ToInt16(t, 136)) / 10;
                                if (dfq < 0 || dfq > 100) DFQuality = 0;
                                //if (dfq > 100) DFQuality = 0;
                                else if (dfq >= 0 && dfq <= 100) DFQuality = dfq;
                                if (dfq > DFQualitySQU)
                                {
                                    decimal az = ((decimal)BitConverter.ToInt16(t, 134)) / 10;
                                    if (az >= 0 && az <= 360) { DFAzimuth = az; }
                                }

                                DFLevelStrength = DFLevel + DFAntennaFactor;
                                #endregion
                            }
                            if (freqstart != TraceTemp[0].freq || TraceTemp.Length != traceDataLength || freqstop != TraceTemp[TraceTemp.Length - 1].freq || step != (TraceTemp[1].freq - TraceTemp[0].freq))
                            {
                                #region reset
                                TraceTemp = new tracepoint[traceDataLength];
                                if (LevelUnitInd == 0)
                                {
                                    for (int i = 0; i < TraceTemp.Length; i++)
                                    {
                                        tracepoint p = new tracepoint()
                                        {
                                            freq = freqstart + step * i,
                                            level = (double)((Int16)((t[i * 2 + traceDataFrom + 1] << 8) | (t[i * 2 + traceDataFrom]))) / 10
                                        };
                                        TraceTemp[i] = p;
                                    }
                                }
                                else if (LevelUnitInd == 1)
                                {
                                    for (int i = 0; i < traceDataLength; i++)
                                    {
                                        tracepoint p = new tracepoint()
                                        {
                                            freq = freqstart + step * i,
                                            level = ((double)((Int16)((t[i * 2 + traceDataFrom + 1] << 8) | (t[i * 2 + traceDataFrom]))) / 10) - 107
                                        };
                                        TraceTemp[i] = p;
                                    }
                                }
                                #endregion
                            }
                            else
                            {
                                #region without reset
                                if (LevelUnitInd == 0)
                                {

                                    for (int i = 0; i < TraceTemp.Length; i++)
                                    {
                                        TraceTemp[i].level = (double)((Int16)((t[i * 2 + traceDataFrom + 1] << 8) | (t[i * 2 + traceDataFrom]))) / 10;
                                    }
                                }
                                else if (LevelUnitInd == 1)
                                {
                                    for (int i = 0; i < TraceTemp.Length; i++)
                                    {
                                        TraceTemp[i].level = ((double)((Int16)((t[i * 2 + traceDataFrom + 1] << 8) | (t[i * 2 + traceDataFrom]))) / 10) - 107;
                                    }
                                }
                                #endregion
                            }

                            //Trace1 = temp;
                            if (FFMFreqCentr != freqCentr || FreqCentr != freqCentr) { FFMFreqCentr = freqCentr; FreqCentr = FFMFreqCentr; FreqStart = FFMFreqCentr - freqSpan / 2; FreqStop = FFMFreqCentr + freqSpan / 2; }
                            if (FFMFreqSpan != freqSpan) { FFMFreqSpanInd = System.Array.IndexOf(UniqueData.FFMSpanBW, freqSpan); FreqSpan = freqSpan; FreqStart = FFMFreqCentr - freqSpan / 2; FreqStop = FFMFreqCentr + freqSpan / 2; }
                            if (UniqueData.FFMStepBW[FFMStepInd] != step) { FFMStepInd = System.Array.IndexOf(UniqueData.FFMStepBW, step); }
                            IsRuningUDP = true;
                            SetTraceData(TraceTemp, step, true);
                            //Temp = System.Text.Encoding.UTF8.GetString(t);
                        }
                        #endregion
                        #region
                        //Temp = ""; Temp1 = Ind.ToString();
                        //Temp += t.Length.ToString() + " " + traceDataFrom.ToString() + " " + (traceDataFrom + traceDataLength * 2 + (traceDataLength / 2) * 2).ToString() + " " + (traceDataFrom + (traceDataLength / 2) * 10 + 4).ToString() + "\r\n" +
                        //    t[0 + Ind].ToString() + t[1 + Ind].ToString() + t[2 + Ind].ToString() + t[3 + Ind].ToString() + t[4 + Ind].ToString() + "\r\n" +
                        //    "DF_Squelch_Mode " + DFSquelchMode.ToString() + "\r\n" +
                        //    "DF_Squelch_Value " + DFSquelchValue.ToString() + "\r\n" +
                        //    "DFBandwidth " + DFBandwidth.ToString() + "\r\n" +
                        //    "StepWidth " + StepWidth.ToString() + "\r\n" +
                        //    "DFMeasureTime " + DFMeasureTime.ToString() + "\r\n" +
                        //    "DFOption " + DFOption.ToString() + "\r\n" +
                        //    "CompassHeading " + CompassHeading.ToString() + "\r\n" +
                        //    "CompassHeadingType " + CompassHeadingType.ToString() + "\r\n" +
                        //    "AntennaFactor " + AntennaFactor.ToString() + "\r\n" +
                        //    "DemodFreqChannel " + DemodFreqChannel.ToString() + "\r\n" +
                        //    "DemodFreq " + DemodFreq.ToString() + "\r\n" +
                        //    "dt " + dt.ToString() + "\r\n" +





                        //    "Azimuth " + Azimuth.ToString() + "\r\n" +
                        //    "DFQuality " + DFQuality.ToString() + "\r\n" +


                        //            //"uint64 " + ((UInt64)((UInt64)(t[28]) + (UInt64)(t[29] << 8) + (UInt64)(t[30] << 16) + (UInt64)(t[31] << 24) + (UInt64)(t[40] << 32) + (UInt64)(t[41] << 40) + (UInt64)(t[42] << 48) + (UInt64)(t[43] << 56))).ToString() + "\r\n" +
                        //            "uint8 " + ((t[0 + Ind])).ToString() + "\r\n" +
                        //            "int16 " + ((double)((Int16)((t[0 + Ind] << 8) | (t[1 + Ind])))).ToString() + "\r\n" +
                        //            "int16 " + ((double)((Int16)((t[1 + Ind] << 8) | (t[0 + Ind])))).ToString() + "\r\n" +
                        //            "uint16 " + ((double)((UInt16)((t[0 + Ind] << 8) | (t[1 + Ind])))).ToString() + "\r\n" +
                        //            "uint16 " + ((double)((UInt16)((t[1 + Ind] << 8) | (t[0 + Ind])))).ToString() + "\r\n" +
                        //            "uint32 " + ((UInt32)((UInt32)(t[3 + Ind]) + (UInt32)(t[2 + Ind] << 8) + (UInt32)(t[2 + Ind] << 16) + (UInt32)(t[1 + Ind] << 24))).ToString() + "\r\n" +
                        //            "uint32 " + ((UInt32)((UInt32)(t[0 + Ind]) + (UInt32)(t[1 + Ind] << 8) + (UInt32)(t[2 + Ind] << 16) + (UInt32)(t[3 + Ind] << 24))).ToString() + "  " + "\r\n" +
                        //            "UInt32 " + BitConverter.ToUInt32(t, Ind).ToString() + "\r\n" +
                        //            "Int16 " + ((double)BitConverter.ToInt16(t, Ind)).ToString() + "\r\n" +
                        //            "Float " + BitConverter.ToSingle(t, Ind).ToString() + "\r\n" +
                        //            "UInt64 " + BitConverter.ToUInt64(t, Ind).ToString() + "\r\n" +
                        //            "uint64 " + ((UInt64)((UInt64)(t[28]) + (UInt64)(t[29] << 8) + (UInt64)(t[30] << 16) + (UInt64)(t[31] << 24) + (UInt64)(t[40] << 32) + (UInt64)(t[41] << 40) + (UInt64)(t[42] << 48) + (UInt64)(t[43] << 56))).ToString();

                        #endregion
                    }
                    else if (streammode == 1801)//GPS Data
                    {
                        #region
                        MainWindow.gps.GPSIsValid = Convert.ToBoolean(BitConverter.ToUInt16(t, 40));
                        if (MainWindow.gps.GPSIsValid)
                        {
                            try
                            {
                                MainWindow.gps._LastLatitude = MainWindow.gps.LatitudeDecimal;
                                MainWindow.gps._LastLongitude = MainWindow.gps.LongitudeDecimal;
                                MainWindow.gps.LatitudeDecimal = (decimal)Math.Round(BitConverter.ToUInt16(t, 46) + BitConverter.ToSingle(t, 48) / 60, 6);
                                MainWindow.gps.LongitudeDecimal = (decimal)Math.Round(BitConverter.ToUInt16(t, 54) + BitConverter.ToSingle(t, 56) / 60, 6); // + " " + tok[5];
                                MainWindow.gps.LatitudeStr = BitConverter.ToUInt16(t, 46).ToString() + "° " + (int)BitConverter.ToSingle(t, 48) + "' " +
                                    Math.Round((BitConverter.ToSingle(t, 48) - (int)BitConverter.ToSingle(t, 48)) * 60, 1) + "\" " + Convert.ToChar(BitConverter.ToUInt16(t, 44));

                                MainWindow.gps.LongitudeStr = BitConverter.ToUInt16(t, 54).ToString() + "° " + (int)BitConverter.ToSingle(t, 56) + "' " +
                                    Math.Round((BitConverter.ToSingle(t, 56) - (int)BitConverter.ToSingle(t, 56)) * 60, 1) + "\" " + Convert.ToChar(BitConverter.ToUInt16(t, 52));
                                MainWindow.gps.NumbSat = BitConverter.ToUInt16(t, 42);
                                MainWindow.gps.Sats = MainWindow.gps.NumbSat.ToString();
                                double dist = 0, ang = 0;
                                MainWindow.help.calcDistance((double)MainWindow.gps._LastLatitude, (double)MainWindow.gps._LastLongitude, (double)MainWindow.gps.LatitudeDecimal, (double)MainWindow.gps.LongitudeDecimal, out dist, out ang);
                                MainWindow.gps.AngleCourse = ang;
                                MainWindow.gps.Horizontaldilution = BitConverter.ToSingle(t, 60);
                                UInt64 TimeStamp = BitConverter.ToUInt64(t, 28);
                                DateTime dt = new DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);
                                dt = dt.AddTicks((long)TimeStamp / 100);
                                MainWindow.gps.UTCTime = dt;
                                MainWindow.gps.LocalTime = dt.ToLocalTime();
                            }
                            catch { }
                        }
                        //Temp = System.Text.Encoding.UTF8.GetString(t);
                        #endregion
                    }

                    //if (Markers[0].Level != null && ChannelPower)
                    //{
                    //    //if (Markers[0].OnTrace == 1) { ChannelPowerResult = Math.Round(LM.MeasChannelPower(Trace, (FreqStart + FreqStop) / 2, ChannelPowerBW, 0), 2); }
                    //    //else if (Markers[0].OnTrace == 2) { ChannelPowerResult = Math.Round(LM.MeasChannelPower(TraceMaxHold, (FreqStart + FreqStop) / 2, ChannelPowerBW, 0), 2); }
                    //    //else if (Markers[0].OnTrace == 3) { ChannelPowerResult = Math.Round(LM.MeasChannelPower(TraceAverage, (FreqStart + FreqStop) / 2, ChannelPowerBW, 0), 2); }
                    //}
                    //if (NdBStateEst == true)
                    //{
                    //    //GetNDB((int)Markers[0].Freq, (decimal)NdBLevel);
                    //}
                    //if (IsMeasMon && ((FinishPscan && Mode.Mode == "PSCAN") || Mode.Mode == "FFM")) { ThisMeasCount++; }
                    if (IsMeasMon && ((FinishPscan && Mode.Mode == "PSCAN") || Mode.Mode == "FFM") && MeasMonItem != null && MeasMonItem.AllTraceCountToMeas > MeasMonItem.AllTraceCount) // MeasTraceCount > -1)
                    {
                        #region
                        bool t1 = MeasMonItem.FreqDN == FreqCentr;
                        bool t2 = MeasMonItem.SpecData.FreqSpan == FreqSpan;
                        bool t3 = ((FinishPscan && Mode.Mode == "PSCAN") || Mode.Mode == "FFM");
                        //decimal ndblevel = App.Sett.MeasMons_Settings.MeasMons.Where(x => x.Techonology.ToString() == MeasMonItem.Techonology).First().NdBLevel;

                        if (t1 && t2 && t3)
                        {
                            #region
                            if (MeasMonItem.SpecData.Trace[0].freq == Trace1[0].freq &&
                                MeasMonItem.SpecData.Trace[MeasMonItem.SpecData.Trace.Length - 1].freq == Trace1[Trace1.Length - 1].freq &&
                                MeasSpecOldLow != Trace1[0].level &&
                                MeasSpecOldHi != Trace1[Trace1.Length - 1].level)
                            { MeasSpec++; MeasSpecOldLow = Trace1[0].level; MeasSpecOldHi = Trace1[Trace1.Length - 1].level; }
                            int dfc = 0;
                            int ufc = 0;
                            int cf = 0;
                            double dfl = 0;
                            double ufl = 0;
                            double cfl = 0;
                            if (MeasMonItem.Techonology == "GSM")
                            {
                                dfc = LM.FindMarkerIndOnTrace(Trace1, (decimal)(MeasMonItem.FreqDN - (decimal)(MeasMonItem.BWData.BWMeasMax / 2)));
                                ufc = LM.FindMarkerIndOnTrace(Trace1, (decimal)(MeasMonItem.FreqDN + (decimal)(MeasMonItem.BWData.BWMeasMax / 2)));
                                cf = LM.FindMarkerIndOnTrace(Trace1, (decimal)(MeasMonItem.FreqDN));
                                dfl = LM.AverageLevelNearPoint(Trace1, dfc, 10);
                                ufl = LM.AverageLevelNearPoint(Trace1, ufc, 10);
                                cfl = LM.AverageLevelNearPoint(Trace1, cf, 10);
                            }
                            if (MeasMonItem.Techonology != "GSM" || (cfl > dfl + MeasMonItem.BWData.NdBLevel + 5 && cfl > ufl + MeasMonItem.BWData.NdBLevel + 5))
                            {
                                bool changeTrace = false;
                                if (MeasMonItem.SpecData.Trace == null || MeasMonItem.SpecData.Trace[0] == null || MeasMonItem.SpecData.Trace.Length != TracePoints || MeasMonItem.SpecData.Trace[0].freq != Trace1[0].freq || MeasMonItem.SpecData.Trace[MeasMonItem.SpecData.Trace.Length - 1].freq != Trace1[Trace1.Length - 1].freq)
                                {
                                    MeasMonItem.SpecData.Trace = new tracepoint[TracePoints];
                                    for (int i = 0; i < TracePoints; i++)
                                    {
                                        tracepoint p = new tracepoint() { freq = Trace1[i].freq, level = Trace1[i].level };
                                        MeasMonItem.SpecData.Trace[i] = p;
                                    }
                                    MeasMonItem.SpecData.MeasStart = MainWindow.gps.LocalTime;
                                    MeasMonItem.SpecData.MeasStop = MainWindow.gps.LocalTime;
                                    MeasMonItem.SpecData.LastMeasLatitude = (double)MainWindow.gps.LatitudeDecimal;
                                    MeasMonItem.SpecData.LastMeasLongitude = (double)MainWindow.gps.LongitudeDecimal;
                                    MeasMonItem.SpecData.LastMeasAltitude = (double)MainWindow.gps.Altitude;
                                    MeasMonItem.SpecData.TraceCount++;
                                    //if (Mode.Mode == "PSCAN") { MeasMonItem.TraceStep = UniqueData.PSCANStepBW[PScanStepInd]; }
                                    //if (Mode.Mode == "FFM") { MeasMonItem.TraceStep = UniqueData.FFMStepBW[FFMStepInd]; }
                                    changeTrace = true;
                                }
                                else /*if (MeasMonItem.Trace != null && MeasMonItem.Trace[0] != null && MeasMonItem.Trace.Length == TracePoints && MeasMonItem.Trace[0].Freq == Trace[0].Freq && MeasMonItem.Trace[MeasMonItem.Trace.Length - 1].Freq == Trace[Trace.Length - 1].Freq)*/
                                {
                                    // если чето в накоплении этот трейс поменяет
                                    for (int i = 0; i < Trace1.Length; i++)
                                    {
                                        if (Trace1[i].level >= MeasMonItem.SpecData.Trace[i].level)
                                        { MeasMonItem.SpecData.Trace[i].level = Trace1[i].level; changeTrace = true; }
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
                                //ищем пик уровня измерения NdB
                                if (changeTrace)//&& MeasMonItem.AllTraceCountToMeas - 1 == MeasMonItem.AllTraceCount)
                                {
                                    int ind = -1;
                                    double tl = double.MinValue;
                                    decimal minf = (MeasMonItem.FreqDN - (MeasMonItem.BWData.BWMarPeak / 2));
                                    decimal maxf = (MeasMonItem.FreqDN + (MeasMonItem.BWData.BWMarPeak / 2));
                                    for (int i = 0; i < MeasMonItem.SpecData.Trace.Length; i++)
                                    {
                                        if (MeasMonItem.SpecData.Trace[i].freq > minf && MeasMonItem.SpecData.Trace[i].freq < maxf && MeasMonItem.SpecData.Trace[i].level > tl)
                                        { tl = MeasMonItem.SpecData.Trace[i].level; ind = i; }
                                        //if (MeasMonItem.Techonology == "UHF" && MeasMonItem.Trace[i].Freq > MeasMonItem.FreqDN - MeasMonItem.TraceStep / 2 && MeasMonItem.Trace[i].Freq < MeasMonItem.FreqDN + MeasMonItem.TraceStep / 2)
                                        //{
                                        //    MeasMonItem.Power = MeasMonItem.Trace[i].Level;
                                        //    for (int y = 0; y < MainWindow.IdfData.UHFBTS.Count; y++)
                                        //    {
                                        //        if (MainWindow.IdfData.UHFBTS[y].PlanFreq_ID == MeasMonItem.PlanFreq_ID && MainWindow.IdfData.UHFBTS[y].Plan_ID == MeasMonItem.PLAN_ID && MainWindow.IdfData.UHFBTS[y].FreqDn == MeasMonItem.FreqDN)
                                        //        { MainWindow.IdfData.UHFBTS[y].Power = MeasMonItem.Power; }
                                        //    }
                                        //}
                                    }
                                    MeasMonItem.BWData.NdBResult[0] = ind;
                                    int[] mar = new int[3];
                                    if (MeasMonItem.Techonology == "GSM")
                                    {
                                        mar = LM.GetMeasNDB(MeasMonItem.SpecData.Trace, MeasMonItem.BWData.NdBResult[0], MeasMonItem.BWData.NdBLevel, MeasMonItem.SpecData.FreqCentr, MeasMonItem.BWData.BWMeasMax, MeasMonItem.BWData.BWMeasMin);
                                    }
                                    else
                                    {
                                        mar = LM.GetMeasNDB(MeasMonItem.SpecData.Trace, MeasMonItem.BWData.NdBResult[0], MeasMonItem.BWData.NdBLevel, MeasMonItem.SpecData.FreqCentr, MeasMonItem.BWData.BWMeasMax, MeasMonItem.BWData.BWMeasMin);
                                    }
                                    if (mar != null && mar[1] > -1 && mar[2] > -1)
                                    {
                                        MeasMonItem.BWData.BWMeasured = (decimal)(MeasMonItem.SpecData.Trace[mar[2]].freq - MeasMonItem.SpecData.Trace[mar[1]].freq);
                                        MeasMonItem.BWData.NdBResult = mar;
                                        //MeasMonItem.MarkerT2Ind = mar[1];
                                        MeasMonItem.DeltaFreqMeasured = Math.Round(((Math.Abs(((MeasMonItem.SpecData.Trace[mar[1]].freq + MeasMonItem.SpecData.Trace[mar[2]].freq) / 2) - (MeasMonItem.SpecData.FreqCentr))) / (MeasMonItem.SpecData.FreqCentr)) / 1000000, 3);
                                        //if (Math.Abs(MeasMonItem.SpecData.Trace[MeasMonItem.MarkerInd].level - MeasMonItem.Trace[mar[0]].level - MeasMonItem.NdBLevel) < 2 && Math.Abs(MeasMonItem.Trace[MeasMonItem.MarkerInd].level - MeasMonItem.Trace[mar[1]].level - MeasMonItem.NdBLevel) < 2) MeasMonItem.Measured = true;
                                        MeasMonItem.NewDataToSave = true;
                                    }
                                    else
                                    {
                                        MeasMonItem.BWData.NdBResult[1] = -1;
                                        MeasMonItem.BWData.NdBResult[2] = -1;
                                        //MeasMonItem.Measured = false;
                                        MeasMonItem.NewDataToSave = true;
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
                                //MeasMonItem.TraceCount++;
                                // (int)(MeasItem.Trace[MeasItem.Trace.Length - 1].Freq - MeasItem.Trace[0].Freq);// mar[1];
                            }
                            if (MeasMonItem.AllTraceCount < MeasMonItem.AllTraceCountToMeas)
                            { MeasMonItem.AllTraceCount++; }

                            #endregion
                        }
                        else if ((!t1 || !t2) && t3) { /*MeasMonItem.TraceCount++; */MeasMonItem.AllTraceCount++; }
                        if (MeasMonItem.AllTraceCountToMeas == MeasMonItem.AllTraceCount)
                        { MeasMonItem.SpecData.MeasDuration += new TimeSpan(DateTime.Now.Ticks - MeasMonTimeMeas).TotalSeconds; }
                        //if (MeasMonItem.AllTraceCount == MeasMonItem.AllTraceCountToMeas || MeasMonItem.TraceCount == MeasTraceCount || ThisMeasCount >= 10 || MeasMonItem.Power < App.Sett.MeasMons_Settings.MeasMons.Where(x => x.Techonology == MeasMonItem.Techonology).First().DetectionLevel)
                        //{
                        //    ThisMeasCount = -1;
                        //    MeasTraceCount = -1; /*MeasItem = null;*/
                        //}
                        //if (MeasMonItem.AllTraceCount == MeasTraceCount || MeasMonItem.TraceCount == MeasTraceCount || ThisMeasCount >= 10 || MeasMonItem.Power < App.Sett.MeasMons_Settings.MeasMons.Where(x => x.Techonology == MeasMonItem.Techonology).First().DetectionLevel)
                        //{
                        //    ThisMeasCount = -1;
                        //    MeasTraceCount = -1; /*MeasItem = null;*/
                        //}
                        //if (MainWindow.db.MonMeas[MeasItem].TraceCount == MeasTraceCount + 100) MeasTraceCount = 0;
                        #endregion

                    }


                }
                #region
                //Temp = "";
                //Temp += t.Length.ToString() + "\r\n" +
                //            "uint64 " + ((UInt64)((UInt64)(t[28]) + (UInt64)(t[29] << 8) + (UInt64)(t[30] << 16) + (UInt64)(t[31] << 24) + (UInt64)(t[40] << 32) + (UInt64)(t[41] << 40) + (UInt64)(t[42] << 48) + (UInt64)(t[43] << 56))).ToString() + "\r\n" +
                //            "int16 " + ((double)((Int16)((t[0 + Ind] << 8) | (t[1 + Ind])))).ToString() + "\r\n" +
                //            "int16 " + ((double)((Int16)((t[1 + Ind] << 8) | (t[0 + Ind])))).ToString() + "\r\n" +
                //            "uint16 " + ((double)((UInt16)((t[0 + Ind] << 8) | (t[1 + Ind])))).ToString() + "\r\n" +
                //            "uint16 " + ((double)((UInt16)((t[1 + Ind] << 8) | (t[0 + Ind])))).ToString() + "\r\n" +
                //            "uint32 " + ((UInt32)((UInt32)(t[3 + Ind]) + (UInt32)(t[2 + Ind] << 8) + (UInt32)(t[2 + Ind] << 16) + (UInt32)(t[1 + Ind] << 24))).ToString() + "\r\n" +
                //            "uint32 " + ((UInt32)((UInt32)(t[0 + Ind]) + (UInt32)(t[1 + Ind] << 8) + (UInt32)(t[2 + Ind] << 16) + (UInt32)(t[3 + Ind] << 24))).ToString() + "  " + "\r\n" +
                //            "UInt32 " + BitConverter.ToUInt32(t, Ind).ToString() + "\r\n" +
                //            "Int16 " + ((double)BitConverter.ToInt16(t, Ind)).ToString() + "\r\n" +
                //            "Float " + BitConverter.ToSingle(t, Ind).ToString() + "\r\n" +
                //            "UInt64 " + BitConverter.ToUInt64(t, Ind).ToString() + "\r\n" +
                //            "uint64 " + ((UInt64)((UInt64)(t[28]) + (UInt64)(t[29] << 8) + (UInt64)(t[30] << 16) + (UInt64)(t[31] << 24) + (UInt64)(t[40] << 32) + (UInt64)(t[41] << 40) + (UInt64)(t[42] << 48) + (UInt64)(t[43] << 56))).ToString();

                #endregion
                //}
                //#region Exception
                //catch (Exception exp)
                //{
                //    MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "RsReceiver", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
                //}
                //#endregion
            }
        }


        private void SetTraceData(tracepoint[] trace, decimal step, bool finishPscan)
        {
            tracepoint[] temp11 = Trace1;
            tracepoint[] temp21 = Trace2;
            tracepoint[] temp31 = Trace3;
            if (trace.Length > 0 && step > 0)
            {
                if (TracePoints != trace.Length || temp11[1].freq - temp11[0].freq != step || temp11[0].freq != trace[0].freq || temp11[temp11.Length - 1].freq != trace[trace.Length - 1].freq)
                {
                    TracePoints = (int)trace.Length;
                    temp11 = new tracepoint[trace.Length];
                    temp21 = new tracepoint[trace.Length];
                    temp31 = new tracepoint[trace.Length];
                    for (int i = 0; i < trace.Length; i++)
                    {
                        //decimal freq = freqStart + step * i;
                        temp11[i] = new tracepoint() { freq = trace[i].freq, level = trace[i].level };
                        temp21[i] = new tracepoint() { freq = trace[i].freq, level = trace[i].level };
                        temp31[i] = new tracepoint() { freq = trace[i].freq, level = trace[i].level };
                    }
                    MarkersTraceLegthOrFreqsChanged(trace);
                }
                #region Trace 1
                if (Trace1Type.Parameter == "0")
                {
                    for (int i = 0; i < trace.Length; i++)
                    {
                        temp11[i].level = trace[i].level;
                    }
                }
                else if (finishPscan && Trace1Type.Parameter == "1")
                {
                    tracepoint[] temp = new tracepoint[trace.Length];
                    for (int i = 0; i < trace.Length; i++)
                    {
                        temp[i] = new tracepoint() { freq = trace[i].freq, level = trace[i].level };
                    }
                    if (Trace1Reset) { Trace1AveragedList.Reset(); Trace1Reset = false; }
                    Trace1AveragedList.AddTraceToAverade(temp);
                    temp11 = Trace1AveragedList.AveragedTrace;
                }
                else if (finishPscan && Trace1Type.Parameter == "2")
                {
                    tracepoint[] temp = new tracepoint[trace.Length];
                    for (int i = 0; i < trace.Length; i++)
                    {
                        temp[i] = new tracepoint() { freq = trace[i].freq, level = trace[i].level };
                    }
                    if (Trace1Reset) { Trace1TrackedList.Reset(); Trace1Reset = false; }
                    Trace1TrackedList.AddTraceToTracking(temp);
                    temp11 = Trace1TrackedList.TrackingTrace;
                }
                else if (finishPscan && Trace1Type.Parameter == "3")
                {
                    if (Trace1Reset == false)
                    {
                        for (int i = 0; i < trace.Length; i++)
                        {
                            if (trace[i].level > temp11[i].level) temp11[i].level = trace[i].level;
                        }
                    }
                    else
                    {
                        for (int i = 0; i < trace.Length; i++)
                        {
                            temp11[i].level = trace[i].level;
                        }
                        Trace1Reset = false;
                    }
                }
                else if (finishPscan && Trace1Type.Parameter == "4")
                {
                    if (Trace1Reset == false)
                    {
                        for (int i = 0; i < trace.Length; i++)
                        {
                            if (trace[i].level < temp11[i].level) temp11[i].level = trace[i].level;
                        }
                    }
                    else
                    {
                        for (int i = 0; i < trace.Length; i++)
                        {
                            temp11[i].level = trace[i].level;
                        }
                        Trace1Reset = false;
                    }
                }
                else if (Trace1Type.Parameter == "5")
                {
                    //пропускаем 
                }
                else if (Trace1Type.Parameter == "6")
                {
                    //пропускаем и не рисуем
                }
                #endregion Trace 1
                #region Trace 2
                if (Trace2Type.Parameter == "0")
                {
                    for (int i = 0; i < trace.Length; i++)
                    {
                        temp21[i].level = trace[i].level;
                    }
                }
                else if (finishPscan && Trace2Type.Parameter == "1")
                {
                    tracepoint[] temp = new tracepoint[trace.Length];
                    for (int i = 0; i < trace.Length; i++)
                    {
                        temp[i] = new tracepoint() { freq = trace[i].freq, level = trace[i].level };
                    }
                    if (Trace2Reset) { Trace2AveragedList.Reset(); Trace2Reset = false; }
                    Trace2AveragedList.AddTraceToAverade(temp);
                    temp21 = Trace2AveragedList.AveragedTrace;
                }
                else if (finishPscan && Trace2Type.Parameter == "2")
                {
                    tracepoint[] temp = new tracepoint[trace.Length];
                    for (int i = 0; i < trace.Length; i++)
                    {
                        temp[i] = new tracepoint() { freq = trace[i].freq, level = trace[i].level };
                    }
                    if (Trace2Reset) { Trace2TrackedList.Reset(); Trace2Reset = false; }
                    Trace2TrackedList.AddTraceToTracking(temp);
                    temp21 = Trace2TrackedList.TrackingTrace;
                }
                else if (finishPscan && Trace2Type.Parameter == "3")
                {
                    if (Trace2Reset == false)
                    {
                        for (int i = 0; i < trace.Length; i++)
                        {
                            if (trace[i].level > temp21[i].level) temp21[i].level = trace[i].level;
                        }
                    }
                    else
                    {
                        for (int i = 0; i < trace.Length; i++)
                        {
                            temp21[i].level = trace[i].level;
                        }
                        Trace2Reset = false;
                    }
                }
                else if (finishPscan && Trace2Type.Parameter == "4")
                {
                    if (Trace2Reset == false)
                    {
                        for (int i = 0; i < trace.Length; i++)
                        {
                            if (trace[i].level < temp21[i].level) temp21[i].level = trace[i].level;
                        }
                    }
                    else
                    {
                        for (int i = 0; i < trace.Length; i++)
                        {
                            temp21[i].level = trace[i].level;
                        }
                        Trace2Reset = false;
                    }
                }
                else if (Trace2Type.Parameter == "5")
                {
                    //пропускаем 
                }
                else if (Trace2Type.Parameter == "6")
                {
                    //пропускаем и не рисуем
                    for (int i = 0; i < Markers.Count(); i++)
                    {
                        if (Markers[i].State && Markers[i].TraceNumberIndex == 1)
                        {
                            Markers[i].TraceNumberIndex = 0;
                        }
                    }
                }
                #endregion Trace 2
                #region Trace 3
                if (Trace3Type.Parameter == "0")
                {
                    for (int i = 0; i < trace.Length; i++)
                    {
                        temp31[i].level = trace[i].level;
                    }
                }
                else if (finishPscan && Trace3Type.Parameter == "1")
                {
                    tracepoint[] temp = new tracepoint[trace.Length];
                    for (int i = 0; i < trace.Length; i++)
                    {
                        temp[i] = new tracepoint() { freq = trace[i].freq, level = trace[i].level };
                    }
                    if (Trace3Reset) { Trace3AveragedList.Reset(); Trace3Reset = false; }
                    Trace3AveragedList.AddTraceToAverade(temp);
                    temp31 = Trace3AveragedList.AveragedTrace;
                }
                else if (finishPscan && Trace3Type.Parameter == "2")
                {
                    tracepoint[] temp = new tracepoint[trace.Length];
                    for (int i = 0; i < trace.Length; i++)
                    {
                        temp[i] = new tracepoint() { freq = trace[i].freq, level = trace[i].level };
                    }
                    if (Trace3Reset) { Trace3TrackedList.Reset(); Trace3Reset = false; }
                    Trace3TrackedList.AddTraceToTracking(temp);
                    temp31 = Trace3TrackedList.TrackingTrace;
                }
                else if (finishPscan && Trace3Type.Parameter == "3")
                {
                    if (Trace3Reset == false)
                    {
                        for (int i = 0; i < trace.Length; i++)
                        {
                            if (trace[i].level > temp31[i].level) temp31[i].level = trace[i].level;
                        }
                    }
                    else
                    {
                        for (int i = 0; i < trace.Length; i++)
                        {
                            temp31[i].level = trace[i].level;
                        }
                        Trace3Reset = false;
                    }
                }
                else if (finishPscan && Trace3Type.Parameter == "4")
                {
                    if (Trace3Reset == false)
                    {
                        for (int i = 0; i < trace.Length; i++)
                        {
                            if (trace[i].level < temp31[i].level) temp31[i].level = trace[i].level;
                        }
                    }
                    else
                    {
                        for (int i = 0; i < trace.Length; i++)
                        {
                            temp31[i].level = trace[i].level;
                        }
                        Trace3Reset = false;
                    }
                }
                else if (Trace3Type.Parameter == "5")
                {
                    //пропускаем 
                }
                else if (Trace3Type.Parameter == "6")
                {
                    //пропускаем и не рисуем
                    //и грохнем маркеры с этого трейса
                    for (int i = 0; i < Markers.Count(); i++)
                    {
                        if (Markers[i].State && Markers[i].TraceNumberIndex == 2)
                        {
                            if (Trace2Type.Parameter != "6")
                            { Markers[i].TraceNumberIndex = 1; }
                            else { Markers[i].TraceNumberIndex = 0; }
                        }
                    }
                }
                #endregion Trace 3
                Trace1 = temp11;
                Trace2 = temp21;
                Trace3 = temp31;
                if (ChannelPowerState)
                {
                    //if (Markers[0].Level != null)
                    //{
                    //    if (Markers[0].TraceNumber.Parameter == "0") { ChannelPowerResult = Math.Round(LM.MeasChannelPower(Trace1, (FreqStart + FreqStop) / 2, ChannelPowerBW, 0), new AllLevelUnits().dBµV.ind); }
                    //    else if (Markers[0].TraceNumber.Parameter == "1") { ChannelPowerResult = Math.Round(LM.MeasChannelPower(Trace2, (FreqStart + FreqStop) / 2, ChannelPowerBW, 0), new AllLevelUnits().dBµV.ind); }
                    //    else if (Markers[0].TraceNumber.Parameter == "2") { ChannelPowerResult = Math.Round(LM.MeasChannelPower(Trace3, (FreqStart + FreqStop) / 2, ChannelPowerBW, 0), new AllLevelUnits().dBµV.ind); }
                    //}
                    //ChannelPowerResult = LM.MeasChannelPower(Trace1, FreqCentr, ChannelPowerBW, new AllLevelUnits().dBµV.ind);
                }
                SetMarkerData();
            }
        }
        private void SetMarkerData()
        {
            int count = 0;
            for (int i = 0; i < Markers.Count(); i++)
            {
                if (Markers[i].State == true)
                {
                    //уровни
                    if (Markers[i].TraceNumber.Parameter == "0")
                    {
                        if (Trace1 != null && Trace1.Length > 0)
                        {
                            Markers[i].Level = Trace1[Markers[i].IndexOnTrace].level;
                            if (Markers[i].MarkerType == 3)
                            {
                                int[] t = LM.GetMeasNDB(Trace1, Markers[i].IndexOnTrace, (double)NdBLevel);

                                Markers[i].TMarkers[0].IndexOnTrace = t[0];
                                Markers[i].TMarkers[0].Freq = Trace1[t[0]].freq;
                                Markers[i].TMarkers[0].Level = Trace1[t[0]].level;

                                Markers[i].TMarkers[1].IndexOnTrace = t[1];
                                Markers[i].TMarkers[1].Freq = Trace1[t[1]].freq;
                                Markers[i].TMarkers[1].Level = Trace1[t[1]].level;
                                Markers[i].TMarkers[0].Funk2 = Markers[i].TMarkers[1].Freq - Markers[i].TMarkers[0].Freq;
                                NdBResult = Markers[i].TMarkers[0].Funk2;
                            }
                        }
                    }
                    else if (Markers[i].TraceNumber.Parameter == "1")
                    {
                        if (Trace2 != null && Trace2.Length > 0)
                        {
                            Markers[i].Level = Trace2[Markers[i].IndexOnTrace].level;
                            if (Markers[i].MarkerType == 3)
                            {
                                int[] t = LM.GetMeasNDB(Trace2, Markers[i].IndexOnTrace, (double)NdBLevel);

                                Markers[i].TMarkers[0].IndexOnTrace = t[0];
                                Markers[i].TMarkers[0].Freq = Trace2[t[0]].freq;
                                Markers[i].TMarkers[0].Level = Trace2[t[0]].level;

                                Markers[i].TMarkers[1].IndexOnTrace = t[1];
                                Markers[i].TMarkers[1].Freq = Trace2[t[1]].freq;
                                Markers[i].TMarkers[1].Level = Trace2[t[1]].level;
                                Markers[i].TMarkers[0].Funk2 = Markers[i].TMarkers[1].Freq - Markers[i].TMarkers[0].Freq;
                                NdBResult = Markers[i].TMarkers[0].Funk2;
                            }
                        }
                    }
                    else if (Markers[i].TraceNumber.Parameter == "2")
                    {
                        if (Trace3 != null && Trace3.Length > 0)
                        {
                            Markers[i].Level = Trace3[Markers[i].IndexOnTrace].level;
                            if (Markers[i].MarkerType == 3)
                            {
                                int[] t = LM.GetMeasNDB(Trace3, Markers[i].IndexOnTrace, (double)NdBLevel);

                                Markers[i].TMarkers[0].IndexOnTrace = t[0];
                                Markers[i].TMarkers[0].Freq = Trace3[t[0]].freq;
                                Markers[i].TMarkers[0].Level = Trace3[t[0]].level;

                                Markers[i].TMarkers[1].IndexOnTrace = t[1];
                                Markers[i].TMarkers[1].Freq = Trace3[t[1]].freq;
                                Markers[i].TMarkers[1].Level = Trace3[t[1]].level;
                                Markers[i].TMarkers[0].Funk2 = Markers[i].TMarkers[1].Freq - Markers[i].TMarkers[0].Freq;
                                NdBResult = Markers[i].TMarkers[0].Funk2;
                            }
                        }
                    }
                    count++;
                }
            }
            for (int i = 0; i < Markers.Count(); i++)
            {
                if (Markers[i].State == true && Markers[i].MarkerType == 1)
                {
                    if (Markers[i].Funk1 != Markers[i].Freq - Markers[i].MarkerParent.Freq) Markers[i].Funk1 = Markers[i].Freq - Markers[i].MarkerParent.Freq;
                    if ((double)Markers[i].Funk2 != Markers[i].Level - Markers[i].MarkerParent.Level) Markers[i].Funk2 = (decimal)(Markers[i].Level - Markers[i].MarkerParent.Level);
                }
            }
            //MarkersIsEnabled = count;
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #region Public Method
        public void SetMeasMode(bool measmode)
        {
            MeasMode = measmode;
            SetToDelegate(SetMeasMode);
        }
        public void SetMeasTime(decimal meastime)
        {
            MeasTime = meastime;
            SetToDelegate(SetMeasTime);
        }
        public void SetDemodFromIndex(int index)
        {
            DemodInd = index;
            SetToDelegate(SetDemodFromIndex);
        }
        public void SetDemodBWFromIndex(int index)
        {
            DemodBWInd = index;
            SetToDelegate(SetDemodBWFromIndex);
        }
        public void SetDetectorFromIndex(int index)
        {
            DetectorInd = index;
            SetToDelegate(SetDetectorFromIndex);
        }
        public void SetAFC(bool state)
        {
            AFCState = state;
            SetToDelegate(SetAFC);
        }
        public void SetATTFixState(bool state)
        {
            ATTFixState = state;
            SetToDelegate(SetATTFixState);
        }
        public void SetATTAutoState(bool state)
        {
            ATTAutoState = state;
            SetToDelegate(SetATTAutoState);
        }
        public void SetATT(int att)
        {
            ATT = att;
            SetToDelegate(SetATT);
        }
        public void SetMGC(int mgc)
        {
            MGC = mgc;
            SetToDelegate(SetMGC);
        }
        public void SetMGCAutoState(bool mgcauto)
        {
            MGCAutoState = mgcauto;
            SetToDelegate(SetMGCAutoState);
        }
        #region FFM
        public void SetFFMFreqCentr(decimal ffmfreqcentr)
        {
            FFMFreqCentrToSet = ffmfreqcentr;
            SetToDelegate(SetFFMFreqCentr);
        }
        public void SetFFMFreqSpanFromIndex(int index)
        {
            FFMFreqSpanIndToSet = index;
            SetToDelegate(SetFFMFreqSpanFromIndex);
        }
        public void SetFFMFreqSpan(decimal ffmfreqspan)
        {
            FFMFreqSpanIndToSet = System.Array.IndexOf(UniqueData.FFMSpanBW, ffmfreqspan);
            SetToDelegate(SetFFMFreqSpanFromIndex);
        }

        public void SetFFMStepFromIndex(int index)
        {
            FFMStepIndToSet = index;
            SetToDelegate(SetFFMStepFromIndex);
        }
        public void SetFFMStep(decimal ffmstep)
        {
            FFMStepIndToSet = System.Array.IndexOf(UniqueData.FFMStepBW, ffmstep);
            SetToDelegate(SetFFMStepFromIndex);
        }
        public void SetFFMStepAuto(bool stepauto)
        {
            FFMStepAuto = stepauto;
            SetToDelegate(SetFFMStepAuto);
        }
        public void SetFFMFFTMode(ParamWithUI ffmfftmode)
        {
            FFMFFTMode = ffmfftmode;
            SetToDelegate(SetFFMFFTMode);
        }
        public void SetFFMRefLevel(decimal reflevel)
        {
            FFMRefLevel = reflevel;
            SetToDelegate(SetFFMRefLevel);
        }
        public void SetFFMRangeLevel(decimal rangelevel)
        {
            FFMRangeLevel = rangelevel;
            SetToDelegate(SetFFMRangeLevel);
        }
        public void SetSelectivity(ParamWithUI selectivitymode)
        {
            SelectivityMode = selectivitymode;
            SetToDelegate(SetSelectivity);
        }
        #endregion FFM

        #region PSCAN
        public void SetPScanFreqStart(decimal pscanfreqstart)
        {
            PScanFreqStart = pscanfreqstart;
            SetToDelegate(SetPScanFreqStart);
        }
        public void SetPSCANFreqStop(decimal pscanfreqstop)
        {
            PScanFreqStop = pscanfreqstop;
            SetToDelegate(SetPScanFreqStop);
        }
        public void SetPSCANFreqCentr(decimal pscanfreqcentr)
        {
            PScanFreqCentr = pscanfreqcentr;
            SetToDelegate(SetPScanFreqCentr);
        }
        public void SetPSCANFreqSpan(decimal pscanfreqspan)
        {
            PScanFreqSpan = pscanfreqspan;
            SetToDelegate(SetPScanFreqSpan);
        }

        public void SetPScanStepIndex(int index)
        {
            PScanStepInd = index;
            SetToDelegate(SetPScanStepIndex);
        }
        public void SetPScanFFTMode(ParamWithUI pscanfftmode)
        {
            PScanFFTMode = pscanfftmode;
            SetToDelegate(SetPScanFFTMode);
        }
        public void SetPScanRefLevel(decimal reflevel)
        {
            PScanRefLevel = reflevel;
            SetToDelegate(SetPScanRefLevel);
        }
        public void SetPScanRangeLevel(decimal rangelevel)
        {
            PScanRangeLevel = rangelevel;
            SetToDelegate(SetPScanRangeLevel);
        }
        #endregion PSCAN

        #region Markers
        public void SetMarkerState(Marker marker, bool state)
        {
            marker.StateNew = state;
            marker.State = state;
            if (marker.State == true && marker.StateNew == true)//был выключен
            {
                if (marker.IndexOnTrace < 0)
                {
                    if (marker.TraceNumber.Parameter == "0")
                    {
                        marker.IndexOnTrace = LM.PeakSearch(Trace1);
                        marker.Freq = Trace1[marker.IndexOnTrace].freq;
                    }
                    else if (marker.TraceNumber.Parameter == "1")
                    {
                        marker.IndexOnTrace = LM.PeakSearch(Trace2);
                        marker.Freq = Trace2[marker.IndexOnTrace].freq;
                    }
                    else if (marker.TraceNumber.Parameter == "2")
                    {
                        marker.IndexOnTrace = LM.PeakSearch(Trace3);
                        marker.Freq = Trace3[marker.IndexOnTrace].freq;
                    }
                }
                else
                {
                    if (marker.IndexOnTrace > Trace1.Length - 1 && marker.TraceNumber.Parameter == "0")
                    {
                        marker.IndexOnTrace = Trace1.Length - 1;
                        marker.Freq = Trace1[marker.IndexOnTrace].freq;
                    }
                    else if (marker.IndexOnTrace > Trace2.Length - 1 && marker.TraceNumber.Parameter == "1")
                    {
                        marker.IndexOnTrace = Trace2.Length - 1;
                        marker.Freq = Trace2[marker.IndexOnTrace].freq;
                    }
                    else if (marker.IndexOnTrace > Trace3.Length - 1 && marker.TraceNumber.Parameter == "2")
                    {
                        marker.IndexOnTrace = Trace3.Length - 1;
                        marker.Freq = Trace3[marker.IndexOnTrace].freq;
                    }
                }
            }
            else if (marker.State == false && marker.StateNew == false)
            {
                //marker.StateNew = false;
                marker.MarkerType = 0;
                marker.MarkerTypeNew = 0;
                marker.FunctionDataType = 0;
                if (marker.Index == 1)
                {
                    for (int i = 0; i < Markers.Count; i++)
                    {
                        if (Markers[i].MarkerType == 1)
                        {
                            Markers[i].State = false;
                            Markers[i].StateNew = false;
                            Markers[i].MarkerType = 0;
                            Markers[i].MarkerTypeNew = 0;
                            Markers[i].FunctionDataType = 0;
                        }
                    }
                }
            }
        }

        public void SetMarkerFromFreq(Marker marker, decimal freq)
        {

            if (marker.State == false)//был выключен
            { marker.State = true; marker.StateNew = true; }
            if (marker.TraceNumber.Parameter == "0")
            {
                marker.IndexOnTrace = LM.FindMarkerIndOnTrace(Trace1, freq);
                marker.Freq = Trace1[marker.IndexOnTrace].freq;
            }
            else if (marker.TraceNumber.Parameter == "1")
            {
                marker.IndexOnTrace = LM.FindMarkerIndOnTrace(Trace2, freq);
                marker.Freq = Trace2[marker.IndexOnTrace].freq;
            }
            else if (marker.TraceNumber.Parameter == "2")
            {
                marker.IndexOnTrace = LM.FindMarkerIndOnTrace(Trace3, freq);
                marker.Freq = Trace3[marker.IndexOnTrace].freq;
            }
            marker.FreqNew = marker.Freq;
        }

        public void SetMarkerFromIndex(Marker marker, int newindex)
        {
            if (marker.State == false)//был выключен
            { marker.State = true; marker.StateNew = true; }
            if (newindex < 0) newindex = 0;
            else if (newindex > TracePoints) newindex = TracePoints - 1;
            if (marker.TraceNumber.Parameter == "0")
            {
                marker.IndexOnTrace = newindex;
                marker.Freq = Trace1[marker.IndexOnTrace].freq;
            }
            else if (marker.TraceNumber.Parameter == "1")
            {
                marker.IndexOnTrace = newindex;
                marker.Freq = Trace2[marker.IndexOnTrace].freq;
            }
            else if (marker.TraceNumber.Parameter == "2")
            {
                marker.IndexOnTrace = newindex;
                marker.Freq = Trace3[marker.IndexOnTrace].freq;
            }
            marker.FreqNew = marker.Freq;
        }

        public void SetMarkerPeakSearch(Marker marker)
        {
            if (marker.State == false)//был выключен
            { marker.State = true; marker.StateNew = true; }
            if (marker.TraceNumber.Parameter == "0")
            {
                marker.IndexOnTrace = LM.PeakSearch(Trace1);
                marker.Freq = Trace1[marker.IndexOnTrace].freq;
            }
            else if (marker.TraceNumber.Parameter == "1")
            {
                marker.IndexOnTrace = LM.PeakSearch(Trace2);
                marker.Freq = Trace2[marker.IndexOnTrace].freq;
            }
            else if (marker.TraceNumber.Parameter == "2")
            {
                marker.IndexOnTrace = LM.PeakSearch(Trace3);
                marker.Freq = Trace3[marker.IndexOnTrace].freq;
            }
        }

        public void SetMarkerChangeType(Marker marker)
        {
            if (marker.State == false)//был выключен
            {
                if (Markers[0].State == false) SetMarkerState(Markers[0], true);
                marker.State = true;
                marker.StateNew = true;
                marker.IndexOnTrace = LM.PeakSearch(Trace1);
                marker.Freq = Trace1[marker.IndexOnTrace].freq;
            }
            if (marker.MarkerType == 0)
            {
                marker.MarkerParent = Markers[0];
                marker.MarkerType = 1;
                marker.MarkerTypeNew = marker.MarkerType;
                marker.FunctionDataType = 1;
            }
            else if (marker.MarkerType == 1)
            { marker.MarkerParent = null; marker.MarkerType = 0; marker.MarkerTypeNew = 0; marker.FunctionDataType = 0; }
        }

        /// <summary>
        /// При любом чихе трейса по частоте или количевству точек 
        /// дергать это ибо маркеры сойдут с ума 
        /// (уровень получают из индекса, и т.к. он не менялся то уровень не от той частоты будит)
        /// </summary>
        private void MarkersTraceLegthOrFreqsChanged(tracepoint[] trace)
        {
            for (int i = 0; i < Markers.Count; i++)
            {
                if (Markers[i].State == true)//был выключен
                {
                    Markers[i].IndexOnTrace = LM.FindMarkerIndOnTrace(trace, Markers[i].Freq);
                    Markers[i].Freq = trace[Markers[i].IndexOnTrace].freq;
                    Markers[i].FreqNew = Markers[i].FreqNew;
                }
            }
        }

        public void SetMarkerNdB()
        {
            if (_NdBState == true) { Markers[0].MarkerType = 3; Markers[0].MarkerTypeNew = 3; Markers[0].FunctionDataType = 2; }
            else { Markers[0].MarkerType = 0; Markers[0].MarkerTypeNew = 0; Markers[0].FunctionDataType = 0; }
            if (Markers[0].State == false)//был выключен
            {
                Markers[0].State = true;
                Markers[0].IndexOnTrace = LM.PeakSearch(Trace1);
                Markers[0].Freq = Trace1[Markers[0].IndexOnTrace].freq;
                Markers[0].FreqNew = Markers[0].Freq;
                Markers[0].FunctionDataType = 2;
                Markers[0].Funk2 = NdBLevel;
            }


        }
        #endregion Markers


        #endregion Public Method

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #region private Methods
        #region Global
        private void SetMeasMode()
        {
            try
            {
                if (tc.IsOpen)
                {
                    if (MeasMode) { tc.WriteLine("MEAS:MODE CONT"); }
                    else { tc.WriteLine("MEAS:MODE PER"); }
                    string t = tc.Query("MEAS:MODE?").TrimEnd();
                    if (t.Contains("CONT")) { MeasMode = true; }
                    else if (t.Contains("PER")) { MeasMode = false; }
                }
            }
            #region Exception
            catch (Exception exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "RsReceiver", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            #endregion
            finally { TelnetDM -= SetMeasMode; }
        }

        /// <summary>
        /// Установка времени измерения
        /// </summary>
        private void SetMeasTime()
        {
            try
            {
                if (tc.IsOpen)
                {
                    tc.WriteLine("MEAS:TIME " + MeasTime.ToString("G29").Replace(',', '.'));
                }
            }
            #region Exception
            catch (Exception exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "RsReceiver", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            #endregion
            finally { TelnetDM -= SetMeasTime; }
        }

        private void SetSelectivity()
        {
            try
            {
                if (tc.IsOpen && UniqueData.SelectivityChangeable == true)
                {
                    tc.WriteLine("CALC:IFP:SEL " + SelectivityMode.Parameter);
                }
            }
            #region Exception
            catch (Exception exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "RsReceiver", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            #endregion
            finally { TelnetDM -= SetSelectivity; }
        }

        /// <summary>
        /// Установка типа демодуляции
        /// </summary>
        private void SetDemodFromIndex()
        {
            try
            {
                if (tc.IsOpen)
                {
                    tc.WriteLine("DEM " + UniqueData.Demod[DemodInd]);
                    string ss = tc.Query(":DEM?");
                    for (int i = 0; i < UniqueData.Demod.Length; i++)
                    {
                        if (ss.ToUpper() == UniqueData.Demod[i]) { DemodInd = i; }
                    }
                }
            }
            #region Exception
            catch (Exception exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "RsReceiver", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            #endregion
            finally { TelnetDM -= SetDemodFromIndex; }
        }

        /// <summary>
        /// Установка полосы демодуляции
        /// </summary>
        private void SetDemodBWFromIndex()
        {
            try
            {
                if (tc.IsOpen)
                {
                    tc.WriteLine("BAND " + UniqueData.DemodBW[DemodBWInd].ToString("G29").Replace(',', '.'));
                    DemodBW = decimal.Parse(tc.Query(":BAND?").Replace('.', ','));
                    for (int i = 0; i < UniqueData.DemodBW.Length; i++)
                    {
                        if (DemodBW == UniqueData.DemodBW[i]) { DemodBWInd = i; }
                    }
                }
            }
            #region Exception
            catch (Exception exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "RsReceiver", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            #endregion
            finally { TelnetDM -= SetDemodBWFromIndex; }
        }

        /// <summary>
        /// Установка типа детектора
        /// </summary>
        private void SetDetectorFromIndex()
        {
            try
            {
                if (tc.IsOpen)
                {
                    tc.WriteLine("DET " + Detector.Parameter);
                    string ss = tc.Query(":DET?");
                    for (int i = 0; i < UniqueData.Detectors.Count; i++)
                    {
                        if (ss.ToUpper() == UniqueData.Detectors[i].Parameter) { DetectorInd = i; }
                    }
                }
            }
            #region Exception
            catch (Exception exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "RsReceiver", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            #endregion
            finally { TelnetDM -= SetDetectorFromIndex; }
        }

        /// <summary>
        /// Установка AFC
        /// </summary>
        private void SetAFC()
        {
            try
            {
                if (tc.IsOpen)
                {
                    if (AFCState) tc.WriteLine("FREQ:AFC 1");
                    else tc.WriteLine("FREQ:AFC 0");
                }
            }
            #region Exception
            catch (Exception exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "RsReceiver", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            #endregion
            finally { TelnetDM -= SetAFC; }
        }


        private void SetATTFixState()
        {
            try
            {
                if (tc.IsOpen && UniqueData.ATTFix == true)
                {
                    if (ATTFixState) { tc.WriteLine("INPut:ATTenuation:STATe 1"); }
                    else if (!ATTFixState) { tc.WriteLine("INPut:ATTenuation:STATe 0"); }
                }
            }
            #region Exception
            catch (Exception exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "RsReceiver", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            #endregion
            finally { TelnetDM -= SetATTFixState; }
        }
        private void SetATTAutoState()
        {
            try
            {
                if (tc.IsOpen)
                {
                    if (ATTAutoState) { tc.WriteLine("INP:ATT:AUTO 1"); }
                    else
                    {
                        tc.WriteLine("INP:ATT:AUTO 0");
                        ATT = Int32.Parse(tc.Query("INP:ATT?").Replace('.', ','));
                    }
                }
            }
            #region Exception
            catch (Exception exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "RsReceiver", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            #endregion
            finally { TelnetDM -= SetATTAutoState; }
        }
        /// <summary>
        /// Установка ATT
        /// </summary>
        private void SetATT()
        {
            try
            {
                if (tc.IsOpen && UniqueData.ATTFix == false)
                {

                    ATTAutoState = false;
                    tc.WriteLine("INP:ATT " + ATT.ToString());
                }
            }
            #region Exception
            catch (Exception exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "RsReceiver", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            #endregion
            finally { TelnetDM -= SetATT; }
        }

        /// <summary>
        /// Установка MGC
        /// </summary>
        private void SetMGC()
        {
            try
            {
                if (tc.IsOpen)
                {
                    MGCAutoState = false;
                    tc.WriteLine("GCON:MODE FIX");
                    tc.WriteLine("GCON " + MGC.ToString());
                }
            }
            #region Exception
            catch (Exception exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "RsReceiver", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            #endregion
            finally { TelnetDM -= SetMGC; }
        }

        private void SetMGCAutoState()
        {
            try
            {
                if (tc.IsOpen)
                {
                    if (MGCAutoState) { tc.WriteLine("GCON:MODE AUTO"); }
                    else { tc.WriteLine("GCON:MODE FIX"); MGC = Int32.Parse(tc.Query("GCON?").Replace('.', ',')); }
                }
            }
            #region Exception
            catch (Exception exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "RsReceiver", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            #endregion
            finally { TelnetDM -= SetMGCAutoState; }
        }


        /// <summary>
        /// Установка SQU
        /// </summary>
        public void SetSQU()
        {
            if (tc.IsOpen)
            {
                try
                {
                    SQUState = true;
                    tc.WriteLine("OUTP:SQU ON");
                    tc.WriteLine("OUTP:SQU:THR " + SQU.ToString());
                }
                catch { }
            }
            TelnetDM -= SetSQU;
        }
        public void SetSQUUp()
        {
            if (tc.IsOpen)
            {
                try
                {
                    SQU++;
                    SQUState = true;
                    tc.WriteLine("OUTP:SQU ON");
                    tc.WriteLine("OUTP:SQU:THR " + SQU.ToString());
                }
                catch { }
            }
            TelnetDM -= SetSQUUp;
        }
        public void SetSQUDn()
        {
            if (tc.IsOpen)
            {
                try
                {
                    SQU--;
                    SQUState = true;
                    tc.WriteLine("OUTP:SQU ON");
                    tc.WriteLine("OUTP:SQU:THR " + SQU.ToString());
                }
                catch { }
            }
            TelnetDM -= SetSQUDn;
        }

        public void SetSQUState()
        {
            if (tc.IsOpen)
            {
                try
                {
                    SQUState = !SQUState;
                    if (SQUState) { tc.WriteLine("OUTP:SQU ON"); }
                    else { tc.WriteLine("OUTP:SQU OFF"); SQU = Int32.Parse(tc.Query("OUTP:SQU:THR?").Replace('.', ',')); }
                }
                catch { }
            }
            TelnetDM -= SetSQUState;
        }
        #endregion Global


        #region FFM
        /// <summary>
        /// Установка центральной частоты
        /// </summary>
        private void SetFFMFreqCentr()
        {
            try
            {
                if (tc.IsOpen)
                {
                    tc.WriteLine(":FREQ " + FFMFreqCentrToSet.ToString("G29").Replace(',', '.'));
                }
            }
            #region Exception
            catch (Exception exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "RsReceiver", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            #endregion
            finally { TelnetDM -= SetFFMFreqCentr; }
        }
        /// <summary>
        /// Установка полосы просмотра SPAN 
        /// </summary>
        private void SetFFMFreqSpanFromIndex()
        {
            try
            {
                if (tc.IsOpen)
                {
                    tc.WriteLine("FREQ:SPAN " + UniqueData.FFMSpanBW[FFMFreqSpanIndToSet].ToString("G29").Replace(',', '.'));
                    FFMFreqSpanInd = System.Array.IndexOf(UniqueData.FFMSpanBW, decimal.Parse(tc.Query("FREQ:SPAN?")));
                }
            }
            #region Exception
            catch (Exception exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "RsReceiver", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            #endregion
            finally { TelnetDM -= SetFFMFreqSpanFromIndex; }
        }

        /// <summary>
        /// Установка шаг просмотра STEP 
        /// </summary>
        private void SetFFMStepFromIndex()
        {
            try
            {
                if (tc.IsOpen)
                {

                    tc.WriteLine("CALC:IFP:STEP " + UniqueData.FFMStepBW[FFMStepIndToSet].ToString("G29").Replace(',', '.'));
                    FFMStepInd = System.Array.IndexOf(UniqueData.FFMStepBW, decimal.Parse(tc.Query("CALC:IFP:STEP?").Replace('.', ',')));
                }
            }
            #region Exception
            catch (Exception exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "RsReceiver", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            #endregion
            finally { TelnetDM -= SetFFMStepFromIndex; }

        }

        private void SetFFMStepAuto()
        {
            try
            {
                if (tc.IsOpen)
                {

                    if (FFMStepAuto) { tc.WriteLine("CALC:IFP:STEP:AUTO 1"); }
                    else { tc.WriteLine("CALC:IFP:STEP:AUTO 0"); }
                    string t = tc.Query("CALC:IFP:STEP:AUTO?");
                    if (t.Contains("1")) { FFMStepAuto = true; }
                    else if (t.Contains("0")) { FFMStepAuto = false; }
                    FFMStepInd = System.Array.IndexOf(UniqueData.FFMStepBW, decimal.Parse(tc.Query("CALC:IFP:STEP?").Replace('.', ',')));
                }
            }
            #region Exception
            catch (Exception exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "RsReceiver", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            #endregion
            finally { TelnetDM -= SetFFMStepAuto; }
        }

        /// <summary>
        /// Установка режима усреднения FFT 
        /// </summary>
        private void SetFFMFFTMode()
        {
            try
            {
                if (tc.IsOpen)
                {
                    tc.WriteLine("CALC:IFP:AVER:TYPE " + FFMFFTMode.Parameter);
                }
            }
            #region Exception
            catch (Exception exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "RsReceiver", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            #endregion
            finally { TelnetDM -= SetFFMFFTMode; }
        }

        private void SetFFMRefLevel()
        {
            try
            {
                if (tc.IsOpen)
                {
                    if (UniqueData.InstrModel == "EM100" || UniqueData.InstrModel == "PR100")
                    {
                        tc.WriteLine("DISPlay:IFPan:LEVel:REFerence " + FFMRefLevel.ToString("G29").Replace(',', '.'));
                        FFMRefLevel = decimal.Parse(tc.Query("DISPlay:IFPan:LEVel:REFerence?").Replace('.', ','));
                    }
                    else if (UniqueData.InstrModel.Contains("EB5") || UniqueData.InstrModel.Contains("DDF2") || UniqueData.InstrModel == "ESMD")
                    {
                        tc.WriteLine("CALCulate:PIFPan:RLEVel " + FFMRefLevel.ToString("G29").Replace(',', '.'));
                        FFMRefLevel = decimal.Parse(tc.Query("CALCulate:PIFPan:RLEVel?").Replace('.', ','));
                    }
                }
            }
            #region Exception
            catch (Exception exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "RsReceiver", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            #endregion
            finally { TelnetDM -= SetFFMRefLevel; }
        }

        public void SetFFMRangeLevel()
        {
            try
            {
                if (tc.IsOpen)
                {
                    if (UniqueData.InstrModel == "EM100" || UniqueData.InstrModel == "PR100")
                    {
                        tc.WriteLine("DISPlay:IFPan:LEVel:RANGe " + ((int)FFMRangeLevel).ToString());//.ToString("G29").Replace(',', '.'));
                        FFMRangeLevel = decimal.Parse(tc.Query("DISPlay:IFPan:LEVel:RANGe?").Replace('.', ','));//
                    }
                    else if (UniqueData.InstrModel.Contains("EB5") || UniqueData.InstrModel.Contains("DDF2") || UniqueData.InstrModel == "ESMD")
                    {
                        tc.WriteLine("CALCulate:PIFPan:LRANge " + FFMRangeLevel.ToString("G29").Replace(',', '.'));
                        FFMRangeLevel = decimal.Parse(tc.Query("CALCulate:PIFPan:LRANge?").Replace('.', ','));
                    }
                }
            }
            #region Exception
            catch (Exception exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "RsReceiver", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            #endregion
            finally { TelnetDM -= SetFFMRangeLevel; }
        }
        #endregion FFM

        #region PScan
        private void SetPScanFreqStart()
        {
            try
            {
                if (tc.IsOpen)
                {
                    tc.WriteLine("FREQ:PSC:STAR " + PScanFreqStart.ToString("G29").Replace(',', '.'));
                    PScanFreqStart = decimal.Parse(tc.Query("FREQ:PSC:STAR?").Replace('.', ','));
                }
            }
            #region Exception
            catch (Exception exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "RsReceiver", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            #endregion
            finally { TelnetDM -= SetPScanFreqStart; }
        }

        private void SetPScanFreqStop()
        {
            try
            {
                if (tc.IsOpen)
                {
                    tc.WriteLine("FREQ:PSC:STOP " + PScanFreqStop.ToString("G29").Replace(',', '.'));
                    PScanFreqStop = decimal.Parse(tc.Query("FREQ:PSC:STOP?"));
                }
            }
            #region Exception
            catch (Exception exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "RsReceiver", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            #endregion
            finally { TelnetDM -= SetPScanFreqStop; }
        }

        private void SetPScanFreqCentr()
        {
            try
            {
                if (tc.IsOpen)
                {
                    tc.WriteLine("FREQ:PSC:CENT " + PScanFreqCentr.ToString("G29").Replace(',', '.'));
                    PScanFreqCentr = decimal.Parse(tc.Query("FREQ:PSC:CENT?").Replace('.', ','));
                }
            }
            #region Exception
            catch (Exception exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "RsReceiver", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            #endregion
            finally { TelnetDM -= SetPScanFreqCentr; }
        }

        private void SetPScanFreqSpan()
        {
            try
            {
                if (tc.IsOpen)
                {
                    tc.WriteLine("FREQ:PSC:SPAN " + PScanFreqSpan.ToString("G29").Replace(',', '.'));
                    PScanFreqSpan = decimal.Parse(tc.Query("FREQ:PSC:SPAN?").Replace('.', ','));
                }
            }
            #region Exception
            catch (Exception exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "RsReceiver", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            #endregion
            finally { TelnetDM -= SetPScanFreqSpan; }
        }

        private void SetPScanRun()
        {
            try
            {
                if (tc.IsOpen)
                {
                    tc.WriteLine("INIT");
                }
            }
            #region Exception
            catch (Exception exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "RsReceiver", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            #endregion
            finally { TelnetDM -= SetPScanRun; }
        }
        private void SetPScanStop()
        {
            try
            {
                if (tc.IsOpen)
                {
                    tc.WriteLine("ABOR");
                }
            }
            #region Exception
            catch (Exception exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "RsReceiver", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            #endregion
            finally { TelnetDM -= SetPScanStop; }
        }

        /// <summary>
        /// Установка шаг просмотра STEP PSCAN
        /// </summary>
        private void SetPScanStepIndex()
        {
            try
            {
                if (tc.IsOpen)
                {
                    tc.WriteLine("PSC:STEP " + UniqueData.PSCANStepBW[PScanStepInd].ToString().Replace(',', '.'));
                }
            }
            #region Exception
            catch (Exception exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "RsReceiver", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            #endregion
            finally { TelnetDM -= SetPScanStepIndex; }
        }

        private void SetPScanFFTMode()
        {
            try
            {
                if (tc.IsOpen)
                {
                    tc.WriteLine("CALC:PSC:AVER:TYPE " + PScanFFTMode.Parameter);
                }
            }
            #region Exception
            catch (Exception exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "RsReceiver", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            #endregion
            finally { TelnetDM -= SetPScanFFTMode; }
        }

        private void SetPScanRefLevel()
        {
            try
            {
                if (tc.IsOpen)
                {
                    if (UniqueData.InstrModel == "EM100" || UniqueData.InstrModel == "PR100")
                    {
                        tc.WriteLine("DISPlay:PSCAN:LEVel:REFerence " + PScanRefLevel.ToString("G29").Replace(',', '.'));
                    }
                    else if (UniqueData.InstrModel == "EB500" || UniqueData.InstrModel == "ESMD")
                    {
                        //tc.WriteLine("DISPlay:PSCAN:LEVel:REFerence " + PScanRefLevel.ToString("G29").Replace(',', '.'));
                    }
                }
            }
            #region Exception
            catch (Exception exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "RsReceiver", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            #endregion
            finally { TelnetDM -= SetPScanRefLevel; }
        }

        private void SetPScanRangeLevel()
        {
            try
            {
                if (tc.IsOpen)
                {
                    if (UniqueData.InstrModel == "EM100" || UniqueData.InstrModel == "PR100")
                    {
                        tc.WriteLine("DISPlay:PSCAN:LEVel:RANGe " + PScanRangeLevel.ToString("G29").Replace(',', '.'));
                        PScanRangeLevel = decimal.Parse(tc.Query("DISPlay:PSCAN:LEVel:RANGe?").Replace('.', ','));
                    }
                    else if (UniqueData.InstrModel == "EB500" || UniqueData.InstrModel == "ESMD")
                    {
                        //tc.WriteLine("DISPlay:PSCAN:LEVel:RANGe " + PScanRangeLevel.ToString("G29").Replace(',', '.'));
                        //PScanRangeLevel = decimal.Parse(tc.Query("DISPlay:PSCAN:LEVel:RANGe?").Replace('.', ','));
                    }
                }
            }
            #region Exception
            catch (Exception exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "RsReceiver", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            #endregion
            finally { TelnetDM -= SetPScanRangeLevel; }
        }



        #endregion PSCAN



        #endregion private Methods




        long GSMBandMeasTicks = 0;
        bool GSMBandMeas = false;
        GSMBandMeas GSMBandMeasSelected = new Equipment.GSMBandMeas() { };
        public void SetMeasMon()
        {
            #region
            if (tc.IsOpen && MainWindow.db_v2.MeasMon.Data.Count > 0)// && MainWindow.gps.GPSIsValid)
            {
                try
                {
                    if (MeasMonItem != null && MeasMonItem.AllTraceCountToMeas == MeasMonItem.AllTraceCount) //if (MeasTraceCount < 0 && MainWindow.db.MonMeas.Count > 0)
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

                                    //int index = 0;
                                    //for (int p = 0; p < UniqueData.SweepPointArr.Length; p++)
                                    //{
                                    //    if (index == 0 && UniqueData.SweepPointArr[p] >= GSMBandMeasSelected.TracePoints)
                                    //    { index = p; }

                                    //}
                                    ////SetFreqSettingsToMeasMon(MeasMonItem.FreqDN, MeasMonItem.MeasSpan, 18);//23
                                    //SetFreqSettingsToMeasMon((GSMBandMeasSelected.Stop + GSMBandMeasSelected.Start) / 2, GSMBandMeasSelected.Stop - GSMBandMeasSelected.Start, Tra);
                                    #endregion

                                }
                            }
                            #endregion
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
                                    if (MainWindow.db_v2.MeasMon.Data[i].Techonology == "GSM" && MainWindow.db_v2.MeasMon.Data[i].Power > TSMxReceiver.DetectionLevelGSM)
                                    {
                                        for (int j = 0; j < IdentificationData.GSM.BTS.Count; j++)
                                        {
                                            //тут проверяем купатыся чы некупатыся
                                            if (IdentificationData.GSM.BTS[j].BSIC == MainWindow.db_v2.MeasMon.Data[i].TechSubInd &&
                                                IdentificationData.GSM.BTS[j].GCID == MainWindow.db_v2.MeasMon.Data[i].GCID)
                                            {
                                                Ind = MainWindow.db_v2.MeasMon.Data[i].AllTraceCount;
                                                ii = i;
                                            }
                                        }
                                    }
                                    else if (MainWindow.db_v2.MeasMon.Data[i].Techonology == "UMTS" && MainWindow.db_v2.MeasMon.Data[i].Power > TSMxReceiver.DetectionLevelUMTS)
                                    {
                                        for (int j = 0; j < IdentificationData.UMTS.BTS.Count; j++)
                                        {
                                            if (IdentificationData.UMTS.BTS[j].SC == MainWindow.db_v2.MeasMon.Data[i].TechSubInd &&
                                                IdentificationData.UMTS.BTS[j].GCID == MainWindow.db_v2.MeasMon.Data[i].GCID)
                                            {
                                                Ind = MainWindow.db_v2.MeasMon.Data[i].AllTraceCount;
                                                ii = i;
                                            }
                                        }
                                    }
                                    else if (MainWindow.db_v2.MeasMon.Data[i].Techonology == "LTE" && MainWindow.db_v2.MeasMon.Data[i].Power > TSMxReceiver.DetectionLevelLTE)
                                    {
                                        for (int j = 0; j < IdentificationData.LTE.BTS.Count; j++)
                                        {
                                            if (IdentificationData.LTE.BTS[j].PCI == MainWindow.db_v2.MeasMon.Data[i].TechSubInd &&
                                                IdentificationData.LTE.BTS[j].GCID == MainWindow.db_v2.MeasMon.Data[i].GCID)
                                            {
                                                Ind = MainWindow.db_v2.MeasMon.Data[i].AllTraceCount;
                                                ii = i;
                                            }
                                        }
                                    }
                                    else if (MainWindow.db_v2.MeasMon.Data[i].Techonology == "CDMA" && MainWindow.db_v2.MeasMon.Data[i].Power > TSMxReceiver.DetectionLevelCDMA)
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
                                    else if (MainWindow.db_v2.MeasMon.Data[i].Techonology == "WIMAX")
                                    {
                                        //double lev = App.Sett.MeasMons_Settings.MeasMons.Where(x => x.Techonology.ToString() == MainWindow.db_v2.MeasMon.Data[i].Techonology).First().DetectionLevel;
                                        //foreach (TopNWiMax twimax in MainWindow.RCR.WiMax)
                                        //{
                                        //    if (twimax.GCID == MainWindow.db_v2.MeasMon.Data[i].GCID && twimax.RSSI > lev)
                                        //    {
                                        //        Ind = MainWindow.db_v2.MeasMon.Data[i].AllTraceCount;
                                        //        ii = i;
                                        //    }
                                        //}
                                    }
                                    else if (MainWindow.db_v2.MeasMon.Data[i].Techonology == "WRLS")
                                    {
                                        //decimal lev = App.Sett.MeasMons_Settings.MeasMons.Where(x => x.Techonology.ToString() == MainWindow.db_v2.MonMeas[i].Techonology).First().DetectionLevel;
                                        //foreach (WRLSBTSData twifi in IdentificationData.WRLSBTS)
                                        //{
                                        //    if (twifi.GCID == MainWindow.db_v2.MonMeas[i].GCID && twifi.Level > lev)
                                        //    {
                                        //        Ind = MainWindow.db_v2.MonMeas[i].AllTraceCount;
                                        //        ii = i;
                                        //    }
                                        //}
                                    }
                                    else if (MainWindow.db_v2.MeasMon.Data[i].Techonology == "UHF")
                                    {
                                        ////decimal lev = App.Sett.MeasMons_Settings.MeasMons.Where(x => x.Techonology.ToString() == MainWindow.db_v2.MonMeas[i].Techonology).First().DetectionLevel;
                                        //foreach (UHFBTSData tuhf in MainWindow.IdfData.UHFBTS)
                                        //{
                                        //    if (tuhf.CallSign == MainWindow.db_v2.MeasMon.Data[i].GCID/* && tuhf.Power > lev*/)
                                        //    {
                                        //        Ind = MainWindow.db_v2.MeasMon.Data[i].AllTraceCount;
                                        //        ii = i;
                                        //    }
                                        //}
                                    }
                                }
                            }
                            #endregion
                            if (ii > -1)
                            {
                                MeasMonItem.ThisToMeas = false;
                                MainWindow.db_v2.MeasMon.Data[ii].ThisToMeas = true;
                                DB.MeasData temp = MainWindow.db_v2.MeasMon.Data[ii];

                                string tec = MainWindow.db_v2.MeasMon.Data[ii].Techonology;
                                //&& MainWindow.db_v2.MonMeas[Ind].Power > MainWindow.db_v2.MonMeas.Where(x => x.FreqDN == (MainWindow.db_v2.MonMeas[Ind].FreqDN +200000)).First().Power + 35 && MainWindow.db_v2.MonMeas[Ind].Power > MainWindow.db_v2.MonMeas.Where(x => x.FreqDN == (MainWindow.db_v2.MonMeas[Ind].FreqDN - 200000)).First().Power + 35
                                if (tec == "GSM")
                                {
                                    #region
                                    if (IdentificationData.GSM.BTS != null && IdentificationData.GSM.BTS.Count > 0)
                                    {
                                        ////ObservableCollection<GSMBTSData> gsm = IdentificationData.GSMBTS;
                                        //int gsmind = -1;
                                        //if (IdentificationData.GSM.BTS != null)
                                        //    for (int i = 0; i < IdentificationData.GSM.BTS.Count; i++)
                                        //    {
                                        //        if (IdentificationData.GSM.BTS[i].GCID == temp.GCID && IdentificationData.GSM.BTS[i].FreqDn == temp.FreqDN) { gsmind = i; }
                                        //    }
                                        //if (gsmind > -1 && IdentificationData.GSM.BTS[gsmind].Power > App.Sett.MeasMons_Settings.MeasMons.Where(x => x.Techonology == DB.Technologys.GSM).First().DetectionLevel)
                                        //{
                                        MeasMonItem = temp;
                                        MeasMonItem.AllTraceCountToMeas = MeasMonItem.AllTraceCount + MeasTraceCountOnFreq;
                                        //MeasTraceCount = MeasMonItem.AllTraceCount + MeasTraceCountOnFreq;
                                        SetSettingsToMeasMon(MeasMonItem.FreqDN, MeasMonItem.SpecData.FreqSpan, 0);
                                        //MeasTraceCountOnFreq = 10;
                                        MeasMonTimeMeas = DateTime.Now.Ticks;
                                        //}
                                    }
                                    #endregion
                                }
                                else if (tec == "UMTS")
                                {
                                    #region
                                    if (IdentificationData.UMTS.BTS != null && IdentificationData.UMTS.BTS.Count > 0)
                                    {
                                        ////ObservableCollection<UMTSBTSData> umts = MainWindow.IdfData.UMTSBTS;
                                        //int umtsind = -1;
                                        //if (IdentificationData.UMTS.BTS != null)
                                        //    for (int i = 0; i < IdentificationData.UMTS.BTS.Count; i++)
                                        //    {
                                        //        if (IdentificationData.UMTS.BTS[i].GCID == temp.GCID && IdentificationData.UMTS.BTS[i].FreqDn == temp.FreqDN) { umtsind = i; }
                                        //    }
                                        //if (umtsind > -1 && IdentificationData.UMTS.BTS[umtsind].RSCP > App.Sett.MeasMons_Settings.MeasMons.Where(x => x.Techonology == DB.Technologys.UMTS).First().DetectionLevel)
                                        //{
                                        MeasMonItem = temp;
                                        MeasMonItem.AllTraceCountToMeas = MeasMonItem.AllTraceCount + MeasTraceCountOnFreq;
                                        //MeasTraceCount = MeasMonItem.AllTraceCount + MeasTraceCountOnFreq;
                                        SetSettingsToMeasMon(MeasMonItem.FreqDN, MeasMonItem.SpecData.FreqSpan, 0);
                                        //MeasTraceCountOnFreq = 10;
                                        MeasMonTimeMeas = DateTime.Now.Ticks;
                                        //}
                                    }
                                    #endregion
                                }
                                else if (tec == "CDMA")
                                {
                                    #region
                                    if (IdentificationData.CDMA.BTS != null && IdentificationData.CDMA.BTS.Count > 0)
                                    {
                                        ////ObservableCollection<CDMABTSData> cdma = MainWindow.IdfData.CDMABTS;
                                        //int cdmaind = -1;
                                        //if (IdentificationData.CDMA.BTS != null)
                                        //    for (int i = 0; i < IdentificationData.CDMA.BTS.Count; i++)
                                        //    {
                                        //        if (IdentificationData.CDMA.BTS[i].GCID == temp.GCID && IdentificationData.CDMA.BTS[i].FreqDn == temp.FreqDN) { cdmaind = i; }
                                        //    }
                                        //if (cdmaind > -1 && IdentificationData.CDMA.BTS[cdmaind].RSCP > App.Sett.MeasMons_Settings.MeasMons.Where(x => x.Techonology == DB.Technologys.CDMA).First().DetectionLevel)
                                        //{
                                        MeasMonItem = temp;
                                        MeasMonItem.AllTraceCountToMeas = MeasMonItem.AllTraceCount + MeasTraceCountOnFreq;
                                        //MeasTraceCount = MeasMonItem.AllTraceCount + MeasTraceCountOnFreq;
                                        SetSettingsToMeasMon(MeasMonItem.FreqDN, MeasMonItem.SpecData.FreqSpan, 0);
                                        //MeasTraceCountOnFreq = 10;
                                        MeasMonTimeMeas = DateTime.Now.Ticks;
                                        //}
                                    }
                                    #endregion
                                }
                                else if (tec == "WIMAX")
                                {
                                    #region
                                    if (MainWindow.RCR.WiMax != null && MainWindow.RCR.WiMax.Count > 0)
                                    {
                                        //ObservableCollection<TopNWiMax> wimax = MainWindow.RCR.WiMax;
                                        //int wimaxind = -1;
                                        //if (wimax != null)
                                        //    for (int i = 0; i < wimax.Count; i++)
                                        //    {
                                        //        if (wimax[i].GCID == temp.GCID && wimax[i].Channel.FreqDn == temp.FreqDN) { wimaxind = i; }
                                        //    }
                                        //if (wimaxind > -1 && wimax[wimaxind].RSSI > App.Sett.MeasMons_Settings.MeasMons.Where(x => x.Techonology == DB.Technologys.WIMAX).First().DetectionLevel)
                                        //{
                                        MeasMonItem = temp;
                                        MeasMonItem.AllTraceCountToMeas = MeasMonItem.AllTraceCount + MeasTraceCountOnFreq;
                                        //MeasTraceCount = MeasMonItem.AllTraceCount + MeasTraceCountOnFreq;
                                        SetSettingsToMeasMon(MeasMonItem.FreqDN, MeasMonItem.SpecData.FreqSpan, 0);
                                        //MeasTraceCountOnFreq = 10;
                                        MeasMonTimeMeas = DateTime.Now.Ticks;
                                        //}
                                    }
                                    #endregion
                                }
                                else if (tec == "WRLS")
                                {
                                    #region
                                    //if (IdentificationData.WRLSBTS != null && IdentificationData.WRLSBTS.Count > 0)
                                    //{
                                    //    //ObservableCollection<WRLSBTSData> wrls = IdentificationData.WRLSBTS;
                                    //    int wrlsind = -1;
                                    //    if (IdentificationData.WRLSBTS != null)
                                    //        for (int i = 0; i < IdentificationData.WRLSBTS.Count; i++)
                                    //        {
                                    //            if (IdentificationData.WRLSBTS[i].GCID == temp.GCID/* && wrls[i].FreqCentr == temp.FreqDN*/) { wrlsind = i; }
                                    //        }
                                    //    if (wrlsind > -1 && IdentificationData.WRLSBTS[wrlsind].Level > App.Sett.MeasMons_Settings.MeasMons.Where(x => x.Techonology == DB.Technologys.WRLS).First().DetectionLevel)
                                    //    {
                                    //        MeasMonItem = temp;
                                    //        MeasMonItem.AllTraceCountToMeas = MeasMonItem.AllTraceCount + MeasTraceCountOnFreq;
                                    //        //MeasTraceCountOnFreq = 10;
                                    //        //MeasTraceCount = MeasMonItem.AllTraceCount + MeasTraceCountOnFreq;
                                    //        SetSettingsToMeasMon(MeasMonItem.FreqDN, MeasMonItem.SpecData.FreqSpan);
                                    //        MeasMonTimeMeas = DateTime.Now.Ticks;
                                    //    }
                                    //}
                                    #endregion
                                }
                                else if (tec == "UHF")
                                {
                                    #region
                                    if (MainWindow.IdfData.UHFBTS != null && MainWindow.IdfData.UHFBTS.Count > 0)
                                    {
                                        //ObservableCollection<GSMBTSData> gsm = MainWindow.IdfData.GSMBTS;
                                        //int gsmind = -1;
                                        //if (gsm != null)
                                        //    for (int i = 0; i < gsm.Count; i++)
                                        //    {
                                        //        if (gsm[i].GCID == temp.GCID && gsm[i].Channel.FreqDn == temp.FreqDN) { gsmind = i; }
                                        //    }
                                        //if (gsmind > -1 && gsm[gsmind].Power > App.Sett.MeasMons_Settings.MeasMons.Where(x => x.Techonology == DB.Technologys.GSM).First().DetectionLevel)
                                        //{
                                        MeasMonItem = temp;
                                        MeasMonItem.AllTraceCountToMeas = MeasMonItem.AllTraceCount + MeasTraceCountOnFreq;
                                        //MeasTraceCount = MeasMonItem.AllTraceCount + MeasTraceCountOnFreq;
                                        SetSettingsToMeasMon(MeasMonItem.FreqDN, MeasMonItem.SpecData.FreqSpan, 0);
                                        //MeasTraceCountOnFreq = 10;
                                        MeasMonTimeMeas = DateTime.Now.Ticks;
                                        //}
                                    }
                                    #endregion
                                }
                                MeasMonItem.device_meas = device_meas;
                                //Thread.Sleep(10);
                            }
                            #endregion
                        }
                    }
                }
                #region Exception
                catch (Exception exp)
                {
                    MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "RsReceiver", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
                }
                #endregion
            }

            #endregion
            //TelnetDM -= SetMeasMon;
        }

        private void MMSCoor_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "LatitudeDouble" || e.PropertyName == "LongitudeDouble")
            {
                if (MainWindow.gps.LatitudeDecimal != 0 && MainWindow.gps.LongitudeDecimal != 0)
                {
                    //TelnetDM += AddUHFPlanItemsToMeas;
                    //App.Current.Dispatcher.BeginInvoke((Action)(() =>
                    //{

                    //    //((MainWindow)App.Current.MainWindow).Message = MainWindow.gps.LatitudeStr + "  " + MainWindow.gps.LongitudeStr;
                    //}));
                }
            }

        }
        private void AddUHFPlanItemsToMeas()
        {
            #region в просмотр
            #region ищем в планах новые

            #endregion
            int UHFBTSCountNDP = 0;
            int UHFBTSCountNPE = 0;
            int UHFBTSCountWithGCID = 0;
            foreach (UHFBTSData ubd in MainWindow.IdfData.UHFBTS)
            {
                if (ubd.FullData == true && ubd.Identifier_Find == 1) UHFBTSCountNDP++;
                if (ubd.FullData == true && ubd.Identifier_Find == 2 && ubd.FreqCheck_Find == 1) UHFBTSCountNPE++;
                if (ubd.FullData == true) UHFBTSCountWithGCID++;
            }
            MainWindow.IdfData.UHFBTSCountNDP = UHFBTSCountNDP;
            MainWindow.IdfData.UHFBTSCountNPE = UHFBTSCountNPE;
            MainWindow.IdfData.UHFBTSCountWithGCID = UHFBTSCountWithGCID;
            #endregion

            #region в измерения
            //Settings.MeasMonTech_Set set = App.Sett.MeasMons_Settings.MeasMons.Where(x => x.Techonology == DB.Technologys.UHF).First();
            //double DetectionLevel = set.DetectionLevel;
            //double NdbLevel = set.Data[0].NdBLevel;
            //decimal NdbBW = set.Data[0].BWLimit;
            //decimal FreqDeviationPPM = set.Data[0].DeltaFreqLimit;
            //App.Current.Dispatcher.BeginInvoke((Action)(() =>
            //{
            //    for (int i = 0; i < MainWindow.IdfData.UHFBTS.Count; i++)
            //    {

            //        #region
            //        // найдено ли в измерениях уже
            //        bool FindInMeasData = false;
            //        //if (MainWindow.db_v2.MonMeas.Count > 0)
            //        //{
            //        //    for (int y = 0; y < MainWindow.db_v2.MonMeas.Count; y++)
            //        //    {
            //        //        if (MainWindow.db_v2.MonMeas[y].Techonology == "UHF" && MainWindow.db_v2.MonMeas[y].FreqDN == MainWindow.IdfData.UHFBTS[i].FreqDn && MainWindow.db_v2.MonMeas[y].PlanFreq_ID == MainWindow.IdfData.UHFBTS[i].PlanFreq_ID && MainWindow.db_v2.MonMeas[y].GCID == MainWindow.IdfData.UHFBTS[i].CallSign)
            //        //        {
            //        //            //MainWindow.IdfData.UHFBTS[i].Power = mg.Power;
            //        //            FindInMeasData = true;
            //        //            //if (MainWindow.IdfData.GSMBTS[i].Power > DetectionLevel)
            //        //            //    mg.LastSeenSignal = DateTime.Now;//заменить на время из GPS с учетом часового пояса
            //        //        }
            //        //    }
            //        //}
            //        //if (FindInMeasData == false/* && MainWindow.IdfData.UHFBTS[i].Power > DetectionLevel*/)
            //        //{
            //        //    #region
            //        //    bool FindInResulData = false;
            //        //    //ищем в результатах
            //        //    TracePoint[] points = null;
            //        //    decimal tracestep = 0;
            //        //    bool LevelUnit = false;
            //        //    int MarkerLevelIndex = -1;
            //        //    int T1LevelIndex = -1;
            //        //    int T2LevelIndex = -1;
            //        //    DateTime MEASURMENT_DATETIME = DateTime.Now;//заменить на время из GPS с учетом часового пояса
            //        //    DateTime LastMeasDateTime = DateTime.MinValue;
            //        //    decimal MEASURMENT_DURATION = 0;
            //        //    decimal DeltaFreqMeasured = 0;
            //        //    decimal ChanelBWMeasured = 0;
            //        //    bool findinresultplan = false;
            //        //    DB.MeasData result = new DB.MeasData()
            //        //    {
            //        //        MeasGuid = Guid.NewGuid(),
            //        //        Techonology = DB.Technologys.UHF.ToString(),
            //        //        StandartSubband = "",
            //        //        FreqUP = MainWindow.IdfData.UHFBTS[i].FreqUp,
            //        //        FreqDN = MainWindow.IdfData.UHFBTS[i].FreqDn,
            //        //        MeasSpan = 500000,
            //        //        MarPeakBW = 200000,
            //        //        NdBBWMax = 400000,
            //        //        NdBBWMin = 150000,
            //        //        ChannelN = 0,
            //        //        GCID = MainWindow.IdfData.UHFBTS[i].CallSign,
            //        //        UCRFGCID = MainWindow.IdfData.UHFBTS[i].CallSign,
            //        //        TechSubInd = 0,
            //        //        FullData = MainWindow.IdfData.UHFBTS[i].FullData,
            //        //        Power = MainWindow.IdfData.UHFBTS[i].Power,
            //        //        TraceCount = 0,
            //        //        LevelUnit = LevelUnit,
            //        //        Trace = points,
            //        //        TraceStep = tracestep,
            //        //        LastSeenSignal = DateTime.Now,
            //        //        MeasStop = LastMeasDateTime,
            //        //        PlanFreq_ID = MainWindow.IdfData.UHFBTS[i].PlanFreq_ID,
            //        //        PLAN_ID = MainWindow.IdfData.UHFBTS[i].Plan_ID,
            //        //        MarkerInd = MarkerLevelIndex,
            //        //        MarkerT1Ind = T1LevelIndex,
            //        //        MarkerT2Ind = T2LevelIndex,
            //        //        MeasStart = MEASURMENT_DATETIME,
            //        //        MeasDuration = MEASURMENT_DURATION,
            //        //        STN_FRQ_ID = 0,
            //        //        FreqCheck_Find = MainWindow.IdfData.UHFBTS[i].FreqCheck_Find,
            //        //        Identifier_Find = MainWindow.IdfData.UHFBTS[i].Identifier_Find,
            //        //        NdBLevel = NdbLevel,
            //        //        ChanelBWLimit = NdbBW,
            //        //        ChanelBWMeasured = ChanelBWMeasured,
            //        //        DeltaFreqLimit = FreqDeviationPPM,
            //        //        DeltaFreqMeasured = DeltaFreqMeasured,
            //        //    };
            //        //    if (FindInResulData == false)
            //        //    {
            //        //        result.LevelUnit = false; //result.Trace = null;
            //        //    }


            //        //    MainWindow.db_v2.MonMeas.Add(result);


            //        //    #endregion
            //        //}

            //        #endregion

            //    }
            //}));
            #endregion
            //MainWindow.IdfData.GSMBTSCount = GSMBTSfromDev.Count;
            TelnetDM -= AddUHFPlanItemsToMeas;
        }
        private void SetStartMeasMonSettings()
        {
            try
            {
                if (tc.IsOpen)
                {
                    if (UniqueData.InstrModel == "EM100" || UniqueData.InstrModel == "PR100")
                    {
                        MeasMode = false;
                        MeasTime = 0.0005m;
                        for (int i = 0; i < UniqueData.Detectors.Count; i++) { if ("PEAK" == UniqueData.Detectors[i].UI) { DetectorInd = i; } }
                        for (int i = 0; i < UniqueData.FFTModes.Count; i++) { if ("CLEAR WRITE" == UniqueData.FFTModes[i].UI) { FFMFFTMode = UniqueData.FFTModes[i]; PScanFFTMode = UniqueData.FFTModes[i]; } }
                        Trace1TypeIndex = 0;//отключаемвлияние постобработки
                        Trace2TypeIndex = 0;//отключаемвлияние постобработки чтоб не жрало проц в путую
                        Trace3TypeIndex = 0;//отключаемвлияние постобработки чтоб не жрало проц в путую
                        AFCState = false;
                        ATTFixState = false;
                        string send =
                            ":MEAS:MODE PER" + //переодический режим, т.к. быстрее
                            ":;MEAS:TIME " + MeasTime.ToString("G29").Replace(',', '.') +//время измерения
                            ":;DET " + Detector.Parameter + // детоктор в пик 
                            ":;CALC:IFP:AVER:TYPE " + FFMFFTMode.Parameter + //тип трейса
                            ":;CALC:PSC:AVER:TYPE " + PScanFFTMode.Parameter + //тип трейса
                            ":;FREQ:AFC 0" + // выключили afc
                            ":;INPut:ATTenuation:STATe 0" + //выключили аттенюатор
                            "";
                        tc.WriteLine(send);
                    }
                    else if (UniqueData.InstrModel.Contains("EB5") || UniqueData.InstrModel.Contains("DDF2") || UniqueData.InstrModel == "ESMD")
                    {
                        FFMStepAuto = true;
                        MeasMode = false;
                        MeasTime = 0.0005m;
                        for (int i = 0; i < UniqueData.Detectors.Count; i++) { if ("PEAK" == UniqueData.Detectors[i].UI) { DetectorInd = i; } }
                        for (int i = 0; i < UniqueData.FFTModes.Count; i++) { if ("CLEAR WRITE" == UniqueData.FFTModes[i].UI) { FFMFFTMode = UniqueData.FFTModes[i]; PScanFFTMode = UniqueData.FFTModes[i]; } }
                        Trace1TypeIndex = 0;//отключаемвлияние постобработки
                        Trace2TypeIndex = 0;//отключаемвлияние постобработки чтоб не жрало проц в путую
                        Trace3TypeIndex = 0;//отключаемвлияние постобработки чтоб не жрало проц в путую
                        AFCState = false;
                        ATTAutoState = true;
                        string send = ":CALC:IFP:STEP:AUTO 1" + //в FFM автоматическая полоса RBW (всегда дает 1601 точку)
                            ":;MEAS:MODE PER" + //переодический режим, т.к. быстрее
                            ":;MEAS:TIME " + MeasTime.ToString("G29").Replace(',', '.') +//время измерения
                            ":;DET " + Detector.Parameter + // детоктор в пик 
                            ":;CALC:IFP:AVER:TYPE " + FFMFFTMode.Parameter + //тип трейса
                            ":;CALC:PSC:AVER:TYPE " + PScanFFTMode.Parameter + //тип трейса
                            ":;FREQ:AFC 0" + // выключили afc
                            ":;INP:ATT:AUTO 1" + //включили аттенюатор в автоматический режим (так лучше)
                            "";
                        tc.WriteLine(send);
                    }
                }
            }
            #region Exception
            catch (Exception exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "RsReceiver", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            #endregion
            finally { TelnetDM -= SetStartMeasMonSettings; }

        }

        /// <summary>
        /// установка частоты мониторинга и спана
        /// </summary>
        /// <param name="freqCentr">если меньше/равно 10МГц то FFM если больше то PSCAN</param>
        /// <param name="span">если меньше/равно 10МГц полоса согласно FFMSpanBW, если больше 10МГц то какая угодно </param>
        public void SetSettingsToMeasMon(decimal freqCentr, decimal span, decimal RBW)
        {
            if (tc.IsOpen)
            {
                try
                {
                    #region set Mode
                    LevelUnitInd = 1;
                    if (UniqueData.InstrModel == "EM100" || UniqueData.InstrModel == "PR100")
                    {
                        #region
                        if (span > 10000000 && Mode.Mode == "FFM")
                        {
                            Mode = UniqueData.Modes.Where(x => x.Mode == "PSCAN").First();
                            SetStreamingMode();
                        }
                        else if (span <= 10000000 && Mode.Mode == "PSCAN")
                        {
                            tc.WriteLine("ABOR");
                            Mode = UniqueData.Modes.Where(x => x.Mode == "FFM").First();
                            SetStreamingMode();
                        }
                        if (span > 10000000)
                        {
                            tc.WriteLine("ABOR");
                            PScanFreqCentr = freqCentr;
                            PScanFreqSpan = span;
                            //Temp1 = "FREQ:PSC:CENT " + ((Int64)freqCentr).ToString();
                            tc.WriteLine("FREQ:PSC:CENT " + ((Int64)freqCentr).ToString());// + ";FREQ:PSC:SPAN " + ((Int64)span).ToString() + ";");
                            tc.WriteLine("FREQ:PSC:SPAN " + ((Int64)span).ToString());
                            int sp = 0;
                            if (span == 20000000) { sp = 12500; }
                            else if (span == 40000000) { sp = 25000; }
                            else if (span == 80000000) { sp = 50000; }
                            else if (span == 160000000) { sp = 100000; }
                            tc.WriteLine("PSC:STEP " + sp.ToString());
                            PScanStepInd = System.Array.IndexOf(UniqueData.PSCANStepBW, (decimal)sp);
                            tc.WriteLine("INIT");
                        }
                        else if (span <= 10000000)
                        {

                            tc.WriteLine(":FREQ " + ((Int64)freqCentr).ToString() + ";FREQ:SPAN " + ((Int64)span).ToString() + ";");
                        }
                        #endregion
                    }
                    else if (UniqueData.InstrModel.Contains("EB5") || UniqueData.InstrModel.Contains("ESMD") || UniqueData.InstrModel.Contains("DDF2"))
                    {
                        if (span > 20000000 && Mode.Mode == "FFM")
                        {
                            Mode = UniqueData.Modes.Where(x => x.Mode == "PSCAN").First();
                            SetStreamingMode();
                        }
                        else if (span <= 20000000 && Mode.Mode == "PSCAN")
                        {
                            tc.WriteLine("ABOR");
                            Mode = UniqueData.Modes.Where(x => x.Mode == "FFM").First();
                            SetStreamingMode();
                        }
                        if (span > 20000000)
                        {
                            tc.WriteLine("ABOR");
                            PScanFreqCentr = freqCentr;
                            PScanFreqSpan = span;
                            //Temp1 = "FREQ:PSC:CENT " + ((Int64)freqCentr).ToString();
                            tc.WriteLine("FREQ:PSC:CENT " + ((Int64)freqCentr).ToString());// + ";FREQ:PSC:SPAN " + ((Int64)span).ToString() + ";");
                            tc.WriteLine("FREQ:PSC:SPAN " + ((Int64)span).ToString());
                            int sp = 0;
                            if (span > 20000000 && span <= 30000000) { sp = 12500; }
                            else if (span > 30000000 && span <= 50000000) { sp = 25000; }
                            else if (span > 50000000 && span <= 100000000) { sp = 50000; }
                            else if (span > 100000000 && span <= 200000000) { sp = 100000; }
                            tc.WriteLine("PSC:STEP " + sp.ToString());
                            PScanStepInd = System.Array.IndexOf(UniqueData.PSCANStepBW, (decimal)sp);
                            tc.WriteLine("INIT");
                        }
                        else if (span <= 20000000)
                        {

                            tc.WriteLine(":FREQ " + ((Int64)freqCentr).ToString() + ";FREQ:SPAN " + ((Int64)span).ToString() + ";");
                        }
                    }
                    #endregion
                    //span
                    //SpanInd = System.Array.IndexOf(SpanBW, Span);
                    //tc.WriteLine("FREQ:SPAN " + ((Int64)Span).ToString());
                }
                #region Exception
                catch (Exception exp)
                {
                    MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "RsReceiver", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
                }
                #endregion
            }
            //TelnetDM -= SetMeasMon;
        }
        private void MeasSygma(tracepoint[] trace, decimal BW)
        {
            if (trace.Length > 0)
            {//DVBSygma = 0;
                double m = 0;
                double t = 0;
                int l = 0;
                try
                {
                    for (int i = 0; i < trace.Length; i++)
                    {
                        if (trace[i].freq > (FreqCentr - BW / 2) && trace[i].freq < (FreqCentr + BW / 2))
                        {
                            m += trace[i].level;
                            l++;
                        }
                    }
                    m /= l;
                    for (int i = 0; i < trace.Length; i++)
                    {
                        if (trace[i].freq > (FreqCentr - BW / 2) && trace[i].freq < (FreqCentr + BW / 2))
                        {
                            t += Math.Pow(trace[i].level - m, 2);
                        }
                    }
                }
                #region Exception
                catch (Exception exp)
                {
                    MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "RsReceiver", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
                }
                #endregion
                finally { /*DVBSygma = (decimal)Math.Sqrt((double)(t / (l - 1)));*/ }

            }
        }
        /// <summary>
        /// меряет сигму в полосе сигнала
        /// </summary>
        /// <param name="FreqCentr">центральная частота сигнала</param>
        /// <param name="SignalSpan">полоса сигнала</param>
        /// <returns></returns>
        private decimal MeasSygma(tracepoint[] trace, decimal FreqCentr, decimal SignalSpan)
        {
            decimal Sygma = 0;
            if (trace.Length > 0)
            {//DVBSygma = 0;
                double m = 0;
                int l = 0;
                for (int i = 0; i < trace.Length; i++)
                {
                    if (trace[i].freq > (FreqCentr - SignalSpan / 2) && trace[i].freq < (FreqCentr + SignalSpan / 2))
                    {
                        m += trace[i].level;
                        l++;
                    }
                }
                m /= l;
                double t = 0;
                for (int i = 0; i < trace.Length; i++)
                {
                    if (trace[i].freq > (FreqCentr - SignalSpan / 2) && trace[i].freq < (FreqCentr + SignalSpan / 2))
                    {
                        t += Math.Pow(trace[i].level - m, 2);
                    }
                }
                Sygma = (decimal)Math.Sqrt((t / (l - 1)));
            }
            return Sygma;
        }
        /// <summary>
        /// незнаю нафига но пригодится
        /// </summary>
        /// <param name="Level"></param>
        /// <param name="BW"></param>
        /// <returns></returns>
        private decimal MeasMarkerPower(decimal Level, decimal BW)
        {
            decimal t = Math.Round(Level + 10 * (decimal)Math.Log10((double)(BW / UniqueData.FFMStepBW[FFMStepInd])), 2);
            return t;
        }









        /// <summary>
        /// Установка центральной частоты демодуляции
        /// </summary>
        public void SetDemodFreq()
        {
            if (tc.IsOpen)
            {
                try
                {
                    tc.WriteLine("FREQ:DEM " + FFMDemodFreq.ToString("G29").Replace(',', '.'));
                }
                #region Exception
                catch (Exception exp)
                {
                    MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "RsReceiver", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
                }
                #endregion
            }
            TelnetDM -= SetDemodFreq;
        }
        public void SetDemodFreqUp()
        {
            if (tc.IsOpen)
            {
                try
                {
                    FFMDemodFreq += FFMFreqSpan / 100;
                    tc.WriteLine("FREQ:DEM " + FFMDemodFreq.ToString("G29").Replace(',', '.'));
                    FFMDemodFreq = decimal.Parse(tc.Query("FREQ:DEM?").Replace('.', ','));
                }
                #region Exception
                catch (Exception exp)
                {
                    MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "RsReceiver", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
                }
                #endregion
            }
            TelnetDM -= SetDemodFreqUp;
        }
        public void SetDemodFreqDn()
        {
            if (tc.IsOpen)
            {
                try
                {
                    FFMDemodFreq -= FFMFreqSpan / 100;
                    tc.WriteLine("FREQ:DEM " + FFMDemodFreq.ToString("G29").Replace(',', '.'));
                    FFMDemodFreq = decimal.Parse(tc.Query("FREQ:DEM?").Replace('.', ','));
                }
                #region Exception
                catch (Exception exp)
                {
                    MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "RsReceiver", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
                }
                #endregion
            }
            TelnetDM -= SetDemodFreqDn;
        }
        /// <summary>
        /// Получаем центральную частоту демодуляции
        /// </summary>
        public void GetDemodFreq()
        {
            if (tc.IsOpen)
            {
                try
                {
                    FFMDemodFreq = decimal.Parse(tc.Query("FREQ:DEM?"));
                }
                #region Exception
                catch (Exception exp)
                {
                    MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "RsReceiver", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
                }
                #endregion
            }
            TelnetDM -= GetDemodFreq;
        }


        /// <summary>
        /// Установка RfMode 
        /// </summary>
        public void SetRfMode()
        {
            if (tc.IsOpen && UniqueData.RFModeChangeable == true)
            {
                try
                {
                    tc.WriteLine("INP:ATT:MODE " + RFMode.Parameter);
                }
                #region Exception
                catch (Exception exp)
                {
                    MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "RsReceiver", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
                }
                #endregion
            }
            TelnetDM -= SetRfMode;
        }
        public void SetRfModeUp()
        {
            if (tc.IsOpen && UniqueData.RFModeChangeable == true)
            {
                try
                {
                    int i = UniqueData.RFModes.IndexOf(RFMode);
                    i++;
                    if (i >= UniqueData.RFModes.Count)
                    {
                        i = 0;
                    }
                    RFMode = UniqueData.RFModes[i];
                    tc.WriteLine("INP:ATT:MODE " + RFMode.Parameter);
                }
                #region Exception
                catch (Exception exp)
                {
                    MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "RsReceiver", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
                }
                #endregion
            }
            TelnetDM -= SetRfModeUp;
        }
        public void SetRfModeDn()
        {
            if (tc.IsOpen && UniqueData.RFModeChangeable == true)
            {
                try
                {
                    int i = UniqueData.RFModes.IndexOf(RFMode);
                    i--;
                    if (i < 0)
                    {
                        i = UniqueData.RFModes.Count - 1;
                    }
                    RFMode = UniqueData.RFModes[i];
                    tc.WriteLine("INP:ATT:MODE " + RFMode.Parameter);
                }
                #region Exception
                catch (Exception exp)
                {
                    MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "RsReceiver", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
                }
                #endregion
            }
            TelnetDM -= SetRfModeDn;
        }
        /// <summary>
        /// Получаем RfMode
        /// </summary>
        public void GetRfMode()
        {
            if (tc.IsOpen && UniqueData.RFModeChangeable == true)
            {
                try
                {
                    string t = tc.Query("INP:ATT:MODE?").TrimEnd();
                    for (int i = 0; i < UniqueData.RFModes.Count; i++)
                    {
                        if (t.ToUpper() == UniqueData.RFModes[i].Parameter) { RFMode = UniqueData.RFModes[i]; }//запилить для каждой железки
                    }
                }
                #region Exception
                catch (Exception exp)
                {
                    MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "RsReceiver", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
                }
                #endregion
            }
            TelnetDM -= GetRfMode;
        }




        #region DF ======================================================================
        public void SetDFSQUMode()
        {
            if (tc.IsOpen)
            {
                try
                {
                    int i = DFSQUMode;
                    i++;
                    if (i > 2) i = 0;

                    if (i == 0) { tc.WriteLine("MEASure:DF:MODE OFF"); }
                    else if (i == 1) { tc.WriteLine("MEASure:DF:MODE GATE"); }
                    else if (i == 2) { tc.WriteLine("MEASure:DF:MODE NORM"); }
                }
                #region Exception
                catch (Exception exp)
                {
                    MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "RsReceiver", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
                }
                #endregion
            }
            TelnetDM -= SetDFSQUMode;
        }
        public void SetDFSQUFromSlider()
        {
            if (tc.IsOpen)
            {
                try
                {
                    tc.WriteLine("MEASure:DF:THReshold " + DFSquelchValueSlider.ToString());
                    DFSquelchValueSliderState = false;
                }
                #region Exception
                catch (Exception exp)
                {
                    MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "RsReceiver", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
                }
                #endregion
            }
            TelnetDM -= SetDFSQUFromSlider;
        }
        public void SetDFSQUUp()
        {
            if (tc.IsOpen)
            {
                try
                {
                    decimal ind = DFSquelchValue;
                    if (ind < UniqueData.DFSQUMAX - 1)
                    {
                        ind++;
                        //DFSquelchValueSlider = (int)ind;
                    }
                    tc.WriteLine("MEASure:DF:THReshold " + ind.ToString("G29").Replace(',', '.'));

                }
                #region Exception
                catch (Exception exp)
                {
                    MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "RsReceiver", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
                }
                #endregion
            }
            TelnetDM -= SetDFSQUUp;
        }
        public void SetDFSQUDn()
        {
            if (tc.IsOpen)
            {
                try
                {
                    decimal ind = DFSquelchValue;
                    if (ind > UniqueData.DFSQUMIN + 1)
                    {
                        ind--;
                        //DFSquelchValueSlider = (int)ind;
                    }
                    tc.WriteLine("MEASure:DF:THReshold " + ind.ToString("G29").Replace(',', '.'));
                }
                #region Exception
                catch (Exception exp)
                {
                    MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "RsReceiver", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
                }
                #endregion
            }
            TelnetDM -= SetDFSQUDn;
        }
        public void SetDFQualSQUUp()
        {
            if (tc.IsOpen)
            {
                try
                {
                    if (DFQualitySQU < 100)
                    {
                        DFQualitySQU++;
                    }
                    //tc.WriteLine("MEASure:DF:THReshold " + ind.ToString("G29").Replace(',', '.'));
                }
                #region Exception
                catch (Exception exp)
                {
                    MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "RsReceiver", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
                }
                #endregion
            }
            TelnetDM -= SetDFQualSQUUp;
        }
        public void SetDFQualSQUDn()
        {
            if (tc.IsOpen)
            {
                try
                {
                    //decimal ind = DFSquelchValue;
                    if (DFQualitySQU > 0)
                    {
                        DFQualitySQU--;
                    }
                    //tc.WriteLine("MEASure:DF:THReshold " + ind.ToString("G29").Replace(',', '.'));
                }
                #region Exception
                catch (Exception exp)
                {
                    MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "RsReceiver", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
                }
                #endregion
            }
            TelnetDM -= SetDFQualSQUDn;
        }
        public void SetDFMeasureTimeUp()
        {
            if (tc.IsOpen)
            {
                try
                {
                    decimal ind = DFMeasureTime;
                    if (ind < UniqueData.DFMeasTimeMAX)
                    {
                        ind += help.PlMntime(ind);
                    }
                    tc.WriteLine("MEASure:DFINder:TIME " + ind.ToString().Replace(',', '.'));
                }
                #region Exception
                catch (Exception exp)
                {
                    MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "RsReceiver", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
                }
                #endregion
            }
            TelnetDM -= SetDFMeasureTimeUp;
        }
        public void SetDFMeasureTimeDn()
        {
            if (tc.IsOpen)
            {
                try
                {
                    decimal ind = DFMeasureTime;
                    if (ind > UniqueData.DFMeasTimeMIN)
                    {
                        ind -= help.PlMntime(ind);
                    }
                    tc.WriteLine("MEASure:DFINder:TIME " + ind.ToString().Replace(',', '.'));
                }
                #region Exception
                catch (Exception exp)
                {
                    MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "RsReceiver", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
                }
                #endregion
            }
            TelnetDM -= SetDFMeasureTimeDn;
        }

        public void SetDFBandwidth()
        {
            if (tc.IsOpen)
            {
                try
                {
                    tc.WriteLine("BAND:DF:RES " + UniqueData.DFBW[DFBandwidthInd].ToString("G29").Replace(',', '.'));
                    DFBandwidthInd = System.Array.IndexOf(UniqueData.DFBW, decimal.Parse(tc.Query("BAND:DF:RES?").Replace('.', ',')));
                }
                #region Exception
                catch (Exception exp)
                {
                    MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "RsReceiver", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
                }
                #endregion
            }
            TelnetDM -= SetDFBandwidth;
        }
        public void SetDFBandwidthUp()
        {
            if (tc.IsOpen)
            {
                try
                {
                    int ind = DFBandwidthInd;
                    if (ind < UniqueData.DFBW.Length - 1)
                    {
                        ind++;
                    }
                    tc.WriteLine("BAND:DF:RES " + UniqueData.DFBW[ind].ToString("G29").Replace(',', '.'));
                    DFBandwidthInd = System.Array.IndexOf(UniqueData.DFBW, decimal.Parse(tc.Query("BAND:DF:RES?").Replace('.', ',')));
                }
                #region Exception
                catch (Exception exp)
                {
                    MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "RsReceiver", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
                }
                #endregion
            }
            TelnetDM -= SetDFBandwidthUp;
        }
        public void SetDFBandwidthDn()
        {
            if (tc.IsOpen)
            {
                try
                {
                    int ind = DFBandwidthInd;
                    if (ind > 0)
                    {
                        ind--;
                    }
                    tc.WriteLine("BAND:DF:RES " + UniqueData.DFBW[ind].ToString("G29").Replace(',', '.'));
                    DFBandwidthInd = System.Array.IndexOf(UniqueData.DFBW, decimal.Parse(tc.Query("BAND:DF:RES?").Replace('.', ',')));
                }
                #region Exception
                catch (Exception exp)
                {
                    MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "RsReceiver", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
                }
                #endregion
            }
            TelnetDM -= SetDFBandwidthDn;
        }
        public void SetDFBandwidthAuto()
        {
            if (tc.IsOpen)
            {
                try
                {
                    if (DFBandwidthAuto) { tc.WriteLine("BAND:DF:RES:AUTO 1"); }
                    else { tc.WriteLine("BAND:DF:RES:AUTO 0"); }
                    string t = tc.Query("BAND:DF:RES:AUTO?");
                    if (t.Contains("1") || t.Contains("ON")) { DFBandwidthAuto = true; }
                    else if (t.Contains("0") || t.Contains("OFF")) { DFBandwidthAuto = false; }
                    DFBandwidthInd = System.Array.IndexOf(UniqueData.DFBW, decimal.Parse(tc.Query("BAND:DF:RES?").Replace('.', ',')));
                }
                #region Exception
                catch (Exception exp)
                {
                    MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "RsReceiver", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
                }
                #endregion
            }
            TelnetDM -= SetDFBandwidthAuto;
        }
        public void GetDFBandwidthAuto()
        {
            if (tc.IsOpen)
            {
                try
                {
                    string t = tc.Query("BAND:DF:RES:AUTO?");
                    if (t.Contains("1") || t.Contains("ON")) { DFBandwidthAuto = true; }
                    else if (t.Contains("0") || t.Contains("OFF")) { DFBandwidthAuto = false; }
                    DFBandwidthInd = System.Array.IndexOf(UniqueData.DFBW, decimal.Parse(tc.Query("BAND:DF:RES?").Replace('.', ',')));
                }
                #region Exception
                catch (Exception exp)
                {
                    MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "RsReceiver", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
                }
                #endregion
            }
            TelnetDM -= GetDFBandwidthAuto;
        }
        #endregion














        /// <summary>
        /// Установка VUHF
        /// </summary>
        public void SetVUHF()
        {
            if (tc.IsOpen)
            {
                try
                {
                    SQUState = true;
                    tc.WriteLine("OUTP:SQU ON");
                    tc.WriteLine("OUTP:SQU:THR " + SQU.ToString());
                }
                catch { }
            }
            TelnetDM -= SetVUHF;
        }
        public void SetVUHFUp()
        {
            if (tc.IsOpen)
            {
                try
                {
                    SQU++;
                    SQUState = true;
                    tc.WriteLine("OUTP:SQU ON");
                    tc.WriteLine("OUTP:SQU:THR " + SQU.ToString());
                }
                catch { }
            }
            TelnetDM -= SetVUHFUp;
        }
        public void SetVUHFDn()
        {
            if (tc.IsOpen)
            {
                try
                {
                    SQU--;
                    AutoAntenna = true;
                    tc.WriteLine("ROUTe:AUTO OFF");
                    tc.WriteLine("ROUTe:VUHF(@1) " + SQU.ToString());
                }
                catch { }
            }
            TelnetDM -= SetVUHFDn;
        }
        /// <summary>
        /// Получаем SQU
        /// </summary>
        public void GetVUHF()
        {
            if (tc.IsOpen)
            {
                try
                {
                    SQU = Int32.Parse(tc.Query("ROUTe:VUHF?").Replace('.', ','));
                }
                catch { }
            }
            TelnetDM -= GetVUHF;
        }
        public void SetAutoAntennaState()
        {
            if (tc.IsOpen)
            {
                try
                {
                    AutoAntenna = !AutoAntenna;
                    if (AutoAntenna) { tc.WriteLine("ROUTe:AUTO ON"); }
                    else { tc.WriteLine("ROUTe:AUTO OFF"); }
                }
                catch { }
            }
            TelnetDM -= SetAutoAntennaState;
        }
        public void GetAutoAntennaState()
        {
            if (tc.IsOpen)
            {
                try
                {
                    string t = tc.Query("ROUT:AUTO?");
                    if (t.Contains("ON")) { AutoAntenna = true; }
                    else if (t.Contains("OFF")) { AutoAntenna = false; }
                }
                catch { }
            }
            TelnetDM -= GetAutoAntennaState;
        }











        /// <summary>
        /// Установка LevelUnit
        /// </summary>
        public void SetLevelUnit()
        {
            if (tc.IsOpen)
            {
                try
                {
                    //MGCAuto = false;
                    //tc.WriteLine("GCON " + MGC.ToString());
                }
                catch { }
            }
            TelnetDM -= SetLevelUnit;
        }
        /// <summary>
        /// Получаем LevelUnit
        /// </summary>
        public void GetLevelUnit()
        {
            if (tc.IsOpen)
            {
                try
                {
                    //for (int i = 0; i < 6; i++)
                    //{
                    //    Markers[i].LevelUnitStr = LevelUnitStr;
                    //}
                    //SQU = Int32.Parse(tc.Query("OUTP:SQU:THR?").Replace('.', ','));
                }
                catch { }
            }
            TelnetDM -= GetLevelUnit;
        }



        public void GetTrack()
        {
            if (tc.IsOpen)
            {
                try
                {
                    //int n = int.Parse(tc.Query("TRAC:UDP? MAX"));
                    //System.Windows.MessageBox.Show(n.ToString());
                    string t = "";
                    for (int i = 0; i < 10; i++)
                    { t += tc.Query("TRAC:UDP? " + i.ToString()) + "\r\n"; }
                    Temp = t;
                }
                catch { }
            }
            TelnetDM -= GetTrack;
        }
        public void GetTemp()
        {
            if (tc.IsOpen)
            {
                try
                {
                    //MessageBox.Show(Temp1);
                    string t = tc.Query(Temp1) + "\r\n";

                    //t += tc.Read() + "\r\n";
                    //t += tc.Read() + "\r\n";
                    //t += tc.Read() + "\r\n";
                    //t += tc.Read() + "\r\n";
                    Temp = t;
                }
                catch { }
            }
            TelnetDM -= GetTemp;
        }
        public void Gettr()
        {
            if (tc.IsOpen)
            {
                string t = tc.Query("stat:ext:data?");
                //System.Windows.MessageBox.Show(t);
                t = tc.Query("TRACe:POINts? IFPAN");
                //System.Windows.MessageBox.Show(t);
                //try
                //{
                //    System.Windows.MessageBox.Show(tc.Query("TRACe:POINt?"));
                //}
                //catch { }
            }
            TelnetDM -= Gettr;
        }







        public void Preset()
        {
            if (tc.IsOpen)
            {
                try
                {
                    tc.WriteLine("SYSTem:PRESet");
                }
                catch { }
            }
            TelnetDM -= Preset;
        }
        public void bla()
        {
            string t = "";
            if (tc.IsOpen)
            {
                long beginTiks = DateTime.Now.Ticks;
                t += "    " + tc.Query("SENS:FREQ:STAR?");
                t += "         " + new TimeSpan(DateTime.Now.Ticks - beginTiks).ToString(); ;// "    " + tc.Query("SENS:FREQ:STAR?");

            }

            //dm -= bla;
        }
    }



    public partial class AllRCVUnqData : INotifyPropertyChanged
    {
        public int InstrManufacrure { get; set; }
        public string InstrModel { get; set; }
        public List<RCVOption> InstrOption { get; set; }
        public List<RCVOption> LoadedInstrOption { get; set; }
        private ObservableCollection<mode> _Modes;
        public ObservableCollection<mode> Modes
        {
            get { return _Modes; }
            set { _Modes = value; OnPropertyChanged("Modes"); }
        }
        private bool _DemodFreq = false;
        /// <summary>
        /// True = отличная от центральной частота демодуляции
        /// </summary>
        public bool DemodFreq
        {
            get { return _DemodFreq; }
            set { _DemodFreq = value; OnPropertyChanged("DemodFreq"); }
        }
        public string[] Demod { get; set; }
        public decimal[] DemodBW { get; set; }
        public decimal[] FFMStepBW { get; set; }
        public decimal[] DFBW { get; set; }
        public decimal[] FFMSpanBW { get; set; }
        public decimal[] PSCANStepBW { get; set; }
        public ObservableCollection<ParamWithUI> FFTModes { get; set; }
        private bool _SelectivityChangeable = false;
        /// <summary>
        /// Фиксированная селективность у прибора если True то неразрешать менять ибо безтолку
        /// </summary>
        public bool SelectivityChangeable
        {
            get { return _SelectivityChangeable; }
            set { _SelectivityChangeable = value; OnPropertyChanged("SelectivityChangeable"); }
        }
        public ObservableCollection<ParamWithUI> SelectivityModes { get; set; }
        public ObservableCollection<ParamWithUI> Detectors { get; set; }
        private bool _RFModeChangeable = false;
        public bool RFModeChangeable
        {
            get { return _RFModeChangeable; }
            set { _RFModeChangeable = value; OnPropertyChanged("RFModeChangeable"); }
        }
        public ObservableCollection<RFMode> RFModes { get; set; }
        private bool _ATTFix = false;
        public bool ATTFix
        {
            get { return _ATTFix; }
            set { _ATTFix = value; OnPropertyChanged("ATTFix"); }
        }
        public int AttStep { get; set; }
        public int AttMax { get; set; }
        public decimal DFSQUMAX { get; set; }
        public decimal DFSQUMIN { get; set; }
        public decimal DFMeasTimeMAX { get; set; }
        public decimal DFMeasTimeMIN { get; set; }
        public decimal RefLevelMIN { get; set; }
        public decimal RefLevelMAX { get; set; }
        public decimal RangeLevelMIN { get; set; }
        public decimal RangeLevelMAX { get; set; }
        public decimal RangeLevelStep { get; set; }
        //private int _NumberOfTrace = 0;
        //public int NumberOfTrace
        //{
        //    get { return _NumberOfTrace; }
        //    set { _NumberOfTrace = value; OnPropertyChanged("NumberOfTrace"); }
        //}
        //public List<TrType> TraceType { get; set; }
        //public List<TrDetector> TraceDetector { get; set; }
        //private bool _ChangeableSweepType = false;
        //public bool ChangeableSweepType
        //{
        //    get { return _ChangeableSweepType; }
        //    set { _ChangeableSweepType = value; OnPropertyChanged("ChangeableSweepType"); }
        //}
        //public List<SwpType> SweepType { get; set; }
        //private bool _SweepPointFix = false;
        //public bool SweepPointFix
        //{
        //    get { return _SweepPointFix; }
        //    set { _SweepPointFix = value; OnPropertyChanged("SweepPointFix"); }
        //}
        //public int[] SweepPointArr { get; set; }
        //public int DefaultSweepPoint { get; set; }
        //public double[] RBWArr { get; set; }
        //public double[] VBWArr { get; set; }
        //private bool _CouplingRatio = false;
        //public bool CouplingRatio
        //{
        //    get { return _CouplingRatio; }
        //    set { _CouplingRatio = value; OnPropertyChanged("CouplingRatio"); }
        //}

        //private bool _PreAmp = false;
        //public bool PreAmp
        //{
        //    get { return _PreAmp; }
        //    set { _PreAmp = value; OnPropertyChanged("PreAmp"); }
        //}
        //public double AttMax { get; set; }
        //public double AttStep { get; set; }

        //public bool Battery { get; set; }
        //public bool NdB { get; set; }
        //public bool OBW { get; set; }
        //public bool ChnPow { get; set; }


        public event PropertyChangedEventHandler PropertyChanged; // Событие, которое нужно вызывать при изменении

        // Для удобства обернем событие в метод с единственным параметром - имя изменяемого свойства
        public void OnPropertyChanged(string propertyName)
        {
            // Если кто-то на него подписан, то вызывем его
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    public partial class RCVOption
    {
        public string Type { get; set; }
        public string Globaltype { get; set; }
        public string Designation { get; set; }
    }
    public partial class mode
    {
        public string Mode { get; set; }
        public string MeasAppl { get; set; }
        public string FreqMode { get; set; }
    }
    public partial class FFTMode
    {
        public string UI { get; set; }
        public string Parameter { get; set; }
    }
    public partial class SelectivityMode
    {
        public string UI { get; set; }
        public string Parameter { get; set; }
    }
    //public partial class Detector
    //{
    //    public string UI { get; set; }
    //    public string Parameter { get; set; }
    //}
    public partial class RFMode
    {
        public string UI { get; set; }
        public string Parameter { get; set; }
    }
    public partial class MVSJammingSearch
    {
        public bool Alahadbar { get; set; }
        public decimal FreqCentr { get; set; }
        public decimal FreqSpan { get; set; }
        public ManyTracePoint[] Trace { get; set; }
        /// <summary>
        /// 0 = не выбрано
        /// 1 = GPS
        /// 2 = GSM
        /// 3 = UMTS
        /// 4 = CDMA
        /// 5 = 450
        /// </summary>
        public int DetectionType { get; set; }

        public decimal SQU { get; set; }
        public int Count { get; set; }
        public int MeasuredCount { get; set; }
        public decimal[] MeasuredLevels { get; set; }
        public tracepoint[] Markers { get; set; }
        //public decimal SQU { get; set; }

    }
    public partial class ManyTracePoint : tracepoint
    {
        public int Id { get; set; }
    }
}
