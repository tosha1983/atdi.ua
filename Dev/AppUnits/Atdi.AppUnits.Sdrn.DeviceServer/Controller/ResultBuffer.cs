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
    internal sealed class ResultBuffer : IResultBuffer, IDisposable
    {
        private readonly object _locker = new object();
        private readonly ConcurrentQueue<ICommandResultPart> _queue;
        private readonly CommandDescriptor _descriptor;

        private EventWaitHandle _waiter;

        //private readonly Guid _id;
        public ResultBuffer(CommandDescriptor descriptor)
        {
            //this._id = Guid.NewGuid();
            this._queue = new ConcurrentQueue<ICommandResultPart>();
            this._waiter = new AutoResetEvent(false);
            this._descriptor = descriptor;
        }

        public ICommandDescriptor Descriptor => _descriptor;

        public void Push(ICommandResultPart resultPart)
        {
            //if (this._waiter == null)
            //{
            //    System.Diagnostics.Debug.WriteLine($"Waiter was disposed: {_id}");
            //}

            this._queue.Enqueue(resultPart);

            //if (this._waiter == null)
            //{
            //    System.Diagnostics.Debug.WriteLine($"Waiter was disposed: {_id}");
            //}

            lock (_locker)
            {
                // to send event about new command
                this._waiter?.Set();
            }
            
        }

        public ICommandResultPart Take()
        {
            while (true)
            {
                if (_queue.TryDequeue(out var resultPart))
                {
                    return resultPart;
                }

                //if (this._waiter == null)
                //{
                //    System.Diagnostics.Debug.WriteLine($"Waiter was disposed: {_id}");
                //}

                // to wait next command
                _waiter?.WaitOne();
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
