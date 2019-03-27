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

namespace Atdi.AppUnits.Sdrn.DeviceServer.Processing.Measurements
{
    public static class CalcGroupingEmitting
    {
        /// <summary>
        /// Мы сливаем все EmittingRaw в EmittingSummary
        /// </summary>
        /// <param name="EmittingRaw"></param>
        /// <param name="EmittingTemp"></param>
        /// <param name="EmittingSummary"></param>
        /// <returns></returns>
        public static bool CalcGrouping(Emitting[] EmittingsRaw, ref Emitting[] EmittingsTemp, ref Emitting[] EmittingsSummary)
        {
            var emittingsSummaryTemp = EmittingsSummary.ToList();
            var emittingsDetailedTemp = EmittingsTemp.ToList();
            for (int i = 0; EmittingsRaw.Length > i; i++)
            {
                bool isSuccess = false;
                bool existTheSameEmitting = false;
                for (int j = 0; emittingsSummaryTemp.Count > j; j++)
                {
                    existTheSameEmitting = MatchCheckEmitting(emittingsSummaryTemp[j], EmittingsRaw[i]);
                    if (existTheSameEmitting) { var em = emittingsSummaryTemp[j]; JoinEmmiting(ref em, EmittingsRaw[i]); emittingsSummaryTemp[j] = em; isSuccess = true; break; }
                }

                if (isSuccess == false)
                {
                    for (int l = 0; emittingsDetailedTemp.Count > l; l++)
                    {
                        existTheSameEmitting = MatchCheckEmitting(emittingsDetailedTemp[l], EmittingsRaw[i]);
                        if (existTheSameEmitting) { var em = emittingsDetailedTemp[l]; JoinEmmiting(ref em, EmittingsRaw[i]); emittingsDetailedTemp[l] = em; break; }
                    }
                    if (!existTheSameEmitting)
                    {
                        if (EmittingsRaw[i].Spectrum.СorrectnessEstimations)
                        {
                            emittingsSummaryTemp.Add(EmittingsRaw[i]);
                        }
                        else
                        {
                            emittingsDetailedTemp.Add(EmittingsRaw[i]);
                        }
                    }
                }

            }
            EmittingsTemp = emittingsDetailedTemp.ToArray();
            EmittingsSummary = emittingsSummaryTemp.ToArray();
            EmittingsRaw = null;
            return true;
        }
        /// <summary>
        /// Необходимо определить является ли это одним и тем же излучением.
        /// </summary>
        /// <param name="emitting1"></param>
        /// <param name="emitting2"></param>
        /// <returns></returns>
        private static bool MatchCheckEmitting(Emitting emitting1, Emitting emitting2)
        {
            // константы 
            double CrossingBWPercentageForGoodSignals = 90;
            double CrossingBWPercentageForBadSignals = 60;
            // конец констант

            // тупа нет пересечения
            if ((emitting1.StopFrequency_MHz < emitting2.StartFrequency_MHz)||(emitting1.StartFrequency_MHz > emitting2.StopFrequency_MHz)) { return false; }

            // пересечение есть следует оценить его степень
            if (emitting1.Spectrum.СorrectnessEstimations && emitting2.Spectrum.СorrectnessEstimations)
            {// оба излучения являються коректными т.е. просто определим степень максимального не пересечения
                double intersection = Math.Min(emitting1.StopFrequency_MHz, emitting2.StopFrequency_MHz) - Math.Max(emitting1.StartFrequency_MHz, emitting2.StartFrequency_MHz);
                intersection = 100*intersection/Math.Max(emitting1.StopFrequency_MHz, emitting2.StopFrequency_MHz) - Math.Min(emitting1.StartFrequency_MHz, emitting2.StartFrequency_MHz);
                if (intersection > CrossingBWPercentageForGoodSignals) { return true; } else { return false;}
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
                intersection = 100 * intersection / Math.Max(emitting1.StopFrequency_MHz, emitting2.StopFrequency_MHz) - Math.Min(emitting1.StartFrequency_MHz, emitting2.StartFrequency_MHz);
                if (intersection > CrossingBWPercentageForBadSignals) { return true; } else { return false; }
            }
        }
        /// <summary>
        /// Объединяем два излучения в одно.
        /// </summary>
        /// <param name="MasterEmitting"></param>
        /// <param name="AttachableEmitting"></param>
        /// <returns></returns>
        private static bool JoinEmmiting(ref Emitting MasterEmitting, Emitting AttachableEmitting)
        {
            //константа 
            double CriticalDeltaPow_dB = 10;
            //конец констант 
            bool res = false; 
            // определяем какое излучение более качественно 
            if ((MasterEmitting.Spectrum.СorrectnessEstimations) && (!AttachableEmitting.Spectrum.СorrectnessEstimations))
            {// коректен мастер
                res =JoinSecondToFirst(ref MasterEmitting, AttachableEmitting);
            }
            else if ((!MasterEmitting.Spectrum.СorrectnessEstimations) && (AttachableEmitting.Spectrum.СorrectnessEstimations))
            {// коректно второе

                res = JoinSecondToFirst(ref AttachableEmitting, MasterEmitting);
                MasterEmitting = CreatIndependEmitting(AttachableEmitting);
            }
            else if ((MasterEmitting.Spectrum.СorrectnessEstimations) || (AttachableEmitting.Spectrum.СorrectnessEstimations))
            {// коректны оба
                // сопоставим мощности 
                double DeltaPow = (MasterEmitting.CurentPower_dBm - AttachableEmitting.CurentPower_dBm);
                if (Math.Abs(DeltaPow) > CriticalDeltaPow_dB)
                {
                    if (DeltaPow > 0)
                    {
                        res = JoinSecondToFirst(ref MasterEmitting, AttachableEmitting);
                    }
                    else
                    {
                        res = JoinSecondToFirst(ref AttachableEmitting, MasterEmitting);
                        MasterEmitting = CreatIndependEmitting(AttachableEmitting);
                    }
                }
                else
                {// если мощности не критичны сопоставим детализацию сигнала 
                    if (MasterEmitting.Spectrum.Levels_dBm.Length >= AttachableEmitting.Spectrum.Levels_dBm.Length)
                    {
                        res = JoinSecondToFirst(ref MasterEmitting, AttachableEmitting);
                    }
                    else
                    {
                        res = JoinSecondToFirst(ref AttachableEmitting, MasterEmitting);
                        MasterEmitting = CreatIndependEmitting(AttachableEmitting);
                    }
                }
            }
            else
            {// оба не коректны
                double DeltaPow = (MasterEmitting.CurentPower_dBm - AttachableEmitting.CurentPower_dBm);
                if (DeltaPow > 0)
                {
                    res = JoinSecondToFirst(ref MasterEmitting, AttachableEmitting);
                }
                else
                {
                    res = JoinSecondToFirst(ref AttachableEmitting, MasterEmitting);
                    MasterEmitting = CreatIndependEmitting(AttachableEmitting);
                }

            }
            // формируем результирующее излуение
            return res;
        }
        private static bool JoinSecondToFirst(ref Emitting MasterEmitting, Emitting AttachableEmitting)
        {
            //константа 
            int TimeBetweenWorkTimes_sec = 60;
            //константа 

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
                    if ((k+i >= 0) && (k+i < MasterEmitting.LevelsDistribution.Levels.Length))
                    {
                        MasterEmitting.LevelsDistribution.Count[i] = MasterEmitting.LevelsDistribution.Count[i] + AttachableEmitting.LevelsDistribution.Count[i + k];
                    }
                }
            }
            // обединение Распределения уровней времени
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
                }
            }
            else
            {
                // обединяем массивы
                var workTimes1 = MasterEmitting.WorkTimes.ToList();
                var workTimes2 = AttachableEmitting.WorkTimes.ToList();
                workTimes1.AddRange(workTimes2);
                var NewWorkTimes = from z in workTimes1 orderby z.StartEmitting select z;
                List<WorkTime> WorkTimes = NewWorkTimes.ToList();
                // найдем и удалим пересечения 
                for (int i = 0; WorkTimes.Count - 1 > i ; i++)
                {
                    TimeSpan timeSpan = WorkTimes[i].StopEmitting - WorkTimes[i].StartEmitting;
                    if (timeSpan.TotalSeconds < TimeBetweenWorkTimes_sec)
                    {// производим обединение и удаление лишнего
                        WorkTimes[i].HitCount = WorkTimes[i].HitCount + WorkTimes[i + 1].HitCount;
                        if (WorkTimes[i].StopEmitting < WorkTimes[i + 1].StopEmitting)
                        { WorkTimes[i].StopEmitting = WorkTimes[i + 1].StopEmitting;}
                        i--;
                    }
                }
                MasterEmitting.WorkTimes = WorkTimes.ToArray();
            }
            return true;
        }
        private static Emitting FillEmittingForStorage(Emitting Emitting)
        {
            // константы 
            int StartLevelsForLevelDistribution = -150;
            int NumberPointForLevelDistribution = 200;
            // конец константы 
            Emitting.WorkTimes[0].HitCount = 1;
            Emitting.WorkTimes[0].StartEmitting = DateTime.Now;
            //Emitting.WorkTimes[0].PersentAvailability = 100;
            Emitting.LevelsDistribution.Count = new int [NumberPointForLevelDistribution];
            Emitting.LevelsDistribution.Levels = new int[NumberPointForLevelDistribution];
            for (int i = 0; i< NumberPointForLevelDistribution; i++)
            {
                Emitting.LevelsDistribution.Levels[i] = StartLevelsForLevelDistribution + i;
                Emitting.LevelsDistribution.Count[i] = 0;
            }
            int indexLevel = (int)Math.Floor(Emitting.CurentPower_dBm) - Emitting.LevelsDistribution.Levels[0];
            if ((indexLevel >= 0) && (indexLevel < Emitting.LevelsDistribution.Levels.Length)){ Emitting.LevelsDistribution.Count[indexLevel]++;}
            return Emitting;
        }
        private static Emitting CreatIndependEmitting(Emitting emitting)
        {
            Emitting emitting1 = new Emitting()
            {
                CurentPower_dBm = emitting.CurentPower_dBm,
                EmittingParameters = emitting.EmittingParameters,
                LevelsDistribution = emitting.LevelsDistribution,
                MeanDeviationFromReference = emitting.MeanDeviationFromReference,
                ReferenceLevel_dBm = emitting.ReferenceLevel_dBm,
                SignalMask = emitting.SignalMask,
                Spectrum = emitting.Spectrum,
                StartFrequency_MHz = emitting.StartFrequency_MHz,
                StopFrequency_MHz = emitting.StopFrequency_MHz,
                TriggerDeviationFromReference = emitting.TriggerDeviationFromReference,
                WorkTimes = emitting.WorkTimes
            };
            return emitting1;
        }
    }
}
