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
            Emitting[] newEmittings = CreateEmittings(Trace.Level, refLevels.levels, index_start_stop, (Trace.Freq_Hz[2] - Trace.Freq_Hz[1])/1000, Trace.Freq_Hz[0]/1000000, NoiseLevel_dBm);
            // сформировали новые параметры излучения теперь надо накатить старые по идее.
            return newEmittings;
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
                        bool CorectCalcBW = CalcBW(levels, start, stop, nDbLevel_dB, NoiseLevel_dBm, MinExcessNoseLevel_dB, NumberIgnoredPoints, ref startreal, ref stoptreal);
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
                            emitting.Spectrum.Levels_dBm = new float[stop_ - start_];
                            Array.Copy(levels, start, emitting.Spectrum.Levels_dBm, 0, stop_ - start_);
                            emitting.Spectrum.SpectrumStartFreq_MHz = startFreq_MHz + stepBW_kHz * start_ / 1000;
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
                emittings.Add(emitting);
            }
            return emittings.ToArray();
        }
        private static bool CalcBW(float[] levels, int start, int stop, double nDbLevel_dB, double NoiseLevel_dBm, double MinExcessNoseLevel_dB, int NumberIgnoredPoints, ref int IndexStart, ref int IndexStop)
        {
            IndexStart = SearchEdgeIndex(levels, start, stop, nDbLevel_dB, NoiseLevel_dBm, MinExcessNoseLevel_dB, false, NumberIgnoredPoints);
            IndexStop = SearchEdgeIndex(levels, start, stop, nDbLevel_dB, NoiseLevel_dBm, MinExcessNoseLevel_dB, true, NumberIgnoredPoints);
            if ((IndexStart == -1)||(IndexStop == -1)){ return false;}
            return true;
        }
        /// <summary>
        /// Ищем индекс масива где наблюдается уровень nDbLevel_dB относительно максимального значения. Уровень должен продержаться не менее точек чем NumberIgnoredPoints
        /// Возращаем -1 если это не удалось
        /// </summary>
        /// <param name="levels"></param>
        /// <param name="start"></param>
        /// <param name="stop"></param>
        /// <param name="nDbLevel_dB"></param>
        /// <param name="NoiseLevel_dBm"></param>
        /// <param name="MinExcessNoseLevel_dB"></param>
        /// <param name="MoveRight"></param>
        /// <param name="NumberIgnoredPoints"></param>
        /// <returns></returns>
        private static int SearchEdgeIndex(float[] levels, int start, int stop, double nDbLevel_dB, double NoiseLevel_dBm, double MinExcessNoseLevel_dB, bool MoveRight, int NumberIgnoredPoints)
        {
            // коснстанты 
            double fluctuationCoef = 10;
            // конец констант

            // конверсия если перепутали местами
            if (start > stop)
            {
                int temp = stop;
                stop = start;
                start = temp;
            }
            // определим уровень флуктуаций на графике
            double everage_fluct = 0;
            for (int i = start; i > stop; i++)
            {
                everage_fluct = Math.Abs(levels[i + 1] - levels[i]);
            }
            everage_fluct = everage_fluct / (stop - start);
            // найдем максимальный уровень излучения в диапазоне притом исключая резкие скачки
            int index_max;
            double level_max;
            if (levels[start] > levels[stop])
            {
                index_max = start;
                level_max = levels[start];
            }
            else 
            {
                index_max = stop;
                level_max = levels[stop];
            }
            for (int i = start+1; i <= stop-1; i++)
            {
                if (levels[i] >= level_max)
                {
                    double difference;
                    if (Math.Abs(levels[i] - levels[i - 1]) > Math.Abs(levels[i] - levels[i + 1]) )
                    { difference = Math.Abs(levels[i] - levels[i + 1]); }
                    else { difference = Math.Abs(levels[i] - levels[i - 1]); }
                    if (difference < fluctuationCoef * everage_fluct)
                    {
                        index_max = i;
                        level_max = levels[i];
                    }

                }
            }
            // определяем достаточно ли максимума для шума
            if (level_max - nDbLevel_dB < NoiseLevel_dBm + MinExcessNoseLevel_dB) { return -1; }
            {
            }
            // идем от максимума по направлению 
            int k = -1;
            int limit = 0;
            if (MoveRight) { k = 1; limit = levels.Length; }
            int CountPoint = 0;
            for (int i = index_max; i * k <= limit * k; i = i + k)
            {// цикл обеспечивающий движение по спектру в нужную сторону от максимального уровня
                if (levels[i] < level_max - nDbLevel_dB)
                {
                    CountPoint++;
                    if (CountPoint > NumberIgnoredPoints)
                    {
                        return (i - k*CountPoint);
                    }
                }
                else
                {
                    CountPoint = 0;
                }
            }
            if (CountPoint > 0) { return limit; }
            return -1;
        }
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
                    if (measSdrBandwidthResults.T1 != null) { start =  measSdrBandwidthResults.T1.Value; Spectrum.T1 = measSdrBandwidthResults.T1.Value; }
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
                    bool CorectCalcBW = CalcBW(templevels, start, stop, nDbLevel_dB, NoiseLevel_dBm, MinExcessNoseLevel_dB, NumberIgnoredPoints, ref startreal, ref stoptreal);
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

    }
}
