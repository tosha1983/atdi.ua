using Atdi.DataModels.Sdrns.Server;
using Atdi.Platform.Workflows;
using Atdi.Contracts.Api.EventSystem;
using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Contracts.CoreServices.EntityOrm;
using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using Atdi.Contracts.Sdrn.Server;
using DM = Atdi.DataModels.Sdrns.Server.Entities;
using Atdi.DataModels.DataConstraint;





namespace Atdi.AppUnits.Sdrn.Server.PrimaryHandlers.PipelineHandlers
{
    public class InitOnlineMeasurementPipelineHandler : IPipelineHandler<InitOnlineMeasurementPipebox, InitOnlineMeasurementPipebox>
    {
        private readonly IDataLayer<EntityDataOrm> _dataLayer;
        private readonly IEventEmitter _eventEmitter;
        private readonly ISdrnServerEnvironment _environment;
        private readonly ISdrnMessagePublisher _messagePublisher;
        private readonly ILogger _logger;
        private readonly IPipelineSite _pipelineSite;

        public InitOnlineMeasurementPipelineHandler(IPipelineSite pipelineSite, IEventEmitter eventEmitter, IDataLayer<EntityDataOrm> dataLayer, ISdrnServerEnvironment environment, ISdrnMessagePublisher messagePublisher, ILogger logger)
        {
            this._dataLayer = dataLayer;
            this._eventEmitter = eventEmitter;
            this._logger = logger;
            this._environment = environment;
            this._messagePublisher = messagePublisher;
            this._pipelineSite = pipelineSite;
        }


