using Atdi.Contracts.Api.EventSystem;
using Atdi.Contracts.Sdrn.Server.DevicesBus;
using Atdi.DataModels.Sdrns.Server.Events;
using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.AppUnits.Sdrn.Server.EventSubscribers.DeviceBus
{
    public abstract class SubscriberBase<TDeliveryObject> : IEventSubscriber<DevicesBusEvent>
    {
        private readonly IMessagesSite _messagesSite;
        protected readonly ILogger _logger;

        public SubscriberBase(IMessagesSite messagesSite, ILogger logger)
        {
            this._messagesSite = messagesSite;
            this._logger = logger;
        }

        public void Notify(DevicesBusEvent @event)
        {
            try
            {
                using (var scope = _messagesSite.StartProcessing<TDeliveryObject>(@event.BusMessageId))
                {
                    try
                    {
                        scope.Status = MessageProcessingStatus.Processing;
                        this.Handle(scope.SensorName, scope.SensorTechId, scope.Delivery);
                        scope.Status = MessageProcessingStatus.Processed;
                    }
                    catch (Exception e)
                    {
                        scope.Status = MessageProcessingStatus.Failure;
                        if (string.IsNullOrEmpty(scope.ResultNote))
                        {
                            scope.ResultNote = e.Message;
                        }
                        else
                        {
                            scope.ResultNote += Environment.NewLine + "Exception: " + e.Message;
                        }
                        
                        throw;
                    }
                }
            }
            catch (Exception e)
            {

                _logger.Exception(Contexts.ThisComponent, Categories.EventProcessing, e, (object)this);
            }
        }
        protected abstract void Handle(string sensorName, string sensorTechId, TDeliveryObject deliveryObject);
    }
}
