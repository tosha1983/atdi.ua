using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.DeviceServer
{
    public interface ICommandDescriptor
    {
        ICommand Command { get; }

        Type ResultType { get; }

        IProcessingContext Context { get; }

        CancellationToken CancellationToken { get; }

        Action<IProcessingContext, ICommand, CommandFailureReason, Exception> FailureAction { get; }
    }
}
