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

namespace Atdi.AppUnits.Sdrn.DeviceServer.Processing.Example.ResultHandlers
{
    public class Test1CommandResultHandler : IResultHandler<Test1Command, Test1CommandResult, Test1Task, Test1Process>
    {
        private readonly IWorkScheduler _workScheduler;
        private readonly ILogger _logger;

        public Test1CommandResultHandler(IWorkScheduler workScheduler, ILogger logger)
        {
            this._workScheduler = workScheduler;
            this._logger = logger;

            this._logger.Debug(Contexts.Test1CommandResultHandler, Categories.Ctor, Events.Call);
        }

        public void Handle(Test1Command command, Test1CommandResult result, ITaskContext<Test1Task, Test1Process> taskContext)
        {
            this._logger.Debug(Contexts.Test1CommandResultHandler, Categories.Handle, Events.HandlingResult.With(result.PartIndex, result.Status));

            if (result.Status == CommandResultStatus.Ragged)
            {
                taskContext.Process.IsDone = true;
                //taskContext.SetEvent<int>(-1);
            }
            else
            {
                taskContext.Process.TotalValues.Add(result.Value);
                _workScheduler.Run($"Part ID #{result.PartIndex}", () => this.Action((int)result.PartIndex, taskContext, command));

                if (result.Status == CommandResultStatus.Final)
                {
                    taskContext.Process.IsDone = true;
                    //taskContext.SetEvent<int>((int)result.PartIndex);
                }
                else
                {
                    //taskContext.SetEvent<int>((int)result.PartIndex);
                    Thread.Sleep(command.Parameter.ResultDelay);
                }

            }
            
        }

        private void Action(int partId, ITaskContext<Test1Task, Test1Process> context, Test1Command command)
        {
            for (int i = 1; i <= command.Parameter.Count2; i++)
            {
                var @event = $"PartId = [{partId}][{i}]";
                //this._logger.Debug(Contexts.Test1CommandResultHandler, Categories.Handle, $"Set event: {@event}");
                context.SetEvent(@event);

                Thread.Sleep(5);
            }

            this._logger.Debug(Contexts.Test1CommandResultHandler, Categories.Handle, $"Set events: PartId = [{partId}]");
        }
    }
}
