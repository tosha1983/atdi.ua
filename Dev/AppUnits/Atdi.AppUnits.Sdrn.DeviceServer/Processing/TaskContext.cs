using Atdi.DataModels.Sdrn.DeviceServer;
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Processing
{
    class TaskContext<TTask, TProcess> : ITaskContext<TTask, TProcess>
        where TTask : ITask
        where TProcess : IProcess
    {
        private readonly TaskBase _taskBase;
        private readonly CancellationTokenSource _tokenSource;
        private readonly ConcurrentDictionary<Type, EventWaitHandle> _waiters;
        private readonly ConcurrentDictionary<Type, object> _events;

        public TaskContext(ITaskDescriptor descriptor)
        {
            this.Descriptor = descriptor;
            this.Task = (TTask)descriptor.Task;
            this.Process = (TProcess)descriptor.Process;
            this._taskBase = descriptor.Task as TaskBase;
            this._tokenSource = CancellationTokenSource.CreateLinkedTokenSource(descriptor.Token);
            this._waiters = new ConcurrentDictionary<Type, EventWaitHandle>();
            this._events = new ConcurrentDictionary<Type, object>();
            this.Token = this._tokenSource.Token;
        }

        public ITaskDescriptor Descriptor { get; private set; }

        public TTask Task { get; private set; }

        public TProcess Process { get; private set; }

        public CancellationToken Token { get; private set; }

        public Exception Exception { get; private set; }

        public void Abort(Exception e)
        {
            this._taskBase.ChangeState(TaskState.Aborted);
            this.Exception = e;
        }

        public void Cancel()
        {
            if (!this._tokenSource.Token.IsCancellationRequested)
            {
                this._tokenSource.Cancel();
            }
            this._taskBase.ChangeState(TaskState.Cancelled);
        }

        public void Finish()
        {
            this._taskBase.ChangeState(TaskState.Done);
        }

        public void SetEvent<TEvent>(TEvent @event)
        {
            var waitHandle = this.GetEventWaitHandle<TEvent>();
            var eventQueue = this.GetEventQueue<TEvent>();
            eventQueue.Enqueue(@event);
            waitHandle.Set();
        }

        public bool WaitEvent<TEvent>(out TEvent @event, int millisecondsTimeout = Timeout.Infinite)
        {
            var waitHandle = this.GetEventWaitHandle<TEvent>();

            while(!this.TryTakeEvent(out @event))
            {
                if (!waitHandle.WaitOne(millisecondsTimeout))
                {
                    return false;
                }
            }

            return true;
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
            if (!_events.TryGetValue(typeof(TEvent), out var value))
            {
                return false;
            }
            var eventQueue = (ConcurrentQueue<TEvent>) value;

            return eventQueue.TryDequeue(out @event); ;
        }

        private ConcurrentQueue<TEvent> GetEventQueue<TEvent>()
        {
            var type = typeof(TEvent);
            if (!this._events.TryGetValue(type, out var eventQueue))
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
            return (ConcurrentQueue<TEvent>) eventQueue;
        }

        public bool WaitEvent<TEvent>(int millisecondsTimeout = Timeout.Infinite)
        {
            var waitHandle = this.GetEventWaitHandle<TEvent>();
            var result = waitHandle.WaitOne(millisecondsTimeout);
            return result;
        }

        public void SetEvent<TEvent>()
        {
            var waitHandle = this.GetEventWaitHandle<TEvent>();
            waitHandle.Set();
        }
    }
}
