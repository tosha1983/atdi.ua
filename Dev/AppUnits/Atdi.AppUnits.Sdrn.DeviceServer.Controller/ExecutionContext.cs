using Atdi.Contracts.Sdrn.DeviceServer;
using Atdi.DataModels.Sdrn.DeviceServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Controller
{
    class ExecutionContext : IExecutionContext
    {
        private readonly CommandDescriptor _commandDescriptor;
        private readonly AdapterWorker _adapterWorker;
        private readonly IResultBuffer _resultBuffer;

        public ExecutionContext(CommandDescriptor commandDescriptor, AdapterWorker adapterWorker, IResultBuffer resultBuffer)
        {
            this._commandDescriptor = commandDescriptor;
            this._adapterWorker = adapterWorker;
            this._resultBuffer = resultBuffer;
        }

        public CancellationToken Token => _commandDescriptor.CancellationToken;

        public void Abort(Exception e)
        {
            _commandDescriptor.Abort(CommandFailureReason.Exception, e);
            _adapterWorker.FinalizeCommand(_commandDescriptor, _resultBuffer);
        }

        public void Cancel()
        {
            _commandDescriptor.Cancel();
            _adapterWorker.FinalizeCommand(_commandDescriptor, _resultBuffer);
        }

        public void Finish()
        {
            _commandDescriptor.Done();
            _adapterWorker.FinalizeCommand(_commandDescriptor, _resultBuffer);
        }

        public void Lock(params CommandType[] types)
        {
            for (int i = 0; i < types.Length; i++)
            {
                this._adapterWorker.Lock(types[i]);
            }
        }

        public void Lock(params Type[] commandTypes)
        {
            for (int i = 0; i < commandTypes.Length; i++)
            {
                this._adapterWorker.Lock(commandTypes[i]);
            }
        }

        public void Lock()
        {
            this._adapterWorker.Lock(_commandDescriptor.CommandType);
        }

        public void PushResult(ICommandResultPart result)
        {
            this._resultBuffer.Push(result);
        }

        public void Unlock(params CommandType[] types)
        {
            for (int i = 0; i < types.Length; i++)
            {
                this._adapterWorker.Unlock(types[i]);
            }
        }

        public void Unlock(params Type[] commandTypes)
        {
            for (int i = 0; i < commandTypes.Length; i++)
            {
                this._adapterWorker.Unlock(commandTypes[i]);
            }
        }

        public void Unlock()
        {
            this._adapterWorker.Unlock(_commandDescriptor.CommandType);
        }
    }
}
