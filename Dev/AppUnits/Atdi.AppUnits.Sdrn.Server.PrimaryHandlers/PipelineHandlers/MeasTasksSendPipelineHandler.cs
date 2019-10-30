using Atdi.DataModels.Sdrns.Server;
using Atdi.Platform.Workflows;
using Atdi.Contracts.Api.EventSystem;
using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Contracts.CoreServices.EntityOrm;
using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using Atdi.Contracts.Sdrn.Server;
using Atdi.DataModels.Api.EventSystem;
using Atdi.DataModels.Sdrns.Server.Events;
using System.Linq;


namespace Atdi.AppUnits.Sdrn.Server.PrimaryHandlers.PipelineHandlers
{
    public class MeasTasksSendPipelineHandler : IPipelineHandler<ClientMeasTaskPipebox, ClientMeasTaskPiperesult>
    {
        private readonly IDataLayer<EntityDataOrm> _dataLayer;
        private readonly IEventEmitter _eventEmitter;
        private readonly ISdrnServerEnvironment _environment;
        private readonly ISdrnMessagePublisher _messagePublisher;
        private readonly ILogger _logger;

        public MeasTasksSendPipelineHandler(IEventEmitter eventEmitter, IDataLayer<EntityDataOrm> dataLayer, ISdrnServerEnvironment environment, ISdrnMessagePublisher messagePublisher, ILogger logger)
        {
            this._dataLayer = dataLayer;
            this._eventEmitter = eventEmitter;
            this._logger = logger;
            this._environment = environment;
            this._messagePublisher = messagePublisher;
        }

        /// <summary>
        /// Метод, выполняющий проверку наличия в БД сведений о серверах AggregationServer, связанных с сенсорами:
        /// если нет сведений о AggregationServer, то делается вывод о том, что нужно просто отправить уведомления в обработчик OnReceivedNewMeasTaskEvent
        /// и последующей передачи сформированных подтасков для DeviceBus
        /// </summary>
        /// <param name="data"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public ClientMeasTaskPiperesult Handle(ClientMeasTaskPipebox data, IPipelineContext<ClientMeasTaskPipebox, ClientMeasTaskPiperesult> context)
        {
            using (this._logger.StartTrace(Contexts.ThisComponent, Categories.MeasTasksSendPipelineHandler, this))
            {
                var prepareSendEvents = data.PrepareSendEvents;
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
                return new ClientMeasTaskPiperesult()
                {
                    MeasTaskIdPipeResult = data.MeasTaskPipeBox.Id.Value
                };
            }
        }
    }
}
