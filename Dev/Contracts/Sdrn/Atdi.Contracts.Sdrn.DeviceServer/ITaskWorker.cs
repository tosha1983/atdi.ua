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
        void Run(ITaskDescriptor descriptor);
    }

    public interface ITaskWorkerLifetime
    {

    }

    public sealed class SingletonTaskWorkerLifetime : ITaskWorkerLifetime { }
    public sealed class PerThreadTaskWorkerLifetime : ITaskWorkerLifetime { }
    public sealed class TransientTaskWorkerLifetime : ITaskWorkerLifetime { }


    public interface ITaskWorker<TTask, TProcess, TLifetime>
        where TTask : ITask
        where TProcess : IProcess
    {
        void Run(ITaskContext<TTask, TProcess> context);
    }

    public interface IAutoTaskWorker<TProcess, TLifetime> : ITaskWorker<AutoTask, TProcess, TLifetime>
        where TProcess : IProcess
    {
    }

    public interface ITaskWorkerAsync<TTask, TProcess, TLifetime>
        where TTask : ITask
        where TProcess : IProcess
    {
        Task RunAsync(ITaskContext<TTask, TProcess> context);
    }
}
