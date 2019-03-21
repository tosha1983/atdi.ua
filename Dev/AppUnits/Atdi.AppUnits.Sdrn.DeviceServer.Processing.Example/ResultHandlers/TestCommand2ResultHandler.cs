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
    public class TestCommand2ResultHandler : IResultHandler<TestCommand2, TestCommand2Result, Test2Task, Test2Process>
    {
        private readonly IWorkScheduler _workScheduler;
        private readonly IEventWaiter _eventWaiter;
        private readonly ILogger _logger;

        public TestCommand2ResultHandler(IWorkScheduler workScheduler, IEventWaiter eventWaiter, ILogger logger)
        {
            this._workScheduler = workScheduler;
            this._eventWaiter = eventWaiter;
            this._logger = logger;

            this._logger.Debug(Contexts.TestCommand2ResultHandler, Categories.Ctor, Events.Call);
        }

        public void Handle(TestCommand2 command, TestCommand2Result result, ITaskContext<Test2Task, Test2Process> taskContext)
        {
            this._logger.Debug(Contexts.TestCommand2ResultHandler, Categories.Handle, Events.HandlingResult.With(result.PartIndex, result.Status));

            Interlocked.Increment(ref taskContext.Process.Count);

            taskContext.Process.TotalValues.Add(result.Value);

            if (result.Status == CommandResultStatus.Ragged)
            {
                taskContext.Process.IsDone = true;
            }
            else
            {
                
                if (result.Status == CommandResultStatus.Final)
                {
                    taskContext.Process.IsDone = true;

                    if (command.Index == command.LastIndex) //0)
                    {
                        this._eventWaiter.Emit<TestCommand2>();
                    }
                }
                else
                {
                    //Thread.Sleep(5);
                }

            }

        }

        
    }
}
