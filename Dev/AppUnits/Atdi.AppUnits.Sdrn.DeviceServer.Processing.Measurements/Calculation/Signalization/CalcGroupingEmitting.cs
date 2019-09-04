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
using Atdi.Platform.Logging;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Processing.Measurements
{
    public static class CalcGroupingEmitting
    {

        //константа 
        private static int TimeBetweenWorkTimes_sec;
        private static int TypeJoinSpectrum; // 0 - Best Emmiting (ClearWrite), 1 - MaxHold, 2 - Avarage
        private static double CrossingBWPercentageForGoodSignals; // определяет насколько процентов должно совпадать излучение если BW определен 
        private static double CrossingBWPercentageForBadSignals; // определяет насколько процентов должно совпадать излучение если BW не определен 
        private static bool AnalyzeByChannel;
        private static bool CorrelationAnalize;
        private static double CorrelationFactor;
        private static double MaxFreqDeviation;
        private static int CountMaxEmission = 1000; // нужно брать из файла конфигурации
        
        // конец констант

        /// <summary>
        /// Мы сливаем все EmittingRaw в EmittingSummary с групировкой данных если излучения подобны 
        /// </summary>
        /// <param name="EmittingRaw"></param>
        /// <param name="EmittingTemp"></param>
        /// <param name="EmittingSummary"></param>
        /// <returns></returns>
        public static bool CalcGrouping(TaskParameters taskParameters, Emitting[] EmittingsRaw, ref Emitting[] EmittingsTemp, ref Emitting[] EmittingsSummary, ILogger logger, double NoiseLevel_dBm)
        {
            try
            {
                TimeBetweenWorkTimes_sec = taskParameters.SignalingMeasTaskParameters.GroupingParameters.TimeBetweenWorkTimes_sec.Value;
                TypeJoinSpectrum = taskParameters.SignalingMeasTaskParameters.GroupingParameters.TypeJoinSpectrum.Value;
                CrossingBWPercentageForGoodSignals = taskParameters.SignalingMeasTaskParameters.GroupingParameters.CrossingBWPercentageForGoodSignals.Value;
                CrossingBWPercentageForBadSignals = taskParameters.SignalingMeasTaskParameters.GroupingParameters.CrossingBWPercentageForBadSignals.Value;
                AnalyzeByChannel = taskParameters.SignalingMeasTaskParameters.AnalyzeByChannel.Value;
                CorrelationAnalize = taskParameters.SignalingMeasTaskParameters.CorrelationAnalize.Value;
                CorrelationFactor = taskParameters.SignalingMeasTaskParameters.CorrelationFactor.Value;
                MaxFreqDeviation = taskParameters.SignalingMeasTaskParameters.InterruptionParameters.MaxFreqDeviation.Value;
                // Увеличиваем счетчики у всех излучений
                if (EmittingsSummary != null)
                {
                    for (int i = 0; EmittingsSummary.Length > i; i++)
                    {
                        if (EmittingsSummary[i].WorkTimes != null)
                        {
                            EmittingsSummary[i].WorkTimes[EmittingsSummary[i].WorkTimes.Length - 1].TempCount++;
                        }
                    }
                }
                if (EmittingsTemp != null)
                {
                    for (int i = 0; EmittingsTemp.Length > i; i++)
                    {
                        if (EmittingsTemp[i].WorkTimes != null)
                        {
                            EmittingsTemp[i].WorkTimes[EmittingsTemp[i].WorkTimes.Length - 1].TempCount++;
                        }
                    }

                }
                List<Emitting> emittingsSummaryTemp;
                List<Emitting> emittingsTempTemp;
                if (EmittingsSummary == null)
                {
                    emittingsSummaryTemp = new List<Emitting>();
                }
                else
                {
                    emittingsSummaryTemp = EmittingsSummary.ToList();
                }
                if (EmittingsTemp == null)
                {
                    emittingsTempTemp = new List<Emitting>();
                }
                else
                {
                    emittingsTempTemp = EmittingsTemp.ToList();
                }
                for (int i = 0; EmittingsRaw.Length > i; i++)
                {
                    bool isSuccess = false;
                    bool existTheSameEmitting = false;
                    for (int j = 0; emittingsSummaryTemp.Count > j; j++)
                    {
                        existTheSameEmitting = MatchCheckEmitting(emittingsSummaryTemp[j], EmittingsRaw[i], AnalyzeByChannel, CorrelationAnalize, MaxFreqDeviation, CorrelationFactor);
                        if (existTheSameEmitting)
                        {
                            var em = emittingsSummaryTemp[j];
                            JoinEmmiting(ref em, EmittingsRaw[i], logger, NoiseLevel_dBm);
                            emittingsSummaryTemp[j] = em;
                            isSuccess = true;
                            break;
                        }
                    }
                    if (isSuccess == false)
                    {
                        for (int l = 0; emittingsTempTemp.Count > l; l++)
                        {
                            existTheSameEmitting = MatchCheckEmitting(emittingsTempTemp[l], EmittingsRaw[i], AnalyzeByChannel, CorrelationAnalize, MaxFreqDeviation, CorrelationFactor);
                            if (existTheSameEmitting)
                            {
                                var em = emittingsTempTemp[l];
                                JoinEmmiting(ref em, EmittingsRaw[i], logger, NoiseLevel_dBm);
                                emittingsTempTemp[l] = em;
                                break;
                            }
                        }
                        if (!existTheSameEmitting)
                        {
                            if (EmittingsRaw[i].Spectrum.СorrectnessEstimations)
                            {
                                EmittingsRaw[i] = CalcSignalization.FillEmittingForStorage(EmittingsRaw[i], logger);
                                emittingsSummaryTemp.Add(EmittingsRaw[i]);
                            }
                            else
                            {
                                EmittingsRaw[i] = CalcSignalization.FillEmittingForStorage(EmittingsRaw[i], logger);
                                emittingsTempTemp.Add(EmittingsRaw[i]);
                            }
                        }
                    }
                }
                // перебрасываем из temp хорошие сигналы в Summary
                for (int i = 0; i < emittingsTempTemp.Count; i++)
                {
                    if (emittingsTempTemp[i].Spectrum.СorrectnessEstimations)
                    {
                        emittingsSummaryTemp.Add(CalcSignalization.CreatIndependEmitting(emittingsTempTemp[i]));
                        emittingsTempTemp.RemoveRange(i, 1);
                        i--;
                    }
                }
                // Пройдемся по Summary нет ли дублирующихся сигналов 
                for (int i = 0; emittingsSummaryTemp.Count > i; i++)
                {
                    for (int j = i + 1; emittingsSummaryTemp.Count > j; j++)
                    {
                        bool existTheSameEmitting = MatchCheckEmitting(emittingsSummaryTemp[i], emittingsSummaryTemp[j], AnalyzeByChannel, CorrelationAnalize, MaxFreqDeviation, CorrelationFactor);
                        if (existTheSameEmitting)
                        {
                            var em = emittingsSummaryTemp[i];
                            JoinEmmiting(ref em, emittingsSummaryTemp[j], logger, NoiseLevel_dBm);
                            emittingsSummaryTemp[i] = em;
                            emittingsSummaryTemp.RemoveRange(j, 1);
                            j--;
                        }
                    }
                }
                // Пройдемся по temp нет ли дублирующихся сигналов 
                for (int i = 0; emittingsTempTemp.Count > i; i++)
                {
                    for (int j = i + 1; emittingsTempTemp.Count > j; j++)
                    {
                        bool existTheSameEmitting = MatchCheckEmitting(emittingsTempTemp[i], emittingsTempTemp[j], AnalyzeByChannel, CorrelationAnalize, MaxFreqDeviation, CorrelationFactor);
                        if (existTheSameEmitting)
                        {
                            var em = emittingsTempTemp[i];
                            JoinEmmiting(ref em, emittingsTempTemp[j], logger, NoiseLevel_dBm);
                            emittingsTempTemp[i] = em;
                            emittingsTempTemp.RemoveRange(j, 1);
                            j--;
                        }
                    }
                }


                DeliteRedundantEmission(emittingsTempTemp, CountMaxEmission - emittingsSummaryTemp.Count);
                EmittingsTemp = emittingsTempTemp.ToArray();
                EmittingsSummary = emittingsSummaryTemp.ToArray();
                EmittingsRaw = null;
            }
            catch (Exception ex)
            {
                logger.Error(Contexts.SignalizationTaskResultHandler, Categories.Measurements, Exceptions.UnknownErrorSignalizationTaskWorker, ex.Message);
            }
            return true;
        }
        /// <summary>
        /// Необходимо определить является ли это одним и тем же излучением.
        /// </summary>
        /// <param name="emitting1"></param>
        /// <param name="emitting2"></param>
        /// <returns></returns>
        public static bool MatchCheckEmitting(Emitting emitting1, Emitting emitting2)
        {
            // тупа нет пересечения
            if ((emitting1.StopFrequency_MHz < emitting2.StartFrequency_MHz) || (emitting1.StartFrequency_MHz > emitting2.StopFrequency_MHz)) { return false; }

            // пересечение есть следует оценить его степень
            if (emitting1.Spectrum.СorrectnessEstimations && emitting2.Spectrum.СorrectnessEstimations)
            {// оба излучения являються коректными т.е. просто определим степень максимального не пересечения
                double intersection = Math.Min(emitting1.StopFrequency_MHz, emitting2.StopFrequency_MHz) - Math.Max(emitting1.StartFrequency_MHz, emitting2.StartFrequency_MHz);
                intersection = 100 * intersection / (Math.Max(emitting1.StopFrequency_MHz, emitting2.StopFrequency_MHz) - Math.Min(emitting1.StartFrequency_MHz, emitting2.StartFrequency_MHz));
                if (intersection > CrossingBWPercentageForGoodSignals)
                { return true; }
                else { return false; }
            }
            else if (emitting1.Spectrum.СorrectnessEstimations || emitting2.Spectrum.СorrectnessEstimations)
            { // коректно только одно излучение т.е. другой процент перекрытия считаем, что коректное может быть больше 
                double intersection = Math.Min(emitting1.StopFrequency_MHz, emitting2.StopFrequency_MHz) - Math.Max(emitting1.StartFrequency_MHz, emitting2.StartFrequency_MHz);
                if (emitting1.Spectrum.СorrectnessEstimations)
                {// первый сигнал корректен
                    if ((100 * intersection / (emitting1.StopFrequency_MHz - emitting1.StartFrequency_MHz) > CrossingBWPercentageForBadSignals) &&
                        (100 * intersection / (emitting2.StopFrequency_MHz - emitting2.StartFrequency_MHz) > CrossingBWPercentageForGoodSignals)) { return true; }
                    else { return false; }
                }
                else
                { //второй сигнал корректен
                    if ((100 * intersection / (emitting2.StopFrequency_MHz - emitting2.StartFrequency_MHz) > CrossingBWPercentageForBadSignals) &&
                        (100 * intersection / (emitting1.StopFrequency_MHz - emitting1.StartFrequency_MHz) > CrossingBWPercentageForGoodSignals)) { return true; }
                    else { return false; }
                }
            }
            else
            {// некоректны оба
                double intersection = Math.Min(emitting1.StopFrequency_MHz, emitting2.StopFrequency_MHz) - Math.Max(emitting1.StartFrequency_MHz, emitting2.StartFrequency_MHz);
                intersection = 100 * intersection / (Math.Max(emitting1.StopFrequency_MHz, emitting2.StopFrequency_MHz) - Math.Min(emitting1.StartFrequency_MHz, emitting2.StartFrequency_MHz));
                if (intersection > CrossingBWPercentageForBadSignals) { return true; }
                else { return false; }
            }
        }
        /// <summary>
        /// Необходимо определить является ли это одним и тем же излучением.
        /// </summary>
        /// <param name="emitting1"></param>
        /// <param name="emitting2"></param>
        /// <returns></returns>
        public static bool MatchCheckEmitting(Emitting emitting1, Emitting emitting2, bool AnalyzeByChannel, bool CorrelationAnalize, double MaxFreqDeviation, double CorrelationFactor)
        {
            bool TraditionalCheck = MatchCheckEmitting(emitting1, emitting2); // пересечение полос частот обязательное условие
            if (!TraditionalCheck) { return false; } // выход если нет пересечения 
            if (CorrelationAnalize) // если требуется корреляция между излучениями
            {if (ChackCorelationForTwoEmitting(emitting1, emitting2, CorrelationFactor)) { return true; } else { return false; }}
            if (!AnalyzeByChannel) { return true; } // выход если нет анализировать поканально
            if (!ChackFreqForTwoEmitting(emitting1, emitting2, MaxFreqDeviation)) { return false; }// если необходимо сравнивать частоты
            return true;
        }
        /// <summary>
        /// Объединяем два излучения в одно
        /// </summary>
        /// <param name="MasterEmitting"></param>
        /// <param name="AttachableEmitting"></param>
        /// <returns></returns>
        public static bool JoinEmmiting(ref Emitting MasterEmitting, Emitting AttachableEmitting, ILogger logger, double NoiseLevel_dBm)
        {
            bool res = false;
            bool JoinAttachableEmittingToMasterEmitting = true;
            try
            {
                // определяем кто к кому присоединяется
                if ((MasterEmitting.SpectrumIsDetailed) || (AttachableEmitting.SpectrumIsDetailed))
                {// если уже есть детализация у первого или у второго то это тогда в приоретете 
                    if ((MasterEmitting.SpectrumIsDetailed) && (AttachableEmitting.SpectrumIsDetailed))
                    {// Если оба хороши выбираем лучший по критерию мощности 
                        if (MasterEmitting.CurentPower_dBm < AttachableEmitting.CurentPower_dBm)
                        {
                            JoinAttachableEmittingToMasterEmitting = false;
                        }
                    }
                    else if (AttachableEmitting.SpectrumIsDetailed)
                    {
                        JoinAttachableEmittingToMasterEmitting = false;
                    }
                }
                else
                { // если детализации нет
                  // определяем есть ли корректное измерение 
                    if ((MasterEmitting.Spectrum.СorrectnessEstimations) || (AttachableEmitting.Spectrum.СorrectnessEstimations))
                    {
                        if ((MasterEmitting.Spectrum.СorrectnessEstimations) && (AttachableEmitting.Spectrum.СorrectnessEstimations))
                        {// Если оба хороши выбираем лучший по критерию мощности 
                            if (MasterEmitting.CurentPower_dBm < AttachableEmitting.CurentPower_dBm)
                            {
                                JoinAttachableEmittingToMasterEmitting = false;
                            }
                        }
                        else if (AttachableEmitting.Spectrum.СorrectnessEstimations)
                        {
                            JoinAttachableEmittingToMasterEmitting = false;
                        }
                    }
                    else
                    {// если оба излучения не коректны выбираем с максимальной мощностью
                        if (MasterEmitting.CurentPower_dBm < AttachableEmitting.CurentPower_dBm)
                        {
                            JoinAttachableEmittingToMasterEmitting = false;
                        }
                    }
                }
                // собственно само присоединение одного к другому
                if (JoinAttachableEmittingToMasterEmitting)
                {// присоединяем к мастеру
                    res = JoinSecondToFirst(ref MasterEmitting, AttachableEmitting, logger, NoiseLevel_dBm);
                }
                else
                {// мастер становиться присоединяемым
                    //AttachableEmitting = CalcSignalization.FillEmittingForStorage(AttachableEmitting, logger);
                    res = JoinSecondToFirst(ref AttachableEmitting, MasterEmitting, logger, NoiseLevel_dBm);
                    MasterEmitting = CalcSignalization.CreatIndependEmitting(AttachableEmitting);
                }
            }
            catch (Exception ex)
            {
                logger.Error(Contexts.SignalizationTaskResultHandler, Categories.Measurements, Exceptions.UnknownErrorSignalizationTaskWorker, ex.Message);
            }
            // формируем результирующее излуение
            return res;
        }
        public static bool JoinSecondToFirst(ref Emitting MasterEmitting, Emitting AttachableEmitting, ILogger logger, double NoiseLevel_dBm)
        {

            try
            {
                // обединение Распределения уровней
                if (AttachableEmitting.LevelsDistribution == null)
                {
                    int indexLevel = (int)Math.Floor(AttachableEmitting.CurentPower_dBm) - MasterEmitting.LevelsDistribution.Levels[0];
                    if ((indexLevel >= 0) && (indexLevel < MasterEmitting.LevelsDistribution.Levels.Length)) { MasterEmitting.LevelsDistribution.Count[indexLevel]++; }
                }
                else if (MasterEmitting.LevelsDistribution == null)
                {
                    int indexLevel = (int)Math.Floor(MasterEmitting.CurentPower_dBm) - AttachableEmitting.LevelsDistribution.Levels[0];
                    if ((indexLevel >= 0) && (indexLevel < AttachableEmitting.LevelsDistribution.Levels.Length)) { AttachableEmitting.LevelsDistribution.Count[indexLevel]++; }
                    var arrLev = AttachableEmitting.LevelsDistribution.Levels.ToList();
                    var arrCount = AttachableEmitting.LevelsDistribution.Count.ToList();
                    MasterEmitting.LevelsDistribution = new LevelsDistribution();
                    MasterEmitting.LevelsDistribution.Levels = arrLev.ToArray();
                    MasterEmitting.LevelsDistribution.Count = arrCount.ToArray();
                }
                else
                {
                    int k = MasterEmitting.LevelsDistribution.Levels[0] - AttachableEmitting.LevelsDistribution.Levels[0];
                    for (int i = 0; MasterEmitting.LevelsDistribution.Levels.Length > i; i++)
                    {
                        if ((k + i >= 0) && (k + i < MasterEmitting.LevelsDistribution.Levels.Length))
                        {
                            MasterEmitting.LevelsDistribution.Count[i] = MasterEmitting.LevelsDistribution.Count[i] + AttachableEmitting.LevelsDistribution.Count[i + k];
                        }
                    }
                }
                // обединение масивов времени
                if ((AttachableEmitting.WorkTimes != null) && (MasterEmitting.WorkTimes != null) && ((MasterEmitting.WorkTimes.Length == 1) || (AttachableEmitting.WorkTimes.Length == 1)))
                {
                    bool MasterISFirst;
                    if ((MasterEmitting.WorkTimes.Length == 1) && (AttachableEmitting.WorkTimes.Length == 1))
                    {
                        if (MasterEmitting.WorkTimes[0].StopEmitting < AttachableEmitting.WorkTimes[0].StopEmitting)
                        { MasterISFirst = true; }
                        else
                        { MasterISFirst = false; }
                    }
                    else
                    {
                        if (MasterEmitting.WorkTimes.Length == 1)
                        { MasterISFirst = false; }
                        else
                        { MasterISFirst = true; }
                    }
                    if (MasterISFirst)
                    {
                        TimeSpan timeSpan = AttachableEmitting.WorkTimes[0].StartEmitting - MasterEmitting.WorkTimes[MasterEmitting.WorkTimes.Length - 1].StopEmitting;
                        if (timeSpan.TotalSeconds > TimeBetweenWorkTimes_sec)
                        { // если пауза затянулась более TimeBetweenWorkTimes_sec
                            var workTimesTemp = MasterEmitting.WorkTimes.ToList();
                            if ((AttachableEmitting.WorkTimes[0].ScanCount == 0) && (AttachableEmitting.WorkTimes[0].TempCount == 0))
                            {
                                AttachableEmitting.WorkTimes[0].ScanCount = 1;
                            }
                            workTimesTemp.Add(AttachableEmitting.WorkTimes[0]);
                            MasterEmitting.WorkTimes = workTimesTemp.ToArray();
                        }
                        else
                        { // если пауза не большая
                            MasterEmitting.WorkTimes[MasterEmitting.WorkTimes.Length - 1].StopEmitting = AttachableEmitting.WorkTimes[0].StopEmitting;
                            MasterEmitting.WorkTimes[MasterEmitting.WorkTimes.Length - 1].HitCount = MasterEmitting.WorkTimes[MasterEmitting.WorkTimes.Length - 1].HitCount + AttachableEmitting.WorkTimes[0].HitCount;
                            MasterEmitting.WorkTimes[MasterEmitting.WorkTimes.Length - 1].ScanCount = MasterEmitting.WorkTimes[MasterEmitting.WorkTimes.Length - 1].ScanCount + MasterEmitting.WorkTimes[MasterEmitting.WorkTimes.Length - 1].TempCount
                                + AttachableEmitting.WorkTimes[0].ScanCount + AttachableEmitting.WorkTimes[0].TempCount;
                            MasterEmitting.WorkTimes[MasterEmitting.WorkTimes.Length - 1].TempCount = 0;
                            MasterEmitting.WorkTimes[MasterEmitting.WorkTimes.Length - 1].PersentAvailability = 100 * MasterEmitting.WorkTimes[MasterEmitting.WorkTimes.Length - 1].HitCount / MasterEmitting.WorkTimes[MasterEmitting.WorkTimes.Length - 1].ScanCount;
                        }
                    }
                    else
                    {
                        TimeSpan timeSpan = MasterEmitting.WorkTimes[0].StartEmitting - AttachableEmitting.WorkTimes[MasterEmitting.WorkTimes.Length - 1].StopEmitting;
                        if (timeSpan.TotalSeconds > TimeBetweenWorkTimes_sec)
                        { // если пауза затянулась более TimeBetweenWorkTimes_sec
                            var workTimesTemp = AttachableEmitting.WorkTimes.ToList();
                            if ((MasterEmitting.WorkTimes[0].ScanCount == 0) && (MasterEmitting.WorkTimes[0].TempCount == 0))
                            {
                                MasterEmitting.WorkTimes[0].ScanCount = 1;
                            }
                            workTimesTemp.Add(MasterEmitting.WorkTimes[0]);
                            MasterEmitting.WorkTimes = workTimesTemp.ToArray();
                        }
                        else
                        { // если пауза не большая
                            AttachableEmitting.WorkTimes[AttachableEmitting.WorkTimes.Length - 1].StopEmitting = MasterEmitting.WorkTimes[0].StopEmitting;
                            AttachableEmitting.WorkTimes[AttachableEmitting.WorkTimes.Length - 1].HitCount = AttachableEmitting.WorkTimes[AttachableEmitting.WorkTimes.Length - 1].HitCount + MasterEmitting.WorkTimes[0].HitCount;
                            AttachableEmitting.WorkTimes[AttachableEmitting.WorkTimes.Length - 1].ScanCount = AttachableEmitting.WorkTimes[AttachableEmitting.WorkTimes.Length - 1].ScanCount + AttachableEmitting.WorkTimes[AttachableEmitting.WorkTimes.Length - 1].TempCount
                                + MasterEmitting.WorkTimes[0].ScanCount + MasterEmitting.WorkTimes[0].TempCount;
                            AttachableEmitting.WorkTimes[AttachableEmitting.WorkTimes.Length - 1].TempCount = 0;
                            AttachableEmitting.WorkTimes[AttachableEmitting.WorkTimes.Length - 1].PersentAvailability = 100 * AttachableEmitting.WorkTimes[AttachableEmitting.WorkTimes.Length - 1].HitCount / AttachableEmitting.WorkTimes[AttachableEmitting.WorkTimes.Length - 1].ScanCount;
                            var workTimesTemp = AttachableEmitting.WorkTimes.ToList();
                            MasterEmitting.WorkTimes = workTimesTemp.ToArray();
                        }
                    }
                }
                else
                {
                    // обединяем массивы
                    List<WorkTime> workTimes1 = new List<WorkTime>();
                    if (MasterEmitting.WorkTimes != null)
                    {
                        workTimes1 = MasterEmitting.WorkTimes.ToList();
                    }
                    List<WorkTime> workTimes2 = new List<WorkTime>();
                    if (AttachableEmitting.WorkTimes != null)
                    {
                        workTimes2 = AttachableEmitting.WorkTimes.ToList();
                    }
                    if ((workTimes2 != null) && (workTimes1 != null))
                    {
                        workTimes1.AddRange(workTimes2);
                        var NewWorkTimes = from z in workTimes1 orderby z.StartEmitting ascending select z;
                        List<WorkTime> WorkTimes = NewWorkTimes.ToList();

                        // найдем и удалим пересечения 
                        for (int i = 0; WorkTimes.Count - 1 > i; i++)
                        {
                            TimeSpan timeSpan = WorkTimes[i + 1].StartEmitting - WorkTimes[i].StopEmitting;
                            if (timeSpan.TotalSeconds < TimeBetweenWorkTimes_sec)
                            {// производим обединение и удаление лишнего
                                WorkTimes[i].HitCount = WorkTimes[i].HitCount + WorkTimes[i + 1].HitCount;
                                if (WorkTimes[i].StopEmitting < WorkTimes[i + 1].StopEmitting)
                                {
                                    WorkTimes[i].StopEmitting = WorkTimes[i + 1].StopEmitting;
                                }
                                WorkTimes[i].ScanCount = WorkTimes[i].ScanCount + WorkTimes[i + 1].ScanCount + WorkTimes[i].TempCount + WorkTimes[i + 1].TempCount; ;
                                WorkTimes[i].TempCount = 0;
                                WorkTimes[i].PersentAvailability = 100 * WorkTimes[i].HitCount / WorkTimes[i].ScanCount;
                                WorkTimes.RemoveRange(i + 1, 1);
                                i--;
                            }
                        }
                        MasterEmitting.WorkTimes = WorkTimes.ToArray();
                    }
                }
                // обединение уровней
                // код не достежим
                //if (TypeJoinSpectrum != 0)
                //{
                //    if ((MasterEmitting.SpectrumIsDetailed) && (AttachableEmitting.SpectrumIsDetailed))
                //    { // если оба детальные 
                //        bool joinCorr = JoinSpectrum(ref MasterEmitting.Spectrum, AttachableEmitting.Spectrum, TypeJoinSpectrum, NoiseLevel_dBm);
                //    }
                //    else if (((!MasterEmitting.SpectrumIsDetailed) && (!AttachableEmitting.SpectrumIsDetailed))&&
                //        ((MasterEmitting.Spectrum.СorrectnessEstimations) && (AttachableEmitting.Spectrum.СorrectnessEstimations)))
                //    { // если оба не детальны
                //        bool joinCorr = JoinSpectrum(ref MasterEmitting.Spectrum, AttachableEmitting.Spectrum, TypeJoinSpectrum, NoiseLevel_dBm);
                //    }
                //}
            }
            catch (Exception ex)
            {
                logger.Error(Contexts.SignalizationTaskResultHandler, Categories.Measurements, Exceptions.UnknownErrorSignalizationTaskWorker, ex.Message);
            }
            return true;
        }
        private static bool JoinSpectrum(ref Spectrum MasterSpectrum, Spectrum AttachableSpectrum, int TypeJoinSpectrum, double NoiseLevel_dBm)
        {// НЕ ТЕСТИРОВАННО

            // TypeJoinSpectrum = 2 пока игнорируется
            bool done = false;
            // вычисление старта спектра
            double minStartFreq_Hz = 1000000.0 * Math.Min(MasterSpectrum.SpectrumStartFreq_MHz, MasterSpectrum.SpectrumStartFreq_MHz);
            double MinStep_Hz = 1000 * Math.Min(MasterSpectrum.SpectrumSteps_kHz, MasterSpectrum.SpectrumSteps_kHz);
            double maxFreq_Hz = 1000.0 * Math.Max(1000.0 * MasterSpectrum.SpectrumStartFreq_MHz + MasterSpectrum.SpectrumSteps_kHz * (MasterSpectrum.Levels_dBm.Length - 1),
                1000.0 * AttachableSpectrum.SpectrumStartFreq_MHz + AttachableSpectrum.SpectrumSteps_kHz * (AttachableSpectrum.Levels_dBm.Length - 1));
            int NewLevelArrCount = (int)Math.Ceiling((maxFreq_Hz - minStartFreq_Hz) / MinStep_Hz);
            float[] NewLevels_dBm = new float[NewLevelArrCount]; // массив с уровнями
            if (Math.Abs(MasterSpectrum.SpectrumSteps_kHz - MasterSpectrum.SpectrumSteps_kHz) < 0.001)
            {
                // сетки совпали просто складываем
                for (int i = 0; NewLevelArrCount > i; i++)
                {
                    NewLevels_dBm[i] = -200;
                    double curFreq = minStartFreq_Hz + i * MinStep_Hz;
                    int indexMaster = (int)Math.Round((curFreq - MasterSpectrum.SpectrumStartFreq_MHz * 1000000) / MinStep_Hz);
                    int indexAttachable = (int)Math.Round((curFreq - AttachableSpectrum.SpectrumStartFreq_MHz * 1000000) / MinStep_Hz);
                    if ((indexMaster >= 0) && (indexMaster < MasterSpectrum.Levels_dBm.Length))
                    {
                        NewLevels_dBm[i] = MasterSpectrum.Levels_dBm[indexMaster];
                    }
                    if ((indexAttachable >= 0) && (indexAttachable < AttachableSpectrum.Levels_dBm.Length))
                    {
                        if (NewLevels_dBm[i] < AttachableSpectrum.Levels_dBm[indexAttachable]) { NewLevels_dBm[i] = AttachableSpectrum.Levels_dBm[indexAttachable]; }
                    }
                }
            }
            else
            {
                double deltaLevelMaster = 10 * Math.Log10(MasterSpectrum.SpectrumSteps_kHz * 1000 / MinStep_Hz);
                double deltaLevelAttachable = 10 * Math.Log10(AttachableSpectrum.SpectrumSteps_kHz * 1000 / MinStep_Hz);
                // сетки не совпали редкое явление но нужно складывать
                for (int i = 0; NewLevelArrCount > i; i++)
                {
                    NewLevels_dBm[i] = -200;
                    double curFreq = minStartFreq_Hz + i * MinStep_Hz;
                    int indexMaster = (int)Math.Round((curFreq - MasterSpectrum.SpectrumStartFreq_MHz * 1000000) / (MasterSpectrum.SpectrumSteps_kHz * 1000));
                    int indexAttachable = (int)Math.Round((curFreq - AttachableSpectrum.SpectrumStartFreq_MHz * 1000000) / (AttachableSpectrum.SpectrumSteps_kHz * 1000));
                    if ((indexMaster >= 0) && (indexMaster < MasterSpectrum.Levels_dBm.Length))
                    {
                        NewLevels_dBm[i] = MasterSpectrum.Levels_dBm[indexMaster];
                    }
                    if ((indexAttachable >= 0) && (indexAttachable < AttachableSpectrum.Levels_dBm.Length))
                    {
                        if (NewLevels_dBm[i] < AttachableSpectrum.Levels_dBm[indexAttachable]) { NewLevels_dBm[i] = AttachableSpectrum.Levels_dBm[indexAttachable]; }
                    }
                }

            }
            Spectrum spectrum = CalcSignalization.CreateSpectrum(NewLevels_dBm, MinStep_Hz, minStartFreq_Hz, NoiseLevel_dBm);
            if ((MasterSpectrum.Contravention) || (AttachableSpectrum.Contravention))
            { spectrum.Contravention = true; }
            else { spectrum.Contravention = false; }
            if (spectrum.СorrectnessEstimations)
            {
                MasterSpectrum = spectrum;
            }
            done = true;
            return done;
        }
        private static bool ChackCorelationForTwoEmitting(Emitting emitting1, Emitting emitting2, double CorrelationFactor)
        { // НЕ ПРОВЕРЕННО
            bool Corr1; bool Corr2;


            if (emitting1.TriggerDeviationFromReference == 0) { Corr1 = false; }
            else { Corr1 = emitting1.MeanDeviationFromReference <= emitting1.TriggerDeviationFromReference; }
            if (emitting2.TriggerDeviationFromReference == 0) { Corr2 = false; }
            else { Corr2 = emitting2.MeanDeviationFromReference <= emitting2.TriggerDeviationFromReference; }
            if ((Corr1) && (Corr2))
            {
                return true;
            }
            else if ((Corr1) || (Corr2))
            {
                return false;
            }
            else
            {
                int index1Start = (int)Math.Round(1000.0 * (emitting1.StartFrequency_MHz - emitting1.Spectrum.SpectrumStartFreq_MHz) / emitting1.Spectrum.SpectrumSteps_kHz);
                int index1Stop = (int)Math.Round(1000.0 * (emitting1.StopFrequency_MHz - emitting1.Spectrum.SpectrumStartFreq_MHz) / emitting1.Spectrum.SpectrumSteps_kHz);
                int index2Start = (int)Math.Round(1000.0 * (emitting2.StartFrequency_MHz - emitting2.Spectrum.SpectrumStartFreq_MHz) / emitting2.Spectrum.SpectrumSteps_kHz);
                int index2Stop = (int)Math.Round(1000.0 * (emitting2.StopFrequency_MHz - emitting2.Spectrum.SpectrumStartFreq_MHz) / emitting2.Spectrum.SpectrumSteps_kHz);
                if (index1Start < 0) { index1Start = 0; }
                if (index2Start < 0) { index2Start = 0; }
                if (index1Stop >= emitting1.Spectrum.Levels_dBm.Length) { index1Stop = emitting1.Spectrum.Levels_dBm.Length - 1; }
                if (index2Stop >= emitting2.Spectrum.Levels_dBm.Length) { index2Stop = emitting2.Spectrum.Levels_dBm.Length - 1; }
                float[] arr1 = new float[index1Stop - index1Start];
                Array.Copy(emitting1.Spectrum.Levels_dBm, index1Start, arr1, 0, index1Stop - index1Start);
                arr1 = SmoothTrace.blackman(arr1);
                float[] arr2 = new float[index2Stop - index2Start];
                arr2 = SmoothTrace.blackman(arr2);

                Array.Copy(emitting2.Spectrum.Levels_dBm, index2Start, arr2, 0, index2Stop - index2Start);

                double freq1_Hz = emitting1.Spectrum.SpectrumStartFreq_MHz * 1000000.0 + emitting1.Spectrum.SpectrumSteps_kHz * index1Start * 1000.0;
                double freq2_Hz = emitting2.Spectrum.SpectrumStartFreq_MHz * 1000000.0 + emitting2.Spectrum.SpectrumSteps_kHz * index2Start * 1000.0;
                double BW1_Hz = emitting1.Spectrum.SpectrumSteps_kHz * 1000.0;
                double BW2_Hz = emitting2.Spectrum.SpectrumSteps_kHz * 1000.0;
                double corr = СorrelationСoefficient.CalcCorrelation(arr1, freq1_Hz, BW1_Hz, arr2, freq2_Hz, BW2_Hz, СorrelationСoefficient.MethodCalcCorrelation.Person);
                if (corr >= CorrelationFactor) { return true; } else { return false; }
            }
        }
        private static bool ChackFreqForTwoEmitting(Emitting emitting1, Emitting emitting2, double MaxFreqDeviation)
        { // НЕ ПРОВЕРЕННО
            bool Freq1; bool Freq2;
            if (emitting1.EmittingParameters.TriggerFreqDeviation == 0) { Freq1 = false; }
            else { Freq1 = emitting1.EmittingParameters.FreqDeviation <= emitting1.EmittingParameters.TriggerFreqDeviation; }
            if (emitting2.EmittingParameters.TriggerFreqDeviation == 0) { Freq2 = false; }
            else { Freq2 = emitting1.EmittingParameters.FreqDeviation <= emitting1.EmittingParameters.TriggerFreqDeviation; }
            if ((Freq1) && (Freq2)) {
                return true; }
            else if ((Freq1) || (Freq2)) {
                return false; }
            else
            {
                double CentrlFreq1_MHz = (emitting1.StartFrequency_MHz + emitting1.StopFrequency_MHz)/2;
                double CentrlFreq2_MHz = (emitting2.StartFrequency_MHz + emitting2.StopFrequency_MHz)/2;
                double FreqDeviation = Math.Abs(CentrlFreq1_MHz - CentrlFreq2_MHz) / ((CentrlFreq1_MHz + CentrlFreq2_MHz) / 2.0);
                double MaxShift = ((emitting1.Spectrum.SpectrumSteps_kHz / (1000 * CentrlFreq1_MHz)) + (emitting2.Spectrum.SpectrumSteps_kHz / (1000 * CentrlFreq2_MHz))) / 2.0;
                if (FreqDeviation <= MaxShift) { FreqDeviation = 0; } else { FreqDeviation = FreqDeviation - MaxShift; }
                if (FreqDeviation < MaxFreqDeviation) { return true; } else { return false; }
            }
        }

        private static void DeliteRedundantEmission(List<Emitting> emittings, int LostCount)
        {
            if (emittings.Count < LostCount) { return; }
            if (LostCount < 0) { emittings = null; return; }
            int CurentHitNumber = 1;
            while ( emittings.Count > LostCount)
            {
                for (int i = 0; emittings.Count > i; i++)
                {
                    int hit = 0;
                    for (int j = 0; emittings[i].WorkTimes.Length>j; j++)
                    {
                        hit = hit + emittings[i].WorkTimes[j].HitCount;
                    }
                    if (hit <= CurentHitNumber)
                    {// удаление излучения
                        emittings.RemoveRange(i, 1);
                        i--;
                    }
                }
                CurentHitNumber++;
            }
        }
    }
}
