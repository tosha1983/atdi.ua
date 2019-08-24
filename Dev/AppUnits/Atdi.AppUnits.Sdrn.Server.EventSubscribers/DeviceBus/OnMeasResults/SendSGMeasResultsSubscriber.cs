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
            ILogger logger)
            : base(messagesSite, logger)
        {
            this._messagePublisher = messagePublisher;
            this._dataLayer = dataLayer;
            this._environment = environment;
            this._statistics = statistics;
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
                    throw new InvalidOperationException("Incorrect MeasurementType. Expected is SpectrumOccupation");
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
                        if (this.SaveMeasResultSignaling(ref context))
                        {
                            var busEvent = new SGMeasResultAppeared($"OnSGMeasResultAppeared", "SendSGMeasResultsSubscriber")
                            {
                                MeasResultId = context.resMeasId
                            };
                            _eventEmitter.Emit(busEvent);
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
        private bool SaveMeasResultSignaling(ref HandleContext context)
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
                                        builderInsertIWorkTime.SetValue(c => c.EmittingId, valInsReferenceEmitting.Id);
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
    }
}
