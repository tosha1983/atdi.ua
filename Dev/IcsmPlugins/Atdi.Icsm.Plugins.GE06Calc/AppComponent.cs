using Atdi.Platform;
using Atdi.Platform.AppComponent;
using Atdi.Platform.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Atdi.Platform.Cqrs;
using Atdi.Icsm.Plugins.Core;

namespace Atdi.Icsm.Plugins.GE06Calc
{
    public class AppComponent : ComponentBase
    {
        public AppComponent() : base("GE06Calc", ComponentType.IcsmPlugin, ComponentBehavior.SingleInstance)
        {

        }

        protected override void OnInstall()
        {
            var currentAssembly = typeof(AppComponentConfig).Assembly;

            var componentConfig = this.Config.Extract<AppComponentConfig>();
            componentConfig.VerifyLicense();

            this.Container.RegisterInstance(componentConfig, ServiceLifetime.Singleton);

            this.Container.Register<CalcServerDataLayer>(ServiceLifetime.Singleton);
            this.Container.Register<InfocenterDataLayer>(ServiceLifetime.Singleton);

            this.Container.Register<PluginMenuCommands>(ServiceLifetime.Singleton);

            var typeResolver = this.Resolver.Resolve<ITypeResolver>();

            // регестрируем все DataAdapters
            this.Container.RegisterClassesBasedOn(
                currentAssembly,
                typeof(EntityDataAdapter<,>),
                ServiceLifetime.Singleton
            );

            // регестрируем все View
            this.Container.RegisterClassesBasedOn(
                currentAssembly,
                typeof(ViewBase),
                ServiceLifetime.Transient
            );

            //var viewModelBaseType = typeof(ViewBase);
            //var viewTypes = typeResolver.ResolveTypes(viewModelBaseType.Assembly, viewModelBaseType);
            //foreach (var viewType in viewTypes)
            //{
            //	this.Container.Register(viewType, ServiceLifetime.Transient);
            //	this.Logger.Info("SdrnCalcServerClient", "AppComponent", $"ViewType = {viewType}");
            //}
        }

        protected override void OnActivate()
        {
            // подключаем обработчики комманд
            var commandDispatcher = this.Resolver.Resolve<ICommandDispatcher>();
            commandDispatcher.RegisterFrom(Assembly.GetAssembly(typeof(AppComponentConfig)));
            this.Logger.Info("GE06Calc", "AppComponent", "Command Handlers were registered");

            // подключаем ридеры
            var objectReader = this.Resolver.Resolve<IObjectReader>();
            objectReader.RegisterFrom(Assembly.GetAssembly(typeof(AppComponentConfig)));
            this.Logger.Info("GE06Calc", "AppComponent", "Read Query Executors were registered");
        }
    }
}
