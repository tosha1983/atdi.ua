using Atdi.DataModels.Api.EventSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Contracts.Sdrn.Server.DevicesBus;
using Atdi.Platform.Logging;
using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Contracts.CoreServices.EntityOrm;
using Atdi.Contracts.Sdrn.Server;
using Atdi.DataModels.Sdrns;
using Atdi.DataModels.Sdrns.Device;
using DM = Atdi.DataModels.Sdrns.Device;
using MD = Atdi.DataModels.Sdrns.Server.Entities;
using Atdi.DataModels.DataConstraint;
using MSG = Atdi.DataModels.Sdrns.BusMessages;
using Atdi.Contracts.Api.EventSystem;
using Atdi.DataModels.Sdrns.Server.Events;
using Atdi.Common;
using Atdi.Platform;
using Atdi.Platform.Caching;
using CALC = Atdi.Modules.Sdrn.Calculation;

namespace Atdi.AppUnits.Sdrn.Server.EventSubscribers.DeviceBus
{
    [SubscriptionEvent(EventName = "OnSGMeasResultsDeviceBusEvent", SubscriberName = "SendSGMeasResultsSubscriber")]
    public class SendSGMeasResultsSubscriber : SubscriberBase<DM.MeasResults>
    {
        class HandleContext
        {
            public long messageId;
            public long resMeasId = 0;
            public string sensorName;
            public string sensorTechId;
            public IDataLayerScope scope;
            public DM.MeasResults measResult;
        }
        private readonly AppServerComponentConfig _config;
        private readonly IDataLayer<EntityDataOrm> _dataLayer;
        private readonly ISdrnServerEnvironment _environment;
        private readonly IStatistics _statistics;
        private readonly ISdrnMessagePublisher _messagePublisher;
        private readonly IEventEmitter _eventEmitter;
        private readonly IQueryExecutor _queryExecutor;

        private readonly IDataCache<string, long> _verifiedSubTaskSensorIdentityCache;
        private readonly IDataCache<string, long> _sensorIdentityCache;

        private readonly IStatisticCounter _signalingCounter;

        public SendSGMeasResultsSubscriber(
            IEventEmitter eventEmitter,
            ISdrnMessagePublisher messagePublisher,
            IMessagesSite messagesSite,
            IDataLayer<EntityDataOrm> dataLayer,
            ISdrnServerEnvironment environment,
            IStatistics statistics,
            IDataCacheSite cacheSite,
            AppServerComponentConfig config,
            ILogger logger)
            : base(messagesSite, logger)
        {
            this._messagePublisher = messagePublisher;
            this._dataLayer = dataLayer;
            this._environment = environment;
            this._statistics = statistics;
            this._config = config;
            this._eventEmitter = eventEmitter;
            this._queryExecutor = this._dataLayer.Executor<SdrnServerDataContext>();

            this._verifiedSubTaskSensorIdentityCache = cacheSite.Ensure(DataCaches.VerifiedSubTaskSensorIdentity);
            this._sensorIdentityCache = cacheSite.Ensure(DataCaches.SensorIdentity);

            if (this._statistics != null)
            {
                this._signalingCounter = _statistics.Counter(Monitoring.Counters.SendMeasResultsSignaling);
            }
        }

