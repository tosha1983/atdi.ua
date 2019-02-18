using Atdi.Platform.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Platform.AppComponent;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Processing.Example
{
    public class AppServerComponent : AppUnitComponent
    {

        public AppServerComponent() 
            : base("SdrnDeviceServerProcessingExampleAppUnit")
        {
            
        }

        protected override void OnInstallUnit()
        {
            var exampleConfig = this.Config.Extract<ExampleConfig>();
            this.Container.RegisterInstance(exampleConfig, ServiceLifetime.Singleton);

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
