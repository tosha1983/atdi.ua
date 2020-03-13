using Atdi.Platform.AppComponent;
using Atdi.Platform.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Adapters.RSFPL
{
    public class AppServerComponent : AppUnitComponent
    {

        public AppServerComponent()
            : base("SdrnDeviceServerRSFPLAdapterAppUnit")
        {

        }

        protected override void OnInstallUnit()
        {
            var adapterConfig = this.Config.Extract<AdapterConfig>();
            this.Container.RegisterInstance(adapterConfig, ServiceLifetime.Singleton);
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
