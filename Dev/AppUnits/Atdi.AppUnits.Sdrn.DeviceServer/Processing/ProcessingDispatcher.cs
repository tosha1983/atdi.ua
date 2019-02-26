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
    class ProcessingDispatcher : IProcessingDispatcher
    {
        private readonly ITimeService _timeService;
        private readonly ILogger _logger;

        public ProcessingDispatcher(ITimeService timeService, ILogger logger)
        {
            this._timeService = timeService;
            this._logger = logger;
        }

        public void Finish(IProcess process)
        {
            
        }

        public TProcess Start<TProcess>(IProcess parentProcess = null) where TProcess : IProcess, new()
        {
            var process = new TProcess();
            var processBase = process as ProcessBase;
            processBase.Parent = parentProcess;
            processBase.TimeStamp = this._timeService.TimeStamp.Milliseconds;
            this._logger.Verbouse(Contexts.ProcessingDispatcher, Categories.Creating, Events.ProcessContextWasCreated.With(process.Name, typeof(TProcess)));
            return process;

        }
    }
}
