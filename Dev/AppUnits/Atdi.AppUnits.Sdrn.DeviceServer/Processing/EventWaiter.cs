using Atdi.Contracts.Sdrn.DeviceServer;
using Atdi.Platform.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Processing
{
    public class EventWaiter : IEventWaiter, IDisposable
    {
        private readonly ILogger _logger;
        private ConcurrentDictionary<Type, EventWaitHandle> _waiters;
        private ConcurrentDictionary<Type, object> _events;

        public EventWaiter(ILogger logger)
        {
            this._logger = logger;
            this._waiters = new ConcurrentDictionary<Type, EventWaitHandle>();
            this._events = new ConcurrentDictionary<Type, object>();
            this._logger.Verbouse(Contexts.EventWaiter, Categories.Creating, Events.CreatedEventWaiter);
        }

        public void Dispose()
        {
            if (this._waiters != null)
            {
                foreach (var item in this._waiters)
                {
                    item.Value.Close();
                }
                this._waiters = null;
                this._events = null;
            }
        }

        public void Emit<TEvent>(TEvent @event)
        {
            var waitHandle = this.GetEventWaitHandle<TEvent>();
            var eventQueue = this.GetEventQueue<TEvent>();
            eventQueue.Enqueue(@event);
            waitHandle.Set();
        }

        public void Emit<TEvent>()
        {
            var waitHandle = this.GetEventWaitHandle<TEvent>();
            waitHandle.Set();
        }

        public bool Wait<TEvent>(out TEvent @event, int millisecondsTimeout = -1)
        {
            var waitHandle = this.GetEventWaitHandle<TEvent>();

            while (!this.TryTakeEvent(out @event))
            {
                if (!waitHandle.WaitOne(millisecondsTimeout))
                {
                    return false;
                }
            }

            return true;
        }

        public bool Wait<TEvent>(int millisecondsTimeout = -1)
        {
            var waitHandle = this.GetEventWaitHandle<TEvent>();
            var result = waitHandle.WaitOne(millisecondsTimeout);
            return result;
        }

        private EventWaitHandle GetEventWaitHandle<TEvent>()
        {
            var type = typeof(TEvent);
            if (!this._waiters.TryGetValue(type, out EventWaitHandle waitHandle))
            {
                waitHandle = new AutoResetEvent(false);
                if (!this._waiters.TryAdd(type, waitHandle))
                {
                    if (!this._waiters.TryGetValue(type, out waitHandle))
                    {
                        throw new InvalidOperationException("Could not get an event wait handler");
                    }
                }
            }
            return waitHandle;
        }

        private bool TryTakeEvent<TEvent>(out TEvent @event)
        {
            @event = default(TEvent);
            if (!_events.TryGetValue(typeof(TEvent), out object value))
            {
                return false;
            }
            var eventQueue = (ConcurrentQueue<TEvent>)value;

            return eventQueue.TryDequeue(out @event); ;
        }

        private ConcurrentQueue<TEvent> GetEventQueue<TEvent>()
        {
            var type = typeof(TEvent);
            if (!this._events.TryGetValue(type, out object eventQueue))
            {
                eventQueue = new ConcurrentQueue<TEvent>();
                if (!this._events.TryAdd(type, eventQueue))
                {
                    if (!this._events.TryGetValue(type, out eventQueue))
                    {
                        throw new InvalidOperationException("Could not get an event queue");
                    }
                }
            }
            return (ConcurrentQueue<TEvent>)eventQueue;
        }
    }
}
