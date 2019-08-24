using Atdi.Contracts.Api.DataBus;
using Atdi.DataModels.Api.DataBus;
using Atdi.DataModels.Sdrns.Server;
using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Platform.Workflows;
using SdrnsServer = Atdi.DataModels.Sdrns.Server;


namespace Atdi.AppUnits.Sdrn.AggregationServer.PrimaryHandlers.MessageHandlers
{
    public sealed class MeasTaskCreateOnAggServerHandler : IMessageHandler<MeasTaskToAggregationServer, CreateMeasTaskPipebox>
    {
        private readonly IPublisher publisher;
        private readonly ILogger _logger;
        private readonly IPipelineSite _pipelineSite;

        public MeasTaskCreateOnAggServerHandler(IPublisher publisher, IPipelineSite pipelineSite, ILogger logger)
        {
            this.publisher = publisher;
            this._logger = logger;
            this._pipelineSite = pipelineSite;
        }

        /// <summary>
        ///  Класс, выполняющий "прослушивание" и обработку сообщений  типа MeasTaskToAggregationServer,
        ///  которые отправляются со стороны MasterServer и несут информацию о таске, предназначенном для сохранения на данном AggregationServer (создание новой задачи)
        ///  Запуск конвеерной обработки полученных данных
        /// </summary>
        /// <param name="envelope"></param>
        /// <param name="result"></param>
        public void Handle(IIncomingEnvelope<MeasTaskToAggregationServer, CreateMeasTaskPipebox> envelope, IHandlingResult result)
        {
            using (this._logger.StartTrace(Contexts.ThisComponent, Categories.MeasTaskCreateOnAggServerHandler, this))
            {
                var deliveryObject = envelope.DeliveryObject;
                var site = this._pipelineSite.GetByName<SdrnsServer.ClientMeasTaskPipebox, SdrnsServer.ClientMeasTaskPiperesult>(SdrnsServer.Pipelines.ClientMeasTasks);
                var resultCreateMeasTask = site.Execute(new SdrnsServer.ClientMeasTaskPipebox()
                {
                    MeasTaskPipeBox = deliveryObject.MeasTaskPipeBox,
                    MeasTaskModePipeBox = deliveryObject.MeasTaskModePipeBox
                });
                result.Status = MessageHandlingStatus.Confirmed;
            }
        }
    }

  
}
