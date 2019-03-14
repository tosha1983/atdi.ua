using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Atdi.DataModels.Sdrn.DeviceServer;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Controller
{
    class CommandsBuffer
    {
        private readonly object _locker = new object();
        private readonly Queue<CommandDescriptor> _queue;
        private readonly Queue<CommandDescriptor> _scheduelQueue;
        private CommandDescriptor _urgentCommand;
        private EventWaitHandle _waiter;

        public CommandsBuffer()
        {
            this._queue = new Queue<CommandDescriptor>();
            this._scheduelQueue = new Queue<CommandDescriptor>();
            this._waiter = new AutoResetEvent(false);
        }

        public bool TryPush(CommandDescriptor commandDescriptor)
        {
            lock(_locker)
            {
                var command = commandDescriptor.Command;

                if ((command.Options & CommandOption.StartImmediately) == CommandOption.StartImmediately)
                {
                    if (this._urgentCommand != null)
                    {
                        /// rejected so the urgent command place is busy
                        return false;
                    }

                    this._urgentCommand = commandDescriptor;
                }
                else if ((command.Options & CommandOption.StartDelayed) == CommandOption.StartDelayed)
                {
                    this._scheduelQueue.Enqueue(commandDescriptor);
                }
                else
                {
                    this._queue.Enqueue(commandDescriptor);
                }
            }
            /// to send event about new command 
            this._waiter.Set();
            return true;
        }

        public CommandDescriptor Take()
        {
            while(true)
            {
                lock(_locker)
                {
                    var result = this._urgentCommand;
                    if (result != null)
                    {
                        this._urgentCommand = null;
                        return result;
                    }
                    if (_scheduelQueue.Count > 0)
                    {
                        result = _scheduelQueue.Dequeue();
                        return result;
                    }
                    if (_queue.Count > 0)
                    {
                        result = _queue.Dequeue();
                        return result;
                    }
                }
                /// to wait next command
                _waiter.WaitOne();
            }
        }
    }
}
