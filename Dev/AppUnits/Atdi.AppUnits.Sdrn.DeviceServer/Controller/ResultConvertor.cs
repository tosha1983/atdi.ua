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
    class ResultConvertor : IResultConvertor
    {
        private readonly ILogger _logger;
        private readonly object _instance;
        private readonly ResultConvertorDecriptor _decriptor;

        public ResultConvertor(ResultConvertorDecriptor descriptor, object instance, ILogger logger)
        {
            this._logger = logger;
            this._decriptor = descriptor;
            this._instance = instance;
        }

        public ICommandResultPart Convert(ICommandResultPart result, ICommand command)
        {
            try
            {
                using (this._logger.StartTrace(Contexts.ResultConvertor, Categories.Converting, TraceScopeNames.ConvertingResult.With(command.Id, this._decriptor.FromType, this._decriptor.ResultType, result.PartIndex, result.Status)))
                {
                    return this._decriptor.Invoker(this._instance, result, command);
                }
            }
            catch (Exception e)
            {
                _logger.Exception(Contexts.ResultConvertor, Categories.Converting, Events.ConvertingResultError.With(this._decriptor.FromType, this._decriptor.ResultType, result.PartIndex, result.Status), e);
                throw new InvalidOperationException($"Failed to finish converting part of results: FromType = '{this._decriptor.FromType}', ResultType = '{this._decriptor.ResultType}', PartIndex = '{result.PartIndex}', Status = '{result.Status}'", e);
            }
        }
    }
}
