using Atdi.Contracts.Api.Sdrn.MessageBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DM = Atdi.DataModels.Sdrns.Device;

namespace Atdi.Test.Api.Sdrn.Device.BusControllerAPI2_0
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
            /*
             На текущий момент определены такие команды:
             SendMeasResultsConfirmed - команда подтверждения успешного принятия результатов на стороне сервиса SDRN
             SendEntityPartResult - команада подтверждения успешного принятия объекта EntityPart на стороне сервиса SDRN
             SendEntityResult - команада подтверждения успешного принятия объета Entity на стороне сервиса SDRN
             */
            Console.WriteLine($"Recieved command '{message.Data.Command}'");
            if ((message.Data.Command == "SendMeasResultsConfirmed") ||
                (message.Data.Command == "SendEntityPartResult") || 
                (message.Data.Command == "SendEntityResult"))
            {
                message.Result = MessageHandlingResult.Confirmed;
            }
            else
            {
                message.Result = MessageHandlingResult.Reject;
            }
        }
    }
}
