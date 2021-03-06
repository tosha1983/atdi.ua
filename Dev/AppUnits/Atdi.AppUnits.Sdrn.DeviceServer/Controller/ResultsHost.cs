﻿using Atdi.Contracts.Sdrn.DeviceServer;
using Atdi.DataModels.Sdrn.DeviceServer;
using Atdi.Platform.Logging;
using System;
using System.Collections.Concurrent;
using Atdi.Platform;
using Atdi.Platform.Data;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Controller
{
    internal class ResultsHost : IResultsHost
    {
        private readonly IResultConvertorsHost _convertorsHost;
        private readonly IResultHandlersHost _handlersHost;
        private readonly IObjectPoolSite _poolSite;
        private readonly ILogger _logger;
        private readonly ConcurrentDictionary<Guid, ResultWorker> _workers;
        private readonly IStatisticCounter _createdCounter;
        private readonly IStatisticCounter _usingCounter;
        private readonly IStatisticCounter _releasedCounter;

        public ResultsHost(IResultConvertorsHost convertorsHost, IResultHandlersHost handlersHost, IObjectPoolSite poolSite, IStatistics statistics, ILogger logger)
        {
            this._convertorsHost = convertorsHost;
            this._handlersHost = handlersHost;
            this._poolSite = poolSite;
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
            _usingCounter?.Decrement();
            _releasedCounter?.Increment();

            //var id = resultBuffer.Id;

            //if (this._workers.TryRemove(id, out var worker))
            //{
                
            //    worker.Stop();
            //}
        }

        public IResultBuffer TakeBuffer()
        {
            var resultBuffer = new ResultBuffer();
            //var worker = new ResultWorker(resultBuffer, this._convertorsHost, this._handlersHost, this._poolSite, this._logger);
            //this._workers.TryAdd(resultBuffer.Id, worker);
            _createdCounter?.Increment();
            _usingCounter?.Increment();
            return resultBuffer;
        }
    }
}
