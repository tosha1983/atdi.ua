using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Contracts.CoreServices.Identity;
using Atdi.Platform.AppComponent;
using Atdi.Platform.DependencyInjection;

namespace Atdi.CoreServices.AuthService.IcsmViisp
{
    public sealed class AppServerComponent : ComponentBase
    {
        public AppServerComponent()
            : base(
                  name: "IcsmViispAuthServiceCoreServices",
                  type: ComponentType.CoreServices,
                  behavior: ComponentBehavior.Simple | ComponentBehavior.SingleInstance)
        {
        }

        protected override void OnInstall()
        {
	        // конфигурация
	        var componentConfig = this.Config.Extract<AppServerComponentConfig>();
	        this.Container.RegisterInstance(componentConfig, ServiceLifetime.Singleton);

			this.Container.Register<AuthService, AuthService>(ServiceLifetime.PerThread);
        }

        protected override void OnActivate()
        {
	        var authServiceSite = this.Resolver.Resolve<IAuthServiceSite>();
	        authServiceSite.Registry(AuthService.Name, typeof(AuthService));
        }
    }
}
