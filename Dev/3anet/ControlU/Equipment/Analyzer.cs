using System;
using System.Xml;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Linq;
using System.Windows;
using System.Collections.ObjectModel;
using NationalInstruments.VisaNS;

namespace ControlU.Equipment
{
    public class Analyzer : INotifyPropertyChanged
    {
        string AppPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        Settings.XMLSettings Sett = App.Sett;
        LocalMeasurement LM = new LocalMeasurement();
        Helpers.Helper help = new Helpers.Helper();

        private TcpipSession session;
        public delegate void DoubMod();
        public delegate void Method();
        public DoubMod an_dm;
        private Thread AnThread;
        private System.Timers.Timer antmr = new System.Timers.Timer(100);

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
        private static bool _IsRuning;
        public bool IsRuning
        {
            get { return _IsRuning; }
            set { if (_IsRuning != value) { _IsRuning = value; OnPropertyChanged("IsRuning"); } }
        }

        public bool GetData
        {
            get { return _GetData; }
            set { _GetData = value; /*OnPropertyChanged("OutText"); */}
        }
        public bool _GetData;

        public long LastUpdate
        {
            get { return _LastUpdate; }
            set { _LastUpdate = value; /*OnPropertyChanged("OutText"); */}
        }
        private long _LastUpdate;

        public bool DataCycle
        {
            get { return _DataCycle; }
            set { _DataCycle = value; OnPropertyChanged("DataCycle"); }
        }
        private bool _DataCycle = false;

        public bool IsConnected
        {
            get { return _IsConnected; }
            set { _IsConnected = value; OnPropertyChanged("IsConnected"); }
        }
        bool _IsConnected = false;

        private bool _AnyMeas;
        public bool AnyMeas
        {
            get { return _AnyMeas; }
            set { _AnyMeas = value; OnPropertyChanged("AnyMeas"); }
        }
        #endregion

        #region Instr Info
        public bool StateResult
        {
            get { return _StateResult; }
            private set { }
        }
        private bool _StateResult = true;

        public DB.localatdi_meas_device device_meas
        {
            get { return _device_meas; }
            set { _device_meas = value; OnPropertyChanged("device_meas"); }
        }
        private DB.localatdi_meas_device _device_meas = new DB.localatdi_meas_device() { };

        private AllAnUnqData _UniqueData = new AllAnUnqData { };
        public AllAnUnqData UniqueData
        {
            get { return _UniqueData; }
            set { _UniqueData = value; OnPropertyChanged("UniqueData"); }
        }
        private ObservableCollection<AllAnUnqData> AllUniqueData = new ObservableCollection<AllAnUnqData>
        {
            #region FSW
            new AllAnUnqData
            {
                InstrManufacture = 1,  NumberOfTrace = 3, ChangeableSweepType = true, SweepPointFix = false, InstrModel = "FSW",
                HiSpeed = true,
                InstrOption = new List<AnOption>()
                {
                    #region 
                     new AnOption(){Type = "B4", Designation = "OCXO, Precision Reference Frequency", Globaltype = "OCXO, Precision Reference Frequency"},
                     new AnOption(){Type = "B8", Designation = "Resolution Bandwidths > 10 MHz (for R&S®FSW8/13/26)", Globaltype = "Resolution Bandwidths > 10 MHz (for R&S®FSW8/13/26)"},
                     new AnOption(){Type = "B8", Designation = "Resolution Bandwidths > 10 MHz (for R&S®FSW43/50/67/85)", Globaltype = "Resolution Bandwidths > 10 MHz (for R&S®FSW43/50/67/85)"},
                     new AnOption(){Type = "B10", Designation = "External Generator Control", Globaltype = "External Generator Control"},
                     new AnOption(){Type = "B13", Designation = "Highpass Filters for Harmonic Measurements", Globaltype = "Highpass Filters for Harmonic Measurements"},
                     new AnOption(){Type = "B17", Designation = "Digital Baseband Interface", Globaltype = "Digital Baseband Interface"},
                     new AnOption(){Type = "B18", Designation = "Solid State Drive (SSD, removable)", Globaltype = "SSD"},
                     new AnOption(){Type = "B21", Designation = "LO/IF Ports for External Mixers (for R&S®FSW26)", Globaltype = "External Mixers"},
                     new AnOption(){Type = "B21", Designation = "LO/IF Ports for External Mixers (for R&S®FSW43/50/67)", Globaltype = "External Mixers"},
                     new AnOption(){Type = "B21", Designation = "LO/IF Ports for External Mixers (for R&S®FSW85)", Globaltype = "External Mixers"},
                     new AnOption(){Type = "B24", Designation = "Preamplifier", Globaltype = "Preamplifier"},
                     new AnOption(){Type = "B25", Designation = "Electronic Attenuator (1 dB steps)", Globaltype = "Attenuator 1 dB"},
                     new AnOption(){Type = "B33", Designation = "USB Mass Memory Write Protection", Globaltype = "USB Mass Memory Write Protection"},
                     new AnOption(){Type = "B28", Designation = "28 MHz Analysis Bandwidth", Globaltype = "28 MHz Analysis Bandwidth"},
                     new AnOption(){Type = "B40", Designation = "40 MHz Analysis Bandwidth", Globaltype = "40 MHz Analysis Bandwidth"},
                     new AnOption(){Type = "B71", Designation = "Analog Baseband Inputs", Globaltype = "Analog Baseband Inputs"},
                     new AnOption(){Type = "B71E", Designation = "80 MHz Bandwidth for Analog Baseband Inputs", Globaltype = "80 MHz Bandwidth for Analog Baseband Inputs"},
                     new AnOption(){Type = "B80", Designation = "80 MHz Analysis Bandwidth", Globaltype = "80 MHz Analysis Bandwidth"},
                     new AnOption(){Type = "B160", Designation = "160 MHz Analysis Bandwidth", Globaltype = "160 MHz Analysis Bandwidth"},
                     new AnOption(){Type = "B160R", Designation = "Real-Time Spectrum Analyzer, 160 MHz", Globaltype = "Real-Time Spectrum Analyzer, 160 MHz"},
                     new AnOption(){Type = "B320", Designation = "320 MHz Analysis Bandwidth", Globaltype = "320 MHz Analysis Bandwidth"},
                     new AnOption(){Type = "B512", Designation = "512 MHz Analysis Bandwidth", Globaltype = "512 MHz Analysis Bandwidth"},
                     new AnOption(){Type = "B512R", Designation = "Real-Time Spectrum Analyzer, 512 MHz", Globaltype = "Real-Time Spectrum Analyzer, 512 MHz"},
                     new AnOption(){Type = "B1200", Designation = "1.2 GHz Analysis Bandwidth", Globaltype = "1.2 GHz Analysis Bandwidth"},
                     new AnOption(){Type = "B2000", Designation = "2 GHz Analysis Bandwidth", Globaltype = "2 GHz Analysis Bandwidth"},
                     new AnOption(){Type = "K0", Designation = "DEMO OPTION (All inclusive)", Globaltype = "DEMO OPTION (All inclusive)"},
                     new AnOption(){Type = "K6", Designation = "Pulse Measurements", Globaltype = "Pulse Measurements"},
                     new AnOption(){Type = "K6S", Designation = "Time Sidelobe Measurement", Globaltype = "Time Sidelobe Measurement"},
                     new AnOption(){Type = "K7", Designation = "Analog Modulation Analysis (AM/FM/φM)", Globaltype = "Analog Modulation Analysis"},
                     new AnOption(){Type = "K10", Designation = "GSM, EDGE, EDGE Evolution and VAMOS Measurements", Globaltype = "GSM, EDGE, EDGE Evolution and VAMOS Measurements"},
                     new AnOption(){Type = "K15", Designation = "VOR/ILS Measurements", Globaltype = "VOR/ILS Measurements"},
                     new AnOption(){Type = "K17", Designation = "Multicarrier Group Delay Measurements", Globaltype = "Multicarrier Group Delay Measurements"},
                     new AnOption(){Type = "K18", Designation = "Amplifier Measurements", Globaltype = "Amplifier Measurements"},
                     new AnOption(){Type = "K18D", Designation = "Direct DPD Measurements", Globaltype = "Direct DPD Measurements"},
                     new AnOption(){Type = "K30", Designation = "Noise Figure Measurements", Globaltype = "Noise Figure Measurements"},
                     new AnOption(){Type = "K33", Designation = "Security Write Protection of solid state drive", Globaltype = "Security Write Protection of solid state drive"},
                     new AnOption(){Type = "K40", Designation = "Phase Noise Measurements", Globaltype = "Phase Noise Measurements"},
                     new AnOption(){Type = "K50", Designation = "Spurious Measurements", Globaltype = "Spurious Measurements"},
                     new AnOption(){Type = "K54", Designation = "EMI Measurements", Globaltype = "EMI Measurements"},
                     new AnOption(){Type = "K60", Designation = "Transient Measurement Application", Globaltype = "Transient Measurement Application"},
                     new AnOption(){Type = "K60C", Designation = "Transient Chirp Measurement", Globaltype = "Transient Chirp Measurement"},
                     new AnOption(){Type = "K60H", Designation = "Transient Hop Measurement", Globaltype = "Transient Hop Measurement"},
                     new AnOption(){Type = "K70", Designation = "Vector Signal Analysis", Globaltype = "Vector Signal Analysis"},
                     new AnOption(){Type = "K72", Designation = "3GPP FDD (WCDMA) BS Measurements (incl. HSDPA and HSDPA+)", Globaltype = "3GPP FDD (WCDMA) BS Measurements (incl. HSDPA and HSDPA+)"},
                     new AnOption(){Type = "K73", Designation = "3GPP FDD (WCDMA) UE Measurements (incl. HSUPA and HSUPA+)", Globaltype = "3GPP FDD (WCDMA) UE Measurements (incl. HSUPA and HSUPA+)"},
                     new AnOption(){Type = "K76", Designation = "3GPP TDD (TD-SCDMA) BS Measurements", Globaltype = "3GPP TDD (TD-SCDMA) BS Measurements"},
                     new AnOption(){Type = "K77", Designation = "3GPP TDD (TD-SCDMA) UE Measurements", Globaltype = "3GPP TDD (TD-SCDMA) UE Measurements"},
                     new AnOption(){Type = "K82", Designation = "CDMA2000® BS (DL) Analysis", Globaltype = "CDMA2000® BS (DL) Analysis"},
                     new AnOption(){Type = "K83", Designation = "CDMA2000® MS (UL) Measurements", Globaltype = "CDMA2000® MS (UL) Measurements"},
                     new AnOption(){Type = "K84", Designation = "1xEV-DO BS (DL) Analysis", Globaltype = "1xEV-DO BS (DL) Analysis"},
                     new AnOption(){Type = "K85", Designation = "1xEV-DO MS (UL) Measurements", Globaltype = "1xEV-DO MS (UL) Measurements"},
                     new AnOption(){Type = "K91", Designation = "WLAN IEEE802.11a/b/g Measurements", Globaltype = "WLAN IEEE802.11a/b/g Measurements"},
                     new AnOption(){Type = "K91N", Designation = "WLAN IEEE802.11n Measurements", Globaltype = "WLAN IEEE802.11n Measurements"},
                     new AnOption(){Type = "K91AC", Designation = "WLAN IEEE802.11ac Measurements", Globaltype = "WLAN IEEE802.11ac Measurements"},
                     new AnOption(){Type = "K91AX", Designation = "WLAN IEEE802.11ax Measurements", Globaltype = "WLAN IEEE802.11ax Measurements"},
                     new AnOption(){Type = "K95", Designation = "WLAN IEEE802.11ad Measurements", Globaltype = "WLAN IEEE802.11ad Measurements"},
                     new AnOption(){Type = "K100", Designation = "EUTRA/LTE FDD Downlink Analysis", Globaltype = "EUTRA/LTE FDD Downlink Analysis"},
                     new AnOption(){Type = "K101", Designation = "EUTRA/LTE FDD Uplink Analysis", Globaltype = "EUTRA/LTE FDD Uplink Analysis"},
                     new AnOption(){Type = "K102", Designation = "EUTRA/LTE Downlink MIMO Analysis", Globaltype = "EUTRA/LTE Downlink MIMO Analysis"},
                     new AnOption(){Type = "K103", Designation = "EUTRA/LTE UL Advanced UL Measurements", Globaltype = "EUTRA/LTE UL Advanced UL Measurements"},
                     new AnOption(){Type = "K104", Designation = "EUTRA/LTE TDD Downlink Analysis", Globaltype = "EUTRA/LTE TDD Downlink Analysis"},
                     new AnOption(){Type = "K105", Designation = "EUTRA/LTE TDD Uplink Analysis", Globaltype = "EUTRA/LTE TDD Uplink Analysis"},
                     new AnOption(){Type = "K96", Designation = "OFDM Vector Signal Analysis Software", Globaltype = "OFDM Vector Signal Analysis Software"},
                     new AnOption(){Type = "K192", Designation = "DOCSIS 3.1 OFDM Downstream", Globaltype = "DOCSIS 3.1 OFDM Downstream"},
                     new AnOption(){Type = "K193", Designation = "DOCSIS 3.1 OFDM Upstream", Globaltype = "DOCSIS 3.1 OFDM Upstream"},
                     new AnOption(){Type = "K160RE", Designation = "160 MHz Real-Time Measurement Application, POI > 15 µs", Globaltype = "160 MHz Real-Time Measurement Application, POI > 15 µs"},
                    #endregion
                },
                DefaultInstrOption = new List<AnOption>() { },
                LoadedInstrOption = new List<AnOption>() { },
                TraceType = new ObservableCollection<ParamWithUI>
                {
                    #region
                    new ParamWithUI {UI = "Clear Write", Parameter = "WRIT" },
                    new ParamWithUI {UI = "Avarege", Parameter = "AVER" },
                    new ParamWithUI {UI = "Max Hold", Parameter = "MAXH" },
                    new ParamWithUI {UI = "Min Hold", Parameter = "MINH" },
                    new ParamWithUI {UI = "View", Parameter = "VIEW" },
                    new ParamWithUI {UI = "Blank", Parameter = "BLAN" }
                    #endregion
                },
                TraceDetector = new List<TrDetector>
                {
                    #region
                    new TrDetector {UI = "Auto Select", Parameter = "Auto Select" },
                    new TrDetector {UI = "Auto Peak", Parameter = "APE" },
                    new TrDetector {UI = "Average", Parameter = "AVER" },
                    new TrDetector {UI = "Max Peak", Parameter = "POS" },
                    new TrDetector {UI = "Min Peak", Parameter = "NEG" },
                    new TrDetector {UI = "Sample", Parameter = "SAMP" },
                    new TrDetector {UI = "RMS", Parameter = "RMS" }
                    #endregion
                },
                SweepType =  new List<SwpType>
                {
                    #region
                    new SwpType {UI = "Auto", Parameter = "AUTO" },
                    new SwpType {UI = "Sweep", Parameter = "SWE" },
                    new SwpType {UI = "FFT", Parameter = "FFT" }
                    #endregion
                },
                SweepPointArr = new int[]{ 101, 125, 155, 173, 201, 251, 301, 313, 345, 401, 501, 601, 625, 691, 701, 801, 901, 1001, 1251, 1383, 1999, 2001, 2501, 2765, 3001, 4001, 5001, 6001, 7001, 8001, 9001, 10001, 11001, 12001, 13001, 14001, 15001, 16001, 17001, 18001, 19001, 20001, 21001, 22001, 23001, 24001, 25001, 26001, 27001, 28001, 29001, 30001, 31001, 32001 },
                DefaultSweepPoint = 1001,
                RBWArr = new decimal[]{ 1, 2, 3, 5, 10, 20, 30, 50, 100, 200, 300, 500, 1000, 2000, 3000, 5000, 6250, 10000, 20000, 30000, 50000, 100000, 200000, 300000, 500000, 1000000, 2000000, 3000000, 5000000, 10000000 },
                VBWArr = new decimal[]{ 1, 2, 3, 5, 10, 20, 30, 50, 100, 200, 300, 500, 1000, 2000, 3000, 5000, 10000, 20000, 30000, 50000, 100000, 200000, 300000, 500000, 1000000, 2000000, 3000000, 5000000, 10000000, 20000000, 28000000, 40000000 },
                CouplingRatio = true,
                AttMax = 70,
                AttStep = 5,
                PreAmp = false,
                Battery = false,
                NdB = true,
                OBW = true,
                ChnPow= true
            },
            #endregion FSW
            #region FSV
            new AllAnUnqData
            {
                InstrManufacture = 1,  NumberOfTrace = 3, ChangeableSweepType = true, SweepPointFix = false, InstrModel = "FSV",
                HiSpeed = true,
                InstrOption = new List<AnOption>()
                {
                    #region 
                     new AnOption(){Type = "B1", Designation = "Ruggedized Housing", Globaltype = "Ruggedized Housing"},
                     new AnOption(){Type = "B3", Designation = "AM/FM Audio Demodulator", Globaltype = "Audio Demodulator"},
                     new AnOption(){Type = "B4", Designation = "OCXO, Precision Reference Frequency", Globaltype = "OCXO, Precision Reference Frequency"},
                     new AnOption(){Type = "B4", Designation = "OCXO, Enhanced Frequency Stability", Globaltype = "OCXO, Enhanced Frequency Stability"},
                     new AnOption(){Type = "B5", Designation = "Additional Interfaces (IF/video/AM/FM output, AUX port, trigger output, two additional USB ports)", Globaltype = "Additional Interfaces"},
                     new AnOption(){Type = "B9", Designation = "Tracking Generator, 100 kHz to 4 GHz/7 GHz", Globaltype = "Tracking Generator"},
                     new AnOption(){Type = "B10", Designation = "External Generator Control", Globaltype = "External Generator Control"},
                     new AnOption(){Type = "B17", Designation = "Digital Baseband Interface", Globaltype = "Digital Baseband Interface"},
                     new AnOption(){Type = "B18", Designation = "Solid State Drive (SSD, removable)", Globaltype = "SSD"},
                     new AnOption(){Type = "B19", Designation = "Spare Hard Disk Drive (HDD, removable)", Globaltype = "HDD"},
                     new AnOption(){Type = "B21", Designation = "LO/IF Ports for External Mixers", Globaltype = "External Mixers"},
                     new AnOption(){Type = "B22", Designation = "RF Preamplifier (100 kHz to 7 GHz)", Globaltype = "Preamplifier"},
                     new AnOption(){Type = "B24", Designation = "Preamplifier, 9 kHz to 13.6 GHz", Globaltype = "Preamplifier"},
                     new AnOption(){Type = "B24", Designation = "Preamplifier, 9 kHz to 30 GHz", Globaltype = "Preamplifier"},
                     new AnOption(){Type = "B24", Designation = "Preamplifier, 9 kHz to 40 GHz", Globaltype = "Preamplifier"},
                     new AnOption(){Type = "B25", Designation = "Electronic Attenuator (1 dB steps)", Globaltype = "Attenuator 1 dB"},
                     new AnOption(){Type = "B30", Designation = "DC Power Supply, 12 V to 15 V", Globaltype = "DC Power Supply"},
                     new AnOption(){Type = "B32", Designation = "Lithium-Ion Battery Pack", Globaltype = "Lithium-Ion Battery Pack"},
                     new AnOption(){Type = "B34", Designation = "Battery Charger, for R&S®FSV-B32 Li-ion battery pack", Globaltype = "Battery Charger"},
                     new AnOption(){Type = "B70", Designation = "40 MHz Analysis Bandwidth", Globaltype = "40 MHz Analysis Bandwidth"},
                     new AnOption(){Type = "B160", Designation = "160 MHz Analysis Bandwidth (for R&S®FSV4 and R&S®FSV7)", Globaltype = "160 MHz Analysis Bandwidth"},
                     new AnOption(){Type = "B160", Designation = "160 MHz Analysis Bandwidth (for R&S®FSV13)", Globaltype = "160 MHz Analysis Bandwidth"},
                     new AnOption(){Type = "B160", Designation = "160 MHz Analysis Bandwidth (for R&S®FSV30 und R&S®FSV40)", Globaltype = "160 MHz Analysis Bandwidth"},
                     new AnOption(){Type = "K0", Designation = "DEMO OPTION (All inclusive)", Globaltype = "DEMO OPTION (All inclusive)"},
                     new AnOption(){Type = "K7", Designation = "Analog Modulation Analysis (AM/FM/φM)", Globaltype = "Analog Modulation Analysis"},
                     new AnOption(){Type = "K7S", Designation = "FM Stereo Measurements (for R&S®FSV-K7)", Globaltype = "FM Stereo Measurements"},
                     new AnOption(){Type = "K8", Designation = "Bluetooth®/EDR Measurement Application", Globaltype = "Bluetooth®/EDR Measurement Application"},
                     new AnOption(){Type = "K9", Designation = "Power Sensor Support (power measurement with the R&S®NRP-Zxx power sensors)", Globaltype = "Power Sensor Support"},
                     new AnOption(){Type = "K10", Designation = "GSM/EDGE/EDGE Evolution Analysis", Globaltype = "GSM/EDGE/EDGE Evolution Analysis"},
                     new AnOption(){Type = "K14", Designation = "Spectrogram Measurements", Globaltype = "Spectrogram Measurements"},
                     new AnOption(){Type = "K30", Designation = "Noise Figure and Gain Measurements", Globaltype = "Noise Figure and Gain Measurements"},
                     new AnOption(){Type = "K40", Designation = "Phase Noise Measurements", Globaltype = "Phase Noise Measurements"},
                     new AnOption(){Type = "K70", Designation = "Vector Signal Analysis", Globaltype = "Vector Signal Analysis"},
                     new AnOption(){Type = "K72", Designation = "3GPP BS (DL) Analysis, incl. HSDPA and HSDPA+", Globaltype = "3GPP BS (DL) Analysis, incl. HSDPA and HSDPA+"},
                     new AnOption(){Type = "K73", Designation = "3GPP UE (UL) Analysis, incl. HSUPA", Globaltype = "3GPP UE (UL) Analysis, incl. HSUPA"},
                     new AnOption(){Type = "K76", Designation = "TD-SCDMA BS Measurements", Globaltype = "TD-SCDMA BS Measurements"},
                     new AnOption(){Type = "K77", Designation = "TD-SCDMA UE Measurements", Globaltype = "TD-SCDMA UE Measurements"},
                     new AnOption(){Type = "K82", Designation = "CDMA2000® BS (DL) Analysis", Globaltype = "CDMA2000® BS (DL) Analysis"},
                     new AnOption(){Type = "K83", Designation = "CDMA2000® MS (UL) Measurements", Globaltype = "CDMA2000® MS (UL) Measurements"},
                     new AnOption(){Type = "K84", Designation = "1xEV-DO BS (DL) Analysis", Globaltype = "1xEV-DO BS (DL) Analysis"},
                     new AnOption(){Type = "K85", Designation = "1xEV-DO MS (UL) Measurements", Globaltype = "1xEV-DO MS (UL) Measurements"},
                     new AnOption(){Type = "K91", Designation = "WLAN IEEE802.11a/b/g/j Analysis", Globaltype = "WLAN IEEE802.11a/b/g/j Analysis"},
                     new AnOption(){Type = "K91N", Designation = "WLAN IEEE802.11n Analysis", Globaltype = "WLAN IEEE802.11n Analysis"},
                     new AnOption(){Type = "K91AC", Designation = "WLAN IEEE802.11ac Analysis", Globaltype = "WLAN IEEE802.11ac Analysis"},
                     new AnOption(){Type = "K91P", Designation = "WLAN IEEE802.11ac Analysis", Globaltype = "WLAN IEEE802.11ac Analysis"},
                     new AnOption(){Type = "K93", Designation = "WiMAX™ IEEE802.16e OFDM/OFDMA Analysis", Globaltype = "WiMAX™ IEEE802.16e OFDM/OFDMA Analysis"},
                     new AnOption(){Type = "K100", Designation = "EUTRA/LTE FDD Downlink Analysis", Globaltype = "EUTRA/LTE FDD Downlink Analysis"},
                     new AnOption(){Type = "K101", Designation = "EUTRA/LTE FDD Uplink Analysis", Globaltype = "EUTRA/LTE FDD Uplink Analysis"},
                     new AnOption(){Type = "K102", Designation = "EUTRA/LTE Downlink MIMO Analysis", Globaltype = "EUTRA/LTE Downlink MIMO Analysis"},
                     new AnOption(){Type = "K104", Designation = "EUTRA/LTE TDD Downlink Analysis", Globaltype = "EUTRA/LTE TDD Downlink Analysis"},
                     new AnOption(){Type = "K105", Designation = "EUTRA/LTE TDD Uplink Analysis", Globaltype = "EUTRA/LTE TDD Uplink Analysis"},
                     new AnOption(){Type = "K96", Designation = "OFDM Vector Signal Analysis Software", Globaltype = "OFDM Vector Signal Analysis Software"},
                     new AnOption(){Type = "K130PC", Designation = "Distortion Analysis Software", Globaltype = "Distortion Analysis Software"},
                    #endregion
                },
                DefaultInstrOption = new List<AnOption>() { },
                LoadedInstrOption = new List<AnOption>() { },
                TraceType = new ObservableCollection<ParamWithUI>
                {
                    #region
                    new ParamWithUI {UI = "Clear Write", Parameter = "WRIT" },
                    new ParamWithUI {UI = "Avarege", Parameter = "AVER" },
                    new ParamWithUI {UI = "Max Hold", Parameter = "MAXH" },
                    new ParamWithUI {UI = "Min Hold", Parameter = "MINH" },
                    new ParamWithUI {UI = "View", Parameter = "VIEW" },
                    new ParamWithUI {UI = "Blank", Parameter = "BLAN" }
                    #endregion
                },
                TraceDetector = new List<TrDetector>
                {
                    #region
                    new TrDetector {UI = "Auto Select", Parameter = "Auto Select" },
                    new TrDetector {UI = "Auto Peak", Parameter = "APE" },
                    new TrDetector {UI = "Average", Parameter = "AVER" },
                    new TrDetector {UI = "Max Peak", Parameter = "POS" },
                    new TrDetector {UI = "Min Peak", Parameter = "NEG" },
                    new TrDetector {UI = "Sample", Parameter = "SAMP" },
                    new TrDetector {UI = "RMS", Parameter = "RMS" }
                    #endregion
                },
                SweepType =  new List<SwpType>
                {
                    #region
                    new SwpType {UI = "Auto", Parameter = "AUTO" },
                    new SwpType {UI = "Sweep", Parameter = "SWE" },
                    new SwpType {UI = "FFT", Parameter = "FFT" }
                    #endregion
                },
                SweepPointArr = new int[]{ 101, 125, 155, 173, 201, 251, 301, 313, 345, 401, 501, 601, 625, 691, 701, 801, 901, 1001, 1251, 1383, 1999, 2001, 2501, 2765, 3001, 4001, 5001, 6001, 7001, 8001, 9001, 10001, 11001, 12001, 13001, 14001, 15001, 16001, 17001, 18001, 19001, 20001, 21001, 22001, 23001, 24001, 25001, 26001, 27001, 28001, 29001, 30001, 31001, 32001 },
                DefaultSweepPoint = 691,
                RBWArr = new decimal[]{ 1, 2, 3, 5, 10, 20, 30, 50, 100, 200, 300, 500, 1000, 2000, 3000, 5000, 6250, 10000, 20000, 30000, 50000, 100000, 200000, 300000, 500000, 1000000, 2000000, 3000000, 5000000, 10000000 },
                VBWArr = new decimal[]{ 1, 2, 3, 5, 10, 20, 30, 50, 100, 200, 300, 500, 1000, 2000, 3000, 5000, 10000, 20000, 30000, 50000, 100000, 200000, 300000, 500000, 1000000, 2000000, 3000000, 5000000, 10000000, 20000000, 28000000, 40000000 },
                CouplingRatio = true,
                AttMax = 70,
                AttStep = 5,
                PreAmp = false,
                Battery = false,
                NdB = true,
                OBW = true,
                ChnPow= true
            },
            #endregion FSV
            #region FSVA
            new AllAnUnqData
            {
                InstrManufacture = 1, NumberOfTrace = 3, ChangeableSweepType = true, SweepPointFix = false, InstrModel = "FSVA",
                HiSpeed = true,
                InstrOption = new List<AnOption>()
                {
                    #region 
                    new AnOption(){Type = "B1", Designation = "Ruggedized Housing", Globaltype = "Ruggedized Housing"},
                    new AnOption(){Type = "B3", Designation = "AM/FM Audio Demodulator", Globaltype = "Audio Demodulator"},
                    new AnOption(){Type = "B4", Designation = "OCXO, Precision Reference Frequency", Globaltype = "OCXO, Precision Reference Frequency"},
                    new AnOption(){Type = "B4", Designation = "OCXO, Enhanced Frequency Stability", Globaltype = "OCXO, Enhanced Frequency Stability"},
                    new AnOption(){Type = "B5", Designation = "Additional Interfaces (IF/video/AM/FM output, AUX port, trigger output, two additional USB ports)", Globaltype = "Additional Interfaces"},
                    new AnOption(){Type = "B9", Designation = "Tracking Generator, 100 kHz to 4 GHz/7 GHz", Globaltype = "Tracking Generator"},
                    new AnOption(){Type = "B10", Designation = "External Generator Control", Globaltype = "External Generator Control"},
                    new AnOption(){Type = "B11", Designation = "YIG Preselector Bypass for R&S®FSVA13", Globaltype = "YIG Preselector Bypass for R&S®FSVA13"},
                    new AnOption(){Type = "B11", Designation = "YIG Preselector Bypass for R&S®FSVA30", Globaltype = "YIG Preselector Bypass for R&S®FSVA30"},
                    new AnOption(){Type = "B11", Designation = "YIG Preselector Bypass for R&S®FSVA40", Globaltype = "YIG Preselector Bypass for R&S®FSVA40"},
                    new AnOption(){Type = "B14", Designation = "Ultra-High Precision Frequency Reference", Globaltype = "Ultra-High Precision Frequency Reference"},
                    new AnOption(){Type = "B17", Designation = "Digital Baseband Interface", Globaltype = "Digital Baseband Interface"},
                    new AnOption(){Type = "B18", Designation = "Solid State Drive (SSD, removable)", Globaltype = "SSD"},
                    new AnOption(){Type = "B19", Designation = "Spare Hard Disk Drive (HDD, removable)", Globaltype = "HDD"},
                    new AnOption(){Type = "B21", Designation = "LO/IF Ports for External Mixers", Globaltype = "External Mixers"},
                    new AnOption(){Type = "B22", Designation = "RF Preamplifier (100 kHz to 7 GHz)", Globaltype = "Preamplifier"},
                    new AnOption(){Type = "B24", Designation = "Preamplifier, 9 kHz to 13.6 GHz", Globaltype = "Preamplifier"},
                    new AnOption(){Type = "B24", Designation = "Preamplifier, 9 kHz to 30 GHz", Globaltype = "Preamplifier"},
                    new AnOption(){Type = "B24", Designation = "Preamplifier, 9 kHz to 40 GHz", Globaltype = "Preamplifier"},
                    new AnOption(){Type = "B25", Designation = "Electronic Attenuator (1 dB steps)", Globaltype = "Attenuator 1 dB"},
                    new AnOption(){Type = "B30", Designation = "DC Power Supply, 12 V to 15 V", Globaltype = "DC Power Supply"},
                    new AnOption(){Type = "B32", Designation = "Lithium-Ion Battery Pack", Globaltype = "Lithium-Ion Battery Pack"},
                    new AnOption(){Type = "B33", Designation = "USB Mass Memory Write Protection", Globaltype = "USB Mass Memory Write Protection"},
                    new AnOption(){Type = "B34", Designation = "Battery Charger, for R&S®FSV-B32 Li-ion battery pack", Globaltype = "Battery Charger"},
                    new AnOption(){Type = "B40", Designation = "40 MHz Analysis Bandwidth", Globaltype = "40 MHz Analysis Bandwidth"},
                    new AnOption(){Type = "B160", Designation = "160 MHz Analysis Bandwidth (for R&S®FSVA4 and R&S®FSVA7)", Globaltype = "160 MHz Analysis Bandwidth"},
                    new AnOption(){Type = "B160", Designation = "160 MHz Analysis Bandwidth (for R&S®FSVA13)", Globaltype = "160 MHz Analysis Bandwidth"},
                    new AnOption(){Type = "B160", Designation = "160 MHz Analysis Bandwidth (for R&S®FSVA30 und R&S®FSVA40)", Globaltype = "160 MHz Analysis Bandwidth"},
                    new AnOption(){Type = "K0", Designation = "DEMO OPTION (All inclusive)", Globaltype = "DEMO OPTION (All inclusive)"},
                    new AnOption(){Type = "K7", Designation = "Analog Modulation Analysis (AM/FM/φM)", Globaltype = "Analog Modulation Analysis"},
                    new AnOption(){Type = "K7S", Designation = "FM Stereo Measurements (for R&S®FSV-K7)", Globaltype = "FM Stereo Measurements"},
                    new AnOption(){Type = "K8", Designation = "Bluetooth®/EDR Measurement Application", Globaltype = "Bluetooth®/EDR Measurement Application"},
                    new AnOption(){Type = "K9", Designation = "Power Sensor Support (power measurement with the R&S®NRP-Zxx power sensors)", Globaltype = "Power Sensor Support"},
                    new AnOption(){Type = "K10", Designation = "GSM/EDGE/EDGE Evolution Analysis", Globaltype = "GSM/EDGE/EDGE Evolution Analysis"},
                    new AnOption(){Type = "K14", Designation = "Spectrogram Measurements", Globaltype = "Spectrogram Measurements"},
                    new AnOption(){Type = "K30", Designation = "Noise Figure and Gain Measurements", Globaltype = "Noise Figure and Gain Measurements"},
                    new AnOption(){Type = "K40", Designation = "Phase Noise Measurements", Globaltype = "Phase Noise Measurements"},
                    new AnOption(){Type = "K54", Designation = "EMI Measurement Application", Globaltype = "EMI Measurement Application"},
                    new AnOption(){Type = "K70", Designation = "Vector Signal Analysis", Globaltype = "Vector Signal Analysis"},
                    new AnOption(){Type = "K72", Designation = "3GPP BS (DL) Analysis, incl. HSDPA and HSDPA+", Globaltype = "3GPP BS (DL) Analysis, incl. HSDPA and HSDPA+"},
                    new AnOption(){Type = "K73", Designation = "3GPP UE (UL) Analysis, incl. HSUPA", Globaltype = "3GPP UE (UL) Analysis, incl. HSUPA"},
                    new AnOption(){Type = "K76", Designation = "TD-SCDMA BS Measurements", Globaltype = "TD-SCDMA BS Measurements"},
                    new AnOption(){Type = "K77", Designation = "TD-SCDMA UE Measurements", Globaltype = "TD-SCDMA UE Measurements"},
                    new AnOption(){Type = "K82", Designation = "CDMA2000® BS (DL) Analysis", Globaltype = "CDMA2000® BS (DL) Analysis"},
                    new AnOption(){Type = "K83", Designation = "CDMA2000® MS (UL) Measurements", Globaltype = "CDMA2000® MS (UL) Measurements"},
                    new AnOption(){Type = "K84", Designation = "1xEV-DO BS (DL) Analysis", Globaltype = "1xEV-DO BS (DL) Analysis"},
                    new AnOption(){Type = "K85", Designation = "1xEV-DO MS (UL) Measurements", Globaltype = "1xEV-DO MS (UL) Measurements"},
                    new AnOption(){Type = "K91", Designation = "WLAN IEEE802.11a/b/g/j Analysis", Globaltype = "WLAN IEEE802.11a/b/g/j Analysis"},
                    new AnOption(){Type = "K91N", Designation = "WLAN IEEE802.11n Analysis", Globaltype = "WLAN IEEE802.11n Analysis"},
                    new AnOption(){Type = "K91AC", Designation = "WLAN IEEE802.11ac Analysis", Globaltype = "WLAN IEEE802.11ac Analysis"},
                    new AnOption(){Type = "K91P", Designation = "WLAN IEEE802.11ac Analysis", Globaltype = "WLAN IEEE802.11ac Analysis"},
                    new AnOption(){Type = "K93", Designation = "WiMAX™ IEEE802.16e OFDM/OFDMA Analysis", Globaltype = "WiMAX™ IEEE802.16e OFDM/OFDMA Analysis"},
                    new AnOption(){Type = "K100", Designation = "EUTRA/LTE FDD Downlink Analysis", Globaltype = "EUTRA/LTE FDD Downlink Analysis"},
                    new AnOption(){Type = "K101", Designation = "EUTRA/LTE FDD Uplink Analysis", Globaltype = "EUTRA/LTE FDD Uplink Analysis"},
                    new AnOption(){Type = "K102", Designation = "EUTRA/LTE Downlink MIMO Analysis", Globaltype = "EUTRA/LTE Downlink MIMO Analysis"},
                    new AnOption(){Type = "K104", Designation = "EUTRA/LTE TDD Downlink Analysis", Globaltype = "EUTRA/LTE TDD Downlink Analysis"},
                    new AnOption(){Type = "K105", Designation = "EUTRA/LTE TDD Uplink Analysis", Globaltype = "EUTRA/LTE TDD Uplink Analysis"},
                    new AnOption(){Type = "K96PC", Designation = "OFDM Vector Signal Analysis Software", Globaltype = "OFDM Vector Signal Analysis Software"},
                    new AnOption(){Type = "K130PC", Designation = "Distortion Analysis Software", Globaltype = "Distortion Analysis Software"},
                    #endregion
                },
                DefaultInstrOption = new List<AnOption>() { },
                LoadedInstrOption = new List<AnOption>() { },
                TraceType = new ObservableCollection<ParamWithUI>
                {
                    #region
                    new ParamWithUI {UI = "Clear Write", Parameter = "WRIT" },
                    new ParamWithUI {UI = "Avarege", Parameter = "AVER" },
                    new ParamWithUI {UI = "Max Hold", Parameter = "MAXH" },
                    new ParamWithUI {UI = "Min Hold", Parameter = "MINH" },
                    new ParamWithUI {UI = "View", Parameter = "VIEW" },
                    new ParamWithUI {UI = "Blank", Parameter = "BLAN" }
                    #endregion
                },
                TraceDetector = new List<TrDetector>
                {
                    #region
                    new TrDetector {UI = "Auto Select", Parameter = "Auto Select" },
                    new TrDetector {UI = "Auto Peak", Parameter = "APE" },
                    new TrDetector {UI = "Average", Parameter = "AVER" },
                    new TrDetector {UI = "Max Peak", Parameter = "POS" },
                    new TrDetector {UI = "Min Peak", Parameter = "NEG" },
                    new TrDetector {UI = "Sample", Parameter = "SAMP" },
                    new TrDetector {UI = "RMS", Parameter = "RMS" }
                    #endregion
                },
                SweepType =  new List<SwpType>
                {
                    #region
                    new SwpType {UI = "Auto", Parameter = "AUTO" },
                    new SwpType {UI = "Sweep", Parameter = "SWE" },
                    new SwpType {UI = "FFT", Parameter = "FFT" }
                    #endregion
                },
                SweepPointArr = new int[]{ 101, 125, 155, 173, 201, 251, 301, 313, 345, 401, 501, 601, 625, 691, 701, 801, 901, 1001, 1251, 1383, 1999, 2001, 2501, 2765, 3001, 4001, 5001, 6001, 7001, 8001, 9001, 10001, 11001, 12001, 13001, 14001, 15001, 16001, 17001, 18001, 19001, 20001, 21001, 22001, 23001, 24001, 25001, 26001, 27001, 28001, 29001, 30001, 31001, 32001 },
                DefaultSweepPoint = 691,
                RBWArr = new decimal[]{ 1, 2, 3, 5, 10, 20, 30, 50, 100, 200, 300, 500, 1000, 2000, 3000, 5000, 6250, 10000, 20000, 30000, 50000, 100000, 200000, 300000, 500000, 1000000, 2000000, 3000000, 5000000, 10000000 },
                VBWArr = new decimal[]{ 1, 2, 3, 5, 10, 20, 30, 50, 100, 200, 300, 500, 1000, 2000, 3000, 5000, 10000, 20000, 30000, 50000, 100000, 200000, 300000, 500000, 1000000, 2000000, 3000000, 5000000, 10000000, 20000000, 28000000, 40000000 },
                CouplingRatio = true,
                AttMax = 70,
                AttStep = 5,
                PreAmp = false,
                Battery = false,
                NdB = true,
                OBW = true,
                ChnPow= true
             },
            #endregion FSVA
            #region ESRP
            new AllAnUnqData
            {
                InstrManufacture = 1, NumberOfTrace = 3, ChangeableSweepType = true, SweepPointFix = false, InstrModel = "ESRP",
                HiSpeed = true,
                InstrOption = new List<AnOption>()
                {
                    #region 
                    new AnOption(){Type = "B1", Designation = "Ruggedized Housing", Globaltype = "Ruggedized Housing"},
                    new AnOption(){Type = "B2", Designation = "Preselection and RF Preamplifier", Globaltype = "Preselection and Preamplifier"},
                    new AnOption(){Type = "B22", Designation = "RF Preamplifier (100 kHz to 7 GHz)", Globaltype = "Preamplifier"},
                    new AnOption(){Type = "B4", Designation = "OCXO, Precision Reference Frequency", Globaltype = "OCXO, Precision Reference Frequency"},
                    new AnOption(){Type = "B4", Designation = "OCXO, Enhanced Frequency Stability", Globaltype = "OCXO, Enhanced Frequency Stability"},
                    new AnOption(){Type = "B9", Designation = "Tracking Generator, 9 kHz to 7 GHz", Globaltype = "Tracking Generator"},
                    new AnOption(){Type = "B18", Designation = "Solid State Drive (SSD, removable)", Globaltype = "SSD"},
                    new AnOption(){Type = "B19", Designation = "Spare Hard Disk Drive (HDD, removable)", Globaltype = "HDD"},
                    new AnOption(){Type = "B29", Designation = "Frequency Extension 10 Hz, including EMI bandwidths in decade steps", Globaltype = "Low Start Frequency"},
                    new AnOption(){Type = "B30", Designation = "DC Power Supply, 12 V to 15 V", Globaltype = "DC Power Supply"},
                    new AnOption(){Type = "B32", Designation = "Lithium-Ion Battery Pack", Globaltype = "Lithium-Ion Battery Pack"},
                    new AnOption(){Type = "K0", Designation = "DEMO OPTION (All inclusive)", Globaltype = "DEMO OPTION (All inclusive)"},
                    new AnOption(){Type = "K53", Designation = "Time Domain Scan", Globaltype = "Time Domain Scan"},
                    new AnOption(){Type = "K56", Designation = "IF Analysis", Globaltype = "IF Analysis"},
                    #endregion
                },
                DefaultInstrOption = new List<AnOption>() { },
                LoadedInstrOption = new List<AnOption>() { },
                TraceType = new ObservableCollection<ParamWithUI>
                {
                    #region
                    new ParamWithUI {UI = "Clear Write", Parameter = "WRIT" },
                    new ParamWithUI {UI = "Avarege", Parameter = "AVER" },
                    new ParamWithUI {UI = "Max Hold", Parameter = "MAXH" },
                    new ParamWithUI {UI = "Min Hold", Parameter = "MINH" },
                    new ParamWithUI {UI = "View", Parameter = "VIEW" },
                    new ParamWithUI {UI = "Blank", Parameter = "BLAN" }
                    #endregion
                },
                TraceDetector = new List<TrDetector>
                {
                    #region
                    new TrDetector {UI = "Auto Select", Parameter = "Auto Select" },
                    new TrDetector {UI = "Auto Peak", Parameter = "APE" },
                    new TrDetector {UI = "Average", Parameter = "AVER" },
                    new TrDetector {UI = "Max Peak", Parameter = "POS" },
                    new TrDetector {UI = "Min Peak", Parameter = "NEG" },
                    new TrDetector {UI = "Sample", Parameter = "SAMP" },
                    new TrDetector {UI = "RMS", Parameter = "RMS" }
                    #endregion
                },
                SweepType =  new List<SwpType>
                {
                    #region
                    new SwpType {UI = "Auto", Parameter = "AUTO" },
                    new SwpType {UI = "Sweep", Parameter = "SWE" },
                    new SwpType {UI = "FFT", Parameter = "FFT" }
                    #endregion
                },
                SweepPointArr = new int[]{ 101, 125, 155, 173, 201, 251, 301, 313, 345, 401, 501, 601, 625, 691, 701, 801, 901, 1001, 1251, 1383, 1999, 2001, 2501, 2765, 3001, 4001, 5001, 6001, 7001, 8001, 9001, 10001, 11001, 12001, 13001, 14001, 15001, 16001, 17001, 18001, 19001, 20001, 21001, 22001, 23001, 24001, 25001, 26001, 27001, 28001, 29001, 30001, 31001, 32001 },
                DefaultSweepPoint = 691,
                RBWArr = new decimal[]{ 1, 2, 3, 5, 10, 20, 30, 50, 100, 200, 300, 500, 1000, 2000, 3000, 5000, 6250, 10000, 20000, 30000, 50000, 100000, 200000, 300000, 500000, 1000000, 2000000, 3000000, 5000000, 10000000 },
                VBWArr = new decimal[]{ 1, 2, 3, 5, 10, 20, 30, 50, 100, 200, 300, 500, 1000, 2000, 3000, 5000, 10000, 20000, 30000, 50000, 100000, 200000, 300000, 500000, 1000000, 2000000, 3000000, 5000000, 10000000, 20000000, 28000000, 40000000 },
                CouplingRatio = true,
                AttMax = 70,
                AttStep = 5,
                PreAmp = false,
                Battery = false,
                NdB = true,
                OBW = true,
                ChnPow= true
            },
            #endregion ESRP
            #region FPH
            new AllAnUnqData
            {
                InstrManufacture = 1, NumberOfTrace = 1, ChangeableSweepType = false, SweepPointFix = true, InstrModel = "FPH",
                HiSpeed = false,
                InstrOption = new List<AnOption>()
                {
                    #region 
                    new AnOption(){Type = "B22", Designation = "Preamplifier", Globaltype = "Preamplifier"},
                    new AnOption(){Type = "B3", Designation = "Frequency Range to 3 GHz", Globaltype = "Frequency Range to 3 GHz"},
                    new AnOption(){Type = "B4", Designation = "Frequency Range to 4 GHz", Globaltype = "Frequency Range to 4 GHz"},
                    new AnOption(){Type = "K0", Designation = "DEMO OPTION (All inclusive)", Globaltype = "DEMO OPTION (All inclusive)"},
                    new AnOption(){Type = "K7", Designation = "Analog Modulation Analysis AM/FM", Globaltype = "Analog Modulation Analysis AM/FM"},
                    new AnOption(){Type = "K9", Designation = "Power Sensor Support", Globaltype = "Power Sensor Support"},
                    new AnOption(){Type = "K15", Designation = "Interference Analysis", Globaltype = "Interference Analysis"},
                    new AnOption(){Type = "K16", Designation = "Signal Strength Mapping", Globaltype = "Signal Strength Mapping"},
                    new AnOption(){Type = "K19", Designation = "Channel Power Meter", Globaltype = "Channel Power Meter"},
                    new AnOption(){Type = "K29", Designation = "Pulse Measurements with Power Sensor", Globaltype = "Pulse Measurements with Power Sensor"},
                    new AnOption(){Type = "K43", Designation = "Receiver Mode", Globaltype = "Receiver Mode"},
                    #endregion
                },
                DefaultInstrOption = new List<AnOption>() { },
                LoadedInstrOption = new List<AnOption>() { },
                TraceType = new ObservableCollection<ParamWithUI>
                {
                    #region
                    new ParamWithUI {UI = "Clear Write", Parameter = "WRIT" },
                    new ParamWithUI {UI = "Avarege", Parameter = "AVER" },
                    new ParamWithUI {UI = "Max Hold", Parameter = "MAXH" },
                    new ParamWithUI {UI = "Min Hold", Parameter = "MINH" },
                    new ParamWithUI {UI = "View", Parameter = "VIEW" },
                    new ParamWithUI {UI = "Blank", Parameter = "BLAN" }    
                    #endregion
                },
                TraceDetector = new List<TrDetector>
                {
                    #region
                    new TrDetector {UI = "Auto Detector", Parameter = "Auto Select" },
                    new TrDetector {UI = "Auto Peak", Parameter = "APE" },
                    new TrDetector {UI = "Max Peak", Parameter = "POS" },
                    new TrDetector {UI = "Min Peak", Parameter = "NEG" },
                    new TrDetector {UI = "Sample", Parameter = "SAMP" },
                    new TrDetector {UI = "RMS", Parameter = "RMS" }
                    #endregion
                },
                SweepType =  new List<SwpType> {},
                SweepPointArr = new int[]{ 711 },
                DefaultSweepPoint = 711,
                RBWArr = new decimal[]{ 1, 3, 10, 30, 100, 300, 1000, 3000, 10000, 30000, 100000, 300000, 1000000, 3000000 },
                VBWArr = new decimal[]{ 1, 3, 10, 30, 100, 300, 1000, 3000, 10000, 30000, 100000, 300000, 1000000, 3000000 },
                CouplingRatio = false,
                AttMax = 40,
                AttStep = 5,
                RangeArr = new decimal[] {5, 10, 20, 30, 50, 100, 120, 130, 150 },
                PreAmp = false,
                Battery = true,
                NdB = true,
                OBW = true,
                ChnPow= true,
                RangeFixed = true,
            },
            #endregion FPH
            #region FSH
            new AllAnUnqData
            {
                InstrManufacture = 1, NumberOfTrace = 1, ChangeableSweepType = false, SweepPointFix = true, InstrModel = "FSH",
                HiSpeed = false,
                InstrOption = new List<AnOption>()
                {
                    #region 
                     new AnOption(){Type = "B22", Designation = "Preamplifier", Globaltype = "Preamplifier"},
                     new AnOption(){Type = "K1", Designation = "Spectrum Analysis Application", Globaltype = "Spectrum Analysis Application"},
                     new AnOption(){Type = "K0", Designation = "DEMO OPTION (All inclusive)", Globaltype = "DEMO OPTION (All inclusive)"},
                     new AnOption(){Type = "K9", Designation = "Power Meter Measurement Application with R&S®FSH-Zxx or R&S®NRP-Zxx power sensors", Globaltype = "Power Sensor Support"},
                     new AnOption(){Type = "K14", Designation = "Spectrogram Measurement Application", Globaltype = "Spectrogram Measurement Application"},
                     new AnOption(){Type = "K19", Designation = "Channel Power Meter", Globaltype = "Channel Power Meter"},
                     new AnOption(){Type = "K29", Designation = "Pulse Measurements with Power Sensor", Globaltype = "Pulse Measurements with Power Sensor"},
                     new AnOption(){Type = "K39", Designation = "Transmission Measurement Application", Globaltype = "Transmission Measurement Application"},
                     new AnOption(){Type = "K40", Designation = "Remote Control via USB or LAN Application", Globaltype = "Remote Control"},
                     new AnOption(){Type = "K42", Designation = "Vector Network Analysis Application", Globaltype = "Vector Network Analysis Application"},
                     new AnOption(){Type = "K45", Designation = "Vector Voltmeter Measurement Application", Globaltype = "Vector Voltmeter Measurement Application"},
                     #endregion
                },
                DefaultInstrOption = new List<AnOption>()
                {
                    #region 
                    new AnOption(){Type = "B22", Designation = "Preamplifier", Globaltype = "Preamplifier"}
                    #endregion
                },
                LoadedInstrOption = new List<AnOption>(),
                TraceType = new ObservableCollection<ParamWithUI>
                {
                    #region
                    new ParamWithUI {UI = "Clear Write", Parameter = "WRIT" },
                    new ParamWithUI {UI = "Avarege", Parameter = "AVER" },
                    new ParamWithUI {UI = "Max Hold", Parameter = "MAXH" },
                    new ParamWithUI {UI = "Min Hold", Parameter = "MINH" },
                    new ParamWithUI {UI = "View", Parameter = "VIEW" },
                    new ParamWithUI {UI = "Blank", Parameter = "BLAN" }
                    #endregion
                },
                TraceDetector = new List<TrDetector>
                {
                    #region
                    new TrDetector {UI = "Auto Detector", Parameter = "Auto Select" },
                    new TrDetector {UI = "Auto Peak", Parameter = "APE" },
                    new TrDetector {UI = "Max Peak", Parameter = "POS" },
                    new TrDetector {UI = "Min Peak", Parameter = "NEG" },
                    new TrDetector {UI = "Sample", Parameter = "SAMP" },
                    new TrDetector {UI = "RMS", Parameter = "RMS" }
                    #endregion
                },
                SweepType =  new List<SwpType> {},
                SweepPointArr = new int[]{ 550 },
                DefaultSweepPoint = 550,
                RBWArr = new decimal[]{ 1, 3, 10, 30, 100, 300, 1000, 3000, 10000, 30000, 100000, 300000, 1000000, 3000000 },
                VBWArr = new decimal[]{ 1, 3, 10, 30, 100, 300, 1000, 3000, 10000, 30000, 100000, 300000, 1000000, 3000000 },
                CouplingRatio = false,
                AttMax = 40,
                AttStep = 5,
                PreAmp = false,
                Battery = true,
                NdB = true,
                OBW = false,
                ChnPow= false
            },
            #endregion FSH
            #region ZVH
            new AllAnUnqData
            {
                InstrManufacture = 1, NumberOfTrace = 1, ChangeableSweepType = false, SweepPointFix = true, InstrModel = "ZVH",
                HiSpeed = false,
                InstrOption = new List<AnOption>()
                {
                    #region 
                    new AnOption(){Type = "K0", Designation = "DEMO OPTION (All inclusive)", Globaltype = "DEMO OPTION (All inclusive)"},
                    new AnOption(){Type = "K1", Designation = "Spectrum Analysis Application", Globaltype = "Spectrum Analysis Application"},
                    new AnOption(){Type = "K9", Designation = "Power Meter Measurement Application with R&S®FSH-Zxx or R&S®NRP-Zxx power sensors", Globaltype = "Power Sensor Support"},
                    new AnOption(){Type = "K14", Designation = "Spectrogram Measurement Application", Globaltype = "Spectrogram Measurement Application"},
                    new AnOption(){Type = "K19", Designation = "Channel Power Meter", Globaltype = "Channel Power Meter"},
                    new AnOption(){Type = "K29", Designation = "Pulse Measurements with Power Sensor", Globaltype = "Pulse Measurements with Power Sensor"},
                    new AnOption(){Type = "K39", Designation = "Transmission Measurement Application", Globaltype = "Transmission Measurement Application"},
                    new AnOption(){Type = "K40", Designation = "Remote Control via USB or LAN Application", Globaltype = "Remote Control"},
                    new AnOption(){Type = "K42", Designation = "Vector Network Analysis Application", Globaltype = "Vector Network Analysis Application"},
                    new AnOption(){Type = "K45", Designation = "Vector Voltmeter Measurement Application", Globaltype = "Vector Voltmeter Measurement Application"},
                    #endregion
                },
                DefaultInstrOption = new List<AnOption>()
                {
                    #region 
                    new AnOption(){Type = "B22", Designation = "Preamplifier", Globaltype = "Preamplifier"}
                    #endregion
                },
                LoadedInstrOption = new List<AnOption>(),
                TraceType = new ObservableCollection<ParamWithUI>
                {
                    #region
                    new ParamWithUI {UI = "Clear Write", Parameter = "WRIT" },
                    new ParamWithUI {UI = "Avarege", Parameter = "AVER" },
                    new ParamWithUI {UI = "Max Hold", Parameter = "MAXH" },
                    new ParamWithUI {UI = "Min Hold", Parameter = "MINH" },
                    new ParamWithUI {UI = "View", Parameter = "VIEW" },
                    new ParamWithUI {UI = "Blank", Parameter = "BLAN" }
                    #endregion
                },
                TraceDetector = new List<TrDetector>
                {
                    #region
                    new TrDetector {UI = "Auto Detector", Parameter = "Auto Select" },
                    new TrDetector {UI = "Auto Peak", Parameter = "APE" },
                    new TrDetector {UI = "Max Peak", Parameter = "POS" },
                    new TrDetector {UI = "Min Peak", Parameter = "NEG" },
                    new TrDetector {UI = "Sample", Parameter = "SAMP" },
                    new TrDetector {UI = "RMS", Parameter = "RMS" }
                    #endregion
                },
                SweepType =  new List<SwpType> {},
                SweepPointArr = new int[]{ 631 },
                DefaultSweepPoint = 631,
                RBWArr = new decimal[]{ 1, 3, 10, 30, 100, 300, 1000, 3000, 10000, 30000, 100000, 300000, 1000000, 3000000 },
                VBWArr = new decimal[]{ 1, 3, 10, 30, 100, 300, 1000, 3000, 10000, 30000, 100000, 300000, 1000000, 3000000 },
                CouplingRatio = false,
                AttMax = 40,
                AttStep = 5,
                PreAmp = false,
                Battery = true,
                NdB = true,
                OBW = true,
                ChnPow= false
            },
            #endregion ZVH
            #region N99
            new AllAnUnqData
            {
                InstrManufacture = 2, NumberOfTrace = 1, ChangeableSweepType = true, SweepPointFix = false, InstrModel = "N99",
                HiSpeed = false,
                InstrOption = new List<AnOption>(),
                DefaultInstrOption = new List<AnOption>(),
                LoadedInstrOption = new List<AnOption>(),
                TraceType = new ObservableCollection<ParamWithUI>
                {
                    #region
                    new ParamWithUI {UI = "Clear Write", Parameter = "CLRW" },
                    new ParamWithUI {UI = "Avarege", Parameter = "AVG" },
                    new ParamWithUI {UI = "Max Hold", Parameter = "MAXH" },
                    new ParamWithUI {UI = "Min Hold", Parameter = "MINH" },
                    new ParamWithUI {UI = "View", Parameter = "VIEW" },
                    new ParamWithUI {UI = "Blank", Parameter = "BLANk" }
                    #endregion
                },
                TraceDetector = new List<TrDetector>
                {
                    #region
                    new TrDetector {UI = "Auto Select", Parameter = "AUTO" },
                    new TrDetector {UI = "Average/RMS", Parameter = "AVER" },
                    new TrDetector {UI = "Max Peak", Parameter = "POS" },
                    new TrDetector {UI = "Min Peak", Parameter = "NEG" },
                    new TrDetector {UI = "Sample", Parameter = "SAMP" },
                    new TrDetector {UI = "Normal", Parameter = "  NORMal" }
                    #endregion
                },
                SweepType =  new List<SwpType>
                {
                    #region
                    new SwpType {UI = "Auto", Parameter = "AUTO" },
                    new SwpType {UI = "Step", Parameter = "STEP" },
                    new SwpType {UI = "FFT", Parameter = "FFT" }
                    #endregion
                },
                SweepPointArr = new int[]{ 101, 201, 301, 401, 501, 601, 625, 691, 701, 801, 901, 1001, 2001, 3001, 4001, 5001, 6001, 7001, 8001, 9001, 10001 },
                DefaultSweepPoint = 401,
                RBWArr = new decimal[]{ 1, 3, 10, 30, 100, 300, 1000, 3000, 10000, 30000, 100000, 300000, 1000000, 3000000, 5000000 },
                VBWArr = new decimal[]{ 1, 3, 10, 30, 100, 300, 1000, 3000, 10000, 30000, 100000, 300000, 1000000, 3000000, 5000000 },
                CouplingRatio = false,
                AttMax = 70,
                AttStep = 5,
                PreAmp = false,
                Battery = true,
                NdB = false,
                OBW = true,
                ChnPow= false
            },
            #endregion N99
            #region MS27
            new AllAnUnqData
            {
                InstrManufacture = 3, NumberOfTrace = 1, ChangeableSweepType = true, SweepPointFix = true, InstrModel = "MS27",
                HiSpeed = false,
                InstrOption = new List<AnOption>(),
                DefaultInstrOption = new List<AnOption>(),
                LoadedInstrOption = new List<AnOption>(),
                TraceType = new ObservableCollection<ParamWithUI>
                {
                    #region
                    new ParamWithUI {UI = "Clear Write", Parameter = "NORM" },
                    new ParamWithUI {UI = "Avarege", Parameter = "AVER" },
                    new ParamWithUI {UI = "Max Hold", Parameter = "MAXH" },
                    new ParamWithUI {UI = "Min Hold", Parameter = "MINH" },
                    new ParamWithUI {UI = "Blank", Parameter = "NONE" }
                    #endregion
                },
                TraceDetector = new List<TrDetector>
                {
                    #region
                    new TrDetector {UI = "Auto Select", Parameter = "AUTO" },
                    new TrDetector {UI = "Average", Parameter = "AVER" },
                    new TrDetector {UI = "Max Peak", Parameter = "POS" },
                    new TrDetector {UI = "Min Peak", Parameter = "NEG" },
                    new TrDetector {UI = "Sample", Parameter = "SAMP" },
                    new TrDetector {UI = "Normal", Parameter = "NORMal" }
                    #endregion
                },
                SweepType =  new List<SwpType>
                {
                    #region
                    new SwpType {UI = "Fast", Parameter = "FAST" },
                    new SwpType {UI = "Performance", Parameter = "PERF" },
                    new SwpType {UI = "No FFT", Parameter = "NOFF" }
                    #endregion
                },
                SweepPointArr = new int[] {551},
                DefaultSweepPoint = 551,
                RBWArr = new decimal[]{ 1, 3, 10, 30, 100, 300, 1000, 3000, 10000, 30000, 100000, 300000, 1000000, 3000000, 5000000 },
                VBWArr = new decimal[]{ 1, 3, 10, 30, 100, 300, 1000, 3000, 10000, 30000, 100000, 300000, 1000000, 3000000 },
                CouplingRatio = false,
                AttMax = 70,
                AttStep = 5,
                PreAmp = false,
                Battery = true,
                NdB = true,
                OBW = true,
                ChnPow= false
            }
            #endregion MS27
        };

