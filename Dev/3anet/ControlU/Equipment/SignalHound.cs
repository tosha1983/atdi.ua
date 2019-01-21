using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Windows.Data;

namespace ControlU.Equipment
{
    public class SignalHound : INotifyPropertyChanged
    {
        //bbStatus status = bbStatus.bbNoError;
        public bbStatus status
        {
            get { return _status; }
            set
            {
                _status = value;

                OnPropertyChanged("status");
            }
        }
        private bbStatus _status = bbStatus.bbNoError;

        //SignalHoundBBApi bb_api;



        LocalMeasurement LM = new LocalMeasurement();
        string AppPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        public delegate void DoubMod();
        public DoubMod SH_dm;
        public Thread SHThread;
        public System.Timers.Timer SHtmr = new System.Timers.Timer(100);
        public string ScreenName
        {
            get { return _ScreenName; }
            set { _ScreenName = value; OnPropertyChanged("ScreenName"); }
        }
        string _ScreenName = "";

        #region DeviceMode
        public int DeviceModeIndex
        {
            get { return _DeviceModeIndex; }
            set
            {
                if (value > 1) _DeviceModeIndex = 1;
                else if (value < 0) _DeviceModeIndex = 0;
                else _DeviceModeIndex = value;
                BBDeviceMode = (BB_Mode)(uint.Parse(DeviceModes[_DeviceModeIndex].Parameter));
                DeviceMode = DetectorTypes[_DeviceModeIndex];
                OnPropertyChanged("DeviceModeIndex");
            }
        }
        private int _DeviceModeIndex = 0;
        public BB_Mode BBDeviceMode = BB_Mode.BB_SWEEPING;
        public ParamWithUI DeviceMode
        {
            get { return _DeviceMode; }
            set { _DeviceMode = value; OnPropertyChanged("Detector"); }
        }
        private ParamWithUI _DeviceMode = new ParamWithUI() { Parameter = "0", UI = "Sweep" };
        public ObservableCollection<ParamWithUI> DeviceModes
        {
            get { return _DeviceModes; }
            set { _DeviceModes = value; OnPropertyChanged("DetectorTypes"); }
        }
        private ObservableCollection<ParamWithUI> _DeviceModes = new ObservableCollection<ParamWithUI>
        {
            new ParamWithUI() { Parameter = "0", UI = "Sweep" },
            new ParamWithUI() { Parameter = "1", UI = "Real Time" },
        };
        #endregion DeviceMode

        #region Trace Data

        public tracepoint[] Trace1
        {
            get { return _Trace1; }
            set { _Trace1 = value; OnPropertyChanged("Trace1"); }
        }
        private tracepoint[] _Trace1;

        public bool DrawTrace1Min = false;
        public tracepoint[] Trace1Min
        {
            get { return _Trace1Min; }
            set { _Trace1Min = value; OnPropertyChanged("Trace1Min"); }
        }
        private tracepoint[] _Trace1Min;

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

        private float[] _RealTimeFrame;
        public float[] RealTimeFrame
        {
            get { return _RealTimeFrame; }
            set { _RealTimeFrame = value; }
        }
        private int _RealTimeFrameWidth = 1;
        public int RealTimeFrameWidth
        {
            get { return _RealTimeFrameWidth; }
            set { _RealTimeFrameWidth = value; }
        }
        private int _RealTimeFrameHeight = 1;
        public int RealTimeFrameHeight
        {
            get { return _RealTimeFrameHeight; }
            set { _RealTimeFrameHeight = value; }
        }
        //public int RealTimeFrameWidth = 0, RealTimeFrameHeight = 0;
        decimal TraceFreqStart = 0;
        decimal TraceFreqStop = 0;


        #endregion

        #region Freqs
        private bool MeasMonNewFreqSet = false;
        public bool FreqSet
        {
            get { return _FreqSet; }
            set { _FreqSet = value; }
        }
        private bool _FreqSet = false;

        public bool NewTrace
        {
            get { return _NewTrace; }
            set { _NewTrace = value; }
        }
        private bool _NewTrace = false;

        public decimal FreqStep
        {
            get { return _FreqStep; }
            set { _FreqStep = value; }
        }
        private decimal _FreqStep = 10000;

        public int TracePoints
        {
            get { return _TracePoints; }
            set { _TracePoints = value; }
        }
        private int _TracePoints = 1601;

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
        private decimal _FreqCentr = 950000000;//2142400000;

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
        private decimal _FreqStart = 947500000;//2139900000;//1800000000;//2600000000;//2490000000;//

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
        private decimal _FreqStop = 952500000;//2144900000;//1900000000;//2700000000;//2510000000;//
        #endregion Freqs

        #region RBW / VBW
        public decimal[] RBWArr = new decimal[] { 1, 3, 30, 100, 300, 1000, 3000, 10000, 30000, 100000, 300000, 1000000, 3000000, 10000000 };

        public decimal RBW
        {
            get { return _RBW; }
            set { _RBW = value; OnPropertyChanged("RBW"); }
        }
        private decimal _RBW = 10000;
        public int RBWIndex
        {
            get { return _RBWIndex; }
            set
            {
                if (value > -1 && value < RBWArr.Length) { _RBWIndex = value; RBW = RBWArr[_RBWIndex]; OnPropertyChanged("RBWIndex"); }
                else if (value < 0) { _RBWIndex = 0; RBW = RBWArr[_RBWIndex]; OnPropertyChanged("RBWIndex"); }
                else if (value >= RBWArr.Length) { _RBWIndex = RBWArr.Length - 1; RBW = RBWArr[_RBWIndex]; OnPropertyChanged("RBWIndex"); }
                if (AutoVBW) VBWIndex = _RBWIndex;
                if (AutoVBW == false && _VBWIndex > _RBWIndex) VBWIndex = _RBWIndex;
            }
        }
        private int _RBWIndex = 7;

        public bool AutoVBW
        {
            get { return _AutoVBW; }
            set { _AutoVBW = value; OnPropertyChanged("AutoVBW"); }
        }
        private bool _AutoVBW = true;
        public decimal[] VBWArr = new decimal[] { 1, 3, 30, 100, 300, 1000, 3000, 10000, 30000, 100000, 300000, 1000000, 3000000, 10000000 };
        public decimal VBW
        {
            get { return _VBW; }
            set { _VBW = value; OnPropertyChanged("VBW"); }
        }
        private decimal _VBW = 10000;
        public int VBWIndex
        {
            get { return _VBWIndex; }
            set
            {
                if (value > -1 && value < VBWArr.Length) { _VBWIndex = value; VBW = VBWArr[_VBWIndex]; OnPropertyChanged("VBWIndex"); }
                else if (value < 0) { _VBWIndex = 0; VBW = VBWArr[_VBWIndex]; OnPropertyChanged("VBWIndex"); }
                else if (value >= VBWArr.Length) { _VBWIndex = VBWArr.Length - 1; VBW = RBWArr[_VBWIndex]; OnPropertyChanged("VBWIndex"); }
            }
        }
        private int _VBWIndex = 7;


        #endregion

        #region Sweep
        public decimal SweepTime
        {
            get { return _SweepTime; }
            set
            {
                if (value >= 0.00001m && value <= 1) _SweepTime = value;
                if (value <= 0.00001m) _SweepTime = 0.00001m;
                if (value >= 1) _SweepTime = 1;
                OnPropertyChanged("SweepTime");
            }
        }
        private decimal _SweepTime = 0.00001m;
        #endregion

        #region Levels

        public bool RFOverload
        {
            get { return _RFOverload; }
            set { _RFOverload = value; OnPropertyChanged("RFOverload"); }
        }
        private bool _RFOverload = false;

        public decimal RefLevel
        {
            get { return _RefLevel; }
            set
            {
                _RefLevel = value;
                _LowestLevel = _RefLevel - Range; OnPropertyChanged("LowestLevel");
                OnPropertyChanged("RefLevel");
            }
        }
        private decimal _RefLevel = -40;

        public decimal Range
        {
            get { return _Range; }
            set
            {
                _Range = value;
                _LowestLevel = _RefLevel - Range; OnPropertyChanged("LowestLevel");
                OnPropertyChanged("Range");
            }
        }
        private decimal _Range = 100;

        public decimal LowestLevel
        {
            get { return _LowestLevel; }
            set { _LowestLevel = value; OnPropertyChanged("LowestLevel"); }
        }
        private decimal _LowestLevel = -140;

        public string LevelUnit
        {
            get { return _LevelUnit; }
            set { _LevelUnit = value; OnPropertyChanged("LevelUnit"); }
        }
        private string _LevelUnit = "dBm";

        public int LevelUnitIndex
        {
            get { return _LevelUnitIndex; }
            set { _LevelUnitIndex = value; LevelUnit = LevelUnits[_LevelUnitIndex].UI; OnPropertyChanged("LevelUnitIndex"); }
        }
        private int _LevelUnitIndex = 0;

        public ObservableCollection<LevelUnit> LevelUnits
        {
            get { return _LevelUnits; }
            set { _LevelUnits = value; OnPropertyChanged("LevelUnits"); }
        }
        private ObservableCollection<LevelUnit> _LevelUnits = new ObservableCollection<LevelUnit>()
        {
            new AllLevelUnits().dBm
        };


        #region Gain
        public int GainIndex
        {
            get { return _GainIndex; }
            set
            {
                if (value < 0) _GainIndex = 0;
                else if (value > Gains.Count() - 1) _GainIndex = Gains.Count() - 1;
                else _GainIndex = value;
                GainSelected = Gains[_GainIndex];
            }
        }
        private int _GainIndex = 2;
        public ParamWithUI GainSelected
        {
            get { return _GainSelected; }
            set { _GainSelected = value; Gain = (BB_Gain)int.Parse(_GainSelected.Parameter); OnPropertyChanged("GainSelected"); }
        }
        private ParamWithUI _GainSelected = new ParamWithUI() { Parameter = "10", UI = "10 dB" };
        public BB_Gain Gain = BB_Gain.BB_Gain_AUTO;
        private ObservableCollection<ParamWithUI> Gains = new ObservableCollection<ParamWithUI>
        {
            new ParamWithUI() { Parameter = "-1", UI = "Auto" },
            new ParamWithUI() { Parameter = "0", UI = "0 dB" },
            new ParamWithUI() { Parameter = "1", UI = "10 dB" },
            new ParamWithUI() { Parameter = "2", UI = "20 dB" },
            new ParamWithUI() { Parameter = "3", UI = "30 dB" },
        };
        #endregion Gain

        #region ATT
        public int AttIndex
        {
            get { return _AttIndex; }
            set
            {
                if (value < 0) _AttIndex = 0;
                else if (value > Atts.Count() - 1) _AttIndex = Atts.Count() - 1;
                else _AttIndex = value;
                AttSelected = Atts[_AttIndex];
            }
        }
        private int _AttIndex = 2;
        public ParamWithUI AttSelected
        {
            get { return _AttSelected; }
            set { _AttSelected = value; Att = (BB_Atten)(int.Parse(_AttSelected.Parameter)); OnPropertyChanged("AttSelected"); }
        }
        private ParamWithUI _AttSelected = new ParamWithUI() { Parameter = "10", UI = "10 dB" };
        public BB_Atten Att = BB_Atten.BB_Atten_AUTO;
        private ObservableCollection<ParamWithUI> Atts = new ObservableCollection<ParamWithUI>
        {
            new ParamWithUI() { Parameter = "-1", UI = "Auto" },
            new ParamWithUI() { Parameter = "0", UI = "0 dB" },
            new ParamWithUI() { Parameter = "10", UI = "10 dB" },
            new ParamWithUI() { Parameter = "20", UI = "20 dB" },
            new ParamWithUI() { Parameter = "30", UI = "30 dB" },
        };
        #endregion ATT
        #endregion

