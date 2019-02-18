using Atdi.Contracts.Sdrn.DeviceServer;
using Atdi.DataModels.Sdrn.DeviceServer;
using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Controller
{
    class ResultHandler : IResultHandler
    {
        private readonly ILogger _logger;
        private readonly object _instance;
        private readonly ResultHandlerDecriptor _decriptor;

        public ResultHandler(ResultHandlerDecriptor descriptor, object instance, ILogger logger)
        {
            this._logger = logger;
            this._decriptor = descriptor;
            this._instance = instance;
        }

        public void Handle(ICommand command, ICommandResultPart result, ITaskContext taskContext)
        {
            try
            {
                using (this._logger.StartTrace(Contexts.ResultHandler, Categories.Handling, TraceScopeNames.HandlingResult.With(command.Id, this._decriptor.CommandType, this._decriptor.ResultType, result.PartIndex, result.Status)))
                {
                    _decriptor.Invoker(this._instance, command, result, taskContext);
                }  
            }
            catch (Exception e)
            {
                _logger.Exception(Contexts.ResultHandler, Categories.Handling, Events.HandlingResultError.With(this._decriptor.CommandType, this._decriptor.ResultType, result.PartIndex, result.Status), e);
                throw new InvalidOperationException($"Failed to finish processing part of results: CommandType = '{this._decriptor.CommandType}', ResultType = '{this._decriptor.ResultType}', PartIndex = '{result.PartIndex}', Status = '{result.Status}'", e);
            }
        }
    }
}
