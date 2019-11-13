using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Atdi.Platform.Logging;

namespace Atdi.Platform.Workflows
{
    public sealed class JobBroker : LoggedObject, IJobBroker, IDisposable
    {
        private readonly IJobExecutorResolver _executorResolver;
        private readonly ConcurrentDictionary<JobWorker, JobWorker> _workers;

        public JobBroker(IJobExecutorResolver executorResolver, ILogger logger) : base(logger)
        {
            _executorResolver = executorResolver;
            _workers = new ConcurrentDictionary<JobWorker, JobWorker>();
        }

        public IJobToken Run<TExecutor>(JobDefinition<TExecutor> definition) where TExecutor : IJobExecutor
        {
            try
            {
                var worker = new JobWorker<TExecutor>(definition, this, _executorResolver, Logger);
                _workers.TryAdd(worker, worker);
                return worker.Run();
            }
            catch (Exception e)
            {
                Logger.Exception("JobBroker", "Running", e, this);
                throw new InvalidOperationException($"Could not run job: {definition}", e);
            }
        }

        public IJobToken Run<TExecutor, TState>(JobDefinition<TExecutor, TState> definition, TState state) where TExecutor : IJobExecutor<TState>
        {
            try
            {
                var worker = new JobWorker<TExecutor, TState>(definition, state, this, _executorResolver, Logger);
                _workers.TryAdd(worker, worker);
                return worker.Run();
            }
            catch (Exception e)
            {
                Logger.Exception("JobBroker", "Running", e, this);
                throw new InvalidOperationException($"Could not run job: {definition}", e);
            }
        }

        internal void OnFinishJobProcessing(JobWorker worker)
        {
            _workers.TryRemove(worker, out worker);
        }

        public void Dispose()
        {
            var workers = _workers.Values.ToArray();
            foreach (var jobWorker in workers)
            {
                try
                {
                    jobWorker.Cancel();
                }
                catch (Exception e)
                {
                    Logger.Exception("JobBroker", "Running", e, this);
                    throw new InvalidOperationException($"Could not cancel job: {jobWorker}", e);
                }
            }
        }
    }
}
