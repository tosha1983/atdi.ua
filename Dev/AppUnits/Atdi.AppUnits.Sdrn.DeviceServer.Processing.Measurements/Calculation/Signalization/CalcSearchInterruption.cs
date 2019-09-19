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
        private static int NumberPointForChangeExcess; // на самом деле зависит от параметров таска там будем вычислять и прокидывать сюда.
        private static double allowableExcess_dB;
        private static double DiffLevelForCalcBW;
        private static double windowBW;
        private static double nDbLevel_dB;
        private static int NumberIgnoredPoints;
        private static double MinExcessNoseLevel_dB;
        private static bool AnalyzeByChannel;
        private static bool CorrelationAnalize;
        private static bool CheckFreqChannel;
        private static double CorrelationFactor;
        private static bool FiltrationTrace;
        private static bool CompareTraceJustWithRefLevels;
        private static bool AutoDivisionEmitting;
        private static double DifferenceMaxMax;

        private static double MaxFreqDeviation;// = 0.00001;
        private static bool ChackLevelChannel;//; = true;
        private static int MinPointForDetailBW;// = 300;
        
        // Конец констант

        /// <summary>
        /// Сопоставляем излучения реальные с реферативными уровнями. В случае превышения формируем излучение. 
        /// </summary>
        /// <param name="referenceLevels"></param>
        /// <param name="Trace"></param>
        /// <returns></returns>
        public static Emitting[] Calc(TaskParameters taskParameters, ReferenceLevels refLevels, MesureTraceResult Trace, double NoiseLevel_dBm, List<double> ChCentrFreqs_Mhz = null, double BWChalnel_kHz = 200)
        { //НЕ ТЕСТИРОВАННО

            NumberPointForChangeExcess = taskParameters.SignalingMeasTaskParameters.InterruptionParameters.NumberPointForChangeExcess.Value;
            allowableExcess_dB = taskParameters.SignalingMeasTaskParameters.allowableExcess_dB.Value;
            DiffLevelForCalcBW = taskParameters.SignalingMeasTaskParameters.InterruptionParameters.DiffLevelForCalcBW.Value;
            windowBW = taskParameters.SignalingMeasTaskParameters.InterruptionParameters.windowBW.Value;
            nDbLevel_dB = taskParameters.SignalingMeasTaskParameters.InterruptionParameters.nDbLevel_dB.Value;
            NumberIgnoredPoints = taskParameters.SignalingMeasTaskParameters.InterruptionParameters.NumberIgnoredPoints.Value;
            MinExcessNoseLevel_dB = taskParameters.SignalingMeasTaskParameters.InterruptionParameters.MinExcessNoseLevel_dB.Value;
            AnalyzeByChannel = taskParameters.SignalingMeasTaskParameters.AnalyzeByChannel.Value;
            CorrelationAnalize = taskParameters.SignalingMeasTaskParameters.CorrelationAnalize.Value;
            CorrelationFactor = taskParameters.SignalingMeasTaskParameters.CorrelationFactor.Value;
            FiltrationTrace = taskParameters.SignalingMeasTaskParameters.FiltrationTrace.Value;
            CheckFreqChannel = taskParameters.SignalingMeasTaskParameters.CheckFreqChannel.Value;
            CompareTraceJustWithRefLevels = taskParameters.SignalingMeasTaskParameters.CompareTraceJustWithRefLevels.Value;
            AutoDivisionEmitting = taskParameters.SignalingMeasTaskParameters.InterruptionParameters.AutoDivisionEmitting.Value;
            DifferenceMaxMax = taskParameters.SignalingMeasTaskParameters.InterruptionParameters.DifferenceMaxMax.Value;
            MaxFreqDeviation = taskParameters.SignalingMeasTaskParameters.InterruptionParameters.MaxFreqDeviation.Value;
            ChackLevelChannel = taskParameters.SignalingMeasTaskParameters.InterruptionParameters.CheckLevelChannel.Value;
            MinPointForDetailBW = taskParameters.SignalingMeasTaskParameters.InterruptionParameters.MinPointForDetailBW.Value;

            if (refLevels.levels.Length != Trace.Level.Length)
            {
                return null; // выход по причине несовпадения количества точек следовательно необходимо перерасчитать CalcReferenceLevels 
            }
            if (AnalyzeByChannel)
            {
                if (ChCentrFreqs_Mhz == null) { return null; }
                // выделение мест где произошло превышение порога 
                List<int> index_start_stop = new List<int>();
                SearchStartStopByChannelPlanGSM(Trace, ChCentrFreqs_Mhz, BWChalnel_kHz, NoiseLevel_dBm, out List<int> GoodChannel, out List<int> AverageChannel);
                // конец выделения 
                //Формируем помехи.
                double stepBW_kHz = (Trace.Freq_Hz[Trace.Freq_Hz.Length - 1] - Trace.Freq_Hz[0]) / ((Trace.Freq_Hz.Length - 1) * 1000.0);
                Emitting[] newEmittings = CreateEmittingsForChannelPlanGSM(Trace, refLevels, ChCentrFreqs_Mhz, BWChalnel_kHz, GoodChannel, AverageChannel);
                // сформировали новые параметры излучения теперь надо накатить старые по идее.
                return newEmittings;
            }
            else
            {
                // выделение мест где произошло превышение порога 
                List<int> index_start_stop = new List<int>();
                if (CompareTraceJustWithRefLevels)
                {
                    index_start_stop = SearchStartStopCompaireWithRefLevels(refLevels, Trace, NoiseLevel_dBm, NumberPointForChangeExcess);
                }
                else
                {
                    index_start_stop = SearchStartStopCompaireWithNoiseLevels(refLevels, Trace, NoiseLevel_dBm, NumberPointForChangeExcess);
                }
                // конец выделения 
                //возможно необходимо произвести разделение нескольких излучений которые моги быть ошибочно восприняты как одно
                if (AutoDivisionEmitting) { index_start_stop = DivisionEmitting(taskParameters, index_start_stop, Trace); }

                //Формируем помехи.
                double stepBW_kHz = (Trace.Freq_Hz[Trace.Freq_Hz.Length - 1] - Trace.Freq_Hz[0]) / ((Trace.Freq_Hz.Length - 1) * 1000.0);
                Emitting[] newEmittings = CreateEmittings(Trace, refLevels, index_start_stop, stepBW_kHz, NoiseLevel_dBm, CompareTraceJustWithRefLevels);
                // сформировали новые параметры излучения теперь надо накатить старые по идее.
                return newEmittings;
            }

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
        private static void SearchStartStopByChannelPlanGSM(MesureTraceResult Trace, List<double> ChCentrFreqs_Mhz, double BWChalnel_kHz, double NoiseLevel_dBm, out List<int> GoodChannel, out List<int> AverageChannel, double DifferencePowerInCoChannel_dB = 15, double DifferencePowerInCoChannelForJustAnalize_dB = 5)
        {// (вроде как работает)
            // константа 
            int PersentSpectrumInCentrForCalcPowInChannelForComparePow = 10;
            // константа 
            GoodChannel = new List<int>();
            AverageChannel = new List<int>();
            if (DifferencePowerInCoChannel_dB < DifferencePowerInCoChannelForJustAnalize_dB)
            { double temp = DifferencePowerInCoChannel_dB; DifferencePowerInCoChannel_dB = DifferencePowerInCoChannelForJustAnalize_dB; DifferencePowerInCoChannelForJustAnalize_dB = temp; }

            // когда или один канал
            if ((ChCentrFreqs_Mhz == null) || (ChCentrFreqs_Mhz.Count==0)) { return; }

            // когда один канал nu
            double step = (Trace.Freq_Hz[Trace.Freq_Hz.Length - 1] - Trace.Freq_Hz[0]) / (Trace.Freq_Hz.Length - 1);
            double Freq_start_Hz = Trace.Freq_Hz[0];
            if (ChCentrFreqs_Mhz.Count == 1)
            {   double Pow = CalcPowInChannel(ref Trace.Level, Freq_start_Hz, step, ChCentrFreqs_Mhz[0], BWChalnel_kHz, NoiseLevel_dBm, out double TrigerLevel_dBmCh, PersentSpectrumInCentrForCalcPowInChannelForComparePow);
                if (Pow > TrigerLevel_dBmCh+10) { GoodChannel.Add(0); }
                else if (Pow > TrigerLevel_dBmCh) { AverageChannel.Add(0); } return;}


            // когда два канала 
            double PowFirstCh = CalcPowInChannel(ref Trace.Level, Freq_start_Hz, step, ChCentrFreqs_Mhz[0], BWChalnel_kHz, NoiseLevel_dBm, out double TrigerLevel_dBmFirstCh, PersentSpectrumInCentrForCalcPowInChannelForComparePow);
            double PowSecondCh = CalcPowInChannel(ref Trace.Level, Freq_start_Hz, step, ChCentrFreqs_Mhz[1], BWChalnel_kHz, NoiseLevel_dBm, out double TrigerLevel_dBmSecondCh, PersentSpectrumInCentrForCalcPowInChannelForComparePow);
            if (ChCentrFreqs_Mhz.Count == 2)
            {
                if (PowSecondCh - DifferencePowerInCoChannel_dB >= PowFirstCh) { GoodChannel.Add(1); }
                else if (PowSecondCh - DifferencePowerInCoChannelForJustAnalize_dB >= PowFirstCh) { if (PowSecondCh > TrigerLevel_dBmSecondCh) { AverageChannel.Add(1); } }
                if (PowFirstCh - DifferencePowerInCoChannel_dB > PowSecondCh) { GoodChannel.Add(0); }
                else if (PowFirstCh - DifferencePowerInCoChannelForJustAnalize_dB > PowSecondCh) { if (PowFirstCh > TrigerLevel_dBmFirstCh) { AverageChannel.Add(0); } }
                return;
            }
            // если каналов больше чем 2
            if (PowFirstCh - DifferencePowerInCoChannel_dB >= PowSecondCh) { GoodChannel.Add(0); }
            else if (PowFirstCh - DifferencePowerInCoChannelForJustAnalize_dB >= PowSecondCh) { if (PowFirstCh > TrigerLevel_dBmFirstCh) { AverageChannel.Add(0); } }
            for (int i = 2; ChCentrFreqs_Mhz.Count > i; i++)
            {
                double PowLastCh = CalcPowInChannel(ref Trace.Level, Freq_start_Hz, step, ChCentrFreqs_Mhz[i], BWChalnel_kHz, NoiseLevel_dBm, out double TrigerLevel_dBmLastCh, PersentSpectrumInCentrForCalcPowInChannelForComparePow);
                if ((PowSecondCh - DifferencePowerInCoChannel_dB >= PowFirstCh) && (PowSecondCh - DifferencePowerInCoChannel_dB >= PowLastCh)) {
                    GoodChannel.Add(i - 1); }
                else if ((PowSecondCh - DifferencePowerInCoChannelForJustAnalize_dB >= PowFirstCh) && (PowSecondCh - DifferencePowerInCoChannelForJustAnalize_dB >= PowLastCh)) {
                    if (PowSecondCh > TrigerLevel_dBmSecondCh) {AverageChannel.Add(i-1);}
                }
                PowFirstCh = PowSecondCh; TrigerLevel_dBmFirstCh = TrigerLevel_dBmSecondCh;
                PowSecondCh = PowLastCh; TrigerLevel_dBmSecondCh = TrigerLevel_dBmLastCh;
            }
            if (PowSecondCh - DifferencePowerInCoChannel_dB >= PowFirstCh) { GoodChannel.Add(ChCentrFreqs_Mhz.Count - 1); }
            else if (PowSecondCh - DifferencePowerInCoChannelForJustAnalize_dB >= PowFirstCh) { if (PowSecondCh > TrigerLevel_dBmSecondCh) { AverageChannel.Add(ChCentrFreqs_Mhz.Count - 1); } }
            return;
        }
        private static List<int> DivisionEmitting(TaskParameters taskParameters, List<int> index_start_stop, MesureTraceResult Trace)
        { // 
            List<int> ResultStartStopIndexArr = new List<int>();
            for (int i = 0; i < index_start_stop.Count - 1; i = i + 2)
            {
                int[] StartStopAnalized;
                int count = EmissionCounting.Counting(Trace.Level, index_start_stop[i], index_start_stop[i + 1], out StartStopAnalized, DifferenceMaxMax, FiltrationTrace);
                for (int j = 0; j < StartStopAnalized.Length; j++)
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
                float[] TemplevelForAnalize = templevel;
                if (FiltrationTrace) { TemplevelForAnalize = SmoothTrace.blackman(templevel); }
                MeasBandwidthResult measSdrBandwidthResults = BandWidthEstimation.GetBandwidthPoint(TemplevelForAnalize, BandWidthEstimation.BandwidthEstimationType.xFromCentr, DiffLevelForCalcBW, NumberIgnoredPoints);
                emitting.Spectrum = new Spectrum();
                if (measSdrBandwidthResults.СorrectnessEstimations != null)
                {
                    if (measSdrBandwidthResults.СorrectnessEstimations.Value)
                    {
                        // значит спектр хороший можно брать его параметры
                        if (measSdrBandwidthResults.T1 != null) { start = start_ + measSdrBandwidthResults.T1.Value; emitting.Spectrum.T1 = measSdrBandwidthResults.T1.Value; }
                        if (measSdrBandwidthResults.T2 != null) { stop = start_ + measSdrBandwidthResults.T2.Value; emitting.Spectrum.T2 = measSdrBandwidthResults.T2.Value; }
                        if (measSdrBandwidthResults.MarkerIndex != null) { emitting.Spectrum.MarkerIndex = measSdrBandwidthResults.MarkerIndex.Value; }
                        // установим 
                        emitting.Spectrum.Bandwidth_kHz = (stop - start) * stepBW_kHz;
                        emitting.Spectrum.СorrectnessEstimations = true;
                        if (stop - start > MinPointForDetailBW) { emitting.SpectrumIsDetailed = true; emitting.LastDetaileMeas = DateTime.Now; }
                    }
                    else
                    {// попробуем пройтись по масиву с помощью метода nDbDown и всеже определить ширину спектра
                        int startreal = -1;
                        int stopreal = -1;
                        bool CorectCalcBW = CalcBWSignalization.CalcBW(levels, start, stop, nDbLevel_dB, NoiseLevel_dBm, MinExcessNoseLevel_dB, NumberIgnoredPoints, ref startreal, ref stopreal);
                        if (CorectCalcBW)
                        {
                            // поиск коректной полосы 
                            if (CompareTraceJustWithRefLevels)
                            {
                                double tempBW = StandartBW.GetStandartBW_kHz((stopreal - startreal) * stepBW_kHz);
                                double CentralFreq = (trace.Freq_Hz[startreal] + trace.Freq_Hz[stopreal]) / 2000000.0;
                                emitting.StartFrequency_MHz = CentralFreq - tempBW / 2000;
                                emitting.StopFrequency_MHz = CentralFreq + tempBW / 2000;
                                start = (int)Math.Floor((emitting.StartFrequency_MHz - trace.Freq_Hz[0] / 1000000.0) / (stepBW_kHz / 1000));
                                stop = (int)Math.Ceiling((emitting.StopFrequency_MHz - trace.Freq_Hz[0] / 1000000.0) / (stepBW_kHz / 1000));
                                if (start < 0) { start = 0; }
                                if (stop >= levels.Length) { stop = levels.Length - 1; }
                            }
                            else
                            {
                                start = startreal;
                                stop = stopreal;
                            }
                            emitting.Spectrum.СorrectnessEstimations = false;
                        }
                        else
                        {
                            emitting.Spectrum.СorrectnessEstimations = false;
                        }
                    }
                }
                else
                {
                    emitting.Spectrum.СorrectnessEstimations = false;
                }
                emitting.Spectrum.SpectrumStartFreq_MHz = trace.Freq_Hz[start_] / 1000000.0;
                emitting.Spectrum.SpectrumSteps_kHz = stepBW_kHz;
                emitting.Spectrum.Levels_dBm = templevel;
                emitting.StartFrequency_MHz = trace.Freq_Hz[start] / 1000000.0;
                emitting.StopFrequency_MHz = trace.Freq_Hz[stop] / 1000000.0;
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
                if (ChackLevelChannel)
                {
                    bool checkcontr = CalcSignalization.CheckContravention(ref spectrum, refLevel);
                }
                emitting.Spectrum = spectrum;
                emittings.Add(emitting);
            }
            return emittings.ToArray();
        }
        private static Emitting[] CreateEmittingsForChannelPlanGSM(MesureTraceResult trace, ReferenceLevels refLevel, List<double> ChCentrFreqs_Mhz, double ChannelBW_kHz, List<int> IndexGoodChannel, List<int> IndexAvarageChannel)
        { // НЕ ТЕСТИРОВАЛОСЬ. Задача создать emmiting там где есть соответсвующее преевышение
            var emittings = new List<Emitting>();
            // идем по хорошим каналам
            var emittings1 = CreateEmittingsForChannelPlanGSM(trace, refLevel, ChCentrFreqs_Mhz, ChannelBW_kHz, IndexGoodChannel, true);
            emittings.AddRange(emittings1);
            var emittings2 = CreateEmittingsForChannelPlanGSM(trace, refLevel, ChCentrFreqs_Mhz, ChannelBW_kHz, IndexAvarageChannel, false);
            emittings.AddRange(emittings2);
            return emittings.ToArray();
        }
        private static Emitting[] CreateEmittingsForChannelPlanGSM(MesureTraceResult trace, ReferenceLevels refLevel, List<double> ChCentrFreqs_Mhz, double ChannelBW_kHz, List<int> IndexChannel, bool GoodChannel)
        { // НЕ ТЕСТИРОВАЛОСЬ первоначально тестировалось. Задача создать emmiting там где есть соответсвующее преевышение
            // константы 
            double persentBWforCorrelationAnalizeforGoodSignal = 120;
            double persentBWforCorrelationAnalizeforAvarageSignal = 100;
            // константы конец

            double persentBWforCorrelationAnalize;
            if (GoodChannel) { persentBWforCorrelationAnalize = persentBWforCorrelationAnalizeforGoodSignal;} else { persentBWforCorrelationAnalize = persentBWforCorrelationAnalizeforAvarageSignal; };

            double StartTraceFreq_Hz = trace.Freq_Hz[0];
            double TraceStep_Hz = (trace.Freq_Hz[trace.Freq_Hz.Length - 1] - trace.Freq_Hz[0]) / (trace.Freq_Hz.Length - 1);
            float[] levels = trace.Level;
            double[] freqTr_Hz = trace.Freq_Hz;
            var emittings = new List<Emitting>();
            // идем по каналам
            for (int i = 0; i < IndexChannel.Count; i++)
            {
                // для каждого излучения вычислим стартовые точки (излучения) и проверим их наличие на трейсе 
                int start = (int)Math.Floor(((ChCentrFreqs_Mhz[IndexChannel[i]] * 1000000.0 - ChannelBW_kHz * 500.0) - freqTr_Hz[0]) / (TraceStep_Hz));
                int stop = (int)Math.Ceiling(((ChCentrFreqs_Mhz[IndexChannel[i]] * 1000000.0 + ChannelBW_kHz * 500.0) - freqTr_Hz[0]) / (TraceStep_Hz));
                int start_ = start; int stop_ = stop;
                // определяем границы для спектра
                double winBW = (stop - start) * windowBW;
                if ((start + stop - winBW) / 2 > 0) { start_ = (int)((start + stop - winBW) / 2.0); } else { start_ = 0; }
                if ((start + stop + winBW) / 2 < levels.Length - 1) { stop_ = (int)((start + stop + winBW) / 2.0); } else { stop_ = levels.Length - 1; }
                // границы находяться в start_  и stop_ 

                Emitting emitting = new Emitting();
                // выделение массива для излучения
                float[] templevel = new float[stop_ - start_];
                Array.Copy(levels, start_, templevel, 0, stop_ - start_);


                emitting.Spectrum = new Spectrum();
                emitting.Spectrum.Contravention = false;
                emitting.EmittingParameters = new EmittingParameters();

                float[] TemplevelForAnalize = templevel;
                if (FiltrationTrace) { TemplevelForAnalize = SmoothTrace.blackman(templevel); }

                // определение BW 
                bool estimBW = false;
                bool estimFreq = false;
                bool ContraversionSignal = false;
                MeasBandwidthResult measSdrBandwidthResults = BandWidthEstimation.GetBandwidthPoint(TemplevelForAnalize, BandWidthEstimation.BandwidthEstimationType.xFromCentr, DiffLevelForCalcBW, NumberIgnoredPoints);
                if (measSdrBandwidthResults.СorrectnessEstimations != null)
                {
                    if (measSdrBandwidthResults.СorrectnessEstimations.Value)
                    { // Определели удачно ура, это полезный результат
                        // значит спектр хороший можно брать его параметры
                        if (measSdrBandwidthResults.T1 != null) { start = start_ + measSdrBandwidthResults.T1.Value; emitting.Spectrum.T1 = measSdrBandwidthResults.T1.Value; }
                        if (measSdrBandwidthResults.T2 != null) { stop = start_ + measSdrBandwidthResults.T2.Value; emitting.Spectrum.T2 = measSdrBandwidthResults.T2.Value; }
                        if (measSdrBandwidthResults.MarkerIndex != null) { emitting.Spectrum.MarkerIndex = measSdrBandwidthResults.MarkerIndex.Value; }
                        emitting.Spectrum.Bandwidth_kHz = (stop - start) * (TraceStep_Hz / 1000.0);
                        emitting.Spectrum.СorrectnessEstimations = true;
                        estimBW = true;
                        estimFreq = true;
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
                if (CorrelationAnalize)
                {
                    int start_A = start; int stop_A = stop;
                    winBW = (stop - start) * persentBWforCorrelationAnalize / 100.0;
                    if ((start + stop - winBW) / 2 > 0) { start_A = (int)((start + stop - winBW) / 2.0); } else { start_A = 0; }
                    if ((start + stop + winBW) / 2 < levels.Length - 1) { stop_A = (int)((start + stop + winBW) / 2.0); } else { stop_A = levels.Length - 1; }
                    float[] TempLevelForCorrelation = new float[stop_A - start_A];
                    Array.Copy(levels, start_A, TempLevelForCorrelation, 0, stop_A - start_A);
                    TempLevelForCorrelation = SmoothTrace.blackman(TempLevelForCorrelation);
                    if (estimBW)
                    {
                        double corr = CorrelationAnalyzeForCompairWithEtalon(TempLevelForCorrelation, StartTraceFreq_Hz + start_A * TraceStep_Hz, TraceStep_Hz, out double StandardBW_Hz, out string standard);
                        if (corr >= 0.75)
                        {
                            emitting.EmittingParameters.StandardBW = StandardBW_Hz;
                            emitting.EmittingParameters.Standard = standard;
                            emitting.MeanDeviationFromReference = corr;
                            emitting.TriggerDeviationFromReference = CorrelationFactor;
                        }
                        else
                        {// Cигнал не идентифицирован 
                            emitting.MeanDeviationFromReference = corr;
                            emitting.TriggerDeviationFromReference = CorrelationFactor;
                            ContraversionSignal = true;
                        }
                    }
                    else
                    {
                        int MarkerIndex = CorrelationAnalyzeForEstimationFreq(TempLevelForCorrelation, StartTraceFreq_Hz + start_A * TraceStep_Hz, TraceStep_Hz, out double BestCorr, out double StandardBW_Hz, out string standard);
                        if (BestCorr >= 0.75)//CorrelationFactor)
                        {// Супер сигнал идентифицирован частота определена 
                            MarkerIndex = MarkerIndex - start_ + start_A;
                            int T1 = (int)Math.Floor(MarkerIndex - (StandardBW_Hz / (2.0 * TraceStep_Hz)));
                            int T2 = (int)Math.Ceiling(MarkerIndex + (StandardBW_Hz / (2.0 * TraceStep_Hz)));
                            if ((T1 >= 0) && (T2 < TemplevelForAnalize.Length))
                            {
                                emitting.Spectrum.MarkerIndex = MarkerIndex;
                                emitting.Spectrum.T1 = T1;
                                emitting.Spectrum.T2 = T2;
                                emitting.Spectrum.Bandwidth_kHz = StandardBW_Hz / 1000.0;
                                estimBW = true;
                                estimFreq = true;
                            }
                            emitting.EmittingParameters.StandardBW = StandardBW_Hz;
                            emitting.EmittingParameters.Standard = standard;
                            emitting.MeanDeviationFromReference = BestCorr;
                            emitting.TriggerDeviationFromReference = CorrelationFactor;
                        }
                        else
                        {// Cигнал не идентифицирован частота не определена
                            emitting.MeanDeviationFromReference = BestCorr;
                            emitting.TriggerDeviationFromReference = CorrelationFactor;
                            ContraversionSignal = true;
                        }
                    }
                }
                bool Contravention_Freq = false;
                if ((CheckFreqChannel)&&(estimFreq))
                {
                    double FreqDiff = Math.Abs(((emitting.Spectrum.T1 + emitting.Spectrum.T2) / 2.0) * TraceStep_Hz + trace.Freq_Hz[start] - ChCentrFreqs_Mhz[IndexChannel[i]] * 1000000.0);
                    if (FreqDiff <= TraceStep_Hz / 2.0) { FreqDiff = 0; } else { FreqDiff = (FreqDiff - TraceStep_Hz / 2.0) / (ChCentrFreqs_Mhz[IndexChannel[i]] * 1000000); }
                    emitting.EmittingParameters.FreqDeviation = FreqDiff;
                    emitting.EmittingParameters.TriggerFreqDeviation = MaxFreqDeviation;
                    if (FreqDiff > MaxFreqDeviation){ Contravention_Freq = true; }
                }
                emitting.Spectrum.Levels_dBm = templevel;
                emitting.Spectrum.SpectrumSteps_kHz = TraceStep_Hz / 1000.0;
                emitting.Spectrum.SpectrumStartFreq_MHz = freqTr_Hz[start_] / 1000000.0;
                emitting.Spectrum.Contravention = Contravention_Freq || ContraversionSignal;

                if ((!emitting.Spectrum.Contravention)&&(ChackLevelChannel))
                {
                    Spectrum spectrum = emitting.Spectrum;
                    bool checkcontr = CalcSignalization.CheckContravention(ref spectrum, refLevel);
                    emitting.Spectrum = spectrum;
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
                emitting.StartFrequency_MHz = trace.Freq_Hz[start] / 1000000.0; // частоты сигнала по каналу
                emitting.StopFrequency_MHz = trace.Freq_Hz[stop] / 1000000.0; // частоты сигнала по каналу
                emitting.WorkTimes = new WorkTime[1];
                emitting.WorkTimes[0] = new WorkTime();
                emitting.WorkTimes[0].StartEmitting = DateTime.Now;
                emitting.WorkTimes[0].StopEmitting = emitting.WorkTimes[0].StartEmitting;
                emitting.WorkTimes[0].HitCount = 1;
                emitting.WorkTimes[0].PersentAvailability = 100;
                emitting.WorkTimes[0].ScanCount = 0;
                emitting.WorkTimes[0].TempCount = 0;
                emittings.Add(emitting);
                //if (emitting.Spectrum.СorrectnessEstimations == true)
                //{
                    //int hh = 0;
                //}
                //if (!(emitting.Spectrum.T2 >= emitting.Spectrum.T1 && emitting.Spectrum.T2 <= emitting.Spectrum.Levels_dBm.Length))
                //{
                 
              
                //}
            }
            return emittings.ToArray();
        }
        private static double CalcPowInChannel(ref float[] Level_dBm, double Freq_start_Hz, double step_Hz, double CentralChannelFreq_MHz, double ChannelBW_kHz, double NoiseLevel_dBm, out double TrigerLevel, int persentpoints = 0)
        { // НЕ ПРОВЕРЕННО (проверенно частично)
            if (persentpoints == 0)
            { // расчет идет только для центрального канала 
                TrigerLevel = NoiseLevel_dBm + allowableExcess_dB;
                int IndexCenterLastChannel = (int)Math.Round((CentralChannelFreq_MHz * 1000000 - Freq_start_Hz) / step_Hz);
                if (IndexCenterLastChannel >= Level_dBm.Length) { return -999; }
                double Pow = Level_dBm[IndexCenterLastChannel];
                return Pow;
            }
            if ((persentpoints <= 100) && (persentpoints > 0))
            { // 
                int numberpoint = (int)Math.Floor(ChannelBW_kHz * 10.0 * persentpoints / step_Hz);
                if (numberpoint < 1) { numberpoint = 1; }
                TrigerLevel = NoiseLevel_dBm + allowableExcess_dB + 10 * Math.Log10(numberpoint);
                int IndexCenterLastChannel = (int)Math.Round((CentralChannelFreq_MHz * 1000000 - Freq_start_Hz) / step_Hz);
                int shift = (int)Math.Floor(numberpoint / 2.0);
                if ((IndexCenterLastChannel + numberpoint - shift >= Level_dBm.Length) || (IndexCenterLastChannel - shift < 0)) { return -999; }
                double Pow_mW = 0;
                for (int i = 0; numberpoint > i; i++)
                {
                    Pow_mW = Math.Pow(10, Level_dBm[i - shift + IndexCenterLastChannel] / 10) + Pow_mW;
                }
                return 10 * Math.Log10(Pow_mW);
            }
            TrigerLevel = -999;
            return -999;
        }
        private static int CorrelationAnalyzeForEstimationFreq(float[] Level_dBm, double StartFreq_Hz, double Step_Hz, out double BestCorr, out double StandartBW_Hz, out string Standard)
        { // Важно Данная функция должна определить центральную частоту (ее индекс) на основании сопоставления с эталонными сигналами пока сравнение идет только с GSM
            //const double GSMWindowAnalize_Hz = 200000;
            double AllBW = Step_Hz * (Level_dBm.Length - 1);
            if ((AllBW >= 150000) && (AllBW <= 400000))
            {// есть подозрение что это GSM
                // производим сравнение с GSM
                //int indexStart = (int)Math.Floor(((Level_dBm.Length - 1) / 2.0) - ((GSMWindowAnalize_Hz / 2.0) / Step_Hz));
                //if (indexStart < 0) { indexStart = 0; };
                //int indexStop = (int)Math.Ceiling(((Level_dBm.Length - 1) / 2.0) + ((GSMWindowAnalize_Hz / 2.0) / Step_Hz));
                //if (indexStop >= Level_dBm.Length) { indexStop = Level_dBm.Length; };

                //float[] SignalAnalizeLevel = new float[indexStop - indexStart];
                //double SignalAnalizeFreq_Hz = StartFreq_Hz + indexStart * Step_Hz;
                //Array.Copy(Level_dBm, indexStart, SignalAnalizeLevel, 0, indexStop - indexStart);


                float[] LevelsRef = SmoothTrace.blackman(ReferenceSpectrum.GSMLevels);
                double CentrFreq_Hz = StartFreq_Hz + (AllBW / 2.0);
                double RefFreq_Hz = CentrFreq_Hz - ReferenceSpectrum.GSMCentralIndex * ReferenceSpectrum.GSMStep_Hz;
                int shift = СorrelationСoefficient.CalcShiftSpectrum(Level_dBm, StartFreq_Hz, Step_Hz, ReferenceSpectrum.GSMLevels,
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
        private static double CorrelationAnalyzeForCompairWithEtalon(float[] Level_dBm, double StartFreq_Hz, double Step_Hz, out double StandartBW_Hz, out string Standard)
        { // Важно Данная функция должна определить центральную частоту (ее индекс) на основании сопоставления с эталонными сигналами пока сравнение идет только с GSM
            double AllBW = Step_Hz * (Level_dBm.Length - 1);
            if ((AllBW >= 150000) && (AllBW <= 400000))
            {// есть подозрение что это GSM

                float[] LevelsRef = SmoothTrace.blackman(ReferenceSpectrum.GSMLevels);
                double CentrFreq_Hz = StartFreq_Hz + (AllBW / 2.0);
                double RefFreq_Hz = CentrFreq_Hz - ReferenceSpectrum.GSMCentralIndex * ReferenceSpectrum.GSMStep_Hz;
                double corr = СorrelationСoefficient.CalcCorrelation(Level_dBm, StartFreq_Hz, Step_Hz, ReferenceSpectrum.GSMLevels,
                    RefFreq_Hz, ReferenceSpectrum.GSMStep_Hz, СorrelationСoefficient.MethodCalcCorrelation.Person);
                StandartBW_Hz = 271000;
                Standard = "GSM";
                return corr;
            }
            StandartBW_Hz = 0;
            Standard = null;
            return -9999999;
        }
    }
}
