﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Api.EventSystem;
using Atdi.Contracts.Api.EventSystem;
using Atdi.Contracts.Sdrn.CalcServer;
using Atdi.Platform;
using Atdi.Platform.AppComponent;
using Atdi.Platform.AppServer;
using Atdi.Platform.DependencyInjection;
using Atdi.Platform.Logging;

namespace Atdi.AppUnits.Sdrn.CalcServer
{
	public class AppServerComponent : AppUnitComponent
	{
		public AppServerComponent()
			: base("SdrnCalcServerAppUnit")
		{

		}

		protected override void OnInstallUnit()
		{
			// конфигурация
			var componentConfig = this.Config.Extract<AppServerComponentConfig>();
			this.Container.RegisterInstance(componentConfig, ServiceLifetime.Singleton);

			// лицензия
			var serverConfig = new CalcServerConfig(componentConfig);
			this.Container.RegisterInstance<ICalcServerConfig>(serverConfig, ServiceLifetime.Singleton);

			// система событий
			var eventSiteConfig = new EventSiteConfig();
			eventSiteConfig.SetValue(EventSiteConfig.ApiVersion, componentConfig.EventSystemApiVersion);
			eventSiteConfig.SetValue(EventSiteConfig.AppName, componentConfig.EventSystemAppName);
			eventSiteConfig.SetValue(EventSiteConfig.ErrorsQueueName, componentConfig.EventSystemErrorsQueueName);
			eventSiteConfig.SetValue(EventSiteConfig.LogQueueName, componentConfig.EventSystemLogQueueName);
			eventSiteConfig.SetValue(EventSiteConfig.EventBusHost, componentConfig.EventSystemEventBusHost);
			eventSiteConfig.SetValue(EventSiteConfig.EventBusPassword, componentConfig.EventSystemEventBusPassword);
			eventSiteConfig.SetValue(EventSiteConfig.EventBusPort, componentConfig.EventSystemEventBusPort);
			eventSiteConfig.SetValue(EventSiteConfig.EventBusUser, componentConfig.EventSystemEventBusUser);
			eventSiteConfig.SetValue(EventSiteConfig.EventBusVirtualHost, componentConfig.EventSystemEventBusVirtualHost);
			eventSiteConfig.SetValue(EventSiteConfig.EventExchange, componentConfig.EventSystemEventExchange);
			eventSiteConfig.SetValue(EventSiteConfig.EventQueueNamePart, componentConfig.EventSystemEventQueueNamePart);
			eventSiteConfig.SetValue(EventSiteConfig.UseCompression, componentConfig.EventSystemUseCompression);
			eventSiteConfig.SetValue(EventSiteConfig.UseEncryption, componentConfig.EventSystemUseEncryption);
			this.Container.RegisterInstance<IEventSiteConfig>(eventSiteConfig, ServiceLifetime.Singleton);

			this.Container.Register<IEventSystemObserver, EventSystemObserver>(ServiceLifetime.Singleton);
			this.Container.Register<ISubscriberActivator, EventSystemSubscriberActivator>(ServiceLifetime.Singleton);
			this.Container.Register<IEventSite, EventSite>(ServiceLifetime.Singleton);
			this.Container.Register<IEventEmitter, EventEmitter>(ServiceLifetime.PerThread);
			this.Container.Register<IEventDispatcher, EventDispatcher>(ServiceLifetime.Singleton);

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
				).ToArray();

			foreach (var subscriberType in subscriberTypes)
			{
				try
				{
					this.Container.Register(subscriberType, subscriberType, ServiceLifetime.PerThread);
					Logger.Verbouse(Contexts.ThisComponent, Categories.Registration, Events.HandlerTypeWasRegistered.With(subscriberType.AssemblyQualifiedName));
				}
				catch (Exception e)
				{
					Logger.Exception(Contexts.ThisComponent, Categories.Registration, e);
				}
			}


			var hostLoader = this.Resolver.Resolve<IServerHostLoader>();

			hostLoader.RegisterTrigger("Running Event System Subscribers", () =>
			{
				foreach (var subscriberType in subscriberTypes)
				{
					try
					{
						eventDispatcher.Subscribe(subscriberType);
						Logger.Verbouse(Contexts.ThisComponent, Categories.Subscribing, Events.HandlerTypeWasConnected.With(subscriberType.AssemblyQualifiedName));
					}
					catch (Exception e)
					{
						Logger.Exception(Contexts.ThisComponent, Categories.Registration, e);
					}
				}
			});
		}
	}
}