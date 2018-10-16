using Atdi.Contracts.Sdrn.Server;
using Atdi.DataModels.Sdrns;
using Atdi.Platform.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.AppUnits.Sdrn.BusController
{
    public class SdrnHandlerLibrary
    {
        public static readonly string HandlerInterfaceName = typeof(ISdrnMessageHandler<,>).Name;
        public static readonly string HandlerInterfaceNamespace = typeof(ISdrnMessageHandler<,>).Namespace;
        public static readonly string HandlerInterfaceFullName = typeof(ISdrnMessageHandler<,>).FullName;

        private class MessageTypeDeliveryObjects
        {
            private readonly Dictionary<string, DeliveryObjectHandlers> _deliveryObjects;

            public MessageTypeDeliveryObjects(SdrnBusMessageType messageType)
            {
                this.MessageType = messageType;
                this._deliveryObjects = new Dictionary<string, DeliveryObjectHandlers>();
            }

            public SdrnBusMessageType MessageType { get; }

            public void Put(Type deliveryObjectType, Type handler)
            {
                if (!this._deliveryObjects.TryGetValue(deliveryObjectType.FullName, out DeliveryObjectHandlers deliveryObjectHandlers))
                {
                    deliveryObjectHandlers = new DeliveryObjectHandlers(deliveryObjectType);
                    _deliveryObjects[deliveryObjectType.FullName] = deliveryObjectHandlers;
                }
                deliveryObjectHandlers.Put(handler);
                
            }

            public Type[] GetHandlers(Type deliveryObjectType)
            {
                if (this._deliveryObjects.TryGetValue(deliveryObjectType.FullName, out DeliveryObjectHandlers deliveryObjectHandlers))
                {
                    return deliveryObjectHandlers.GetHandlers();
                }
                return new Type[] { };
            }
        }

        private class DeliveryObjectHandlers
        {
            private readonly Dictionary<string, Type> _handlers;

            public DeliveryObjectHandlers(Type deliveryObjectType)
            {
                this.DeliveryObjectType = deliveryObjectType;
                this._handlers = new Dictionary<string, Type>();
            }

            public Type DeliveryObjectType { get; }

            public void Put(Type handler)
            {
                if (this._handlers.ContainsKey(handler.FullName))
                {
                    return;
                }
                _handlers[handler.FullName] = handler;
            }

            public Type[] GetHandlers()
            {
                return _handlers.Values.ToArray();
            }
        }

        private readonly IServicesResolver _servicesResolver;
        private readonly Dictionary<string, MessageTypeDeliveryObjects> _messageTypes;

        public SdrnHandlerLibrary(IServicesResolver servicesResolver)
        {
            this._servicesResolver = servicesResolver;
            this._messageTypes = new Dictionary<string, MessageTypeDeliveryObjects>();
        }

        public Type[] GetHandlerTypes(string messageType, Type deliveryObjectType)
        {
            if (_messageTypes.TryGetValue(messageType, out MessageTypeDeliveryObjects deliveryObjects))
            {
                return deliveryObjects.GetHandlers(deliveryObjectType);
            }
            return new Type[] { };
        }

        public object ResolveHandler(Type handlerType)
        {
            return this._servicesResolver.Resolve(handlerType);
        }

        public void RegistryHandler(Type handlerType)
        {
            var handlerInterface = handlerType.GetInterface(SdrnHandlerLibrary.HandlerInterfaceFullName);
            if (handlerInterface == null)
            {
                throw new InvalidOperationException($"Not detected implementation of the interface '{SdrnHandlerLibrary.HandlerInterfaceFullName}' in the hadnler type '{handlerType.AssemblyQualifiedName}'");
            }
            var messageTypeType = handlerInterface.GenericTypeArguments[0];
            var deliveryObjectType = handlerInterface.GenericTypeArguments[1];
            var messageType = Activator.CreateInstance(messageTypeType) as SdrnBusMessageType;

            if (!_messageTypes.TryGetValue(messageType.Name, out MessageTypeDeliveryObjects messageTypeDeliveryObjects ))
            {
                messageTypeDeliveryObjects = new MessageTypeDeliveryObjects(messageType);
                _messageTypes[messageType.Name] = messageTypeDeliveryObjects;
            }
            messageTypeDeliveryObjects.Put(deliveryObjectType, handlerType);
        }

    }


}
