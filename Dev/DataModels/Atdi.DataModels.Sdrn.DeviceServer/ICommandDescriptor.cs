using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.DeviceServer
{
    public delegate void ControllerFailureAction(ITaskContext taskContext, ICommand command, CommandFailureReason failureReason, Exception exception);

    public interface ICommandDescriptor
    {
        ICommand Command { get; }

        Type ResultType { get; }

        ITaskContext TaskContext { get; }

        CancellationToken CancellationToken { get; }

        ControllerFailureAction FailureAction { get; }
    }
}
