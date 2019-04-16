using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Api.DataBus
{
    internal sealed class HandlersHost
    {
        private readonly object _locker = new object();

        private readonly ConcurrentDictionary<string, List<HandlerDescriptor>> _handlers;

        public HandlersHost()
        {
            this._handlers = new ConcurrentDictionary<string, List<HandlerDescriptor>>();
        }

        public HandlerDescriptor RegistryHandler(Type handlerType)
        {
            var descriptor = new HandlerDescriptor(handlerType);

            if (!this._handlers.TryGetValue(descriptor.Key, out List<HandlerDescriptor> descriptorList))
            {
                descriptorList = new List<HandlerDescriptor>();
                if (!this._handlers.TryAdd(descriptor.Key, descriptorList))
                {
                    descriptorList = null;
                    this._handlers.TryGetValue(descriptor.Key, out descriptorList);
                }
            }
            lock(this._locker)
            {
                descriptorList.Add(descriptor);
            }
            return descriptor;
        }

        public List<HandlerDescriptor> GetHandlers(Type messageType, Type deliveryObjectType)
        {
            var key = HandlerDescriptor.BuildKey(messageType, deliveryObjectType);
            if (_handlers.TryGetValue(key, out List<HandlerDescriptor> result))
            {
                return result;
            }

            return new List<HandlerDescriptor>();
        }
    }
}
