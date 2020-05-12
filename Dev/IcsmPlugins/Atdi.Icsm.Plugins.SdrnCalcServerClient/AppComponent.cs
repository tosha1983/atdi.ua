using Atdi.Icsm.Plugins.SdrnCalcServerClient.Core;
using Atdi.Icsm.Plugins.SdrnCalcServerClient.Environment.Wpf;
using Atdi.Platform;
using Atdi.Platform.AppComponent;
using Atdi.Platform.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Icsm.Plugins.SdrnCalcServerClient
{
	public class AppComponent : ComponentBase
	{
		public AppComponent() 
			: base("SdrnCalcServerClient", ComponentType.IcsmPlugin, ComponentBehavior.SingleInstance)
		{

		}

		protected override void OnInstall()
		{
			var componentConfig = this.Config.Extract<AppComponentConfig>();
			this.Container.RegisterInstance(componentConfig, ServiceLifetime.Singleton);


			this.Container.Register<ViewStarter>(ServiceLifetime.Singleton);
			this.Container.Register<PluginMenuCommands>(ServiceLifetime.Singleton);

			// регестрируем все View
			var typeResolver = this.Resolver.Resolve<ITypeResolver>();
			var viewModelBaseType = typeof(WpfViewModelBase);
			var viewTypes = typeResolver.ResolveTypes(viewModelBaseType.Assembly, viewModelBaseType);
			foreach (var viewType in viewTypes)
			{
				this.Container.Register(viewType, ServiceLifetime.Transient);
			}
		}

		protected override void OnActivate()
		{
			this.Logger.Info("SdrnCalcServerClient", "AppComponent", "OnActivate");

			var config = this.Resolver.Resolve<AppComponentConfig>();
			this.Logger.Info("SdrnCalcServerClient", "AppComponent", $"Endpoint: BaseAddress='{config.OrmEndpointBaseAddress}'; Uri='{config.OrmEndpointApiUri}'");
		}
	}
}
