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
    class DeviceSelector : IDeviceSelector
    {
        private readonly ICommandsHost _commandsHost;
        private readonly ILogger _logger;

        public DeviceSelector(ICommandsHost commandsHost, ILogger logger)
        {
            this._commandsHost = commandsHost;
            this._logger = logger;
        }

        public IDevice Select(ICommandDescriptor descriptor)
        {
            var decriptorImpl = descriptor as CommandDescriptor;
            var devices = _commandsHost.GetDevices(decriptorImpl.CommandType);

            for (int i = 0; i < devices.Length; i++)
            {
                var device = devices[i];

                if (device.State == DeviceState.Available)
                {
                    if (device.CheckAbilityToExecute(descriptor))
                    {
                        decriptorImpl.Device = device;
                        return device;
                    }
                }
            }
            return null;
        }
    }
}
