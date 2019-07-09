using Atdi.Platform.DependencyInjection;
using Atdi.Platform.AppComponent;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Adapters.RSTSMx.Test
{
    public class AppServerComponent : AppUnitComponent
    {

        public AppServerComponent()
            : base("SdrnDeviceServerRSTSMxAppUnitTest")
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
