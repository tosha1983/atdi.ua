using Atdi.Contracts.Api.EventSystem;
using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Contracts.Sdrn.Server;
using Atdi.DataModels.Api.EventSystem;
using Atdi.DataModels.Sdrns.Server.Events;
using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Contracts.CoreServices.EntityOrm;
using Atdi.DataModels.Sdrns.Server;
using Atdi.DataModels.DataConstraint;
using MD = Atdi.DataModels.Sdrns.Server.Entities;


namespace Atdi.AppUnits.Sdrn.MasterServer.PrimaryHandlers.Subscribes
{
    [SubscriptionEvent(EventName = "OnUpdateMeasTaskCreateToMasterServerEvent", SubscriberName = "SubscriberOnUpdateMeasTaskCreateToMasterServerEvent")]
    public class OnReceivedUpdateTaskCreateToMasterServerEvent : IEventSubscriber<OnMeasTaskToMasterServerEvent>
    {
        private readonly ILogger _logger;
        private readonly ISdrnServerEnvironment _environment;
        private readonly ISdrnMessagePublisher _messagePublisher;

        public OnReceivedUpdateTaskCreateToMasterServerEvent(ISdrnMessagePublisher messagePublisher, ILogger logger, ISdrnServerEnvironment environment)
        {
            this._logger = logger;
            this._environment = environment;
            this._messagePublisher = messagePublisher;
        }


        public void Notify(OnMeasTaskToMasterServerEvent @event)
        {

            // Механизм обновления таска и перерасчета результатов
        }
    }
}
