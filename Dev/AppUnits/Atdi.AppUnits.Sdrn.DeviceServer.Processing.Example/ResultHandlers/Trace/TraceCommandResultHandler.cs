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
using Atdi.AppUnits.Sdrn.DeviceServer.Processing.Example.TaskWorkers;
using Atdi.DataModels.Sdrn.DeviceServer.TestCommands;
using Atdi.Platform.Data;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Processing.Example.ResultHandlers
{
    public class TraceCommandResultHandler : IResultHandler<TraceCommand, TraceCommandResult, TraceTask, TraceProcess>
    {
        private readonly IWorkScheduler _workScheduler;
        private readonly ILogger _logger;
        private readonly IObjectPoolSite _poolSite;
        private readonly IObjectPool<TraceTaskResultData> _traceResultPool;

        public TraceCommandResultHandler(
            IWorkScheduler workScheduler,
            IObjectPoolSite poolSite,
            ILogger logger)
        {
            this._workScheduler = workScheduler;
            this._logger = logger;
            this._poolSite = poolSite;
            _traceResultPool = this._poolSite.GetPool<TraceTaskResultData>("data");
            this._logger.Debug(Contexts.TraceCommandResultHandler, Categories.Ctor, Events.Call);
        }

        public void Handle(TraceCommand command, TraceCommandResult result, ITaskContext<TraceTask, TraceProcess> taskContext)
        {
            //this._logger.Debug(Contexts.TraceCommandResultHandler, Categories.Handle, Events.HandlingResult.With(result.PartIndex, result.Status));
            try
            {
                if (result.Status == CommandResultStatus.Final)
                {
                    var traceResult = _traceResultPool.Take();

                    //for (int i = 0; i < command.BlockSize; i++)
                    //{
                    //    traceResult.FloatValues[i] = result.ValueAsFloats[i];
                    //    traceResult.DoubleValues[i] = result.ValuesAsDouble[i];
                    //}
                    //Array.Copy(result.ValueAsFloats, traceResult.FloatValues, command.BlockSize);
                    //Array.Copy(result.ValuesAsDouble, traceResult.DoubleValues, command.BlockSize);

                    traceResult.CommandId = command.Id;
                    traceResult.TaskId = taskContext.Task.Id;

                    //taskContext.Task.Timer.Stop();
                    Interlocked.Increment(ref taskContext.Process.CommandCount);
                    //this._logger.Info(Contexts.TraceCommandResultHandler, Categories.Handle, $"Duration (Trace command): {taskContext.Task.Timer.Elapsed.TotalMilliseconds}");

                    // обрабатываем результат


                    //taskContext.Task.Timer.Restart();
                    taskContext.SetEvent(traceResult);


                    // после выходя из процедуры объект результата будет возвращен в буфер
                }
            }
            catch (Exception e)
            {
                this._logger.Exception( Contexts.TraceCommandResultHandler, Categories.Handle, e, this);
            }
            
        }

       
    }
}
