using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Contracts.Sdrn.Infocenter;
using Atdi.Platform;
using Atdi.Platform.AppComponent;
using Atdi.Platform.AppServer;
using Atdi.Platform.DependencyInjection;
using Atdi.Platform.Logging;
using Atdi.Platform.Workflows;

namespace Atdi.AppUnits.Sdrn.Infocenter.Integration
{
	public class AppServerComponent : AppUnitComponent
	{
		public AppServerComponent()
			: base("SdrnInfocenterIntegrationAppUnit")
		{

		}

		protected override void OnInstallUnit()
		{
			// конфигурация
			var componentConfig = this.Config.Extract<AppServerComponentConfig>();
			this.Container.RegisterInstance(componentConfig, ServiceLifetime.Singleton);

			

			//this.Container.Register<SdrnAutoImportJob>(ServiceLifetime.Singleton);
		}

		protected override void OnActivateUnit()
		{
			//var typeResolver = this.Resolver.Resolve<ITypeResolver>();
			//var appConfig = this.Resolver.Resolve<AppServerComponentConfig>();

			//// подключаем обработчики событий
			//var eventDispatcher = this.Resolver.Resolve<IEventDispatcher>();
			//var eventSubscribeInterfaceType = typeof(IEventSubscriber<>);

			//var subscriberTypes = typeResolver.ForeachInAllAssemblies(
			//		(type) =>
			//		{
			//			if (!type.IsClass
			//			|| type.IsNotPublic
			//			|| type.IsAbstract
			//			|| type.IsInterface
			//			|| type.IsEnum)
			//			{
			//				return false;
			//			}

			//			var ft = type.GetInterface(eventSubscribeInterfaceType.FullName);
			//			return ft != null;
			//		}
			//	).ToArray();

			//foreach (var subscriberType in subscriberTypes)
			//{
			//	try
			//	{
			//		this.Container.Register(subscriberType, subscriberType, ServiceLifetime.PerThread);
			//		Logger.Verbouse(Contexts.ThisComponent, Categories.Registration, Events.HandlerTypeWasRegistered.With(subscriberType.AssemblyQualifiedName));
			//	}
			//	catch (Exception e)
			//	{
			//		Logger.Exception(Contexts.ThisComponent, Categories.Registration, e);
			//	}
			//}


			//var hostLoader = this.Resolver.Resolve<IServerHostLoader>();

			//hostLoader.RegisterTrigger("Running Event System Subscribers", () =>
			//{
			//	foreach (var subscriberType in subscriberTypes)
			//	{
			//		try
			//		{
			//			eventDispatcher.Subscribe(subscriberType);
			//			Logger.Verbouse(Contexts.ThisComponent, Categories.Subscribing, Events.HandlerTypeWasConnected.With(subscriberType.AssemblyQualifiedName));
			//		}
			//		catch (Exception e)
			//		{
			//			Logger.Exception(Contexts.ThisComponent, Categories.Registration, e);
			//		}
			//	}
			//});

			//var jobBroker = this.Resolver.Resolve<IJobBroker>();
			//hostLoader.RegisterTrigger("Running Jobs ...", () =>
			//{
			//	var startDelaySeconds = appConfig.AutoImportMapsStartDelay;
			//	if (!startDelaySeconds.HasValue)
			//	{
			//		startDelaySeconds = 0;
			//	}
			//	var repeatDelaySeconds = appConfig.AutoImportMapsRepeatDelay;
			//	if (!repeatDelaySeconds.HasValue)
			//	{
			//		repeatDelaySeconds = 0;
			//	}
			//	var jobDef = new JobDefinition<MapsAutoImportJob>()
			//	{
			//		Name = "Maps Auto Import Job",
			//		Recoverable = true,
			//		Repeatable = true,
			//		StartDelay = new TimeSpan(TimeSpan.TicksPerSecond * startDelaySeconds.Value),
			//		RepeatDelay = new TimeSpan(TimeSpan.TicksPerSecond * repeatDelaySeconds.Value)
			//	};

			//	jobBroker.Run(jobDef);
			//});
		}
	}
}
