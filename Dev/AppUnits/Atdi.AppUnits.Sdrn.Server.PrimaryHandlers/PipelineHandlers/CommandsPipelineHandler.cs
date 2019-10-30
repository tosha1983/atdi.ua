using Atdi.DataModels.Sdrns.Server;
using Atdi.Platform.Workflows;
using Atdi.Contracts.Api.EventSystem;
using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Contracts.CoreServices.EntityOrm;
using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using Atdi.Contracts.Sdrn.Server;


namespace Atdi.AppUnits.Sdrn.Server.PrimaryHandlers.PipelineHandlers
{
    public class CommandsPipelineHandler : IPipelineHandler<ClientMeasTaskPipebox, ClientMeasTaskPiperesult>
    {
        private readonly IDataLayer<EntityDataOrm> _dataLayer;
        private readonly IEventEmitter _eventEmitter;
        private readonly ISdrnServerEnvironment _environment;
        private readonly ISdrnMessagePublisher _messagePublisher;
        private readonly ILogger _logger;
        private readonly IPipelineSite _pipelineSite;

        public CommandsPipelineHandler(IPipelineSite pipelineSite, IEventEmitter eventEmitter, IDataLayer<EntityDataOrm> dataLayer, ISdrnServerEnvironment environment, ISdrnMessagePublisher messagePublisher, ILogger logger)
        {
            this._dataLayer = dataLayer;
            this._eventEmitter = eventEmitter;
            this._logger = logger;
            this._environment = environment;
            this._messagePublisher = messagePublisher;
            this._pipelineSite = pipelineSite;
        }



        /// <summary>
        /// Метод выполняющий обработку команд таска на основе параметров MeasTaskPipeBox и MeasTaskModePipeBox
        /// который передается с WCF - сервиса
        /// </summary>
        /// <param name="data"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public ClientMeasTaskPiperesult Handle(ClientMeasTaskPipebox data, IPipelineContext<ClientMeasTaskPipebox, ClientMeasTaskPiperesult> context)
        {
            using (this._logger.StartTrace(Contexts.ThisComponent, Categories.CommandsPipelineHandler, this))
            {
                var measTaskProcess = new MeasTaskProcess(this._eventEmitter, this._dataLayer, this._environment, this._messagePublisher, this._logger);
                var measTaskIdentifier = new MeasTaskIdentifier();
                measTaskIdentifier = data.MeasTaskPipeBox.Id;
                PrepareSendEvent[] prepareSendEvents = null;
                CommonOperation commonOperationResult = null;
                if (data.MeasTaskPipeBox.Id != null)
                {
                    switch (data.MeasTaskModePipeBox)
                    {
                        case MeasTaskMode.Run:
                            commonOperationResult = measTaskProcess.RunMeasTask(ref data.MeasTaskPipeBox, out prepareSendEvents);
                            break;
                        case MeasTaskMode.Del:
                            commonOperationResult = measTaskProcess.DeleteMeasTask(ref data.MeasTaskPipeBox, out prepareSendEvents);
                            break;
                        case MeasTaskMode.Stop:
                            commonOperationResult = measTaskProcess.StopMeasTask(ref data.MeasTaskPipeBox, out prepareSendEvents);
                            break;
                        default:
                            throw new NotImplementedException($"Value type not supported '{data.MeasTaskModePipeBox}')");
                    }
                    if (commonOperationResult != null)
                    {
                        data.PrepareSendEvents = prepareSendEvents;
                        data.CommonOperationPipeBox = commonOperationResult;
                    }
                }
                // передача в обработчик ClientCommandsSendEventPipelineHandler 
                var res = context.GoAhead(data);
                if (res == null)
                {
                    var site = this._pipelineSite.GetByName<ClientMeasTaskPipebox, ClientMeasTaskPiperesult>(Pipelines.ClientCommandsSendEvents);
                    var resultSendEvent = site.Execute(new ClientMeasTaskPipebox()
                    {
                        MeasTaskPipeBox = data.MeasTaskPipeBox,
                        PrepareSendEvents = data.PrepareSendEvents
                    });
                    resultSendEvent.CommonOperationPipeBoxResult = commonOperationResult;
                    return resultSendEvent;
                }
                else
                {
                    return res;
                }
            }
        }
    }
}
