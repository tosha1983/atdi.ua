using Atdi.Contracts.Api.EventSystem;
using Atdi.DataModels.Api.EventSystem;
using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrns.Server.Events;
using Atdi.Contracts.Sdrn.Server;
using MSG = Atdi.DataModels.Sdrns.BusMessages;
using DEV = Atdi.DataModels.Sdrns.Device;


namespace Atdi.AppUnits.Sdrn.Server.PrimaryHandlers.Subscribes
{
    [SubscriptionEvent(EventName = "OnRunMeasTaskEvent", SubscriberName = "SubscriberOnRunMeasTaskEvent")]
    public class OnReceivedRunMeasTaskEvent : IEventSubscriber<OnMeasTaskEvent>
    {
        private readonly ILogger _logger;
        private readonly ISdrnServerEnvironment _environment;
        private readonly ISdrnMessagePublisher _messagePublisher;

        public OnReceivedRunMeasTaskEvent(ISdrnMessagePublisher messagePublisher, ILogger logger, ISdrnServerEnvironment environment)
        {
            this._logger = logger;
            this._environment = environment;
            this._messagePublisher = messagePublisher;
        }

        /// <summary>
        /// Отправка команды RunMeasTask в шину DeviceBus
        /// </summary>
        /// <param name="event"></param>
        public void Notify(OnMeasTaskEvent @event)
        {
            using (this._logger.StartTrace(Contexts.PrimaryHandler, Categories.Notify, this))
            {
                if (!string.IsNullOrEmpty(@event.MeasTaskIds) && (@event.SensorName != null) && (@event.EquipmentTechId != null))
                {
                    var envelop = _messagePublisher.CreateOutgoingEnvelope<MSG.Server.SendCommandMessage, DEV.DeviceCommand>();
                    envelop.SensorName = @event.SensorName;
                    envelop.SensorTechId = @event.EquipmentTechId;
                    envelop.DeliveryObject = new DEV.DeviceCommand();
                    envelop.DeliveryObject.CommandId = "SendCommand";
                    envelop.DeliveryObject.Command = "RunMeasTask";
                    envelop.DeliveryObject.SensorName = @event.SensorName;
                    envelop.DeliveryObject.SdrnServer = this._environment.ServerInstance;
                    envelop.DeliveryObject.EquipmentTechId = @event.EquipmentTechId;
                    envelop.DeliveryObject.CustTxt1 = @event.MeasTaskIds;
                    _messagePublisher.Send(envelop);
                }
            }
        }
    }
}
