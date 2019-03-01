using Atdi.Contracts.Sdrn.DeviceServer;
using Atdi.DataModels.Sdrn.DeviceServer;
using Atdi.DataModels.Sdrn.DeviceServer.Processing;
using Atdi.Platform.Logging;
using System;
using Atdi.Contracts.Api.Sdrn.MessageBus;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Processing
{
    public class EventTaskWorker : ITaskWorker<EventTask, BaseContext, SingletonTaskWorkerLifetime>
    {
        private readonly ILogger _logger;

        public EventTaskWorker(ILogger logger)
        {
            this._logger = logger;
        }

        public void Run(ITaskContext<EventTask, BaseContext> context)
        {
            try
            {
                _logger.Verbouse(Contexts.EventTaskWorker, Categories.Processing, Events.StartEventTaskWorker.With(context.Task.Id));

                context.Descriptor.Parent.SetEvent<TaskParameters>(context.Task.taskParameters);

                context.Finish();
            }
            catch (Exception e)
            {
                _logger.Error(Contexts.EventTaskWorker, Categories.Processing, Exceptions.UnknownErrorEventTaskWorker, e.Message);
                context.Abort(e);
            }
        }
    }
}
