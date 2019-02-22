﻿using Atdi.Contracts.Sdrn.DeviceServer;
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
    public class ResultConvertor1 : IResultConvertor<Adapter1Result, Test1CommandResult>
    {
        private readonly ILogger _logger;

        public ResultConvertor1(ILogger logger)
        {
            this._logger = logger;

            this._logger.Debug(Contexts.ResultConvertor1, Categories.Ctor, Events.Call);
        }

        public Test1CommandResult Convert(Adapter1Result result, ICommand command)
        {
            this._logger.Debug(Contexts.ResultConvertor1, Categories.Converting, Events.ConvertFromTo.With("Adapter1Type", "CommandResult1Type"));

            var commandResult = new Test1CommandResult(result.PartIndex, result.Status)
            {
                Value = (double)result.Value
            };

            return commandResult;
        }
    }
}
