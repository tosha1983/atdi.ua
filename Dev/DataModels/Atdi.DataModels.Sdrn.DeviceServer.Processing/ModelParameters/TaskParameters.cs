using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrns.Device;


namespace Atdi.DataModels.Sdrn.DeviceServer.Processing
{
    public class TaskParameters
    {
        public string SDRTaskId { get; set; }
        public MeasType MeasurementType { get; set; }
        public double MinFreq_MHz { get; set; }
        public double MaxFreq_MHz { get; set; }
        public double RBW_Hz { get; set; }
        public double VBW_Hz { get; set; }
        public double SweepTime_s { get; set; }
        public double StepSO_kHz { get; set; } //- шаг сетки частот для измерения SO
        public int NChenal { get; set; } //количество интервалов на которое делиться канал при измерении SO
        public double LevelMinOccup_dBm { get; set; } //минимальный уровень для расчета занятости канала SO
        public SOType TypeOfSO { get; set; } // Тип измеряемой занятости канала
        public List<Double> ListFreqCH { get; set; } //MHz // перечень частот для измерения SO
        public string status { get; set; }
        public double ReceivedIQStreemDuration_sec { get; set; }
        public TypeTechnology TypeTechnology { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? StopTime { get; set; }
        public int NCount = 0; //количество сканированиия за измерение (он вичисляется в проессе трансформации из MeasTask)
        public ReferenceSignal[] ReferenceSignals;
    }
}
