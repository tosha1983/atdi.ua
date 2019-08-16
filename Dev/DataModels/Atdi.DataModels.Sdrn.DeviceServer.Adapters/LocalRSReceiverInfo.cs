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
        public bool EB200Protocol { get; set; }
        public bool XMLProtocol { get; set; }
        public List<DeviceOption> InstrOption { get; set; }
        public List<DeviceOption> LoadedInstrOption { get; set; }
        public List<Mode> Modes;

        public decimal FreqMin { get; set; }
        public decimal FreqMax { get; set; }

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
        public decimal[] AllStepBW { get; set; }
        
        public decimal[] DFBW { get; set; }
        public FFMSpanBW[] FFMSpan { get; set; }
        public decimal[] PSCANStepBW { get; set; }
        public List<ParamWithId> FFTModes { get; set; }
        public List<ParamWithId> DFSQUModes { get; set; }

        /// <summary>
        /// Фиксированная селективность у прибора если True то неразрешать менять ибо безтолку
        /// </summary>
        public bool SelectivityChangeable { get; set; }
        public List<ParamWithId> SelectivityModes { get; set; }
        public List<ParamWithId> Detectors { get; set; }
        public bool RFModeChangeable { get; set; }
        public List<ParamWithId> RFModes { get; set; }
        public bool ATTFix { get; set; }
        public int AttStep { get; set; }
        public int AttMax { get; set; }
        public decimal DFSQUMAX { get; set; }
        public decimal DFSQUMIN { get; set; }
        public decimal MeasTimeMAX { get; set; }
        public decimal MeasTimeMIN { get; set; }
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
        public class FFMSpanBW
        {
            public decimal BW { get; set; }
            public decimal[] AvailableStepBW { get; set; }
        }
    }
}