        public int PowerRegister
        {
            get { return _PowerRegister; }
            set
            {
                {
                    if (_PowerRegister != value)
                    {
                        _PowerRegister = value;
                        if (value == 0)
                        {
                            PowerRegisterStr = "";
                        }
                        else
                        {
                            //PowerRegisterBackgroundBrush = new SolidColorBrush(Colors.Red);
                            if (value == 4) { PowerRegisterStr = "IF Overload"; }
                            else if (value == 1) { PowerRegisterStr = "RF Overload"; }//правильно
                            else if (value == 2) { PowerRegisterStr = ""; }
                        }
                        OnPropertyChanged("PowerRegister");
                    }
                }
            }
        }
        int _PowerRegister = 0;
        public string PowerRegisterStr
        {
            get { return _PowerRegisterStr; }
            set { _PowerRegisterStr = value; OnPropertyChanged("PowerRegisterStr"); }
        }
        string _PowerRegisterStr = "";

        public string ScreenName
        {
            get { return _ScreenName; }
            set { _ScreenName = value; OnPropertyChanged("ScreenName"); }
        }
        string _ScreenName = "";

        public string Time
        {
            get { return _Time; }
            set { _Time = value; OnPropertyChanged("Time"); }
        }
        string _Time = "";

        #endregion Instr Info

        #region Freq
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

        public decimal MinFreqAnalayser
        {
            get { return _MinFreqAnalayser; }
            set { if (_MinFreqAnalayser != value) { _MinFreqAnalayser = value; OnPropertyChanged("MinFreqAnalayser"); } }
        }
        private decimal _MinFreqAnalayser = 10;

        public decimal MaxFreqAnalayser
        {
            get { return _MaxFreqAnalayser; }
            set { if (_MaxFreqAnalayser != value) { _MaxFreqAnalayser = value; OnPropertyChanged("MaxFreqAnalayser"); } }
        }
        private decimal _MaxFreqAnalayser = 30000000000;
        #endregion Freq

        #region RBW/VBW

        public decimal RBW
        {
            get { return _RBW; }
            set { _RBW = value; OnPropertyChanged("RBW"); }
        }
        private decimal _RBW = 0;

        public int RBWIndex
        {
            get { return _RBWIndex; }
            set
            {
                if (value > UniqueData.RBWArr.Length - 1) _RBWIndex = UniqueData.RBWArr.Length - 1;
                else if (value < 0) _RBWIndex = 0;
                else _RBWIndex = value;
                RBW = UniqueData.RBWArr[_RBWIndex];
            }
        }
        private int _RBWIndex = 0;

        public bool AutoRBW
        {
            get { return _AutoRBW; }
            set { _AutoRBW = value; OnPropertyChanged("AutoRBW"); }
        }
        bool _AutoRBW = false;


        public decimal VBW
        {
            get { return _VBW; }
            set { _VBW = value; OnPropertyChanged("VBW"); }
        }
        private decimal _VBW = 0;

        public int VBWIndex
        {
            get { return _VBWIndex; }
            set
            {
                if (value > UniqueData.VBWArr.Length - 1) _VBWIndex = UniqueData.VBWArr.Length - 1;
                else if (value < 0) _VBWIndex = 0;
                else _VBWIndex = value;
                VBW = UniqueData.VBWArr[_VBWIndex];
            }
        }
        private int _VBWIndex = 0;

        public bool AutoVBW
        {
            get { return _AutoVBW; }
            set { _AutoVBW = value; OnPropertyChanged("AutoVBW"); }
        }
        bool _AutoVBW = false;
        #endregion RBW/VBW

        #region Sweep
        public decimal SweepTime
        {
            get { return _SweepTime; }
            set { _SweepTime = value; OnPropertyChanged("SweepTime"); }
        }
        private decimal _SweepTime = 1;

        public bool AutoSweepTime
        {
            get { return _AutoSweepTime; }
            set { _AutoSweepTime = value; OnPropertyChanged("AutoSweepTime"); }
        }
        private bool _AutoSweepTime = false;

        public int TracePoints
        {
            get { return _TracePoints; }
            set { _TracePoints = value; OnPropertyChanged("TracePoints"); }
        }
        private int _TracePoints = 101;

        public int SweepPoints
        {
            get { return _SweepPoints; }
            set { if (_SweepPoints != value) { _SweepPoints = value; OnPropertyChanged("SweepPoints"); } }
        }
        private int _SweepPoints = 101;

        public int SweepPointsIndex
        {
            get { return _SweepPointsIndex; }
            set
            {
                if (value > UniqueData.SweepPointArr.Length - 1) SweepPointsIndex = UniqueData.SweepPointArr.Length - 1;
                else if (value < 0) SweepPointsIndex = 0;
                else _SweepPointsIndex = value;
                SweepPoints = UniqueData.SweepPointArr[SweepPointsIndex];
            }
        }
        private int _SweepPointsIndex = 0;

        private SwpType _SweepTypeSelected = new SwpType { UI = "", Parameter = "" };
        public SwpType SweepTypeSelected
        {
            get { return _SweepTypeSelected; }
            set { _SweepTypeSelected = value; OnPropertyChanged("SweepTypeSelected"); }
        }
        #endregion

        #region Level
        public decimal RefLevel
        {
            get { return _RefLevel; }
            set { _RefLevel = value; OnPropertyChanged("RefLevel"); }
        }
        private decimal _RefLevel = -40;

        public int RangeIndex
        {
            get { return _RangeIndex; }
            set
            {
                if (value > UniqueData.RangeArr[UniqueData.RangeArr.Length - 1]) _RangeIndex = UniqueData.RangeArr.Length - 1;
                else if (value < 0) _RangeIndex = 0;
                else _RangeIndex = value;
                Range = UniqueData.RangeArr[_RangeIndex];
            }
        }
        private int _RangeIndex = 5;

        public decimal Range
        {
            get { return _Range; }
            set { _Range = value; OnPropertyChanged("Range"); }
        }
        private decimal _Range = 100;

        public decimal LowestLevel
        {
            get { return _LowestLevel; }
            set { _LowestLevel = value; OnPropertyChanged("LowestLevel"); }
        }
        private decimal _LowestLevel = -140;

        public decimal AttLevel
        {
            get { return _AttLevel; }
            set
            {
                if (value > UniqueData.AttMax) _AttLevel = UniqueData.AttMax;
                else if (value < 0) _AttLevel = 0;
                else _AttLevel = value;
                OnPropertyChanged("AttLevel");
            }
        }
        decimal _AttLevel = 0;

        public bool AttAuto
        {
            get { return _AttAuto; }
            set { _AttAuto = value; OnPropertyChanged("AttAuto"); }
        }
        bool _AttAuto = false;

        public bool PreAmp
        {
            get { return _PreAmp; }
            set { _PreAmp = value; OnPropertyChanged("PreAmp"); }
        }
        bool _PreAmp = false;

