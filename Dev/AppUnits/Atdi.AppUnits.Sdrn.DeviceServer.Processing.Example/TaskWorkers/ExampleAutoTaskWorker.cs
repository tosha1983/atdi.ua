using Atdi.Contracts.Sdrn.DeviceServer;
using Atdi.DataModels.Sdrn.DeviceServer;
using Atdi.DataModels.Sdrn.DeviceServer.Processing;
using Atdi.DataModels.Sdrn.DeviceServer.Processing.Test;
using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Processing.Example.TaskWorkers
{
    public class ExampleAutoTaskWorker : ITaskWorker<ExampleAutoTask1, DeviceServerBackgroundProcess, SingletonTaskWorkerLifetime>
    {
        private readonly IProcessingDispatcher _processingDispatcher;
        private readonly ITimeService _timeService;
        private readonly ITaskStarter _taskStarter;
        private readonly ILogger _logger;

        public ExampleAutoTaskWorker(ITimeService timeService, IProcessingDispatcher processingDispatcher, ITaskStarter taskStarter, ILogger logger)
        {
            this._processingDispatcher = processingDispatcher;
            this._timeService = timeService;
            this._taskStarter = taskStarter;
            this._logger = logger;
        }

        public void Run(ITaskContext<ExampleAutoTask1, DeviceServerBackgroundProcess> context)
        {
            try
            {
                var process = _processingDispatcher.Start<Test2Process>(context.Process);
                var test2Task = new Test2Task
                {
                    TimeStamp = _timeService.TimeStamp.Milliseconds, // фиксируем текущий момент, или берем заранее снятый
                    //Delay = 5, // необходимо запустить через 5 мс, с указанного момента
                    //Options = TaskExecutionOption.RunDelayed,
                };

                var cancelSource = new CancellationTokenSource();
                _taskStarter.Run(test2Task, process, context);

                //for (int i = 0; i < 100; i++)
                //{
                //    var cancelSource = new CancellationTokenSource();
                //    var timer = System.Diagnostics.Stopwatch.StartNew();

                //    var exampleTask1 = new ExampleTask
                //    {
                //        TimeStamp = _timeService.TimeStamp.Milliseconds,
                //        Timer  = System.Diagnostics.Stopwatch.StartNew()
                //    };

                //    var t = _taskStarter.RunParallel(exampleTask1, process, context, cancelSource.Token);
                //    timer.Stop();
                //    t.Wait();
                //    _logger.Verbouse("RunParallel", $"Cost {timer.Elapsed.TotalMilliseconds}ms");

                //}

                //for (int i = 0; i < 100; i++)
                //{
                //    var cancelSource = new CancellationTokenSource();
                //    var timer = System.Diagnostics.Stopwatch.StartNew();

                //    var exampleTask1 = new ExampleTask
                //    {
                //        TimeStamp = _timeService.TimeStamp.Milliseconds,
                //        Timer = System.Diagnostics.Stopwatch.StartNew()
                //    };

                //    _taskStarter.Run(exampleTask1, process, context, cancelSource.Token);
                //    timer.Stop();
                //    _logger.Verbouse("RunSync", $"Cost {timer.Elapsed.TotalMilliseconds}ms");

                //}


                context.Finish();

            }
            catch (Exception e)
            {

                context.Abort(e);
            }
        }
    }
}
