using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControlU.Equipment
{
    public class SomeMeasData : PropertyChangedBase
    {
        public ObservableCollection<DataSomeMeas> DataSomeMeas
        {
            get { return _DataSomeMeas; }
            set { _DataSomeMeas = value; OnPropertyChanged("DataSomeMeas"); }
        }
        private ObservableCollection<DataSomeMeas> _DataSomeMeas = new ObservableCollection<DataSomeMeas>()
        {
           // new DataSomeMeas() { Trace = new TracePoint[] { } }
        };
    }
    public class DataSomeMeas : PropertyChangedBase
    {
        public bool State
        {
            get { return _State; }
            set { _State = value; OnPropertyChanged("State"); }
        }
        private bool _State = true;

        public bool IsMeas
        {
            get { return _IsMeas; }
            set { _IsMeas = value; OnPropertyChanged("IsMeas"); }
        }
        private bool _IsMeas = false;

        /// <summary>
        /// 0 = SA
        /// 1 = R&S Receiver
        /// 2 = R&S TSMx
        /// 3 = Signal Hound
        /// </summary>
        public int DeviceType
        {
            get { return _DeviceType; }
            set { _DeviceType = value; OnPropertyChanged("DeviceType"); }
        }
        private int _DeviceType = 0;

        /// <summary>
        /// 0 = Centr/Span
        /// 1 = Start/Stop
        /// 2 = GSM
        /// </summary>
        public int FreqType
        {
            get { return _FreqType; }
            set { _FreqType = value; OnPropertyChanged("FreqType"); }
        }
        private int _FreqType = 2;

        public int Channel
        {
            get { return _Channel; }
            set { _Channel = value; OnPropertyChanged("Channel"); }
        }
        private int _Channel = 0;

        #region Freq        
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
        private decimal _FreqCentr = 100000000;//2142400000;

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
        private decimal _FreqSpan = 10000000;

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
        private decimal _FreqStart = 95000000;//2139900000;//1800000000;//2600000000;//2490000000;//

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
        private decimal _FreqStop = 105000000;
        #endregion Freq

        #region RBW / VBW
        public decimal RBW
        {
            get { return _RBW; }
            set { _RBW = value; OnPropertyChanged("RBW"); }
        }
        private decimal _RBW = 10000;
        public int RBWIndex = 7;

        public decimal VBW
        {
            get { return _VBW; }
            set { _VBW = value; OnPropertyChanged("VBW"); }
        }
        private decimal _VBW = 10000;
        public int VBWIndex = 7;
        #endregion RBW / VBW

        #region Sweep
        public decimal SweepTime
        {
            get { return _SweepTime; }
            set { _SweepTime = value; OnPropertyChanged("SweepTime"); }
        }
        private decimal _SweepTime = 0.00001m;
        #endregion

        public decimal RefLevel
        {
            get { return _RefLevel; }
            set { _RefLevel = value; OnPropertyChanged("RefLevel"); }
        }
        private decimal _RefLevel = -40;

        public decimal Range
        {
            get { return _Range; }
            set { _Range = value; OnPropertyChanged("Range"); }
        }
        private decimal _Range = 100;

        public Equipment.LevelUnit LevelUnit
        {
            get { return _LevelUnit; }
            set { _LevelUnit = value; OnPropertyChanged("LevelUnit"); }
        }
        private Equipment.LevelUnit _LevelUnit = new Equipment.AllLevelUnits().dBm;

        /// <summary>
        /// -1 = Auto (если есть)
        /// 0 = OFF (выключен)
        /// 1 = 10 or ON
        /// 2 = 20
        /// 3 = 30
        /// </summary>
        public decimal GainIndex
        {
            get { return _GainIndex; }
            set { _GainIndex = value; OnPropertyChanged("GainIndex"); }
        }
        private decimal _GainIndex = 0;

        /// <summary>
        /// -1 = Auto (если есть)
        /// 0 = OFF (выключен)
        /// 1 = AN/RSR = 1db SH = 10db
        /// 2 = AN/RSR = 2db SH = 20db
        /// 3 = AN/RSR = 3db SH = 30db
        /// </summary>
        public decimal AttIndex
        {
            get { return _AttIndex; }
            set { _AttIndex = value; OnPropertyChanged("AttIndex"); }
        }
        private decimal _AttIndex = 0;

        #region Trace Data
        public Equipment.tracepoint[] Trace
        {
            get { return _Trace; }
            set { _Trace = value; OnPropertyChanged("Trace"); }
        }
        private Equipment.tracepoint[] _Trace = new tracepoint[] { new tracepoint() {freq = 0, level = 0 } };

        public AverageList TraceAveragedList
        {
            get { return _TraceAveragedList; }
            set { _TraceAveragedList = value; OnPropertyChanged("TraceAveragedList"); }
        }
        public AverageList _TraceAveragedList = new AverageList();

        public TrackingList TraceTrackedList
        {
            get { return _TraceTrackedList; }
            set { _TraceTrackedList = value; OnPropertyChanged("TraceTrackedList"); }
        }
        public TrackingList _TraceTrackedList = new TrackingList();
        #endregion Trace Data

        public int TraceTypeIndex
        {
            get { return _TraceTypeIndex; }
            set { _TraceTypeIndex = value; OnPropertyChanged("TraceTypeIndex"); }
        }
        private int _TraceTypeIndex = 0;


        public decimal StayOnFrequency
        {
            get { return _StayOnFrequency; }
            set { _StayOnFrequency = value; OnPropertyChanged("StayOnFrequency"); }
        }
        private decimal _StayOnFrequency = 0.001m;

        public decimal ThisStayOnFrequency
        {
            get { return _ThisStayOnFrequency; }
            set { _ThisStayOnFrequency = value; OnPropertyChanged("ThisStayOnFrequency"); }
        }
        private decimal _ThisStayOnFrequency = 0;


        public bool StayOnSignalState
        {
            get { return _StayOnSignalState; }
            set { _StayOnSignalState = value; OnPropertyChanged("StayOnSignalState"); }
        }
        private bool _StayOnSignalState = false;

        public int StayOnSignalType
        {
            get { return _StayOnSignalType; }
            set { _StayOnSignalType = value; OnPropertyChanged("StayOnSignalType"); }
        }
        private int _StayOnSignalType = 0;

        public decimal StayOnSignal
        {
            get { return _StayOnSignal; }
            set { _StayOnSignal = value; OnPropertyChanged("StayOnSignal"); }
        }
        private decimal _StayOnSignal = 1;

        public decimal ThisStayOnSignal
        {
            get { return _ThisStayOnSignal; }
            set { _ThisStayOnSignal = value; OnPropertyChanged("ThisStayOnSignal"); }
        }
        private decimal _ThisStayOnSignal = 0;

    }
}
