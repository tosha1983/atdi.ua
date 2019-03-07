using Atdi.Contracts.Sdrn.DeviceServer;
using Atdi.DataModels.Sdrn.DeviceServer;
using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Common;
using System.Threading;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Processing
{
    class AutoTaskActivator : IAutoTaskActivator
    {
        private readonly IWorkScheduler _workScheduler;
        private readonly IProcessingDispatcher _processingDispatcher;
        private readonly ITaskWorkersHost _workersHost;
        private readonly ITaskStarter _taskStarter;
        private readonly ITimeService _timeService;
        private readonly ILogger _logger;
        private readonly CancellationTokenSource _tokenSource;
        private readonly ConcurrentDictionary<string, Task> _runningTasks;

        private int _disposingTimeout = 2000;
        private int _autoTaskStartDelay = 5000;

        public AutoTaskActivator(
            IWorkScheduler workScheduler,
            IProcessingDispatcher processingDispatcher, 
            ITaskWorkersHost workersHost, 
            ITaskStarter taskStarter, 
            ITimeService timeService, 
            ILogger logger)
        {
            this._workScheduler = workScheduler;
            this._processingDispatcher = processingDispatcher;
            this._workersHost = workersHost;
            this._taskStarter = taskStarter;
            this._timeService = timeService;
            this._logger = logger;
            this._tokenSource = new CancellationTokenSource();
            this._runningTasks = new ConcurrentDictionary<string, Task>();
        }

        public void Dispose()
        {
            if (this._runningTasks.Count > 0)
            {
                this._tokenSource.Cancel();
                Thread.Sleep(_disposingTimeout);
            }
        }

        public Task Run()
        {
            return _workScheduler.Run("Automatic task activation", this.RunAutoTasks);
        }

        private void RunAutoTasks()
        {
            _logger.Verbouse(Contexts.TaskActivator, Categories.Running, Events.AutomaticTaskActivationWasStarted);
            try
            {
                var process = this._processingDispatcher.Start<DeviceServerBackgroundProcess>();

                var tasks = this._workersHost.GetAutoTasks();
                for (int i = 0; i < tasks.Length; i++)
                {
                    var (taskType, processType) = tasks[i];

                    var taskObject = Activator.CreateInstance(taskType) as IAutoTask;
                    taskObject.TimeStamp = _timeService.TimeStamp.Milliseconds;
                    taskObject.Delay = this._autoTaskStartDelay;
                    taskObject.Options = TaskExecutionOption.RunDelayed;

                    var key = TaskWorkerDescriptor.BuildKey(taskType, processType);

                    var result = this._taskStarter.RunParallel(taskObject, process, this._tokenSource.Token);
                    result.ContinueWith((t) =>
                    {
                        _runningTasks.TryRemove(key, out Task t2);
                        _logger.Verbouse(Contexts.TaskActivator, Categories.Running, Events.AutomaticTaskWorkerCodeHasCompleted.With(taskType, processType));
                    });
                    this._runningTasks.TryAdd(key, result);

                }
            }
            catch (Exception e)
            {
                _logger.Exception(Contexts.TaskActivator, Categories.Running, e);
            }
            _logger.Verbouse(Contexts.TaskActivator, Categories.Running, Events.AutomaticTaskActivationWasFinished);
        }
    }
}
