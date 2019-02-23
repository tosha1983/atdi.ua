using Atdi.Contracts.Sdrn.DeviceServer;
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Atdi.Platform.Logging;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Processing
{
    class WorkScheduler : IWorkScheduler
    {
        private readonly ILogger _logger;
        private readonly ConcurrentDictionary<Guid, (string workContext, Task task, int delay)> _runningTasks;

        public WorkScheduler(ILogger logger)
        {
            this._logger = logger;
            this._runningTasks = new ConcurrentDictionary<Guid, (string wrokContext, Task task, int delay)>();
        }

        public Task Run(string workContext, Action action, int delay = 0)
        {
            var task = default(Task);
            if (delay == 0)
            {
                task = Task.Run(action);
            }
            else
            {
                task = Task.Run(() =>
                {
                    Thread.Sleep(delay);
                    action();
                });
            }
            var key = Guid.NewGuid();
            _logger.Verbouse(Contexts.WorkScheduler, Categories.Creating, Events.WorkTaskHasBeenCreated.With(workContext, key));
            this._runningTasks.TryAdd(key, (workContext, task, delay));

            task.ContinueWith(t =>
            {
                if (this._runningTasks.TryRemove(key, out (string workContext, Task task, int delay) data))
                {
                    _logger.Verbouse(Contexts.WorkScheduler, Categories.Finishing, Events.WorkTaskHasFinished.With(workContext, key));
                }
            });

            return task;
        }
    }
}
