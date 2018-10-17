using Atdi.Contracts.Api.EventSystem;
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
        private readonly string _appNameConfigParam;
        private readonly string _eventExchangeConfigParam;
        private readonly string _eventQueueNamePartConfigParam;
        private readonly bool _useCompressionConfigParam;
        private readonly bool _useEncryptionConfigParam;

        public EventEmitter(IEventSite eventSite, IEventSystemObserver observer)
        {
            this._eventSite = eventSite as EventSite;
            this._channel = this._eventSite.EmitterConnection.CreateChannel();
            this._observer = observer;
            this._appNameConfigParam = _eventSite.Config.GetValue<string>(EventSiteConfig.AppName);
            this._useCompressionConfigParam = _eventSite.Config.GetValue<bool>(EventSiteConfig.UseCompression);
            this._useEncryptionConfigParam = _eventSite.Config.GetValue<bool>(EventSiteConfig.UseEncryption);
            this._eventExchangeConfigParam = _eventSite.Config.GetValue<string>(EventSiteConfig.EventExchange);
            this._eventQueueNamePartConfigParam = _eventSite.Config.GetValue<string>(EventSiteConfig.EventQueueNamePart);
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
                message.AppId = $"EventSystem.[{_appNameConfigParam}]";
                message.CorrelationId = @event.Id.ToString();
                message.ContentType = "application/json";
                message.Type = "EventSystem.Emitter.Emit";

                _channel.PackDeliveryObject(message, @event, _useCompressionConfigParam, _useEncryptionConfigParam);

                _channel.DeclareBinding(_eventSite.CommonLogQueueName, _eventExchangeConfigParam, @event.Name);
                _channel.DeclareDurableQueue($"{this._eventQueueNamePartConfigParam}.[{@event.Name}].[log]", _eventExchangeConfigParam, @event.Name);
                _channel.Publish(_eventExchangeConfigParam, @event.Name, message);
            }
            catch(Exception e)
            {
                _observer.Exception(EventSystemEvents.EmitEvent, "EventSystem.EmitEvent", e, this);
            }
        }

       
    }
}
