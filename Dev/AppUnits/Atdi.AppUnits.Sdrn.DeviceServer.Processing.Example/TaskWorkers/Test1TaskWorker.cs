using Atdi.Contracts.Sdrn.DeviceServer;
using Atdi.DataModels.Sdrn.DeviceServer;
using Atdi.DataModels.Sdrn.DeviceServer.Commands;
using Atdi.DataModels.Sdrn.DeviceServer.Processing.Test;
using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Processing.Example.TaskWorkers
{
    public class Test1TaskWorker : ITaskWorker<Test1Task, Test1Process, TransientTaskWorkerLifetime>
    {
        private readonly ExampleConfig _config;
        private readonly IController _controller;
        private readonly ITimeService _timeService;
        private readonly ILogger _logger;

        public Test1TaskWorker(ExampleConfig config, IController controller, ITimeService timeService, ILogger logger)
        {
            this._config = config;
            this._controller = controller;
            this._timeService = timeService;
            this._logger = logger;

            this._logger.Debug(Contexts.Test1CommandResultHandler, Categories.Ctor, Events.Call);
        }

        public void Run(ITaskContext<Test1Task, Test1Process> context)
        {
            this._logger.Debug(Contexts.Test1CommandResultHandler, Categories.Run, Events.RunTask.With(context.Task.Id));

            var deviceCommand1 = new Test1Command()
            {
                Timeout = this._timeService.TimeStamp.Milliseconds
            };

            deviceCommand1.Parameter.Count = 100;
            deviceCommand1.Parameter.Count2 = 10;
            deviceCommand1.Parameter.Delay = 2;
            deviceCommand1.Parameter.ResultDelay = 3;

            this._controller.SendCommand<Test1CommandResult>(context, deviceCommand1);
            var isDone = false;
            var c = 0;
            while (!context.Process.IsDone || !isDone)
            {
                if (context.WaitEvent<string>(out string value, 50))
                {
                    //this._logger.Debug(Contexts.Test1CommandResultHandler, Categories.Run, $"Get event: value = '{value}'");
                    c++;
                    if (c == (deviceCommand1.Parameter.Count * deviceCommand1.Parameter.Count2))
                    {
                        isDone = true;
                    }
                }
                
            }

            context.Finish();
        }
    }
}
