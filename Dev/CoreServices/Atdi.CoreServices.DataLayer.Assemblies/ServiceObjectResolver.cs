using Atdi.Platform.DependencyInjection;
using Atdi.Platform.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.CoreServices.DataLayer.Assemblies
{
    class ServiceObjectResolver : LoggedObject
    {
        private readonly IServicesResolver _servicesResolver;

        public ServiceObjectResolver(IServicesResolver servicesResolver, ILogger logger) : base(logger)
        {
            this._servicesResolver = servicesResolver;
        }

        public IEnumerable Resolve(string name)
        {
            var type = Type.GetType(name);
            var result = (IEnumerable)_servicesResolver.Resolve(type);
            return result;
        }
    }
}
