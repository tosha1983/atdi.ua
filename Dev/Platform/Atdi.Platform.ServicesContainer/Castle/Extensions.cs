using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.MicroKernel.Registration;
using Castle.Facilities.WcfIntegration;
using Atdi.Platform.DependencyInjection;

namespace Atdi.Platform.ServicesContainer.Castle
{
    static class Extensions
    {
        public static ComponentRegistration<TService> SetLifestyle<TService>(this ComponentRegistration<TService> component, ServiceLifetime lifeTime)
            where TService : class
        {
            switch (lifeTime)
            {
                case ServiceLifetime.Transient:
                    return component.LifestyleTransient();
                case ServiceLifetime.Singleton:
                    return component.LifestyleSingleton();
                case ServiceLifetime.PerThread:
                    return component.LifestylePerThread();
                case ServiceLifetime.PerWebRequest:
                    return component.LifestylePerWebRequest();
                case ServiceLifetime.Pooled:
                    return component.LifestylePooled();
                case ServiceLifetime.PerWcfOperation:
                    return component.LifestylePerWcfOperation();
                case ServiceLifetime.PerWcfSession:
                    return component.LifestylePerWcfSession();
                default:
                    return component;
            }
        }

        public static BasedOnDescriptor SetLifestyle(this BasedOnDescriptor descriptor, ServiceLifetime lifeTime)
        {
            switch (lifeTime)
            {
                case ServiceLifetime.Transient:
                    return descriptor.LifestyleTransient();
                case ServiceLifetime.Singleton:
                    return descriptor.LifestyleSingleton();
                case ServiceLifetime.PerThread:
                    return descriptor.LifestylePerThread();
                case ServiceLifetime.PerWebRequest:
                    return descriptor.LifestylePerWebRequest();
                case ServiceLifetime.Pooled:
                    return descriptor.LifestylePooled();
                case ServiceLifetime.PerWcfOperation:
                    return descriptor.LifestylePerWcfOperation();
                case ServiceLifetime.PerWcfSession:
                    return descriptor.LifestylePerWcfSession();
                default:
                    return descriptor;
            }
        }

        public static BasedOnDescriptor SetMode(this BasedOnDescriptor descriptor, BaseOnMode mode)
        {
            switch (mode)
            {
                case BaseOnMode.AllInterface:
                    return descriptor.WithServiceAllInterfaces();
                case BaseOnMode.Base:
                    return descriptor.WithServiceBase();
                case BaseOnMode.DefaultInterfaces:
                    return descriptor.WithServiceDefaultInterfaces();
                case BaseOnMode.FirstInterface:
                    return descriptor.WithServiceFirstInterface();
                case BaseOnMode.Self:
                    return descriptor.WithServiceSelf();
                default:
                    return descriptor;
            }
        }
    }
}
