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
       

        public void GetEmittingAndReferenceLevels(long resId, bool isLoadAllData, out Emitting[] emittings, out ReferenceLevels referenceLevels, double? StartFrequency_Hz = null, double? StopFrequency_Hz = null)
        {
            emittings = null;
            var listIdsEmittings = new List<long>();
            var listEmitings = new List<KeyValuePair<long, Emitting>>();
            var listWorkTimes = new List<KeyValuePair<long, WorkTime>>();
            var listSpectrum = new List<KeyValuePair<long, Spectrum>>();
            referenceLevels = new ReferenceLevels();
            var queryExecuter = this._dataLayer.Executor<SdrnServerDataContext>();
            var listSensors = new List<Sensor>();
            var listEmitting = new List<Emitting>();
            var queryEmitting = this._dataLayer.GetBuilder<MD.IEmitting>()
            .From()
            .Select(c => c.Id, c => c.CurentPower_dBm, c => c.MeanDeviationFromReference, c => c.ReferenceLevel_dBm, c => c.RollOffFactor, c => c.StandardBW, c => c.StartFrequency_MHz, c => c.StopFrequency_MHz, c => c.TriggerDeviationFromReference, c => c.LevelsDistributionLvl, c => c.LevelsDistributionCount, c => c.SensorId, c => c.StationID, c => c.StationTableName, c => c.Loss_dB, c => c.Freq_kHz)
            .OrderByAsc(c => c.StartFrequency_MHz)
            .Where(c => c.RES_MEAS.Id, ConditionOperator.Equal, resId);
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


                    var levelDist = new LevelsDistribution();
                    if (reader.GetValue(c => c.LevelsDistributionLvl) != null)
                    {
                        levelDist.Levels = reader.GetValue(c => c.LevelsDistributionLvl);
                    }
                    if (reader.GetValue(c => c.LevelsDistributionCount) != null)
                    {
                        levelDist.Count = reader.GetValue(c => c.LevelsDistributionCount);
                    }
                   
                    emitting.LevelsDistribution = levelDist;
                    emitting.EmittingParameters = emittingParam;

                    var signalMask = new SignalMask();
                    if (reader.GetValue(c => c.Loss_dB) != null)
                    {
                        signalMask.Loss_dB = reader.GetValue(c => c.Loss_dB);
                    }
                    if (reader.GetValue(c => c.Freq_kHz) != null)
                    {
                        signalMask.Freq_kHz = reader.GetValue(c => c.Freq_kHz);
                    }
                    emitting.SignalMask = signalMask;
                    listIdsEmittings.Add(reader.GetValue(c => c.Id));
                    listEmitings.Add(new KeyValuePair<long, Emitting>(reader.GetValue(c => c.Id), emitting));
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
                            workTime.StartEmitting = readerTime.GetValue(c => c.StartEmitting);
                            workTime.StopEmitting = readerTime.GetValue(c => c.StopEmitting);
                            workTime.HitCount = readerTime.GetValue(c => c.HitCount);
                            workTime.PersentAvailability = readerTime.GetValue(c => c.PersentAvailability);

                        listWorkTimes.Add(new KeyValuePair<long, WorkTime>(readerTime.GetValue(c => c.EMITTING.Id), workTime));
                    }
                    return true;
                });
            }



            var listIntEmittingSpectrum = BreakDownElemBlocks.BreakDown(listIdsEmittings.ToArray());
            for (int i = 0; i < listIntEmittingSpectrum.Count; i++)
            {
                var querySpectrum = this._dataLayer.GetBuilder<MD.ISpectrum>()
                       .From()
                       .Select(c => c.Id, c => c.Levels_dBm, c => c.SpectrumStartFreq_MHz, c => c.SpectrumSteps_kHz, c => c.Bandwidth_kHz, c => c.TraceCount, c => c.SignalLevel_dBm, c => c.MarkerIndex, c => c.CorrectnessEstimations, c => c.T1, c => c.T2, c => c.EMITTING.Id, c => c.Contravention)
                       .Where(c => c.EMITTING.Id, ConditionOperator.In, listIntEmittingSpectrum[i]);
                queryExecuter.Fetch(querySpectrum, readerSpectrum =>
                {
                    while (readerSpectrum.Read())
                    {
                        var spectrum = new Spectrum();
                        if (readerSpectrum.GetValue(c => c.Levels_dBm) != null)
                        {
                            spectrum.Levels_dBm = readerSpectrum.GetValue(c => c.Levels_dBm);
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
                        listSpectrum.Add(new KeyValuePair<long, Spectrum>(readerSpectrum.GetValue(c => c.EMITTING.Id), spectrum));
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
                                if (x.Value.Levels_dBm != null)
                                {
                                    listLevelsdBm.AddRange(x.Value.Levels_dBm);
                                    emitting.Spectrum = x.Value;
                                }
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

        public SignalingSysInfo[] GetSignalingSysInfos(long measResultId, double freq_MHz)
        {
            var queryExecuter = this._dataLayer.Executor<SdrnServerDataContext>();
            var listSysInfo = new List<SignalingSysInfo>();
            var querySysInfo = this._dataLayer.GetBuilder<MD.ISignalingSysInfo>().From()
                .Select(c => c.Id, c => c.BandWidth_Hz, c => c.BSIC, c => c.ChannelNumber, c => c.CID, c => c.CtoI, c => c.Freq_Hz,
                        c => c.LAC, c => c.Level_dBm, c => c.MCC, c => c.MNC, c => c.Power, c => c.RNC, c => c.Standard)
                .Where(c => c.EMITTING.RES_MEAS.Id, ConditionOperator.Equal, measResultId)
                .Where(c => c.EMITTING.StartFrequency_MHz, ConditionOperator.LessThan, freq_MHz)
                .Where(c => c.EMITTING.StopFrequency_MHz, ConditionOperator.GreaterThan, freq_MHz);
            queryExecuter.Fetch(querySysInfo, reader =>
            {
                while (reader.Read())
                {
                    var sysInfo = new SignalingSysInfo();
                    long sysInfoId = reader.GetValue(c => c.Id);
                    sysInfo.BandWidth_Hz = reader.GetValue(c => c.BandWidth_Hz);
                    sysInfo.BSIC = reader.GetValue(c => c.BSIC);
                    sysInfo.ChannelNumber = reader.GetValue(c => c.ChannelNumber);
                    sysInfo.CID = reader.GetValue(c => c.CID);
                    sysInfo.CtoI = reader.GetValue(c => c.CtoI);
                    sysInfo.Freq_Hz = reader.GetValue(c => c.Freq_Hz);
                    sysInfo.LAC = reader.GetValue(c => c.LAC);
                    sysInfo.Level_dBm = reader.GetValue(c => c.Level_dBm);
                    sysInfo.MCC = reader.GetValue(c => c.MCC);
                    sysInfo.MNC = reader.GetValue(c => c.MNC);
                    sysInfo.Power = reader.GetValue(c => c.Power);
                    sysInfo.RNC = reader.GetValue(c => c.RNC);
                    sysInfo.Standart = reader.GetValue(c => c.Standard);

                    var listWorkTimes = new List<WorkTime>();
                    var queryWorkTime = this._dataLayer.GetBuilder<MD.ISignalingSysInfoWorkTime>().From()
                        .Select(c => c.Id, c => c.HitCount, c => c.PersentAvailability, c => c.StartEmitting, c => c.StopEmitting)
                        .Where(c => c.SYSINFO.Id, ConditionOperator.Equal, sysInfoId);

                    queryExecuter.Fetch(queryWorkTime, readerWorkTime =>
                    {
                        while (readerWorkTime.Read())
                        {
                            var workTime = new WorkTime();
                            workTime.HitCount = readerWorkTime.GetValue(c => c.HitCount);
                            workTime.PersentAvailability = readerWorkTime.GetValue(c => c.PersentAvailability);
                            workTime.StartEmitting = readerWorkTime.GetValue(c => c.StartEmitting);
                            workTime.StopEmitting = readerWorkTime.GetValue(c => c.StopEmitting);
                            listWorkTimes.Add(workTime);
                        }
                        return true;
                    });
                    sysInfo.WorkTimes = listWorkTimes.ToArray();
                    listSysInfo.Add(sysInfo);
                }
                return true;
            });
            return listSysInfo.ToArray();
        }
        public Emitting[] GetEmittingsByIcsmId(long[] ids, string icsmTableName)
        {
            var listIdsEmittings = new List<long>();
            var listEmitings = new List<KeyValuePair<long, Emitting>>();
            var listWorkTimes = new List<KeyValuePair<long, WorkTime>>();
            var listSpectrum = new List<KeyValuePair<long, Spectrum>>();
            var listSensors = new List<Sensor>();
            var listEmitting = new List<Emitting>();
            var listIntids = BreakDownElemBlocks.BreakDown(ids);
            var arrIntids = listIntids.ToArray();
            var queryExecuter = this._dataLayer.Executor<SdrnServerDataContext>();
            for (int l = 0; l < arrIntids.Length; l++)
            {
                long?[] listIntIdsConvert = listIntids[l].Select(n => (long?)(n)).ToArray();
                var queryEmitting = this._dataLayer.GetBuilder<MD.IEmitting>()
            .From()
            .Select(c => c.Id, c => c.CurentPower_dBm, c => c.MeanDeviationFromReference, c => c.ReferenceLevel_dBm, c => c.RollOffFactor, c => c.StandardBW, c => c.StartFrequency_MHz, c => c.StopFrequency_MHz, c => c.TriggerDeviationFromReference, c => c.LevelsDistributionLvl, c => c.LevelsDistributionCount, c => c.SensorId, c => c.StationID, c => c.StationTableName, c => c.Loss_dB, c => c.Freq_kHz)
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

                    var levelDist = new LevelsDistribution();

                    if (reader.GetValue(c => c.LevelsDistributionLvl) != null)
                    {
                        levelDist.Levels = reader.GetValue(c => c.LevelsDistributionLvl);
                    }

                    if (reader.GetValue(c => c.LevelsDistributionCount) != null)
                    {
                        levelDist.Count = reader.GetValue(c => c.LevelsDistributionCount);
                    }

                    emitting.LevelsDistribution = levelDist;
                    emitting.EmittingParameters = emittingParam;

                    var signalMask = new SignalMask();
                    if (reader.GetValue(c => c.Loss_dB) != null)
                    {
                        signalMask.Loss_dB = reader.GetValue(c => c.Loss_dB);
                    }
                    if (reader.GetValue(c => c.Freq_kHz) != null)
                    {
                        signalMask.Freq_kHz = reader.GetValue(c => c.Freq_kHz);
                    }
                    emitting.SignalMask = signalMask;

                    listIdsEmittings.Add(reader.GetValue(c => c.Id));
                    listEmitings.Add(new KeyValuePair<long, Emitting>(reader.GetValue(c => c.Id), emitting));
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
                            workTime.StartEmitting = readerTime.GetValue(c => c.StartEmitting);
                            workTime.StopEmitting = readerTime.GetValue(c => c.StopEmitting);
                            workTime.HitCount = readerTime.GetValue(c => c.HitCount);
                            workTime.PersentAvailability = readerTime.GetValue(c => c.PersentAvailability);

                            listWorkTimes.Add(new KeyValuePair<long, WorkTime>(readerTime.GetValue(c => c.EMITTING.Id), workTime));
                        }
                        return true;
                    });
                }



                var listIntEmittingSpectrum = BreakDownElemBlocks.BreakDown(listIdsEmittings.ToArray());
                for (int i = 0; i < listIntEmittingSpectrum.Count; i++)
                {
                    var querySpectrum = this._dataLayer.GetBuilder<MD.ISpectrum>()
                           .From()
                           .Select(c => c.Id, c => c.Levels_dBm, c => c.SpectrumStartFreq_MHz, c => c.SpectrumSteps_kHz, c => c.Bandwidth_kHz, c => c.TraceCount, c => c.SignalLevel_dBm, c => c.MarkerIndex, c => c.CorrectnessEstimations, c => c.T1, c => c.T2, c => c.EMITTING.Id, c => c.Contravention)
                           .Where(c => c.EMITTING.Id, ConditionOperator.In, listIntEmittingSpectrum[i]);
                    queryExecuter.Fetch(querySpectrum, readerSpectrum =>
                    {
                        while (readerSpectrum.Read())
                        {
                            var spectrum = new Spectrum();

                            if (readerSpectrum.GetValue(c => c.Levels_dBm) != null)
                            {
                                spectrum.Levels_dBm = readerSpectrum.GetValue(c => c.Levels_dBm);
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
                           
                            listSpectrum.Add(new KeyValuePair<long, Spectrum>(readerSpectrum.GetValue(c => c.EMITTING.Id), spectrum));
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
                                    if (x.Value.Levels_dBm != null)
                                    {
                                        listLevelsdBm.AddRange(x.Value.Levels_dBm);
                                        emitting.Spectrum = x.Value;
                                    }
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

        public ReferenceLevels GetReferenceLevelsByResId(long resId, bool isLoadAllData, double? StartFrequency_Hz = null, double? StopFrequency_Hz = null)
        {
            var queryExecuter = this._dataLayer.Executor<SdrnServerDataContext>();
            var referenceLevels = new ReferenceLevels();
            var level = new ReferenceLevels();
            long? subMeasTaskStaId = null;


           var queryMeasTaskId = this._dataLayer.GetBuilder<MD.IReferenceLevels>()
          .From()
          .Select(c => c.Id, c => c.RES_MEAS.SUBTASK_SENSOR.Id)
          .Where(c => c.RES_MEAS.Id, ConditionOperator.Equal, resId);
            queryExecuter.Fetch(queryMeasTaskId, readerReferenceLevels =>
            {
                if (readerReferenceLevels.Read())
                {
                    subMeasTaskStaId = readerReferenceLevels.GetValue(c => c.RES_MEAS.SUBTASK_SENSOR.Id);
                }
                return true;
            });
            if (subMeasTaskStaId != null)
            {
                var queryLevels = this._dataLayer.GetBuilder<MD.IReferenceLevels>()
               .From()
               .Select(c => c.Id, c => c.StartFrequency_Hz, c => c.StepFrequency_Hz, c => c.RefLevels)
               .Where(c => c.RES_MEAS.SUBTASK_SENSOR.Id, ConditionOperator.Equal, subMeasTaskStaId);
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

                        if (readerLevels.GetValue(c => c.RefLevels) != null)
                        {
                            level.levels = readerLevels.GetValue(c => c.RefLevels);
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

        public ReferenceLevels GetReferenceLevelsByResultId(long resId, bool isLoadAllData, double? StartFrequency_Hz = null, double? StopFrequency_Hz = null)
        {
            var queryExecuter = this._dataLayer.Executor<SdrnServerDataContext>();
            var level = new ReferenceLevels();
            long? subMeasTaskStaId = null;


            var queryMeasTaskId = this._dataLayer.GetBuilder<MD.IReferenceLevels>()
          .From()
          .Select(c => c.Id, c => c.RES_MEAS.SUBTASK_SENSOR.Id)
          .Where(c => c.RES_MEAS.Id, ConditionOperator.Equal, resId);
            queryExecuter.Fetch(queryMeasTaskId, readerReferenceLevels =>
            {
                if (readerReferenceLevels.Read())
                {
                    subMeasTaskStaId = readerReferenceLevels.GetValue(c => c.RES_MEAS.SUBTASK_SENSOR.Id);
                }
                return true;
            });
            if (subMeasTaskStaId != null)
            {
                var queryLevels = this._dataLayer.GetBuilder<MD.IReferenceLevels>()
               .From()
               .Select(c => c.Id, c => c.StartFrequency_Hz, c => c.StepFrequency_Hz, c => c.RefLevels)
               .Where(c => c.RES_MEAS.SUBTASK_SENSOR.Id, ConditionOperator.Equal, subMeasTaskStaId);
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

                        if (readerLevels.GetValue(c => c.RefLevels) != null)
                        {
                            level.levels = readerLevels.GetValue(c => c.RefLevels);
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
                builderResMeas.Select(c => c.SUBTASK_SENSOR.Id);
                builderResMeas.Select(c => c.SUBTASK_SENSOR.SUBTASK.MEAS_TASK.Id);
                builderResMeas.Select(c => c.SUBTASK_SENSOR.SENSOR.Id);
                builderResMeas.Select(c => c.SUBTASK_SENSOR.SUBTASK.Id);
                builderResMeas.Select(c => c.N);
                builderResMeas.Select(c => c.ScansNumber);
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
                        levelmeasurementResults.Status = readerResMeas.GetValue(c => c.Status);
                        levelmeasurementResults.Id = new MeasurementResultsIdentifier();
                        levelmeasurementResults.Id.MeasTaskId = new MeasTaskIdentifier();
                        levelmeasurementResults.Id.MeasSdrResultsId = readerResMeas.GetValue(c => c.Id);
                        try
                        {
                            
                            levelmeasurementResults.Id.MeasTaskId.Value = readerResMeas.GetValue(c => c.SUBTASK_SENSOR.SUBTASK.MEAS_TASK.Id);
                            levelmeasurementResults.StationMeasurements = new StationMeasurements();
                            levelmeasurementResults.StationMeasurements.StationId = new SensorIdentifier();
                            levelmeasurementResults.StationMeasurements.StationId.Value = readerResMeas.GetValue(c => c.SUBTASK_SENSOR.SENSOR.Id);
                            levelmeasurementResults.Id.SubMeasTaskId = readerResMeas.GetValue(c => c.SUBTASK_SENSOR.SUBTASK.Id);
                            levelmeasurementResults.Id.SubMeasTaskStationId = readerResMeas.GetValue(c => c.SUBTASK_SENSOR.Id);
                        }
                        catch (Exception e)
                        {
                            this._logger.Exception(Contexts.ThisComponent, e);
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


                    /// Location
                    var listLocationSensorMeasurement = new List<LocationSensorMeasurement>();
                        var builderResLocSensorMeas = this._dataLayer.GetBuilder<MD.IResLocSensorMeas>().From();
                        builderResLocSensorMeas.Select(c => c.Agl);
                        builderResLocSensorMeas.Select(c => c.Asl);
                        builderResLocSensorMeas.Select(c => c.Id);
                        builderResLocSensorMeas.Select(c => c.Lat);
                        builderResLocSensorMeas.Select(c => c.Lon);
                        builderResLocSensorMeas.Select(c => c.RES_MEAS.Id);
                        builderResLocSensorMeas.OrderByAsc(c => c.Id);
                        builderResLocSensorMeas.Where(c => c.RES_MEAS.Id, ConditionOperator.Equal, readerResMeas.GetValue(c => c.Id));
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
                            builderResMeasStation.Where(c => c.RES_MEAS.Id, ConditionOperator.Equal, readerResMeas.GetValue(c => c.Id));
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
                            builderResMeasStation.Where(c => c.RES_MEAS.Id, ConditionOperator.Equal, readerResMeas.GetValue(c => c.Id));
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
                            builderResMeasStation.Where(c => c.RES_MEAS.Id, ConditionOperator.Equal, readerResMeas.GetValue(c => c.Id));
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
                        builderLinkResSensoT.Select(c => c.SENSOR.Id);
                        builderLinkResSensoT.Select(c => c.SENSOR.Name);
                        builderLinkResSensoT.Select(c => c.SENSOR.TechId);
                        builderLinkResSensoT.Where(c => c.RES_MEAS_STATION.RES_MEAS.Id, ConditionOperator.Equal, readerResMeas.GetValue(c => c.Id));
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
                    builderResMeas.Select(c => c.RES_MEAS.AntVal);
                    builderResMeas.Select(c => c.RES_MEAS.DataRank);
                    builderResMeas.Select(c => c.RES_MEAS.Id);
                    builderResMeas.Select(c => c.Lon);
                    builderResMeas.Select(c => c.Lat);
                    builderResMeas.Select(c => c.RES_MEAS.MeasResultSID);
                    builderResMeas.Select(c => c.RES_MEAS.SUBTASK_SENSOR.Id);
                    builderResMeas.Select(c => c.RES_MEAS.SUBTASK_SENSOR.SUBTASK.MEAS_TASK.Id);
                    builderResMeas.Select(c => c.RES_MEAS.SUBTASK_SENSOR.SENSOR.Id);
                    builderResMeas.Select(c => c.RES_MEAS.SUBTASK_SENSOR.SUBTASK.Id);
                    builderResMeas.Select(c => c.RES_MEAS.N);
                    builderResMeas.Select(c => c.RES_MEAS.ScansNumber);
                    builderResMeas.Select(c => c.RES_MEAS.StartTime);
                    builderResMeas.Select(c => c.RES_MEAS.Status);
                    builderResMeas.Select(c => c.RES_MEAS.StopTime);
                    builderResMeas.Select(c => c.RES_MEAS.Synchronized);
                    builderResMeas.Select(c => c.RES_MEAS.TimeMeas);
                    builderResMeas.Select(c => c.RES_MEAS.TypeMeasurements);
                    builderResMeas.Select(c => c.RES_MEAS.SUBTASK_SENSOR.SENSOR.Name);
                    builderResMeas.Select(c => c.RES_MEAS.SUBTASK_SENSOR.SENSOR.TechId);

                    builderResMeas.OrderByAsc(c => c.Id);
                    if (measResultsId.MeasSdrResultsId > 0)
                    {
                        builderResMeas.Where(c => c.RES_MEAS.Id, ConditionOperator.Equal, measResultsId.MeasSdrResultsId);
                    }
                   
                    if ((measResultsId.SubMeasTaskStationId > 0))
                    {
                        builderResMeas.Where(c => c.RES_MEAS.SUBTASK_SENSOR.Id, ConditionOperator.Equal, measResultsId.SubMeasTaskStationId);
                    }
                    builderResMeas.Where(c => c.RES_MEAS.Status, ConditionOperator.NotEqual, Status.Z.ToString());

                    queryExecuter.Fetch(builderResMeas, readerResMeas =>
                    {
                        while (readerResMeas.Read())
                        {

                            levelmeasurementResults.DataRank = readerResMeas.GetValue(c => c.RES_MEAS.DataRank);
                            levelmeasurementResults.Number = readerResMeas.GetValue(c => c.RES_MEAS.N).HasValue ? readerResMeas.GetValue(c => c.RES_MEAS.N).Value : -1;
                            levelmeasurementResults.Status = readerResMeas.GetValue(c => c.RES_MEAS.Status);
                            levelmeasurementResults.Id = new MeasurementResultsIdentifier();
                            levelmeasurementResults.Id.MeasTaskId = new MeasTaskIdentifier();
                            levelmeasurementResults.Id.MeasSdrResultsId = readerResMeas.GetValue(c => c.RES_MEAS.Id);
                            try
                            {
                                
                                levelmeasurementResults.Id.MeasTaskId.Value = readerResMeas.GetValue(c => c.RES_MEAS.SUBTASK_SENSOR.SUBTASK.MEAS_TASK.Id);
                                levelmeasurementResults.Id.SubMeasTaskId = readerResMeas.GetValue(c => c.RES_MEAS.SUBTASK_SENSOR.SUBTASK.Id);
                                levelmeasurementResults.Id.SubMeasTaskStationId = readerResMeas.GetValue(c => c.RES_MEAS.SUBTASK_SENSOR.Id);
                                levelmeasurementResults.SensorName = readerResMeas.GetValue(c => c.RES_MEAS.SUBTASK_SENSOR.SENSOR.Name);
                                levelmeasurementResults.SensorTechId = readerResMeas.GetValue(c => c.RES_MEAS.SUBTASK_SENSOR.SENSOR.TechId);
                            }
                            catch (Exception e)
                            {
                                this._logger.Exception(Contexts.ThisComponent, e);
                            }


                            if (readerResMeas.GetValue(c => c.RES_MEAS.TimeMeas) != null)
                            {
                                levelmeasurementResults.TimeMeas = readerResMeas.GetValue(c => c.RES_MEAS.TimeMeas).Value;
                            }
                            if (readerResMeas.GetValue(c => c.RES_MEAS.StartTime) != null)
                            {
                                levelmeasurementResults.StartTime = readerResMeas.GetValue(c => c.RES_MEAS.StartTime).Value;
                            }
                            if (readerResMeas.GetValue(c => c.RES_MEAS.StopTime) != null)
                            {
                                levelmeasurementResults.StopTime = readerResMeas.GetValue(c => c.RES_MEAS.StopTime).Value;
                            }
                            MeasurementType outResType;
                            if (Enum.TryParse<MeasurementType>(readerResMeas.GetValue(c => c.RES_MEAS.TypeMeasurements), out outResType))
                            {
                                levelmeasurementResults.TypeMeasurements = outResType;
                            }
                            


                            levelmeasurementResults.CurrentLon = readerResMeas.GetValue(c => c.Lon);
                            levelmeasurementResults.CurrentLat = readerResMeas.GetValue(c => c.Lat);


                            levelmeasurementResults.CountUnknownStationMeasurements = 0;
                            levelmeasurementResults.CountStationMeasurements = 0;
                            if (outResType == MeasurementType.MonitoringStations)
                            {
                                var builderResMeasStation = this._dataLayer.GetBuilder<MD.IResMeasStation>().From();
                                builderResMeasStation.Select(c => c.Id);
                                builderResMeasStation.Select(c => c.Status);
                                builderResMeasStation.Where(c => c.RES_MEAS.Id, ConditionOperator.Equal, readerResMeas.GetValue(c => c.RES_MEAS.Id));
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
                                builderResMeasStation.Where(c => c.RES_MEAS.Id, ConditionOperator.Equal, readerResMeas.GetValue(c => c.RES_MEAS.Id));
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
                                builderResMeasStation.Where(c => c.RES_MEAS.Id, ConditionOperator.Equal, readerResMeas.GetValue(c => c.RES_MEAS.Id));
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
                builderResMeas.Select(c => c.RES_MEAS.AntVal);
                builderResMeas.Select(c => c.RES_MEAS.DataRank);
                builderResMeas.Select(c => c.RES_MEAS.Id);
                builderResMeas.Select(c => c.Lon);
                builderResMeas.Select(c => c.Lat);
                builderResMeas.Select(c => c.RES_MEAS.MeasResultSID);
                builderResMeas.Select(c => c.RES_MEAS.SUBTASK_SENSOR.Id);
                builderResMeas.Select(c => c.RES_MEAS.SUBTASK_SENSOR.SUBTASK.MEAS_TASK.Id);
                builderResMeas.Select(c => c.RES_MEAS.SUBTASK_SENSOR.SENSOR.Id);
                builderResMeas.Select(c => c.RES_MEAS.SUBTASK_SENSOR.SUBTASK.Id);
                builderResMeas.Select(c => c.RES_MEAS.N);
                builderResMeas.Select(c => c.RES_MEAS.ScansNumber);
                
                builderResMeas.Select(c => c.RES_MEAS.StartTime);
                builderResMeas.Select(c => c.RES_MEAS.Status);
                builderResMeas.Select(c => c.RES_MEAS.StopTime);
                builderResMeas.Select(c => c.RES_MEAS.Synchronized);
                builderResMeas.Select(c => c.RES_MEAS.TimeMeas);
                builderResMeas.Select(c => c.RES_MEAS.TypeMeasurements);
                builderResMeas.Select(c => c.RES_MEAS.SUBTASK_SENSOR.SENSOR.Name);
                builderResMeas.Select(c => c.RES_MEAS.SUBTASK_SENSOR.SENSOR.TechId);
                builderResMeas.OrderByAsc(c => c.Id);
                builderResMeas.Where(c => c.RES_MEAS.TimeMeas, ConditionOperator.GreaterEqual, constraint.Start);
                builderResMeas.Where(c => c.RES_MEAS.TimeMeas, ConditionOperator.LessEqual, constraint.End);
                builderResMeas.Where(c => c.RES_MEAS.Status, ConditionOperator.NotEqual, Status.Z.ToString());
                queryExecuter.Fetch(builderResMeas, readerResMeas =>
                {
                    while (readerResMeas.Read())
                    {
                        var levelmeasurementResults = new ShortMeasurementResults();
                        levelmeasurementResults.DataRank = readerResMeas.GetValue(c => c.RES_MEAS.DataRank);
                        levelmeasurementResults.Number = readerResMeas.GetValue(c => c.RES_MEAS.N).HasValue ? readerResMeas.GetValue(c => c.RES_MEAS.N).Value : -1;
                        levelmeasurementResults.Status = readerResMeas.GetValue(c => c.RES_MEAS.Status);
                        levelmeasurementResults.Id = new MeasurementResultsIdentifier();
                        levelmeasurementResults.Id.MeasTaskId = new MeasTaskIdentifier();
                        levelmeasurementResults.Id.MeasSdrResultsId = readerResMeas.GetValue(c => c.RES_MEAS.Id);
                        try
                        {
                            
                            levelmeasurementResults.Id.MeasTaskId.Value = readerResMeas.GetValue(c => c.RES_MEAS.SUBTASK_SENSOR.SUBTASK.MEAS_TASK.Id);
                            levelmeasurementResults.Id.SubMeasTaskId = readerResMeas.GetValue(c => c.RES_MEAS.SUBTASK_SENSOR.SUBTASK.Id);
                            levelmeasurementResults.Id.SubMeasTaskStationId = readerResMeas.GetValue(c => c.RES_MEAS.SUBTASK_SENSOR.Id);
                            levelmeasurementResults.SensorName = readerResMeas.GetValue(c => c.RES_MEAS.SUBTASK_SENSOR.SENSOR.Name);
                            levelmeasurementResults.SensorTechId = readerResMeas.GetValue(c => c.RES_MEAS.SUBTASK_SENSOR.SENSOR.TechId);
                        }
                        catch (Exception e)
                        {
                            this._logger.Exception(Contexts.ThisComponent, e);
                        }

                        if (readerResMeas.GetValue(c => c.RES_MEAS.TimeMeas) != null)
                        {
                            levelmeasurementResults.TimeMeas = readerResMeas.GetValue(c => c.RES_MEAS.TimeMeas).Value;
                        }
                        if (readerResMeas.GetValue(c => c.RES_MEAS.StartTime) != null)
                        {
                            levelmeasurementResults.StartTime = readerResMeas.GetValue(c => c.RES_MEAS.StartTime).Value;
                        }
                        if (readerResMeas.GetValue(c => c.RES_MEAS.StopTime) != null)
                        {
                            levelmeasurementResults.StopTime = readerResMeas.GetValue(c => c.RES_MEAS.StopTime).Value;
                        }
                        MeasurementType outResType;
                        if (Enum.TryParse<MeasurementType>(readerResMeas.GetValue(c => c.RES_MEAS.TypeMeasurements), out outResType))
                        {
                            levelmeasurementResults.TypeMeasurements = outResType;
                        }
                        


                        levelmeasurementResults.CurrentLon = readerResMeas.GetValue(c => c.Lon);
                        levelmeasurementResults.CurrentLat = readerResMeas.GetValue(c => c.Lat);


                        levelmeasurementResults.CountUnknownStationMeasurements = 0;
                        levelmeasurementResults.CountStationMeasurements = 0;
                        if (outResType == MeasurementType.MonitoringStations)
                        {
                            var builderResMeasStation = this._dataLayer.GetBuilder<MD.IResMeasStation>().From();
                            builderResMeasStation.Select(c => c.Id);
                            builderResMeasStation.Select(c => c.Status);
                            builderResMeasStation.Where(c => c.RES_MEAS.Id, ConditionOperator.Equal, readerResMeas.GetValue(c => c.RES_MEAS.Id));
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
                            builderResMeasStation.Where(c => c.RES_MEAS.Id, ConditionOperator.Equal, readerResMeas.GetValue(c => c.RES_MEAS.Id));
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
                            builderResMeasStation.Where(c => c.RES_MEAS.Id, ConditionOperator.Equal, readerResMeas.GetValue(c => c.RES_MEAS.Id));
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
        public SensorPoligonPoint[] GetSensorPoligonPoint(long MeasResultsId)
        {
            var results = new List<SensorPoligonPoint>();
            try
            {

                var listSensorIds = new List<long?>();
                this._logger.Info(Contexts.ThisComponent, Categories.Processing, Events.HandlerCallGetSensorPoligonPointMethod.Text);
                var queryExecuter = this._dataLayer.Executor<SdrnServerDataContext>();
                var builderLinkResSensoT = this._dataLayer.GetBuilder<MD.ILinkResSensor>().From();
                builderLinkResSensoT.Select(c => c.Id);
                builderLinkResSensoT.Select(c => c.SENSOR.Id);
                builderLinkResSensoT.Select(c => c.SENSOR.Name);
                builderLinkResSensoT.Select(c => c.SENSOR.TechId);
                builderLinkResSensoT.Where(c => c.RES_MEAS_STATION.RES_MEAS.Id, ConditionOperator.Equal, MeasResultsId);
                builderLinkResSensoT.OrderByAsc(c => c.Id);
                queryExecuter.Fetch(builderLinkResSensoT, readerLinkResSensor =>
                {
                    while (readerLinkResSensor.Read())
                    {
                        long? sensorId = readerLinkResSensor.GetValue(c => c.SENSOR.Id);
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
                    for (int i =0; i< listIntEmittingSensorPolygon.Count; i++)
                    {
                        var builderSensorPolygon = this._dataLayer.GetBuilder<MD.ISensorPolygon>().From();
                        builderSensorPolygon.Select(c => c.Id);
                        builderSensorPolygon.Select(c => c.Lon);
                        builderSensorPolygon.Select(c => c.Lat);
                        builderSensorPolygon.Where(c => c.SENSOR.Id, ConditionOperator.In, listIntEmittingSensorPolygon[i]);
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
        public ResultsMeasurementsStation ReadResultResMeasStation(long StationId)
        {
            var resMeasStatiion = new ResultsMeasurementsStation();
            try
            {
                this._logger.Info(Contexts.ThisComponent, Categories.Processing, Events.HandlerCallReadResultResMeasStationMethod.Text);
                var queryExecuter = this._dataLayer.Executor<SdrnServerDataContext>();
                var builderResMeasStation = this._dataLayer.GetBuilder<MD.IResMeasStation>().From();
                builderResMeasStation.Select(c => c.GlobalSID);
                builderResMeasStation.Select(c => c.Id);
                builderResMeasStation.Select(c => c.MeasGlobalSID);
                builderResMeasStation.Select(c => c.RES_MEAS.Id);
                builderResMeasStation.Select(c => c.ClientSectorCode);
                builderResMeasStation.Select(c => c.Standard);
                builderResMeasStation.Select(c => c.Frequency);
                builderResMeasStation.Select(c => c.ClientStationCode);
                builderResMeasStation.Select(c => c.Status);
                builderResMeasStation.Where(c => c.Id, ConditionOperator.Equal, StationId);
                builderResMeasStation.OrderByAsc(c => c.Id);
                queryExecuter.Fetch(builderResMeasStation, readerResMeasStation =>
                {
                    while (readerResMeasStation.Read())
                    {
                        resMeasStatiion.IdSector = readerResMeasStation.GetValue(c => c.ClientSectorCode);
                        resMeasStatiion.GlobalSID = readerResMeasStation.GetValue(c => c.GlobalSID);
                        resMeasStatiion.MeasGlobalSID = readerResMeasStation.GetValue(c => c.MeasGlobalSID);
                        resMeasStatiion.Status = readerResMeasStation.GetValue(c => c.Status);
                        resMeasStatiion.Id = readerResMeasStation.GetValue(c => c.Id);
                        resMeasStatiion.Idstation = readerResMeasStation.GetValue(c => c.ClientStationCode).ToString();
                        resMeasStatiion.Standard = readerResMeasStation.GetValue(c => c.Standard);

                        double? rbw = null;
                        double? vbw = null;
                        double? bw = null;
                        double? centralFrequency =  readerResMeasStation.GetValue(c => c.Frequency);

                        var measurementsParameterGeneral = new MeasurementsParameterGeneral();
                        var builderResStGeneral = this._dataLayer.GetBuilder<MD.IResStGeneral>().From();
                        builderResStGeneral.Select(c => c.CentralFrequency);
                        builderResStGeneral.Select(c => c.CentralFrequencyMeas);
                        builderResStGeneral.Select(c => c.Correctnessestim);
                        builderResStGeneral.Select(c => c.DurationMeas);
                        builderResStGeneral.Select(c => c.Id);
                        builderResStGeneral.Select(c => c.MarkerIndex);
                        builderResStGeneral.Select(c => c.OffsetFrequency);
                        builderResStGeneral.Select(c => c.RES_MEAS_STATION.Id);
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
                        builderResStGeneral.Select(c => c.LevelsSpectrumdBm);
                        builderResStGeneral.Where(c => c.RES_MEAS_STATION.Id, ConditionOperator.Equal, readerResMeasStation.GetValue(c => c.Id));
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
                                bw = readerResStGeneral.GetValue(c => c.BW);

                                //centralFrequency = readerResStGeneral.GetValue(c => c.CentralFrequency);


                                var listMaskElements = new List<MaskElements>();
                                var builderResStMaskElement = this._dataLayer.GetBuilder<MD.IResStMaskElement>().From();
                                builderResStMaskElement.Select(c => c.Bw);
                                builderResStMaskElement.Select(c => c.Level);
                                builderResStMaskElement.Where(c => c.RES_STGENERAL.Id, ConditionOperator.Equal, readerResStGeneral.GetValue(c => c.Id));
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
                                measurementsParameterGeneral.LevelsSpecrum = readerResStGeneral.GetValue(c=>c.LevelsSpectrumdBm);


                                var valSysInfo = "";


                                var queryStationSysInfo = this._dataLayer.GetBuilder<MD.IResSysInfo>()
                               .From()
                               .Select(c => c.Id, c => c.Agl, c => c.Asl, c => c.Bandwidth, c => c.BaseId, c => c.Bsic, c => c.ChannelNumber, c => c.Cid, c => c.Code, c => c.Ctoi, c => c.Eci, c => c.Enodebid, c => c.Freq, c => c.Icio, c => c.InbandPower, c => c.Iscp, c => c.Lac)
                               .Select(c => c.Lat, c => c.Lon, c => c.Mcc, c => c.Mnc, c => c.Nid, c => c.Pci, c => c.Pn, c => c.Power, c => c.Ptotal, c => c.Rnc, c => c.Rscp, c => c.Rsrp, c => c.Rsrq, c => c.Sc, c => c.Sid, c => c.Tac, c => c.TypeCdmaevdo, c => c.Ucid)
                               .Where(c => c.RES_STGENERAL.Id, ConditionOperator.Equal, readerResStGeneral.GetValue(c => c.Id));
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
                                        .Select(c => c.Id, c => c.Data, c => c.Type)
                                        .Where(c => c.RES_SYS_INFO.Id, ConditionOperator.Equal, readerStationSysInfo.GetValue(c => c.Id));
                                        queryExecuter.Fetch(queryStationSysInfoBls, readerStationSysInfoBls =>
                                        {
                                            while (readerStationSysInfoBls.Read())
                                            {
                                                valSysInfo += string.Format("SysInfoBlocks.Data  : {0}", readerStationSysInfoBls.GetValue(c => c.Data)) + Environment.NewLine;
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
                        builderResStLevelCar.Select(c => c.DifferenceTimeStamp);
                        builderResStLevelCar.Select(c => c.Id);
                        builderResStLevelCar.Select(c => c.Lat);
                        builderResStLevelCar.Select(c => c.LevelDbm);
                        builderResStLevelCar.Select(c => c.LevelDbmkvm);
                        builderResStLevelCar.Select(c => c.Lon);
                        builderResStLevelCar.Select(c => c.RES_MEAS_STATION.Id);
                        builderResStLevelCar.Select(c => c.TimeOfMeasurements);
                        builderResStLevelCar.Where(c => c.RES_MEAS_STATION.Id, ConditionOperator.Equal, readerResMeasStation.GetValue(c => c.Id));
                        builderResStLevelCar.OrderByAsc(c => c.Id);
                        queryExecuter.Fetch(builderResStLevelCar, readerResStLevelCar =>
                        {
                            while (readerResStLevelCar.Read())
                            {
                                var levelMeasurementsCar = new LevelMeasurementsCar();
                                levelMeasurementsCar.Altitude = readerResStLevelCar.GetValue(c => c.Altitude);
                                levelMeasurementsCar.DifferenceTimestamp = readerResStLevelCar.GetValue(c => c.DifferenceTimeStamp);
                                levelMeasurementsCar.Lat = readerResStLevelCar.GetValue(c => c.Lat);
                                levelMeasurementsCar.LeveldBm = readerResStLevelCar.GetValue(c => c.LevelDbm);
                                levelMeasurementsCar.LeveldBmkVm = readerResStLevelCar.GetValue(c => c.LevelDbmkvm);
                                levelMeasurementsCar.Lon = readerResStLevelCar.GetValue(c => c.Lon);

                                levelMeasurementsCar.RBW = rbw;
                                levelMeasurementsCar.VBW = vbw;
                                levelMeasurementsCar.BW = bw;
                                levelMeasurementsCar.CentralFrequency = (decimal?)centralFrequency;


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
        public Route[] GetRoutes(long ResId)
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
                builderResRoutes.Select(c => c.RES_MEAS.Id);
                builderResRoutes.Select(c => c.RouteId);
                builderResRoutes.Select(c => c.StartTime);
                builderResRoutes.Where(c => c.RES_MEAS.Id, ConditionOperator.Equal, ResId);
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
        public ShortResultsMeasurementsStation[] GetShortMeasResStation(long ResId)
        {
            var listResMeasStatiion = new List<ShortResultsMeasurementsStation>();
            try
            {
                this._logger.Info(Contexts.ThisComponent, Categories.Processing, Events.HandlerGetShortMeasResStationMethod.Text);
                var queryExecuter = this._dataLayer.Executor<SdrnServerDataContext>();
                var builderResMeasStation = this._dataLayer.GetBuilder<MD.IResMeasStation>().From();
                builderResMeasStation.Select(c => c.GlobalSID);
                builderResMeasStation.Select(c => c.Id);
                builderResMeasStation.Select(c => c.MeasGlobalSID);
                builderResMeasStation.Select(c => c.RES_MEAS.Id);
                builderResMeasStation.Select(c => c.ClientSectorCode);
                builderResMeasStation.Select(c => c.Standard);
                builderResMeasStation.Select(c => c.ClientStationCode);
                builderResMeasStation.Select(c => c.Status);
                builderResMeasStation.Where(c => c.RES_MEAS.Id, ConditionOperator.Equal, ResId);
                builderResMeasStation.OrderByAsc(c => c.Id);
                queryExecuter.Fetch(builderResMeasStation, readerResMeasStation =>
                {
                    while (readerResMeasStation.Read())
                    {
                        var resMeasStatiion = new ShortResultsMeasurementsStation();
                        
                        resMeasStatiion.Idstation = readerResMeasStation.GetValue(c => c.ClientStationCode).ToString();
                        resMeasStatiion.GlobalSID = readerResMeasStation.GetValue(c => c.GlobalSID);
                        resMeasStatiion.MeasGlobalSID = readerResMeasStation.GetValue(c => c.MeasGlobalSID);
                        resMeasStatiion.Status = readerResMeasStation.GetValue(c => c.Status);
                        resMeasStatiion.IdSector = readerResMeasStation.GetValue(c => c.ClientSectorCode);
                        resMeasStatiion.Standard = readerResMeasStation.GetValue(c => c.Standard);


                        var builderStation = this._dataLayer.GetBuilder<MD.IStation>().From();
                        builderStation.Select(c => c.MEAS_TASK.Id);
                        builderStation.Select(c => c.Id);
                        builderStation.Select(c => c.STATION_SITE.Id);
                        builderStation.Where(c => c.Id, ConditionOperator.Equal, (long)readerResMeasStation.GetValue(c => c.ClientStationCode));
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
                                builderStationSite.Where(c => c.Id, ConditionOperator.Equal, readerStation.GetValue(c => c.STATION_SITE.Id));
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
                        builderResStGeneral.Select(c => c.RES_MEAS_STATION.Id);
                        builderResStGeneral.Select(c => c.SpecrumStartFreq);
                        builderResStGeneral.Select(c => c.SpecrumSteps);
                        builderResStGeneral.Select(c => c.T1);
                        builderResStGeneral.Select(c => c.T2);
                        builderResStGeneral.Select(c => c.BW);
                        builderResStGeneral.Select(c => c.TimeFinishMeas);
                        builderResStGeneral.Select(c => c.TimeStartMeas);
                        builderResStGeneral.Select(c => c.TraceCount);
                        builderResStGeneral.Where(c => c.RES_MEAS_STATION.Id, ConditionOperator.Equal, readerResMeasStation.GetValue(c => c.Id));
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
        public ResultsMeasurementsStation[] GetResMeasStationHeaderByResId(long ResId)
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
                builderResStGeneral.Select(c => c.RES_MEAS_STATION.Id);
                builderResStGeneral.Select(c => c.SpecrumStartFreq);
                builderResStGeneral.Select(c => c.SpecrumSteps);
                builderResStGeneral.Select(c => c.T1);
                builderResStGeneral.Select(c => c.T2);
                builderResStGeneral.Select(c => c.BW);
                builderResStGeneral.Select(c => c.TimeFinishMeas);
                builderResStGeneral.Select(c => c.TimeStartMeas);
                builderResStGeneral.Select(c => c.TraceCount);
                builderResStGeneral.Select(c => c.RES_MEAS_STATION.ClientSectorCode);
                builderResStGeneral.Select(c => c.RES_MEAS_STATION.ClientStationCode);
                builderResStGeneral.Select(c => c.RES_MEAS_STATION.GlobalSID);
                builderResStGeneral.Select(c => c.RES_MEAS_STATION.MeasGlobalSID);
                builderResStGeneral.Select(c => c.RES_MEAS_STATION.Status);
                builderResStGeneral.Select(c => c.RES_MEAS_STATION.Standard);
                builderResStGeneral.Select(c => c.RES_MEAS_STATION.RES_MEAS.Id);
                builderResStGeneral.Where(c => c.RES_MEAS_STATION.RES_MEAS.Id, ConditionOperator.Equal, ResId);
                builderResStGeneral.OrderByAsc(c => c.Id);
                queryExecuter.Fetch(builderResStGeneral, readerResStGeneral =>
                {
                    while (readerResStGeneral.Read())
                    {
                        var resMeasStatiion = new ResultsMeasurementsStation();

                        if (listResMeasStatiion.Find(x => x.Id == readerResStGeneral.GetValue(c => c.RES_MEAS_STATION.Id)) == null)
                        {
                            resMeasStatiion.Idstation = readerResStGeneral.GetValue(c => c.RES_MEAS_STATION.ClientStationCode).ToString();
                            resMeasStatiion.GlobalSID = readerResStGeneral.GetValue(c => c.RES_MEAS_STATION.GlobalSID);
                            resMeasStatiion.MeasGlobalSID = readerResStGeneral.GetValue(c => c.RES_MEAS_STATION.MeasGlobalSID);
                            resMeasStatiion.Status = readerResStGeneral.GetValue(c => c.RES_MEAS_STATION.Status);
                            resMeasStatiion.Id = readerResStGeneral.GetValue(c => c.RES_MEAS_STATION.Id);
                            resMeasStatiion.IdSector = readerResStGeneral.GetValue(c => c.RES_MEAS_STATION.ClientSectorCode);
                            resMeasStatiion.Standard = readerResStGeneral.GetValue(c => c.RES_MEAS_STATION.Standard);


                            var measurementsParameterGeneral = new MeasurementsParameterGeneral();

                            measurementsParameterGeneral.CentralFrequency = readerResStGeneral.GetValue(c => c.CentralFrequency);
                            measurementsParameterGeneral.CentralFrequencyMeas = readerResStGeneral.GetValue(c => c.CentralFrequencyMeas);
                            measurementsParameterGeneral.DurationMeas = readerResStGeneral.GetValue(c => c.DurationMeas);
                            measurementsParameterGeneral.MarkerIndex = readerResStGeneral.GetValue(c => c.MarkerIndex);
                            measurementsParameterGeneral.OffsetFrequency = readerResStGeneral.GetValue(c => c.OffsetFrequency);
                            measurementsParameterGeneral.SpecrumStartFreq = readerResStGeneral.GetValue(c => c.SpecrumStartFreq);
                            measurementsParameterGeneral.SpecrumSteps = readerResStGeneral.GetValue(c => c.SpecrumSteps);
                            measurementsParameterGeneral.T1 = readerResStGeneral.GetValue(c => c.T1);
                            measurementsParameterGeneral.T2 = readerResStGeneral.GetValue(c => c.T2);
                            measurementsParameterGeneral.TimeFinishMeas = readerResStGeneral.GetValue(c => c.TimeFinishMeas);
                            measurementsParameterGeneral.TimeStartMeas = readerResStGeneral.GetValue(c => c.TimeStartMeas);
                            resMeasStatiion.GeneralResult = measurementsParameterGeneral;
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
        public ResultsMeasurementsStation[] GetResMeasStation(long ResId, long StationId)
        {
            var listResMeasStatiion = new List<ResultsMeasurementsStation>();
            try
            {
                this._logger.Info(Contexts.ThisComponent, Categories.Processing, Events.HandlerCallGetResMeasStationMethod.Text);
                var queryExecuter = this._dataLayer.Executor<SdrnServerDataContext>();
                var builderResMeasStation = this._dataLayer.GetBuilder<MD.IResMeasStation>().From();
                builderResMeasStation.Select(c => c.GlobalSID);
                builderResMeasStation.Select(c => c.Id);
                builderResMeasStation.Select(c => c.MeasGlobalSID);
                builderResMeasStation.Select(c => c.RES_MEAS.Id);
                builderResMeasStation.Select(c => c.ClientSectorCode);
                builderResMeasStation.Select(c => c.Standard);
                builderResMeasStation.Select(c => c.ClientStationCode);
                builderResMeasStation.Select(c => c.Frequency);
                builderResMeasStation.Select(c => c.Status);
                builderResMeasStation.Where(c => c.RES_MEAS.Id, ConditionOperator.Equal, ResId);
                builderResMeasStation.Where(c => c.ClientStationCode, ConditionOperator.Equal, (int)StationId);
                builderResMeasStation.OrderByAsc(c => c.Id);
                queryExecuter.Fetch(builderResMeasStation, readerResMeasStation =>
                {
                    while (readerResMeasStation.Read())
                    {
                        var resMeasStatiion = new ResultsMeasurementsStation();
                        
                        resMeasStatiion.GlobalSID = readerResMeasStation.GetValue(c => c.GlobalSID);
                        resMeasStatiion.MeasGlobalSID = readerResMeasStation.GetValue(c => c.MeasGlobalSID);
                        resMeasStatiion.Status = readerResMeasStation.GetValue(c => c.Status);
                        resMeasStatiion.Id = readerResMeasStation.GetValue(c => c.Id);
                        resMeasStatiion.IdSector = readerResMeasStation.GetValue(c => c.ClientSectorCode);
                        resMeasStatiion.Idstation = readerResMeasStation.GetValue(c => c.ClientStationCode).ToString();
                        resMeasStatiion.Standard = readerResMeasStation.GetValue(c => c.Standard);

                        double? rbw = null;
                        double? vbw = null;
                        double? bw = null;
                        //double? centralFrequency = null;
                        double? centralFrequency = readerResMeasStation.GetValue(c => c.Frequency);

                        var measurementsParameterGeneral = new MeasurementsParameterGeneral();
                        var builderResStGeneral = this._dataLayer.GetBuilder<MD.IResStGeneral>().From();
                        builderResStGeneral.Select(c => c.CentralFrequency);
                        builderResStGeneral.Select(c => c.CentralFrequencyMeas);
                        builderResStGeneral.Select(c => c.Correctnessestim);
                        builderResStGeneral.Select(c => c.DurationMeas);
                        builderResStGeneral.Select(c => c.Id);
                        builderResStGeneral.Select(c => c.MarkerIndex);
                        builderResStGeneral.Select(c => c.OffsetFrequency);
                        builderResStGeneral.Select(c => c.RES_MEAS_STATION.Id);
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
                        builderResStGeneral.Select(c => c.LevelsSpectrumdBm);
                        builderResStGeneral.Where(c => c.RES_MEAS_STATION.Id, ConditionOperator.Equal, readerResMeasStation.GetValue(c => c.Id));
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
                                bw = readerResStGeneral.GetValue(c => c.BW);
                                //centralFrequency = readerResStGeneral.GetValue(c => c.CentralFrequencyMeas);


                                var listMaskElements = new List<MaskElements>();
                                var builderResStMaskElement = this._dataLayer.GetBuilder<MD.IResStMaskElement>().From();
                                builderResStMaskElement.Select(c => c.Bw);
                                builderResStMaskElement.Select(c => c.Level);
                                builderResStMaskElement.Where(c => c.RES_STGENERAL.Id, ConditionOperator.Equal, readerResStGeneral.GetValue(c => c.Id));
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
                                measurementsParameterGeneral.LevelsSpecrum = readerResStGeneral.GetValue(c=>c.LevelsSpectrumdBm);



                                var valSysInfo = "";


                                var queryStationSysInfo = this._dataLayer.GetBuilder<MD.IResSysInfo>()
                               .From()
                               .Select(c => c.Id, c => c.Agl, c => c.Asl, c => c.Bandwidth, c => c.BaseId, c => c.Bsic, c => c.ChannelNumber, c => c.Cid, c => c.Code, c => c.Ctoi, c => c.Eci, c => c.Enodebid, c => c.Freq, c => c.Icio, c => c.InbandPower, c => c.Iscp, c => c.Lac)
                               .Select(c => c.Lat, c => c.Lon, c => c.Mcc, c => c.Mnc, c => c.Nid, c => c.Pci, c => c.Pn, c => c.Power, c => c.Ptotal, c => c.Rnc, c => c.Rscp, c => c.Rsrp, c => c.Rsrq, c => c.Sc, c => c.Sid, c => c.Tac, c => c.TypeCdmaevdo, c => c.Ucid)
                               .Where(c => c.RES_STGENERAL.Id, ConditionOperator.Equal, readerResStGeneral.GetValue(c => c.Id));
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
                                        .Select(c => c.Id, c => c.Data, c => c.Type)
                                        .Where(c => c.RES_SYS_INFO.Id, ConditionOperator.Equal, readerStationSysInfo.GetValue(c => c.Id));
                                        queryExecuter.Fetch(queryStationSysInfoBls, readerStationSysInfoBls =>
                                        {
                                            while (readerStationSysInfoBls.Read())
                                            {
                                                valSysInfo += string.Format("SysInfoBlocks.Data  : {0}", readerStationSysInfoBls.GetValue(c => c.Data)) + Environment.NewLine;
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
                        builderResStLevelCar.Select(c => c.DifferenceTimeStamp);
                        builderResStLevelCar.Select(c => c.Id);
                        builderResStLevelCar.Select(c => c.Lat);
                        builderResStLevelCar.Select(c => c.LevelDbm);
                        builderResStLevelCar.Select(c => c.LevelDbmkvm);
                        builderResStLevelCar.Select(c => c.Lon);
                        builderResStLevelCar.Select(c => c.RES_MEAS_STATION.Id);
                        builderResStLevelCar.Select(c => c.TimeOfMeasurements);
                        builderResStLevelCar.Where(c => c.RES_MEAS_STATION.Id, ConditionOperator.Equal, readerResMeasStation.GetValue(c => c.Id));
                        builderResStLevelCar.OrderByAsc(c => c.Id);
                        queryExecuter.Fetch(builderResStLevelCar, readerResStLevelCar =>
                        {
                            while (readerResStLevelCar.Read())
                            {
                                var levelMeasurementsCar = new LevelMeasurementsCar();
                                levelMeasurementsCar.Altitude = readerResStLevelCar.GetValue(c => c.Altitude);
                                levelMeasurementsCar.DifferenceTimestamp = readerResStLevelCar.GetValue(c => c.DifferenceTimeStamp);
                                levelMeasurementsCar.Lat = readerResStLevelCar.GetValue(c => c.Lat);
                                levelMeasurementsCar.LeveldBm = readerResStLevelCar.GetValue(c => c.LevelDbm);
                                levelMeasurementsCar.LeveldBmkVm = readerResStLevelCar.GetValue(c => c.LevelDbmkvm);
                                levelMeasurementsCar.Lon = readerResStLevelCar.GetValue(c => c.Lon);
                                levelMeasurementsCar.RBW = rbw;
                                levelMeasurementsCar.VBW = vbw;
                                levelMeasurementsCar.BW = bw;
                                levelMeasurementsCar.CentralFrequency = (decimal?)centralFrequency;

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
        public MeasurementResults GetMeasurementResultByResId(long ResId, bool isLoadAllData, double? StartFrequency_Hz=null, double? StopFrequency_Hz = null)
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
                builderResMeas.Select(c => c.SUBTASK_SENSOR.Id);
                builderResMeas.Select(c => c.SUBTASK_SENSOR.SUBTASK.MEAS_TASK.Id);
                builderResMeas.Select(c => c.SUBTASK_SENSOR.SENSOR.Id);
                builderResMeas.Select(c => c.SUBTASK_SENSOR.SUBTASK.Id);
                builderResMeas.Select(c => c.N);
                builderResMeas.Select(c => c.ScansNumber);
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
                        levelmeasurementResults.Id.MeasSdrResultsId = readerResMeas.GetValue(c => c.Id);
                        levelmeasurementResults.Status = readerResMeas.GetValue(c => c.Status);
                        try
                        {
                            
                            levelmeasurementResults.Id.MeasTaskId.Value = readerResMeas.GetValue(c => c.SUBTASK_SENSOR.SUBTASK.MEAS_TASK.Id);
                            levelmeasurementResults.StationMeasurements = new StationMeasurements();
                            levelmeasurementResults.StationMeasurements.StationId = new SensorIdentifier();
                            levelmeasurementResults.StationMeasurements.StationId.Value = readerResMeas.GetValue(c => c.SUBTASK_SENSOR.SENSOR.Id);
                            levelmeasurementResults.Id.SubMeasTaskId = readerResMeas.GetValue(c => c.SUBTASK_SENSOR.SUBTASK.Id);
                            levelmeasurementResults.Id.SubMeasTaskStationId = readerResMeas.GetValue(c => c.SUBTASK_SENSOR.Id);
                        }
                        catch (Exception e)
                        {
                            this._logger.Exception(Contexts.ThisComponent, e);
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
                        builderResLevels.Select(c => c.RES_MEAS.Id);
                        builderResLevels.Select(c => c.StddevLev);
                        builderResLevels.Select(c => c.StdDevSpect);
                        builderResLevels.Select(c => c.ValueLvl);
                        builderResLevels.Select(c => c.ValueSpect);
                        builderResLevels.Select(c => c.VMinLvl);
                        builderResLevels.Select(c => c.VMinSpect);
                        builderResLevels.Select(c => c.VMMaxLvl);
                        builderResLevels.Select(c => c.VMMaxSpect);
                        builderResLevels.Where(c => c.RES_MEAS.Id, ConditionOperator.Equal, readerResMeas.GetValue(c => c.Id));
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
                        builderResLevMeasOnline.Select(c => c.RES_MEAS.Id);
                        builderResLevMeasOnline.Select(c => c.Value);
                        builderResLevMeasOnline.Where(c => c.RES_MEAS.Id, ConditionOperator.Equal, readerResMeas.GetValue(c => c.Id));
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
                        builderResLocSensorMeas.Select(c => c.RES_MEAS.Id);
                        builderResLocSensorMeas.OrderByAsc(c => c.Id);
                        builderResLocSensorMeas.Where(c => c.RES_MEAS.Id, ConditionOperator.Equal, readerResMeas.GetValue(c => c.Id));
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
                            builderResMeasStation.Where(c => c.RES_MEAS.Id, ConditionOperator.Equal, readerResMeas.GetValue(c => c.Id));
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
                            builderResMeasStation.Where(c => c.RES_MEAS.Id, ConditionOperator.Equal, readerResMeas.GetValue(c => c.Id));
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
                            builderResMeasStation.Where(c => c.RES_MEAS.Id, ConditionOperator.Equal, readerResMeas.GetValue(c => c.Id));
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
                            builderLinkResSensoT.Select(c => c.SENSOR.Id);
                            builderLinkResSensoT.Select(c => c.SENSOR.Name);
                            builderLinkResSensoT.Select(c => c.SENSOR.TechId);
                            builderLinkResSensoT.Where(c => c.RES_MEAS_STATION.RES_MEAS.Id, ConditionOperator.Equal, readerResMeas.GetValue(c => c.Id));
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


        public MeasurementResults[] GetMeasResultsHeaderByTaskId(long MeasTaskId)
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
                builderResMeas.Select(c => c.SUBTASK_SENSOR.Id);
                builderResMeas.Select(c => c.SUBTASK_SENSOR.SUBTASK.MEAS_TASK.Id);
                builderResMeas.Select(c => c.SUBTASK_SENSOR.SENSOR.Id);
                builderResMeas.Select(c => c.SUBTASK_SENSOR.SUBTASK.Id);
                builderResMeas.Select(c => c.N);
                builderResMeas.Select(c => c.ScansNumber);
                
                builderResMeas.Select(c => c.StartTime);
                builderResMeas.Select(c => c.Status);
                builderResMeas.Select(c => c.StopTime);
                builderResMeas.Select(c => c.Synchronized);
                builderResMeas.Select(c => c.TimeMeas);
                builderResMeas.Select(c => c.TypeMeasurements);
                builderResMeas.OrderByAsc(c => c.Id);
                builderResMeas.Where(c => c.SUBTASK_SENSOR.SUBTASK.MEAS_TASK.Id, ConditionOperator.Equal, MeasTaskId);
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
                        levelmeasurementResults.Id.MeasSdrResultsId = readerResMeas.GetValue(c => c.Id);
                        levelmeasurementResults.Status = readerResMeas.GetValue(c => c.Status);
                        try
                        {
                            
                            levelmeasurementResults.Id.MeasTaskId.Value = readerResMeas.GetValue(c => c.SUBTASK_SENSOR.SUBTASK.MEAS_TASK.Id);
                            levelmeasurementResults.StationMeasurements = new StationMeasurements();
                            levelmeasurementResults.StationMeasurements.StationId = new SensorIdentifier();
                            levelmeasurementResults.StationMeasurements.StationId.Value = readerResMeas.GetValue(c => c.SUBTASK_SENSOR.SENSOR.Id);
                            levelmeasurementResults.Id.SubMeasTaskId = readerResMeas.GetValue(c => c.SUBTASK_SENSOR.SUBTASK.Id);
                            levelmeasurementResults.Id.SubMeasTaskStationId = readerResMeas.GetValue(c => c.SUBTASK_SENSOR.Id);
                        }
                        catch (Exception e)
                        {
                            this._logger.Exception(Contexts.ThisComponent, e);
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

       public MeasurementResults[] GetMeasResultsByTaskId(long MeasTaskId)
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
                builderResMeas.Select(c => c.SUBTASK_SENSOR.Id);
                builderResMeas.Select(c => c.SUBTASK_SENSOR.SUBTASK.MEAS_TASK.Id);
                builderResMeas.Select(c => c.SUBTASK_SENSOR.SENSOR.Id);
                builderResMeas.Select(c => c.SUBTASK_SENSOR.SUBTASK.Id);
                builderResMeas.Select(c => c.N);
                builderResMeas.Select(c => c.ScansNumber);
                
                builderResMeas.Select(c => c.StartTime);
                builderResMeas.Select(c => c.Status);
                builderResMeas.Select(c => c.StopTime);
                builderResMeas.Select(c => c.Synchronized);
                builderResMeas.Select(c => c.TimeMeas);
                builderResMeas.Select(c => c.TypeMeasurements);
                builderResMeas.OrderByAsc(c => c.Id);
                builderResMeas.Where(c => c.SUBTASK_SENSOR.SUBTASK.MEAS_TASK.Id, ConditionOperator.Equal, MeasTaskId);
                builderResMeas.Where(c => c.Status, ConditionOperator.NotEqual, Status.Z.ToString());
                queryExecuter.Fetch(builderResMeas, readerResMeas =>
                {
                    while (readerResMeas.Read())
                    {
                        var levelmeasurementResults = new MeasurementResults();

                        levelmeasurementResults.AntVal = readerResMeas.GetValue(c => c.AntVal);
                        levelmeasurementResults.DataRank = readerResMeas.GetValue(c => c.DataRank);
                        levelmeasurementResults.N = readerResMeas.GetValue(c => c.N);
                        levelmeasurementResults.Status = readerResMeas.GetValue(c => c.Status);
                        levelmeasurementResults.Id = new MeasurementResultsIdentifier();
                        levelmeasurementResults.Id.MeasTaskId = new MeasTaskIdentifier();
                        levelmeasurementResults.Id.MeasSdrResultsId = readerResMeas.GetValue(c => c.Id);
                        try
                        {
                            
                            levelmeasurementResults.Id.MeasTaskId.Value = readerResMeas.GetValue(c => c.SUBTASK_SENSOR.SUBTASK.MEAS_TASK.Id);
                            levelmeasurementResults.StationMeasurements = new StationMeasurements();
                            levelmeasurementResults.StationMeasurements.StationId = new SensorIdentifier();
                            levelmeasurementResults.StationMeasurements.StationId.Value = readerResMeas.GetValue(c => c.SUBTASK_SENSOR.SENSOR.Id);
                            levelmeasurementResults.Id.SubMeasTaskId = readerResMeas.GetValue(c => c.SUBTASK_SENSOR.SUBTASK.Id);
                            levelmeasurementResults.Id.SubMeasTaskStationId = readerResMeas.GetValue(c => c.SUBTASK_SENSOR.Id);
                        }
                        catch (Exception e)
                        {
                            this._logger.Exception(Contexts.ThisComponent, e);
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
                        builderResLevels.Select(c => c.RES_MEAS.Id);
                        builderResLevels.Select(c => c.StddevLev);
                        builderResLevels.Select(c => c.StdDevSpect);
                        builderResLevels.Select(c => c.ValueLvl);
                        builderResLevels.Select(c => c.ValueSpect);
                        builderResLevels.Select(c => c.VMinLvl);
                        builderResLevels.Select(c => c.VMinSpect);
                        builderResLevels.Select(c => c.VMMaxLvl);
                        builderResLevels.Select(c => c.VMMaxSpect);
                        builderResLevels.Where(c => c.RES_MEAS.Id, ConditionOperator.Equal, readerResMeas.GetValue(c => c.Id));
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
                        builderResLevMeasOnline.Select(c => c.RES_MEAS.Id);
                        builderResLevMeasOnline.Select(c => c.Value);
                        builderResLevMeasOnline.Where(c => c.RES_MEAS.Id, ConditionOperator.Equal, readerResMeas.GetValue(c => c.Id));
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
                        builderResLocSensorMeas.Select(c => c.RES_MEAS.Id);
                        builderResLocSensorMeas.OrderByAsc(c => c.Id);
                        builderResLocSensorMeas.Where(c => c.RES_MEAS.Id, ConditionOperator.Equal, readerResMeas.GetValue(c => c.Id));
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
                        builderResMeasStation.Select(c => c.MeasGlobalSID);
                        builderResMeasStation.Select(c => c.RES_MEAS.Id);
                        builderResMeasStation.Select(c => c.ClientSectorCode);
                        builderResMeasStation.Select(c => c.Standard);
                        builderResMeasStation.Select(c => c.ClientStationCode);
                        builderResMeasStation.Select(c => c.Frequency);
                        builderResMeasStation.Select(c => c.Status);
                        builderResMeasStation.Where(c => c.RES_MEAS.Id, ConditionOperator.Equal, readerResMeas.GetValue(c => c.Id));
                        builderResMeasStation.OrderByAsc(c => c.Id);
                        queryExecuter.Fetch(builderResMeasStation, readerResMeasStation =>
                        {
                            while (readerResMeasStation.Read())
                            {
                                var resMeasStatiion = new ResultsMeasurementsStation();
                                resMeasStatiion.IdSector = readerResMeasStation.GetValue(c => c.ClientSectorCode);
                                resMeasStatiion.Idstation = readerResMeasStation.GetValue(c => c.ClientStationCode).ToString();
                                resMeasStatiion.GlobalSID = readerResMeasStation.GetValue(c => c.GlobalSID);
                                resMeasStatiion.MeasGlobalSID = readerResMeasStation.GetValue(c => c.MeasGlobalSID);
                                resMeasStatiion.Status = readerResMeasStation.GetValue(c => c.Status);
                                resMeasStatiion.Id = readerResMeasStation.GetValue(c => c.Id);
                                resMeasStatiion.Standard = readerResMeasStation.GetValue(c => c.Standard);



                                double? rbw = null;
                                double? vbw = null;
                                double? bw = null;
                                double? centralFrequency = readerResMeasStation.GetValue(c => c.Frequency);

                                var measurementsParameterGeneral = new MeasurementsParameterGeneral();
                                var builderResStGeneral = this._dataLayer.GetBuilder<MD.IResStGeneral>().From();
                                builderResStGeneral.Select(c => c.CentralFrequency);
                                builderResStGeneral.Select(c => c.CentralFrequencyMeas);
                                builderResStGeneral.Select(c => c.Correctnessestim);
                                builderResStGeneral.Select(c => c.DurationMeas);
                                builderResStGeneral.Select(c => c.Id);
                                builderResStGeneral.Select(c => c.MarkerIndex);
                                builderResStGeneral.Select(c => c.OffsetFrequency);
                                builderResStGeneral.Select(c => c.RES_MEAS_STATION.Id);
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
                                builderResStGeneral.Select(c => c.LevelsSpectrumdBm);
                                builderResStGeneral.Where(c => c.RES_MEAS_STATION.Id, ConditionOperator.Equal, readerResMeasStation.GetValue(c => c.Id));
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
                                        bw = readerResStGeneral.GetValue(c => c.BW);
                                        //centralFrequency = readerResStGeneral.GetValue(c => c.CentralFrequency);


                                        var listMaskElements = new List<MaskElements>();
                                        var builderResStMaskElement = this._dataLayer.GetBuilder<MD.IResStMaskElement>().From();
                                        builderResStMaskElement.Select(c => c.Bw);
                                        builderResStMaskElement.Select(c => c.Level);
                                        builderResStMaskElement.Where(c => c.RES_STGENERAL.Id, ConditionOperator.Equal, readerResStGeneral.GetValue(c => c.Id));
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
                                        measurementsParameterGeneral.LevelsSpecrum = readerResStGeneral.GetValue(c=>c.LevelsSpectrumdBm);
                                    }
                                    return true;
                                });
                                resMeasStatiion.GeneralResult = measurementsParameterGeneral;


                                var listLevelMeasurementsCar = new List<LevelMeasurementsCar>();
                                var builderResStLevelCar = this._dataLayer.GetBuilder<MD.IResStLevelCar>().From();
                                builderResStLevelCar.Select(c => c.Agl);
                                builderResStLevelCar.Select(c => c.Altitude);
                                builderResStLevelCar.Select(c => c.DifferenceTimeStamp);
                                builderResStLevelCar.Select(c => c.Id);
                                builderResStLevelCar.Select(c => c.Lat);
                                builderResStLevelCar.Select(c => c.LevelDbm);
                                builderResStLevelCar.Select(c => c.LevelDbmkvm);
                                builderResStLevelCar.Select(c => c.Lon);
                                builderResStLevelCar.Select(c => c.RES_MEAS_STATION.Id);
                                builderResStLevelCar.Select(c => c.TimeOfMeasurements);
                                builderResStLevelCar.Where(c => c.RES_MEAS_STATION.Id, ConditionOperator.Equal, readerResMeasStation.GetValue(c => c.Id));
                                builderResStLevelCar.OrderByAsc(c => c.Id);
                                queryExecuter.Fetch(builderResStLevelCar, readerResStLevelCar =>
                                {
                                    while (readerResStLevelCar.Read())
                                    {
                                        var levelMeasurementsCar = new LevelMeasurementsCar();
                                        levelMeasurementsCar.Altitude = readerResStLevelCar.GetValue(c => c.Altitude);
                                        levelMeasurementsCar.DifferenceTimestamp = readerResStLevelCar.GetValue(c => c.DifferenceTimeStamp);
                                        levelMeasurementsCar.Lat = readerResStLevelCar.GetValue(c => c.Lat);
                                        levelMeasurementsCar.LeveldBm = readerResStLevelCar.GetValue(c => c.LevelDbm);
                                        levelMeasurementsCar.LeveldBmkVm = readerResStLevelCar.GetValue(c => c.LevelDbmkvm);
                                        levelMeasurementsCar.Lon = readerResStLevelCar.GetValue(c => c.Lon);
                                        levelMeasurementsCar.RBW = rbw;
                                        levelMeasurementsCar.VBW = vbw;
                                        levelMeasurementsCar.BW = bw;
                                        levelMeasurementsCar.CentralFrequency = (decimal?)centralFrequency;

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
                builderResLocSensorMeasFast.Select(c => c.RES_MEAS.AntVal);
                builderResLocSensorMeasFast.Select(c => c.RES_MEAS.DataRank);
                builderResLocSensorMeasFast.Select(c => c.RES_MEAS.Id);
                builderResLocSensorMeasFast.Select(c => c.RES_MEAS.MeasResultSID);
                builderResLocSensorMeasFast.Select(c => c.RES_MEAS.SUBTASK_SENSOR.Id);
                builderResLocSensorMeasFast.Select(c => c.RES_MEAS.SUBTASK_SENSOR.SUBTASK.MEAS_TASK.Id);
                builderResLocSensorMeasFast.Select(c => c.RES_MEAS.SUBTASK_SENSOR.SENSOR.Id);
                builderResLocSensorMeasFast.Select(c => c.RES_MEAS.SUBTASK_SENSOR.SUBTASK.Id);
                builderResLocSensorMeasFast.Select(c => c.RES_MEAS.N);
                builderResLocSensorMeasFast.Select(c => c.RES_MEAS.ScansNumber);
                builderResLocSensorMeasFast.Select(c => c.RES_MEAS.StartTime);
                builderResLocSensorMeasFast.Select(c => c.RES_MEAS.Status);
                builderResLocSensorMeasFast.Select(c => c.RES_MEAS.StopTime);
                builderResLocSensorMeasFast.Select(c => c.RES_MEAS.Synchronized);
                builderResLocSensorMeasFast.Select(c => c.RES_MEAS.TimeMeas);
                builderResLocSensorMeasFast.Select(c => c.RES_MEAS.TypeMeasurements);
                builderResLocSensorMeasFast.Select(c => c.RES_MEAS.SUBTASK_SENSOR.SENSOR.Name);
                builderResLocSensorMeasFast.Select(c => c.RES_MEAS.SUBTASK_SENSOR.SENSOR.TechId);
                builderResLocSensorMeasFast.Select(c => c.RES_MEAS.Id);
                builderResLocSensorMeasFast.Where(c => c.RES_MEAS.Status, ConditionOperator.NotEqual, Status.Z.ToString());
                builderResLocSensorMeasFast.OrderByAsc(c => c.Id);
                var locationSensorMeasurement = new LocationSensorMeasurement();
                queryExecuter.Fetch(builderResLocSensorMeasFast, readerResLocSensorMeas =>
                {
                    while (readerResLocSensorMeas.Read())
                    {
                        var shortMeasurementResultsFast = new ShortMeasurementResults();
                        shortMeasurementResultsFast.CurrentLon = readerResLocSensorMeas.GetValue(c => c.Lon);
                        shortMeasurementResultsFast.CurrentLat = readerResLocSensorMeas.GetValue(c => c.Lat);
                        shortMeasurementResultsFast.Status = readerResLocSensorMeas.GetValue(c => c.RES_MEAS.Status);
                        shortMeasurementResultsFast.DataRank = readerResLocSensorMeas.GetValue(c => c.RES_MEAS.DataRank);
                        shortMeasurementResultsFast.Id = new MeasurementResultsIdentifier();
                        shortMeasurementResultsFast.Id.MeasTaskId = new MeasTaskIdentifier();
                        shortMeasurementResultsFast.Id.MeasSdrResultsId = readerResLocSensorMeas.GetValue(c => c.RES_MEAS.Id);
                        try
                        {
                            shortMeasurementResultsFast.Id.MeasTaskId.Value = readerResLocSensorMeas.GetValue(c => c.RES_MEAS.SUBTASK_SENSOR.SUBTASK.MEAS_TASK.Id);
                            shortMeasurementResultsFast.Id.SubMeasTaskId = readerResLocSensorMeas.GetValue(c => c.RES_MEAS.SUBTASK_SENSOR.SUBTASK.Id);
                            shortMeasurementResultsFast.Id.SubMeasTaskStationId = readerResLocSensorMeas.GetValue(c => c.RES_MEAS.SUBTASK_SENSOR.Id);
                            shortMeasurementResultsFast.SensorName = readerResLocSensorMeas.GetValue(c => c.RES_MEAS.SUBTASK_SENSOR.SENSOR.Name);
                            shortMeasurementResultsFast.SensorTechId = readerResLocSensorMeas.GetValue(c => c.RES_MEAS.SUBTASK_SENSOR.SENSOR.TechId);
                        }
                        catch (Exception e)
                        {
                            this._logger.Exception(Contexts.ThisComponent, e);
                        }

                        if (readerResLocSensorMeas.GetValue(c => c.RES_MEAS.TimeMeas) != null)
                        {
                            shortMeasurementResultsFast.TimeMeas = readerResLocSensorMeas.GetValue(c => c.RES_MEAS.TimeMeas).Value;
                        }
                        if (readerResLocSensorMeas.GetValue(c => c.RES_MEAS.StartTime) != null)
                        {
                            shortMeasurementResultsFast.StartTime = readerResLocSensorMeas.GetValue(c => c.RES_MEAS.StartTime).Value;
                        }
                        if (readerResLocSensorMeas.GetValue(c => c.RES_MEAS.StopTime) != null)
                        {
                            shortMeasurementResultsFast.StopTime = readerResLocSensorMeas.GetValue(c => c.RES_MEAS.StopTime).Value;
                        }
                        MeasurementType outResType;
                        if (Enum.TryParse<MeasurementType>(readerResLocSensorMeas.GetValue(c => c.RES_MEAS.TypeMeasurements), out outResType))
                        {
                            shortMeasurementResultsFast.TypeMeasurements = outResType;
                        }
                        shortMeasurementResultsFast.Number = readerResLocSensorMeas.GetValue(c => c.RES_MEAS.N).HasValue ? readerResLocSensorMeas.GetValue(c => c.RES_MEAS.N).Value : -1;

                        

                        if ((results.Find(c => c.Id.MeasSdrResultsId == shortMeasurementResultsFast.Id.MeasSdrResultsId)) == null)
                        {
                            results.Add(shortMeasurementResultsFast);
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

        public ShortMeasurementResults[] GetShortMeasResultsByTaskId(long MeasTaskId)
        {
            var listlevelmeasurementResults = new List<ShortMeasurementResults>();
            try
            {
                this._logger.Info(Contexts.ThisComponent, Categories.Processing, Events.HandlerGetShortMeasResultsByTaskIdMethod.Text);
                var queryExecuter = this._dataLayer.Executor<SdrnServerDataContext>();
                var builderResMeas = this._dataLayer.GetBuilder<MD.IResLocSensorMeas>().From();
                builderResMeas.Select(c => c.RES_MEAS.AntVal);
                builderResMeas.Select(c => c.RES_MEAS.DataRank);
                builderResMeas.Select(c => c.RES_MEAS.Id);
                builderResMeas.Select(c => c.Lon);
                builderResMeas.Select(c => c.Lat);
                builderResMeas.Select(c => c.RES_MEAS.MeasResultSID);
                builderResMeas.Select(c => c.RES_MEAS.SUBTASK_SENSOR.Id);
                builderResMeas.Select(c => c.RES_MEAS.SUBTASK_SENSOR.SUBTASK.MEAS_TASK.Id);
                builderResMeas.Select(c => c.RES_MEAS.SUBTASK_SENSOR.SENSOR.Id);
                builderResMeas.Select(c => c.RES_MEAS.SUBTASK_SENSOR.SUBTASK.Id);
                builderResMeas.Select(c => c.RES_MEAS.N);
                builderResMeas.Select(c => c.RES_MEAS.ScansNumber);
                builderResMeas.Select(c => c.RES_MEAS.StartTime);
                builderResMeas.Select(c => c.RES_MEAS.Status);
                builderResMeas.Select(c => c.RES_MEAS.StopTime);
                builderResMeas.Select(c => c.RES_MEAS.Synchronized);
                builderResMeas.Select(c => c.RES_MEAS.TimeMeas);
                builderResMeas.Select(c => c.RES_MEAS.TypeMeasurements);
                builderResMeas.Select(c => c.RES_MEAS.SUBTASK_SENSOR.Id);
                builderResMeas.Select(c => c.RES_MEAS.SUBTASK_SENSOR.SENSOR.Name);
                builderResMeas.Select(c => c.RES_MEAS.SUBTASK_SENSOR.SENSOR.TechId);

                builderResMeas.OrderByAsc(c => c.Id);
                builderResMeas.Where(c => c.RES_MEAS.SUBTASK_SENSOR.SUBTASK.MEAS_TASK.Id, ConditionOperator.Equal, MeasTaskId);
                builderResMeas.Where(c => c.RES_MEAS.Status, ConditionOperator.NotEqual, Status.Z.ToString());
                queryExecuter.Fetch(builderResMeas, readerResMeas =>
                {
                    while (readerResMeas.Read())
                    {
                        var levelmeasurementResults = new ShortMeasurementResults();
                        levelmeasurementResults.DataRank = readerResMeas.GetValue(c => c.RES_MEAS.DataRank);
                        levelmeasurementResults.Number = readerResMeas.GetValue(c => c.RES_MEAS.N).HasValue ? readerResMeas.GetValue(c => c.RES_MEAS.N).Value : -1;
                        levelmeasurementResults.Status = readerResMeas.GetValue(c => c.RES_MEAS.Status);
                        levelmeasurementResults.Id = new MeasurementResultsIdentifier();
                        levelmeasurementResults.Id.MeasTaskId = new MeasTaskIdentifier();
                        levelmeasurementResults.Id.MeasSdrResultsId = readerResMeas.GetValue(c => c.RES_MEAS.Id);
                        try
                        {
                            levelmeasurementResults.Id.MeasTaskId.Value = readerResMeas.GetValue(c => c.RES_MEAS.SUBTASK_SENSOR.SUBTASK.MEAS_TASK.Id);
                            levelmeasurementResults.Id.SubMeasTaskId = readerResMeas.GetValue(c => c.RES_MEAS.SUBTASK_SENSOR.SUBTASK.Id);
                            levelmeasurementResults.Id.SubMeasTaskStationId = readerResMeas.GetValue(c => c.RES_MEAS.SUBTASK_SENSOR.Id);
                            levelmeasurementResults.SensorName = readerResMeas.GetValue(c => c.RES_MEAS.SUBTASK_SENSOR.SENSOR.Name);
                            levelmeasurementResults.SensorTechId = readerResMeas.GetValue(c => c.RES_MEAS.SUBTASK_SENSOR.SENSOR.TechId);
                        }
                        catch (Exception e)
                        {
                            this._logger.Exception(Contexts.ThisComponent, e);
                        }

                        if (readerResMeas.GetValue(c => c.RES_MEAS.TimeMeas) != null)
                        {
                            levelmeasurementResults.TimeMeas = readerResMeas.GetValue(c => c.RES_MEAS.TimeMeas).Value;
                        }
                        if (readerResMeas.GetValue(c => c.RES_MEAS.StartTime) != null)
                        {
                            levelmeasurementResults.StartTime = readerResMeas.GetValue(c => c.RES_MEAS.StartTime).Value;
                        }
                        if (readerResMeas.GetValue(c => c.RES_MEAS.StopTime) != null)
                        {
                            levelmeasurementResults.StopTime = readerResMeas.GetValue(c => c.RES_MEAS.StopTime).Value;
                        }

                        MeasurementType outResType;
                        if (Enum.TryParse<MeasurementType>(readerResMeas.GetValue(c => c.RES_MEAS.TypeMeasurements), out outResType))
                        {
                            levelmeasurementResults.TypeMeasurements = outResType;
                        }
                        
                        levelmeasurementResults.CurrentLon = readerResMeas.GetValue(c => c.Lon);
                        levelmeasurementResults.CurrentLat = readerResMeas.GetValue(c => c.Lat);


                        levelmeasurementResults.CountUnknownStationMeasurements = 0;
                        levelmeasurementResults.CountStationMeasurements = 0;
                        if (outResType == MeasurementType.MonitoringStations)
                        {
                            var builderResMeasStation = this._dataLayer.GetBuilder<MD.IResMeasStation>().From();
                            builderResMeasStation.Select(c => c.Id);
                            builderResMeasStation.Select(c => c.Status);
                            builderResMeasStation.Where(c => c.RES_MEAS.Id, ConditionOperator.Equal, readerResMeas.GetValue(c => c.RES_MEAS.Id));
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
                            builderResMeasStation.Where(c => c.RES_MEAS.Id, ConditionOperator.Equal, readerResMeas.GetValue(c => c.RES_MEAS.Id));
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
                            builderResMeasStation.Where(c => c.RES_MEAS.Id, ConditionOperator.Equal, readerResMeas.GetValue(c => c.RES_MEAS.Id));
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

        public ShortMeasurementResults[] GetShortMeasResultsByTypeAndTaskId(MeasurementType measurementType, long taskId)
        {
            var listlevelmeasurementResults = new List<ShortMeasurementResults>();
            try
            {
                this._logger.Info(Contexts.ThisComponent, Categories.Processing, Events.HandlerGetShortMeasResultsByTypeAndTaskIdMethod.Text);
                var queryExecuter = this._dataLayer.Executor<SdrnServerDataContext>();
                var builderResMeas = this._dataLayer.GetBuilder<MD.IResLocSensorMeas>().From();
                builderResMeas.Select(c => c.RES_MEAS.AntVal);
                builderResMeas.Select(c => c.RES_MEAS.DataRank);
                builderResMeas.Select(c => c.RES_MEAS.Id);
                builderResMeas.Select(c => c.Lon);
                builderResMeas.Select(c => c.Lat);
                builderResMeas.Select(c => c.RES_MEAS.MeasResultSID);
                builderResMeas.Select(c => c.RES_MEAS.SUBTASK_SENSOR.Id);
                builderResMeas.Select(c => c.RES_MEAS.SUBTASK_SENSOR.SUBTASK.MEAS_TASK.Id);
                builderResMeas.Select(c => c.RES_MEAS.SUBTASK_SENSOR.SENSOR.Id);
                builderResMeas.Select(c => c.RES_MEAS.SUBTASK_SENSOR.SUBTASK.Id);
                builderResMeas.Select(c => c.RES_MEAS.N);
                builderResMeas.Select(c => c.RES_MEAS.ScansNumber);
                builderResMeas.Select(c => c.RES_MEAS.StartTime);
                builderResMeas.Select(c => c.RES_MEAS.Status);
                builderResMeas.Select(c => c.RES_MEAS.StopTime);
                builderResMeas.Select(c => c.RES_MEAS.Synchronized);
                builderResMeas.Select(c => c.RES_MEAS.TimeMeas);
                builderResMeas.Select(c => c.RES_MEAS.TypeMeasurements);
                builderResMeas.Select(c => c.RES_MEAS.SUBTASK_SENSOR.SENSOR.Name);
                builderResMeas.Select(c => c.RES_MEAS.SUBTASK_SENSOR.SENSOR.TechId);
                builderResMeas.OrderByAsc(c => c.Id);
                builderResMeas.Where(c => c.RES_MEAS.TypeMeasurements, ConditionOperator.Equal, measurementType.ToString());
                builderResMeas.Where(c => c.RES_MEAS.SUBTASK_SENSOR.SUBTASK.MEAS_TASK.Id, ConditionOperator.Equal, taskId);
                builderResMeas.Where(c => c.RES_MEAS.Status, ConditionOperator.NotEqual, Status.Z.ToString());
                queryExecuter.Fetch(builderResMeas, readerResMeas =>
                {
                    while (readerResMeas.Read())
                    {
                        var levelmeasurementResults = new ShortMeasurementResults();

                        levelmeasurementResults.DataRank = readerResMeas.GetValue(c => c.RES_MEAS.DataRank);
                        levelmeasurementResults.Number = readerResMeas.GetValue(c => c.RES_MEAS.N).HasValue ? readerResMeas.GetValue(c => c.RES_MEAS.N).Value : -1;
                        levelmeasurementResults.Status = readerResMeas.GetValue(c => c.RES_MEAS.Status);
                        levelmeasurementResults.Id = new MeasurementResultsIdentifier();
                        levelmeasurementResults.Id.MeasTaskId = new MeasTaskIdentifier();
                        levelmeasurementResults.Id.MeasSdrResultsId = readerResMeas.GetValue(c => c.RES_MEAS.Id);
                        try
                        {
                            levelmeasurementResults.Id.MeasTaskId.Value = readerResMeas.GetValue(c => c.RES_MEAS.SUBTASK_SENSOR.SUBTASK.MEAS_TASK.Id);
                            levelmeasurementResults.Id.SubMeasTaskId = readerResMeas.GetValue(c => c.RES_MEAS.SUBTASK_SENSOR.SUBTASK.Id);
                            levelmeasurementResults.Id.SubMeasTaskStationId = readerResMeas.GetValue(c => c.RES_MEAS.SUBTASK_SENSOR.Id);
                            levelmeasurementResults.SensorName = readerResMeas.GetValue(c => c.RES_MEAS.SUBTASK_SENSOR.SENSOR.Name);
                            levelmeasurementResults.SensorTechId = readerResMeas.GetValue(c => c.RES_MEAS.SUBTASK_SENSOR.SENSOR.TechId);
                        }
                        catch (Exception e)
                        {
                            this._logger.Exception(Contexts.ThisComponent, e);
                        }

                        if (readerResMeas.GetValue(c => c.RES_MEAS.TimeMeas) != null)
                        {
                            levelmeasurementResults.TimeMeas = readerResMeas.GetValue(c => c.RES_MEAS.TimeMeas).Value;
                        }
                        if (readerResMeas.GetValue(c => c.RES_MEAS.StartTime) != null)
                        {
                            levelmeasurementResults.StartTime = readerResMeas.GetValue(c => c.RES_MEAS.StartTime).Value;
                        }
                        if (readerResMeas.GetValue(c => c.RES_MEAS.StopTime) != null)
                        {
                            levelmeasurementResults.StopTime = readerResMeas.GetValue(c => c.RES_MEAS.StopTime).Value;
                        }
                        MeasurementType outResType;
                        if (Enum.TryParse<MeasurementType>(readerResMeas.GetValue(c => c.RES_MEAS.TypeMeasurements), out outResType))
                        {
                            levelmeasurementResults.TypeMeasurements = outResType;
                        }
                        
                        levelmeasurementResults.CurrentLon = readerResMeas.GetValue(c => c.Lon);
                        levelmeasurementResults.CurrentLat = readerResMeas.GetValue(c => c.Lat);

                        levelmeasurementResults.CountUnknownStationMeasurements = 0;
                        levelmeasurementResults.CountStationMeasurements = 0;

                        if (outResType == MeasurementType.MonitoringStations)
                        {
                            var builderResMeasStation = this._dataLayer.GetBuilder<MD.IResMeasStation>().From();
                            builderResMeasStation.Select(c => c.Id);
                            builderResMeasStation.Select(c => c.Status);
                            builderResMeasStation.Where(c => c.RES_MEAS.Id, ConditionOperator.Equal, readerResMeas.GetValue(c => c.RES_MEAS.Id));
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
                            builderResMeasStation.Where(c => c.RES_MEAS.Id, ConditionOperator.Equal, readerResMeas.GetValue(c => c.RES_MEAS.Id));
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
                            builderResMeasStation.Where(c => c.RES_MEAS.Id, ConditionOperator.Equal, readerResMeas.GetValue(c => c.RES_MEAS.Id));
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
                builderResMeas.Select(c => c.RES_MEAS.AntVal);
                builderResMeas.Select(c => c.RES_MEAS.DataRank);
                builderResMeas.Select(c => c.RES_MEAS.Id);
                builderResMeas.Select(c => c.Lon);
                builderResMeas.Select(c => c.Lat);
                builderResMeas.Select(c => c.RES_MEAS.MeasResultSID);
                builderResMeas.Select(c => c.RES_MEAS.SUBTASK_SENSOR.Id);
                builderResMeas.Select(c => c.RES_MEAS.SUBTASK_SENSOR.SUBTASK.MEAS_TASK.Id);
                builderResMeas.Select(c => c.RES_MEAS.SUBTASK_SENSOR.SENSOR.Id);
                builderResMeas.Select(c => c.RES_MEAS.SUBTASK_SENSOR.SUBTASK.Id);
                builderResMeas.Select(c => c.RES_MEAS.N);
                builderResMeas.Select(c => c.RES_MEAS.ScansNumber);
                
                builderResMeas.Select(c => c.RES_MEAS.StartTime);
                builderResMeas.Select(c => c.RES_MEAS.Status);
                builderResMeas.Select(c => c.RES_MEAS.StopTime);
                builderResMeas.Select(c => c.RES_MEAS.Synchronized);
                builderResMeas.Select(c => c.RES_MEAS.TimeMeas);
                builderResMeas.Select(c => c.RES_MEAS.TypeMeasurements);
                builderResMeas.Select(c => c.RES_MEAS.SUBTASK_SENSOR.SENSOR.Name);
                builderResMeas.Select(c => c.RES_MEAS.SUBTASK_SENSOR.SENSOR.TechId);
                builderResMeas.OrderByAsc(c => c.Id);
                builderResMeas.Where(c => c.RES_MEAS.TypeMeasurements, ConditionOperator.Equal, measurementType.ToString());
                builderResMeas.Where(c => c.RES_MEAS.Status, ConditionOperator.NotEqual, Status.Z.ToString());
                queryExecuter.Fetch(builderResMeas, readerResMeas =>
                {
                    while (readerResMeas.Read())
                    {
                        var levelmeasurementResults = new ShortMeasurementResults();

                        levelmeasurementResults.DataRank = readerResMeas.GetValue(c => c.RES_MEAS.DataRank);
                        levelmeasurementResults.Number = readerResMeas.GetValue(c => c.RES_MEAS.N).HasValue ? readerResMeas.GetValue(c => c.RES_MEAS.N).Value : -1;
                        levelmeasurementResults.Id = new MeasurementResultsIdentifier();
                        levelmeasurementResults.Id.MeasTaskId = new MeasTaskIdentifier();
                        levelmeasurementResults.Id.MeasSdrResultsId = readerResMeas.GetValue(c => c.RES_MEAS.Id);
                        levelmeasurementResults.Status = readerResMeas.GetValue(c => c.RES_MEAS.Status);
                        try
                        {
                            levelmeasurementResults.Id.MeasTaskId.Value = readerResMeas.GetValue(c => c.RES_MEAS.SUBTASK_SENSOR.SUBTASK.MEAS_TASK.Id);
                            levelmeasurementResults.Id.SubMeasTaskId = readerResMeas.GetValue(c => c.RES_MEAS.SUBTASK_SENSOR.SUBTASK.Id);
                            levelmeasurementResults.Id.SubMeasTaskStationId = readerResMeas.GetValue(c => c.RES_MEAS.SUBTASK_SENSOR.Id);
                            levelmeasurementResults.SensorName = readerResMeas.GetValue(c => c.RES_MEAS.SUBTASK_SENSOR.SENSOR.Name);
                            levelmeasurementResults.SensorTechId = readerResMeas.GetValue(c => c.RES_MEAS.SUBTASK_SENSOR.SENSOR.TechId);
                        }
                        catch (Exception e)
                        {
                            this._logger.Exception(Contexts.ThisComponent, e);
                        }

                        if (readerResMeas.GetValue(c => c.RES_MEAS.TimeMeas) != null)
                        {
                            levelmeasurementResults.TimeMeas = readerResMeas.GetValue(c => c.RES_MEAS.TimeMeas).Value;
                        }
                        if (readerResMeas.GetValue(c => c.RES_MEAS.StartTime) != null)
                        {
                            levelmeasurementResults.StartTime = readerResMeas.GetValue(c => c.RES_MEAS.StartTime).Value;
                        }
                        if (readerResMeas.GetValue(c => c.RES_MEAS.StopTime) != null)
                        {
                            levelmeasurementResults.StopTime = readerResMeas.GetValue(c => c.RES_MEAS.StopTime).Value;
                        }
                        MeasurementType outResType;
                        if (Enum.TryParse<MeasurementType>(readerResMeas.GetValue(c => c.RES_MEAS.TypeMeasurements), out outResType))
                        {
                            levelmeasurementResults.TypeMeasurements = outResType;
                        }
                        


                        levelmeasurementResults.CurrentLon = readerResMeas.GetValue(c => c.Lon);
                        levelmeasurementResults.CurrentLat = readerResMeas.GetValue(c => c.Lat);


                        levelmeasurementResults.CountUnknownStationMeasurements = 0;
                        levelmeasurementResults.CountStationMeasurements = 0;
                        if (outResType == MeasurementType.MonitoringStations)
                        {
                            var builderResMeasStation = this._dataLayer.GetBuilder<MD.IResMeasStation>().From();
                            builderResMeasStation.Select(c => c.Id);
                            builderResMeasStation.Select(c => c.Status);
                            builderResMeasStation.Where(c => c.RES_MEAS.Id, ConditionOperator.Equal, readerResMeas.GetValue(c => c.RES_MEAS.Id));
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
                            builderResMeasStation.Where(c => c.RES_MEAS.Id, ConditionOperator.Equal, readerResMeas.GetValue(c => c.RES_MEAS.Id));
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
                            builderResMeasStation.Where(c => c.RES_MEAS.Id, ConditionOperator.Equal, readerResMeas.GetValue(c => c.RES_MEAS.Id));
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




