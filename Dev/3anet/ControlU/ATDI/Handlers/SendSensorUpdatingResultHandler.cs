using Atdi.Contracts.Api.Sdrn.MessageBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DM = Atdi.DataModels.Sdrns.Device;

namespace Atdi.Test.Api.Sdrn.Device.BusControllerAPI2_0
{
    class SendSensorUpdatingResultHandler : MessageHandlerBase<DM.SensorUpdatingResult>
    {
        private readonly IBusGate _gate;

        public SendSensorUpdatingResultHandler(IBusGate gate)
            : base("SendSensorUpdatingResult")
        {
            this._gate = gate;
        }

        public override void OnHandle(IReceivedMessage<DM.SensorUpdatingResult> message)
        {
            Console.WriteLine($"{message.CorrelationToken}: Recieved update sensor info '{message.Data.Status}'");
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
