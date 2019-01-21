using Atdi.Contracts.Api.Sdrn.MessageBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DM = Atdi.DataModels.Sdrns.Device;

namespace Atdi.Test.Api.Sdrn.Device.BusControllerAPI2_0
{
    class SendRegistrationResultHandler : MessageHandlerBase<DM.SensorRegistrationResult>
    {
        private readonly IBusGate _gate;

        public SendRegistrationResultHandler(IBusGate gate)
            : base("SendRegistrationResult")
        {
            this._gate = gate;
        }

        public override void OnHandle(IReceivedMessage<DM.SensorRegistrationResult> message)
        {
            Console.WriteLine($"{message.CorrelationToken}: Recieved registration info '{message.Data.Status}'");
            if (message.Data.Status == "Success")
            {
                // подтверждаем обработку
                message.Result = MessageHandlingResult.Confirmed;
            }
            else
            {
                message.Result = MessageHandlingResult.Reject;
            }
        }
    }
}
