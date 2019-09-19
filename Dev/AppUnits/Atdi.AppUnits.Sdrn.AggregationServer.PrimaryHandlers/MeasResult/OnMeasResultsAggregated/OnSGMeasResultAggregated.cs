using Atdi.DataModels.Api.EventSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Platform.Logging;
using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Contracts.CoreServices.EntityOrm;
using Atdi.Contracts.Sdrn.Server;
using Atdi.DataModels.Sdrns;
using MD = Atdi.DataModels.Sdrns.Server.Entities;
using Atdi.DataModels.DataConstraint;
using Atdi.Contracts.Api.EventSystem;
using Atdi.DataModels.Sdrns.Server.Events;
using Atdi.Common;
using Atdi.Platform;
using Atdi.Platform.Caching;
using Atdi.DataModels.Sdrns.Device;
using DMS = Atdi.DataModels.Sdrns.Server;
using Atdi.Contracts.Api.DataBus;

namespace Atdi.AppUnits.Sdrn.AggregationServer.PrimaryHandlers
{
    [SubscriptionEvent(EventName = "OnSGMeasResultAggregated", SubscriberName = "SGMeasResultSubscriber")]
    public class OnSGMeasResultAggregated : IEventSubscriber<SGMeasResultAggregated>
    {
        private readonly ILogger _logger;
        private readonly IEventEmitter _eventEmitter;
        private readonly IQueryExecutor _queryExecutor;
        private readonly IDataLayer<EntityDataOrm> _dataLayer;
        private readonly ISdrnServerEnvironment _environment;
        private readonly IPublisher _publisher;
        public OnSGMeasResultAggregated(IEventEmitter eventEmitter, IDataLayer<EntityDataOrm> dataLayer, ILogger logger, ISdrnServerEnvironment environment, IPublisher publisher)
        {
            this._logger = logger;
            this._dataLayer = dataLayer;
            this._environment = environment;
            this._eventEmitter = eventEmitter;
            this._publisher = publisher;
            this._queryExecutor = this._dataLayer.Executor<SdrnServerDataContext>();
        }
        public void Notify(SGMeasResultAggregated @event)
        {
            try
            {
                _logger.Debug(Contexts.ThisComponent, Categories.EventProcessing, "Start processing SGMeasResultAggregated, ResultId = " + @event.MeasResultId.ToString());
                this.Handle(@event.MeasResultId);
                _logger.Debug(Contexts.ThisComponent, Categories.EventProcessing, "Stop processing SGMeasResultAggregated, ResultId = " + @event.MeasResultId.ToString());
            }
            catch (Exception e)
            {
                _logger.Exception(Contexts.ThisComponent, Categories.EventProcessing, e, (object)this);
            }
        }
        private void Handle(long measResultId)
        {
            var measResult = new MeasResults();

            // IResMeas
            long subTaskSensorId = 0;
            var builderResMeas = this._dataLayer.GetBuilder<MD.IResMeas>().From();
            builderResMeas.Select(c => c.MeasResultSID, c => c.TimeMeas, c => c.Status, c => c.StartTime, c => c.StopTime, c => c.ScansNumber, c => c.TypeMeasurements, c => c.SUBTASK_SENSOR.Id);
            builderResMeas.Where(c => c.Id, ConditionOperator.Equal, measResultId);
            this._queryExecutor.Fetch(builderResMeas, readerResMeas =>
            {
                while (readerResMeas.Read())
                {
                    subTaskSensorId = (readerResMeas.GetValue(c => c.SUBTASK_SENSOR.Id));
                    measResult.ResultId = (readerResMeas.GetValue(c => c.MeasResultSID));
                    measResult.Status = (readerResMeas.GetValue(c => c.Status));
                    measResult.Measured = readerResMeas.GetValue(c => c.TimeMeas).GetValueOrDefault();
                    measResult.StartTime = readerResMeas.GetValue(c => c.StartTime).GetValueOrDefault();
                    measResult.StopTime = readerResMeas.GetValue(c => c.StopTime).GetValueOrDefault();
                    measResult.ScansNumber = readerResMeas.GetValue(c => c.ScansNumber).GetValueOrDefault();
                    measResult.Measurement = (Enum.TryParse<MeasurementType>(readerResMeas.GetValue(c => c.TypeMeasurements), out MeasurementType outResType)) ? outResType : MeasurementType.SpectrumOccupation;
                }
                return true;
            });

            // ILinkSubTaskSensorMasterId
            var builderResMeasTaskId = this._dataLayer.GetBuilder<MD.ILinkSubTaskSensorMasterId>().From();
            builderResMeasTaskId.Select(c => c.SubtaskSensorMasterId);
            builderResMeasTaskId.Where(c => c.SUBTASK_SENSOR.Id, ConditionOperator.Equal, subTaskSensorId);
            this._queryExecutor.Fetch(builderResMeasTaskId, readerResMeasTaskId =>
            {
                while (readerResMeasTaskId.Read())
                {
                    var subtaskSensorMasterId = readerResMeasTaskId.GetValue(c => c.SubtaskSensorMasterId);
                    if (subtaskSensorMasterId.HasValue)
                        measResult.TaskId = subtaskSensorMasterId.Value.ToString();
                }
                return true;
            });

            // IResLocSensorMeas
            var location = new GeoLocation();
            var builderResMeasLocation = this._dataLayer.GetBuilder<MD.IResLocSensorMeas>().From();
            builderResMeasLocation.Select(c => c.Agl, c => c.Asl, c => c.Lon, c => c.Lat);
            builderResMeasLocation.Where(c => c.RES_MEAS.Id, ConditionOperator.Equal, measResultId);
            this._queryExecutor.Fetch(builderResMeasLocation, readerResMeasLocation =>
            {
                while (readerResMeasLocation.Read())
                {
                    location.AGL = readerResMeasLocation.GetValue(c => c.Agl).GetValueOrDefault();
                    location.ASL = readerResMeasLocation.GetValue(c => c.Asl).GetValueOrDefault();
                    location.Lon = readerResMeasLocation.GetValue(c => c.Lon).GetValueOrDefault();
                    location.Lat = readerResMeasLocation.GetValue(c => c.Lat).GetValueOrDefault();
                }
                return true;
            });
            measResult.Location = location;

            // IReferenceLevels
            var refLevels = new ReferenceLevels();
            var builderRefLevels = this._dataLayer.GetBuilder<MD.IReferenceLevels>().From();
            builderRefLevels.Select(c => c.StartFrequency_Hz, c => c.StepFrequency_Hz, c => c.RefLevels);
            builderRefLevels.Where(c => c.RES_MEAS.Id, ConditionOperator.Equal, measResultId);
            this._queryExecutor.Fetch(builderRefLevels, readerRefLevels =>
            {
                while (readerRefLevels.Read())
                {
                    refLevels.StartFrequency_Hz = readerRefLevels.GetValue(c => c.StartFrequency_Hz).GetValueOrDefault();
                    refLevels.StepFrequency_Hz = readerRefLevels.GetValue(c => c.StepFrequency_Hz).GetValueOrDefault();
                    refLevels.levels = readerRefLevels.GetValue(c => c.RefLevels);
                }
                return true;
            });
            measResult.RefLevels = refLevels;

            // IEmitting
            var emittings = new Dictionary<long, Emitting>();
            var builderEmittings = this._dataLayer.GetBuilder<MD.IEmitting>().From();
            builderEmittings.Select(c => c.Id, c => c.CurentPower_dBm, c => c.MeanDeviationFromReference, c => c.ReferenceLevel_dBm, c => c.TriggerDeviationFromReference, c => c.SensorId, c => c.RollOffFactor, c => c.StandardBW, c => c.StartFrequency_MHz, c => c.StopFrequency_MHz, c => c.LevelsDistributionCount, c => c.LevelsDistributionLvl, c => c.Loss_dB, c => c.Freq_kHz);
            builderEmittings.Where(c => c.RES_MEAS.Id, ConditionOperator.Equal, measResultId);
            this._queryExecutor.Fetch(builderEmittings, readerEmittings =>
            {
                while (readerEmittings.Read())
                {
                    var emitting = new Emitting();
                    emitting.CurentPower_dBm = readerEmittings.GetValue(c => c.CurentPower_dBm).GetValueOrDefault();
                    emitting.MeanDeviationFromReference = readerEmittings.GetValue(c => c.MeanDeviationFromReference).GetValueOrDefault();
                    emitting.ReferenceLevel_dBm = readerEmittings.GetValue(c => c.ReferenceLevel_dBm).GetValueOrDefault();
                    emitting.TriggerDeviationFromReference = readerEmittings.GetValue(c => c.TriggerDeviationFromReference).GetValueOrDefault();
                    emitting.StartFrequency_MHz = readerEmittings.GetValue(c => c.StartFrequency_MHz).GetValueOrDefault();
                    emitting.StopFrequency_MHz = readerEmittings.GetValue(c => c.StopFrequency_MHz).GetValueOrDefault();
                    emitting.SensorId = (int?)readerEmittings.GetValue(c => c.SensorId);

                    var param = new EmittingParameters();
                    param.RollOffFactor = readerEmittings.GetValue(c => c.RollOffFactor).GetValueOrDefault();
                    param.StandardBW = readerEmittings.GetValue(c => c.StandardBW).GetValueOrDefault();
                    emitting.EmittingParameters = param;

                    var levelDistr = new LevelsDistribution();
                    levelDistr.Count = readerEmittings.GetValue(c => c.LevelsDistributionCount);
                    levelDistr.Levels = readerEmittings.GetValue(c => c.LevelsDistributionLvl);
                    emitting.LevelsDistribution = levelDistr;

                    var signMask = new SignalMask();
                    signMask.Loss_dB = readerEmittings.GetValue(c => c.Loss_dB);
                    signMask.Freq_kHz = readerEmittings.GetValue(c => c.Freq_kHz);
                    emitting.SignalMask = signMask;

                    emittings.Add(readerEmittings.GetValue(c => c.Id), emitting);
                }
                return true;
            });

            foreach (var emittingId in emittings.Keys)
            {
                // IWorkTime
                var workTimes = new List<WorkTime>();
                var builderWorkTimes = this._dataLayer.GetBuilder<MD.IWorkTime>().From();
                builderWorkTimes.Select(c => c.HitCount, c => c.PersentAvailability, c => c.StartEmitting, c => c.StopEmitting);
                builderWorkTimes.Where(c => c.EMITTING.Id, ConditionOperator.Equal, emittingId);
                this._queryExecutor.Fetch(builderWorkTimes, readerWorkTimes =>
                {
                    while (readerWorkTimes.Read())
                    {
                        var workTime = new WorkTime();
                        workTime.HitCount = readerWorkTimes.GetValue(c => c.HitCount);
                        workTime.PersentAvailability = readerWorkTimes.GetValue(c => c.PersentAvailability);
                        workTime.StartEmitting = readerWorkTimes.GetValue(c => c.StartEmitting);
                        workTime.StopEmitting = readerWorkTimes.GetValue(c => c.StopEmitting);
                        workTimes.Add(workTime);
                    }
                    return true;
                });
                if (workTimes.Count > 0)
                    emittings[emittingId].WorkTimes = workTimes.ToArray();

                // ISpectrum
                var spectrum = new Spectrum();
                var builderSpectrum = this._dataLayer.GetBuilder<MD.ISpectrum>().From();
                builderSpectrum.Select(c => c.CorrectnessEstimations, c => c.Contravention, c => c.Bandwidth_kHz, c => c.MarkerIndex, c => c.SignalLevel_dBm, c => c.SpectrumStartFreq_MHz, c => c.SpectrumSteps_kHz, c => c.T1, c => c.T2, c => c.TraceCount, c => c.Levels_dBm);
                builderSpectrum.Where(c => c.EMITTING.Id, ConditionOperator.Equal, emittingId);
                this._queryExecutor.Fetch(builderSpectrum, readerSpectrum =>
                {
                    while (readerSpectrum.Read())
                    {
                        spectrum.СorrectnessEstimations = readerSpectrum.GetValue(c => c.CorrectnessEstimations).GetValueOrDefault() == 1 ? true : false;
                        spectrum.Contravention = readerSpectrum.GetValue(c => c.Contravention).GetValueOrDefault() == 1 ? true : false;
                        spectrum.Bandwidth_kHz = readerSpectrum.GetValue(c => c.Bandwidth_kHz).GetValueOrDefault();
                        spectrum.MarkerIndex = readerSpectrum.GetValue(c => c.MarkerIndex).GetValueOrDefault();
                        spectrum.SignalLevel_dBm = readerSpectrum.GetValue(c => c.SignalLevel_dBm).GetValueOrDefault();
                        spectrum.SpectrumStartFreq_MHz = readerSpectrum.GetValue(c => c.SpectrumStartFreq_MHz).GetValueOrDefault();
                        spectrum.SpectrumSteps_kHz = readerSpectrum.GetValue(c => c.SpectrumSteps_kHz).GetValueOrDefault();
                        spectrum.T1 = readerSpectrum.GetValue(c => c.T1).GetValueOrDefault();
                        spectrum.T2 = readerSpectrum.GetValue(c => c.T2).GetValueOrDefault();
                        spectrum.TraceCount = readerSpectrum.GetValue(c => c.TraceCount).GetValueOrDefault();
                        spectrum.Levels_dBm = readerSpectrum.GetValue(c => c.Levels_dBm);
                    }
                    return true;
                });
                emittings[emittingId].Spectrum = spectrum;

                // ISignalingSysInfo
                var sysInfos = new Dictionary<long, SignalingSysInfo>();
                var builderSysInfo = this._dataLayer.GetBuilder<MD.ISignalingSysInfo>().From();
                builderSysInfo.Select(c => c.BandWidth_Hz, c => c.BSIC, c => c.ChannelNumber, c => c.CID, c => c.CtoI, c => c.Freq_Hz, c => c.LAC, c => c.Level_dBm, c => c.MCC, c => c.MNC, c => c.Power, c => c.RNC, c => c.Standard);
                builderSysInfo.Where(c => c.EMITTING.Id, ConditionOperator.Equal, emittingId);
                this._queryExecutor.Fetch(builderSysInfo, readerSysInfo =>
                {
                    while (readerSysInfo.Read())
                    {
                        var sysInfo = new SignalingSysInfo();
                        sysInfo.BandWidth_Hz = readerSysInfo.GetValue(c => c.BandWidth_Hz);
                        sysInfo.BSIC = readerSysInfo.GetValue(c => c.BSIC);
                        sysInfo.ChannelNumber = readerSysInfo.GetValue(c => c.ChannelNumber);
                        sysInfo.CID = readerSysInfo.GetValue(c => c.CID);
                        sysInfo.CtoI = readerSysInfo.GetValue(c => c.CtoI);
                        sysInfo.Freq_Hz = readerSysInfo.GetValue(c => c.Freq_Hz);
                        sysInfo.LAC = readerSysInfo.GetValue(c => c.LAC);
                        sysInfo.Level_dBm = readerSysInfo.GetValue(c => c.Level_dBm);
                        sysInfo.MCC = readerSysInfo.GetValue(c => c.MCC);
                        sysInfo.MNC = readerSysInfo.GetValue(c => c.MNC);
                        sysInfo.Power = readerSysInfo.GetValue(c => c.Power);
                        sysInfo.RNC = readerSysInfo.GetValue(c => c.RNC);
                        sysInfo.Standard = readerSysInfo.GetValue(c => c.Standard);
                        sysInfos.Add(readerSysInfo.GetValue(c => c.Id), sysInfo);
                    }
                    return true;
                });

                // ISignalingSysInfoWorkTime
                foreach (var sysInfoId in sysInfos.Keys)
                {
                    var sysWorkTimes = new List<WorkTime>();
                    var builderSysWorkTimes = this._dataLayer.GetBuilder<MD.ISignalingSysInfoWorkTime>().From();
                    builderSysWorkTimes.Select(c => c.HitCount, c => c.PersentAvailability, c => c.StartEmitting, c => c.StopEmitting);
                    builderSysWorkTimes.Where(c => c.SYSINFO.Id, ConditionOperator.Equal, sysInfoId);
                    this._queryExecutor.Fetch(builderSysWorkTimes, readerSysWorkTimes =>
                    {
                        while (readerSysWorkTimes.Read())
                        {
                            var sysWorkTime = new WorkTime();
                            sysWorkTime.HitCount = readerSysWorkTimes.GetValue(c => c.HitCount);
                            sysWorkTime.PersentAvailability = readerSysWorkTimes.GetValue(c => c.PersentAvailability);
                            sysWorkTime.StartEmitting = readerSysWorkTimes.GetValue(c => c.StartEmitting);
                            sysWorkTime.StopEmitting = readerSysWorkTimes.GetValue(c => c.StopEmitting);
                            sysWorkTimes.Add(sysWorkTime);
                        }
                        return true;
                    });
                    if (sysWorkTimes.Count > 0)
                        sysInfos[sysInfoId].WorkTimes = sysWorkTimes.ToArray();
                }
                if (sysInfos.Count > 0)
                    emittings[emittingId].SysInfos = sysInfos.Values.ToArray();
            }
            if (emittings.Count > 0)
                measResult.Emittings = emittings.Values.ToArray();

            var deliveryObject = new DMS.MeasResultContainer() { MeasResult = measResult };
            var envelope = this._publisher.CreateEnvelope<DMS.SendMeasResultSGToMasterServer, DMS.MeasResultContainer>();
            envelope.To = this._environment.MasterServerInstance;
            envelope.DeliveryObject = deliveryObject;
            this._publisher.Send(envelope);
            _logger.Debug(Contexts.ThisComponent, Categories.EventProcessing, "OnSGMeasResultAggregated - SendEvent SendMeasResultSGToMasterServer, ResultId = " + measResultId.ToString());
        }
    }
}
