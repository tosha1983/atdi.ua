using Atdi.Contracts.Sdrn.DeviceServer;
using Atdi.Platform.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Controller
{
    class AdapterFactory : IAdapterFactory
    {
        private readonly IServicesResolver _resolver;

        public AdapterFactory(IServicesContainer servicesContainer)
        {
            this._resolver = servicesContainer.GetResolver<IServicesResolver>();
        }

        public IAdapter Create(Type adapterType)
        {
            return this._resolver.Resolve(adapterType) as IAdapter;
        }
    }
}
