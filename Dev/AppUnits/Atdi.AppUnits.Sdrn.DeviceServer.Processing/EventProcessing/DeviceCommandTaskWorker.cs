using Atdi.Contracts.Sdrn.DeviceServer;
using Atdi.DataModels.Sdrn.DeviceServer;
using Atdi.DataModels.Sdrn.DeviceServer.Processing;
using Atdi.Platform.Logging;
using System;
using DM = Atdi.DataModels.Sdrns.Device;
using Atdi.Contracts.Api.Sdrn.MessageBus;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Processing
{
    public class DeviceCommandTaskWorker : ITaskWorker<DeviceCommandTask, BaseContext, SingletonTaskWorkerLifetime>
    {
        private readonly ILogger _logger;

        public DeviceCommandTaskWorker(ILogger logger)
        {
            this._logger = logger;
        }

        public void Run(ITaskContext<DeviceCommandTask, BaseContext> context)
        {
            try
            {
                _logger.Verbouse(Contexts.DeviceCommandTaskWorker, Categories.Processing, Events.StartDeviceCommandTaskWorker.With(context.Task.Id));

                context.Descriptor.Parent.SetEvent<DM.DeviceCommand>(context.Task.deviceCommand);

                context.Finish();
            }
            catch (Exception e)
            {
                _logger.Error(Contexts.DeviceCommandTaskWorker, Categories.Processing, Exceptions.DeviceCommandTaskWorker, e.Message);
                context.Abort(e);
            }
        }
    }
}
