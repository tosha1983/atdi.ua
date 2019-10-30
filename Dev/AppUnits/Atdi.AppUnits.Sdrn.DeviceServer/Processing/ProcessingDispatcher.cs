using Atdi.Contracts.Sdrn.DeviceServer;
using Atdi.DataModels.Sdrn.DeviceServer;
using Atdi.Platform.Logging;
using Atdi.Platform;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Processing
{
    internal class ProcessingDispatcher : IProcessingDispatcher
    {
        private readonly ITimeService _timeService;
        private readonly ILogger _logger;
        private readonly IStatisticCounter _startedCounter;
        private readonly IStatisticCounter _runningCounter;
        private readonly IStatisticCounter _finishedCounter;

        public ProcessingDispatcher(ITimeService timeService, IStatistics statistics, ILogger logger)
        {
            this._timeService = timeService;
            this._logger = logger;

            if (statistics != null)
            {
                this._startedCounter = statistics.Counter(Monitoring.ProcessingStartedKey);
                this._runningCounter = statistics.Counter(Monitoring.ProcessingRunningKey);
                this._finishedCounter = statistics.Counter(Monitoring.ProcessingFinishedKey);
            }
        }

        public void Finish(IProcess process)
        {
            this._runningCounter?.Decrement();
            this._finishedCounter?.Increment();
        }

        public TProcess Start<TProcess>(IProcess parentProcess = null) where TProcess : IProcess, new()
        {
            var process = new TProcess();

            this._startedCounter?.Increment();
            this._runningCounter?.Increment();

            var processBase = process as ProcessBase;
            processBase.Parent = parentProcess;
            processBase.TimeStamp = this._timeService.TimeStamp.Milliseconds;
            this._logger.Verbouse(Contexts.ProcessingDispatcher, Categories.Creating, Events.ProcessContextWasCreated.With(process.Name, typeof(TProcess)));
            return process;

        }
    }
}