        public string LevelUnit
        {
            get { return _LevelUnit; }
            set { _LevelUnit = value; OnPropertyChanged("LevelUnit"); }
        }
        private string _LevelUnit = "dBm";

        public int LevelUnitIndex
        {
            get { return _LevelUnitIndex; }
            set
            {
                if (value > LevelUnits.Count() - 1) _LevelUnitIndex = LevelUnits.Count() - 1;
                else if (value < 0) _LevelUnitIndex = 0;
                else _LevelUnitIndex = value;
                LevelUnit = LevelUnits[_LevelUnitIndex].UI; OnPropertyChanged("LevelUnit");
                OnPropertyChanged("LevelUnitIndex");
            }
        }
        private int _LevelUnitIndex = 0;

        public ObservableCollection<LevelUnit> LevelUnits
        {
            get { return _LevelUnits; }
            set { _LevelUnits = value; OnPropertyChanged("LevelUnits"); }
        }
        private ObservableCollection<LevelUnit> _LevelUnits = new ObservableCollection<LevelUnit>()
        {
            new AllLevelUnits().dBm,
            new AllLevelUnits().dBmV,
            new AllLevelUnits().dBµV,
            new AllLevelUnits().dBµVm,
        };
        #endregion Level

        #region Trace

        public int TraceNumberToReset
        {
            get { return _TraceNumberToReset; }
            set
            {
                if (value < 1) _TraceNumberToReset = 1;
                else if (value > 3) _TraceNumberToReset = 3;
                else _TraceNumberToReset = value;
                OnPropertyChanged("TraceNumberToReset");
            }
        }
        private int _TraceNumberToReset = 0;

        public ParamWithUI Trace1Type
        {
            get { return _Trace1Type; }
            set { if (_Trace1Type != value) { _Trace1Type = value; OnPropertyChanged("Trace1Type"); } }
        }
        private ParamWithUI _Trace1Type = new ParamWithUI { UI = "Blank", Parameter = "BLAN" };

        public TrDetector Trace1Detector
        {
            get { return _Trace1Detector; }
            set { if (_Trace1Detector != value) { _Trace1Detector = value; OnPropertyChanged("Trace1Detector"); } }
        }
        private TrDetector _Trace1Detector = new TrDetector { UI = "Auto Select", Parameter = "Auto Select" };

        public ParamWithUI Trace2Type
        {
            get { return _Trace2Type; }
            set { if (_Trace2Type != value) { _Trace2Type = value; OnPropertyChanged("Trace2Type"); } }
        }
        private ParamWithUI _Trace2Type = new ParamWithUI { UI = "Blank", Parameter = "BLAN" };

        public TrDetector Trace2Detector
        {
            get { return _Trace2Detector; }
            set { if (_Trace2Detector != value) { _Trace2Detector = value; OnPropertyChanged("Trace2Detector"); } }
        }
        private TrDetector _Trace2Detector = new TrDetector { UI = "Auto Select", Parameter = "Auto Select" };

        public ParamWithUI Trace3Type
        {
            get { return _Trace3Type; }
            set { if (_Trace3Type != value) { _Trace3Type = value; OnPropertyChanged("Trace3Type"); } }
        }
        private ParamWithUI _Trace3Type = new ParamWithUI { UI = "Blank", Parameter = "BLAN" };

        public TrDetector Trace3Detector
        {
            get { return _Trace3Detector; }
            set { if (_Trace3Detector != value) { _Trace3Detector = value; OnPropertyChanged("Trace3Detector"); } }
        }
        private TrDetector _Trace3Detector = new TrDetector { UI = "Auto Select", Parameter = "Auto Select" };

        public int AveragingCount
        {
            get { return _AveragingCount; }
            set
            {
                if (UniqueData.HiSpeed == true)
                {
                    if (value > 30000) _AveragingCount = 30000;
                    else if (value < 1) _AveragingCount = 1;
                    else _AveragingCount = value;
                }
                else if (UniqueData.HiSpeed == false)
                {
                    if (value > 1000) _AveragingCount = 999;
                    else if (value < 1) _AveragingCount = 1;
                    else _AveragingCount = value;
                }
                OnPropertyChanged("AveragingCount");
            }
        }
        private int _AveragingCount = 10;

        public int NumberOfSweeps
        {
            get { return _NumberOfSweeps; }
            set { _NumberOfSweeps = value; OnPropertyChanged("NumberOfSweeps"); }
        }
        private int _NumberOfSweeps = 0;

        public bool NOSEquallyAC
        {
            get { return _NOSEquallyAC; }
            set { _NOSEquallyAC = value; OnPropertyChanged("NOSEquallyAC"); }
        }
        private bool _NOSEquallyAC = false;


        public tracepoint[] Trace1
        {
            get { return _Trace1; }
            set { _Trace1 = value; OnPropertyChanged("Trace1"); }
        }
        private tracepoint[] _Trace1;
        private bool _Trace1New = false;
        public bool Trace1New
        {
            get { return _Trace1New; }
            set { _Trace1New = value; OnPropertyChanged("Trace1New"); }
        }

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
        #endregion Trace

        #region Markers
        public ObservableCollection<Equipment.Marker> Markers
        {
            get { return _Markers; }
            set { _Markers = value; OnPropertyChanged("Markers"); }
        }
        private ObservableCollection<Equipment.Marker> _Markers = new ObservableCollection<Equipment.Marker>
        {
            new Equipment.Marker() { Index = 1, MarkerType = 0, IndexOnTrace = -1, TraceNumber = new ParamWithUI() { Parameter = "0", UI = "Trace 1"}, Freq = 99000000 },
            new Equipment.Marker() { Index = 2, MarkerType = 0, IndexOnTrace = -1, TraceNumber = new ParamWithUI() { Parameter = "0", UI = "Trace 1"}, Freq = 99000000 },
            new Equipment.Marker() { Index = 3, MarkerType = 0, IndexOnTrace = -1, TraceNumber = new ParamWithUI() { Parameter = "0", UI = "Trace 1"}, Freq = 99000000 },
            new Equipment.Marker() { Index = 4, MarkerType = 0, IndexOnTrace = -1, TraceNumber = new ParamWithUI() { Parameter = "0", UI = "Trace 1"}, Freq = 99000000 },
            new Equipment.Marker() { Index = 5, MarkerType = 0, IndexOnTrace = -1, TraceNumber = new ParamWithUI() { Parameter = "0", UI = "Trace 1"}, Freq = 99000000 },
            new Equipment.Marker() { Index = 6, MarkerType = 0, IndexOnTrace = -1, TraceNumber = new ParamWithUI() { Parameter = "0", UI = "Trace 1"}, Freq = 99000000 },
        };
        public int MarkersIsEnabled
        {
            get { return _MarkersIsEnabled; }
            set { _MarkersIsEnabled = value; OnPropertyChanged("MarkersIsEnabled"); }
        }
        private int _MarkersIsEnabled = 0;
        #endregion Markers

