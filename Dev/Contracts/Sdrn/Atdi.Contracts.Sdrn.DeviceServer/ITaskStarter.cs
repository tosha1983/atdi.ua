using Atdi.DataModels.Sdrn.DeviceServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Atdi.Contracts.Sdrn.DeviceServer
{
    public interface ITaskStarter
    {
        void Run(ITask task, IProcess process, ITaskContext parentTaskContext, CancellationToken cancellationToken);

        Task RunParallel(ITask task, IProcess process, ITaskContext parentTaskContext, CancellationToken cancellationToken);

        Task RunAsync(ITask task, IProcess process, ITaskContext parentTaskContext, CancellationToken cancellationToken);
    }
}
