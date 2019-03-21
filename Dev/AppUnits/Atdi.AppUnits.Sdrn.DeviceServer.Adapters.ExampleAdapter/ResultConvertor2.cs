using Atdi.Contracts.Sdrn.DeviceServer;
using Atdi.DataModels.Sdrn.DeviceServer;
using Atdi.DataModels.Sdrn.DeviceServer.Commands;
using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Adapters.ExampleAdapter
{
    public class ResultConvertor2 : IResultConvertor<Adapter2Result, TestCommand2Result>
    {
        private readonly ILogger _logger;

        public ResultConvertor2(ILogger logger)
        {
            this._logger = logger;

            this._logger.Debug(Contexts.ResultConvertor2, Categories.Ctor, Events.Call);
        }

        public TestCommand2Result Convert(Adapter2Result result, ICommand command)
        {
            //this._logger.Debug(Contexts.ResultConvertor2, Categories.Converting, Events.ConvertFromTo.With("Adapter2Type", "Command2ResultType"));

            var commandResult = new TestCommand2Result(result.PartIndex, result.Status)
            {
                Value = (double)result.Value
            };

            return commandResult;
        }
    }
}
