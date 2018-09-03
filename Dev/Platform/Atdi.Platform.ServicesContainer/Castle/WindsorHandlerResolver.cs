using Atdi.Platform.DependencyInjection;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Platform.ServicesContainer.Castle
{
    class WindsorHandlerResolver : WindsorServicesResolver, IHandlerResolver
    {

        public WindsorHandlerResolver(IWindsorContainer container) : base(container)
        {
        }

        public void RegisterHandler<THandler>(ServiceLifetime lifetime = ServiceLifetime.Transient) where THandler : class
        {
            var component = Component
                .For<THandler>()
                .ImplementedBy<THandler>()
                .SetLifestyle(lifetime);

            this._container.Register(component);
        }

        public void RegisterHandler(Type handlerType, ServiceLifetime lifetime = ServiceLifetime.Transient)
        {
            var component = Component
                .For(handlerType)
                .ImplementedBy(handlerType)
                .SetLifestyle(lifetime);

            this._container.Register(component);
        }

        public void ReleaseHandler<THandler>(THandler handler)
        {
            this._container.Release(handler);
        }

        public THandler ResolveHandler<THandler>()
        {
            return this._container.Resolve<THandler>();
        }
    }
}
