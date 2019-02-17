using Atdi.Common;
using Atdi.Contracts.Sdrn.DeviceServer;
using Atdi.DataModels.Sdrn.DeviceServer;
using Atdi.DataModels.Sdrn.DeviceServer.Commands;
using Atdi.DataModels.Sdrn.DeviceServer.Commands.Results;
using Atdi.DataModels.Sdrn.DeviceServer.Processing;
using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Processing.Example
{
    class ExampleTaskWorker : ITaskWorker<ExampleTask, ExampleProcess, PerThreadTaskWorkerLifetime>
    {
        private readonly IController _controller;
        private readonly ILogger _logger;

        public ExampleTaskWorker(IController controller, ILogger logger)
        {
            this._controller = controller;
            this._logger = logger;
        }

        public void Run(ITaskContext<ExampleTask, ExampleProcess> context)
        {
            try
            {
                var deviceCommand = new MesureTraceCommand()
                {
                    Options = CommandOption.StartDelayed,
                    Delay = 1000,
                    StartTimeStamp = TimeStamp.Milliseconds,
                    Timeout = (long)TimeSpan.FromSeconds(2).TotalMilliseconds
                };

                this._controller.SendCommand<MesureTraceResult>(context, deviceCommand);

                // ждем результатов - тут поток засыпает пока не поступит евент указанного типа
                // послать евент можно через метод SetEvent() из потока обработки результатов когда они будут готовы 
                var result = context.WaitEvent<object>();


                if (context.Token.IsCancellationRequested)
                {
                    context.Cancel();
                    return;
                }

                this._controller.SendCommand<MesureTraceResult>(context, deviceCommand);

                /// теперь шлем задаче родителю евент
                context.Descriptor.Parent.SetEvent(result);
                context.Finish();
            }
            catch (Exception e)
            {
                context.Abort(e);
            }
        }
    }
}
