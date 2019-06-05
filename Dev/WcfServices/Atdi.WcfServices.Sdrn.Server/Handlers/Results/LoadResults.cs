using System.Collections.Generic;
using Atdi.Contracts.Api.EventSystem;
using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Contracts.CoreServices.EntityOrm;
using Atdi.Contracts.Sdrn.Server;
using Atdi.DataModels.DataConstraint;
using Atdi.Platform.Logging;
using System;
using MD = Atdi.DataModels.Sdrns.Server.Entities;
using Atdi.Contracts.WcfServices.Sdrn.Server;
using System.Xml;
using System.Linq;
using Atdi.Common;


namespace Atdi.WcfServices.Sdrn.Server
{
    public class LoadResults 
    {
        private const int CountInParams = 1000;
        private readonly IDataLayer<EntityDataOrm> _dataLayer;
        private readonly ILogger _logger;



        public LoadResults(IDataLayer<EntityDataOrm> dataLayer, ILogger logger)
        {
            this._dataLayer = dataLayer;
            this._logger = logger;
        }
        /*
        public static float[] blackman(float[] ArrIn, bool LogInterpolation = false)
        { // НЕ ТЕСТИРОВАННО

            double PersentLongFilterFromTrace = 3;
            int N = (int)Math.Floor(ArrIn.Length * PersentLongFilterFromTrace / 200); // Длина фильтра
            N = N * 2 + 1;
            if (N <= 2) { return ArrIn; }
            double[] H = new double[N]; // Импульсная характеристика фильтра
            // Расчет импульсной характеристики фильтра Блекмана
            for (int i = 0; N > i; i++)
            {
                H[i] = 0.42 - 0.5 * Math.Cos((2 * Math.PI * (i - 1)) / (N - 1)) + 0.08 * Math.Cos((4 * Math.PI * (i - 1)) / (N - 1));
            }
            //Нормировка импульсной характеристики
            double SUM = 0;
            for (int i = 0; N > i; i++)
            {
                SUM = SUM + H[i];
            }
            for (int i = 0; N > i; i++)
            {
                H[i] = H[i] / SUM;  // сумма коэффициентов равна 1
            }

            int z = ArrIn.Length;
            double[] outArr = new double[z];
            double[] ArrIn1 = new double[z];
            // Фильтрация входных данных
            if (!LogInterpolation)
            {
                for (int i = 0; z > i; i++)
                {
                    ArrIn1[i] = Math.Pow(10, ArrIn[i] / 10);
                }
            }
            int st = (int)(Math.Floor(N / 2.0)); // Индекс точки с максимальной характеристикой 
            for (int i = st; z + st > i; i++)
            {
                outArr[i - st] = 0;
                for (int j = 0; N > j; j++)
                {
                    if (i - j >= 0)
                    {
                        if (i - j <= ArrIn1.Length - 1)
                        {
                            outArr[i - st] = outArr[i - st] + H[j] * ArrIn1[i - j]; // самая формула фильтра
                        }
                        else
                        {
                            outArr[i - st] = outArr[i - st] + H[j] * ArrIn1[ArrIn1.Length - 1];
                        }
                    }
                    else
                    {
                        outArr[i - st] = outArr[i - st] + H[j] * ArrIn1[0];
                    }
                }
            }
            if (!LogInterpolation)
            {
                for (int i = 0; z > i; i++)
                {
                    outArr[i] = 10 * Math.Log10(outArr[i]);
                }
            }
            float[] ArrOut = new float[outArr.Length];
            for (int i = 0; z > i; i++)
            {
                ArrOut[i] = (float)outArr[i];
            }
            return ArrOut as float[];
        }

        public static int Counting(float[] Level_dBm, int PointStart, int PointStop, out int[] StartStop, double DifferenceMaxMax = 20, bool FiltrationTrace = true)
        { // НЕ ТЕСТИРОВАЛОСЬ
          // проверка коректности принимаемых данных
            if (PointStop < PointStart) { int T = PointStop; PointStop = PointStart; PointStart = T; }
            if (PointStart < 0) { PointStart = 0; }
            if (PointStop > Level_dBm.Length - 1) { PointStop = Level_dBm.Length - 1; }
            if (DifferenceMaxMax < 5) { DifferenceMaxMax = 5; }
            // конец проверки корректности 

            float[] Level = new float[PointStop - PointStart + 1];
            Array.Copy(Level_dBm, PointStart, Level, 0, PointStop - PointStart + 1);
            if (FiltrationTrace) { Level = blackman(Level, false); } // провели сглаживание массива

            double LocMax1; int IndexLocMax1;
            double LocMin1; int IndexLocMin1;
            bool gotoMax;

            List<int> MinMax = new List<int>();

            LocMin1 = Level[0]; IndexLocMin1 = 0;
            LocMax1 = Level[0]; IndexLocMax1 = 0;
            gotoMax = true;

            for (int i = 1; Level.Length > i; i++)
            {
                if (gotoMax)
                {
                    if (Level[i] > LocMax1)
                    {
                        LocMax1 = Level[i]; IndexLocMax1 = i;
                    }
                    if (Level[i] < LocMin1)
                    {
                        LocMin1 = Level[i]; IndexLocMin1 = i;
                    }
                    if (LocMax1 - LocMin1 >= DifferenceMaxMax)
                        if ((IndexLocMin1 < IndexLocMax1))
                        { // мы превысили уровень. Теперь точно можно зафиксировать минимум. 
                            if (MinMax.Count == 0)
                            {
                                MinMax.Add(IndexLocMin1);
                            }
                            else
                            {
                                MinMax.Add(IndexLocMin1);
                                MinMax.Add(IndexLocMin1);
                            }
                            LocMin1 = Level[i]; IndexLocMin1 = i;
                            gotoMax = false;
                        }
                        else
                        {
                            LocMax1 = Level[i]; IndexLocMax1 = i;
                        }
                }
                else
                {
                    if (Level[i] > LocMax1)
                    {
                        LocMax1 = Level[i]; IndexLocMax1 = i;
                    }
                    if (Level[i] < LocMin1)
                    {
                        LocMin1 = Level[i]; IndexLocMin1 = i;
                    }
                    if (LocMax1 - LocMin1 >= DifferenceMaxMax)
                    {
                        if (IndexLocMin1 > IndexLocMax1)
                        {
                            // мы превысили уровень. Теперь точно можно зафиксировать максимум. 
                            LocMax1 = Level[i]; IndexLocMax1 = i;
                            gotoMax = true;
                        }
                        else
                        {
                            LocMin1 = Level[i]; IndexLocMin1 = i;
                        }
                    }
                }
            }
            if (MinMax.Count == 0)
            {
                MinMax.Add(0); MinMax.Add(Level.Length - 1);
            }
            else
            {
                if (gotoMax) { MinMax.Add(IndexLocMin1); }
                else { MinMax.RemoveAt(MinMax.Count - 1); }
            }
            StartStop = new int[MinMax.Count];
            for (int i = 0; MinMax.Count > i; i++)
            {
                StartStop[i] = MinMax[i] + PointStart;
            }
            return (int)(StartStop.Length / 2);
        }
        */