        public InitOnlineMeasurementPipebox Handle(InitOnlineMeasurementPipebox data, IPipelineContext<InitOnlineMeasurementPipebox, InitOnlineMeasurementPipebox> context)
        {
            using (this._logger.StartTrace(Contexts.ThisComponent, Categories.OnInitOnlineMeasurement, this))
            {
                var resultSendEvent = new InitOnlineMeasurementPipebox();
                var result = new InitOnlineMeasurementPipebox();
                // 1. Поиск сенсора - нету - отказ
                // 2. Поиск уже иницированых измерений по сенсору, 
                //    если активно отказ 
                //    иначе нужно обновит состояние в БД об измирении и отменить его со стороны сервера по причин езавершения времени отведенного клиентом для измерения
                // 3. Сгенерировать серверный токен и создать щапись об измерении в БД
                // 4. Подготовить и вернуть результат

                try
                {
                    if (data == null)
                    {
                        throw new ArgumentNullException(nameof(data));
                    }

                    if (data.Period.TotalMinutes <= 0)
                    {
                        throw new ArgumentException("Incorrect value of Period.");
                    }
                    using (var dbScope = this._dataLayer.CreateScope<SdrnServerDataContext>())
                    {

                        // step 1

                        var sensorQuery = _dataLayer.GetBuilder<DM.ISensor>()
                            .From()
                            .Select(c => c.Id)
                            .Select(c => c.Name)
                            .Select(c => c.TechId)
                            .Where(c => c.Id, ConditionOperator.Equal, data.SensorId);

                        var sensorExists = dbScope.Executor.ExecuteAndFetch(sensorQuery, reader =>
                        {
                            var exists = reader.Read();
                            if (exists)
                            {
                                if (reader.GetValue(c => c.Id) == data.SensorId)
                                {
                                    data.SensorName = reader.GetValue(c => c.Name);
                                    data.SensorTechId = reader.GetValue(c => c.TechId);
                                    exists = true;
                                }
                            // read some data od the Sensor with ID = options.SensorId
                        }
                            return exists;
                        });

                        if (!sensorExists)
                        {
                            result.Allowed = false;
                            result.Message = $"Not found a sensor with ID #{data.SensorId}.";
                            return result;
                        }

                        // step 2

                        var onlineMeasQuery = _dataLayer.GetBuilder<DM.IOnlineMesurement>()
                            .From()
                            .Where(c => c.SENSOR.Id, ConditionOperator.Equal, data.SensorId)
                            .Where(c => c.StatusCode, ConditionOperator.NotIn,
                                (byte)OnlineMeasurementStatus.CanceledByClient,
                                (byte)OnlineMeasurementStatus.CanceledBySensor,
                                (byte)OnlineMeasurementStatus.CanceledByServer,
                                (byte)OnlineMeasurementStatus.DeniedBySensor,
                                (byte)OnlineMeasurementStatus.DeniedByServer)
                            .Select(
                                c => c.Id,
                                c => c.CreatedDate,
                                c => c.StatusCode,
                                c => c.PeriodMinutes,
                                c => c.ServerToken,
                                c => c.StartTime,
                                c => c.FinishTime);

                        var onlineMeases = dbScope.Executor.ExecuteAndFetch(onlineMeasQuery, reader =>
                        {
                            var list = new List<InitiationOnlineMesurementModel>();
                            while (reader.Read())
                            {
                                var model = new InitiationOnlineMesurementModel
                                {
                                    Id = reader.GetValue(c => c.Id),
                                    CreatedDate = reader.GetValue(c => c.CreatedDate),
                                    FinishTime = reader.GetValue(c => c.FinishTime),
                                    PeriodMinutes = reader.GetValue(c => c.PeriodMinutes),
                                    ServerToken = reader.GetValue(c => c.ServerToken),
                                    StartTime = reader.GetValue(c => c.StartTime),
                                    Status = reader.GetValue(c => c.StatusCode)
                                };
                                list.Add(model);
                            }
                            return list.ToArray();
                        });
                        for (int i = 0; i < onlineMeases.Length; i++)
                        {
                            var meas = onlineMeases[i];
                            if (meas.Status == (byte)OnlineMeasurementStatus.Initiation || meas.Status == (byte)OnlineMeasurementStatus.WaitSensor)
                            {
                                if ((DateTimeOffset.Now - meas.CreatedDate).TotalMinutes > meas.PeriodMinutes)
                                {
                                    var updateQuery = _dataLayer.GetBuilder<DM.IOnlineMesurement>()
                                        .Update()
                                        .SetValue(c => c.StatusCode, (byte)OnlineMeasurementStatus.CanceledByServer)
                                        .SetValue(c => c.StatusNote, "CanceledByServer: Measurement period was expired")
                                        .SetValue(c => c.FinishTime, DateTimeOffset.Now)
                                        .Where(c => c.Id, ConditionOperator.Equal, meas.Id);

                                    dbScope.Executor.Execute(updateQuery);
                                }
                                else
                                {
                                    result.Allowed = false;
                                    result.Message = $"The sensor is busy with another measurement (meas token is '{meas.ServerToken}')";
                                    return result;
                                }
                            }
                            else if (meas.Status == (byte)OnlineMeasurementStatus.SonsorReady)
                            {
                                if ((DateTimeOffset.Now - (meas.StartTime ?? meas.CreatedDate)).TotalMinutes > meas.PeriodMinutes)
                                {
                                    var updateQuery = _dataLayer.GetBuilder<DM.IOnlineMesurement>()
                                        .Update()
                                        .SetValue(c => c.StatusCode, (byte)OnlineMeasurementStatus.CanceledByServer)
                                        .SetValue(c => c.StatusNote, "CanceledByServer: Measurement period was expired")
                                        .SetValue(c => c.FinishTime, DateTimeOffset.Now)
                                        .Where(c => c.Id, ConditionOperator.Equal, meas.Id);

                                    dbScope.Executor.Execute(updateQuery);
                                }
                                else
                                {
                                    result.Allowed = false;
                                    result.Message = $"The sensor is busy with another measurement (meas token is '{meas.ServerToken}')";
                                    return result;
                                }
                            }
                        }

                        // step 3

                        var serverToken = Guid.NewGuid();
                        result.ServerToken = serverToken.ToByteArray();

                        var insert = _dataLayer.GetBuilder<DM.IOnlineMesurement>()
                            .Insert()
                            .SetValue(c => c.PeriodMinutes, Convert.ToInt32(data.Period.TotalMinutes))
                            .SetValue(c => c.SENSOR.Id, data.SensorId)
                            .SetValue(c => c.CreatedDate, DateTimeOffset.Now)
                            .SetValue(c => c.StatusCode, (byte)OnlineMeasurementStatus.Initiation)
                            .SetValue(c => c.StatusNote, "Initiation: SDRN Server sent request to the Sensor")
                            .SetValue(c => c.ServerToken, serverToken);

                        var pk = dbScope.Executor.Execute<DM.IOnlineMesurement_PK>(insert);

                        result.OnlineMeasLocalId = pk.Id;
                        result.Allowed = true;
                        result.SensorId = data.SensorId;
                        result.Period = data.Period;
                        result.SensorName = data.SensorName;
                        result.SensorTechId = data.SensorTechId;
                        data = result;


                        var site = this._pipelineSite.GetByName<InitOnlineMeasurementPipebox, InitOnlineMeasurementPipebox>(Pipelines.ClientSendEventOnlineMeasurement);
                        resultSendEvent = site.Execute(new InitOnlineMeasurementPipebox()
                        {
                            Allowed = data.Allowed,
                            Message = data.Message,
                            OnlineMeasLocalId = data.OnlineMeasLocalId,
                            OnlineMeasMasterId = data.OnlineMeasMasterId,
                            Period = data.Period,
                            SensorId = data.SensorId,
                            SensorName = data.SensorName,
                            SensorTechId = data.SensorTechId,
                            ServerToken = data.ServerToken
                        });
                    }
                }
                catch (Exception e)
                {
                    _logger.Exception(Contexts.ThisComponent, (EventCategory)"OnInitOnlineMeasurement", e, this);
                    throw;
                }
                return resultSendEvent;
            }
        }
    }
}
