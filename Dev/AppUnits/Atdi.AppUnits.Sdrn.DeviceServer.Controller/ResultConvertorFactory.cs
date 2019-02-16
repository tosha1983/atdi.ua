using Atdi.Contracts.Sdrn.DeviceServer;
using Atdi.Platform.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Controller
{
    class ResultConvertorFactory : IResultConvertorFactory
    {
        private readonly IServicesResolver _resolver;

        public ResultConvertorFactory(IServicesContainer servicesContainer)
        {
            this._resolver = servicesContainer.GetResolver<IServicesResolver>();
        }

        public IResultConvertor Create(Type convertorType)
        {
            return this._resolver.Resolve(convertorType) as IResultConvertor;
        }
    }
}
