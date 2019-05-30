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
        public string InstrModel { get; set; }
        public string SerialNumber { get; set; }
        public bool HiSpeed { get; set; }

        public decimal FreqMin { get; set; }
        public decimal FreqMax { get; set; }
        public List<DeviceOption> InstrOption { get; set; }
        public List<DeviceOption> DefaultInstrOption { get; set; }
        public List<DeviceOption> LoadedInstrOption { get; set; }
        public int NumberOfTrace { get; set; } = 0;
        public List<ParamWithId> TraceType { get; set; }
        public List<ParamWithId> TraceDetector { get; set; }
        public bool ChangeableSweepType { get; set; } = false;
        public List<ParamWithId> SweepType { get; set; }
        public bool SweepPointFix { get; set; } = false;
        public int[] SweepPointArr { get; set; }
        public decimal SWTMin { get; set; }
        public decimal SWTMax { get; set; }
        public int DefaultSweepPoint { get; set; }
        public decimal[] RBWArr { get; set; }
        public decimal[] VBWArr { get; set; }
        public bool CouplingRatio { get; set; } = false;
        public List<ParamWithId> LevelUnits { get; set; } = new List<ParamWithId>() { };

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
        public bool RangeFixed { get; set; }
        public decimal RefLevelMin { get; set; }
        public decimal RefLevelMax { get; set; }
        public decimal RefLevelStep { get; set; }

        public bool IQAvailable { get; set; }
        public decimal IQMaxSampleSpeed { get; set; }
        public decimal IQMinSampleSpeed { get; set; }
        public int IQMaxSampleLength { get; set; }
        public decimal TriggerOffsetMax { get; set; }
        public bool OptimizationAvailable { get; set; }
        public List<ParamWithId> Optimization { get; set; }          
    }
}
