using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Atdi.Contracts.WcfServices.Sdrn.Server.IeStation
{
    /// <summary>
    /// Represent triggers levels adopted to trace of Devise (SDR)
    /// </summary>
    public class ReferenceLevels
    {
        public double StartFrequency_Hz;
        public double StepFrequency_Hz;
        public float[] levels;
        /// <summary>
        /// Create triggers levels from set of signals
        /// </summary>
        /// <param name="referenceSignalSet"></param>
        /// <param name="sDRParameters"></param>
        /// <param name="trigerLevel_dBm_Hz"></param>
        //public ReferenceLevels(ReferenceSignal[] referenceSignalSet, SDRTraceParameters sDRTraceParameters, double triggerLevel_dBm_Hz, SensorParameters sensorParameters)
        //{ // НЕ ТЕСТИРОВАННО
        //    //const
        //    double allowableExcess_dB = 10;
        //    //const
        //    StartFrequency_MHz = sDRTraceParameters.StartFreq_Hz/1000000.0;
        //    StepFrequency_kHz = sDRTraceParameters.StepFreq_Hz / 1000.0;
        //    levels = new float[sDRTraceParameters.TraceSize];
        //    for (int i = 0; i<sDRTraceParameters.TraceSize; i++)
        //    {
        //        // расчет уровня для одной частоты StartFrequency_MHz + StepFrequency_kHz*1000*i
        //        levels[i] = (float)(triggerLevel_dBm_Hz + 10.0*Math.Log10(StepFrequency_kHz)+30.0);
        //        double levelFromSignal_mW = 0;
        //        double gainInFreq = sensorParameters.getGainLoss(StartFrequency_MHz + StepFrequency_kHz * 1000 * i);
        //        if (referenceSignalSet != null)
        //        {
        //            for (int j = 0; j < referenceSignalSet.Length; j++)
        //            {
        //                // проверка на попадение
        //                double freqStart = StartFrequency_MHz + StepFrequency_kHz * 1000 * (i - 0.5);
        //                double freqStop = StartFrequency_MHz + StepFrequency_kHz * 1000 * (i + 0.5);
        //                double freqStartSignal = referenceSignalSet[j].Frequency_MHz - 500 * referenceSignalSet[j].Bandwidth_kHz;
        //                double freqStopSignal = referenceSignalSet[j].Frequency_MHz + 500 * referenceSignalSet[j].Bandwidth_kHz;
        //                if (!((freqStart < freqStopSignal) || (freqStop > freqStartSignal)))
        //                { // попали определяем долю сигнала в данном таймштампе в ватах
        //                    double interseption = Math.Min(freqStop, freqStopSignal) - Math.Min(freqStart, freqStartSignal) / (freqStop - freqStart);
        //                    levelFromSignal_mW = levelFromSignal_mW + interseption * Math.Pow(10, referenceSignalSet[j].Level_dBm / 10);
        //                }
        //            }
        //        }
        //        if (levelFromSignal_mW > 0)
        //        {// доп проверка на нулевые значения 
        //            double lev = levelFromSignal_mW  / (Math.Pow(10, gainInFreq/10));
        //            levels[i] =(float) (10*Math.Log10(Math.Pow(10,levels[i]/10) + lev ) + allowableExcess_dB);
        //        }

        //    }
        //}
    }
}
