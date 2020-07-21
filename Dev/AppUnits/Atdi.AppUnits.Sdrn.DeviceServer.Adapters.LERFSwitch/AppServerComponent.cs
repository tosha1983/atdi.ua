using Atdi.Platform.AppComponent;
using Atdi.Platform.DependencyInjection;
namespace Atdi.AppUnits.Sdrn.DeviceServer.Adapters.LERFSwitch
{
    public class AppServerComponent : AppUnitComponent
    {
        public AppServerComponent()
               : base("SdrnDeviceServerLERFSwitchAdapterAppUnit")
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
