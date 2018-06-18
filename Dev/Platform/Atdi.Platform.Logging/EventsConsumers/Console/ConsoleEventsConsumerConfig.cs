using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Platform.Logging.EventsConsumers
{
    public sealed class ConsoleEventsConsumerConfig
    {
        private readonly bool _hasLevels;
        private readonly bool _hasContexts;
        private readonly bool _hasCategories;

        public string[] Categories { get; private set; }

        public string[] Contexts { get; private set; }

        public EventLevel[] Levels { get; private set; }

        public ConsoleEventsConsumerConfig(IConfigParameters parameters)
        {
            if (parameters.Has("Levels"))
            {
                var levels = (string)parameters["Levels"];
                if (!string.IsNullOrEmpty(levels))
                {
                    var levelStrings = levels.Split(new string[] { ", ", "; ", ",", ";" }, StringSplitOptions.RemoveEmptyEntries);
                    if (levelStrings.Length > 0)
                    {
                        this.Levels = levelStrings.Select(s => (EventLevel)Enum.Parse(typeof(EventLevel), s, true)).ToArray();
                        _hasLevels = true;
                    }
                    
                }
            }

            if (parameters.Has("Contexts"))
            {
                var contexts = (string)parameters["Contexts"];
                if (!string.IsNullOrEmpty(contexts))
                {
                    this.Contexts = contexts.Split(new string[] { ", ", "; ", ",", ";" }, StringSplitOptions.RemoveEmptyEntries);
                    _hasContexts = this.Contexts.Length > 0;
                }
            }

            if (parameters.Has("Categories"))
            {
                var categories = (string)parameters["Categories"];
                if (!string.IsNullOrEmpty(categories))
                {
                    this.Categories = categories.Split(new string[] { ", ", "; ", ",", ";" }, StringSplitOptions.RemoveEmptyEntries);
                    _hasCategories = this.Categories.Length > 0;
                }
            }
        }

        public bool HasFilters => this._hasLevels || this._hasContexts || this._hasCategories;

        public bool HasLevels => this._hasLevels;

        public bool HasContexts => this._hasContexts;

        public bool HasCategories => this._hasCategories;

        public bool Check(IEvent @event)
        {
            if (@event.Level == EventLevel.Exception || @event.Level == EventLevel.Critical)
            {
                return true;
            }

            if (this._hasLevels)
            {
                if (!this.Levels.Where(l => l == @event.Level).Any())
                {
                    return false;
                }
            }

            if (this._hasContexts)
            {
                if (!this.Contexts.Where(c => c == @event.Context.Name).Any())
                {
                    return false;
                }
            }

            if (this._hasCategories)
            {
                if (!this.Categories.Where(c => c == @event.Category.Name).Any())
                {
                    return false;
                }
            }

            return true;
        }
    }
}
