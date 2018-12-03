using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using Atdi.AppServer.Contracts.Sdrns;

namespace Atdi.Modules.MonitoringProcess
{
    public class SDRParameters
    {
        #region parameters
        public int TaskId;
        public bool filled;
        public MeasType MeasurementType;
        public TraceType traceType; 
        public DetectType DetectTypeSDR;
        public double MinFreq_MHz;
        public double MaxFreq_MHz;
        public SpectrScanType TypeSpectrumScan; // тип сканирования RealTime или Sweep
        public double RefLevel_dBm;
        public double RfAttenuationSDR;
        public double PreamplificationSDR;
        public double RBW_Hz;
        public double VBW_Hz;
        public double MeasTime_Sec;
        public int SwNumber;// количество проходов спектра осуществляемое при одном измерении
        public double StepSO_kHz; //- шаг сетки частот для измерения SO
        public int NChenal; //количество интервалов на которое делиться канал при измерении SO
        public double LevelMinOccup_dBm; //минимальный уровень для расчета занятости канала
        public SOType Type_of_SO; // Тип измеряемой занятости канала
        public List<Double> List_freq_CH = new List<Double>();//MHz // перечень частот для измерения SO
        #endregion
    }
}

