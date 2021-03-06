﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrns.Device;
using Atdi.DataModels.Sdrns;

namespace Atdi.DataModels.Sdrn.DeviceServer.Processing
{
    [Serializable]
    public class TaskParameters
    {
        public string SDRTaskId { get; set; }
        public MeasType MeasurementType { get; set; }
        public double MinFreq_MHz { get; set; }
        public double MaxFreq_MHz { get; set; }
        public double? RBW_Hz { get; set; }
        public double? VBW_Hz { get; set; }
        public double SweepTime_s { get; set; }
        public double StepSO_kHz { get; set; } //- шаг сетки частот для измерения SO
        public int NChenal { get; set; } //количество интервалов на которое делиться канал при измерении SO, Signalization
        public double LevelMinOccup_dBm { get; set; } //минимальный уровень для расчета занятости канала SO
        public bool SupportMultyLevel { get; set; } 
        public SOType TypeOfSO { get; set; } // Тип измеряемой занятости канала
        public List<Double> ListFreqCH { get; set; } //MHz // перечень частот для измерения SO
        public string status { get; set; }
        public double ReceivedIQStreemDuration_sec { get; set; }
        public TypeTechnology TypeTechnology { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? StopTime { get; set; }
        public int NCount = 0; //количество сканированиия за измерение (он вичисляется в проессе трансформации из MeasTask)
        public ReferenceSituation RefSituation { get; set; }
        public int SensorId { get; set; }
        public SignalingMeasTask SignalingMeasTaskParameters { get; set; }
        public double PercentForCalcNoise { get; set; }
        public bool Smooth { get; set; } // требования по усреднению спектра при проведении оценки BandWidth и др.
        public List<double> ChCentrFreqs_Mhz { get; set; } 
        public double BWChalnel_kHz { get; set; }
        public double? RefLevel_dBm { get; set; }// 1000000000 = auto
        public DetectingType DetectType { get; set; }
        public int? Preamplification_dB { get; set; }//-1 = auto, 
        public int? RfAttenuation_dB { get; set; }//-1 = auto, 
    }
}
