﻿using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrns.Server;
using Atdi.Platform.Workflows;
using Atdi.Contracts.Api.EventSystem;
using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Contracts.CoreServices.EntityOrm;
using Atdi.Platform.Logging;
using Atdi.Contracts.Sdrn.Server;
using Atdi.Contracts.Api.DataBus;
using Atdi.DataModels.Api.EventSystem;
using Atdi.Modules.Sdrn.Server.Events;




namespace Atdi.AppUnits.Sdrn.AggregationServer.PrimaryHandlers.Handlers
{
    public class MeasTasksOnAggServerSendEventPipelineHandler : IPipelineHandler<ClientMeasTaskPipebox, ClientMeasTaskPiperesult>
    {
        private readonly IDataLayer<EntityDataOrm> _dataLayer;
        private readonly IPublisher _publisher;
        private readonly ILogger _logger;
        private readonly IEventEmitter _eventEmitter;

        public MeasTasksOnAggServerSendEventPipelineHandler(IDataLayer<EntityDataOrm> dataLayer, IEventEmitter eventEmitter, ISdrnServerEnvironment environment, IPublisher publisher, ILogger logger)
        {
            this._dataLayer = dataLayer;
            this._logger = logger;
            this._publisher = publisher;
            this._eventEmitter = eventEmitter;
        }


        /// <summary>
        /// Отправка  сформированных подзадач в обработчикb (OnReceivedDelMeasTaskEvent,OnReceivedStopMeasTaskEvent или OnReceivedRunMeasTaskEvent в зависимости от назначения команды) 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public ClientMeasTaskPiperesult Handle(ClientMeasTaskPipebox data, IPipelineContext<ClientMeasTaskPipebox, ClientMeasTaskPiperesult> context)
        {
            using (this._logger.StartTrace(Contexts.ThisComponent, Categories.MeasTasksOnAggServerSendEventPipelineHandler, this))
            {
                var prepareSendEvents = data.PrepareSendEvents;
                if (prepareSendEvents != null)
                {
                    for (int k = 0; k < prepareSendEvents.Length; k++)
                    {
                        var measTaskEvent = new OnMeasTaskEvent()
                        {
                            SensorId = prepareSendEvents[k].SensorId,
                            MeasTaskId = prepareSendEvents[k].MeasTaskId,
                            SensorName = prepareSendEvents[k].SensorName,
                            EquipmentTechId = prepareSendEvents[k].EquipmentTechId,
                            Name = $"On{prepareSendEvents[k].ActionType}MeasTaskEvent",
                            MeasTaskIds = prepareSendEvents[k].MeasTaskIds
                        };
                        this._eventEmitter.Emit(measTaskEvent, new EventEmittingOptions()
                        {
                            Rule = EventEmittingRule.Default,
                            Destination = new string[] { $"SubscriberOn{prepareSendEvents[k].ActionType}MeasTaskEvent" }
                        });
                    }
                }
                return new ClientMeasTaskPiperesult()
                {
                    MeasTaskIdPipeResult = data.MeasTaskPipeBox.Id.Value
                };
            }
        }
    }
}