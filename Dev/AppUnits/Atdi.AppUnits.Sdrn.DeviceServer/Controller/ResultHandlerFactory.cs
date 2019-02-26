using Atdi.Contracts.Sdrn.DeviceServer;
using Atdi.Platform.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Controller
{
    class ResultHandlerFactory : IResultHandlerFactory
    {
        private readonly IServicesResolver _resolver;

        public ResultHandlerFactory(IServicesContainer servicesContainer)
        {
            this._resolver = servicesContainer.GetResolver<IServicesResolver>();
        }

        public object Create(Type handlerType)
        {
            return this._resolver.Resolve(handlerType);
        }
    }
}
