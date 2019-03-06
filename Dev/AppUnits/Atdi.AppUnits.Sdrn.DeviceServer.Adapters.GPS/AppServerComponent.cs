using Atdi.Platform.DependencyInjection;
using System;
using System.Collections.Generic;
using Atdi.Platform.AppComponent;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Adapters.GPS
{
    public class AppServerComponent : AppUnitComponent
    {

        public AppServerComponent() 
            : base("SdrnDeviceServerGPSAdapterAppUnit")
        {
            

        }

        protected override void OnInstallUnit()
        {
            var exampleConfig = this.Config.Extract<ConfigGPS>();
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
