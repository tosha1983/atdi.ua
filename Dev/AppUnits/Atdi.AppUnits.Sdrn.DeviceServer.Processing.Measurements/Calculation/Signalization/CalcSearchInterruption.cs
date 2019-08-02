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
        private static double windowBW = 1.5;
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
        /// Команда должна вернуть индексы каналов в которых имеет смысл анализировать сигнал. По принципу, что в соеднем канале уровень ниже на некий уровень. Удобно подходит для GSM сетей.
        /// </summary>
        /// <param name="Trace"></param>
        /// <param name="ChCentrFreqs_Mhz">центральные частоты каналов, обязательно должны быть отсортированы по возоастанию и без повторений !!!</param>
        /// <param name="BWChalnel_kHz"></param>
        /// <param name="NoiseLevel_dBm"></param>
        /// <param name="GoodChannel">Каналы вероятно хорошего качества, которые имеет смысл потом анализировать</param> 
        /// <param name="AvarageChannel">Каналы среднего качества, проанализировать можно, но второй сорт</param>
        /// <param name="DifferencePowerInCoChannel_dB"></param>
        /// <param name="DifferencePowerInCoChannelForJustAnalize_dB"></param>
        private static void SearchStartStopByChannelPlanGSM(MesureTraceResult Trace, List<double> ChCentrFreqs_Mhz, double BWChalnel_kHz, double NoiseLevel_dBm, out List<int> GoodChannel, out List<int> AverageChannel, double DifferencePowerInCoChannel_dB = 15, double DifferencePowerInCoChannelForJustAnalize_dB = 0)
        {// не тестилось
            // константа 
            int PersentSpectrumInCentrForCalcPowInChannelForComparePow = 10;
            // константа 
            GoodChannel = new List<int>();
            AverageChannel = new List<int>();
            if (DifferencePowerInCoChannel_dB < DifferencePowerInCoChannelForJustAnalize_dB)
            { double temp = DifferencePowerInCoChannel_dB; DifferencePowerInCoChannel_dB = DifferencePowerInCoChannelForJustAnalize_dB; DifferencePowerInCoChannelForJustAnalize_dB = temp; }

            // когда нет каналов или один канал
            if (ChCentrFreqs_Mhz is null) { return;}
            if (ChCentrFreqs_Mhz.Count == 1) { GoodChannel.Add(0); return;}

            double Freq_start_Hz = Trace.Freq_Hz[0];
            double step = (Trace.Freq_Hz[Trace.Freq_Hz.Length-1] - Trace.Freq_Hz[0]) / (Trace.Freq_Hz.Length - 1);

            // когда два канала 
            double PowFirstCh = CalcPowInChannel(ref Trace.Level,Freq_start_Hz, step, ChCentrFreqs_Mhz[0], BWChalnel_kHz, PersentSpectrumInCentrForCalcPowInChannelForComparePow);
            double PowSecondCh = CalcPowInChannel(ref Trace.Level, Freq_start_Hz, step, ChCentrFreqs_Mhz[1], BWChalnel_kHz, PersentSpectrumInCentrForCalcPowInChannelForComparePow);
            if (ChCentrFreqs_Mhz.Count == 2)
            {
                if (PowSecondCh - DifferencePowerInCoChannel_dB >= PowFirstCh) { GoodChannel.Add(1);}
                else if (PowSecondCh - DifferencePowerInCoChannelForJustAnalize_dB >= PowFirstCh) { AverageChannel.Add(1);}
                if (PowFirstCh - DifferencePowerInCoChannel_dB > PowSecondCh) { GoodChannel.Add(0);}
                else if (PowFirstCh - DifferencePowerInCoChannelForJustAnalize_dB > PowSecondCh) { AverageChannel.Add(0); }
                return;
            }
            // если каналов больше чем 2
            if (PowFirstCh - DifferencePowerInCoChannel_dB >= PowSecondCh) { GoodChannel.Add(0);}
            else if (PowFirstCh - DifferencePowerInCoChannelForJustAnalize_dB >= PowSecondCh) {AverageChannel.Add(0); }
            for (int i = 2; ChCentrFreqs_Mhz.Count > i; i++)
            {
                double PowLastCh = CalcPowInChannel(ref Trace.Level, Freq_start_Hz, step, ChCentrFreqs_Mhz[i], BWChalnel_kHz, PersentSpectrumInCentrForCalcPowInChannelForComparePow);
                if ((PowSecondCh - DifferencePowerInCoChannel_dB >= PowFirstCh) && (PowSecondCh - DifferencePowerInCoChannel_dB >= PowLastCh)) { GoodChannel.Add(i - 1); }
                else if ((PowSecondCh - DifferencePowerInCoChannelForJustAnalize_dB >= PowFirstCh) && (PowSecondCh - DifferencePowerInCoChannelForJustAnalize_dB >= PowLastCh)) { AverageChannel.Add(i - 1);}
                PowFirstCh = PowSecondCh;
                PowSecondCh = PowLastCh;
            }
            if (PowSecondCh - DifferencePowerInCoChannel_dB >= PowFirstCh) { GoodChannel.Add(ChCentrFreqs_Mhz.Count-1);}
            else if (PowSecondCh - DifferencePowerInCoChannelForJustAnalize_dB >= PowFirstCh) { AverageChannel.Add(ChCentrFreqs_Mhz.Count - 1);}
            return;
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
        private static Emitting[] CreateEmittingsForChannelPlanGSM(MesureTraceResult trace, ReferenceLevels refLevel, List<double> ChCentrFreqs_Mhz, double ChannelBW_kHz, List<int> IndexGoodChannel, List<int> IndexAvarageChannel, double NoiseLevel_dBm)
        { // НЕ ТЕСТИРОВАЛОСЬ. Задача создать emmiting там где есть соответсвующее преевышение
            bool CorrelationAnalize = true; 
            double CorrelationFactor = 0.9;


            double StartTraceFreq_Hz = trace.Freq_Hz[0];
            double TraceStep_Hz = (trace.Freq_Hz[trace.Freq_Hz.Length - 1] - trace.Freq_Hz[0]) / (trace.Freq_Hz.Length - 1);

            float[] levels = trace.Level;
            double[] freqTr_Hz = trace.Freq_Hz;
            var emittings = new List<Emitting>();
            // идем по хорошим каналам
            for (int i = 0; i < IndexGoodChannel.Count; i++)
            {
                // для каждого излучения вычислим стартовые точки и проверим их наличие на трейсе 
                int start = (int)Math.Floor(((ChCentrFreqs_Mhz[IndexGoodChannel[i]] * 1000000.0 - ChannelBW_kHz * 1000.0)-freqTr_Hz[0])/(TraceStep_Hz));
                int stop = (int)Math.Ceiling(((ChCentrFreqs_Mhz[IndexGoodChannel[i]] * 1000000.0 + ChannelBW_kHz * 1000.0) - freqTr_Hz[0]) / (TraceStep_Hz));
                int start_ = start; int stop_ = stop;
                // определяем границы для канала
                double winBW = (stop - start) * windowBW; 
                if ((start + stop - winBW) / 2 > 0) { start_ = (int)((start + stop - winBW) / 2.0); } else { start_ = 0; }
                if ((start + stop + winBW) / 2 < levels.Length - 1) { stop_ = (int)((start + stop + winBW) / 2.0); } else { stop_ = levels.Length - 1; }
                // границы находяться в start_  и stop_ 

                Emitting emitting = new Emitting();
                float[] templevel = new float[stop_ - start_];
                // выделение массива для излучения
                Array.Copy(levels, start_, templevel, 0, stop_ - start_);
                bool ErrorCorrelationEstim = false;

                emitting.Spectrum = new Spectrum();
                if (CorrelationAnalize)
                {
                    int MarkerIndex = CorrelationAnalyzeForEstimationFreq(templevel, StartTraceFreq_Hz + start_* TraceStep_Hz, TraceStep_Hz, out double BestCorr, out double StandardBW_Hz, out string standard);
                    if (BestCorr <= CorrelationFactor)
                    {// Супер сигнал идентифицирован частота определена, значит спектр хороший можно брать его параметры
                        emitting.Spectrum.MarkerIndex = MarkerIndex;
                        emitting.Spectrum.T1 = (int)Math.Floor(MarkerIndex - (StandardBW_Hz / (2.0* TraceStep_Hz)));
                        emitting.Spectrum.T2 = (int)Math.Ceiling(MarkerIndex + (StandardBW_Hz / (2.0 * TraceStep_Hz))); 
                        emitting.Spectrum.Bandwidth_kHz = StandardBW_Hz/1000.0;
                        emitting.Spectrum.СorrectnessEstimations = true;
                        emitting.EmittingParameters.StandardBW = StandardBW_Hz;
                        emitting.EmittingParameters.Standard = standard;
                        emitting.MeanDeviationFromReference = BestCorr;
                        emitting.TriggerDeviationFromReference = CorrelationFactor;
                        start = start_ + emitting.Spectrum.T1;
                        stop = start_ + emitting.Spectrum.T2;
                    }
                    else
                    {// Cигнал не идентифицирован частота не определена
                        emitting.Spectrum.СorrectnessEstimations = false;
                        emitting.MeanDeviationFromReference = BestCorr;
                        emitting.TriggerDeviationFromReference = CorrelationFactor;
                        ErrorCorrelationEstim = true;
                    }
                }
                if ((!CorrelationAnalize) ||(ErrorCorrelationEstim))
                {
                    MeasBandwidthResult measSdrBandwidthResults = BandWidthEstimation.GetBandwidthPoint(templevel, BandWidthEstimation.BandwidthEstimationType.xFromCentr);
                    if (measSdrBandwidthResults.СorrectnessEstimations != null)
                    {
                        if (measSdrBandwidthResults.СorrectnessEstimations.Value)
                        {
                            // значит спектр хороший можно брать его параметры
                            if (measSdrBandwidthResults.T1 != null) { start = start_ + measSdrBandwidthResults.T1.Value; emitting.Spectrum.T1 = measSdrBandwidthResults.T1.Value; }
                            if (measSdrBandwidthResults.T2 != null) { stop = start_ + measSdrBandwidthResults.T2.Value; emitting.Spectrum.T2 = measSdrBandwidthResults.T2.Value; }
                            if (measSdrBandwidthResults.MarkerIndex != null) { emitting.Spectrum.MarkerIndex = measSdrBandwidthResults.MarkerIndex.Value; }
                            emitting.Spectrum.Bandwidth_kHz = (stop - start) * (TraceStep_Hz/1000.0);
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
                }
                emitting.Spectrum.Levels_dBm = templevel;
                emitting.Spectrum.SpectrumSteps_kHz = TraceStep_Hz/1000.0;
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
                emittings.Add(emitting);
            }
            return emittings.ToArray();
        }
        private static double CalcPowInChannel(ref float[] Level_dBm, double Freq_start_Hz, double step_Hz, double CentralChannelFreq_MHz, double ChannelBW_kHz, int persentpoints = 0)
        { // НЕ ПРОВЕРЕННО
            if (persentpoints == 0)
            { // расчет идет только для центрального канала 

                int IndexCenterLastChannel = (int)Math.Round((CentralChannelFreq_MHz * 1000000 - Freq_start_Hz) / step_Hz);
                if (IndexCenterLastChannel >= Level_dBm.Length) { return -999;}
                double Pow = Level_dBm[IndexCenterLastChannel];
                return Pow;
            }
            if ((persentpoints <= 100)&&(persentpoints > 0))
            { // 
                int numberpoint = (int)Math.Floor(ChannelBW_kHz * 10.0 * persentpoints / step_Hz);
                if (numberpoint < 1) { numberpoint = 1;}
                int IndexCenterLastChannel = (int)Math.Round((CentralChannelFreq_MHz * 1000000 - Freq_start_Hz) / step_Hz);
                int shift = (int)Math.Floor(numberpoint / 2.0);
                if ((IndexCenterLastChannel + numberpoint - shift >= Level_dBm.Length)|| (IndexCenterLastChannel - shift < 0)) { return -999;}
                double Pow_mW = 0;
                for (int i = 0; numberpoint > i; i++)
                {
                    Pow_mW = Math.Pow(10, Level_dBm[i-shift+IndexCenterLastChannel]/10) + Pow_mW;
                }
                return 10 * Math.Log10(Pow_mW);
            }
            return -999;
        }
        private static int CorrelationAnalyzeForEstimationFreq(float[] Level_dBm, double StartFreq_Hz, double Step_Hz, out double BestCorr, out double StandartBW_Hz, out string Standard)
        { // НЕ ТЕСТИРОВАЛОСЬ Важно Данная функция должна определить центральную частоту (ее индекс) на основании сопоставления с эталонными сигналами пока сравнение идет только с GSM
            const double GSMWindowAnalize_Hz = 200000;
            double AllBW = Step_Hz * (Level_dBm.Length - 1);
            if ((AllBW >=150000)&&(AllBW <= 400000))
            {// есть подозрение что это GSM
                // производим сравнение с GSM
                int indexStart = (int)Math.Floor(((Level_dBm.Length - 1) / 2.0) - ((GSMWindowAnalize_Hz / 2.0)/Step_Hz));
                if (indexStart < 0) {indexStart = 0;};
                int indexStop = (int)Math.Ceiling(((Level_dBm.Length - 1) / 2.0) + ((GSMWindowAnalize_Hz / 2.0) / Step_Hz));
                if (indexStop >= Level_dBm.Length) { indexStop = Level_dBm.Length; };

                float[] SignalAnalizeLevel = new float[indexStop - indexStart];
                double SignalAnalizeFreq_Hz = StartFreq_Hz + indexStart * Step_Hz;
                Array.Copy(Level_dBm, indexStart, SignalAnalizeLevel, 0, indexStop - indexStart);


                float[] LevelsRef = ReferenceSpectrum.GSMLevels;
                double CentrFreq_Hz = StartFreq_Hz + (AllBW/2.0);
                double RefFreq_Hz = CentrFreq_Hz - ReferenceSpectrum.GSMCentralIndex * ReferenceSpectrum.GSMStep_Hz;
                int shift = СorrelationСoefficient.CalcShiftSpectrum(Level_dBm, SignalAnalizeFreq_Hz, Step_Hz, ReferenceSpectrum.GSMLevels,
                    RefFreq_Hz, ReferenceSpectrum.GSMStep_Hz, СorrelationСoefficient.MethodCalcCorrelation.Person, out BestCorr, 30);
                double MeasCentrFreq_Hz = CentrFreq_Hz + shift * Math.Max(Step_Hz, ReferenceSpectrum.GSMStep_Hz);
                int MarkerIndex = (int)Math.Round((MeasCentrFreq_Hz - StartFreq_Hz) / Step_Hz);
                StandartBW_Hz = 271000;
                Standard = "GSM";
                return MarkerIndex;
            }
            BestCorr = 0;
            StandartBW_Hz = 0;
            Standard = null;
            return -9999999;
        }



    }
}
