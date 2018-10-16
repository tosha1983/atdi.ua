using Atdi.Contracts.Api.EventSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Api.EventSystem
{
    public class EventSiteConfig : IEventSiteConfig
    {
        private readonly Dictionary<string, object> _data;

        public EventSiteConfig()
        {
            this._data = new Dictionary<string, object>();
        }

        public object this[string name] { get => this._data[name]; set => this._data[name] = value; }

        public T GetValue<T>(string name, T defaultValue = default(T))
        {
            return (T)this._data[name];
        }

        public void SetValue<T>(string name, T value)
        {
            this._data[name] = value;
        }

        public bool TryGetValue<T>(string name, out T value)
        {
            value = default(T);

            if (this._data.ContainsKey(name))
            {
                value = this.GetValue(name, value);
                return true;
            }
            return false;
        }

        public static readonly string ApiVersion = "ApiVersion";
        public static readonly string AppName = "AppName";
        public static readonly string EventBusHost = "EventBus.Host";
        public static readonly string EventBusVirtualHost = "EventBus.VirtualHost";
        public static readonly string EventBusPort = "EventBus.Port";
        public static readonly string EventBusUser = "EventBus.User";
        public static readonly string EventBusPassword = "EventBus.Password";
        public static readonly string EventExchange = "EventExchange";
        public static readonly string EventQueueNamePart = "EventQueueNamePart";
        public static readonly string ErrorsQueueName = "ErrorsQueueName";
        public static readonly string LogQueueName = "LogQueueName";
        public static readonly string UseEncryption = "UseEncryption";
        public static readonly string UseCompression = "UseCompression";

        

    }
}
