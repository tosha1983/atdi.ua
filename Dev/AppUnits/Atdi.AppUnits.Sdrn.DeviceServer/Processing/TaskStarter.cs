using Atdi.Contracts.Sdrn.DeviceServer;
using Atdi.DataModels.Sdrn.DeviceServer;
using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Processing
{
    class TaskStarter : ITaskStarter
    {
        private readonly IWorkScheduler _workScheduler;
        private readonly ITaskWorkersHost _workersHost;
        private readonly ILogger _logger;

        public TaskStarter(IWorkScheduler workScheduler, ITaskWorkersHost workersHost, ILogger logger)
        {
            this._workScheduler = workScheduler;
            this._workersHost = workersHost;
            this._logger = logger;
        }

        public void Run(ITask task, IProcess process, ITaskContext parentTaskContext, CancellationToken cancellationToken)
        {
            var descriptor = new TaskDescriptor
            {
                Parent = parentTaskContext,
                Process = process,
                Task = task,
                ProcessType = process.GetType(),
                TaskType = task.GetType(),
                Token = cancellationToken
            };

            this._logger.Verbouse(Contexts.TaskStarter, Categories.Running, Events.TaskIsBeingRunSync.With(descriptor.TaskType, task.Id, process.Name));

            try
            {
                var worker = this._workersHost.GetTaskWorker(descriptor.TaskType, descriptor.ProcessType);

                if ((task.Options & TaskExecutionOption.RunDelayed) == TaskExecutionOption.RunDelayed)
                {
                    if (task.Delay > 0)
                    {
                        Thread.Sleep((int)task.Delay);
                    }
                }

                if (!worker.IsAsync)
                {
                    worker.Run(descriptor);
                }
                else
                {
                    worker.RunAsync(descriptor).GetAwaiter().GetResult();
                }

                this._logger.Verbouse(Contexts.TaskStarter, Categories.Running, Events.TaskHasBeenRunSync.With(descriptor.TaskType, task.Id, process.Name));
            }
            catch (Exception e)
            {
                _logger.Exception(Contexts.TaskStarter, Categories.Running, e);
                var taskBase = task as TaskBase;
                taskBase.ChangeState(TaskState.Rejected);
                throw new InvalidOperationException(Exceptions.ErrorCccurredWhileStartingTask.With(descriptor.TaskType, task.Id, process.Name), e);
            }
            
        }

        public async Task RunAsync(ITask task, IProcess process, ITaskContext parentTaskContext, CancellationToken cancellationToken)
        {
            var descriptor = new TaskDescriptor
            {
                Parent = parentTaskContext,
                Process = process,
                Task = task,
                ProcessType = process.GetType(),
                TaskType = task.GetType(),
                Token = cancellationToken
            };

            this._logger.Verbouse(Contexts.TaskStarter, Categories.Running, Events.TaskIsBeingRunAsync.With(descriptor.TaskType, task.Id, process.Name));

            var worker = this._workersHost.GetTaskWorker(descriptor.TaskType, descriptor.ProcessType);

            if ((task.Options & TaskExecutionOption.RunDelayed) == TaskExecutionOption.RunDelayed)
            {
                if (task.Delay > 0)
                {
                    await Task.Delay((int)task.Delay);
                }
            }

            if (!worker.IsAsync)
            {
                await this._workScheduler.Run($"RunAsync.SyncTask.[{descriptor.TaskType.FullName}]. ({task.Id})", () => worker.Run(descriptor));
            }
            else
            {
                await worker.RunAsync(descriptor);
            }
        }

        public Task RunParallel(ITask task, IProcess process, ITaskContext parentTaskContext, CancellationToken cancellationToken)
        {
            var descriptor = new TaskDescriptor
            {
                Parent = parentTaskContext,
                Process = process,
                Task = task,
                ProcessType = process.GetType(),
                TaskType = task.GetType(),
                Token = cancellationToken
            };

            this._logger.Verbouse(Contexts.TaskStarter, Categories.Running, Events.TaskIsBeingRunParallel.With(descriptor.TaskType, task.Id, process.Name));

            try
            {
                var worker = this._workersHost.GetTaskWorker(descriptor.TaskType, descriptor.ProcessType);

                var result = default(Task);
                var delay = default(int);
                if ((task.Options & TaskExecutionOption.RunDelayed) == TaskExecutionOption.RunDelayed)
                {
                    if (task.Delay > 0)
                    {
                        delay = (int)task.Delay;
                    }
                }

                if (!worker.IsAsync)
                {
                    result = this._workScheduler.Run($"RunParallel.SyncTask.[{descriptor.TaskType.FullName}]. ({task.Id})", () => worker.Run(descriptor), delay);
                }
                else
                {
                    result = this._workScheduler.Run($"RunParallel.AsyncTask.[{descriptor.TaskType.FullName}]. ({task.Id})", () => worker.RunAsync(descriptor).GetAwaiter().GetResult(), delay);
                }

                this._logger.Verbouse(Contexts.TaskStarter, Categories.Running, Events.TaskHasBeenRunParallel.With(descriptor.TaskType, task.Id, process.Name));
                return result;
            }
            catch (Exception e)
            {
                this._logger.Exception(Contexts.TaskStarter, Categories.Running, e);
                var taskBase = task as TaskBase;
                taskBase.ChangeState(TaskState.Rejected);
                throw new InvalidOperationException(Exceptions.ErrorCccurredWhileStartingTask.With(descriptor.TaskType, task.Id, process.Name), e);
            }
            
        }
    }
}
