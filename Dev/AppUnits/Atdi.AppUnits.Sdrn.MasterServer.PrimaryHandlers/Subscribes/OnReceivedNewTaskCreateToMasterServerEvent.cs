using Atdi.Contracts.Api.EventSystem;
using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Contracts.Sdrn.Server;
using Atdi.DataModels.Api.EventSystem;
using Atdi.Modules.Sdrn.Server.Events;


namespace Atdi.AppUnits.Sdrn.MasterServer.PrimaryHandlers.Subscribes
{
    [SubscriptionEvent(EventName = "OnNewMeasTaskCreateToMasterServerEvent", SubscriberName = "SubscriberOnNewMeasTaskCreateToMasterServerEvent")]
    public class OnReceivedNewTaskCreateToMasterServerEvent : IEventSubscriber<OnMeasTaskToMasterServerEvent>
    {
        private readonly ILogger _logger;
        private readonly ISdrnServerEnvironment _environment;
        private readonly ISdrnMessagePublisher _messagePublisher;

        public OnReceivedNewTaskCreateToMasterServerEvent(ISdrnMessagePublisher messagePublisher, ILogger logger, ISdrnServerEnvironment environment)
        {
            this._logger = logger;
            this._environment = environment;
            this._messagePublisher = messagePublisher;
        }

        public void Notify(OnMeasTaskToMasterServerEvent @event)
        {
            using (this._logger.StartTrace(Contexts.ThisComponent, Categories.OnReceivedNewTaskCreateToMasterServerEvent, this))
            {
                
            }
        }
    }
}
