using Atdi.Contracts.Sdrn.DeviceServer;
using Atdi.DataModels.Sdrn.DeviceServer;
using Atdi.Platform.Logging;
using System;
using Atdi.Platform;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Controller
{
    internal class ResultHandler : IResultHandler
    {
        private readonly ILogger _logger;
        private readonly object _instance;
        private readonly ResultHandlerDecriptor _descriptor;
        private readonly IStatisticCounter _processedCounter;
        private readonly IStatisticCounter _processingCounter;
        private readonly IStatisticCounter _completedCounter;
        private readonly IStatisticCounter _abortedCounter;

        public ResultHandler(ResultHandlerDecriptor descriptor, object instance, IStatistics statistics, ILogger logger)
        {
            this._logger = logger;
            this._descriptor = descriptor;
            this._instance = instance;
            if (statistics != null)
            {
                var context = string.Intern($"{descriptor.InstanceType.Name}");
                this._processedCounter = statistics.Counter(Monitoring.DefineAdapterResultHandlerCounter(context, "Processed"));
                this._processingCounter = statistics.Counter(Monitoring.DefineAdapterResultHandlerCounter(context, "Processing"));
                this._completedCounter = statistics.Counter(Monitoring.DefineAdapterResultHandlerCounter(context, "Completed"));
                this._abortedCounter = statistics.Counter(Monitoring.DefineAdapterResultHandlerCounter(context, "Aborted"));

                this.StartedWorkerCounter = statistics.Counter(Monitoring.DefineAdapterResultWorkerCounter(context, "Started"));
                this.RunningWorkerCounter = statistics.Counter(Monitoring.DefineAdapterResultWorkerCounter(context, "Running"));
                this.FinishedWorkerCounter = statistics.Counter(Monitoring.DefineAdapterResultWorkerCounter(context, "Finished"));
                this.AbortedWorkerCounter = statistics.Counter(Monitoring.DefineAdapterResultWorkerCounter(context, "Aborted"));
            }
        }

        public IStatisticCounter AbortedWorkerCounter { get; }

        public IStatisticCounter FinishedWorkerCounter { get;  }

        public IStatisticCounter RunningWorkerCounter { get; }

        public IStatisticCounter StartedWorkerCounter { get; }

        public void Handle(ICommand command, ICommandResultPart result, ITaskContext taskContext)
        {
            try
            {
                //using (this._logger.StartTrace(Contexts.ResultHandler, Categories.Handling, TraceScopeNames.HandlingResult.With(command.Id, this._decriptor.CommandType, this._decriptor.ResultType, result.PartIndex, result.Status)))
                {
                    this._processedCounter?.Increment();
                    this._processingCounter?.Increment();
                    _descriptor.Invoker(this._instance, command, result, taskContext);
                    this._processingCounter?.Decrement();
                    this._completedCounter?.Increment();
                }  
            }
            catch (Exception e)
            {
                this._processingCounter?.Decrement();
                this._abortedCounter?.Increment();
                _logger.Exception(Contexts.ResultHandler, Categories.Handling, Events.HandlingResultError.With(this._descriptor.CommandType, this._descriptor.ResultType, result.PartIndex, result.Status), e);
                throw new InvalidOperationException($"Failed to finish processing part of results: CommandType = '{this._descriptor.CommandType}', ResultType = '{this._descriptor.ResultType}', PartIndex = '{result.PartIndex}', Status = '{result.Status}'", e);
            }
        }
    }
}