        #region Trace
        public int DetectorTypeIndex
        {
            get { return _DetectorTypeIndex; }
            set
            {
                if (value > 3) _DetectorTypeIndex = 3;
                else if (value < 0) _DetectorTypeIndex = 0;
                else _DetectorTypeIndex = value;
                BBDetector = (BB_Detector)(uint.Parse(DetectorTypes[_DetectorTypeIndex].Parameter));
                Detector = DetectorTypes[_DetectorTypeIndex];
                DrawTrace1Min = DetectorTypes[_DetectorTypeIndex].UI.Contains("MIN and MAX") ? true : false;
                OnPropertyChanged("DetectorTypeIndex");
            }
        }
        private int _DetectorTypeIndex = 0;
        public BB_Detector BBDetector = BB_Detector.BB_MAX_ONLY;
        public ParamWithUI Detector
        {
            get { return _Detector; }
            set { _Detector = value; OnPropertyChanged("Detector"); }
        }
        private ParamWithUI _Detector = new ParamWithUI() { Parameter = "3", UI = "MAX" };
        public ObservableCollection<ParamWithUI> DetectorTypes
        {
            get { return _DetectorTypes; }
            set { _DetectorTypes = value; OnPropertyChanged("DetectorTypes"); }
        }
        private ObservableCollection<ParamWithUI> _DetectorTypes = new ObservableCollection<ParamWithUI>
        {
            new ParamWithUI() { Parameter = "3", UI = "MAX" },
            new ParamWithUI() { Parameter = "2", UI = "MIN" },
            new ParamWithUI() { Parameter = "1", UI = "Average" },
            new ParamWithUI() { Parameter = "0", UI = "MIN and MAX" },
        };


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

        public int VideoUnitTypeIndex
        {
            get { return _VideoUnitTypeIndex; }
            set
            {
                if (value > 3) _VideoUnitTypeIndex = 3;
                else if (value < 0) _VideoUnitTypeIndex = 0;
                else _VideoUnitTypeIndex = value;
                VideoUnitType = VideoUnitTypes[_VideoUnitTypeIndex];
                VideoUnit = (BB_Units)(uint.Parse(VideoUnitTypes[_VideoUnitTypeIndex].Parameter));
                OnPropertyChanged("VideoUnitTypeIndex");
            }
        }
        private int _VideoUnitTypeIndex = 0;
        public BB_Units VideoUnit = BB_Units.BB_LOG;
        public ParamWithUI VideoUnitType = new ParamWithUI() { Parameter = "0", UI = "Clear Write" };
        public ObservableCollection<ParamWithUI> VideoUnitTypes
        {
            get { return _VideoUnitTypes; }
            set { _VideoUnitTypes = value; OnPropertyChanged("VideoUnitTypes"); }
        }
        private ObservableCollection<ParamWithUI> _VideoUnitTypes = new ObservableCollection<ParamWithUI>
        {
            new ParamWithUI() { Parameter = "0", UI = "LOG" },
            new ParamWithUI() { Parameter = "1", UI = "VOLTAGE" },
            new ParamWithUI() { Parameter = "2", UI = "POWER" },
            new ParamWithUI() { Parameter = "3", UI = "SAMPLE" },
        };



        #endregion

        #region Markers
        public ObservableCollection<Marker> Markers
        {
            get { return _Markers; }
            set { _Markers = value; OnPropertyChanged("Markers"); }
        }
        private ObservableCollection<Marker> _Markers = new ObservableCollection<Marker>
        {
            new Marker() { Index = 1, MarkerType = 0, IndexOnTrace = -1, TraceNumber = new ParamWithUI() { Parameter = "0", UI = "Trace 1"}, Freq = 99000000 },
            new Marker() { Index = 2, MarkerType = 0, IndexOnTrace = -1, TraceNumber = new ParamWithUI() { Parameter = "0", UI = "Trace 1"}, Freq = 99000000 },
            new Marker() { Index = 3, MarkerType = 0, IndexOnTrace = -1, TraceNumber = new ParamWithUI() { Parameter = "0", UI = "Trace 1"}, Freq = 99000000 },
            new Marker() { Index = 4, MarkerType = 0, IndexOnTrace = -1, TraceNumber = new ParamWithUI() { Parameter = "0", UI = "Trace 1"}, Freq = 99000000 },
            new Marker() { Index = 5, MarkerType = 0, IndexOnTrace = -1, TraceNumber = new ParamWithUI() { Parameter = "0", UI = "Trace 1"}, Freq = 99000000 },
            new Marker() { Index = 6, MarkerType = 0, IndexOnTrace = -1, TraceNumber = new ParamWithUI() { Parameter = "0", UI = "Trace 1"}, Freq = 99000000 },
        };
        public int MarkersIsEnabled
        {
            get { return _MarkersIsEnabled; }
            set { _MarkersIsEnabled = value; OnPropertyChanged("MarkersIsEnabled"); }
        }
        private int _MarkersIsEnabled = 0;
        #endregion Markers

        #region Measurment

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

        #region AutoMeas
        private BB_Gain AutoMeas_Gain = BB_Gain.BB_Gain_AUTO;
        private BB_Atten AutoMeas_Att = BB_Atten.BB_Atten_AUTO;
        private GainATT AutoMeas_GainAtt = new GainATT();
        //private bbStatus AutoMeas_Status = bbStatus.bbNoError;
        public bbStatus AutoMeas_Status
        {
            get { return _AutoMeas_Status; }
            set { _AutoMeas_Status = value; OnPropertyChanged("AutoMeas_Status"); }
        }
        private bbStatus _AutoMeas_Status = bbStatus.bbNoError;

        private bool AutoMeas_GainAttIsSet = false;


        private bool AutoMeas_GainAttSet = false;
        private decimal AutoMeas_LeftSNR = 0;
        private decimal AutoMeas_RightSNR = 0;
        private decimal AutoMeas_PreviousLeftSNR = 0;
        private decimal AutoMeas_PreviousRightSNR = 0;
        #endregion AutoMeas
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
        public bool _GetData;// = "";
        public bool GetData
        {
            get { return _GetData; }
            set { _GetData = value; /*OnPropertyChanged("OutText"); */}
        }
        private long _LastUpdate;// = "";
        public long LastUpdate
        {
            get { return _LastUpdate; }
            set { _LastUpdate = value; /*OnPropertyChanged("OutText"); */}
        }
        #endregion

        #region Device info
        decimal Device_FreqMin = 9000;
        decimal Device_FreqMax = 6000000000;

        /// <summary>
        /// температура приемника от нее пляшем с калибровкой
        /// </summary>
        public double Device_BoardTemp
        {
            get { return _Device_BoardTemp; }
            set { _Device_BoardTemp = value; OnPropertyChanged("Device_BoardTemp"); }
        }
        private double _Device_BoardTemp = 10000;
        /// <summary>
        /// последняя температура калибровки
        /// </summary>
        private double Device_LastCalcBoardTemp = 10000;

        public double Device_USBVoltage
        {
            get { return _Device_USBVoltage; }
            set { _Device_USBVoltage = value; OnPropertyChanged("Device_USBVoltage"); }
        }
        private double _Device_USBVoltage = 10000;

        public double Device_USBCurrent
        {
            get { return _Device_USBCurrent; }
            set { _Device_USBCurrent = value; OnPropertyChanged("Device_USBCurrent"); }
        }
        private double _Device_USBCurrent = 10000;

        public int Device_ID
        {
            get { return _Device_ID; }
            set { _Device_ID = value; OnPropertyChanged("Device_ID"); }
        }
        private int _Device_ID = -1;

        public string Device_Type
        {
            get { return _Device_Type; }
            set { _Device_Type = value; OnPropertyChanged("Device_Type"); }
        }
        private string _Device_Type = "";

        public string Device_SerialNumber
        {
            get { return _Device_SerialNumber; }
            set { _Device_SerialNumber = value; OnPropertyChanged("Device_SerialNumber"); }
        }
        private string _Device_SerialNumber = "";

        public string Device_FirmwareVersion
        {
            get { return _Device_FirmwareVersion; }
            set { _Device_FirmwareVersion = value; OnPropertyChanged("Device_FirmwareVersion"); }
        }
        private string _Device_FirmwareVersion = "";

        public string Device_APIVersion
        {
            get { return _Device_APIVersion; }
            set { _Device_APIVersion = value; OnPropertyChanged("Device_APIVersion"); }
        }
        private string _Device_APIVersion = "";

        public DB.localatdi_meas_device device_meas
        {
            get { return _device_meas; }
            set { _device_meas = value; }
        }
        private DB.localatdi_meas_device _device_meas = new DB.localatdi_meas_device() { };
        #endregion

        #region meas
        public bool AnyMeas
        {
            get { return _AnyMeas; }
            set { _AnyMeas = value; OnPropertyChanged("AnyMeas"); }
        }
        private bool _AnyMeas = false;

        #region MeasMon
        public bool IsMeasMon
        {
            get { return _IsMeasMon; }
            set
            {
                _IsMeasMon = value;
                if (_IsMeasMon) { BBDeviceMode = BB_Mode.BB_SWEEPING; SH_dm += SetMeasMon; }
                else { SH_dm -= SetMeasMon; }
                AnyMeas = _IsMeasMon;
                OnPropertyChanged("IsMeasMon");
            }
        }
        private bool _IsMeasMon = false;

        public DB.MeasData MeasMonItem
        {
            get { return _MeasMonItem; }
            set { _MeasMonItem = value; OnPropertyChanged("MeasMonItem"); }
        }
        private DB.MeasData _MeasMonItem = new DB.MeasData() { };

        public long MeasMonTimeMeas
        {
            get { return _MeasMonTimeMeas; }
            set { _MeasMonTimeMeas = value; OnPropertyChanged("MeasMonTimeMeas"); }
        }
        private long _MeasMonTimeMeas = 0;

        /// <summary>
        /// Количевство измерений на частоте
        /// </summary>
        public int MeasTraceCountOnFreq
        {
            get { return _MeasTraceCountOnFreq; }
            set { _MeasTraceCountOnFreq = value; OnPropertyChanged("MeasTraceCountOnFreq"); }
        }
        private int _MeasTraceCountOnFreq = 10;
        #endregion MeasMon

