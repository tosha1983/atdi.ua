using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.DeviceServer
{
    public interface ITaskDescriptor
    {
        ITaskContext Parent { get; }

        CancellationToken Token { get; }

        ITask Task { get; }

        IProcess Process { get; }

        Type TaskType { get; }

        Type ProcessType { get; }
    }
}
