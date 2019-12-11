using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.Windsor;
using Castle.MicroKernel.Registration;
using Castle.Facilities.TypedFactory;
using Castle.Facilities.WcfIntegration;
using Castle.MicroKernel.SubSystems.Configuration;
using Atdi.Platform.DependencyInjection;
using Atdi.Platform.Workflows;

namespace Atdi.Platform.ServicesContainer.Castle
{
    class WindsorInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.AddFacility<TypedFactoryFacility>();
            container.AddFacility<WcfFacility>();

            container.Register(
                    Component.For<IWindsorContainer>()
                        .Instance(container)
                        .LifeStyle.Singleton
                );

            container.Register(
                    Component.For<IServicesResolver>()
                        .ImplementedBy<WindsorServicesResolver>()
                        .LifeStyle.Singleton
                );

            container.Register(
                    Component.For<IWcfServicesResolver>()
                        .ImplementedBy<WindsorWcfServicesResolver>()
                        .LifeStyle.Singleton
                );

            container.Register(
                    Component.For<IHandlerResolver>()
                        .ImplementedBy<WindsorHandlerResolver>()
                        .LifeStyle.Singleton
                );

            container.Register(
                    Component.For<IStatistics>()
                        .ImplementedBy<Statistics>()
                        .LifeStyle.Singleton
                );

            container.Register(
                    Component.For<Caching.IDataCacheSite>()
                        .ImplementedBy<Caching.DataCacheSite>()
                        .LifeStyle.Singleton
                );

            container.Register(
                    Component.For<IPipelineHandlerFactory>()
                        .ImplementedBy<PipelineHandlerFactory>()
                        .LifeStyle.Singleton
                );

            container.Register(
                    Component.For<IPipelineSite>()
                        .ImplementedBy<PipelineSite>()
                        .LifeStyle.Singleton
                );

            container.Register(
                Component.For<IJobExecutorResolver>()
                    .ImplementedBy<JobExecutorResolver>()
                    .LifeStyle.Singleton
            );

            container.Register(
                Component.For<IJobBroker>()
                    .ImplementedBy<JobBroker>()
                    .LifeStyle.Singleton
            );

            container.Register(
                Component.For<Data.IObjectPoolSite>()
                    .ImplementedBy<Data.ObjectPoolSite>()
                    .LifeStyle.Singleton
            );

        }
    }
}
