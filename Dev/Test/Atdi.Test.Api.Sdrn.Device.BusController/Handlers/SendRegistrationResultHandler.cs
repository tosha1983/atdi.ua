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

    class SendRegistrationResultHandler : MessageHandlerBase<DM.SensorRegistrationResult>
    {
        private readonly IBusGate _gate;
        private readonly int _index;

        public SendRegistrationResultHandler(IBusGate gate, int index)
            : base("SendRegistrationResult")
        {
            this._gate = gate;
            _index = index;
        }

        public override void OnHandle(IReceivedMessage<SensorRegistrationResult> message)
        {
            Console.WriteLine($"{message.CorrelationToken}: Received registration info '{message.Data.Status}'");

            message.Result = MessageHandlingResult.Confirmed;
            message.ReasonFailure = "Some reason of send command - #" + _index;


            using (var publisher = _gate.CreatePublisher("SendRegistrationResultHandler"))
            {
                var sensor = Program.GetSensor(_index);
                publisher.Send("UpdateSensor", sensor, message.CorrelationToken);
            }
        }

    }

}
