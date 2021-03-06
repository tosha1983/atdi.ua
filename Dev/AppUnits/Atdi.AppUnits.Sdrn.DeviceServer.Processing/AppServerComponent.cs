﻿using Atdi.Platform.DependencyInjection;
using Atdi.DataModels.Sdrn.DeviceServer.Processing;
using Atdi.Platform.AppComponent;


namespace Atdi.AppUnits.Sdrn.DeviceServer.Processing
{
    public class AppServerComponent : AppUnitComponent
    {

        public AppServerComponent() 
            : base("SdrnDeviceServerProcessingAppUnit")
        {
        }

        protected override void OnInstallUnit()
        {
            var exampleConfig = this.Config.Extract<ConfigProcessing>();
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
