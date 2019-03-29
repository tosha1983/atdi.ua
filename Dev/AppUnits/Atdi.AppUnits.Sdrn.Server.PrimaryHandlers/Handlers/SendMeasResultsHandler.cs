
using Atdi.Contracts.Api.EventSystem;
using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Contracts.CoreServices.EntityOrm;
using Atdi.Contracts.Sdrn.Server;
using Atdi.DataModels.DataConstraint;
using Atdi.DataModels.Sdrns.Device;
using Atdi.Platform.Logging;
using System;
using MD = Atdi.DataModels.Sdrns.Server.Entities;
using MSG = Atdi.DataModels.Sdrns.BusMessages;
using MDE = Atdi.Modules.Sdrn.Server.Events;

namespace Atdi.AppUnits.Sdrn.Server.PrimaryHandlers.Handlers
{
    public class SendMeasResultsHandler : ISdrnMessageHandler<MSG.Device.SendMeasResultsMessage, MeasResults>
    {
        private readonly ISdrnMessagePublisher _messagePublisher;
        private readonly IDataLayer<EntityDataOrm> _dataLayer;
        private readonly ISdrnServerEnvironment _environment;
        private readonly IEventEmitter _eventEmitter;
        private readonly ILogger _logger;

        public SendMeasResultsHandler(
            ISdrnMessagePublisher messagePublisher, 
            IDataLayer<EntityDataOrm> dataLayer, 
            ISdrnServerEnvironment environment, 
            IEventEmitter eventEmitter, ILogger logger)
        {
            this._messagePublisher = messagePublisher;
            this._dataLayer = dataLayer;
            this._environment = environment;
            this._eventEmitter = eventEmitter;
            this._logger = logger;
        }
        /*
        public void GetMeasTaskSDRIdentifier(MeasResults measResults,  out int SubTaskId, out int SubTaskStationId, out int SensorId)
        {
            int? _SubTaskId = null;
            int? _SubTaskStationId = null;
            int? _SensorId = null;

            int valMeasTask = -1;
            Int32.TryParse(measResults.TaskId, out valMeasTask);
            var query = this._dataLayer.GetBuilder<MD.IMeasTaskSDR>()
                  .From()
                  .Select(c => c.MeasTaskId)
                  .Select(c => c.Id)
                  .Select(c => c.MeasSubTaskId)
                  .Select(c => c.MeasSubTaskStaId)
                  .Select(c => c.SensorId)
                  .Where(c => c.MeasTaskId, ConditionOperator.Equal, valMeasTask)
                  .OrderByAsc(c => c.Id)
                  ;

            var res = this._dataLayer.Executor<SdrnServerDataContext>()
            .Fetch(query, reader =>
            {
                var result = false;
                while (reader.Read())
                {
                    _SubTaskId = reader.GetValue(c => c.MeasSubTaskId);
                    _SubTaskStationId = reader.GetValue(c => c.MeasSubTaskStaId);
                    _SensorId = reader.GetValue(c => c.SensorId);
                    result = true;
                }
                return result;
            });
            SubTaskId = _SubTaskId.HasValue ? _SubTaskId.Value : -1;
            SubTaskStationId = _SubTaskStationId.HasValue ? _SubTaskStationId.Value : -1;
            SensorId = _SensorId.HasValue ? _SensorId.Value : -1;
        }
        */

        public void GetMeasTaskSDRIdentifier(MeasResults measResults, out int SubTaskId, out int SubTaskStationId, out int SensorId)
        {
            int? _SubTaskId = null;
            int? _SubTaskStationId = null;
            int? _SensorId = null;

            int valMeasTask = -1;
            Int32.TryParse(measResults.TaskId, out valMeasTask);
            var query = this._dataLayer.GetBuilder<MD.IMeasSubTaskSta>()
                  .From()
                  .Select(c => c.Id)
                  .Select(c => c.SensorId)
                  .Select(c => c.MeasSubTaskId)
                  .Where(c => c.MEASSUBTASK.MeasTaskId, ConditionOperator.Equal, valMeasTask)
                  .OrderByAsc(c => c.Id)
                  ;

            var res = this._dataLayer.Executor<SdrnServerDataContext>()
            .Fetch(query, reader =>
            {
                var result = false;
                while (reader.Read())
                {
                    _SubTaskId = reader.GetValue(c => c.MeasSubTaskId);
                    _SubTaskStationId = reader.GetValue(c => c.Id);
                    _SensorId = reader.GetValue(c => c.SensorId);
                    result = true;
                }
                return result;
            });
            SubTaskId = _SubTaskId.HasValue ? _SubTaskId.Value : -1;
            SubTaskStationId = _SubTaskStationId.HasValue ? _SubTaskStationId.Value : -1;
            SensorId = _SensorId.HasValue ? _SensorId.Value : -1;
        }

