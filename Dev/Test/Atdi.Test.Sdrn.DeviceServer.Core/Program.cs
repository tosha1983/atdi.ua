using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Atdi.DataModels.Sdrn.DeviceServer;
using Atdi.DataModels.Sdrn.DeviceServer.Commands;
using Atdi.DataModels.Sdrn.DeviceServer.Commands.Results;

using Atdi.Contracts.Sdrn.DeviceServer;
using Atdi.Common;

namespace Atdi.Test.Sdrn.DeviceServer.Core
{
    class Program
    {
        static void Main(string[] args)
        {
            IController controller = null;
            //IProcess context = null;

            var command = new MesureGpsLocationExampleCommand();

            command.StartTimeStamp = TimeStamp.Milliseconds;
            command.Options = CommandOption.StartDelayed | CommandOption.StartImmediately;

            var source = new CancellationTokenSource();
            
            controller.SendCommand<MesureGpsLocationExampleResult>(null, command);
            controller.SendCommand<MesureGpsLocationExampleResult>(null, command, source.Token);
            controller.SendCommand<MesureGpsLocationExampleResult>(null, command, onFailureAction);
            controller.SendCommand<MesureGpsLocationExampleResult>(null, command, source.Token, onFailureAction);

            source.Cancel();
        }

        static void onFailureAction(ITaskContext context, ICommand command, CommandFailureReason failureReason, Exception exception)
        {
            return;
        }
    }
}
