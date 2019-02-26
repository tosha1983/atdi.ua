using Atdi.Contracts.Sdrn.DeviceServer;
using Atdi.Platform.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Processing
{
    class TaskWorkersHost : ITaskWorkersHost
    {
        private readonly ITaskWorkerFactory _workerFactory;
        private readonly ILogger _logger;
        private readonly Dictionary<string, TaskWorkerDescriptor> _descriptors;
        private readonly Dictionary<string, (Type taskType, Type processType)> _autoTasks;
        private readonly ConcurrentDictionary<string, ITaskWorker> _workers;
        private object _loker = new object();

        public TaskWorkersHost(ITaskWorkerFactory workerFactory, ILogger logger)
        {
            this._workerFactory = workerFactory;
            this._logger = logger;
            this._descriptors = new Dictionary<string, TaskWorkerDescriptor>();
            this._autoTasks = new Dictionary<string, (Type taskType, Type processType)>();
            this._workers = new ConcurrentDictionary<string, ITaskWorker>();
        }

        public void Dispose()
        {
           
        }

        public (Type taskType, Type processType)[] GetAutoTasks()
        {
            return this._autoTasks.Values.ToArray();
        }

        public ITaskWorker GetTaskWorker(Type taskType, Type processType)
        {
            var key = TaskWorkerDescriptor.BuildKey(taskType, processType);

            if (this._workers.TryGetValue(key, out ITaskWorker worker))
            {
                return worker;
            }

            if (!this._descriptors.ContainsKey(key))
            {
                throw new InvalidOperationException($"Not found a task worker for the task of type '{taskType}' and the process of type '{processType}'");
            }
            lock (this._loker)
            {
                var descriptor = this._descriptors[key];
                worker = new TaskWorker(descriptor, this._workerFactory, this._logger);
                this._workers.TryAdd(key, worker);
                return worker;
            }
        }

        public void Register(Type workerInstanceType)
        {
            var descriptor = new TaskWorkerDescriptor(workerInstanceType);
            lock (this._loker)
            {
                if (this._descriptors.ContainsKey(descriptor.Key))
                {
                    throw new InvalidOperationException($"Duplicate task worker: TaskType = '{descriptor.TaskType}', ProccesType = '{descriptor.ProccesType}'");
                }
                this._descriptors.Add(descriptor.Key, descriptor);

                if (descriptor.IsAutoTask)
                {
                    this._autoTasks.Add(descriptor.Key, (descriptor.TaskType, descriptor.ProccesType));
                }
            }
        }
    }
}
