using Atdi.Api.EventSystem;
using Atdi.Contracts.Api.EventSystem;
using Atdi.Contracts.Sdrn.Server;
using Atdi.Platform.DependencyInjection;
using Atdi.Platform.AppComponent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Platform;
using Atdi.Platform.Logging;
using Atdi.Platform.Workflows;
using Atdi.DataModels.Sdrns.Server;

namespace Atdi.AppUnits.Sdrn.Server
{
    public class SdrnServerComponent : AppUnitComponent
    {

        public SdrnServerComponent() 
            : base("SdrnServerAppUnit")
        {
            
        }

        protected override void OnInstallUnit()
        {
            var environment = new SdrnServerEnvironment(this.Config);
            this.Container.RegisterInstance<ISdrnServerEnvironment, ISdrnServerEnvironmentModifier>(environment, ServiceLifetime.Singleton);

            var eventSiteConfig = new EventSiteConfig();
            SetValueToEventSiteConfig(eventSiteConfig, EventSiteConfig.ApiVersion);
            SetValueToEventSiteConfig(eventSiteConfig, EventSiteConfig.AppName);
            SetValueToEventSiteConfig(eventSiteConfig, EventSiteConfig.EventBusHost);
            SetValueToEventSiteConfig(eventSiteConfig, EventSiteConfig.EventBusVirtualHost);
            SetValueToEventSiteConfig<int?>(eventSiteConfig, EventSiteConfig.EventBusPort);
            SetValueToEventSiteConfig(eventSiteConfig, EventSiteConfig.EventBusUser);
            SetValueToEventSiteConfigDecode(eventSiteConfig, EventSiteConfig.EventBusPassword);
            SetValueToEventSiteConfig(eventSiteConfig, EventSiteConfig.EventExchange);
            SetValueToEventSiteConfig(eventSiteConfig, EventSiteConfig.EventQueueNamePart);
            SetValueToEventSiteConfig(eventSiteConfig, EventSiteConfig.ErrorsQueueName);
            SetValueToEventSiteConfig(eventSiteConfig, EventSiteConfig.LogQueueName);
            SetValueToEventSiteConfig<bool>(eventSiteConfig, EventSiteConfig.UseEncryption);
            SetValueToEventSiteConfig<bool>(eventSiteConfig, EventSiteConfig.UseCompression);
            this.Container.RegisterInstance<IEventSiteConfig>(eventSiteConfig, ServiceLifetime.Singleton);

            this.Container.Register <IEventSystemObserver, SdrnEventSystemObserver>(ServiceLifetime.Singleton);
            this.Container.Register<ISubscriberActivator, SdrnSubscriberActivator>(ServiceLifetime.Singleton);
            this.Container.Register<IEventSite, EventSite>(ServiceLifetime.Singleton);
            this.Container.Register<IEventEmitter, EventEmitter>(ServiceLifetime.PerThread);
            this.Container.Register<IEventDispatcher, EventDispatcher>(ServiceLifetime.Singleton);
        }

        private void SetValueToEventSiteConfig(EventSiteConfig eventSiteConfig, string name)
        {
            eventSiteConfig.SetValue(name, this.Config.GetParameterAsString($"EventSystem.{name}"));
        }
        private void SetValueToEventSiteConfigDecode(EventSiteConfig eventSiteConfig, string name)
        {
            eventSiteConfig.SetValue(name, this.Config.GetParameterAsDecodeString($"EventSystem.{name}", "Atdi.AppServer.AppService.SdrnsController"));
        }

        private void SetValueToEventSiteConfig<T>(EventSiteConfig eventSiteConfig, string name)
        {
            if (typeof(T) == typeof(int?))
            {
                eventSiteConfig.SetValue(name, this.Config.GetParameterAsInteger($"EventSystem.{name}"));
            }
            else if (typeof(T) == typeof(bool))
            {
                eventSiteConfig.SetValue(name, this.Config.GetParameterAsBoolean($"EventSystem.{name}"));
            }
            else
            {
                eventSiteConfig.SetValue(name, this.Config.GetParameterAsString($"EventSystem.{name}"));
            }
            
        }

        protected override void OnActivateUnit()
        {
            var typeResolver = this.Resolver.Resolve<ITypeResolver>();

            // 
            

            var eventDispatcher = this.Resolver.Resolve<IEventDispatcher>();
            var eventSubscribeInterfaceType = typeof(IEventSubscriber<>);

            var subscriberTypes = typeResolver.ForeachInAllAssemblies(
                    (type) =>
                    {
                        if (!type.IsClass
                        || type.IsNotPublic
                        || type.IsAbstract
                        || type.IsInterface
                        || type.IsEnum)
                        {
                            return false;
                        }

                        var ft = type.GetInterface(eventSubscribeInterfaceType.FullName);
                        return ft != null;
                    }
                );

            foreach (var subscriberType in subscriberTypes)
            {
                try
                {
                    this.Container.Register(subscriberType, subscriberType, ServiceLifetime.PerThread);
                    eventDispatcher.Subscribe(subscriberType);
                    Logger.Verbouse(Contexts.ThisComponent, Categories.Registration, Events.HandlerTypeWasRegistred.With(subscriberType.AssemblyQualifiedName));
                }
                catch (Exception e)
                {
                    Logger.Exception(Contexts.ThisComponent, Categories.Registration, e);
                }
            }
        }

        protected override void OnDeactivateUnit()
        {
        }
        protected override void OnUninstallUnit()
        {
        }
    }
}
