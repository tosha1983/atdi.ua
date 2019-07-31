using Atdi.Contracts.Sdrn.DeviceServer;
using Atdi.Platform;
using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Controller
{
    class DevicesHost : IDevicesHost
    {
        private readonly IAdapterFactory _adapterFactory;
        private readonly ICommandsHost _commandsHost;
        private readonly IResultsHost _resultsHost;
        private readonly ITimeService _timeService;
        private readonly IEventWaiter _eventWaiter;
        private readonly IStatistics _statistics;
        private readonly ILogger _logger;
        private readonly Dictionary<Type, AdapterWorker> _workers;

        public DevicesHost(
            IAdapterFactory adapterFactory, 
            ICommandsHost commandsHost, 
            IResultsHost resultsHost, 
            ITimeService timeService,
            IEventWaiter eventWaiter,
            IStatistics statistics,
            ILogger logger)
        {
            this._adapterFactory = adapterFactory;
            this._commandsHost = commandsHost;
            this._resultsHost = resultsHost;
            this._timeService = timeService;
            this._eventWaiter = eventWaiter;
            this._statistics = statistics;
            this._logger = logger;
            this._workers = new Dictionary<Type, AdapterWorker>();
        }

        public void Dispose()
        {
            if (_workers.Count > 0)
            {
                try
                {
                    foreach (var worker in _workers.Values)
                    {
                        worker.Dispose();
                    }
                }
                catch (Exception e)
                {

                    _logger.Exception(Contexts.DevicesHost, Categories.Disposing, e);
                }
                _workers.Clear();
            }
        }

        public IDevice[] GetDevices()
        {
            return this._workers.Values.ToArray();
        }

        public void Register(Type adapterType)
        {
            if (this._workers.ContainsKey(adapterType))
            {
                return;
            }
            var worker = new AdapterWorker(adapterType, this._adapterFactory, this._commandsHost, this._resultsHost, this._timeService, this._eventWaiter, this._statistics, this._logger);
            this._workers.Add(adapterType, worker);

            _logger.Verbouse(Contexts.DevicesHost, Categories.Registering, Events.RegisteredAdapter.With(adapterType));

            worker.Run();
        }
    }
}
