using Atdi.Contracts.Api.Sdrn.MessageBus;
using Atdi.Contracts.Sdrn.DeviceServer;
using Atdi.DataModels.Sdrn.DeviceServer.Processing;
using Atdi.DataModels.Sdrns.BusMessages.Server;
using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DM = Atdi.DataModels.Sdrns.Device;
using Atdi.DataModels.EntityOrm;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Messaging.Handlers
{
    class SendSensorUpdatingResultHandler : MessageHandlerBase<DM.SensorUpdatingResult, SendSensorUpdatingResultMessage>
    {
        private readonly IProcessingDispatcher _processingDispatcher;
        private readonly ITimeService _timeService;
        private readonly ITaskStarter _taskStarter;
        private readonly ILogger _logger;

        public SendSensorUpdatingResultHandler(ITimeService timeService, IProcessingDispatcher processingDispatcher, ITaskStarter taskStarter, ILogger logger)
        {
            this._processingDispatcher = processingDispatcher;
            this._timeService = timeService;
            this._taskStarter = taskStarter;
            this._logger = logger;
        }

        public override void OnHandle(IReceivedMessage<DM.SensorUpdatingResult> message)
        {
            _logger.Verbouse(Contexts.ThisComponent, Categories.Handling, Events.MessageIsBeingHandled.With(message.Token.Type));
            try
            {
                DM.SensorUpdatingResult sensorRegistrationResult = message.Data;
                message.Result = MessageHandlingResult.Confirmed;
            }
            catch (Exception e)
            {
                message.Result = MessageHandlingResult.Error;
                this._logger.Error(Contexts.ThisComponent, Exceptions.UnknownErrorsInSendUpdatingResultHandler, e.Message);
            }
        }
    }
}