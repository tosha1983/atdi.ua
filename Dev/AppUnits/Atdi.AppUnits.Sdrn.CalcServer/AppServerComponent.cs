using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Api.EventSystem;
using Atdi.AppUnits.Sdrn.CalcServer.Services;
using Atdi.Contracts.Api.EventSystem;
using Atdi.Contracts.Sdrn.CalcServer;
using Atdi.Contracts.Sdrn.CalcServer.DeepServices;
using Atdi.Contracts.Sdrn.CalcServer.Internal;
using Atdi.Platform;
using Atdi.Platform.AppComponent;
using Atdi.Platform.AppServer;
using Atdi.Platform.DependencyInjection;
using Atdi.Platform.Logging;
using Atdi.Platform.Workflows;

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

			// среда сервера расчетов
			this.Container.Register<ITasksFactory, TasksFactory>(ServiceLifetime.Singleton);
			this.Container.Register<IIterationsPool, IterationsPool>(ServiceLifetime.Singleton);
			this.Container.Register<ITaskDispatcher, TaskDispatcher>(ServiceLifetime.Singleton);

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

			// шина данных 
			this.Container.Register<MapBuilder>(ServiceLifetime.Singleton);
			this.Container.Register<ProcessJob>(ServiceLifetime.Singleton);
			this.Container.Register<TaskWorkerJob>(ServiceLifetime.Singleton);
			this.Container.Register<IMapService, MapService>(ServiceLifetime.Singleton);
		}

		protected override void OnActivateUnit()
		{
			var typeResolver = this.Resolver.Resolve<ITypeResolver>();
			var mapBuilder = this.Resolver.Resolve<MapBuilder>();

			// подключаем обработчики задач
			var taskFactory = this.Resolver.Resolve<ITasksFactory>();
			var taskHandlerInterfaceType = typeof(ITaskHandler);
			var taskHandlerTypes = typeResolver.ForeachInAllAssemblies(
				(type) =>
				{
					//if (type.Name == "CoverageProfilesCalcTask")
					//{
					//	System.Diagnostics.Debug.WriteLine(type.Name);
					//}
					//else
					//{
					//	System.Diagnostics.Debug.WriteLine(type.Name);
					//}
					if (!type.IsClass
					    || type.IsAbstract
					    || type.IsInterface
					    || type.IsEnum)
					{
						return false;
					}

					var ft = type.GetInterface(taskHandlerInterfaceType.FullName);
					return ft != null;
				}
			).ToArray();

			foreach (var taskHandlerType in taskHandlerTypes)
			{
				try
				{
					var taskHandlerAttr = taskHandlerType.GetAttributeByType<TaskHandlerAttribute>();
					if (taskHandlerAttr != null)
					{
						taskFactory.Register(taskHandlerAttr.CalcType, taskHandlerType);
						this.Container.Register(taskHandlerType, taskHandlerType, ServiceLifetime.Transient);
						Logger.Verbouse(Contexts.ThisComponent, Categories.Registration, Events.TaskHandlerTypeWasRegistered.With(taskHandlerType.AssemblyQualifiedName));
					}
					else
					{
						throw new InvalidOperationException("Detected the task handler without attribute [TaskHandler]. It's ignored");
					}
				}
				catch (Exception e)
				{
					Logger.Exception(Contexts.ThisComponent, Categories.Registration, e);
				}
			}


			// подключаем обработчики итераций
			var iterationsPool = this.Resolver.Resolve<IIterationsPool>();
			var iterationHandlerInterfaceType = typeof(IIterationHandler<,>);
			var iterationHandlerTypes = typeResolver.ForeachInAllAssemblies(
				(type) =>
				{
					if (!type.IsClass
					    || type.IsAbstract
					    || type.IsInterface
					    || type.IsEnum)
					{
						return false;
					}

					var ft = type.GetInterface(iterationHandlerInterfaceType.FullName);
					return ft != null;
				}
			).ToArray();

			foreach (var iterationHandlerType in iterationHandlerTypes)
			{
				try
				{
					iterationsPool.Register(iterationHandlerType);
					this.Container.Register(iterationHandlerType, iterationHandlerType, ServiceLifetime.Singleton);
					Logger.Verbouse(Contexts.ThisComponent, Categories.Registration, Events.IterationHandlerTypeWasRegistered.With(iterationHandlerType.AssemblyQualifiedName));
					
				}
				catch (Exception e)
				{
					Logger.Exception(Contexts.ThisComponent, Categories.Registration, e);
				}
			}

			// подключаем низкоуровневые сервисы 
			var deepServiceInterfaceType = typeof(IDeepService);
			var deepServiceTypes = typeResolver.ForeachInAllAssemblies(
				(type) =>
				{
					if (!type.IsClass
					    || type.IsAbstract
					    || type.IsInterface
					    || type.IsEnum)
					{
						return false;
					}

					var ft = type.GetInterface(deepServiceInterfaceType.FullName);
					return ft != null;
				}
			).ToArray();

			foreach (var deepServiceImplType in deepServiceTypes)
			{
				try
				{
					var interfaces = deepServiceImplType.GetInterfaces();
					foreach (var deepServiceContractType in interfaces)
					{
						if (deepServiceContractType.GetInterface(iterationHandlerInterfaceType.FullName) != null)
						{
							this.Container.Register(deepServiceContractType, deepServiceImplType, ServiceLifetime.Singleton);
							Logger.Verbouse(Contexts.ThisComponent, Categories.Registration, Events.DeepServiceTypeWasRegistered.With(deepServiceImplType.AssemblyQualifiedName));
						}
					}
				}
				catch (Exception e)
				{
					Logger.Exception(Contexts.ThisComponent, Categories.Registration, e);
				}
			}

			// подключаем обработчики событий
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

			var appConfig = this.Resolver.Resolve<AppServerComponentConfig>();

			var jobBroker = this.Resolver.Resolve<IJobBroker>();
			hostLoader.RegisterTrigger("Running process jobs ...", () =>
			{
				var startDelaySeconds = appConfig.ProcessJobStartDelay;
				if (!startDelaySeconds.HasValue)
				{
					startDelaySeconds = 0;
				}
				var repeatDelaySeconds = appConfig.ProcessJobRepeatDelay;
				if (!repeatDelaySeconds.HasValue)
				{
					repeatDelaySeconds = 5000;
				}
				var jobDef = new JobDefinition<ProcessJob>()
				{
					Name = "Process Job",
					Recoverable = true,
					Repeatable = true,
					StartDelay = new TimeSpan(TimeSpan.TicksPerSecond * startDelaySeconds.Value),
					RepeatDelay = new TimeSpan(TimeSpan.TicksPerMillisecond * repeatDelaySeconds.Value)
				};

				jobBroker.Run(jobDef);
			});

		}
	}
}
