using Atdi.Contracts.Sdrn.DeviceServer;
using Atdi.DataModels.Sdrn.DeviceServer;
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
        private readonly ILogger _logger;

        public TaskWorker(TaskWorkerDescriptor descriptor, ITaskWorkerFactory workerFactory, ILogger logger)
        {
            this._descriptor = descriptor;
            this._workerFactory = workerFactory;
            this._logger = logger;
        }

        public bool IsAsync => _descriptor.IsAsync;

        public void Run(ITaskDescriptor descriptor)
        {
            var task = descriptor.Task as TaskBase;
            var taskContext = _descriptor.CreateTaskContext(descriptor);
            var instance = this._workerFactory.Create(this._descriptor.InstanceType);
            task.ChangeState(TaskState.Executing);
            _descriptor.InvokerSync(instance, taskContext);
        }

        public Task RunAsync(ITaskDescriptor descriptor)
        {
            var task = descriptor.Task as TaskBase;
            var taskContext = _descriptor.CreateTaskContext(descriptor);
            var instance = this._workerFactory.Create(this._descriptor.InstanceType);
            task.ChangeState(TaskState.Executing);
            return _descriptor.InvokerAsync(instance, taskContext);
        }
    }
}