        #region Measurment
        #region NdB
        public bool NdBState
        {
            get { return _NdBState; }
            set
            {
                _NdBState = value;
                if (_NdBState && _OBWState) { _OBWState = false; OnPropertyChanged("OBWState"); }
                OnPropertyChanged("NdBState");
            }
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
        #endregion NdB

        #region OBW
        public bool OBWState
        {
            get { return _OBWState; }
            set
            {
                _OBWState = value;
                if (_OBWState && _NdBState) { _NdBState = false; OnPropertyChanged("NdBState"); }
                OnPropertyChanged("OBWState");
            }
        }
        private bool _OBWState = false;

        public decimal OBWPercent
        {
            get { return _OBWPercent; }
            set
            {
                if (value < 10) _OBWPercent = 10;
                else if (value > 99.9m) _OBWPercent = 99.9m;
                else _OBWPercent = value;
                OnPropertyChanged("OBWPercent");
            }
        }
        private decimal _OBWPercent = 99;

        public decimal OBWChnlBW
        {
            get { return _OBWChnlBW; }
            set
            {
                if (value < 0) _OBWChnlBW = 0;
                else if (value > FreqSpan) _OBWChnlBW = FreqSpan;
                else _OBWChnlBW = value;
                OnPropertyChanged("OBWChnlBW");
            }
        }
        private decimal _OBWChnlBW = 100000;

        public decimal OBWResult
        {
            get { return _OBWResult; }
            set { _OBWResult = value; OnPropertyChanged("OBWResult"); }
        }
        private decimal _OBWResult = 0;
        #endregion OBW

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
            set
            {
                if (value < 0) _ChannelPowerBW = 0;
                else if (value > FreqSpan) _ChannelPowerBW = FreqSpan;
                else _ChannelPowerBW = value;
                OnPropertyChanged("ChannelPowerBW");
            }
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

        #region MeasMon
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

        public bool IsMeasMon
        {
            get { return _IsMeasMon; }
            set
            {
                _IsMeasMon = value;
                if (_IsMeasMon) { an_dm += SetMeasMonAnSettings; SetAllMarkerOff(); an_dm += SetMeasMon; }
                else { an_dm -= SetMeasMon; }
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
        #endregion MeasMon
        public bool AnyTransducerSet
        {
            get { return _AnyTransducerSet; }
            set { _AnyTransducerSet = value; OnPropertyChanged("AnyTransducerSet"); }
        }
        private bool _AnyTransducerSet = false;

        public Transducer TransducerSelected
        {
            get { return _TransducerSelected; }
            set { _TransducerSelected = value; OnPropertyChanged("TransducerSelected"); }
        }
        private Transducer _TransducerSelected;
        #region SomeMeas
        public bool IsSomeMeas
        {
            get { return _IsSomeMeas; }
            set
            {
                _IsSomeMeas = value;
                if (_IsSomeMeas) { an_dm += SetSomeMeas; }
                else { an_dm -= SetSomeMeas; }
                AnyMeas = _IsSomeMeas;
                OnPropertyChanged("IsSomeMeas");
            }
        }
        private bool _IsSomeMeas = false;
        private int SomeMeasIndex = 0;
        public long SomeMeasTimeMeas
        {
            get { return _SomeMeasTimeMeas; }
            set { _SomeMeasTimeMeas = value; OnPropertyChanged("SomeMeasTimeMeas"); }
        }
        private long _SomeMeasTimeMeas = 0;
        public DataSomeMeas SomeMeasItem
        {
            get { return _SomeMeasItem; }
            set { _SomeMeasItem = value; OnPropertyChanged("SomeMeasItem"); }
        }
        private DataSomeMeas _SomeMeasItem = new DataSomeMeas() { };
        #endregion SomeMeas
        #endregion Meas

        #region Battery
        private decimal _BatteryCharge = 0;
        public decimal BatteryCharge
        {
            get { return _BatteryCharge; }
            set { _BatteryCharge = value; OnPropertyChanged("BatteryCharge"); }
        }

        public bool BatteryCharging
        {
            get { return _BatteryCharging; }
            set { _BatteryCharging = value; OnPropertyChanged("BatteryCharging"); }
        }
        private bool _BatteryCharging = false;
        #endregion Battery
        public Analyzer()
        {

        }
        public void Connect()
        {
            if (Sett.Analyzer_Settings.IPAdress != "")
            {
                an_dm = SetConnect;
                AnThread = new Thread(allworks);
                AnThread.Name = "AnalyzerThread";
                AnThread.IsBackground = true;
                AnThread.Start();
                //an_dm += SomeWork;

                antmr.AutoReset = true;
                antmr.Enabled = true;
                antmr.Elapsed += WatchDog;
                antmr.Start();
            }
            else
            {
                Run = false;
                string str = ((SplashWindow)App.Current.MainWindow).m_mainWindow.FindResource("NotSetIPAddressEquipment").ToString()
                    .Replace("*Equipment*", ((SplashWindow)App.Current.MainWindow).m_mainWindow.FindResource("EqSpectrumAnalyzer").ToString());
                ((SplashWindow)App.Current.MainWindow).m_mainWindow.Message = str;
            }
        }
        private void WatchDog(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (IsRuning && Run == true && new TimeSpan(DateTime.Now.Ticks - LastUpdate) > new TimeSpan(0, 0, 0, 0, (int)(SweepTime * 1000 + 500)))
            {
                IsRuning = false;
            }
        }
        public void SetConnect()
        {
            try
            {
                session = (TcpipSession)ResourceManager.GetLocalManager().Open(String.Concat("TCPIP0::", Sett.Analyzer_Settings.IPAdress, "::inst0::INSTR"));
                //:SYSTem:VERSion?
                //SYST:ERR?
                //SYST:TIME?  23, 24, 27.926
                Thread.Sleep(25);
                string[] temp = session.Query("*IDN?").Trim('"').Split(',');
                int InstrManufacrure = 0;
                string InstrModel = "", SerialNumber = "";
                if (temp[0].Contains("Rohde&Schwarz")) InstrManufacrure = 1;
                else if (temp[0].Contains("Keysight Technologies")) InstrManufacrure = 2;
                else if (temp[0].Contains("Anritsu")) InstrManufacrure = 3;
                InstrModel = temp[1];
                SerialNumber = temp[2];
                bool st = false;
                if (InstrManufacrure == 1)
                    device_meas.manufacture = "Rohde&Schwarz";
                else if (InstrManufacrure == 2)
                    device_meas.manufacture = "Keysight Technologies";
                else if (InstrManufacrure == 3)
                    device_meas.manufacture = "Anritsu";
                device_meas.model = InstrModel;
                device_meas.sn = SerialNumber;
                for (int i = 0; i < AllUniqueData.Count; i++)
                {
                    #region
                    if (AllUniqueData[i].InstrManufacture == InstrManufacrure)
                    {
                        if (InstrModel.Contains(AllUniqueData[i].InstrModel))
                        {
                            UniqueData = AllUniqueData[i];
                            List<AnOption> Loaded = new List<AnOption>() { };
                            UniqueData.LoadedInstrOption = new List<AnOption>();
                            foreach (AnOption dop in UniqueData.DefaultInstrOption)
                            {
                                Loaded.Add(dop);
                            }
                            string[] op = session.Query("*OPT?").TrimEnd().ToUpper().Split(',');
                            if (op.Length > 0 && op[0] != "0")
                            {
                                bool findDemoOption = false;
                                foreach (string s in op)
                                {
                                    if (s.ToUpper() == "K0")
                                    {
                                        findDemoOption = true;
                                        Loaded = UniqueData.InstrOption;
                                    }

                                }
                                if (findDemoOption == false)
                                {
                                    foreach (string s in op)
                                    {
                                        foreach (AnOption so in UniqueData.InstrOption)
                                        {
                                            if (so.Type == s)
                                            {
                                                Loaded.Add(so);
                                            }
                                        }
                                        //Loaded.Add(a.InstrOption.Find(item => item.Type.ToUpper() == s.ToUpper()));
                                    }
                                }
                            }
                            UniqueData.LoadedInstrOption = Loaded;
                        }
                    }
                    #endregion
                }
                _StateResult = st;
                if (UniqueData.InstrManufacture == 1)
                {
                    #region
                    if (UniqueData.HiSpeed == true)
                    { session.Write(":FORM:DATA ASC"); }//передавать трейс в ASCII
                    else if (UniqueData.HiSpeed == false) { session.Write("FORM:DATA REAL,32"); session.Write("INST SAN"); }
                    if (Sett.Analyzer_Settings.DisplayUpdate) { session.Write(":SYST:DISP:UPD ON"); }
                    else { session.Write(":SYST:DISP:UPD OFF"); }
                    if (UniqueData.HiSpeed == true)
                    {
                        SweepPoints = int.Parse(session.Query(":SWE:POIN?").Replace('.', ','));
                        Trace2Type = UniqueData.TraceType[5];
                        Trace3Type = UniqueData.TraceType[5];
                    }
                    else if (UniqueData.HiSpeed == false)
                    {
                        SweepPoints = UniqueData.DefaultSweepPoint;
                        TracePoints = SweepPoints;
                        Trace2Type = UniqueData.TraceType[5];
                        Trace3Type = UniqueData.TraceType[5];
                    }
                    session.DefaultBufferSize = SweepPoints * 18 + 25; //увеличиваем буфер чтобы влезло 32001 точка трейса
                    MinFreqAnalayser = decimal.Parse(session.Query(":SENSe:FREQuency:STAR? MIN").Replace('.', ','));
                    MaxFreqAnalayser = decimal.Parse(session.Query(":SENSe:FREQuency:STOP? MAX").Replace('.', ','));

                    UniqueData.PreAmp = false;
                    if (UniqueData.LoadedInstrOption != null && UniqueData.LoadedInstrOption.Count > 0)
                        for (int i = 0; i < UniqueData.LoadedInstrOption.Count(); i++)
                        {
                            if (UniqueData.LoadedInstrOption[i].Globaltype == "Preamplifier") { UniqueData.PreAmp = true; }
                            if (UniqueData.LoadedInstrOption[i].Type == "B25") { UniqueData.AttStep = 1; }
                        }
                    if (!UniqueData.SweepPointFix) SweepPointsIndex = System.Array.IndexOf(UniqueData.SweepPointArr, SweepPoints);
                    #endregion
                }
                else if (UniqueData.InstrManufacture == 2)
                {
                    #region
                    session.Write("FORM:DATA REAL,32"); //передавать трейс в ASCII
                    SweepPoints = int.Parse(session.Query(":SENS:SWE:POIN?").Replace('.', ','));
                    SweepPointsIndex = System.Array.IndexOf(UniqueData.SweepPointArr, SweepPoints);

                    TracePoints = SweepPoints;
                    session.DefaultBufferSize = SweepPoints * 4 + 20;
                    if (Sett.Analyzer_Settings.DisplayUpdate) { session.Write(":DISP:ENAB 1"); }
                    else { session.Write(":DISP:ENAB 0"); }
                    Trace2Type = UniqueData.TraceType[5];
                    Trace3Type = UniqueData.TraceType[5];
                    #endregion
                }
                else if (UniqueData.InstrManufacture == 3)
                {
                    #region
                    session.Write("FORM:DATA REAL,32"); //передавать трейс в ASCII
                    SweepPoints = 551;
                    TracePoints = 551;
                    SweepPointsIndex = 0;
                    session.DefaultBufferSize = 2204 + 20;
                    session.Write(":MMEMory:MSIS INT");
                    if (Sett.Analyzer_Settings.DisplayUpdate) { session.Write(":DISP:ENAB 1"); }
                    else { session.Write(":DISP:ENAB 0"); }
                    SweepPointsIndex = System.Array.IndexOf(UniqueData.SweepPointArr, SweepPoints);
                    Trace2Type = UniqueData.TraceType[4];
                    Trace3Type = UniqueData.TraceType[4];
                    #endregion
                }
                #region
                an_dm += GetLevelUnit;
                an_dm += GetFreqCentr;
                an_dm += GetFreqSpan;
                an_dm += GetRBW;
                an_dm += GetAutoRBW;
                an_dm += GetVBW;
                an_dm += GetAutoVBW;

                //an_dm += SetCouplingRatio;
                an_dm += GetSweepTime;
                an_dm += GetAutoSweepTime;
                an_dm += GetSweepType;
                an_dm += GetSweepPoints;
                an_dm += GetRefLevel;
                an_dm += GetRange;
                an_dm += GetAttLevel;
                an_dm += GetAutoAttLevel;
                an_dm += GetPreAmp;
                an_dm += GetTraceType;
                an_dm += GetDetectorType;
                an_dm += GetAverageCount;
                an_dm += GetNumberOfSweeps;
                an_dm += GetRunMarkers;

                an_dm += GetNdB;
                //an_dm += GetOBW;

                //an_dm += GetMarkerTable;

                an_dm += GetChannelPower;
                an_dm += GetTransducer;
                an_dm += GetSelectedTransducer;
                an_dm += GetSetAnSysDateTime;
                an_dm += SomeWork;

                //if (Sett.Screen_Settings.SaveScreenFromInstr)
                //{ dm += SetImageFormat; /*dm += SetNetworkDrive;*/ }
                IsConnected = true;
                #endregion
            }
            #region Exception
            catch (VisaException v_exp)
            {
                App.Current.Dispatcher.Invoke((Action)(() =>
                {
                    MainWindow.exp.ExceptionData = new ExData() { ex = v_exp, ClassName = "ANALYZER", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
                }));
            }
            catch (Exception exp)
            {
                App.Current.Dispatcher.Invoke((Action)(() =>
                {
                    MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "ANALYZER", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
                }));
            }
            #endregion
            finally
            {
                an_dm -= SetConnect;
            }
        }
        public void Disconnect()
        {
            an_dm += Dispose;
        }
        private void Dispose()
        {
            DataCycle = false;
            an_dm = null;
            //an_dm -= SomeWork;

            //foreach (Delegate d in an_dm.GetInvocationList())
            //{
            //    an_dm -= (DoubMod)d;
            //}
            //session.Dispose();
            //session = null;
            if (session != null)
            {
                session.Dispose();
                session = null;
            }
            IsRuning = false;
            IsConnected = false;

            AnThread.Abort();
        }
        private void allworks()
        {
            while (_DataCycle)
            {
                long beginTiks = DateTime.Now.Ticks;
                //an_dm();
                if (an_dm != null) { an_dm(); IsRuning = true; LastUpdate = DateTime.Now.Ticks; }
                Time = new TimeSpan(DateTime.Now.Ticks - beginTiks).ToString();

                //foreach (Delegate d in dm.GetInvocationList())
                //{
                //    Time += "\r\n" + d.Method.Name.ToString();
                //}
            }
        }
        public void SetToDelegate(DoubMod m)
        {
            if (an_dm != null && m != null)
            {
                bool find = false;
                foreach (Delegate d in an_dm.GetInvocationList())
                {
                    if (d.Method.Name == m.GetInvocationList()[0].Method.Name) find = true;
                }
                if (find == false) an_dm += (DoubMod)m.GetInvocationList()[0];
            }
        }
        public void SomeWork()
        {
            /// <summary>
            /// Получаем данные Trace
            /// </summary>
            try
            {
                #region Tr1
                if (Trace1Type.UI != "Blank")
                {
                    Trace1New = false;
                    if (UniqueData.InstrManufacture == 1)
                    {
                        if (UniqueData.HiSpeed == true)
                        {
                            string r = session.Query("TRAC:DATA? TRACE1");//TRAC:DATA? TRACE1
                            //MessageBox.Show(r.Split(',').Length.ToString());
                            string[] responseString = r.Split(',');
                            for (int i = 0; i < responseString.Length; i++)
                            {
                                double lev = double.Parse(responseString[i], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture); //decimal.Parse(responseString[i].Replace('.', ','));
                                if (Trace1New == false && Trace1[i].level != lev) Trace1New = true;
                                Trace1[i].level = lev;
                            }
                            //string r = session.Query("TRAC:DATA? TRACE1");//TRAC:DATA? TRACE1
                            //                                              //MessageBox.Show(r.Split(',').Length.ToString());
                            //string[] responseString = r.Split(',');
                            //Trace1[0].level = double.Parse(responseString[0].Replace('.', ','));
                            //Trace1[1] = Trace1[0];
                            //Trace1[responseString.Length + 1].level = double.Parse(responseString[responseString.Length - 1].Replace('.', ','));
                            //for (int i = 1; i < responseString.Length; i++)
                            //{
                            //    double lev = double.Parse(responseString[i].Replace('.', ','));
                            //    if (Trace1New == false && Trace1[i].level != lev) Trace1New = true;
                            //    Trace1[i + 1].level = lev;
                            //}
                        }
                        else if (UniqueData.HiSpeed == false)
                        {
                            session.Write("TRAC:DATA? TRACE1");
                            var byteArray = session.ReadByteArray();
                            int start = 6;
                            if ((byteArray.Length - start) / 4 == TracePoints)
                            {
                                double[] t = new double[((byteArray.Length - start) / 4)];
                                for (int i = 0; i < (byteArray.Length - start) / 4; i++)
                                {
                                    Single tlev = (BitConverter.ToSingle(byteArray, i * 4 + start));
                                    if (tlev != Single.MaxValue)
                                    {
                                        double lev = tlev;
                                        if (Trace1New == false && Trace1[i].level != lev) Trace1New = true;
                                        Trace1[i].level = lev;
                                    }
                                }
                            }
                        }
                    }
                    else if (UniqueData.InstrManufacture == 2)
                    {

                        //SWT = Double.Parse(session.Query(":SWE:TIME:ACT?").Replace('.', ','));
                        session.Write("TRAC1:DATA?");
                        string t = String.Concat("#4", (SweepPoints * 4));
                        MessageBasedSessionReader reader = new MessageBasedSessionReader(session);
                        reader.BinaryEncoding = BinaryEncoding.DefiniteLengthBlockData;
                        reader.BinaryEncoding = BinaryEncoding.RawLittleEndian;
                        var byteArray = session.ReadByteArray(t.Length);
                        float[] l = reader.ReadSingles(SweepPoints * 4);
                        for (int i = 0; i < l.Length; i++)
                        {
                            double lev = l[i];
                            if (Trace1New == false && Trace1[i].level != lev) Trace1New = true;
                            Trace1[i].level = lev;
                        }
                    }
                    else if (UniqueData.InstrManufacture == 3)
                    {
                        SweepTime = decimal.Parse(session.Query(":SWE:TIME:ACT?").Replace('.', ','));
                        Thread.Sleep((int)(SweepTime * 1200));
                        //if (SWT < 0.220) Thread.Sleep(350);
                        //else Thread.Sleep((int)(SWT * 1000) + 100);
                        session.Write("TRAC:DATA? 1"); //TRAC1:DATA?
                        MessageBasedSessionReader reader = new MessageBasedSessionReader(session);
                        reader.BinaryEncoding = BinaryEncoding.DefiniteLengthBlockData;
                        reader.BinaryEncoding = BinaryEncoding.RawLittleEndian;
                        var byteArray = session.ReadByteArray(6);
                        float[] l = reader.ReadSingles(2204);
                        for (int i = 0; i < l.Length; i++)
                        {
                            double lev = l[i];
                            if (Trace1New == false && Trace1[i].level != lev) Trace1New = true;
                            Trace1[i].level = lev;
                        }
                    }
                }
                #endregion
                #region Tr2
                if (Trace2Type.UI != "Blank")
                {
                    if (UniqueData.InstrManufacture == 1 && UniqueData.HiSpeed == true)
                    {
                        string r = session.Query("TRAC:DATA? TRACE2");//TRAC:DATA? TRACE1
                                                                      //MessageBox.Show(r.Split(',').Length.ToString());
                        string[] responseString = r.Split(',');
                        Trace2[0].level = double.Parse(responseString[0].Replace('.', ','));
                        Trace2[1] = Trace2[0];
                        Trace2[responseString.Length + 1].level = double.Parse(responseString[responseString.Length - 1].Replace('.', ','));
                        for (int i = 1; i < responseString.Length; i++)
                        {
                            Trace2[i + 1].level = double.Parse(responseString[i].Replace('.', ','));
                        }
                    }
                    //:SENSe:DETector:FUNCtion?
                }
                #endregion
                #region Tr3
                if (Trace3Type.UI != "Blank")
                {
                    if (UniqueData.InstrManufacture == 1 && UniqueData.HiSpeed == true)
                    {
                        string r = session.Query("TRAC:DATA? TRACE3");//TRAC:DATA? TRACE1
                                                                      //MessageBox.Show(r.Split(',').Length.ToString());
                        string[] responseString = r.Split(',');
                        Trace3[0].level = double.Parse(responseString[0].Replace('.', ','));
                        Trace3[1] = Trace3[0];
                        Trace3[responseString.Length + 1].level = double.Parse(responseString[responseString.Length - 1].Replace('.', ','));
                        for (int i = 1; i < responseString.Length; i++)
                        {
                            Trace3[i + 1].level = double.Parse(responseString[i].Replace('.', ','));
                        }
                    }
                    //:SENSe:DETector:FUNCtion?
                }
                #endregion
                /// <summary>
                /// Принимаем ошибки 
                /// </summary> 
                //if (UniqueData.InstrManufacrure == 1/* && UniqueData.HiSpeed == true*/)
                //{
                //    PowerRegister = int.Parse(session.Query(":STAT:QUES:POW?"));
                //}
                if (ChannelPowerState)
                {
                    GetChannelPowerResult();
                }
            }
            #region Exception
            catch (VisaException v_exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = v_exp, ClassName = "ANALYZER", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            catch (Exception exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "ANALYZER", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            #endregion
            #region Avarege
            if (Trace1Type.UI == "Avarege")
            {
                if (UniqueData.HiSpeed == true)
                {
                    an_dm += GetNumberOfSweeps;
                }
                else if (UniqueData.HiSpeed == false)
                {
                    NumberOfSweeps = AveragingCount;
                    //Trace1Legend = "T(1) " + Trace1Type.UI + " (" + Trace1Detector.UI + ") " + AveragingCount.ToString() + " ";
                }
            }
            if (Trace2Type.UI == "Avarege")
            {
                an_dm += GetNumberOfSweeps;
                //Trace2Legend = "T(2) " + Trace2Type.UI + " (" + Trace2Detector.UI + ") " + NumberOfSweeps.ToString() + "/" + AveragingCount.ToString() + " ";
            }
            if (Trace3Type.UI == "Avarege")
            {
                an_dm += GetNumberOfSweeps;
                //Trace3Legend = "T(3) " + Trace3Type.UI + " (" + Trace3Detector.UI + ") " + NumberOfSweeps.ToString() + "/" + AveragingCount.ToString() + " ";
            }
            #endregion
            GetDeviceInfo();
            SetMarkerData();
            if (IsSomeMeas == true)
            {
                SomeMeasItem.ThisStayOnFrequency = (decimal)(new TimeSpan(DateTime.Now.Ticks - SomeMeasTimeMeas).TotalSeconds);
                //SetTraceDataSomeMeas(SomeMeasItem, Trace1);
                //SomeMeasItem.ThisStayOnFrequency >= SomeMeasItem.StayOnFrequency
            }
            #region MeasMon
            if (IsMeasMon && Trace1New && MeasMonItem != null) // MeasTraceCount > -1)
            {
                if (GSMBandMeas == false)
                {
                    #region
                    if (MeasMonItem.AllTraceCountToMeas > MeasMonItem.AllTraceCount)
                    {
                        bool t1 = MeasMonItem.FreqDN == (decimal)FreqCentr;
                        bool t2 = MeasMonItem.SpecData.FreqSpan == (decimal)FreqSpan;
                        bool t3 = MeasMonItem.ThisIsMaximumSignalAtThisFrequency;
                        //decimal ndblevel = App.Sett.MeasMons_Settings.MeasMons.Where(x => x.Techonology.ToString() == MeasMonItem.Techonology).First().NdBLevel;
                        bool plus = false;
                        if (t1 && t2 && t3)
                        {
                            #region
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

                            if (MeasMonItem.Techonology != "GSM" || ((cfl > dfl + MeasMonItem.BWData.NdBLevel + 5 && cfl > ufl + MeasMonItem.BWData.NdBLevel + 5)))
                            {
                                bool changeTrace = false;
                                #region несовпадает имеющийся трейс из сохранений с текущим по частоте и точкам то затираем на новый
                                if (MeasMonItem.SpecData.Trace == null ||
                                    MeasMonItem.SpecData.Trace[0] == null ||
                                    MeasMonItem.SpecData.Trace.Length != TracePoints ||
                                    MeasMonItem.SpecData.Trace[0].freq != Trace1[0].freq ||
                                    MeasMonItem.SpecData.Trace[MeasMonItem.SpecData.Trace.Length - 1].freq != Trace1[Trace1.Length - 1].freq)//(tt1 || tt2 || tt3 || tt4 || tt5)
                                {
                                    MeasMonItem.SpecData.Trace = new tracepoint[TracePoints];
                                    for (int i = 0; i < TracePoints; i++)
                                    {
                                        MeasMonItem.SpecData.Trace[i] = new tracepoint() { freq = Trace1[i].freq, level = Trace1[i].level };
                                    }
                                    MeasMonItem.SpecData.MeasStart = MainWindow.gps.LocalTime;
                                    MeasMonItem.SpecData.MeasStop = MainWindow.gps.LocalTime;
                                    MeasMonItem.SpecData.LastMeasLatitude = (double)MainWindow.gps.LatitudeDecimal;
                                    MeasMonItem.SpecData.LastMeasLongitude = (double)MainWindow.gps.LongitudeDecimal;
                                    MeasMonItem.SpecData.LastMeasAltitude = (double)MainWindow.gps.Altitude;
                                    MeasMonItem.SpecData.TraceCount = 1;
                                    MeasMonItem.Resets++;
                                    changeTrace = true;
                                }
                                #endregion
                                #region накапливаем трейс и если есть изменения то changeTrace = true                                   
                                else //if (MeasMonItem.Trace[0].Freq == Trace1[0].Freq && MeasMonItem.Trace[MeasMonItem.Trace.Length - 1].Freq == Trace1[Trace1.Length - 1].Freq)
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
                                        MeasMonItem.BWData.BWMeasured = (decimal)(MeasMonItem.SpecData.Trace[mar[2]].freq - MeasMonItem.SpecData.Trace[mar[1]].freq);
                                        MeasMonItem.BWData.NdBResult = mar;
                                        //MeasMonItem.MarkerT2Ind = mar[1];
                                        MeasMonItem.DeltaFreqMeasured = Math.Round(((Math.Abs(((MeasMonItem.SpecData.Trace[mar[1]].freq + MeasMonItem.SpecData.Trace[mar[2]].freq) / 2) - MeasMonItem.SpecData.FreqCentr)) / (MeasMonItem.SpecData.FreqCentr)) * 1000000, 3);
                                        //if (Math.Abs(MeasMonItem.Trace[MeasMonItem.MarkerInd].level - MeasMonItem.Trace[mar[0]].level - MeasMonItem.NdBLevel) < 2 && Math.Abs(MeasMonItem.Trace[MeasMonItem.MarkerInd].level - MeasMonItem.Trace[mar[1]].level - MeasMonItem.NdBLevel) < 2) MeasMonItem.Measured = true;
                                        MeasMonItem.NewDataToSave = true;
                                    }
                                    else
                                    {
                                        MeasMonItem.BWData.NdBResult[1] = -1;
                                        MeasMonItem.BWData.NdBResult[2] = -1;
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
                                #endregion
                            }
                            #endregion
                            if (MeasMonItem.AllTraceCount < MeasMonItem.AllTraceCountToMeas &&
                                MeasMonItem.SpecData.Trace[0].freq == Trace1[0].freq &&
                                MeasMonItem.SpecData.Trace[MeasMonItem.SpecData.Trace.Length - 1].freq == Trace1[Trace1.Length - 1].freq)
                            { MeasMonItem.AllTraceCount++; plus = true; }
                        }
                        else if (!plus && (!t1 || !t2 || !t3))
                        {
                            if (MeasMonItem.AllTraceCount < MeasMonItem.AllTraceCountToMeas &&
                                MeasMonItem.SpecData.Trace[0].freq == Trace1[0].freq &&
                                MeasMonItem.SpecData.Trace[MeasMonItem.SpecData.Trace.Length - 1].freq == Trace1[Trace1.Length - 1].freq)
                            { MeasMonItem.AllTraceCount++; }
                        }
                        if (MeasMonItem.AllTraceCountToMeas == MeasMonItem.AllTraceCount)
                        {
                            MeasMonItem.SpecData.MeasDuration += new TimeSpan(DateTime.Now.Ticks - MeasMonTimeMeas).TotalSeconds;
                            MeasMonItem.SpecData.MeasDuration = Math.Round(MeasMonItem.SpecData.MeasDuration, 4);
                        }
                        Trace1New = false;
                    }

                    #endregion
                }
                else
                {
                    #region 
                    if (GSMBandMeasSelected.CountAll > GSMBandMeasSelected.Count)//if (GSMBandMeasSelected.Start == FreqStart && GSMBandMeasSelected.Stop == FreqStop)
                    {
                        if (Trace1[0].level > -200 && Trace1[Trace1.Length - 1].level > -200)
                        {
                            long Time = DateTime.Now.Ticks;
                            for (int i = 0; i < GSMBandMeasSelected.Trace.Length; i++)
                            {
                                GSMBandMeasSelected.Trace[i].level = Math.Round(LM.MeasChannelPower(Trace1, GSMBandMeasSelected.Trace[i].freq, 200000.0m), 2);
                            }
                            GSMBandMeasSelected.Time = new TimeSpan(DateTime.Now.Ticks - Time);
                            GSMBandMeasSelected.MeasTime = MainWindow.gps.LocalTime;
                            GSMBandMeasSelected.latitude = (double)MainWindow.gps.LatitudeDecimal;
                            GSMBandMeasSelected.longitude = (double)MainWindow.gps.LongitudeDecimal;
                            GSMBandMeasSelected.altitude = (double)MainWindow.gps.Altitude;
                            GSMBandMeasSelected.saved = false;
                            GSMBandMeasSelected.Count++;
                        }
                    }
                    if (GSMBandMeasSelected.id == 0 && GSMBandMeasSelected.Count >= GSMBandMeasSelected.CountAll)
                    {
                        GSMBandMeas = false; GSMBandMeasTicks = MainWindow.gps.LocalTime.Ticks;
                    }
                    #endregion
                    Trace1New = false;
                }
            }
            #endregion
        }

        #region Public Methods
        #region Freq
        public void SetFreqCentr(decimal freqcentr)
        {
            FreqCentr = freqcentr;
            SetToDelegate(SetFreqCentr);
        }
        public void SetFreqSpan(decimal freqspan)
        {
            FreqSpan = freqspan;
            SetToDelegate(SetFreqSpan);
        }
        public void SetFreqStart(decimal freqstart)
        {
            FreqStart = freqstart;
            SetToDelegate(SetFreqStart);
        }
        public void SetFreqStop(decimal freqstop)
        {
            FreqStop = freqstop;
            SetToDelegate(SetFreqStop);
        }
        #endregion Freq

        #region RBW/VBW
        public void SetRBWFromFreq(decimal rbw)
        {
            RBWIndex = System.Array.IndexOf(UniqueData.RBWArr, rbw);
            an_dm += SetRBW;
        }
        public void SetRBWFromIndex(int index)
        {
            RBWIndex = index;
            SetToDelegate(SetRBW);
        }
        public void SetAutoRBW(bool autorbw)
        {
            AutoRBW = autorbw;
            SetToDelegate(SetAutoRBW);
        }

        public void SetVBWFromFreq(decimal vbw)
        {
            VBWIndex = System.Array.IndexOf(UniqueData.VBWArr, vbw);
            SetToDelegate(SetVBW);
        }
        public void SetVBWFromIndex(int index)
        {
            VBWIndex = index;
            SetToDelegate(SetVBW);
        }
        public void SetAutoVBW(bool autovbw)
        {
            AutoVBW = autovbw;
            SetToDelegate(SetAutoVBW);
        }

        #endregion RBW/VBW

        #region Sweep
        public void SetSweepTime(decimal sweeptime)
        {
            SweepTime = sweeptime;
            SetToDelegate(SetSweepTime);
        }
        public void SetAutoSweepTime(bool autosweeptime)
        {
            AutoSweepTime = autosweeptime;
            SetToDelegate(SetAutoSweepTime);
        }
        public void SetAutoSweepType(SwpType sweeptype)
        {
            SweepTypeSelected = sweeptype;
            SetToDelegate(SetSweepType);
        }
        public void SetSweepPoints(int sweeppoints)
        {
            SweepPoints = sweeppoints;
            SetToDelegate(SetSweepPoints);
        }
        #endregion Sweep

        #region Level
        public void SetRefLevel(decimal reflevel)
        {
            RefLevel = reflevel;
            SetToDelegate(SetRefLevel);
        }

        public void SetRange(decimal range)
        {
            Range = range;
            SetToDelegate(SetRange);
        }
        public void SetRangeFromIndex(int rangeindex)
        {
            RangeIndex = rangeindex;
            SetToDelegate(SetRange);
        }

        public void SetAttLevel(decimal attlevel)
        {
            AttLevel = attlevel;
            SetToDelegate(SetAttLevel);
        }

        public void SetAutoAttLevel(bool autoatt)
        {
            AttAuto = autoatt;
            SetToDelegate(SetAutoAttLevel);
        }

        public void SetPreAmp(bool preamp)
        {
            PreAmp = preamp;
            SetToDelegate(SetPreAmp);
        }

        public void SetLevelUnitFromIndex(int levelunitindex)
        {
            LevelUnitIndex = levelunitindex;
            SetToDelegate(SetLevelUnit);
        }
        public void SetLevelUnitFromValue(LevelUnit levelunit)
        {
            for (int i = 0; i < LevelUnits.Count(); i++)
            {
                if (levelunit.ind == LevelUnits[i].ind) { LevelUnitIndex = i; }
            }
            SetToDelegate(SetLevelUnit);
        }
        #endregion Level

        #region Trace
        public void SetTraceType(int TraceNumber, ParamWithUI TraceType)
        {
            if (TraceNumber == 1) { Trace1Type = TraceType; }
            else if (TraceNumber == 2 && UniqueData.HiSpeed == true) { Trace2Type = TraceType; }
            else if (TraceNumber == 3 && UniqueData.HiSpeed == true) { Trace3Type = TraceType; }
            SetToDelegate(SetTraceType);
        }

        public void SetResetTrace(int tracenumber)
        {
            TraceNumberToReset = tracenumber;
            SetToDelegate(SetResetTrace);
        }

        public void SetDetectorType(int TraceNumber, TrDetector TraceDetector)
        {
            if (TraceNumber == 1) { Trace1Detector = TraceDetector; }
            else if (TraceNumber == 2 && UniqueData.HiSpeed == true) { Trace2Detector = TraceDetector; }
            else if (TraceNumber == 3 && UniqueData.HiSpeed == true) { Trace3Detector = TraceDetector; }
            SetToDelegate(SetDetectorType);
        }

        public void SetAverageCount(int averagingcount)
        {
            AveragingCount = averagingcount;
            SetToDelegate(SetAverageCount);
        }
        #endregion Trace

        #region Markers
        public void SetMarkerState(Equipment.Marker marker, bool state)
        {
            marker.StateNew = state;
            if (marker.State == false && marker.StateNew == true)//был выключен
            {
                marker.MarkerTypeNew = 0;
                if (marker.IndexOnTrace < 0)
                {
                    if (marker.TraceNumber.Parameter == "0" && Trace1 != null)
                    {
                        marker.IndexOnTrace = LM.PeakSearch(Trace1);
                        marker.Freq = Trace1[marker.IndexOnTrace].freq;
                    }
                    else if (marker.TraceNumber.Parameter == "1" && Trace2 != null)
                    {
                        marker.IndexOnTrace = LM.PeakSearch(Trace2);
                        marker.Freq = Trace2[marker.IndexOnTrace].freq;
                    }
                    else if (marker.TraceNumber.Parameter == "2" && Trace3 != null)
                    {
                        marker.IndexOnTrace = LM.PeakSearch(Trace3);
                        marker.Freq = Trace3[marker.IndexOnTrace].freq;
                    }
                }
                else
                {
                    if (Trace1 != null && marker.IndexOnTrace > Trace1.Length - 1 && marker.TraceNumber.Parameter == "0")
                    {
                        marker.IndexOnTrace = Trace1.Length - 1;
                        marker.Freq = Trace1[marker.IndexOnTrace].freq;
                    }
                    else if (Trace2 != null && marker.IndexOnTrace > Trace2.Length - 1 && marker.TraceNumber.Parameter == "1")
                    {
                        marker.IndexOnTrace = Trace2.Length - 1;
                        marker.Freq = Trace2[marker.IndexOnTrace].freq;
                    }
                    else if (Trace3 != null && marker.IndexOnTrace > Trace3.Length - 1 && marker.TraceNumber.Parameter == "2")
                    {
                        marker.IndexOnTrace = Trace3.Length - 1;
                        marker.Freq = Trace3[marker.IndexOnTrace].freq;
                    }
                }
            }
            else if (marker.State == true && marker.StateNew == false)
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
            SetToDelegate(SetMarkerState);
        }
        public void SetAllMarkerOff()
        {
            for (int i = 0; i < Markers.Count; i++)
            {
                Markers[i].State = false;
                Markers[i].StateNew = false;
                Markers[i].MarkerType = 0;
                Markers[i].MarkerTypeNew = 0;
            }
            SetToDelegate(SetMarkerAllOff);
        }

        public void SetMarkerFromFreq(Equipment.Marker marker, decimal freq)
        {
            if (marker.State == false)//был выключен
            { marker.State = true; }
            if (marker.TraceNumber.Parameter == "0")
            {
                marker.IndexOnTrace = LM.FindMarkerIndOnTrace(Trace1, freq);
                marker.FreqNew = Trace1[marker.IndexOnTrace].freq;
            }
            else if (marker.TraceNumber.Parameter == "1")
            {
                marker.IndexOnTrace = LM.FindMarkerIndOnTrace(Trace2, freq);
                marker.FreqNew = Trace2[marker.IndexOnTrace].freq;
            }
            else if (marker.TraceNumber.Parameter == "2")
            {
                marker.IndexOnTrace = LM.FindMarkerIndOnTrace(Trace3, freq);
                marker.FreqNew = Trace3[marker.IndexOnTrace].freq;
            }
            SetToDelegate(SetMarkerFreq);
        }

        public void SetMarkerFromIndex(Equipment.Marker marker, int newindex)
        {
            if (marker.State == false)//был выключен
            { marker.State = true; }
            if (newindex < 0) newindex = 0;
            else if (newindex > TracePoints - 1) newindex = TracePoints - 1;
            if (marker.TraceNumber.Parameter == "0")
            {
                marker.IndexOnTrace = newindex;
                marker.FreqNew = Trace1[marker.IndexOnTrace].freq;
            }
            else if (marker.TraceNumber.Parameter == "1")
            {
                marker.IndexOnTrace = newindex;
                marker.FreqNew = Trace2[marker.IndexOnTrace].freq;
            }
            else if (marker.TraceNumber.Parameter == "2")
            {
                marker.IndexOnTrace = newindex;
                marker.FreqNew = Trace3[marker.IndexOnTrace].freq;
            }
            SetToDelegate(SetMarkerFreq);
        }

        public void SetMarkerPeakSearch(Equipment.Marker marker)
        {
            if (marker.State == false)//был выключен
            { marker.State = true; }

            if (marker.TraceNumber.Parameter == "0")
            {
                marker.IndexOnTrace = LM.PeakSearch(Trace1);
                marker.FreqNew = Trace1[marker.IndexOnTrace].freq;
            }
            else if (marker.TraceNumber.Parameter == "1")
            {
                marker.IndexOnTrace = LM.PeakSearch(Trace2);
                marker.FreqNew = Trace2[marker.IndexOnTrace].freq;
            }
            else if (marker.TraceNumber.Parameter == "2")
            {
                marker.IndexOnTrace = LM.PeakSearch(Trace3);
                marker.FreqNew = Trace3[marker.IndexOnTrace].freq;
            }
            SetToDelegate(SetMarkerFreq);
        }

        public void SetMarkerChangeType(Equipment.Marker marker)
        {
            if (marker.State == false)//был выключен
            {
                if (Markers[0].State == false) SetMarkerState(Markers[0], true);
                marker.StateNew = true;
                SetMarkerState(marker, true);
                marker.IndexOnTrace = LM.PeakSearch(Trace1);
                marker.Freq = Trace1[marker.IndexOnTrace].freq;
            }

            if (marker.MarkerTypeNew == 0) { marker.MarkerParent = Markers[0]; marker.MarkerTypeNew = 1; marker.FunctionDataType = 1; }
            else if (marker.MarkerTypeNew == 1) { marker.MarkerParent = null; marker.MarkerTypeNew = 0; marker.FunctionDataType = 0; }
            SetToDelegate(SetMarkerState);
        }

        public void SetMarkerToTrace(Equipment.Marker marker)
        {
            if (marker.State == false)//был выключен
            {
                if (Markers[0].State == false) SetMarkerState(Markers[0], true);
                marker.StateNew = true;
                SetMarkerState(marker, true);
                marker.IndexOnTrace = LM.PeakSearch(Trace1);
                marker.Freq = Trace1[marker.IndexOnTrace].freq;
            }

            if (marker.MarkerTypeNew == 0) { marker.MarkerParent = Markers[0]; marker.MarkerTypeNew = 1; marker.FunctionDataType = 1; }
            else if (marker.MarkerTypeNew == 1) { marker.MarkerParent = null; marker.MarkerTypeNew = 0; marker.FunctionDataType = 0; }
            SetToDelegate(SetMarkerState);
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
                }
            }
        }

        public void SetMarkerNdB()
        {
            SetToDelegate(SetNdB);
        }
        public void SetNdBLevel(decimal ndblevel)
        {
            NdBLevel = ndblevel;
            Markers[0].Funk2 = NdBLevel;
            SetToDelegate(SetNdBLevel);
        }

        public void SetMarkerOBW()
        {
            SetToDelegate(SetOBW);
        }
        public void SetOBWPercent(decimal obwpercent)
        {
            OBWPercent = obwpercent;
            Markers[0].Funk2 = OBWPercent;
            SetToDelegate(SetOBWPercent);
        }
        public void SetOBWChnlBW(decimal obwchnlbw)
        {
            OBWChnlBW = obwchnlbw;
            //Markers[0].Funk2 = OBWPercent;
            SetToDelegate(SetOBWChnlBW);
        }
        #endregion Markers

        #region Meas
        #region ChannelPower
        public void SetChannelPower(bool channelpowerstate)
        {
            ChannelPowerState = channelpowerstate;
            SetToDelegate(SetChannelPower);
        }
        public void SetChannelPowerBW(decimal channelpowerbw)
        {
            ChannelPowerBW = channelpowerbw;
            SetToDelegate(SetChannelPowerBW);
        }
        #endregion ChannelPower

        #region Transducer
        public void SetSelectedTransducer(Transducer transducerselected)
        {
            TransducerSelected = transducerselected;
            SetToDelegate(SetSelectedTransducer);
            SetToDelegate(Dispose);
        }
        #endregion Transducer
        #endregion Meas

        public void SetPreset()
        {
            SetToDelegate(Preset);
        }
        public void GetScreen()
        {
            SetToDelegate(GetScreenFromDevice);
        }
        public void ThisInstrShutdown()
        {
            SetToDelegate(InstrShutdown);
        }
        #endregion Public Methods

        #region private Methods

        #region Freq
        private void GetFreqArr()
        {
            try
            {
                if (UniqueData.InstrManufacture == 1)
                {
                    if (UniqueData.HiSpeed == true)
                    {
                        #region
                        decimal[] FreqTemp = new decimal[SweepPoints];
                        decimal step = (decimal)FreqSpan / (SweepPoints - 1);
                        for (int i = 0; i < SweepPoints; i++)
                        {
                            FreqTemp[i] = (decimal)FreqStart + step * i;
                        }
                        Trace1 = new tracepoint[SweepPoints];
                        Trace2 = new tracepoint[SweepPoints];
                        Trace3 = new tracepoint[SweepPoints];
                        for (int i = 0; i < FreqTemp.Length; i++)
                        {
                            Trace1[i] = new tracepoint() { freq = FreqTemp[i], level = -1000 };
                            Trace2[i] = new tracepoint() { freq = FreqTemp[i], level = -1000 };
                            Trace3[i] = new tracepoint() { freq = FreqTemp[i], level = -1000 };
                        }
                        #endregion
                        #region
                        //decimal step = 0;
                        //decimal[] FreqTemp = new decimal[SweepPoints + 2];
                        //decimal[] Freq = new decimal[SweepPoints + 2];
                        //step = (decimal)Math.Round((FreqSpan / (FreqTemp.Length - 2)), 2);
                        //FreqTemp[0] = (decimal)(FreqCentr - FreqSpan / 2);
                        //FreqTemp[1] = Math.Round(FreqTemp[0] + step / 2, 5);
                        //FreqTemp[FreqTemp.Length - 1] = (decimal)(FreqCentr + FreqSpan / 2);
                        //for (int i = 2; i < FreqTemp.Length; i++)
                        //{
                        //    if (i > 1) { FreqTemp[i] = Math.Round(FreqTemp[i - 1] + step, 5); }
                        //    Freq[i] = Math.Round(FreqTemp[i] / 10, 0) * 10;
                        //}
                        //Trace1 = new tracepoint[SweepPoints + 2];
                        //Trace2 = new tracepoint[SweepPoints + 2];
                        //Trace3 = new tracepoint[SweepPoints + 2];
                        //for (int i = 0; i < Freq.Length; i++)
                        //{
                        //    Trace1[i] = new tracepoint() { freq = Freq[i], level = -1000 };
                        //    Trace2[i] = new tracepoint() { freq = Freq[i], level = -1000 };
                        //    Trace3[i] = new tracepoint() { freq = Freq[i], level = -1000 };
                        //}
                        #endregion
                    }
                    else if (UniqueData.HiSpeed == false)
                    {
                        #region
                        decimal[] FreqTemp = new decimal[TracePoints];
                        decimal step = (decimal)FreqSpan / (TracePoints - 1);
                        for (int i = 0; i < TracePoints; i++)
                        {
                            FreqTemp[i] = (decimal)FreqStart + step * i;
                        }
                        Trace1 = new tracepoint[SweepPoints];
                        Trace2 = new tracepoint[SweepPoints];
                        Trace3 = new tracepoint[SweepPoints];
                        for (int i = 0; i < FreqTemp.Length; i++)
                        {
                            Trace1[i] = new tracepoint() { freq = FreqTemp[i], level = -1000 };
                            Trace2[i] = new tracepoint() { freq = FreqTemp[i], level = -1000 };
                            Trace3[i] = new tracepoint() { freq = FreqTemp[i], level = -1000 };
                        }
                        #endregion
                    }
                }
                else if (UniqueData.InstrManufacture == 2)
                {
                    #region
                    decimal[] FreqTemp = new decimal[TracePoints];
                    decimal step = (decimal)FreqSpan / (TracePoints - 1);
                    for (int i = 0; i < TracePoints; i++)
                    {
                        FreqTemp[i] = (decimal)FreqStart + step * i;
                    }
                    Trace1 = new tracepoint[SweepPoints];
                    Trace2 = new tracepoint[SweepPoints];
                    Trace3 = new tracepoint[SweepPoints];
                    for (int i = 0; i < FreqTemp.Length; i++)
                    {
                        Trace1[i] = new tracepoint() { freq = FreqTemp[i], level = -1000 };
                        Trace2[i] = new tracepoint() { freq = FreqTemp[i], level = -1000 };
                        Trace3[i] = new tracepoint() { freq = FreqTemp[i], level = -1000 };
                    }
                    #endregion
                }
                else if (UniqueData.InstrManufacture == 3)
                {
                    #region
                    decimal[] FreqTemp = new decimal[TracePoints];
                    decimal step = (decimal)FreqSpan / (TracePoints - 1);
                    for (int i = 0; i < TracePoints; i++)
                    {
                        FreqTemp[i] = (decimal)FreqStart + step * i;
                    }
                    Trace1 = new tracepoint[SweepPoints];
                    Trace2 = new tracepoint[SweepPoints];
                    Trace3 = new tracepoint[SweepPoints];
                    for (int i = 0; i < FreqTemp.Length; i++)
                    {
                        Trace1[i] = new tracepoint() { freq = FreqTemp[i], level = -1000 };
                        Trace2[i] = new tracepoint() { freq = FreqTemp[i], level = -1000 };
                        Trace3[i] = new tracepoint() { freq = FreqTemp[i], level = -1000 };
                    }
                    #endregion
                }
            }
            #region Exception
            catch (VisaException v_exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = v_exp, ClassName = "ANALYZER", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            catch (Exception exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "ANALYZER", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            #endregion
            //OnPropertyChanged("ResizeDraw");
            an_dm -= GetFreqArr;
        }
        /// <summary>
        /// Установка центральной частоты 
        /// </summary>
        private void SetFreqCentr()
        {
            try
            {
                if (UniqueData.InstrManufacture == 1)
                {
                    session.Write(":SENSe:FREQuency:CENTer " + FreqCentr.ToString().Replace(',', '.'));
                    FreqCentr = decimal.Parse(session.Query(":SENSe:FREQuency:CENTer?").Replace('.', ','));
                }
                else if (UniqueData.InstrManufacture == 2)
                {
                    session.Write(":FREQ:CENT " + FreqCentr.ToString().Replace(',', '.'));
                    FreqCentr = decimal.Parse(session.Query(":FREQ:CENT?").Replace('.', ','));
                }
                else if (UniqueData.InstrManufacture == 3)
                {
                    session.Write(":SENSe:FREQuency:CENTer " + FreqCentr.ToString().Replace(',', '.'));
                    FreqCentr = decimal.Parse(session.Query(":SENSe:FREQuency:CENTer?").Replace('.', ','));
                }
            }
            #region Exception
            catch (VisaException v_exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = v_exp, ClassName = "ANALYZER", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            catch (Exception exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "ANALYZER", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            #endregion
            GetFreqArr();
            if (AutoRBW == true) { GetRBW(); }
            if (AutoVBW == true) { GetVBW(); }
            if (AutoSweepTime == true) { GetSweepTime(); }
            MarkersTraceLegthOrFreqsChanged(Trace1);
            an_dm -= SetFreqCentr;
        }
        /// <summary>
        /// Получаем центральную частоту 
        /// </summary>
        private void GetFreqCentr()
        {
            try
            {
                if (UniqueData.InstrManufacture == 1)
                {
                    FreqCentr = decimal.Parse(session.Query(":SENSe:FREQuency:CENTer?").Replace('.', ','));
                }
                else if (UniqueData.InstrManufacture == 2)
                {
                    FreqCentr = decimal.Parse(session.Query(":FREQ:CENT?").Replace('.', ','));
                }
                else if (UniqueData.InstrManufacture == 3)
                {
                    FreqCentr = decimal.Parse(session.Query(":SENSe:FREQuency:CENTer?"));
                }
            }
            #region Exception
            catch (VisaException v_exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = v_exp, ClassName = "ANALYZER", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            catch (Exception exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "ANALYZER", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            #endregion
            GetFreqArr();
            an_dm -= GetFreqCentr;
        }


        /// <summary>
        /// Установка span
        /// </summary>
        private void SetFreqSpan()
        {
            try
            {
                if (UniqueData.InstrManufacture == 1)
                {
                    session.Write(":SENSe:FREQuency:SPAN " + FreqSpan.ToString().Replace(',', '.'));
                    FreqSpan = decimal.Parse(session.Query(":SENSe:FREQuency:SPAN?").Replace('.', ','));
                }
                else if (UniqueData.InstrManufacture == 2)
                {
                    session.Write(":FREQuency:SPAN " + FreqSpan.ToString().Replace(',', '.'));
                    FreqSpan = decimal.Parse(session.Query(":FREQuency:SPAN?").Replace('.', ','));
                }
                else if (UniqueData.InstrManufacture == 3)
                {
                    session.Write(":SENSe:FREQuency:SPAN " + FreqSpan.ToString().Replace(',', '.'));
                    FreqSpan = decimal.Parse(session.Query(":SENSe:FREQuency:SPAN?").Replace('.', ','));
                }
            }
            #region Exception
            catch (VisaException v_exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = v_exp, ClassName = "ANALYZER", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            catch (Exception exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "ANALYZER", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            #endregion
            GetFreqArr();
            if (AutoRBW == true) { GetRBW(); }
            if (AutoVBW == true) { GetVBW(); }
            if (AutoSweepTime == true) { GetSweepTime(); }
            MarkersTraceLegthOrFreqsChanged(Trace1);
            an_dm -= SetFreqSpan;
        }
        /// <summary>
        /// получаем span
        /// </summary>
        private void GetFreqSpan()
        {
            try
            {
                if (UniqueData.InstrManufacture == 1)
                {
                    FreqSpan = decimal.Parse(session.Query(":SENSe:FREQuency:SPAN?").Replace('.', ','));
                }
                else if (UniqueData.InstrManufacture == 2)
                {
                    FreqSpan = decimal.Parse(session.Query(":FREQuency:SPAN?").Replace('.', ','));
                }
                else if (UniqueData.InstrManufacture == 3)
                {
                    FreqSpan = decimal.Parse(session.Query(":SENSe:FREQuency:SPAN?"));
                }
            }
            #region Exception
            catch (VisaException v_exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = v_exp, ClassName = "ANALYZER", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            catch (Exception exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "ANALYZER", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            #endregion
            GetFreqArr();
            an_dm -= GetFreqSpan;
        }


        /// <summary>
        /// Установка Начальной частоты просмотра 
        /// </summary>
        private void SetFreqStart()
        {
            try
            {
                if (UniqueData.InstrManufacture == 1)
                {
                    session.Write(":SENSe:FREQ:STAR " + FreqStart.ToString().Replace(',', '.'));
                    FreqStart = decimal.Parse(session.Query(":SENSe:FREQ:STAR?").Replace('.', ','));
                }
                else if (UniqueData.InstrManufacture == 2)
                {
                    session.Write("FREQ:STAR " + FreqStart.ToString().Replace(',', '.'));
                    FreqStart = decimal.Parse(session.Query(":FREQ:STAR?").Replace('.', ','));
                }
                else if (UniqueData.InstrManufacture == 3)
                {
                    FreqCentr = decimal.Parse(session.Query(":SENSe:FREQuency:CENTer?"));
                    FreqSpan = decimal.Parse(session.Query(":SENSe:FREQuency:SPAN?"));
                }
            }
            #region Exception
            catch (VisaException v_exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = v_exp, ClassName = "ANALYZER", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            catch (Exception exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "ANALYZER", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            #endregion
            GetFreqArr();
            if (AutoRBW == true) { GetRBW(); }
            if (AutoVBW == true) { GetVBW(); }
            if (AutoSweepTime == true) { GetSweepTime(); }
            MarkersTraceLegthOrFreqsChanged(Trace1);
            an_dm -= SetFreqStart;
        }
        /// <summary>
        /// Получение Начальной частоты просмотра 
        /// </summary>
        private void GetFreqStart()
        {
            try
            {
                if (UniqueData.InstrManufacture == 1)
                {
                    FreqStart = decimal.Parse(session.Query(":SENSe:FREQ:STAR?").Replace('.', ','));
                }
                else if (UniqueData.InstrManufacture == 2)
                {
                    FreqStart = decimal.Parse(session.Query("FREQ:STAR?").Replace('.', ','));
                }
                else if (UniqueData.InstrManufacture == 3)
                {
                    FreqCentr = decimal.Parse(session.Query(":SENSe:FREQuency:CENTer?"));
                    FreqSpan = decimal.Parse(session.Query(":SENSe:FREQuency:SPAN?"));
                }
            }
            #region Exception
            catch (VisaException v_exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = v_exp, ClassName = "ANALYZER", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            catch (Exception exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "ANALYZER", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            #endregion
            GetFreqArr();
            //:SENSe:FREQuency:STARt 98000000
            //:SENSe:FREQuency:STOP 99000000
            an_dm -= GetFreqStart;
        }


        private void SetFreqStop()
        {
            try
            {
                if (UniqueData.InstrManufacture == 1)
                {
                    session.Write(":SENSe:FREQ:STOP " + FreqStop.ToString().Replace(',', '.'));
                    FreqStop = decimal.Parse(session.Query(":SENSe:FREQ:STOP?").Replace('.', ','));
                }
                else if (UniqueData.InstrManufacture == 2)
                {
                    session.Write(":FREQ:STOP " + FreqStop.ToString().Replace(',', '.'));
                    FreqStop = decimal.Parse(session.Query(":FREQ:STOP?").Replace('.', ','));
                }
                else if (UniqueData.InstrManufacture == 3)
                {
                    FreqCentr = decimal.Parse(session.Query(":SENSe:FREQuency:CENTer?"));
                    FreqSpan = decimal.Parse(session.Query(":SENSe:FREQuency:SPAN?"));
                }
            }
            #region Exception
            catch (VisaException v_exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = v_exp, ClassName = "ANALYZER", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            catch (Exception exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "ANALYZER", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            #endregion
            GetFreqArr();
            if (AutoRBW == true) { GetRBW(); }
            if (AutoVBW == true) { GetVBW(); }
            if (AutoSweepTime == true) { GetSweepTime(); }
            MarkersTraceLegthOrFreqsChanged(Trace1);
            an_dm -= SetFreqStop;
        }
        /// <summary>
        /// Получкние Начальной и Конечной частоты просмотра 
        /// </summary>
        private void GetFreqStop()
        {
            try
            {
                if (UniqueData.InstrManufacture == 1)
                {
                    FreqStop = decimal.Parse(session.Query("SENSe:FREQ:STOP?").Replace('.', ','));
                }
                else if (UniqueData.InstrManufacture == 2)
                {
                    FreqStop = decimal.Parse(session.Query("FREQ:STOP?").Replace('.', ','));
                }
            }
            #region Exception
            catch (VisaException v_exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = v_exp, ClassName = "ANALYZER", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            catch (Exception exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "ANALYZER", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            #endregion
            GetFreqArr();
            if (AutoRBW == true) { GetRBW(); }
            if (AutoVBW == true) { GetVBW(); }
            if (AutoSweepTime == true) { GetSweepTime(); }
            an_dm -= GetFreqStop;
        }
        #endregion Freq

        #region RBW/VBW
        #region RBW
        /// <summary>
        /// Установка RBW 
        /// </summary>
        private void SetRBW()
        {
            try
            {
                if (UniqueData.InstrManufacture == 1)
                {
                    session.Write(":SENSe:BWIDth:RESolution " + RBW.ToString().Replace(',', '.'));
                }
                else if (UniqueData.InstrManufacture == 2)
                {
                    session.Write(":SENS:BAND:RES " + RBW.ToString().Replace(',', '.'));
                }
                else if (UniqueData.InstrManufacture == 3)
                {
                    session.Write(":SENSe:BWIDth:RESolution " + RBW.ToString().Replace(',', '.'));
                }
            }
            #region Exception
            catch (VisaException v_exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = v_exp, ClassName = "ANALYZER", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            catch (Exception exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "ANALYZER", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            #endregion
            AutoRBW = false;
            if (AutoSweepTime == true) { GetSweepTime(); }
            if (AutoVBW == true) { GetVBW(); }
            GetSweepType();
            an_dm -= SetRBW;
        }
        /// <summary>
        /// Получение RBW 
        /// </summary>
        private void GetRBW()
        {
            try
            {
                if (UniqueData.InstrManufacture == 1)
                {
                    RBW = decimal.Parse(session.Query(":SENSe:BWIDth:RESolution?").Replace('.', ','));
                    RBWIndex = System.Array.IndexOf(UniqueData.RBWArr, RBW);
                }
                else if (UniqueData.InstrManufacture == 2)
                {
                    RBW = decimal.Parse(session.Query(":SENS:BAND:RES?").Replace('.', ','));
                    RBWIndex = System.Array.IndexOf(UniqueData.RBWArr, RBW);
                }
                else if (UniqueData.InstrManufacture == 3)
                {
                    RBW = decimal.Parse(session.Query(":SENSe:BWIDth:RESolution?"));
                    RBWIndex = System.Array.IndexOf(UniqueData.RBWArr, RBW);
                }
            }
            #region Exception
            catch (VisaException v_exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = v_exp, ClassName = "ANALYZER", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            catch (Exception exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "ANALYZER", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            #endregion
            //if (AutoSWT == true) { GetSWT(); }
            //if (AutoVBW == true) { GetVBW(); }
            an_dm -= GetRBW;
        }
        /// <summary>
        /// Вкл/Выкл Auto RBW 
        /// </summary>
        private void SetAutoRBW()
        {
            try
            {
                if (UniqueData.InstrManufacture == 1)
                {
                    if (AutoRBW == true) session.Write(":SENSe:BWIDth:RESolution:AUTO 1");
                    if (AutoRBW == false) session.Write(":SENSe:BWIDth:RESolution:AUTO 0");
                }
                else if (UniqueData.InstrManufacture == 2)
                {
                    if (AutoRBW == true) session.Write(":SENS:BWID:RES:AUTO 1");
                    if (AutoRBW == false) session.Write(":SENS:BWID:RES:AUTO 0");
                }
                else if (UniqueData.InstrManufacture == 3)
                {
                    if (AutoRBW == true) session.Write(":SENSe:BWIDth:RESolution:AUTO 1");
                    if (AutoRBW == false) session.Write(":SENSe:BWIDth:RESolution:AUTO 0");
                }
            }
            #region Exception
            catch (VisaException v_exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = v_exp, ClassName = "ANALYZER", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            catch (Exception exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "ANALYZER", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            #endregion
            if (AutoSweepTime == true) { GetSweepTime(); }
            GetRBW();
            GetVBW();
            GetSweepType();
            an_dm -= SetAutoRBW;
        }
        /// <summary>
        /// Получаем состояние Auto RBW 
        /// </summary>
        private void GetAutoRBW()
        {
            try
            {
                if (UniqueData.InstrManufacture == 1)
                {
                    string s = session.Query(":SENSe:BWIDth:RESolution:AUTO?").TrimEnd();
                    if (s.Contains("1")) { AutoRBW = true; }
                    else { AutoRBW = false; }
                }
                else if (UniqueData.InstrManufacture == 2)
                {
                    //    if (session.Query(":SENS:BWID:RES:AUTO?") == "0\n") { AutoRBW = false; }
                    //    else { AutoRBW = true; }
                }
                else if (UniqueData.InstrManufacture == 3)
                {
                    AutoRBW = Boolean.Parse(session.Query(":SENSe:BWIDth:RESolution:AUTO?"));
                }
            }
            #region Exception
            catch (VisaException v_exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = v_exp, ClassName = "ANALYZER", AdditionalInformation = "GetAutoRBW" };
            }
            catch (Exception exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "ANALYZER", AdditionalInformation = "GetAutoRBW" };
            }
            #endregion
            if (AutoSweepTime == true) { GetSweepTime(); }

            an_dm -= GetAutoRBW;
        }
        #endregion RBW

        #region VBW
        /// <summary>
        /// Установка VBW 
        /// </summary>
        private void SetVBW()
        {
            try
            {
                if (UniqueData.InstrManufacture == 1)
                {
                    VBWIndex = System.Array.IndexOf(UniqueData.VBWArr, VBW);
                    session.Write(":SENSe:BANDwidth:VIDeo " + UniqueData.VBWArr[VBWIndex].ToString().Replace(',', '.'));
                }
                else if (UniqueData.InstrManufacture == 2)
                {
                    VBWIndex = System.Array.IndexOf(UniqueData.VBWArr, VBW);
                    session.Write(":SENSe:BANDwidth:VIDeo " + UniqueData.VBWArr[VBWIndex].ToString().Replace(',', '.'));
                }
                else if (UniqueData.InstrManufacture == 3)
                {
                    VBWIndex = System.Array.IndexOf(UniqueData.VBWArr, VBW);
                    session.Write(":SENSe:BANDwidth:VIDeo " + UniqueData.VBWArr[VBWIndex].ToString());
                }
            }
            #region Exception
            catch (VisaException v_exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = v_exp, ClassName = "ANALYZER", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            catch (Exception exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "ANALYZER", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            #endregion
            AutoVBW = false;
            if (AutoSweepTime == true && RBWIndex - VBWIndex > 0) { GetSweepTime(); }
            if (AutoRBW == true) { GetRBW(); }
            GetSweepType();
            an_dm -= SetVBW;
        }
        /// <summary>
        /// Получение VBW 
        /// </summary>
        private void GetVBW()
        {
            try
            {
                if (UniqueData.InstrManufacture == 1)
                {
                    VBW = decimal.Parse(session.Query(":SENSe:BANDwidth:VIDeo?").Replace('.', ','));
                    VBWIndex = System.Array.IndexOf(UniqueData.VBWArr, VBW);
                }
                else if (UniqueData.InstrManufacture == 2)
                {
                    VBW = decimal.Parse(session.Query(":SENS:BAND:VID?").Replace('.', ','));
                    VBWIndex = System.Array.IndexOf(UniqueData.VBWArr, VBW);
                }
                else if (UniqueData.InstrManufacture == 3)
                {
                    VBW = decimal.Parse(session.Query(":SENSe:BANDwidth:VIDeo?"));
                    VBWIndex = System.Array.IndexOf(UniqueData.VBWArr, VBW);
                }
            }
            #region Exception
            catch (VisaException v_exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = v_exp, ClassName = "ANALYZER", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            catch (Exception exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "ANALYZER", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            #endregion
            //if (AutoSWT == true && RBWIndex - VBWIndex > 0) { GetSWT(); }
            an_dm -= GetVBW;
        }
        /// <summary>
        /// Вкл Auto RBW 
        /// </summary>
        private void SetAutoVBW()
        {
            try
            {
                if (UniqueData.InstrManufacture == 1)
                {
                    if (AutoVBW == true) session.Write(":SENSe:BANDwidth:VIDeo:AUTO 1");
                    if (AutoVBW == false) session.Write(":SENSe:BANDwidth:VIDeo:AUTO 0");
                }
                else if (UniqueData.InstrManufacture == 2)
                {
                    if (AutoVBW == true) session.Write(":SENS:BAND:VID:AUTO 1");
                    if (AutoVBW == false) session.Write(":SENS:BAND:VID:AUTO 0");
                }
                else if (UniqueData.InstrManufacture == 3)
                {
                    if (AutoVBW == true) session.Write(":SENSe:BWIDth:VIDeo:AUTO 1");
                    if (AutoVBW == false) session.Write(":SENSe:BWIDth:VIDeo:AUTO 0");
                }
            }
            #region Exception
            catch (VisaException v_exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = v_exp, ClassName = "ANALYZER", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            catch (Exception exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "ANALYZER", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            #endregion
            if (AutoVBW == true) { GetVBW(); }
            if (AutoSweepTime == true) { GetSweepTime(); }
            GetSweepType();
            //:SENSe:BANDwidth|BWIDth:VIDeo:AUTO? 1
            //:SENSe:BWIDth:VIDeo:RATio? 
            an_dm -= SetAutoVBW;
        }
        /// <summary>
        /// Получаем состояние Auto VBW 
        /// </summary>
        private void GetAutoVBW()
        {
            try
            {
                if (UniqueData.InstrManufacture == 1)
                {
                    if (session.Query(":SENSe:BANDwidth:VIDeo:AUTO?").Contains("1")) { AutoVBW = true; }
                    else { AutoVBW = false; }
                }
                else if (UniqueData.InstrManufacture == 2)
                {
                    if (session.Query(":SENS:BAND:VID:AUTO?") == "0\n") { AutoVBW = false; }
                    else { AutoVBW = true; }
                }
                else if (UniqueData.InstrManufacture == 3)
                {
                    AutoVBW = Boolean.Parse(session.Query(":SENSe:BWIDth:VIDeo:AUTO?"));
                }
            }
            #region Exception
            catch (VisaException v_exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = v_exp, ClassName = "ANALYZER", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            catch (Exception exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "ANALYZER", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            #endregion
            //if (AutoSWT == true) { GetSWT(); }
            an_dm -= GetAutoVBW;

        }
        #endregion VBW
        #endregion RBW/VBW

        #region Sweep
        /// <summary>
        /// Установка свиптайма 
        /// </summary>        
        private void SetSweepTime()
        {
            try
            {
                if (UniqueData.InstrManufacture == 1)
                {
                    session.Write(":SWE:TIME " + SweepTime.ToString().Replace(',', '.'));
                    SweepTime = decimal.Parse(session.Query(":SWE:TIME?").Replace('.', ','));
                    AutoSweepTime = false;
                }
                else if (UniqueData.InstrManufacture == 2)
                {
                    session.Write(":SWE:TIME " + SweepTime.ToString().Replace(',', '.'));
                    SweepTime = decimal.Parse(session.Query(":SWE:TIME?").Replace('.', ','));
                    AutoSweepTime = false;
                }
                else if (UniqueData.InstrManufacture == 3)
                {
                    session.Write(":SWE:TIME:ACT " + SweepTime.ToString().Replace(',', '.'));
                    SweepTime = decimal.Parse(session.Query(":SWE:TIME:ACT?").Replace('.', ','));
                    AutoSweepTime = false;
                }
            }
            #region Exception
            catch (VisaException v_exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = v_exp, ClassName = "ANALYZER", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            catch (Exception exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "ANALYZER", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            #endregion
            an_dm -= SetSweepTime;
        }
        /// <summary>
        /// Получаем свиптайм 
        /// </summary>
        private void GetSweepTime()
        {
            try
            {
                if (UniqueData.InstrManufacture == 1)
                {
                    SweepTime = decimal.Parse(session.Query(":SWE:TIME?").Replace('.', ','));
                }
                else if (UniqueData.InstrManufacture == 2)
                {
                    SweepTime = decimal.Parse(session.Query(":SWE:MTIM?").Replace('.', ','));
                }
                else if (UniqueData.InstrManufacture == 3)
                {
                    SweepTime = decimal.Parse(session.Query(":SWE:TIME:ACT?").Replace('.', ','));
                }
            }
            #region Exception
            catch (VisaException v_exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = v_exp, ClassName = "ANALYZER", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            catch (Exception exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "ANALYZER", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            #endregion
            an_dm -= GetSweepTime;
        }
        /// <summary>
        /// Вкл/Выкл Auto свиптайма 
        /// </summary>
        private void SetAutoSweepTime()
        {
            try
            {
                if (UniqueData.InstrManufacture == 1)
                {
                    if (AutoSweepTime == true) session.Write(":SWE:TIME:AUTO 1");
                    if (AutoSweepTime == false) session.Write(":SWE:TIME:AUTO 0");
                }
                else if (UniqueData.InstrManufacture == 2)
                {
                    if (AutoSweepTime == true) session.Write(":SENS:SWE:ACQ:AUTO 1");
                    if (AutoSweepTime == false) session.Write(":SENS:SWE:ACQ:AUTO 0");
                }
                else if (UniqueData.InstrManufacture == 3)
                {
                    if (AutoSweepTime == true) session.Write(":SENS:SWE:AUTO ON");
                    if (AutoSweepTime == false) session.Write(":SENS:SWE:AUTO OFF");
                }
            }
            #region Exception
            catch (VisaException v_exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = v_exp, ClassName = "ANALYZER", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            catch (Exception exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "ANALYZER", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            #endregion
            GetSweepTime();
            an_dm -= SetAutoSweepTime;
        }
        /// <summary>
        /// Получаем состояние Auto SWT 
        /// </summary>
        private void GetAutoSweepTime()
        {
            try
            {
                if (UniqueData.InstrManufacture == 1)
                {
                    if (session.Query(":SWE:TIME:AUTO?") == "0\n") { AutoSweepTime = false; }
                    else { AutoSweepTime = true; }
                }
                else if (UniqueData.InstrManufacture == 2)
                {
                    if (session.Query(":SENS:SWE:ACQ:AUTO?") == "0\n") { AutoSweepTime = false; }
                    else { AutoSweepTime = true; }
                }
            }
            #region Exception
            catch (VisaException v_exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = v_exp, ClassName = "ANALYZER", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            catch (Exception exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "ANALYZER", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            #endregion
            //:SENSe:BWIDth:RESolution:AUTO?
            an_dm -= GetAutoSweepTime;
        }
        /// <summary>
        /// Установка метода свипирования 
        /// </summary>        
        private void SetSweepType()
        {
            try
            {
                if (UniqueData.InstrManufacture == 1)
                {
                    session.Write(":SWE:TYPE " + SweepTypeSelected.Parameter);
                    //if (SweepType == 0) { session.Write(":SWE:TYPE Auto"); SweepTypeStr = "Auto"; }
                    //else if (SweepType == 1) { session.Write(":SWE:TYPE Sweep"); SweepTypeStr = "Sweep"; }
                    //else if (SweepType == 2) { session.Write(":SWE:TYPE FFT"); SweepTypeStr = "FFT"; }
                }
                else if (UniqueData.InstrManufacture == 2)
                {
                    session.Write(":SWE:TYPE " + SweepTypeSelected.Parameter);
                    //if (SweepType == 0) { session.Write(":SWE:TYPE AUTO"); SweepTypeStr = "Auto"; }
                    //else if (SweepType == 1) { session.Write(":SWE:TYPE STEP"); SweepTypeStr = "STEP"; }
                    //else if (SweepType == 2) { session.Write(":SWE:TYPE FFT"); SweepTypeStr = "FFT"; }
                }
                else if (UniqueData.InstrManufacture == 3)
                {
                    session.Write(":SWE:MODE " + SweepTypeSelected.Parameter);
                }
            }
            #region Exception
            catch (VisaException v_exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = v_exp, ClassName = "ANALYZER", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            catch (Exception exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "ANALYZER", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            #endregion
            an_dm -= SetSweepType;
        }
        /// <summary>
        /// Получаем метод свиптайма 
        /// </summary> 
        private void GetSweepType()
        {
            try
            {
                if (UniqueData.InstrManufacture == 1)
                {
                    if (UniqueData.HiSpeed == true)
                    {
                        string t = session.Query(":SWE:TYPE?");
                        foreach (SwpType ST in UniqueData.SweepType)
                        {
                            if (t.TrimEnd().ToLower() == ST.Parameter.ToLower()) { SweepTypeSelected = ST; }
                        }
                    }
                    else { SweepTypeSelected = new SwpType { UI = "FFT", Parameter = "FFT" }; }
                }
                else if (UniqueData.InstrManufacture == 2)
                {
                    string temp = session.Query(":SWE:TYPE?");
                    foreach (SwpType ST in UniqueData.SweepType)
                    {
                        if (temp.Contains(ST.Parameter)) { SweepTypeSelected = ST; }
                    }
                }
                else if (UniqueData.InstrManufacture == 3)
                {
                    string temp = session.Query(":SWE:MODE?");
                    foreach (SwpType ST in UniqueData.SweepType)
                    {
                        if (temp.Contains(ST.Parameter)) { SweepTypeSelected = ST; }
                    }
                }
            }
            #region Exception
            catch (VisaException v_exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = v_exp, ClassName = "ANALYZER", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            catch (Exception exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "ANALYZER", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            #endregion
            an_dm -= GetSweepType;
        }

        /// <summary>
        /// Установка количевства точек свипов 
        /// </summary>        
        private void SetSweepPoints()
        {
            try
            {
                if (UniqueData.InstrManufacture == 1)
                {
                    session.Write(":SWE:POIN " + SweepPoints.ToString());
                    session.DefaultBufferSize = SweepPoints * 18 + 25;
                }
                else if (UniqueData.InstrManufacture == 2)
                {
                    session.Write(":SENS:SWE:POIN " + SweepPoints.ToString());
                    session.DefaultBufferSize = SweepPoints * 4 + 20;
                }
            }
            #region Exception
            catch (VisaException v_exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = v_exp, ClassName = "ANALYZER", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            catch (Exception exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "ANALYZER", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            #endregion
            GetFreqArr();
            MarkersTraceLegthOrFreqsChanged(Trace1);
            an_dm -= SetSweepPoints;
        }
        /// <summary>
        /// Получаем количевства точек свипов 
        /// </summary>
        private void GetSweepPoints()
        {
            try
            {
                if (UniqueData.InstrManufacture == 1)
                {
                    if (UniqueData.SweepPointFix == false)
                    {
                        SweepPoints = int.Parse(session.Query(":SWE:POIN?").Replace('.', ','));
                        SweepPointsIndex = System.Array.IndexOf(UniqueData.SweepPointArr, SweepPoints);
                    }
                    if (UniqueData.HiSpeed == true) { TracePoints = SweepPoints + 2; }
                    if (UniqueData.HiSpeed == false) { TracePoints = SweepPoints; }
                }
                else if (UniqueData.InstrManufacture == 2)
                {
                    SweepPoints = int.Parse(session.Query(":SENS:SWE:POIN?").Replace('.', ','));
                    SweepPointsIndex = System.Array.IndexOf(UniqueData.SweepPointArr, SweepPoints);
                    session.DefaultBufferSize = SweepPoints * 4 + 20;
                    TracePoints = SweepPoints;
                }
            }
            #region Exception
            catch (VisaException v_exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = v_exp, ClassName = "ANALYZER", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            catch (Exception exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "ANALYZER", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            #endregion
            GetFreqArr();
            an_dm -= GetSweepPoints;
        }
        #endregion sweep

        #region Level
        /// <summary>
        /// Установка опорного уровня 
        /// </summary>
        private void SetRefLevel()
        {
            try
            {
                if (UniqueData.InstrManufacture == 1)
                {
                    if (UniqueData.HiSpeed)
                    { session.Write(":DISPlay:WINDow:TRACe:Y:SCALe:RLEVel " + RefLevel.ToString().Replace(',', '.')); }
                    else { session.Write(":DISP:TRAC:Y:RLEV " + RefLevel.ToString().Replace(',', '.')); }
                    if (AttAuto == true)
                    {
                        AttLevel = decimal.Parse(session.Query(":INP:ATT?").Replace('.', ','));
                    }

                }
                else if (UniqueData.InstrManufacture == 2)
                {
                    session.Write("DISP:WIND:TRAC:Y:SCAL:RLEV " + RefLevel.ToString().Replace(',', '.'));
                    if (AttAuto == true)
                    {
                        AttLevel = decimal.Parse(session.Query(":POW:ATT?").Replace('.', ','));
                    }
                }
                else if (UniqueData.InstrManufacture == 3)
                {
                    session.Write(":DISPlay:WINDow:TRACe:Y:SCALe:RLEVel " + RefLevel.ToString());
                }
            }
            #region Exception
            catch (VisaException v_exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = v_exp, ClassName = "ANALYZER", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            catch (Exception exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "ANALYZER", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            #endregion
            if (AttAuto == true) { GetAttLevel(); }
            an_dm -= SetRefLevel;
        }
        /// <summary>
        /// Получаем опорный уровнь 
        /// </summary>
        private void GetRefLevel()
        {
            try
            {
                if (UniqueData.InstrManufacture == 1)
                {
                    if (UniqueData.HiSpeed)
                    { RefLevel = Math.Round(decimal.Parse(session.Query(":DISPlay:WINDow:TRACe:Y:SCALe:RLEVel?").Replace('.', ','))); }
                    else { RefLevel = Math.Round(decimal.Parse(session.Query(":DISP:TRAC:Y:RLEV?").Replace('.', ','))); }
                }
                else if (UniqueData.InstrManufacture == 2)
                {
                    RefLevel = Math.Round(decimal.Parse(session.Query(":DISP:WIND:TRAC:Y:SCAL:RLEV?").Replace('.', ',')));
                }
                else if (UniqueData.InstrManufacture == 3)
                {
                    RefLevel = Math.Round(decimal.Parse(session.Query(":DISP:WIND:TRAC:Y:SCAL:RLEV?").Replace('.', ',')));
                }
            }
            #region Exception
            catch (VisaException v_exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = v_exp, ClassName = "ANALYZER", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            catch (Exception exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "ANALYZER", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            #endregion
            an_dm -= GetRefLevel;
        }


        private void SetRange()
        {
            try
            {
                if (UniqueData.InstrManufacture == 1)
                {
                    session.Write(":DISP:TRAC:Y " + Range.ToString().Replace(',', '.'));
                }
                else if (UniqueData.InstrManufacture == 2)
                {
                    //session.Write("DISP:WIND:TRAC:Y:SCAL:RLEV " + RefLevel.ToString().Replace(',', '.'));
                }
                else if (UniqueData.InstrManufacture == 3)
                {
                    //session.Write(":DISPlay:WINDow:TRACe:Y:SCALe:RLEVel " + RefLevel.ToString());
                }
            }
            #region Exception
            catch (VisaException v_exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = v_exp, ClassName = "ANALYZER", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            catch (Exception exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "ANALYZER", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            #endregion
            if (AttAuto == true) { GetAttLevel(); }
            an_dm -= SetRange;
        }
        private void GetRange()
        {
            try
            {
                if (UniqueData.InstrManufacture == 1)
                {
                    Range = Math.Round(decimal.Parse(session.Query(":DISP:TRAC:Y?").Replace('.', ',')));
                    RangeIndex = Array.FindIndex(UniqueData.RangeArr, w => w == Range);
                }
                else if (UniqueData.InstrManufacture == 2)
                {
                    //Range = Math.Round(decimal.Parse(session.Query(":DISP:WIND:TRAC:Y:SCAL:RLEV?").Replace('.', ',')));
                }
                else if (UniqueData.InstrManufacture == 3)
                {
                    //Range = Math.Round(decimal.Parse(session.Query(":DISP:WIND:TRAC:Y:SCAL:RLEV?").Replace('.', ',')));
                }
            }
            #region Exception
            catch (VisaException v_exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = v_exp, ClassName = "ANALYZER", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            catch (Exception exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "ANALYZER", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            #endregion
            finally { an_dm -= GetRange; }

        }
        /// <summary>
        /// Вкл/Выкл атоматического аттенюатора зависящего от опорного уровня, при выкл
        /// АвтоАТТ изменяем настройку аттенюатора
        /// </summary>
        private void SetAttLevel()
        {
            try
            {
                if (UniqueData.InstrManufacture == 1)
                {
                    session.Write(":INP:ATT " + AttLevel.ToString().Replace(',', '.')); //INP:ATT:AUTO
                }
                else if (UniqueData.InstrManufacture == 2)
                {
                    session.Write(":POW:ATT " + AttLevel.ToString().Replace(',', '.'));
                }
            }
            #region Exception
            catch (VisaException v_exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = v_exp, ClassName = "ANALYZER", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            catch (Exception exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "ANALYZER", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            #endregion
            an_dm -= SetAttLevel;
        }
        /// <summary>
        /// Получаем настройку аттенюатора
        /// </summary>
        private void GetAttLevel()
        {
            try
            {
                if (UniqueData.InstrManufacture == 1)
                {
                    AttLevel = decimal.Parse(session.Query(":INP:ATT?").Replace('.', ','));
                }
                else if (UniqueData.InstrManufacture == 2)
                {
                    AttLevel = decimal.Parse(session.Query(":POW:ATT?").Replace('.', ','));
                }
                else if (UniqueData.InstrManufacture == 3)
                {
                    string t = session.Query(":POW:ATT?").Replace('.', ',');
                    if (t != "0.0") { AttLevel = decimal.Parse(t); }
                    else AttLevel = 0;
                }
            }
            #region Exception
            catch (VisaException v_exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = v_exp, ClassName = "ANALYZER", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            catch (Exception exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "ANALYZER", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            #endregion
            an_dm -= GetAttLevel;
        }

        private void SetAutoAttLevel()
        {
            try
            {
                if (UniqueData.InstrManufacture == 1)
                {
                    if (AttAuto == true) session.Write(":INP:ATT:AUTO 1");
                    if (AttAuto == false) session.Write(":INP:ATT:AUTO 0");
                    AttLevel = decimal.Parse(session.Query(":INP:ATT?").Replace('.', ','));
                }
                else if (UniqueData.InstrManufacture == 2)
                {
                    if (AttAuto == true) session.Write(":POW:ATT:AUTO 1");
                    if (AttAuto == false) session.Write(":POW:ATT:AUTO 0");
                    AttLevel = decimal.Parse(session.Query(":POW:ATT?").Replace('.', ','));
                }
                else if (UniqueData.InstrManufacture == 3)
                {
                    if (AttAuto == true) session.Write(":POW:ATT:AUTO 1");
                    if (AttAuto == false) session.Write(":POW:ATT:AUTO 0");
                    AttLevel = decimal.Parse(session.Query(":POW:ATT?").Replace('.', ','));
                }
            }
            #region Exception
            catch (VisaException v_exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = v_exp, ClassName = "ANALYZER", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            catch (Exception exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "ANALYZER", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            #endregion
            an_dm -= SetAutoAttLevel;
        }
        private void GetAutoAttLevel()
        {
            try
            {
                if (UniqueData.InstrManufacture == 1)
                {
                    string temp = session.Query(":INP:ATT:AUTO?");
                    if (temp == "1\n") { AttAuto = true; }
                    else if (temp == "0\n") { AttAuto = false; }
                }
                else if (UniqueData.InstrManufacture == 2)
                {
                    string temp = session.Query(":POW:ATT:AUTO?");
                    if (temp == "1\n") { AttAuto = true; }
                    else if (temp == "0\n") { AttAuto = false; }
                }
                else if (UniqueData.InstrManufacture == 3)
                {
                    string temp = session.Query(":POW:ATT:AUTO?");
                    if (temp == "1\n") { AttAuto = true; }
                    else if (temp == "0\n") { AttAuto = false; }
                }
            }
            #region Exception
            catch (VisaException v_exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = v_exp, ClassName = "ANALYZER", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            catch (Exception exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "ANALYZER", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            #endregion
            an_dm -= GetAutoAttLevel;
        }


        /// <summary>
        /// Вкл/Выкл предусилителя 
        /// </summary>
        private void SetPreAmp()
        {
            try
            {
                if (UniqueData.InstrManufacture == 1 && UniqueData.PreAmp == true)
                {
                    if (PreAmp == true) session.Write(":INP:GAIN:STAT 1");
                    if (PreAmp == false) session.Write(":INP:GAIN:STAT 0");
                }
                else if (UniqueData.InstrManufacture == 2)
                {
                    if (PreAmp == true) session.Write(":POW:GAIN 1");
                    if (PreAmp == false) session.Write(":POW:GAIN 0");
                }
                else if (UniqueData.InstrManufacture == 3)
                {
                    if (PreAmp == true) session.Write(":POW:GAIN 1");
                    if (PreAmp == false) session.Write(":POW:GAIN 0");
                }
            }
            #region Exception
            catch (VisaException v_exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = v_exp, ClassName = "ANALYZER", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            catch (Exception exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "ANALYZER", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            #endregion
            an_dm -= SetPreAmp;
        }
        /// <summary>
        /// Узнаем состояние предусилителя 
        /// </summary>
        private void GetPreAmp()
        {
            try
            {
                if (UniqueData.InstrManufacture == 1 && UniqueData.PreAmp == true)
                {
                    string temp = session.Query(":INP:GAIN:STAT?");
                    if (temp == "1\n") { PreAmp = true; }
                    else if (temp == "0\n") { PreAmp = false; }
                }
                else if (UniqueData.InstrManufacture == 2)
                {
                    string temp = session.Query(":POW:GAIN:STAT?");
                    if (temp == "1\n") { PreAmp = true; }
                    else if (temp == "0\n") { PreAmp = false; }
                }
                else if (UniqueData.InstrManufacture == 3)
                {
                    string temp = session.Query(":POW:GAIN:STAT?");
                    if (temp.Contains("1")) { PreAmp = true; }
                    else if (temp.Contains("0")) { PreAmp = false; }
                }
            }
            #region Exception
            catch (VisaException v_exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = v_exp, ClassName = "ANALYZER", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            catch (Exception exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "ANALYZER", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            #endregion
            an_dm -= GetPreAmp;
        }

        /// <summary>
        /// Установка Units
        /// </summary>
        private void SetLevelUnit()
        {
            try
            {
                if (UniqueData.InstrManufacture == 1)
                {
                    session.Write(":UNIT:POWer " + LevelUnits[LevelUnitIndex].AnParameter);
                    //if (LevelUnit == 0) session.Write(":UNIT:POWer DBM");
                    //else if (LevelUnit == 1) session.Write(":UNIT:POWer DBMV");
                    //else if (LevelUnit == 2) session.Write(":UNIT:POWer DBUV");
                    //else if (LevelUnit == 2) session.Write(":UNIT:POWer DBUV/M");//dBuV/M
                }
                else if (UniqueData.InstrManufacture == 2)
                {
                    session.Write(":AMPL:UNIT " + LevelUnits[LevelUnitIndex].AnParameter);
                    //if (LevelUnit == 0) session.Write(":AMPL:UNIT DBM");
                    //else if (LevelUnit == 1) session.Write(":AMPL:UNIT DBMV");
                    //else if (LevelUnit == 2) session.Write(":AMPL:UNIT DBUV");
                    //else if (LevelUnit == 2) session.Write(":AMPL:UNIT DBUV/M");//dBuV/M
                }
                else if (UniqueData.InstrManufacture == 3)
                {
                    session.Write(":UNIT:POWer " + LevelUnits[LevelUnitIndex].AnParameter);
                    //if (LevelUnit == 0) session.Write(":UNIT:POWer DBM");
                    //else if (LevelUnit == 1) session.Write(":UNIT:POWer DBMV");
                    //else if (LevelUnit == 2) session.Write(":UNIT:POWer DBUV");
                    //else if (LevelUnit == 2) session.Write(":UNIT:POWer DBUV/M");//dBuV/M
                }
            }
            #region Exception
            catch (VisaException v_exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = v_exp, ClassName = "ANALYZER", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            catch (Exception exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "ANALYZER", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            #endregion
            finally
            {
                for (int i = 0; i < Markers.Count(); i++)
                {
                    Markers[i].LevelUnit = LevelUnit;
                }
                GetRefLevel();
                GetLevelUnit();
                an_dm -= SetLevelUnit;
            }
        }
        /// <summary>
        /// Получаем Units 
        /// </summary>
        private void GetLevelUnit()
        {
            try
            {
                if (UniqueData.InstrManufacture == 1)
                {
                    if (UniqueData.HiSpeed)
                    {
                        for (int i = 0; i < LevelUnits.Count(); i++)
                        {
                            if (LevelUnits[i].ind == 0) { LevelUnits[i].AnParameter = "DBM"; }
                            else if (LevelUnits[i].ind == 1) { LevelUnits[i].AnParameter = "DBMV"; }
                            else if (LevelUnits[i].ind == 2) { LevelUnits[i].AnParameter = "DBUV"; }
                            else if (LevelUnits[i].ind == 3) { LevelUnits[i].AnParameter = "DBUV/M"; }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < LevelUnits.Count(); i++)
                        {
                            if (LevelUnits[i].ind == 0) { LevelUnits[i].AnParameter = "DBM"; }
                            else if (LevelUnits[i].ind == 1) { LevelUnits[i].AnParameter = "DBMV"; }
                            else if (LevelUnits[i].ind == 2) { LevelUnits[i].AnParameter = "DBUV"; }
                            else if (LevelUnits[i].ind == 3) { LevelUnits[i].AnParameter = "DUVM"; }
                        }
                    }

                    string temp = session.Query(":UNIT:POWer?").TrimEnd();
                    for (int i = 0; i < LevelUnits.Count(); i++)
                    {
                        if (temp.ToLower() == LevelUnits[i].AnParameter.ToLower())
                        {
                            LevelUnitIndex = i;
                            if (LevelUnits[i].ind == 3) { LevelUnits[i].IsEnabled = true; }
                            else { LevelUnits[3].IsEnabled = false; }
                        }
                    }
                }
                else if (UniqueData.InstrManufacture == 2)
                {
                    for (int i = 0; i < LevelUnits.Count(); i++)
                    {
                        if (LevelUnits[i].ind == 0) { LevelUnits[i].AnParameter = "DBM"; }
                        else if (LevelUnits[i].ind == 1) { LevelUnits[i].AnParameter = "DBMV"; }
                        else if (LevelUnits[i].ind == 2) { LevelUnits[i].AnParameter = "DBUV"; }
                        else if (LevelUnits[i].ind == 3) { LevelUnits[i].IsEnabled = false; }
                    }
                    string temp = session.Query(":AMPL:UNIT?").TrimEnd();
                    for (int i = 0; i < LevelUnits.Count(); i++)
                    {
                        if (temp.ToLower() == LevelUnits[i].AnParameter.ToLower()) { LevelUnitIndex = i; }
                    }
                }
                else if (UniqueData.InstrManufacture == 3)
                {
                    for (int i = 0; i < LevelUnits.Count(); i++)
                    {
                        if (LevelUnits[i].ind == 0) { LevelUnits[i].AnParameter = "dBm"; }
                        else if (LevelUnits[i].ind == 1) { LevelUnits[i].AnParameter = "dBmV"; }
                        else if (LevelUnits[i].ind == 2) { LevelUnits[i].AnParameter = "dBuV"; }
                        else if (LevelUnits[i].ind == 3) { LevelUnits[i].IsEnabled = false; }
                    }
                    string temp = session.Query(":UNIT:POWer?").TrimEnd();
                    for (int i = 0; i < LevelUnits.Count(); i++)
                    {
                        if (temp.ToLower() == LevelUnits[i].AnParameter.ToLower()) { LevelUnitIndex = i; }
                    }
                }
            }
            #region Exception
            catch (VisaException v_exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = v_exp, ClassName = "ANALYZER", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            catch (Exception exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "ANALYZER", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            #endregion
            finally
            {
                for (int i = 0; i < Markers.Count(); i++)
                {
                    Markers[i].LevelUnit = LevelUnit;
                }
                an_dm -= GetLevelUnit;
            }
        }
        #endregion Level

        #region Trace

        private void SetTraceType()
        {
            try
            {
                if (UniqueData.InstrManufacture == 1)
                {
                    session.Write(":DISP:TRAC1:MODE " + Trace1Type.Parameter);
                    session.Write(":DISP:TRAC2:MODE " + Trace2Type.Parameter);
                    session.Write(":DISP:TRAC3:MODE " + Trace3Type.Parameter);
                }
                else if (UniqueData.InstrManufacture == 2)
                {
                    session.Write(":TRAC1:TYPE " + Trace1Type.Parameter);
                    session.Write(":TRAC2:TYPE " + Trace2Type.Parameter);
                    session.Write(":TRAC3:TYPE " + Trace3Type.Parameter);
                }
                else if (UniqueData.InstrManufacture == 3)
                {
                    session.Write(":TRACe1:OPERation " + Trace1Type.Parameter);
                    session.Write(":TRACe2:OPERation " + Trace2Type.Parameter);
                    session.Write(":TRACe3:OPERation " + Trace3Type.Parameter);
                }
            }
            #region Exception
            catch (VisaException v_exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = v_exp, ClassName = "ANALYZER", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            catch (Exception exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "ANALYZER", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            #endregion
            an_dm -= SetTraceType;
        }
        private void GetTraceType()
        {
            try
            {
                if (UniqueData.InstrManufacture == 1)
                {
                    string temp1 = string.Empty;
                    string temp2 = string.Empty;
                    string temp3 = string.Empty;
                    if (UniqueData.HiSpeed == true)
                    {
                        if (session.Query(":DISP:TRAC1?").Contains("1")) { temp1 = session.Query(":DISP:TRAC1:MODE?"); }
                        else { Trace1Type = new ParamWithUI { UI = "Blank", Parameter = "BLAN" }; }
                        if (session.Query(":DISP:TRAC2?").Contains("1")) { temp2 = session.Query(":DISP:TRAC2:MODE?"); }
                        else { Trace2Type = new ParamWithUI { UI = "Blank", Parameter = "BLAN" }; }
                        if (session.Query(":DISP:TRAC3?").Contains("1")) { temp3 = session.Query(":DISP:TRAC3:MODE?"); }
                        else { Trace3Type = new ParamWithUI { UI = "Blank", Parameter = "BLAN" }; }
                    }
                    else if (UniqueData.HiSpeed == false)
                    {
                        temp1 = session.Query(":DISP:TRAC1:MODE?");
                        temp2 = "BLAN";
                        temp3 = "BLAN";
                    }
                    foreach (ParamWithUI TT in UniqueData.TraceType)
                    {
                        if (temp1.Contains(TT.Parameter)) { Trace1Type = TT; }
                        if (temp2.Contains(TT.Parameter)) { Trace2Type = TT; }
                        if (temp3.Contains(TT.Parameter)) { Trace3Type = TT; }
                    }
                }
                else if (UniqueData.InstrManufacture == 2)
                {
                    string temp1 = session.Query(":TRACe1:TYPE?");
                    //string temp2 = session.Query(":TRACe2:TYPE?");
                    //string temp3 = session.Query(":TRACe3:TYPE?");
                    foreach (ParamWithUI TT in UniqueData.TraceType)
                    {
                        if (temp1.Contains(TT.Parameter)) { Trace1Type = TT; }
                        //if (temp2.Contains(TT.Parameter)) { Trace2Type = TT; }
                        //if (temp3.Contains(TT.Parameter)) { Trace3Type = TT; }
                    }
                }
                else if (UniqueData.InstrManufacture == 3)
                {
                    string temp1 = session.Query(":TRACe1:OPERation?");
                    //string temp2 = session.Query(":TRACe2:OPERation?");
                    //string temp3 = session.Query(":TRACe3:OPERation?");
                    foreach (ParamWithUI TT in UniqueData.TraceType)
                    {
                        if (temp1.Contains(TT.Parameter)) { Trace1Type = TT; }
                        //if (temp2.Contains(TT.Parameter)) { Trace2Type = TT; }
                        //if (temp3.Contains(TT.Parameter)) { Trace3Type = TT; }
                    }
                }
            }
            #region Exception
            catch (VisaException v_exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = v_exp, ClassName = "ANALYZER", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            catch (Exception exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "ANALYZER", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            #endregion
            an_dm -= GetTraceType;
        }

        private void SetResetTrace()
        {
            try
            {
                if (UniqueData.InstrManufacture == 1)
                {
                    session.Write(":DISP:TRAC" + TraceNumberToReset.ToString() + ":MODE " + UniqueData.TraceType[0].Parameter);
                    session.Write(":DISP:TRAC" + TraceNumberToReset.ToString() + ":MODE " + Trace1Type.Parameter);
                }
                else if (UniqueData.InstrManufacture == 2)
                {
                    session.Write(":TRAC" + TraceNumberToReset.ToString() + ":TYPE " + UniqueData.TraceType[0].Parameter);
                    session.Write(":TRAC" + TraceNumberToReset.ToString() + ":TYPE " + Trace1Type.Parameter);
                }
                else if (UniqueData.InstrManufacture == 3)
                {
                    session.Write(":TRACe" + TraceNumberToReset.ToString() + ":OPERation " + UniqueData.TraceType[0].Parameter);
                    session.Write(":TRACe" + TraceNumberToReset.ToString() + ":OPERation " + Trace1Type.Parameter);
                }
            }
            #region Exception
            catch (VisaException v_exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = v_exp, ClassName = "ANALYZER", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            catch (Exception exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "ANALYZER", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            #endregion
            finally { an_dm -= SetResetTrace; }
        }
        private void SetDetectorType()
        {
            try
            {
                if (UniqueData.InstrManufacture == 1)
                {
                    if (Trace1Detector.Parameter == "Auto Select") session.Write(":SENSe:DETector1:AUTO 1");
                    else
                    {
                        session.Write(":SENSe:DETector1 " + Trace1Detector.Parameter);
                    }
                    if (UniqueData.HiSpeed == true && Trace2Detector.Parameter == "Auto Select") session.Write(":SENSe:DETector2:AUTO 1");
                    else
                    {
                        session.Write(":SENSe:DETector2 " + Trace2Detector.Parameter);
                    }
                    if (UniqueData.HiSpeed == true && Trace3Detector.Parameter == "Auto Select") session.Write(":SENSe:DETector3:AUTO 1");
                    else
                    {
                        session.Write(":SENSe:DETector3 " + Trace3Detector.Parameter);
                    }
                }
                else if (UniqueData.InstrManufacture == 2)
                {
                    //if (Trace1Detector.UI == "Auto Select") session.Write(":SENSe:DETector:AUTO");
                    //else
                    //{
                    session.Write(":SENSe:DETector:FUNCtion " + Trace1Detector.Parameter);
                    //}
                }
                else if (UniqueData.InstrManufacture == 3)
                {
                    if (Trace1Detector.Parameter == "Auto Select") session.Write(":SENSe:DETector1:AUTO 1");
                    else
                    {
                        session.Write(":SENSe:DETector:FUNCtion " + Trace1Detector.Parameter);
                    }
                }
            }
            #region Exception
            catch (VisaException v_exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = v_exp, ClassName = "ANALYZER", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            catch (Exception exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "ANALYZER", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            #endregion
            //:SENSe:DETector:FUNCtion POSitive|RMS|NEGative|SAMPle
            an_dm -= SetDetectorType;
            GetVBW();
        }
        /// <summary>
        /// Получение типа Trace Detecror 
        /// </summary>
        private void GetDetectorType()
        {
            try
            {
                if (UniqueData.InstrManufacture == 1)
                {

                    string temp1 = string.Empty;
                    string temp2 = string.Empty;
                    string temp3 = string.Empty;
                    if (UniqueData.HiSpeed == true)
                    {
                        temp1 = session.Query(":SENSe:DETector1?").TrimEnd();
                        temp2 = session.Query(":SENSe:DETector2?");
                        temp3 = session.Query(":SENSe:DETector3?");
                    }
                    else if (UniqueData.HiSpeed == false)
                    {
                        temp1 = session.Query(":SENSe:DETector1?").TrimEnd();
                        temp2 = "POS";
                        temp3 = "POS";
                    }
                    foreach (TrDetector TT in UniqueData.TraceDetector)
                    {
                        if (temp1.Contains(TT.Parameter)) { Trace1Detector = TT; }
                        if (temp2.Contains(TT.Parameter)) { Trace2Detector = TT; }
                        if (temp3.Contains(TT.Parameter)) { Trace3Detector = TT; }
                    }
                }
                else if (UniqueData.InstrManufacture == 2)
                {
                    string temp1 = session.Query(":SENSe:DETector:FUNCtion?").TrimEnd();
                    foreach (TrDetector TT in UniqueData.TraceDetector)
                    {
                        if (temp1.Contains(TT.Parameter)) { Trace1Detector = TT; }
                    }
                }
                else if (UniqueData.InstrManufacture == 3)
                {
                    string temp1 = session.Query(":SENSe:DETector:FUNCtion?").TrimEnd();
                    foreach (TrDetector TT in UniqueData.TraceDetector)
                    {
                        if (temp1.Contains(TT.Parameter)) { Trace1Detector = TT; }
                    }
                }
            }
            #region Exception
            catch (VisaException v_exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = v_exp, ClassName = "ANALYZER", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            catch (Exception exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "ANALYZER", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            #endregion
            an_dm -= GetDetectorType;
            //GetVBW();
        }


        /// <summary>
        /// Настраиваем усреднение
        /// </summary>
        private void SetAverageCount()
        {
            try
            {
                if (UniqueData.InstrManufacture == 1)
                {
                    if (UniqueData.HiSpeed == true)
                    {
                        if (AveragingCount > 30000) AveragingCount = 30000;
                        else if (AveragingCount < 1) AveragingCount = 1;
                        session.Write(":SENSe:AVERage:COUNt " + AveragingCount.ToString().Replace(',', '.'));
                    }
                    else if (UniqueData.HiSpeed == false)
                    {
                        if (AveragingCount > 1000) AveragingCount = 999;
                        else if (AveragingCount < 1) AveragingCount = 1;
                        session.Write("SENSe:SWEep:COUNt " + AveragingCount.ToString().Replace(',', '.'));
                    }
                }
                if (UniqueData.InstrManufacture == 2)
                {
                    if (AveragingCount > 1000) AveragingCount = 1000;
                    else if (AveragingCount < 1) AveragingCount = 1;
                    session.Write(":SENSe:AVERage:COUNt " + AveragingCount.ToString().Replace(',', '.'));
                }
            }
            #region Exception
            catch (VisaException v_exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = v_exp, ClassName = "ANALYZER", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            catch (Exception exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "ANALYZER", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            #endregion
            an_dm -= SetAverageCount;
        }
        /// <summary>
        /// Получаем усреднение
        /// </summary>
        private void GetAverageCount()
        {
            try
            {
                if (UniqueData.InstrManufacture == 1)
                {
                    if (UniqueData.HiSpeed == true) { AveragingCount = int.Parse(session.Query(":SENSe:AVERage:COUNt?").Replace('.', ',')); }
                    else if (UniqueData.HiSpeed == false) { AveragingCount = int.Parse(session.Query("SENSe:SWEep:COUNt?").Replace('.', ',')); }
                }
                if (UniqueData.InstrManufacture == 2)
                {
                    AveragingCount = int.Parse(session.Query(":SENSe:AVERage:COUNt?").Replace('.', ','));
                }
            }
            #region Exception
            catch (VisaException v_exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = v_exp, ClassName = "ANALYZER", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            catch (Exception exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "ANALYZER", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            #endregion
            //:SENSe:AVERage:COUNt?
            an_dm -= GetAverageCount;
        }
        /// <summary>
        /// Получаем количевство трейсов в усреднении
        /// </summary>
        private void GetNumberOfSweeps()
        {
            try
            {
                if (UniqueData.InstrManufacture == 1)
                {
                    if (UniqueData.HiSpeed == true)
                    {
                        NumberOfSweeps = int.Parse(session.Query(":SWE:COUN:CURR?").Replace('.', ','));
                        if (NumberOfSweeps == AveragingCount) { NOSEquallyAC = true; }
                        else { NOSEquallyAC = false; }
                    }
                    else if (UniqueData.HiSpeed == false) { NumberOfSweeps = AveragingCount; }
                }
                else if (UniqueData.InstrManufacture == 2)
                {
                    //NumberOfSweeps = int.Parse(session.Query(":SWE:COUN:CURR?").Replace('.', ','));
                    //NumberOfSweepsStr = String.Concat(NumberOfSweeps, "/", AveragingCount);
                    //if (NumberOfSweeps == AveragingCount) { NOSEquallyAC = true; }
                    //else { NOSEquallyAC = false; }
                }
            }
            #region Exception
            catch (VisaException v_exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = v_exp, ClassName = "ANALYZER", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            catch (Exception exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "ANALYZER", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            #endregion
            an_dm -= GetNumberOfSweeps;
        }
        #endregion Trace

        #region Markers

        private void SetMarkerState()
        {
            for (int i = 0; i < Markers.Count; i++)
            {
                try
                {
                    if (UniqueData.InstrManufacture == 1)
                    {
                        #region
                        if (Markers[i].State != Markers[i].StateNew || Markers[i].MarkerTypeNew != Markers[i].MarkerType)
                        {
                            Markers[i].State = Markers[i].StateNew;
                            Markers[i].MarkerType = Markers[i].MarkerTypeNew;
                            if (Markers[i].State == true)
                            {
                                if (Markers[i].MarkerType == 0)
                                {
                                    decimal freq = 0;
                                    if (UniqueData.HiSpeed == true)
                                    {
                                        session.Write(":CALCulate:MARK" + Markers[i].Index.ToString() + ":STAT 1");
                                        Markers[i].TraceNumber.Parameter = session.Query("CALC:MARK" + Markers[i].Index.ToString() + ":TRAC?").TrimEnd();
                                        string st = session.Query(":CALC:MARK" + Markers[i].Index.ToString() + ":X?;".Replace('.', ',')).TrimEnd();
                                        freq = decimal.Parse(st, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture);
                                    }
                                    else
                                    {
                                        session.Write(":CALCulate:MARK" + Markers[i].Index.ToString() + " ON");
                                        string st = session.Query(":CALC:MARK" + Markers[i].Index.ToString() + ":X?;".Replace(',', '.')).TrimEnd();
                                        freq = decimal.Parse(st, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture);
                                    }
                                    Markers[i].IndexOnTrace = LM.FindMarkerIndOnTrace(Trace1, freq);
                                    Markers[i].Freq = Trace1[Markers[i].IndexOnTrace].freq;
                                    Markers[i].FreqNew = Markers[i].Freq;

                                }
                                else if (Markers[i].MarkerType == 1 && i > 0)
                                {
                                    decimal freq = 0;
                                    if (UniqueData.HiSpeed == true)
                                    {
                                        session.Write("CALC:DELT" + Markers[i].Index.ToString() + ":MREF 1");
                                        session.Write("CALC:DELT" + Markers[i].Index.ToString() + ":STAT 1");
                                        string st = session.Query(":CALC:DELT" + Markers[i].Index.ToString() + ":X?;".Replace('.', ',')).TrimEnd();
                                        freq = decimal.Parse(st, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture);
                                    }
                                    else
                                    {
                                        session.Write("CALC:DELT" + Markers[i].Index.ToString() + ":MREF 1");
                                        session.Write("CALC:DELT" + Markers[i].Index.ToString() + " ON");
                                        string st = session.Query(":CALC:DELT" + Markers[i].Index.ToString() + ":X?;".Replace('.', ',')).TrimEnd();
                                        freq = decimal.Parse(st, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture);
                                    }
                                    Markers[i].IndexOnTrace = LM.FindMarkerIndOnTrace(Trace1, freq);
                                    Markers[i].Freq = Trace1[Markers[i].IndexOnTrace].freq;
                                    Markers[i].FreqNew = Markers[i].Freq;
                                }
                            }
                            else if (Markers[i].State == false)
                            {
                                if (UniqueData.HiSpeed == true)
                                {
                                    session.Write("CALC:MARK" + Markers[i].Index.ToString() + ":STAT 0");
                                    session.Write("CALC:DELT" + Markers[i].Index.ToString() + ":STAT 0");
                                }
                                else
                                {
                                    if (Markers[i].MarkerType == 0)
                                    {
                                        session.Write("CALC:MARK" + Markers[i].Index.ToString() + " OFF");
                                    }
                                    else if (Markers[i].MarkerType == 1 && i > 0)
                                    {
                                        session.Write("CALC:DELT" + Markers[i].Index.ToString() + " OFF");
                                    }
                                }
                                //GetRunMarkers();
                            }

                        }
                        #endregion
                    }
                    else if (UniqueData.InstrManufacture == 2)
                    {
                        #region
                        if (Markers[i].State == true)
                        {
                            if (Markers[i].MarkerType == 0)
                            {
                                session.Write(":CALC:MARK" + Markers[i].Index.ToString() + ":STAT NORM");
                                decimal freq = decimal.Parse(session.Query(":CALC:MARK" + (i + 1).ToString() + ":X?;").Replace('.', ','));
                                Markers[i].IndexOnTrace = LM.FindMarkerIndOnTrace(Trace1, freq);
                                Markers[i].Freq = Trace1[Markers[i].IndexOnTrace].freq;
                                Markers[i].FreqNew = Markers[i].Freq;
                            }
                        }
                        else if (Markers[i].State == false)
                        {
                            session.Write("CALC:MARK" + Markers[i].Index.ToString() + ":STAT OFF");
                        }
                        #endregion
                    }
                    else if (UniqueData.InstrManufacture == 3)
                    {
                        #region
                        if (Markers[i].State == true)
                        {
                            if (Markers[i].MarkerType == 0)
                            {
                                session.Write(":CALC:MARK" + Markers[i].Index.ToString() + ":STAT 1");
                                decimal freq = decimal.Parse(session.Query(":CALC:MARK" + (i + 1).ToString() + ":X?;").Replace('.', ','));
                                Markers[i].IndexOnTrace = LM.FindMarkerIndOnTrace(Trace1, freq);
                                Markers[i].Freq = Trace1[Markers[i].IndexOnTrace].freq;
                                Markers[i].FreqNew = Markers[i].Freq;
                            }
                        }
                        else if (Markers[i].State == false)
                        {
                            session.Write("CALC:MARK" + Markers[i].Index.ToString() + ":STAT 0");
                        }
                        #endregion
                    }
                }
                #region Exception
                catch (VisaException v_exp)
                {
                    App.Current.Dispatcher.Invoke((Action)(() =>
                    {
                        MainWindow.exp.ExceptionData = new ExData() { ex = v_exp, ClassName = "ANALYZER", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
                    }));
                }
                catch (Exception exp)
                {
                    App.Current.Dispatcher.Invoke((Action)(() =>
                    {
                        MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "ANALYZER", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
                    }));
                }
                #endregion
            }

            an_dm -= SetMarkerState;
        }

        private void SetMarkerAllOff()
        {
            try
            {
                if (UniqueData.InstrManufacture == 1)
                {
                    session.Write(":CALC:MARK:AOFF");
                    session.Write(":CALC:DELT:AOFF");
                }
                else if (UniqueData.InstrManufacture == 2)
                {
                    session.Write(":CALC:MARK:AOFF");
                    //session.Write(":CALC:DELT:AOFF");
                }
                else if (UniqueData.InstrManufacture == 3)
                {
                    session.Write(":CALC:MARK:AOFF");
                    //session.Write(":CALC:DELT:AOFF");
                }
            }
            #region Exception
            catch (VisaException v_exp)
            {
                App.Current.Dispatcher.Invoke((Action)(() =>
                {
                    MainWindow.exp.ExceptionData = new ExData() { ex = v_exp, ClassName = "ANALYZER", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
                }));
            }
            catch (Exception exp)
            {
                App.Current.Dispatcher.Invoke((Action)(() =>
                {
                    MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "ANALYZER", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
                }));
            }
            #endregion
            an_dm -= SetMarkerAllOff;
        }

        private void SetMarkerFreq()
        {
            for (int i = 0; i < Markers.Count; i++)
            {
                try
                {
                    if (Markers[i].State == true)
                    {
                        if (Markers[i].Freq != Markers[i].FreqNew)
                        {
                            Markers[i].Freq = Markers[i].FreqNew;
                            if (UniqueData.InstrManufacture == 1)
                            {
                                #region
                                if (Markers[i].MarkerType == 0 || Markers[i].MarkerType == 3 || Markers[i].MarkerType == 4)
                                {
                                    session.Write(":CALCulate:MARKer" + Markers[i].Index.ToString() + ":X " + Markers[i].Freq.ToString().Replace(',', '.'));
                                    if (UniqueData.HiSpeed == true)
                                    {
                                        string st = session.Query(":CALC:MARK" + Markers[i].Index.ToString() + ":X?;".Replace('.', ',')).TrimEnd();
                                        Markers[i].Freq = decimal.Parse(st, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture);
                                    }
                                    else
                                    {
                                        string st = session.Query(":CALC:MARK" + Markers[i].Index.ToString() + ":X?;".Replace(',', '.')).TrimEnd();
                                        Markers[i].Freq = decimal.Parse(st, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture);
                                    }
                                }
                                else if (Markers[i].MarkerType == 1)
                                {
                                    session.Write(":CALCulate:DELT" + Markers[i].Index.ToString() + ":X " + Markers[i].Freq.ToString().Replace(',', '.'));
                                    if (UniqueData.HiSpeed == true)
                                    {
                                        string st = session.Query(":CALC:DELT" + Markers[i].Index.ToString() + ":X?;".Replace('.', ',')).TrimEnd();
                                        Markers[i].Freq = decimal.Parse(st, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture);
                                    }
                                    else
                                    {
                                        string st = session.Query(":CALC:DELT" + Markers[i].Index.ToString() + ":X?;".Replace('.', ',')).TrimEnd();
                                        Markers[i].Freq = decimal.Parse(st, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture);
                                    }
                                }
                                #endregion
                            }
                            else if (UniqueData.InstrManufacture == 2)
                            {
                                #region
                                if (Markers[i].MarkerType == 0)
                                {
                                    session.Write(":CALCulate:MARKer" + Markers[i].Index.ToString() + ":X " + Markers[i].Freq.ToString().Replace(',', '.'));
                                }
                                else if (Markers[i].MarkerType == 1)
                                {

                                }
                                #endregion
                            }
                            else if (UniqueData.InstrManufacture == 3)
                            {
                                #region
                                if (Markers[i].MarkerType == 0)
                                {
                                    session.Write(":CALC:MARK" + Markers[i].Index.ToString() + ":X " + Markers[i].Freq.ToString().Replace(',', '.'));
                                }
                                else if (Markers[i].MarkerType == 1)
                                {

                                }
                                #endregion
                            }
                        }
                    }
                }
                #region Exception
                catch (VisaException v_exp)
                {
                    App.Current.Dispatcher.Invoke((Action)(() =>
                    {
                        MainWindow.exp.ExceptionData = new ExData() { ex = v_exp, ClassName = "ANALYZER", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
                    }));
                }
                catch (Exception exp)
                {
                    App.Current.Dispatcher.Invoke((Action)(() =>
                    {
                        MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "ANALYZER", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
                    }));
                }
                #endregion
            }
            an_dm -= SetMarkerFreq;
        }

        private void GetRunMarkers()
        {
            for (int i = 0; i < Markers.Count; i++)
            {
                try
                {
                    if (UniqueData.InstrManufacture == 1)
                    {
                        string t1 = session.Query(":CALC:MARK" + Markers[i].Index.ToString() + ":STAT?").TrimEnd();
                        #region M
                        if (t1 == "1")
                        {
                            Markers[i].State = true;
                            Markers[i].MarkerType = 0;
                            if (UniqueData.HiSpeed == true) { Markers[i].TraceNumber.Parameter = session.Query("CALC:MARK" + Markers[i].Index.ToString() + ":TRAC?").TrimEnd(); }
                            string ss = session.Query(":CALC:MARK" + Markers[i].Index.ToString() + ":X?;").TrimEnd();

                            decimal freq = decimal.Parse(ss, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture);
                            Markers[i].IndexOnTrace = LM.FindMarkerIndOnTrace(Trace1, freq);
                            Markers[i].Freq = Trace1[Markers[i].IndexOnTrace].freq;
                            //Markers[i].BitmapTexture = MarkerTexture(Markers[i].NameInLegend);
                        }
                        else if (t1 == "0" && Markers[i].MarkerType == 0) { Markers[i].State = false; }
                        #endregion M
                        #region D
                        string t2 = session.Query(":CALC:DELT" + Markers[i].Index.ToString() + ":STAT?").TrimEnd();
                        if (t2 == "1" && i > 0)
                        {
                            Markers[i].State = true;
                            Markers[i].MarkerType = 1;
                            Markers[i].FunctionDataType = 1;
                            Markers[i].MarkerParent = Markers[0];
                            if (UniqueData.HiSpeed == true)
                            {
                                Markers[i].TraceNumber.Parameter = session.Query("CALC:DELT" + Markers[i].Index.ToString() + ":TRAC?").TrimEnd();
                                decimal freq = decimal.Parse(session.Query(":CALC:DELT" + Markers[i].Index.ToString() + ":X?;".Replace('.', ',')));
                                Markers[i].IndexOnTrace = LM.FindMarkerIndOnTrace(Trace1, freq);
                                Markers[i].Freq = Trace1[Markers[i].IndexOnTrace].freq;
                            }
                            else
                            {
                                string st = session.Query(":CALC:DELT" + Markers[i].Index.ToString() + ":X?;".Replace('.', ',')).TrimEnd();
                                decimal freq = decimal.Parse(st, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture);
                                Markers[i].IndexOnTrace = LM.FindMarkerIndOnTrace(Trace1, freq);
                                Markers[i].Freq = Trace1[Markers[i].IndexOnTrace].freq;
                            }
                        }
                        else if (t2 == "0" && Markers[i].MarkerType == 1 && i > 0) { Markers[i].State = false; }
                        #endregion D
                    }
                    else if (UniqueData.InstrManufacture == 2)
                    {
                        #region
                        string t = session.Query(":CALC:MARK" + Markers[i].Index.ToString() + ":STAT?").TrimEnd();
                        if (t == "NORM")
                        {
                            Markers[i].State = true;
                            Markers[i].MarkerType = 0;
                            Markers[i].Freq = decimal.Parse(session.Query(":CALC:MARK" + Markers[i].Index.ToString() + ":X?;").Replace('.', ','));
                            Markers[i].IndexOnTrace = LM.FindMarkerIndOnTrace(Trace1, Markers[i].Freq);
                            Markers[i].TraceNumber.Parameter = "1";
                            //Markers[i].BitmapTexture = MarkerTexture(Markers[i].NameInLegend);
                        }
                        else if (t == "DELT " && i > 0)
                        {
                            Markers[i].State = true;
                            Markers[i].MarkerType = 1;
                            Markers[i].Freq = decimal.Parse(session.Query(":CALC:DELT" + Markers[i].Index.ToString() + ":X?;".Replace('.', ',')));
                            Markers[i].Funk1 = Markers[i].Freq - Markers[0].Freq;
                            Markers[i].IndexOnTrace = LM.FindMarkerIndOnTrace(Trace1, Markers[i].Freq);
                            Markers[i].TraceNumber.Parameter = "1";
                            //Markers[i].BitmapTexture = MarkerTexture(Markers[i].NameInLegend);
                        }
                        else if (t == "OFF") { Markers[i].State = false; }
                        #endregion
                    }
                    else if (UniqueData.InstrManufacture == 3)
                    {
                        #region
                        string t = session.Query(":CALC:MARK" + Markers[i].Index.ToString() + ":STAT?").TrimEnd();
                        //MessageBox.Show("1"+t+"1");
                        if (t == "1")
                        {
                            Markers[i].State = true;
                            Markers[i].MarkerType = 0;
                            Markers[i].Freq = decimal.Parse(session.Query(":CALC:MARK" + Markers[i].Index.ToString() + ":X?;").Replace('.', ','));
                            Markers[i].IndexOnTrace = LM.FindMarkerIndOnTrace(Trace1, Markers[i].Freq);
                            Markers[i].TraceNumber.Parameter = "1";
                            //Markers[i].BitmapTexture = MarkerTexture(Markers[i].NameInLegend);
                        }
                        else if (t == "DELT " && i > 0)
                        {
                            Markers[i].State = true;
                            Markers[i].MarkerType = 1;
                            Markers[i].Freq = decimal.Parse(session.Query(":CALC:DELT" + Markers[i].Index.ToString() + ":X?;".Replace('.', ',')));
                            Markers[i].Funk1 = Markers[i].Freq - Markers[0].Freq;
                            Markers[i].IndexOnTrace = LM.FindMarkerIndOnTrace(Trace1, Markers[i].Freq);
                            Markers[i].TraceNumber.Parameter = "1";
                            //Markers[i].BitmapTexture = MarkerTexture(Markers[i].NameInLegend);
                        }
                        else if (t == "OFF") { Markers[i].State = false; }
                        #endregion
                    }
                    Markers[i].StateNew = Markers[i].State;
                    Markers[i].MarkerTypeNew = Markers[i].MarkerType;
                    Markers[i].FreqNew = Markers[i].Freq;
                }
                #region Exception
                catch (VisaException v_exp)
                {
                    App.Current.Dispatcher.Invoke((Action)(() =>
                    {
                        MainWindow.exp.ExceptionData = new ExData() { ex = v_exp, ClassName = "ANALYZER", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
                    }));
                }
                catch (Exception exp)
                {
                    App.Current.Dispatcher.Invoke((Action)(() =>
                    {
                        MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "ANALYZER", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
                    }));
                }
                #endregion
            }
            an_dm -= GetRunMarkers;
        }

        public void SetMarkerToTrace()
        {
            try
            {
                for (int i = 0; i < Markers.Count; i++)
                {
                    if (UniqueData.InstrManufacture == 1)
                    {
                        if (Markers[i].State == true && UniqueData.HiSpeed == true)
                        {
                            if (Markers[i].MarkerType == 0 || Markers[i].MarkerType == 3 || Markers[i].MarkerType == 4)
                            {
                                session.Query("CALC:MARK" + Markers[i].Index.ToString() + ":TRAC " + Markers[i].TraceNumber.ToString());
                            }
                            else if (Markers[i].MarkerType == 1)
                            {
                                session.Query("CALC:DELT" + Markers[i].Index.ToString() + ":TRAC " + Markers[i].TraceNumber.ToString());
                            }
                        }
                    }
                }
            }
            #region Exception
            catch (VisaException v_exp)
            {
                App.Current.Dispatcher.Invoke((Action)(() =>
                {
                    MainWindow.exp.ExceptionData = new ExData() { ex = v_exp, ClassName = "ANALYZER", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
                }));
            }
            catch (Exception exp)
            {
                App.Current.Dispatcher.Invoke((Action)(() =>
                {
                    MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "ANALYZER", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
                }));
            }
            #endregion
            finally { an_dm -= SetMarkerToTrace; }
        }
        /// < summary >
        /// Получаем данные уровня и частоту маркера
        /// </ summary >
        private void SetMarkerData()
        {
            #region Markers
            for (int i = 0; i < Markers.Count(); i++)
            {
                if (Markers[i].State == true)
                {
                    try
                    {
                        if (Markers[i].MarkerType == 0 || Markers[i].MarkerType == 3 || Markers[i].MarkerType == 4)
                        {
                            if (UniqueData.InstrManufacture == 1 && Trace1.Length > 0)
                            {
                                if (UniqueData.HiSpeed == true)
                                {
                                    //Markers[i].Freq = double.Parse(session.Query(":CALC:MARK" + Markers[i].Name.ToString() + ":X?;".Replace('.', ',')));
                                    if (Markers[i].TraceNumberIndex == 0)
                                        Markers[i].Level = Trace1[Markers[i].IndexOnTrace].level;
                                    if (Markers[i].TraceNumberIndex == 1)
                                        Markers[i].Level = Trace2[Markers[i].IndexOnTrace].level;
                                    if (Markers[i].TraceNumberIndex == 2)
                                        Markers[i].Level = Trace3[Markers[i].IndexOnTrace].level;
                                    #region NdB
                                    if (NdBState == true && Markers[i].MarkerType == 3)
                                    {
                                        string[] temp1 = session.Query(":CALC:MARK:FUNC:NDBD:FREQ?").Split(',');
                                        decimal QFAC = Math.Round(decimal.Parse(session.Query(":CALC:MARK:FUNC:NDBD:QFAC?;").Replace('.', ',')), 2);
                                        int[] t = LM.GetMeasNDB(Trace1, Markers[i].IndexOnTrace, (double)NdBLevel);

                                        Markers[i].TMarkers[0].IndexOnTrace = t[0];
                                        Markers[i].TMarkers[0].Freq = Trace1[t[0]].freq;
                                        Markers[i].TMarkers[0].Level = Trace1[t[0]].level;

                                        Markers[i].TMarkers[1].IndexOnTrace = t[1];
                                        Markers[i].TMarkers[1].Freq = Trace1[t[1]].freq;
                                        Markers[i].TMarkers[1].Level = Trace1[t[1]].level;

                                        Markers[i].TMarkers[0].Funk2 = Markers[i].TMarkers[1].Freq - Markers[i].TMarkers[0].Freq;
                                        Markers[i].TMarkers[1].Funk2 = QFAC;
                                        NdBResult = Markers[i].TMarkers[0].Funk2;
                                    }
                                    #endregion NdB
                                    #region OBW
                                    if (OBWState == true && Markers[i].MarkerType == 3)
                                    {
                                        string[] temp1 = session.Query("CALC:MARK:FUNC:POW:RES? AOBW").Split(',');

                                        decimal tf1 = decimal.Parse(temp1[1].Replace('.', ','));
                                        Markers[i].TMarkers[0].IndexOnTrace = LM.FindMarkerIndOnTrace(Trace1, tf1);
                                        Markers[i].TMarkers[0].Freq = Trace1[Markers[i].TMarkers[0].IndexOnTrace].freq;
                                        Markers[i].TMarkers[0].Level = Trace1[Markers[i].TMarkers[0].IndexOnTrace].level;

                                        decimal tf2 = decimal.Parse(temp1[3].Replace('.', ','));
                                        Markers[i].TMarkers[1].IndexOnTrace = LM.FindMarkerIndOnTrace(Trace1, tf2);
                                        Markers[i].TMarkers[1].Freq = Trace1[Markers[i].TMarkers[1].IndexOnTrace].freq;
                                        Markers[i].TMarkers[1].Level = Trace1[Markers[i].TMarkers[1].IndexOnTrace].level;


                                        Markers[i].TMarkers[0].Funk2 = Markers[i].TMarkers[1].Freq - Markers[i].TMarkers[0].Freq;
                                        OBWResult = Markers[i].TMarkers[0].Funk2;//decimal.Parse(temp1[0].Replace('.', ','));
                                    }
                                    #endregion OBW
                                }
                                else
                                {
                                    if (Markers[i].TraceNumberIndex == 0)
                                        Markers[i].Level = Trace1[Markers[i].IndexOnTrace].level;
                                    if (Markers[i].TraceNumberIndex == 1)
                                        Markers[i].Level = Trace2[Markers[i].IndexOnTrace].level;
                                    if (Markers[i].TraceNumberIndex == 2)
                                        Markers[i].Level = Trace3[Markers[i].IndexOnTrace].level;
                                    #region NdB
                                    if (NdBState == true && Markers[i].MarkerType == 3 && Markers[i].TMarkers != null && Markers[i].TMarkers.Count > 0)
                                    {
                                        //string st = session.Query("CALCulate:MARK:FUNCtion:NDBDown:RESult?").TrimEnd();
                                        //decimal bw = (decimal)Math.Round(double.Parse(st, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture), 2);
                                        string[] temp1 = session.Query(":CALC:MARK:FUNC:NDBD:FREQ?").Split(',');
                                        if (temp1.Length > 1)
                                        {
                                            Markers[i].TMarkers[0].Freq = decimal.Parse(temp1[0].Replace('.', ','));
                                            Markers[i].TMarkers[0].IndexOnTrace = LM.FindMarkerIndOnTrace(Trace1, Markers[i].TMarkers[0].Freq);
                                            Markers[i].TMarkers[0].Level = Trace1[Markers[i].TMarkers[0].IndexOnTrace].level;

                                            Markers[i].TMarkers[1].Freq = decimal.Parse(temp1[1].Replace('.', ','));
                                            Markers[i].TMarkers[1].IndexOnTrace = LM.FindMarkerIndOnTrace(Trace1, Markers[i].TMarkers[1].Freq);
                                            Markers[i].TMarkers[1].Level = Trace1[Markers[i].TMarkers[1].IndexOnTrace].level;

                                            Markers[i].TMarkers[0].Funk2 = Markers[i].TMarkers[1].Freq - Markers[i].TMarkers[0].Freq;
                                            NdBResult = Markers[i].TMarkers[0].Funk2;
                                        }
                                    }
                                    #endregion
                                    #region OBW
                                    if (OBWState == true && Markers[i].MarkerType == 4 && Markers[i].TMarkers != null && Markers[i].TMarkers.Count > 0)
                                    {
                                        string st = session.Query("CALC:MARK:FUNC:POW:RES? OBW").TrimEnd();
                                        if (st != null && st != "99.1e+36")
                                        {
                                            double bw = double.Parse(st, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture);
                                            //string[] temp1 = session.Query(":CALC:MARK:FUNC:OBW:FREQ?").Split(',');
                                            //if (temp1.Length > 1)
                                            //{
                                            //    //Markers[i].TMarkers[0].Freq = decimal.Parse(temp1[0].Replace('.', ','));
                                            //    //Markers[i].TMarkers[0].IndexOnTrace = LM.FindMarkerIndOnTrace(Trace1, Markers[i].TMarkers[0].Freq);
                                            //    //Markers[i].TMarkers[0].Level = Trace1[Markers[i].TMarkers[0].IndexOnTrace].Level;

                                            //    //Markers[i].TMarkers[1].Freq = decimal.Parse(temp1[1].Replace('.', ','));
                                            //    //Markers[i].TMarkers[1].IndexOnTrace = LM.FindMarkerIndOnTrace(Trace1, Markers[i].TMarkers[1].Freq);
                                            //    //Markers[i].TMarkers[1].Level = Trace1[Markers[i].TMarkers[1].IndexOnTrace].Level;

                                            //    Markers[i].TMarkers[0].Funk2 = bw;// Markers[i].TMarkers[1].Freq - Markers[i].TMarkers[0].Freq;
                                            //    OBWResult = Markers[i].TMarkers[0].Funk2;
                                            //}

                                            Markers[i].TMarkers[0].Funk2 = (decimal)bw;
                                            OBWResult = Markers[i].TMarkers[0].Funk2;
                                        }
                                    }
                                    #endregion OBW
                                }
                            }
                            else if (UniqueData.InstrManufacture == 2)
                            {
                                if (Trace1.Length > 0)
                                {
                                    //Markers[i].Freq = double.Parse(session.Query(":CALC:MARK" + Markers[i].Name.ToString() + ":X?;".Replace('.', ',')));
                                    //Markers[i].Level = Trace1[Markers[i].IndexOnTrace].level;
                                    if (Markers[i].TraceNumberIndex == 0)
                                        Markers[i].Level = Trace1[Markers[i].IndexOnTrace].level;
                                    if (Markers[i].TraceNumberIndex == 1)
                                        Markers[i].Level = Trace2[Markers[i].IndexOnTrace].level;
                                    if (Markers[i].TraceNumberIndex == 2)
                                        Markers[i].Level = Trace3[Markers[i].IndexOnTrace].level;
                                }
                            }
                            else if (UniqueData.InstrManufacture == 3)
                            {
                                if (Trace1.Length > 0)
                                {
                                    //Markers[i].Freq = double.Parse(session.Query(":CALC:MARK" + Markers[i].Name.ToString() + ":X?;".Replace('.', ',')));
                                    if (Markers[i].TraceNumberIndex == 0)
                                        Markers[i].Level = Trace1[Markers[i].IndexOnTrace].level;
                                    if (Markers[i].TraceNumberIndex == 1)
                                        Markers[i].Level = Trace2[Markers[i].IndexOnTrace].level;
                                    if (Markers[i].TraceNumberIndex == 2)
                                        Markers[i].Level = Trace3[Markers[i].IndexOnTrace].level;
                                    //Markers[i].Level = Trace1[Markers[i].IndexOnTrace].level;
                                    if (NdBState == true && Markers[i].MarkerType == 3 && Markers[i].TMarkers != null && Markers[i].TMarkers.Count > 0)
                                    {

                                    }
                                }
                            }
                        }
                        if (Markers[i].MarkerType == 1)
                        {
                            if (UniqueData.InstrManufacture == 1 && Trace1.Length > 0)
                            {
                                if (UniqueData.HiSpeed == true)
                                {
                                    string st = session.Query(":CALC:DELT" + Markers[i].Name.ToString() + ":X?;".Replace('.', ',')).TrimEnd();
                                    Markers[i].Freq = decimal.Parse(st, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture);
                                }
                                else
                                {
                                    Markers[i].Level = Trace1[Markers[i].IndexOnTrace].level;
                                    //string st = session.Query(":CALC:DELT" + Markers[i].Name.ToString() + ":X?;".Replace('.', ',')).TrimEnd();
                                    //Markers[i].Freq = double.Parse(st, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture);
                                }
                            }
                        }
                    }
                    #region Exception
                    catch (VisaException v_exp)
                    {
                        MainWindow.exp.ExceptionData = new ExData() { ex = v_exp, ClassName = "ANALYZER", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
                    }
                    catch (Exception exp)
                    {
                        MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "ANALYZER", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
                    }
                    #endregion
                }
            }
            for (int i = 0; i < Markers.Count(); i++)
            {
                if (Markers[i].State == true && Markers[i].MarkerType == 1 && Markers[i].MarkerParent != null)
                {
                    if (Markers[i].Funk1 != Markers[i].Freq - Markers[i].MarkerParent.Freq) Markers[i].Funk1 = Markers[i].Freq - Markers[i].MarkerParent.Freq;
                    if ((double)Markers[i].Funk2 != Markers[i].Level - Markers[i].MarkerParent.Level) Markers[i].Funk2 = (decimal)(Markers[i].Level - Markers[i].MarkerParent.Level);
                }
            }
            #endregion
        }
        #endregion Markers

        #region Measurment
        #region NdB
        private void SetNdB()
        {
            try
            {
                if (UniqueData.NdB == true)
                {
                    if (UniqueData.InstrManufacture == 1)
                    {
                        if (NdBState == true)
                        {
                            session.Write(":CALC:MARK1:FUNC:NDBD:STAT ON");
                            NdBLevel = decimal.Parse(session.Query(":CALC:MARK:FUNC:NDBD?").Replace('.', ','));
                            Markers[0].Funk2 = NdBLevel;
                            Markers[0].MarkerType = 3;
                            Markers[0].MarkerTypeNew = 3;
                            Markers[0].FunctionDataType = 2;
                            if (Markers[0].State == false)//был выключен
                            {
                                Markers[0].StateNew = true;

                                Markers[0].IndexOnTrace = LM.PeakSearch(Trace1);
                                Markers[0].Freq = Trace1[Markers[0].IndexOnTrace].freq;
                                SetToDelegate(SetMarkerState);
                            }
                        }
                        else
                        {
                            Markers[0].MarkerType = 0;
                            Markers[0].FunctionDataType = 0;
                            session.Write(":CALC:MARK1:FUNC:NDBD:STAT OFF");
                        }
                    }
                    else if (UniqueData.InstrManufacture == 3)
                    {
                        if (NdBState == true)
                        {
                            session.Write(":SENSe:OBWidth:STATe 1;:OBWidth:METHod XDB");
                            NdBLevel = decimal.Parse(session.Query(":OBWidth:XDB?").Replace('.', ','));
                            Markers[0].Funk2 = NdBLevel;
                            Markers[0].MarkerType = 3;
                            Markers[0].MarkerTypeNew = 3;
                            Markers[0].FunctionDataType = 2;
                            if (Markers[0].State == false)//был выключен
                            {
                                Markers[0].StateNew = true;

                                Markers[0].IndexOnTrace = LM.PeakSearch(Trace1);
                                Markers[0].Freq = Trace1[Markers[0].IndexOnTrace].freq;
                                SetToDelegate(SetMarkerState);
                            }
                        }
                        else
                        {
                            Markers[0].MarkerType = 0;
                            Markers[0].FunctionDataType = 0;
                            session.Write(":SENSe:OBWidth:STATe 0");
                        }
                    }
                }
            }
            #region Exception
            catch (VisaException v_exp)
            {
                App.Current.Dispatcher.Invoke((Action)(() =>
                {
                    MainWindow.exp.ExceptionData = new ExData() { ex = v_exp, ClassName = "ANALYZER", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
                }));
            }
            catch (Exception exp)
            {
                App.Current.Dispatcher.Invoke((Action)(() =>
                {
                    MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "ANALYZER", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
                }));
            }
            #endregion
            an_dm -= SetNdB;
        }
        private void GetNdB()
        {
            try
            {
                if (UniqueData.NdB == true)
                {
                    if (UniqueData.InstrManufacture == 1)
                    {
                        string st = session.Query(":CALC:MARK1:FUNC:NDBD:STAT?").TrimEnd();
                        if (st.Contains("1"))
                        {
                            NdBLevel = decimal.Parse(session.Query(":CALC:MARK:FUNC:NDBD?").Replace('.', ','));
                            NdBState = true;
                            Markers[0].Funk2 = NdBLevel;
                            Markers[0].MarkerType = 3;
                            Markers[0].MarkerTypeNew = 3;
                            Markers[0].FunctionDataType = 2;

                        }
                        else
                        {
                            NdBState = false;
                            Markers[0].MarkerType = 0;
                            Markers[0].MarkerTypeNew = 0;
                            Markers[0].FunctionDataType = 0;
                        }
                    }
                    else if (UniqueData.InstrManufacture == 3)
                    {
                        string st = session.Query(":SENSe:OBWidth:STATe?").TrimEnd();
                        string st2 = session.Query(":OBWidth:METHod?").TrimEnd();
                        if (st.Contains("1") && st.Contains("XDB"))
                        {
                            NdBState = true;
                            NdBLevel = decimal.Parse(session.Query(":OBWidth:XDB?").Replace('.', ','));
                            Markers[0].Funk2 = NdBLevel;
                            Markers[0].MarkerType = 3;
                            Markers[0].MarkerTypeNew = 3;
                            Markers[0].FunctionDataType = 2;
                        }
                        else
                        {
                            NdBState = false;
                            Markers[0].MarkerType = 0;
                            Markers[0].MarkerTypeNew = 0;
                            Markers[0].FunctionDataType = 0;
                        }
                    }
                }
            }
            #region Exception
            catch (VisaException v_exp)
            {
                App.Current.Dispatcher.Invoke((Action)(() =>
                {
                    MainWindow.exp.ExceptionData = new ExData() { ex = v_exp, ClassName = "ANALYZER", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
                }));
            }
            catch (Exception exp)
            {
                App.Current.Dispatcher.Invoke((Action)(() =>
                {
                    MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "ANALYZER", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
                }));
            }
            #endregion
            an_dm -= GetNdB;
        }
        private void SetNdBLevel()
        {
            try
            {
                if (UniqueData.NdB == true)
                {
                    if (UniqueData.InstrManufacture == 1)
                    {
                        session.Write(":CALC:MARK:FUNC:NDBD " + NdBLevel.ToString().Replace(',', '.'));
                        Markers[0].Funk2 = NdBLevel;
                    }
                    else if (UniqueData.InstrManufacture == 3)
                    {
                        session.Write(":OBWidth:XDB " + NdBLevel.ToString().Replace(',', '.'));
                        Markers[0].Funk2 = NdBLevel;

                    }
                }
            }
            #region Exception
            catch (VisaException v_exp)
            {
                App.Current.Dispatcher.Invoke((Action)(() =>
                {
                    MainWindow.exp.ExceptionData = new ExData() { ex = v_exp, ClassName = "ANALYZER", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
                }));
            }
            catch (Exception exp)
            {
                App.Current.Dispatcher.Invoke((Action)(() =>
                {
                    MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "ANALYZER", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
                }));
            }
            #endregion
            an_dm -= SetNdBLevel;
        }
        #endregion NdB

        #region OBW
        private void SetOBW()
        {
            try
            {
                if (UniqueData.OBW == true)
                {
                    if (UniqueData.InstrManufacture == 1)
                    {
                        if (OBWState == true)
                        {
                            session.Write(":CALC:MARK:FUNC:POW:SEL OBW");
                            if (UniqueData.HiSpeed == true)
                            {
                                OBWPercent = decimal.Parse(session.Query(":POW:BWID?").Replace('.', ','));
                            }
                            else
                            {
                                string st = session.Query("CALC:MARK:FUNC:CPOW:BAND?").TrimEnd();
                                OBWChnlBW = decimal.Parse(st, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture);
                                OBWPercent = decimal.Parse(session.Query("CALC:MARK:FUNC:OBW:BAND:PCT?").Replace('.', ','));
                            }


                            Markers[0].Funk2 = OBWPercent;
                            Markers[0].MarkerType = 4;
                            Markers[0].MarkerTypeNew = 4;
                            Markers[0].FunctionDataType = 5;
                            if (Markers[0].State == false)//был выключен
                            {
                                Markers[0].StateNew = true;

                                Markers[0].IndexOnTrace = LM.PeakSearch(Trace1);
                                Markers[0].Freq = Trace1[Markers[0].IndexOnTrace].freq;
                                SetToDelegate(SetMarkerState);
                            }
                        }
                        else
                        {
                            Markers[0].MarkerType = 0;
                            Markers[0].FunctionDataType = 0;
                            session.Write(":CALC:MARK:FUNC:POW OFF");
                        }
                    }
                    else if (UniqueData.InstrManufacture == 3)
                    {
                        #region
                        if (NdBState == true)
                        {
                            session.Write(":SENSe:OBWidth:STATe 1;:OBWidth:METHod XDB");
                            NdBLevel = decimal.Parse(session.Query(":OBWidth:XDB?").Replace('.', ','));
                            Markers[0].Funk2 = NdBLevel;
                            Markers[0].MarkerType = 3;
                            Markers[0].MarkerTypeNew = 3;
                            Markers[0].FunctionDataType = 2;
                            if (Markers[0].State == false)//был выключен
                            {
                                Markers[0].StateNew = true;

                                Markers[0].IndexOnTrace = LM.PeakSearch(Trace1);
                                Markers[0].Freq = Trace1[Markers[0].IndexOnTrace].freq;
                                SetToDelegate(SetMarkerState);
                            }
                        }
                        else
                        {
                            Markers[0].MarkerType = 0;
                            Markers[0].FunctionDataType = 0;
                            session.Write(":SENSe:OBWidth:STATe 0");
                        }
                        #endregion
                    }
                }
            }
            #region Exception
            catch (VisaException v_exp)
            {
                App.Current.Dispatcher.Invoke((Action)(() =>
                {
                    MainWindow.exp.ExceptionData = new ExData() { ex = v_exp, ClassName = "ANALYZER", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
                }));
            }
            catch (Exception exp)
            {
                App.Current.Dispatcher.Invoke((Action)(() =>
                {
                    MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "ANALYZER", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
                }));
            }
            #endregion
            an_dm -= SetOBW;
        }
        private void GetOBW()
        {
            try
            {
                if (UniqueData.OBW == true)
                {
                    if (UniqueData.InstrManufacture == 1)
                    {
                        //string st = session.Query(":CALC:MARK:FUNC:POW?").TrimEnd();
                        //string st1 = session.Query(":CALC:MARK:FUNC:POW:SEL?").TrimEnd();
                        //if (st.Contains("1") && st.Contains("OBW"))
                        //{
                        //    OBWState = true;
                        //    Markers[0].Funk2 = OBWPercent;
                        //    Markers[0].MarkerType = 3;
                        //    Markers[0].FunctionDataType = 2;

                        //}
                        //else
                        //{
                        //    OBWState = false;
                        //    Markers[0].MarkerType = 0;
                        //    Markers[0].FunctionDataType = 0;
                        //}
                    }
                    else if (UniqueData.InstrManufacture == 3)
                    {
                        //string st = session.Query(":SENSe:OBWidth:STATe?").TrimEnd();
                        //string st2 = session.Query(":OBWidth:METHod?").TrimEnd();
                        //if (st.Contains("1") && st.Contains("XDB"))
                        //{
                        //    OBWState = true;
                        //    //NdBLevel = decimal.Parse(session.Query(":OBWidth:XDB?").Replace('.', ','));
                        //    Markers[0].Funk2 = NdBLevel;
                        //    Markers[0].MarkerType = 3;
                        //    Markers[0].FunctionDataType = 2;
                        //}
                        //else
                        //{
                        //    OBWState = false;
                        //    Markers[0].MarkerType = 0;
                        //    Markers[0].FunctionDataType = 0;
                        //}
                    }
                }
            }
            #region Exception
            catch (VisaException v_exp)
            {
                App.Current.Dispatcher.Invoke((Action)(() =>
                {
                    MainWindow.exp.ExceptionData = new ExData() { ex = v_exp, ClassName = "ANALYZER", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
                }));
            }
            catch (Exception exp)
            {
                App.Current.Dispatcher.Invoke((Action)(() =>
                {
                    MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "ANALYZER", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
                }));
            }
            #endregion
            an_dm -= GetOBW;
        }
        private void SetOBWPercent()
        {
            try
            {
                if (UniqueData.OBW == true)
                {
                    if (UniqueData.InstrManufacture == 1)
                    {
                        if (UniqueData.HiSpeed == true)
                        {
                            session.Write(":POW:BWID " + Math.Round(OBWPercent, 2).ToString().Replace(',', '.') + "PCT");
                        }
                        else
                        {
                            session.Write(":CALC:MARK:FUNC:OBW:BAND:PCT " + Math.Round(OBWPercent, 2).ToString().Replace(',', '.'));
                        }
                    }
                    else if (UniqueData.InstrManufacture == 2)
                    {
                        session.Write(":OBW:PPOW " + Math.Round(OBWPercent, 2).ToString().Replace(',', '.'));
                    }
                    else if (UniqueData.InstrManufacture == 3)
                    {
                        session.Write(":OBW:PERC " + Math.Round(OBWPercent, 2).ToString().Replace(',', '.'));
                    }
                    Markers[0].Funk2 = OBWPercent;
                }
            }
            #region Exception
            catch (VisaException v_exp)
            {
                App.Current.Dispatcher.Invoke((Action)(() =>
                {
                    MainWindow.exp.ExceptionData = new ExData() { ex = v_exp, ClassName = "ANALYZER", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
                }));
            }
            catch (Exception exp)
            {
                App.Current.Dispatcher.Invoke((Action)(() =>
                {
                    MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "ANALYZER", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
                }));
            }
            #endregion
            an_dm -= SetOBWPercent;
        }
        private void SetOBWChnlBW()
        {
            try
            {
                if (UniqueData.OBW == true)
                {
                    if (UniqueData.InstrManufacture == 1)
                    {
                        if (UniqueData.HiSpeed == true)
                        {

                        }
                        else
                        {
                            session.Write("CALC:MARK:FUNC:CPOW:BAND " + OBWChnlBW.ToString().Replace(',', '.'));
                        }
                    }
                    else if (UniqueData.InstrManufacture == 2)
                    {
                    }
                    else if (UniqueData.InstrManufacture == 3)
                    {
                    }
                    //Markers[0].Funk2 = OBWChnlBW;
                }
            }
            #region Exception
            catch (VisaException v_exp)
            {
                App.Current.Dispatcher.Invoke((Action)(() =>
                {
                    MainWindow.exp.ExceptionData = new ExData() { ex = v_exp, ClassName = "ANALYZER", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
                }));
            }
            catch (Exception exp)
            {
                App.Current.Dispatcher.Invoke((Action)(() =>
                {
                    MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "ANALYZER", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
                }));
            }
            #endregion
            an_dm -= SetOBWChnlBW;
        }
        #endregion OBW

        #endregion Measurment

        /// <summary>
        /// Get PowerRegister & Battery
        /// </summary>
        private void GetDeviceInfo()
        {
            try
            {
                if (UniqueData.InstrManufacture == 1)
                {
                    if (UniqueData.Battery)
                    {
                        string[] st = session.Query(":STAT:QUES:POW?;:SYSTem:POWer:STATus?").TrimEnd().Split(';');
                        PowerRegister = int.Parse(st[0]);
                        double d = double.Parse(st[1], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture);
                        if (d > 100) { BatteryCharging = true; }
                        else if (d > 0 && d <= 100) { BatteryCharging = false; BatteryCharge = (decimal)d; }
                    }
                    else
                    {
                        PowerRegister = int.Parse(session.Query(":STAT:QUES:POW?"));
                    }
                }
                else if (UniqueData.InstrManufacture == 2)
                {
                    string t = session.Query(":SYST:BATT:ARTT?");
                    //BatteryAbsoluteCharge = int.Parse(session.Query(":SYST:BATT:ACUR?").Replace('.', ','));
                    //System.Windows.MessageBox.Show(t);
                }
            }
            #region Exception
            catch (VisaException v_exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = v_exp, ClassName = "ANALYZER", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            catch (Exception exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "ANALYZER", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            #endregion
            finally { an_dm -= GetDeviceInfo; }
        }
        private void GetSetAnSysDateTime()
        {
            try
            {
                if (UniqueData.InstrManufacture == 1)
                {
                    string[] d = session.Query("SYST:DATE?").TrimEnd().Trim(' ').Split(',');
                    string[] t = session.Query("SYST:TIME?").TrimEnd().Trim(' ').Split(',');
                    string time = d[0] + "-" + d[1] + "-" + d[2] + " " +
                        t[0] + ":" + t[1] + ":" + t[2];
                    DateTime andt = DateTime.Parse(time);
                    if (new TimeSpan(DateTime.Now.Ticks - andt.Ticks) > new TimeSpan(0, 0, 1, 0, 0))
                    {
                        session.Write("SYST:DATE " + DateTime.Now.Year.ToString() + "," + DateTime.Now.Month.ToString() + "," + DateTime.Now.Day.ToString());
                        session.Write("SYST:TIME " + DateTime.Now.Hour.ToString() + "," + DateTime.Now.Minute.ToString() + "," + DateTime.Now.Second.ToString());
                    }
                }
                else if (UniqueData.InstrManufacture == 2)
                {

                }
                else if (UniqueData.InstrManufacture == 3)
                {

                }
            }
            #region Exception
            catch (VisaException v_exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = v_exp, ClassName = "ANALYZER", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            catch (Exception exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "ANALYZER", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            #endregion
            finally { an_dm -= GetSetAnSysDateTime; }
        }

        private void GetScreenFromDevice()
        {
            try
            {
                if (true)//StateResult == true)
                {
                    if (UniqueData.InstrManufacture == 1)
                    {
                        if (UniqueData.HiSpeed == true)
                        {

                            session.Write("HCOP:DEST 'MMEM'");
                            session.Write("MMEM:NAME 'C:\\ControlUTemp.PNG'");
                            session.Write("HCOP");
                            session.DefaultBufferSize = 262136;
                            session.Write(":MMEMory:Data? 'C:\\ControlUTemp.png'");
                            var byteArray = session.ReadByteArray(7);
                            byteArray = session.ReadByteArray();
                            session.DefaultBufferSize = SweepPoints * 18 + 25;
                            System.IO.File.WriteAllBytes(Sett.Screen_Settings.ScreenFolder + "\\" + ScreenName + ".png", byteArray);
                            session.Write(":MMEMory:DELete 'C:\\ControlUTemp.PNG'");
                            #region old with SMB
                            //session.Write("HCOP:DEST 'MMEM'");
                            //session.Write("MMEM:NAME 'Z:\\" + ScreenName + "." + Sett.Screen_Settings.SaveScreenImageFormat.ToUpper() + "'");
                            //session.Write("HCOP");
                            #endregion
                        }
                        else if (UniqueData.HiSpeed == false)
                        {
                            if (UniqueData.InstrModel.Contains("FPH"))
                            #region
                            {
                                session.DefaultBufferSize = 262136;
                                session.Write("HCOP:DEV:LANG PNG");
                                session.Write("HCOP:DEST 'MMEM'");//
                                session.Write(@"MMEM:NAME '\Public\Screen Shots\ControlUTemp.png'");
                                session.Write("DISP:WIND:STOR");
                                session.Write("HCOP");
                                session.Write(@":MMEMory:Data? '\Public\Screen Shots\ControlUTemp.png'");
                                var byteArray = session.ReadByteArray();
                                int start = 0;// stop = 0;
                                for (int i = 0; i < 10; i++)
                                {
                                    if (byteArray[i].ToString() == "137")
                                    {
                                        start = i;
                                    }
                                }
                                byte[] outArray = new byte[byteArray.Length - start];
                                for (int i = 0; i < byteArray.Length - start; i++)
                                {
                                    outArray[i] = byteArray[i + start];
                                }
                                session.DefaultBufferSize = SweepPoints * 18 + 25;
                                System.IO.File.WriteAllBytes(Sett.Screen_Settings.ScreenFolder + "\\" + ScreenName + ".png", outArray);
                                //session.Write(@":MMEMory:DELete '\Public\Screen Shots\FMeasTemp.png'");
                            }
                            #endregion
                            else if (UniqueData.InstrModel.Contains("ZVH"))
                            #region
                            {
                                //MessageBox.Show(StateResult.ToString());
                                session.DefaultBufferSize = 262136;
                                session.Write("HCOP:DEV:LANG PNG");
                                session.Write("HCOP:DEST 'MMEM'");//
                                session.Write(@"MMEM:NAME '\Public\Screen Shots\ControlUTemp.png'");
                                session.Write("DISP:WIND:STOR");

                                session.Write("HCOP");
                                session.Write(@":MMEMory:Data? '\Public\Screen Shots\ControlUTemp.png'");
                                var byteArray = session.ReadByteArray();
                                int start = 0;// stop = 0;
                                for (int i = 0; i < 10; i++)
                                {
                                    if (byteArray[i].ToString() == "137")
                                    {
                                        start = i;
                                    }
                                }
                                byte[] outArray = new byte[byteArray.Length - start];
                                for (int i = 0; i < byteArray.Length - start; i++)
                                {
                                    outArray[i] = byteArray[i + start];
                                }
                                session.DefaultBufferSize = SweepPoints * 18 + 25;
                                System.IO.File.WriteAllBytes(Sett.Screen_Settings.ScreenFolder + "\\" + ScreenName + ".png", outArray);
                                session.Write(@":MMEMory:DELete '\Public\Screen Shots\ControlUTemp.png'");

                            }
                            #endregion
                        }
                    }
                    else if (UniqueData.InstrManufacture == 2)
                    {
                        session.Write(":MMEMory:STORe:IMAGe \"ControlUTemp.png\"");
                        session.Write(":MMEMory:Data? \"ControlUTemp.png\"");
                        session.DefaultBufferSize = 262136;
                        var byteArray = session.ReadByteArray(7);
                        byteArray = session.ReadByteArray();
                        session.DefaultBufferSize = SweepPoints * 4 + 20;
                        System.IO.File.WriteAllBytes(Sett.Screen_Settings.ScreenFolder + "\\" + ScreenName + ".png", byteArray);
                        session.Write(":MMEMory:DELete \"ControlUTemp.png\"");

                    }
                    else if (UniqueData.InstrManufacture == 3)
                    {

                        session.Write(":MMEMory:STORe:JPEG \"ControlUTemp\"");
                        session.Write(":MMEMory:DATA? \"ControlUTemp.jpg\"");
                        session.DefaultBufferSize = 262136;
                        var byteArray = session.ReadByteArray(8);
                        byteArray = session.ReadByteArray();
                        session.DefaultBufferSize = 2204 + 20;
                        System.IO.File.WriteAllBytes(Sett.Screen_Settings.ScreenFolder + "\\" + ScreenName + ".jpg", byteArray);
                        session.Write(":MMEMory:DELete \"ControlUTemp.jpg\"");

                    }
                }
            }
            #region Exception
            catch (VisaException v_exp)
            {
                App.Current.Dispatcher.Invoke((Action)(() =>
                {
                    MainWindow.exp.ExceptionData = new ExData() { ex = v_exp, ClassName = "ANALYZER", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
                }));
            }
            catch (Exception exp)
            {
                App.Current.Dispatcher.Invoke((Action)(() =>
                {
                    MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "ANALYZER", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
                }));
            }
            #endregion
            finally { an_dm -= GetScreenFromDevice; }

        }

        private void InstrShutdown()
        {
            try
            {
                if (UniqueData.InstrManufacture == 1)
                {
                    session.Write(":SYSTem:SHUTdown");
                }
                else if (UniqueData.InstrManufacture == 2)
                {
                    session.Write(":SYST:PWR:SHUT 1");
                }
                Run = false;
                //session.Dispose();
                //session = null;
            }
            #region Exception
            catch (VisaException v_exp)
            {
                App.Current.Dispatcher.Invoke((Action)(() =>
                {
                    MainWindow.exp.ExceptionData = new ExData() { ex = v_exp, ClassName = "ANALYZER", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
                }));
            }
            catch (Exception exp)
            {
                App.Current.Dispatcher.Invoke((Action)(() =>
                {
                    MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "ANALYZER", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
                }));
            }
            #endregion
            finally { an_dm -= InstrShutdown; }
        }
        /// <summary>
        /// Сбрасываем настойки прибора 
        /// </summary>
        private void Preset()
        {
            try
            {
                if (UniqueData.InstrManufacture == 1)
                {
                    session.Write(":SYSTem:PRES");
                }
                else if (UniqueData.InstrManufacture == 3)
                {
                    session.Write(":SYSTem:PRES");
                }
                an_dm += GetLevelUnit;
                an_dm += GetFreqCentr;
                an_dm += GetFreqSpan;
                an_dm += GetRBW;
                an_dm += GetAutoRBW;
                an_dm += GetVBW;
                an_dm += GetAutoVBW;

                //an_dm += SetCouplingRatio;
                an_dm += GetSweepTime;
                an_dm += GetAutoSweepTime;
                an_dm += GetSweepType;
                an_dm += GetSweepPoints;
                an_dm += GetRefLevel;
                an_dm += GetAttLevel;
                an_dm += GetAutoAttLevel;
                an_dm += GetPreAmp;
                an_dm += GetTraceType;
                an_dm += GetDetectorType;
                an_dm += GetAverageCount;
                an_dm += GetNumberOfSweeps;
                an_dm += GetRunMarkers;

                an_dm += GetNdB;


                an_dm += GetTransducer;
                an_dm += GetSelectedTransducer;
                an_dm += GetSetAnSysDateTime;
            }
            #region Exception
            catch (VisaException v_exp)
            {
                App.Current.Dispatcher.Invoke((Action)(() =>
                {
                    MainWindow.exp.ExceptionData = new ExData() { ex = v_exp, ClassName = "ANALYZER", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
                }));
            }
            catch (Exception exp)
            {
                App.Current.Dispatcher.Invoke((Action)(() =>
                {
                    MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "ANALYZER", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
                }));
            }
            #endregion
            finally { an_dm -= Preset; }
        }

        #region Meas
        #region ChannelPower
        private void SetChannelPower()
        {
            try
            {
                if (UniqueData.ChnPow == true)
                {
                    if (UniqueData.InstrManufacture == 1)
                    {
                        if (UniqueData.HiSpeed == true)
                        {
                            if (ChannelPowerState)
                            {
                                session.Write("CALC:MARK:FUNC:POW:SEL CPOW");
                                ChannelPowerBW = decimal.Parse(session.Query(":SENS:POW:ACH:BAND?").Replace('.', ','));
                            }
                            else { session.Write("CALC:MARK:FUNC:POW:SEL 0"); }
                        }
                        else
                        {
                            if (ChannelPowerState)
                            {
                                session.Write("CALC:MARK:FUNC:POW ON");
                                session.Write("CALC:MARK:FUNC:POW:SEL CPOW");
                                ChannelPowerBW = decimal.Parse(session.Query("CALC:MARK:FUNC:CPOW:BAND?").Replace('.', ','));
                            }
                            else { session.Write("CALC:MARK:FUNC:POW OFF"); }
                        }
                    }
                    else if (UniqueData.InstrManufacture == 2)
                    {
                        //ChannelPowerBW = decimal.Parse(session.Query(":SENSe:ACPower:OFFSet:FREQuency?").Replace('.', ','));
                    }
                }
            }
            #region Exception
            catch (VisaException v_exp)
            {
                App.Current.Dispatcher.Invoke((Action)(() =>
                {
                    MainWindow.exp.ExceptionData = new ExData() { ex = v_exp, ClassName = "ANALYZER", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
                }));
            }
            catch (Exception exp)
            {
                App.Current.Dispatcher.Invoke((Action)(() =>
                {
                    MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "ANALYZER", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
                }));
            }
            #endregion
            finally { an_dm -= SetChannelPower; }
        }
        private void GetChannelPower()
        {
            try
            {
                if (UniqueData.ChnPow == true)
                {
                    if (UniqueData.InstrManufacture == 1)
                    {
                        if (UniqueData.HiSpeed == true)
                        {
                            String temp = session.Query("CALC:LIM:ACP?").TrimEnd();
                            if (temp.Contains("CPOW")) { ChannelPowerBW = decimal.Parse(session.Query(":SENS:POW:ACH:BAND?").Replace('.', ',')); ChannelPowerState = true; }
                            else if (temp.Contains("0")) { ChannelPowerState = false; }
                        }
                        else
                        {
                            String temp = session.Query("CALC:MARK:FUNC:POW?").TrimEnd();
                            if (temp.Contains("1")) { ChannelPowerBW = decimal.Parse(session.Query("CALC:MARK:FUNC:CPOW:BAND?").Replace('.', ',')); ChannelPowerState = true; }
                            else if (temp.Contains("0")) { ChannelPowerState = false; }
                        }
                    }
                }
            }
            #region Exception
            catch (VisaException v_exp)
            {
                App.Current.Dispatcher.Invoke((Action)(() =>
                {
                    MainWindow.exp.ExceptionData = new ExData() { ex = v_exp, ClassName = "ANALYZER", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
                }));
            }
            catch (Exception exp)
            {
                App.Current.Dispatcher.Invoke((Action)(() =>
                {
                    MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "ANALYZER", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
                }));
            }
            #endregion
            finally { an_dm -= GetChannelPower; }
        }
        private void SetChannelPowerBW()
        {
            try
            {
                if (UniqueData.ChnPow == true)
                {
                    if (UniqueData.InstrManufacture == 1)
                    {
                        if (UniqueData.HiSpeed == true)
                        {
                            session.Write(":SENS:POW:ACH:BAND " + ChannelPowerBW.ToString().Replace(',', '.'));
                        }
                        else
                        {
                            session.Write("CALC:MARK:FUNC:CPOW:BAND " + ChannelPowerBW.ToString().Replace(',', '.'));
                        }
                    }
                    else if (UniqueData.InstrManufacture == 2)
                    {
                        //ChannelPowerBW = decimal.Parse(session.Query(":SENSe:ACPower:OFFSet:FREQuency?").Replace('.', ','));
                    }
                }
            }
            #region Exception
            catch (VisaException v_exp)
            {
                App.Current.Dispatcher.Invoke((Action)(() =>
                {
                    MainWindow.exp.ExceptionData = new ExData() { ex = v_exp, ClassName = "ANALYZER", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
                }));
            }
            catch (Exception exp)
            {
                App.Current.Dispatcher.Invoke((Action)(() =>
                {
                    MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "ANALYZER", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
                }));
            }
            #endregion
            finally { an_dm -= SetChannelPowerBW; }
        }
        private void GetChannelPowerResult()
        {
            try
            {
                if (UniqueData.ChnPow == true)
                {
                    if (UniqueData.InstrManufacture == 1)
                    {
                        if (UniqueData.HiSpeed == true)
                        {
                            ChannelPowerResult = Math.Round(double.Parse(session.Query("CALC:MARK:FUNC:POW:RES? ACP").Replace('.', ',')), 2);
                        }
                        else if (UniqueData.HiSpeed == false)
                        {
                            ChannelPowerResult = Math.Round(double.Parse(session.Query("CALC:MARK:FUNC:POW:RES? CPOW").Replace('.', ',')), 2);
                        }
                    }
                    else if (UniqueData.InstrManufacture == 2)
                    {
                        ChannelPowerResult = Math.Round(double.Parse(session.Query(":SENSe:ACPower:OFFSet2:LLIMit ?").Replace('.', ',')), 2);
                    }
                }
            }
            #region Exception
            catch (VisaException v_exp)
            {
                App.Current.Dispatcher.Invoke((Action)(() =>
                {
                    MainWindow.exp.ExceptionData = new ExData() { ex = v_exp, ClassName = "ANALYZER", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
                }));
            }
            catch (Exception exp)
            {
                App.Current.Dispatcher.Invoke((Action)(() =>
                {
                    MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "ANALYZER", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
                }));
            }
            #endregion
            finally { an_dm -= GetChannelPowerResult; }
        }
        private void GetChannelPowerPreset()
        {
            try
            {
                if (UniqueData.ChnPow == true)
                {
                    if (UniqueData.InstrManufacture == 1)
                    {
                        if (UniqueData.HiSpeed == true)
                        {
                            //String temp = session.Query("CALC:MARK:FUNC:POW:PRES?").TrimEnd();
                            //((SplashWindow)App.Current.MainWindow).m_mainWindow.Message = temp;
                            //if (temp.Contains("CPOW")) { ChannelPowerBW = decimal.Parse(session.Query(":SENS:POW:ACH:BAND?").Replace('.', ',')); ChannelPowerState = true; }
                            //else if (temp.Contains("0")) { ChannelPowerState = false; }
                        }
                        else
                        {
                            //String temp = session.Query("CALC:MARK:FUNC:POW:PRES?").TrimEnd();
                            //((SplashWindow)App.Current.MainWindow).m_mainWindow.Message = temp;
                            //if (temp.Contains("1")) { ChannelPowerBW = decimal.Parse(session.Query("CALC:MARK:FUNC:CPOW:BAND?").Replace('.', ',')); ChannelPowerState = true; }
                            //else if (temp.Contains("0")) { ChannelPowerState = false; }
                        }
                    }
                }
            }
            #region Exception
            catch (VisaException v_exp)
            {
                App.Current.Dispatcher.Invoke((Action)(() =>
                {
                    MainWindow.exp.ExceptionData = new ExData() { ex = v_exp, ClassName = "ANALYZER", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
                }));
            }
            catch (Exception exp)
            {
                App.Current.Dispatcher.Invoke((Action)(() =>
                {
                    MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "ANALYZER", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
                }));
            }
            #endregion
            finally { an_dm -= GetChannelPowerPreset; }
        }
        #endregion ChannelPower
        #region Transducer
        private void GetTransducer()
        {
            try
            {
                if (UniqueData.InstrManufacture == 1)
                {
                    if (UniqueData.HiSpeed == true)
                    {
                        string[] t = session.Query(@"MMEM:CAT? 'C:\R_S\Instr\trd\*.tdf'").Replace("'", "").Replace(".TDF", "").Split(',');
                        List<Transducer> temp = new List<Transducer> { };
                        for (int i = 0; i < t.Length; i++)
                        {
                            temp.Add(new Transducer { Name = t[i], Active = false });
                        }
                        UniqueData.Transducers = temp; //GetSelectedTransducer();
                    }
                    else if (UniqueData.HiSpeed == false && (UniqueData.InstrModel == "FPH" || UniqueData.InstrModel == "ZVH"))
                    {
                        session.Write(@"MMEM:CDIR '\Public\Transducers'");
                        string s = session.Query(@"MMEM:CAT?");
                        //MessageBox.Show(s);
                        string[] t = s.Replace("'", "").Split(',');
                        List<Transducer> temp = new List<Transducer> { };
                        for (int i = 0; i < (int)(t.Length / 9); i++)
                        {
                            if (t[3 + 9 * i] == "pritrd" || t[3 + 9 * i] == "isotrd")
                            {
                                temp.Add(new Transducer { Name = t[2 + 9 * i], FileType = t[3 + 9 * i], Active = false });
                            }
                        }
                        UniqueData.Transducers = temp;
                    }
                }
            }
            #region Exception
            catch (VisaException v_exp)
            {
                App.Current.Dispatcher.Invoke((Action)(() =>
                {
                    MainWindow.exp.ExceptionData = new ExData() { ex = v_exp, ClassName = "ANALYZER", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
                }));
            }
            catch (Exception exp)
            {
                App.Current.Dispatcher.Invoke((Action)(() =>
                {
                    MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "ANALYZER", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
                }));
            }
            #endregion
            finally { an_dm -= GetTransducer; an_dm += GetSelectedTransducer; }
        }
        private void GetSelectedTransducer()
        {
            try
            {
                if (UniqueData.InstrManufacture == 1)
                {
                    if (UniqueData.HiSpeed == true)
                    {
                        string s = session.Query("CORR:TRAN:SEL?").TrimEnd();
                        bool any = false;
                        foreach (Transducer tr in UniqueData.Transducers)
                        {
                            if (s.ToLower().Contains(tr.Name.ToLower()))
                            { TransducerSelected = tr; tr.Active = true; any = true; }
                            else { tr.Active = false; }
                        }
                        AnyTransducerSet = any;
                        //string temp;
                        //foreach (Transducer tr in UniqueData.Transducers)
                        //{
                        //    session.Write("CORR:TRAN:SEL '" + tr.Name + "'");
                        //    temp = session.Query("CORR:TRAN:STAT?").TrimEnd();
                        //        tr.Active = temp == "0" ? false : true;
                        //    //if (temp.Contains(tr.Name)) { tr.Active = true; }
                        //    //else { tr.Active = false; }
                        //}
                    }
                    else
                    {
                        string s = session.Query("CORR:TRAN1:SEL?").TrimEnd();
                        bool any = false;
                        foreach (Transducer tr in UniqueData.Transducers)
                        {
                            if (s.ToLower().Contains(tr.Name.ToLower()))
                            { TransducerSelected = tr; tr.Active = true; any = true; }
                            else { tr.Active = false; }
                        }
                        AnyTransducerSet = any;
                    }
                }
            }
            #region Exception
            catch (VisaException v_exp)
            {
                App.Current.Dispatcher.Invoke((Action)(() =>
                {
                    MainWindow.exp.ExceptionData = new ExData() { ex = v_exp, ClassName = "ANALYZER", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
                }));
            }
            catch (Exception exp)
            {
                App.Current.Dispatcher.Invoke((Action)(() =>
                {
                    MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "ANALYZER", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
                }));
            }
            #endregion
            finally { an_dm -= GetSelectedTransducer; }
        }
        private void SetSelectedTransducer()
        {
            try
            {
                if (UniqueData.InstrManufacture == 1)
                {

                    if (UniqueData.HiSpeed == true)
                    {
                        if (TransducerSelected.Active)
                        {
                            session.Write("CORR:TRAN OFF");
                            session.Write("CORR:TRAN:SEL '" + TransducerSelected.Name + "'");
                            session.Write("CORR:TRAN ON");
                        }
                        else { session.Write("CORR:TRAN:SEL '" + TransducerSelected.Name + "'"); session.Write("CORR:TRAN OFF"); }
                        AnyTransducerSet = TransducerSelected.Active;
                        foreach (Transducer tr in UniqueData.Transducers)
                        {
                            if (tr.Active == true && TransducerSelected.Name != tr.Name) { tr.Active = false; }
                        }
                    }
                    else
                    {
                        if (TransducerSelected.Active)
                        {
                            session.Write("CORR:TRAN1 OFF");
                            session.Write("CORR:TRAN1:SEL '" + TransducerSelected.Name + "." + TransducerSelected.FileType + "'");
                            session.Write("CORR:TRAN1 ON");
                        }
                        else { session.Write("CORR:TRAN1 OFF"); }
                        AnyTransducerSet = TransducerSelected.Active;
                        foreach (Transducer tr in UniqueData.Transducers)
                        {
                            if (tr.Active == true && TransducerSelected.Name != tr.Name)
                            {
                                tr.Active = false;
                            }
                        }
                    }

                }
            }
            #region Exception
            catch (VisaException v_exp)
            {
                App.Current.Dispatcher.Invoke((Action)(() =>
                {
                    MainWindow.exp.ExceptionData = new ExData() { ex = v_exp, ClassName = "ANALYZER", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
                }));
            }
            catch (Exception exp)
            {
                App.Current.Dispatcher.Invoke((Action)(() =>
                {
                    MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "ANALYZER", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
                }));
            }
            #endregion
            finally { an_dm -= SetSelectedTransducer; an_dm += GetRefLevel; an_dm += GetLevelUnit; }

        }
        #endregion Transducer


        long GSMBandMeasTicks = 0;
        bool GSMBandMeas = false;
        GSMBandMeas GSMBandMeasSelected = new Equipment.GSMBandMeas() { };
        private void SetMeasMon()
        {
            #region
            if (IsRuning == true && MainWindow.db_v2.MeasMon.Data.Count > 0)//&& MainWindow.gps.GPSIsValid)
            {
                try
                {
                    if (MeasMonItem != null && MeasMonItem.AllTraceCountToMeas == MeasMonItem.AllTraceCount)// && MainWindow.db_v2.MeasMon.Data.Count > 0) //if (MeasTraceCount < 0 && MainWindow.db.MonMeas.Count > 0)
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

                                    int index = 0;
                                    for (int p = 0; p < UniqueData.SweepPointArr.Length; p++)
                                    {
                                        if (index == 0 && UniqueData.SweepPointArr[p] >= GSMBandMeasSelected.TracePoints)
                                        { index = p; }

                                    }
                                    //SetFreqSettingsToMeasMon(MeasMonItem.FreqDN, MeasMonItem.MeasSpan, 18);//23
                                    SetSettingsToMeasMon((GSMBandMeasSelected.Stop + GSMBandMeasSelected.Start) / 2, GSMBandMeasSelected.Stop - GSMBandMeasSelected.Start, index);
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
                                            if (IdentificationData.UMTS.BTS[j].SC == MainWindow.db_v2.MeasMon.Data[i].TechSubInd && IdentificationData.UMTS.BTS[j].GCID == MainWindow.db_v2.MeasMon.Data[i].GCID)
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
                                            if (IdentificationData.LTE.BTS[j].PCI == MainWindow.db_v2.MeasMon.Data[i].TechSubInd && IdentificationData.LTE.BTS[j].GCID == MainWindow.db_v2.MeasMon.Data[i].GCID)
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
                                        //ObservableCollection<GSMBTSData> gsm = IdentificationData.GSM.BTS;
                                        //int gsmind = -1;
                                        //if (gsm != null)
                                        //    for (int i = 0; i < gsm.Count; i++)
                                        //    {
                                        //        if (gsm[i].GCID == temp.GCID && gsm[i].FreqDn == temp.FreqDN) { gsmind = i; }
                                        //    }
                                        //if (gsmind > -1 && gsm[gsmind].Power > TSMxReceiver.DetectionLevelGSM)
                                        //{
                                        MeasMonItem = temp;
                                        MeasMonItem.AllTraceCountToMeas = ((MeasMonItem.AllTraceCount + MeasTraceCountOnFreq) / 10) * 10;
                                        //MeasTraceCount = MeasMonItem.AllTraceCount + MeasTraceCountOnFreq;
                                        SetSettingsToMeasMon(MeasMonItem.FreqDN, MeasMonItem.SpecData.FreqSpan, 17);
                                        MeasMonTimeMeas = DateTime.Now.Ticks;
                                        //MeasTraceCountOnFreq = 10;
                                        //}
                                    }
                                    #endregion
                                }
                                else if (temp.Techonology == "UMTS")
                                {
                                    #region
                                    if (IdentificationData.UMTS.BTS != null && IdentificationData.UMTS.BTS.Count > 0)
                                    {
                                        //ObservableCollection<UMTSBTSData> umts = IdentificationData.UMTS.BTS;
                                        //int umtsind = -1;
                                        //if (umts != null)
                                        //    for (int i = 0; i < umts.Count; i++)
                                        //    {
                                        //        if (umts[i].GCID == temp.GCID && umts[i].FreqDn == temp.FreqDN) { umtsind = i; }
                                        //    }
                                        //if (umtsind > -1 && umts[umtsind].RSCP > TSMxReceiver.DetectionLevelUMTS)
                                        //{
                                        MeasMonItem = temp;
                                        MeasMonItem.AllTraceCountToMeas = ((MeasMonItem.AllTraceCount + MeasTraceCountOnFreq) / 10) * 10;
                                        //MeasTraceCount = MeasMonItem.AllTraceCount + MeasTraceCountOnFreq;
                                        SetSettingsToMeasMon(MeasMonItem.FreqDN, MeasMonItem.SpecData.FreqSpan, 17);
                                        MeasMonTimeMeas = DateTime.Now.Ticks;
                                        //MeasTraceCountOnFreq = 10;
                                        //}
                                    }
                                    #endregion
                                }
                                else if (temp.Techonology == "LTE")
                                {
                                    #region
                                    if (IdentificationData.LTE.BTS != null && IdentificationData.LTE.BTS.Count > 0)
                                    {
                                        ////ObservableCollection<LTEBTSData> umts = IdentificationData.LTE.BTS;
                                        //int lteind = -1;
                                        //if (IdentificationData.LTE.BTS != null)
                                        //    for (int i = 0; i < IdentificationData.LTE.BTS.Count; i++)
                                        //    {
                                        //        if (IdentificationData.LTE.BTS[i].GCID == temp.GCID && IdentificationData.LTE.BTS[i].FreqDn == temp.FreqDN) { lteind = i; }
                                        //    }
                                        //if (lteind > -1 && IdentificationData.LTE.BTS[lteind].RSRP > TSMxReceiver.DetectionLevelLTE)
                                        //{
                                        MeasMonItem = temp;
                                        MeasMonItem.AllTraceCountToMeas = ((MeasMonItem.AllTraceCount + MeasTraceCountOnFreq) / 10) * 10;
                                        //MeasTraceCount = MeasMonItem.AllTraceCount + MeasTraceCountOnFreq;
                                        SetSettingsToMeasMon(MeasMonItem.FreqDN, MeasMonItem.SpecData.FreqSpan, 17);
                                        MeasMonTimeMeas = DateTime.Now.Ticks;
                                        //MeasTraceCountOnFreq = 10;
                                        //}
                                    }
                                    #endregion
                                }
                                else if (temp.Techonology == "CDMA")
                                {
                                    #region
                                    if (IdentificationData.CDMA.BTS != null && IdentificationData.CDMA.BTS.Count > 0)
                                    {
                                        //ObservableCollection<LTEBTSData> umts = IdentificationData.LTE.BTS;
                                        //int cdmaind = -1;
                                        //if (IdentificationData.CDMA.BTS != null)
                                        //    for (int i = 0; i < IdentificationData.CDMA.BTS.Count; i++)
                                        //    {
                                        //        if (IdentificationData.CDMA.BTS[i].GCID == temp.GCID && IdentificationData.CDMA.BTS[i].FreqDn == temp.FreqDN) { cdmaind = i; }
                                        //    }
                                        //if (cdmaind > -1 && IdentificationData.CDMA.BTS[cdmaind].RSCP > TSMxReceiver.DetectionLevelCDMA)
                                        //{
                                        MeasMonItem = temp;
                                        MeasMonItem.AllTraceCountToMeas = ((MeasMonItem.AllTraceCount + MeasTraceCountOnFreq) / 10) * 10;
                                        //MeasTraceCount = MeasMonItem.AllTraceCount + MeasTraceCountOnFreq;
                                        SetSettingsToMeasMon(MeasMonItem.FreqDN, MeasMonItem.SpecData.FreqSpan, 17);
                                        MeasMonTimeMeas = DateTime.Now.Ticks;
                                        //MeasTraceCountOnFreq = 10;
                                        //}
                                    }
                                    #endregion
                                }
                                else if (temp.Techonology == "WIMAX")
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
                                        MeasMonItem.AllTraceCountToMeas = ((MeasMonItem.AllTraceCount + MeasTraceCountOnFreq) / 10) * 10;
                                        //MeasTraceCount = MeasMonItem.AllTraceCount + MeasTraceCountOnFreq;
                                        SetSettingsToMeasMon(MeasMonItem.FreqDN, MeasMonItem.SpecData.FreqSpan, 17);
                                        //MeasTraceCountOnFreq = 10;
                                        //}
                                    }
                                    #endregion
                                }
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
                                    //        //MeasTraceCountOnFreq = 10;
                                    //        //MeasTraceCount = MeasMonItem.AllTraceCount + MeasTraceCountOnFreq;
                                    //        SetFreqSettingsToMeasMon(MeasMonItem.FreqDN, MeasMonItem.SpecData.FreqSpan);
                                    //        MeasMonTimeMeas = DateTime.Now.Ticks;

                                    //    }
                                    //}
                                    #endregion
                                }
                                MeasMonItem.device_meas = device_meas;
                                //Thread.Sleep(10);
                            }
                            #endregion
                        }
                        Thread.Sleep(20);
                    }
                }
                #region Exception
                catch (VisaException v_exp)
                {
                    App.Current.Dispatcher.Invoke((Action)(() =>
                    {
                        MainWindow.exp.ExceptionData = new ExData() { ex = v_exp, ClassName = "ANALYZER", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
                    }));
                }
                catch (Exception exp)
                {
                    App.Current.Dispatcher.Invoke((Action)(() =>
                    {
                        MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "ANALYZER", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
                    }));
                }
                #endregion
            }

            #endregion
            //TelnetDM -= SetMeasMon;
        }
        private void SetSomeMeas()
        {
            if (Run && IsRuning == true)
            {
                if (SomeMeasItem.ThisStayOnFrequency >= SomeMeasItem.StayOnFrequency)
                {
                    SomeMeasItem.IsMeas = false;
                    SomeMeasItem.ThisStayOnFrequency = 0;
                    SomeMeasIndex++;
                    if (SomeMeasIndex >= MainWindow.amd.DataSomeMeas.Count()) SomeMeasIndex = 0;
                    DataSomeMeas amd = MainWindow.amd.DataSomeMeas[SomeMeasIndex];
                    if (amd.State == true && amd.DeviceType == 3)
                    {
                        //SomeMeasIndex = i;
                        SomeMeasItem = amd;
                        SomeMeasItem.IsMeas = true;
                        SomeMeasTimeMeas = DateTime.Now.Ticks;
                        FreqCentr = SomeMeasItem.FreqCentr;
                        FreqSpan = SomeMeasItem.FreqSpan;
                        RBWIndex = SomeMeasItem.RBWIndex;
                        VBWIndex = SomeMeasItem.VBWIndex;
                        SweepTime = SomeMeasItem.SweepTime;
                    }
                }
            }
        }

        private void SetTraceDataSomeMeas(DataSomeMeas data, int newLength, float[] mintrace, float[] maxtrace, decimal freqStart, decimal step)
        {
            tracepoint[] temp1 = (tracepoint[])data.Trace.Clone();
            if (maxtrace.Length > 0 && (temp1.Length == 0 || maxtrace[0] != (float)temp1[0].level))
            {
                if (newLength > 0 && step > 0 && temp1.Length != newLength || (Math.Abs(data.FreqStart - (decimal)freqStart) >= (decimal)step))
                {
                    temp1 = new tracepoint[newLength];
                    for (int i = 0; i < newLength; i++)
                    {
                        decimal freq = freqStart + step * i;
                        temp1[i] = new tracepoint() { freq = freq, level = maxtrace[i] };
                    }
                    //MarkersTraceLegthOrFreqsChanged(temp1);
                }
                #region Trace
                if (data.TraceTypeIndex == 0)
                {
                    for (int i = 0; i < newLength; i++)
                    {
                        temp1[i].level = maxtrace[i];
                    }
                }
                else if (data.TraceTypeIndex == 1)
                {
                    tracepoint[] temp = new tracepoint[newLength];
                    for (int i = 0; i < newLength; i++)
                    {
                        decimal freq = freqStart + step * i;
                        temp[i] = new tracepoint() { freq = freq, level = maxtrace[i] };
                    }
                    //if (Trace1Reset) { data.TraceAveragedList.Reset(); Trace1Reset = false; }
                    data.TraceAveragedList.AddTraceToAverade(temp);
                    temp1 = data.TraceAveragedList.AveragedTrace;
                }
                else if (data.TraceTypeIndex == 2)
                {
                    tracepoint[] temp = new tracepoint[newLength];
                    for (int i = 0; i < newLength; i++)
                    {
                        decimal freq = freqStart + step * i;
                        temp[i] = new tracepoint() { freq = freq, level = maxtrace[i] };
                    }
                    //if (Trace1Reset) { data.TraceTrackedList.Reset(); Trace1Reset = false; }
                    data.TraceTrackedList.AddTraceToTracking(temp);
                    temp1 = data.TraceTrackedList.TrackingTrace;
                }
                else if (data.TraceTypeIndex == 3)
                {
                    //if (Trace1Reset == false)
                    //{
                    //    for (int i = 0; i < newLength; i++)
                    //    {
                    //        if ((decimal)maxtrace[i] > temp1[i].Level) temp1[i].Level = (decimal)maxtrace[i];
                    //    }
                    //}
                    //else
                    //{
                    //    for (int i = 0; i < newLength; i++)
                    //    {
                    //        temp1[i].Level = (decimal)maxtrace[i];
                    //    }
                    //    Trace1Reset = false;
                    //}
                }
                else if (data.TraceTypeIndex == 4)
                {
                    //    if (Trace1Reset == false)
                    //    {
                    //        for (int i = 0; i < newLength; i++)
                    //        {
                    //            if ((decimal)maxtrace[i] < temp1[i].Level) temp1[i].Level = (decimal)maxtrace[i];
                    //        }
                    //    }
                    //    else
                    //    {
                    //        for (int i = 0; i < newLength; i++)
                    //        {
                    //            temp1[i].Level = (decimal)maxtrace[i];
                    //        }
                    //        Trace1Reset = false;
                    //    }
                }
                else if (data.TraceTypeIndex == 5)
                {
                    //пропускаем 
                }
                else if (data.TraceTypeIndex == 6)
                {
                    //пропускаем и не рисуем
                }
                #endregion Trace 1
                //NewTrace = true;
                //if (ChannelPowerState) ChannelPowerResult = LM.MeasChannelPower(Trace, FreqCentr, ChannelPowerBW, LevelUnits[LevelUnitIndex].ind);
                data.Trace = temp1;
            }
        }
        /// <summary>
        /// астановка AutoSweepTime Att TraceType Detector
        /// </summary>
        private void SetMeasMonAnSettings()
        {
            if (IsRuning == true)
            {
                try
                {
                    #region set
                    if (UniqueData.InstrManufacture == 1)
                    {
                        AutoSweepTime = true; session.Write(":SWE:TIME:AUTO 1");
                        AttLevel = 0; session.Write(":INP:ATT " + AttLevel.ToString().Replace(',', '.'));
                        Trace1Type = UniqueData.TraceType[0]; session.Write(":DISP:TRAC1:MODE " + Trace1Type.Parameter);
                        Trace2Type = UniqueData.TraceType[5]; session.Write(":DISP:TRAC2:MODE " + Trace2Type.Parameter);
                        Trace3Type = UniqueData.TraceType[5]; session.Write(":DISP:TRAC3:MODE " + Trace3Type.Parameter);
                        foreach (TrDetector td in UniqueData.TraceDetector)
                        {
                            if (td.UI.ToLower().Contains("max peak"))
                            {
                                Trace1Detector = td;
                                Trace2Detector = td;
                                Trace3Detector = td;
                            }
                        }
                        if (UniqueData.HiSpeed)
                        {
                            session.Write(":SENSe:DETector1 " + Trace1Detector.Parameter);
                            session.Write(":SENSe:DETector2 " + Trace2Detector.Parameter);
                            session.Write(":SENSe:DETector3 " + Trace3Detector.Parameter);
                        }
                        else { session.Write(":SENSe:DETector1 " + Trace1Detector.Parameter); }

                    }
                    else if (UniqueData.InstrManufacture == 2)
                    {
                        AutoSweepTime = true; session.Write(":SENS:SWE:ACQ:AUTO 1");
                        AttLevel = 0; session.Write(":POW:ATT " + AttLevel.ToString().Replace(',', '.'));
                        Trace1Type = UniqueData.TraceType[0]; session.Write(":TRAC1:TYPE " + Trace1Type.Parameter);
                        Trace2Type = UniqueData.TraceType[5]; session.Write(":TRAC2:TYPE " + Trace2Type.Parameter);
                        Trace3Type = UniqueData.TraceType[5]; session.Write(":TRAC3:TYPE " + Trace3Type.Parameter);
                        foreach (TrDetector td in UniqueData.TraceDetector)
                        {
                            if (td.UI.ToLower().Contains("max peak"))
                            {
                                Trace1Detector = td;
                                Trace2Detector = td;
                                Trace3Detector = td;
                            }
                        }
                        session.Write(":SENSe:DETector:FUNCtion " + Trace1Detector.Parameter);
                    }
                    else if (UniqueData.InstrManufacture == 3)
                    {
                        AutoSweepTime = true; session.Write(":SENS:SWE:AUTO ON");

                        Trace1Type = UniqueData.TraceType[0]; session.Write(":TRACe1:OPERation " + Trace1Type.Parameter);
                        Trace2Type = UniqueData.TraceType[4]; session.Write(":TRACe2:OPERation " + Trace2Type.Parameter);
                        Trace3Type = UniqueData.TraceType[4]; session.Write(":TRACe3:OPERation " + Trace3Type.Parameter);
                        foreach (TrDetector td in UniqueData.TraceDetector)
                        {
                            if (td.UI.ToLower().Contains("max peak"))
                            {
                                Trace1Detector = td;
                                Trace2Detector = td;
                                Trace3Detector = td;
                            }
                        }
                        session.Write(":SENSe:DETector:FUNCtion " + Trace1Detector.Parameter);
                    }
                    #endregion
                }
                #region Exception
                catch (VisaException v_exp)
                {
                    App.Current.Dispatcher.Invoke((Action)(() =>
                    {
                        MainWindow.exp.ExceptionData = new ExData() { ex = v_exp, ClassName = "ANALYZER", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
                    }));
                }
                catch (Exception exp)
                {
                    App.Current.Dispatcher.Invoke((Action)(() =>
                    {
                        MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "ANALYZER", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
                    }));
                }
                #endregion
            }
            an_dm -= SetMeasMonAnSettings;
        }
        /// <summary>
        /// установка частоты, спана и количевства точек
        /// </summary>
        /// <param name="freqCentr"></param>
        /// <param name="span"></param>
        private void SetSettingsToMeasMon(decimal freqCentr, decimal span, int tracepointsindex)
        {
            if (IsRuning == true)
            {
                try
                {
                    #region set
                    if (UniqueData.InstrManufacture == 1)
                    {
                        if (UniqueData.HiSpeed)
                        {
                            SweepPointsIndex = tracepointsindex;
                            session.Write(":SWE:POIN " + SweepPoints.ToString());
                            session.DefaultBufferSize = SweepPoints * 18 + 25;
                        }
                        FreqCentr = freqCentr;
                        FreqSpan = span;
                        session.Write(":SENSe:FREQuency:CENTer " + freqCentr.ToString().Replace(',', '.'));
                        session.Write(":SENSe:FREQuency:SPAN " + span.ToString().Replace(',', '.'));
                        FreqCentr = decimal.Parse(session.Query(":SENSe:FREQuency:CENTer?").Replace('.', ','));
                        FreqSpan = decimal.Parse(session.Query(":SENSe:FREQuency:SPAN?").Replace('.', ','));
                    }
                    else if (UniqueData.InstrManufacture == 2)
                    {
                        session.Write(":FREQ:CENT " + freqCentr.ToString().Replace(',', '.') + ";FREQuency:SPAN " + span.ToString().Replace(',', '.'));
                        //FreqSpan = Double.Parse(session.Query(":FREQuency:SPAN?").Replace('.', ','));
                    }
                    else if (UniqueData.InstrManufacture == 3)
                    {
                        session.Write(":SENSe:FREQuency:CENTer " + freqCentr.ToString().Replace(',', '.') + ";:SENSe:FREQuency:SPAN " + span.ToString().Replace(',', '.'));
                        //session.Write(":SENSe:FREQuency:CENTer " + FreqCentr.ToString().Replace(',', '.'));
                        //FreqSpan = Double.Parse(session.Query(":SENSe:FREQuency:SPAN?").Replace('.', ','));
                    }
                    GetFreqArr();
                    if (AutoRBW == true) { GetRBW(); }
                    if (AutoVBW == true) { GetVBW(); }
                    if (AutoSweepTime == true) { GetSweepTime(); }
                    #endregion

                }
                #region Exception
                catch (VisaException v_exp)
                {
                    App.Current.Dispatcher.Invoke((Action)(() =>
                    {
                        MainWindow.exp.ExceptionData = new ExData() { ex = v_exp, ClassName = "ANALYZER", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
                    }));
                }
                catch (Exception exp)
                {
                    App.Current.Dispatcher.Invoke((Action)(() =>
                    {
                        MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "ANALYZER", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
                    }));
                }
                #endregion
            }
            //TelnetDM -= SetMeasMon;
        }
        #endregion Meas
        #endregion private Methods


        public event PropertyChangedEventHandler PropertyChanged; // Событие, которое нужно вызывать при изменении

        // Для удобства обернем событие в метод с единственным параметром - имя изменяемого свойства
        public void OnPropertyChanged(string propertyName)
        {
            // Если кто-то на него подписан, то вызывем его
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        public partial class AllAnUnqData : INotifyPropertyChanged
        {
            /// <summary>
            /// 1 = Rohde&Schwarz
            /// 2 = Keysight Technologies
            /// 3 = Anritsu
            /// </summary>
            public int InstrManufacture { get; set; }
            public ParamWithUI InstrManufacrureData
            {
                get
                {
                    ParamWithUI im = new InstrManufacrures().Unk;
                    if (InstrManufacture == 1) im = new InstrManufacrures().RuS;
                    else if (InstrManufacture == 2) im = new InstrManufacrures().Keysight;
                    else if (InstrManufacture == 3) im = new InstrManufacrures().Anritsu;
                    return im;
                }
                set { }
            }
            public string InstrModel { get; set; }
            public string SerialNumber { get; set; }
            public bool HiSpeed { get; set; }

            public List<AnOption> InstrOption { get; set; }
            public List<AnOption> DefaultInstrOption { get; set; }
            public List<AnOption> LoadedInstrOption { get; set; }
            private int _NumberOfTrace = 0;
            public int NumberOfTrace
            {
                get { return _NumberOfTrace; }
                set { _NumberOfTrace = value; OnPropertyChanged("NumberOfTrace"); }
            }
            public ObservableCollection<ParamWithUI> TraceType { get; set; }
            public List<TrDetector> TraceDetector { get; set; }
            private bool _ChangeableSweepType = false;
            public bool ChangeableSweepType
            {
                get { return _ChangeableSweepType; }
                set { _ChangeableSweepType = value; OnPropertyChanged("ChangeableSweepType"); }
            }
            public List<SwpType> SweepType { get; set; }
            private bool _SweepPointFix = false;
            public bool SweepPointFix
            {
                get { return _SweepPointFix; }
                set { _SweepPointFix = value; OnPropertyChanged("SweepPointFix"); }
            }
            public int[] SweepPointArr { get; set; }
            public int DefaultSweepPoint { get; set; }
            public decimal[] RBWArr { get; set; }
            public decimal[] VBWArr { get; set; }
            private bool _CouplingRatio = false;
            public bool CouplingRatio
            {
                get { return _CouplingRatio; }
                set { _CouplingRatio = value; OnPropertyChanged("CouplingRatio"); }
            }

            private bool _PreAmp = false;
            public bool PreAmp
            {
                get { return _PreAmp; }
                set { _PreAmp = value; OnPropertyChanged("PreAmp"); }
            }
            public decimal AttMax { get; set; }
            public decimal AttStep { get; set; }
            public decimal[] RangeArr { get; set; }

            public bool Battery { get; set; }
            public bool NdB { get; set; }
            public bool OBW { get; set; }
            public bool ChnPow { get; set; }
            public bool RangeFixed { get; set; }

            private List<Transducer> _Transducers;
            public List<Transducer> Transducers
            {
                get { return _Transducers; }
                set { _Transducers = value; OnPropertyChanged("Transducers"); }
            }
            public event PropertyChangedEventHandler PropertyChanged; // Событие, которое нужно вызывать при изменении

            // Для удобства обернем событие в метод с единственным параметром - имя изменяемого свойства
            public void OnPropertyChanged(string propertyName)
            {
                // Если кто-то на него подписан, то вызывем его
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        public partial class AnOption
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
        public partial class TrType
        {
            public string UI { get; set; }
            public string Parameter { get; set; }
        }
        public partial class TrDetector
        {
            public string UI { get; set; }
            public string Parameter { get; set; }
        }
        public partial class SwpType
        {
            public string UI { get; set; }
            public string Parameter { get; set; }
        }
        public partial class Transducer
        {
            public string Name { get; set; }
            public string FileType { get; set; }
            public bool Active { get; set; }
        }
    }
}
