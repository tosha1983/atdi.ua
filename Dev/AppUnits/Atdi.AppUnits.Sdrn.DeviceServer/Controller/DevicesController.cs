using Atdi.Common;
using Atdi.Contracts.Sdrn.DeviceServer;
using Atdi.DataModels.Sdrn.DeviceServer;
using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
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
        private ConcurrentDictionary<CommandType, TimerScheduler> _schedulers;

        public DevicesController(IDevicesHost devicesHost, IDeviceSelector deviceSelector, ITimeService timeService, ILogger logger)
        {
            this._devicesHost = devicesHost;
            this._deviceSelector = deviceSelector;
            this._timeService = timeService;
            this._logger = logger;
            this._schedulers = new ConcurrentDictionary<CommandType, TimerScheduler>();
        }

        public void Dispose()
        {
            if (this._schedulers != null)
            {
                foreach (var item in this._schedulers)
                {
                    item.Value.Dispose();
                }
                this._schedulers = null;
            }
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
            try
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
            catch(Exception e)
            {
                _logger.Exception(Contexts.Controller, Categories.Processing, e);
                throw new InvalidOperationException(Exceptions.ErrorOccurredWhileInvokingControllerCommand.With(command.Type, command.Id), e);
            }
        }

        private void PostponeCommand(CommandDescriptor descriptor)
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
                var scheduler = this.GetTimerScheduler(descriptor.Command.Type);
                scheduler.StartAt(command.StartTimeStamp, command.Delay, () => this.SendCommandToDevice(descriptor));
            }
        }

        private TimerScheduler GetTimerScheduler(CommandType commandType)
        {
            if (!this._schedulers.TryGetValue(commandType, out TimerScheduler scheduler))
            {
                scheduler = new TimerScheduler(commandType, _timeService, _logger);
                if (!this._schedulers.TryAdd(commandType, scheduler))
                {
                    if (!this._schedulers.TryGetValue(commandType, out scheduler))
                    {
                        throw new InvalidOperationException("Could not get a timer scheduler by command " + commandType.ToString());
                    }
                }
            }
            return scheduler;
        }

        private void SendCommandToDevice(CommandDescriptor descriptor)
        {
            
            var device = _deviceSelector.Select(descriptor);

            if (device == null)
            {
                descriptor.Reject(CommandFailureReason.NotFoundDevice);
                return;
            }

            // это лишнее все это сделает селектор - он умный
            //if (device.State == DeviceState.Basy)
            //{
            //    descriptor.Reject(CommandFailureReason.DeviceIsBusy);
            //    return;
            //}

            //if (!device.CheckAbilityToExecute(descriptor))
            //{
            //    descriptor.Reject(CommandFailureReason.DeviceIsBusy);
            //    return;
            //}

            if (!device.TryPushCommand(descriptor))
            {
                descriptor.Reject(CommandFailureReason.DeviceIsBusy);
                return;
            }
        }
    }
}
