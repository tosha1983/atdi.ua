using Atdi.Platform.DependencyInjection;
using Atdi.Platform.AppComponent;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Processing.Measurements
{
    public class AppServerComponent : AppUnitComponent
    {

        public AppServerComponent() 
            : base("SdrnDeviceServerProcessingMeasurementsAppUnit")
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
