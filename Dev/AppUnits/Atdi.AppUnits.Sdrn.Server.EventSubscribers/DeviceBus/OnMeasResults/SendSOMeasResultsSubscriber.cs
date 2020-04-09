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
    [SubscriptionEvent(EventName = "OnSOMeasResultsDeviceBusEvent", SubscriberName = "SendSOMeasResultsSubscriber")]
    public class SendSOMeasResultsSubscriber : SubscriberBase<DM.MeasResults>
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

        private readonly IStatisticCounter _spectrumOccupationCounter;

        public SendSOMeasResultsSubscriber(
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
                this._spectrumOccupationCounter = _statistics.Counter(Monitoring.Counters.SendMeasResultsSpectrumOccupation);
            }

        }

        protected override void Handle(string sensorName, string sensorTechId, DM.MeasResults deliveryObject, long messageId)
        {
            using (this._logger.StartTrace(Contexts.ThisComponent, Categories.MessageProcessing, this))
            {
                if (deliveryObject.Measurement != MeasurementType.SpectrumOccupation)
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

                        this._spectrumOccupationCounter?.Increment();
                        if (this.SaveMeasResultSpectrumOccupation(ref context))
                        {
                            var busEvent = new SOMeasResultAppeared($"OnSOMeasResultAppeared", "SendSOMeasResultsSubscriber")
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
        private bool SaveMeasResultSpectrumOccupation(ref HandleContext context)
        {
            try
            {
                var measResult = context.measResult;
                var scope = context.scope;
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

                if (!(measResult.SwNumber >= 0 && measResult.SwNumber <= 10000))
                    WriteLog("Incorrect value SwNumber", "IResMeas", context);

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
                var pk = scope.Executor.Execute<MD.IResMeas_PK>(builderInsertIResMeas);
                context.resMeasId = pk.Id;

                if (context.resMeasId > 0)
                {
                    if (measResult.FrequencySamples != null)
                    {
                        bool validationResult = true;
                        foreach (var freqSample in measResult.FrequencySamples)
                        {
                            if (freqSample.Occupation_Pt < 0 || freqSample.Occupation_Pt > 100)
                            {
                                validationResult = false;
                                WriteLog("Incorrect value Occupation_Pt", "IFreqSample", context);
                            }
                            if (freqSample.Freq_MHz < 0 || freqSample.Freq_MHz > 400000)
                            {
                                validationResult = false;
                                WriteLog("Incorrect value Freq_MHz", "IFreqSample", context);
                            }
                            var builderInsertResLevels = this._dataLayer.GetBuilder<MD.IResLevels>().Insert();
                            if (freqSample.LevelMax_dBm >= -150 && freqSample.LevelMax_dBm <= 20)
                                builderInsertResLevels.SetValue(c => c.VMMaxLvl, freqSample.LevelMax_dBm);
                            if (freqSample.LevelMin_dBm >= -150 && freqSample.LevelMin_dBm <= 20)
                                builderInsertResLevels.SetValue(c => c.VMinLvl, freqSample.LevelMin_dBm);
                            if (freqSample.Level_dBm >= -150 && freqSample.Level_dBm <= 20)
                                builderInsertResLevels.SetValue(c => c.ValueLvl, freqSample.Level_dBm);
                            if (freqSample.LevelMinArr >= -150 && freqSample.LevelMinArr <= 20)
                                builderInsertResLevels.SetValue(c => c.LevelMinArr, freqSample.LevelMinArr);
                            if (freqSample.Level_dBmkVm >= 10 && freqSample.Level_dBmkVm <= 140)
                                builderInsertResLevels.SetValue(c => c.ValueSpect, freqSample.Level_dBmkVm);
                            builderInsertResLevels.SetValue(c => c.OccupancySpect, freqSample.Occupation_Pt);
                            if ((freqSample.SpectrumOccupationArr!=null) && (freqSample.SpectrumOccupationArr.Length>0))
                            {
                                var checkSpectrumOccupationArr = new List<float>();
                                for (int i = 0; i < freqSample.SpectrumOccupationArr.Length; i++)
                                {
                                    var spectVal = freqSample.SpectrumOccupationArr[i];
                                    if (freqSample.Occupation_Pt >=0 && freqSample.Occupation_Pt <= 100)
                                    {
                                        checkSpectrumOccupationArr.Add(spectVal);
                                    }
                                }
                                var spectrumOccupationArr = checkSpectrumOccupationArr.ToArray();
                                if (spectrumOccupationArr.Length > 0)
                                {
                                    builderInsertResLevels.SetValue(c => c.SpectrumOccupationArr, spectrumOccupationArr);
                                }
                            }
                            builderInsertResLevels.SetValue(c => c.FreqMeas, freqSample.Freq_MHz);
                            builderInsertResLevels.SetValue(c => c.RES_MEAS.Id, context.resMeasId);
                            if (validationResult)
                                scope.Executor.Execute(builderInsertResLevels);
                        }
                    }
                    if (this.ValidateGeoLocation(measResult.Location, "IResMeas", context))
                    {
                        var builderInsertResLocSensorMeas = this._dataLayer.GetBuilder<MD.IResLocSensorMeas>().Insert();
                        builderInsertResLocSensorMeas.SetValue(c => c.Agl, measResult.Location.AGL);
                        builderInsertResLocSensorMeas.SetValue(c => c.Asl, measResult.Location.ASL);
                        builderInsertResLocSensorMeas.SetValue(c => c.Lon, measResult.Location.Lon);
                        builderInsertResLocSensorMeas.SetValue(c => c.Lat, measResult.Location.Lat);
                        builderInsertResLocSensorMeas.SetValue(c => c.RES_MEAS.Id, context.resMeasId);
                        scope.Executor.Execute(builderInsertResLocSensorMeas);
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
    }
}
