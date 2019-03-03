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
        private readonly IDevicesHost _devicesHost;
        private readonly IDeviceSelector _deviceSelector;
        private readonly ITimeService _timeService;
        private readonly ILogger _logger;

        public DevicesController(IDevicesHost devicesHost, IDeviceSelector deviceSelector, ITimeService timeService, ILogger logger)
        {
            this._devicesHost = devicesHost;
            this._deviceSelector = deviceSelector;
            this._timeService = timeService;
            this._logger = logger;
        }

        public void Dispose()
        {
            
        }

        public TProperties[] EnsureDevicesProperties<TProperties>(CommandType commandType) where TProperties : IDeviceProperties, new()
        {
            var result = new List<TProperties>();
            var devices = this._devicesHost.GetDevices();

            for (int i = 0; i < devices.Length; i++)
            {
                var device = devices[i];
                var deviceProperties = device.EnsureProperties(commandType);
                if (deviceProperties != null)
                {
                    result.Add((TProperties)deviceProperties);
                }
            }
            return result.ToArray();
        }

        public IReadOnlyDictionary<CommandType, IDeviceProperties[]> GetDevicesProperties()
        {
            var data = new Dictionary<CommandType, List<IDeviceProperties>>();
            var devices = this._devicesHost.GetDevices();

            for (int i = 0; i < devices.Length; i++)
            {
                var device = devices[i];
                var commandTypeEnums = Enum.GetValues(typeof(CommandType));

                for (int j = 0; j < commandTypeEnums.Length; j++)
                {
                    var commandType = (CommandType)commandTypeEnums.GetValue(j);

                    if (!data.TryGetValue(commandType, out List<IDeviceProperties> list))
                    {
                        list = new List<IDeviceProperties>();
                        data.Add(commandType, list);
                    }

                    var deviceProperties = device.EnsureProperties(commandType);
                    if (deviceProperties != null)
                    {
                        list.Add(deviceProperties);
                    }
                }
            }

            return data.ToDictionary(k => k.Key, v => v.Value.ToArray());
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
                else
                {
                    var timer = new Timer(args => this.SendCommandToDevice(descriptor), null, (int)restTimeout, Timeout.Infinite);
                }
                
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
