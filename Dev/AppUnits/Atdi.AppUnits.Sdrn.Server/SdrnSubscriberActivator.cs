using Atdi.Contracts.Api.EventSystem;
using Atdi.Platform.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.AppUnits.Sdrn.Server
{
    public class SdrnSubscriberActivator : ISubscriberActivator
    {
        private readonly IServicesResolver _serviceResolver;

        public SdrnSubscriberActivator(IServicesResolver serviceResolver)
        {
            this._serviceResolver = serviceResolver;
        }

        public object CreateInstance(Type type)
        {
            return this._serviceResolver.Resolve(type);
        }
    }
}
