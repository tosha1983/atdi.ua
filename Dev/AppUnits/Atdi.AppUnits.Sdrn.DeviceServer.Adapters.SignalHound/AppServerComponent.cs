using Atdi.Platform.DependencyInjection;

using Atdi.Platform.AppComponent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Adapters.SignalHound
{
    public class AppServerComponent : AppUnitComponent
    {

        public AppServerComponent() 
            : base("SdrnDeviceServerSignalHoundAdapterAppUnit")
        {
            
        }

        protected override void OnInstallUnit()
        {
            //var adapterConfig = this.Config.Extract<AdapterConfig>();
            //this.Container.RegisterInstance(adapterConfig, ServiceLifetime.Singleton);
            var adapterConfig = new AdapterConfig(this.Config, this.Logger);//old
            this.Container.RegisterInstance(adapterConfig, ServiceLifetime.Singleton);//old

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
