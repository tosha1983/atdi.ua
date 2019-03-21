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
        private class EventToken : IEventToken
        {
            private EventWaiter _eventWaiter;
            private readonly Guid _value;

            public EventToken(EventWaiter eventWaiter)
            {
                this._eventWaiter = eventWaiter;
                this._value = Guid.NewGuid();
            }

            public Guid Value => this._value;

            public void Dispose()
            {
                if (this._eventWaiter != null)
                {
                    var token = this as IEventToken;
                    this._eventWaiter.ReleaseToken(ref token);
                    this._eventWaiter = null;
                }
            }
        }
        private readonly ILogger _logger;
        private ConcurrentDictionary<Type, EventWaitHandle> _waiters;
        private ConcurrentDictionary<Type, object> _events;

        private ConcurrentDictionary<Guid, ConcurrentDictionary<Type, EventWaitHandle>> _waitersByToken;
        private ConcurrentDictionary<Guid, ConcurrentDictionary<Type, object>> _eventsByToken;

        public EventWaiter(ILogger logger)
        {
            this._logger = logger;
            this._waiters = new ConcurrentDictionary<Type, EventWaitHandle>();
            this._waitersByToken = new ConcurrentDictionary<Guid, ConcurrentDictionary<Type, EventWaitHandle>>();
            this._events = new ConcurrentDictionary<Type, object>();
            this._eventsByToken = new ConcurrentDictionary<Guid, ConcurrentDictionary<Type, object>>();

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

            if (this._waitersByToken != null)
            {
                foreach (var waiters in this._waitersByToken)
                {
                    foreach (var item in waiters.Value)
                    {
                        item.Value.Close();
                    }
                }
                this._waitersByToken = null;
                this._eventsByToken = null;
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

        public bool Wait<TEvent>(IEventToken token, out TEvent @event, int millisecondsTimeout = -1)
        {
            var tokenObject = token as EventToken;
            var waitHandle = this.GetEventWaitHandleByToken<TEvent>(tokenObject.Value);

            while (!this.TryTakeEvent(out @event))
            {
                if (!waitHandle.WaitOne(millisecondsTimeout))
                {
                    return false;
                }
            }

            return true;
        }

        public bool Wait<TEvent>(IEventToken token, int millisecondsTimeout = -1)
        {
            var tokenObject = token as EventToken;
            var waitHandle = this.GetEventWaitHandleByToken<TEvent>(tokenObject.Value);
            var result = waitHandle.WaitOne(millisecondsTimeout);
            return result;
        }

        public void Emit<TEvent>(IEventToken token, TEvent @event)
        {
            var tokenObject = token as EventToken;
            var waitHandle = this.GetEventWaitHandleByToken<TEvent>(tokenObject.Value);
            var eventQueue = this.GetEventQueueByToken<TEvent>(tokenObject.Value);
            eventQueue.Enqueue(@event);
            waitHandle.Set();
        }

        public void Emit<TEvent>(IEventToken token)
        {
            var tokenObject = token as EventToken;
            var waitHandle = this.GetEventWaitHandleByToken<TEvent>(tokenObject.Value);
            waitHandle.Set();
        }

        public IEventToken GetToken()
        {
            return new EventToken(this);
        }

        public void ReleaseToken(ref IEventToken token)
        {
            var tokenObject = token as EventToken;

            if (this._waitersByToken.TryRemove(tokenObject.Value, out ConcurrentDictionary<Type, EventWaitHandle> waiters))
            {
                foreach (var item in waiters)
                {
                    item.Value.Close();
                }
            }
            this._eventsByToken.TryRemove(tokenObject.Value, out ConcurrentDictionary<Type, object> queue);
            token = null;
        }


        private EventWaitHandle GetEventWaitHandleByToken<TEvent>(Guid token)
        {

            var waiters = this.GetEventWaitersByToken(token);

            var type = typeof(TEvent);
            if (!waiters.TryGetValue(type, out EventWaitHandle waitHandle))
            {
                waitHandle = new AutoResetEvent(false);
                if (!waiters.TryAdd(type, waitHandle))
                {
                    if (!waiters.TryGetValue(type, out waitHandle))
                    {
                        throw new InvalidOperationException("Could not get an event wait handler by token");
                    }
                }
            }
            return waitHandle;
        }

        private ConcurrentDictionary<Type, EventWaitHandle> GetEventWaitersByToken(Guid token)
        {
            if (!this._waitersByToken.TryGetValue(token, out ConcurrentDictionary<Type, EventWaitHandle> waiters))
            {
                waiters = new ConcurrentDictionary<Type, EventWaitHandle>();
                if (!this._waitersByToken.TryAdd(token, waiters))
                {
                    if (!this._waitersByToken.TryGetValue(token, out waiters))
                    {
                        throw new InvalidOperationException("Could not get an event waiters by token");
                    }
                }
            }

            return waiters;
        }

        private ConcurrentQueue<TEvent> GetEventQueueByToken<TEvent>(Guid token)
        {
            var eventQueues = this.GetEventQueuesByToken(token);
            var type = typeof(TEvent);
            if (!eventQueues.TryGetValue(type, out object eventQueue))
            {
                eventQueue = new ConcurrentQueue<TEvent>();
                if (!eventQueues.TryAdd(type, eventQueue))
                {
                    if (!eventQueues.TryGetValue(type, out eventQueue))
                    {
                        throw new InvalidOperationException("Could not get an event queue by token");
                    }
                }
            }
            return (ConcurrentQueue<TEvent>)eventQueue;
        }

        private ConcurrentDictionary<Type, object> GetEventQueuesByToken(Guid token)
        {
            if (!this._eventsByToken.TryGetValue(token, out ConcurrentDictionary<Type, object> queues))
            {
                queues = new ConcurrentDictionary<Type, object>();
                if (!this._eventsByToken.TryAdd(token, queues))
                {
                    if (!this._eventsByToken.TryGetValue(token, out queues))
                    {
                        throw new InvalidOperationException("Could not get an event queues by token");
                    }
                }
            }
            return queues;
        }

        private bool TryTakeEventByToken<TEvent>(out TEvent @event, Guid token)
        {
            @event = default(TEvent);
            var eventQueues = this.GetEventQueuesByToken(token);
            if (!eventQueues.TryGetValue(typeof(TEvent), out object value))
            {
                return false;
            }
            var eventQueue = (ConcurrentQueue<TEvent>)value;

            return eventQueue.TryDequeue(out @event); ;
        }
    }
}
