using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.AppServer.Contracts.Sdrns;

namespace Atdi.SDR.Server.MeasurementProcessing
{
    public class SDRParameters
    {
        #region parameters
        public int TaskId;
        public bool filled;
        public MeasurementType MeasurementType;
        public TraceType traceType; 
        public Atdi.AppServer.Contracts.Sdrns.DetectingType DetectTypeSDR;
        public double MinFreq_MHz;
        public double MaxFreq_MHz;
        public Atdi.AppServer.Contracts.Sdrns.SpectrumScanType TypeSpectrumScan; // тип сканирования RealTime или Sweep
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
        public Atdi.AppServer.Contracts.Sdrns.SpectrumOccupationType Type_of_SO; // Тип измеряемой занятости канала
        public List<Double> List_freq_CH = new List<Double>();//MHz // перечень частот для измерения SO
        #endregion
        #region metods
        public SDRParameters(object TaskSDR)
        {
            filled = false;
            if (TaskSDR is MeasSdrTask)
            {
                Filling_parameters_v1(TaskSDR as MeasSdrTask);
                SetTraceType();
            }
        }
        private void Filling_parameters_v1(MeasSdrTask taskSDR)
        {
            DetectTypeSDR = taskSDR.MeasSDRParam.DetectTypeSDR;
            if (taskSDR.MeasFreqParam.MeasFreqs != null)
            {
                List_freq_CH.Clear();
                for (int i = 0; i < taskSDR.MeasFreqParam.MeasFreqs.Length; i++)
                { double freq = taskSDR.MeasFreqParam.MeasFreqs[i].Freq; List_freq_CH.Add(freq); }
            }
            if ((taskSDR.MeasFreqParam.RgL == null) || (taskSDR.MeasFreqParam.RgU == null)) // если вдруг отсутвуют начало и конец
            {
                if (List_freq_CH != null)
                {
                    if (List_freq_CH.Count > 0)
                    {
                        List_freq_CH.Sort();
                        MinFreq_MHz = List_freq_CH[0];
                        MaxFreq_MHz = List_freq_CH[List_freq_CH.Count - 1];
                    }
                }
                else { MinFreq_MHz = 100; MaxFreq_MHz = 110; }
            }
            else
            {
                MaxFreq_MHz = taskSDR.MeasFreqParam.RgU.Value;
                MinFreq_MHz = taskSDR.MeasFreqParam.RgL.Value;
            }
            if (taskSDR.TypeM == Atdi.AppServer.Contracts.Sdrns.SpectrumScanType.RealTime) { TypeSpectrumScan = taskSDR.TypeM; } else { TypeSpectrumScan = Atdi.AppServer.Contracts.Sdrns.SpectrumScanType.Sweep; }
            if (taskSDR.MeasSDRParam.ref_level_dbm != 0) { RefLevel_dBm = taskSDR.MeasSDRParam.ref_level_dbm; } else { RefLevel_dBm = -30; }
            RfAttenuationSDR = taskSDR.MeasSDRParam.RfAttenuationSDR;
            PreamplificationSDR = taskSDR.MeasSDRParam.PreamplificationSDR;
            if (taskSDR.MeasSDRParam.RBW != 0) { RBW_Hz = taskSDR.MeasSDRParam.RBW * 1000; } else { RBW_Hz = 10000; }
            if (taskSDR.MeasSDRParam.VBW != 0) { VBW_Hz = taskSDR.MeasSDRParam.VBW * 1000; } else { VBW_Hz = 10000; }
            if (taskSDR.MeasSDRParam.MeasTime != 0) { MeasTime_Sec = taskSDR.MeasSDRParam.MeasTime; } else { MeasTime_Sec = 0.001; }
            if (taskSDR.SwNumber != 0) { SwNumber = taskSDR.SwNumber; } else { SwNumber = 10; }
            if (taskSDR.MeasFreqParam.Step != null) { StepSO_kHz = taskSDR.MeasFreqParam.Step.Value; } // обязательный параметер для SO (типа ширина канала или шаг сетки частот)
            // присвоение параметров для измерения SO
            if (taskSDR.MeasDataType == Atdi.AppServer.Contracts.Sdrns.MeasurementType.SpectrumOccupation)
            {
                if ((taskSDR.MeasSDRSOParam.TypeSO == Atdi.AppServer.Contracts.Sdrns.SpectrumOccupationType.FreqBandwidthOccupation) || (taskSDR.MeasSDRSOParam.TypeSO == Atdi.AppServer.Contracts.Sdrns.SpectrumOccupationType.FreqChannelOccupation))
                {
                    if ((taskSDR.MeasSDRSOParam.NChenal > 0) && (taskSDR.MeasSDRSOParam.NChenal < 1000)) { NChenal = taskSDR.MeasSDRSOParam.NChenal; } else { NChenal = 10; }
                    if (taskSDR.MeasSDRSOParam.LevelMinOccup <= 0) { LevelMinOccup_dBm = taskSDR.MeasSDRSOParam.LevelMinOccup; } else { LevelMinOccup_dBm = -80; }
                    Type_of_SO = taskSDR.MeasSDRSOParam.TypeSO;
                    // формируем начало и конец для измерений 
                    List_freq_CH.Sort();
                    MinFreq_MHz = List_freq_CH[0] - StepSO_kHz / 2000;
                    MaxFreq_MHz = List_freq_CH[List_freq_CH.Count - 1] + StepSO_kHz / 2000;
                    // расчитываем желаемое RBW и VBW
                    VBW_Hz = StepSO_kHz * 1000 / NChenal;
                    RBW_Hz = StepSO_kHz * 1000 / NChenal;
                }
            }
            // коректировка режима измерения 
            if ((TypeSpectrumScan == Atdi.AppServer.Contracts.Sdrns.SpectrumScanType.RealTime) && ((MaxFreq_MHz - MinFreq_MHz > 20) || ((MaxFreq_MHz - MinFreq_MHz) * 1000000 / RBW_Hz > 8000))) { TypeSpectrumScan = Atdi.AppServer.Contracts.Sdrns.SpectrumScanType.Sweep; }
            TaskId = taskSDR.Id;
            MeasurementType = taskSDR.MeasDataType;
            filled = true;
        }
        private void SetTraceType()
        {
            traceType = TraceType.Average;
            if (MeasurementType == MeasurementType.SpectrumOccupation) { traceType = TraceType.Average;}
            if (MeasurementType == MeasurementType.BandwidthMeas) { traceType = TraceType.MaxHold;}
        }
        #endregion
    }
}

