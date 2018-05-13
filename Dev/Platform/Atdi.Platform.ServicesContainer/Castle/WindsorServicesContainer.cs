using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Castle.Windsor;
using Castle.MicroKernel.Registration;
using Castle.Facilities.TypedFactory;
using Castle.Facilities.WcfIntegration;
using Atdi.Platform.DependencyInjection;

namespace Atdi.Platform.ServicesContainer.Castle
{
    public class WindsorServicesContainer : IServicesContainer
    {

        private IWindsorContainer _container;
        private bool _containerDisposed = false;

        public WindsorServicesContainer()
        {
            this._container = new WindsorContainer()
               .Install(new WindsorInstaller());

            this._container.Register(
                    Component
                    .For<IServicesContainer>()
                    .Instance(this)
                    .LifestyleSingleton()
                );

        }

        public void Dispose()
        {
            if (!this._containerDisposed)
            {
                if (this._container != null)
                {
                    this._container.Dispose();
                    this._container = null;
                }
                this._containerDisposed = true;
            }
        }

        public TResolver GetResolver<TResolver>() 
            where TResolver : IServicesResolver
        {
            return this._container.Resolve<TResolver>();
        }

        public void Register<TService>(ServiceLifetime lifetime = ServiceLifetime.Transient)
            where TService : class
        {
            var component = Component
                .For<TService>()
                .ImplementedBy<TService>()
                .SetLifestyle(lifetime);

            this._container.Register(component);
        }

        public void Register<TService, TImpl>(ServiceLifetime lifetime = ServiceLifetime.Transient)
            where TService : class
            where TImpl : TService
        {
            var component = Component
                .For<TService>()
                .ImplementedBy<TImpl>()
                .SetLifestyle(lifetime);

            this._container.Register(component);
        }

        public void Register<TService1, TService2, TImpl>(ServiceLifetime lifetime = ServiceLifetime.Transient)
            where TService1 : class
            where TService2 : class
            where TImpl : TService1, TService2
        {
            var component = Component
                .For<TService1, TService2>()
                .ImplementedBy<TImpl>()
                .SetLifestyle(lifetime);

            this._container.Register(component);
        }

        public void Register(Type serviceType, Type implementType, ServiceLifetime lifetime = ServiceLifetime.Transient)
        {
            var component = Component
                .For(serviceType)
                .ImplementedBy(implementType)
                .SetLifestyle(lifetime);

            this._container.Register(component);
        }

        public void RegisterAsFactory<TFactoryService>(ServiceLifetime lifetime = ServiceLifetime.Singleton) where TFactoryService : class
        {
            var component = Component
                .For<TFactoryService>()
                .AsFactory()
                .SetLifestyle(lifetime);

            this._container.Register(component);
        }

        public void RegisterAsFactory(Type factoryServiceType, ServiceLifetime lifetime = ServiceLifetime.Singleton)
        {
            var component = Component
                .For(factoryServiceType)
                .AsFactory()
                .SetLifestyle(lifetime);

            this._container.Register(component);
        }

        public void RegisterBoth<TService, TImpl>(ServiceLifetime lifetime = ServiceLifetime.Transient)
            where TService : class
            where TImpl : TService
        {
            var component = Component
                .For<TService, TImpl>()
                .ImplementedBy<TImpl>()
                .SetLifestyle(lifetime);

            this._container.Register(component);
        }

        public void RegisterClassesBasedOn(Assembly assembly, Type baseType, ServiceLifetime lifetime = ServiceLifetime.Transient)
        {
            var components = Classes
                .FromAssembly(assembly)
                .BasedOn(baseType)
                .SetLifestyle(lifetime);

            this._container.Register(components);
        }

        public void RegisterClassesBasedOn(Assembly assembly, Type baseType, BaseOnMode mode, ServiceLifetime lifetime = ServiceLifetime.Transient)
        {
            var components = Classes
                .FromAssembly(assembly)
                .BasedOn(baseType)
                .SetMode(mode)
                .SetLifestyle(lifetime);

            this._container.Register(components);
        }

        public void RegisterClassesBasedOn<TBase>(Assembly assembly, ServiceLifetime lifetime = ServiceLifetime.Transient)
        {
            var components = Classes
                .FromAssembly(assembly)
                .BasedOn<TBase>()
                .SetLifestyle(lifetime);

            this._container.Register(components);
        }

        public void RegisterClassesBasedOn<TBase>(Assembly assembly, BaseOnMode mode, ServiceLifetime lifetime = ServiceLifetime.Transient)
        {
            var components = Classes
                .FromAssembly(assembly)
                .BasedOn<TBase>()
                .SetMode(mode)
                .SetLifestyle(lifetime);

            this._container.Register(components);
        }

        public void RegisterInstance<TService>(TService instance, ServiceLifetime lifetime = ServiceLifetime.Transient) where TService : class
        {
            var component = Component
                .For<TService>()
                .Instance(instance)
                .SetLifestyle(lifetime);

            this._container.Register(component);
        }

        public void RegisterInstance(Type serviceType, object instance, ServiceLifetime lifetime = ServiceLifetime.Transient)
        {
            var component = Component
                .For(serviceType)
                .Instance(instance)
                .SetLifestyle(lifetime);

            this._container.Register(component);
        }

        public void RegisterServicesBasedOn(Assembly assembly, Type baseType, ServiceLifetime lifetime = ServiceLifetime.Transient)
        {
            var components = Types
                .FromAssembly(assembly)
                .BasedOn(baseType)
                .SetLifestyle(lifetime);

            this._container.Register(components);
        }

        public void RegisterServicesBasedOn(Assembly assembly, Type baseType, BaseOnMode mode, ServiceLifetime lifetime = ServiceLifetime.Transient)
        {
            var components = Types
                .FromAssembly(assembly)
                .BasedOn(baseType)
                .SetMode(mode)
                .SetLifestyle(lifetime);

            this._container.Register(components);
        }

        public void RegisterServicesBasedOn<TBase>(Assembly assembly, ServiceLifetime lifetime = ServiceLifetime.Transient)
        {
            var components = Types
                .FromAssembly(assembly)
                .BasedOn<TBase>()
                .SetLifestyle(lifetime);

            this._container.Register(components);
        }

        public void RegisterServicesBasedOn<TBase>(Assembly assembly, BaseOnMode mode, ServiceLifetime lifetime = ServiceLifetime.Transient)
        {
            var components = Types
                .FromAssembly(assembly)
                .BasedOn<TBase>()
                .SetMode(mode)
                .SetLifestyle(lifetime);

            this._container.Register(components);
        }
    }
}
