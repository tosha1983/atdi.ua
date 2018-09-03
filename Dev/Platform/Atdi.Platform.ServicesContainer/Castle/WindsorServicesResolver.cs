using Atdi.Platform.DependencyInjection;
using Castle.Windsor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Platform.ServicesContainer.Castle
{
    class WindsorServicesResolver : IServicesResolver
    {
        protected readonly IWindsorContainer _container;

        public WindsorServicesResolver(IWindsorContainer container)
        {
            this._container = container;
        }

        public void Release(object instance)
        {
            this._container.Release(instance);
        }


        public object Resolve(Type serviceType)
        {
            return this._container.Resolve(serviceType);
        }

        public T Resolve<T>()
        {
            return this._container.Resolve<T>();
        }
    }
}
