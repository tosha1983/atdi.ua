using Atdi.Api.EventSystem;
using Atdi.Contracts.Api.EventSystem;
using Atdi.Contracts.Sdrn.Server;
using Atdi.Platform.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            this.Container.RegisterInstance<ISdrnServerEnvironment>(environment, ServiceLifetime.Singleton);

            var eventSiteConfig = new EventSiteConfig();
            this.Container.RegisterInstance<IEventSiteConfig>(eventSiteConfig, ServiceLifetime.Singleton);
            this.Container.Register<IEventSite, EventSite>(ServiceLifetime.Singleton);
            this.Container.Register<IEventEmitter, EventEmitter>(ServiceLifetime.PerThread);
        }

        protected override void OnActivateUnit()
        {
        }

        protected override void OnDeactivateUnit()
        {
        }
        protected override void OnUninstallUnit()
        {
        }
    }
}