        public void Handle(ISdrnIncomingEnvelope<MeasResults> incomingEnvelope, ISdrnMessageHandlingResult result)
        {
            using (this._logger.StartTrace(Contexts.PrimaryHandler, Categories.MessageProcessing, this))
            {
                var queryExecuter = this._dataLayer.Executor<SdrnServerDataContext>();
                result.Status = SdrnMessageHandlingStatus.Unprocessed;
                int valInsResMeas = 0;
                try
                {
                    queryExecuter.BeginTransaction();
                    result.Status = SdrnMessageHandlingStatus.Trash;
                    var resObject = incomingEnvelope.DeliveryObject;
                    int SensorId; int SubMeasTaskId; int SubMeasTaskStationId;
                    GetMeasTaskSDRIdentifier(resObject, out SubMeasTaskId, out SubMeasTaskStationId, out SensorId);

                    var builderInsertIResMeas = this._dataLayer.GetBuilder<MD.IResMeasRaw>().Insert();
                    builderInsertIResMeas.SetValue(c => c.TimeMeas, resObject.Measured);
                    builderInsertIResMeas.SetValue(c => c.MeasTaskId, resObject.TaskId);
                    builderInsertIResMeas.SetValue(c => c.SensorId, SensorId);
                    builderInsertIResMeas.SetValue(c => c.MeasSubTaskId, SubMeasTaskId);
                    builderInsertIResMeas.SetValue(c => c.MeasSubTaskStationId, SubMeasTaskStationId);
                    builderInsertIResMeas.SetValue(c => c.DataRank, resObject.SwNumber);
                    builderInsertIResMeas.SetValue(c => c.Status, resObject.Status);
                    builderInsertIResMeas.SetValue(c => c.TypeMeasurements, resObject.Measurement.ToString());
                    builderInsertIResMeas.SetValue(c => c.MeasResultSID, resObject.ResultId);
                    builderInsertIResMeas.SetValue(c => c.StartTime, resObject.StartTime);
                    builderInsertIResMeas.SetValue(c => c.StopTime, resObject.StopTime);
                    builderInsertIResMeas.SetValue(c => c.ScansNumber, resObject.ScansNumber);
                    builderInsertIResMeas.Select(c => c.Id);
                    queryExecuter
                    .ExecuteAndFetch(builderInsertIResMeas, reader =>
                    {
                        var res = reader.Read();
                       if (res)
                       {
                           valInsResMeas = reader.GetValue(c => c.Id);
                       }
                       return res;
                   });

                    if (valInsResMeas > 0)
                    {
                        var builderUpdateIResMeas = this._dataLayer.GetBuilder<MD.IResMeasRaw>().Update();
                        builderUpdateIResMeas.SetValue(c => c.MeasResultSID, valInsResMeas.ToString());
                        builderUpdateIResMeas.Where(c => c.Id, ConditionOperator.Equal, valInsResMeas);
                        var cnt = queryExecuter.Execute(builderUpdateIResMeas);

                        if (resObject.RefLevels != null)
                        {
                            int valInsReferenceLevelsRaw = 0;
                            var refLevels = resObject.RefLevels;
                            var builderInsertReferenceLevelsRaw = this._dataLayer.GetBuilder<MD.IReferenceLevelsRaw>().Insert();
                            builderInsertReferenceLevelsRaw.SetValue(c => c.StartFrequency_Hz, refLevels.StartFrequency_Hz);
                            builderInsertReferenceLevelsRaw.SetValue(c => c.StepFrequency_Hz, refLevels.StepFrequency_Hz);
                            builderInsertReferenceLevelsRaw.SetValue(c => c.ResMeasId, valInsResMeas);
                            builderInsertReferenceLevelsRaw.Select(c => c.Id);
                            queryExecuter
                            .ExecuteAndFetch(builderInsertReferenceLevelsRaw, readerReferenceLevelsRaw =>
                            {
                                var res = readerReferenceLevelsRaw.Read();
                                if (res)
                                {
                                    valInsReferenceLevelsRaw = readerReferenceLevelsRaw.GetValue(c => c.Id);
                                    if (valInsReferenceLevelsRaw > 0)
                                    {
                                        var lstInslevels = new IQueryInsertStatement<MD.IDetailReferenceLevelsRaw>[refLevels.levels.Length];
                                        for (int l = 0; l < refLevels.levels.Length; l++)
                                        {
                                            var lvl = refLevels.levels[l];

                                            var builderInsertDetailReferenceLevelsRaw = this._dataLayer.GetBuilder<MD.IDetailReferenceLevelsRaw>().Insert();
                                            builderInsertDetailReferenceLevelsRaw.SetValue(c => c.level, lvl);
                                            builderInsertDetailReferenceLevelsRaw.SetValue(c => c.ReferenceLevelId, valInsReferenceLevelsRaw);
                                            builderInsertDetailReferenceLevelsRaw.Select(c => c.Id);
                                            lstInslevels[l] = builderInsertDetailReferenceLevelsRaw;
                                        }
                                        queryExecuter.ExecuteAndFetch(lstInslevels, readerDetailReferenceLevelsRaw =>
                                        {
                                            return true;
                                        });
                                    }

                                }
                                return true;
                            });
                        }

                        if (resObject.Emittings != null)
                        {
                            var emittings = resObject.Emittings;
                            for (int l = 0; l < emittings.Length; l++)
                            {
                                int valInsReferenceEmittingRaw = 0;
                                var builderInsertEmittingRaw = this._dataLayer.GetBuilder<MD.IEmittingRaw>().Insert();
                                builderInsertEmittingRaw.SetValue(c => c.CurentPower_dBm, emittings[l].CurentPower_dBm);
                                builderInsertEmittingRaw.SetValue(c => c.MeanDeviationFromReference, emittings[l].MeanDeviationFromReference);
                                builderInsertEmittingRaw.SetValue(c => c.ReferenceLevel_dBm, emittings[l].ReferenceLevel_dBm);
                                builderInsertEmittingRaw.SetValue(c => c.ResMeasId, valInsResMeas);
                                if (emittings[l].EmittingParameters != null)
                                {
                                    builderInsertEmittingRaw.SetValue(c => c.RollOffFactor, emittings[l].EmittingParameters.RollOffFactor);
                                    builderInsertEmittingRaw.SetValue(c => c.StandardBW, emittings[l].EmittingParameters.StandardBW);
                                }
                                builderInsertEmittingRaw.SetValue(c => c.StartFrequency_MHz, emittings[l].StartFrequency_MHz);
                                builderInsertEmittingRaw.SetValue(c => c.StopFrequency_MHz, emittings[l].StopFrequency_MHz);
                                builderInsertEmittingRaw.SetValue(c => c.TriggerDeviationFromReference, emittings[l].TriggerDeviationFromReference);
                                builderInsertEmittingRaw.Select(c => c.Id);
                                queryExecuter
                                .ExecuteAndFetch(builderInsertEmittingRaw, readerEmittingRaw =>
                                {
                                    var res = readerEmittingRaw.Read();
                                    if (res)
                                    {
                                        valInsReferenceEmittingRaw = readerEmittingRaw.GetValue(c=>c.Id);
                                        if (valInsReferenceEmittingRaw > 0)
                                        {
                                            var workTimes = emittings[l].WorkTimes;
                                            if (workTimes != null)
                                            {
                                                var lstInsWorkTimeRaw = new IQueryInsertStatement<MD.IWorkTimeRaw>[workTimes.Length];
                                                for (int r = 0; r < workTimes.Length; r++)
                                                {
                                                    var builderInsertIWorkTimeRaw = this._dataLayer.GetBuilder<MD.IWorkTimeRaw>().Insert();
                                                    builderInsertIWorkTimeRaw.SetValue(c => c.EmittingId, valInsReferenceEmittingRaw);
                                                    builderInsertIWorkTimeRaw.SetValue(c => c.HitCount, workTimes[r].HitCount);
                                                    builderInsertIWorkTimeRaw.SetValue(c => c.PersentAvailability, workTimes[r].PersentAvailability);
                                                    builderInsertIWorkTimeRaw.SetValue(c => c.StartEmitting, workTimes[r].StartEmitting);
                                                    builderInsertIWorkTimeRaw.SetValue(c => c.StopEmitting, workTimes[r].StopEmitting);
                                                    builderInsertIWorkTimeRaw.Select(c => c.Id);
                                                    lstInsWorkTimeRaw[r] = builderInsertIWorkTimeRaw;
                                                }
                                                queryExecuter.ExecuteAndFetch(lstInsWorkTimeRaw, readerWorkTimeRaw =>
                                                {
                                                    return true;
                                                });
                                            }

                                            var spectrum = emittings[l].Spectrum;
                                            if (spectrum != null)
                                            {
                                                int valInsSpectrumRaw = 0;

                                                var builderInsertISpectrumRaw = this._dataLayer.GetBuilder<MD.ISpectrumRaw>().Insert();
                                                builderInsertISpectrumRaw.SetValue(c => c.EmittingId, valInsReferenceEmittingRaw);
                                                builderInsertISpectrumRaw.SetValue(c => c.CorrectnessEstimations, spectrum.СorrectnessEstimations);
                                                builderInsertISpectrumRaw.SetValue(c => c.Bandwidth_kHz, spectrum.Bandwidth_kHz);
                                                builderInsertISpectrumRaw.SetValue(c => c.MarkerIndex, spectrum.MarkerIndex);
                                                builderInsertISpectrumRaw.SetValue(c => c.SignalLevel_dBm, spectrum.SignalLevel_dBm);
                                                builderInsertISpectrumRaw.SetValue(c => c.SpectrumStartFreq_MHz, spectrum.SpectrumStartFreq_MHz);
                                                builderInsertISpectrumRaw.SetValue(c => c.SpectrumSteps_kHz, spectrum.SpectrumSteps_kHz);
                                                builderInsertISpectrumRaw.SetValue(c => c.T1, spectrum.T1);
                                                builderInsertISpectrumRaw.SetValue(c => c.T2, spectrum.T2);
                                                builderInsertISpectrumRaw.SetValue(c => c.TraceCount, spectrum.TraceCount);
                                                builderInsertISpectrumRaw.Select(c => c.Id);
                                                queryExecuter
                                                .ExecuteAndFetch(builderInsertISpectrumRaw, readerISpectrumRaw =>
                                                {
                                                    var resSpectrumRaw = readerISpectrumRaw.Read();
                                                    if (resSpectrumRaw)
                                                    {
                                                        valInsSpectrumRaw = readerISpectrumRaw.GetValue(c => c.Id);
                                                        if (valInsSpectrumRaw > 0)
                                                        {
                                                            var lstInsLevels_dBm = new IQueryInsertStatement<MD.IDetailSpectrumLevelsRaw>[spectrum.Levels_dBm.Length];
                                                            for (int k = 0; k < spectrum.Levels_dBm.Length; k++)
                                                            {
                                                                var level_dBm = spectrum.Levels_dBm[k];

                                                                var builderInsertIDetailReferenceLevelsRaw = this._dataLayer.GetBuilder<MD.IDetailSpectrumLevelsRaw>().Insert();
                                                                builderInsertIDetailReferenceLevelsRaw.SetValue(c => c.level, level_dBm);
                                                                builderInsertIDetailReferenceLevelsRaw.SetValue(c => c.SpectrumId, valInsSpectrumRaw);
                                                                builderInsertIDetailReferenceLevelsRaw.Select(c => c.Id);
                                                                lstInsLevels_dBm[k] = builderInsertIDetailReferenceLevelsRaw;
                                                            }
                                                            queryExecuter.ExecuteAndFetch(lstInsLevels_dBm, readerDetailSpectrumLevelsRaw =>
                                                            {
                                                                return true;
                                                            });
                                                        }
                                                    }
                                                    return true;
                                                });
                                            }

                                            var signalMask = emittings[l].SignalMask;
                                            if (signalMask != null)
                                            {
                                                var lstInsSignalMaskRaw = new IQueryInsertStatement<MD.ISignalMaskRaw>[signalMask.Freq_kHz.Length];
                                                for (int k = 0; k < signalMask.Freq_kHz.Length; k++)
                                                {
                                                    var freq_kH = signalMask.Freq_kHz[k];
                                                    var loss_dB = signalMask.Loss_dB[k];

                                                    var builderInsertSignalMaskRaw = this._dataLayer.GetBuilder<MD.ISignalMaskRaw>().Insert();
                                                    builderInsertSignalMaskRaw.SetValue(c => c.Freq_kHz, freq_kH);
                                                    builderInsertSignalMaskRaw.SetValue(c => c.Loss_dB, loss_dB);
                                                    builderInsertSignalMaskRaw.SetValue(c => c.EmittingId, valInsReferenceEmittingRaw);
                                                    builderInsertSignalMaskRaw.Select(c => c.Id);
                                                    lstInsSignalMaskRaw[k] = builderInsertSignalMaskRaw;
                                                }
                                                queryExecuter.ExecuteAndFetch(lstInsSignalMaskRaw, readerSignalMaskRaw =>
                                                {
                                                    return true;
                                                });
                                            }

                                            var levelsDistribution = emittings[l].LevelsDistribution;
                                            if (levelsDistribution!=null)
                                            {
                                                var lstInsLevelsDistributionRaw = new IQueryInsertStatement<MD.ILevelsDistributionRaw>[levelsDistribution.Levels.Length];
                                                for (int k = 0; k < levelsDistribution.Levels.Length; k++)
                                                {
                                                    var lvl = levelsDistribution.Levels[k];
                                                    var count = levelsDistribution.Count[k];
                                                    var builderInsertLevelsDistributionRaw = this._dataLayer.GetBuilder<MD.ILevelsDistributionRaw>().Insert();
                                                    builderInsertLevelsDistributionRaw.SetValue(c => c.level, lvl);
                                                    builderInsertLevelsDistributionRaw.SetValue(c => c.count, count);
                                                    builderInsertLevelsDistributionRaw.SetValue(c => c.EmittingId, valInsReferenceEmittingRaw);
                                                    builderInsertLevelsDistributionRaw.Select(c => c.Id);
                                                    lstInsLevelsDistributionRaw[k] = builderInsertLevelsDistributionRaw;
                                                }
                                                queryExecuter.ExecuteAndFetch(lstInsLevelsDistributionRaw, readerLevelsDistributionRaw =>
                                                {
                                                    return true;
                                                });
                                            }
                                        }
                                    }
                                    return true;
                                });
                            }
                        }



                        if (resObject.BandwidthResult != null)
                        {
                            int valInsBWMeasResultRaw = 0;
                            var builderInsertBWMeasResultRaw = this._dataLayer.GetBuilder<MD.IBWMeasResultRaw>().Insert();
                            builderInsertBWMeasResultRaw.SetValue(c => c.BW_kHz, resObject.BandwidthResult.Bandwidth_kHz);
                            builderInsertBWMeasResultRaw.SetValue(c => c.MarkerIndex, resObject.BandwidthResult.MarkerIndex);
                            builderInsertBWMeasResultRaw.SetValue(c => c.T1, resObject.BandwidthResult.T1);
                            builderInsertBWMeasResultRaw.SetValue(c => c.T2, resObject.BandwidthResult.T2);
                            builderInsertBWMeasResultRaw.SetValue(c => c.TraceCount, resObject.BandwidthResult.TraceCount);
                            builderInsertBWMeasResultRaw.SetValue(c => c.Сorrectnessestim, resObject.BandwidthResult.СorrectnessEstimations == true ? 1 : 0);
                            builderInsertBWMeasResultRaw.SetValue(c => c.ResMeasId, valInsResMeas);
                            builderInsertBWMeasResultRaw.Select(c => c.Id);
                            queryExecuter
                            .ExecuteAndFetch(builderInsertBWMeasResultRaw, reader =>
                            {
                                var res = reader.Read();
                                if (res)
                                {
                                    valInsBWMeasResultRaw = reader.GetValue(c => c.Id);
                                }
                                return res;
                            });
                        }

                        if (resObject.Frequencies != null)
                        {
                            var lstIns = new IQueryInsertStatement<MD.IResFreqLevelsRaw>[resObject.Frequencies.Length];
                            for (int i = 0; i < resObject.Frequencies.Length; i++)
                            {
                                var builderInsertResFreqLevelsRaw = this._dataLayer.GetBuilder<MD.IResFreqLevelsRaw>().Insert();
                                builderInsertResFreqLevelsRaw.SetValue(c => c.Freq_MHz, resObject.Frequencies[i]);
                                builderInsertResFreqLevelsRaw.SetValue(c => c.ResMeasId, valInsResMeas);
                                builderInsertResFreqLevelsRaw.Select(c => c.Id);
                                lstIns[i] = builderInsertResFreqLevelsRaw;

                            }
                            queryExecuter.ExecuteAndFetch(lstIns, reader =>
                            {
                                return true;
                            });
                            
                        }


                        if (resObject.Levels_dBm != null)
                        {
                            var lstIns = new IQueryInsertStatement<MD.IResFreqLevelsRaw>[resObject.Levels_dBm.Length];
                            for (int i = 0; i < resObject.Levels_dBm.Length; i++)
                            {
                                var builderInsertResFreqLevelsRaw = this._dataLayer.GetBuilder<MD.IResFreqLevelsRaw>().Insert();
                                builderInsertResFreqLevelsRaw.SetValue(c => c.Level_dBm, resObject.Levels_dBm[i]);
                                builderInsertResFreqLevelsRaw.SetValue(c => c.ResMeasId, valInsResMeas);
                                builderInsertResFreqLevelsRaw.Select(c => c.Id);
                                lstIns[i] = builderInsertResFreqLevelsRaw;
                            }
                            queryExecuter.ExecuteAndFetch(lstIns, reader =>
                            {
                                return true;
                            });
                        }

                        if (resObject.Location != null)
                        {
                            int valInsResLocSensorMeas = 0;
                            var builderInsertResLocSensorMeas = this._dataLayer.GetBuilder<MD.IResLocSensorRaw>().Insert();
                            builderInsertResLocSensorMeas.SetValue(c => c.Agl, resObject.Location.AGL);
                            builderInsertResLocSensorMeas.SetValue(c => c.Asl, resObject.Location.ASL);
                            builderInsertResLocSensorMeas.SetValue(c => c.Lon, resObject.Location.Lon);
                            builderInsertResLocSensorMeas.SetValue(c => c.Lat, resObject.Location.Lat);
                            builderInsertResLocSensorMeas.SetValue(c => c.ResMeasId, valInsResMeas);
                            builderInsertResLocSensorMeas.Select(c => c.Id);
                            queryExecuter
                            .ExecuteAndFetch(builderInsertResLocSensorMeas, reader =>
                            {
                                var res = reader.Read();
                                if (res)
                                {
                                    valInsResLocSensorMeas = reader.GetValue(c => c.Id);
                                }
                                return res;
                            });
                        }

                        if (resObject.FrequencySamples != null)
                        {
                            var lstIns = new IQueryInsertStatement<MD.IFreqSampleRaw>[resObject.FrequencySamples.Length];
                            for (int i = 0; i < resObject.FrequencySamples.Length; i++)
                            {
                                var item = resObject.FrequencySamples[i];
                                var builderInsertFreqSampleRaw = this._dataLayer.GetBuilder<MD.IFreqSampleRaw>().Insert();
                                builderInsertFreqSampleRaw.SetValue(c => c.Freq_MHz, item.Freq_MHz);
                                builderInsertFreqSampleRaw.SetValue(c => c.LevelMax_dBm, item.LevelMax_dBm);
                                builderInsertFreqSampleRaw.SetValue(c => c.LevelMin_dBm, item.LevelMin_dBm);
                                builderInsertFreqSampleRaw.SetValue(c => c.Level_dBm, item.Level_dBm);
                                builderInsertFreqSampleRaw.SetValue(c => c.Level_dBmkVm, item.Level_dBmkVm);
                                builderInsertFreqSampleRaw.SetValue(c => c.OccupationPt, item.Occupation_Pt);
                                builderInsertFreqSampleRaw.SetValue(c => c.ResMeasId, valInsResMeas);
                                builderInsertFreqSampleRaw.Select(c => c.Id);
                                lstIns[i] = builderInsertFreqSampleRaw;
                            }
                            queryExecuter.ExecuteAndFetch(lstIns, reader =>
                            {
                                return true;
                            });
                        }

                        if (resObject.Routes != null)
                        {

                            for (int l = 0; l < resObject.Routes.Length; l++)
                            {
                                Route route = resObject.Routes[l];

                                if (route.RoutePoints != null)
                                {
                                    var lstIns = new IQueryInsertStatement<MD.IResRoutesRaw>[route.RoutePoints.Length];
                                    for (int j = 0; j < route.RoutePoints.Length; j++)
                                    {
                                        var routePoint = route.RoutePoints[j];
                                        var builderInsertroutePoints = this._dataLayer.GetBuilder<MD.IResRoutesRaw>().Insert();
                                        builderInsertroutePoints.SetValue(c => c.Agl, routePoint.AGL);
                                        builderInsertroutePoints.SetValue(c => c.Asl, routePoint.ASL);
                                        builderInsertroutePoints.SetValue(c => c.FinishTime, routePoint.FinishTime);
                                        builderInsertroutePoints.SetValue(c => c.StartTime, routePoint.StartTime);
                                        builderInsertroutePoints.SetValue(c => c.RouteId, route.RouteId);
                                        builderInsertroutePoints.SetValue(c => c.PointStayType, routePoint.PointStayType.ToString());
                                        builderInsertroutePoints.SetValue(c => c.Lat, routePoint.Lat);
                                        builderInsertroutePoints.SetValue(c => c.Lon, routePoint.Lon);
                                        builderInsertroutePoints.SetValue(c => c.ResMeasId, valInsResMeas);
                                        builderInsertroutePoints.Select(c => c.Id);
                                        lstIns[j] = builderInsertroutePoints;
                                    }
                                    queryExecuter.ExecuteAndFetch(lstIns, reader =>
                                    {
                                        return true;
                                    });
                                }
                            }
                        }




                        if (resObject.StationResults != null)
                        {
                            for (int n = 0; n < resObject.StationResults.Length; n++)
                            {
                                int valInsResMeasStation = 0;
                                int Idstation; int IdSector;
                                StationMeasResult station = resObject.StationResults[n];
                                var builderInsertResMeasStation = this._dataLayer.GetBuilder<MD.IResMeasStaRaw>().Insert();
                                builderInsertResMeasStation.SetValue(c => c.Status, station.Status);
                                builderInsertResMeasStation.SetValue(c => c.MeasGlobalSID, station.RealGlobalSid);
                                builderInsertResMeasStation.SetValue(c => c.ResMeasId, valInsResMeas);
                                builderInsertResMeasStation.SetValue(c => c.Standard, station.Standard);
                                if (int.TryParse(station.StationId, out Idstation))
                                {
                                    builderInsertResMeasStation.SetValue(c => c.StationId, Idstation);
                                }
                                if (int.TryParse(station.SectorId, out IdSector))
                                {
                                    builderInsertResMeasStation.SetValue(c => c.SectorId, IdSector);
                                }
                                builderInsertResMeasStation.Select(c => c.Id);

                                queryExecuter
                               .ExecuteAndFetch(builderInsertResMeasStation, reader =>
                                {
                                    var res = reader.Read();
                                    if (res)
                                    {
                                        valInsResMeasStation = reader.GetValue(c => c.Id);
                                    }
                                    return res;
                                });


                                if (valInsResMeasStation > 0)
                                {
                                    if (station.Bearings != null)
                                    {
                                        if (station.Bearings.Length > 0)
                                        {
                                            var listBearings = station.Bearings;
                                            var lstInsBearingRaw = new IQueryInsertStatement<MD.IBearingRaw>[listBearings.Length];
                                            for (int p = 0; p < listBearings.Length; p++)
                                            {
                                                DirectionFindingData directionFindingData = listBearings[p];
                                                var builderInsertBearingRaw = this._dataLayer.GetBuilder<MD.IBearingRaw>().Insert();
                                                builderInsertBearingRaw.SetValue(c => c.ResMeasStaId, valInsResMeasStation);
                                                if (directionFindingData.Location != null)
                                                {
                                                    builderInsertBearingRaw.SetValue(c => c.Agl, directionFindingData.Location.AGL);
                                                    builderInsertBearingRaw.SetValue(c => c.Asl, directionFindingData.Location.ASL);
                                                    builderInsertBearingRaw.SetValue(c => c.Lon, directionFindingData.Location.Lon);
                                                    builderInsertBearingRaw.SetValue(c => c.Lat, directionFindingData.Location.Lat);
                                                }

                                                builderInsertBearingRaw.SetValue(c => c.Level_dBm, directionFindingData.Level_dBm);
                                                builderInsertBearingRaw.SetValue(c => c.Level_dBmkVm, directionFindingData.Level_dBmkVm);
                                                builderInsertBearingRaw.SetValue(c => c.MeasurementTime, directionFindingData.MeasurementTime);
                                                builderInsertBearingRaw.SetValue(c => c.Quality, directionFindingData.Quality);
                                                builderInsertBearingRaw.SetValue(c => c.AntennaAzimut, directionFindingData.AntennaAzimut);
                                                builderInsertBearingRaw.SetValue(c => c.Bandwidth_kHz, directionFindingData.Bandwidth_kHz);
                                                builderInsertBearingRaw.SetValue(c => c.Bearing, directionFindingData.Bearing);
                                                builderInsertBearingRaw.SetValue(c => c.CentralFrequency_MHz, directionFindingData.CentralFrequency_MHz);
                                                builderInsertBearingRaw.Select(c => c.Id);
                                                lstInsBearingRaw[p] = builderInsertBearingRaw;
                                            }

                                            queryExecuter.ExecuteAndFetch(lstInsBearingRaw, reader =>
                                            {
                                                return true;
                                            });
                                        }
                                    }


                                    int StationId;
                                    int idLinkRes = -1;
                                    var builderInsertLinkResSensor = this._dataLayer.GetBuilder<MD.ILinkResSensorRaw>().Insert();
                                    builderInsertLinkResSensor.SetValue(c => c.ResMeasStaId, valInsResMeasStation);
                                    if (int.TryParse(station.StationId, out StationId))
                                    {
                                        builderInsertLinkResSensor.SetValue(c => c.SensorId, StationId);
                                    }
                                    builderInsertLinkResSensor.Select(c => c.Id);
                                    queryExecuter
                                    .ExecuteAndFetch(builderInsertLinkResSensor, reader =>
                                    {
                                        var res = reader.Read();
                                        if (res)
                                        {
                                            idLinkRes = reader.GetValue(c => c.Id);
                                        }
                                        return res;
                                    });


                                    var generalResult = station.GeneralResult;
                                    if (generalResult != null)
                                    {
                                        int IDResGeneral = -1;
                                        var builderInsertResStGeneral = this._dataLayer.GetBuilder<MD.IResStGeneralRaw>().Insert();
                                        builderInsertResStGeneral.SetValue(c => c.CentralFrequencyMeas, generalResult.CentralFrequencyMeas_MHz);
                                        builderInsertResStGeneral.SetValue(c => c.CentralFrequency, generalResult.CentralFrequency_MHz);
                                        builderInsertResStGeneral.SetValue(c => c.DurationMeas, generalResult.MeasDuration_sec);
                                        builderInsertResStGeneral.SetValue(c => c.Rbw, generalResult.RBW_kHz);
                                        builderInsertResStGeneral.SetValue(c => c.Vbw, generalResult.VBW_kHz);

                                        if (generalResult.BandwidthResult != null)
                                        {
                                            var bandwidthResult = generalResult.BandwidthResult;
                                            builderInsertResStGeneral.SetValue(c => c.MarkerIndex, bandwidthResult.MarkerIndex);
                                            builderInsertResStGeneral.SetValue(c => c.T1, bandwidthResult.T1);
                                            builderInsertResStGeneral.SetValue(c => c.T2, bandwidthResult.T2);
                                            builderInsertResStGeneral.SetValue(c => c.TraceCount, bandwidthResult.TraceCount);
                                            builderInsertResStGeneral.SetValue(c => c.Correctnessestim, bandwidthResult.СorrectnessEstimations);
                                        }
                                        builderInsertResStGeneral.SetValue(c => c.OffsetFrequency, generalResult.OffsetFrequency_mk);
                                        builderInsertResStGeneral.SetValue(c => c.SpecrumStartFreq, Convert.ToDouble(generalResult.SpectrumStartFreq_MHz));
                                        builderInsertResStGeneral.SetValue(c => c.SpecrumSteps, Convert.ToDouble(generalResult.SpectrumSteps_kHz));
                                        builderInsertResStGeneral.SetValue(c => c.TimeFinishMeas, generalResult.MeasFinishTime);
                                        builderInsertResStGeneral.SetValue(c => c.TimeStartMeas, generalResult.MeasStartTime);
                                        builderInsertResStGeneral.SetValue(c => c.ResMeasStaId, valInsResMeasStation);
                                        builderInsertResStGeneral.Select(c => c.Id);
                                        queryExecuter
                                        .ExecuteAndFetch(builderInsertResStGeneral, reader =>
                                        {
                                            var res = reader.Read();
                                            if (res)
                                            {
                                                IDResGeneral = reader.GetValue(c => c.Id);
                                            }
                                            return res;
                                        });


                                        if (IDResGeneral > 0)
                                        {
                                            if (station.GeneralResult.StationSysInfo != null)
                                            {
                                                var stationSysInfo = station.GeneralResult.StationSysInfo;
                                                int IDResSysInfoGeneral = -1;
                                                var builderInsertResSysInfo = this._dataLayer.GetBuilder<MD.IResSysInfoRaw>().Insert();
                                                if (stationSysInfo.Location != null)
                                                {
                                                    var stationSysInfoLocation = stationSysInfo.Location;
                                                    builderInsertResSysInfo.SetValue(c => c.Agl, stationSysInfoLocation.AGL);
                                                    builderInsertResSysInfo.SetValue(c => c.Asl, stationSysInfoLocation.ASL);
                                                    builderInsertResSysInfo.SetValue(c => c.Lat, stationSysInfoLocation.Lat);
                                                    builderInsertResSysInfo.SetValue(c => c.Lon, stationSysInfoLocation.Lon);
                                                }
                                                builderInsertResSysInfo.SetValue(c => c.Bandwidth, stationSysInfo.BandWidth);
                                                builderInsertResSysInfo.SetValue(c => c.BaseId, stationSysInfo.BaseID);
                                                builderInsertResSysInfo.SetValue(c => c.Bsic, stationSysInfo.BSIC);
                                                builderInsertResSysInfo.SetValue(c => c.ChannelNumber, stationSysInfo.ChannelNumber);
                                                builderInsertResSysInfo.SetValue(c => c.Cid, stationSysInfo.CID);
                                                builderInsertResSysInfo.SetValue(c => c.Code, stationSysInfo.Code);
                                                builderInsertResSysInfo.SetValue(c => c.Ctoi, stationSysInfo.CtoI);
                                                builderInsertResSysInfo.SetValue(c => c.Eci, stationSysInfo.ECI);
                                                builderInsertResSysInfo.SetValue(c => c.Enodebid, stationSysInfo.eNodeBId);
                                                builderInsertResSysInfo.SetValue(c => c.Freq, stationSysInfo.Freq);
                                                builderInsertResSysInfo.SetValue(c => c.Icio, stationSysInfo.IcIo);
                                                builderInsertResSysInfo.SetValue(c => c.InbandPower, stationSysInfo.INBAND_POWER);
                                                builderInsertResSysInfo.SetValue(c => c.Iscp, stationSysInfo.ISCP);
                                                builderInsertResSysInfo.SetValue(c => c.Lac, stationSysInfo.LAC);
                                                builderInsertResSysInfo.SetValue(c => c.Mcc, stationSysInfo.MCC);
                                                builderInsertResSysInfo.SetValue(c => c.Mnc, stationSysInfo.MNC);
                                                builderInsertResSysInfo.SetValue(c => c.Nid, stationSysInfo.NID);
                                                builderInsertResSysInfo.SetValue(c => c.Pci, stationSysInfo.PCI);
                                                builderInsertResSysInfo.SetValue(c => c.Pn, stationSysInfo.PN);
                                                builderInsertResSysInfo.SetValue(c => c.Power, stationSysInfo.Power);
                                                builderInsertResSysInfo.SetValue(c => c.Ptotal, stationSysInfo.Ptotal);
                                                builderInsertResSysInfo.SetValue(c => c.Rnc, stationSysInfo.RNC);
                                                builderInsertResSysInfo.SetValue(c => c.Rscp, stationSysInfo.RSCP);
                                                builderInsertResSysInfo.SetValue(c => c.Rsrp, stationSysInfo.RSRP);
                                                builderInsertResSysInfo.SetValue(c => c.Rsrq, stationSysInfo.RSRQ);
                                                builderInsertResSysInfo.SetValue(c => c.Sc, stationSysInfo.SC);
                                                builderInsertResSysInfo.SetValue(c => c.Sid, stationSysInfo.SID);
                                                builderInsertResSysInfo.SetValue(c => c.Tac, stationSysInfo.TAC);
                                                builderInsertResSysInfo.SetValue(c => c.TypeCdmaevdo, stationSysInfo.TypeCDMAEVDO);
                                                builderInsertResSysInfo.SetValue(c => c.Ucid, stationSysInfo.UCID);
                                                builderInsertResSysInfo.SetValue(c => c.ResStGeneralId, IDResGeneral);
                                                builderInsertResSysInfo.Select(c => c.Id);

                                                queryExecuter
                                                .ExecuteAndFetch(builderInsertResSysInfo, reader =>
                                                {
                                                    var res = reader.Read();
                                                    if (res)
                                                    {
                                                        IDResSysInfoGeneral = reader.GetValue(c => c.Id);
                                                    }
                                                    return res;
                                                });


                                                if (IDResSysInfoGeneral > 0)
                                                {
                                                    if (stationSysInfo.InfoBlocks != null)
                                                    {
                                                        for (int b=0; b< stationSysInfo.InfoBlocks.Length; b++)
                                                        {
                                                            StationSysInfoBlock blocks = stationSysInfo.InfoBlocks[b];

                                                            int IDResSysInfoBlocks = -1;
                                                            var builderInsertStationSysInfoBlock = this._dataLayer.GetBuilder<MD.IResSysInfoBlsRaw>().Insert();
                                                            builderInsertStationSysInfoBlock.SetValue(c => c.Data, blocks.Data);
                                                            builderInsertStationSysInfoBlock.SetValue(c => c.Type, blocks.Type);
                                                            builderInsertStationSysInfoBlock.SetValue(c => c.ResSysInfoId, IDResSysInfoGeneral);
                                                            builderInsertStationSysInfoBlock.Select(c => c.Id);
                                                            queryExecuter
                                                            .ExecuteAndFetch(builderInsertStationSysInfoBlock, reader =>
                                                            {
                                                                var res = reader.Read();
                                                                if (res)
                                                                {
                                                                    IDResSysInfoBlocks = reader.GetValue(c => c.Id);
                                                                }
                                                                return res;
                                                            });
                                                        }
                                                    }
                                                }
                                            }

                                            if (station.GeneralResult.BWMask != null)
                                            {
                                                if (station.GeneralResult.BWMask.Length > 0)
                                                {
                                                    var lstIns = new IQueryInsertStatement<MD.IResStMaskElementRaw>[station.GeneralResult.BWMask.Length];
                                                    for (int l = 0; l < station.GeneralResult.BWMask.Length; l++)
                                                    {
                                                        ElementsMask maskElem = station.GeneralResult.BWMask[l];
                                                        var builderInsertmaskElem = this._dataLayer.GetBuilder<MD.IResStMaskElementRaw>().Insert();
                                                        builderInsertmaskElem.SetValue(c => c.Bw, maskElem.BW_kHz);
                                                        builderInsertmaskElem.SetValue(c => c.Level, maskElem.Level_dB);
                                                        builderInsertmaskElem.SetValue(c => c.ResStGeneralId, IDResGeneral);
                                                        builderInsertmaskElem.Select(c => c.Id);
                                                        lstIns[l] = builderInsertmaskElem;
                                                    }
                                                    queryExecuter.ExecuteAndFetch(lstIns, reader =>
                                                    {
                                                        return true;
                                                    });
                                                }
                                            }


                                            if (station.GeneralResult.LevelsSpectrum_dBm != null)
                                            {
                                                if (station.GeneralResult.LevelsSpectrum_dBm.Length > 0)
                                                {
                                                    var lstIns = new IQueryInsertStatement<MD.IResStLevelsSpectRaw>[station.GeneralResult.LevelsSpectrum_dBm.Length];
                                                    for (int l = 0; l < station.GeneralResult.LevelsSpectrum_dBm.Length; l++)
                                                    {
                                                        double lvl = station.GeneralResult.LevelsSpectrum_dBm[l];
                                                        var builderInsertResStLevelsSpect = this._dataLayer.GetBuilder<MD.IResStLevelsSpectRaw>().Insert();
                                                        builderInsertResStLevelsSpect.SetValue(c => c.LevelSpecrum, lvl);
                                                        builderInsertResStLevelsSpect.SetValue(c => c.ResStGeneralId, IDResGeneral);
                                                        builderInsertResStLevelsSpect.Select(c => c.Id);
                                                        lstIns[l] = builderInsertResStLevelsSpect;
                                                    }
                                                    queryExecuter.ExecuteAndFetch(lstIns, reader =>
                                                    {
                                                        return true;
                                                    });
                                                }
                                            }


                                            if (station.LevelResults != null)
                                            {
                                                if (station.LevelResults.Length > 0)
                                                {
                                                    var lstIns = new IQueryInsertStatement<MD.IResStLevelCarRaw>[station.LevelResults.Length];
                                                    for (int l = 0; l < station.LevelResults.Length; l++)
                                                    {
                                                        LevelMeasResult car = station.LevelResults[l];
                                                        var builderInsertResStLevelCar = this._dataLayer.GetBuilder<MD.IResStLevelCarRaw>().Insert();
                                                        if (car.Location != null)
                                                        {
                                                            builderInsertResStLevelCar.SetValue(c => c.Agl, car.Location.AGL);
                                                            builderInsertResStLevelCar.SetValue(c => c.Altitude, car.Location.ASL);
                                                            builderInsertResStLevelCar.SetValue(c => c.Lon, car.Location.Lon);
                                                            builderInsertResStLevelCar.SetValue(c => c.Lat, car.Location.Lat);
                                                        }
                                                        builderInsertResStLevelCar.SetValue(c => c.DifferenceTimeStamp, car.DifferenceTimeStamp_ns);
                                                        builderInsertResStLevelCar.SetValue(c => c.LevelDbm, car.Level_dBm);
                                                        builderInsertResStLevelCar.SetValue(c => c.LevelDbmkvm, car.Level_dBmkVm);
                                                        builderInsertResStLevelCar.SetValue(c => c.TimeOfMeasurements, car.MeasurementTime);

                                                        if (station.GeneralResult != null)
                                                        {
                                                            var generalResults = station.GeneralResult;


                                                            builderInsertResStLevelCar.SetValue(c => c.CentralFrequency, generalResults.CentralFrequency_MHz);
                                                            if (generalResults.BandwidthResult != null)
                                                            {
                                                                 builderInsertResStLevelCar.SetValue(c => c.Bw, generalResults.BandwidthResult.Bandwidth_kHz);
                                                            }
                                                        }
                                                        builderInsertResStLevelCar.SetValue(c => c.ResStationId, valInsResMeasStation);
                                                        builderInsertResStLevelCar.Select(c => c.Id);
                                                        lstIns[l] = builderInsertResStLevelCar;

                                                    }
                                                    queryExecuter.ExecuteAndFetch(lstIns, reader =>
                                                    {
                                                        return true;
                                                    });
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    queryExecuter.CommitTransaction();
                    // с этого момента нужно считать что сообщение удачно обработано
                    result.Status = SdrnMessageHandlingStatus.Confirmed;
                    this._eventEmitter.Emit(new MDE.OnReceivedNewSOResultEvent() { ResultId = valInsResMeas }, new EventEmittingOptions { Rule = EventEmittingRule.Default });
                }
                catch (Exception e)
                {
                    queryExecuter.RollbackTransaction();
                    this._logger.Exception(Contexts.PrimaryHandler, Categories.MessageProcessing, e, this);
                    if (result.Status == SdrnMessageHandlingStatus.Unprocessed)
                    {
                        result.Status = SdrnMessageHandlingStatus.Error;
                        result.ReasonFailure = e.ToString();
                    }
                }
                finally
                {
                    // независимо упали мы по ошибке мы обязаны отправить ответ клиенту
                    // формируем объект подтвержденяи о обновлении данных о сенсоре
                    var deviceCommandResult = new DeviceCommand
                    {
                        EquipmentTechId = incomingEnvelope.SensorTechId,
                        SensorName = incomingEnvelope.SensorName,
                        SdrnServer = this._environment.ServerInstance,
                        Command = "SendMeasResultsResult",
                        CommandId = "SendCommand",
                        CustTxt1 = "Success"
                    };

                    if (result.Status == SdrnMessageHandlingStatus.Error)
                    {
                        deviceCommandResult.CustTxt1 = "Error";
                    }
                    else if (valInsResMeas > 0)
                    {
                        deviceCommandResult.CustTxt1 = "Success";
                    }
                    else
                    {
                        deviceCommandResult.CustTxt1 = "Error";
                    }
                    var envelop = _messagePublisher.CreateOutgoingEnvelope<MSG.Server.SendCommandMessage, DeviceCommand>();
                    envelop.SensorName = incomingEnvelope.SensorName;
                    envelop.SensorTechId = incomingEnvelope.SensorTechId;
                    envelop.DeliveryObject = deviceCommandResult;
                    _messagePublisher.Send(envelop);
                }

            }
        }

    }
}


