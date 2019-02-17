using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.DeviceServer.Adapters
{
    public class LocalSpectrumAnalyzerInfo
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

        public List<DeviceOption> InstrOption { get; set; }
        public List<DeviceOption> DefaultInstrOption { get; set; }
        public List<DeviceOption> LoadedInstrOption { get; set; }
        private int _NumberOfTrace = 0;
        public int NumberOfTrace
        {
            get { return _NumberOfTrace; }
            set { _NumberOfTrace = value; }
        }
        public List<ParamWithId> TraceType { get; set; }
        public List<ParamWithId> TraceDetector { get; set; }
        private bool _ChangeableSweepType = false;
        public bool ChangeableSweepType
        {
            get { return _ChangeableSweepType; }
            set { _ChangeableSweepType = value; }
        }
        public List<ParamWithId> SweepType { get; set; }
        private bool _SweepPointFix = false;
        public bool SweepPointFix
        {
            get { return _SweepPointFix; }
            set { _SweepPointFix = value; }
        }
        public int[] SweepPointArr { get; set; }
        public int DefaultSweepPoint { get; set; }
        public decimal[] RBWArr { get; set; }
        public decimal[] VBWArr { get; set; }
        private bool _CouplingRatio = false;
        public bool CouplingRatio
        {
            get { return _CouplingRatio; }
            set { _CouplingRatio = value; }
        }
        public List<ParamWithId> LevelUnits
        {
            get { return _LevelUnits; }
            set { _LevelUnits = value; }
        }
        private List<ParamWithId> _LevelUnits = new List<ParamWithId>() { };

        private bool _PreAmp = false;
        public bool PreAmp
        {
            get { return _PreAmp; }
            set { _PreAmp = value; }
        }
        public decimal AttMax { get; set; }
        public decimal AttStep { get; set; }
        public decimal[] RangeArr { get; set; }

        public bool Battery { get; set; }
        public bool NdB { get; set; }
        public bool OBW { get; set; }
        public bool ChnPow { get; set; }
        public bool RangeFixed { get; set; }

        //private List<Transducer> _Transducers;
        //public List<Transducer> Transducers
        //{
        //    get { return _Transducers; }
        //    set { _Transducers = value; OnPropertyChanged("Transducers"); }
        //}       
    }
}
