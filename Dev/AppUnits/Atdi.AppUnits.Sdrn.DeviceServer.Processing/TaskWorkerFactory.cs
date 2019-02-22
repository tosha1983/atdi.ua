using Atdi.Contracts.Sdrn.DeviceServer;
using Atdi.Platform.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Processing
{
    class TaskWorkerFactory : ITaskWorkerFactory
    {
        private readonly IServicesResolver _resolver;

        public TaskWorkerFactory(IServicesContainer servicesContainer)
        {
            this._resolver = servicesContainer.GetResolver<IServicesResolver>();
        }

        public object Create(Type workerInstanceType)
        {
            return this._resolver.Resolve(workerInstanceType);
        }
    }
}
