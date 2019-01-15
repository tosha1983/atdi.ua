using Atdi.Contracts.Api.Sdrn.MessageBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DM = Atdi.AppServer.Contracts.Sdrns;



namespace Atdi.AppUnits.Sdrn.ControlA.Handlers
{
    class SendMeasSdrTaskHandler : MessageHandlerBase<List<Atdi.AppServer.Contracts.Sdrns.MeasSdrTask>>
    {
        private readonly IBusGate _gate;

        public SendMeasSdrTaskHandler(IBusGate gate)
            : base("SendMeasSdrTask")
        {
            this._gate = gate;
        }

        public override void OnHandle(IReceivedMessage<List<Atdi.AppServer.Contracts.Sdrns.MeasSdrTask>> message)
        {
            if (message.Data != null)
            {
                for (int i = 0; i < message.Data.Count; i++)
                {
                    Console.WriteLine($"Recieved meas task with ID = '{message.Data[i].Id}'");
                    LoadDataMeasTask loadTask = new LoadDataMeasTask();
                    SaveMeasTaskSDR svTsk = new SaveMeasTaskSDR();
                    DM.MeasSdrTask mtSDR = message.Data[i];
                    {
                        var measTaskFind = (loadTask.FindMeasTaskSDR(mtSDR.SensorId.Value, mtSDR.MeasTaskId.Value, mtSDR.MeasSubTaskStationId, mtSDR.MeasSubTaskId.Value));
                        if (measTaskFind.Count == 0)
                        {
                            mtSDR.NumberScanPerTask = -999;
                            svTsk.CreateNewMeasTaskSDR(message.Data[i]);
                            message.Result = MessageHandlingResult.Confirmed;
                        }
                        else
                        {
                            DM.MeasSdrTask sdrFindTask = measTaskFind.Find(v => v.SensorId.Value == mtSDR.SensorId.Value && v.MeasTaskId.Value == mtSDR.MeasTaskId.Value && v.MeasSubTaskStationId == mtSDR.MeasSubTaskStationId && v.MeasSubTaskId.Value == mtSDR.MeasSubTaskId.Value);
                            sdrFindTask.NumberScanPerTask = -999;
                            sdrFindTask.status = mtSDR.status;
                            SaveMeasSDRResults.SaveStatusMeasTaskSDR(sdrFindTask);
                            message.Result = MessageHandlingResult.Confirmed;
                        }
                    }
                }
            }
        }
    }
}
