using Atdi.Contracts.Api.Sdrn.MessageBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DM = Atdi.DataModels.Sdrns.Device;
using SM = Atdi.AppServer.Contracts.Sdrns;



namespace Atdi.AppUnits.Sdrn.ControlA.Handlers
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
            SensorDBExtension svr = new SensorDBExtension();
            Console.WriteLine($"Recieved command '{message.Data.Command}'");
            if ((message.Data.Command == "SendMeasResultsConfirmed") ||
                (message.Data.Command == "SendEntityPartResult") ||
                (message.Data.Command == "SendEntityResult") ||
                (message.Data.Command == "SendActivitySensorResult"))
            {
                //if (message.Data.Command == "SendActivitySensorResult")
                //{
                    //List<SM.Sensor> L_ser = svr.LoadObjectSensor();
                    //if (L_ser != null)
                    //{
                        //if (L_ser.Count > 0)
                        //{
                           //BusManager._messagePublisher.Send("SendActivitySensor", L_ser[0]);
                           //SensorActivity.cntSeconds = 0;
                        //}
                    //}
                //}
                message.Result = MessageHandlingResult.Confirmed;
            }
            else
            {
                message.Result = MessageHandlingResult.Reject;
            }
        }
    }
}
