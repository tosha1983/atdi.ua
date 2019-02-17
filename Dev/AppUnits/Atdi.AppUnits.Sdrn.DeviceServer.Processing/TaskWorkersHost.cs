using Atdi.Contracts.Sdrn.DeviceServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Processing
{
    class TaskWorkersHost : ITaskWorkersHost
    {
        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public ITaskWorker[] GetAutoTaskWorkers()
        {
            throw new NotImplementedException();
        }

        public ITaskWorker GetTaskWorker(Type taskType, Type processType)
        {
            throw new NotImplementedException();
        }

        public void Register(Type workerInstanceType)
        {
            throw new NotImplementedException();
        }
    }
}
