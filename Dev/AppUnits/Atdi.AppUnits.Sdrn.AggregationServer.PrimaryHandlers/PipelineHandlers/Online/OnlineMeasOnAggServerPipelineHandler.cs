﻿using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrns.Server;
using Atdi.Platform.Workflows;
using System;
using System.Collections.Generic;
using Atdi.Contracts.Api.EventSystem;
using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Contracts.CoreServices.EntityOrm;
using Atdi.Platform.Logging;
using Atdi.AppUnits.Sdrn.AggregationServer.PrimaryHandlers.Handlers;
using SdrnsServer = Atdi.DataModels.Sdrns.Server;
using DM = Atdi.DataModels.Sdrns.Server.Entities;
using Atdi.DataModels.DataConstraint;
using Atdi.Contracts.Sdrn.Server;
using Atdi.DataModels.Sdrns.Server.Events.OnlineMeasurement;


namespace Atdi.AppUnits.Sdrn.AggregationServer.PrimaryHandlers.PipelineHandlers
{
    public class OnlineMeasOnAggServerPipelineHandler : IPipelineHandler<InitOnlineMeasurementPipebox, InitOnlineMeasurementPipebox>
    {
        private readonly IDataLayer<EntityDataOrm> _dataLayer;
        private readonly ILogger _logger;
        private readonly IEventEmitter _eventEmitter;


        public OnlineMeasOnAggServerPipelineHandler(IDataLayer<EntityDataOrm> dataLayer, IEventEmitter eventEmitter, ILogger logger)
        {
            this._dataLayer = dataLayer;
            this._logger = logger;
            this._eventEmitter = eventEmitter;
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public InitOnlineMeasurementPipebox Handle(InitOnlineMeasurementPipebox data, IPipelineContext<InitOnlineMeasurementPipebox, InitOnlineMeasurementPipebox> context)
        {
            using (this._logger.StartTrace(Contexts.ThisComponent, Categories.OnlineMeasOnAggServerPipelineHandler, this))
            {
                var result = new InitOnlineMeasurementPipebox();
                try
                {
                    // 1. Поиск сенсора - нету - отказ
                    // 2. Поиск уже иницированых измерений по сенсору, 
                    //    если активно отказ 
                    //    иначе нужно обновит состояние в БД об измирении и отменить его со стороны сервера по причин езавершения времени отведенного клиентом для измерения
                    // 3. Сгенерировать серверный токен и создать щапись об измерении в БД
                    // 4. Подготовить и вернуть результат


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
                            .Where(c => c.Id, ConditionOperator.Equal, data.SensorId);

                        var sensorExists = dbScope.Executor.ExecuteAndFetch(sensorQuery, reader =>
                        {
                            var exists = reader.Read();
                            if (exists)
                            {
                                exists = reader.GetValue(c => c.Id) == data.SensorId;
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

                        result.OnlineMeasMasterId = data.OnlineMeasMasterId;
                        result.OnlineMeasLocalId = pk.Id;
                        result.Allowed = true;
                        data = result;

                        bool isFindlinkOnlineMesurement = false;
                        var linkOnlineMesurementQuery = _dataLayer.GetBuilder<DM.ILinkOnlineMesurement>()
                             .From()
                             .Select(c => c.Id)
                             .Where(c => c.ONLINE_MEAS.Id, ConditionOperator.Equal, result.OnlineMeasLocalId);
                        var linkOnlineMesurementExists = dbScope.Executor.ExecuteAndFetch(linkOnlineMesurementQuery, reader =>
                        {
                            var exists = reader.Read();
                            if (exists)
                            {
                                isFindlinkOnlineMesurement = true;
                            }
                            return exists;
                        });

                        if (isFindlinkOnlineMesurement == false)
                        {
                            var builderInsertlinkOnlineMesurement = this._dataLayer.GetBuilder<DM.ILinkOnlineMesurement>().Insert();
                            builderInsertlinkOnlineMesurement.SetValue(c => c.OnlineMesurementMasterId, result.OnlineMeasMasterId);
                            builderInsertlinkOnlineMesurement.SetValue(c => c.ONLINE_MEAS.Id, result.OnlineMeasLocalId);
                            var linkOnlineMesurementPK = dbScope.Executor.Execute<DM.ILinkOnlineMesurement_PK>(builderInsertlinkOnlineMesurement);
                        }

                        var initEvent = new OnInitOnlineMeasurement(this.GetType().FullName)
                        {
                            OnlineMeasId = data.OnlineMeasLocalId
                        };
                        this._eventEmitter.Emit(initEvent);

                    }
                    return result;
                }
                catch (Exception e)
                {
                    _logger.Exception(Contexts.ThisComponent, (EventCategory)"OnInitOnlineMeasurement", e, this);
                    throw;
                }
            }
        }
    }
}
