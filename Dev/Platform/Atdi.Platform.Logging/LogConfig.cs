using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Platform.Logging
{
    public sealed class LogConfig : ILogConfig
    {
        public static readonly int DefaultEventsCapacity = 100000;
        public static readonly string EventsCapacityConfigKey = "EventsCapacity";

        private readonly Dictionary<string, object> _data;

        public LogConfig()
        {
            this._data = new Dictionary<string, object>
            {
                [LogConfig.EventsCapacityConfigKey] = LogConfig.DefaultEventsCapacity
            };
        }

        public object this[string key] { get => this._data[key]; set => this._data[key] = value; }

        public void Disable(EventLevel level)
        {
            throw new NotImplementedException();
        }

        public void Enable(EventLevel level)
        {
            throw new NotImplementedException();
        }

        public bool IsAllowed(EventLevel level)
        {
            return true;
        }
    }
}
