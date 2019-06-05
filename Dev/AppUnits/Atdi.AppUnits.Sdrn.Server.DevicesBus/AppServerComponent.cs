using Atdi.Contracts.Sdrn.Server;
using Atdi.Platform.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Atdi.Platform.AppComponent;
using Atdi.Platform.Logging;
using Atdi.Contracts.Sdrn.Server.DevicesBus;

namespace Atdi.AppUnits.Sdrn.Server.DevicesBus
{
    public class AppServerComponent : AppUnitComponent
    {

        public AppServerComponent() 
            : base("SdrnServerDevicesBusAppUnit")
        {
            
        }

        protected override void OnInstallUnit()
        {
            var componentConfig = this.Config.Extract<AppServerComponentConfig>();
            this.Container.RegisterInstance(componentConfig, ServiceLifetime.Singleton);
            this.Container.Register<IMessagesSite, MessagesSite>(ServiceLifetime.PerThread);
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
