using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.DeviceServer.Adapters
{
    public class LocalRSReceiverInfo
    {
        public int InstrManufacrure { get; set; }
        public string InstrModel { get; set; }
        public string SerialNumber { get; set; }
        public List<DeviceOption> InstrOption { get; set; }
        public List<DeviceOption> LoadedInstrOption { get; set; }
        public List<Mode> Modes;
        
        private bool _DemodFreq = false;
        /// <summary>
        /// True = отличная от центральной частота демодуляции
        /// </summary>
        public bool DemodFreq
        {
            get { return _DemodFreq; }
            set { _DemodFreq = value; }
        }
        public string[] Demod { get; set; }
        public decimal[] DemodBW { get; set; }
        public decimal[] FFMStepBW { get; set; }
        public decimal[] DFBW { get; set; }
        public decimal[] FFMSpanBW { get; set; }
        public decimal[] PSCANStepBW { get; set; }
        public List<ParamWithUI> FFTModes { get; set; }
        public List<ParamWithUI> DFSQUModes { get; set; }

        /// <summary>
        /// Фиксированная селективность у прибора если True то неразрешать менять ибо безтолку
        /// </summary>
        public bool SelectivityChangeable { get; set; }
        public List<ParamWithUI> SelectivityModes { get; set; }
        public List<ParamWithUI> Detectors { get; set; }
        public bool RFModeChangeable { get; set; }
        public List<ParamWithUI> RFModes { get; set; }
        public bool ATTFix { get; set; }
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
        
        public class Mode
        {
            public string ModeName { get; set; }
            public string MeasAppl { get; set; }
            public string FreqMode { get; set; }
        }
    }
}
