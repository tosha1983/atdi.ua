using Atdi.Contracts.Api.Sdrn.MessageBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DM = Atdi.DataModels.Sdrns.Device;

namespace Atdi.Test.Api.Sdrn.Device.BusController.Handlers
{
    class SendCommandHandler : MessageHandlerBase<DM.DeviceCommand>
    {
        private readonly IBusGate _gate;

        public SendCommandHandler(IBusGate gate)
            : base("SendCommand")
        {
            this._gate = gate;
        }

        public override void OnHandle(IReceivedMessage<DM.DeviceCommand> message)
        {
            Console.WriteLine($"Recieved command '{message.Data.Command}'");

            message.Result = MessageHandlingResult.Confirmed;
            message.ReasonFailure = "Some reason of send command";
        }
    }
}
