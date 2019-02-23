using Atdi.Common;
using Atdi.Contracts.Sdrn.DeviceServer;
using Atdi.DataModels.Sdrn.DeviceServer;
using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Controller
{
    public class DevicesController : IController
    {
        private readonly IDeviceSelector _deviceSelector;
        private readonly ITimeService _timeService;
        private readonly ILogger _logger;

        public DevicesController(IDeviceSelector deviceSelector, ITimeService timeService, ILogger logger)
        {
            this._deviceSelector = deviceSelector;
            this._timeService = timeService;
            this._logger = logger;
        }

        public void Dispose()
        {
            
        }

        public void SendCommand<TResult>(ITaskContext taskContext, ICommand command, CancellationToken cancellationToken, ControllerFailureAction onFailureAction)
        {
            var descriptor = new CommandDescriptor
            {
                Command = command,
                TaskContext = taskContext,
                CancellationToken = cancellationToken,
                FailureAction = onFailureAction,
                ResultType = typeof(TResult)
            };

            descriptor.ToPending();

            if ((command.Options & CommandOption.StartDelayed) == CommandOption.StartDelayed)
            {
                // post pone command
                this.PostponeCommand(descriptor);
            }
            else
            {
                this.SendCommandToDevice(descriptor);
            }
        }

        private void PostponeCommand(CommandDescriptor descriptor)
        {
            try
            {
                var command = descriptor.Command;
                if (command.Delay <= 0)
                {
                    throw new InvalidOperationException("Delay value not defined");
                }
                if (command.StartTimeStamp <= 0)
                {
                    throw new InvalidOperationException("Start time stamp value not defined");
                }
                // 100, 1000 1050
                var restTimeout = command.Delay - (_timeService.TimeStamp.Milliseconds - command.StartTimeStamp);

                if (restTimeout <= 0)
                {
                    this.SendCommandToDevice(descriptor);
                }

                var timer = new Timer(args => this.SendCommandToDevice(descriptor), null, (int)restTimeout, Timeout.Infinite);
            }
            catch (Exception e)
            {
                descriptor.Reject(CommandFailureReason.Exception, e);
            }
        }

        private void SendCommandToDevice(CommandDescriptor descriptor)
        {
            
            var device = _deviceSelector.Select(descriptor);

            if (device == null)
            {
                descriptor.Reject(CommandFailureReason.NotFoundDevice);
                return;
            }

            
            if (device.State == DeviceState.Basy)
            {
                descriptor.Reject(CommandFailureReason.DeviceIsBusy);
                return;
            }

            if (!device.CheckAbilityToExecute(descriptor))
            {
                descriptor.Reject(CommandFailureReason.DeviceIsBusy);
                return;
            }

            if (!device.TryPushCommand(descriptor))
            {
                descriptor.Reject(CommandFailureReason.DeviceIsBusy);
                return;
            }
        }
    }
}
