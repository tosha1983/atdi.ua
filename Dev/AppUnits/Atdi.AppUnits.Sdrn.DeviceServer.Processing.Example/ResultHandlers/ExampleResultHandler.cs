using Atdi.Contracts.Sdrn.DeviceServer;
using Atdi.DataModels.Sdrn.DeviceServer.Commands;
using Atdi.DataModels.Sdrn.DeviceServer.Commands.Results;
using Atdi.DataModels.Sdrn.DeviceServer.Processing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Processing.Example.ResultHandlers
{
    class ExampleResultHandler : IResultHandler<MesureTraceCommand, MesureTraceResult, ExampleTask, ExampleProcess>
    {
        public void Handle(MesureTraceCommand command, MesureTraceResult result, DataModels.Sdrn.DeviceServer.ITaskContext<ExampleTask, ExampleProcess> taskContext)
        {

            taskContext.Process.TaskId += result.Level;
            taskContext.Task.ExampleValue1 +=

        }
    }
}
