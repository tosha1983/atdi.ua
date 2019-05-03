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
        // константы потом стоит передать в виде переменных
        private static bool CompareTraceJustWithRefLevels = false;
        private static bool AutoDivisionEmitting = true;
        private static double DifferenceMaxMax = 20;
        private static bool FiltrationTrace = true;

        // конец констант
        /// <summary>
        /// Сопоставляем излучения реальные с реферативными уровнями. В случае превышения формируем излучение. 
        /// </summary>
        /// <param name="referenceLevels"></param>
        /// <param name="Trace"></param>
        /// <returns></returns>
        public static Emitting[] Calc(ReferenceLevels refLevels, MesureTraceResult Trace, double NoiseLevel_dBm)
        { //НЕ ТЕСТИРОВАННО
            // Константы 
            int NumberPointForChangeExcess=10; // на самом деле зависит от параметров таска там будем вычислять и прокидывать сюда.
            // Конец констант
            if (refLevels.levels.Length != Trace.Level.Length)
            {
                return null; // выход по причине несовпадения количества точек следовательно необходимо перерасчитать CalcReferenceLevels 
            }
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
            if (AutoDivisionEmitting) { index_start_stop = DivisionEmitting(index_start_stop, Trace); }

            //Формируем помехи.
            double stepBW_kHz = (Trace.Freq_Hz[Trace.Freq_Hz.Length] - Trace.Freq_Hz[0]) / ((Trace.Freq_Hz.Length-1)*1000.0);
            Emitting[] newEmittings = CreateEmittings(Trace.Level, refLevels.levels, index_start_stop, stepBW_kHz, Trace.Freq_Hz[0]/1000000, NoiseLevel_dBm);
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
            if (Trace.Level[0] > NoiseLevel_dBm) { excess = true; startSignalIndex = 0; }
            // выделение мест где произошло превышение порога 
            List<int> index_start_stop = new List<int>();
            for (int i = 0; i < Trace.Level.Length; i++)
            {
                if (Trace.Level[i] > NoiseLevel_dBm)
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

        private static List<int> DivisionEmitting (List<int> index_start_stop,  MesureTraceResult Trace)
        { // НЕ ПРОВЕРЕННО
            List<int> ResultStartStopIndexArr = new List<int>();
            for (int i = 0; i < index_start_stop.Count - 1; i = i + 2)
            {
                int[] StartStopAnalized;
                int count = EmissionCounting.Counting(Trace.Level, index_start_stop[i], index_start_stop[i + 1], out StartStopAnalized, DifferenceMaxMax, FiltrationTrace);
                for (int j = 0; j< StartStopAnalized.Length; j++)
                {
                    ResultStartStopIndexArr.Add(StartStopAnalized[j]);
                }
            }
            return ResultStartStopIndexArr;
        }
        private static Emitting[] CreateEmittings(float[] levels, float[] refLevel, List<int> index_start_stop, double stepBW_kHz, double startFreq_MHz, double NoiseLevel_dBm)
        { // задача локализовать излучения
            // константы начало
            List<Emitting> emittings = new List<Emitting>();
            double DiffLevelForCalcBW = 25;
            double windowBW = 1.5;
            double nDbLevel_dB = 15;
            int NumberIgnoredPoints = 1;
            double MinExcessNoseLevel_dB = 5;
            // константы конец
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
                        emitting.Spectrum.SpectrumStartFreq_MHz = startFreq_MHz + stepBW_kHz * start_ / 1000;
                        emitting.Spectrum.SpectrumSteps_kHz = stepBW_kHz;
                        emitting.Spectrum.Levels_dBm = templevel;
                        emitting.StartFrequency_MHz = startFreq_MHz + stepBW_kHz * start / 1000.0;
                        emitting.StopFrequency_MHz = startFreq_MHz + stepBW_kHz * stop / 1000.0;
                        emitting.Spectrum.СorrectnessEstimations = true;
                    }
                    else
                    {// попробуем пройтись по масиву с помощью метода nDbDown и всеже определить ширину спектра
                        int startreal = -1;
                        int stoptreal = -1;
                        bool CorectCalcBW = CalcBWSignalization.CalcBW(levels, start, stop, nDbLevel_dB, NoiseLevel_dBm, MinExcessNoseLevel_dB, NumberIgnoredPoints, ref startreal, ref stoptreal);
                        if (CorectCalcBW)
                        {
                            // необходимо найти корректное значение полосы частот
                            double tempBW = StandartBW.GetStandartBW_kHz((stoptreal - startreal) * stepBW_kHz);
                            double CentralFreq = startFreq_MHz + stepBW_kHz * (startreal + stoptreal) / 2000;

                            emitting.StartFrequency_MHz = CentralFreq - tempBW / 2000;
                            emitting.StopFrequency_MHz = CentralFreq + tempBW / 2000;
                            start = (int)Math.Floor((emitting.StartFrequency_MHz - startFreq_MHz) / (stepBW_kHz / 1000));
                            if (start < 0) { start = 0; }
                            stop = (int)Math.Ceiling((emitting.StopFrequency_MHz - startFreq_MHz) / (stepBW_kHz / 1000));
                            if (stop >= levels.Length) { stop = levels.Length - 1; }

                            emitting.Spectrum.Levels_dBm = new float[stop - start];

                            Array.Copy(levels, start, emitting.Spectrum.Levels_dBm, 0, stop - start);
                            emitting.Spectrum.SpectrumStartFreq_MHz = startFreq_MHz + stepBW_kHz * start / 1000;
                            emitting.Spectrum.SpectrumSteps_kHz = stepBW_kHz;
                            emitting.Spectrum.СorrectnessEstimations = true;
                        }
                        else
                        {
                            emitting.StartFrequency_MHz = startFreq_MHz + stepBW_kHz * start / 1000.0;
                            emitting.StopFrequency_MHz = startFreq_MHz + stepBW_kHz * stop / 1000.0;
                            emitting.Spectrum.Levels_dBm = new float[stop - start];


                            Array.Copy(levels, start, emitting.Spectrum.Levels_dBm, 0, stop - start);
                            emitting.Spectrum.SpectrumStartFreq_MHz = startFreq_MHz + stepBW_kHz * start / 1000;
                            emitting.Spectrum.SpectrumSteps_kHz = stepBW_kHz;
                            emitting.Spectrum.Levels_dBm = templevel;
                            emitting.Spectrum.СorrectnessEstimations = false;
                        }
                    }
                }
                else
                {
                    emitting.StartFrequency_MHz = startFreq_MHz + stepBW_kHz * start / 1000.0;
                    emitting.StopFrequency_MHz = startFreq_MHz + stepBW_kHz * stop / 1000.0;
                    emitting.Spectrum.Levels_dBm = new float[stop_ - start_];
                    Array.Copy(levels, start, emitting.Spectrum.Levels_dBm, 0, stop_ - start_);
                    emitting.Spectrum.SpectrumStartFreq_MHz = startFreq_MHz + stepBW_kHz * start_ / 1000;
                    emitting.Spectrum.SpectrumSteps_kHz = stepBW_kHz;
                    emitting.Spectrum.Levels_dBm = templevel;
                    emitting.Spectrum.СorrectnessEstimations = false;
                }
                emitting.ReferenceLevel_dBm = 0;
                emitting.CurentPower_dBm = 0;
                for (int j = start; j < stop; j++)
                {
                    emitting.ReferenceLevel_dBm = emitting.ReferenceLevel_dBm + Math.Pow(10, refLevel[j] / 10);
                    emitting.CurentPower_dBm = emitting.CurentPower_dBm + Math.Pow(10, levels[j] / 10);
                }
                emitting.ReferenceLevel_dBm = 10 * Math.Log10(emitting.ReferenceLevel_dBm);
                emitting.CurentPower_dBm = 10 * Math.Log10(emitting.CurentPower_dBm);
                emitting.WorkTimes = new WorkTime[1];
                emitting.WorkTimes[0] = new WorkTime();
                emitting.WorkTimes[0].StartEmitting = DateTime.Now;
                emitting.WorkTimes[0].StopEmitting = emitting.WorkTimes[0].StartEmitting;
                emitting.WorkTimes[0].HitCount = 1;
                emitting.WorkTimes[0].ScanCount = 1;
                emitting.WorkTimes[0].TempCount= 0;
                emitting.WorkTimes[0].PersentAvailability = 100;
                emittings.Add(emitting);
            }
            return emittings.ToArray();
        }
    }
}
