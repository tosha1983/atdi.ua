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
        public static ReferenceLevels CalcRefLevels (TaskParameters taskParameters, ReferenceSituation referenceSituation, MesureTraceResult Trace, MesureTraceDeviceProperties mesureTraceDeviceProperties, ref double NoiseLevel_dBm, double triggerLevel_dBm_Hz = -999)
        {
            if (triggerLevel_dBm_Hz == -999)
            {// расчитываем уровеньшума по трейсу // Надо проверить
                triggerLevel_dBm_Hz = GetNoiseLeveldBm(Trace, taskParameters.PercentForCalcNoise) - 10*Math.Log10(Trace.Freq_Hz[1] - Trace.Freq_Hz[0]);
            }
            double stepTraceHz = (Trace.Freq_Hz[Trace.Freq_Hz.Length - 1] - Trace.Freq_Hz[0]) / (Trace.Freq_Hz.Length - 1);
            NoiseLevel_dBm = triggerLevel_dBm_Hz + 10*Math.Log10(stepTraceHz);
            ReferenceLevels referenceLevels = new ReferenceLevels();
            referenceLevels.StartFrequency_Hz = Trace.Freq_Hz[0];
            referenceLevels.StepFrequency_Hz = (stepTraceHz);
            referenceLevels.levels = new float[Trace.Level.Length];
            bool corectMask = SortReferenceSituationMask(ref referenceSituation);
            for (int i = 0; i < referenceLevels.levels.Length; i++)
            {
                // расчет уровня для одной частоты StartFrequency_Hz + StepFrequency_Hz*i
                referenceLevels.levels[i] = (float)(triggerLevel_dBm_Hz + 10.0 * Math.Log10(referenceLevels.StepFrequency_Hz) + taskParameters.SignalingMeasTaskParameters.allowableExcess_dB.Value);
                double levelFromSignal_mW = 0;
                double gainInFreq = CalcSDRParameters.SDRGainFromFrequency(mesureTraceDeviceProperties, referenceLevels.StartFrequency_Hz + referenceLevels.StepFrequency_Hz * (i));
                if ((referenceSituation != null)&&(referenceSituation.ReferenceSignal != null))
                {
                    double freqStart = (referenceLevels.StartFrequency_Hz + referenceLevels.StepFrequency_Hz * (i - 0.5)) / 1000000;
                    double freqStop = (referenceLevels.StartFrequency_Hz + referenceLevels.StepFrequency_Hz * (i + 0.5)) / 1000000;

                    for (int j = 0; j < referenceSituation.ReferenceSignal.Length; j++)
                    {
                        // проверка на попадение
                        double freqStartSignal;
                        double freqStopSignal; 
                        if ((referenceSituation.ReferenceSignal[j].SignalMask != null) && 
                            (referenceSituation.ReferenceSignal[j].SignalMask.Freq_kHz!=null)&&
                            referenceSituation.ReferenceSignal[j].SignalMask.Freq_kHz.Length>1)
                        {
                            freqStartSignal = referenceSituation.ReferenceSignal[j].Frequency_MHz + (double)(referenceSituation.ReferenceSignal[j].SignalMask.Freq_kHz[0]/ 1000);
                            freqStopSignal = referenceSituation.ReferenceSignal[j].Frequency_MHz + (double)(referenceSituation.ReferenceSignal[j].SignalMask.Freq_kHz[referenceSituation.ReferenceSignal[j].SignalMask.Freq_kHz.Length-1] / 1000);
                        }
                        else
                        {
                            freqStartSignal = referenceSituation.ReferenceSignal[j].Frequency_MHz - (double)(referenceSituation.ReferenceSignal[j].Bandwidth_kHz / 2000);
                            freqStopSignal = referenceSituation.ReferenceSignal[j].Frequency_MHz + (double)(referenceSituation.ReferenceSignal[j].Bandwidth_kHz / 2000);
                        }
                        if (((freqStart < freqStopSignal) && (freqStop > freqStartSignal)))
                        { // попали определяем долю сигнала в данном таймштампе в ватах
                            //double interseption = (Math.Min(freqStop, freqStopSignal) - Math.Max(freqStart, freqStartSignal)) / (freqStopSignal - freqStartSignal);
                            //levelFromSignal_mW = levelFromSignal_mW + interseption * Math.Pow(10, referenceSituation.ReferenceSignal[j].LevelSignal_dBm / 10);

                            levelFromSignal_mW = levelFromSignal_mW + GetPowerOfSignalInBand_mW(referenceSituation.ReferenceSignal[j], freqStart, freqStop);
                        }
                    }
                }
                if (levelFromSignal_mW > 0)
                {// доп проверка на нулевые значения
                    double lev = levelFromSignal_mW / (Math.Pow(10, gainInFreq / 10));
                    lev = lev * Math.Pow(10, taskParameters.SignalingMeasTaskParameters.allowableExcess_dB.Value / 10);
                    referenceLevels.levels[i] = (float)(10 * Math.Log10(Math.Pow(10, referenceLevels.levels[i] / 10) + lev));
                }
            }
            return referenceLevels;
        }
        public static double GetNoiseLeveldBm(MesureTraceResult Trace, double PercentForCalcNoise = 10)
        { // НЕ ПРОВЕРЕННО
            float[] nums = new float[Trace.Level.Length];
            for (int i = 0; i < nums.Length; i++)
            {
                nums[i] = Trace.Level[i];
            }
            float temp;
            int number = (int)Math.Floor(Trace.Level.Length * PercentForCalcNoise / 100.0);
            if (number > nums.Length-1) { number = nums.Length - 1; }
            for (int i = 0; i < number; i++)
            {
                for (int j = i + 1; j < nums.Length; j++)
                {
                    if (nums[i] > nums[j])
                    {
                        temp = nums[i];
                        nums[i] = nums[j];
                        nums[j] = temp;
                    }
                }
            }
            return nums[number-1];
        }
        private static bool SortReferenceSituationMask(ref ReferenceSituation referenceSituation)
        { //НЕ ПРОТЕСТИРОВАННО
            if (referenceSituation != null)
            {
                if (referenceSituation.ReferenceSignal != null)
                {
                    for (int i = 0; referenceSituation.ReferenceSignal.Length > i; i++)
                    {// цикл по сигналам
                        if ((referenceSituation.ReferenceSignal[i].SignalMask != null) && (referenceSituation.ReferenceSignal[i].SignalMask.Freq_kHz != null) &&
                            (referenceSituation.ReferenceSignal[i].SignalMask.Loss_dB != null) && (referenceSituation.ReferenceSignal[i].SignalMask.Freq_kHz.Length > 0) &&
                            (referenceSituation.ReferenceSignal[i].SignalMask.Freq_kHz.Length == referenceSituation.ReferenceSignal[i].SignalMask.Loss_dB.Length))
                        {
                            for (int k = 0; k < referenceSituation.ReferenceSignal[i].SignalMask.Freq_kHz.Length - 1; k++)
                            {
                                for (int j = k + 1; j < referenceSituation.ReferenceSignal[i].SignalMask.Freq_kHz.Length; j++)
                                {
                                    if (referenceSituation.ReferenceSignal[i].SignalMask.Freq_kHz[k] > referenceSituation.ReferenceSignal[i].SignalMask.Freq_kHz[j])
                                    {
                                        double TempFreq = referenceSituation.ReferenceSignal[i].SignalMask.Freq_kHz[k];
                                        float TempLoss = referenceSituation.ReferenceSignal[i].SignalMask.Loss_dB[k];
                                        referenceSituation.ReferenceSignal[i].SignalMask.Freq_kHz[k] = referenceSituation.ReferenceSignal[i].SignalMask.Freq_kHz[j];
                                        referenceSituation.ReferenceSignal[i].SignalMask.Loss_dB[k] = referenceSituation.ReferenceSignal[i].SignalMask.Loss_dB[j];
                                        referenceSituation.ReferenceSignal[i].SignalMask.Freq_kHz[j] = TempFreq;
                                        referenceSituation.ReferenceSignal[i].SignalMask.Loss_dB[j] = TempLoss;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return true;
        }
        private static double GetPowerOfSignalInBand_mW(ReferenceSignal referenceSignal, double FreqStart_MHz, double FreqStop_MHz)
        {
            // НЕ ТЕСТИРОВАННО
            double FreqStop_kHz = FreqStop_MHz * 1000.0;
            double FreqStart_kHz = FreqStart_MHz * 1000.0;
            double freqStopSignal_kHz = referenceSignal.Frequency_MHz * 1000.0 + referenceSignal.Bandwidth_kHz / 2.0;
            double freqStartSignal_kHz = referenceSignal.Frequency_MHz * 1000.0 - referenceSignal.Bandwidth_kHz / 2.0;
            bool СalcWithautMask = false;
            if ((referenceSignal.SignalMask == null)|| (referenceSignal.SignalMask.Loss_dB == null)||(referenceSignal.SignalMask.Freq_kHz == null) ||
                (referenceSignal.SignalMask.Freq_kHz.Length < 2)||(referenceSignal.SignalMask.Freq_kHz.Length != referenceSignal.SignalMask.Loss_dB.Length))
            {// Маски нет или она калечная
                СalcWithautMask = true;
            }
            else
            {
                for (int i = 0; referenceSignal.SignalMask.Freq_kHz.Length-1>i; i++)
                {
                    if (referenceSignal.SignalMask.Freq_kHz[i] > referenceSignal.SignalMask.Freq_kHz[i + 1]) { СalcWithautMask = true; break; }
                }
            }
            if (СalcWithautMask)
            {

                double interseption = (Math.Min(FreqStop_kHz, freqStopSignal_kHz) - Math.Max(FreqStart_kHz, freqStartSignal_kHz)) / (freqStopSignal_kHz - freqStartSignal_kHz);
                if (interseption <= 0) { return 0;}
                double Level = interseption * Math.Pow(10, referenceSignal.LevelSignal_dBm / 10);
                return Level;
            }

            if ((referenceSignal.SignalMask.Freq_kHz[0] + referenceSignal.Frequency_MHz * 1000.0> FreqStop_kHz)&&
                (referenceSignal.SignalMask.Freq_kHz[referenceSignal.SignalMask.Freq_kHz.Length-1] + referenceSignal.Frequency_MHz * 1000.0 < FreqStart_kHz))
            {
                return 0;
            }
            // Если дошли то надо считать с маской
            // расчет делаем не по отношению площадей, а по отношению к 0 где максимальное излучение
            double SpectrumDensyti_mWinHz = Math.Pow(10, referenceSignal.LevelSignal_dBm / 10) / (referenceSignal.Bandwidth_kHz * 1000);
            double Level_mW = 0;
            for (int i = 1; referenceSignal.SignalMask.Freq_kHz.Length>i; i++)
            {
                double MaskFreqStart_kHz = referenceSignal.SignalMask.Freq_kHz[i - 1] + 1000.0 * referenceSignal.Frequency_MHz;
                double MaskFreqStop_kHz = referenceSignal.SignalMask.Freq_kHz[i] + 1000.0 * referenceSignal.Frequency_MHz;
                if (!((FreqStart_kHz > MaskFreqStop_kHz) || (FreqStop_kHz < MaskFreqStart_kHz)))
                {// есть пересечение
                    double freq_start_calc = Math.Max(MaskFreqStart_kHz, FreqStart_kHz);
                    double freq_stop_calc = Math.Min(MaskFreqStop_kHz, FreqStop_kHz);
                    double LossStart = referenceSignal.SignalMask.Loss_dB[i - 1] + (referenceSignal.SignalMask.Loss_dB[i] - referenceSignal.SignalMask.Loss_dB[i - 1]) *
                        (freq_start_calc - MaskFreqStart_kHz) / (MaskFreqStop_kHz - MaskFreqStart_kHz);
                    double LossStop = referenceSignal.SignalMask.Loss_dB[i - 1] + (referenceSignal.SignalMask.Loss_dB[i] - referenceSignal.SignalMask.Loss_dB[i - 1]) *
                        (freq_stop_calc - MaskFreqStart_kHz) / (MaskFreqStop_kHz - MaskFreqStart_kHz);
                    double loss = Math.Pow(10, ((LossStart + LossStop) / 2.0) / 10); // в разах
                    double LocBW_Hz = (freq_stop_calc - freq_start_calc) * 1000.0;
                    Level_mW = Level_mW + SpectrumDensyti_mWinHz * (LocBW_Hz) / loss;
                }
                else if (FreqStart_kHz < MaskFreqStop_kHz) { break; }
            }
            return Level_mW;
        }

    }
}
