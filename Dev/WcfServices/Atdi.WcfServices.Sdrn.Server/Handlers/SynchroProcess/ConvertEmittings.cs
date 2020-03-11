using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wcf = Atdi.Contracts.WcfServices.Sdrn.Server;
using Calculation = Atdi.Modules.Sdrn.Calculation;

namespace Atdi.WcfServices.Sdrn.Server
{
    public static class ConvertEmittings
    {
        public static Calculation.Emitting[] ConvertArray(Wcf.Emitting[] emitting)
        {
            var emittingsOut = new List<Calculation.Emitting>();
            for (int i = 0; i < emitting.Length; i++)
            {
                emittingsOut.Add(Convert(emitting[i]));
            }
            return emittingsOut.ToArray();
        }
        public static Calculation.Emitting Convert(Wcf.Emitting emitting)
        {
            Calculation.Emitting devEmitting = new Calculation.Emitting();
            devEmitting.Id = emitting.Id;
            devEmitting.CurentPower_dBm = emitting.CurentPower_dBm;
            devEmitting.MeanDeviationFromReference = emitting.MeanDeviationFromReference;
            devEmitting.ReferenceLevel_dBm = emitting.ReferenceLevel_dBm;
            //devEmitting.SensorId = emitting..SensorId;
            //devEmitting.SpectrumIsDetailed = emitting.;
            devEmitting.StartFrequency_MHz = emitting.StartFrequency_MHz;
            devEmitting.StopFrequency_MHz = emitting.StopFrequency_MHz;
            devEmitting.TriggerDeviationFromReference = emitting.TriggerDeviationFromReference;
            if (emitting.EmittingParameters != null)
            {
                devEmitting.EmittingParameters = new Calculation.EmittingParameters();
                //devEmitting.EmittingParameters.FreqDeviation = emitting..FreqDeviation;
                devEmitting.EmittingParameters.RollOffFactor = emitting.EmittingParameters.RollOffFactor;
                devEmitting.EmittingParameters.StandardBW = emitting.EmittingParameters.StandardBW;
                devEmitting.EmittingParameters.TriggerFreqDeviation = emitting.TriggerDeviationFromReference;
            }
            if (emitting.LevelsDistribution != null)
            {
                devEmitting.LevelsDistribution = new Calculation.LevelsDistribution();
                devEmitting.LevelsDistribution.Count = emitting.LevelsDistribution.Count;
                devEmitting.LevelsDistribution.Levels = emitting.LevelsDistribution.Levels;
            }
            if (emitting.SignalMask != null)
            {
                devEmitting.SignalMask = new Calculation.SignalMask();
                devEmitting.SignalMask.Freq_kHz = emitting.SignalMask.Freq_kHz;
                devEmitting.SignalMask.Loss_dB = emitting.SignalMask.Loss_dB;
            }
            if (emitting.Spectrum != null)
            {
                devEmitting.Spectrum = new Calculation.Spectrum();
                devEmitting.Spectrum.Bandwidth_kHz = emitting.Spectrum.Bandwidth_kHz;
                devEmitting.Spectrum.Contravention = emitting.Spectrum.Contravention;
                devEmitting.Spectrum.Levels_dBm = emitting.Spectrum.Levels_dBm;
                devEmitting.Spectrum.MarkerIndex = emitting.Spectrum.MarkerIndex;
                devEmitting.Spectrum.SignalLevel_dBm = emitting.Spectrum.SignalLevel_dBm;
                devEmitting.Spectrum.SpectrumStartFreq_MHz = emitting.Spectrum.SpectrumStartFreq_MHz;
                devEmitting.Spectrum.SpectrumSteps_kHz = emitting.Spectrum.SpectrumSteps_kHz;
                devEmitting.Spectrum.T1 = emitting.Spectrum.T1;
                devEmitting.Spectrum.T2 = emitting.Spectrum.T2;
                devEmitting.Spectrum.TraceCount = emitting.Spectrum.TraceCount;
                devEmitting.Spectrum.СorrectnessEstimations = emitting.Spectrum.CorrectnessEstimations;
            }
            if (emitting.WorkTimes != null)
            {
                devEmitting.WorkTimes = new Calculation.WorkTime[emitting.WorkTimes.Length];
                for (int i=0; i< emitting.WorkTimes.Length; i++)
                {
                    devEmitting.WorkTimes[i] = new Calculation.WorkTime();
                    devEmitting.WorkTimes[i].HitCount = emitting.WorkTimes[i].HitCount;
                    devEmitting.WorkTimes[i].PersentAvailability = emitting.WorkTimes[i].PersentAvailability;
                    devEmitting.WorkTimes[i].StartEmitting = emitting.WorkTimes[i].StartEmitting;
                    devEmitting.WorkTimes[i].StopEmitting = emitting.WorkTimes[i].StopEmitting;
                }
            }
            return devEmitting;
        }
    }
}
