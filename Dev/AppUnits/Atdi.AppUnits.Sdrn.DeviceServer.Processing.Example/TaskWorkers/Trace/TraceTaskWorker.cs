using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Atdi.Contracts.Sdrn.DeviceServer;
using Atdi.DataModels.Sdrn.DeviceServer;
using Atdi.DataModels.Sdrn.DeviceServer.Processing;
using Atdi.DataModels.Sdrn.DeviceServer.Processing.Test;
using Atdi.DataModels.Sdrn.DeviceServer.TestCommands;
using Atdi.Platform.Logging;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Processing.Example.TaskWorkers
{
    public class TraceTaskWorker : ITaskWorker<TraceTask, TraceProcess, SingletonTaskWorkerLifetime>
    {
        private readonly IProcessingDispatcher _processingDispatcher;
        private readonly ITimeService _timeService;
        private readonly ITaskStarter _taskStarter;
        private readonly ILogger _logger;
        private readonly IController _controller;

        public TraceTaskWorker(ITimeService timeService, IController controller, IProcessingDispatcher processingDispatcher, ITaskStarter taskStarter, ILogger logger)
        {
            this._processingDispatcher = processingDispatcher;
            this._timeService = timeService;
            this._controller = controller;
            this._taskStarter = taskStarter;
            this._logger = logger;
        }

        public void Run(ITaskContext<TraceTask, TraceProcess> context)
        {
            try
            {
                _logger.Info(Contexts.TraceAutoTask, Categories.Run,
                    $"Trace task was started: index=#{context.Task.Index}");
                double maxTime = double.MinValue;
                double minTime = double.MaxValue;
                double avgTime = 0;

                var timer = context.Task.Timer;
                var totalMilliseconds = timer.Elapsed.TotalMilliseconds;
                var mainTimer = System.Diagnostics.Stopwatch.StartNew();  
                for (int i = 0; i < context.Task.Count; i++)
                {

                    timer.Restart();
                    // по сути у нас 2.5 милисекунды на выполнение комманды
                    // будем стараться  кэтому прийти
                    // пока эмулируем комманду
                    //Thread.Sleep(1000);

                    var deviceCommand = new TraceCommand()
                    {
                        Options = CommandOption.PutInQueue,
                        BlockSize = context.Task.BlockSize
                    };

                    this._controller.SendCommand<TraceCommandResult>(context, deviceCommand);

                    // ждем завершение комманды
                    //context.WaitEvent<TraceCommandResult>(out var result);

                    timer.Stop();
                    totalMilliseconds = timer.Elapsed.TotalMilliseconds;
                    if (minTime > totalMilliseconds)
                    {
                        minTime = totalMilliseconds;
                    }
                    if (maxTime < totalMilliseconds)
                    {
                        maxTime = totalMilliseconds;
                    }

                    avgTime += totalMilliseconds;

                    //_logger.Info(Contexts.TraceAutoTask, Categories.Run,
                    //    $"Duration (command): index=#{context.Task.Index.ToString()}, Time={context.Task.Timer.Elapsed.TotalMilliseconds.ToString(CultureInfo.InvariantCulture)}");

                    //context.Task.Timer.Restart();

                    //if (result.Id != deviceCommand.Id)
                    //{
                    //    _logger.Error(Contexts.TraceAutoTask, Categories.Run,
                    //        $"Incorrect command ID: index=#{context.Task.Index}");
                    //}

                    // генерируем некую нагрзузку связанную собработкой результата
                    //for (int j = 0; j < context.Task.BlockSize; j++)
                    //{
                    //    var f = result.ValueAsFloats[j] / float.MinValue;
                    //    result.ValueAsFloats[j] = f;

                    //    var d = result.ValuesAsDouble[j] / double.MinValue;
                    //    result.ValuesAsDouble[j] = d;
                    //}
                    
                    //context.Task.Timer.Stop();
                    //_logger.Info(Contexts.TraceAutoTask, Categories.Run,
                    //    $"Duration (handle): index=#{context.Task.Index.ToString()}, Time={context.Task.Timer.Elapsed.TotalMilliseconds.ToString(CultureInfo.InvariantCulture)}");
                }
                mainTimer.Stop();
                avgTime /= context.Task.Count;

                _logger.Info(Contexts.TraceAutoTask, Categories.Run,
                    $"Trace task was finished: index=#{context.Task.Index.ToString()}, Count={context.Task.Count.ToString()} MinTime={minTime.ToString(CultureInfo.InvariantCulture)}, AvgTime={avgTime.ToString(CultureInfo.InvariantCulture)}, MaxTime={maxTime.ToString(CultureInfo.InvariantCulture)}, FullTime={mainTimer.Elapsed.TotalMilliseconds.ToString(CultureInfo.InvariantCulture)}, AvgFullTime={(mainTimer.Elapsed.TotalMilliseconds/context.Task.Count).ToString(CultureInfo.InvariantCulture)}");

                context.Finish();
            }
            catch (Exception e)
            {
                _logger.Exception(Contexts.TraceAutoTask, Categories.Run, e, this);
                context.Cancel();
            }
            finally
            {
                Interlocked.Increment(ref context.Process.TaskCount);
            }
        }
    }
}
