using Atdi.Platform.DependencyInjection;
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
            this.Container.RegisterInstance<MainProcess>(new MainProcess(), ServiceLifetime.Singleton);
            exampleConfig.DurationWaitingEventWithTask = this.Config.GetParameterAsInteger("DurationWaitingEventWithTask").Value;
            exampleConfig.MaxDurationBeforeStartTimeTask = this.Config.GetParameterAsInteger("MaxDurationBeforeStartTimeTask").Value;
            exampleConfig.DurationForSendResult = this.Config.GetParameterAsInteger("DurationForSendResult").Value;
            exampleConfig.MaxTimeOutReceiveSensorRegistrationResult = this.Config.GetParameterAsInteger("MaxTimeOutReceiveSensorRegistrationResult").Value;
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
