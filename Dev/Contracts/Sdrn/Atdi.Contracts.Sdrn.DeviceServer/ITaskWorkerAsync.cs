using System.Threading.Tasks;
using Atdi.DataModels.Sdrn.DeviceServer;

namespace Atdi.Contracts.Sdrn.DeviceServer
{
    public interface ITaskWorkerAsync<TTask, TProcess, TLifetime>
        where TTask : ITask
        where TProcess : IProcess
    {
        Task RunAsync(ITaskContext<TTask, TProcess> context);
    }
}