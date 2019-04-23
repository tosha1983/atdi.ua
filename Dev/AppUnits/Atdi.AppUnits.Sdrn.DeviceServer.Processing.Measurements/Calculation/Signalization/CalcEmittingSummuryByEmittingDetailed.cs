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

namespace Atdi.AppUnits.Sdrn.DeviceServer.Processing.Measurements
{
    public static class CalcEmittingSummuryByEmittingDetailed
    {
        public static bool GetEmittingDetailed(ref Emitting[] emittingSummary, List<BWResult> listBWResult, ReferenceLevels referenceLevels)
        {
            //Константа 
            double PersentForJoinDetailEmToSummEm = 20;

            // обновляем emittingSummary результатами новых измерений
            for (int i = 0; listBWResult.Count > i; i++)
            {
                // на первом шаге мы должны определить индекс того излучения которому более всего соответсвует измерение
                var BWRes = listBWResult[i];
                double Min_penalty = 9999;
                int CountInEmittingSummary = 0;
                for (int j = 0; emittingSummary.Length > j; j++)
                {
                    var emiting = emittingSummary[j];
                    double StartFreq = emiting.Spectrum.SpectrumStartFreq_MHz + emiting.Spectrum.T1 * emiting.Spectrum.SpectrumSteps_kHz / 1000;
                    double StopFreq = emiting.Spectrum.SpectrumStartFreq_MHz + emiting.Spectrum.T2 * emiting.Spectrum.SpectrumSteps_kHz / 1000;
                    double CentralFreq = (StartFreq + StopFreq) / 2;
                    double BW = StopFreq - StartFreq;
                    double stepBW_Hz = (BWRes.Freq_Hz[BWRes.Freq_Hz.Length - 1] - BWRes.Freq_Hz[0]) / (BWRes.Freq_Hz.Length - 1);
                    double StartFreqD = (BWRes.Freq_Hz[0] + stepBW_Hz * BWRes.T1)/1000000;
                    double StopFreqD = (BWRes.Freq_Hz[0] + stepBW_Hz * BWRes.T2)/1000000;
                    double CentralFreqD = (StartFreqD + StopFreqD) / 2;
                    double BWD = StopFreqD - StartFreqD;
                    if (!((StartFreq > StopFreqD)|| (StartFreqD > StopFreq)))
                    {
                        double CurPenalty = Math.Abs(BWD - BW) / BWD + Math.Abs(CentralFreq - CentralFreqD) / BWD;
                        if (CurPenalty < Min_penalty) { Min_penalty = CurPenalty; CountInEmittingSummary = j;}
                    }
                }
                if (Min_penalty < PersentForJoinDetailEmToSummEm / 100)
                {
                    // присоединяемся к существующему емитингу
                    bool r = JoinBWResToEmitting(ref emittingSummary[CountInEmittingSummary], BWRes, referenceLevels);
                }
                else
                {
                    // создаем новый емитинг
                    var emitting = CreateEmittingFromBWRes(BWRes, referenceLevels);
                }
            }
            return true;
        }
        private static Emitting CreateEmittingFromBWRes(BWResult BWResult, ReferenceLevels referenceLevels)
        {// НЕ ПРОВЕРЕННО

            Emitting emitting = new Emitting();
            double stepBW_Hz = (BWResult.Freq_Hz[BWResult.Freq_Hz.Length - 1] - BWResult.Freq_Hz[0]) / (BWResult.Freq_Hz.Length - 1);
            emitting.StartFrequency_MHz = (BWResult.Freq_Hz[0] + stepBW_Hz * BWResult.T1) / 1000000;
            emitting.StopFrequency_MHz = (BWResult.Freq_Hz[0] + stepBW_Hz * BWResult.T2) / 1000000;
            emitting.SpectrumIsDetailed = true;
            emitting.LastDetaileMeas = BWResult.TimeMeas;
            emitting.CurentPower_dBm = CommonCalcPowFromTrace.GetPow_dBm(BWResult);
            emitting.ReferenceLevel_dBm = CommonCalcPowFromTrace.GetPow_dBm(referenceLevels.levels, referenceLevels.StartFrequency_Hz, referenceLevels.StepFrequency_Hz, emitting.StartFrequency_MHz * 1000000, emitting.StopFrequency_MHz * 1000000);
            emitting.Spectrum = new Spectrum();
            emitting.Spectrum.Bandwidth_kHz = BWResult.Bandwidth_kHz;
            emitting.Spectrum.Levels_dBm = new float[BWResult.Levels_dBm.Length];
            BWResult.Levels_dBm.CopyTo(emitting.Spectrum.Levels_dBm, 0);
            emitting.Spectrum.MarkerIndex = BWResult.MarkerIndex;
            emitting.Spectrum.SignalLevel_dBm = (float)emitting.CurentPower_dBm;
            emitting.Spectrum.SpectrumStartFreq_MHz = BWResult.Freq_Hz[0] / 1000000;
            emitting.Spectrum.SpectrumSteps_kHz = stepBW_Hz;
            emitting.Spectrum.T1 = BWResult.T1;
            emitting.Spectrum.T2 = BWResult.T2;
            emitting.Spectrum.TraceCount  = BWResult.TraceCount;
            emitting.Spectrum.СorrectnessEstimations = BWResult.СorrectnessEstimations;
            CalcSignalization.FillEmittingForStorage(emitting);
            return emitting;
        }
        private static bool JoinBWResToEmitting(ref Emitting emitting, BWResult BWResult, ReferenceLevels referenceLevels)
        {
            double stepBW_Hz = (BWResult.Freq_Hz[BWResult.Freq_Hz.Length - 1] - BWResult.Freq_Hz[0]) / (BWResult.Freq_Hz.Length - 1);
            emitting.StartFrequency_MHz = (BWResult.Freq_Hz[0] + stepBW_Hz * BWResult.T1) / 1000000;
            emitting.StopFrequency_MHz = (BWResult.Freq_Hz[0] + stepBW_Hz * BWResult.T2) / 1000000;
            emitting.SpectrumIsDetailed = true;
            emitting.LastDetaileMeas = BWResult.TimeMeas;
            emitting.CurentPower_dBm = CommonCalcPowFromTrace.GetPow_dBm(BWResult);
            emitting.ReferenceLevel_dBm = CommonCalcPowFromTrace.GetPow_dBm(referenceLevels.levels, referenceLevels.StartFrequency_Hz, referenceLevels.StepFrequency_Hz, emitting.StartFrequency_MHz * 1000000, emitting.StopFrequency_MHz * 1000000);
            emitting.Spectrum = new Spectrum();
            emitting.Spectrum.Bandwidth_kHz = BWResult.Bandwidth_kHz;
            emitting.Spectrum.Levels_dBm = new float[BWResult.Levels_dBm.Length];
            BWResult.Levels_dBm.CopyTo(emitting.Spectrum.Levels_dBm, 0);
            emitting.Spectrum.MarkerIndex = BWResult.MarkerIndex;
            emitting.Spectrum.SignalLevel_dBm = (float)emitting.CurentPower_dBm;
            emitting.Spectrum.SpectrumStartFreq_MHz = BWResult.Freq_Hz[0] / 1000000;
            emitting.Spectrum.SpectrumSteps_kHz = stepBW_Hz;
            emitting.Spectrum.T1 = BWResult.T1;
            emitting.Spectrum.T2 = BWResult.T2;
            emitting.Spectrum.TraceCount = BWResult.TraceCount;
            emitting.Spectrum.СorrectnessEstimations = BWResult.СorrectnessEstimations;
            int indexLevel = (int)Math.Floor(emitting.CurentPower_dBm) - emitting.LevelsDistribution.Levels[0];
            if ((indexLevel >= 0) && (indexLevel < emitting.LevelsDistribution.Levels.Length)) { emitting.LevelsDistribution.Count[indexLevel]++; }
            emitting.WorkTimes[emitting.WorkTimes.Length - 1].StopEmitting = BWResult.TimeMeas;
            emitting.WorkTimes[emitting.WorkTimes.Length - 1].HitCount++;
            emitting.WorkTimes[emitting.WorkTimes.Length - 1].ScanCount = emitting.WorkTimes[emitting.WorkTimes.Length - 1].ScanCount + emitting.WorkTimes[emitting.WorkTimes.Length - 1].TempCount + 1;
            emitting.WorkTimes[emitting.WorkTimes.Length - 1].TempCount = 0;
            emitting.WorkTimes[emitting.WorkTimes.Length - 1].PersentAvailability =100*emitting.WorkTimes[emitting.WorkTimes.Length - 1].HitCount/ emitting.WorkTimes[emitting.WorkTimes.Length - 1].ScanCount;

            return true;
        }
    }
}
