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

			this.Container.Register<CalcServerDataLayer>(ServiceLifetime.Singleton);
			this.Container.Register<InfocenterDataLayer>(ServiceLifetime.Singleton);

			this.Container.Register<ViewStarter>(ServiceLifetime.Singleton);
			this.Container.Register<PluginMenuCommands>(ServiceLifetime.Singleton);

			var typeResolver = this.Resolver.Resolve<ITypeResolver>();

            // регестрируем все DataAdapters
            this.Container.Register<ViewModels.EntityOrmTest.Adapters.ProjectTestDataAdapter>(ServiceLifetime.Singleton);
            this.Container.Register<ViewModels.Adapters.ProjectDataAdapter>(ServiceLifetime.Singleton);
            this.Container.Register<ViewModels.Adapters.ProjectMapDataAdapter>(ServiceLifetime.Singleton);

            //var dataAdaptersBaseType = typeof(ViewModels.EntityOrmTest.DataAdapter<,>);
            //var dataAdapterTypes = typeResolver.ForeachInAllAssemblies(
            //	(type) =>
            //	{
            //		if (!type.IsClass
            //		    || type.IsAbstract
            //		    || type.IsInterface
            //		    || type.IsEnum)
            //		{
            //			return false;
            //		}

            //		var baseType = type.BaseType;
            //		if (baseType == null || !baseType.IsGenericType)
            //		{
            //			return false;
            //		}

            //		baseType = baseType.GetGenericTypeDefinition();
            //		//this.Logger.Info("SdrnCalcServerClient", "AppComponent", $"BaseType = {baseType}");
            //		return dataAdaptersBaseType == baseType;
            //	}
            //).ToArray();
            //foreach (var dataAdapterType in dataAdapterTypes)
            //{
            //	this.Logger.Info("SdrnCalcServerClient", "AppComponent", $"DataAdapterType = {dataAdapterType}");
            //	this.Container.Register(dataAdapterType, ServiceLifetime.Transient);
            //}
            //this.Logger.Info("SdrnCalcServerClient", "AppComponent", $"dataAdapterTypes.Count = {dataAdapterTypes.Length}");

            // регестрируем все View

            var viewModelBaseType = typeof(WpfViewModelBase);
			var viewTypes = typeResolver.ResolveTypes(viewModelBaseType.Assembly, viewModelBaseType);
			foreach (var viewType in viewTypes)
			{
				this.Container.Register(viewType, ServiceLifetime.Transient);
				this.Logger.Info("SdrnCalcServerClient", "AppComponent", $"ViewType = {viewType}");
			}
		}

		protected override void OnActivate()
		{
			this.Logger.Info("SdrnCalcServerClient", "AppComponent", "OnActivate");
		}
	}
}