        protected override void Handle(string sensorName, string sensorTechId, DM.MeasResults deliveryObject, long messageId)
        {
            using (this._logger.StartTrace(Contexts.ThisComponent, Categories.MessageProcessing, this))
            {
                if (deliveryObject.Measurement != MeasurementType.Signaling)
                {
                    throw new InvalidOperationException("Incorrect MeasurementType. Expected is Signaling");
                }

                var status = SdrnMessageHandlingStatus.Unprocessed;
                bool isSuccessProcessed = false;
                var reasonFailure = "";
                try
                {
                    using (var scope = this._dataLayer.CreateScope<SdrnServerDataContext>())
                    {
                        var context = new HandleContext()
                        {
                            messageId = messageId,
                            resMeasId = 0,
                            sensorName = sensorName,
                            sensorTechId = sensorTechId,
                            scope = scope,
                            measResult = deliveryObject
                        };

                        this._signalingCounter?.Increment();



                        bool? collectEmissionInstrumentalEstimation = null;
                        var rawTaskId = context.measResult.TaskId.Replace("SDRN.SubTaskSensorId.", "");
                        if (long.TryParse(rawTaskId, out long taskId))
                        {
                            var builderSubTaskSensor = this._dataLayer.GetBuilder<MD.ISubTaskSensor>().From();
                            builderSubTaskSensor.Select(c => c.SUBTASK.Id, c => c.SUBTASK.MEAS_TASK.Id);
                            builderSubTaskSensor.Where(c => c.Id, ConditionOperator.Equal, taskId);
                            this._queryExecutor.Fetch(builderSubTaskSensor, readerSubTaskSensor =>
                            {
                                while (readerSubTaskSensor.Read())
                                {
                                    var builderTask = this._dataLayer.GetBuilder<MD.IMeasTaskSignaling>().From();
                                    builderTask.Select(c => c.CollectEmissionInstrumentalEstimation);
                                    builderTask.Where(c => c.MEAS_TASK.Id, ConditionOperator.Equal, readerSubTaskSensor.GetValue(c => c.SUBTASK.MEAS_TASK.Id));
                                    this._queryExecutor.Fetch(builderTask, readerResMeas =>
                                    {
                                        while (readerResMeas.Read())
                                        {
                                            collectEmissionInstrumentalEstimation = (readerResMeas.GetValue(c => c.CollectEmissionInstrumentalEstimation));
                                        }
                                        return true;
                                    });
                                }
                                return true;
                            });

                         
                        }

                        if (this.SaveMeasResultSignaling(ref context, collectEmissionInstrumentalEstimation.GetValueOrDefault(false)))
                        {
                            if (collectEmissionInstrumentalEstimation.GetValueOrDefault(false))
                            {
                                if (context.measResult.Status == "C" || context.measResult.Status == "COMPLETE" || context.measResult.Status == "COMPLETED")
                                {
                                    var busEvent = new SGMeasResultAppeared($"OnSGMeasResultAppeared", "SendSGMeasResultsSubscriber")
                                    {
                                        MeasResultId = context.resMeasId
                                    };
                                    _eventEmitter.Emit(busEvent);
                                }
                            }
                            else
                            {
                                var busEvent = new SGMeasResultAppeared($"OnSGMeasResultAppeared", "SendSGMeasResultsSubscriber")
                                {
                                    MeasResultId = context.resMeasId
                                };
                                _eventEmitter.Emit(busEvent);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    this._logger.Exception(Contexts.ThisComponent, Categories.MessageProcessing, e, this);
                    //status = SdrnMessageHandlingStatus.Error;
                    reasonFailure = e.StackTrace;
                }
                finally
                {
                    // независимо упали мы по ошибке мы обязаны отправить ответ клиенту
                    // формируем объект подтвержденяи о обновлении данных о сенсоре
                    var deviceCommandResult = new DeviceCommand
                    {
                        EquipmentTechId = sensorTechId,
                        SensorName = sensorName,
                        SdrnServer = this._environment.ServerInstance,
                        Command = "SendMeasResultsConfirmed",
                        CommandId = "SendCommand",
                        CustTxt1 = "Success"
                    };

                    if (status == SdrnMessageHandlingStatus.Error)
                    {
                        deviceCommandResult.CustTxt1 = "{ " + string.Format("{0}: {1}, {2}: {3}, {4}: {5}, {6}: {7} ", "\"Status\"", "\"Fault\"", "\"ResultId\"", "\"" + deliveryObject.ResultId + "\"", "\"Message\"", "\"" + reasonFailure + "\"", "\"DateCreated\"", "\"" + DateTime.Now.ToString("dd.MM.yyyyTHH:mm:ss") + "\"") + " }";
                    }
                    else if (isSuccessProcessed)
                    {
                        deviceCommandResult.CustTxt1 = "{ " + string.Format("{0}: {1}, {2}: {3}, {4}: {5}, {6}: {7} ", "\"Status\"", "\"Success\"", "\"ResultId\"", "\"" + deliveryObject.ResultId + "\"", "\"Message\"", "\"\"", "\"DateCreated\"", "\"" + DateTime.Now.ToString("dd.MM.yyyyTHH:mm:ss") + "\"") + " }";
                    }
                    else
                    {
                        deviceCommandResult.CustTxt1 = "{ " + string.Format("{0}: {1}, {2}: {3}, {4}: {5}, {6}: {7} ", "\"Status\"", "\"Fault\"", "\"ResultId\"", "\"" + deliveryObject.ResultId + "\"", "\"Message\"", "\"" + reasonFailure + "\"", "\"DateCreated\"", "\"" + DateTime.Now.ToString("dd.MM.yyyyTHH:mm:ss") + "\"") + " }";
                    }
                    var envelop = _messagePublisher.CreateOutgoingEnvelope<MSG.Server.SendCommandMessage, DeviceCommand>();
                    envelop.SensorName = sensorName;
                    envelop.SensorTechId = sensorTechId;
                    envelop.DeliveryObject = deviceCommandResult;
                    _messagePublisher.Send(envelop);
                }

            }
        }
        private bool SaveMeasResultSignaling(ref HandleContext context, bool collectEmissionInstrumentalEstimation)
        {
            try
            {
                var measResult = context.measResult;

                if (string.IsNullOrEmpty(measResult.ResultId))
                {
                    WriteLog("Undefined value ResultId", "IResMeas", context);
                    return false;
                }
                if (string.IsNullOrEmpty(measResult.TaskId))
                {
                    WriteLog("Undefined value TaskId", "IResMeas", context);
                    return false;
                }

                measResult.ResultId = measResult.ResultId.SubString(50);
                measResult.TaskId = measResult.TaskId.SubString(200);

                if (collectEmissionInstrumentalEstimation)
                {
                    long subTaskSensorId = 0;
                    DateTime? TimeStart = null;
                    DateTime? TimeStop = null;
                    int? SignalizationNCount = null;
                    long? sensorId=null;
                    double? CrossingBWPercentageForGoodSignals = null;
                    double? CrossingBWPercentageForBadSignals = null;
                    int? TypeJoinSpectrum = null;
                    bool? AnalyzeByChannel = null;
                    bool? CorrelationAnalize = null;

                    double? CorrelationFactor = null;
                    double? MaxFreqDeviation = null;

                    var rawTaskId = measResult.TaskId.Replace("SDRN.SubTaskSensorId.", "");
                    if (long.TryParse(rawTaskId, out subTaskSensorId))
                    {
                        if (subTaskSensorId > 0)
                        {
                            var builderSubTaskSensor = this._dataLayer.GetBuilder<MD.ISubTaskSensor>().From();
                            builderSubTaskSensor.Select(c => c.SENSOR.Id, c => c.SUBTASK.Id, c => c.SUBTASK.MEAS_TASK.Id, c => c.SUBTASK.MEAS_TASK.TimeStart, c => c.SUBTASK.MEAS_TASK.TimeStop);
                            builderSubTaskSensor.Where(c => c.Id, ConditionOperator.Equal, subTaskSensorId);
                            this._queryExecutor.Fetch(builderSubTaskSensor, readerSubTaskSensor =>
                            {
                                while (readerSubTaskSensor.Read())
                                {
                                    sensorId = readerSubTaskSensor.GetValue(c => c.SENSOR.Id);
                                    TimeStart = readerSubTaskSensor.GetValue(c => c.SUBTASK.MEAS_TASK.TimeStart);
                                    TimeStop = readerSubTaskSensor.GetValue(c => c.SUBTASK.MEAS_TASK.TimeStop);
                                    var builderMeasTaskSignaling = this._dataLayer.GetBuilder<MD.IMeasTaskSignaling>().From();
                                    builderMeasTaskSignaling.Select(c => c.SignalizationNCount, c => c.CrossingBWPercentageForGoodSignals, c => c.CrossingBWPercentageForBadSignals, c => c.TypeJoinSpectrum, c => c.AnalyzeByChannel, c => c.CorrelationAnalize, c => c.CorrelationFactor, c => c.MaxFreqDeviation);
                                    builderMeasTaskSignaling.Where(c => c.MEAS_TASK.Id, ConditionOperator.Equal, readerSubTaskSensor.GetValue(c => c.SUBTASK.MEAS_TASK.Id));
                                    this._queryExecutor.Fetch(builderMeasTaskSignaling, readerMeasTaskSignaling =>
                                    {
                                        while (readerMeasTaskSignaling.Read())
                                        {
                                            SignalizationNCount = readerMeasTaskSignaling.GetValue(c => c.SignalizationNCount);
                                            CrossingBWPercentageForGoodSignals = readerMeasTaskSignaling.GetValue(c => c.CrossingBWPercentageForGoodSignals);
                                            CrossingBWPercentageForBadSignals = readerMeasTaskSignaling.GetValue(c => c.CrossingBWPercentageForBadSignals);
                                            TypeJoinSpectrum = readerMeasTaskSignaling.GetValue(c => c.TypeJoinSpectrum);
                                            AnalyzeByChannel = readerMeasTaskSignaling.GetValue(c => c.AnalyzeByChannel);
                                            CorrelationAnalize = readerMeasTaskSignaling.GetValue(c => c.CorrelationAnalize);
                                            CorrelationFactor = readerMeasTaskSignaling.GetValue(c => c.CorrelationFactor);
                                            MaxFreqDeviation = readerMeasTaskSignaling.GetValue(c => c.MaxFreqDeviation);

                                        }
                                        return true;
                                    });
                                }
                                return true;
                            });


                            GroupEmitting(measResult, subTaskSensorId, sensorId, SignalizationNCount, TimeStart, TimeStop, CrossingBWPercentageForGoodSignals, CrossingBWPercentageForBadSignals, TypeJoinSpectrum, AnalyzeByChannel, CorrelationAnalize, CorrelationFactor, MaxFreqDeviation);
                            DeleteOldResult(subTaskSensorId);
                        }
                    }
                    /*
                    if (long.TryParse(measResult.ResultId, out long resultId))
                    {
                        var builderResult = this._dataLayer.GetBuilder<MD.IResMeas>().From();
                        builderResult.Select(c => c.SUBTASK_SENSOR.Id);
                        builderResult.Where(c => c.Id, ConditionOperator.Equal, resultId);
                        this._queryExecutor.Fetch(builderResult, readerResMeas =>
                        {
                            while (readerResMeas.Read())
                            {
                                subTaskSensorId = (readerResMeas.GetValue(c => c.SUBTASK_SENSOR.Id));
                            }
                            return true;
                        });
                    }
                   
                    if (subTaskSensorId > 0)
                    {
                        GroupEmitting(measResult, subTaskSensorId);
                        DeleteOldResult(subTaskSensorId);
                    }
                    */
                }

                if ((measResult.Status != null) && (measResult.Status.Length > 5))
                {
                    measResult.Status = "";
                }

                if (!(measResult.ScansNumber >= 0 && measResult.ScansNumber <= 10000000))
                    WriteLog("Incorrect value ScansNumber", "IResMeas", context);

                if (measResult.StartTime > measResult.StopTime)
                    WriteLog("StartTime must be less than StopTime", "IResMeas", context);

                var subMeasTaskStaId = EnsureSubTaskSensorId(context.sensorName, context.sensorTechId, measResult.Measurement, measResult.TaskId, measResult.Measured, context.scope);
                var builderInsertIResMeas = this._dataLayer.GetBuilder<MD.IResMeas>().Insert();
                builderInsertIResMeas.SetValue(c => c.MeasResultSID, measResult.ResultId);
                builderInsertIResMeas.SetValue(c => c.TimeMeas, measResult.Measured);
                builderInsertIResMeas.SetValue(c => c.Status, measResult.Status);
                builderInsertIResMeas.SetValue(c => c.StartTime, measResult.StartTime);
                builderInsertIResMeas.SetValue(c => c.StopTime, measResult.StopTime);
                builderInsertIResMeas.SetValue(c => c.ScansNumber, measResult.ScansNumber);
                builderInsertIResMeas.SetValue(c => c.TypeMeasurements, measResult.Measurement.ToString());
                builderInsertIResMeas.SetValue(c => c.SUBTASK_SENSOR.Id, subMeasTaskStaId);
                var valInsResMeas = context.scope.Executor.Execute<MD.IResMeas_PK>(builderInsertIResMeas);
                context.resMeasId = valInsResMeas.Id;
                if (context.resMeasId > 0)
                {
                    if (this.ValidateGeoLocation(measResult.Location, "IResMeas", context))
                    {
                        var builderInsertResLocSensorMeas = this._dataLayer.GetBuilder<MD.IResLocSensorMeas>().Insert();
                        builderInsertResLocSensorMeas.SetValue(c => c.Agl, measResult.Location.AGL);
                        builderInsertResLocSensorMeas.SetValue(c => c.Asl, measResult.Location.ASL);
                        builderInsertResLocSensorMeas.SetValue(c => c.Lon, measResult.Location.Lon);
                        builderInsertResLocSensorMeas.SetValue(c => c.Lat, measResult.Location.Lat);
                        builderInsertResLocSensorMeas.SetValue(c => c.RES_MEAS.Id, context.resMeasId);
                        context.scope.Executor.Execute(builderInsertResLocSensorMeas);
                    }

                    if (measResult.RefLevels != null)
                    {
                        bool validationResult = true;
                        var refLevels = measResult.RefLevels;

                        List<float> listLevels = new List<float>();
                        foreach (float levels in refLevels.levels)
                        {
                            if (levels >= -200 && levels <= 50)
                                listLevels.Add(levels);
                            else
                                WriteLog("Incorrect value level", "IReferenceLevels", context);
                        }
                        if (listLevels.Count > 0)
                            refLevels.levels = listLevels.ToArray();
                        else
                            validationResult = false;

                        if (refLevels.StartFrequency_Hz < 9000 || refLevels.StartFrequency_Hz > 400000000000)
                        {
                            validationResult = false;
                            WriteLog("Incorrect value StartFrequency_Hz", "IReferenceLevels", context);
                        }
                        if (refLevels.StepFrequency_Hz < 1 || refLevels.StepFrequency_Hz > 1000000000)
                        {
                            validationResult = false;
                            WriteLog("Incorrect value StepFrequency_Hz", "IReferenceLevels", context);
                        }
                        var builderInsertReferenceLevels = this._dataLayer.GetBuilder<MD.IReferenceLevels>().Insert();
                        builderInsertReferenceLevels.SetValue(c => c.StartFrequency_Hz, refLevels.StartFrequency_Hz);
                        builderInsertReferenceLevels.SetValue(c => c.StepFrequency_Hz, refLevels.StepFrequency_Hz);
                        if (refLevels.levels != null)
                        {
                            builderInsertReferenceLevels.SetValue(c => c.RefLevels, refLevels.levels);
                        }
                        builderInsertReferenceLevels.SetValue(c => c.RES_MEAS.Id, context.resMeasId);
                        if (validationResult)
                            context.scope.Executor.Execute<MD.IReferenceLevels_PK>(builderInsertReferenceLevels);
                    }
                    if (measResult.Emittings != null)
                    {
                        foreach (Emitting emitting in measResult.Emittings)
                        {
                            bool validationResult = true;
                            if (!(emitting.StartFrequency_MHz >= 0.009 && emitting.StartFrequency_MHz <= 400000))
                            {
                                WriteLog("Incorrect value StartFrequency_MHz", "IEmitting", context);
                                validationResult = false;
                            }
                            if (!(emitting.StopFrequency_MHz >= 0.009 && emitting.StopFrequency_MHz <= 400000))
                            {
                                WriteLog("Incorrect value StopFrequency_MHz", "IEmitting", context);
                                validationResult = false;
                            }
                            if (emitting.StartFrequency_MHz > emitting.StopFrequency_MHz)
                            {
                                WriteLog("StartFrequency_MHz must be less than StopFrequency_MHz", "IEmitting", context);
                                validationResult = false;
                            }
                            if (!validationResult)
                                continue;

                            var builderInsertEmitting = this._dataLayer.GetBuilder<MD.IEmitting>().Insert();
                            if (emitting.CurentPower_dBm >= -200 && emitting.CurentPower_dBm <= 50)
                                builderInsertEmitting.SetValue(c => c.CurentPower_dBm, emitting.CurentPower_dBm);
                            if (emitting.MeanDeviationFromReference >= 0 && emitting.MeanDeviationFromReference <= 1)
                                builderInsertEmitting.SetValue(c => c.MeanDeviationFromReference, emitting.MeanDeviationFromReference);
                            if (emitting.ReferenceLevel_dBm >= -200 && emitting.ReferenceLevel_dBm <= 50)
                                builderInsertEmitting.SetValue(c => c.ReferenceLevel_dBm, emitting.ReferenceLevel_dBm);
                            if (emitting.TriggerDeviationFromReference >= 0 && emitting.TriggerDeviationFromReference <= 1)
                                builderInsertEmitting.SetValue(c => c.TriggerDeviationFromReference, emitting.TriggerDeviationFromReference);
                            builderInsertEmitting.SetValue(c => c.RES_MEAS.Id, context.resMeasId);
                            builderInsertEmitting.SetValue(c => c.SensorId, emitting.SensorId);
                            if (emitting.EmittingParameters != null)
                            {
                                if (emitting.EmittingParameters.StandardBW >= 0 && emitting.EmittingParameters.StandardBW <= 1000000)
                                {
                                    if (emitting.EmittingParameters.RollOffFactor >= 0 && emitting.EmittingParameters.RollOffFactor <= 2.5)
                                        builderInsertEmitting.SetValue(c => c.RollOffFactor, emitting.EmittingParameters.RollOffFactor);
                                    builderInsertEmitting.SetValue(c => c.StandardBW, emitting.EmittingParameters.StandardBW);
                                }
                            }
                            builderInsertEmitting.SetValue(c => c.StartFrequency_MHz, emitting.StartFrequency_MHz);
                            builderInsertEmitting.SetValue(c => c.StopFrequency_MHz, emitting.StopFrequency_MHz);
                            var levelsDistribution = emitting.LevelsDistribution;
                            if (levelsDistribution != null)
                            {
                                List<int> listLevels = new List<int>();
                                List<int> listCounts = new List<int>();
                                for (int i = 0; i < levelsDistribution.Count.Length; i++)
                                {
                                    if (levelsDistribution.Count[i] >= 0 && levelsDistribution.Count[i] <= Int32.MaxValue && levelsDistribution.Levels[i] >= -200 && levelsDistribution.Levels[i] <= 100)
                                    {
                                        listLevels.Add(levelsDistribution.Levels[i]);
                                        listCounts.Add(levelsDistribution.Count[i]);
                                    }
                                }
                                if (listLevels.Count > 0 && listCounts.Count > 0)
                                {
                                    builderInsertEmitting.SetValue(c => c.LevelsDistributionCount, listCounts.ToArray());
                                    builderInsertEmitting.SetValue(c => c.LevelsDistributionLvl, listLevels.ToArray());
                                }
                            }
                            if (emitting.SignalMask != null)
                            {
                                builderInsertEmitting.SetValue(c => c.Loss_dB, emitting.SignalMask.Loss_dB);
                                builderInsertEmitting.SetValue(c => c.Freq_kHz, emitting.SignalMask.Freq_kHz);
                            }
                            var valInsReferenceEmitting = context.scope.Executor.Execute<MD.IEmitting_PK>(builderInsertEmitting);

                            if (valInsReferenceEmitting.Id > 0)
                            {
                                if (emitting.WorkTimes != null)
                                {
                                    foreach (WorkTime workTime in emitting.WorkTimes)
                                    {
                                        bool validationTimeResult = true;
                                        if (workTime.StartEmitting > workTime.StopEmitting)
                                        {
                                            WriteLog("StartEmitting must be less than StopEmitting", "IWorkTime", context);
                                            validationTimeResult = false;
                                        }
                                        if (!(workTime.PersentAvailability >= 0 && workTime.PersentAvailability <= 100))
                                        {
                                            WriteLog("Incorrect value PersentAvailability", "IWorkTime", context);
                                            validationTimeResult = false;
                                        }

                                        if (!validationTimeResult)
                                            continue;

                                        var builderInsertIWorkTime = this._dataLayer.GetBuilder<MD.IWorkTime>().Insert();
                                        builderInsertIWorkTime.SetValue(c => c.EMITTING.Id, valInsReferenceEmitting.Id);
                                        if (workTime.HitCount >= 0 && workTime.HitCount <= Int32.MaxValue)
                                            builderInsertIWorkTime.SetValue(c => c.HitCount, workTime.HitCount);
                                        builderInsertIWorkTime.SetValue(c => c.PersentAvailability, workTime.PersentAvailability);
                                        builderInsertIWorkTime.SetValue(c => c.StartEmitting, workTime.StartEmitting);
                                        builderInsertIWorkTime.SetValue(c => c.StopEmitting, workTime.StopEmitting);
                                        context.scope.Executor.Execute(builderInsertIWorkTime);
                                    }
                                }
                                var spectrum = emitting.Spectrum;
                                if (spectrum != null)
                                {
                                    bool validationSpectrumResult = true;

                                    List<float> listLevelsdBmB = new List<float>();
                                    foreach (float levelsdBmB in spectrum.Levels_dBm)
                                    {
                                        if (levelsdBmB >= -200 && levelsdBmB <= 50)
                                            listLevelsdBmB.Add(levelsdBmB);
                                        else
                                            WriteLog("Incorrect value level", "ISpectrum", context);
                                    }

                                    if (listLevelsdBmB.Count > 0)
                                        spectrum.Levels_dBm = listLevelsdBmB.ToArray();
                                    else
                                        validationSpectrumResult = false;

                                    if (!(spectrum.SpectrumStartFreq_MHz >= 0.009 && spectrum.SpectrumStartFreq_MHz <= 400000))
                                    {
                                        WriteLog("Incorrect value SpectrumStartFreq_MHz", "ISpectrumRaw", context);
                                        validationSpectrumResult = false;
                                    }
                                    if (!(spectrum.SpectrumSteps_kHz >= 0.001 && spectrum.SpectrumSteps_kHz <= 1000000))
                                    {
                                        WriteLog("Incorrect value SpectrumSteps_kHz", "ISpectrumRaw", context);
                                        validationSpectrumResult = false;
                                    }

                                    if (!(spectrum.T1 <= spectrum.MarkerIndex && spectrum.MarkerIndex <= spectrum.T2))
                                        WriteLog("Incorrect value MarkerIndex", "ISpectrumRaw", context);
                                    if (!(spectrum.T1 >= 0 && spectrum.T1 <= spectrum.T2))
                                    {
                                        WriteLog("Incorrect value T1", "ISpectrumRaw", context);
                                        validationSpectrumResult = false;
                                    }
                                    if (!(spectrum.T2 >= spectrum.T1 && spectrum.T2 <= spectrum.Levels_dBm.Length))
                                    {
                                        WriteLog("Incorrect value T2", "ISpectrumRaw", context);
                                        validationSpectrumResult = false;
                                    }

                                    if (validationSpectrumResult)
                                    {

                                        var builderInsertISpectrum = this._dataLayer.GetBuilder<MD.ISpectrum>().Insert();
                                        builderInsertISpectrum.SetValue(c => c.EMITTING.Id, valInsReferenceEmitting.Id);
                                        builderInsertISpectrum.SetValue(c => c.CorrectnessEstimations, spectrum.СorrectnessEstimations == true ? 1 : 0);
                                        builderInsertISpectrum.SetValue(c => c.Contravention, spectrum.Contravention == true ? 1 : 0);
                                        if (spectrum.Bandwidth_kHz >= 0 && spectrum.Bandwidth_kHz <= 1000000)
                                            builderInsertISpectrum.SetValue(c => c.Bandwidth_kHz, spectrum.Bandwidth_kHz);
                                        else
                                            WriteLog("Incorrect value Bandwidth_kHz", "ISpectrum", context);
                                        builderInsertISpectrum.SetValue(c => c.MarkerIndex, spectrum.MarkerIndex);
                                        if (spectrum.SignalLevel_dBm >= -200 && spectrum.SignalLevel_dBm <= 50)
                                            builderInsertISpectrum.SetValue(c => c.SignalLevel_dBm, spectrum.SignalLevel_dBm);
                                        else
                                            WriteLog("Incorrect value SignalLevel_dBm", "ISpectrum", context);
                                        builderInsertISpectrum.SetValue(c => c.SpectrumStartFreq_MHz, spectrum.SpectrumStartFreq_MHz);
                                        builderInsertISpectrum.SetValue(c => c.SpectrumSteps_kHz, spectrum.SpectrumSteps_kHz);
                                        builderInsertISpectrum.SetValue(c => c.T1, spectrum.T1);
                                        builderInsertISpectrum.SetValue(c => c.T2, spectrum.T2);
                                        if (spectrum.TraceCount >= 0 && spectrum.TraceCount <= 10000)
                                            builderInsertISpectrum.SetValue(c => c.TraceCount, spectrum.TraceCount);
                                        else
                                            WriteLog("Incorrect value TraceCount", "ISpectrum", context);
                                        builderInsertISpectrum.SetValue(c => c.Levels_dBm, spectrum.Levels_dBm);
                                        context.scope.Executor.Execute<MD.ISpectrum_PK>(builderInsertISpectrum);
                                    }
                                }
                            }
                            if (emitting.SysInfos != null)
                            {
                                foreach (SignalingSysInfo sysInfo in emitting.SysInfos)
                                {
                                    var builderInsertSysInfo = this._dataLayer.GetBuilder<MD.ISignalingSysInfo>().Insert();
                                    builderInsertSysInfo.SetValue(c => c.EMITTING.Id, valInsReferenceEmitting.Id);
                                    builderInsertSysInfo.SetValue(c => c.BandWidth_Hz, sysInfo.BandWidth_Hz);
                                    builderInsertSysInfo.SetValue(c => c.BSIC, sysInfo.BSIC);
                                    builderInsertSysInfo.SetValue(c => c.ChannelNumber, sysInfo.ChannelNumber);
                                    builderInsertSysInfo.SetValue(c => c.CID, sysInfo.CID);
                                    builderInsertSysInfo.SetValue(c => c.CtoI, sysInfo.CtoI);
                                    builderInsertSysInfo.SetValue(c => c.Freq_Hz, sysInfo.Freq_Hz);
                                    builderInsertSysInfo.SetValue(c => c.LAC, sysInfo.LAC);
                                    builderInsertSysInfo.SetValue(c => c.Level_dBm, sysInfo.Level_dBm);
                                    builderInsertSysInfo.SetValue(c => c.MCC, sysInfo.MCC);
                                    builderInsertSysInfo.SetValue(c => c.MNC, sysInfo.MNC);
                                    builderInsertSysInfo.SetValue(c => c.Power, sysInfo.Power);
                                    builderInsertSysInfo.SetValue(c => c.RNC, sysInfo.RNC);
                                    builderInsertSysInfo.SetValue(c => c.Standard, sysInfo.Standard);
                                    var valInsSysInfo = context.scope.Executor.Execute<MD.ISignalingSysInfo_PK>(builderInsertSysInfo);
                                    if (valInsSysInfo.Id > 0 && sysInfo.WorkTimes != null)
                                    {
                                        foreach (WorkTime workTime in sysInfo.WorkTimes)
                                        {
                                            bool validationTimeResult = true;
                                            if (workTime.StartEmitting > workTime.StopEmitting)
                                            {
                                                WriteLog("StartEmitting must be less than StopEmitting", "ISignalingSysInfoWorkTime", context);
                                                validationTimeResult = false;
                                            }
                                            if (!(workTime.PersentAvailability >= 0 && workTime.PersentAvailability <= 100))
                                            {
                                                WriteLog("Incorrect value PersentAvailability", "ISignalingSysInfoWorkTime", context);
                                                validationTimeResult = false;
                                            }

                                            if (!validationTimeResult)
                                                continue;

                                            var builderInsertIWorkTime = this._dataLayer.GetBuilder<MD.ISignalingSysInfoWorkTime>().Insert();
                                            builderInsertIWorkTime.SetValue(c => c.SYSINFO.Id, valInsSysInfo.Id);
                                            if (workTime.HitCount >= 0 && workTime.HitCount <= Int32.MaxValue)
                                                builderInsertIWorkTime.SetValue(c => c.HitCount, workTime.HitCount);
                                            builderInsertIWorkTime.SetValue(c => c.PersentAvailability, workTime.PersentAvailability);
                                            builderInsertIWorkTime.SetValue(c => c.StartEmitting, workTime.StartEmitting);
                                            builderInsertIWorkTime.SetValue(c => c.StopEmitting, workTime.StopEmitting);
                                            context.scope.Executor.Execute(builderInsertIWorkTime);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                return true;
            }
            catch (Exception exp)
            {
                _logger.Exception(Contexts.ThisComponent, exp);
                return false;
            }
        }

        private bool ValidateGeoLocation<T>(T location, string tableName, HandleContext context)
            where T : GeoLocation
        {
            if (location == null)
                return false;

            bool result = true;
            if (!(location.Lon >= -180 && location.Lon <= 180))
            {
                WriteLog($"Incorrect value Lon {location.Lon}", tableName, context);
                return false;
            }
            if (!(location.Lat >= -90 && location.Lat <= 90))
            {
                WriteLog($"Incorrect value Lat {location.Lat}", tableName, context);
                return false;
            }
            if (location.ASL < -1000 || location.ASL > 9000)
            {
                WriteLog($"Incorrect value Asl {location.ASL}", tableName, context);
            }
            if (location.AGL < -100 || location.AGL > 500)
            {
                WriteLog($"Incorrect value Agl {location.AGL}", tableName, context);
            }
            return result;
        }
        private void WriteLog(string msg, string tableName, HandleContext context)
        {
            var builderInsertLog = this._dataLayer.GetBuilder<MD.IValidationLogs>().Insert();
            builderInsertLog.SetValue(c => c.TableName, tableName);
            builderInsertLog.SetValue(c => c.When, DateTime.Now);
            builderInsertLog.SetValue(c => c.Info, msg);
            builderInsertLog.SetValue(c => c.MESSAGE.Id, context.messageId);
            builderInsertLog.SetValue(c => c.RES_MEAS.Id, context.resMeasId);
            context.scope.Executor.Execute(builderInsertLog);
        }
        private long EnsureSubTaskSensorId(string sensorName, string techId, MeasurementType measurement, string clientTaskId, DateTime measDate, IDataLayerScope scope)
        {
            // формат токена SDRN.FieldName.LongValue
            var tokens = clientTaskId.Split('.');

            if (tokens.Length == 3 && "SDRN".Equals(tokens[0]))
            {
                if (!"SubTaskSensorId".Equals(tokens[1], StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException($"Incorrect SDRN Task ID token '{clientTaskId}'");
                }

                if (!long.TryParse(tokens[2], out long subTaskSensorId))
                {
                    throw new InvalidOperationException($"Incorrect Task ID value '{clientTaskId}'");
                }
                var key = $"sensorName: {sensorName}, techId: {techId}, subTaskSensorId: {subTaskSensorId}";

                if (_verifiedSubTaskSensorIdentityCache.TryGet(key, out long data))
                {
                    return data;
                }
                var sensorId = this.EnsureSensor(sensorName, techId, scope);

                if (!this.ExistsSubTaskSensorFromStorage(subTaskSensorId, sensorId, scope))
                {
                    throw new InvalidOperationException($"A SubTaskSensor entry not found in storage by ID #{subTaskSensorId} and SensorName '{sensorName}' and Tech ID '{techId}'");
                }

                _verifiedSubTaskSensorIdentityCache.Set(key, subTaskSensorId);
                return subTaskSensorId;
            }
            throw new InvalidOperationException($"Incorrect Task ID value '{clientTaskId}'");
        }
        private bool ExistsSubTaskSensorFromStorage(long subTaskSensorId, long sensorId, IDataLayerScope scope)
        {
            var query = _dataLayer.GetBuilder<MD.ISubTaskSensor>()
                .From()
                .OnTop(1)
                .Select(c => c.Id)
                .Where(c => c.Id, ConditionOperator.Equal, subTaskSensorId)
                .Where(c => c.SENSOR.Id, ConditionOperator.Equal, sensorId);

            return scope.Executor.ExecuteAndFetch(query, reader =>
            {
                return reader.Read();
            });
        }
        private long EnsureSensor(string sensorName, string techId, IDataLayerScope scope)
        {
            var key = $"name: {sensorName}, techId: {techId}";

            // поиск в кеше
            if (_sensorIdentityCache.TryGet(key, out long sensorId))
            {
                return sensorId;
            }

            // поиск в хранилище
            if (this.TryGetSensorFromStorage(sensorName, techId, scope, out sensorId))
            {
                _sensorIdentityCache.Set(key, sensorId);
                return sensorId;
            }

            throw new InvalidOperationException($"Not found sensor in storage by name {sensorName} and TechID {techId}");
        }
        private bool TryGetSensorFromStorage(string sensorName, string techId, IDataLayerScope scope, out long sensorId)
        {
            var query = _dataLayer.GetBuilder<MD.ISensor>()
                .From()
                .OnTop(1)
                .Select(c => c.Id)
                .Where(c => c.Name, ConditionOperator.Equal, sensorName)
                .Where(c => c.TechId, ConditionOperator.Equal, techId);

            var id = default(long);
            var result = scope.Executor.ExecuteAndFetch(query, reader =>
            {
                var readState = reader.Read();
                if (readState)
                {
                    id = reader.GetValue(c => c.Id);
                }
                return readState;
            });

            sensorId = id;
            return result;
        }
        private void GroupEmitting(MeasResults result,
                                   long subTaskSensorId,
                                   long? sensorId,
                                   int? SignalizationNCount,
                                   DateTime? StartTime,
                                   DateTime? StopTime,
                                   double? CrossingBWPercentageForGoodSignals, 
                                   double? CrossingBWPercentageForBadSignal,
                                   int? TypeJoinSpectrum,
                                   bool? AnalyzeByChannel,
                                   bool? CorrelationAnalize,
                                   double? CorrelationFactor,
                                   double? MaxFreqDeviation
            )
        {
            var NoiseLevel_dBm = -100;
            var TimeBetweenWorkTimes_sec = this._config.TimeBetweenWorkTimes_sec;
            var lstEmitting = new List<CALC.Emitting>();
            var emittingSummury = LoadOthersEmittings(subTaskSensorId);
            var emittingRaw = ConvertEmitting(result.Emittings);
            // Группировка
            CALC.Emitting[] emittingTemp = null;
            var prmCorrelationFactor = CorrelationFactor.Value;
            if (emittingSummury.Length != 0)
            {
                lstEmitting.AddRange(emittingSummury.ToList());
                lstEmitting.AddRange(emittingRaw.ToList());

                var lstEmittingTempTemp = new List<CALC.Emitting>();
                var lstEmittingSummury = new List<CALC.Emitting>();
                for (int i = 0; i < lstEmitting.Count; i++)
                {
                    if (lstEmitting[i].Spectrum != null)
                    {
                        if (lstEmitting[i].Spectrum.СorrectnessEstimations == true)
                        {
                            lstEmittingSummury.Add(lstEmitting[i]);
                        }
                        else
                        {
                            lstEmittingTempTemp.Add(lstEmitting[i]);
                        }
                    }
                    else
                    {
                        lstEmittingTempTemp.Add(lstEmitting[i]);
                    }
                }

                var signalizationNCount = SignalizationNCount;
                if (signalizationNCount != 0)
                {
                    var startTime = StartTime.Value;
                    var stopTime = StopTime.Value;
                    var val = stopTime - startTime;
                    if (val.TotalSeconds > 0)
                    {
                        var TimeBetweenWorkTimesTask_sec = (int)((val.TotalSeconds / signalizationNCount) * 5);
                        if (TimeBetweenWorkTimes_sec < TimeBetweenWorkTimesTask_sec)
                        {
                            TimeBetweenWorkTimes_sec = TimeBetweenWorkTimesTask_sec;
                        }
                    }
                }

                var emitEaw = new CALC.Emitting[0];
                emittingSummury = lstEmittingSummury.ToArray();
                emittingTemp = lstEmittingTempTemp.ToArray();

                CALC.EmitParams emitParams = new CALC.EmitParams()
                {
                    TimeBetweenWorkTimes_sec = TimeBetweenWorkTimes_sec,
                    TypeJoinSpectrum = TypeJoinSpectrum,
                    CrossingBWPercentageForGoodSignals = CrossingBWPercentageForGoodSignals,
                    CrossingBWPercentageForBadSignals = CrossingBWPercentageForBadSignal,
                    AnalyzeByChannel = AnalyzeByChannel,
                    CorrelationAnalize = CorrelationAnalize,
                    CorrelationFactor = prmCorrelationFactor,
                    MaxFreqDeviation = MaxFreqDeviation,
                    CorrelationAdaptation = this._config.CorrelationAdaptation,
                    MaxNumberEmitingOnFreq = this._config.MaxNumberEmitingOnFreq,
                    MinCoeffCorrelation = this._config.MinCoeffCorrelation,
                    UkraineNationalMonitoring = this._config.UkraineNationalMonitoring
                };


                CALC.CalcGroupingEmitting.CalcGrouping(ref prmCorrelationFactor, emitParams, ref emitEaw, ref emittingTemp, ref emittingSummury, NoiseLevel_dBm, this._config.CountMaxEmission);
                //CALC.CalcGroupingEmitting.CalcGrouping(TimeBetweenWorkTimes_sec, TypeJoinSpectrum, CrossingBWPercentageForGoodSignals, CrossingBWPercentageForBadSignal, AnalyzeByChannel, CorrelationAnalize, ref prmCorrelationFactor, MaxFreqDeviation, this._config.CorrelationAdaptation, this._config.MaxNumberEmitingOnFreq, this._config.MinCoeffCorrelation, this._config.UkraineNationalMonitoring, ref emitEaw, ref emittingTemp, ref emittingSummury, NoiseLevel_dBm, this._config.CountMaxEmission);
                //CALC.CalcGroupingEmitting.CalcGrouping(60, 0, 90, 60, false, true, ref prmCorrelationFactor, 0.0001, true, 25, 0.8, true, ref emittingRaw, ref emittingTemp, ref emittingSummury, -100, 1000);
                lstEmitting = new List<CALC.Emitting>();
                lstEmitting.AddRange(emittingSummury);
                lstEmitting.AddRange(emittingTemp);

                for (int i = 0; i < lstEmitting.Count; i++)
                {
                    lstEmitting[i].SensorId = (int)sensorId;
                }
                result.Emittings = ConvertEmitting(lstEmitting.ToArray());
            }
            else
            {
                result.Emittings = ConvertEmitting(emittingRaw);
            }
        }
        private CALC.Emitting[] ConvertEmitting(Emitting[] emittings)
        {
            int StartLevelsForLevelDistribution = -150;
            int NumberPointForLevelDistribution = 200;

            foreach (var emitting in emittings)
            {
                var levelsDistribution = new LevelsDistribution();
                levelsDistribution.Count = new int[NumberPointForLevelDistribution];
                levelsDistribution.Levels = new int[NumberPointForLevelDistribution];
                for (var i = 0; i < NumberPointForLevelDistribution; i++)
                {
                    levelsDistribution.Levels[i] = StartLevelsForLevelDistribution + i;
                    levelsDistribution.Count[i] = 0;
                }
                
                for (var i = 0; i < levelsDistribution.Levels.Length; i++)
                {
                    for (var j = 0; j < emitting.LevelsDistribution.Levels.Length; j++)
                    {
                        if (levelsDistribution.Levels[i] == emitting.LevelsDistribution.Levels[j])
                        {
                            levelsDistribution.Count[i] = emitting.LevelsDistribution.Count[j];
                        }
                    }
                }
                emitting.LevelsDistribution = levelsDistribution;
            }



            var listEmittings = new List<CALC.Emitting>();
            foreach (var emitting in emittings)
            {
                var emitt = new CALC.Emitting()
                {
                    CurentPower_dBm = emitting.CurentPower_dBm,
                    LastDetaileMeas = emitting.LastDetaileMeas,
                    MeanDeviationFromReference = emitting.MeanDeviationFromReference,
                    ReferenceLevel_dBm = emitting.ReferenceLevel_dBm,
                    SensorId = emitting.SensorId,
                    SpectrumIsDetailed = emitting.SpectrumIsDetailed,
                    StartFrequency_MHz = emitting.StartFrequency_MHz,
                    StopFrequency_MHz = emitting.StopFrequency_MHz,
                    TriggerDeviationFromReference = emitting.TriggerDeviationFromReference,
                    LevelsDistribution = new CALC.LevelsDistribution()
                    {
                        Count = emitting.LevelsDistribution.Count,
                        Levels = emitting.LevelsDistribution.Levels
                    },
                    /*
                    EmittingParameters = new CALC.EmittingParameters()
                    {
                        FreqDeviation = emitting.EmittingParameters.FreqDeviation,
                        RollOffFactor = emitting.EmittingParameters.RollOffFactor,
                        Standard = emitting.EmittingParameters.Standard,
                        StandardBW = emitting.EmittingParameters.StandardBW,
                        TriggerFreqDeviation = emitting.EmittingParameters.TriggerFreqDeviation
                    },
                    SignalMask = new CALC.SignalMask()
                    {
                        Freq_kHz = emitting.SignalMask.Freq_kHz,
                        Loss_dB = emitting.SignalMask.Loss_dB
                    },
                    */
                    Spectrum = new CALC.Spectrum()
                    {
                        Bandwidth_kHz = emitting.Spectrum.Bandwidth_kHz,
                        Contravention = emitting.Spectrum.Contravention,
                        Levels_dBm = emitting.Spectrum.Levels_dBm,
                        MarkerIndex = emitting.Spectrum.MarkerIndex,
                        T1 = emitting.Spectrum.T1,
                        T2 = emitting.Spectrum.T2,
                        SignalLevel_dBm = emitting.Spectrum.SignalLevel_dBm,
                        SpectrumStartFreq_MHz = emitting.Spectrum.SpectrumStartFreq_MHz,
                        SpectrumSteps_kHz = emitting.Spectrum.SpectrumSteps_kHz,
                        TraceCount = emitting.Spectrum.TraceCount,
                        СorrectnessEstimations = emitting.Spectrum.СorrectnessEstimations
                    }
                };

                var wtList = new List<CALC.WorkTime>();
                foreach (var workTime in emitting.WorkTimes)
                {
                    var wt = new CALC.WorkTime()
                    {
                        HitCount = workTime.HitCount,
                        PersentAvailability = workTime.PersentAvailability,
                        ScanCount = workTime.ScanCount,
                        StartEmitting = workTime.StartEmitting,
                        StopEmitting = workTime.StopEmitting,
                        TempCount = workTime.TempCount
                    };
                    wtList.Add(wt);
                }
                emitt.WorkTimes = wtList.ToArray();

                /*
                var sysInfoList = new List<CALC.SignalingSysInfo>();
                foreach (var sysInfo in emitting.SysInfos)
                {
                    var si = new CALC.SignalingSysInfo()
                    {
                        BandWidth_Hz = sysInfo.BandWidth_Hz,
                        BSIC = sysInfo.BSIC,
                        ChannelNumber = sysInfo.ChannelNumber,
                        CID = sysInfo.CID,
                        CtoI = sysInfo.CtoI,
                        Freq_Hz = sysInfo.Freq_Hz,
                        LAC = sysInfo.LAC,
                        Level_dBm = sysInfo.Level_dBm,
                        MCC = sysInfo.MCC,
                        MNC = sysInfo.MNC,
                        Power = sysInfo.Power,
                        RNC = sysInfo.RNC,
                        Standard = sysInfo.Standard
                    };

                    var wtSysList = new List<CALC.WorkTime>();
                    foreach (var workTime in sysInfo.WorkTimes)
                    {
                        var wt = new CALC.WorkTime()
                        {
                            HitCount = workTime.HitCount,
                            PersentAvailability = workTime.PersentAvailability,
                            ScanCount = workTime.ScanCount,
                            StartEmitting = workTime.StartEmitting,
                            StopEmitting = workTime.StopEmitting,
                            TempCount = workTime.TempCount
                        };
                        wtSysList.Add(wt);
                    }
                    si.WorkTimes = wtSysList.ToArray();
                    sysInfoList.Add(si);
                }
                emitt.SysInfos = sysInfoList.ToArray();
                */
                listEmittings.Add(emitt);
            }
            return listEmittings.ToArray();
        }
        private Emitting[] ConvertEmitting(CALC.Emitting[] emittings)
        {
            var listEmittings = new List<Emitting>();
            foreach (var emitting in emittings)
            {
                var emitt = new Emitting()
                {
                    CurentPower_dBm = emitting.CurentPower_dBm,
                    LastDetaileMeas = emitting.LastDetaileMeas,
                    MeanDeviationFromReference = emitting.MeanDeviationFromReference,
                    ReferenceLevel_dBm = emitting.ReferenceLevel_dBm,
                    SensorId = emitting.SensorId,
                    SpectrumIsDetailed = emitting.SpectrumIsDetailed,
                    StartFrequency_MHz = emitting.StartFrequency_MHz,
                    StopFrequency_MHz = emitting.StopFrequency_MHz,
                    TriggerDeviationFromReference = emitting.TriggerDeviationFromReference,
                    LevelsDistribution = new LevelsDistribution()
                    {
                        Count = emitting.LevelsDistribution.Count,
                        Levels = emitting.LevelsDistribution.Levels
                    },
                    /*
                    EmittingParameters = new EmittingParameters()
                    {
                        FreqDeviation = emitting.EmittingParameters.FreqDeviation,
                        RollOffFactor = emitting.EmittingParameters.RollOffFactor,
                        Standard = emitting.EmittingParameters.Standard,
                        StandardBW = emitting.EmittingParameters.StandardBW,
                        TriggerFreqDeviation = emitting.EmittingParameters.TriggerFreqDeviation
                    },
                    SignalMask = new SignalMask()
                    {
                        Freq_kHz = emitting.SignalMask.Freq_kHz,
                        Loss_dB = emitting.SignalMask.Loss_dB
                    },
                    */
                    Spectrum = new Spectrum()
                    {
                        Bandwidth_kHz = emitting.Spectrum.Bandwidth_kHz,
                        Contravention = emitting.Spectrum.Contravention,
                        Levels_dBm = emitting.Spectrum.Levels_dBm,
                        MarkerIndex = emitting.Spectrum.MarkerIndex,
                        T1 = emitting.Spectrum.T1,
                        T2 = emitting.Spectrum.T2,
                        SignalLevel_dBm = emitting.Spectrum.SignalLevel_dBm,
                        SpectrumStartFreq_MHz = emitting.Spectrum.SpectrumStartFreq_MHz,
                        SpectrumSteps_kHz = emitting.Spectrum.SpectrumSteps_kHz,
                        TraceCount = emitting.Spectrum.TraceCount,
                        СorrectnessEstimations = emitting.Spectrum.СorrectnessEstimations
                    }
                };

                var wtList = new List<WorkTime>();
                foreach (var workTime in emitting.WorkTimes)
                {
                    var wt = new WorkTime()
                    {
                        HitCount = workTime.HitCount,
                        PersentAvailability = workTime.PersentAvailability,
                        ScanCount = workTime.ScanCount,
                        StartEmitting = workTime.StartEmitting,
                        StopEmitting = workTime.StopEmitting,
                        TempCount = workTime.TempCount
                    };
                    wtList.Add(wt);
                }
                emitt.WorkTimes = wtList.ToArray();
                /*
                var sysInfoList = new List<SignalingSysInfo>();
                foreach (var sysInfo in emitting.SysInfos)
                {
                    var si = new SignalingSysInfo()
                    {
                        BandWidth_Hz = sysInfo.BandWidth_Hz,
                        BSIC = sysInfo.BSIC,
                        ChannelNumber = sysInfo.ChannelNumber,
                        CID = sysInfo.CID,
                        CtoI = sysInfo.CtoI,
                        Freq_Hz = sysInfo.Freq_Hz,
                        LAC = sysInfo.LAC,
                        Level_dBm = sysInfo.Level_dBm,
                        MCC = sysInfo.MCC,
                        MNC = sysInfo.MNC,
                        Power = sysInfo.Power,
                        RNC = sysInfo.RNC,
                        Standard = sysInfo.Standard
                    };

                    var wtSysList = new List<WorkTime>();
                    foreach (var workTime in sysInfo.WorkTimes)
                    {
                        var wt = new WorkTime()
                        {
                            HitCount = workTime.HitCount,
                            PersentAvailability = workTime.PersentAvailability,
                            ScanCount = workTime.ScanCount,
                            StartEmitting = workTime.StartEmitting,
                            StopEmitting = workTime.StopEmitting,
                            TempCount = workTime.TempCount
                        };
                        wtSysList.Add(wt);
                    }
                    si.WorkTimes = wtSysList.ToArray();
                    sysInfoList.Add(si);
                }
                emitt.SysInfos = sysInfoList.ToArray();
                */

                var levelDistributionCorrCount = new List<int>();
                var levelDistributionCorrLevel = new List<int>();
                if (emitt != null)
                {
                    if (emitt.LevelsDistribution != null)
                    {
                        var newEmittingLevelsDistributionCount = emitt.LevelsDistribution.Count;
                        var newEmittingLevelsDistributionLevel = emitt.LevelsDistribution.Levels;
                        int startIndex = -1;
                        int endIndex = -1;
                        for (var j = 0; j < newEmittingLevelsDistributionCount.Length; j++)
                        {
                            var valCount = newEmittingLevelsDistributionCount[j];
                            var valLevel = newEmittingLevelsDistributionLevel[j];
                            if (valCount == 0)
                            {
                                continue;
                            }
                            else
                            {
                                startIndex = j;
                                break;
                            }
                        }

                        for (var j = newEmittingLevelsDistributionCount.Length - 1; j >= 0; j--)
                        {
                            var valCount = newEmittingLevelsDistributionCount[j];
                            var valLevel = newEmittingLevelsDistributionLevel[j];
                            if (valCount == 0)
                            {
                                continue;
                            }
                            else
                            {
                                endIndex = j;
                                break;
                            }
                        }

                        if ((startIndex >= 0) && (endIndex >= 0))
                        {
                            for (var k = startIndex; k <= endIndex; k++)
                            {
                                var valCount = newEmittingLevelsDistributionCount[k];
                                var valLevel = newEmittingLevelsDistributionLevel[k];

                                levelDistributionCorrCount.Add(valCount);
                                levelDistributionCorrLevel.Add(valLevel);
                            }
                            emitt.LevelsDistribution.Levels = levelDistributionCorrLevel.ToArray();
                            emitt.LevelsDistribution.Count = levelDistributionCorrCount.ToArray();
                        }
                        else
                        {
                            emitt.LevelsDistribution.Levels = newEmittingLevelsDistributionLevel.ToArray();
                            emitt.LevelsDistribution.Count = newEmittingLevelsDistributionCount.ToArray();
                        }
                    }
                }



                listEmittings.Add(emitt);
            }
            return listEmittings.ToArray();
        }
        private CALC.Emitting[] LoadOthersEmittings(long subTaskSensorId)
        {
            // IEmitting
            var emittings = new Dictionary<long, CALC.Emitting>();
            var builderEmittings = this._dataLayer.GetBuilder<MD.IEmitting>().From();
            builderEmittings.Select(c => c.Id, c => c.CurentPower_dBm, c => c.MeanDeviationFromReference, c => c.ReferenceLevel_dBm, c => c.TriggerDeviationFromReference, c => c.SensorId, c => c.RollOffFactor, c => c.StandardBW, c => c.StartFrequency_MHz, c => c.StopFrequency_MHz, c => c.LevelsDistributionCount, c => c.LevelsDistributionLvl, c => c.Loss_dB, c => c.Freq_kHz);
            builderEmittings.Where(c => c.RES_MEAS.SUBTASK_SENSOR.Id, ConditionOperator.Equal, subTaskSensorId);
            this._queryExecutor.Fetch(builderEmittings, readerEmittings =>
            {
                while (readerEmittings.Read())
                {
                    var emitting = new CALC.Emitting();
                    emitting.CurentPower_dBm = readerEmittings.GetValue(c => c.CurentPower_dBm).GetValueOrDefault();
                    emitting.MeanDeviationFromReference = readerEmittings.GetValue(c => c.MeanDeviationFromReference).GetValueOrDefault();
                    emitting.ReferenceLevel_dBm = readerEmittings.GetValue(c => c.ReferenceLevel_dBm).GetValueOrDefault();
                    emitting.TriggerDeviationFromReference = readerEmittings.GetValue(c => c.TriggerDeviationFromReference).GetValueOrDefault();
                    emitting.StartFrequency_MHz = readerEmittings.GetValue(c => c.StartFrequency_MHz).GetValueOrDefault();
                    emitting.StopFrequency_MHz = readerEmittings.GetValue(c => c.StopFrequency_MHz).GetValueOrDefault();
                    emitting.SensorId = (int?)readerEmittings.GetValue(c => c.SensorId);

                    var param = new CALC.EmittingParameters();
                    param.RollOffFactor = readerEmittings.GetValue(c => c.RollOffFactor).GetValueOrDefault();
                    param.StandardBW = readerEmittings.GetValue(c => c.StandardBW).GetValueOrDefault();
                    emitting.EmittingParameters = param;

                    var levelDistr = new CALC.LevelsDistribution();
                    levelDistr.Count = readerEmittings.GetValue(c => c.LevelsDistributionCount);
                    levelDistr.Levels = readerEmittings.GetValue(c => c.LevelsDistributionLvl);
                    emitting.LevelsDistribution = levelDistr;

                    var signMask = new CALC.SignalMask();
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
                var workTimes = new List<CALC.WorkTime>();
                var builderWorkTimes = this._dataLayer.GetBuilder<MD.IWorkTime>().From();
                builderWorkTimes.Select(c => c.HitCount, c => c.PersentAvailability, c => c.StartEmitting, c => c.StopEmitting);
                builderWorkTimes.Where(c => c.EMITTING.Id, ConditionOperator.Equal, emittingId);
                this._queryExecutor.Fetch(builderWorkTimes, readerWorkTimes =>
                {
                    while (readerWorkTimes.Read())
                    {
                        var workTime = new CALC.WorkTime();
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
                var spectrum = new CALC.Spectrum();
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
                var sysInfos = new Dictionary<long, CALC.SignalingSysInfo>();
                var builderSysInfo = this._dataLayer.GetBuilder<MD.ISignalingSysInfo>().From();
                builderSysInfo.Select(c => c.BandWidth_Hz, c => c.BSIC, c => c.ChannelNumber, c => c.CID, c => c.CtoI, c => c.Freq_Hz, c => c.LAC, c => c.Level_dBm, c => c.MCC, c => c.MNC, c => c.Power, c => c.RNC, c => c.Standard);
                builderSysInfo.Where(c => c.EMITTING.Id, ConditionOperator.Equal, emittingId);
                this._queryExecutor.Fetch(builderSysInfo, readerSysInfo =>
                {
                    while (readerSysInfo.Read())
                    {
                        var sysInfo = new CALC.SignalingSysInfo();
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
                    var sysWorkTimes = new List<CALC.WorkTime>();
                    var builderSysWorkTimes = this._dataLayer.GetBuilder<MD.ISignalingSysInfoWorkTime>().From();
                    builderSysWorkTimes.Select(c => c.HitCount, c => c.PersentAvailability, c => c.StartEmitting, c => c.StopEmitting);
                    builderSysWorkTimes.Where(c => c.SYSINFO.Id, ConditionOperator.Equal, sysInfoId);
                    this._queryExecutor.Fetch(builderSysWorkTimes, readerSysWorkTimes =>
                    {
                        while (readerSysWorkTimes.Read())
                        {
                            var sysWorkTime = new CALC.WorkTime();
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

            int StartLevelsForLevelDistribution = -150;
            int NumberPointForLevelDistribution = 200;

            foreach (var emitting in emittings.Values)
            {
                var levelsDistribution = new LevelsDistribution();
                levelsDistribution.Count = new int[NumberPointForLevelDistribution];
                levelsDistribution.Levels = new int[NumberPointForLevelDistribution];
                for (var i = 0; i < NumberPointForLevelDistribution; i++)
                {
                    levelsDistribution.Levels[i] = StartLevelsForLevelDistribution + i;
                    levelsDistribution.Count[i] = 0;
                }

                for (var i = 0; i < levelsDistribution.Levels.Length; i++)
                {
                    for (var j = 0; j < emitting.LevelsDistribution.Levels.Length; j++)
                    {
                        if (levelsDistribution.Levels[i] == emitting.LevelsDistribution.Levels[j])
                        {
                            levelsDistribution.Count[i] = emitting.LevelsDistribution.Count[j];
                        }
                    }
                }
                emitting.LevelsDistribution = new CALC.LevelsDistribution()
                {
                     Count = levelsDistribution.Count,
                     Levels = levelsDistribution.Levels
                }; 
            }

            return emittings.Values.ToArray();
        }
        private void DeleteOldResult(long subTaskSensorId)
        {
            //var queryDelSubTaskSensor = this._dataLayer.GetBuilder<MD.ILinkSubTaskSensorMasterId>().Delete();
            //queryDelSubTaskSensor.Where(c => c.SUBTASK_SENSOR.Id, ConditionOperator.Equal, subTaskSensorId);
            //this._queryExecutor.Execute(queryDelSubTaskSensor);

            var queryDelResLoc = this._dataLayer.GetBuilder<MD.IResLocSensorMeas>().Delete();
            queryDelResLoc.Where(c => c.RES_MEAS.SUBTASK_SENSOR.Id, ConditionOperator.Equal, subTaskSensorId);
            this._queryExecutor.Execute(queryDelResLoc);

            var queryDelRefLevel = this._dataLayer.GetBuilder<MD.IReferenceLevels>().Delete();
            queryDelRefLevel.Where(c => c.RES_MEAS.SUBTASK_SENSOR.Id, ConditionOperator.Equal, subTaskSensorId);
            this._queryExecutor.Execute(queryDelRefLevel);


            var queryDelEmitWorkTime = this._dataLayer.GetBuilder<MD.IWorkTime>().Delete();
            queryDelEmitWorkTime.Where(c => c.EMITTING.RES_MEAS.SUBTASK_SENSOR.Id, ConditionOperator.Equal, subTaskSensorId);
            this._queryExecutor.Execute(queryDelEmitWorkTime);

            var queryDelSpectrum = this._dataLayer.GetBuilder<MD.ISpectrum>().Delete();
            queryDelSpectrum.Where(c => c.EMITTING.RES_MEAS.SUBTASK_SENSOR.Id, ConditionOperator.Equal, subTaskSensorId);
            this._queryExecutor.Execute(queryDelSpectrum);

            var queryDelSysInfoWorkTime = this._dataLayer.GetBuilder<MD.ISignalingSysInfoWorkTime>().Delete();
            queryDelSysInfoWorkTime.Where(c => c.SYSINFO.EMITTING.RES_MEAS.SUBTASK_SENSOR.Id, ConditionOperator.Equal, subTaskSensorId);
            this._queryExecutor.Execute(queryDelSysInfoWorkTime);

            var queryDelSysInfo = this._dataLayer.GetBuilder<MD.ISignalingSysInfo>().Delete();
            queryDelSysInfo.Where(c => c.EMITTING.RES_MEAS.SUBTASK_SENSOR.Id, ConditionOperator.Equal, subTaskSensorId);
            this._queryExecutor.Execute(queryDelSysInfo);

            var queryDelEmitting = this._dataLayer.GetBuilder<MD.IEmitting>().Delete();
            queryDelEmitting.Where(c => c.RES_MEAS.SUBTASK_SENSOR.Id, ConditionOperator.Equal, subTaskSensorId);
            this._queryExecutor.Execute(queryDelEmitting);

            var queryDelRes = this._dataLayer.GetBuilder<MD.IResMeas>().Delete();
            queryDelRes.Where(c => c.SUBTASK_SENSOR.Id, ConditionOperator.Equal, subTaskSensorId);
            this._queryExecutor.Execute(queryDelRes);
        }
    }
}
