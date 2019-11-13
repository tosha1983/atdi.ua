using Atdi.Contracts.Sdrn.DeviceServer;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Atdi.Platform;
using Atdi.Platform.Logging;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Processing
{
    internal class WorkScheduler : IWorkScheduler
    {
        private readonly ILogger _logger;
        private readonly ConcurrentDictionary<Guid, (string workContext, Task task, int delay)> _runningTasks;
        private readonly IStatisticCounter _startedCounter;
        private readonly IStatisticCounter _runningCounter;
        private readonly IStatisticCounter _finishedCounter;

        public WorkScheduler(IStatistics statistics, ILogger logger)
        {
            this._logger = logger;
            this._runningTasks = new ConcurrentDictionary<Guid, (string wrokContext, Task task, int delay)>();

            if (statistics != null)
            {
                this._startedCounter = statistics.Counter(Monitoring.WorkSchedulerStartedKey);
                this._runningCounter = statistics.Counter(Monitoring.WorkSchedulerRunningKey);
                this._finishedCounter = statistics.Counter(Monitoring.WorkSchedulerFinishedKey);
            }
        }

        public Task Run(string workContext, Action action, int delay = 0)
        {
            var task = default(Task);
            this._startedCounter?.Increment();
            this._runningCounter?.Increment();
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
                this._runningCounter?.Decrement();
                this._finishedCounter?.Increment();

                if (this._runningTasks.TryRemove(key, out _))
                {
                    _logger.Verbouse(Contexts.WorkScheduler, Categories.Finishing, Events.WorkTaskHasFinished.With(workContext, key));
                }
            });

            return task;
        }
    }
}