        #region SomeMeas
        public bool IsSomeMeas
        {
            get { return _IsSomeMeas; }
            set
            {
                _IsSomeMeas = value;
                if (_IsSomeMeas) { BBDeviceMode = BB_Mode.BB_SWEEPING; SH_dm += SetSomeMeas; }
                else { SH_dm -= SetSomeMeas; }
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
        #endregion

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private readonly object _itemsLock = new object();
        public SignalHound()
        {
            FreqCentr = 2142400000;
            FreqSpan = 5000000;
            Trace1 = new tracepoint[TracePoints];
            Trace1Min = new tracepoint[TracePoints];
            for (int i = 0; i < TracePoints; i++)
            {
                decimal freq = FreqStart + FreqStep * i;
                Trace1[i] = new tracepoint()
                {
                    freq = freq,
                    level = -100
                };
                Trace1Min[i] = new tracepoint()
                {
                    freq = freq,
                    level = -100
                };
            }
            //BindingOperations.EnableCollectionSynchronization(MainWindow.db_v2.LocalAtdiTasks, _itemsLock);
        }

        private void Connect()
        {
            SH_dm = sameWork;
            SHThread = new Thread(AllTimeWorks);
            SHThread.Name = "SignalHoundThread";
            SHThread.IsBackground = true;

            SHThread.Start();
            try
            {
                SH_dm += BBConnect;

            }
            catch { }
            // создаем таймер
            SHtmr.AutoReset = true;
            SHtmr.Enabled = true;
            SHtmr.Elapsed += WatchDog;
            SHtmr.Start();
        }
        private void BBConnect()
        {
            try
            {
                //int id = -1;
                status = bbOpenDevice(ref _Device_ID);
                if (status != bbStatus.bbNoError)
                {
                    Debug.Write("Error: Unable to open BB60\n");
                    Debug.Write(bbGetStatusString(status) + "\n");
                    return;
                }
                else
                {
                    Device_Type = bbGetDeviceName(_Device_ID);
                    Device_SerialNumber = bbGetSerialString(_Device_ID);
                    Device_APIVersion = bbGetAPIString();
                    Device_FirmwareVersion = bbGetFirmwareString(_Device_ID);

                    device_meas.manufacture = "Signal Hound";
                    device_meas.model = Device_Type;
                    device_meas.sn = Device_SerialNumber;

                    GetSystemInfo();
                    SetTraceDetector();
                    SetFreqCentrSpan();
                    SetRefATT();
                    SetGain();
                    SetRbwVbwSweepTimeRbwType();
                    SetDetectorLevelUnits();
                    status = bbInitiate(_Device_ID, (int)BB_Mode.BB_SWEEPING, 0);
                }
            }
            #region Exception
            catch (Exception exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "SignalHound", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            #endregion
            SH_dm -= BBConnect;
        }
        private void Disconnect()
        {
            SH_dm -= sameWork;
            SH_dm += SetDisconnect;

        }
        private void SetDisconnect()
        {
            bool methods = false;
            foreach (Delegate d in SH_dm.GetInvocationList())
            {
                if (methods == false)
                { methods = ((DoubMod)d).Method.Name != "SetDisconnect"; }
            }
            if (methods == false)
            {
                SH_dm -= SetDisconnect;
                SHtmr.Stop();
                GetData = false;
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
                SH_dm();
                IsRuning = IsRuning;
                //LastUpdate = DateTime.Now.Ticks;
            }
            bbCloseDevice(_Device_ID);
            //System.Windows.MessageBox.Show("Disconnected");
            SH_dm -= sameWork;
            IsRuning = false;
            SHThread.Abort();
        }
        private void WatchDog(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (Run == true && IsRuning == true && new TimeSpan(DateTime.Now.Ticks - LastUpdate) > new TimeSpan(0, 0, 0, 0, 500 + (int)(SweepTime * 1000)))
            {
                IsRuning = false;
                IsRuning = IsRuning;
            }
        }
        /// <summary>
        /// Get DATA 
        /// </summary>
        public void sameWork()
        {
            int sleep = (int)((SweepTime / 3) * 1000);
            if (sleep == 0) sleep = 1;
            if (sleep > 100) sleep = 100;
            Thread.Sleep(sleep);
            //try
            //{
            if (Run)
            {
                if (BBDeviceMode == BB_Mode.BB_SWEEPING)
                {
                    #region собираем трейс
                    //получаем спектр
                    uint trace_len = 0;
                    double bin_size = 0.0;
                    double start_freq = 0.0;
                    status = bbQueryTraceInfo(_Device_ID, ref trace_len, ref bin_size, ref start_freq);
                    SetOverLoad(status);

                    bin_size = Math.Round(bin_size, 8);
                    FreqStep = (decimal)bin_size;
                    if (status != bbStatus.bbDeviceConnectionErr || status != bbStatus.bbDeviceInvalidErr || status != bbStatus.bbDeviceNotOpenErr || status != bbStatus.bbUSBTimeoutErr)
                    { IsRuning = true; LastUpdate = DateTime.Now.Ticks; }
                    float[] sweep_max, sweep_min;
                    sweep_max = new float[trace_len];
                    sweep_min = new float[trace_len];
                    AutoMeas_Status = bbFetchTrace_32f(_Device_ID, unchecked((int)trace_len), sweep_min, sweep_max);
                    status = AutoMeas_Status;
                    SetOverLoad(status);
                    SetTraceData((int)trace_len, sweep_min, sweep_max, (decimal)start_freq, (decimal)bin_size);

                    #endregion

                    if (NewTrace == true && IsMeasMon == true)
                    {
                        if (GSMBandMeas == false)
                        {
                            //if (MeasMonItem.Trace.Length == TracePoints && MeasMonItem.Trace[0].Freq == Trace[0].Freq && MeasMonItem.Trace[MeasMonItem.Trace.Length - 1].Freq == Trace[Trace.Length - 1].Freq)
                            //{
                            if (AutoMeas_GainAttIsSet == true && MeasMonItem != null && MeasMonItem.AllTraceCountToMeas > MeasMonItem.AllTraceCount) // MeasTraceCount > -1)
                            {
                                #region
                                //совпадает ли частота и полоса
                                bool t1 = MeasMonItem.FreqDN == FreqCentr;
                                bool t2 = Math.Abs(MeasMonItem.FreqDN - MeasMonItem.SpecData.FreqSpan / 2 - (decimal)start_freq) <= (decimal)bin_size;
                                bool t3 = MeasMonItem.SpecData.FreqSpan == (decimal)FreqSpan;
                                bool t4 = MeasMonItem.ThisIsMaximumSignalAtThisFrequency;
                                if (t1 && t2 && t3 && t4)
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
                                        dfc = LM.FindMarkerIndOnTrace(Trace1, MeasMonItem.FreqDN - MeasMonItem.BWData.BWMeasMax / 2);
                                        ufc = LM.FindMarkerIndOnTrace(Trace1, MeasMonItem.FreqDN + MeasMonItem.BWData.BWMeasMax / 2);
                                        cf = LM.FindMarkerIndOnTrace(Trace1, MeasMonItem.FreqDN);
                                        dfl = LM.AverageLevelNearPoint(Trace1, dfc, 10);
                                        ufl = LM.AverageLevelNearPoint(Trace1, ufc, 10);
                                        cfl = LM.AverageLevelNearPoint(Trace1, cf, 10);
                                    }
                                    if (MeasMonItem.Techonology != "GSM" || (cfl > dfl + MeasMonItem.BWData.NdBLevel + 5 && cfl > ufl + MeasMonItem.BWData.NdBLevel + 5))
                                    {
                                        bool changeTrace = false;
                                        #region несовпадает имеющийся трейс из сохранений с текущим по частоте и точкам то затираем на новый
                                        if (MeasMonItem.SpecData.Trace == null ||
                                            MeasMonItem.SpecData.Trace[0] == null ||
                                            MeasMonItem.SpecData.TracePoints != TracePoints ||
                                            MeasMonItem.SpecData.Trace[0].freq != Trace1[0].freq ||
                                            MeasMonItem.SpecData.Trace[MeasMonItem.SpecData.Trace.Length - 1].freq != Trace1[Trace1.Length - 1].freq)//(tt1 || tt2 || tt3 || tt4 || tt5)
                                        {
                                            MeasMonItem.SpecData.Trace = new tracepoint[TracePoints];
                                            for (int i = 0; i < TracePoints; i++)
                                            {
                                                tracepoint p = new tracepoint() { freq = Trace1[i].freq, level = Trace1[i].level };
                                                MeasMonItem.SpecData.Trace[i] = p;
                                            }
                                            MeasMonItem.SpecData.TracePoints = TracePoints;
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
                                        else
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
                                                if (MeasMonItem.SpecData.Trace[i].freq > minf && MeasMonItem.SpecData.Trace[i].freq < maxf && MeasMonItem.SpecData.Trace[i].level > tl)
                                                { tl = MeasMonItem.SpecData.Trace[i].level; ind = i; }
                                            }
                                            MeasMonItem.BWData.NdBResult[0] = ind;
                                            int[] mar = new int[2];
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

                                    if (MeasMonItem.AllTraceCount < MeasMonItem.AllTraceCountToMeas)
                                    { MeasMonItem.AllTraceCount++; }
                                    #endregion
                                }
                                else if ((!t1 || !t2 || !t3 || !t4))
                                {
                                    if (MeasMonItem.AllTraceCount < MeasMonItem.AllTraceCountToMeas && 
                                        MeasMonItem.SpecData.Trace[0].freq == Trace1[0].freq && 
                                        MeasMonItem.SpecData.Trace[MeasMonItem.SpecData.Trace.Length - 1].freq == Trace1[Trace1.Length - 1].freq)
                                    { MeasMonItem.AllTraceCount++; }
                                }
                                //if (MeasMonItem.AllTraceCount < MeasMonItem.AllTraceCountToMeas)
                                //{ MeasMonItem.AllTraceCount++; }
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
                                    GSMBandMeasSelected.Trace[i].level = Math.Round(LM.MeasChannelPower(Trace1, GSMBandMeasSelected.Trace[i].freq, 200000.0m), 2);
                                }
                                GSMBandMeasSelected.Time = new TimeSpan(DateTime.Now.Ticks - Time);
                                GSMBandMeasSelected.MeasTime = MainWindow.gps.LocalTime;
                                GSMBandMeasSelected.latitude = (double)MainWindow.gps.LatitudeDecimal;
                                GSMBandMeasSelected.longitude = (double)MainWindow.gps.LongitudeDecimal;
                                GSMBandMeasSelected.altitude = (double)MainWindow.gps.Altitude;
                                GSMBandMeasSelected.saved = false;
                                GSMBandMeasSelected.Step = TracePoints;
                                GSMBandMeasSelected.Count++;
                            }
                            if (GSMBandMeasSelected.id == 0 && GSMBandMeasSelected.Count >= GSMBandMeasSelected.CountAll)
                            {
                                GSMBandMeas = false; GSMBandMeasTicks = MainWindow.gps.LocalTime.Ticks;
                            }
                            #endregion
                        }
                    }
                    if (NewTrace == true && IsSomeMeas == true)
                    {
                        SomeMeasItem.ThisStayOnFrequency = (decimal)(new TimeSpan(DateTime.Now.Ticks - SomeMeasTimeMeas).TotalSeconds);
                        SetTraceDataSomeMeas(SomeMeasItem, (int)trace_len, sweep_min, sweep_max, (decimal)start_freq, (decimal)bin_size);
                        //SomeMeasItem.ThisStayOnFrequency >= SomeMeasItem.StayOnFrequency
                    }
                    #region
                    if (MeasMonItem != null && MeasMonItem.BWData != null && MeasMonItem.SpecData != null && IsMeasMon && AutoMeas_GainAttIsSet == false && MeasMonNewFreqSet)
                    {
                        if (GSMBandMeas == false)
                        {
                            #region
                            int centrMar = 0;
                            double LeftLevel = 0;
                            double RightLevel = 0;
                            double CentrLevel = 0;
                            decimal minf = (MeasMonItem.FreqDN - (MeasMonItem.BWData.BWMarPeak / 2));
                            decimal maxf = (MeasMonItem.FreqDN + (MeasMonItem.BWData.BWMarPeak / 2));
                            double tl = double.MinValue;
                            for (int i = 0; i < Trace1.Length; i++)
                            {
                                if (Trace1[i].freq > minf && Trace1[i].freq < maxf && Trace1[i].level > tl)
                                { tl = Trace1[i].level; centrMar = i; }
                            }
                            if (MeasMonItem.Techonology != "CDMA")
                            {
                                LeftLevel = LM.AverageLevelNearPointTrue(Trace1, 0, 10);
                                RightLevel = LM.AverageLevelNearPointTrue(Trace1, Trace1.Length - 1, 10);
                                CentrLevel = LM.AverageLevelNearPointTrue(Trace1, centrMar, 10);

                                AutoMeas_GainAtt.gainatt[AutoMeas_GainAtt.GainIndex].att[AutoMeas_GainAtt.AttIndex].CentrLevel = CentrLevel;
                                AutoMeas_GainAtt.gainatt[AutoMeas_GainAtt.GainIndex].att[AutoMeas_GainAtt.AttIndex].LeftLevel = LeftLevel;
                                AutoMeas_GainAtt.gainatt[AutoMeas_GainAtt.GainIndex].att[AutoMeas_GainAtt.AttIndex].RightLevel = RightLevel;
                            }
                            else
                            {
                                CentrLevel = LM.AverageLevelNearPointTrue(Trace1, centrMar, 10);
                                int[] mar = LM.GetMeasNDB(Trace1, centrMar, MeasMonItem.BWData.NdBLevel, MeasMonItem.SpecData.FreqCentr, MeasMonItem.BWData.BWMeasMax, MeasMonItem.BWData.BWMeasMin);
                                LeftLevel = Trace1[mar[0]].level;// LM.AverageLevelNearPointTrue(Trace, mar[0], 10);
                                RightLevel = Trace1[mar[1]].level;// LM.AverageLevelNearPointTrue(Trace, mar[1], 10);

                                AutoMeas_GainAtt.gainatt[AutoMeas_GainAtt.GainIndex].att[AutoMeas_GainAtt.AttIndex].CentrLevel = CentrLevel;
                                AutoMeas_GainAtt.gainatt[AutoMeas_GainAtt.GainIndex].att[AutoMeas_GainAtt.AttIndex].LeftLevel = LeftLevel;
                                AutoMeas_GainAtt.gainatt[AutoMeas_GainAtt.GainIndex].att[AutoMeas_GainAtt.AttIndex].RightLevel = RightLevel;
                            }
                            #endregion
                        }
                        #region
                        //теперь выбираем лучший сигнал шум
                        if (AutoMeas_GainAtt.AttIndex == 3 || AutoMeas_GainAtt.GainIndex == 3)
                        {
                            bool SNR = true;
                            //теперь выбираем лучший сигнал шум
                            #region
                            double ll = 0, rl = 0;
                            int setg = 0, seta = 0;
                            for (int g = 0; g < 4; g++)
                            {
                                for (int a = 0; a < 4; a++)
                                {
                                    if (SNR)
                                    {
                                        if (AutoMeas_GainAtt.gainatt[g].att[a].isoverload == false &&
                                            AutoMeas_GainAtt.gainatt[g].att[a].CentrLevel - AutoMeas_GainAtt.gainatt[g].att[a].LeftLevel > ll &&
                                            AutoMeas_GainAtt.gainatt[g].att[a].CentrLevel - AutoMeas_GainAtt.gainatt[g].att[a].RightLevel > rl)
                                        {
                                            ll = AutoMeas_GainAtt.gainatt[g].att[a].CentrLevel - AutoMeas_GainAtt.gainatt[g].att[a].LeftLevel;
                                            rl = AutoMeas_GainAtt.gainatt[g].att[a].CentrLevel - AutoMeas_GainAtt.gainatt[g].att[a].RightLevel;
                                            setg = g; seta = a;
                                        }
                                    }
                                    else
                                    {
                                        if (AutoMeas_GainAtt.gainatt[g].att[a].isoverload == false && AutoMeas_GainAtt.gainatt[g].att[a].LeftLevel < ll && AutoMeas_GainAtt.gainatt[g].att[a].RightLevel < rl)
                                        {
                                            ll = AutoMeas_GainAtt.gainatt[g].att[a].LeftLevel;
                                            rl = AutoMeas_GainAtt.gainatt[g].att[a].RightLevel;
                                            setg = g; seta = a;
                                        }
                                    }
                                }
                            }
                            #endregion
                            Gain = AutoMeas_GainAtt.gainatt[setg].gain;
                            bbConfigureGain(_Device_ID, (int)Gain);
                            Att = AutoMeas_GainAtt.gainatt[setg].att[seta].att;
                            bbConfigureLevel(_Device_ID, (double)RefLevel, (double)Att);
                            AutoMeas_Status = bbInitiate(_Device_ID, (uint)BBDeviceMode, 0);
                            AutoMeas_GainAttIsSet = true;
                            MeasMonNewFreqSet = false;
                            //Thread.Sleep(1000);
                        }
                        else //еще меряем
                        {
                            //есть перегруз то добавляем атт
                            if (AutoMeas_Status == bbStatus.bbADCOverflow)
                            {
                                AutoMeas_GainAtt.gainatt[AutoMeas_GainAtt.GainIndex].att[AutoMeas_GainAtt.AttIndex].isoverload = true;
                                AutoMeas_GainAtt.PreviousAttIndex = AutoMeas_GainAtt.AttIndex;
                                AutoMeas_GainAtt.AttIndex++;
                                if (AutoMeas_GainAtt.AttIndex > 3) AutoMeas_GainAtt.AttIndex = 3;
                                Att = AutoMeas_GainAtt.gainatt[AutoMeas_GainAtt.GainIndex].att[AutoMeas_GainAtt.AttIndex].att;
                                bbConfigureLevel(_Device_ID, (double)RefLevel, (double)Att);
                                AutoMeas_Status = bbInitiate(_Device_ID, (uint)BBDeviceMode, 0);
                            }
                            //нету то усилим
                            else if (AutoMeas_Status != bbStatus.bbADCOverflow)
                            {
                                AutoMeas_GainAtt.PreviousGainIndex = AutoMeas_GainAtt.GainIndex;
                                AutoMeas_GainAtt.GainIndex++;
                                if (AutoMeas_GainAtt.GainIndex > 3) AutoMeas_GainAtt.GainIndex = 3;
                                Gain = AutoMeas_GainAtt.gainatt[AutoMeas_GainAtt.GainIndex].gain;
                                bbConfigureGain(_Device_ID, (int)Gain);
                                AutoMeas_Status = bbInitiate(_Device_ID, (uint)BBDeviceMode, 0);
                            }
                        }
                        #endregion
                    }
                    #endregion
                    NewTrace = false;
                }
                else if (BBDeviceMode == BB_Mode.BB_REAL_TIME)
                {
                    #region RT
                    uint trace_len = 0;
                    double bin_size = 0, start_freq = 0;
                    status = bbQueryTraceInfo(_Device_ID, ref trace_len, ref bin_size, ref start_freq);
                    SetOverLoad(status);
                    if (status != bbStatus.bbDeviceConnectionErr || status != bbStatus.bbDeviceInvalidErr || status != bbStatus.bbDeviceNotOpenErr || status != bbStatus.bbUSBTimeoutErr)
                    { IsRuning = true; LastUpdate = DateTime.Now.Ticks; }


                    //int frameWidth = 0, RealTimeFrameHeight = 0;
                    status = bbQueryRealTimeInfo(_Device_ID, ref _RealTimeFrameWidth, ref _RealTimeFrameHeight);
                    SetOverLoad(status);
                    float[] sweep = new float[trace_len];
                    float[] Frame = new float[RealTimeFrameWidth * RealTimeFrameHeight];

                    // Retrieve roughly 1 second worth of real-time persistence frames and sweeps.
                    float frameCount = 0;

                    // Don't care about alphaFrame, pass NULL
                    status = bbFetchRealTimeFrame(_Device_ID, sweep, Frame, null);

                    if (TracePoints != trace_len || Math.Abs(TraceFreqStart - (decimal)start_freq) >= (decimal)bin_size)
                    {
                        TraceFreqStart = (decimal)start_freq;
                        TracePoints = (int)trace_len;
                        SetFreqRTArray(TracePoints, sweep, (decimal)start_freq, (decimal)bin_size);
                    }

                    if (Frame.Length > 1000) RealTimeFrame = Frame;
                    if (sweep[0] != (float)Trace1[0].level)
                    {
                        NewTrace = true;
                        for (int i = 0; i < trace_len; i++)
                        {
                            Trace1[i].level = sweep[i];
                        }
                    }
                    #endregion
                }
            }
            //}
            //#region Exception            
            //catch (Exception exp)
            //{
            //    App.Current.Dispatcher.Invoke((Action)(() =>
            //    {
            //        MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "SignalHound", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            //    }));
            //}
            //#endregion
        }
        private void SetOverLoad(bbStatus value)
        {
            if (value == bbStatus.bbADCOverflow) { RFOverload = true; }
            else { RFOverload = false; }
        }
        private void SetTraceData(int newLength, float[] mintrace, float[] maxtrace, decimal freqStart, decimal step)
        {

            tracepoint[] temp11 = Trace1;
            tracepoint[] temp12 = Trace1Min;
            tracepoint[] temp21 = Trace2;
            tracepoint[] temp31 = Trace3;
            if (maxtrace.Length > 0 && newLength > 0 && step > 0)
            {
                if (TracePoints != newLength || (Math.Abs(TraceFreqStart - (decimal)freqStart) >= (decimal)step))
                {
                    TraceFreqStart = (decimal)freqStart;
                    TracePoints = (int)newLength;
                    temp11 = new tracepoint[newLength];
                    temp12 = new tracepoint[newLength];
                    temp21 = new tracepoint[newLength];
                    temp31 = new tracepoint[newLength];
                    for (int i = 0; i < newLength; i++)
                    {
                        decimal freq = freqStart + step * i;
                        temp11[i] = new tracepoint() { freq = freq, level = maxtrace[i] };
                        temp12[i] = new tracepoint() { freq = freq, level = mintrace[i] };
                        temp21[i] = new tracepoint() { freq = freq, level = maxtrace[i] };
                        temp31[i] = new tracepoint() { freq = freq, level = maxtrace[i] };
                    }
                    MarkersTraceLegthOrFreqsChanged(temp11);
                }
                #region Trace 1
                if (Trace1Type.Parameter == "0")
                {
                    for (int i = 0; i < newLength; i++)
                    {
                        temp11[i].level = maxtrace[i];
                        if (DrawTrace1Min)
                            temp12[i].level = mintrace[i];
                    }
                }
                else if (Trace1Type.Parameter == "1")
                {
                    tracepoint[] temp = new tracepoint[newLength];
                    for (int i = 0; i < newLength; i++)
                    {
                        decimal freq = freqStart + step * i;
                        temp[i] = new tracepoint() { freq = freq, level = maxtrace[i] };
                    }
                    if (Trace1Reset) { Trace1AveragedList.Reset(); Trace1Reset = false; }
                    Trace1AveragedList.AddTraceToAverade(temp);
                    temp11 = Trace1AveragedList.AveragedTrace;
                }
                else if (Trace1Type.Parameter == "2")
                {
                    tracepoint[] temp = new tracepoint[newLength];
                    for (int i = 0; i < newLength; i++)
                    {
                        decimal freq = freqStart + step * i;
                        temp[i] = new tracepoint() { freq = freq, level = maxtrace[i] };
                    }
                    if (Trace1Reset) { Trace1TrackedList.Reset(); Trace1Reset = false; }
                    Trace1TrackedList.AddTraceToTracking(temp);
                    temp11 = Trace1TrackedList.TrackingTrace;
                }
                else if (Trace1Type.Parameter == "3")
                {
                    if (Trace1Reset == false)
                    {
                        for (int i = 0; i < newLength; i++)
                        {
                            if (maxtrace[i] > temp11[i].level) temp11[i].level = maxtrace[i];
                        }
                    }
                    else
                    {
                        for (int i = 0; i < newLength; i++)
                        {
                            temp11[i].level = maxtrace[i];
                        }
                        Trace1Reset = false;
                    }
                }
                else if (Trace1Type.Parameter == "4")
                {
                    if (Trace1Reset == false)
                    {
                        for (int i = 0; i < newLength; i++)
                        {
                            if (maxtrace[i] < temp11[i].level) temp11[i].level = maxtrace[i];
                        }
                    }
                    else
                    {
                        for (int i = 0; i < newLength; i++)
                        {
                            temp11[i].level = maxtrace[i];
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
                    for (int i = 0; i < newLength; i++)
                    {
                        temp21[i].level = maxtrace[i];
                    }
                }
                else if (Trace2Type.Parameter == "1")
                {
                    tracepoint[] temp = new tracepoint[newLength];
                    for (int i = 0; i < newLength; i++)
                    {
                        decimal freq = freqStart + step * i;
                        temp[i] = new tracepoint() { freq = freq, level = maxtrace[i] };
                    }
                    if (Trace2Reset) { Trace2AveragedList.Reset(); Trace2Reset = false; }
                    Trace2AveragedList.AddTraceToAverade(temp);
                    temp21 = Trace2AveragedList.AveragedTrace;
                }
                else if (Trace2Type.Parameter == "2")
                {
                    tracepoint[] temp = new tracepoint[newLength];
                    for (int i = 0; i < newLength; i++)
                    {
                        decimal freq = freqStart + step * i;
                        temp[i] = new tracepoint() { freq = freq, level = maxtrace[i] };
                    }
                    if (Trace2Reset) { Trace2TrackedList.Reset(); Trace2Reset = false; }
                    Trace2TrackedList.AddTraceToTracking(temp);
                    temp21 = Trace2TrackedList.TrackingTrace;
                }
                else if (Trace2Type.Parameter == "3")
                {
                    if (Trace2Reset == false)
                    {
                        for (int i = 0; i < newLength; i++)
                        {
                            if (maxtrace[i] > temp21[i].level) temp21[i].level = maxtrace[i];
                        }
                    }
                    else
                    {
                        for (int i = 0; i < newLength; i++)
                        {
                            temp21[i].level = maxtrace[i];
                        }
                        Trace2Reset = false;
                    }
                }
                else if (Trace2Type.Parameter == "4")
                {
                    if (Trace2Reset == false)
                    {
                        for (int i = 0; i < newLength; i++)
                        {
                            if (maxtrace[i] < temp21[i].level) temp21[i].level = maxtrace[i];
                        }
                    }
                    else
                    {
                        for (int i = 0; i < newLength; i++)
                        {
                            temp21[i].level = maxtrace[i];
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
                    for (int i = 0; i < newLength; i++)
                    {
                        temp31[i].level = maxtrace[i];
                    }
                }
                else if (Trace3Type.Parameter == "1")
                {
                    tracepoint[] temp = new tracepoint[newLength];
                    for (int i = 0; i < newLength; i++)
                    {
                        decimal freq = freqStart + step * i;
                        temp[i] = new tracepoint() { freq = freq, level = maxtrace[i] };
                    }
                    if (Trace3Reset) { Trace3AveragedList.Reset(); Trace3Reset = false; }
                    Trace3AveragedList.AddTraceToAverade(temp);
                    temp31 = Trace3AveragedList.AveragedTrace;
                }
                else if (Trace3Type.Parameter == "2")
                {
                    tracepoint[] temp = new tracepoint[newLength];
                    for (int i = 0; i < newLength; i++)
                    {
                        decimal freq = freqStart + step * i;
                        temp[i] = new tracepoint() { freq = freq, level = maxtrace[i] };
                    }
                    if (Trace3Reset) { Trace3TrackedList.Reset(); Trace3Reset = false; }
                    Trace3TrackedList.AddTraceToTracking(temp);
                    temp31 = Trace3TrackedList.TrackingTrace;
                }
                else if (Trace3Type.Parameter == "3")
                {
                    if (Trace3Reset == false)
                    {
                        for (int i = 0; i < newLength; i++)
                        {
                            if (maxtrace[i] > temp31[i].level) temp31[i].level = maxtrace[i];
                        }
                    }
                    else
                    {
                        for (int i = 0; i < newLength; i++)
                        {
                            temp31[i].level = maxtrace[i];
                        }
                        Trace3Reset = false;
                    }
                }
                else if (Trace3Type.Parameter == "4")
                {
                    if (Trace3Reset == false)
                    {
                        for (int i = 0; i < newLength; i++)
                        {
                            if (maxtrace[i] < temp31[i].level) temp31[i].level = maxtrace[i];
                        }
                    }
                    else
                    {
                        for (int i = 0; i < newLength; i++)
                        {
                            temp31[i].level = maxtrace[i];
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
                NewTrace = true;
                Trace1 = temp11;
                Trace1Min = temp12;
                Trace2 = temp21;
                Trace3 = temp31;

                if (ChannelPowerState)
                    ChannelPowerResult = LM.MeasChannelPower(Trace1, FreqCentr, ChannelPowerBW);
                SetMarkerData();
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
                    if (Trace1Reset) { data.TraceAveragedList.Reset(); Trace1Reset = false; }
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
                    if (Trace1Reset) { data.TraceTrackedList.Reset(); Trace1Reset = false; }
                    data.TraceTrackedList.AddTraceToTracking(temp);
                    temp1 = data.TraceTrackedList.TrackingTrace;
                }
                else if (data.TraceTypeIndex == 3)
                {
                    if (Trace1Reset == false)
                    {
                        for (int i = 0; i < newLength; i++)
                        {
                            if (maxtrace[i] > temp1[i].level) temp1[i].level = maxtrace[i];
                        }
                    }
                    else
                    {
                        for (int i = 0; i < newLength; i++)
                        {
                            temp1[i].level = maxtrace[i];
                        }
                        Trace1Reset = false;
                    }
                }
                else if (data.TraceTypeIndex == 4)
                {
                    if (Trace1Reset == false)
                    {
                        for (int i = 0; i < newLength; i++)
                        {
                            if (maxtrace[i] < temp1[i].level) temp1[i].level = maxtrace[i];
                        }
                    }
                    else
                    {
                        for (int i = 0; i < newLength; i++)
                        {
                            temp1[i].level = maxtrace[i];
                        }
                        Trace1Reset = false;
                    }
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
                NewTrace = true;
                //if (ChannelPowerState) ChannelPowerResult = LM.MeasChannelPower(Trace, FreqCentr, ChannelPowerBW, LevelUnits[LevelUnitIndex].ind);
                data.Trace = temp1;
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

                                Markers[i].TMarkers[0].IndexOnTrace = t[1];
                                Markers[i].TMarkers[0].Freq = Trace1[t[1]].freq;
                                Markers[i].TMarkers[0].Level = Trace1[t[1]].level;

                                Markers[i].TMarkers[1].IndexOnTrace = t[2];
                                Markers[i].TMarkers[1].Freq = Trace1[t[2]].freq;
                                Markers[i].TMarkers[1].Level = Trace1[t[2]].level;
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

                                Markers[i].TMarkers[0].IndexOnTrace = t[1];
                                Markers[i].TMarkers[0].Freq = Trace2[t[1]].freq;
                                Markers[i].TMarkers[0].Level = Trace2[t[1]].level;

                                Markers[i].TMarkers[1].IndexOnTrace = t[2];
                                Markers[i].TMarkers[1].Freq = Trace2[t[2]].freq;
                                Markers[i].TMarkers[1].Level = Trace2[t[2]].level;
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

                                Markers[i].TMarkers[0].IndexOnTrace = t[1];
                                Markers[i].TMarkers[0].Freq = Trace3[t[1]].freq;
                                Markers[i].TMarkers[0].Level = Trace3[t[1]].level;

                                Markers[i].TMarkers[1].IndexOnTrace = t[2];
                                Markers[i].TMarkers[1].Freq = Trace3[t[2]].freq;
                                Markers[i].TMarkers[1].Level = Trace3[t[2]].level;
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
            MarkersIsEnabled = count;
        }
        private void SetFreqRTArray(int newLength, float[] trace, decimal freqStart, decimal step)
        {
            try
            {
                tracepoint[] temp1 = new tracepoint[newLength];
                for (int i = 0; i < newLength; i++)
                {
                    decimal freq = freqStart + step * i;
                    temp1[i] = new tracepoint() { freq = freq, level = trace[i] };
                }
                Trace1 = temp1;
            }
            #region Exception
            catch (Exception exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "SignalHound", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            #endregion
        }

        #region Public Method
        #region Freq
        public void SetFreqCentrSqeeping(decimal freq)
        {
            if (freq > Device_FreqMax) FreqCentr = Device_FreqMax;
            else if (freq < Device_FreqMin) FreqCentr = Device_FreqMin;
            else FreqCentr = freq;
            SH_dm += SetFreqCentrSpan;
            SH_dm += Initiate;
        }
        public void SetFreqSpanSqeeping(decimal freq)
        {
            if (freq > Device_FreqMax) FreqSpan = Device_FreqMax;
            else if (freq < Device_FreqMin) FreqSpan = Device_FreqMin;
            else FreqSpan = freq;
            SH_dm += SetFreqCentrSpan;
            SH_dm += Initiate;
        }
        public void SetFreqStartSqeeping(decimal freq)
        {
            if (freq > Device_FreqMax) FreqStart = Device_FreqMax;
            else if (freq < Device_FreqMin) FreqStart = Device_FreqMin;
            else FreqStart = freq;
            SH_dm += SetFreqCentrSpan;
            SH_dm += Initiate;
        }
        public void SetFreqStopSqeeping(decimal freq)
        {
            if (freq > Device_FreqMax) FreqStop = Device_FreqMax;
            else if (freq < Device_FreqMin) FreqStop = Device_FreqMin;
            else FreqStop = freq;
            SH_dm += SetFreqCentrSpan;
            SH_dm += Initiate;
        }
        #endregion Freq

        #region RBW /VBW
        public void SetRBWFromIndex(int index)
        {
            RBWIndex = index;
            SH_dm += SetRbwVbwSweepTimeRbwType;
            SH_dm += Initiate;
        }
        public void SetRBWFromBW(decimal rbw)
        {
            //Gain = rbw;
            SH_dm += SetRbwVbwSweepTimeRbwType;
            SH_dm += Initiate;
        }
        public void SetVBWFromIndex(int index)
        {
            VBWIndex = index;
            SH_dm += SetRbwVbwSweepTimeRbwType;
            SH_dm += Initiate;
        }
        public void SetVBWFromBW(decimal vbw)
        {
            //Gain = rbw;
            SH_dm += SetRbwVbwSweepTimeRbwType;
            SH_dm += Initiate;
        }
        #endregion

        #region Sweep
        public void SetSweepTime(decimal sweeptime)
        {
            SweepTime = sweeptime;
            SH_dm += SetRbwVbwSweepTimeRbwType;
            SH_dm += Initiate;
        }

        #endregion

        #region Amplitude
        public void SetRefLevel(decimal reflevel)
        {
            RefLevel = reflevel;
            SH_dm += SetRefATT;
            SH_dm += Initiate;
        }
        public void SetGain(BB_Gain gain)
        {
            Gain = gain;
            SH_dm += SetGain;
            SH_dm += Initiate;
        }
        public void SetAtt(BB_Atten att)
        {
            Att = att;
            SH_dm += SetRefATT;
            SH_dm += Initiate;
        }
        #endregion

        #region Trace
        public void SetDetector(BB_Detector detector)
        {
            BBDetector = detector;
            SH_dm += SetDetectorLevelUnits;
            SH_dm += Initiate;
        }
        public void SetVideoUnit(BB_Units unit)
        {
            VideoUnit = unit;
            SH_dm += SetVideoUnits;
            SH_dm += Initiate;
        }
        #endregion

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

        #region DeviceMode
        public void SetDeviceMode(BB_Mode devicemode)
        {
            BBDeviceMode = devicemode;
            SH_dm += SetDeviceMode;
        }
        #endregion DeviceMode

        #endregion
        private void Initiate()
        {
            try
            {
                status = bbInitiate(_Device_ID, (uint)BBDeviceMode, 0);
            }
            #region Exception
            catch (Exception exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "SignalHound", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            #endregion
            SH_dm -= Initiate;
        }
        private void GetSystemInfo()
        {
            try
            {
                float temp = 0.0F, voltage = 0.0F, current = 0.0F;
                bbGetDeviceDiagnostics(_Device_ID, ref temp, ref voltage, ref current);
            }
            #region Exception
            catch (Exception exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "SignalHound", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            #endregion
            SH_dm -= GetSystemInfo;
        }
        private void SetTraceDetector()
        {
            try
            {
                bbConfigureAcquisition(_Device_ID, (uint)BB_Detector.BB_AVERAGE, (uint)BB_SCALE.BB_LOG_SCALE);
            }
            #region Exception
            catch (Exception exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "SignalHound", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            #endregion
            SH_dm -= SetTraceDetector;
        }
        private void SetFreqCentrSpan()
        {
            try
            {
                bbConfigureCenterSpan(_Device_ID, (double)FreqCentr, (double)FreqSpan);
                FreqSet = true;
            }
            #region Exception
            catch (Exception exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "SignalHound", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            #endregion
            SH_dm -= SetFreqCentrSpan;
        }
        private void SetRefATT()
        {
            try
            {
                bbConfigureLevel(_Device_ID, (double)RefLevel, (double)Att);
            }
            #region Exception
            catch (Exception exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "SignalHound", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            #endregion
            SH_dm -= SetRefATT;
        }
        private void SetGain()
        {
            try
            {
                status = bbConfigureGain(_Device_ID, (int)Gain);
                //App.Current.Dispatcher.Invoke((Action)(() =>
                //{
                //    ((SplashWindow)App.Current.MainWindow).m_mainWindow.Message = status.ToString();
                //}));
            }
            #region Exception
            catch (Exception exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "SignalHound", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            #endregion
            SH_dm -= SetGain;
        }
        private void SetRbwVbwSweepTimeRbwType()
        {
            try
            {
                status = bbConfigureSweepCoupling(_Device_ID, (double)RBW, (double)VBW, (double)SweepTime, (uint)BB_RBWType.BB_RBW_SHAPE_FLATTOP, (uint)BB_Rejection.BB_SPUR_REJECT);
                //App.Current.Dispatcher.Invoke((Action)(() =>
                //{
                //    ((SplashWindow)App.Current.MainWindow).m_mainWindow.Message = status.ToString();
                //}));
            }
            #region Exception
            catch (Exception exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "SignalHound", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            #endregion
            SH_dm -= SetRbwVbwSweepTimeRbwType;
        }
        private void SetVideoUnits()
        {
            try
            {
                bbConfigureProcUnits(_Device_ID, (uint)VideoUnit);
            }
            #region Exception
            catch (Exception exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "SignalHound", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            #endregion
            SH_dm -= SetVideoUnits;
        }
        private void SetDetectorLevelUnits()
        {
            try
            {
                status = bbConfigureAcquisition(_Device_ID, (uint)BBDetector, (uint)BB_SCALE.BB_LOG_SCALE);
            }
            #region Exception
            catch (Exception exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "SignalHound", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            #endregion
            SH_dm -= SetDetectorLevelUnits;
        }
        private void SetDeviceMode()
        {
            if (BBDeviceMode == BB_Mode.BB_SWEEPING)
            { }
            else if (BBDeviceMode == BB_Mode.BB_REAL_TIME)
            {
                //bbAbort(_Device_ID);
                bbConfigureAcquisition(_Device_ID, (uint)BBDetector, (uint)BB_SCALE.BB_LOG_SCALE);
                if (FreqSpan > 27000000) FreqSpan = 27000000;
                bbConfigureCenterSpan(_Device_ID, (double)FreqCentr, (double)FreqSpan);
                bbConfigureLevel(_Device_ID, (double)RefLevel, (double)Att);
                bbConfigureGain(_Device_ID, (int)BB_Gain.BB_Gain_AUTO);
                // 9.8kHz RBW, for real-time must specify a Nuttall BW value,
                // See API manual for possible RBW values
                bbConfigureSweepCoupling(_Device_ID, 9863.28125, 9863.28125, 0.001, (uint)BB_RBWType.BB_RBW_SHAPE_NUTTALL, (uint)BB_Rejection.BB_NO_SPUR_REJECT);
                // Configure a frame rate of 30fps and 100dB scale
                bbConfigureRealTime(_Device_ID, (double)Range, 30);

                // Initialize the device for real-time mode
                status = bbInitiate(_Device_ID, (uint)BBDeviceMode, 0);
                if (status != bbStatus.bbNoError)
                {
                    // Handle error
                }
            }
            SH_dm -= SetDeviceMode;
        }

        #region Meas
        long GSMBandMeasTicks = 0;
        bool GSMBandMeas = false;
        GSMBandMeas GSMBandMeasSelected = new Equipment.GSMBandMeas() { };
        public void SetMeasMon()
        {
            #region
            if (Run && IsRuning == true && MainWindow.gps.GPSIsValid && MainWindow.db_v2.MeasMon.Data.Count > 0)//)//
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
                                    if (FreqCentr != (GSMBandMeasSelected.Start + GSMBandMeasSelected.Stop) / 2)
                                    {
                                        FreqStart = GSMBandMeasSelected.Start;
                                        FreqStop = GSMBandMeasSelected.Stop;
                                        decimal step = (FreqSpan / GSMBandMeasSelected.TracePoints) * 4;
                                        bbConfigureSweepCoupling(_Device_ID, (double)step, (double)step, 0.001, (uint)BB_RBWType.BB_RBW_SHAPE_FLATTOP, (uint)BB_Rejection.BB_NO_SPUR_REJECT);
                                        bbConfigureCenterSpan(_Device_ID, (double)FreqCentr, (double)FreqSpan);
                                        #region начинаем играеться с Gain и ATT
                                        //AutoMeas_GainAtt = new GainATT();
                                        Gain = BB_Gain.BB_Gain_2;// AutoMeas_GainAtt.gainatt[2].gain;
                                        bbConfigureGain(_Device_ID, (int)Gain);
                                        Att = BB_Atten.BB_Atten_10;
                                        bbConfigureLevel(_Device_ID, (double)RefLevel, (double)Att);
                                        #endregion
                                        status = bbInitiate(_Device_ID, (uint)BBDeviceMode, 0);
                                    }
                                    FreqSet = true;
                                    MeasMonNewFreqSet = true;
                                    AutoMeas_GainAttIsSet = false;


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
                                MainWindow.db_v2.MeasMon.Data[i].ThisToMeas = false;
                                if (MainWindow.db_v2.MeasMon.Data[i].ThisIsMaximumSignalAtThisFrequency == true &&
                                    MainWindow.db_v2.MeasMon.Data[i].AllTraceCountToMeas < Ind)
                                {
                                    if (MainWindow.db_v2.MeasMon.Data[i].Techonology == "GSM" && MainWindow.db_v2.MeasMon.Data[i].Power > TSMxReceiver.DetectionLevelGSM)
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
                                MainWindow.db_v2.MeasMon.Data[ii].ThisToMeas = true;
                                DB.MeasData temp = MainWindow.db_v2.MeasMon.Data[ii];

                                string tec = MainWindow.db_v2.MeasMon.Data[ii].Techonology;
                                if (tec == "GSM")
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
                                        //if (gsmind > -1 && gsm[gsmind].Power > App.Sett.MeasMons_Settings.MeasMons.Where(x => x.Techonology == DB.Technologys.GSM).First().DetectionLevel)
                                        //{
                                        MeasMonItem = temp;
                                        MeasMonItem.AllTraceCountToMeas = ((MeasMonItem.AllTraceCount + MeasTraceCountOnFreq) / 10) * 10;
                                        //смена частоты
                                        long beginTiks = DateTime.Now.Ticks;
                                        if (FreqCentr != MeasMonItem.FreqDN)
                                        {
                                            FreqCentr = MeasMonItem.FreqDN;
                                            FreqSpan = MeasMonItem.SpecData.FreqSpan;
                                            decimal step = (FreqSpan / MeasMonItem.SpecData.TracePoints) * 4;
                                            bbConfigureSweepCoupling(_Device_ID, (double)step, (double)step, 0.001, (uint)BB_RBWType.BB_RBW_SHAPE_FLATTOP, (uint)BB_Rejection.BB_NO_SPUR_REJECT);
                                            bbConfigureCenterSpan(_Device_ID, (double)FreqCentr, (double)FreqSpan);
                                            #region начинаем играеться с Gain и ATT
                                            AutoMeas_GainAtt = new GainATT();
                                            Gain = AutoMeas_GainAtt.gainatt[AutoMeas_GainAtt.GainIndex].gain;
                                            bbConfigureGain(_Device_ID, (int)Gain);
                                            Att = AutoMeas_GainAtt.gainatt[AutoMeas_GainAtt.GainIndex].att[AutoMeas_GainAtt.AttIndex].att;
                                            bbConfigureLevel(_Device_ID, (double)RefLevel, (double)Att);
                                            #endregion
                                            status = bbInitiate(_Device_ID, (uint)BBDeviceMode, 0);
                                        }
                                        FreqSet = true;
                                        MeasMonNewFreqSet = true;
                                        AutoMeas_GainAttIsSet = false;
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
                                        //ObservableCollection<UMTSBTSData> umts = IdentificationData.UMTS.BTS;
                                        //int umtsind = -1;
                                        //if (umts != null)
                                        //    for (int i = 0; i < umts.Count; i++)
                                        //    {
                                        //        if (umts[i].GCID == temp.GCID && umts[i].FreqDn == temp.FreqDN) { umtsind = i; }
                                        //    }
                                        //if (umtsind > -1 && umts[umtsind].RSCP > App.Sett.MeasMons_Settings.MeasMons.Where(x => x.Techonology == DB.Technologys.UMTS).First().DetectionLevel)
                                        //{
                                        MeasMonItem = temp;
                                        MeasMonItem.AllTraceCountToMeas = ((MeasMonItem.AllTraceCount + MeasTraceCountOnFreq) / 10) * 10;
                                        //смена частоты
                                        long beginTiks = DateTime.Now.Ticks;
                                        if (FreqCentr != MeasMonItem.FreqDN)
                                        {
                                            FreqCentr = MeasMonItem.FreqDN;
                                            FreqSpan = MeasMonItem.SpecData.FreqSpan;
                                            decimal step = (FreqSpan / MeasMonItem.SpecData.TracePoints) * 4;
                                            bbConfigureSweepCoupling(_Device_ID, (double)step, (double)step, 0.001, (uint)BB_RBWType.BB_RBW_SHAPE_FLATTOP, (uint)BB_Rejection.BB_NO_SPUR_REJECT);
                                            bbConfigureCenterSpan(_Device_ID, (double)FreqCentr, (double)FreqSpan);
                                            #region начинаем играеться с Gain и ATT
                                            AutoMeas_GainAtt = new GainATT();
                                            Gain = AutoMeas_GainAtt.gainatt[AutoMeas_GainAtt.GainIndex].gain;
                                            bbConfigureGain(_Device_ID, (int)Gain);
                                            Att = AutoMeas_GainAtt.gainatt[AutoMeas_GainAtt.GainIndex].att[AutoMeas_GainAtt.AttIndex].att;
                                            bbConfigureLevel(_Device_ID, (double)RefLevel, (double)Att);
                                            #endregion
                                            status = bbInitiate(_Device_ID, (uint)BBDeviceMode, 0);
                                        }
                                        FreqSet = true;
                                        MeasMonNewFreqSet = true;
                                        AutoMeas_GainAttIsSet = false;
                                        MeasMonTimeMeas = DateTime.Now.Ticks;
                                        //}
                                    }
                                    #endregion
                                }
                                else if (tec == "LTE")
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
                                        //if (lteind > -1 && IdentificationData.LTE.BTS[lteind].RSRP > App.Sett.MeasMons_Settings.MeasMons.Where(x => x.Techonology == DB.Technologys.LTE).First().DetectionLevel)
                                        //{
                                        MeasMonItem = temp;
                                        MeasMonItem.AllTraceCountToMeas = ((MeasMonItem.AllTraceCount + MeasTraceCountOnFreq) / 10) * 10;
                                        //смена частоты
                                        long beginTiks = DateTime.Now.Ticks;
                                        if (FreqCentr != MeasMonItem.FreqDN)
                                        {
                                            FreqCentr = MeasMonItem.FreqDN;
                                            FreqSpan = MeasMonItem.SpecData.FreqSpan;
                                            decimal step = (FreqSpan / MeasMonItem.SpecData.TracePoints) * 4;
                                            bbConfigureSweepCoupling(_Device_ID, (double)step, (double)step, 0.001, (uint)BB_RBWType.BB_RBW_SHAPE_FLATTOP, (uint)BB_Rejection.BB_NO_SPUR_REJECT);
                                            bbConfigureCenterSpan(_Device_ID, (double)FreqCentr, (double)FreqSpan);
                                            #region начинаем играеться с Gain и ATT
                                            AutoMeas_GainAtt = new GainATT();
                                            Gain = AutoMeas_GainAtt.gainatt[AutoMeas_GainAtt.GainIndex].gain;
                                            bbConfigureGain(_Device_ID, (int)Gain);
                                            Att = AutoMeas_GainAtt.gainatt[AutoMeas_GainAtt.GainIndex].att[AutoMeas_GainAtt.AttIndex].att;
                                            bbConfigureLevel(_Device_ID, (double)RefLevel, (double)Att);
                                            #endregion
                                            status = bbInitiate(_Device_ID, (uint)BBDeviceMode, 0);
                                        }
                                        FreqSet = true;
                                        MeasMonNewFreqSet = true;
                                        AutoMeas_GainAttIsSet = false;
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
                                        ////ObservableCollection<LTEBTSData> umts = IdentificationData.LTE.BTS;
                                        //int cdmaind = -1;
                                        //if (IdentificationData.CDMA.BTS != null)
                                        //    for (int i = 0; i < IdentificationData.CDMA.BTS.Count; i++)
                                        //    {
                                        //        if (IdentificationData.CDMA.BTS[i].GCID == temp.GCID && IdentificationData.CDMA.BTS[i].FreqDn == temp.FreqDN) { cdmaind = i; }
                                        //    }
                                        //if (cdmaind > -1 && IdentificationData.CDMA.BTS[cdmaind].RSCP > App.Sett.MeasMons_Settings.MeasMons.Where(x => x.Techonology == DB.Technologys.CDMA).First().DetectionLevel)
                                        //{
                                        MeasMonItem = temp;
                                        MeasMonItem.AllTraceCountToMeas = ((MeasMonItem.AllTraceCount + MeasTraceCountOnFreq) / 10) * 10;
                                        //смена частоты
                                        long beginTiks = DateTime.Now.Ticks;
                                        if (FreqCentr != MeasMonItem.FreqDN)
                                        {
                                            FreqCentr = MeasMonItem.FreqDN;
                                            FreqSpan = MeasMonItem.SpecData.FreqSpan;
                                            decimal step = (FreqSpan / MeasMonItem.SpecData.TracePoints) * 4;
                                            bbConfigureSweepCoupling(_Device_ID, (double)step, (double)step, 0.001, (uint)BB_RBWType.BB_RBW_SHAPE_FLATTOP, (uint)BB_Rejection.BB_NO_SPUR_REJECT);
                                            bbConfigureCenterSpan(_Device_ID, (double)FreqCentr, (double)FreqSpan);
                                            #region начинаем играеться с Gain и ATT
                                            AutoMeas_GainAtt = new GainATT();
                                            Gain = AutoMeas_GainAtt.gainatt[AutoMeas_GainAtt.GainIndex].gain;
                                            bbConfigureGain(_Device_ID, (int)Gain);
                                            Att = AutoMeas_GainAtt.gainatt[AutoMeas_GainAtt.GainIndex].att[AutoMeas_GainAtt.AttIndex].att;
                                            bbConfigureLevel(_Device_ID, (double)RefLevel, (double)Att);
                                            #endregion
                                            status = bbInitiate(_Device_ID, (uint)BBDeviceMode, 0);
                                        }
                                        FreqSet = true;
                                        MeasMonNewFreqSet = true;
                                        AutoMeas_GainAttIsSet = false;
                                        MeasMonTimeMeas = DateTime.Now.Ticks;
                                        //}
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
                                else if (tec == "WRLS")
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
                                    //        //смена частоты
                                    //        long beginTiks = DateTime.Now.Ticks;
                                    //        if (FreqCentr != MeasMonItem.FreqDN)
                                    //        {
                                    //            FreqCentr = MeasMonItem.FreqDN;
                                    //            FreqSpan = MeasMonItem.MeasSpan;
                                    //            decimal step = (FreqSpan / 1600) * 4;
                                    //            bbConfigureSweepCoupling(_Device_ID, (double)step, (double)step, 0.001, (uint)BB_RBWType.BB_NON_NATIVE_RBW, (uint)BB_Rejection.BB_NO_SPUR_REJECT);
                                    //            bbConfigureCenterSpan(_Device_ID, (double)FreqCentr, (double)FreqSpan);
                                    //            #region начинаем играеться с Gain и ATT
                                    //            AutoMeas_GainAtt = new GainATT();
                                    //            Gain = AutoMeas_GainAtt.gainatt[AutoMeas_GainAtt.GainIndex].gain;
                                    //            bbConfigureGain(_Device_ID, (int)Gain);
                                    //            Att = AutoMeas_GainAtt.gainatt[AutoMeas_GainAtt.GainIndex].att[AutoMeas_GainAtt.AttIndex].att;
                                    //            bbConfigureLevel(_Device_ID, (double)RefLevel, (double)Att);
                                    //            #endregion
                                    //            status = bbInitiate(_Device_ID, (uint)BBDeviceMode, 0);
                                    //        }
                                    //        FreqSet = true;
                                    //        MeasMonNewFreqSet = true;
                                    //        AutoMeas_GainAttIsSet = false;
                                    //        MeasMonTimeMeas = DateTime.Now.Ticks;

                                    //    }
                                    //}
                                    #endregion
                                }
                                MeasMonItem.device_meas = device_meas;
                                //Thread.Sleep(1);
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
            //}));
            #endregion
        }

        public void SetSomeMeas()
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
                        bbConfigureCenterSpan(_Device_ID, (double)FreqCentr, (double)FreqSpan);
                        RBWIndex = SomeMeasItem.RBWIndex;
                        VBWIndex = SomeMeasItem.VBWIndex;
                        SweepTime = SomeMeasItem.SweepTime;
                        status = bbConfigureSweepCoupling(_Device_ID, (double)RBW, (double)VBW, (double)SweepTime, (uint)BB_RBWType.BB_RBW_SHAPE_FLATTOP, (uint)BB_Rejection.BB_SPUR_REJECT);
                        status = bbInitiate(_Device_ID, (uint)BBDeviceMode, 0);
                    }
                }
            }
        }
        #endregion







        #region Dll import
        #region Enums
        public enum BB_Device : int
        {
            BB_BB60A = 0x1,
            BB_BB60C = 0x2
        }
        public enum BB_Atten : int
        {
            BB_Atten_AUTO = -1,
            BB_Atten_0 = 0,
            BB_Atten_10 = 10,
            BB_Atten_20 = 20,
            BB_Atten_30 = 30,
        }
        public enum BB_Gain : int
        {
            BB_Gain_AUTO = -1,
            BB_Gain_0 = 0,
            BB_Gain_1 = 1,
            BB_Gain_2 = 2,
            BB_Gain_3 = 3,
        }
        public enum BB_Detector : uint
        {
            BB_MIN_AND_MAX = 0x0,
            BB_AVERAGE = 0x1,
            BB_MIN_ONLY = 0x2,
            BB_MAX_ONLY = 0x3,
        }
        public enum BB_SCALE : uint
        {
            BB_LOG_SCALE = 0x0,
            BB_LIN_SCALE = 0x1,
            BB_LOG_FULL_SCALE = 0x2,
            BB_LIN_FULL_SCALE = 0x3,
        }
        public enum BB_RBWType : uint
        {
            BB_RBW_SHAPE_NUTTALL = 0x0,
            BB_RBW_SHAPE_FLATTOP = 0x1,
            BB_RBW_SHAPE_CISPR = 0x2,
        }
        public enum BB_Rejection : uint
        {
            BB_NO_SPUR_REJECT = 0x0,
            BB_SPUR_REJECT = 0x1,
        }
        public enum BB_Window : uint
        {
            BB_NUTALL = 0x0,
            BB_BLACKMAN = 0x1,
            BB_HAMMING = 0x2,
            BB_FLAT_TOP = 0x3,
        }
        public enum BB_Units : uint
        {
            BB_LOG = 0x0,
            BB_VOLTAGE = 0x1,
            BB_POWER = 0x2,
            BB_SAMPLE = 0x3,
        }
        public enum BB_DownSampleFactor : int
        {
            BB_MIN_DECIMATION = 1,
            BB_MAX_DECIMATION = 128,
        }
        public enum BB_Mode : uint
        {
            BB_SWEEPING = 0x0,
            BB_REAL_TIME = 0x1,
            BB_STREAMING = 0x4,
            BB_AUDIO_DEMOD = 0x7,
        }
        public enum BB_Flag : uint
        {
            BB_STREAM_IQ = 0x0,
            BB_STREAM_IF = 0x1,
        }
        public enum BB_Port1 : uint
        {
            BB_AC_COUPLED = 0x00,
            BB_DC_COUPLED = 0x04,
            BB_INT_REF_OUT = 0x00,
            BB_EXT_REF_IN = 0x08,
            BB_OUT_AC_LOAD = 0x10,
            BB_OUT_LOGIC_LOW = 0x14,
            BB_OUT_LOGIC_HIGH = 0x1C,
        }
        public enum BB_Port2 : uint
        {
            BB_OUT_LOGIC_LOW = 0x00,
            BB_OUT_LOGIC_HIGH = 0x20,
            BB_IN_TRIGGER_RISING_EDGE = 0x40,
            BB_IN_TRIGGER_FALLING_EDGE = 0x60,
        }
        public enum bbStatus
        {
            // Configuration Errors
            bbInvalidModeErr = -112,
            bbReferenceLevelErr = -111,
            bbInvalidVideoUnitsErr = -110,
            bbInvalidWindowErr = -109,
            bbInvalidBandwidthTypeErr = -108,
            bbInvalidSweepTimeErr = -107,
            bbBandwidthErr = -106,
            bbInvalidGainErr = -105,
            bbAttenuationErr = -104,
            bbFrequencyRangeErr = -103,
            bbInvalidSpanErr = -102,
            bbInvalidScaleErr = -101,
            bbInvalidDetectorErr = -100,

            // General Errors
            bbUSBTimeoutErr = -15,
            bbDeviceConnectionErr = -14,
            bbPacketFramingErr = -13,
            bbGPSErr = -12,
            bbGainNotSetErr = -11,
            bbDeviceNotIdleErr = -10,
            bbDeviceInvalidErr = -9,
            bbBufferTooSmallErr = -8,
            bbNullPtrErr = -7,
            bbAllocationLimitErr = -6,
            bbDeviceAlreadyStreamingErr = -5,
            bbInvalidParameterErr = -4,
            bbDeviceNotConfiguredErr = -3,
            bbDeviceNotStreamingErr = -2,
            bbDeviceNotOpenErr = -1,

            // No Error
            bbNoError = 0,

            // Warnings/Messages
            bbAdjustedParameter = 1,
            bbADCOverflow = 2,
            bbNoTriggerFound = 3,
            bbClampedToUpperLimit = 4,
            bbClampedToLowerLimit = 5,
            bbUncalibratedDevice = 6
        };
        #endregion Enums

        [DllImport("bb_api.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern bbStatus bbOpenDevice(ref int device);

        [DllImport("bb_api.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern bbStatus bbCloseDevice(int device);

        [DllImport("bb_api.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern bbStatus bbConfigureAcquisition(int device,
            uint detector, uint scale);

        [DllImport("bb_api.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern bbStatus bbConfigureCenterSpan(int device,
            double center, double span);

        [DllImport("bb_api.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern bbStatus bbConfigureLevel(int device,
            double ref_level, double atten);

        [DllImport("bb_api.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern bbStatus bbConfigureGain(int device, int gain);

        [DllImport("bb_api.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern bbStatus bbConfigureSweepCoupling(int device,
            double rbw, double vbw, double sweepTime, uint rbw_type, uint rejection);

        [DllImport("bb_api.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern bbStatus bbConfigureRealTime(int device,
            double frameScale, int frameRate);

        [DllImport("bb_api.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern bbStatus bbConfigureWindow(int device, uint window);

        [DllImport("bb_api.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern bbStatus bbConfigureProcUnits(int device, uint units);

        [DllImport("bb_api.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern bbStatus bbConfigureIO(int device,
            uint port1, uint port2);

        [DllImport("bb_api.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern bbStatus bbConfigureDemod(int device,
            int mod_type, double freq, float if_bandwidth, float low_pass_freq,
            float high_pass_freq, float fm_deemphasis);

        [DllImport("bb_api.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern bbStatus bbConfigureIQ(int device,
            int downsampleFactor, double bandwidth);

        [DllImport("bb_api.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern bbStatus bbInitiate(int device,
            uint mode, uint flag);

        [DllImport("bb_api", CallingConvention = CallingConvention.Cdecl)]
        public static extern bbStatus bbFetchTrace_32f(int device,
            int arraySize, float[] min, float[] max);

        [DllImport("bb_api.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern bbStatus bbFetchTrace(int device,
            int array_size, double[] min, double[] max);

        [DllImport("bb_api.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern bbStatus bbFetchAudio(int device, ref float audio);

        [DllImport("bb_api.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern bbStatus bbFetchRaw(int device,
            float[] buffer, int[] triggers);

        [DllImport("bb_api.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern bbStatus bbFetchRealTimeFrame(int device,
            float[] sweep, float[] frame, object magicAHAHAHA_null);

        [DllImport("bb_api.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern bbStatus bbQueryTraceInfo(int device,
            ref uint trace_len, ref double bin_size, ref double start);

        [DllImport("bb_api.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern bbStatus bbQueryRealTimeInfo(int device,
            ref int frameWidth, ref int frameHeight);

        [DllImport("bb_api.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern bbStatus bbQueryStreamInfo(int device,
            ref int return_len, ref double bandwidth, ref int samples_per_sec);

        [DllImport("bb_api.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern bbStatus bbQueryTimestamp(int device,
            ref uint seconds, ref uint nanoseconds);

        [DllImport("bb_api.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern bbStatus bbAbort(int device);

        [DllImport("bb_api.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern bbStatus bbPreset(int device);

        [DllImport("bb_api.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern bbStatus bbSelfCal(int device);

        [DllImport("bb_api.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern bbStatus bbSyncCPUtoGPS(int com_port, int baud_rate);

        [DllImport("bb_api.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern bbStatus bbGetDeviceType(int device, ref int type);

        [DllImport("bb_api.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern bbStatus bbGetSerialNumber(int device, ref uint serial_number);

        [DllImport("bb_api.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern bbStatus bbGetFirmwareVersion(int device, ref int version);

        [DllImport("bb_api.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern bbStatus bbGetDeviceDiagnostics(int device,
            ref float temperature, ref float usbVoltage, ref float usbCurrent);

        public static string bbGetDeviceName(int device)
        {
            int device_type = -1;
            bbGetDeviceType(device, ref device_type);
            if (device_type == (int)BB_Device.BB_BB60A)
                return "BB60A";
            if (device_type == (int)BB_Device.BB_BB60C)
                return "BB60C";

            return "Unknown device";
        }

        public static string bbGetSerialString(int device)
        {
            uint serial_number = 0;
            if (bbGetSerialNumber(device, ref serial_number) == bbStatus.bbNoError)
                return serial_number.ToString();

            return "";
        }

        public static string bbGetFirmwareString(int device)
        {
            int firmware_version = 0;
            if (bbGetFirmwareVersion(device, ref firmware_version) == bbStatus.bbNoError)
                return firmware_version.ToString();

            return "";
        }

        public static string bbGetAPIString()
        {
            IntPtr str_ptr = bbGetAPIVersion();
            return System.Runtime.InteropServices.Marshal.PtrToStringAnsi(str_ptr);
        }

        public static string bbGetStatusString(bbStatus status)
        {
            IntPtr str_ptr = bbGetErrorString(status);
            return System.Runtime.InteropServices.Marshal.PtrToStringAnsi(str_ptr);
        }

        // Call get_string variants above instead
        [DllImport("bb_api.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr bbGetAPIVersion();
        [DllImport("bb_api.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr bbGetErrorString(bbStatus status);




        #endregion
        // Событие, которое нужно вызывать при изменении
        public event PropertyChangedEventHandler PropertyChanged;
        // Для удобства обернем событие в метод с единственным параметром - имя изменяемого свойства
        public void OnPropertyChanged(string propertyName)
        {
            // Если кто-то на него подписан, то вызывем его
            GUIThreadDispatcher.Instance.Invoke(() =>
            {
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            });
        }

        private class GainATT
        {
            public int GainIndex = 1;
            public int AttIndex = 0;

            public int PreviousGainIndex = 0;
            public int PreviousAttIndex = 0;

            public Gain[] gainatt = new Gain[4]
            {
                new Gain() { gain = BB_Gain.BB_Gain_0},
                new Gain() { gain = BB_Gain.BB_Gain_1},
                new Gain() { gain = BB_Gain.BB_Gain_2},
                new Gain() { gain = BB_Gain.BB_Gain_3},
            };
            public class Gain
            {
                public BB_Gain gain;
                public Att[] att = new Att[4]
                {
                    new Att() { att = BB_Atten.BB_Atten_0, CentrLevel=0, LeftLevel = 0, RightLevel = 0 },
                    new Att() { att = BB_Atten.BB_Atten_10, CentrLevel=0, LeftLevel = 0, RightLevel = 0  },
                    new Att() { att = BB_Atten.BB_Atten_20, CentrLevel=0, LeftLevel = 0, RightLevel = 0  },
                    new Att() { att = BB_Atten.BB_Atten_30, CentrLevel=0, LeftLevel = 0, RightLevel = 0  }
                };
                public class Att
                {
                    public bool isoverload = false;
                    public BB_Atten att;
                    public double CentrLevel = 0;
                    public double LeftLevel = 0;
                    public double RightLevel = 0;
                }
            }

        }







    }
}