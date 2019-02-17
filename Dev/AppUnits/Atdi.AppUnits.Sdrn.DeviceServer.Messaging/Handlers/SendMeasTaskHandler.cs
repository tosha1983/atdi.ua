using Atdi.Contracts.Api.Sdrn.MessageBus;
using Atdi.Contracts.Sdrn.DeviceServer;
using Atdi.DataModels.Sdrn.DeviceServer.Processing;
using Atdi.DataModels.Sdrns.BusMessages.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DM = Atdi.DataModels.Sdrns.Device;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Messaging.Handlers
{
    class SendMeasTaskHandler : MessageHandlerBase<DM.MeasTask, SendMeasTaskMessage>
    {
        private readonly IProcessingDispatcher _processingDispatcher;
        private readonly ITaskStarter _taskStarter;

        public SendMeasTaskHandler(IProcessingDispatcher processingDispatcher, ITaskStarter taskStarter)
        {
            this._processingDispatcher = processingDispatcher;
            this._taskStarter = taskStarter;
        }

        public override void OnHandle(IReceivedMessage<DM.MeasTask> message)
        {
            var process = this._processingDispatcher.Start<ExampleProcess>();
            var exampleTask = new ExampleTask();

            _taskStarter.Run(exampleTask, process);


            message.Result = MessageHandlingResult.Confirmed;
        }
    }
}
