using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace Atdi.Platform.DependencyInjection
{
    public interface IServicesContainer : IDisposable
    {
        void Register<TService>(ServiceLifetime lifetime = ServiceLifetime.Transient)
            where TService : class;

        void Register<TService, TImpl>(ServiceLifetime lifetime = ServiceLifetime.Transient)
            where TService : class
            where TImpl : TService;

        void Register<TService1, TService2, TImpl>(ServiceLifetime lifetime = ServiceLifetime.Transient)
            where TService1 : class
            where TService2 : class
            where TImpl : TService1, TService2;

        void RegisterBoth<TService, TImpl>(ServiceLifetime lifetime = ServiceLifetime.Transient)
            where TService : class
            where TImpl : TService;

        void Register(Type serviceType, Type implementType, ServiceLifetime lifetime = ServiceLifetime.Transient);

        void RegisterInstance<TService>(TService instance, ServiceLifetime lifetime = ServiceLifetime.Transient)
            where TService : class;

        void RegisterInstance(Type serviceType, object instance, ServiceLifetime lifetime = ServiceLifetime.Transient);


        void RegisterAsFactory<TFactoryService>(ServiceLifetime lifetime = ServiceLifetime.Singleton)
            where TFactoryService : class;

        void RegisterAsFactory(Type factoryServiceType, ServiceLifetime lifetime = ServiceLifetime.Singleton);


        void RegisterServicesBasedOn(Assembly assembly, Type baseType, ServiceLifetime lifetime = ServiceLifetime.Transient);

        void RegisterServicesBasedOn(Assembly assembly, Type baseType, BaseOnMode mode, ServiceLifetime lifetime = ServiceLifetime.Transient);

        void RegisterServicesBasedOn<TBase>(Assembly assembly, ServiceLifetime lifetime = ServiceLifetime.Transient);

        void RegisterServicesBasedOn<TBase>(Assembly assembly, BaseOnMode mode, ServiceLifetime lifetime = ServiceLifetime.Transient);



        void RegisterClassesBasedOn(Assembly assembly, Type baseType, ServiceLifetime lifetime = ServiceLifetime.Transient);

        void RegisterClassesBasedOn(Assembly assembly, Type baseType, BaseOnMode mode, ServiceLifetime lifetime = ServiceLifetime.Transient);

        void RegisterClassesBasedOn<TBase>(Assembly assembly, ServiceLifetime lifetime = ServiceLifetime.Transient);

        void RegisterClassesBasedOn<TBase>(Assembly assembly, BaseOnMode mode, ServiceLifetime lifetime = ServiceLifetime.Transient);

        TResolver GetResolver<TResolver>() where TResolver : IServicesResolver;

    }
}
