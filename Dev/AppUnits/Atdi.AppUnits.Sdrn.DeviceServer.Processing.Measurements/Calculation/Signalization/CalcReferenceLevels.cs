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
    public static class CalcReferenceLevels
    {
        public static ReferenceLevels CalcRefLevels (ReferenceSituation referenceSituation, MesureTraceResult Trace, MesureTraceDeviceProperties mesureTraceDeviceProperties)
        {
            // НЕ ТЕСТИРОВАННО
            //const
            double allowableExcess_dB = 10; 
            double triggerLevel_dBm_Hz = -150; // должно быть определено в оборудовании
            //const
            ReferenceLevels referenceLevels = new ReferenceLevels();
            referenceLevels.StartFrequency_Hz = Trace.Freq_Hz[0];
            referenceLevels.StepFrequency_Hz = (Trace.Freq_Hz[1]- Trace.Freq_Hz[0]);
            referenceLevels.levels = new float[Trace.Level.Length];
            for (int i = 0; i < referenceLevels.levels.Length; i++)
            {
                // расчет уровня для одной частоты StartFrequency_Hz + StepFrequency_Hz*i
                referenceLevels.levels[i] = (float)(triggerLevel_dBm_Hz + 10.0 * Math.Log10(referenceLevels.StepFrequency_Hz));
                double levelFromSignal_mW = 0;
                double gainInFreq = CalcSDRParameters.SDRGainFromFrequency(mesureTraceDeviceProperties, referenceLevels.StartFrequency_Hz + referenceLevels.StepFrequency_Hz * (i));
                if ((referenceSituation != null)&&(referenceSituation.ReferenceSignal != null))
                {
                    for (int j = 0; j < referenceSituation.ReferenceSignal.Length; j++)
                    {
                        // проверка на попадение
                        double freqStart = (referenceLevels.StartFrequency_Hz + referenceLevels.StepFrequency_Hz * (i - 0.5))/1000000;
                        double freqStop = (referenceLevels.StartFrequency_Hz + referenceLevels.StepFrequency_Hz * (i + 0.5))/1000000;
                        double freqStartSignal = referenceSituation.ReferenceSignal[j].Frequency_MHz - 500 * referenceSituation.ReferenceSignal[j].Bandwidth_kHz;
                        double freqStopSignal = referenceSituation.ReferenceSignal[j].Frequency_MHz + 500 * referenceSituation.ReferenceSignal[j].Bandwidth_kHz;
                        if (!((freqStart < freqStopSignal) || (freqStop > freqStartSignal)))
                        { // попали определяем долю сигнала в данном таймштампе в ватах
                            double interseption = Math.Min(freqStop, freqStopSignal) - Math.Min(freqStart, freqStartSignal) / (freqStop - freqStart);
                            levelFromSignal_mW = levelFromSignal_mW + interseption * Math.Pow(10, referenceSituation.ReferenceSignal[j].LevelSignal_dBm / 10);
                        }
                    }
                }
                if (levelFromSignal_mW > 0)
                {// доп проверка на нулевые значения
                    double lev = levelFromSignal_mW / (Math.Pow(10, gainInFreq / 10));
                    referenceLevels.levels[i] = (float)(10 * Math.Log10(Math.Pow(10, referenceLevels.levels[i] / 10) + lev) + allowableExcess_dB);
                }
            }
            return referenceLevels;
        }
    }
}
