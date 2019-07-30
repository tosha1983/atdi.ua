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
    public static class CalcSearchInterruption
    {
        // Константы 
        private static int NumberPointForChangeExcess = 10; // на самом деле зависит от параметров таска там будем вычислять и прокидывать сюда.
        private static double allowableExcess_dB = 10;
        private static double DiffLevelForCalcBW = 25;
        private static double windowBW = 1.05;
        private static double nDbLevel_dB = 15;
        private static int NumberIgnoredPoints = 1;
        private static double MinExcessNoseLevel_dB = 5;
        // Конец констант

        /// <summary>
        /// Сопоставляем излучения реальные с реферативными уровнями. В случае превышения формируем излучение. 
        /// </summary>
        /// <param name="referenceLevels"></param>
        /// <param name="Trace"></param>
        /// <returns></returns>
        public static Emitting[] Calc(TaskParameters taskParameters, ReferenceLevels refLevels, MesureTraceResult Trace, double NoiseLevel_dBm)
        { //НЕ ТЕСТИРОВАННО
            
            if (refLevels.levels.Length != Trace.Level.Length)
            {
                return null; // выход по причине несовпадения количества точек следовательно необходимо перерасчитать CalcReferenceLevels 
            }
            // выделение мест где произошло превышение порога 
            List<int> index_start_stop = new List<int>();
            if (taskParameters.CompareTraceJustWithRefLevels)
            {
                index_start_stop = SearchStartStopCompaireWithRefLevels(refLevels, Trace, NoiseLevel_dBm, NumberPointForChangeExcess);
            }
            else
            {
                index_start_stop = SearchStartStopCompaireWithNoiseLevels(refLevels, Trace, NoiseLevel_dBm, NumberPointForChangeExcess);
            }
            // конец выделения 
            //возможно необходимо произвести разделение нескольких излучений которые моги быть ошибочно восприняты как одно
            if (taskParameters.AutoDivisionEmitting) { index_start_stop = DivisionEmitting(taskParameters, index_start_stop, Trace); }

            //Формируем помехи.
            double stepBW_kHz = (Trace.Freq_Hz[Trace.Freq_Hz.Length-1] - Trace.Freq_Hz[0]) / ((Trace.Freq_Hz.Length-1)*1000.0);
            Emitting[] newEmittings = CreateEmittings(Trace, refLevels, index_start_stop, stepBW_kHz, NoiseLevel_dBm, taskParameters.CompareTraceJustWithRefLevels);
            // сформировали новые параметры излучения теперь надо накатить старые по идее.
            return newEmittings;
        }
        /// <summary>
        /// Производит выделение излучений на основании сравнения уровня RefLevels и полученного. Если превышение наблюдается подряд менее чем в NumberPointForChangeExcess точек то оно игнорируется.   
        /// </summary>
        /// <param name="refLevels"></param>
        /// <param name="Trace"></param>
        /// <param name="NoiseLevel_dBm"></param>
        /// <param name="NumberPointForChangeExcess"></param>
        /// <returns></returns>
        private static List<int> SearchStartStopCompaireWithRefLevels(ReferenceLevels refLevels, MesureTraceResult Trace, double NoiseLevel_dBm, int NumberPointForChangeExcess = 10)
        {
            if (refLevels.levels.Length != Trace.Level.Length)
            {
                return null; // выход по причине несовпадения количества точек следовательно необходимо перерасчитать CalcReferenceLevels 
            }
            bool excess = false; int startSignalIndex = 0;
            int NumberPointBeforExcess = 0;
            int NumberPointAfterExcess = 0;
            if (Trace.Level[0] > refLevels.levels[0]) { excess = true; startSignalIndex = 0; }
            // выделение мест где произошло превышение порога 
            List<int> index_start_stop = new List<int>();
            for (int i = 0; i < refLevels.levels.Length; i++)
            {
                if (Trace.Level[i] > refLevels.levels[i])
                { //Превышение
                    NumberPointAfterExcess = 0;
                    if (!excess)
                    {//начало превышения
                        NumberPointBeforExcess++;
                        if (NumberPointBeforExcess >= NumberPointForChangeExcess)
                        {
                            startSignalIndex = i - NumberPointBeforExcess + 1;
                            excess = true;
                        }
                    }
                }
                else
                { // Не превышение
                    NumberPointBeforExcess = 0;
                    if (excess)
                    {
                        NumberPointAfterExcess++;
                        if (NumberPointAfterExcess >= NumberPointForChangeExcess)
                        {
                            // Конец превышения
                            index_start_stop.Add(startSignalIndex);
                            index_start_stop.Add(i - NumberPointAfterExcess + 1);
                            excess = false;
                        }
                    }
                }
            }
            if (excess)
            { // Конец превышения
                index_start_stop.Add(startSignalIndex);
                index_start_stop.Add(refLevels.levels.Length - 1);
                excess = false;
            }
            // выделение произошло. 
            return (index_start_stop);
        }
        /// <summary>
        /// Производит выделение излучений на основании сравнения полученного уровня c шумом а потом с RefLevels. Если превышение наблюдается подряд менее чем в NumberPointForChangeExcess точек то оно игнорируется.   
        /// </summary>
        /// <param name="refLevels"></param>
        /// <param name="Trace"></param>
        /// <param name="NoiseLevel_dBm"></param>
        /// <param name="NumberPointForChangeExcess"></param>
        /// <returns></returns>
        private static List<int> SearchStartStopCompaireWithNoiseLevels(ReferenceLevels refLevels_, MesureTraceResult Trace, double NoiseLevel_dBm, int NumberPointForChangeExcess = 10)
        {// должны произвести разделение согласно пересечению шумового уровня
            bool excess = false; int startSignalIndex = 0;
            int NumberPointBeforExcess = 0;
            int NumberPointAfterExcess = 0;
            if (Trace.Level[0] > NoiseLevel_dBm + allowableExcess_dB) { excess = true; startSignalIndex = 0; }
            // выделение мест где произошло превышение порога 
            List<int> index_start_stop = new List<int>();
            for (int i = 0; i < Trace.Level.Length; i++)
            {
                if (Trace.Level[i] > NoiseLevel_dBm + allowableExcess_dB)
                { //Превышение
                    NumberPointAfterExcess = 0;
                    if (!excess)
                    {//начало превышения
                        NumberPointBeforExcess++;
                        if (NumberPointBeforExcess >= NumberPointForChangeExcess)
                        {
                            startSignalIndex = i - NumberPointBeforExcess + 1;
                            excess = true;
                        }
                    }
                }
                else
                { // Не превышение
                    NumberPointBeforExcess = 0;
                    if (excess)
                    {
                        NumberPointAfterExcess++;
                        if (NumberPointAfterExcess >= NumberPointForChangeExcess)
                        {
                            // Конец превышения
                            index_start_stop.Add(startSignalIndex);
                            index_start_stop.Add(i - NumberPointAfterExcess + 1);
                            excess = false;
                        }
                    }
                }
            }
            if (excess)
            { // Конец превышения
                index_start_stop.Add(startSignalIndex);
                index_start_stop.Add(Trace.Level.Length - 1);
                excess = false;
            }
            // выделение произошло. 
            return (index_start_stop);
        }
        /// <summary>
        /// Команда должна вернуть индексы каналов в которых имеет смысл анализировать сигнал. По принципу, что в соеднем канале он ниже. Удобно подходит для GSM сетей.
        /// </summary>
        /// <param name="Trace"></param>
        /// <param name="Freq_Mhz"></param>
        /// <param name="BWChalnel_kHz"></param>
        /// <param name="NoiseLevel_dBm"></param>
        /// <param name="DifferencePowerInCoChannel_dB"></param>
        /// <returns></returns>
        private static List<int> SearchStartStopByChannelPlanGSM(MesureTraceResult Trace, List<double> Freq_Mhz, double BWChalnel_kHz, double NoiseLevel_dBm, int DifferencePowerInCoChannel_dB = 15)
        {// не тестилось
            List<int> result= new List<int>();
            if (Freq_Mhz is null) { return null;}
            if (Freq_Mhz.Count == 1) { result.Add(0); return result; }
            double Freq_start_Hz = Trace.Freq_Hz[0];
            double step = (Trace.Freq_Hz[Trace.Freq_Hz.Length-1] - Trace.Freq_Hz[0]) / (Trace.Freq_Hz.Length - 1);

            int IndexCenterFirstChannel = (int)Math.Round((Freq_Mhz[0] * 1000000 - Freq_start_Hz) / step);
            double PowFirstCh = Trace.Level[IndexCenterFirstChannel];
            int IndexCenterSecondChannel = (int)Math.Round((Freq_Mhz[1] * 1000000 - Freq_start_Hz) / step);
            double PowSecondCh = Trace.Level[IndexCenterSecondChannel];
            if (Freq_Mhz.Count == 2)
            {
                if (PowSecondCh - DifferencePowerInCoChannel_dB >= PowFirstCh) { result.Add(1); return result; }
                else if (PowFirstCh  - DifferencePowerInCoChannel_dB > PowSecondCh) { result.Add(0); return result; }
            }

            if (PowFirstCh - DifferencePowerInCoChannel_dB >= PowSecondCh) { result.Add(0); return result;}
            for (int i = 2;  Freq_Mhz.Count > i; i++)
            {
                int IndexCenterLastChannel = (int)Math.Round((Freq_Mhz[i] * 1000000 - Freq_start_Hz) / step);
                double PowLastCh = Trace.Level[IndexCenterLastChannel];
                if ((PowSecondCh - DifferencePowerInCoChannel_dB >= PowFirstCh)&& (PowSecondCh - DifferencePowerInCoChannel_dB >= PowLastCh)){ result.Add(i-1);}
                PowFirstCh = PowSecondCh;
                PowSecondCh = PowLastCh;
            }
            if (PowSecondCh - DifferencePowerInCoChannel_dB >= PowFirstCh) { result.Add(Freq_Mhz.Count-1); return result; }
            return result;
        }
        private static List<int> DivisionEmitting (TaskParameters taskParameters, List<int> index_start_stop,  MesureTraceResult Trace)
        { // НЕ ПРОВЕРЕННО
            List<int> ResultStartStopIndexArr = new List<int>();
            for (int i = 0; i < index_start_stop.Count - 1; i = i + 2)
            {
                int[] StartStopAnalized;
                int count = EmissionCounting.Counting(Trace.Level, index_start_stop[i], index_start_stop[i + 1], out StartStopAnalized, taskParameters.DifferenceMaxMax, taskParameters.FiltrationTrace);
                for (int j = 0; j< StartStopAnalized.Length; j++)
                {
                    ResultStartStopIndexArr.Add(StartStopAnalized[j]);
                }
            }
            return ResultStartStopIndexArr;
        }
        private static Emitting[] CreateEmittings(MesureTraceResult trace, ReferenceLevels refLevel, List<int> index_start_stop, double stepBW_kHz, double NoiseLevel_dBm, bool CompareTraceJustWithRefLevels)
        { // задача локализовать излучения

            float[] levels = trace.Level;
            var emittings = new List<Emitting>();
            for (int i = 0; i < index_start_stop.Count; i = i + 2)
            {
                // для каждого излучения вычислим BW 
                int start = index_start_stop[i]; int stop = index_start_stop[i + 1];
                int start_ = start; int stop_ = stop; // нужны только для определения BW
                double winBW = (stop - start) * windowBW;
                if ((start + stop - winBW) / 2 > 0) { start_ = (int)((start + stop - winBW) / 2.0); } else { start_ = 0; }
                if ((start + stop + winBW) / 2 < levels.Length - 1) { stop_ = (int)((start + stop + winBW) / 2.0); } else { stop_ = levels.Length - 1; }
                Emitting emitting = new Emitting();
                float[] templevel = new float[stop_ - start_];
                Array.Copy(levels, start_, templevel, 0, stop_ - start_);
                MeasBandwidthResult measSdrBandwidthResults = BandWidthEstimation.GetBandwidthPoint(templevel, BandWidthEstimation.BandwidthEstimationType.xFromCentr, DiffLevelForCalcBW, 0);
                emitting.Spectrum = new Spectrum();
                if (measSdrBandwidthResults.СorrectnessEstimations != null)
                {
                    if (measSdrBandwidthResults.СorrectnessEstimations.Value)
                    {
                        // значит спектр хороший можно брать его параметры
                        if (measSdrBandwidthResults.T1 != null) { start = start_ + measSdrBandwidthResults.T1.Value; emitting.Spectrum.T1 = measSdrBandwidthResults.T1.Value; }
                        if (measSdrBandwidthResults.T2 != null) { stop = start_ + measSdrBandwidthResults.T2.Value; emitting.Spectrum.T2 = measSdrBandwidthResults.T2.Value; }
                        if (measSdrBandwidthResults.MarkerIndex != null) { emitting.Spectrum.MarkerIndex = measSdrBandwidthResults.MarkerIndex.Value; }
                        emitting.Spectrum.Bandwidth_kHz = (stop - start) * stepBW_kHz;
                        emitting.Spectrum.SpectrumStartFreq_MHz = trace.Freq_Hz[start_]/1000000.0;
                        emitting.Spectrum.SpectrumSteps_kHz = stepBW_kHz;
                        emitting.Spectrum.Levels_dBm = templevel;
                        emitting.StartFrequency_MHz = trace.Freq_Hz[start] / 1000000.0;
                        emitting.StopFrequency_MHz = trace.Freq_Hz[stop] / 1000000.0;
                        emitting.Spectrum.СorrectnessEstimations = true;
                    }
                    else
                    {// попробуем пройтись по масиву с помощью метода nDbDown и всеже определить ширину спектра
                        int startreal = -1;
                        int stoptreal = -1;
                        bool CorectCalcBW = CalcBWSignalization.CalcBW(levels, start, stop, nDbLevel_dB, NoiseLevel_dBm, MinExcessNoseLevel_dB, NumberIgnoredPoints, ref startreal, ref stoptreal);
                        if (CorectCalcBW)
                        {
                            // поиск коректной полосы 
                            if (CompareTraceJustWithRefLevels)
                            {
                                double tempBW = StandartBW.GetStandartBW_kHz((stoptreal - startreal) * stepBW_kHz);
                                double CentralFreq = (trace.Freq_Hz[startreal] + trace.Freq_Hz[stoptreal]) / 2000000.0;
                                emitting.StartFrequency_MHz = CentralFreq - tempBW / 2000;
                                emitting.StopFrequency_MHz = CentralFreq + tempBW / 2000;
                                start = (int)Math.Floor((emitting.StartFrequency_MHz - trace.Freq_Hz[0] / 1000000.0) / (stepBW_kHz / 1000));
                                stop = (int)Math.Ceiling((emitting.StopFrequency_MHz - trace.Freq_Hz[0] / 1000000.0) / (stepBW_kHz / 1000));
                                if (start < 0) { start = 0; }
                                if (stop >= levels.Length) { stop = levels.Length - 1; }
                            }
                            else
                            {
                                emitting.StartFrequency_MHz = trace.Freq_Hz[start] / 1000000.0;
                                emitting.StopFrequency_MHz = trace.Freq_Hz[stop] / 1000000.0;
                            }

                            emitting.Spectrum.Levels_dBm = new float[stop - start];
                            Array.Copy(levels, start, emitting.Spectrum.Levels_dBm, 0, stop - start);
                            emitting.Spectrum.SpectrumStartFreq_MHz = trace.Freq_Hz[start] / 1000000.0;
                            emitting.Spectrum.SpectrumSteps_kHz = stepBW_kHz;
                            emitting.Spectrum.СorrectnessEstimations = true;
                        }
                        else
                        {
                            emitting.StartFrequency_MHz = trace.Freq_Hz[start] / 1000000.0; 
                            emitting.StopFrequency_MHz = trace.Freq_Hz[stop] / 1000000.0;
                            emitting.Spectrum.Levels_dBm = new float[stop - start];
                            Array.Copy(levels, start, emitting.Spectrum.Levels_dBm, 0, stop - start);
                            emitting.Spectrum.SpectrumStartFreq_MHz = trace.Freq_Hz[start] / 1000000.0;
                            emitting.Spectrum.SpectrumSteps_kHz = stepBW_kHz;
                            emitting.Spectrum.Levels_dBm = templevel;
                            emitting.Spectrum.СorrectnessEstimations = false;
                        }
                    }
                }
                else
                {
                    emitting.StartFrequency_MHz = trace.Freq_Hz[start] / 1000000.0; 
                    emitting.StopFrequency_MHz = trace.Freq_Hz[stop] / 1000000.0;
                    emitting.Spectrum.Levels_dBm = new float[stop_ - start_];
                    Array.Copy(levels, start, emitting.Spectrum.Levels_dBm, 0, stop_ - start_);
                    emitting.Spectrum.SpectrumStartFreq_MHz = trace.Freq_Hz[start_] / 1000000.0;  
                    emitting.Spectrum.SpectrumSteps_kHz = stepBW_kHz;
                    emitting.Spectrum.Levels_dBm = templevel;
                    emitting.Spectrum.СorrectnessEstimations = false;
                }
                emitting.ReferenceLevel_dBm = 0;
                emitting.CurentPower_dBm = 0;
                for (int j = start; j < stop; j++)
                {
                    emitting.ReferenceLevel_dBm = emitting.ReferenceLevel_dBm + Math.Pow(10, refLevel.levels[j] / 10);
                    emitting.CurentPower_dBm = emitting.CurentPower_dBm + Math.Pow(10, levels[j] / 10);
                }

                emitting.ReferenceLevel_dBm = 10 * Math.Log10(emitting.ReferenceLevel_dBm);
                emitting.CurentPower_dBm = 10 * Math.Log10(emitting.CurentPower_dBm);
                emitting.WorkTimes = new WorkTime[1];
                emitting.WorkTimes[0] = new WorkTime();
                emitting.WorkTimes[0].StartEmitting = DateTime.Now;
                emitting.WorkTimes[0].StopEmitting = emitting.WorkTimes[0].StartEmitting;
                emitting.WorkTimes[0].HitCount = 1;
                emitting.WorkTimes[0].PersentAvailability = 100;
                emitting.WorkTimes[0].ScanCount = 0;
                emitting.WorkTimes[0].TempCount= 0;

                Spectrum spectrum = emitting.Spectrum;
                bool checkcontr = CalcSignalization.CheckContravention(ref spectrum, refLevel);
                emitting.Spectrum = spectrum;
                //emitting.WorkTimes[0].PersentAvailability = 100;
                emittings.Add(emitting);
            }
            return emittings.ToArray();
        }
        private static Emitting[] CreateEmittingsForChannelPlanGSM(MesureTraceResult trace, ReferenceLevels refLevel, List<double> Freq_Mhz, double ChannelBW_kHz, List<int> IndexChannel, double TraceStep_kHz, double NoiseLevel_dBm)
        { // не тестировалось. Задача создать emmiting там где есть соответсвующее преевышение

            float[] levels = trace.Level;
            double[] freqTr_Hz = trace.Freq_Hz;
            var emittings = new List<Emitting>();
            for (int i = 0; i < IndexChannel.Count; i = i + 1)
            {
                // для каждого излучения вычислим стартовые точки и проверим их наличие на трейсе 
                int start = (int)Math.Floor(((Freq_Mhz[IndexChannel[i]] * 1000000.0 - ChannelBW_kHz * 1000.0)-freqTr_Hz[0])/(TraceStep_kHz*1000.0));
                int stop = (int)Math.Ceiling(((Freq_Mhz[IndexChannel[i]] * 1000000.0 + ChannelBW_kHz * 1000.0) - freqTr_Hz[0]) / (TraceStep_kHz * 1000.0));
                int start_ = start; int stop_ = stop;
                // определяем границы для канала
                double winBW = (stop - start) * windowBW; 
                if ((start + stop - winBW) / 2 > 0) { start_ = (int)((start + stop - winBW) / 2.0); } else { start_ = 0; }
                if ((start + stop + winBW) / 2 < levels.Length - 1) { stop_ = (int)((start + stop + winBW) / 2.0); } else { stop_ = levels.Length - 1; }
                // границы находяться в start_  и stop_ 

                Emitting emitting = new Emitting();
                float[] templevel = new float[stop_ - start_];
                Array.Copy(levels, start_, templevel, 0, stop_ - start_);
                MeasBandwidthResult measSdrBandwidthResults = BandWidthEstimation.GetBandwidthPoint(templevel, BandWidthEstimation.BandwidthEstimationType.xFromCentr, DiffLevelForCalcBW, 0);
                emitting.Spectrum = new Spectrum();
                if (measSdrBandwidthResults.СorrectnessEstimations != null)
                {
                    if (measSdrBandwidthResults.СorrectnessEstimations.Value)
                    {
                        // значит спектр хороший можно брать его параметры
                        if (measSdrBandwidthResults.T1 != null) { start = start_ + measSdrBandwidthResults.T1.Value; emitting.Spectrum.T1 = measSdrBandwidthResults.T1.Value; }
                        if (measSdrBandwidthResults.T2 != null) { stop = start_ + measSdrBandwidthResults.T2.Value; emitting.Spectrum.T2 = measSdrBandwidthResults.T2.Value; }
                        if (measSdrBandwidthResults.MarkerIndex != null) { emitting.Spectrum.MarkerIndex = measSdrBandwidthResults.MarkerIndex.Value; }
                        emitting.Spectrum.Bandwidth_kHz = (stop - start) * TraceStep_kHz;
                        emitting.Spectrum.СorrectnessEstimations = true;
                    }
                    else
                    { // если нет коректного то хоть какойто.
                            emitting.Spectrum.СorrectnessEstimations = false;
                    }
                }
                else
                {
                    emitting.Spectrum.СorrectnessEstimations = false;
                }
                emitting.Spectrum.Levels_dBm = templevel;
                emitting.Spectrum.SpectrumSteps_kHz = TraceStep_kHz;
                emitting.Spectrum.SpectrumStartFreq_MHz = freqTr_Hz[start_] / 1000000.0;
                emitting.StartFrequency_MHz = trace.Freq_Hz[start] / 1000000.0; // частоты сигнала по каналу
                emitting.StopFrequency_MHz = trace.Freq_Hz[stop] / 1000000.0; // частоты сигнала по каналу

                emitting.ReferenceLevel_dBm = 0;
                emitting.CurentPower_dBm = 0;
                for (int j = start; j < stop; j++)
                {
                    emitting.ReferenceLevel_dBm = emitting.ReferenceLevel_dBm + Math.Pow(10, refLevel.levels[j] / 10);
                    emitting.CurentPower_dBm = emitting.CurentPower_dBm + Math.Pow(10, levels[j] / 10);
                }

                emitting.ReferenceLevel_dBm = 10 * Math.Log10(emitting.ReferenceLevel_dBm);
                emitting.CurentPower_dBm = 10 * Math.Log10(emitting.CurentPower_dBm);
                emitting.WorkTimes = new WorkTime[1];
                emitting.WorkTimes[0] = new WorkTime();
                emitting.WorkTimes[0].StartEmitting = DateTime.Now;
                emitting.WorkTimes[0].StopEmitting = emitting.WorkTimes[0].StartEmitting;
                emitting.WorkTimes[0].HitCount = 1;
                emitting.WorkTimes[0].PersentAvailability = 100;
                emitting.WorkTimes[0].ScanCount = 0;
                emitting.WorkTimes[0].TempCount = 0;

                Spectrum spectrum = emitting.Spectrum;
                bool checkcontr = CalcSignalization.CheckContravention(ref spectrum, refLevel);
                emitting.Spectrum = spectrum;
                //emitting.WorkTimes[0].PersentAvailability = 100;
                emittings.Add(emitting);
            }
            return emittings.ToArray();
        }



    }
}
