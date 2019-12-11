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
using Atdi.DataModels.Sdrn.DeviceServer.TestCommands;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Processing.Example.ResultHandlers
{
    public class TraceCommandResultHandler : IResultHandler<TraceCommand, TraceCommandResult, TraceTask, TraceProcess>
    {
        private readonly IWorkScheduler _workScheduler;
        private readonly ILogger _logger;

        public TraceCommandResultHandler(IWorkScheduler workScheduler, ILogger logger)
        {
            this._workScheduler = workScheduler;
            this._logger = logger;

            this._logger.Debug(Contexts.TraceCommandResultHandler, Categories.Ctor, Events.Call);
        }

        public void Handle(TraceCommand command, TraceCommandResult result, ITaskContext<TraceTask, TraceProcess> taskContext)
        {
            //this._logger.Debug(Contexts.TraceCommandResultHandler, Categories.Handle, Events.HandlingResult.With(result.PartIndex, result.Status));

            if (result.Status == CommandResultStatus.Final)
            {
                //taskContext.Task.Timer.Stop();
                Interlocked.Increment(ref taskContext.Process.CommandCount);
                //this._logger.Info(Contexts.TraceCommandResultHandler, Categories.Handle, $"Duration (Trace command): {taskContext.Task.Timer.Elapsed.TotalMilliseconds}");

                // обрабатываем результат
                

                //taskContext.Task.Timer.Restart();
               // taskContext.SetEvent(result);


                // после выходя из процедуры объект результата будет возвращен в буфер
            }
        }

       
    }
}
