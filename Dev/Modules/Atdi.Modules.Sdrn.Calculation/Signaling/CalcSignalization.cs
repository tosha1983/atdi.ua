using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Common;


namespace Atdi.Modules.Sdrn.Calculation
{
    public static class CalcSignalization
    {
        private static int StartLevelsForLevelDistribution = -150;
        private static int NumberPointForLevelDistribution = 200;

        public static Spectrum CreateSpectrum(float[] templevels, double stepBW_kHz, double startFreq_MHz, double NoiseLevel_dBm, double DiffLevelForCalcBW = 25, double windowBW = 1.5,
           double nDbLevel_dB = 15, int NumberIgnoredPoints = 1, double MinExcessNoseLevel_dB = 5)
        {
            int? prmT1;
            int? prmT2;
            int? prmMarkerIndex;
            double? prmBandwidthkHz;
            bool? prmСorrectnessEstimations;

            BandWidthEstimation.GetBandwidthPoint(out prmT1, out prmT2, out prmMarkerIndex,  out prmBandwidthkHz,  out prmСorrectnessEstimations, templevels, BandWidthEstimation.BandwidthEstimationType.xFromCentr, DiffLevelForCalcBW, 0);
            var Spectrum = new Spectrum();
            int start = 0; int stop = templevels.Length;
            if (prmСorrectnessEstimations != null)
            {
                if (prmСorrectnessEstimations.Value)
                {
                    // значит спектр хороший можно брать его параметры
                    if (prmT1 != null) { start = prmT1.Value; Spectrum.T1 = prmT1.Value; }
                    if (prmT2 != null) { stop = prmT2.Value; Spectrum.T2 = prmT2.Value; }
                    if (prmMarkerIndex != null) { Spectrum.MarkerIndex = prmMarkerIndex.Value; }
                    Spectrum.Bandwidth_kHz = (stop - start) * stepBW_kHz;
                    Spectrum.SpectrumStartFreq_MHz = startFreq_MHz;
                    Spectrum.SpectrumSteps_kHz = stepBW_kHz;
                    Spectrum.Levels_dBm = templevels;
                    Spectrum.СorrectnessEstimations = true;
                }
                else
                {// попробуем пройтись по масиву с помощью метода nDbDown и всеже определить ширину спектра
                    int startreal = -1;
                    int stoptreal = -1;
                    bool CorectCalcBW = CalcBWSignalization.CalcBW(templevels, templevels.Length, start, stop, nDbLevel_dB, NoiseLevel_dBm, MinExcessNoseLevel_dB, NumberIgnoredPoints, ref startreal, ref stoptreal);
                    if (CorectCalcBW)
                    {
                        // необходимо найти корректное значение полосы частот
                        double tempBW = StandartBW.GetStandartBW_kHz((stoptreal - startreal) * stepBW_kHz);
                        double CentralFreq = startFreq_MHz + stepBW_kHz * (startreal + stoptreal) / 2000;
                        double StartFrequency_MHz = CentralFreq - tempBW / 2000;
                        double StopFrequency_MHz = CentralFreq + tempBW / 2000;
                        start = (int)Math.Floor((StartFrequency_MHz - startFreq_MHz) / (stepBW_kHz / 1000));
                        if (start < 0) { start = 0; }
                        stop = (int)Math.Ceiling((StopFrequency_MHz - startFreq_MHz) / (stepBW_kHz / 1000));
                        if (stop >= templevels.Length) { stop = templevels.Length - 1; }
                        Spectrum.Levels_dBm = new float[stop - start];
                        Array.Copy(templevels, start, Spectrum.Levels_dBm, 0, stop - start);
                        Spectrum.SpectrumStartFreq_MHz = startFreq_MHz + stepBW_kHz * start / 1000;
                        Spectrum.SpectrumSteps_kHz = stepBW_kHz;
                        Spectrum.СorrectnessEstimations = true;
                    }
                    else
                    {
                        Spectrum.SpectrumStartFreq_MHz = startFreq_MHz;
                        Spectrum.SpectrumSteps_kHz = stepBW_kHz;
                        Spectrum.Levels_dBm = templevels;
                        Spectrum.СorrectnessEstimations = false;
                    }
                }
            }
            else
            {
                Spectrum.SpectrumStartFreq_MHz = startFreq_MHz;
                Spectrum.SpectrumSteps_kHz = stepBW_kHz;
                Spectrum.Levels_dBm = templevels;
                Spectrum.СorrectnessEstimations = false;
            }
            return Spectrum;
        }
        /// <summary>
        /// Filling WorkTimes, LevelDistribution,
        /// </summary>
        /// <param name="EmittingParameter"></param>
        /// <param name="logger"></param>
        /// <returns></returns>
        public static Emitting FillEmittingForStorage(Emitting EmittingParameter)
        {
            var emitting = EmittingParameter;
            if ((emitting.WorkTimes != null) && (emitting.WorkTimes.Length > 0))
            {
                emitting.WorkTimes[0].HitCount = 1;
                emitting.WorkTimes[0].StartEmitting = DateTime.Now;
                emitting.WorkTimes[0].ScanCount = 1;
                emitting.WorkTimes[0].TempCount = 0;
                emitting.WorkTimes[0].PersentAvailability = 100;
                emitting.WorkTimes[0].StopEmitting = emitting.WorkTimes[0].StartEmitting;
            }
            emitting.LevelsDistribution = new LevelsDistribution();
            emitting.LevelsDistribution.Count = new int[NumberPointForLevelDistribution];
            emitting.LevelsDistribution.Levels = new int[NumberPointForLevelDistribution];
            for (var i = 0; i < NumberPointForLevelDistribution; i++)
            {
                emitting.LevelsDistribution.Levels[i] = StartLevelsForLevelDistribution + i;
                emitting.LevelsDistribution.Count[i] = 0;
            }
            int indexLevel = (int)Math.Floor(emitting.CurentPower_dBm) - emitting.LevelsDistribution.Levels[0];
            if ((indexLevel >= 0) && (indexLevel < emitting.LevelsDistribution.Levels.Length)) { emitting.LevelsDistribution.Count[indexLevel]++; }
            return emitting;
        }
        public static Emitting CreatIndependEmitting(Emitting emitting)
        {
            var emitting1 = new Emitting()
            {
                Id = emitting.Id,
                ReferenceLevels = emitting.ReferenceLevels,
                CurentPower_dBm = emitting.CurentPower_dBm,
                EmittingParameters = emitting.EmittingParameters,
                LevelsDistribution = emitting.LevelsDistribution,
                MeanDeviationFromReference = emitting.MeanDeviationFromReference,
                ReferenceLevel_dBm = emitting.ReferenceLevel_dBm,
                SignalMask = emitting.SignalMask,
                Spectrum = emitting.Spectrum,
                StartFrequency_MHz = emitting.StartFrequency_MHz,
                StopFrequency_MHz = emitting.StopFrequency_MHz,
                TriggerDeviationFromReference = emitting.TriggerDeviationFromReference,
                WorkTimes = emitting.WorkTimes,
                SpectrumIsDetailed = emitting.SpectrumIsDetailed,
                LastDetaileMeas = emitting.LastDetaileMeas
            };
            return emitting1;
        }
    }
}


