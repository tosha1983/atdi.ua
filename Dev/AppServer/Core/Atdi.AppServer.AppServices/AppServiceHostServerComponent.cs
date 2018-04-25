using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.Windsor;
using Castle.MicroKernel.Registration;
using Castle.Facilities.TypedFactory;
using Atdi.AppServer.Models.AppServices;
using Atdi.AppServer.Common;

namespace Atdi.AppServer.AppServices
{
    public class AppServiceHostServerComponent<TServiceModel, TServiceImpementBy> : IAppServerComponent
    {
        private readonly string _name;
        private ILogger _logger;

        public AppServiceHostServerComponent()
        {
            this._name = "AppServiceHostServerComponent." + typeof(TServiceModel).Name;
        }

        AppServerComponentType IAppServerComponent.Type => AppServerComponentType.AppService;

        string IAppServerComponent.Name => this._name;

        void IAppServerComponent.Activate()
        {
            ;
        }

        void IAppServerComponent.Deactivate()
        {
            ;
        }

        void IAppServerComponent.Install(IWindsorContainer container, IAppServerContext serverContext)
        {
            this._logger = container.Resolve<ILogger>();

            var services =
                    Types.FromAssembly(typeof(TServiceModel).Assembly)
                        .BasedOn(typeof(IAppService))
                        //.WithService.AllInterfaces()
                        .LifestyleSingleton();

            var operations =
                    Types.FromAssembly(typeof(TServiceModel).Assembly)
                        .BasedOn(typeof(IAppOperation))
                        // .WithService.AllInterfaces()
                        .LifestyleSingleton();

            var operationHandlers =
                    Types.FromAssembly(typeof(TServiceImpementBy).Assembly)
                        .BasedOn(typeof(IAppOperationHandler<,,,>))
                        .WithService.AllInterfaces()
                        .LifestyleSingleton();


            container.Register(
                    services,
                    operations,
                    operationHandlers,
                    Component.For<IAppOperationHandlerFactory>()
                        .AsFactory()
                        .LifeStyle.Singleton
                );

            var constraintParsers =
                Types.FromAssembly(typeof(TServiceImpementBy).Assembly)
                        .BasedOn(typeof(IConstraintParser<>))
                        .WithService.AllInterfaces()
                        .LifestyleSingleton();
        }

        void IAppServerComponent.Uninstall(IWindsorContainer container, IAppServerContext serverContext)
        {
            ;
        }
    }
}
