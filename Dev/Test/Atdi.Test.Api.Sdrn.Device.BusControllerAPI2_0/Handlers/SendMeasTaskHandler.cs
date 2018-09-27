using Atdi.Contracts.Api.Sdrn.MessageBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DM = Atdi.DataModels.Sdrns.Device;

namespace Atdi.Test.Api.Sdrn.Device.BusControllerAPI2_0
{
    class SendMeasTaskHandler : MessageHandlerBase<DM.MeasTask>
    {
        private readonly IBusGate _gate;

        public SendMeasTaskHandler(IBusGate gate)
            : base("SendMeasTask")
        {
            this._gate = gate;
        }

        public override void OnHandle(IReceivedMessage<DM.MeasTask> message)
        {
            Console.WriteLine($"Recieved meas task with ID = '{message.Data.TaskId}'");

            message.Result = MessageHandlingResult.Confirmed;
            message.ReasonFailure = "Some reason od send meas task";
        }
    }
}
