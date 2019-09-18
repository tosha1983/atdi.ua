﻿using Atdi.DataModels.Sdrns.Server;
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
using Atdi.DataModels.Sdrns.Server.Events.OnlineMeasurement;



namespace Atdi.AppUnits.Sdrn.MasterServer.PrimaryHandlers
{
    public class SendEventOnlineMeasurementPipelineHandler : IPipelineHandler<InitOnlineMeasurementPipebox, InitOnlineMeasurementPipebox>
    {
        private readonly IDataLayer<EntityDataOrm> _dataLayer;
        private readonly IEventEmitter _eventEmitter;
        private readonly ISdrnServerEnvironment _environment;
        private readonly ISdrnMessagePublisher _messagePublisher;
        private readonly ILogger _logger;

        public SendEventOnlineMeasurementPipelineHandler(IEventEmitter eventEmitter, IDataLayer<EntityDataOrm> dataLayer, ISdrnServerEnvironment environment, ISdrnMessagePublisher messagePublisher, ILogger logger)
        {
            this._dataLayer = dataLayer;
            this._eventEmitter = eventEmitter;
            this._logger = logger;
            this._environment = environment;
            this._messagePublisher = messagePublisher;
        }


        /// <summary>
        ///  Обработчик уровня роли мастер сервера, его задача понять что сенсор удаленный, и соотв. сгенерировать джругое событйи "OnInitExternalOnlineMeasurement"
        /// </summary>
        /// <param name="data"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public InitOnlineMeasurementPipebox Handle(InitOnlineMeasurementPipebox data, IPipelineContext<InitOnlineMeasurementPipebox, InitOnlineMeasurementPipebox> context)
        {
            using (this._logger.StartTrace(Contexts.ThisComponent, Categories.SendEventOnlineMeasurementPipelineHandler, this))
            {
                var result = new InitOnlineMeasurementPipebox();
                try
                {
                    if (data == null)
                    {
                        throw new ArgumentNullException(nameof(data));
                    }

                    var initEvent = new OnInitExternalOnlineMeasurement(this.GetType().FullName);

                    using (var dbScope = this._dataLayer.CreateScope<SdrnServerDataContext>())
                    {
                        var linkAggregationSensor = _dataLayer.GetBuilder<DM.ILinkAggregationSensor>()
                           .From()
                           .Select(c => c.Id)
                           .Select(c => c.AggregationServerName)
                           .Select(c => c.SENSOR.Id)
                           .Select(c => c.SENSOR.Name)
                           .Select(c => c.SENSOR.TechId)
                           .Where(c => c.SENSOR.Id, ConditionOperator.Equal, data.SensorId);

                        var sensorExists = dbScope.Executor.ExecuteAndFetch(linkAggregationSensor, reader =>
                        {
                            var exists = reader.Read();
                            if (exists)
                            {
                                initEvent.SensorName = reader.GetValue(c => c.SENSOR.Name);
                                initEvent.SensorTechId = reader.GetValue(c => c.SENSOR.TechId);
                                initEvent.AggregationServerInstance = reader.GetValue(c => c.AggregationServerName);
                            }
                            return exists;
                        });
                    }
                    initEvent.OnlineMeasId = data.OnlineMeasLocalId;
                    initEvent.ServerToken = data.ServerToken;
                    initEvent.Period = data.Period;

                    this._eventEmitter.Emit(initEvent);

                }
                catch (Exception e)
                {
                    _logger.Exception(Contexts.ThisComponent, (EventCategory)"OnInitOnlineMeasurement", e, this);
                    throw;
                }
                return data;
            }
        }
    }
}