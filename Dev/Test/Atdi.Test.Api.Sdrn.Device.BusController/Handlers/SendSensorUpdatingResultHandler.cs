using Atdi.Contracts.Api.Sdrn.MessageBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DM = Atdi.DataModels.Sdrns.Device;
using Atdi.DataModels.Sdrns.Device;


namespace Atdi.Test.Api.Sdrn.Device.BusController.Handlers
{
   
    class SendSensorUpdatingResultHandler : MessageHandlerBase<DM.SensorUpdatingResult>
    {
        private readonly IBusGate _gate;
        private readonly int _index;

        public SendSensorUpdatingResultHandler(IBusGate gate, int index)
            : base("SendSensorUpdatingResult")
        {
            this._gate = gate;
            _index = index;
        }

        public override void OnHandle(IReceivedMessage<SensorUpdatingResult> message)
        {
            Console.WriteLine($"{message.CorrelationToken}: Received update sensor info '{message.Data.Status}'");

            message.Result = MessageHandlingResult.Error;
            message.ReasonFailure = "Some reason of send command - #" + _index;
        }


    }
   
}
