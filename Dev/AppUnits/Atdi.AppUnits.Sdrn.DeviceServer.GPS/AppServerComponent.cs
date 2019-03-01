using Atdi.Platform.DependencyInjection;
using System;
using System.Collections.Generic;
using Atdi.Platform.AppComponent;
using Atdi.Contracts.Sdrn.DeviceServer.GPS;


namespace Atdi.AppUnits.Sdrn.DeviceServer.GPS
{
    public class AppServerComponent : AppUnitComponent
    {

        public AppServerComponent() 
            : base("SdrnDeviceServerGPS")
        {

            
        }

        protected override void OnInstallUnit()
        {
            var exampleConfig = this.Config.Extract<ConfigGPS>();
            this.Container.RegisterInstance(exampleConfig, ServiceLifetime.Singleton);
            exampleConfig.PortBaudRate = this.Config.GetParameterAsString("PortBaudRate");
            exampleConfig.PortDataBits = this.Config.GetParameterAsString("PortDataBits");
            exampleConfig.PortHandshake = this.Config.GetParameterAsString("PortHandshake");
            exampleConfig.PortName = this.Config.GetParameterAsString("PortName");
            exampleConfig.PortStopBits = this.Config.GetParameterAsString("PortStopBits");
            this.Container.Register<IGpsDevice, RunGPS>(ServiceLifetime.Singleton);
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
