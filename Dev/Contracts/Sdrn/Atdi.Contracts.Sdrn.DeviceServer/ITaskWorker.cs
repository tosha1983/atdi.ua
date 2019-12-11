using Atdi.DataModels.Sdrn.DeviceServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Atdi.Contracts.Sdrn.DeviceServer
{
    public interface ITaskWorker
    {
        bool IsAsync { get; }

        void Run(ITaskDescriptor descriptor);

        Task RunAsync(ITaskDescriptor descriptor);
    }


    public interface ITaskWorker<TTask, TProcess, TLifetime>
        where TTask : ITask
        where TProcess : IProcess
    {
        void Run(ITaskContext<TTask, TProcess> context);
    }
}
