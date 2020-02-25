using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Common;


namespace Atdi.Modules.Sdrn.Calculation
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
        private static int CountMaxEmission; // нужно брать из файла конфигурации
        private static bool CorrelationAdaptation; // true означает что можно адаптировать коэфициент корреляции. Устанавливается когда первоначально коэфициент корреляции 0.99 и выше.
        private static int MaxNumberEmitingOnFreq; // брать из файла конфигурации.
        private static double MinCoeffCorrelation; // брать из файла конфигурации.
        private static bool UkraineNationalMonitoring; // признак что делается все для Украины


        // конец констант

        /// <summary>
        /// Мы сливаем все EmittingRaw в EmittingSummary с групировкой данных если излучения подобны 
        /// </summary>
        /// <param name="EmittingRaw"></param>
        /// <param name="EmittingTemp"></param>
        /// <param name="EmittingSummary"></param>
        /// <returns></returns>
        public static bool CalcGrouping(
                                        int? prmTimeBetweenWorkTimes_sec,
                                        int? prmTypeJoinSpectrum,
                                        double? prmCrossingBWPercentageForGoodSignals,
                                        double? prmCrossingBWPercentageForBadSignals,
                                        bool? prmAnalyzeByChannel,
                                        bool? prmCorrelationAnalize,
                                        ref double? prmCorrelationFactor,
                                        double? prmMaxFreqDeviation,
                                        bool? prmCorrelationAdaptation,
                                        int? prmMaxNumberEmitingOnFreq,
                                        double? prmMinCoeffCorrelation,
                                        bool? prmUkraineNationalMonitoring,
                                        ref Emitting[] EmittingsRaw,
                                        ref Emitting[] EmittingsTemp,
                                        ref Emitting[] EmittingsSummary,
                                        double NoiseLevel_dBm,
                                        int CountMaxEmissionFromConfig)
        {

            if (EmittingsRaw == null)
            {
                return true;
            }

            CountMaxEmission = CountMaxEmissionFromConfig;
            TimeBetweenWorkTimes_sec = 600;
            TypeJoinSpectrum = 0;
            CrossingBWPercentageForGoodSignals = 70;
            CrossingBWPercentageForBadSignals = 40;
            AnalyzeByChannel = false;
            CorrelationAnalize = false;
            MaxFreqDeviation = 0.0001;
            CorrelationFactor = 0.85;
            CorrelationAdaptation = true;
            MaxNumberEmitingOnFreq = 75;
            MinCoeffCorrelation = 0.9;
            UkraineNationalMonitoring = true;
            CountMaxEmissionFromConfig = 1000;
            NoiseLevel_dBm = -100;

            /*
            CountMaxEmission = CountMaxEmissionFromConfig;
            TimeBetweenWorkTimes_sec = prmTimeBetweenWorkTimes_sec.Value;
            TypeJoinSpectrum = prmTypeJoinSpectrum.Value;
            CrossingBWPercentageForGoodSignals = prmCrossingBWPercentageForGoodSignals.Value;
            CrossingBWPercentageForBadSignals = prmCrossingBWPercentageForBadSignals.Value;
            AnalyzeByChannel = prmAnalyzeByChannel.Value;
            CorrelationAnalize = prmCorrelationAnalize.Value;
            MaxFreqDeviation = prmMaxFreqDeviation.Value;
            CorrelationFactor = prmCorrelationFactor.Value;
            CorrelationAdaptation = prmCorrelationAdaptation.Value;
            MaxNumberEmitingOnFreq = prmMaxNumberEmitingOnFreq.Value;
            MinCoeffCorrelation = prmMinCoeffCorrelation.Value;
            UkraineNationalMonitoring = prmUkraineNationalMonitoring.Value;
            */

            // Увеличиваем счетчики у всех излучений
            if (EmittingsSummary != null)
            {
                for (var i = 0; EmittingsSummary.Length > i; i++)
                {
                    if (EmittingsSummary[i].WorkTimes != null)
                    {
                        EmittingsSummary[i].WorkTimes[EmittingsSummary[i].WorkTimes.Length - 1].TempCount++;
                    }
                }
            }
            if (EmittingsTemp != null)
            {
                for (var i = 0; EmittingsTemp.Length > i; i++)
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
            for (var i = 0; EmittingsRaw.Length > i; i++)
            {
                bool isSuccess = false;
                bool existTheSameEmitting = false;
                for (var j = 0; emittingsSummaryTemp.Count > j; j++)
                {
                    existTheSameEmitting = MatchCheckEmitting(emittingsSummaryTemp[j], EmittingsRaw[i], AnalyzeByChannel, CorrelationAnalize, MaxFreqDeviation, CorrelationFactor);
                    if (existTheSameEmitting)
                    {
                        var em = emittingsSummaryTemp[j];
                        JoinEmmiting(ref em, EmittingsRaw[i], NoiseLevel_dBm);
                        emittingsSummaryTemp[j] = em;
                        isSuccess = true;
                        break;
                    }
                }
                if (isSuccess == false)
                {
                    for (var l = 0; emittingsTempTemp.Count > l; l++)
                    {
                        existTheSameEmitting = MatchCheckEmitting(emittingsTempTemp[l], EmittingsRaw[i], AnalyzeByChannel, CorrelationAnalize, MaxFreqDeviation, CorrelationFactor);
                        if (existTheSameEmitting)
                        {
                            var em = emittingsTempTemp[l];
                            JoinEmmiting(ref em, EmittingsRaw[i], NoiseLevel_dBm);
                            emittingsTempTemp[l] = em;
                            break;
                        }
                    }
                    if (!existTheSameEmitting)
                    {
                        if (EmittingsRaw[i].Spectrum.СorrectnessEstimations)
                        {
                            EmittingsRaw[i] = CalcSignalization.FillEmittingForStorage(EmittingsRaw[i]);
                            emittingsSummaryTemp.Add(EmittingsRaw[i]);
                        }
                        else
                        {
                            EmittingsRaw[i] = CalcSignalization.FillEmittingForStorage(EmittingsRaw[i]);
                            emittingsTempTemp.Add(EmittingsRaw[i]);
                        }
                    }
                }
            }
            if ((UkraineNationalMonitoring) && (CorrelationAdaptation) && (emittingsTempTemp.Count < CountMaxEmission) && (emittingsSummaryTemp.Count < CountMaxEmission))
            {
                DeliteRedundantUncorrelationEmission(emittingsTempTemp, NoiseLevel_dBm);
                DeliteRedundantUncorrelationEmission(emittingsSummaryTemp, NoiseLevel_dBm);
            }
            else
            {
                // перебрасываем из temp хорошие сигналы в Summary
                for (var i = 0; i < emittingsTempTemp.Count; i++)
                {
                    if (emittingsTempTemp[i].Spectrum.СorrectnessEstimations)
                    {
                        emittingsSummaryTemp.Add(CalcSignalization.CreatIndependEmitting(emittingsTempTemp[i]));
                        emittingsTempTemp.RemoveRange(i, 1);
                        i--;
                    }
                }
                // Пройдемся по Summary нет ли дублирующихся сигналов 
                for (var i = 0; emittingsSummaryTemp.Count > i; i++)
                {
                    for (var j = i + 1; emittingsSummaryTemp.Count > j; j++)
                    {
                        bool existTheSameEmitting = MatchCheckEmitting(emittingsSummaryTemp[i], emittingsSummaryTemp[j], AnalyzeByChannel, CorrelationAnalize, MaxFreqDeviation, CorrelationFactor);
                        if (existTheSameEmitting)
                        {
                            var em = emittingsSummaryTemp[i];
                            JoinEmmiting(ref em, emittingsSummaryTemp[j], NoiseLevel_dBm);
                            emittingsSummaryTemp[i] = em;
                            emittingsSummaryTemp.RemoveRange(j, 1);
                            j--;
                        }
                    }
                }
                // Пройдемся по temp нет ли дублирующихся сигналов 
                for (var i = 0; emittingsTempTemp.Count > i; i++)
                {
                    for (var j = i + 1; emittingsTempTemp.Count > j; j++)
                    {
                        bool existTheSameEmitting = MatchCheckEmitting(emittingsTempTemp[i], emittingsTempTemp[j], AnalyzeByChannel, CorrelationAnalize, MaxFreqDeviation, CorrelationFactor);
                        if (existTheSameEmitting)
                        {
                            var em = emittingsTempTemp[i];
                            JoinEmmiting(ref em, emittingsTempTemp[j],  NoiseLevel_dBm);
                            emittingsTempTemp[i] = em;
                            emittingsTempTemp.RemoveRange(j, 1);
                            j--;
                        }
                    }
                }
                DeliteRedundantEmission(emittingsTempTemp, CountMaxEmission);
                DeliteRedundantEmission(emittingsSummaryTemp, CountMaxEmission);
            }


            prmCorrelationFactor = CorrelationFactor;
            EmittingsTemp = emittingsTempTemp.ToArray();
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
        private static bool MatchCheckEmitting(Emitting emitting1, Emitting emitting2, bool AnalyzeByChannel, bool CorrelationAnalize, double MaxFreqDeviation, double CorrelationFactor)
        {
            bool TraditionalCheck = MatchCheckEmitting(emitting1, emitting2); // пересечение полос частот обязательное условие
            if (!TraditionalCheck) { return false; } // выход если нет пересечения 
            if (CorrelationAnalize) // если требуется корреляция между излучениями
            { if (ChackCorelationForTwoEmitting(emitting1, emitting2, CorrelationFactor)) { return true; } else { return false; } }
            if (!AnalyzeByChannel) { return true; } // выход если нет анализировать поканально
            if (!ChackFreqForTwoEmitting(emitting1, emitting2, MaxFreqDeviation)) { return false; }// если необходимо сравнивать частоты
            return true;
        }
        private static bool MatchCheckEmitting(Emitting emitting1, Emitting emitting2, bool AnalyzeByChannel, bool CorrelationAnalize, double MaxFreqDeviation, double CorrelationFactor, out double RealCorrelation)
        {
            RealCorrelation = -2;
            bool TraditionalCheck = MatchCheckEmitting(emitting1, emitting2); // пересечение полос частот обязательное условие
            if (!TraditionalCheck) { return false; } // выход если нет пересечения 
            if (CorrelationAnalize) // если требуется корреляция между излучениями
            { if (ChackCorelationForTwoEmitting(emitting1, emitting2, CorrelationFactor, out RealCorrelation)) { return true; } else { return false; } }
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
        private static bool JoinEmmiting(ref Emitting MasterEmitting, Emitting AttachableEmitting, double NoiseLevel_dBm)
        {
            var res = false;
            var JoinAttachableEmittingToMasterEmitting = true;

            // определяем кто к кому присоединяется
            if (MasterEmitting.SpectrumIsDetailed != AttachableEmitting.SpectrumIsDetailed)
            {
                if (!MasterEmitting.SpectrumIsDetailed) { JoinAttachableEmittingToMasterEmitting = false; }
            }
            else if (MasterEmitting.Spectrum.СorrectnessEstimations != AttachableEmitting.Spectrum.СorrectnessEstimations)
            {
                if (!MasterEmitting.Spectrum.СorrectnessEstimations) { JoinAttachableEmittingToMasterEmitting = false; }
            }
            else if (false)//MasterEmitting.MeanDeviationFromReference != AttachableEmitting.MeanDeviationFromReference)
            {
                if (AttachableEmitting.MeanDeviationFromReference > MasterEmitting.MeanDeviationFromReference)
                {
                    JoinAttachableEmittingToMasterEmitting = false;
                }
            }
            else if ((AnalyzeByChannel))
            {
                int k = CompareTwoEmittingCenterWithStartEnd(MasterEmitting, AttachableEmitting);
                if (k == 0)
                {
                    if (MasterEmitting.CurentPower_dBm < AttachableEmitting.CurentPower_dBm) { JoinAttachableEmittingToMasterEmitting = false; }
                }
                else
                {
                    if (k == -1) { JoinAttachableEmittingToMasterEmitting = false; }
                }
            }
            else
            {
                if (MasterEmitting.CurentPower_dBm < AttachableEmitting.CurentPower_dBm) { JoinAttachableEmittingToMasterEmitting = false; }
            }


            // собственно само присоединение одного к другому
            if (JoinAttachableEmittingToMasterEmitting)
            {// присоединяем к мастеру
                res = JoinSecondToFirst(ref MasterEmitting, AttachableEmitting, NoiseLevel_dBm);
            }
            else
            {// мастер становиться присоединяемым
             //AttachableEmitting = CalcSignalization.FillEmittingForStorage(AttachableEmitting, logger);
                res = JoinSecondToFirst(ref AttachableEmitting, MasterEmitting,  NoiseLevel_dBm);
                MasterEmitting = CalcSignalization.CreatIndependEmitting(AttachableEmitting);
            }
            // формируем результирующее излуение
            return res;
        }
        private static bool JoinSecondToFirst(ref Emitting MasterEmitting, Emitting AttachableEmitting, double NoiseLevel_dBm)
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

            // обединяем массивы
            var workTimes1 = new List<WorkTime>();
            if (MasterEmitting.WorkTimes != null)
            {
                workTimes1 = MasterEmitting.WorkTimes.ToList();
            }
            var workTimes2 = new List<WorkTime>();
            if (AttachableEmitting.WorkTimes != null)
            {
                workTimes2 = AttachableEmitting.WorkTimes.ToList();
            }
            if ((workTimes2 != null) && (workTimes1 != null))
            {
                workTimes1.AddRange(workTimes2);
                var NewWorkTimes = from z in workTimes1 orderby z.StartEmitting ascending select z;
                var WorkTimes = NewWorkTimes.ToList();

                // найдем и удалим пересечения 
                for (var i = 0; WorkTimes.Count - 1 > i; i++)
                {
                    TimeSpan timeSpan = WorkTimes[i + 1].StartEmitting - WorkTimes[i].StopEmitting;
                    if (timeSpan.TotalSeconds < TimeBetweenWorkTimes_sec)
                    {// производим обединение и удаление лишнего
                        WorkTimes[i].HitCount = WorkTimes[i].HitCount + WorkTimes[i + 1].HitCount;
                        if (WorkTimes[i].StopEmitting < WorkTimes[i + 1].StopEmitting)
                        {
                            WorkTimes[i].StopEmitting = WorkTimes[i + 1].StopEmitting;
                        }

                        WorkTimes[i].ScanCount = Math.Max(WorkTimes[i].ScanCount, 1) + Math.Max(WorkTimes[i + 1].ScanCount, 1) + WorkTimes[i].TempCount + WorkTimes[i + 1].TempCount;
                        WorkTimes[i].TempCount = 0;
                        WorkTimes[i].PersentAvailability = 100 * WorkTimes[i].HitCount / WorkTimes[i].ScanCount;
                        WorkTimes.RemoveRange(i + 1, 1);
                        i--;
                    }
                }
                for (var i = 0; WorkTimes.Count - 1 > i; i++)
                {
                    if (WorkTimes[i].ScanCount == 0)
                    {
                        WorkTimes[i].ScanCount = 1;
                    }
                }
                MasterEmitting.WorkTimes = WorkTimes.ToArray();
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
            var NewLevels_dBm = new float[NewLevelArrCount]; // массив с уровнями
            if (Math.Abs(MasterSpectrum.SpectrumSteps_kHz - MasterSpectrum.SpectrumSteps_kHz) < 0.001)
            {
                // сетки совпали просто складываем
                for (var i = 0; NewLevelArrCount > i; i++)
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
                for (var i = 0; NewLevelArrCount > i; i++)
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
            var spectrum = CalcSignalization.CreateSpectrum(NewLevels_dBm, MinStep_Hz, minStartFreq_Hz, NoiseLevel_dBm);
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
            int index1Start = (int)Math.Round(1000.0 * (emitting1.StartFrequency_MHz - emitting1.Spectrum.SpectrumStartFreq_MHz) / emitting1.Spectrum.SpectrumSteps_kHz);
            int index1Stop = (int)Math.Round(1000.0 * (emitting1.StopFrequency_MHz - emitting1.Spectrum.SpectrumStartFreq_MHz) / emitting1.Spectrum.SpectrumSteps_kHz);
            int index2Start = (int)Math.Round(1000.0 * (emitting2.StartFrequency_MHz - emitting2.Spectrum.SpectrumStartFreq_MHz) / emitting2.Spectrum.SpectrumSteps_kHz);
            int index2Stop = (int)Math.Round(1000.0 * (emitting2.StopFrequency_MHz - emitting2.Spectrum.SpectrumStartFreq_MHz) / emitting2.Spectrum.SpectrumSteps_kHz);
            if (index1Start < 0) { index1Start = 0; }
            if (index2Start < 0) { index2Start = 0; }
            if (index1Stop >= emitting1.Spectrum.Levels_dBm.Length) { index1Stop = emitting1.Spectrum.Levels_dBm.Length - 1; }
            if (index2Stop >= emitting2.Spectrum.Levels_dBm.Length) { index2Stop = emitting2.Spectrum.Levels_dBm.Length - 1; }
            var arr1 = new float[index1Stop - index1Start];
            Array.Copy(emitting1.Spectrum.Levels_dBm, index1Start, arr1, 0, index1Stop - index1Start);
            arr1 = SmoothTrace.blackman(arr1, arr1.Length);
            var arr2 = new float[index2Stop - index2Start];
            Array.Copy(emitting2.Spectrum.Levels_dBm, index2Start, arr2, 0, index2Stop - index2Start);
            arr2 = SmoothTrace.blackman(arr2, arr2.Length);

            double freq1_Hz = emitting1.Spectrum.SpectrumStartFreq_MHz * 1000000.0 + emitting1.Spectrum.SpectrumSteps_kHz * index1Start * 1000.0;
            double freq2_Hz = emitting2.Spectrum.SpectrumStartFreq_MHz * 1000000.0 + emitting2.Spectrum.SpectrumSteps_kHz * index2Start * 1000.0;
            double BW1_Hz = emitting1.Spectrum.SpectrumSteps_kHz * 1000.0;
            double BW2_Hz = emitting2.Spectrum.SpectrumSteps_kHz * 1000.0;
            double corr = СorrelationСoefficient.CalcCorrelation(arr1, freq1_Hz, BW1_Hz, arr2, freq2_Hz, BW2_Hz, СorrelationСoefficient.MethodCalcCorrelation.Person);
            if (corr >= CorrelationFactor) { return true; } else { return false; }
            ////}
        }
        private static bool ChackCorelationForTwoEmitting(Emitting emitting1, Emitting emitting2, double CorrelationFactor, out double RealCorrelation)
        { // НЕ ПРОВЕРЕННО
            int index1Start = (int)Math.Round(1000.0 * (emitting1.StartFrequency_MHz - emitting1.Spectrum.SpectrumStartFreq_MHz) / emitting1.Spectrum.SpectrumSteps_kHz);
            int index1Stop = (int)Math.Round(1000.0 * (emitting1.StopFrequency_MHz - emitting1.Spectrum.SpectrumStartFreq_MHz) / emitting1.Spectrum.SpectrumSteps_kHz);
            int index2Start = (int)Math.Round(1000.0 * (emitting2.StartFrequency_MHz - emitting2.Spectrum.SpectrumStartFreq_MHz) / emitting2.Spectrum.SpectrumSteps_kHz);
            int index2Stop = (int)Math.Round(1000.0 * (emitting2.StopFrequency_MHz - emitting2.Spectrum.SpectrumStartFreq_MHz) / emitting2.Spectrum.SpectrumSteps_kHz);
            if (index1Start < 0) { index1Start = 0; }
            if (index2Start < 0) { index2Start = 0; }
            if (index1Stop >= emitting1.Spectrum.Levels_dBm.Length) { index1Stop = emitting1.Spectrum.Levels_dBm.Length - 1; }
            if (index2Stop >= emitting2.Spectrum.Levels_dBm.Length) { index2Stop = emitting2.Spectrum.Levels_dBm.Length - 1; }
            var arr1 = new float[index1Stop - index1Start];
            Array.Copy(emitting1.Spectrum.Levels_dBm, index1Start, arr1, 0, index1Stop - index1Start);
            arr1 = SmoothTrace.blackman(arr1, arr1.Length);
            var arr2 = new float[index2Stop - index2Start];
            Array.Copy(emitting2.Spectrum.Levels_dBm, index2Start, arr2, 0, index2Stop - index2Start);
            arr2 = SmoothTrace.blackman(arr2, arr2.Length);

            double freq1_Hz = emitting1.Spectrum.SpectrumStartFreq_MHz * 1000000.0 + emitting1.Spectrum.SpectrumSteps_kHz * index1Start * 1000.0;
            double freq2_Hz = emitting2.Spectrum.SpectrumStartFreq_MHz * 1000000.0 + emitting2.Spectrum.SpectrumSteps_kHz * index2Start * 1000.0;
            double BW1_Hz = emitting1.Spectrum.SpectrumSteps_kHz * 1000.0;
            double BW2_Hz = emitting2.Spectrum.SpectrumSteps_kHz * 1000.0;
            double corr = СorrelationСoefficient.CalcCorrelation(arr1, freq1_Hz, BW1_Hz, arr2, freq2_Hz, BW2_Hz, СorrelationСoefficient.MethodCalcCorrelation.Person);
            RealCorrelation = corr;
            if (corr >= CorrelationFactor) { return true; } else { return false; }
            ////}
        }
        private static bool ChackFreqForTwoEmitting(Emitting emitting1, Emitting emitting2, double MaxFreqDeviation)
        { // НЕ ПРОВЕРЕННО

            bool Freq1; bool Freq2;
            if (emitting1.EmittingParameters.TriggerFreqDeviation == 0) { Freq1 = false; }
            else { Freq1 = emitting1.EmittingParameters.FreqDeviation <= emitting1.EmittingParameters.TriggerFreqDeviation; }
            if (emitting2.EmittingParameters.TriggerFreqDeviation == 0) { Freq2 = false; }
            else { Freq2 = emitting1.EmittingParameters.FreqDeviation <= emitting1.EmittingParameters.TriggerFreqDeviation; }
            if ((Freq1) && (Freq2))
            {
                return true;
            }
            else if ((Freq1) || (Freq2))
            {
                return false;
            }
            else
            {
                double CentrlFreq1_MHz = (emitting1.StartFrequency_MHz + emitting1.StopFrequency_MHz) / 2;
                double CentrlFreq2_MHz = (emitting2.StartFrequency_MHz + emitting2.StopFrequency_MHz) / 2;
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
            while (emittings.Count > LostCount)
            {
                for (int i = 0; emittings.Count > i; i++)
                {
                    int hit = 0;
                    for (int j = 0; emittings[i].WorkTimes.Length > j; j++)
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
        private static void DeliteRedundantUncorrelationEmission(List<Emitting> emittings, double NoiseLevel_dBm)
        { // НЕ ТЕСТИЛ по идее функция должна удалить все излучения оставив там какоето количество
          // считаем количество излучений на частоте
            while (true)
            {

                var EmittingOnFreq = new int[emittings.Count];
                int MaxNumber = 0;
                for (var i = 0; emittings.Count > i; i++)
                {
                    int count = 0;
                    for (var j = 0; emittings.Count > j; j++)
                    {
                        if (i != j)
                        {
                            if (MatchCheckEmitting(emittings[i], emittings[j]))
                            { count++; }
                        }
                    }
                    EmittingOnFreq[i] = count;
                    if (count > MaxNumber) { MaxNumber = count; }
                }
                if (MaxNumber < MaxNumberEmitingOnFreq)
                {
                    return; // выход поскольку все хорошо
                }
                // Надо ужимать пройдемся нет ли дублирующих сигналов
                var CorrelationFactors = new double[emittings.Count];// В данном месте мы собираем максимальную кореляцию для сигнала по соотношению с другими сигналами
                for (var i = 0; emittings.Count > i; i++)
                {
                    double MaxCorelationFactors = 0;
                    for (int j = i + 1; emittings.Count > j; j++)
                    {
                        bool existTheSameEmitting = MatchCheckEmitting(emittings[i], emittings[j], AnalyzeByChannel, CorrelationAnalize, MaxFreqDeviation, CorrelationFactor, out double RealCorrelation);
                        if (existTheSameEmitting)
                        {
                            var em = emittings[i];
                            JoinEmmiting(ref em, emittings[j], NoiseLevel_dBm);
                            emittings[i] = em;
                            emittings.RemoveRange(j, 1);
                            j--;
                        }
                        else
                        {
                            if ((RealCorrelation > MaxCorelationFactors) && (RealCorrelation > -1))
                            {
                                MaxCorelationFactors = RealCorrelation;
                            }
                        }
                    }
                    CorrelationFactors[i] = MaxCorelationFactors;
                }
                // повторная оценка после сжатия
                EmittingOnFreq = new int[emittings.Count];
                MaxNumber = 0;
                for (var i = 0; emittings.Count > i; i++)
                {
                    int count = 0;
                    for (var j = 0; emittings.Count > j; j++)
                    {
                        if (i != j)
                        {
                            if (MatchCheckEmitting(emittings[i], emittings[j]))
                            { count++; }
                        }
                    }
                    EmittingOnFreq[i] = count;
                    if (count > MaxNumber) { MaxNumber = count; }
                }
                if (MaxNumber < MaxNumberEmitingOnFreq)
                {
                    return; // выход поскольку все хорошо
                }
                // тут все плохо и необходимо уменьшать коэффициент корреляции если можно
                if (CorrelationFactor > MinCoeffCorrelation)
                { // Уменьшеаем коэфициент корреляции и заходим в рекурсию. при повторном ужимании мы сократимся
                    CorrelationFactor = CorrelationFactor - 0.01;
                    //DeliteRedundantUncorrelationEmission(emittings, logger, NoiseLevel_dBm);
                }
                else
                {
                    // уменьшать нельзя поэтому будем удалять излучения на частотах которые наиболее забиты притом удаляем 1/10 от общего количество
                    // считаем массив хитов
                    var ArrHit = new int[emittings.Count];
                    for (var i = 0; emittings.Count > i; i++)
                    {
                        int hit = 0;
                        for (var j = 0; emittings[i].WorkTimes.Length > j; j++)
                        {
                            hit = hit + emittings[i].WorkTimes[j].HitCount;
                        }
                        ArrHit[i] = hit;
                    }
                    int numberDel = (int)(MaxNumber / 10.0); // определяем количество элементов которые стоит удалить 
                    if (MaxNumber - numberDel > MaxNumberEmitingOnFreq) { numberDel = MaxNumber - MaxNumberEmitingOnFreq; }
                    // определяем кого удалять
                    var IndexDel = EmittingProcessing.BedCorrelationEmitting(ArrHit, CorrelationFactors, EmittingOnFreq, numberDel);
                    // удаляем
                    if (IndexDel != null)
                    {
                        for (int i = 0; IndexDel.Length > i; i++)
                        {
                            emittings.RemoveRange(i, 1);
                        }
                    }
                    // рекурсивный прогон
                    //DeliteRedundantUncorrelationEmission(emittings, logger, NoiseLevel_dBm);
                }
            }
        }

        private static int CompareTwoEmittingCenterWithStartEnd(Emitting emitting1, Emitting emitting2)
        {// Не проверенно 
         // 1 первое лучше
         // -1 второе
         // 0 не смог сравнить
            if ((emitting1.Spectrum == null) || (emitting1.Spectrum.Levels_dBm.Length < 50)) { return 0; }
            if ((emitting2.Spectrum == null) || (emitting2.Spectrum.Levels_dBm.Length < 50)) { return 0; }
            if (CompareCenterWithStartEnd(emitting1) > CompareCenterWithStartEnd(emitting2)) { return 1; } else { return -1; }
        }
        private static double CompareCenterWithStartEnd(Emitting emitting)
        {
            const double persent = 5;
            if ((emitting.Spectrum == null) || (emitting.Spectrum.Levels_dBm.Length < 50)) { return 0; }
            int CountPointForAnalize = (int)Math.Ceiling(persent * emitting.Spectrum.Levels_dBm.Length / 100.0);
            int centrIndexSt = (int)Math.Ceiling((emitting.Spectrum.Levels_dBm.Length - CountPointForAnalize) / 2.0);
            double Center = EmittingProcessing.CalcPartialPow(emitting.Spectrum.Levels_dBm, centrIndexSt, centrIndexSt + CountPointForAnalize);
            double Start = EmittingProcessing.CalcPartialPow(emitting.Spectrum.Levels_dBm, 0, CountPointForAnalize);
            double Stop = EmittingProcessing.CalcPartialPow(emitting.Spectrum.Levels_dBm, emitting.Spectrum.Levels_dBm.Length - 1 - CountPointForAnalize, emitting.Spectrum.Levels_dBm.Length - 1);
            return (Center - Math.Max(Start, Stop));
        }

    }
}
