using Atdi.Contracts.Sdrn.DeviceServer;
using Atdi.DataModels.Sdrn.DeviceServer;
using Atdi.DataModels.Sdrn.DeviceServer.Commands;
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
    public class Test2TaskWorker : ITaskWorker<Test2Task, Test2Process, TransientTaskWorkerLifetime>
    {
        private readonly ExampleConfig _config;
        private readonly IController _controller;
        private readonly IEventWaiter _eventWaiter;
        private readonly ITimeService _timeService;
        private readonly ILogger _logger;
        private long _timeoutExpiredCount = 0;
        private long _callCommandCount = 0;

        public Test2TaskWorker(ExampleConfig config, IController controller, IEventWaiter eventWaiter, ITimeService timeService, ILogger logger)
        {
            this._config = config;
            this._controller = controller;
            this._eventWaiter = eventWaiter;
            this._timeService = timeService;
            this._logger = logger;

            this._logger.Debug(Contexts.Test2TaskWorker, Categories.Ctor, Events.Call);
        }

        public void Run(ITaskContext<Test2Task, Test2Process> context)
        {
            this._logger.Debug(Contexts.Test2TaskWorker, Categories.Run, Events.RunTask.With(context.Task.Id));

            var count = 200000; // 0;
            var delayStep = 50;
            var timestamp = this._timeService.TimeStamp.Milliseconds;
            var timeoutStep = 10;

            // подогрев
            {
                var delay = delayStep;
                var timeout = delayStep + timeoutStep;
                this.SendTask2ToController(100, 0, timestamp, delay, timeout, context);

                timestamp = this._timeService.TimeStamp.Milliseconds;
            }

            for (int i = 1; i <= count; i++)
            {
                var delay = i * delayStep;
                var timeout = i * delayStep + timeoutStep;
                this.SendTask2ToController(count, i, timestamp, delay, timeout, context);
            }

            _eventWaiter.Wait<TestCommand2>();
            Thread.Sleep(50000);
            this._logger.Debug(Contexts.Test2TaskWorker, Categories.Run, $"Finished: result count = {context.Process.TotalValues.Count}, count 2 = {context.Process.Count}");

            context.Finish();
        }

        private void SendTask2ToController(int lastIndex, int index, long timestamp, long delay, long timeout, ITaskContext<Test2Task, Test2Process> context)
        {
            var deviceCommand2 = new TestCommand2()
            {
                Options = CommandOption.StartDelayed | CommandOption.RaiseExecutionCompleted,
                StartTimeStamp = timestamp,
                Delay = delay,
                Timeout = timeout,
                Index = index,
                LastIndex = lastIndex
            };

            deviceCommand2.Parameter.Count = 1_0;// 0_000;
            deviceCommand2.Parameter.Predel = 1_0;


            this._controller.SendCommand<TestCommand2Result>(context, deviceCommand2, this.ControllerFailureAction);
        }

        private void ControllerFailureAction(ITaskContext taskContext, ICommand command, CommandFailureReason failureReason, Exception exception)
        {
            Interlocked.Increment(ref _callCommandCount);
            switch (failureReason)
            {
                case CommandFailureReason.NotFoundDevice:
                    _logger.Error(Contexts.Test2TaskWorker, Categories.Run, $"Not found device: Delay = {command.Delay}, Count = {_callCommandCount}");
                    break;
                case CommandFailureReason.NotFoundConvertor:
                    _logger.Error(Contexts.Test2TaskWorker, Categories.Run, $"Not found convertor: Delay = {command.Delay}, Count = {_callCommandCount}");
                    break;
                case CommandFailureReason.DeviceIsBusy:
                    _logger.Error(Contexts.Test2TaskWorker, Categories.Run, $"Device is busy: Delay = {command.Delay}, Count = {_callCommandCount}");
                    break;
                case CommandFailureReason.TimeoutExpired:
                    Interlocked.Increment(ref _timeoutExpiredCount);
                    _logger.Error(Contexts.Test2TaskWorker, Categories.Run, $"Timeout expired: Delay = {command.Delay}, timeout = {command.Timeout}, Count = {_callCommandCount}");
                    break;
                case CommandFailureReason.CanceledBeforeExecution:
                    _logger.Error(Contexts.Test2TaskWorker, Categories.Run, $"Canceled before execution: Delay = {command.Delay}, Count = {_callCommandCount}");
                    break;
                case CommandFailureReason.CanceledExecution:
                    _logger.Error(Contexts.Test2TaskWorker, Categories.Run, $"Canceled execution: Delay = {command.Delay}, Count = {_callCommandCount}");
                    break;
                case CommandFailureReason.Exception:
                    _logger.Exception(Contexts.Test2TaskWorker, Categories.Run, exception);
                    break;
                case CommandFailureReason.ExecutionCompleted:
                    _logger.Info(Contexts.Test2TaskWorker, Categories.Run, $"Execution Completed: Delay = {command.Delay}, TimeoutExpiredCount = {this._timeoutExpiredCount}, Count = {_callCommandCount}");
                    break;
                default:
                    break;
            }
        }
    }
}
