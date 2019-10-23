using Atdi.Contracts.Sdrn.DeviceServer;
using Atdi.DataModels.Sdrn.DeviceServer;
using Atdi.Platform;
using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Processing
{
    class TaskWorker : ITaskWorker
    {
        private readonly TaskWorkerDescriptor _descriptor;
        private readonly ITaskWorkerFactory _workerFactory;
        private readonly IStatistics _statistics;
        private readonly ILogger _logger;
        private readonly IStatisticCounter _tasksHitsCounter;
        private readonly IStatisticCounter _tasksSyncCounter;
        private readonly IStatisticCounter _tasksAsyncCounter;

        public TaskWorker(TaskWorkerDescriptor descriptor, ITaskWorkerFactory workerFactory, IStatistics statistics, ILogger logger)
        {
            this._descriptor = descriptor;
            this._workerFactory = workerFactory;
            this._statistics = statistics;
            this._logger = logger;
            if (this._statistics != null)
            {
                this._tasksHitsCounter = _statistics.Counter(Monitoring.TasksHitsKey);
                this._tasksSyncCounter = _statistics.Counter(Monitoring.TasksSyncKey);
                this._tasksAsyncCounter = _statistics.Counter(Monitoring.TasksAsyncKey);
            }
        }

        public bool IsAsync => _descriptor.IsAsync;

        public void Run(ITaskDescriptor descriptor)
        {
            this._tasksHitsCounter?.Increment();
            this._tasksSyncCounter?.Increment();

            var task = descriptor.Task as TaskBase;
            var taskContext = _descriptor.CreateTaskContext(descriptor);
            var instance = this._workerFactory.Create(this._descriptor.InstanceType);
            task.ChangeState(TaskState.Executing);
            _descriptor.InvokerSync(instance, taskContext);
        }

        public Task RunAsync(ITaskDescriptor descriptor)
        {
            this._tasksHitsCounter?.Increment();
            this._tasksAsyncCounter?.Increment();
            var task = descriptor.Task as TaskBase;
            var taskContext = _descriptor.CreateTaskContext(descriptor);
            var instance = this._workerFactory.Create(this._descriptor.InstanceType);
            task.ChangeState(TaskState.Executing);
            return _descriptor.InvokerAsync(instance, taskContext);
        }
    }
}
