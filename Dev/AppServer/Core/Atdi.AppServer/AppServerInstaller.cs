using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.Windsor;
using Castle.Facilities.Logging;
using Castle.Services.Logging.NLogIntegration;
using Castle.MicroKernel.Registration;
using Castle.Facilities.TypedFactory;
using Castle.MicroKernel.SubSystems.Configuration;

using Atdi.AppServer.Models.AppServices;

namespace Atdi.AppServer
{
    public class AppServerInstaller : IWindsorInstaller
    {
        private readonly AppServerHost _host;
        public AppServerInstaller(AppServerHost host)
        {
            this._host = host;
        }

        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.AddFacility<TypedFactoryFacility>();

            container.AddFacility<LoggingFacility>(
                    f => f
                    .LogUsing<NLogFactory>()
                    .WithConfig("Logger\\nlog.config")
                );

            container.Register(
                    Component.For<IWindsorContainer>()
                        .Instance(container)
                        .LifeStyle.Singleton
                );

            container.Register(
                    Component.For<ILogger>()
                        .ImplementedBy<Logger>()
                        .LifeStyle.Singleton
                );

            container.Register(
                    Component.For<AppServerHost>()
                        .Instance(this._host)
                        .LifestyleSingleton()
                );

            container.Register(
                    Component.For<IAppServerContext>()
                        .ImplementedBy<AppServerContext>()
                        .LifeStyle.Singleton
                );

            container.Register(
                    Component.For(typeof(IAppServiceInvoker<>))
                    .ImplementedBy(typeof(AppServiceInvoker<>))
                    .LifeStyle.Singleton
                );

            container.Register(
                    Component.For(typeof(IAppOperationInvoker<,,>))
                    .ImplementedBy(typeof(AppOperationInvoker<,,>))
                    .LifeStyle.Singleton
                );

            container.Register(
                    Component.For<IAppServiceInvokerFactory>()
                        .AsFactory()
                        .LifeStyle.Singleton
                );

            container.Register(
                    Component.For<IAppOperationInvokerFactory>()
                        .AsFactory()
                        .LifeStyle.Singleton
                );
        }
    }
}
