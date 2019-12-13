using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Atdi.Contracts.Sdrn.DeviceServer;
using Atdi.DataModels.Sdrn.DeviceServer;
using Atdi.DataModels.Sdrn.DeviceServer.Processing;
using Atdi.DataModels.Sdrn.DeviceServer.Processing.Test;
using Atdi.Platform.Logging;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Processing.Example.TaskWorkers
{
    public class TraceAutoTaskWorker : ITaskWorker<TraceAutoTask, DeviceServerBackgroundProcess, SingletonTaskWorkerLifetime>
    {
        private readonly IProcessingDispatcher _processingDispatcher;
        private readonly ITimeService _timeService;
        private readonly ITaskStarter _taskStarter;
        private readonly ILogger _logger;

        public TraceAutoTaskWorker(ITimeService timeService, IProcessingDispatcher processingDispatcher, ITaskStarter taskStarter, ILogger logger)
        {
            this._processingDispatcher = processingDispatcher;
            this._timeService = timeService;
            this._taskStarter = taskStarter;
            this._logger = logger;
        }

        public void Run(ITaskContext<TraceAutoTask, DeviceServerBackgroundProcess> context)
        {
            try
            {
                var traceProcess = new TraceProcess()
                {
                    CommandCount = 0
                };

                //var task = new TraceTask()
                //{
                //    Count = 100000,
                //    BlockSize = 1000_000,
                //    Index = 100,
                //    Timer = new Stopwatch()
                //};

                //_taskStarter.RunParallel(task, traceProcess, context.Token);

                // запускаем 4 паралельных задачи, который грузят контролер измерениями трейса
                for (int i = 0; i < 2; i++)
                {
                    var task = new TraceTask()
                    {
                        Count = 100000000,
                        BlockSize = 1000_000,
                        Index = i,
                        Timer = new Stopwatch()
                    };

                    _taskStarter.RunParallel(task, traceProcess, context.Token);
                }
                for (int i = 2; i < 4; i++)
                {
                    var task = new TraceTask()
                    {
                        Count = 100000000,
                        BlockSize = 10_000,
                        Index = i,
                        Timer = new Stopwatch()
                    };

                    _taskStarter.RunParallel(task, traceProcess, context.Token);
                }

                while (traceProcess.TaskCount < 4)
                {
                    
                    // и наблюдаем за процессом, раз в секунду скидываем статистику в лог
                    Thread.Sleep(10000);
                    _logger.Info(Contexts.TraceAutoTask, Categories.Run, $"The commands done: {traceProcess.CommandCount}");
                    context.Token.ThrowIfCancellationRequested();

                }
                _logger.Info(Contexts.TraceAutoTask, Categories.Run, $"Finished: {traceProcess.CommandCount}");
                context.Finish();
            }
            catch (Exception e)
            {
                _logger.Exception(Contexts.TraceAutoTask, Categories.Run, e, this);
                context.Cancel();
            }
        }
    }
}
