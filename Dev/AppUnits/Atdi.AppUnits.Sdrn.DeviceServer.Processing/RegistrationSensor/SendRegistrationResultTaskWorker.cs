using Atdi.Common;
using Atdi.Contracts.Sdrn.DeviceServer;
using Atdi.DataModels.Sdrn.DeviceServer;
using Atdi.DataModels.Sdrn.DeviceServer.Processing;
using Atdi.Platform.Logging;
using System;
using DM = Atdi.DataModels.Sdrns.Device;
using Atdi.Contracts.Api.Sdrn.MessageBus;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Processing
{
    /// <summary>
    ///  Воркер для передачи уведомлений типа DM.SensorRegistrationResult в конекст ITaskContext<RegisterSensorTask, BaseContext>
    /// </summary>
    public class SendRegistrationResultTaskWorker : ITaskWorker<SendRegistrationResultTask, BaseContext, SingletonTaskWorkerLifetime>
    {
        private readonly ILogger _logger;

        public SendRegistrationResultTaskWorker(ILogger logger)
        {
            this._logger = logger;
        }

        public void Run(ITaskContext<SendRegistrationResultTask, BaseContext> context)
        {
            try
            {
                _logger.Verbouse(Contexts.SendRegistrationResultTaskWorker, Categories.Processing, Events.StartSendRegistrationResultTaskWorker.With(context.Task.Id));

                context.Descriptor.Parent.SetEvent<DM.SensorRegistrationResult>(context.Task.sensorRegistrationResult);

                context.Finish();
            }
            catch (Exception e)
            {
                _logger.Error(Contexts.SendRegistrationResultTaskWorker, Categories.Processing, Exceptions.UnknownErrorSendRegistrationResultTaskWorker, e.Message);
                context.Abort(e);
            }
        }
    }
}
