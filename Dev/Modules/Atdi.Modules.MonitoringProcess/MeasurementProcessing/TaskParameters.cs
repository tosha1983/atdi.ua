using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.AppServer.Contracts.Sdrns;

namespace Atdi.SDR.Server.MeasurementProcessing
{
    public class TaskParameters
    {
        #region parameters
        public int TaskId;
        public MeasurementType MeasurementType;
        public double MinFreq_MHz;
        public double MaxFreq_MHz;
        public double RBW_Hz;
        public double VBW_Hz;
        public double StepSO_kHz; //- шаг сетки частот для измерения SO
        public int NChenal; //количество интервалов на которое делиться канал при измерении SO
        public double LevelMinOccup_dBm; //минимальный уровень для расчета занятости канала SO
        public Atdi.AppServer.Contracts.Sdrns.SpectrumOccupationType Type_of_SO; // Тип измеряемой занятости канала
        public List<Double> List_freq_CH = new List<Double>();//MHz // перечень частот для измерения SO
        public string status;
        #endregion
        #region metods
        public TaskParameters(object TaskSDR)
        {
            if (TaskSDR is MeasSdrTask)
            {
                FillingTaskParameters_v1(TaskSDR as MeasSdrTask);
            }

        }
        private void FillingTaskParameters_v1(MeasSdrTask taskSDR)
        {
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
            if (taskSDR.MeasSDRParam.RBW != 0) { RBW_Hz = taskSDR.MeasSDRParam.RBW * 1000; } else { RBW_Hz = 10000; }
            if (taskSDR.MeasSDRParam.VBW != 0) { VBW_Hz = taskSDR.MeasSDRParam.VBW * 1000; } else { VBW_Hz = 10000; }
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
  
            TaskId = taskSDR.Id;
            MeasurementType = taskSDR.MeasDataType;
            status = taskSDR.status;
        }
        #endregion
    }
}
