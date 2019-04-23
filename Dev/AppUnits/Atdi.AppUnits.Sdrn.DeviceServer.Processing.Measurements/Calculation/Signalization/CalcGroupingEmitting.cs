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
        /// <summary>
        /// Мы сливаем все EmittingRaw в EmittingSummary с групировкой данных если излучения подобны 
        /// </summary>
        /// <param name="EmittingRaw"></param>
        /// <param name="EmittingTemp"></param>
        /// <param name="EmittingSummary"></param>
        /// <returns></returns>
        public static bool CalcGrouping(Emitting[] EmittingsRaw, ref Emitting[] EmittingsTemp, ref Emitting[] EmittingsSummary,  ILogger logger, double NoiseLevel_dBm)
        {
            try
            {
                // Увеличиваем счетчики у всех излучений
                for (int i = 0; EmittingsSummary.Length>i; i++)
                {
                    EmittingsSummary[i].WorkTimes[EmittingsSummary[i].WorkTimes.Length - 1].TempCount++;
                }
                for (int i = 0; EmittingsSummary.Length > i; i++)
                {
                    EmittingsTemp[i].WorkTimes[EmittingsTemp[i].WorkTimes.Length - 1].TempCount++;
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
                        existTheSameEmitting = MatchCheckEmitting(emittingsSummaryTemp[j], EmittingsRaw[i]);
                        if (existTheSameEmitting)
                        {
                            var em = emittingsSummaryTemp[j];
                            JoinEmmiting(ref em, EmittingsRaw[i], logger, NoiseLevel_dBm); emittingsSummaryTemp[j] = em;
                            isSuccess = true;
                            break;
                        }
                    }
                    if (isSuccess == false)
                    {
                        for (int l = 0; emittingsTempTemp.Count > l; l++)
                        {
                            existTheSameEmitting = MatchCheckEmitting(emittingsTempTemp[l], EmittingsRaw[i]);
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
                                EmittingsRaw[i] = CalcSignalization.FillEmittingForStorage(EmittingsRaw[i],logger);
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
                for (int i = 0; i< emittingsTempTemp.Count; i++)
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
                    for (int j = i+1; emittingsSummaryTemp.Count > j; j++)
                    {
                        bool existTheSameEmitting = MatchCheckEmitting(emittingsSummaryTemp[i], emittingsSummaryTemp[j]);
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
                        bool existTheSameEmitting = MatchCheckEmitting(emittingsTempTemp[i], emittingsTempTemp[j]);
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
                EmittingsTemp = emittingsTempTemp.ToArray();
                EmittingsSummary = emittingsSummaryTemp.ToArray();
                EmittingsRaw = null;
            }
            catch (Exception ex)
            {
                logger.Error(Contexts.SignalizationTaskResultHandler, Categories.Measurements,Exceptions.UnknownErrorSignalizationTaskWorker, ex.Message);
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
            // константы 
            double CrossingBWPercentageForGoodSignals = 70; // определяет насколько процентов должно совпадать излучение если BW определен 
            double CrossingBWPercentageForBadSignals = 40; // определяет насколько процентов должно совпадать излучение если BW не определен 
            // конец констант

            // тупа нет пересечения
            if ((emitting1.StopFrequency_MHz < emitting2.StartFrequency_MHz)||(emitting1.StartFrequency_MHz > emitting2.StopFrequency_MHz)) { return false; }

            // пересечение есть следует оценить его степень
            if (emitting1.Spectrum.СorrectnessEstimations && emitting2.Spectrum.СorrectnessEstimations)
            {// оба излучения являються коректными т.е. просто определим степень максимального не пересечения
                double intersection = Math.Min(emitting1.StopFrequency_MHz, emitting2.StopFrequency_MHz) - Math.Max(emitting1.StartFrequency_MHz, emitting2.StartFrequency_MHz);
                intersection = 100*intersection/(Math.Max(emitting1.StopFrequency_MHz, emitting2.StopFrequency_MHz) - Math.Min(emitting1.StartFrequency_MHz, emitting2.StartFrequency_MHz));
                if (intersection > CrossingBWPercentageForGoodSignals)
                { return true; }
                else { return false;}
            }
            else if (emitting1.Spectrum.СorrectnessEstimations || emitting2.Spectrum.СorrectnessEstimations)
            { // коректно только одно излучение т.е. другой процент перекрытия считаем, что коректное может быть больше 
                double intersection = Math.Min(emitting1.StopFrequency_MHz, emitting2.StopFrequency_MHz) - Math.Max(emitting1.StartFrequency_MHz, emitting2.StartFrequency_MHz);
                if (emitting1.Spectrum.СorrectnessEstimations)
                {// первый сигнал корректен
                    if ((100 * intersection / (emitting1.StopFrequency_MHz - emitting1.StartFrequency_MHz) > CrossingBWPercentageForBadSignals) &&
                        (100 * intersection / (emitting2.StopFrequency_MHz - emitting2.StartFrequency_MHz) > CrossingBWPercentageForGoodSignals)) { return true; }
                    else { return false;}
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
                if ((MasterEmitting.SpectrumIsDetailed)|| (AttachableEmitting.SpectrumIsDetailed))
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
                    AttachableEmitting = CalcSignalization.FillEmittingForStorage(AttachableEmitting, logger);
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
            //константа 
            int TimeBetweenWorkTimes_sec = 60;
            int TypeJoinSpectrum = 0; // 0 - Best Emmiting (ClearWrite), 1 - MaxHold, 2 - Avarage
            //константа 
            try
            {
                // обединение Распределения уровней
                if (AttachableEmitting.LevelsDistribution == null)
                {
                    int indexLevel = (int)Math.Floor(AttachableEmitting.CurentPower_dBm) - MasterEmitting.LevelsDistribution.Levels[0];
                    if ((indexLevel >= 0) && (indexLevel < MasterEmitting.LevelsDistribution.Levels.Length)) { MasterEmitting.LevelsDistribution.Count[indexLevel]++; }
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
                if ((AttachableEmitting.WorkTimes.Length == 1) && (AttachableEmitting.WorkTimes[0].StopEmitting >= MasterEmitting.WorkTimes[MasterEmitting.WorkTimes.Length - 1].StopEmitting))
                {
                    TimeSpan timeSpan = AttachableEmitting.WorkTimes[0].StartEmitting - MasterEmitting.WorkTimes[MasterEmitting.WorkTimes.Length - 1].StopEmitting;
                    if (timeSpan.TotalSeconds > TimeBetweenWorkTimes_sec)
                    { // если пауза затянулась более TimeBetweenWorkTimes_sec
                        var workTimesTemp = MasterEmitting.WorkTimes.ToList();
                        workTimesTemp.Add(AttachableEmitting.WorkTimes[0]);
                        MasterEmitting.WorkTimes = workTimesTemp.ToArray();
                    }
                    else
                    { // если пауза не большая
                        MasterEmitting.WorkTimes[MasterEmitting.WorkTimes.Length - 1].StopEmitting = AttachableEmitting.WorkTimes[0].StopEmitting;
                        MasterEmitting.WorkTimes[MasterEmitting.WorkTimes.Length - 1].HitCount++;
                        MasterEmitting.WorkTimes[MasterEmitting.WorkTimes.Length - 1].ScanCount = MasterEmitting.WorkTimes[MasterEmitting.WorkTimes.Length - 1].ScanCount + MasterEmitting.WorkTimes[MasterEmitting.WorkTimes.Length - 1].TempCount;
                        MasterEmitting.WorkTimes[MasterEmitting.WorkTimes.Length - 1].TempCount = 0;
                        MasterEmitting.WorkTimes[MasterEmitting.WorkTimes.Length - 1].PersentAvailability = 100*MasterEmitting.WorkTimes[MasterEmitting.WorkTimes.Length - 1].HitCount/ MasterEmitting.WorkTimes[MasterEmitting.WorkTimes.Length - 1].ScanCount;
                    }
                }
                else
                {
                    // обединяем массивы
                    var workTimes1 = MasterEmitting.WorkTimes.ToList();
                    var workTimes2 = AttachableEmitting.WorkTimes.ToList();
                    workTimes1.AddRange(workTimes2);
                    var NewWorkTimes = from z in workTimes1 orderby z.StartEmitting ascending select z;
                    List<WorkTime> WorkTimes = NewWorkTimes.ToList();
                    
                    // найдем и удалим пересечения 
                    for (int i = 0; WorkTimes.Count - 1 > i; i++)
                    {
                        TimeSpan timeSpan = WorkTimes[i+1].StartEmitting - WorkTimes[i].StopEmitting;
                        if (timeSpan.TotalSeconds < TimeBetweenWorkTimes_sec)
                        {// производим обединение и удаление лишнего
                            WorkTimes[i].HitCount = WorkTimes[i].HitCount + WorkTimes[i + 1].HitCount;
                            WorkTimes[i].HitCount = WorkTimes[i].ScanCount + WorkTimes[i+1].ScanCount + WorkTimes[i].TempCount + WorkTimes[i+1].TempCount;
                            WorkTimes[i].TempCount = 0;
                            WorkTimes[i].PersentAvailability = WorkTimes[i].HitCount/ WorkTimes[i].ScanCount;
                            if (WorkTimes[i].StopEmitting < WorkTimes[i + 1].StopEmitting)
                            { WorkTimes[i].StopEmitting = WorkTimes[i + 1].StopEmitting; }
                            WorkTimes.RemoveRange(i + 1, 1);
                            i--;
                        }
                    }
                   
                    MasterEmitting.WorkTimes = WorkTimes.ToArray();
                }
                // обединение уровней
                if (TypeJoinSpectrum != 0)
                {
                    if ((MasterEmitting.SpectrumIsDetailed) && (AttachableEmitting.SpectrumIsDetailed))
                    { // если оба детальные 
                        bool joinCorr = JoinSpectrum(ref MasterEmitting.Spectrum, AttachableEmitting.Spectrum, TypeJoinSpectrum, NoiseLevel_dBm);
                    }
                    else if (((!MasterEmitting.SpectrumIsDetailed) && (!AttachableEmitting.SpectrumIsDetailed))&&
                        ((MasterEmitting.Spectrum.СorrectnessEstimations) && (AttachableEmitting.Spectrum.СorrectnessEstimations)))
                    { // если оба не детальны
                        bool joinCorr = JoinSpectrum(ref MasterEmitting.Spectrum, AttachableEmitting.Spectrum, TypeJoinSpectrum, NoiseLevel_dBm);
                    }
                }
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
            double minStartFreq_Hz =1000000.0 * Math.Min(MasterSpectrum.SpectrumStartFreq_MHz, MasterSpectrum.SpectrumStartFreq_MHz);
            double MinStep_Hz = 1000*Math.Min(MasterSpectrum.SpectrumSteps_kHz, MasterSpectrum.SpectrumSteps_kHz);
            double maxFreq_Hz = 1000.0*Math.Max(1000.0*MasterSpectrum.SpectrumStartFreq_MHz + MasterSpectrum.SpectrumSteps_kHz * (MasterSpectrum.Levels_dBm.Length - 1),
                1000.0*AttachableSpectrum.SpectrumStartFreq_MHz + AttachableSpectrum.SpectrumSteps_kHz * (AttachableSpectrum.Levels_dBm.Length - 1));
            int NewLevelArrCount = (int)Math.Ceiling((maxFreq_Hz - minStartFreq_Hz) / MinStep_Hz);
            float[] NewLevels_dBm = new float[NewLevelArrCount]; // массив с уровнями
            if (Math.Abs(MasterSpectrum.SpectrumSteps_kHz - MasterSpectrum.SpectrumSteps_kHz)<0.001)
            {
                // сетки совпали просто складываем
                for (int i = 0; NewLevelArrCount >i; i++)
                {
                    NewLevels_dBm[i] = -200;
                    double curFreq = minStartFreq_Hz + i * MinStep_Hz;
                    int indexMaster = (int)Math.Round((curFreq - MasterSpectrum.SpectrumStartFreq_MHz*1000000)/MinStep_Hz);
                    int indexAttachable = (int)Math.Round((curFreq - AttachableSpectrum.SpectrumStartFreq_MHz * 1000000) / MinStep_Hz);
                    if ((indexMaster>=0)&&(indexMaster < MasterSpectrum.Levels_dBm.Length))
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
                double deltaLevelMaster = 10*Math.Log10(MasterSpectrum.SpectrumSteps_kHz * 1000 / MinStep_Hz);
                double deltaLevelAttachable = 10*Math.Log10(AttachableSpectrum.SpectrumSteps_kHz * 1000 / MinStep_Hz);
                // сетки не совпали редкое явление но нужно складывать
                for (int i = 0; NewLevelArrCount > i; i++)
                {
                    NewLevels_dBm[i] = -200;
                    double curFreq = minStartFreq_Hz + i * MinStep_Hz;
                    int indexMaster = (int)Math.Round((curFreq - MasterSpectrum.SpectrumStartFreq_MHz * 1000000) / (MasterSpectrum.SpectrumSteps_kHz*1000));
                    int indexAttachable = (int)Math.Round((curFreq - AttachableSpectrum.SpectrumStartFreq_MHz * 1000000) / (AttachableSpectrum.SpectrumSteps_kHz*1000));
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
            if (spectrum.СorrectnessEstimations)
            {
                MasterSpectrum = spectrum;
            }
            done = true;
            return done;
        }
    }
}
