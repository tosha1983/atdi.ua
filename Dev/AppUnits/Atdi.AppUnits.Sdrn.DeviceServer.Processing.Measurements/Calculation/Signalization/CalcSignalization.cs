using Atdi.Contracts.Sdrn.DeviceServer;
using Atdi.DataModels.Sdrn.DeviceServer.Processing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrns.Device;
using Atdi.DataModels.Sdrn.DeviceServer;
using Atdi.DataModels.Sdrn.DeviceServer.Commands.Parameters;
using Atdi.DataModels.Sdrn.DeviceServer.Commands.Results;
using Atdi.Platform.Logging;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Processing.Measurements
{
    public static class CalcSignalization
    {
        private static int StartLevelsForLevelDistribution = -150;
        private static int NumberPointForLevelDistribution = 200;

        public static Spectrum CreateSpectrum(float[] templevels, double stepBW_kHz, double startFreq_MHz, double NoiseLevel_dBm, double DiffLevelForCalcBW = 25, double windowBW = 1.5,
           double nDbLevel_dB = 15, int NumberIgnoredPoints = 1, double MinExcessNoseLevel_dB = 5)
        {
            MeasBandwidthResult measSdrBandwidthResults = BandWidthEstimation.GetBandwidthPoint(templevels, BandWidthEstimation.BandwidthEstimationType.xFromCentr, DiffLevelForCalcBW, 0);
            Spectrum Spectrum = new Spectrum();
            int start = 0; int stop = templevels.Length;
            if (measSdrBandwidthResults.СorrectnessEstimations != null)
            {
                if (measSdrBandwidthResults.СorrectnessEstimations.Value)
                {
                    // значит спектр хороший можно брать его параметры
                    if (measSdrBandwidthResults.T1 != null) { start = measSdrBandwidthResults.T1.Value; Spectrum.T1 = measSdrBandwidthResults.T1.Value; }
                    if (measSdrBandwidthResults.T2 != null) { stop = measSdrBandwidthResults.T2.Value; Spectrum.T2 = measSdrBandwidthResults.T2.Value; }
                    if (measSdrBandwidthResults.MarkerIndex != null) { Spectrum.MarkerIndex = measSdrBandwidthResults.MarkerIndex.Value; }
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
                    bool CorectCalcBW = CalcBWSignalization.CalcBW(templevels, start, stop, nDbLevel_dB, NoiseLevel_dBm, MinExcessNoseLevel_dB, NumberIgnoredPoints, ref startreal, ref stoptreal);
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
        public static Emitting FillEmittingForStorage(Emitting EmittingParameter, ILogger logger)
        {
            var emitting = EmittingParameter;
            try
            {
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
                for (int i = 0; i < NumberPointForLevelDistribution; i++)
                {
                    emitting.LevelsDistribution.Levels[i] = StartLevelsForLevelDistribution + i;
                    emitting.LevelsDistribution.Count[i] = 0;
                }
                int indexLevel = (int)Math.Floor(emitting.CurentPower_dBm) - emitting.LevelsDistribution.Levels[0];
                if ((indexLevel >= 0) && (indexLevel < emitting.LevelsDistribution.Levels.Length)) { emitting.LevelsDistribution.Count[indexLevel]++; }
            }
            catch (Exception ex)
            {
                logger.Error(Contexts.SignalizationTaskResultHandler, Categories.Measurements, Exceptions.UnknownErrorSignalizationTaskWorker, ex.Message);
            }
            return emitting;
        }
        public static Emitting CreatIndependEmitting(Emitting emitting)
        {
            Emitting emitting1 = new Emitting()
            {
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
        /// <summary>
        /// Проверка на то выходит ли излучение за пределы RefLevels
        /// </summary>
        /// <param name="emitting"></param>
        /// <param name="referenceLevels"></param>
        /// <returns></returns>
        public static bool CheckContravention(ref Spectrum spectrum, ReferenceLevels referenceLevels, bool Smooth = true)
        { // ПРОТЕСТИРОВАНО
            float[] Levels = new float[spectrum.Levels_dBm.Length];
            if (Smooth)
            {
                Levels = SmoothTrace.blackman(spectrum.Levels_dBm);
            }
            else
            {
                Levels = spectrum.Levels_dBm;
            }
            double LogFreqSpectrum = 10 * Math.Log10(spectrum.SpectrumSteps_kHz * 1000.0);
            double LogFreqRef = 10 * Math.Log10(referenceLevels.StepFrequency_Hz);

            for (int i = 0; i < Levels.Length; i++)
            {
                // double DensityLevel = Levels[i] - LogFreqSpectrum;
                double freq_Level_Hz = spectrum.SpectrumStartFreq_MHz * 1000000.0 + spectrum.SpectrumSteps_kHz * i * 1000.0;
                int IndexRef = (int)Math.Round((freq_Level_Hz - referenceLevels.StartFrequency_Hz) / referenceLevels.StepFrequency_Hz);
                if ((IndexRef >= 0) && (IndexRef < referenceLevels.levels.Length))
                {
                    if (referenceLevels.levels[IndexRef] - LogFreqRef < Levels[i] - LogFreqSpectrum)
                    { spectrum.Contravention = true; return spectrum.Contravention; }
                }
            }
            spectrum.Contravention = false;  return spectrum.Contravention;
        }
    }
}


