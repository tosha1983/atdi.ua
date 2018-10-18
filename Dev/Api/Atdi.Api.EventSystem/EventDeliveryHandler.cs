using Atdi.Contracts.Api.EventSystem;
using Atdi.Modules.AmqpBroker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Api.EventSystem
{
    public class EventDeliveryHandler : IDeliveryHandler, IDisposable
    {
        private static readonly Type SubscriberInterfaceType = typeof(IEventSubscriber<>);

        private readonly string _eventName;
        private readonly string _subscriberName;
        private readonly Type _subscriberType;
        private readonly Type _subscriberEventType;
        private readonly EventSite _eventSite;
        private readonly IEventSystemObserver _observer;
        private Channel _redirectChannel;
        private Channel _channel;
        private readonly EventSystemConfig _sysConfig;
        private readonly string _tag;

        public EventDeliveryHandler(string eventName, Type subscriberType, string subscriberName, EventSite eventSite, IEventSystemObserver observer)
        {
            this._eventName = eventName;
            this._subscriberName = subscriberName;
            this._subscriberType = subscriberType;
            this._eventSite = eventSite;
            this._sysConfig = this._eventSite.SysConfig;
            this._channel = this._eventSite.DispatcherConnection.CreateChannel();
            
            this._observer = observer;
            this._tag = $"[@{_eventName}].[#{_subscriberName}]";

            var subscriberInterface = _subscriberType.GetInterface(SubscriberInterfaceType.FullName);
            this._subscriberEventType = subscriberInterface.GenericTypeArguments[0];

            
        }

        public void Join()
        {
            _channel.JoinConsumer(_sysConfig.BuildEventQueueName(_eventName, _subscriberName), _tag, this);
        }

        public void Dispose()
        {
            if (this._channel != null)
            {
                try
                {
                    _channel.UnjoinConsumer(_tag);
                }
                catch (Exception e)
                {
                    _observer.Exception(EventSystemEvents.EventDeliveryHandlerDisposeExeption, "EventSystem.Dispose", e, this);
                }

                try
                {
                    _channel.Dispose();
                }
                catch (Exception e)
                {
                    _observer.Exception(EventSystemEvents.EventDeliveryHandlerDisposeExeption, "EventSystem.Dispose", e, this);
                }
                _channel = null;
            }

            if (this._redirectChannel != null)
            {
                try
                {
                    _redirectChannel.Dispose();
                }
                catch (Exception e)
                {
                    _observer.Exception(EventSystemEvents.EventDeliveryHandlerDisposeExeption, "EventSystem.Dispose", e, this);
                }
                _redirectChannel = null;
            }
        }

        private Channel RedirectChannel
        {
            get
            {
                if(_redirectChannel == null)
                {
                    _redirectChannel = _eventSite.EmitterConnection.CreateChannel();
                }
                return _redirectChannel;
            }
        }
        public HandlingResult Handle(IDeliveryMessage message, IDeliveryContext deliveryContext)
        {
            this._observer.Verbouse("EventSystem.EventDeliveryHandling", $"The event notification is received: {message.CorrelationId}, DeliveryTag='{deliveryContext.DeliveryTag}', RoutingKey='{deliveryContext.RoutingKey},' ConsumerTag='{deliveryContext.ConsumerTag}', Exchange='{deliveryContext.Exchange}'", this);
            var innerProcess = "Verify";
            try
            {
                var apiVersion = message.GetHeaderValue("ApiVersion");
                if (!_sysConfig.ApiVersion.Equals(apiVersion, StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidCastException($"Version mismatch: SystemEvent API Version '{_sysConfig.ApiVersion}'");
                }

                if (!"EventSystem.Emitter.Emit".Equals(message.Type, StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidCastException($"Message type mismatch: Expected type 'EventSystem.Emitter.Emit'");
                }

                if (!"application/json".Equals(message.ContentType, StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidCastException($"Content type mismatch: Expected content type 'application/json'");
                }

                innerProcess = "ActivateSubscriberInstance";

                var subscriberInstance = _eventSite.Activator.CreateInstance(_subscriberType);

                innerProcess = "UnpackDeliveryEventObject";

                var deliveryEventObject = _channel.UnpackDeliveryObject(message, this._subscriberEventType);

                innerProcess = "InvokeNotifyMethod";

                var notifyMethod = _subscriberType.GetMethod("Notify");
                notifyMethod.Invoke(subscriberInstance, new object[] { deliveryEventObject });


            }
            catch(Exception e)
            {
                _observer.Exception(EventSystemEvents.ExceptionEvent, "EventSystem.EventDeliveryHandling", e, this);
                message.CorrelationId = message.Id;

                message.Headers["Message.Id"] = message.Id;
                message.Headers["Message.Type"] = message.Type;
                message.Headers["Message.ContentType"] = message.ContentType;
                message.Headers["Message.ContentEncoding"] = message.ContentEncoding;
                message.Headers["Message.Created"] = message.GetHeaderValue("Created");
                message.Headers["Message.ApiVersion"] = message.GetHeaderValue("ApiVersion");
                message.Headers["Message.AppId"] = message.AppId;

                message.Id = Guid.NewGuid().ToString();
                message.Type = "EventSystem.DeliveryHandler.RedirectToError";

                message.Headers["Subscriber.Name"] = _subscriberName;
                message.Headers["Subscriber.InstanceType"] = _subscriberType.AssemblyQualifiedName;
                message.Headers["Subscriber.EventObjectType"] = _subscriberEventType.AssemblyQualifiedName;
                message.Headers["Subscriber.ExpectedEvent"] = _eventName;
                message.Headers["Subscriber.Tag"] = _tag;

                message.Headers["Excepation.Message"] = e.Message;
                message.Headers["Excepation.StackTrace"] = e.StackTrace;

                message.Headers["Code.ManagedThreadId"] = System.Threading.Thread.CurrentThread.ManagedThreadId.ToString();
                message.Headers["Code.InternalProcess"] = innerProcess;

                message.Headers["DeliveryContext.ConsumerTag"] = deliveryContext.ConsumerTag;
                message.Headers["DeliveryContext.DeliveryTag"] = deliveryContext.DeliveryTag;
                message.Headers["DeliveryContext.Exchange"] = deliveryContext.Exchange;
                message.Headers["DeliveryContext.Redelivered"] = deliveryContext.Redelivered.ToString();
                message.Headers["DeliveryContext.RoutingKey"] = deliveryContext.RoutingKey;

                try
                {
                    this.RedirectChannel.Publish(_sysConfig.BuildEventExchangeName(), deliveryContext.RoutingKey + ".[error]", message);
                }
                catch(Exception e2)
                {
                    _observer.Exception(EventSystemEvents.ExceptionEvent, "EventSystem.EventDeliveryHandling", e2, this);
                    return HandlingResult.Ignore;
                }
                
            }
            return HandlingResult.Confirm;
        }
    }
}
