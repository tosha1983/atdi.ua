using System;
using System.Threading;
using Atdi.Platform.Logging;

namespace Atdi.Platform.Workflows
{
    internal abstract class JobWorker
    {
        internal sealed class  JobToken : IJobToken
        {
            public JobToken(JobDefinition definition)
            {
                Definition = definition;
                Instance = Guid.NewGuid();
                Status = JobExecutionStatus.Created;
                Breaker = new CancellationTokenSource();
            }

            public Guid Instance { get; }

            public JobExecutionStatus Status { get; set; }

            public long StartAttempts { get; set; }

            public long RecoveryAttempts { get; set; }

            public CancellationTokenSource Breaker { get; }

            public JobDefinition Definition { get; }

            public override string ToString()
            {
                return $"{this.Definition}, Status={this.Status}, Instance='{this.Instance}'";
            }
        }

        private readonly JobDefinition _jobDefinition;
        private readonly ILogger _logger;
        private readonly JobToken _jobToken;
        private readonly JobBroker _broker;
        private readonly IJobExecutorResolver _executorResolver;
        protected readonly JobExecutionContext _executionContext;
        private Thread _thread;

        protected JobWorker(JobDefinition jobDefinition, JobBroker broker, IJobExecutorResolver executorResolver, ILogger logger)
        {
            this._jobDefinition = jobDefinition;
            this._broker = broker;
            this._executorResolver = executorResolver;
            this._logger = logger;
            this._jobToken = new JobToken(_jobDefinition);
            this._executionContext = new JobExecutionContext(this._jobToken);
        }

        private void StartExecution()
        {
            this._thread = new Thread(this.Process)
            {
                Name = $"ATDI.JobWorker.Execution: {_jobDefinition.Name} - {_jobToken.Instance}"
            };
            this._thread.Start();
        }

        private void Process()
        {
            _logger.Verbouse("JobWorker", "Processing", $"The job thread started: Name='{Thread.CurrentThread.Name}', {_jobToken}");

            bool loop;
            var delay = _jobDefinition.StartDelay;
            do
            {
                loop = false;
                try
                {
                    if (delay != null)
                    {
                        Thread.Sleep(delay.Value);
                        delay = null;
                    }

                    _executionContext.CancellationToken.ThrowIfCancellationRequested();

                    _jobToken.Status = JobExecutionStatus.Running;
                    ++_jobToken.StartAttempts;

                    var result = this.Execute();

                    if (result == JobExecutionResult.Completed)
                    {
                        _jobToken.Status = JobExecutionStatus.Completed;
                        loop = _jobDefinition.Repeatable;
                        if (loop)
                        {
                            delay = _jobDefinition.RepeatDelay;
                            _executionContext.IsRecovery = false;
                            _executionContext.IsRepeat = true;
                        }
                    }
                    else if (result == JobExecutionResult.Canceled)
                    {
                        _jobToken.Status = JobExecutionStatus.Canceled;
                        //loop = false;
                    }
                    else if (result == JobExecutionResult.Failure)
                    {
                        _jobToken.Status = JobExecutionStatus.Failure;
                        loop = _jobDefinition.Recoverable;
                        if (loop)
                        {
                            ++_jobToken.RecoveryAttempts;
                            _executionContext.IsRecovery = true;
                            _executionContext.IsRepeat = false;
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    _jobToken.Status = JobExecutionStatus.Canceled;
                    loop = false;
                }
                catch (ThreadAbortException e)
                {
                    Thread.ResetAbort();
                    this._logger.Critical("JobWorker", "Processing", "Abort the thread of the Job Processing. Please fix the problem and restart the service.", e);

                    _jobToken.Status = JobExecutionStatus.Canceled;
                    loop = false;
                }
                catch (Exception e)
                {
                    _logger.Exception("JobWorker", "Processing", e, this);

                    _jobToken.Status = JobExecutionStatus.Failure;
                    if (_jobDefinition.Recoverable)
                    {
                        loop = true;
                        ++_jobToken.RecoveryAttempts;
                        _executionContext.IsRecovery = true;
                        _executionContext.IsRepeat = false;
                    }
                }

            } while (loop);

            _logger.Verbouse("JobWorker", "Processing", $"The job thread finished: Name='{Thread.CurrentThread.Name}', {_jobToken}");

            _broker.OnFinishJobProcessing(this);
            _thread = null;
        }

        protected abstract JobExecutionResult Execute();


        public IJobToken Run()
        {
            this.StartExecution();
            return _jobToken;
        }

        public void Cancel()
        {
            if (_thread != null)
            {
                this._jobToken.Breaker.Cancel();
            }
        }

        public override string ToString()
        {
            return $"{_jobDefinition}, {_jobToken}";
        }
    }

    internal sealed class JobWorker<TExecutor> : JobWorker
        where TExecutor : IJobExecutor
    {

        private readonly IJobExecutor _executor;

        public JobWorker(JobDefinition jobDefinition, JobBroker broker, IJobExecutorResolver executorResolver, ILogger logger)
            : base(jobDefinition, broker, executorResolver, logger)
        {
            this._executor = executorResolver.Resolve<TExecutor>();
        }

        protected override JobExecutionResult Execute()
        {
            return _executor.Execute(_executionContext);
        }
    }

    internal sealed class JobWorker<TExecutor, TState> : JobWorker
        where TExecutor : IJobExecutor<TState>
    {
        
        private readonly TState _jobState;
        private readonly IJobExecutor<TState> _executor;

        public JobWorker(JobDefinition jobDefinition, TState jobState, JobBroker broker, IJobExecutorResolver executorResolver, ILogger logger) 
            : base(jobDefinition, broker, executorResolver,  logger)
        {
            this._jobState = jobState;
            this._executor = executorResolver.Resolve<TExecutor, TState>();
        }


        protected override JobExecutionResult Execute()
        {
            return _executor.Execute(_executionContext, _jobState);
        }
    }
}
