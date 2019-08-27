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
using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Contracts.CoreServices.EntityOrm;
using Atdi.AppUnits.Sdrn.MasterServer.PrimaryHandlers;

namespace Atdi.AppUnits.Sdrn.MasterServer.PrimaryHandlers.MessageHandlers
{
    public sealed class RegistrationSensorOnMasterServerHandler : IMessageHandler<SendSensorFromAggregationToMasterServer, Sensor>
    {
        private readonly IPublisher publisher;
        private readonly ILogger _logger;
        private readonly IPipelineSite _pipelineSite;
        private readonly IDataLayer<EntityDataOrm> _dataLayer;


        public RegistrationSensorOnMasterServerHandler(IPublisher publisher, IPipelineSite pipelineSite, IDataLayer<EntityDataOrm> dataLayer, ILogger logger)
        {
            this.publisher = publisher;
            this._logger = logger;
            this._pipelineSite = pipelineSite;
            this._dataLayer = dataLayer;
        }

        /// <summary>
        /// Получение сообщения со стороны AggregationServer о необходимости запуска процедуры регистрации/обновления сенсора в БД MasterServer
        /// </summary>
        /// <param name="envelope"></param>
        /// <param name="result"></param>
        public void Handle(IIncomingEnvelope<SendSensorFromAggregationToMasterServer, Sensor> envelope, IHandlingResult result)
        {
            using (this._logger.StartTrace(Contexts.ThisComponent, Categories.RegistrationSensorOnMasterServerHandler, this))
            {
                var deliveryObject = envelope.DeliveryObject;
                var registerSensor = new RegisterSensor(this._dataLayer, this._logger);
                registerSensor.Handle(envelope.DeliveryObject);
                result.Status = MessageHandlingStatus.Confirmed;
            }
        }
    }

  
}
