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


namespace Atdi.WcfServices.Sdrn.Server
{
    public class LoadResults 
    {
        private readonly IDataLayer<EntityDataOrm> _dataLayer;
        private readonly ILogger _logger;



        public LoadResults(IDataLayer<EntityDataOrm> dataLayer, ILogger logger)
        {
            this._dataLayer = dataLayer;
            this._logger = logger;
        }
        

        public void GetEmittingAndReferenceLevels(int resId, out Emitting[] emittings, out ReferenceLevels referenceLevels)
        {
            emittings = null;
            referenceLevels = new ReferenceLevels();
            var queryExecuter = this._dataLayer.Executor<SdrnServerDataContext>();
            var listEmitting = new List<Emitting>();
            var queryEmitting = this._dataLayer.GetBuilder<MD.IEmitting>()
            .From()
            .Select(c => c.Id, c => c.CurentPower_dBm, c => c.MeanDeviationFromReference, c => c.ReferenceLevel_dBm, c => c.RollOffFactor, c => c.StandardBW, c => c.StartFrequency_MHz, c => c.StopFrequency_MHz, c => c.TriggerDeviationFromReference)
            .Where(c => c.ResMeasId, ConditionOperator.Equal, resId);
            queryExecuter.Fetch(queryEmitting, reader =>
            {
                while (reader.Read())
                {
                    var emitting = new Emitting();
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


                    var emittingParam = new EmittingParameters();

                    if (reader.GetValue(c => c.RollOffFactor).HasValue)
                        emittingParam.RollOffFactor = reader.GetValue(c => c.RollOffFactor).Value;
                    if (reader.GetValue(c => c.StandardBW).HasValue)
                    {
                        emittingParam.StandardBW = reader.GetValue(c => c.StandardBW).Value;
                    }
                    emitting.EmittingParameters = emittingParam;

                    var listTime = new List<WorkTime>();
                    var queryTime = this._dataLayer.GetBuilder<MD.IWorkTime>()
                    .From()
                    .Select(c => c.Id, c => c.StartEmitting, c => c.StopEmitting, c => c.HitCount, c => c.PersentAvailability)
                    .Where(c => c.EmittingId, ConditionOperator.Equal, reader.GetValue(c => c.Id));
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
                                workTime.PersentAvailability = (float)readerTime.GetValue(c => c.PersentAvailability).Value;

                            listTime.Add(workTime);
                        }
                        return true;
                    });
                    if (listTime.Count > 0)
                        emitting.WorkTimes = listTime.ToArray();



                    var listLoss_dB = new List<float>();
                    var listFreq_kHz = new List<double>();
                    var querySignalMask = this._dataLayer.GetBuilder<MD.ISignalMask>()
                    .From()
                    .Select(c => c.Id, c => c.Loss_dB, c => c.Freq_kHz)
                    .Where(c => c.EmittingId, ConditionOperator.Equal, reader.GetValue(c => c.Id));
                    queryExecuter.Fetch(querySignalMask, readerSignalMask =>
                    {
                        while (readerSignalMask.Read())
                        {
                            if (readerSignalMask.GetValue(c => c.Loss_dB).HasValue)
                                listLoss_dB.Add((float)readerSignalMask.GetValue(c => c.Loss_dB).Value);

                            if (readerSignalMask.GetValue(c => c.Freq_kHz).HasValue)
                                listFreq_kHz.Add(readerSignalMask.GetValue(c => c.Freq_kHz).Value);

                        }
                        return true;
                    });

                    var signalMask = new SignalMask();
                    if (listLoss_dB.Count > 0)
                        signalMask.Loss_dB = listLoss_dB.ToArray();
                    if (listFreq_kHz.Count > 0)
                        signalMask.Freq_kHz = listFreq_kHz.ToArray();

                    emitting.SignalMask = signalMask;

                    var listLevel = new List<int>();
                    var listCount = new List<int>();
                    var queryLevelDist = this._dataLayer.GetBuilder<MD.ILevelsDistribution>()
                    .From()
                    .Select(c => c.Id, c => c.level, c => c.count)
                    .Where(c => c.EmittingId, ConditionOperator.Equal, reader.GetValue(c => c.Id));
                    queryExecuter.Fetch(queryLevelDist, readerLevelDist =>
                    {
                        while (readerLevelDist.Read())
                        {
                            listLevel.Add(readerLevelDist.GetValue(c => c.level).Value);
                            listCount.Add(readerLevelDist.GetValue(c => c.count).Value);
                        }
                        return true;
                    });

                    var levelDist = new LevelsDistribution();
                    if (listLevel.Count > 0)
                        levelDist.Levels = listLevel.ToArray();
                    if (listCount.Count > 0)
                        levelDist.Count = listCount.ToArray();

                    emitting.LevelsDistribution = levelDist;
             
                    var spectrum = new Spectrum();
                    var listLevelsdBm = new List<float>();
                    var querySpectrum = this._dataLayer.GetBuilder<MD.ISpectrum>()
                    .From()
                    .Select(c => c.Id, c => c.Bandwidth_kHz, c => c.CorrectnessEstimations, c => c.MarkerIndex, c => c.SignalLevel_dBm, c => c.SpectrumStartFreq_MHz, c => c.SpectrumSteps_kHz, c => c.T1, c => c.T2, c => c.TraceCount)
                    .Where(c => c.EmittingId, ConditionOperator.Equal, reader.GetValue(c => c.Id));
                    queryExecuter.Fetch(querySpectrum, readerSpectrum =>
                    {
                        while (readerSpectrum.Read())
                        {

                            var queryLevelsdBm = this._dataLayer.GetBuilder<MD.IDetailSpectrumLevels>()
                            .From()
                            .Select(c => c.Id, c => c.level)
                            .Where(c => c.SpectrumId, ConditionOperator.Equal, readerSpectrum.GetValue(c => c.Id));
                            queryExecuter.Fetch(queryLevelsdBm, readerLevelsdBm =>
                            {
                                while (readerLevelsdBm.Read())
                                {
                                    if (readerLevelsdBm.GetValue(c => c.level).HasValue)
                                    {
                                        listLevelsdBm.Add((float)readerLevelsdBm.GetValue(c => c.level).Value);
                                    }
                                }
                                return true;
                            });

                            if (listLevelsdBm.Count > 0)
                                spectrum.Levels_dBm = listLevelsdBm.ToArray();


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
                                spectrum.SignalLevel_dBm = (float)readerSpectrum.GetValue(c => c.SignalLevel_dBm).Value;
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
                                spectrum.СorrectnessEstimations = readerSpectrum.GetValue(c => c.CorrectnessEstimations).Value;
                            }
                        }
                        return true;
                    });


