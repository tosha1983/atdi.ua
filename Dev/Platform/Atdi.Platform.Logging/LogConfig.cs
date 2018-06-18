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
        private EventLevel _levels;

        public LogConfig(IConfigParameters parameters)
        {
            this._levels = EventLevel.None;
            if (parameters.Has("Levels"))
            {
                var levelsParam = (string)parameters["Levels"];
                if (!string.IsNullOrEmpty(levelsParam))
                {
                    var levelStrings = levelsParam.Split(new string[] { ", ", "; ", ",", ";" }, StringSplitOptions.RemoveEmptyEntries);
                    if (levelStrings.Length > 0)
                    {
                        levelStrings.Select(s => (EventLevel)Enum.Parse(typeof(EventLevel), s, true))
                            .ToList()
                            .ForEach(l => 
                            {
                                this._levels = this._levels | l;
                            });
                    }
                }
            }

            this._data = new Dictionary<string, object>
            {
                [LogConfig.EventsCapacityConfigKey] = LogConfig.DefaultEventsCapacity
            };

            if (parameters.Has(LogConfig.EventsCapacityConfigKey))
            {
                this[LogConfig.EventsCapacityConfigKey] = parameters[LogConfig.EventsCapacityConfigKey];
            }
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
            if (this._levels == EventLevel.None)
            {
                return true;
            }
            return (this._levels & level) == level;
        }
    }
}
