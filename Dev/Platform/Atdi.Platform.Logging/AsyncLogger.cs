using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Platform.Logging
{
    public sealed class AsyncLogger : ILogger, IEventsProducer
    {
        private class ConsumerDescriptor
        {
            private readonly IEventsConsumer _consumer;
            private Task _task;
            public ConsumerDescriptor(IEventsConsumer consumer)
            {
                this._consumer = consumer;
                this._task = Task.Run(() => {; });
            }

            public void Push(IEvent[] events)
            {
                _task = _task.ContinueWith(t =>
                {
                    try
                    {
                        _consumer.Push(events);
                    }
                    catch (Exception){ }
                });
            }
        }

        private bool _loggerDisposed = false;
        private readonly IStatistics _statistics;
        private readonly ILogConfig _config;
        private readonly IEventDataConvertor _dataConvertor;
        private readonly BlockingCollection<IEvent> _events;

        private readonly int _eventsCapacity;
        private readonly int _taskCapacity;

        private readonly Dictionary<IEventsConsumer, ConsumerDescriptor> _consumers;
        private readonly Task _eventsHandlerTask;

        private static readonly IStatisticCounterKey ConsumerCountCounter = STS.DefineCounterKey("Logger.Consumers.Count");
        private static readonly IStatisticCounterKey EventCountCounter = STS.DefineCounterKey("Logger.Events.Count");
        private static readonly IStatisticCounterKey EventErrorsCounter = STS.DefineCounterKey("Logger.Events.Errors");
        private static readonly IStatisticCounterKey EventExceptionsCounter = STS.DefineCounterKey("Logger.Events.Exceptions");
        private static readonly IStatisticCounterKey EventCriticalsCounter = STS.DefineCounterKey("Logger.Events.Criticals");
        private static readonly IStatisticCounterKey EventWarnsCounter = STS.DefineCounterKey("Logger.Events.Warns");

        public AsyncLogger(IStatistics statistics, ILogConfig config, IEventDataConvertor dataConvertor)
        {
            this._statistics = statistics;
            this._config = config;
            this._dataConvertor = dataConvertor;

            var eventsCapacity = _config[LogConfig.EventsCapacityConfigKey];
            if (eventsCapacity == null || !int.TryParse(eventsCapacity.ToString(), out this._eventsCapacity))
            {
                this._eventsCapacity = LogConfig.DefaultEventsCapacity;
            }

            this._taskCapacity = this._eventsCapacity * 3;
            this._events = new BlockingCollection<IEvent>(_eventsCapacity);
            this._consumers = new Dictionary<IEventsConsumer, ConsumerDescriptor>();
            this._eventsHandlerTask = new Task(this.EventsHandler);
            this._eventsHandlerTask.Start();
            
        }

        private void EventsHandler()
        {
            while (!this._events.IsCompleted)
            {

                var tookEntries = new List<IEvent>(_taskCapacity);
                IEvent blockedEntry = null;
                try
                {
                    // blocks if number is zero
                    blockedEntry = this._events.Take();
                }
                catch (InvalidOperationException)
                { }

                if (blockedEntry != null)
                {
                    tookEntries.Add(blockedEntry);
                }

                var cancel = false;
                while (!cancel)
                {
                    if (this._events.TryTake(out IEvent entry, 1000))
                    {
                        tookEntries.Add(entry);
                        cancel = tookEntries.Count >= 1000; // _taskCapacity;
                    }
                    else
                    {
                        cancel = true;
                    }
                }

                if (tookEntries.Count > 0 && this._consumers != null && this._consumers.Count > 0)
                {
                    //var evt = new CriticalEvent()
                    //{
                    //    Category = "Internal",
                    //    Context = "Logger",
                    //    ManagedThread = System.Threading.Thread.CurrentThread.ManagedThreadId,
                    //    Text = $"Took Entries {tookEntries.Count} ----------------------------------------------------------------------------",

                    //};
                    //tookEntries.Add(evt);
                    var sortedEntries = tookEntries.OrderBy(e => e.Time.Ticks).ToArray();

                    foreach (var consumer in this._consumers.Values)
                    {
                        consumer.Push(sortedEntries);
                    }
                }
            }
        }

        private void WriteEvent(IEvent @event)
        {
            try
            {
                _statistics.Increment(EventCountCounter);

                // Try quickly adding
                if (!this._events.TryAdd(@event))
                {
                    // Add with blocking
                    this._events.Add(@event);
                }
            }
            catch (Exception)
            {
            }
        }

        private IReadOnlyDictionary<string, string> ConvertEventData(IReadOnlyDictionary<string, object> data)
        {
            return data.ToDictionary(k => k.Key, v => _dataConvertor.Convert(v.Value));
        }

        internal void StopTrace(ITraceScope scope, TimeSpan? duration, IReadOnlyDictionary<string, object> data)
        {
            var now = DateTime.Now;

            var @event = new EndTraceEvent(scope)
            {
                Time = now,
                Context = scope.BeginEvent.Context,
                Category = scope.BeginEvent.Category,
                ScopeData = scope.ScopeData,
                Source = scope.BeginEvent.Source,
                Duration = duration
            };

            if (data != null)
            {
                @event.Data = ConvertEventData(data);
            }
            this.WriteEvent(@event);
        }

        internal void Trace(ITraceScope scope, EventText eventText, string source, TimeSpan? duration, IReadOnlyDictionary<string, object> data)
        {
            var now = DateTime.Now;

            var @event = new TraceEvent(TraceEventType.Trace)
            {
                Time = now,
                Context = scope.BeginEvent.Context,
                Category = scope.BeginEvent.Category,
                ScopeData = scope.ScopeData,
                Source = source ?? scope.BeginEvent.Source,
                Duration = duration,
                Text = eventText
            };

            if (data != null)
            {
                @event.Data = ConvertEventData(data);
            }
            this.WriteEvent(@event);
        }

        #region IEventsProducer

        public void AddConsumer(IEventsConsumer consumer)
        {
            var descriptor = new ConsumerDescriptor(consumer);
            _consumers[consumer] = descriptor;

            _statistics.Increment(ConsumerCountCounter);
        }

        #endregion

        #region IDisposable Support

        public void Dispose()
        {
            if (!this._loggerDisposed)
            {
                this._events.CompleteAdding();
                this._eventsHandlerTask.Wait();
                
                this._loggerDisposed = true;
            }
        }

        #endregion

        #region ILogger

        public void Critical(EventContext context, EventCategory category, EventText eventText, Exception e, string source, IReadOnlyDictionary<string, object> data)
        {
            var now = DateTime.Now;

            if (!this.IsAllowed(EventLevel.Critical))
                return;

            var @event = new CriticalEvent(e)
            {
                Time = now,
                Context = context,
                Category = category,
                Text = eventText,
                Source = source
            };

            if (data != null)
            {
                @event.Data = ConvertEventData(data);
            }
            this.WriteEvent(@event);
        }

        public void Debug(EventContext context, EventCategory category, EventText eventText, string source, IReadOnlyDictionary<string, object> data)
        {
            var now = DateTime.Now;

            if (!this.IsAllowed(EventLevel.Debug))
                return;

            var @event = new DebugEvent(EventLevel.Debug)
            {
                Time = now,
                Context = context,
                Category = category,
                Text = eventText,
                Source = source
            };

            if (data != null)
            {
                @event.Data = ConvertEventData(data);
            }
            this.WriteEvent(@event);
        }

        public void Error(EventContext context, EventCategory category, EventText eventText, string source, IReadOnlyDictionary<string, object> data)
        {
            var now = DateTime.Now;

            if (!this.IsAllowed(EventLevel.Error))
                return;

            var @event = new ErrorEvent()
            {
                Time = now,
                Context = context,
                Category = category,
                Text = eventText,
                Source = source
            };

            if (data != null)
            {
                @event.Data = ConvertEventData(data);
            }
            this.WriteEvent(@event);
            _statistics.Increment(EventErrorsCounter);
        }

        public void Exception(EventContext context, EventCategory category, EventText eventText, Exception e, string source, IReadOnlyDictionary<string, object> data)
        {
            var now = DateTime.Now;

            if (!this.IsAllowed(EventLevel.Exception))
                return;

            var @event = new ExceptionEvent(e)
            {
                Time = now,
                Context = context,
                Category = category,
                Text = eventText,
                Source = source
            };

            if (data != null)
            {
                @event.Data = ConvertEventData(data);
            }
            this.WriteEvent(@event);
            _statistics.Increment(EventExceptionsCounter);
        }

        public void Info(EventContext context, EventCategory category, EventText eventText)
        {
            var now = DateTime.Now;

            if (!this.IsAllowed(EventLevel.Info))
                return;

            var @event = new Event(EventLevel.Info)
            {
                Time = now,
                Context = context,
                Category = category,
                Text = eventText
            };

            this.WriteEvent(@event);
        }

        
        public bool IsAllowed(EventLevel level)
        {
            return _config.IsAllowed(level);
        }

        public ITraceScope StartTrace(EventContext context, EventCategory category, TraceScopeName scopeName, string source, IReadOnlyDictionary<string, object> data)
        {
            var now = DateTime.Now;

            if (!this.IsAllowed(EventLevel.Trace))
                return new TraceScopeDummy();

            var scopeData = new TraceScopeData(scopeName);

            var @event = new BeginTraceEvent()
            {
                Time = now,
                Context = context,
                Category = category,
                ScopeData = scopeData,
                Source = source
            };

            if (data != null)
            {
                @event.Data = ConvertEventData(data);
            }
            this.WriteEvent(@event);

            var scope = new TraceScope(this, @event, scopeData);
            return scope;
        }

        public void Verbouse(EventContext context, EventCategory category, EventText eventText)
        {
            var now = DateTime.Now;

            if (!this.IsAllowed(EventLevel.Verbouse))
                return;

            var @event = new Event(EventLevel.Verbouse)
            {
                Time = now,
                Context = context,
                Category = category,
                Text = eventText
            };

            this.WriteEvent(@event);
        }

        public void Warning(EventContext context, EventCategory category, EventText eventText)
        {
            var now = DateTime.Now;

            if (!this.IsAllowed(EventLevel.Warning))
                return;

            var @event = new Event(EventLevel.Warning)
            {
                Time = now,
                Context = context,
                Category = category,
                Text = eventText
            };


            this.WriteEvent(@event);
            _statistics.Increment(EventWarnsCounter);
        }

        #endregion
    }
}
