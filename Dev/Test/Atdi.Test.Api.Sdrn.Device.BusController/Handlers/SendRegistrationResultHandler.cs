using Atdi.Contracts.Api.Sdrn.MessageBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DM = Atdi.DataModels.Sdrns.Device;
using Atdi.Contracts.Sdrn.Server;
using Atdi.Contracts.Api.EventSystem;
using Atdi.DataModels.Sdrns.Device;
using Atdi.Platform.Logging;


namespace Atdi.Test.Api.Sdrn.Device.BusController.Handlers
{
    class SendRegistrationResultHandler : MessageHandlerBase<DM.SensorRegistrationResult>
    {
        private readonly IBusGate _gate;

        public SendRegistrationResultHandler(IBusGate gate)
            : base("SendRegistrationResult")
        {
            this._gate = gate;
        }

        public override void OnHandle(ISdrnReceivedMessage<SensorRegistrationResult> message)
        {
            Console.WriteLine($"{message.CorrelationToken}: Recieved registration info '{message.Data.Status}'");

            message.Result = MessageHandlingResult.Confirmed;
            message.ReasonFailure = "Some reason of send command";


            using (var publisher = _gate.CreatePublisher("SendRegistrationResultHandler"))
            {
                var sensor = new DM.Sensor
                {
                    Name = "SENSOR-DBD12-A00-1280",
                    Equipment = new DM.SensorEquipment
                    {
                        TechId = "SomeSensor 2.3 SN:00009093"
                    }
                };
                publisher.Send("UpdateSensor", sensor, message.CorrelationToken);
            }
        }

    }
}