                    spectrum.Levels_dBm = listLevelsdBm.ToArray();
                    emitting.Spectrum = spectrum;
                    listEmitting.Add(emitting);
                }
                return true;
            });

            if (listEmitting.Count > 0)
            {
                emittings = listEmitting.ToArray();
            }


            var level = new ReferenceLevels();
            var listLevels = new List<float>();
            var queryLevels = this._dataLayer.GetBuilder<MD.IReferenceLevels>()
            .From()
            .Select(c => c.Id, c => c.StartFrequency_Hz, c => c.StepFrequency_Hz)
            .Where(c => c.ResMeasId, ConditionOperator.Equal, resId);
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


                    var queryLevelsDet = this._dataLayer.GetBuilder<MD.IDetailReferenceLevels>()
                    .From()
                    .Select(c => c.Id, c => c.level)
                    .Where(c => c.ReferenceLevelId, ConditionOperator.Equal, readerLevels.GetValue(c => c.Id));
                    queryExecuter.Fetch(queryLevelsDet, readerLevelsDet =>
                    {
                        while (readerLevelsDet.Read())
                        {
                            if (readerLevelsDet.GetValue(c => c.level).HasValue)
                            {
                                listLevels.Add((float)readerLevelsDet.GetValue(c => c.level).Value);
                            }
                        }
                        return true;
                    });

                    if (listLevels.Count > 0)
                    {
                        level.levels = listLevels.ToArray();
                    }

                }
                return true;
            });

            referenceLevels = level;
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
                        if (readerResMeas.GetValue(c => c.MeasSubTaskStationId) != null)
                        {
                            levelmeasurementResults.Id.SubMeasTaskStationId = readerResMeas.GetValue(c => c.MeasSubTaskStationId).Value;
                        }
                        if (readerResMeas.GetValue(c => c.TimeMeas) != null)
                        {
                            levelmeasurementResults.TimeMeas = readerResMeas.GetValue(c => c.TimeMeas).Value;
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


                        levelmeasurementResults.CountStationMeasurements = 0;
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

                        Emitting[] emittings = null;
                        ReferenceLevels referenceLevels = null;
                        GetEmittingAndReferenceLevels(readerResMeas.GetValue(c => c.Id), out emittings, out referenceLevels);
                        levelmeasurementResults.Emittings = emittings;
                        levelmeasurementResults.RefLevels = referenceLevels;
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


                            levelmeasurementResults.CountStationMeasurements = 0;
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


                        levelmeasurementResults.CountStationMeasurements = 0;
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
                        var builderSensorPolygon = this._dataLayer.GetBuilder<MD.ISensorPolygon>().From();
                        builderSensorPolygon.Select(c => c.Id);
                        builderSensorPolygon.Select(c => c.Lon);
                        builderSensorPolygon.Select(c => c.Lat);
                        builderSensorPolygon.Where(c => c.SensorId, ConditionOperator.Equal, sensorId);
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
                                            levelSpectrum.Add((float)readerResStLevelsSpect.GetValue(c => c.LevelSpecrum));
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
                        builderResStGeneral.Select(c => c.TimeFinishMeas);
                        builderResStGeneral.Select(c => c.TimeStartMeas);
                        builderResStGeneral.Select(c => c.TraceCount);
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
                            }
                            return true;
                        });
                        resMeasStatiion.GeneralResult = measurementsParameterGeneral;
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
                                            levelSpectrum.Add((float)readerResStLevelsSpect.GetValue(c => c.LevelSpecrum));
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
        public MeasurementResults GetMeasurementResultByResId(int ResId)
        {
            var levelmeasurementResults = new MeasurementResults();
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
                        if (readerResMeas.GetValue(c => c.TimeMeas) != null)
                        {
                            levelmeasurementResults.TimeMeas = readerResMeas.GetValue(c => c.TimeMeas).Value;
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


                        levelmeasurementResults.CountStationMeasurements = 0;
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


                        Emitting[] emittings = null;
                        ReferenceLevels referenceLevels = null;
                        GetEmittingAndReferenceLevels(readerResMeas.GetValue(c => c.Id), out emittings, out referenceLevels);
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
                        if (readerResMeas.GetValue(c => c.TimeMeas) != null)
                        {
                            levelmeasurementResults.TimeMeas = readerResMeas.GetValue(c => c.TimeMeas).Value;
                        }
                        MeasurementType outResType;
                        if (Enum.TryParse<MeasurementType>(readerResMeas.GetValue(c => c.TypeMeasurements), out outResType))
                        {
                            levelmeasurementResults.TypeMeasurements = outResType;
                        }


                        Emitting[] emittings = null;
                        ReferenceLevels referenceLevels = null;
                        GetEmittingAndReferenceLevels(readerResMeas.GetValue(c => c.Id), out emittings, out referenceLevels);
                        levelmeasurementResults.Emittings = emittings;
                        levelmeasurementResults.RefLevels = referenceLevels;

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
                        if (readerResMeas.GetValue(c => c.TimeMeas) != null)
                        {
                            levelmeasurementResults.TimeMeas = readerResMeas.GetValue(c => c.TimeMeas).Value;
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
                                                    levelSpectrum.Add((float)readerResStLevelsSpect.GetValue(c => c.LevelSpecrum));
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


                        Emitting[] emittings = null;
                        ReferenceLevels referenceLevels = null;
                        GetEmittingAndReferenceLevels(readerResMeas.GetValue(c => c.Id), out emittings, out referenceLevels);
                        levelmeasurementResults.Emittings = emittings;


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


                        levelmeasurementResults.CountStationMeasurements = 0;
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


                        levelmeasurementResults.CountStationMeasurements = 0;
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


                        levelmeasurementResults.CountStationMeasurements = 0;
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

    }
}




