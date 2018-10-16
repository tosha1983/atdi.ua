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
    }
}
