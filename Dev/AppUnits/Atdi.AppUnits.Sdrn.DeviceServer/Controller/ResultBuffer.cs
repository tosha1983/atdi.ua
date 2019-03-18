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
    class ResultBuffer : IResultBuffer
    {
        private readonly object _locker = new object();
        //private readonly ConcurrentBag<ICommandResultPart> _queue;
        private readonly ConcurrentQueue<ICommandResultPart> _queue;
        private readonly CommandDescriptor _descriptor;
        private readonly CancellationTokenSource _tokenSource;
        private readonly CancellationToken _cancellationToken;
        private EventWaitHandle _waiter;

        public ResultBuffer(CommandDescriptor descriptor)
        {
            this._queue = new ConcurrentQueue<ICommandResultPart>();
            //this._queue = new ConcurrentBag<ICommandResultPart>();
            this._waiter = new AutoResetEvent(false);
            this._descriptor = descriptor;
            this._tokenSource = new CancellationTokenSource();
            this._cancellationToken = _tokenSource.Token;
        }

        public ICommandDescriptor Descriptor => _descriptor;

        public void Push(ICommandResultPart resultPart)
        {
            this._queue.Enqueue(resultPart);
            //this._queue.Add(resultPart);

            /// to send event about new command 
            this._waiter.Set();
        }

        public ICommandResultPart Take()
        {
            while (true)
            {

                if (_queue.TryDequeue(out ICommandResultPart resultPart))
                {
                    return resultPart;
                }

                //if (_queue.TryTake(out ICommandResultPart resultPart))
                //{
                //    return resultPart;
                //}
                /// to wait next command
                _waiter.WaitOne();
                
                if (_cancellationToken.IsCancellationRequested)
                {
                    return null;
                }
            }
        }

        public void Cancel()
        {
            this._tokenSource.Cancel();
            this._waiter.Set();
        }
    }
}
