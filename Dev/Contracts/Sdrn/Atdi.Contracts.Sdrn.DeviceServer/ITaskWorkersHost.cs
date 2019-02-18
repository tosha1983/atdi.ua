using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Contracts.Sdrn.DeviceServer
{
    public interface ITaskWorkersHost : IDisposable
    {
        void Register(Type workerInstanceType);

        ITaskWorker GetTaskWorker(Type taskType, Type processType);

        ITaskWorker[] GetAutoTaskWorkers();
    }
}
