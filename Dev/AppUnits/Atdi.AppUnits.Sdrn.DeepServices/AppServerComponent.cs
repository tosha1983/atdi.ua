using Atdi.Contracts.Sdrn.DeepServices;
using Atdi.Platform;
using Atdi.Platform.DependencyInjection;
using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Atdi.AppUnits.Sdrn.DeepServices
{
    public class AppServerComponent : AppUnitComponent
	{
		public AppServerComponent()
			: base("SdrnDeepServicesAppUnit")
		{

		}

		protected override void OnInstallUnit()
		{
			
		}

		protected override void OnActivateUnit()
		{
			base.OnActivateUnit();
			var typeResolver = this.Resolver.Resolve<ITypeResolver>();
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
						if (deepServiceContractType.GetInterface(deepServiceInterfaceType.FullName) != null)
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
		}
	}
}
