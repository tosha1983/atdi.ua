using Atdi.Contracts.Api.EventSystem;
using Atdi.DataModels.Api.EventSystem;
using Atdi.Modules.AmqpBroker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Api.EventSystem
{
    public class EventEmitter : IEventEmitter
    {
        private readonly EventSite _eventSite;
        private readonly IEventSystemObserver _observer;
        private Channel _channel;
        private readonly EventSystemConfig _sysConfig;

        public EventEmitter(IEventSite eventSite, IEventSystemObserver observer)
        {
            this._eventSite = eventSite as EventSite;
            this._sysConfig = this._eventSite.SysConfig;
            this._channel = this._eventSite.EmitterConnection.CreateChannel();
            this._observer = observer;
        }

        public void Dispose()
        {
            if (this._channel != null)
            {
                try
                {
                    _channel.Dispose();
                }
                catch (Exception e)
                {
                    _observer.Exception(EventSystemEvents.EventEmitterDisposeExeption, "EventSystem.Dispose", e, this);
                }
                _channel = null;
            }
        }

        public void Emit(IEvent @event, EventEmittingOptions options)
        {
            try
            {
                var message = _channel.CreateMessage();
                message.Id = Guid.NewGuid().ToString();
                message.AppId = $"EventSystem.[{_sysConfig.AppName}]";
                message.CorrelationId = $"EventId: {@event.Id.ToString()}" ;
                message.ContentType = "application/json";
                message.Type = "EventSystem.Emitter.Emit";
                message.Headers = new Dictionary<string, object>
                {
                    ["ApiVersion"] = _sysConfig.ApiVersion,
                    ["Created"] = DateTime.Now.ToString("o"),

                    ["Event.Id"] = @event.Id.ToString(),
                    ["Event.Name"] = @event.Name,
                    ["Event.Source"] = @event.Source,
                    ["Event.Created"] = @event.Created.ToString("o"),
                    
                    ["Options.Rule"] = options.Rule.ToString(),
                    ["Options.Destination"] = options.Destination
                };
                _channel.PackDeliveryObject(message, @event, _sysConfig.UseCompression, _sysConfig.UseEncryption);

                var routingKey = $"[@{@event.Name}]";
                var exchange = this._sysConfig.BuildEventExchangeName();

                _channel.DeclareBinding(_sysConfig.BuildCommonLogQueueName(), exchange, routingKey);
                _channel.DeclareDurableQueue(_sysConfig.BuildEventLogQueueName(@event.Name), exchange, routingKey);
                _channel.Publish(exchange, routingKey, message);
            }
            catch(Exception e)
            {
                _observer.Exception(EventSystemEvents.EmitEvent, "EventSystem.EmitEvent", e, this);
            }
        }

       
    }
}
