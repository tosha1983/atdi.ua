using Atdi.Contracts.Sdrn.DeviceServer;
using Atdi.DataModels.Sdrn.DeviceServer;
using Atdi.Platform;
using System;
using System.Threading;
using Atdi.Platform.Data;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Controller
{
    internal sealed class ExecutionContext : IExecutionContext
    {
        private CommandDescriptor _commandDescriptor;
        private CommandContext _commandContext;

        private readonly AdapterWorker _adapterWorker;
        private readonly IResultBuffer _resultBuffer;
        private readonly IObjectPoolSite _poolSite;

        private readonly IStatisticCounter _adaptersCommandsExecutionCountCounter;
        private readonly IStatisticCounter _adaptersCommandsExecutionCompletedCounter;
        private readonly IStatisticCounter _adaptersCommandsExecutionCanceledCounter;
        private readonly IStatisticCounter _adaptersCommandsExecutionAbortedCounter;

        private ICommandResultPart _lastResult;

        public ExecutionContext(AdapterWorker adapterWorker, IResultBuffer resultBuffer, IObjectPoolSite poolSite, IStatistics statistics)
        {
            this._adapterWorker = adapterWorker;
            this._resultBuffer = resultBuffer;
            this._poolSite = poolSite;

            if (statistics != null)
            {
                this._adaptersCommandsExecutionCountCounter = statistics.Counter(Monitoring.AdaptersCommandsExecutionCountKey);
                this._adaptersCommandsExecutionCompletedCounter = statistics.Counter(Monitoring.AdaptersCommandsExecutionCompletedKey);
                this._adaptersCommandsExecutionCanceledCounter = statistics.Counter(Monitoring.AdaptersCommandsExecutionCanceledKey);
                this._adaptersCommandsExecutionAbortedCounter = statistics.Counter(Monitoring.AdaptersCommandsExecutionAbortedKey);
            }
        }

        public void Use(CommandDescriptor descriptor, CommandContext context)
        {
            this._commandDescriptor = descriptor;
            this._commandContext = context;
            this._adaptersCommandsExecutionCountCounter?.Increment();
        }

        public CancellationToken Token => _commandDescriptor.CancellationToken;

        public void Abort(Exception e)
        {
            if (_lastResult != null)
            {
                this.ReleaseResult(_lastResult);
                _lastResult = null;
            }
            _commandDescriptor.Abort(CommandFailureReason.Exception, e);
            _adapterWorker.FinalizeCommand(_commandDescriptor, this._commandContext);

            this._adaptersCommandsExecutionAbortedCounter?.Increment();
            this._adaptersCommandsExecutionCountCounter?.Decrement();
        }

        public void Cancel()
        {
            if (_lastResult != null)
            {
                this.ReleaseResult(_lastResult);
                _lastResult = null;
            }

            _commandDescriptor.Cancel();
            _adapterWorker.FinalizeCommand(_commandDescriptor, this._commandContext);

            this._adaptersCommandsExecutionCanceledCounter?.Increment();
            this._adaptersCommandsExecutionCountCounter?.Decrement();

        }

        public void Finish()
        {
            if (_lastResult != null)
            {
                this.ReleaseResult(_lastResult);
                _lastResult = null;
            }
            _commandDescriptor.Done();
            _adapterWorker.FinalizeCommand(_commandDescriptor, this._commandContext);

            this._adaptersCommandsExecutionCompletedCounter?.Increment();
            this._adaptersCommandsExecutionCountCounter?.Decrement();

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
            _lastResult = null;
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

        public T TakeResult<T>(string key, ulong index, CommandResultStatus status) where T : ICommandResultPart
        {
            if (_lastResult != null)
            {
                throw new InvalidOperationException("Last returned result pool object not returned to the result pool");
            }

            var pool = this.GetResultPool<T>(key);
            var result = pool.Take();
            result.PoolKey = key;
            result.PoolId = Guid.NewGuid();
            result.PartIndex = index;
            result.Status = status;
            _lastResult = result;
            return result;
        }

        public void ReleaseResult<T>(T result) where T : ICommandResultPart
        {
            if (result == null) throw new ArgumentNullException(nameof(result));
            
            var pool = this.GetResultPool<T>(result.PoolKey);
            result.PoolId = Guid.Empty;
            pool.Put(result);
        }

        private IObjectPool<T> GetResultPool<T>(string key)
        {
            var resultPoolKey = new ValueTuple<Type, CommandType, string>(typeof(T), _commandDescriptor.Command.Type, key); // AdapterWorker.BuildPoolKey(_commandDescriptor.Command.Type, key);
            var pool = (IObjectPool<T>)_commandContext.ResultsPool[resultPoolKey];
            return pool;
        }
    }
}
