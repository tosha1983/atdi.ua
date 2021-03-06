﻿using Atdi.Contracts.Api.Sdrn.MessageBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DM = Atdi.DataModels.Sdrns.Device;
using Atdi.DataModels.Sdrns.Device;


namespace Atdi.Test.Api.Sdrn.Device.BusController.GR.Handlers
{
   
    class SendSensorUpdatingResultHandler : MessageHandlerBase<DM.SensorUpdatingResult>
    {
        private readonly IBusGate _gate;

        public SendSensorUpdatingResultHandler(IBusGate gate)
            : base("SendSensorUpdatingResult")
        {
            this._gate = gate;
        }

        public override void OnHandle(IReceivedMessage<SensorUpdatingResult> message)
        {
            Console.WriteLine($"{message.CorrelationToken}: Recieved update sensor info '{message.Data.Status}'");

            message.Result = MessageHandlingResult.Confirmed;
            message.ReasonFailure = "Some reason of send command";
        }


    }
   
}
