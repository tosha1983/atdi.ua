using Atdi.Contracts.Api.DataBus;
using Atdi.Platform.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.AppUnits.Sdrn.AggregationServer
{
    internal sealed class DataBusHandlerResolver : IMessageHandlerResolver
    {
        private readonly IServicesResolver _servicesResolver;

        public DataBusHandlerResolver(IServicesResolver servicesResolver)
        {
            this._servicesResolver = servicesResolver;
        }

        public IMessageHandler Resolve(Type type)
        {
            return this._servicesResolver.Resolve(type) as IMessageHandler;
        }
    }
}
