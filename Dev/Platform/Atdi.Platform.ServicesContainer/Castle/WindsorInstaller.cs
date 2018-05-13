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
        }
    }
}
