using Atdi.Contracts.Sdrn.DeviceServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Processing
{
    class TaskWorkerFactory : ITaskWorkerFactory
    {
        public ITaskWorker Create(Type workerInstanceType)
        {
            throw new NotImplementedException();
        }
    }
}
