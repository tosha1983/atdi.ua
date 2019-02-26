using Atdi.DataModels.Sdrns.BusMessages.Server;
using DM = Atdi.DataModels.Sdrns.Device;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Platform.Logging;
using Atdi.Contracts.Api.Sdrn.MessageBus;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Messaging.Handlers
{
    class SendCommandHandler : MessageHandlerBase<DM.DeviceCommand, SendCommandMessage>
    {
        private readonly ILogger _logger;

        public SendCommandHandler(ILogger logger)
        {
            this._logger = logger;
        }

        public override void OnHandle(IReceivedMessage<DM.DeviceCommand> message)
        {
            _logger.Verbouse(Contexts.ThisComponent, Categories.Handling, Events.MessageIsBeingHandled.With(message.Token.Type));
            message.Result = MessageHandlingResult.Confirmed;
        }
    }
}
