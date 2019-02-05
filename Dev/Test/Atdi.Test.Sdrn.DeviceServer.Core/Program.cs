﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Atdi.DataModels.Sdrn.DeviceServer;
using Atdi.DataModels.Sdrn.DeviceServer.Commands;
using Atdi.DataModels.Sdrn.DeviceServer.Commands.Results;

using Atdi.Contracts.Sdrn.DeviceServer;

namespace Atdi.Test.Sdrn.DeviceServer.Core
{
    class Program
    {
        static void Main(string[] args)
        {
            IController controller = null;
            IProcessingContext context = null;

            var command = new MesureGpsLocationExampleCommand();

            var source = new CancellationTokenSource();

            controller.SendCommand<MesureGpsLocationExampleResult>(context, command);
            controller.SendCommand<MesureGpsLocationExampleResult>(context, command, source.Token);
            controller.SendCommand<MesureGpsLocationExampleResult>(context, command, onFailureAction);
            controller.SendCommand<MesureGpsLocationExampleResult>(context, command, source.Token, onFailureAction);

            source.Cancel();
        }

        static void onFailureAction(CommandFailureReason failureReason, Exception exception)
        {
            return;
        }
    }
}
