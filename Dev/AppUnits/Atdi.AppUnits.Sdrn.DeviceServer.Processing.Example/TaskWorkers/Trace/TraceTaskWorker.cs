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
using Atdi.DataModels.Sdrn.DeviceServer.Adapters;
using Atdi.DataModels.Sdrn.DeviceServer.Processing;
using Atdi.DataModels.Sdrn.DeviceServer.Processing.Test;
using Atdi.DataModels.Sdrn.DeviceServer.TestCommands;
using Atdi.Platform.Data;
using Atdi.Platform.Logging;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Processing.Example.TaskWorkers
{
    internal class TraceTaskResultData
    {
        public int Index;
        public Guid TaskId;
        public Guid CommandId;
        public float[] FloatValues;
        public double[] DoubleValues;
    }

    public class TraceTaskWorker : ITaskWorker<TraceTask, TraceProcess, SingletonTaskWorkerLifetime>
    {
        private readonly IProcessingDispatcher _processingDispatcher;
        private readonly ITimeService _timeService;
        private readonly ITaskStarter _taskStarter;
        private readonly IObjectPoolSite _poolSite;
        private readonly ILogger _logger;
        private readonly IController _controller;
        private readonly IObjectPool<TraceTaskResultData> _traceResultPool;

        public TraceTaskWorker(
            ITimeService timeService, 
            IController controller, 
            IProcessingDispatcher processingDispatcher, 
            ITaskStarter taskStarter,
            IObjectPoolSite poolSite,
            ILogger logger)
        {
            this._processingDispatcher = processingDispatcher;
            this._timeService = timeService;
            this._controller = controller;
            this._taskStarter = taskStarter;
            this._poolSite = poolSite;
            this._logger = logger;

            this._traceResultPool = poolSite.Register<TraceTaskResultData>(new ObjectPoolDescriptor<TraceTaskResultData>
            {
                Key = "data",
                MinSize = 4,
                MaxSize = 50,
                Factory = () => new TraceTaskResultData()
                {
                    DoubleValues = new double[1_000_000],
                    FloatValues = new float[1_000_000],
                }
            });
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
                        BlockSize = context.Task.BlockSize,
                    };

                    this._controller.SendCommand<TraceCommandResult>(context, deviceCommand);

                    // ждем завершение комманды
                    context.WaitEvent<TraceTaskResultData>(out var result);

                    timer.Stop(); // тут реально заканчивается цепочка - создание комманды - выполнение комманды - получение результата - обработка

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
                    //    $"Duration (command): index=#{context.Task.Index.ToString()}, Time={totalMilliseconds.ToString(CultureInfo.InvariantCulture)}");

                    //context.Task.Timer.Restart();

                    if (result.CommandId != deviceCommand.Id)
                    {
                        _logger.Error(Contexts.TraceAutoTask, Categories.Run,
                            $"Incorrect command ID: index=#{context.Task.Index}, ID={deviceCommand.Id.ToString()} - {result.CommandId.ToString()}");
                    }
                    //else
                    //{
                    //    _logger.Info(Contexts.TraceAutoTask, Categories.Run,
                    //        $"Correct command ID: index=#{context.Task.Index}, ID={deviceCommand.Id.ToString()} - {result.CommandId.ToString()}");
                    //}
                    if (result.TaskId != context.Task.Id)
                    {
                        _logger.Error(Contexts.TraceAutoTask, Categories.Run,
                            $"Incorrect task ID: index=#{context.Task.Index}");
                    }

                    // генерируем некую нагрзузку связанную собработкой результата
                    //for (int j = 0; j < context.Task.BlockSize; j++)
                    //{
                    //    var f = result.FloatValues[j] / float.MinValue;
                    //    result.FloatValues[j] = f;

                    //    var d = result.DoubleValues[j] / double.MinValue;
                    //    result.DoubleValues[j] = d;
                    //}
                    //var (item1, item2) = new Tuple<float[], double[]>
                    //(new float[deviceCommand.BlockSize],
                    //    new double[deviceCommand.BlockSize]);


                    //Array.Copy(result.FloatValues, item1, deviceCommand.BlockSize);
                    //Array.Copy(result.DoubleValues, item2, deviceCommand.BlockSize);

                    _traceResultPool.Put(result);

                    //context.Process.IterationCount = item1.Length + item2.Length;

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