        public void GetEmittingAndReferenceLevels(int resId, bool isLoadAllData, out Emitting[] emittings, out ReferenceLevels referenceLevels, double? StartFrequency_Hz = null, double? StopFrequency_Hz = null)
        {
            emittings = null;
            var listIdsEmittings = new List<int>();
            var listEmitings = new List<KeyValuePair<int, Emitting>>();
            var listWorkTimes = new List<KeyValuePair<int, WorkTime>>();
            var listSignalMask = new List<KeyValuePair<int, SignalMask>>();
            var listSpectrum = new List<KeyValuePair<int, Spectrum>>();
            referenceLevels = new ReferenceLevels();
            var queryExecuter = this._dataLayer.Executor<SdrnServerDataContext>();
            var listSensors = new List<Sensor>();
            var listEmitting = new List<Emitting>();
            var queryEmitting = this._dataLayer.GetBuilder<MD.IEmitting>()
            .From()
            .Select(c => c.Id, c => c.CurentPower_dBm, c => c.MeanDeviationFromReference, c => c.ReferenceLevel_dBm, c => c.RollOffFactor, c => c.StandardBW, c => c.StartFrequency_MHz, c => c.StopFrequency_MHz, c => c.TriggerDeviationFromReference, c => c.LevelsDistribution, c => c.SensorId, c => c.StationID, c => c.StationTableName)
            .OrderByAsc(c => c.StartFrequency_MHz)
            .Where(c => c.ResMeasId, ConditionOperator.Equal, resId);
            //.Where(c => c.Id, ConditionOperator.Equal, 1216364);
            queryExecuter.Fetch(queryEmitting, reader =>
            {
                while (reader.Read())
                {
                    var emitting = new Emitting();
                    emitting.Id = reader.GetValue(c => c.Id);
                    if (reader.GetValue(c => c.StationID).HasValue)
                    {
                        emitting.AssociatedStationID = reader.GetValue(c => c.StationID).Value;
                    }
                    emitting.AssociatedStationTableName = reader.GetValue(c => c.StationTableName);

                    if (reader.GetValue(c => c.StartFrequency_MHz).HasValue)
                        emitting.StartFrequency_MHz = reader.GetValue(c => c.StartFrequency_MHz).Value;
                    if (reader.GetValue(c => c.StopFrequency_MHz).HasValue)
                        emitting.StopFrequency_MHz = reader.GetValue(c => c.StopFrequency_MHz).Value;
                    if (reader.GetValue(c => c.CurentPower_dBm).HasValue)
                        emitting.CurentPower_dBm = reader.GetValue(c => c.CurentPower_dBm).Value;
                    if (reader.GetValue(c => c.ReferenceLevel_dBm).HasValue)
                        emitting.ReferenceLevel_dBm = reader.GetValue(c => c.ReferenceLevel_dBm).Value;
                    if (reader.GetValue(c => c.MeanDeviationFromReference).HasValue)
                        emitting.MeanDeviationFromReference = reader.GetValue(c => c.MeanDeviationFromReference).Value;
                    if (reader.GetValue(c => c.TriggerDeviationFromReference).HasValue)
                        emitting.TriggerDeviationFromReference = reader.GetValue(c => c.TriggerDeviationFromReference).Value;

                    if (reader.GetValue(c => c.SensorId).HasValue)
                    {
                        if (listSensors.Find(x => x.Id.Value == reader.GetValue(c => c.SensorId).Value) == null)
                        {
                            var querySensor = this._dataLayer.GetBuilder<MD.ISensor>()
                            .From()
                            .Select(c => c.Id, c => c.Name, c => c.TechId)
                            .Where(c => c.Id, ConditionOperator.Equal, reader.GetValue(c => c.SensorId).Value);
                            queryExecuter.Fetch(querySensor, readerSensor =>
                            {
                                while (readerSensor.Read())
                                {
                                    emitting.SensorName = readerSensor.GetValue(c => c.Name);
                                    emitting.SensorTechId = readerSensor.GetValue(c => c.TechId);
                                    listSensors.Add(new Sensor()
                                    {
                                        Id = new SensorIdentifier()
                                        {
                                            Value = reader.GetValue(c => c.SensorId).Value
                                        },
                                        Name = readerSensor.GetValue(c => c.Name),
                                        Equipment = new SensorEquip()
                                        {
                                            TechId = readerSensor.GetValue(c => c.TechId)
                                        }
                                    });
                                    break;
                                }
                                return true;
                            });
                        }
                        else
                        {
                            var fndSensor = listSensors.Find(x => x.Id.Value == reader.GetValue(c => c.SensorId).Value);
                            if (fndSensor != null)
                            {
                                emitting.SensorName = fndSensor.Name;
                                emitting.SensorTechId = fndSensor.Equipment.TechId;
                            }
                        }
                    }

                    var emittingParam = new EmittingParameters();

                    if (reader.GetValue(c => c.RollOffFactor).HasValue)
                        emittingParam.RollOffFactor = reader.GetValue(c => c.RollOffFactor).Value;
                    if (reader.GetValue(c => c.StandardBW).HasValue)
                    {
                        emittingParam.StandardBW = reader.GetValue(c => c.StandardBW).Value;
                    }

                    var listLevel = new List<int>();
                    var listCount = new List<int>();

                    if (reader.GetValue(c => c.LevelsDistribution) != null)
                    {
                        var objLevelsDistribution = BinaryDecoder.Deserialize<string>(reader.GetValue(c => c.LevelsDistribution));
                        if (!string.IsNullOrEmpty(objLevelsDistribution))
                        {
                            var wrds = objLevelsDistribution.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                            if ((wrds != null) && (wrds.Length > 0))
                            {
                                for (int h = 0; h < wrds.Length; h++)
                                {
                                    var oneString = wrds[h];
                                    if (!string.IsNullOrEmpty(oneString))
                                    {
                                        var wrdLevelCount = oneString.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                                        if ((wrdLevelCount != null) && (wrdLevelCount.Length == 2))
                                        {
                                            int levelValue; int countValue;
                                            if ((int.TryParse(wrdLevelCount[0], out levelValue)) && (int.TryParse(wrdLevelCount[1], out countValue)))
                                            {
                                                listLevel.Add(levelValue);
                                                listCount.Add(countValue);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    var levelDist = new LevelsDistribution();
                    if (listLevel.Count > 0)
                        levelDist.Levels = listLevel.ToArray();
                    if (listCount.Count > 0)
                        levelDist.Count = listCount.ToArray();
                    emitting.LevelsDistribution = levelDist;
                    emitting.EmittingParameters = emittingParam;
                    listIdsEmittings.Add(reader.GetValue(c => c.Id));
                    listEmitings.Add(new KeyValuePair<int, Emitting>(reader.GetValue(c => c.Id), emitting));
                }
                return true;
            });

            var listIntEmittingWorkTime = BreakDownElemBlocks.BreakDown(listIdsEmittings.ToArray());
            for (int i = 0; i < listIntEmittingWorkTime.Count; i++)
            {
                var queryTime = this._dataLayer.GetBuilder<MD.IWorkTime>()
                      .From()
                      .Select(c => c.Id, c => c.StartEmitting, c => c.StopEmitting, c => c.HitCount, c => c.PersentAvailability, c => c.EMITTING.Id)
                      .Where(c => c.EMITTING.Id, ConditionOperator.In, listIntEmittingWorkTime[i]);
                queryExecuter.Fetch(queryTime, readerTime =>
                {
                    while (readerTime.Read())
                    {
                        var workTime = new WorkTime();
                        if (readerTime.GetValue(c => c.StartEmitting).HasValue)
                            workTime.StartEmitting = readerTime.GetValue(c => c.StartEmitting).Value;
                        if (readerTime.GetValue(c => c.StopEmitting).HasValue)
                            workTime.StopEmitting = readerTime.GetValue(c => c.StopEmitting).Value;
                        if (readerTime.GetValue(c => c.HitCount).HasValue)
                            workTime.HitCount = readerTime.GetValue(c => c.HitCount).Value;
                        if (readerTime.GetValue(c => c.PersentAvailability).HasValue)
                            workTime.PersentAvailability = readerTime.GetValue(c => c.PersentAvailability).Value;

                        listWorkTimes.Add(new KeyValuePair<int, WorkTime>(readerTime.GetValue(c => c.EMITTING.Id), workTime));
                    }
                    return true;
                });
            }



            var listIntEmittingSignalMask = BreakDownElemBlocks.BreakDown(listIdsEmittings.ToArray());
            for (int i = 0; i < listIntEmittingSignalMask.Count; i++)
            {
                var listLoss_dB = new List<float>();
                var listFreq_kHz = new List<double>();
                var querySignalMask = this._dataLayer.GetBuilder<MD.ISignalMask>()
                .From()
                .Select(c => c.Id, c => c.Loss_dB, c => c.Freq_kHz, c => c.EMITTING.Id)
                .Where(c => c.EMITTING.Id, ConditionOperator.In, listIntEmittingSignalMask[i]);
                queryExecuter.Fetch(querySignalMask, readerSignalMask =>
                {
                    while (readerSignalMask.Read())
                    {
                        listLoss_dB = new List<float>();
                        listFreq_kHz = new List<double>();

                        if (readerSignalMask.GetValue(c => c.Loss_dB).HasValue)
                            listLoss_dB.Add(readerSignalMask.GetValue(c => c.Loss_dB).Value);

                        if (readerSignalMask.GetValue(c => c.Freq_kHz).HasValue)
                            listFreq_kHz.Add(readerSignalMask.GetValue(c => c.Freq_kHz).Value);

                        var signalMask = new SignalMask();
                        if (listLoss_dB.Count > 0)
                            signalMask.Loss_dB = listLoss_dB.ToArray();
                        if (listFreq_kHz.Count > 0)
                            signalMask.Freq_kHz = listFreq_kHz.ToArray();

                        listSignalMask.Add(new KeyValuePair<int, SignalMask>(readerSignalMask.GetValue(c => c.EMITTING.Id), signalMask));
                    }
                    return true;
                });
            }

            var listIntEmittingSpectrum = BreakDownElemBlocks.BreakDown(listIdsEmittings.ToArray());
            for (int i = 0; i < listIntEmittingSpectrum.Count; i++)
            {
                var querySpectrum = this._dataLayer.GetBuilder<MD.ISpectrum>()
                       .From()
                       .Select(c => c.Id, c => c.LevelsdBm, c => c.SpectrumStartFreq_MHz, c => c.SpectrumSteps_kHz, c => c.Bandwidth_kHz, c => c.TraceCount, c => c.SignalLevel_dBm, c => c.MarkerIndex, c => c.MarkerIndex, c => c.CorrectnessEstimations, c => c.T1, c => c.T2, c => c.EMITTING.Id, c => c.Contravention)
                       .Where(c => c.EMITTING.Id, ConditionOperator.In, listIntEmittingSpectrum[i]);
                queryExecuter.Fetch(querySpectrum, readerSpectrum =>
                {
                    while (readerSpectrum.Read())
                    {
                        var spectrum = new Spectrum();
                        var listLevelsdBm = new List<float>();

                        if (readerSpectrum.GetValue(c => c.LevelsdBm) != null)
                        {
                            var levelsdBm = BinaryDecoder.Deserialize<float[]>(readerSpectrum.GetValue(c => c.LevelsdBm));
                            if (levelsdBm != null)
                            {
                                listLevelsdBm.AddRange(levelsdBm as float[]);

                                ////////////////////
                                /*
                                if (readerSpectrum.GetValue(c => c.Id) == 1373893)
                                {
                                    var lelvDbm = levelsdBm as float[];
                                    spectrum.SpectrumStartFreq_MHz = readerSpectrum.GetValue(c => c.SpectrumStartFreq_MHz).Value;
                                    spectrum.SpectrumSteps_kHz = readerSpectrum.GetValue(c => c.SpectrumSteps_kHz).Value;
                                    //int[] startStop;
                                    //var val = Counting(lelvDbm, 0, lelvDbm.Length - 1, out startStop);
                                }

                                if (readerSpectrum.GetValue(c => c.Id) == 1373894)
                                {
                                    var lelvDbm = levelsdBm as float[];
                                    spectrum.SpectrumStartFreq_MHz = readerSpectrum.GetValue(c => c.SpectrumStartFreq_MHz).Value;
                                    spectrum.SpectrumSteps_kHz = readerSpectrum.GetValue(c => c.SpectrumSteps_kHz).Value;
                                    //int[] startStop;
                                    //var val = Counting(lelvDbm, 0, lelvDbm.Length - 1, out startStop);
                                }
                                */
                                /////
                            }
                        }
                       
                        if (spectrum.SpectrumStartFreq_MHz == 0)
                        {
                            if (readerSpectrum.GetValue(c => c.SpectrumStartFreq_MHz).HasValue)
                            {
                                spectrum.SpectrumStartFreq_MHz = readerSpectrum.GetValue(c => c.SpectrumStartFreq_MHz).Value;
                            }


                            if (readerSpectrum.GetValue(c => c.SpectrumSteps_kHz).HasValue)
                            {
                                spectrum.SpectrumSteps_kHz = readerSpectrum.GetValue(c => c.SpectrumSteps_kHz).Value;
                            }


                            if (readerSpectrum.GetValue(c => c.Bandwidth_kHz).HasValue)
                            {
                                spectrum.Bandwidth_kHz = readerSpectrum.GetValue(c => c.Bandwidth_kHz).Value;
                            }


                            if (readerSpectrum.GetValue(c => c.TraceCount).HasValue)
                            {
                                spectrum.TraceCount = readerSpectrum.GetValue(c => c.TraceCount).Value;
                            }


                            if (readerSpectrum.GetValue(c => c.SignalLevel_dBm).HasValue)
                            {
                                spectrum.SignalLevel_dBm = readerSpectrum.GetValue(c => c.SignalLevel_dBm).Value;
                            }


                            if (readerSpectrum.GetValue(c => c.MarkerIndex).HasValue)
                            {
                                spectrum.MarkerIndex = readerSpectrum.GetValue(c => c.MarkerIndex).Value;
                            }


                            if (readerSpectrum.GetValue(c => c.T1).HasValue)
                            {
                                spectrum.T1 = readerSpectrum.GetValue(c => c.T1).Value;
                            }


                            if (readerSpectrum.GetValue(c => c.T2).HasValue)
                            {
                                spectrum.T2 = readerSpectrum.GetValue(c => c.T2).Value;
                            }

                            if (readerSpectrum.GetValue(c => c.CorrectnessEstimations).HasValue)
                            {
                                spectrum.CorrectnessEstimations = readerSpectrum.GetValue(c => c.CorrectnessEstimations).Value == 1 ? true : false;
                            }

                            if (readerSpectrum.GetValue(c => c.Contravention).HasValue)
                            {
                                spectrum.Contravention = readerSpectrum.GetValue(c => c.Contravention).Value == 1 ? true : false;
                            }
                        }
                        spectrum.Levels_dBm = listLevelsdBm.ToArray();
                        listSpectrum.Add(new KeyValuePair<int, Spectrum>(readerSpectrum.GetValue(c => c.EMITTING.Id), spectrum));
                    }
                    return true;
                });
            }

            var arrayListIdsEmittings = listIdsEmittings.ToArray();
            for (int i = 0; i < arrayListIdsEmittings.Length; i++)
            {
                var id = arrayListIdsEmittings[i];
                var fndEmitings = listEmitings.Find(c => c.Key == id);
                if (fndEmitings.Value != null)
                {
                    var emitting = fndEmitings.Value;

                    var fndWorkTime = listWorkTimes.FindAll(c => c.Key == id);
                    if (fndWorkTime != null)
                    {
                        var arrayFndWorkTime = fndWorkTime.ToArray();
                        var listWorkTime = new List<WorkTime>();
                        for (int t = 0; t < arrayFndWorkTime.Length; t++)
                        {
                            listWorkTime.Add(arrayFndWorkTime[t].Value);
                        }
                        if (listWorkTime.Count > 0)
                        {
                            emitting.WorkTimes = listWorkTime.ToArray();
                        }
                    }

                    listWorkTimes.RemoveAll(c => c.Key == id);
                    var fndSignalMask = listSignalMask.FindAll(c => c.Key == id);
                    if (fndSignalMask != null)
                    {
                        var arrayFndSignalMask = fndSignalMask.ToArray();
                        var signalMask = new SignalMask();
                        var listLoss_dB = new List<float>();
                        var listFreq_kHz = new List<double>();
                        for (int t = 0; t < arrayFndSignalMask.Length; t++)
                        {
                            var x = arrayFndSignalMask[t];
                            listLoss_dB.AddRange(x.Value.Loss_dB);
                            listFreq_kHz.AddRange(x.Value.Freq_kHz);
                        }
                        if (listLoss_dB.Count > 0)
                        {
                            signalMask.Loss_dB = listLoss_dB.ToArray();
                            signalMask.Freq_kHz = listFreq_kHz.ToArray();
                        }
                        emitting.SignalMask = signalMask;
                    }
                    listSignalMask.RemoveAll(c => c.Key == id);


                    var fndSpectrum = listSpectrum.FindAll(c => c.Key == id);
                    if (fndSpectrum != null)
                    {
                        var arrayFndSpectrum = fndSpectrum.ToArray();
                        var listLevelsdBm = new List<float>();
                        for (int t = 0; t < arrayFndSpectrum.Length; t++)
                        {
                            var x = arrayFndSpectrum[t];
                            if (x.Value != null)
                            {
                                listLevelsdBm.AddRange(x.Value.Levels_dBm);
                                emitting.Spectrum = x.Value;
                            }
                        }
                        if (listLevelsdBm.Count > 0)
                        {
                            emitting.Spectrum.Levels_dBm = listLevelsdBm.ToArray();
                        }
                    }
                    listSpectrum.RemoveAll(c => c.Key == id);
                    listEmitting.Add(emitting);
                }
                listEmitings.RemoveAll(c => c.Key == id);
            }
            if (listEmitting.Count > 0)
            {
                emittings = listEmitting.ToArray();
            }
            referenceLevels = GetReferenceLevelsByResId(resId, isLoadAllData, StartFrequency_Hz, StopFrequency_Hz);
        }


        public Emitting[] GetEmittingsByIcsmId(int[] ids, string icsmTableName)
        {
            var listIdsEmittings = new List<int>();
            var listEmitings = new List<KeyValuePair<int, Emitting>>();
            var listWorkTimes = new List<KeyValuePair<int, WorkTime>>();
            var listSignalMask = new List<KeyValuePair<int, SignalMask>>();
            var listSpectrum = new List<KeyValuePair<int, Spectrum>>();
            var listSensors = new List<Sensor>();
            var listEmitting = new List<Emitting>();
            var listIntids = BreakDownElemBlocks.BreakDown(ids);
            var arrIntids = listIntids.ToArray();
            var queryExecuter = this._dataLayer.Executor<SdrnServerDataContext>();
            for (int l = 0; l < arrIntids.Length; l++)
            {
                int?[] listIntIdsConvert = listIntids[l].Select(n => (int?)(n)).ToArray();
                var queryEmitting = this._dataLayer.GetBuilder<MD.IEmitting>()
            .From()
            .Select(c => c.Id, c => c.CurentPower_dBm, c => c.MeanDeviationFromReference, c => c.ReferenceLevel_dBm, c => c.RollOffFactor, c => c.StandardBW, c => c.StartFrequency_MHz, c => c.StopFrequency_MHz, c => c.TriggerDeviationFromReference, c => c.LevelsDistribution, c => c.SensorId, c => c.StationID, c => c.StationTableName)
            .OrderByAsc(c => c.StartFrequency_MHz)
            .Where(c => c.StationID, ConditionOperator.In, listIntIdsConvert)
                .Where(c => c.StationTableName, ConditionOperator.Equal, icsmTableName);
                queryExecuter.Fetch(queryEmitting, reader =>
            {
                while (reader.Read())
                {
                    var emitting = new Emitting();
                    emitting.Id = reader.GetValue(c => c.Id);
                    if (reader.GetValue(c => c.StationID).HasValue)
                    {
                        emitting.AssociatedStationID = reader.GetValue(c => c.StationID).Value;
                    }
                    emitting.AssociatedStationTableName = reader.GetValue(c => c.StationTableName);

                    if (reader.GetValue(c => c.StartFrequency_MHz).HasValue)
                        emitting.StartFrequency_MHz = reader.GetValue(c => c.StartFrequency_MHz).Value;
                    if (reader.GetValue(c => c.StopFrequency_MHz).HasValue)
                        emitting.StopFrequency_MHz = reader.GetValue(c => c.StopFrequency_MHz).Value;
                    if (reader.GetValue(c => c.CurentPower_dBm).HasValue)
                        emitting.CurentPower_dBm = reader.GetValue(c => c.CurentPower_dBm).Value;
                    if (reader.GetValue(c => c.ReferenceLevel_dBm).HasValue)
                        emitting.ReferenceLevel_dBm = reader.GetValue(c => c.ReferenceLevel_dBm).Value;
                    if (reader.GetValue(c => c.MeanDeviationFromReference).HasValue)
                        emitting.MeanDeviationFromReference = reader.GetValue(c => c.MeanDeviationFromReference).Value;
                    if (reader.GetValue(c => c.TriggerDeviationFromReference).HasValue)
                        emitting.TriggerDeviationFromReference = reader.GetValue(c => c.TriggerDeviationFromReference).Value;

                    if (reader.GetValue(c => c.SensorId).HasValue)
                    {
                        if (listSensors.Find(x => x.Id.Value == reader.GetValue(c => c.SensorId).Value) == null)
                        {
                            var querySensor = this._dataLayer.GetBuilder<MD.ISensor>()
                            .From()
                            .Select(c => c.Id, c => c.Name, c => c.TechId)
                            .Where(c => c.Id, ConditionOperator.Equal, reader.GetValue(c => c.SensorId).Value);
                            queryExecuter.Fetch(querySensor, readerSensor =>
                            {
                                while (readerSensor.Read())
                                {
                                    emitting.SensorName = readerSensor.GetValue(c => c.Name);
                                    emitting.SensorTechId = readerSensor.GetValue(c => c.TechId);
                                    listSensors.Add(new Sensor()
                                    {
                                        Id = new SensorIdentifier()
                                        {
                                            Value = reader.GetValue(c => c.SensorId).Value
                                        },
                                        Name = readerSensor.GetValue(c => c.Name),
                                        Equipment = new SensorEquip()
                                        {
                                            TechId = readerSensor.GetValue(c => c.TechId)
                                        }
                                    });
                                    break;
                                }
                                return true;
                            });
                        }
                        else
                        {
                            var fndSensor = listSensors.Find(x => x.Id.Value == reader.GetValue(c => c.SensorId).Value);
                            if (fndSensor != null)
                            {
                                emitting.SensorName = fndSensor.Name;
                                emitting.SensorTechId = fndSensor.Equipment.TechId;
                            }
                        }
                    }

                    var emittingParam = new EmittingParameters();

                    if (reader.GetValue(c => c.RollOffFactor).HasValue)
                        emittingParam.RollOffFactor = reader.GetValue(c => c.RollOffFactor).Value;
                    if (reader.GetValue(c => c.StandardBW).HasValue)
                    {
                        emittingParam.StandardBW = reader.GetValue(c => c.StandardBW).Value;
                    }

                    var listLevel = new List<int>();
                    var listCount = new List<int>();

                    if (reader.GetValue(c => c.LevelsDistribution) != null)
                    {
                        var objLevelsDistribution = BinaryDecoder.Deserialize<string>(reader.GetValue(c => c.LevelsDistribution));
                        if (!string.IsNullOrEmpty(objLevelsDistribution))
                        {
                            var wrds = objLevelsDistribution.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                            if ((wrds != null) && (wrds.Length > 0))
                            {
                                for (int h = 0; h < wrds.Length; h++)
                                {
                                    var oneString = wrds[h];
                                    if (!string.IsNullOrEmpty(oneString))
                                    {
                                        var wrdLevelCount = oneString.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                                        if ((wrdLevelCount != null) && (wrdLevelCount.Length == 2))
                                        {
                                            int levelValue; int countValue;
                                            if ((int.TryParse(wrdLevelCount[0], out levelValue)) && (int.TryParse(wrdLevelCount[1], out countValue)))
                                            {
                                                listLevel.Add(levelValue);
                                                listCount.Add(countValue);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    var levelDist = new LevelsDistribution();
                    if (listLevel.Count > 0)
                        levelDist.Levels = listLevel.ToArray();
                    if (listCount.Count > 0)
                        levelDist.Count = listCount.ToArray();
                    emitting.LevelsDistribution = levelDist;
                    emitting.EmittingParameters = emittingParam;
                    listIdsEmittings.Add(reader.GetValue(c => c.Id));
                    listEmitings.Add(new KeyValuePair<int, Emitting>(reader.GetValue(c => c.Id), emitting));
                }
                return true;
            });

                var listIntEmittingWorkTime = BreakDownElemBlocks.BreakDown(listIdsEmittings.ToArray());
                for (int i = 0; i < listIntEmittingWorkTime.Count; i++)
                {
                    var queryTime = this._dataLayer.GetBuilder<MD.IWorkTime>()
                          .From()
                          .Select(c => c.Id, c => c.StartEmitting, c => c.StopEmitting, c => c.HitCount, c => c.PersentAvailability, c => c.EMITTING.Id)
                          .Where(c => c.EMITTING.Id, ConditionOperator.In, listIntEmittingWorkTime[i]);
                    queryExecuter.Fetch(queryTime, readerTime =>
                    {
                        while (readerTime.Read())
                        {
                            var workTime = new WorkTime();
                            if (readerTime.GetValue(c => c.StartEmitting).HasValue)
                                workTime.StartEmitting = readerTime.GetValue(c => c.StartEmitting).Value;
                            if (readerTime.GetValue(c => c.StopEmitting).HasValue)
                                workTime.StopEmitting = readerTime.GetValue(c => c.StopEmitting).Value;
                            if (readerTime.GetValue(c => c.HitCount).HasValue)
                                workTime.HitCount = readerTime.GetValue(c => c.HitCount).Value;
                            if (readerTime.GetValue(c => c.PersentAvailability).HasValue)
                                workTime.PersentAvailability = readerTime.GetValue(c => c.PersentAvailability).Value;

                            listWorkTimes.Add(new KeyValuePair<int, WorkTime>(readerTime.GetValue(c => c.EMITTING.Id), workTime));
                        }
                        return true;
                    });
                }



                var listIntEmittingSignalMask = BreakDownElemBlocks.BreakDown(listIdsEmittings.ToArray());
                for (int i = 0; i < listIntEmittingSignalMask.Count; i++)
                {
                    var listLoss_dB = new List<float>();
                    var listFreq_kHz = new List<double>();
                    var querySignalMask = this._dataLayer.GetBuilder<MD.ISignalMask>()
                    .From()
                    .Select(c => c.Id, c => c.Loss_dB, c => c.Freq_kHz, c => c.EMITTING.Id)
                    .Where(c => c.EMITTING.Id, ConditionOperator.In, listIntEmittingSignalMask[i]);
                    queryExecuter.Fetch(querySignalMask, readerSignalMask =>
                    {
                        while (readerSignalMask.Read())
                        {
                            listLoss_dB = new List<float>();
                            listFreq_kHz = new List<double>();

                            if (readerSignalMask.GetValue(c => c.Loss_dB).HasValue)
                                listLoss_dB.Add(readerSignalMask.GetValue(c => c.Loss_dB).Value);

                            if (readerSignalMask.GetValue(c => c.Freq_kHz).HasValue)
                                listFreq_kHz.Add(readerSignalMask.GetValue(c => c.Freq_kHz).Value);

                            var signalMask = new SignalMask();
                            if (listLoss_dB.Count > 0)
                                signalMask.Loss_dB = listLoss_dB.ToArray();
                            if (listFreq_kHz.Count > 0)
                                signalMask.Freq_kHz = listFreq_kHz.ToArray();

                            listSignalMask.Add(new KeyValuePair<int, SignalMask>(readerSignalMask.GetValue(c => c.EMITTING.Id), signalMask));
                        }
                        return true;
                    });
                }

                var listIntEmittingSpectrum = BreakDownElemBlocks.BreakDown(listIdsEmittings.ToArray());
                for (int i = 0; i < listIntEmittingSpectrum.Count; i++)
                {
                    var querySpectrum = this._dataLayer.GetBuilder<MD.ISpectrum>()
                           .From()
                           .Select(c => c.Id, c => c.LevelsdBm, c => c.SpectrumStartFreq_MHz, c => c.SpectrumSteps_kHz, c => c.Bandwidth_kHz, c => c.TraceCount, c => c.SignalLevel_dBm, c => c.MarkerIndex, c => c.MarkerIndex, c => c.CorrectnessEstimations, c => c.T1, c => c.T2, c => c.EMITTING.Id, c => c.Contravention)
                           .Where(c => c.EMITTING.Id, ConditionOperator.In, listIntEmittingSpectrum[i]);
                    queryExecuter.Fetch(querySpectrum, readerSpectrum =>
                    {
                        while (readerSpectrum.Read())
                        {
                            var spectrum = new Spectrum();
                            var listLevelsdBm = new List<float>();

                            if (readerSpectrum.GetValue(c => c.LevelsdBm) != null)
                            {
                                var levelsdBm = BinaryDecoder.Deserialize<float[]>(readerSpectrum.GetValue(c => c.LevelsdBm));
                                if (levelsdBm != null)
                                {
                                    listLevelsdBm.AddRange(levelsdBm as float[]);
                                }
                            }

                            if (spectrum.SpectrumStartFreq_MHz == 0)
                            {
                                if (readerSpectrum.GetValue(c => c.SpectrumStartFreq_MHz).HasValue)
                                {
                                    spectrum.SpectrumStartFreq_MHz = readerSpectrum.GetValue(c => c.SpectrumStartFreq_MHz).Value;
                                }


                                if (readerSpectrum.GetValue(c => c.SpectrumSteps_kHz).HasValue)
                                {
                                    spectrum.SpectrumSteps_kHz = readerSpectrum.GetValue(c => c.SpectrumSteps_kHz).Value;
                                }


                                if (readerSpectrum.GetValue(c => c.Bandwidth_kHz).HasValue)
                                {
                                    spectrum.Bandwidth_kHz = readerSpectrum.GetValue(c => c.Bandwidth_kHz).Value;
                                }


                                if (readerSpectrum.GetValue(c => c.TraceCount).HasValue)
                                {
                                    spectrum.TraceCount = readerSpectrum.GetValue(c => c.TraceCount).Value;
                                }


                                if (readerSpectrum.GetValue(c => c.SignalLevel_dBm).HasValue)
                                {
                                    spectrum.SignalLevel_dBm = readerSpectrum.GetValue(c => c.SignalLevel_dBm).Value;
                                }


                                if (readerSpectrum.GetValue(c => c.MarkerIndex).HasValue)
                                {
                                    spectrum.MarkerIndex = readerSpectrum.GetValue(c => c.MarkerIndex).Value;
                                }


                                if (readerSpectrum.GetValue(c => c.T1).HasValue)
                                {
                                    spectrum.T1 = readerSpectrum.GetValue(c => c.T1).Value;
                                }


                                if (readerSpectrum.GetValue(c => c.T2).HasValue)
                                {
                                    spectrum.T2 = readerSpectrum.GetValue(c => c.T2).Value;
                                }

                                if (readerSpectrum.GetValue(c => c.CorrectnessEstimations).HasValue)
                                {
                                    spectrum.CorrectnessEstimations = readerSpectrum.GetValue(c => c.CorrectnessEstimations).Value == 1 ? true : false;
                                }

                                if (readerSpectrum.GetValue(c => c.Contravention).HasValue)
                                {
                                    spectrum.Contravention = readerSpectrum.GetValue(c => c.Contravention).Value == 1 ? true : false;
                                }
                            }
                            spectrum.Levels_dBm = listLevelsdBm.ToArray();
                            listSpectrum.Add(new KeyValuePair<int, Spectrum>(readerSpectrum.GetValue(c => c.EMITTING.Id), spectrum));
                        }
                        return true;
                    });
                }

                var arrayListIdsEmittings = listIdsEmittings.ToArray();
                for (int i = 0; i < arrayListIdsEmittings.Length; i++)
                {
                    var id = arrayListIdsEmittings[i];
                    var fndEmitings = listEmitings.Find(c => c.Key == id);
                    if (fndEmitings.Value != null)
                    {
                        var emitting = fndEmitings.Value;

                        var fndWorkTime = listWorkTimes.FindAll(c => c.Key == id);
                        if (fndWorkTime != null)
                        {
                            var arrayFndWorkTime = fndWorkTime.ToArray();
                            var listWorkTime = new List<WorkTime>();
                            for (int t = 0; t < arrayFndWorkTime.Length; t++)
                            {
                                listWorkTime.Add(arrayFndWorkTime[t].Value);
                            }
                            if (listWorkTime.Count > 0)
                            {
                                emitting.WorkTimes = listWorkTime.ToArray();
                            }
                        }

                        listWorkTimes.RemoveAll(c => c.Key == id);
                        var fndSignalMask = listSignalMask.FindAll(c => c.Key == id);
                        if (fndSignalMask != null)
                        {
                            var arrayFndSignalMask = fndSignalMask.ToArray();
                            var signalMask = new SignalMask();
                            var listLoss_dB = new List<float>();
                            var listFreq_kHz = new List<double>();
                            for (int t = 0; t < arrayFndSignalMask.Length; t++)
                            {
                                var x = arrayFndSignalMask[t];
                                listLoss_dB.AddRange(x.Value.Loss_dB);
                                listFreq_kHz.AddRange(x.Value.Freq_kHz);
                            }
                            if (listLoss_dB.Count > 0)
                            {
                                signalMask.Loss_dB = listLoss_dB.ToArray();
                                signalMask.Freq_kHz = listFreq_kHz.ToArray();
                            }
                            emitting.SignalMask = signalMask;
                        }
                        listSignalMask.RemoveAll(c => c.Key == id);


                        var fndSpectrum = listSpectrum.FindAll(c => c.Key == id);
                        if (fndSpectrum != null)
                        {
                            var arrayFndSpectrum = fndSpectrum.ToArray();
                            var listLevelsdBm = new List<float>();
                            for (int t = 0; t < arrayFndSpectrum.Length; t++)
                            {
                                var x = arrayFndSpectrum[t];
                                if (x.Value != null)
                                {
                                    listLevelsdBm.AddRange(x.Value.Levels_dBm);
                                    emitting.Spectrum = x.Value;
                                }
                            }
                            if (listLevelsdBm.Count > 0)
                            {
                                emitting.Spectrum.Levels_dBm = listLevelsdBm.ToArray();
                            }
                        }
                        listSpectrum.RemoveAll(c => c.Key == id);
                        listEmitting.Add(emitting);
                    }
                    listEmitings.RemoveAll(c => c.Key == id);
                }
            }
            return listEmitting.ToArray();
        }

        public ReferenceLevels GetReferenceLevelsByResId(int resId, bool isLoadAllData, double? StartFrequency_Hz = null, double? StopFrequency_Hz = null)
        {
            var queryExecuter = this._dataLayer.Executor<SdrnServerDataContext>();
            var referenceLevels = new ReferenceLevels();
            var level = new ReferenceLevels();
            var taskId = "";
            int? subMeasTaskId = null;
            int? subMeasTaskStaId = null;
            int? sensorId = null;

            var queryMeasTaskId = this._dataLayer.GetBuilder<MD.IReferenceLevels>()
          .From()
          .Select(c => c.Id, c => c.RESMEAS.MeasTaskId, c => c.RESMEAS.MeasSubTaskId, c => c.RESMEAS.MeasSubTaskStationId, c => c.RESMEAS.SensorId)
          .Where(c => c.ResMeasId, ConditionOperator.Equal, resId);
            queryExecuter.Fetch(queryMeasTaskId, readerReferenceLevels =>
            {
                if (readerReferenceLevels.Read())
                {
                    taskId = readerReferenceLevels.GetValue(c => c.RESMEAS.MeasTaskId);
                    if (readerReferenceLevels.GetValue(c => c.RESMEAS.MeasSubTaskId).HasValue)
                    {
                        subMeasTaskId = readerReferenceLevels.GetValue(c => c.RESMEAS.MeasSubTaskId).Value;
                    }
                    if (readerReferenceLevels.GetValue(c => c.RESMEAS.MeasSubTaskStationId).HasValue)
                    {
                        subMeasTaskStaId = readerReferenceLevels.GetValue(c => c.RESMEAS.MeasSubTaskStationId).Value;
                    }
                    if (readerReferenceLevels.GetValue(c => c.RESMEAS.SensorId).HasValue)
                    {
                        sensorId = readerReferenceLevels.GetValue(c => c.RESMEAS.SensorId).Value;
                    }
                }
                return true;
            });
            if (!string.IsNullOrEmpty(taskId) && (subMeasTaskId != null) && (subMeasTaskStaId != null) && (sensorId != null))
            {
                var queryLevels = this._dataLayer.GetBuilder<MD.IReferenceLevels>()
               .From()
               .Select(c => c.Id, c => c.StartFrequency_Hz, c => c.StepFrequency_Hz, c => c.ReferenceLevels)
               .Where(c => c.RESMEAS.MeasTaskId, ConditionOperator.Equal, taskId)
               .Where(c => c.RESMEAS.MeasSubTaskId, ConditionOperator.Equal, subMeasTaskId)
               .Where(c => c.RESMEAS.MeasSubTaskStationId, ConditionOperator.Equal, subMeasTaskStaId)
               .Where(c => c.RESMEAS.SensorId, ConditionOperator.Equal, sensorId);
                queryExecuter.Fetch(queryLevels, readerLevels =>
                {
                    while (readerLevels.Read())
                    {
                        if (readerLevels.GetValue(c => c.StartFrequency_Hz).HasValue)
                        {
                            level.StartFrequency_Hz = readerLevels.GetValue(c => c.StartFrequency_Hz).Value;
                        }

                        if (readerLevels.GetValue(c => c.StepFrequency_Hz).HasValue)
                        {
                            level.StepFrequency_Hz = readerLevels.GetValue(c => c.StepFrequency_Hz).Value;
                        }

                        if (readerLevels.GetValue(c => c.ReferenceLevels) != null)
                        {
                            var refLevels = BinaryDecoder.Deserialize<float[]>(readerLevels.GetValue(c => c.ReferenceLevels));
                            if (refLevels != null)
                            {
                                level.levels = refLevels as float[];
                            }
                        }
                    }
                    return true;
                });
            }
            if (isLoadAllData == false)
            {
                referenceLevels = ReferenceLevelsCut(level, StartFrequency_Hz, StopFrequency_Hz);
            }
            else
            {
                referenceLevels = level;
            }
            return referenceLevels;
        }

        public ReferenceLevels GetReferenceLevelsByResultId(int resId, bool isLoadAllData, double? StartFrequency_Hz = null, double? StopFrequency_Hz = null)
        {
            var queryExecuter = this._dataLayer.Executor<SdrnServerDataContext>();
            var level = new ReferenceLevels();
            var taskId = "";
            int? subMeasTaskId = null;
            int? subMeasTaskStaId = null;
            int? sensorId = null;

            var queryMeasTaskId = this._dataLayer.GetBuilder<MD.IReferenceLevels>()
          .From()
          .Select(c => c.Id, c => c.RESMEAS.MeasTaskId, c => c.RESMEAS.MeasSubTaskId, c => c.RESMEAS.MeasSubTaskStationId, c => c.RESMEAS.SensorId)
          .Where(c => c.ResMeasId, ConditionOperator.Equal, resId);
            queryExecuter.Fetch(queryMeasTaskId, readerReferenceLevels =>
            {
                if (readerReferenceLevels.Read())
                {
                    taskId = readerReferenceLevels.GetValue(c => c.RESMEAS.MeasTaskId);
                    if (readerReferenceLevels.GetValue(c => c.RESMEAS.MeasSubTaskId).HasValue)
                    {
                        subMeasTaskId = readerReferenceLevels.GetValue(c => c.RESMEAS.MeasSubTaskId).Value;
                    }
                    if (readerReferenceLevels.GetValue(c => c.RESMEAS.MeasSubTaskStationId).HasValue)
                    {
                        subMeasTaskStaId = readerReferenceLevels.GetValue(c => c.RESMEAS.MeasSubTaskStationId).Value;
                    }
                    if (readerReferenceLevels.GetValue(c => c.RESMEAS.SensorId).HasValue)
                    {
                        sensorId = readerReferenceLevels.GetValue(c => c.RESMEAS.SensorId).Value;
                    }
                }
                return true;
            });
            if (!string.IsNullOrEmpty(taskId) && (subMeasTaskId != null) && (subMeasTaskStaId != null) && (sensorId != null))
            {
                var queryLevels = this._dataLayer.GetBuilder<MD.IReferenceLevels>()
               .From()
               .Select(c => c.Id, c => c.StartFrequency_Hz, c => c.StepFrequency_Hz, c => c.ReferenceLevels)
               .Where(c => c.RESMEAS.MeasTaskId, ConditionOperator.Equal, taskId)
               .Where(c => c.RESMEAS.MeasSubTaskId, ConditionOperator.Equal, subMeasTaskId)
               .Where(c => c.RESMEAS.MeasSubTaskStationId, ConditionOperator.Equal, subMeasTaskStaId)
               .Where(c => c.RESMEAS.SensorId, ConditionOperator.Equal, sensorId);
               //.Where(c => c.RESMEAS.MeasResultSID, ConditionOperator.Equal, "1");
                queryExecuter.Fetch(queryLevels, readerLevels =>
                {
                    while (readerLevels.Read())
                    {
                        if (readerLevels.GetValue(c => c.StartFrequency_Hz).HasValue)
                        {
                            level.StartFrequency_Hz = readerLevels.GetValue(c => c.StartFrequency_Hz).Value;
                        }

                        if (readerLevels.GetValue(c => c.StepFrequency_Hz).HasValue)
                        {
                            level.StepFrequency_Hz = readerLevels.GetValue(c => c.StepFrequency_Hz).Value;
                        }

                        if (readerLevels.GetValue(c => c.ReferenceLevels) != null)
                        {
                            var refLevels = BinaryDecoder.Deserialize<float[]>(readerLevels.GetValue(c => c.ReferenceLevels));
                            if (refLevels != null)
                            {
                                level.levels = refLevels as float[];
                            }
                        }
                    }
                    return true;
                });
            }
            if (isLoadAllData == false)
            {
                return ReferenceLevelsCut(level, StartFrequency_Hz, StopFrequency_Hz);
            }
            else
            {
                return level;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="measurementType"></param>
        /// <returns></returns>
        public MeasurementResults[] GetMeasResultsHeaderSpecial(MeasurementType  measurementType)
        {
            var results = new List<MeasurementResults>();
            try
            {
                this._logger.Info(Contexts.ThisComponent, Categories.Processing, Events.HandlerCallGetMeasResultsHeaderSpecialMethod.Text);
                var queryExecuter = this._dataLayer.Executor<SdrnServerDataContext>();
                var builderResMeas = this._dataLayer.GetBuilder<MD.IResMeas>().From();
                builderResMeas.Select(c => c.AntVal);
                builderResMeas.Select(c => c.DataRank);
                builderResMeas.Select(c => c.Id);
                builderResMeas.Select(c => c.MeasResultSID);
                builderResMeas.Select(c => c.MeasSubTaskId);
                builderResMeas.Select(c => c.MeasSubTaskStationId);
                builderResMeas.Select(c => c.MeasTaskId);
                builderResMeas.Select(c => c.N);
                builderResMeas.Select(c => c.ScansNumber);
                builderResMeas.Select(c => c.SensorId);
                builderResMeas.Select(c => c.StartTime);
                builderResMeas.Select(c => c.Status);
                builderResMeas.Select(c => c.StopTime);
                builderResMeas.Select(c => c.Synchronized);
                builderResMeas.Select(c => c.TimeMeas);
                builderResMeas.Select(c => c.TypeMeasurements);
                builderResMeas.OrderByAsc(c => c.Id);
                builderResMeas.Where(c => c.TypeMeasurements, ConditionOperator.Equal, measurementType.ToString());
                builderResMeas.Where(c => c.Status, ConditionOperator.NotEqual, Status.Z.ToString());
                queryExecuter.Fetch(builderResMeas, readerResMeas =>
                {
                    while (readerResMeas.Read())
                    {
                        var levelmeasurementResults = new MeasurementResults();
                        levelmeasurementResults.AntVal = readerResMeas.GetValue(c => c.AntVal);
                        levelmeasurementResults.DataRank = readerResMeas.GetValue(c => c.DataRank);
                        levelmeasurementResults.N = readerResMeas.GetValue(c => c.N);
                        levelmeasurementResults.Id = new MeasurementResultsIdentifier();
                        levelmeasurementResults.Id.MeasTaskId = new MeasTaskIdentifier();
                        if (readerResMeas.GetValue(c => c.MeasTaskId) != null)
                        {
                            int measTaskId = -1; int.TryParse(readerResMeas.GetValue(c => c.MeasTaskId), out measTaskId);
                            levelmeasurementResults.Id.MeasTaskId.Value = measTaskId;
                        }
                        levelmeasurementResults.Id.MeasSdrResultsId = readerResMeas.GetValue(c => c.Id);
                        levelmeasurementResults.StationMeasurements = new StationMeasurements();
                        levelmeasurementResults.StationMeasurements.StationId = new SensorIdentifier();
                        if (readerResMeas.GetValue(c => c.SensorId) != null)
                        {
                            levelmeasurementResults.StationMeasurements.StationId.Value = readerResMeas.GetValue(c => c.SensorId).Value;
                        }
                        levelmeasurementResults.Status = readerResMeas.GetValue(c => c.Status);
                        if (readerResMeas.GetValue(c => c.MeasSubTaskId) != null)
                        {
                            levelmeasurementResults.Id.SubMeasTaskId = readerResMeas.GetValue(c => c.MeasSubTaskId).Value;
                        }
                        if (readerResMeas.GetValue(c => c.ScansNumber) != null)
                        {
                            levelmeasurementResults.ScansNumber = readerResMeas.GetValue(c => c.ScansNumber).Value;
                        }

                        if (readerResMeas.GetValue(c => c.MeasSubTaskStationId) != null)
                        {
                            levelmeasurementResults.Id.SubMeasTaskStationId = readerResMeas.GetValue(c => c.MeasSubTaskStationId).Value;
                        }
                        if (readerResMeas.GetValue(c => c.TimeMeas) != null)
                        {
                            levelmeasurementResults.TimeMeas = readerResMeas.GetValue(c => c.TimeMeas).Value;
                        }
                        if (readerResMeas.GetValue(c => c.StartTime) != null)
                        {
                            levelmeasurementResults.StartTime = readerResMeas.GetValue(c => c.StartTime).Value;
                        }
                        if (readerResMeas.GetValue(c => c.StopTime) != null)
                        {
                            levelmeasurementResults.StopTime = readerResMeas.GetValue(c => c.StopTime).Value;
                        }
                        MeasurementType outResType;
                        if (Enum.TryParse<MeasurementType>(readerResMeas.GetValue(c => c.TypeMeasurements), out outResType))
                        {
                            levelmeasurementResults.TypeMeasurements = outResType;
                        }


                    /// Location
                    var listLocationSensorMeasurement = new List<LocationSensorMeasurement>();
                        var builderResLocSensorMeas = this._dataLayer.GetBuilder<MD.IResLocSensorMeas>().From();
                        builderResLocSensorMeas.Select(c => c.Agl);
                        builderResLocSensorMeas.Select(c => c.Asl);
                        builderResLocSensorMeas.Select(c => c.Id);
                        builderResLocSensorMeas.Select(c => c.Lat);
                        builderResLocSensorMeas.Select(c => c.Lon);
                        builderResLocSensorMeas.Select(c => c.ResMeasId);
                        builderResLocSensorMeas.OrderByAsc(c => c.Id);
                        builderResLocSensorMeas.Where(c => c.ResMeasId, ConditionOperator.Equal, readerResMeas.GetValue(c => c.Id));
                        var locationSensorMeasurement = new LocationSensorMeasurement();
                        queryExecuter.Fetch(builderResLocSensorMeas, readerResLocSensorMeas =>
                        {
                            while (readerResLocSensorMeas.Read())
                            {
                                var locSensorMeas = new LocationSensorMeasurement();
                                locSensorMeas.ASL = readerResLocSensorMeas.GetValue(c => c.Asl);
                                locSensorMeas.Lon = readerResLocSensorMeas.GetValue(c => c.Lon);
                                locSensorMeas.Lat = readerResLocSensorMeas.GetValue(c => c.Lat);
                                listLocationSensorMeasurement.Add(locSensorMeas);
                            }
                            return true;
                        });
                        levelmeasurementResults.LocationSensorMeasurement = listLocationSensorMeasurement.ToArray();

                        levelmeasurementResults.CountUnknownStationMeasurements = 0;
                        levelmeasurementResults.CountStationMeasurements = 0;
                        if (outResType == MeasurementType.MonitoringStations)
                        {

                            var builderResMeasStation = this._dataLayer.GetBuilder<MD.IResMeasStation>().From();
                            builderResMeasStation.Select(c => c.Id);
                            builderResMeasStation.Select(c => c.Status);
                            builderResMeasStation.Where(c => c.ResMeasId, ConditionOperator.Equal, readerResMeas.GetValue(c => c.Id));
                            builderResMeasStation.Where(c => c.Status, ConditionOperator.Equal, Status.E.ToString());
                            builderResMeasStation.OrderByAsc(c => c.Id);
                            queryExecuter.Fetch(builderResMeasStation, readerResMeasStation =>
                            {
                                while (readerResMeasStation.Read())
                                {
                                    levelmeasurementResults.CountUnknownStationMeasurements++;
                                }
                                return true;
                            });


                         
                            builderResMeasStation = this._dataLayer.GetBuilder<MD.IResMeasStation>().From();
                            builderResMeasStation.Select(c => c.Id);
                            builderResMeasStation.Select(c => c.Status);
                            builderResMeasStation.Where(c => c.ResMeasId, ConditionOperator.Equal, readerResMeas.GetValue(c => c.Id));
                            builderResMeasStation.Where(c => c.Status, ConditionOperator.NotEqual, Status.E.ToString());
                            builderResMeasStation.OrderByAsc(c => c.Id);
                            queryExecuter.Fetch(builderResMeasStation, readerResMeasStation =>
                            {
                                while (readerResMeasStation.Read())
                                {
                                    levelmeasurementResults.CountStationMeasurements++;
                                }
                                return true;
                            });



                            builderResMeasStation = this._dataLayer.GetBuilder<MD.IResMeasStation>().From();
                            builderResMeasStation.Select(c => c.Id);
                            builderResMeasStation.Select(c => c.Status);
                            builderResMeasStation.Where(c => c.ResMeasId, ConditionOperator.Equal, readerResMeas.GetValue(c => c.Id));
                            builderResMeasStation.Where(c => c.Status, ConditionOperator.IsNull);
                            builderResMeasStation.OrderByAsc(c => c.Id);
                            queryExecuter.Fetch(builderResMeasStation, readerResMeasStation =>
                            {
                                while (readerResMeasStation.Read())
                                {
                                    levelmeasurementResults.CountStationMeasurements++;
                                }
                                return true;
                            });
                        }

                        var builderLinkResSensoT = this._dataLayer.GetBuilder<MD.ILinkResSensor>().From();
                        builderLinkResSensoT.Select(c => c.Id);
                        builderLinkResSensoT.Select(c => c.SensorId);
                        builderLinkResSensoT.Select(c => c.SENSOR.Name);
                        builderLinkResSensoT.Select(c => c.SENSOR.TechId);
                        builderLinkResSensoT.Where(c => c.RESMEASSTA.RESMEAS.Id, ConditionOperator.Equal, readerResMeas.GetValue(c => c.Id));
                        builderLinkResSensoT.OrderByAsc(c => c.Id);
                        queryExecuter.Fetch(builderLinkResSensoT, readerLinkResSensor =>
                        {
                            while (readerLinkResSensor.Read())
                            {
                                if (readerLinkResSensor.GetValue(c => c.SENSOR.Name) != null)
                                {
                                    levelmeasurementResults.SensorName = readerLinkResSensor.GetValue(c => c.SENSOR.Name);
                                    levelmeasurementResults.SensorTechId = readerLinkResSensor.GetValue(c => c.SENSOR.TechId);
                                    break;
                                }
                            }
                            return true;
                        });

                        results.Add(levelmeasurementResults);
                    }
                    return true;

                });
            }
            catch (Exception e)
            {
                this._logger.Exception(Contexts.ThisComponent, e);
            }
            return results.ToArray();
        }

        public ShortMeasurementResults GetShortMeasResultsById(MeasurementResultsIdentifier measResultsId)
        {
            var levelmeasurementResults = new ShortMeasurementResults();
            try
            {
                if (measResultsId != null)
                {
                    this._logger.Info(Contexts.ThisComponent, Categories.Processing, Events.HandlerGetShortMeasResultsByIdMethod.Text);
                    var queryExecuter = this._dataLayer.Executor<SdrnServerDataContext>();
                    var builderResMeas = this._dataLayer.GetBuilder<MD.IResLocSensorMeas>().From();
                    builderResMeas.Select(c => c.RESMEAS.AntVal);
                    builderResMeas.Select(c => c.RESMEAS.DataRank);
                    builderResMeas.Select(c => c.RESMEAS.Id);
                    builderResMeas.Select(c => c.Lon);
                    builderResMeas.Select(c => c.Lat);
                    builderResMeas.Select(c => c.RESMEAS.MeasResultSID);
                    builderResMeas.Select(c => c.RESMEAS.MeasSubTaskId);
                    builderResMeas.Select(c => c.RESMEAS.MeasSubTaskStationId);
                    builderResMeas.Select(c => c.RESMEAS.MeasTaskId);
                    builderResMeas.Select(c => c.RESMEAS.N);
                    builderResMeas.Select(c => c.RESMEAS.ScansNumber);
                    builderResMeas.Select(c => c.RESMEAS.SensorId);
                    builderResMeas.Select(c => c.RESMEAS.StartTime);
                    builderResMeas.Select(c => c.RESMEAS.Status);
                    builderResMeas.Select(c => c.RESMEAS.StopTime);
                    builderResMeas.Select(c => c.RESMEAS.Synchronized);
                    builderResMeas.Select(c => c.RESMEAS.TimeMeas);
                    builderResMeas.Select(c => c.RESMEAS.TypeMeasurements);
                    builderResMeas.Select(c => c.RESMEAS.SENSOR.Name);
                    builderResMeas.Select(c => c.RESMEAS.SENSOR.TechId);
                    builderResMeas.OrderByAsc(c => c.Id);
                    if (measResultsId.MeasSdrResultsId > 0)
                    {
                        builderResMeas.Where(c => c.RESMEAS.Id, ConditionOperator.Equal, measResultsId.MeasSdrResultsId);
                    }
                    if ((measResultsId.MeasTaskId != null) && (measResultsId.MeasTaskId.Value > 0))
                    {
                        builderResMeas.Where(c => c.RESMEAS.MeasTaskId, ConditionOperator.Equal, measResultsId.MeasTaskId.Value.ToString());
                    }
                    if ((measResultsId.SubMeasTaskId > 0))
                    {
                        builderResMeas.Where(c => c.RESMEAS.MeasSubTaskId, ConditionOperator.Equal, measResultsId.SubMeasTaskId);
                    }
                    if ((measResultsId.SubMeasTaskStationId > 0))
                    {
                        builderResMeas.Where(c => c.RESMEAS.MeasSubTaskStationId, ConditionOperator.Equal, measResultsId.SubMeasTaskStationId);
                    }
                    builderResMeas.Where(c => c.RESMEAS.Status, ConditionOperator.NotEqual, Status.Z.ToString());

                    queryExecuter.Fetch(builderResMeas, readerResMeas =>
                    {
                        while (readerResMeas.Read())
                        {

                            levelmeasurementResults.DataRank = readerResMeas.GetValue(c => c.RESMEAS.DataRank);
                            levelmeasurementResults.Number = readerResMeas.GetValue(c => c.RESMEAS.N).HasValue ? readerResMeas.GetValue(c => c.RESMEAS.N).Value : -1;
                            levelmeasurementResults.Id = new MeasurementResultsIdentifier();
                            levelmeasurementResults.Id.MeasTaskId = new MeasTaskIdentifier();
                            if (readerResMeas.GetValue(c => c.RESMEAS.MeasTaskId) != null)
                            {
                                int measTaskId = -1; int.TryParse(readerResMeas.GetValue(c => c.RESMEAS.MeasTaskId), out measTaskId);
                                levelmeasurementResults.Id.MeasTaskId.Value = measTaskId;
                            }
                            levelmeasurementResults.Id.MeasSdrResultsId = readerResMeas.GetValue(c => c.RESMEAS.Id);


                            levelmeasurementResults.Status = readerResMeas.GetValue(c => c.RESMEAS.Status);
                            if (readerResMeas.GetValue(c => c.RESMEAS.MeasSubTaskId) != null)
                            {
                                levelmeasurementResults.Id.SubMeasTaskId = readerResMeas.GetValue(c => c.RESMEAS.MeasSubTaskId).Value;
                            }
                            if (readerResMeas.GetValue(c => c.RESMEAS.MeasSubTaskStationId) != null)
                            {
                                levelmeasurementResults.Id.SubMeasTaskStationId = readerResMeas.GetValue(c => c.RESMEAS.MeasSubTaskStationId).Value;
                            }
                            if (readerResMeas.GetValue(c => c.RESMEAS.TimeMeas) != null)
                            {
                                levelmeasurementResults.TimeMeas = readerResMeas.GetValue(c => c.RESMEAS.TimeMeas).Value;
                            }
                            if (readerResMeas.GetValue(c => c.RESMEAS.StartTime) != null)
                            {
                                levelmeasurementResults.StartTime = readerResMeas.GetValue(c => c.RESMEAS.StartTime).Value;
                            }
                            if (readerResMeas.GetValue(c => c.RESMEAS.StopTime) != null)
                            {
                                levelmeasurementResults.StopTime = readerResMeas.GetValue(c => c.RESMEAS.StopTime).Value;
                            }
                            MeasurementType outResType;
                            if (Enum.TryParse<MeasurementType>(readerResMeas.GetValue(c => c.RESMEAS.TypeMeasurements), out outResType))
                            {
                                levelmeasurementResults.TypeMeasurements = outResType;
                            }
                            levelmeasurementResults.SensorName = readerResMeas.GetValue(c => c.RESMEAS.SENSOR.Name);
                            levelmeasurementResults.SensorTechId = readerResMeas.GetValue(c => c.RESMEAS.SENSOR.TechId);


                            levelmeasurementResults.CurrentLon = readerResMeas.GetValue(c => c.Lon);
                            levelmeasurementResults.CurrentLat = readerResMeas.GetValue(c => c.Lat);


                            levelmeasurementResults.CountUnknownStationMeasurements = 0;
                            levelmeasurementResults.CountStationMeasurements = 0;
                            if (outResType == MeasurementType.MonitoringStations)
                            {
                                var builderResMeasStation = this._dataLayer.GetBuilder<MD.IResMeasStation>().From();
                                builderResMeasStation.Select(c => c.Id);
                                builderResMeasStation.Select(c => c.Status);
                                builderResMeasStation.Where(c => c.ResMeasId, ConditionOperator.Equal, readerResMeas.GetValue(c => c.RESMEAS.Id));
                                builderResMeasStation.Where(c => c.Status, ConditionOperator.Equal, Status.E.ToString());
                                builderResMeasStation.OrderByAsc(c => c.Id);
                                queryExecuter.Fetch(builderResMeasStation, readerResMeasStation =>
                                {
                                    while (readerResMeasStation.Read())
                                    {
                                        levelmeasurementResults.CountUnknownStationMeasurements++;
                                    }
                                    return true;
                                });



                                builderResMeasStation = this._dataLayer.GetBuilder<MD.IResMeasStation>().From();
                                builderResMeasStation.Select(c => c.Id);
                                builderResMeasStation.Select(c => c.Status);
                                builderResMeasStation.Where(c => c.ResMeasId, ConditionOperator.Equal, readerResMeas.GetValue(c => c.RESMEAS.Id));
                                builderResMeasStation.Where(c => c.Status, ConditionOperator.NotEqual, Status.E.ToString());
                                builderResMeasStation.OrderByAsc(c => c.Id);
                                queryExecuter.Fetch(builderResMeasStation, readerResMeasStation =>
                                {
                                    while (readerResMeasStation.Read())
                                    {
                                        levelmeasurementResults.CountStationMeasurements++;
                                    }
                                    return true;
                                });


                                builderResMeasStation = this._dataLayer.GetBuilder<MD.IResMeasStation>().From();
                                builderResMeasStation.Select(c => c.Id);
                                builderResMeasStation.Select(c => c.Status);
                                builderResMeasStation.Where(c => c.ResMeasId, ConditionOperator.Equal, readerResMeas.GetValue(c => c.RESMEAS.Id));
                                builderResMeasStation.Where(c => c.Status, ConditionOperator.IsNull);
                                builderResMeasStation.OrderByAsc(c => c.Id);
                                queryExecuter.Fetch(builderResMeasStation, readerResMeasStation =>
                                {
                                    while (readerResMeasStation.Read())
                                    {
                                        levelmeasurementResults.CountStationMeasurements++;
                                    }
                                    return true;
                                });
                            }

                        }
                        return true;

                    });
                }
            }
            catch (Exception e)
            {
                this._logger.Exception(Contexts.ThisComponent, e);
            }
            return levelmeasurementResults;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="measurementType"></param>
        /// <returns></returns>
        public ShortMeasurementResults[] GetShortMeasResultsByDate(GetShortMeasResultsByDateValue constraint)
        {
            var results = new List<ShortMeasurementResults>();
            try
            {
                this._logger.Info(Contexts.ThisComponent, Categories.Processing, Events.HandlerGetShortMeasResultsByDateMethod.Text);
                var queryExecuter = this._dataLayer.Executor<SdrnServerDataContext>();
                var builderResMeas = this._dataLayer.GetBuilder<MD.IResLocSensorMeas>().From();
                builderResMeas.Select(c => c.RESMEAS.AntVal);
                builderResMeas.Select(c => c.RESMEAS.DataRank);
                builderResMeas.Select(c => c.RESMEAS.Id);
                builderResMeas.Select(c => c.Lon);
                builderResMeas.Select(c => c.Lat);
                builderResMeas.Select(c => c.RESMEAS.MeasResultSID);
                builderResMeas.Select(c => c.RESMEAS.MeasSubTaskId);
                builderResMeas.Select(c => c.RESMEAS.MeasSubTaskStationId);
                builderResMeas.Select(c => c.RESMEAS.MeasTaskId);
                builderResMeas.Select(c => c.RESMEAS.N);
                builderResMeas.Select(c => c.RESMEAS.ScansNumber);
                builderResMeas.Select(c => c.RESMEAS.SensorId);
                builderResMeas.Select(c => c.RESMEAS.StartTime);
                builderResMeas.Select(c => c.RESMEAS.Status);
                builderResMeas.Select(c => c.RESMEAS.StopTime);
                builderResMeas.Select(c => c.RESMEAS.Synchronized);
                builderResMeas.Select(c => c.RESMEAS.TimeMeas);
                builderResMeas.Select(c => c.RESMEAS.TypeMeasurements);
                builderResMeas.Select(c => c.RESMEAS.SENSOR.Name);
                builderResMeas.Select(c => c.RESMEAS.SENSOR.TechId);
                builderResMeas.OrderByAsc(c => c.Id);
                builderResMeas.Where(c => c.RESMEAS.TimeMeas, ConditionOperator.GreaterEqual, constraint.Start);
                builderResMeas.Where(c => c.RESMEAS.TimeMeas, ConditionOperator.LessEqual, constraint.End);
                builderResMeas.Where(c => c.RESMEAS.Status, ConditionOperator.NotEqual, Status.Z.ToString());
                queryExecuter.Fetch(builderResMeas, readerResMeas =>
                {
                    while (readerResMeas.Read())
                    {
                        var levelmeasurementResults = new ShortMeasurementResults();
                        levelmeasurementResults.DataRank = readerResMeas.GetValue(c => c.RESMEAS.DataRank);
                        levelmeasurementResults.Number = readerResMeas.GetValue(c => c.RESMEAS.N).HasValue ? readerResMeas.GetValue(c => c.RESMEAS.N).Value : -1;
                        levelmeasurementResults.Id = new MeasurementResultsIdentifier();
                        levelmeasurementResults.Id.MeasTaskId = new MeasTaskIdentifier();
                        if (readerResMeas.GetValue(c => c.RESMEAS.MeasTaskId) != null)
                        {
                            int measTaskId = -1; int.TryParse(readerResMeas.GetValue(c => c.RESMEAS.MeasTaskId), out measTaskId);
                            levelmeasurementResults.Id.MeasTaskId.Value = measTaskId;
                        }
                        levelmeasurementResults.Id.MeasSdrResultsId = readerResMeas.GetValue(c => c.RESMEAS.Id);


                        levelmeasurementResults.Status = readerResMeas.GetValue(c => c.RESMEAS.Status);
                        if (readerResMeas.GetValue(c => c.RESMEAS.MeasSubTaskId) != null)
                        {
                            levelmeasurementResults.Id.SubMeasTaskId = readerResMeas.GetValue(c => c.RESMEAS.MeasSubTaskId).Value;
                        }
                        if (readerResMeas.GetValue(c => c.RESMEAS.MeasSubTaskStationId) != null)
                        {
                            levelmeasurementResults.Id.SubMeasTaskStationId = readerResMeas.GetValue(c => c.RESMEAS.MeasSubTaskStationId).Value;
                        }
                        if (readerResMeas.GetValue(c => c.RESMEAS.TimeMeas) != null)
                        {
                            levelmeasurementResults.TimeMeas = readerResMeas.GetValue(c => c.RESMEAS.TimeMeas).Value;
                        }
                        if (readerResMeas.GetValue(c => c.RESMEAS.StartTime) != null)
                        {
                            levelmeasurementResults.StartTime = readerResMeas.GetValue(c => c.RESMEAS.StartTime).Value;
                        }
                        if (readerResMeas.GetValue(c => c.RESMEAS.StopTime) != null)
                        {
                            levelmeasurementResults.StopTime = readerResMeas.GetValue(c => c.RESMEAS.StopTime).Value;
                        }
                        MeasurementType outResType;
                        if (Enum.TryParse<MeasurementType>(readerResMeas.GetValue(c => c.RESMEAS.TypeMeasurements), out outResType))
                        {
                            levelmeasurementResults.TypeMeasurements = outResType;
                        }
                        levelmeasurementResults.SensorName = readerResMeas.GetValue(c => c.RESMEAS.SENSOR.Name);
                        levelmeasurementResults.SensorTechId = readerResMeas.GetValue(c => c.RESMEAS.SENSOR.TechId);


                        levelmeasurementResults.CurrentLon = readerResMeas.GetValue(c => c.Lon);
                        levelmeasurementResults.CurrentLat = readerResMeas.GetValue(c => c.Lat);


                        levelmeasurementResults.CountUnknownStationMeasurements = 0;
                        levelmeasurementResults.CountStationMeasurements = 0;
                        if (outResType == MeasurementType.MonitoringStations)
                        {
                            var builderResMeasStation = this._dataLayer.GetBuilder<MD.IResMeasStation>().From();
                            builderResMeasStation.Select(c => c.Id);
                            builderResMeasStation.Select(c => c.Status);
                            builderResMeasStation.Where(c => c.ResMeasId, ConditionOperator.Equal, readerResMeas.GetValue(c => c.RESMEAS.Id));
                            builderResMeasStation.Where(c => c.Status, ConditionOperator.Equal, Status.E.ToString());
                            builderResMeasStation.OrderByAsc(c => c.Id);
                            queryExecuter.Fetch(builderResMeasStation, readerResMeasStation =>
                            {
                                while (readerResMeasStation.Read())
                                {
                                    levelmeasurementResults.CountUnknownStationMeasurements++;
                                }
                                return true;
                            });



                            builderResMeasStation = this._dataLayer.GetBuilder<MD.IResMeasStation>().From();
                            builderResMeasStation.Select(c => c.Id);
                            builderResMeasStation.Select(c => c.Status);
                            builderResMeasStation.Where(c => c.ResMeasId, ConditionOperator.Equal, readerResMeas.GetValue(c => c.RESMEAS.Id));
                            builderResMeasStation.Where(c => c.Status, ConditionOperator.NotEqual, Status.E.ToString());
                            builderResMeasStation.OrderByAsc(c => c.Id);
                            queryExecuter.Fetch(builderResMeasStation, readerResMeasStation =>
                            {
                                while (readerResMeasStation.Read())
                                {
                                    levelmeasurementResults.CountStationMeasurements++;
                                }
                                return true;
                            });


                            builderResMeasStation = this._dataLayer.GetBuilder<MD.IResMeasStation>().From();
                            builderResMeasStation.Select(c => c.Id);
                            builderResMeasStation.Select(c => c.Status);
                            builderResMeasStation.Where(c => c.ResMeasId, ConditionOperator.Equal, readerResMeas.GetValue(c => c.RESMEAS.Id));
                            builderResMeasStation.Where(c => c.Status, ConditionOperator.IsNull);
                            builderResMeasStation.OrderByAsc(c => c.Id);
                            queryExecuter.Fetch(builderResMeasStation, readerResMeasStation =>
                            {
                                while (readerResMeasStation.Read())
                                {
                                    levelmeasurementResults.CountStationMeasurements++;
                                }
                                return true;
                            });
                        }

                        if ((results.Find(x => x.Id.MeasSdrResultsId == levelmeasurementResults.Id.MeasSdrResultsId)) == null)
                        {
                            results.Add(levelmeasurementResults);
                        }
                    }
                    return true;

                });
            }
            catch (Exception e)
            {
                this._logger.Exception(Contexts.ThisComponent, e);
            }
            return results.ToArray();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="measurementType"></param>
        /// <returns></returns>
        public SensorPoligonPoint[] GetSensorPoligonPoint(int MeasResultsId)
        {
            var results = new List<SensorPoligonPoint>();
            try
            {

                var listSensorIds = new List<int?>();
                this._logger.Info(Contexts.ThisComponent, Categories.Processing, Events.HandlerCallGetSensorPoligonPointMethod.Text);
                var queryExecuter = this._dataLayer.Executor<SdrnServerDataContext>();
                var builderLinkResSensoT = this._dataLayer.GetBuilder<MD.ILinkResSensor>().From();
                builderLinkResSensoT.Select(c => c.Id);
                builderLinkResSensoT.Select(c => c.SensorId);
                builderLinkResSensoT.Select(c => c.SENSOR.Name);
                builderLinkResSensoT.Select(c => c.SENSOR.TechId);
                builderLinkResSensoT.Where(c => c.RESMEASSTA.RESMEAS.Id, ConditionOperator.Equal, MeasResultsId);
                builderLinkResSensoT.OrderByAsc(c => c.Id);
                queryExecuter.Fetch(builderLinkResSensoT, readerLinkResSensor =>
                {
                    while (readerLinkResSensor.Read())
                    {
                        int? sensorId = readerLinkResSensor.GetValue(c => c.SensorId);
                        if (sensorId != null)
                        {
                            if (!listSensorIds.Contains(sensorId))
                            {
                                listSensorIds.Add(sensorId);
                            }
                        }
                    }
                    return true;
                });

                if ((listSensorIds != null) && (listSensorIds.Count > 0))
                {
                    var listIntEmittingSensorPolygon = BreakDownElemBlocks.BreakDown(listSensorIds.ToArray());
                    for (int i=0; i< listIntEmittingSensorPolygon.Count; i++)
                    {
                        var builderSensorPolygon = this._dataLayer.GetBuilder<MD.ISensorPolygon>().From();
                        builderSensorPolygon.Select(c => c.Id);
                        builderSensorPolygon.Select(c => c.Lon);
                        builderSensorPolygon.Select(c => c.Lat);
                        builderSensorPolygon.Where(c => c.SensorId, ConditionOperator.In, listIntEmittingSensorPolygon[i]);
                        builderSensorPolygon.OrderByAsc(c => c.Id);
                        queryExecuter.Fetch(builderSensorPolygon, readerSensorPolygon =>
                        {
                            while (readerSensorPolygon.Read())
                            {
                                var sensorPoligonPoint = new SensorPoligonPoint();
                                sensorPoligonPoint.Lon = readerSensorPolygon.GetValue(c => c.Lon);
                                sensorPoligonPoint.Lat = readerSensorPolygon.GetValue(c => c.Lat);
                                results.Add(sensorPoligonPoint);
                            }
                            return true;
                        });
                    }
                }
            }
            catch (Exception e)
            {
                this._logger.Exception(Contexts.ThisComponent, e);
            }
            return results.ToArray();
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="measurementType"></param>
        /// <returns></returns>
        public ResultsMeasurementsStation ReadResultResMeasStation(int StationId)
        {
            var resMeasStatiion = new ResultsMeasurementsStation();
            try
            {
                this._logger.Info(Contexts.ThisComponent, Categories.Processing, Events.HandlerCallReadResultResMeasStationMethod.Text);
                var queryExecuter = this._dataLayer.Executor<SdrnServerDataContext>();
                var builderResMeasStation = this._dataLayer.GetBuilder<MD.IResMeasStation>().From();
                builderResMeasStation.Select(c => c.GlobalSID);
                builderResMeasStation.Select(c => c.Id);
                builderResMeasStation.Select(c => c.IdStation);
                builderResMeasStation.Select(c => c.MeasGlobalSID);
                builderResMeasStation.Select(c => c.ResMeasId);
                builderResMeasStation.Select(c => c.SectorId);
                builderResMeasStation.Select(c => c.Standard);
                builderResMeasStation.Select(c => c.StationId);
                builderResMeasStation.Select(c => c.Status);
                builderResMeasStation.Where(c => c.Id, ConditionOperator.Equal, StationId);
                builderResMeasStation.OrderByAsc(c => c.Id);
                queryExecuter.Fetch(builderResMeasStation, readerResMeasStation =>
                {
                    while (readerResMeasStation.Read())
                    {
                        resMeasStatiion.IdSector = readerResMeasStation.GetValue(c => c.SectorId);
                        if (readerResMeasStation.GetValue(c => c.StationId) != null)
                        {
                            resMeasStatiion.Idstation = readerResMeasStation.GetValue(c => c.StationId).Value.ToString();
                        }
                        resMeasStatiion.GlobalSID = readerResMeasStation.GetValue(c => c.GlobalSID);
                        resMeasStatiion.MeasGlobalSID = readerResMeasStation.GetValue(c => c.MeasGlobalSID);
                        resMeasStatiion.Status = readerResMeasStation.GetValue(c => c.Status);
                        resMeasStatiion.Id = readerResMeasStation.GetValue(c => c.Id);
                        resMeasStatiion.IdSector = readerResMeasStation.GetValue(c => c.SectorId);
                        resMeasStatiion.Idstation = readerResMeasStation.GetValue(c => c.StationId).HasValue ? readerResMeasStation.GetValue(c => c.StationId).Value.ToString() : "";
                        resMeasStatiion.Standard = readerResMeasStation.GetValue(c => c.Standard);



                        double? rbw = null;
                        double? vbw = null;



                        var measurementsParameterGeneral = new MeasurementsParameterGeneral();
                        var builderResStGeneral = this._dataLayer.GetBuilder<MD.IResStGeneral>().From();
                        builderResStGeneral.Select(c => c.CentralFrequency);
                        builderResStGeneral.Select(c => c.CentralFrequencyMeas);
                        builderResStGeneral.Select(c => c.Correctnessestim);
                        builderResStGeneral.Select(c => c.DurationMeas);
                        builderResStGeneral.Select(c => c.Id);
                        builderResStGeneral.Select(c => c.MarkerIndex);
                        builderResStGeneral.Select(c => c.OffsetFrequency);
                        builderResStGeneral.Select(c => c.ResMeasStaId);
                        builderResStGeneral.Select(c => c.SpecrumStartFreq);
                        builderResStGeneral.Select(c => c.SpecrumSteps);
                        builderResStGeneral.Select(c => c.T1);
                        builderResStGeneral.Select(c => c.T2);
                        builderResStGeneral.Select(c => c.BW);
                        builderResStGeneral.Select(c => c.TimeFinishMeas);
                        builderResStGeneral.Select(c => c.TimeStartMeas);
                        builderResStGeneral.Select(c => c.TraceCount);
                        builderResStGeneral.Select(c => c.Rbw);
                        builderResStGeneral.Select(c => c.Vbw);
                        builderResStGeneral.Where(c => c.ResMeasStaId, ConditionOperator.Equal, readerResMeasStation.GetValue(c => c.Id));
                        builderResStGeneral.OrderByAsc(c => c.Id);
                        queryExecuter.Fetch(builderResStGeneral, readerResStGeneral =>
                        {
                            while (readerResStGeneral.Read())
                            {
                                measurementsParameterGeneral.CentralFrequency = readerResStGeneral.GetValue(c => c.CentralFrequency);
                                measurementsParameterGeneral.CentralFrequencyMeas = readerResStGeneral.GetValue(c => c.CentralFrequencyMeas);
                                measurementsParameterGeneral.DurationMeas = readerResStGeneral.GetValue(c => c.DurationMeas);
                                measurementsParameterGeneral.MarkerIndex = readerResStGeneral.GetValue(c => c.MarkerIndex);
                                measurementsParameterGeneral.OffsetFrequency = readerResStGeneral.GetValue(c => c.OffsetFrequency);
                                measurementsParameterGeneral.SpecrumStartFreq = (decimal?)readerResStGeneral.GetValue(c => c.SpecrumStartFreq);
                                measurementsParameterGeneral.SpecrumSteps = (decimal?)readerResStGeneral.GetValue(c => c.SpecrumSteps);
                                measurementsParameterGeneral.T1 = readerResStGeneral.GetValue(c => c.T1);
                                measurementsParameterGeneral.T2 = readerResStGeneral.GetValue(c => c.T2);
                                measurementsParameterGeneral.TimeFinishMeas = readerResStGeneral.GetValue(c => c.TimeFinishMeas);
                                measurementsParameterGeneral.TimeStartMeas = readerResStGeneral.GetValue(c => c.TimeStartMeas);
                                rbw = readerResStGeneral.GetValue(c => c.Rbw);
                                vbw = readerResStGeneral.GetValue(c => c.Vbw);



                                var listMaskElements = new List<MaskElements>();
                                var builderResStMaskElement = this._dataLayer.GetBuilder<MD.IResStMaskElement>().From();
                                builderResStMaskElement.Select(c => c.Bw);
                                builderResStMaskElement.Select(c => c.Level);
                                builderResStMaskElement.Where(c => c.ResStGeneralId, ConditionOperator.Equal, readerResStGeneral.GetValue(c => c.Id));
                                builderResStMaskElement.OrderByAsc(c => c.Id);
                                queryExecuter.Fetch(builderResStMaskElement, readerResStMaskElement =>
                                {

                                    while (readerResStMaskElement.Read())
                                    {
                                        var maskElements = new MaskElements();
                                        maskElements.BW = readerResStMaskElement.GetValue(c => c.Bw);
                                        maskElements.level = readerResStMaskElement.GetValue(c => c.Level);
                                        listMaskElements.Add(maskElements);
                                    }
                                    return true;

                                });
                                measurementsParameterGeneral.MaskBW = listMaskElements.ToArray();



                                var levelSpectrum = new List<float>();
                                var builderResStLevelsSpect = this._dataLayer.GetBuilder<MD.IResStLevelsSpect>().From();
                                builderResStLevelsSpect.Select(c => c.LevelSpecrum);
                                builderResStLevelsSpect.Select(c => c.Id);
                                builderResStLevelsSpect.Where(c => c.ResStGeneralId, ConditionOperator.Equal, readerResStGeneral.GetValue(c => c.Id));
                                builderResStLevelsSpect.OrderByAsc(c => c.Id);
                                queryExecuter.Fetch(builderResStLevelsSpect, readerResStLevelsSpect =>
                                {
                                    while (readerResStLevelsSpect.Read())
                                    {
                                        if (readerResStLevelsSpect.GetValue(c => c.LevelSpecrum) != null)
                                        {
                                            if (readerResStLevelsSpect.GetValue(c => c.LevelSpecrum).HasValue)
                                            {
                                                levelSpectrum.Add(readerResStLevelsSpect.GetValue(c => c.LevelSpecrum).Value);
                                            }
                                        }
                                    }
                                    return true;
                                });
                                measurementsParameterGeneral.LevelsSpecrum = levelSpectrum.ToArray();


                                var valSysInfo = "";


                                var queryStationSysInfo = this._dataLayer.GetBuilder<MD.IResSysInfo>()
                               .From()
                               .Select(c => c.Id, c => c.Agl, c => c.Asl, c => c.Bandwidth, c => c.BaseId, c => c.Bsic, c => c.ChannelNumber, c => c.Cid, c => c.Code, c => c.Ctoi, c => c.Eci, c => c.Enodebid, c => c.Freq, c => c.Icio, c => c.InbandPower, c => c.Iscp, c => c.Lac)
                               .Select(c => c.Lat, c => c.Lon, c => c.Mcc, c => c.Mnc, c => c.Nid, c => c.Pci, c => c.Pn, c => c.Power, c => c.Ptotal, c => c.Rnc, c => c.Rscp, c => c.Rsrp, c => c.Rsrq, c => c.Sc, c => c.Sid, c => c.Tac, c => c.TypeCdmaevdo, c => c.Ucid)
                               .Where(c => c.ResStGeneralId, ConditionOperator.Equal, readerResStGeneral.GetValue(c => c.Id));
                                queryExecuter.Fetch(queryStationSysInfo, readerStationSysInfo =>
                                {
                                    while (readerStationSysInfo.Read())
                                    {

                                        valSysInfo += string.Format("AGL : {0}", readerStationSysInfo.GetValue(c => c.Agl)) + Environment.NewLine;
                                        valSysInfo += string.Format("ASL : {0}", readerStationSysInfo.GetValue(c => c.Asl)) + Environment.NewLine;
                                        valSysInfo += string.Format("BandWidth : {0}", readerStationSysInfo.GetValue(c => c.Bandwidth)) + Environment.NewLine;
                                        valSysInfo += string.Format("BaseID : {0}", readerStationSysInfo.GetValue(c => c.BaseId)) + Environment.NewLine;
                                        valSysInfo += string.Format("BSIC : {0}", readerStationSysInfo.GetValue(c => c.Bsic)) + Environment.NewLine;
                                        valSysInfo += string.Format("ChannelNumber : {0}", readerStationSysInfo.GetValue(c => c.ChannelNumber)) + Environment.NewLine;
                                        valSysInfo += string.Format("CID : {0}", readerStationSysInfo.GetValue(c => c.Cid)) + Environment.NewLine;
                                        valSysInfo += string.Format("Code : {0}", readerStationSysInfo.GetValue(c => c.Code)) + Environment.NewLine;
                                        valSysInfo += string.Format("CtoI : {0}", readerStationSysInfo.GetValue(c => c.Ctoi)) + Environment.NewLine;
                                        valSysInfo += string.Format("ECI : {0}", readerStationSysInfo.GetValue(c => c.Eci)) + Environment.NewLine;
                                        valSysInfo += string.Format("eNodeBId : {0}", readerStationSysInfo.GetValue(c => c.Enodebid)) + Environment.NewLine;
                                        valSysInfo += string.Format("Freq : {0}", readerStationSysInfo.GetValue(c => c.Freq)) + Environment.NewLine;
                                        valSysInfo += string.Format("IcIo : {0}", readerStationSysInfo.GetValue(c => c.Icio)) + Environment.NewLine;
                                        valSysInfo += string.Format("INBAND_POWER : {0}", readerStationSysInfo.GetValue(c => c.InbandPower)) + Environment.NewLine;
                                        valSysInfo += string.Format("ISCP : {0}", readerStationSysInfo.GetValue(c => c.Iscp)) + Environment.NewLine;
                                        valSysInfo += string.Format("LAC : {0}", readerStationSysInfo.GetValue(c => c.Lac)) + Environment.NewLine;
                                        valSysInfo += string.Format("Lat : {0}", readerStationSysInfo.GetValue(c => c.Lat)) + Environment.NewLine;
                                        valSysInfo += string.Format("Lon : {0}", readerStationSysInfo.GetValue(c => c.Lon)) + Environment.NewLine;
                                        valSysInfo += string.Format("MCC : {0}", readerStationSysInfo.GetValue(c => c.Mcc)) + Environment.NewLine;
                                        valSysInfo += string.Format("MNC : {0}", readerStationSysInfo.GetValue(c => c.Mnc)) + Environment.NewLine;
                                        valSysInfo += string.Format("NID : {0}", readerStationSysInfo.GetValue(c => c.Nid)) + Environment.NewLine;
                                        valSysInfo += string.Format("PCI : {0}", readerStationSysInfo.GetValue(c => c.Pci)) + Environment.NewLine;
                                        valSysInfo += string.Format("PN : {0}", readerStationSysInfo.GetValue(c => c.Pn)) + Environment.NewLine;
                                        valSysInfo += string.Format("Power : {0}", readerStationSysInfo.GetValue(c => c.Power)) + Environment.NewLine;
                                        valSysInfo += string.Format("Ptotal : {0}", readerStationSysInfo.GetValue(c => c.Ptotal)) + Environment.NewLine;
                                        valSysInfo += string.Format("RNC : {0}", readerStationSysInfo.GetValue(c => c.Rnc)) + Environment.NewLine;
                                        valSysInfo += string.Format("RSCP : {0}", readerStationSysInfo.GetValue(c => c.Rscp)) + Environment.NewLine;
                                        valSysInfo += string.Format("RSRP : {0}", readerStationSysInfo.GetValue(c => c.Rsrp)) + Environment.NewLine;
                                        valSysInfo += string.Format("RSRQ : {0}", readerStationSysInfo.GetValue(c => c.Rsrq)) + Environment.NewLine;
                                        valSysInfo += string.Format("SC : {0}", readerStationSysInfo.GetValue(c => c.Sc)) + Environment.NewLine;
                                        valSysInfo += string.Format("SID : {0}", readerStationSysInfo.GetValue(c => c.Sid)) + Environment.NewLine;
                                        valSysInfo += string.Format("TAC : {0}", readerStationSysInfo.GetValue(c => c.Tac)) + Environment.NewLine;
                                        valSysInfo += string.Format("TypeCDMAEVDO : {0}", readerStationSysInfo.GetValue(c => c.TypeCdmaevdo)) + Environment.NewLine;
                                        valSysInfo += string.Format("UCID : {0}", readerStationSysInfo.GetValue(c => c.Ucid)) + Environment.NewLine;


                                        var queryStationSysInfoBls = this._dataLayer.GetBuilder<MD.IResSysInfoBlocks>()
                                        .From()
                                        .Select(c => c.Id, c => c.BinData, c => c.Type)
                                        .Where(c => c.ResSysInfoId, ConditionOperator.Equal, readerStationSysInfo.GetValue(c => c.Id));
                                        queryExecuter.Fetch(queryStationSysInfoBls, readerStationSysInfoBls =>
                                        {
                                            while (readerStationSysInfoBls.Read())
                                            {
                                                valSysInfo += string.Format("SysInfoBlocks.Data  : {0}", BinaryDecoder.Deserialize<string>(readerStationSysInfoBls.GetValue(c => c.BinData))) + Environment.NewLine;
                                                valSysInfo += string.Format("SysInfoBlocks.Type  : {0}", readerStationSysInfoBls.GetValue(c => c.Type)) + Environment.NewLine;
                                            }
                                            return true;
                                        });

                                        resMeasStatiion.StationSysInfo = valSysInfo;

                                        break;
                                    }
                                    return true;
                                });

                            }
                            return true;
                        });
                        resMeasStatiion.GeneralResult = measurementsParameterGeneral;



                        var listLevelMeasurementsCar = new List<LevelMeasurementsCar>();
                        var builderResStLevelCar = this._dataLayer.GetBuilder<MD.IResStLevelCar>().From();
                        builderResStLevelCar.Select(c => c.Agl);
                        builderResStLevelCar.Select(c => c.Altitude);
                        builderResStLevelCar.Select(c => c.Bw);
                        builderResStLevelCar.Select(c => c.CentralFrequency);
                        builderResStLevelCar.Select(c => c.DifferenceTimeStamp);
                        builderResStLevelCar.Select(c => c.Id);
                        builderResStLevelCar.Select(c => c.Lat);
                        builderResStLevelCar.Select(c => c.LevelDbm);
                        builderResStLevelCar.Select(c => c.LevelDbmkvm);
                        builderResStLevelCar.Select(c => c.Lon);
                        builderResStLevelCar.Select(c => c.ResStationId);
                        builderResStLevelCar.Select(c => c.TimeOfMeasurements);
                        builderResStLevelCar.Where(c => c.ResStationId, ConditionOperator.Equal, readerResMeasStation.GetValue(c => c.Id));
                        builderResStLevelCar.OrderByAsc(c => c.Id);
                        queryExecuter.Fetch(builderResStLevelCar, readerResStLevelCar =>
                        {
                            while (readerResStLevelCar.Read())
                            {
                                var levelMeasurementsCar = new LevelMeasurementsCar();
                                levelMeasurementsCar.Altitude = readerResStLevelCar.GetValue(c => c.Altitude);
                                levelMeasurementsCar.BW = readerResStLevelCar.GetValue(c => c.Bw);
                                levelMeasurementsCar.CentralFrequency = (decimal?)readerResStLevelCar.GetValue(c => c.CentralFrequency);
                                levelMeasurementsCar.DifferenceTimestamp = readerResStLevelCar.GetValue(c => c.DifferenceTimeStamp);
                                levelMeasurementsCar.Lat = readerResStLevelCar.GetValue(c => c.Lat);
                                levelMeasurementsCar.LeveldBm = readerResStLevelCar.GetValue(c => c.LevelDbm);
                                levelMeasurementsCar.LeveldBmkVm = readerResStLevelCar.GetValue(c => c.LevelDbmkvm);
                                levelMeasurementsCar.Lon = readerResStLevelCar.GetValue(c => c.Lon);
                                levelMeasurementsCar.RBW = rbw;
                                levelMeasurementsCar.VBW = vbw;



                                if (readerResStLevelCar.GetValue(c => c.TimeOfMeasurements) != null)
                                {
                                    levelMeasurementsCar.TimeOfMeasurements = readerResStLevelCar.GetValue(c => c.TimeOfMeasurements).Value;
                                }
                                listLevelMeasurementsCar.Add(levelMeasurementsCar);
                            }
                            return true;
                        });
                        resMeasStatiion.LevelMeasurements = listLevelMeasurementsCar.ToArray();

                    }
                    return true;
                });
            }
            catch (Exception e)
            {
                this._logger.Exception(Contexts.ThisComponent, e);
            }
            return resMeasStatiion;
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="measurementType"></param>
        /// <returns></returns>
        public Route[] GetRoutes(int ResId)
        {
            var routes = new List<Route>();
            try
            {
                this._logger.Info(Contexts.ThisComponent, Categories.Processing, Events.HandlerCallGetRoutesMethod.Text);
                List<RoutePoint> points = new List<RoutePoint>();
                var queryExecuter = this._dataLayer.Executor<SdrnServerDataContext>();
                var builderResRoutes = this._dataLayer.GetBuilder<MD.IResRoutes>().From();
                builderResRoutes.Select(c => c.Agl);
                builderResRoutes.Select(c => c.Asl);
                builderResRoutes.Select(c => c.FinishTime);
                builderResRoutes.Select(c => c.Id);
                builderResRoutes.Select(c => c.Lat);
                builderResRoutes.Select(c => c.Lon);
                builderResRoutes.Select(c => c.PointStayType);
                builderResRoutes.Select(c => c.ResMeasId);
                builderResRoutes.Select(c => c.RouteId);
                builderResRoutes.Select(c => c.StartTime);
                builderResRoutes.Where(c => c.ResMeasId, ConditionOperator.Equal, ResId);
                queryExecuter.Fetch(builderResRoutes, readerResRoutes =>
                {
                    var route = new Route();
                    while (readerResRoutes.Read())
                    {
                        route.RouteId = readerResRoutes.GetValue(c => c.RouteId);
                        RoutePoint point = new RoutePoint();
                        point.AGL = readerResRoutes.GetValue(c => c.Agl);
                        point.ASL = readerResRoutes.GetValue(c => c.Asl);
                        if (readerResRoutes.GetValue(c => c.FinishTime) != null)
                        {
                            point.FinishTime = readerResRoutes.GetValue(c => c.FinishTime).Value;
                        }
                        if (readerResRoutes.GetValue(c => c.Lat) != null)
                        {
                            point.Lat = readerResRoutes.GetValue(c => c.Lat).Value;
                        }
                        if (readerResRoutes.GetValue(c => c.Lon) != null)
                        {
                            point.Lon = readerResRoutes.GetValue(c => c.Lon).Value;
                        }
                        PointStayType pointStayType;
                        if (Enum.TryParse<PointStayType>(readerResRoutes.GetValue(c => c.PointStayType), out pointStayType))
                            point.PointStayType = pointStayType;
                        if (readerResRoutes.GetValue(c => c.StartTime) != null)
                        {
                            point.StartTime = readerResRoutes.GetValue(c => c.StartTime).Value;
                        }
                        points.Add(point);
                    }
                    route.RoutePoints = points.ToArray();
                    routes.Add(route);
                    return true;
                });
            }
            catch (Exception e)
            {
                this._logger.Exception(Contexts.ThisComponent, e);
            }
            return routes.ToArray();
        }

        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="measurementType"></param>
        /// <returns></returns>
        public ShortResultsMeasurementsStation[] GetShortMeasResStation(int ResId)
        {
            var listResMeasStatiion = new List<ShortResultsMeasurementsStation>();
            try
            {
                this._logger.Info(Contexts.ThisComponent, Categories.Processing, Events.HandlerGetShortMeasResStationMethod.Text);
                var queryExecuter = this._dataLayer.Executor<SdrnServerDataContext>();
                var builderResMeasStation = this._dataLayer.GetBuilder<MD.IResMeasStation>().From();
                builderResMeasStation.Select(c => c.GlobalSID);
                builderResMeasStation.Select(c => c.Id);
                builderResMeasStation.Select(c => c.IdStation);
                builderResMeasStation.Select(c => c.MeasGlobalSID);
                builderResMeasStation.Select(c => c.ResMeasId);
                builderResMeasStation.Select(c => c.SectorId);
                builderResMeasStation.Select(c => c.Standard);
                builderResMeasStation.Select(c => c.StationId);
                builderResMeasStation.Select(c => c.Status);
                builderResMeasStation.Where(c => c.ResMeasId, ConditionOperator.Equal, ResId);
                builderResMeasStation.OrderByAsc(c => c.Id);
                queryExecuter.Fetch(builderResMeasStation, readerResMeasStation =>
                {
                    while (readerResMeasStation.Read())
                    {
                        var resMeasStatiion = new ShortResultsMeasurementsStation();
                        resMeasStatiion.IdSector = readerResMeasStation.GetValue(c => c.SectorId);
                        if (readerResMeasStation.GetValue(c => c.StationId) != null)
                        {
                            resMeasStatiion.Idstation = readerResMeasStation.GetValue(c => c.StationId).Value.ToString();
                        }
                        resMeasStatiion.GlobalSID = readerResMeasStation.GetValue(c => c.GlobalSID);
                        resMeasStatiion.MeasGlobalSID = readerResMeasStation.GetValue(c => c.MeasGlobalSID);
                        resMeasStatiion.Status = readerResMeasStation.GetValue(c => c.Status);
                        resMeasStatiion.IdSector = readerResMeasStation.GetValue(c => c.SectorId);
                        resMeasStatiion.Idstation = readerResMeasStation.GetValue(c => c.StationId).HasValue ? readerResMeasStation.GetValue(c => c.StationId).Value.ToString() : "";
                        resMeasStatiion.Standard = readerResMeasStation.GetValue(c => c.Standard);


                        var builderStation = this._dataLayer.GetBuilder<MD.IStation>().From();
                        builderStation.Select(c => c.MeasTaskId);
                        builderStation.Select(c => c.Id);
                        builderStation.Select(c => c.StationSiteId);
                        builderStation.Where(c => c.Id, ConditionOperator.Equal, readerResMeasStation.GetValue(c => c.StationId));
                        builderStation.OrderByAsc(c => c.Id);
                        queryExecuter.Fetch(builderStation, readerStation =>
                        {
                            while (readerStation.Read())
                            {
                                var listStationSite = new List<SiteStationForMeas>();
                                var builderStationSite = this._dataLayer.GetBuilder<MD.IStationSite>().From();
                                builderStationSite.Select(c => c.Address);
                                builderStationSite.Select(c => c.Id);
                                builderStationSite.Select(c => c.Lat);
                                builderStationSite.Select(c => c.Lon);
                                builderStationSite.Select(c => c.Region);
                                builderStationSite.Where(c => c.Id, ConditionOperator.Equal, readerStation.GetValue(c => c.StationSiteId));
                                builderStationSite.OrderByAsc(c => c.Id);
                                queryExecuter.Fetch(builderStationSite, readerStationSite =>
                                {
                                    while (readerStationSite.Read())
                                    {
                                        SiteStationForMeas siteStationForMeas = new SiteStationForMeas();
                                        siteStationForMeas.Lon = readerStationSite.GetValue(c => c.Lon);
                                        siteStationForMeas.Lat = readerStationSite.GetValue(c => c.Lat);
                                        siteStationForMeas.Adress = readerStationSite.GetValue(c => c.Address);
                                        siteStationForMeas.Region = readerStationSite.GetValue(c => c.Region);
                                        siteStationForMeas.Id = readerStationSite.GetValue(c => c.Id);
                                        listStationSite.Add(siteStationForMeas);

                                    }
                                    return true;
                                });
                                resMeasStatiion.StationLocations = listStationSite.ToArray();
                            }
                            return true;
                        });

                        var builderResStGeneral = this._dataLayer.GetBuilder<MD.IResStGeneral>().From();
                        builderResStGeneral.Select(c => c.CentralFrequency);
                        builderResStGeneral.Select(c => c.CentralFrequencyMeas);
                        builderResStGeneral.Select(c => c.Correctnessestim);
                        builderResStGeneral.Select(c => c.DurationMeas);
                        builderResStGeneral.Select(c => c.Id);
                        builderResStGeneral.Select(c => c.MarkerIndex);
                        builderResStGeneral.Select(c => c.OffsetFrequency);
                        builderResStGeneral.Select(c => c.ResMeasStaId);
                        builderResStGeneral.Select(c => c.SpecrumStartFreq);
                        builderResStGeneral.Select(c => c.SpecrumSteps);
                        builderResStGeneral.Select(c => c.T1);
                        builderResStGeneral.Select(c => c.T2);
                        builderResStGeneral.Select(c => c.BW);
                        builderResStGeneral.Select(c => c.TimeFinishMeas);
                        builderResStGeneral.Select(c => c.TimeStartMeas);
                        builderResStGeneral.Select(c => c.TraceCount);
                        builderResStGeneral.Where(c => c.ResMeasStaId, ConditionOperator.Equal, readerResMeasStation.GetValue(c => c.Id));
                        builderResStGeneral.OrderByAsc(c => c.Id);
                        queryExecuter.Fetch(builderResStGeneral, readerResStGeneral =>
                        {
                            while (readerResStGeneral.Read())
                            {
                                resMeasStatiion.CentralFrequencyMeas_MHz = readerResStGeneral.GetValue(c => c.CentralFrequencyMeas);
                            }
                            return true;
                        });
                        listResMeasStatiion.Add(resMeasStatiion);
                    }
                    return true;
                });
            }
            catch (Exception e)
            {
                this._logger.Exception(Contexts.ThisComponent, e);
            }
            return listResMeasStatiion.ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="measurementType"></param>
        /// <returns></returns>
        public ResultsMeasurementsStation[] GetResMeasStationHeaderByResId(int ResId)
        {
            var listResMeasStatiion = new List<ResultsMeasurementsStation>();
            try
            {
                this._logger.Info(Contexts.ThisComponent, Categories.Processing, Events.HandlerCallGetResMeasStationHeaderByResIdMethod.Text);
                var queryExecuter = this._dataLayer.Executor<SdrnServerDataContext>();

                var builderResStGeneral = this._dataLayer.GetBuilder<MD.IResStGeneral>().From();
                builderResStGeneral.Select(c => c.CentralFrequency);
                builderResStGeneral.Select(c => c.CentralFrequencyMeas);
                builderResStGeneral.Select(c => c.Correctnessestim);
                builderResStGeneral.Select(c => c.DurationMeas);
                builderResStGeneral.Select(c => c.Id);
                builderResStGeneral.Select(c => c.MarkerIndex);
                builderResStGeneral.Select(c => c.OffsetFrequency);
                builderResStGeneral.Select(c => c.ResMeasStaId);
                builderResStGeneral.Select(c => c.SpecrumStartFreq);
                builderResStGeneral.Select(c => c.SpecrumSteps);
                builderResStGeneral.Select(c => c.T1);
                builderResStGeneral.Select(c => c.T2);
                builderResStGeneral.Select(c => c.BW);
                builderResStGeneral.Select(c => c.TimeFinishMeas);
                builderResStGeneral.Select(c => c.TimeStartMeas);
                builderResStGeneral.Select(c => c.TraceCount);
                builderResStGeneral.Select(c => c.RESMEASSTA.SectorId);
                builderResStGeneral.Select(c => c.RESMEASSTA.StationId);
                builderResStGeneral.Select(c => c.RESMEASSTA.GlobalSID);
                builderResStGeneral.Select(c => c.RESMEASSTA.MeasGlobalSID);
                builderResStGeneral.Select(c => c.RESMEASSTA.Status);
                builderResStGeneral.Select(c => c.RESMEASSTA.Id);
                builderResStGeneral.Select(c => c.RESMEASSTA.Standard);
                builderResStGeneral.Select(c => c.RESMEASSTA.ResMeasId);
                builderResStGeneral.Where(c => c.RESMEASSTA.ResMeasId, ConditionOperator.Equal, ResId);
                builderResStGeneral.OrderByAsc(c => c.Id);
                queryExecuter.Fetch(builderResStGeneral, readerResStGeneral =>
                {
                    while (readerResStGeneral.Read())
                    {
                        var resMeasStatiion = new ResultsMeasurementsStation();

                        if (listResMeasStatiion.Find(x => x.Id == readerResStGeneral.GetValue(c => c.RESMEASSTA.Id)) == null)
                        {
                            resMeasStatiion.IdSector = readerResStGeneral.GetValue(c => c.RESMEASSTA.SectorId);
                            if (readerResStGeneral.GetValue(c => c.RESMEASSTA.StationId) != null)
                            {
                                resMeasStatiion.Idstation = readerResStGeneral.GetValue(c => c.RESMEASSTA.StationId).Value.ToString();
                            }
                            resMeasStatiion.GlobalSID = readerResStGeneral.GetValue(c => c.RESMEASSTA.GlobalSID);
                            resMeasStatiion.MeasGlobalSID = readerResStGeneral.GetValue(c => c.RESMEASSTA.MeasGlobalSID);
                            resMeasStatiion.Status = readerResStGeneral.GetValue(c => c.RESMEASSTA.Status);
                            resMeasStatiion.Id = readerResStGeneral.GetValue(c => c.RESMEASSTA.Id);
                            resMeasStatiion.IdSector = readerResStGeneral.GetValue(c => c.RESMEASSTA.SectorId);
                            resMeasStatiion.Idstation = readerResStGeneral.GetValue(c => c.RESMEASSTA.StationId).HasValue ? readerResStGeneral.GetValue(c => c.RESMEASSTA.StationId).Value.ToString() : "";
                            resMeasStatiion.Standard = readerResStGeneral.GetValue(c => c.RESMEASSTA.Standard);


                            var measurementsParameterGeneral = new MeasurementsParameterGeneral();

                            measurementsParameterGeneral.CentralFrequency = readerResStGeneral.GetValue(c => c.CentralFrequency);
                            measurementsParameterGeneral.CentralFrequencyMeas = readerResStGeneral.GetValue(c => c.CentralFrequencyMeas);
                            measurementsParameterGeneral.DurationMeas = readerResStGeneral.GetValue(c => c.DurationMeas);
                            measurementsParameterGeneral.MarkerIndex = readerResStGeneral.GetValue(c => c.MarkerIndex);
                            measurementsParameterGeneral.OffsetFrequency = readerResStGeneral.GetValue(c => c.OffsetFrequency);
                            measurementsParameterGeneral.SpecrumStartFreq = (decimal?)readerResStGeneral.GetValue(c => c.SpecrumStartFreq);
                            measurementsParameterGeneral.SpecrumSteps = (decimal?)readerResStGeneral.GetValue(c => c.SpecrumSteps);
                            measurementsParameterGeneral.T1 = readerResStGeneral.GetValue(c => c.T1);
                            measurementsParameterGeneral.T2 = readerResStGeneral.GetValue(c => c.T2);
                            measurementsParameterGeneral.TimeFinishMeas = readerResStGeneral.GetValue(c => c.TimeFinishMeas);
                            measurementsParameterGeneral.TimeStartMeas = readerResStGeneral.GetValue(c => c.TimeStartMeas);

                            resMeasStatiion.GeneralResult = measurementsParameterGeneral;
                            /*
                            var valSysInfo = "";


                            var queryStationSysInfo = this._dataLayer.GetBuilder<MD.IResSysInfo>()
                           .From()
                           .Select(c => c.Id, c => c.Agl, c => c.Asl, c => c.Bandwidth, c => c.BaseId, c => c.Bsic, c => c.ChannelNumber, c => c.Cid, c => c.Code, c => c.Ctoi, c => c.Eci, c => c.Enodebid, c => c.Freq, c => c.Icio, c => c.InbandPower, c => c.Iscp, c => c.Lac)
                           .Select(c => c.Lat, c => c.Lon, c => c.Mcc, c => c.Mnc, c => c.Nid, c => c.Pci, c => c.Pn, c => c.Power, c => c.Ptotal, c => c.Rnc, c => c.Rscp, c => c.Rsrp, c => c.Rsrq, c => c.Sc, c => c.Sid, c => c.Tac, c => c.TypeCdmaevdo, c => c.Ucid)
                           .Where(c => c.ResStGeneralId, ConditionOperator.Equal, readerResStGeneral.GetValue(c=>c.Id));
                            queryExecuter.Fetch(queryStationSysInfo, readerStationSysInfo =>
                            {
                                while (readerStationSysInfo.Read())
                                {

                                    valSysInfo += string.Format("AGL : {0}", readerStationSysInfo.GetValue(c => c.Agl)) + Environment.NewLine;
                                    valSysInfo += string.Format("ASL : {0}", readerStationSysInfo.GetValue(c => c.Asl)) + Environment.NewLine;
                                    valSysInfo += string.Format("BandWidth : {0}", readerStationSysInfo.GetValue(c => c.Bandwidth)) + Environment.NewLine;
                                    valSysInfo += string.Format("BaseID : {0}", readerStationSysInfo.GetValue(c => c.BaseId)) + Environment.NewLine;
                                    valSysInfo += string.Format("BSIC : {0}", readerStationSysInfo.GetValue(c => c.Bsic)) + Environment.NewLine;
                                    valSysInfo += string.Format("ChannelNumber : {0}", readerStationSysInfo.GetValue(c => c.ChannelNumber)) + Environment.NewLine;
                                    valSysInfo += string.Format("CID : {0}", readerStationSysInfo.GetValue(c => c.Cid)) + Environment.NewLine;
                                    valSysInfo += string.Format("Code : {0}", readerStationSysInfo.GetValue(c => c.Code)) + Environment.NewLine;
                                    valSysInfo += string.Format("CtoI : {0}", readerStationSysInfo.GetValue(c => c.Ctoi)) + Environment.NewLine;
                                    valSysInfo += string.Format("ECI : {0}", readerStationSysInfo.GetValue(c => c.Eci)) + Environment.NewLine;
                                    valSysInfo += string.Format("eNodeBId : {0}", readerStationSysInfo.GetValue(c => c.Enodebid)) + Environment.NewLine;
                                    valSysInfo += string.Format("Freq : {0}", readerStationSysInfo.GetValue(c => c.Freq)) + Environment.NewLine;
                                    valSysInfo += string.Format("IcIo : {0}", readerStationSysInfo.GetValue(c => c.Icio)) + Environment.NewLine;
                                    valSysInfo += string.Format("INBAND_POWER : {0}", readerStationSysInfo.GetValue(c => c.InbandPower)) + Environment.NewLine;
                                    valSysInfo += string.Format("ISCP : {0}", readerStationSysInfo.GetValue(c => c.Iscp)) + Environment.NewLine;
                                    valSysInfo += string.Format("LAC : {0}", readerStationSysInfo.GetValue(c => c.Lac)) + Environment.NewLine;
                                    valSysInfo += string.Format("Lat : {0}", readerStationSysInfo.GetValue(c => c.Lat)) + Environment.NewLine;
                                    valSysInfo += string.Format("Lon : {0}", readerStationSysInfo.GetValue(c => c.Lon)) + Environment.NewLine;
                                    valSysInfo += string.Format("MCC : {0}", readerStationSysInfo.GetValue(c => c.Mcc)) + Environment.NewLine;
                                    valSysInfo += string.Format("MNC : {0}", readerStationSysInfo.GetValue(c => c.Mnc)) + Environment.NewLine;
                                    valSysInfo += string.Format("NID : {0}", readerStationSysInfo.GetValue(c => c.Nid)) + Environment.NewLine;
                                    valSysInfo += string.Format("PCI : {0}", readerStationSysInfo.GetValue(c => c.Pci)) + Environment.NewLine;
                                    valSysInfo += string.Format("PN : {0}", readerStationSysInfo.GetValue(c => c.Pn)) + Environment.NewLine;
                                    valSysInfo += string.Format("Power : {0}", readerStationSysInfo.GetValue(c => c.Power)) + Environment.NewLine;
                                    valSysInfo += string.Format("Ptotal : {0}", readerStationSysInfo.GetValue(c => c.Ptotal)) + Environment.NewLine;
                                    valSysInfo += string.Format("RNC : {0}", readerStationSysInfo.GetValue(c => c.Rnc)) + Environment.NewLine;
                                    valSysInfo += string.Format("RSCP : {0}", readerStationSysInfo.GetValue(c => c.Rscp)) + Environment.NewLine;
                                    valSysInfo += string.Format("RSRP : {0}", readerStationSysInfo.GetValue(c => c.Rsrp)) + Environment.NewLine;
                                    valSysInfo += string.Format("RSRQ : {0}", readerStationSysInfo.GetValue(c => c.Rsrq)) + Environment.NewLine;
                                    valSysInfo += string.Format("SC : {0}", readerStationSysInfo.GetValue(c => c.Sc)) + Environment.NewLine;
                                    valSysInfo += string.Format("SID : {0}", readerStationSysInfo.GetValue(c => c.Sid)) + Environment.NewLine;
                                    valSysInfo += string.Format("TAC : {0}", readerStationSysInfo.GetValue(c => c.Tac)) + Environment.NewLine;
                                    valSysInfo += string.Format("TypeCDMAEVDO : {0}", readerStationSysInfo.GetValue(c => c.TypeCdmaevdo)) + Environment.NewLine;
                                    valSysInfo += string.Format("UCID : {0}", readerStationSysInfo.GetValue(c => c.Ucid)) + Environment.NewLine;


                                    var queryStationSysInfoBls = this._dataLayer.GetBuilder<MD.IResSysInfoBlocks>()
                                    .From()
                                    .Select(c => c.Id, c => c.BinData, c => c.Type)
                                    .Where(c => c.ResSysInfoId, ConditionOperator.Equal, readerStationSysInfo.GetValue(c => c.Id));
                                    queryExecuter.Fetch(queryStationSysInfoBls, readerStationSysInfoBls =>
                                    {
                                        while (readerStationSysInfoBls.Read())
                                        {
                                            valSysInfo += string.Format("SysInfoBlocks.Data  : {0}", BinaryDecoder.Deserialize<string>(readerStationSysInfoBls.GetValue(c => c.BinData))) + Environment.NewLine;
                                            valSysInfo += string.Format("SysInfoBlocks.Type  : {0}", readerStationSysInfoBls.GetValue(c => c.Type)) + Environment.NewLine;
                                        }
                                        return true;
                                    });


                                }
                                return true;
                            });

                            resMeasStatiion.StationSysInfo = valSysInfo;
                            */
                            listResMeasStatiion.Add(resMeasStatiion);
                        }

                    }
                    return true;
                });
            }
            catch (Exception e)
            {
                this._logger.Exception(Contexts.ThisComponent, e);
            }
            return listResMeasStatiion.ToArray();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="measurementType"></param>
        /// <returns></returns>
        public ResultsMeasurementsStation[] GetResMeasStation(int ResId, int StationId)
        {
            var listResMeasStatiion = new List<ResultsMeasurementsStation>();
            try
            {
                this._logger.Info(Contexts.ThisComponent, Categories.Processing, Events.HandlerCallGetResMeasStationMethod.Text);
                var queryExecuter = this._dataLayer.Executor<SdrnServerDataContext>();
                var builderResMeasStation = this._dataLayer.GetBuilder<MD.IResMeasStation>().From();
                builderResMeasStation.Select(c => c.GlobalSID);
                builderResMeasStation.Select(c => c.Id);
                builderResMeasStation.Select(c => c.IdStation);
                builderResMeasStation.Select(c => c.MeasGlobalSID);
                builderResMeasStation.Select(c => c.ResMeasId);
                builderResMeasStation.Select(c => c.SectorId);
                builderResMeasStation.Select(c => c.Standard);
                builderResMeasStation.Select(c => c.StationId);
                builderResMeasStation.Select(c => c.Status);
                builderResMeasStation.Where(c => c.ResMeasId, ConditionOperator.Equal, ResId);
                builderResMeasStation.Where(c => c.IdStation, ConditionOperator.Equal, StationId);
                builderResMeasStation.OrderByAsc(c => c.Id);
                queryExecuter.Fetch(builderResMeasStation, readerResMeasStation =>
                {
                    while (readerResMeasStation.Read())
                    {
                        var resMeasStatiion = new ResultsMeasurementsStation();
                        resMeasStatiion.IdSector = readerResMeasStation.GetValue(c => c.SectorId);
                        if (readerResMeasStation.GetValue(c => c.StationId) != null)
                        {
                            resMeasStatiion.Idstation = readerResMeasStation.GetValue(c => c.StationId).Value.ToString();
                        }
                        resMeasStatiion.GlobalSID = readerResMeasStation.GetValue(c => c.GlobalSID);
                        resMeasStatiion.MeasGlobalSID = readerResMeasStation.GetValue(c => c.MeasGlobalSID);
                        resMeasStatiion.Status = readerResMeasStation.GetValue(c => c.Status);
                        resMeasStatiion.Id = readerResMeasStation.GetValue(c => c.Id);
                        resMeasStatiion.IdSector = readerResMeasStation.GetValue(c => c.SectorId);
                        resMeasStatiion.Idstation = readerResMeasStation.GetValue(c => c.StationId).HasValue ? readerResMeasStation.GetValue(c => c.StationId).Value.ToString() : "";
                        resMeasStatiion.Standard = readerResMeasStation.GetValue(c => c.Standard);


                      

                        double? rbw = null;
                        double? vbw = null;

                        var measurementsParameterGeneral = new MeasurementsParameterGeneral();
                        var builderResStGeneral = this._dataLayer.GetBuilder<MD.IResStGeneral>().From();
                        builderResStGeneral.Select(c => c.CentralFrequency);
                        builderResStGeneral.Select(c => c.CentralFrequencyMeas);
                        builderResStGeneral.Select(c => c.Correctnessestim);
                        builderResStGeneral.Select(c => c.DurationMeas);
                        builderResStGeneral.Select(c => c.Id);
                        builderResStGeneral.Select(c => c.MarkerIndex);
                        builderResStGeneral.Select(c => c.OffsetFrequency);
                        builderResStGeneral.Select(c => c.ResMeasStaId);
                        builderResStGeneral.Select(c => c.SpecrumStartFreq);
                        builderResStGeneral.Select(c => c.SpecrumSteps);
                        builderResStGeneral.Select(c => c.T1);
                        builderResStGeneral.Select(c => c.T2);
                        builderResStGeneral.Select(c => c.BW);
                        builderResStGeneral.Select(c => c.TimeFinishMeas);
                        builderResStGeneral.Select(c => c.TimeStartMeas);
                        builderResStGeneral.Select(c => c.TraceCount);
                        builderResStGeneral.Select(c => c.Rbw);
                        builderResStGeneral.Select(c => c.Vbw);
                        builderResStGeneral.Where(c => c.ResMeasStaId, ConditionOperator.Equal, readerResMeasStation.GetValue(c => c.Id));
                        builderResStGeneral.OrderByAsc(c => c.Id);
                        queryExecuter.Fetch(builderResStGeneral, readerResStGeneral =>
                        {
                            while (readerResStGeneral.Read())
                            {
                                measurementsParameterGeneral.CentralFrequency = readerResStGeneral.GetValue(c => c.CentralFrequency);
                                measurementsParameterGeneral.CentralFrequencyMeas = readerResStGeneral.GetValue(c => c.CentralFrequencyMeas);
                                measurementsParameterGeneral.DurationMeas = readerResStGeneral.GetValue(c => c.DurationMeas);
                                measurementsParameterGeneral.MarkerIndex = readerResStGeneral.GetValue(c => c.MarkerIndex);
                                measurementsParameterGeneral.OffsetFrequency = readerResStGeneral.GetValue(c => c.OffsetFrequency);
                                measurementsParameterGeneral.SpecrumStartFreq = (decimal?)readerResStGeneral.GetValue(c => c.SpecrumStartFreq);
                                measurementsParameterGeneral.SpecrumSteps = (decimal?)readerResStGeneral.GetValue(c => c.SpecrumSteps);
                                measurementsParameterGeneral.T1 = readerResStGeneral.GetValue(c => c.T1);
                                measurementsParameterGeneral.T2 = readerResStGeneral.GetValue(c => c.T2);
                                measurementsParameterGeneral.TimeFinishMeas = readerResStGeneral.GetValue(c => c.TimeFinishMeas);
                                measurementsParameterGeneral.TimeStartMeas = readerResStGeneral.GetValue(c => c.TimeStartMeas);
                                rbw = readerResStGeneral.GetValue(c => c.Rbw);
                                vbw = readerResStGeneral.GetValue(c => c.Vbw);


                                var listMaskElements = new List<MaskElements>();
                                var builderResStMaskElement = this._dataLayer.GetBuilder<MD.IResStMaskElement>().From();
                                builderResStMaskElement.Select(c => c.Bw);
                                builderResStMaskElement.Select(c => c.Level);
                                builderResStMaskElement.Where(c => c.ResStGeneralId, ConditionOperator.Equal, readerResStGeneral.GetValue(c => c.Id));
                                builderResStMaskElement.OrderByAsc(c => c.Id);
                                queryExecuter.Fetch(builderResStMaskElement, readerResStMaskElement =>
                                {
                                    while (readerResStMaskElement.Read())
                                    {
                                        var maskElements = new MaskElements();
                                        maskElements.BW = readerResStMaskElement.GetValue(c => c.Bw);
                                        maskElements.level = readerResStMaskElement.GetValue(c => c.Level);
                                        listMaskElements.Add(maskElements);
                                    }
                                    return true;

                                });
                                measurementsParameterGeneral.MaskBW = listMaskElements.ToArray();



                                var levelSpectrum = new List<float>();
                                var builderResStLevelsSpect = this._dataLayer.GetBuilder<MD.IResStLevelsSpect>().From();
                                builderResStLevelsSpect.Select(c => c.LevelSpecrum);
                                builderResStLevelsSpect.Select(c => c.Id);
                                builderResStLevelsSpect.Where(c => c.ResStGeneralId, ConditionOperator.Equal, readerResStGeneral.GetValue(c => c.Id));
                                builderResStLevelsSpect.OrderByAsc(c => c.Id);
                                queryExecuter.Fetch(builderResStLevelsSpect, readerResStLevelsSpect =>
                                {
                                    while (readerResStLevelsSpect.Read())
                                    {
                                        if (readerResStLevelsSpect.GetValue(c => c.LevelSpecrum) != null)
                                        {
                                            levelSpectrum.Add(readerResStLevelsSpect.GetValue(c => c.LevelSpecrum).Value);
                                        }
                                    }
                                    return true;

                                });
                                measurementsParameterGeneral.LevelsSpecrum = levelSpectrum.ToArray();



                                var valSysInfo = "";


                                var queryStationSysInfo = this._dataLayer.GetBuilder<MD.IResSysInfo>()
                               .From()
                               .Select(c => c.Id, c => c.Agl, c => c.Asl, c => c.Bandwidth, c => c.BaseId, c => c.Bsic, c => c.ChannelNumber, c => c.Cid, c => c.Code, c => c.Ctoi, c => c.Eci, c => c.Enodebid, c => c.Freq, c => c.Icio, c => c.InbandPower, c => c.Iscp, c => c.Lac)
                               .Select(c => c.Lat, c => c.Lon, c => c.Mcc, c => c.Mnc, c => c.Nid, c => c.Pci, c => c.Pn, c => c.Power, c => c.Ptotal, c => c.Rnc, c => c.Rscp, c => c.Rsrp, c => c.Rsrq, c => c.Sc, c => c.Sid, c => c.Tac, c => c.TypeCdmaevdo, c => c.Ucid)
                               .Where(c => c.ResStGeneralId, ConditionOperator.Equal, readerResStGeneral.GetValue(c => c.Id));
                                queryExecuter.Fetch(queryStationSysInfo, readerStationSysInfo =>
                                {
                                    while (readerStationSysInfo.Read())
                                    {

                                        valSysInfo += string.Format("AGL : {0}", readerStationSysInfo.GetValue(c => c.Agl)) + Environment.NewLine;
                                        valSysInfo += string.Format("ASL : {0}", readerStationSysInfo.GetValue(c => c.Asl)) + Environment.NewLine;
                                        valSysInfo += string.Format("BandWidth : {0}", readerStationSysInfo.GetValue(c => c.Bandwidth)) + Environment.NewLine;
                                        valSysInfo += string.Format("BaseID : {0}", readerStationSysInfo.GetValue(c => c.BaseId)) + Environment.NewLine;
                                        valSysInfo += string.Format("BSIC : {0}", readerStationSysInfo.GetValue(c => c.Bsic)) + Environment.NewLine;
                                        valSysInfo += string.Format("ChannelNumber : {0}", readerStationSysInfo.GetValue(c => c.ChannelNumber)) + Environment.NewLine;
                                        valSysInfo += string.Format("CID : {0}", readerStationSysInfo.GetValue(c => c.Cid)) + Environment.NewLine;
                                        valSysInfo += string.Format("Code : {0}", readerStationSysInfo.GetValue(c => c.Code)) + Environment.NewLine;
                                        valSysInfo += string.Format("CtoI : {0}", readerStationSysInfo.GetValue(c => c.Ctoi)) + Environment.NewLine;
                                        valSysInfo += string.Format("ECI : {0}", readerStationSysInfo.GetValue(c => c.Eci)) + Environment.NewLine;
                                        valSysInfo += string.Format("eNodeBId : {0}", readerStationSysInfo.GetValue(c => c.Enodebid)) + Environment.NewLine;
                                        valSysInfo += string.Format("Freq : {0}", readerStationSysInfo.GetValue(c => c.Freq)) + Environment.NewLine;
                                        valSysInfo += string.Format("IcIo : {0}", readerStationSysInfo.GetValue(c => c.Icio)) + Environment.NewLine;
                                        valSysInfo += string.Format("INBAND_POWER : {0}", readerStationSysInfo.GetValue(c => c.InbandPower)) + Environment.NewLine;
                                        valSysInfo += string.Format("ISCP : {0}", readerStationSysInfo.GetValue(c => c.Iscp)) + Environment.NewLine;
                                        valSysInfo += string.Format("LAC : {0}", readerStationSysInfo.GetValue(c => c.Lac)) + Environment.NewLine;
                                        valSysInfo += string.Format("Lat : {0}", readerStationSysInfo.GetValue(c => c.Lat)) + Environment.NewLine;
                                        valSysInfo += string.Format("Lon : {0}", readerStationSysInfo.GetValue(c => c.Lon)) + Environment.NewLine;
                                        valSysInfo += string.Format("MCC : {0}", readerStationSysInfo.GetValue(c => c.Mcc)) + Environment.NewLine;
                                        valSysInfo += string.Format("MNC : {0}", readerStationSysInfo.GetValue(c => c.Mnc)) + Environment.NewLine;
                                        valSysInfo += string.Format("NID : {0}", readerStationSysInfo.GetValue(c => c.Nid)) + Environment.NewLine;
                                        valSysInfo += string.Format("PCI : {0}", readerStationSysInfo.GetValue(c => c.Pci)) + Environment.NewLine;
                                        valSysInfo += string.Format("PN : {0}", readerStationSysInfo.GetValue(c => c.Pn)) + Environment.NewLine;
                                        valSysInfo += string.Format("Power : {0}", readerStationSysInfo.GetValue(c => c.Power)) + Environment.NewLine;
                                        valSysInfo += string.Format("Ptotal : {0}", readerStationSysInfo.GetValue(c => c.Ptotal)) + Environment.NewLine;
                                        valSysInfo += string.Format("RNC : {0}", readerStationSysInfo.GetValue(c => c.Rnc)) + Environment.NewLine;
                                        valSysInfo += string.Format("RSCP : {0}", readerStationSysInfo.GetValue(c => c.Rscp)) + Environment.NewLine;
                                        valSysInfo += string.Format("RSRP : {0}", readerStationSysInfo.GetValue(c => c.Rsrp)) + Environment.NewLine;
                                        valSysInfo += string.Format("RSRQ : {0}", readerStationSysInfo.GetValue(c => c.Rsrq)) + Environment.NewLine;
                                        valSysInfo += string.Format("SC : {0}", readerStationSysInfo.GetValue(c => c.Sc)) + Environment.NewLine;
                                        valSysInfo += string.Format("SID : {0}", readerStationSysInfo.GetValue(c => c.Sid)) + Environment.NewLine;
                                        valSysInfo += string.Format("TAC : {0}", readerStationSysInfo.GetValue(c => c.Tac)) + Environment.NewLine;
                                        valSysInfo += string.Format("TypeCDMAEVDO : {0}", readerStationSysInfo.GetValue(c => c.TypeCdmaevdo)) + Environment.NewLine;
                                        valSysInfo += string.Format("UCID : {0}", readerStationSysInfo.GetValue(c => c.Ucid)) + Environment.NewLine;


                                        var queryStationSysInfoBls = this._dataLayer.GetBuilder<MD.IResSysInfoBlocks>()
                                        .From()
                                        .Select(c => c.Id, c => c.BinData, c => c.Type)
                                        .Where(c => c.ResSysInfoId, ConditionOperator.Equal, readerStationSysInfo.GetValue(c => c.Id));
                                        queryExecuter.Fetch(queryStationSysInfoBls, readerStationSysInfoBls =>
                                        {
                                            while (readerStationSysInfoBls.Read())
                                            {
                                                valSysInfo += string.Format("SysInfoBlocks.Data  : {0}", BinaryDecoder.Deserialize<string>(readerStationSysInfoBls.GetValue(c => c.BinData))) + Environment.NewLine;
                                                valSysInfo += string.Format("SysInfoBlocks.Type  : {0}", readerStationSysInfoBls.GetValue(c => c.Type)) + Environment.NewLine;
                                            }
                                            return true;
                                        });

                                        resMeasStatiion.StationSysInfo = valSysInfo;

                                        break;
                                    }
                                    return true;
                                });


                                

                            }
                            return true;
                        });
                        resMeasStatiion.GeneralResult = measurementsParameterGeneral;

                        var listLevelMeasurementsCar = new List<LevelMeasurementsCar>();
                        var builderResStLevelCar = this._dataLayer.GetBuilder<MD.IResStLevelCar>().From();
                        builderResStLevelCar.Select(c => c.Agl);
                        builderResStLevelCar.Select(c => c.Altitude);
                        builderResStLevelCar.Select(c => c.Bw);
                        builderResStLevelCar.Select(c => c.CentralFrequency);
                        builderResStLevelCar.Select(c => c.DifferenceTimeStamp);
                        builderResStLevelCar.Select(c => c.Id);
                        builderResStLevelCar.Select(c => c.Lat);
                        builderResStLevelCar.Select(c => c.LevelDbm);
                        builderResStLevelCar.Select(c => c.LevelDbmkvm);
                        builderResStLevelCar.Select(c => c.Lon);
                        builderResStLevelCar.Select(c => c.ResStationId);
                        builderResStLevelCar.Select(c => c.TimeOfMeasurements);
                        builderResStLevelCar.Where(c => c.ResStationId, ConditionOperator.Equal, readerResMeasStation.GetValue(c => c.Id));
                        builderResStLevelCar.OrderByAsc(c => c.Id);
                        queryExecuter.Fetch(builderResStLevelCar, readerResStLevelCar =>
                        {
                            while (readerResStLevelCar.Read())
                            {
                                var levelMeasurementsCar = new LevelMeasurementsCar();
                                levelMeasurementsCar.Altitude = readerResStLevelCar.GetValue(c => c.Altitude);
                                levelMeasurementsCar.BW = readerResStLevelCar.GetValue(c => c.Bw);
                                levelMeasurementsCar.CentralFrequency = (decimal?)readerResStLevelCar.GetValue(c => c.CentralFrequency);
                                levelMeasurementsCar.DifferenceTimestamp = readerResStLevelCar.GetValue(c => c.DifferenceTimeStamp);
                                levelMeasurementsCar.Lat = readerResStLevelCar.GetValue(c => c.Lat);
                                levelMeasurementsCar.LeveldBm = readerResStLevelCar.GetValue(c => c.LevelDbm);
                                levelMeasurementsCar.LeveldBmkVm = readerResStLevelCar.GetValue(c => c.LevelDbmkvm);
                                levelMeasurementsCar.Lon = readerResStLevelCar.GetValue(c => c.Lon);
                                levelMeasurementsCar.RBW = rbw;
                                levelMeasurementsCar.VBW = vbw;

                                if (readerResStLevelCar.GetValue(c => c.TimeOfMeasurements) != null)
                                {
                                    levelMeasurementsCar.TimeOfMeasurements = readerResStLevelCar.GetValue(c => c.TimeOfMeasurements).Value;
                                }
                                listLevelMeasurementsCar.Add(levelMeasurementsCar);
                            }
                            return true;
                        });
                        resMeasStatiion.LevelMeasurements = listLevelMeasurementsCar.ToArray();
                        listResMeasStatiion.Add(resMeasStatiion);
                    }
                    return true;
                });
            }
            catch (Exception e)
            {
                this._logger.Exception(Contexts.ThisComponent, e);
            }
            return listResMeasStatiion.ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="measurementType"></param>
        /// <returns></returns>
        public MeasurementResults GetMeasurementResultByResId(int ResId, bool isLoadAllData, double? StartFrequency_Hz=null, double? StopFrequency_Hz = null)
        {
            var levelmeasurementResults = new MeasurementResults();
            var loadMeasTask = new LoadMeasTask(_dataLayer, _logger);
            try
            {
                this._logger.Info(Contexts.ThisComponent, Categories.Processing, Events.HandlerCallGetMeasurementResultByResIdMethod.Text);
                var queryExecuter = this._dataLayer.Executor<SdrnServerDataContext>();
                var builderResMeas = this._dataLayer.GetBuilder<MD.IResMeas>().From();
                builderResMeas.Select(c => c.AntVal);
                builderResMeas.Select(c => c.DataRank);
                builderResMeas.Select(c => c.Id);
                builderResMeas.Select(c => c.MeasResultSID);
                builderResMeas.Select(c => c.MeasSubTaskId);
                builderResMeas.Select(c => c.MeasSubTaskStationId);
                builderResMeas.Select(c => c.MeasTaskId);
                builderResMeas.Select(c => c.N);
                builderResMeas.Select(c => c.ScansNumber);
                builderResMeas.Select(c => c.SensorId);
                builderResMeas.Select(c => c.StartTime);
                builderResMeas.Select(c => c.Status);
                builderResMeas.Select(c => c.StopTime);
                builderResMeas.Select(c => c.Synchronized);
                builderResMeas.Select(c => c.TimeMeas);
                builderResMeas.Select(c => c.TypeMeasurements);
                builderResMeas.OrderByAsc(c => c.Id);
                builderResMeas.Where(c => c.Id, ConditionOperator.Equal, ResId);
                builderResMeas.Where(c => c.Status, ConditionOperator.NotEqual, Status.Z.ToString());
                queryExecuter.Fetch(builderResMeas, readerResMeas =>
                {
                    while (readerResMeas.Read())
                    {

                        levelmeasurementResults.AntVal = readerResMeas.GetValue(c => c.AntVal);
                        levelmeasurementResults.DataRank = readerResMeas.GetValue(c => c.DataRank);
                        levelmeasurementResults.N = readerResMeas.GetValue(c => c.N);
                        levelmeasurementResults.Id = new MeasurementResultsIdentifier();
                        levelmeasurementResults.Id.MeasTaskId = new MeasTaskIdentifier();
                        if (readerResMeas.GetValue(c => c.MeasTaskId) != null)
                        {
                            int measTaskId = -1; int.TryParse(readerResMeas.GetValue(c => c.MeasTaskId), out measTaskId);
                            levelmeasurementResults.Id.MeasTaskId.Value = measTaskId;
                        }
                        levelmeasurementResults.Id.MeasSdrResultsId = readerResMeas.GetValue(c => c.Id);
                        levelmeasurementResults.StationMeasurements = new StationMeasurements();
                        levelmeasurementResults.StationMeasurements.StationId = new SensorIdentifier();
                        if (readerResMeas.GetValue(c => c.SensorId) != null)
                        {
                            levelmeasurementResults.StationMeasurements.StationId.Value = readerResMeas.GetValue(c => c.SensorId).Value;
                        }
                        levelmeasurementResults.Status = readerResMeas.GetValue(c => c.Status);
                        if (readerResMeas.GetValue(c => c.MeasSubTaskId) != null)
                        {
                            levelmeasurementResults.Id.SubMeasTaskId = readerResMeas.GetValue(c => c.MeasSubTaskId).Value;
                        }
                        if (readerResMeas.GetValue(c => c.MeasSubTaskStationId) != null)
                        {
                            levelmeasurementResults.Id.SubMeasTaskStationId = readerResMeas.GetValue(c => c.MeasSubTaskStationId).Value;
                        }
                        if (readerResMeas.GetValue(c => c.ScansNumber) != null)
                        {
                            levelmeasurementResults.ScansNumber = readerResMeas.GetValue(c => c.ScansNumber).Value;
                        }
                        if (readerResMeas.GetValue(c => c.TimeMeas) != null)
                        {
                            levelmeasurementResults.TimeMeas = readerResMeas.GetValue(c => c.TimeMeas).Value;
                        }
                        if (readerResMeas.GetValue(c => c.StartTime) != null)
                        {
                            levelmeasurementResults.StartTime = readerResMeas.GetValue(c => c.StartTime).Value;
                        }
                        if (readerResMeas.GetValue(c => c.StopTime) != null)
                        {
                            levelmeasurementResults.StopTime = readerResMeas.GetValue(c => c.StopTime).Value;
                        }

                        MeasurementType outResType;
                        if (Enum.TryParse<MeasurementType>(readerResMeas.GetValue(c => c.TypeMeasurements), out outResType))
                        {
                            levelmeasurementResults.TypeMeasurements = outResType;
                        }


                        var listMeasResult = new List<MeasurementResult>();
                        var listFreqMeas = new List<FrequencyMeasurement>();
                        var builderResLevels = this._dataLayer.GetBuilder<MD.IResLevels>().From();
                        builderResLevels.Select(c => c.FreqMeas);
                        builderResLevels.Select(c => c.Id);
                        builderResLevels.Select(c => c.LimitLvl);
                        builderResLevels.Select(c => c.LimitSpect);
                        builderResLevels.Select(c => c.OccupancyLvl);
                        builderResLevels.Select(c => c.OccupancySpect);
                        builderResLevels.Select(c => c.PDiffLvl);
                        builderResLevels.Select(c => c.PMaxLvl);
                        builderResLevels.Select(c => c.PMinLvl);
                        builderResLevels.Select(c => c.ResMeasId);
                        builderResLevels.Select(c => c.StddevLev);
                        builderResLevels.Select(c => c.StdDevSpect);
                        builderResLevels.Select(c => c.ValueLvl);
                        builderResLevels.Select(c => c.ValueSpect);
                        builderResLevels.Select(c => c.VMinLvl);
                        builderResLevels.Select(c => c.VMinSpect);
                        builderResLevels.Select(c => c.VMMaxLvl);
                        builderResLevels.Select(c => c.VMMaxSpect);
                        builderResLevels.Where(c => c.ResMeasId, ConditionOperator.Equal, readerResMeas.GetValue(c => c.Id));
                        builderResLevels.OrderByAsc(c => c.Id);
                        queryExecuter.Fetch(builderResLevels, readerResLevels =>
                        {
                            while (readerResLevels.Read())
                            {
                                var freqMeas = new FrequencyMeasurement();
                                freqMeas.Id = readerResLevels.GetValue(c => c.Id);
                                if (readerResLevels.GetValue(c => c.FreqMeas) != null)
                                {
                                    freqMeas.Freq = readerResLevels.GetValue(c => c.FreqMeas).Value;
                                }
                                listFreqMeas.Add(freqMeas);

                                if (levelmeasurementResults.TypeMeasurements == MeasurementType.Level)
                                {
                                    var levelMeasurementResult = new LevelMeasurementResult();
                                    levelMeasurementResult.Id = new MeasurementResultIdentifier();
                                    levelMeasurementResult.Id.Value = readerResLevels.GetValue(c => c.Id);
                                    levelMeasurementResult.Value = readerResLevels.GetValue(c => c.ValueLvl);
                                    levelMeasurementResult.PMin = readerResLevels.GetValue(c => c.PMinLvl);
                                    levelMeasurementResult.PMax = readerResLevels.GetValue(c => c.PMaxLvl);
                                    listMeasResult.Add(levelMeasurementResult);
                                }
                                if (levelmeasurementResults.TypeMeasurements == MeasurementType.SpectrumOccupation)
                                {
                                    var spectrumOccupationMeasurementResult = new SpectrumOccupationMeasurementResult();
                                    spectrumOccupationMeasurementResult.Id = new MeasurementResultIdentifier();
                                    spectrumOccupationMeasurementResult.Id.Value = readerResLevels.GetValue(c => c.Id);
                                    spectrumOccupationMeasurementResult.Value = readerResLevels.GetValue(c => c.OccupancySpect);
                                    listMeasResult.Add(spectrumOccupationMeasurementResult);
                                }
                            }
                            return true;
                        });
                        levelmeasurementResults.FrequenciesMeasurements = listFreqMeas.ToArray();


                        var builderResLevMeasOnline = this._dataLayer.GetBuilder<MD.IResLevMeasOnline>().From();
                        builderResLevMeasOnline.Select(c => c.Id);
                        builderResLevMeasOnline.Select(c => c.ResMeasId);
                        builderResLevMeasOnline.Select(c => c.Value);
                        builderResLevMeasOnline.Where(c => c.ResMeasId, ConditionOperator.Equal, readerResMeas.GetValue(c => c.Id));
                        builderResLevMeasOnline.OrderByAsc(c => c.Id);
                        queryExecuter.Fetch(builderResLevMeasOnline, readerResLevMeasOnline =>
                        {
                            while (readerResLevMeasOnline.Read())
                            {
                                var levelMeasurementOnlineResult = new LevelMeasurementOnlineResult();
                                levelMeasurementOnlineResult.Id = new MeasurementResultIdentifier();
                                levelMeasurementOnlineResult.Id.Value = readerResLevMeasOnline.GetValue(c => c.Id);
                                if (readerResLevMeasOnline.GetValue(c => c.Value) != null)
                                {
                                    levelMeasurementOnlineResult.Value = readerResLevMeasOnline.GetValue(c => c.Value).Value;
                                }
                                listMeasResult.Add(levelMeasurementOnlineResult);
                            }
                            return true;
                        });

                        levelmeasurementResults.MeasurementsResults = listMeasResult.ToArray();


                    /// Location
                    var listLocationSensorMeasurement = new List<LocationSensorMeasurement>();
                        var builderResLocSensorMeas = this._dataLayer.GetBuilder<MD.IResLocSensorMeas>().From();
                        builderResLocSensorMeas.Select(c => c.Agl);
                        builderResLocSensorMeas.Select(c => c.Asl);
                        builderResLocSensorMeas.Select(c => c.Id);
                        builderResLocSensorMeas.Select(c => c.Lat);
                        builderResLocSensorMeas.Select(c => c.Lon);
                        builderResLocSensorMeas.Select(c => c.ResMeasId);
                        builderResLocSensorMeas.OrderByAsc(c => c.Id);
                        builderResLocSensorMeas.Where(c => c.ResMeasId, ConditionOperator.Equal, readerResMeas.GetValue(c => c.Id));
                        var locationSensorMeasurement = new LocationSensorMeasurement();
                        queryExecuter.Fetch(builderResLocSensorMeas, readerResLocSensorMeas =>
                        {
                            while (readerResLocSensorMeas.Read())
                            {
                                var locSensorMeas = new LocationSensorMeasurement();
                                locSensorMeas.ASL = readerResLocSensorMeas.GetValue(c => c.Asl);
                                locSensorMeas.Lon = readerResLocSensorMeas.GetValue(c => c.Lon);
                                locSensorMeas.Lat = readerResLocSensorMeas.GetValue(c => c.Lat);
                                listLocationSensorMeasurement.Add(locSensorMeas);
                            }
                            return true;
                        });
                        levelmeasurementResults.LocationSensorMeasurement = listLocationSensorMeasurement.ToArray();



                        levelmeasurementResults.CountUnknownStationMeasurements = 0;
                        levelmeasurementResults.CountStationMeasurements = 0;
                        if (outResType == MeasurementType.MonitoringStations)
                        {
                            var builderResMeasStation = this._dataLayer.GetBuilder<MD.IResMeasStation>().From();
                            builderResMeasStation.Select(c => c.Id);
                            builderResMeasStation.Select(c => c.Status);
                            builderResMeasStation.Where(c => c.ResMeasId, ConditionOperator.Equal, readerResMeas.GetValue(c => c.Id));
                            builderResMeasStation.Where(c => c.Status, ConditionOperator.Equal, Status.E.ToString());
                            builderResMeasStation.OrderByAsc(c => c.Id);
                            queryExecuter.Fetch(builderResMeasStation, readerResMeasStation =>
                            {
                                while (readerResMeasStation.Read())
                                {
                                    levelmeasurementResults.CountUnknownStationMeasurements++;
                                }
                                return true;
                            });



                            builderResMeasStation = this._dataLayer.GetBuilder<MD.IResMeasStation>().From();
                            builderResMeasStation.Select(c => c.Id);
                            builderResMeasStation.Select(c => c.Status);
                            builderResMeasStation.Where(c => c.ResMeasId, ConditionOperator.Equal, readerResMeas.GetValue(c => c.Id));
                            builderResMeasStation.Where(c => c.Status, ConditionOperator.NotEqual, Status.E.ToString());
                            builderResMeasStation.OrderByAsc(c => c.Id);
                            queryExecuter.Fetch(builderResMeasStation, readerResMeasStation =>
                            {
                                while (readerResMeasStation.Read())
                                {
                                    levelmeasurementResults.CountStationMeasurements++;
                                }
                                return true;
                            });




                            builderResMeasStation = this._dataLayer.GetBuilder<MD.IResMeasStation>().From();
                            builderResMeasStation.Select(c => c.Id);
                            builderResMeasStation.Select(c => c.Status);
                            builderResMeasStation.Where(c => c.ResMeasId, ConditionOperator.Equal, readerResMeas.GetValue(c => c.Id));
                            builderResMeasStation.Where(c => c.Status, ConditionOperator.IsNull);
                            builderResMeasStation.OrderByAsc(c => c.Id);
                            queryExecuter.Fetch(builderResMeasStation, readerResMeasStation =>
                            {
                                while (readerResMeasStation.Read())
                                {
                                    levelmeasurementResults.CountStationMeasurements++;
                                }
                                return true;
                            });


                            var builderLinkResSensoT = this._dataLayer.GetBuilder<MD.ILinkResSensor>().From();
                            builderLinkResSensoT.Select(c => c.Id);
                            builderLinkResSensoT.Select(c => c.SensorId);
                            builderLinkResSensoT.Select(c => c.SENSOR.Name);
                            builderLinkResSensoT.Select(c => c.SENSOR.TechId);
                            builderLinkResSensoT.Where(c => c.RESMEASSTA.RESMEAS.Id, ConditionOperator.Equal, readerResMeas.GetValue(c => c.Id));
                            builderLinkResSensoT.OrderByAsc(c => c.Id);
                            queryExecuter.Fetch(builderLinkResSensoT, readerLinkResSensor =>
                            {
                                while (readerLinkResSensor.Read())
                                {
                                    if (readerLinkResSensor.GetValue(c => c.SENSOR.Name) != null)
                                    {
                                        levelmeasurementResults.SensorName = readerLinkResSensor.GetValue(c => c.SENSOR.Name);
                                        levelmeasurementResults.SensorTechId = readerLinkResSensor.GetValue(c => c.SENSOR.TechId);
                                        break;
                                    }
                                }
                                return true;
                            });
                        }
                        Emitting[] emittings = null;
                        ReferenceLevels referenceLevels = null;
                        GetEmittingAndReferenceLevels(readerResMeas.GetValue(c => c.Id), isLoadAllData, out emittings, out referenceLevels,  StartFrequency_Hz,  StopFrequency_Hz);
                        levelmeasurementResults.Emittings = emittings;
                        levelmeasurementResults.RefLevels = referenceLevels;
                    }
                    return true;
                });
            }
            catch (Exception e)
            {
                this._logger.Exception(Contexts.ThisComponent, e);
            }
            return levelmeasurementResults;
        }


        public MeasurementResults[] GetMeasResultsHeaderByTaskId(int MeasTaskId)
        {
            var results = new List<MeasurementResults>();
            try
            {
                this._logger.Info(Contexts.ThisComponent, Categories.Processing, Events.HandlerCallGetMeasResultsHeaderByTaskIdMethod.Text);
                var queryExecuter = this._dataLayer.Executor<SdrnServerDataContext>();
                var builderResMeas = this._dataLayer.GetBuilder<MD.IResMeas>().From();
                builderResMeas.Select(c => c.AntVal);
                builderResMeas.Select(c => c.DataRank);
                builderResMeas.Select(c => c.Id);
                builderResMeas.Select(c => c.MeasResultSID);
                builderResMeas.Select(c => c.MeasSubTaskId);
                builderResMeas.Select(c => c.MeasSubTaskStationId);
                builderResMeas.Select(c => c.MeasTaskId);
                builderResMeas.Select(c => c.N);
                builderResMeas.Select(c => c.ScansNumber);
                builderResMeas.Select(c => c.SensorId);
                builderResMeas.Select(c => c.StartTime);
                builderResMeas.Select(c => c.Status);
                builderResMeas.Select(c => c.StopTime);
                builderResMeas.Select(c => c.Synchronized);
                builderResMeas.Select(c => c.TimeMeas);
                builderResMeas.Select(c => c.TypeMeasurements);
                builderResMeas.OrderByAsc(c => c.Id);
                builderResMeas.Where(c => c.MeasTaskId, ConditionOperator.Equal, MeasTaskId.ToString());
                builderResMeas.Where(c => c.Status, ConditionOperator.NotEqual, Status.Z.ToString());
                queryExecuter.Fetch(builderResMeas, readerResMeas =>
                {
                    while (readerResMeas.Read())
                    {
                        var levelmeasurementResults = new MeasurementResults();

                        levelmeasurementResults.AntVal = readerResMeas.GetValue(c => c.AntVal);
                        levelmeasurementResults.DataRank = readerResMeas.GetValue(c => c.DataRank);
                        levelmeasurementResults.N = readerResMeas.GetValue(c => c.N);
                        levelmeasurementResults.Id = new MeasurementResultsIdentifier();
                        levelmeasurementResults.Id.MeasTaskId = new MeasTaskIdentifier();
                        int measTaskId = -1; int.TryParse(readerResMeas.GetValue(c => c.MeasTaskId), out measTaskId);
                        levelmeasurementResults.Id.MeasTaskId.Value = MeasTaskId;
                        levelmeasurementResults.Id.MeasSdrResultsId = readerResMeas.GetValue(c => c.Id);
                        levelmeasurementResults.StationMeasurements = new StationMeasurements();
                        levelmeasurementResults.StationMeasurements.StationId = new SensorIdentifier();
                        if (readerResMeas.GetValue(c => c.SensorId) != null)
                        {
                            levelmeasurementResults.StationMeasurements.StationId.Value = readerResMeas.GetValue(c => c.SensorId).Value;
                        }
                        levelmeasurementResults.Status = readerResMeas.GetValue(c => c.Status);
                        if (readerResMeas.GetValue(c => c.MeasSubTaskId) != null)
                        {
                            levelmeasurementResults.Id.SubMeasTaskId = readerResMeas.GetValue(c => c.MeasSubTaskId).Value;
                        }
                        if (readerResMeas.GetValue(c => c.ScansNumber) != null)
                        {
                            levelmeasurementResults.ScansNumber = readerResMeas.GetValue(c => c.ScansNumber).Value;
                        }
                        if (readerResMeas.GetValue(c => c.MeasSubTaskStationId) != null)
                        {
                            levelmeasurementResults.Id.SubMeasTaskStationId = readerResMeas.GetValue(c => c.MeasSubTaskStationId).Value;
                        }
                        if (readerResMeas.GetValue(c => c.TimeMeas) != null)
                        {
                            levelmeasurementResults.TimeMeas = readerResMeas.GetValue(c => c.TimeMeas).Value;
                        }
                        if (readerResMeas.GetValue(c => c.StartTime) != null)
                        {
                            levelmeasurementResults.StartTime = readerResMeas.GetValue(c => c.StartTime).Value;
                        }
                        if (readerResMeas.GetValue(c => c.StopTime) != null)
                        {
                            levelmeasurementResults.StopTime = readerResMeas.GetValue(c => c.StopTime).Value;
                        }
                        MeasurementType outResType;
                        if (Enum.TryParse<MeasurementType>(readerResMeas.GetValue(c => c.TypeMeasurements), out outResType))
                        {
                            levelmeasurementResults.TypeMeasurements = outResType;
                        }
                        results.Add(levelmeasurementResults);
                    }
                    return true;

                });
            }
            catch (Exception e)
            {
                this._logger.Exception(Contexts.ThisComponent, e);
            }
            return results.ToArray();
        }

       public MeasurementResults[] GetMeasResultsByTaskId(int MeasTaskId)
        {
            var results = new List<MeasurementResults>();
            try
            {
                this._logger.Info(Contexts.ThisComponent, Categories.Processing, Events.HandlerCallGetMeasResultsByTaskIdMethod.Text);
                var queryExecuter = this._dataLayer.Executor<SdrnServerDataContext>();
                var builderResMeas = this._dataLayer.GetBuilder<MD.IResMeas>().From();
                builderResMeas.Select(c => c.AntVal);
                builderResMeas.Select(c => c.DataRank);
                builderResMeas.Select(c => c.Id);
                builderResMeas.Select(c => c.MeasResultSID);
                builderResMeas.Select(c => c.MeasSubTaskId);
                builderResMeas.Select(c => c.MeasSubTaskStationId);
                builderResMeas.Select(c => c.MeasTaskId);
                builderResMeas.Select(c => c.N);
                builderResMeas.Select(c => c.ScansNumber);
                builderResMeas.Select(c => c.SensorId);
                builderResMeas.Select(c => c.StartTime);
                builderResMeas.Select(c => c.Status);
                builderResMeas.Select(c => c.StopTime);
                builderResMeas.Select(c => c.Synchronized);
                builderResMeas.Select(c => c.TimeMeas);
                builderResMeas.Select(c => c.TypeMeasurements);
                builderResMeas.OrderByAsc(c => c.Id);
                builderResMeas.Where(c => c.MeasTaskId, ConditionOperator.Equal, MeasTaskId.ToString());
                builderResMeas.Where(c => c.Status, ConditionOperator.NotEqual, Status.Z.ToString());
                queryExecuter.Fetch(builderResMeas, readerResMeas =>
                {
                    while (readerResMeas.Read())
                    {
                        var levelmeasurementResults = new MeasurementResults();

                        levelmeasurementResults.AntVal = readerResMeas.GetValue(c => c.AntVal);
                        levelmeasurementResults.DataRank = readerResMeas.GetValue(c => c.DataRank);
                        levelmeasurementResults.N = readerResMeas.GetValue(c => c.N);
                        levelmeasurementResults.Id = new MeasurementResultsIdentifier();
                        levelmeasurementResults.Id.MeasTaskId = new MeasTaskIdentifier();
                        int measTaskId = -1; int.TryParse(readerResMeas.GetValue(c => c.MeasTaskId), out measTaskId);
                        levelmeasurementResults.Id.MeasTaskId.Value = MeasTaskId;
                        levelmeasurementResults.Id.MeasSdrResultsId = readerResMeas.GetValue(c => c.Id);
                        levelmeasurementResults.StationMeasurements = new StationMeasurements();
                        levelmeasurementResults.StationMeasurements.StationId = new SensorIdentifier();
                        if (readerResMeas.GetValue(c => c.SensorId) != null)
                        {
                            levelmeasurementResults.StationMeasurements.StationId.Value = readerResMeas.GetValue(c => c.SensorId).Value;
                        }
                        levelmeasurementResults.Status = readerResMeas.GetValue(c => c.Status);
                        if (readerResMeas.GetValue(c => c.MeasSubTaskId) != null)
                        {
                            levelmeasurementResults.Id.SubMeasTaskId = readerResMeas.GetValue(c => c.MeasSubTaskId).Value;
                        }
                        if (readerResMeas.GetValue(c => c.MeasSubTaskStationId) != null)
                        {
                            levelmeasurementResults.Id.SubMeasTaskStationId = readerResMeas.GetValue(c => c.MeasSubTaskStationId).Value;
                        }
                        if (readerResMeas.GetValue(c => c.ScansNumber) != null)
                        {
                            levelmeasurementResults.ScansNumber = readerResMeas.GetValue(c => c.ScansNumber).Value;
                        }
                        if (readerResMeas.GetValue(c => c.TimeMeas) != null)
                        {
                            levelmeasurementResults.TimeMeas = readerResMeas.GetValue(c => c.TimeMeas).Value;
                        }
                        if (readerResMeas.GetValue(c => c.StartTime) != null)
                        {
                            levelmeasurementResults.StartTime = readerResMeas.GetValue(c => c.StartTime).Value;
                        }
                        if (readerResMeas.GetValue(c => c.StopTime) != null)
                        {
                            levelmeasurementResults.StopTime = readerResMeas.GetValue(c => c.StopTime).Value;
                        }
                        MeasurementType outResType;
                        if (Enum.TryParse<MeasurementType>(readerResMeas.GetValue(c => c.TypeMeasurements), out outResType))
                        {
                            levelmeasurementResults.TypeMeasurements = outResType;
                        }


                        var listMeasResult = new List<MeasurementResult>();
                        var listFreqMeas = new List<FrequencyMeasurement>();
                        var builderResLevels = this._dataLayer.GetBuilder<MD.IResLevels>().From();
                        builderResLevels.Select(c => c.FreqMeas);
                        builderResLevels.Select(c => c.Id);
                        builderResLevels.Select(c => c.LimitLvl);
                        builderResLevels.Select(c => c.LimitSpect);
                        builderResLevels.Select(c => c.OccupancyLvl);
                        builderResLevels.Select(c => c.OccupancySpect);
                        builderResLevels.Select(c => c.PDiffLvl);
                        builderResLevels.Select(c => c.PMaxLvl);
                        builderResLevels.Select(c => c.PMinLvl);
                        builderResLevels.Select(c => c.ResMeasId);
                        builderResLevels.Select(c => c.StddevLev);
                        builderResLevels.Select(c => c.StdDevSpect);
                        builderResLevels.Select(c => c.ValueLvl);
                        builderResLevels.Select(c => c.ValueSpect);
                        builderResLevels.Select(c => c.VMinLvl);
                        builderResLevels.Select(c => c.VMinSpect);
                        builderResLevels.Select(c => c.VMMaxLvl);
                        builderResLevels.Select(c => c.VMMaxSpect);
                        builderResLevels.Where(c => c.ResMeasId, ConditionOperator.Equal, readerResMeas.GetValue(c => c.Id));
                        builderResLevels.OrderByAsc(c => c.Id);
                        queryExecuter.Fetch(builderResLevels, readerResLevels =>
                        {
                            while (readerResLevels.Read())
                            {
                                var freqMeas = new FrequencyMeasurement();
                                freqMeas.Id = readerResLevels.GetValue(c => c.Id);
                                if (readerResLevels.GetValue(c => c.FreqMeas) != null)
                                {
                                    freqMeas.Freq = readerResLevels.GetValue(c => c.FreqMeas).Value;
                                }
                                listFreqMeas.Add(freqMeas);

                                if (levelmeasurementResults.TypeMeasurements == MeasurementType.Level)
                                {
                                    var levelMeasurementResult = new LevelMeasurementResult();
                                    levelMeasurementResult.Id = new MeasurementResultIdentifier();
                                    levelMeasurementResult.Id.Value = readerResLevels.GetValue(c => c.Id);
                                    levelMeasurementResult.Value = readerResLevels.GetValue(c => c.ValueLvl);
                                    levelMeasurementResult.PMin = readerResLevels.GetValue(c => c.PMinLvl);
                                    levelMeasurementResult.PMax = readerResLevels.GetValue(c => c.PMaxLvl);
                                    listMeasResult.Add(levelMeasurementResult);
                                }
                                if (levelmeasurementResults.TypeMeasurements == MeasurementType.SpectrumOccupation)
                                {
                                    var spectrumOccupationMeasurementResult = new SpectrumOccupationMeasurementResult();
                                    spectrumOccupationMeasurementResult.Id = new MeasurementResultIdentifier();
                                    spectrumOccupationMeasurementResult.Id.Value = readerResLevels.GetValue(c => c.Id);
                                    spectrumOccupationMeasurementResult.Value = readerResLevels.GetValue(c => c.OccupancySpect);
                                    listMeasResult.Add(spectrumOccupationMeasurementResult);
                                }
                            }
                            return true;
                        });
                        levelmeasurementResults.FrequenciesMeasurements = listFreqMeas.ToArray();


                        var builderResLevMeasOnline = this._dataLayer.GetBuilder<MD.IResLevMeasOnline>().From();
                        builderResLevMeasOnline.Select(c => c.Id);
                        builderResLevMeasOnline.Select(c => c.ResMeasId);
                        builderResLevMeasOnline.Select(c => c.Value);
                        builderResLevMeasOnline.Where(c => c.ResMeasId, ConditionOperator.Equal, readerResMeas.GetValue(c => c.Id));
                        builderResLevMeasOnline.OrderByAsc(c => c.Id);
                        queryExecuter.Fetch(builderResLevMeasOnline, readerResLevMeasOnline =>
                        {
                            while (readerResLevMeasOnline.Read())
                            {
                                var levelMeasurementOnlineResult = new LevelMeasurementOnlineResult();
                                levelMeasurementOnlineResult.Id = new MeasurementResultIdentifier();
                                levelMeasurementOnlineResult.Id.Value = readerResLevMeasOnline.GetValue(c => c.Id);
                                if (readerResLevMeasOnline.GetValue(c => c.Value) != null)
                                {
                                    levelMeasurementOnlineResult.Value = readerResLevMeasOnline.GetValue(c => c.Value).Value;
                                }
                                listMeasResult.Add(levelMeasurementOnlineResult);
                            }
                            return true;
                        });

                        levelmeasurementResults.MeasurementsResults = listMeasResult.ToArray();




                    /// Location
                    var listLocationSensorMeasurement = new List<LocationSensorMeasurement>();
                        var builderResLocSensorMeas = this._dataLayer.GetBuilder<MD.IResLocSensorMeas>().From();
                        builderResLocSensorMeas.Select(c => c.Agl);
                        builderResLocSensorMeas.Select(c => c.Asl);
                        builderResLocSensorMeas.Select(c => c.Id);
                        builderResLocSensorMeas.Select(c => c.Lat);
                        builderResLocSensorMeas.Select(c => c.Lon);
                        builderResLocSensorMeas.Select(c => c.ResMeasId);
                        builderResLocSensorMeas.OrderByAsc(c => c.Id);
                        builderResLocSensorMeas.Where(c => c.ResMeasId, ConditionOperator.Equal, readerResMeas.GetValue(c => c.Id));
                        var locationSensorMeasurement = new LocationSensorMeasurement();
                        queryExecuter.Fetch(builderResLocSensorMeas, readerResLocSensorMeas =>
                        {
                            while (readerResLocSensorMeas.Read())
                            {
                                var locSensorMeas = new LocationSensorMeasurement();
                                locSensorMeas.ASL = readerResLocSensorMeas.GetValue(c => c.Asl);
                                locSensorMeas.Lon = readerResLocSensorMeas.GetValue(c => c.Lon);
                                locSensorMeas.Lat = readerResLocSensorMeas.GetValue(c => c.Lat);
                                listLocationSensorMeasurement.Add(locSensorMeas);
                            }
                            return true;
                        });
                        levelmeasurementResults.LocationSensorMeasurement = listLocationSensorMeasurement.ToArray();


                        var listResMeasStatiion = new List<ResultsMeasurementsStation>();
                        var builderResMeasStation = this._dataLayer.GetBuilder<MD.IResMeasStation>().From();
                        builderResMeasStation.Select(c => c.GlobalSID);
                        builderResMeasStation.Select(c => c.Id);
                        builderResMeasStation.Select(c => c.IdStation);
                        builderResMeasStation.Select(c => c.MeasGlobalSID);
                        builderResMeasStation.Select(c => c.ResMeasId);
                        builderResMeasStation.Select(c => c.SectorId);
                        builderResMeasStation.Select(c => c.Standard);
                        builderResMeasStation.Select(c => c.StationId);
                        builderResMeasStation.Select(c => c.Status);
                        builderResMeasStation.Where(c => c.ResMeasId, ConditionOperator.Equal, readerResMeas.GetValue(c => c.Id));
                        builderResMeasStation.OrderByAsc(c => c.Id);
                        queryExecuter.Fetch(builderResMeasStation, readerResMeasStation =>
                        {
                            while (readerResMeasStation.Read())
                            {
                                var resMeasStatiion = new ResultsMeasurementsStation();
                                resMeasStatiion.IdSector = readerResMeasStation.GetValue(c => c.SectorId);
                                if (readerResMeasStation.GetValue(c => c.StationId) != null)
                                {
                                    resMeasStatiion.Idstation = readerResMeasStation.GetValue(c => c.StationId).Value.ToString();
                                }
                                resMeasStatiion.GlobalSID = readerResMeasStation.GetValue(c => c.GlobalSID);
                                resMeasStatiion.MeasGlobalSID = readerResMeasStation.GetValue(c => c.MeasGlobalSID);
                                resMeasStatiion.Status = readerResMeasStation.GetValue(c => c.Status);
                                resMeasStatiion.Id = readerResMeasStation.GetValue(c => c.Id);
                                resMeasStatiion.IdSector = readerResMeasStation.GetValue(c => c.SectorId);
                                resMeasStatiion.Idstation = readerResMeasStation.GetValue(c => c.StationId).HasValue ? readerResMeasStation.GetValue(c => c.StationId).Value.ToString() : "";
                                resMeasStatiion.Standard = readerResMeasStation.GetValue(c => c.Standard);



                                double? rbw = null;
                                double? vbw = null;

                                var measurementsParameterGeneral = new MeasurementsParameterGeneral();
                                var builderResStGeneral = this._dataLayer.GetBuilder<MD.IResStGeneral>().From();
                                builderResStGeneral.Select(c => c.CentralFrequency);
                                builderResStGeneral.Select(c => c.CentralFrequencyMeas);
                                builderResStGeneral.Select(c => c.Correctnessestim);
                                builderResStGeneral.Select(c => c.DurationMeas);
                                builderResStGeneral.Select(c => c.Id);
                                builderResStGeneral.Select(c => c.MarkerIndex);
                                builderResStGeneral.Select(c => c.OffsetFrequency);
                                builderResStGeneral.Select(c => c.ResMeasStaId);
                                builderResStGeneral.Select(c => c.SpecrumStartFreq);
                                builderResStGeneral.Select(c => c.SpecrumSteps);
                                builderResStGeneral.Select(c => c.T1);
                                builderResStGeneral.Select(c => c.T2);
                                builderResStGeneral.Select(c => c.BW);
                                builderResStGeneral.Select(c => c.TimeFinishMeas);
                                builderResStGeneral.Select(c => c.TimeStartMeas);
                                builderResStGeneral.Select(c => c.TraceCount);
                                builderResStGeneral.Select(c => c.Rbw);
                                builderResStGeneral.Select(c => c.Vbw);
                                builderResStGeneral.Where(c => c.ResMeasStaId, ConditionOperator.Equal, readerResMeasStation.GetValue(c => c.Id));
                                builderResStGeneral.OrderByAsc(c => c.Id);
                                queryExecuter.Fetch(builderResStGeneral, readerResStGeneral =>
                                {
                                    while (readerResStGeneral.Read())
                                    {
                                        measurementsParameterGeneral.CentralFrequency = readerResStGeneral.GetValue(c => c.CentralFrequency);
                                        measurementsParameterGeneral.CentralFrequencyMeas = readerResStGeneral.GetValue(c => c.CentralFrequencyMeas);
                                        measurementsParameterGeneral.DurationMeas = readerResStGeneral.GetValue(c => c.DurationMeas);
                                        measurementsParameterGeneral.MarkerIndex = readerResStGeneral.GetValue(c => c.MarkerIndex);
                                        measurementsParameterGeneral.OffsetFrequency = readerResStGeneral.GetValue(c => c.OffsetFrequency);
                                        measurementsParameterGeneral.SpecrumStartFreq = (decimal?)readerResStGeneral.GetValue(c => c.SpecrumStartFreq);
                                        measurementsParameterGeneral.SpecrumSteps = (decimal?)readerResStGeneral.GetValue(c => c.SpecrumSteps);
                                        measurementsParameterGeneral.T1 = readerResStGeneral.GetValue(c => c.T1);
                                        measurementsParameterGeneral.T2 = readerResStGeneral.GetValue(c => c.T2);
                                        measurementsParameterGeneral.TimeFinishMeas = readerResStGeneral.GetValue(c => c.TimeFinishMeas);
                                        measurementsParameterGeneral.TimeStartMeas = readerResStGeneral.GetValue(c => c.TimeStartMeas);
                                        rbw = readerResStGeneral.GetValue(c => c.Rbw);
                                        vbw = readerResStGeneral.GetValue(c => c.Vbw);


                                        var listMaskElements = new List<MaskElements>();
                                        var builderResStMaskElement = this._dataLayer.GetBuilder<MD.IResStMaskElement>().From();
                                        builderResStMaskElement.Select(c => c.Bw);
                                        builderResStMaskElement.Select(c => c.Level);
                                        builderResStMaskElement.Where(c => c.ResStGeneralId, ConditionOperator.Equal, readerResStGeneral.GetValue(c => c.Id));
                                        builderResStMaskElement.OrderByAsc(c => c.Id);
                                        queryExecuter.Fetch(builderResStMaskElement, readerResStMaskElement =>
                                        {

                                            while (readerResStMaskElement.Read())
                                            {
                                                var maskElements = new MaskElements();
                                                maskElements.BW = readerResStMaskElement.GetValue(c => c.Bw);
                                                maskElements.level = readerResStMaskElement.GetValue(c => c.Level);
                                                listMaskElements.Add(maskElements);
                                            }
                                            return true;

                                        });
                                        measurementsParameterGeneral.MaskBW = listMaskElements.ToArray();



                                        var levelSpectrum = new List<float>();
                                        var builderResStLevelsSpect = this._dataLayer.GetBuilder<MD.IResStLevelsSpect>().From();
                                        builderResStLevelsSpect.Select(c => c.LevelSpecrum);
                                        builderResStLevelsSpect.Select(c => c.Id);
                                        builderResStLevelsSpect.Where(c => c.ResStGeneralId, ConditionOperator.Equal, readerResStGeneral.GetValue(c => c.Id));
                                        builderResStLevelsSpect.OrderByAsc(c => c.Id);
                                        queryExecuter.Fetch(builderResStLevelsSpect, readerResStLevelsSpect =>
                                        {
                                            while (readerResStLevelsSpect.Read())
                                            {
                                                if (readerResStLevelsSpect.GetValue(c => c.LevelSpecrum) != null)
                                                {
                                                    levelSpectrum.Add(readerResStLevelsSpect.GetValue(c => c.LevelSpecrum).Value);
                                                }
                                            }
                                            return true;

                                        });
                                        measurementsParameterGeneral.LevelsSpecrum = levelSpectrum.ToArray();
                                    }
                                    return true;
                                });
                                resMeasStatiion.GeneralResult = measurementsParameterGeneral;


                                var listLevelMeasurementsCar = new List<LevelMeasurementsCar>();
                                var builderResStLevelCar = this._dataLayer.GetBuilder<MD.IResStLevelCar>().From();
                                builderResStLevelCar.Select(c => c.Agl);
                                builderResStLevelCar.Select(c => c.Altitude);
                                builderResStLevelCar.Select(c => c.Bw);
                                builderResStLevelCar.Select(c => c.CentralFrequency);
                                builderResStLevelCar.Select(c => c.DifferenceTimeStamp);
                                builderResStLevelCar.Select(c => c.Id);
                                builderResStLevelCar.Select(c => c.Lat);
                                builderResStLevelCar.Select(c => c.LevelDbm);
                                builderResStLevelCar.Select(c => c.LevelDbmkvm);
                                builderResStLevelCar.Select(c => c.Lon);
                                builderResStLevelCar.Select(c => c.ResStationId);
                                builderResStLevelCar.Select(c => c.TimeOfMeasurements);
                                builderResStLevelCar.Where(c => c.ResStationId, ConditionOperator.Equal, readerResMeasStation.GetValue(c => c.Id));
                                builderResStLevelCar.OrderByAsc(c => c.Id);
                                queryExecuter.Fetch(builderResStLevelCar, readerResStLevelCar =>
                                {
                                    while (readerResStLevelCar.Read())
                                    {
                                        var levelMeasurementsCar = new LevelMeasurementsCar();
                                        levelMeasurementsCar.Altitude = readerResStLevelCar.GetValue(c => c.Altitude);
                                        levelMeasurementsCar.BW = readerResStLevelCar.GetValue(c => c.Bw);
                                        levelMeasurementsCar.CentralFrequency = (decimal?)readerResStLevelCar.GetValue(c => c.CentralFrequency);
                                        levelMeasurementsCar.DifferenceTimestamp = readerResStLevelCar.GetValue(c => c.DifferenceTimeStamp);
                                        levelMeasurementsCar.Lat = readerResStLevelCar.GetValue(c => c.Lat);
                                        levelMeasurementsCar.LeveldBm = readerResStLevelCar.GetValue(c => c.LevelDbm);
                                        levelMeasurementsCar.LeveldBmkVm = readerResStLevelCar.GetValue(c => c.LevelDbmkvm);
                                        levelMeasurementsCar.Lon = readerResStLevelCar.GetValue(c => c.Lon);
                                        levelMeasurementsCar.RBW = rbw;
                                        levelMeasurementsCar.VBW = vbw;

                                        if (readerResStLevelCar.GetValue(c => c.TimeOfMeasurements) != null)
                                        {
                                            levelMeasurementsCar.TimeOfMeasurements = readerResStLevelCar.GetValue(c => c.TimeOfMeasurements).Value;
                                        }
                                        listLevelMeasurementsCar.Add(levelMeasurementsCar);
                                    }
                                    return true;
                                });
                                resMeasStatiion.LevelMeasurements = listLevelMeasurementsCar.ToArray();


                                listResMeasStatiion.Add(resMeasStatiion);
                            }
                            return true;
                        });
                        levelmeasurementResults.ResultsMeasStation = listResMeasStatiion.ToArray();
                        results.Add(levelmeasurementResults);
                        return true;
                    }
                    return true;
                });
            }
            catch (Exception e)
            {
                this._logger.Exception(Contexts.ThisComponent, e);
            }
            return results.ToArray();
        }


        public ShortMeasurementResults[] GetShortMeasResults()
        {
            var results = new List<ShortMeasurementResults>();
            try
            {
                this._logger.Info(Contexts.ThisComponent, Categories.Processing, Events.HandlerGetShortMeasResultsMethod.Text);
                var queryExecuter = this._dataLayer.Executor<SdrnServerDataContext>();

                var builderResLocSensorMeasFast = this._dataLayer.GetBuilder<MD.IResLocSensorMeas>().From();
                builderResLocSensorMeasFast.Select(c => c.Agl);
                builderResLocSensorMeasFast.Select(c => c.Asl);
                builderResLocSensorMeasFast.Select(c => c.Id);
                builderResLocSensorMeasFast.Select(c => c.Lat);
                builderResLocSensorMeasFast.Select(c => c.Lon);
                builderResLocSensorMeasFast.Select(c => c.RESMEAS.AntVal);
                builderResLocSensorMeasFast.Select(c => c.RESMEAS.DataRank);
                builderResLocSensorMeasFast.Select(c => c.RESMEAS.Id);
                builderResLocSensorMeasFast.Select(c => c.RESMEAS.MeasResultSID);
                builderResLocSensorMeasFast.Select(c => c.RESMEAS.MeasSubTaskId);
                builderResLocSensorMeasFast.Select(c => c.RESMEAS.MeasSubTaskStationId);
                builderResLocSensorMeasFast.Select(c => c.RESMEAS.MeasTaskId);
                builderResLocSensorMeasFast.Select(c => c.RESMEAS.N);
                builderResLocSensorMeasFast.Select(c => c.RESMEAS.ScansNumber);
                builderResLocSensorMeasFast.Select(c => c.RESMEAS.SensorId);
                builderResLocSensorMeasFast.Select(c => c.RESMEAS.StartTime);
                builderResLocSensorMeasFast.Select(c => c.RESMEAS.Status);
                builderResLocSensorMeasFast.Select(c => c.RESMEAS.StopTime);
                builderResLocSensorMeasFast.Select(c => c.RESMEAS.Synchronized);
                builderResLocSensorMeasFast.Select(c => c.RESMEAS.TimeMeas);
                builderResLocSensorMeasFast.Select(c => c.RESMEAS.TypeMeasurements);
                builderResLocSensorMeasFast.Select(c => c.RESMEAS.SENSOR.Name);
                builderResLocSensorMeasFast.Select(c => c.RESMEAS.SENSOR.TechId);
                builderResLocSensorMeasFast.Select(c => c.ResMeasId);
                builderResLocSensorMeasFast.Where(c => c.RESMEAS.Status, ConditionOperator.NotEqual, Status.Z.ToString());
                builderResLocSensorMeasFast.OrderByAsc(c => c.Id);
                var locationSensorMeasurement = new LocationSensorMeasurement();
                queryExecuter.Fetch(builderResLocSensorMeasFast, readerResLocSensorMeas =>
                {
                    while (readerResLocSensorMeas.Read())
                    {
                        var shortMeasurementResultsFast = new ShortMeasurementResults();
                        shortMeasurementResultsFast.CurrentLon = readerResLocSensorMeas.GetValue(c => c.Lon);
                        shortMeasurementResultsFast.CurrentLat = readerResLocSensorMeas.GetValue(c => c.Lat);

                        shortMeasurementResultsFast.DataRank = readerResLocSensorMeas.GetValue(c => c.RESMEAS.DataRank);
                        shortMeasurementResultsFast.Id = new MeasurementResultsIdentifier();
                        shortMeasurementResultsFast.Id.MeasTaskId = new MeasTaskIdentifier();
                        int measTaskId = -1; int.TryParse(readerResLocSensorMeas.GetValue(c => c.RESMEAS.MeasTaskId), out measTaskId);
                        shortMeasurementResultsFast.Id.MeasTaskId.Value = measTaskId;
                        shortMeasurementResultsFast.Id.MeasSdrResultsId = readerResLocSensorMeas.GetValue(c => c.RESMEAS.Id);
                        shortMeasurementResultsFast.Status = readerResLocSensorMeas.GetValue(c => c.RESMEAS.Status);
                        if (readerResLocSensorMeas.GetValue(c => c.RESMEAS.MeasSubTaskId) != null)
                        {
                            shortMeasurementResultsFast.Id.SubMeasTaskId = readerResLocSensorMeas.GetValue(c => c.RESMEAS.MeasSubTaskId).Value;
                        }
                        if (readerResLocSensorMeas.GetValue(c => c.RESMEAS.MeasSubTaskStationId) != null)
                        {
                            shortMeasurementResultsFast.Id.SubMeasTaskStationId = readerResLocSensorMeas.GetValue(c => c.RESMEAS.MeasSubTaskStationId).Value;
                        }
                        if (readerResLocSensorMeas.GetValue(c => c.RESMEAS.TimeMeas) != null)
                        {
                            shortMeasurementResultsFast.TimeMeas = readerResLocSensorMeas.GetValue(c => c.RESMEAS.TimeMeas).Value;
                        }
                        if (readerResLocSensorMeas.GetValue(c => c.RESMEAS.StartTime) != null)
                        {
                            shortMeasurementResultsFast.StartTime = readerResLocSensorMeas.GetValue(c => c.RESMEAS.StartTime).Value;
                        }
                        if (readerResLocSensorMeas.GetValue(c => c.RESMEAS.StopTime) != null)
                        {
                            shortMeasurementResultsFast.StopTime = readerResLocSensorMeas.GetValue(c => c.RESMEAS.StopTime).Value;
                        }
                        MeasurementType outResType;
                        if (Enum.TryParse<MeasurementType>(readerResLocSensorMeas.GetValue(c => c.RESMEAS.TypeMeasurements), out outResType))
                        {
                            shortMeasurementResultsFast.TypeMeasurements = outResType;
                        }
                        shortMeasurementResultsFast.Number = readerResLocSensorMeas.GetValue(c => c.RESMEAS.N).HasValue ? readerResLocSensorMeas.GetValue(c => c.RESMEAS.N).Value : -1;

                        shortMeasurementResultsFast.SensorName = readerResLocSensorMeas.GetValue(c => c.RESMEAS.SENSOR.Name);
                        shortMeasurementResultsFast.SensorTechId = readerResLocSensorMeas.GetValue(c => c.RESMEAS.SENSOR.TechId);

                        if ((results.Find(c => c.Id.MeasSdrResultsId == shortMeasurementResultsFast.Id.MeasSdrResultsId)) == null)
                        {
                            results.Add(shortMeasurementResultsFast);
                        }
                    }
                    return true;
                });



                /*
                var builderResMeas = this._dataLayer.GetBuilder<MD.IResMeas>().From();
                builderResMeas.Select(c => c.AntVal);
                builderResMeas.Select(c => c.DataRank);
                builderResMeas.Select(c => c.Id);
                builderResMeas.Select(c => c.MeasResultSID);
                builderResMeas.Select(c => c.MeasSubTaskId);
                builderResMeas.Select(c => c.MeasSubTaskStationId);
                builderResMeas.Select(c => c.MeasTaskId);
                builderResMeas.Select(c => c.N);
                builderResMeas.Select(c => c.ScansNumber);
                builderResMeas.Select(c => c.SensorId);
                builderResMeas.Select(c => c.StartTime);
                builderResMeas.Select(c => c.Status);
                builderResMeas.Select(c => c.StopTime);
                builderResMeas.Select(c => c.Synchronized);
                builderResMeas.Select(c => c.TimeMeas);
                builderResMeas.Select(c => c.TypeMeasurements);
                builderResMeas.OrderByAsc(c => c.Id);
                queryExecuter.Fetch(builderResMeas, readerResMeas =>
                {
                    var resultMeasurementResults = true;
                    while (readerResMeas.Read())
                    {
                        var shortMeasurementResults = new ShortMeasurementResults();

                        shortMeasurementResults.DataRank = readerResMeas.GetValue(c => c.DataRank);
                        shortMeasurementResults.Id = new MeasurementResultsIdentifier();
                        shortMeasurementResults.Id.MeasTaskId = new MeasTaskIdentifier();
                        int measTaskId = -1; int.TryParse(readerResMeas.GetValue(c => c.MeasTaskId), out measTaskId);
                        shortMeasurementResults.Id.MeasTaskId.Value = measTaskId;
                        shortMeasurementResults.Id.MeasSdrResultsId = readerResMeas.GetValue(c => c.Id);
                        shortMeasurementResults.Status = readerResMeas.GetValue(c => c.Status);
                        if (readerResMeas.GetValue(c => c.MeasSubTaskId) != null)
                        {
                            shortMeasurementResults.Id.SubMeasTaskId = readerResMeas.GetValue(c => c.MeasSubTaskId).Value;
                        }
                        if (readerResMeas.GetValue(c => c.MeasSubTaskStationId) != null)
                        {
                            shortMeasurementResults.Id.SubMeasTaskStationId = readerResMeas.GetValue(c => c.MeasSubTaskStationId).Value;
                        }
                        if (readerResMeas.GetValue(c => c.TimeMeas) != null)
                        {
                            shortMeasurementResults.TimeMeas = readerResMeas.GetValue(c => c.TimeMeas).Value;
                        }
                        MeasurementType outResType;
                        if (Enum.TryParse<MeasurementType>(readerResMeas.GetValue(c => c.TypeMeasurements), out outResType))
                        {
                            shortMeasurementResults.TypeMeasurements = outResType;
                        }
                        shortMeasurementResults.Number = readerResMeas.GetValue(c => c.N).HasValue ? readerResMeas.GetValue(c => c.N).Value : -1;


                        var builderResLocSensorMeas = this._dataLayer.GetBuilder<MD.IResLocSensorMeas>().From();
                        builderResLocSensorMeas.Select(c => c.Agl);
                        builderResLocSensorMeas.Select(c => c.Asl);
                        builderResLocSensorMeas.Select(c => c.Id);
                        builderResLocSensorMeas.Select(c => c.Lat);
                        builderResLocSensorMeas.Select(c => c.Lon);
                        builderResLocSensorMeas.Select(c => c.ResMeasId);
                        builderResLocSensorMeas.OrderByAsc(c => c.Id);
                        builderResLocSensorMeas.Where(c => c.ResMeasId, ConditionOperator.Equal, readerResMeas.GetValue(c => c.Id));
                        var locationSensorMeasurement = new LocationSensorMeasurement();
                        queryExecuter.Fetch(builderResLocSensorMeas, readerResLocSensorMeas =>
                        {
                            var resultResLocSensorMeas = true;
                            while (readerResLocSensorMeas.Read())
                            {
                                shortMeasurementResults.CurrentLon = readerResLocSensorMeas.GetValue(c => c.Lon);
                                shortMeasurementResults.CurrentLat = readerResLocSensorMeas.GetValue(c => c.Lat);
                            }
                            return resultResLocSensorMeas;
                        });


                        /*
                        shortMeasurementResults.CountUnknownStationMeasurements = 0;
                        var builderResMeasStation = this._dataLayer.GetBuilder<MD.IResMeasStation>().From();
                        builderResMeasStation.Select(c => c.Id);
                        builderResMeasStation.Select(c => c.Status);
                        builderResMeasStation.Where(c => c.ResMeasId, ConditionOperator.Equal, readerResMeas.GetValue(c => c.Id));
                        builderResMeasStation.Where(c => c.Status, ConditionOperator.Equal, "E");
                        builderResMeasStation.OrderByAsc(c => c.Id);
                        queryExecuter.Fetch(builderResMeasStation, readerResMeasStation =>
                        {
                            var resultResMeasStation = true;
                            while (readerResMeasStation.Read())
                            {
                                shortMeasurementResults.CountUnknownStationMeasurements++;
                            }
                            return resultResMeasStation;
                        });


                        shortMeasurementResults.CountStationMeasurements = 0;
                        builderResMeasStation = this._dataLayer.GetBuilder<MD.IResMeasStation>().From();
                        builderResMeasStation.Select(c => c.Id);
                        builderResMeasStation.Select(c => c.Status);
                        builderResMeasStation.Where(c => c.ResMeasId, ConditionOperator.Equal, readerResMeas.GetValue(c => c.Id));
                        builderResMeasStation.Where(c => c.Status, ConditionOperator.NotEqual, "E");
                        builderResMeasStation.OrderByAsc(c => c.Id);
                        queryExecuter.Fetch(builderResMeasStation, readerResMeasStation =>
                        {
                            var resultResMeasStation = true;
                            while (readerResMeasStation.Read())
                            {
                                shortMeasurementResults.CountStationMeasurements++;
                            }
                            return resultResMeasStation;
                        });


                        var builderLinkResSensoT = this._dataLayer.GetBuilder<MD.ILinkResSensor>().From();
                        builderLinkResSensoT.Select(c => c.Id);
                        builderLinkResSensoT.Select(c => c.SensorId);
                        builderLinkResSensoT.Select(c => c.SENSOR.Name);
                        builderLinkResSensoT.Select(c => c.SENSOR.TechId);
                        builderLinkResSensoT.Where(c => c.RESMEASSTA.RESMEAS.Id, ConditionOperator.Equal, readerResMeas.GetValue(c => c.Id));
                        builderLinkResSensoT.OrderByAsc(c => c.Id);
                        queryExecuter.Fetch(builderLinkResSensoT, readerLinkResSensor =>
                        {
                            var resultResMeasStation = true;
                            while (readerLinkResSensor.Read())
                            {
                                if (readerLinkResSensor.GetValue(c => c.SENSOR.Name) != null)
                                {
                                    shortMeasurementResults.SensorName = readerLinkResSensor.GetValue(c => c.SENSOR.Name);
                                    shortMeasurementResults.SensorTechId = readerLinkResSensor.GetValue(c => c.SENSOR.TechId);
                                    break;
                                }
                            }
                            return resultResMeasStation;
                        });


                        results.Add(shortMeasurementResults);
                    }
                    return resultMeasurementResults;

                });
        */
            }
            catch (Exception e)
            {
                this._logger.Exception(Contexts.ThisComponent, e);
            }
            return results.ToArray();
        }

        public ShortMeasurementResults[] GetShortMeasResultsByTaskId(int MeasTaskId)
        {
            var listlevelmeasurementResults = new List<ShortMeasurementResults>();
            try
            {
                this._logger.Info(Contexts.ThisComponent, Categories.Processing, Events.HandlerGetShortMeasResultsByTaskIdMethod.Text);
                var queryExecuter = this._dataLayer.Executor<SdrnServerDataContext>();
                var builderResMeas = this._dataLayer.GetBuilder<MD.IResLocSensorMeas>().From();
                builderResMeas.Select(c => c.RESMEAS.AntVal);
                builderResMeas.Select(c => c.RESMEAS.DataRank);
                builderResMeas.Select(c => c.RESMEAS.Id);
                builderResMeas.Select(c => c.Lon);
                builderResMeas.Select(c => c.Lat);
                builderResMeas.Select(c => c.RESMEAS.MeasResultSID);
                builderResMeas.Select(c => c.RESMEAS.MeasSubTaskId);
                builderResMeas.Select(c => c.RESMEAS.MeasSubTaskStationId);
                builderResMeas.Select(c => c.RESMEAS.MeasTaskId);
                builderResMeas.Select(c => c.RESMEAS.N);
                builderResMeas.Select(c => c.RESMEAS.ScansNumber);
                builderResMeas.Select(c => c.RESMEAS.SensorId);
                builderResMeas.Select(c => c.RESMEAS.StartTime);
                builderResMeas.Select(c => c.RESMEAS.Status);
                builderResMeas.Select(c => c.RESMEAS.StopTime);
                builderResMeas.Select(c => c.RESMEAS.Synchronized);
                builderResMeas.Select(c => c.RESMEAS.TimeMeas);
                builderResMeas.Select(c => c.RESMEAS.TypeMeasurements);
                builderResMeas.Select(c => c.RESMEAS.SENSOR.Name);
                builderResMeas.Select(c => c.RESMEAS.SENSOR.TechId);
                builderResMeas.OrderByAsc(c => c.Id);
                builderResMeas.Where(c => c.RESMEAS.MeasTaskId, ConditionOperator.Equal, MeasTaskId.ToString());
                builderResMeas.Where(c => c.RESMEAS.Status, ConditionOperator.NotEqual, Status.Z.ToString());
                queryExecuter.Fetch(builderResMeas, readerResMeas =>
                {
                    while (readerResMeas.Read())
                    {
                        var levelmeasurementResults = new ShortMeasurementResults();

                        levelmeasurementResults.DataRank = readerResMeas.GetValue(c => c.RESMEAS.DataRank);
                        levelmeasurementResults.Number = readerResMeas.GetValue(c => c.RESMEAS.N).HasValue ? readerResMeas.GetValue(c => c.RESMEAS.N).Value : -1;
                        levelmeasurementResults.Id = new MeasurementResultsIdentifier();
                        levelmeasurementResults.Id.MeasTaskId = new MeasTaskIdentifier();
                        if (readerResMeas.GetValue(c => c.RESMEAS.MeasTaskId) != null)
                        {
                            int measTaskId = -1; int.TryParse(readerResMeas.GetValue(c => c.RESMEAS.MeasTaskId), out measTaskId);
                            levelmeasurementResults.Id.MeasTaskId.Value = measTaskId;
                        }
                        levelmeasurementResults.Id.MeasSdrResultsId = readerResMeas.GetValue(c => c.RESMEAS.Id);
                        levelmeasurementResults.Status = readerResMeas.GetValue(c => c.RESMEAS.Status);
                        if (readerResMeas.GetValue(c => c.RESMEAS.MeasSubTaskId) != null)
                        {
                            levelmeasurementResults.Id.SubMeasTaskId = readerResMeas.GetValue(c => c.RESMEAS.MeasSubTaskId).Value;
                        }
                        if (readerResMeas.GetValue(c => c.RESMEAS.MeasSubTaskStationId) != null)
                        {
                            levelmeasurementResults.Id.SubMeasTaskStationId = readerResMeas.GetValue(c => c.RESMEAS.MeasSubTaskStationId).Value;
                        }
                        if (readerResMeas.GetValue(c => c.RESMEAS.TimeMeas) != null)
                        {
                            levelmeasurementResults.TimeMeas = readerResMeas.GetValue(c => c.RESMEAS.TimeMeas).Value;
                        }
                        if (readerResMeas.GetValue(c => c.RESMEAS.StartTime) != null)
                        {
                            levelmeasurementResults.StartTime = readerResMeas.GetValue(c => c.RESMEAS.StartTime).Value;
                        }
                        if (readerResMeas.GetValue(c => c.RESMEAS.StopTime) != null)
                        {
                            levelmeasurementResults.StopTime = readerResMeas.GetValue(c => c.RESMEAS.StopTime).Value;
                        }

                        MeasurementType outResType;
                        if (Enum.TryParse<MeasurementType>(readerResMeas.GetValue(c => c.RESMEAS.TypeMeasurements), out outResType))
                        {
                            levelmeasurementResults.TypeMeasurements = outResType;
                        }
                        levelmeasurementResults.SensorName = readerResMeas.GetValue(c => c.RESMEAS.SENSOR.Name);
                        levelmeasurementResults.SensorTechId = readerResMeas.GetValue(c => c.RESMEAS.SENSOR.TechId);
                        levelmeasurementResults.CurrentLon = readerResMeas.GetValue(c => c.Lon);
                        levelmeasurementResults.CurrentLat = readerResMeas.GetValue(c => c.Lat);


                        levelmeasurementResults.CountUnknownStationMeasurements = 0;
                        levelmeasurementResults.CountStationMeasurements = 0;
                        if (outResType == MeasurementType.MonitoringStations)
                        {
                            var builderResMeasStation = this._dataLayer.GetBuilder<MD.IResMeasStation>().From();
                            builderResMeasStation.Select(c => c.Id);
                            builderResMeasStation.Select(c => c.Status);
                            builderResMeasStation.Where(c => c.ResMeasId, ConditionOperator.Equal, readerResMeas.GetValue(c => c.RESMEAS.Id));
                            builderResMeasStation.Where(c => c.Status, ConditionOperator.Equal, Status.E.ToString());
                            builderResMeasStation.OrderByAsc(c => c.Id);
                            queryExecuter.Fetch(builderResMeasStation, readerResMeasStation =>
                            {
                                while (readerResMeasStation.Read())
                                {
                                    levelmeasurementResults.CountUnknownStationMeasurements++;
                                }
                                return true;
                            });



                            builderResMeasStation = this._dataLayer.GetBuilder<MD.IResMeasStation>().From();
                            builderResMeasStation.Select(c => c.Id);
                            builderResMeasStation.Select(c => c.Status);
                            builderResMeasStation.Where(c => c.ResMeasId, ConditionOperator.Equal, readerResMeas.GetValue(c => c.RESMEAS.Id));
                            builderResMeasStation.Where(c => c.Status, ConditionOperator.NotEqual, Status.E.ToString());
                            builderResMeasStation.OrderByAsc(c => c.Id);
                            queryExecuter.Fetch(builderResMeasStation, readerResMeasStation =>
                            {
                                while (readerResMeasStation.Read())
                                {
                                    levelmeasurementResults.CountStationMeasurements++;
                                }
                                return true;
                            });

                            builderResMeasStation = this._dataLayer.GetBuilder<MD.IResMeasStation>().From();
                            builderResMeasStation.Select(c => c.Id);
                            builderResMeasStation.Select(c => c.Status);
                            builderResMeasStation.Where(c => c.ResMeasId, ConditionOperator.Equal, readerResMeas.GetValue(c => c.RESMEAS.Id));
                            builderResMeasStation.Where(c => c.Status, ConditionOperator.IsNull);
                            builderResMeasStation.OrderByAsc(c => c.Id);
                            queryExecuter.Fetch(builderResMeasStation, readerResMeasStation =>
                            {
                                while (readerResMeasStation.Read())
                                {
                                    levelmeasurementResults.CountStationMeasurements++;
                                }
                                return true;
                            });
                        }
                        listlevelmeasurementResults.Add(levelmeasurementResults);
                    }
                    return true;

                });
            }
            catch (Exception e)
            {
                this._logger.Exception(Contexts.ThisComponent, e);
            }
            return listlevelmeasurementResults.ToArray();
        }

        public ShortMeasurementResults[] GetShortMeasResultsByTypeAndTaskId(MeasurementType measurementType, int taskId)
        {
            var listlevelmeasurementResults = new List<ShortMeasurementResults>();
            try
            {
                this._logger.Info(Contexts.ThisComponent, Categories.Processing, Events.HandlerGetShortMeasResultsByTypeAndTaskIdMethod.Text);
                var queryExecuter = this._dataLayer.Executor<SdrnServerDataContext>();
                var builderResMeas = this._dataLayer.GetBuilder<MD.IResLocSensorMeas>().From();
                builderResMeas.Select(c => c.RESMEAS.AntVal);
                builderResMeas.Select(c => c.RESMEAS.DataRank);
                builderResMeas.Select(c => c.RESMEAS.Id);
                builderResMeas.Select(c => c.Lon);
                builderResMeas.Select(c => c.Lat);
                builderResMeas.Select(c => c.RESMEAS.MeasResultSID);
                builderResMeas.Select(c => c.RESMEAS.MeasSubTaskId);
                builderResMeas.Select(c => c.RESMEAS.MeasSubTaskStationId);
                builderResMeas.Select(c => c.RESMEAS.MeasTaskId);
                builderResMeas.Select(c => c.RESMEAS.N);
                builderResMeas.Select(c => c.RESMEAS.ScansNumber);
                builderResMeas.Select(c => c.RESMEAS.SensorId);
                builderResMeas.Select(c => c.RESMEAS.StartTime);
                builderResMeas.Select(c => c.RESMEAS.Status);
                builderResMeas.Select(c => c.RESMEAS.StopTime);
                builderResMeas.Select(c => c.RESMEAS.Synchronized);
                builderResMeas.Select(c => c.RESMEAS.TimeMeas);
                builderResMeas.Select(c => c.RESMEAS.TypeMeasurements);
                builderResMeas.Select(c => c.RESMEAS.SENSOR.Name);
                builderResMeas.Select(c => c.RESMEAS.SENSOR.TechId);
                builderResMeas.OrderByAsc(c => c.Id);
                builderResMeas.Where(c => c.RESMEAS.TypeMeasurements, ConditionOperator.Equal, measurementType.ToString());
                builderResMeas.Where(c => c.RESMEAS.MeasTaskId, ConditionOperator.Equal, taskId.ToString());
                builderResMeas.Where(c => c.RESMEAS.Status, ConditionOperator.NotEqual, Status.Z.ToString());
                queryExecuter.Fetch(builderResMeas, readerResMeas =>
                {
                    while (readerResMeas.Read())
                    {
                        var levelmeasurementResults = new ShortMeasurementResults();

                        levelmeasurementResults.DataRank = readerResMeas.GetValue(c => c.RESMEAS.DataRank);
                        levelmeasurementResults.Number = readerResMeas.GetValue(c => c.RESMEAS.N).HasValue ? readerResMeas.GetValue(c => c.RESMEAS.N).Value : -1;
                        levelmeasurementResults.Id = new MeasurementResultsIdentifier();
                        levelmeasurementResults.Id.MeasTaskId = new MeasTaskIdentifier();
                        if (readerResMeas.GetValue(c => c.RESMEAS.MeasTaskId) != null)
                        {
                            int measTaskId = -1; int.TryParse(readerResMeas.GetValue(c => c.RESMEAS.MeasTaskId), out measTaskId);
                            levelmeasurementResults.Id.MeasTaskId.Value = measTaskId;
                        }
                        levelmeasurementResults.Id.MeasSdrResultsId = readerResMeas.GetValue(c => c.RESMEAS.Id);


                        levelmeasurementResults.Status = readerResMeas.GetValue(c => c.RESMEAS.Status);
                        if (readerResMeas.GetValue(c => c.RESMEAS.MeasSubTaskId) != null)
                        {
                            levelmeasurementResults.Id.SubMeasTaskId = readerResMeas.GetValue(c => c.RESMEAS.MeasSubTaskId).Value;
                        }
                        if (readerResMeas.GetValue(c => c.RESMEAS.MeasSubTaskStationId) != null)
                        {
                            levelmeasurementResults.Id.SubMeasTaskStationId = readerResMeas.GetValue(c => c.RESMEAS.MeasSubTaskStationId).Value;
                        }
                        if (readerResMeas.GetValue(c => c.RESMEAS.TimeMeas) != null)
                        {
                            levelmeasurementResults.TimeMeas = readerResMeas.GetValue(c => c.RESMEAS.TimeMeas).Value;
                        }
                        if (readerResMeas.GetValue(c => c.RESMEAS.StartTime) != null)
                        {
                            levelmeasurementResults.StartTime = readerResMeas.GetValue(c => c.RESMEAS.StartTime).Value;
                        }
                        if (readerResMeas.GetValue(c => c.RESMEAS.StopTime) != null)
                        {
                            levelmeasurementResults.StopTime = readerResMeas.GetValue(c => c.RESMEAS.StopTime).Value;
                        }
                        MeasurementType outResType;
                        if (Enum.TryParse<MeasurementType>(readerResMeas.GetValue(c => c.RESMEAS.TypeMeasurements), out outResType))
                        {
                            levelmeasurementResults.TypeMeasurements = outResType;
                        }
                        levelmeasurementResults.SensorName = readerResMeas.GetValue(c => c.RESMEAS.SENSOR.Name);
                        levelmeasurementResults.SensorTechId = readerResMeas.GetValue(c => c.RESMEAS.SENSOR.TechId);
                        levelmeasurementResults.CurrentLon = readerResMeas.GetValue(c => c.Lon);
                        levelmeasurementResults.CurrentLat = readerResMeas.GetValue(c => c.Lat);

                        levelmeasurementResults.CountUnknownStationMeasurements = 0;
                        levelmeasurementResults.CountStationMeasurements = 0;

                        if (outResType == MeasurementType.MonitoringStations)
                        {
                            var builderResMeasStation = this._dataLayer.GetBuilder<MD.IResMeasStation>().From();
                            builderResMeasStation.Select(c => c.Id);
                            builderResMeasStation.Select(c => c.Status);
                            builderResMeasStation.Where(c => c.ResMeasId, ConditionOperator.Equal, readerResMeas.GetValue(c => c.RESMEAS.Id));
                            builderResMeasStation.Where(c => c.Status, ConditionOperator.Equal, Status.E.ToString());
                            builderResMeasStation.OrderByAsc(c => c.Id);
                            queryExecuter.Fetch(builderResMeasStation, readerResMeasStation =>
                            {
                                while (readerResMeasStation.Read())
                                {
                                    levelmeasurementResults.CountUnknownStationMeasurements++;
                                }
                                return true;
                            });



                            builderResMeasStation = this._dataLayer.GetBuilder<MD.IResMeasStation>().From();
                            builderResMeasStation.Select(c => c.Id);
                            builderResMeasStation.Select(c => c.Status);
                            builderResMeasStation.Where(c => c.ResMeasId, ConditionOperator.Equal, readerResMeas.GetValue(c => c.RESMEAS.Id));
                            builderResMeasStation.Where(c => c.Status, ConditionOperator.NotEqual, Status.E.ToString());
                            builderResMeasStation.OrderByAsc(c => c.Id);
                            queryExecuter.Fetch(builderResMeasStation, readerResMeasStation =>
                            {
                                while (readerResMeasStation.Read())
                                {
                                    levelmeasurementResults.CountStationMeasurements++;
                                }
                                return true;
                            });


                            builderResMeasStation = this._dataLayer.GetBuilder<MD.IResMeasStation>().From();
                            builderResMeasStation.Select(c => c.Id);
                            builderResMeasStation.Select(c => c.Status);
                            builderResMeasStation.Where(c => c.ResMeasId, ConditionOperator.Equal, readerResMeas.GetValue(c => c.RESMEAS.Id));
                            builderResMeasStation.Where(c => c.Status, ConditionOperator.IsNull);
                            builderResMeasStation.OrderByAsc(c => c.Id);
                            queryExecuter.Fetch(builderResMeasStation, readerResMeasStation =>
                            {
                                while (readerResMeasStation.Read())
                                {
                                    levelmeasurementResults.CountStationMeasurements++;
                                }
                                return true;
                            });
                        }
                        listlevelmeasurementResults.Add(levelmeasurementResults);
                    }
                    return true;
                });
            }
            catch (Exception e)
            {
                this._logger.Exception(Contexts.ThisComponent, e);
            }
            return listlevelmeasurementResults.ToArray();
        }

        public ShortMeasurementResults[] GetShortMeasResultsSpecial(MeasurementType measurementType)
        {
            var listlevelmeasurementResults = new List<ShortMeasurementResults>();
            try
            {
                this._logger.Info(Contexts.ThisComponent, Categories.Processing, Events.HandlerGetShortMeasResultsSpecialMethod.Text);
                var queryExecuter = this._dataLayer.Executor<SdrnServerDataContext>();
                var builderResMeas = this._dataLayer.GetBuilder<MD.IResLocSensorMeas>().From();
                builderResMeas.Select(c => c.RESMEAS.AntVal);
                builderResMeas.Select(c => c.RESMEAS.DataRank);
                builderResMeas.Select(c => c.RESMEAS.Id);
                builderResMeas.Select(c => c.Lon);
                builderResMeas.Select(c => c.Lat);
                builderResMeas.Select(c => c.RESMEAS.MeasResultSID);
                builderResMeas.Select(c => c.RESMEAS.MeasSubTaskId);
                builderResMeas.Select(c => c.RESMEAS.MeasSubTaskStationId);
                builderResMeas.Select(c => c.RESMEAS.MeasTaskId);
                builderResMeas.Select(c => c.RESMEAS.N);
                builderResMeas.Select(c => c.RESMEAS.ScansNumber);
                builderResMeas.Select(c => c.RESMEAS.SensorId);
                builderResMeas.Select(c => c.RESMEAS.StartTime);
                builderResMeas.Select(c => c.RESMEAS.Status);
                builderResMeas.Select(c => c.RESMEAS.StopTime);
                builderResMeas.Select(c => c.RESMEAS.Synchronized);
                builderResMeas.Select(c => c.RESMEAS.TimeMeas);
                builderResMeas.Select(c => c.RESMEAS.TypeMeasurements);
                builderResMeas.Select(c => c.RESMEAS.SENSOR.Name);
                builderResMeas.Select(c => c.RESMEAS.SENSOR.TechId);
                builderResMeas.OrderByAsc(c => c.Id);
                builderResMeas.Where(c => c.RESMEAS.TypeMeasurements, ConditionOperator.Equal, measurementType.ToString());
                builderResMeas.Where(c => c.RESMEAS.Status, ConditionOperator.NotEqual, Status.Z.ToString());
                queryExecuter.Fetch(builderResMeas, readerResMeas =>
                {
                    while (readerResMeas.Read())
                    {
                        var levelmeasurementResults = new ShortMeasurementResults();

                        levelmeasurementResults.DataRank = readerResMeas.GetValue(c => c.RESMEAS.DataRank);
                        levelmeasurementResults.Number = readerResMeas.GetValue(c => c.RESMEAS.N).HasValue ? readerResMeas.GetValue(c => c.RESMEAS.N).Value : -1;
                        levelmeasurementResults.Id = new MeasurementResultsIdentifier();
                        levelmeasurementResults.Id.MeasTaskId = new MeasTaskIdentifier();
                        if (readerResMeas.GetValue(c => c.RESMEAS.MeasTaskId) != null)
                        {
                            int measTaskId = -1; int.TryParse(readerResMeas.GetValue(c => c.RESMEAS.MeasTaskId), out measTaskId);
                            levelmeasurementResults.Id.MeasTaskId.Value = measTaskId;
                        }
                        levelmeasurementResults.Id.MeasSdrResultsId = readerResMeas.GetValue(c => c.RESMEAS.Id);


                        levelmeasurementResults.Status = readerResMeas.GetValue(c => c.RESMEAS.Status);
                        if (readerResMeas.GetValue(c => c.RESMEAS.MeasSubTaskId) != null)
                        {
                            levelmeasurementResults.Id.SubMeasTaskId = readerResMeas.GetValue(c => c.RESMEAS.MeasSubTaskId).Value;
                        }
                        if (readerResMeas.GetValue(c => c.RESMEAS.MeasSubTaskStationId) != null)
                        {
                            levelmeasurementResults.Id.SubMeasTaskStationId = readerResMeas.GetValue(c => c.RESMEAS.MeasSubTaskStationId).Value;
                        }
                        if (readerResMeas.GetValue(c => c.RESMEAS.TimeMeas) != null)
                        {
                            levelmeasurementResults.TimeMeas = readerResMeas.GetValue(c => c.RESMEAS.TimeMeas).Value;
                        }
                        if (readerResMeas.GetValue(c => c.RESMEAS.StartTime) != null)
                        {
                            levelmeasurementResults.StartTime = readerResMeas.GetValue(c => c.RESMEAS.StartTime).Value;
                        }
                        if (readerResMeas.GetValue(c => c.RESMEAS.StopTime) != null)
                        {
                            levelmeasurementResults.StopTime = readerResMeas.GetValue(c => c.RESMEAS.StopTime).Value;
                        }
                        MeasurementType outResType;
                        if (Enum.TryParse<MeasurementType>(readerResMeas.GetValue(c => c.RESMEAS.TypeMeasurements), out outResType))
                        {
                            levelmeasurementResults.TypeMeasurements = outResType;
                        }
                        levelmeasurementResults.SensorName = readerResMeas.GetValue(c => c.RESMEAS.SENSOR.Name);
                        levelmeasurementResults.SensorTechId = readerResMeas.GetValue(c => c.RESMEAS.SENSOR.TechId);


                        levelmeasurementResults.CurrentLon = readerResMeas.GetValue(c => c.Lon);
                        levelmeasurementResults.CurrentLat = readerResMeas.GetValue(c => c.Lat);


                        levelmeasurementResults.CountUnknownStationMeasurements = 0;
                        levelmeasurementResults.CountStationMeasurements = 0;
                        if (outResType == MeasurementType.MonitoringStations)
                        {
                            var builderResMeasStation = this._dataLayer.GetBuilder<MD.IResMeasStation>().From();
                            builderResMeasStation.Select(c => c.Id);
                            builderResMeasStation.Select(c => c.Status);
                            builderResMeasStation.Where(c => c.ResMeasId, ConditionOperator.Equal, readerResMeas.GetValue(c => c.RESMEAS.Id));
                            builderResMeasStation.Where(c => c.Status, ConditionOperator.Equal, Status.E.ToString());
                            builderResMeasStation.OrderByAsc(c => c.Id);
                            queryExecuter.Fetch(builderResMeasStation, readerResMeasStation =>
                            {
                                while (readerResMeasStation.Read())
                                {
                                    levelmeasurementResults.CountUnknownStationMeasurements++;
                                }
                                return true;
                            });

                           
                            builderResMeasStation = this._dataLayer.GetBuilder<MD.IResMeasStation>().From();
                            builderResMeasStation.Select(c => c.Id);
                            builderResMeasStation.Select(c => c.Status);
                            builderResMeasStation.Where(c => c.ResMeasId, ConditionOperator.Equal, readerResMeas.GetValue(c => c.RESMEAS.Id));
                            builderResMeasStation.Where(c => c.Status, ConditionOperator.NotEqual, Status.E.ToString());
                            builderResMeasStation.OrderByAsc(c => c.Id);
                            queryExecuter.Fetch(builderResMeasStation, readerResMeasStation =>
                            {
                                while (readerResMeasStation.Read())
                                {
                                    levelmeasurementResults.CountStationMeasurements++;
                                }
                                return true;
                            });



                            builderResMeasStation = this._dataLayer.GetBuilder<MD.IResMeasStation>().From();
                            builderResMeasStation.Select(c => c.Id);
                            builderResMeasStation.Select(c => c.Status);
                            builderResMeasStation.Where(c => c.ResMeasId, ConditionOperator.Equal, readerResMeas.GetValue(c => c.RESMEAS.Id));
                            builderResMeasStation.Where(c => c.Status, ConditionOperator.IsNull);
                            builderResMeasStation.OrderByAsc(c => c.Id);
                            queryExecuter.Fetch(builderResMeasStation, readerResMeasStation =>
                            {
                                while (readerResMeasStation.Read())
                                {
                                    levelmeasurementResults.CountStationMeasurements++;
                                }
                                return true;
                            });
                        }
                        listlevelmeasurementResults.Add(levelmeasurementResults);
                    }
                    return true;
                });
            }
            catch (Exception e)
            {
                this._logger.Exception(Contexts.ThisComponent, e);
            }
            return listlevelmeasurementResults.ToArray();
        }

        /// <summary>
        /// Обрезание графика refLevelesValues.levels когда много значений
        /// </summary>
        /// <param name="refLevelesValues"></param>
        /// <param name="StartFrequency_Hz"></param>
        /// <param name="StopFrequency_Hz"></param>
        /// <returns></returns>
        private static ReferenceLevels ReferenceLevelsCut(ReferenceLevels refLevelesValues, double? StartFrequency_Hz = null, double? StopFrequency_Hz = null, int Npoint = 5000)
        { // НЕ ТЕСТИРОВАННО
            if (refLevelesValues == null) { return null;}
            if (refLevelesValues.levels.Length < Npoint) { return refLevelesValues;}
            if (StartFrequency_Hz > StopFrequency_Hz) { double k = StartFrequency_Hz.Value; StartFrequency_Hz = StopFrequency_Hz; StopFrequency_Hz = k;}
            int StartIndex;
            int StopIndex;
            if ((StartFrequency_Hz is null)  || (StopFrequency_Hz is null))
            {
                StartFrequency_Hz = refLevelesValues.StartFrequency_Hz;
                StopFrequency_Hz = refLevelesValues.StartFrequency_Hz + refLevelesValues.StepFrequency_Hz * (refLevelesValues.levels.Length-1);
                StartIndex = 0;
                StopIndex = refLevelesValues.levels.Length - 1;
            }
            else
            {
                // определение начала 
                if (refLevelesValues.StartFrequency_Hz >= StartFrequency_Hz)
                {
                    StartFrequency_Hz = refLevelesValues.StartFrequency_Hz;
                    StartIndex = 0;
                }
                else if ((refLevelesValues.StartFrequency_Hz + refLevelesValues.StepFrequency_Hz * (refLevelesValues.levels.Length - 1)) >= StartFrequency_Hz)
                {
                    StartIndex = (int)Math.Floor((StartFrequency_Hz.Value - refLevelesValues.StartFrequency_Hz) / refLevelesValues.StepFrequency_Hz);
                    StartFrequency_Hz = refLevelesValues.StartFrequency_Hz + refLevelesValues.levels[StartIndex] * refLevelesValues.StepFrequency_Hz;
                }
                else { return null; }
                // определение конеца 
                if ((refLevelesValues.StartFrequency_Hz + refLevelesValues.StepFrequency_Hz * (refLevelesValues.levels.Length - 1)) <= StopFrequency_Hz)
                {
                    StopFrequency_Hz = refLevelesValues.StartFrequency_Hz + refLevelesValues.StepFrequency_Hz * (refLevelesValues.levels.Length - 1);
                    StopIndex = refLevelesValues.levels.Length - 1;
                }
                else if (refLevelesValues.StartFrequency_Hz < StopFrequency_Hz)
                {
                    StopIndex = (int)Math.Ceiling((StopFrequency_Hz.Value - refLevelesValues.StartFrequency_Hz) / refLevelesValues.StepFrequency_Hz);
                    if (StopIndex > refLevelesValues.levels.Length-1) { StopIndex = refLevelesValues.levels.Length - 1; }
                    StartFrequency_Hz = refLevelesValues.StartFrequency_Hz + refLevelesValues.levels[StopIndex] * refLevelesValues.StepFrequency_Hz;
                }
                else { return null; }
            }
            if (StopIndex - StartIndex < Npoint)
            {
                // возвращаем просто усеченное значение
                var referenceLevels = new ReferenceLevels();
                referenceLevels.StartFrequency_Hz = StartFrequency_Hz.Value;
                referenceLevels.StepFrequency_Hz = refLevelesValues.StepFrequency_Hz;
                referenceLevels.levels = new float[StopIndex - StartIndex + 1];
                Array.Copy(refLevelesValues.levels, StartIndex, referenceLevels.levels, 0, StopIndex - StartIndex + 1);
                return referenceLevels;
            }
            else
            {
                // надо сжимать определяем коэфициент сжатия
                int k = (int)Math.Ceiling((double)(StopIndex - StartIndex) / Npoint);
                var referenceLevels = new ReferenceLevels();
                referenceLevels.StartFrequency_Hz = StartFrequency_Hz.Value;
                referenceLevels.StepFrequency_Hz = refLevelesValues.StepFrequency_Hz * k;
                int Arr_Count = (int)Math.Ceiling((double)(StopIndex - StartIndex+1) / k);
                referenceLevels.levels = new float[Arr_Count];
                for (int i = 0; Arr_Count-1 > i; i++)
                {
                    referenceLevels.levels[i] = refLevelesValues.levels[i*k];
                    for (int j = 0; k > j; j++)
                    {
                        if (refLevelesValues.levels[i*k+j] > referenceLevels.levels[i]) { referenceLevels.levels[i] = refLevelesValues.levels[i * k + j];}
                    }
                }
                if (refLevelesValues.levels.Length - 1 >= (Arr_Count - 1) * k)
                {
                    referenceLevels.levels[Arr_Count-1] = refLevelesValues.levels[(Arr_Count-1) * k];
                    for (int j = 0; k > j; j++)
                    {
                        if (refLevelesValues.levels.Length - 1 >= (Arr_Count - 1) * k + j)
                        {
                            if (refLevelesValues.levels[(Arr_Count - 1) * k + j] > referenceLevels.levels[(Arr_Count - 1)])
                            { referenceLevels.levels[(Arr_Count - 1)] = refLevelesValues.levels[(Arr_Count - 1) * k + j]; }
                        }
                    }
                }
                return referenceLevels;
            }
        }


    }
}




