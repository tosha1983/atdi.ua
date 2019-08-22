using Atdi.Platform.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Platform.Workflows
{
    public class PipelineHandlerFactory : IPipelineHandlerFactory
    {
        private readonly IServicesResolver _servicesResolver;

        public PipelineHandlerFactory(IServicesResolver servicesResolver)
        {
            this._servicesResolver = servicesResolver;
        }

        public IPipelineHandler<TData, TResult> Create<TData, TResult>(Type handlerType)
        {
            var instance = _servicesResolver.Resolve(handlerType) as IPipelineHandler<TData, TResult>;
            if (instance == null)
            {
                throw new InvalidOperationException($"Cannot create handler instance by type '{handlerType}' and with Data Type '{typeof(TData)}' and with Result Type '{typeof(TResult)}'");
            }
            return instance;

        }
    }
}
