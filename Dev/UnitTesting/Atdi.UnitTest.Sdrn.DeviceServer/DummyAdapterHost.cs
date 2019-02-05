using Atdi.Contracts.Sdrn.DeviceServer;
using Atdi.DataModels.Sdrn.DeviceServer;
using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.UnitTest.Sdrn.DeviceServer
{
    public class DummyAdapterHost : IAdapterHost
    {
        private readonly ILogger _logger;

        public DummyAdapterHost(ILogger logger)
        {
            this._logger = logger;
        }

        public void RegisterHandler<TCommand, TResult>(Action<TCommand, IExecutionContext> commandHandler) where TCommand : new()
        {
            this._logger.Verbouse("DummyAdapterHost", "Call method", $"RegisterHandler");
        }
    }
}
