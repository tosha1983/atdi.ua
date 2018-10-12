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
    class SendSensorUpdatingResultHandler : SdrnPrimaryHandlerBase<DM.SensorUpdatingResult>
    {
        private readonly IBusGate _gate;

        public SendSensorUpdatingResultHandler(IBusGate gate)
            : base("SendSensorUpdatingResult")
        {
            this._gate = gate;
        }

        public override void OnHandle(ISdrnReceivedMessage<SensorUpdatingResult> message)
        {
            Console.WriteLine($"{message.CorrelationToken}: Recieved update sensor info '{message.Data.Status}'");

            message.Result = MessageHandlingResult.Confirmed;
            message.ReasonFailure = "Some reason of send command";
        }


    }
}
