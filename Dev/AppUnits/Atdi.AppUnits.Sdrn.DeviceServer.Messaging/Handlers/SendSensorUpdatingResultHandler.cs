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

namespace Atdi.AppUnits.Sdrn.DeviceServer.Messaging.Handlers
{
    class SendSensorUpdatingResultHandler : MessageHandlerBase<DM.SensorUpdatingResult, SendSensorUpdatingResultMessage>
    {
        private readonly ILogger _logger;

        public SendSensorUpdatingResultHandler(ILogger logger)
        {
            this._logger = logger;
        }

        public override void OnHandle(IReceivedMessage<DM.SensorUpdatingResult> message)
        {
            _logger.Verbouse(Contexts.ThisComponent, Categories.Handling, Events.MessageIsBeingHandled.With(message.Token.Type));
            message.Result = MessageHandlingResult.Confirmed;
        }
    }
}