using Atdi.Contracts.Sdrn.DeviceServer;
using Atdi.DataModels.Sdrn.DeviceServer;
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Controller
{
    internal sealed class ResultBuffer : IResultBuffer
    {
        private readonly object _locker = new object();
        private readonly ConcurrentQueue<ICommandResultPart> _queue;
        private EventWaitHandle _waiter;

        //private readonly Guid _id;
        public ResultBuffer()
        {
            this.Id = Guid.NewGuid();
            this._queue = new ConcurrentQueue<ICommandResultPart>();
            this._waiter = new AutoResetEvent(false);
        }

        public Guid Id { get; }
    

        public void Push(ICommandResultPart resultPart)
        {
            this._queue.Enqueue(resultPart);
            lock (_locker)
            {
                // to send event about new command
                this._waiter?.Set();
            }
        }

        public bool TryTake(out ICommandResultPart result, CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                if (_queue.TryDequeue(out result))
                {
                    return true;
                }

                // to wait next command
                // c просыпанием - для проверки токена отмены
                _waiter?.WaitOne();
            }
            result = null;
            return false;
        }

        public void Cancel()
        {
            if (this._waiter == null)
            {
                return;
            }

            lock (_locker)
            {
                // to send event about new command
                this._waiter?.Set();
            }
        }

        public void Dispose()
        {
            if (_waiter == null)
            {
                return;
            }

            lock (_locker)
            {
                _waiter.Close();
                _waiter = null;
            }
            
            //System.Diagnostics.Debug.WriteLine($"Waiter disposing: {_id}");
        }
    }
}
