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
    public class AutoTaskWorkerSO : ITaskWorker<SOTask, DeviceServerBackgroundProcess, SingletonTaskWorkerLifetime>
    {
        private readonly IProcessingDispatcher _processingDispatcher;
        private readonly ITimeService _timeService;
        private readonly ITaskStarter _taskStarter;
        private readonly ILogger _logger;

        public AutoTaskWorkerSO(ITimeService timeService, IProcessingDispatcher processingDispatcher, ITaskStarter taskStarter, ILogger logger)
        {
            this._processingDispatcher = processingDispatcher;
            this._timeService = timeService;
            this._taskStarter = taskStarter;
            this._logger = logger;
        }

        public void Run(ITaskContext<SOTask, DeviceServerBackgroundProcess> context)
        {
            try
            {
                var process = _processingDispatcher.Start<MeasProcess>(context.Process);
                var test1Task = new SOTask
                {
                    TimeStamp = _timeService.TimeStamp.Milliseconds, // фиксируем текущий момент, или берем заранее снятый
                    Delay = 5, // необходимо запустить через 5 мс, с указанного момента
                    Options = TaskExecutionOption.RunDelayed
                };

                var cancelSource = new CancellationTokenSource();
                var t = _taskStarter.RunParallel(test1Task, process, context, cancelSource.Token);
                context.Finish();
            }
            catch (Exception e)
            {

                context.Abort(e);
            }
        }
    }
}
