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
using Atdi.Contracts.WcfServices.Sdrn.Server;
using Atdi.DataModels.Sdrns.Server.Events.OnlineMeasurement;



namespace Atdi.AppUnits.Sdrn.Server.PrimaryHandlers.PipelineHandlers
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


        public InitOnlineMeasurementPipebox Handle(InitOnlineMeasurementPipebox data, IPipelineContext<InitOnlineMeasurementPipebox, InitOnlineMeasurementPipebox> context)
        {
            using (this._logger.StartTrace(Contexts.ThisComponent, Categories.OnInitOnlineMeasurement, this))
            {
                var result = new InitOnlineMeasurementPipebox();
                try
                {
                    if (data == null)
                    {
                        throw new ArgumentNullException(nameof(data));
                    }


                    var isFindLinkAggregationServer = false;
                    var loadSensor = new LoadSensor(this._dataLayer, this._logger);
                    var allSensors = loadSensor.LoadAllSensorIds();
                    if ((allSensors != null) && (allSensors.Length > 0))
                    {
                        for (int i = 0; i < allSensors.Length; i++)
                        {
                            isFindLinkAggregationServer = loadSensor.GetAggregationServerBySensorId(allSensors[i], out string AggrServer);
                            if (isFindLinkAggregationServer == true)
                            {
                                break;
                            }
                        }
                    }
                    // если сенсор не принадлежит к аггрегейшн серверам, тогда просто отправка уведомления OnInitOnlineMeasurement
                    if (isFindLinkAggregationServer == false)
                    {
                        var initEvent = new OnInitOnlineMeasurement(this.GetType().FullName)
                        {
                            OnlineMeasId = data.OnlineMeasLocalId
                        };
                        this._eventEmitter.Emit(initEvent);

                        return data;
                    }
                }
                catch (Exception e)
                {
                    _logger.Exception(Contexts.ThisComponent, (EventCategory)"OnInitOnlineMeasurement", e, this);
                    throw;
                }
                return context.GoAhead(data);
            }
        }
    }
}
