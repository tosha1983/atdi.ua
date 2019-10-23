using Atdi.Contracts.Sdrn.DeviceServer;
using Atdi.DataModels.Sdrn.DeviceServer;
using Atdi.Platform.Logging;
using System;
using System.Collections.Concurrent;
using Atdi.Platform;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Controller
{
    internal class ResultsHost : IResultsHost
    {
        private readonly IResultConvertorsHost _convertorsHost;
        private readonly IResultHandlersHost _handlersHost;
        private readonly ILogger _logger;
        private readonly ConcurrentDictionary<Guid, ResultWorker> _workers;
        private readonly IStatisticCounter _createdCounter;
        private readonly IStatisticCounter _usingCounter;
        private readonly IStatisticCounter _releasedCounter;

        public ResultsHost(IResultConvertorsHost convertorsHost, IResultHandlersHost handlersHost, IStatistics statistics, ILogger logger)
        {
            this._convertorsHost = convertorsHost;
            this._handlersHost = handlersHost;
            this._logger = logger;
            this._workers = new ConcurrentDictionary<Guid, ResultWorker>();
            if (statistics != null)
            {
                this._createdCounter = statistics.Counter(Monitoring.ResultBufferCreatedKey);
                this._usingCounter = statistics.Counter(Monitoring.ResultBufferUsingKey);
                this._releasedCounter = statistics.Counter(Monitoring.ResultBufferReleasedKey);
            }
        }

        public void ReleaseBuffer(IResultBuffer resultBuffer)
        {
            var id = resultBuffer.Descriptor.Command.Id;

            if (this._workers.TryRemove(id, out var worker))
            {
                _usingCounter?.Decrement();
                _releasedCounter?.Increment();
                worker.Stop();
            }
        }

        public IResultBuffer TakeBuffer(ICommandDescriptor commandDescriptor)
        {
            var worker = new ResultWorker(commandDescriptor as CommandDescriptor, this._convertorsHost, this._handlersHost,  _logger);
            this._workers.TryAdd(commandDescriptor.Command.Id, worker);
            _createdCounter?.Increment();
            _usingCounter?.Increment();
            return worker.Run();
        }
    }
}
