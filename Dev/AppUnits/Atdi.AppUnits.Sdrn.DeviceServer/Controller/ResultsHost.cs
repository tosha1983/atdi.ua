using Atdi.Contracts.Sdrn.DeviceServer;
using Atdi.DataModels.Sdrn.DeviceServer;
using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Controller
{
    class ResultsHost : IResultsHost
    {
        private readonly IResultConvertorsHost _convertorsHost;
        private readonly IResultHandlersHost _handlersHost;
        private readonly ILogger _logger;
        private readonly ConcurrentDictionary<Guid, ResultWorker> _workers;

        public ResultsHost(IResultConvertorsHost convertorsHost, IResultHandlersHost handlersHost, ILogger logger)
        {
            this._convertorsHost = convertorsHost;
            this._handlersHost = handlersHost;
            this._logger = logger;
            this._workers = new ConcurrentDictionary<Guid, ResultWorker>();
        }

        public void ReleaseBuffer(IResultBuffer resultBuffer)
        {
            var id = resultBuffer.Descriptor.Command.Id;

            if (this._workers.TryRemove(id, out ResultWorker worker))
            {
                worker.Stop();
            }
        }

        public IResultBuffer TakeBuffer(ICommandDescriptor commandDescriptor)
        {
            var worker = new ResultWorker(commandDescriptor as CommandDescriptor, this._convertorsHost, this._handlersHost,  _logger);
            this._workers.TryAdd(commandDescriptor.Command.Id, worker);
            return worker.Run();
        }
    }
}
