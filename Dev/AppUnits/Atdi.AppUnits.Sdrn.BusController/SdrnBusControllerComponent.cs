using Atdi.Contracts.Sdrn.Server;
using Atdi.Modules.Sdrn.AmqpBroker;
using Atdi.Modules.Sdrn.MessageBus;
using Atdi.Platform;
using Atdi.Platform.Logging;
using Atdi.Platform.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.AppUnits.Sdrn.BusController
{
    public class SdrnBusControllerComponent : AppUnitComponent
    {
        private SdrnBusControllerConfig _busControllerConfig;

        public SdrnBusControllerComponent() 
            : base("SdrnBusControllerAppUnit")
        {
            
        }

        protected override void OnInstallUnit()
        {
            var environment = this.Resolver.Resolve<ISdrnServerEnvironment>();
            this._busControllerConfig = new SdrnBusControllerConfig(this.Config, environment);
            this.Container.RegisterInstance(this._busControllerConfig, ServiceLifetime.Singleton);

            var convertorSettings = new MessageConvertSettings
            {
                UseEncryption = this._busControllerConfig.UseEncryption,
                UseCompression = this._busControllerConfig.UseCompression
            };
            var typeResolver = MessageObjectTypeResolver.CreateForApi20();
            var messageConvertor = new MessageConverter(convertorSettings, typeResolver);
            this.Container.RegisterInstance(messageConvertor, ServiceLifetime.Singleton);

            this.Container.Register<IBrokerObserver, SdrnBrokerObserver>(ServiceLifetime.Singleton);
            this.Container.Register<BusConnectionFactory>(ServiceLifetime.Singleton);
            this.Container.Register<ISdrnMessagePublisher, SdrnMessagePublisher>(ServiceLifetime.PerThread);
            this.Container.Register<ISdrnMessageDispatcher, SdrnMessageDispatcher>(ServiceLifetime.Singleton);

            //this.Container.Register<ConsumersRabbitMQConnection>(ServiceLifetime.PerThread);
            //this.Container.Register<PublisherRabbitMQConnection>(ServiceLifetime.PerThread);
            //this.Container.Register<ConsumersBusConnector>(ServiceLifetime.Transient);
            //this.Container.Register<PublisherBusConnector>(ServiceLifetime.PerThread);
        }

        protected override void OnActivateUnit()
        {
            var typeResolver = this.Resolver.Resolve<ITypeResolver>();
            var dispatcher = this.Resolver.Resolve<ISdrnMessageDispatcher>();

            //TODO: Search and connect all primary handlers
            var handlerTypes = typeResolver.ForeachInAllAssemblies(
                    (type) => 
                    {
                        if (!type.IsClass
                        ||  type.IsNotPublic
                        ||  type.IsAbstract
                        ||  type.IsInterface
                        ||  type.IsEnum)
                        {
                            return false;
                        }

                        var ft = type.GetInterface(SdrnHandlerLibrary.HandlerInterfaceFullName);
                        return ft != null;
                    }
                );

            foreach (var handlerType in handlerTypes)
            {
                try
                {
                    dispatcher.RegistryHandler(handlerType);
                    this.Container.Register(handlerType, handlerType, ServiceLifetime.PerThread);
                    Logger.Verbouse(Contexts.ThisComponent, Categories.Registration, Events.HandlerTypeWasRegistred.With(handlerType.AssemblyQualifiedName)); 
                }
                catch(Exception e)
                {
                    Logger.Exception(Contexts.ThisComponent, Categories.Registration, e);
                }
            }

            //TODO: Activate all consumers
            dispatcher.Activate();
        }

        protected override void OnDeactivateUnit()
        {

        }
        protected override void OnUninstallUnit()
        {

            this._busControllerConfig = null;
        }
    }
}
