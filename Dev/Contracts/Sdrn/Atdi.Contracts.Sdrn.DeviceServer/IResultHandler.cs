using Atdi.DataModels.Sdrn.DeviceServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Contracts.Sdrn.DeviceServer
{
    public interface IResultHandler
    {
        void Handle(ICommand command, ICommandResultPart result, ITaskContext taskContext);
    }

    public interface IResultHandler<TCommand, TResult, TTask, TProcess>
        where TCommand : ICommand
        where TResult : ICommandResultPart
        where TTask : ITask
        where TProcess : IProcess
    {
        void Handle(TCommand command, TResult result, ITaskContext<TTask, TProcess> taskContext);
    }
}
