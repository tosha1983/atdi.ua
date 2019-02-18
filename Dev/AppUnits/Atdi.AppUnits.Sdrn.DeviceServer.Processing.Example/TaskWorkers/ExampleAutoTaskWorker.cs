using Atdi.Contracts.Sdrn.DeviceServer;
using Atdi.DataModels.Sdrn.DeviceServer;
using Atdi.DataModels.Sdrn.DeviceServer.Processing;
using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Processing.Example.TaskWorkers
{
    class ExampleAutoTaskWorker : IAutoTaskWorker<ExampleProcess, SingletonTaskWorkerLifetime>
    {
        private readonly ILogger _logger;

        public ExampleAutoTaskWorker(ILogger logger)
        {
            this._logger = logger;
        }

        public void Run(ITaskContext<AutoTask, ExampleProcess> context)
        {
            try
            {
                context.Finish();
            }
            catch (Exception e)
            {

                context.Abort(e);
            }
        }
    }
}
