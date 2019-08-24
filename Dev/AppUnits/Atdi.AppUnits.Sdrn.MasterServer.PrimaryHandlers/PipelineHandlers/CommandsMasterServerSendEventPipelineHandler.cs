using System.Linq;
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
using Atdi.Contracts.Sdrn.Server;
using Atdi.Contracts.Api.DataBus;
using Atdi.DataModels.Api.DataBus;
using Atdi.DataModels.Sdrns.Server.Test;


namespace Atdi.AppUnits.Sdrn.MasterServer.PrimaryHandlers.PipelineHandlers
{
    public class CommandsMasterServerSendEventPipelineHandler : IPipelineHandler<ClientMeasTaskPipebox, ClientMeasTaskPiperesult>
    {
        private readonly IDataLayer<EntityDataOrm> _dataLayer;
        private readonly IPublisher _publisher;
        private readonly ILogger _logger;
        private readonly IEventEmitter _eventEmitter;


        public CommandsMasterServerSendEventPipelineHandler(IDataLayer<EntityDataOrm> dataLayer, IEventEmitter eventEmitter, ISdrnServerEnvironment environment, IPublisher publisher, ILogger logger)
        {
            this._dataLayer = dataLayer;
            this._logger = logger;
            this._publisher = publisher;
            this._eventEmitter = eventEmitter;
        }


        /// <summary>
        /// Конвеерный обработчик комманд, который выполняет "группировку" комманд по списку AggregationServer'ов
        /// и отправляет в сторону AggregationServer таски с учетом такой разбивки
        /// </summary>
        /// <param name="data"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public ClientMeasTaskPiperesult Handle(ClientMeasTaskPipebox data, IPipelineContext<ClientMeasTaskPipebox, ClientMeasTaskPiperesult> context)
        {
            using (this._logger.StartTrace(Contexts.ThisComponent, Categories.CommandsMasterServerSendEventPipelineHandler, this))
            {
                var generateMeasTasksPipelineHandler = new GenerateMeasTasksPipelineHandler(this._eventEmitter, this._dataLayer, this._logger);
                generateMeasTasksPipelineHandler.Handle(ref data);
                if ((data.MeasTasksWithAggregationServerPipeBox != null) && (data.MeasTasksWithAggregationServerPipeBox.Length > 0))
                {
                    for (int i = 0; i < data.MeasTasksWithAggregationServerPipeBox.Length; i++)
                    {
                        var retEnvelope = this._publisher.CreateEnvelope<MeasTaskToAggregationServer, CommandMeasTaskPipebox>();
                        retEnvelope.To = data.AggregationServerInstancesPipeBox[i];
                        retEnvelope.DeliveryObject = new CommandMeasTaskPipebox() { MeasTaskPipeBox = data.MeasTasksWithAggregationServerPipeBox[i], MeasTaskModePipeBox = data.MeasTaskModePipeBox };
                        this._publisher.Send(retEnvelope);
                    }
                }
                return new ClientMeasTaskPiperesult()
                {
                    MeasTaskIdPipeResult = data.MeasTaskPipeBox.Id.Value,
                    CommonOperationPipeBoxResult = data.CommonOperationPipeBox
                };
            }
        }
    }
}
