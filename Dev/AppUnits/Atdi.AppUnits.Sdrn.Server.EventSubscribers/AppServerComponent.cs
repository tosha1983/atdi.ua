using Atdi.Platform.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Atdi.Platform.AppComponent;



namespace Atdi.AppUnits.Sdrn.Server.EventSubscribers
{
    public class AppServerComponent : AppUnitComponent
    {

        public AppServerComponent() 
            : base("SdrnServerEventSubscribersAppUnit")
        {
            
        }

        protected override void OnInstallUnit()
        {
            var componentConfig = this.Config.Extract<AppServerComponentConfig>();
            this.Container.RegisterInstance(componentConfig, ServiceLifetime.Singleton);
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
